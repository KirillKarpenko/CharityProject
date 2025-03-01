using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace PresentationLayer;
public static class Operations
{
    // п.1 - Знайти донорів, які пожертвували в 3 і більше різні фонди, причому 
    // загальна сума перевищує 10000 грн.
    public static IList<Donor> GetHighValueDonor(CharityDbContext context)
    {
        IQueryable<Donor> allDonors = context.Donors.Include(d => d.Donations);
        var selectedDonors = allDonors
            .Where(d => d.Donations
                .Select(don => don.OrganizationId).Distinct().Count() >= 3
                    && d.Donations.Sum(don => don.Amount) > 10000)
            .ToList();

        return selectedDonors;
    }

    // п.2 - Визначити фонди, які отримали найбільше коштів за останній 
    // квартал, але витратили менше 50%.
    public static IList<Organization> GetRecentTopOrganizations(CharityDbContext context)
    {
        var dateThreshold = DateTime.UtcNow.AddMonths(-3);
        IQueryable<Organization> allOrganizations = context.Organizations.Include(o => o.Fundings);
        var recentTopOrganizations = allOrganizations
            .Where(o => o.Donations.Where(d => d.TimeOfOperation >= dateThreshold).Sum(d => d.Amount) > 5000
                && o.Fundings.Sum(e => e.Amount) < o.Donations.Sum(d => d.Amount) * 0.5m)
            .ToList();

        return recentTopOrganizations;
    }

    // п.3 - Визначити донорів, які зробили внески у більше ніж 5 фондів, але не 
    // більше одного разу в кожен фонд.
    public static IList<Donor> GetDonorsWhoDonatedOnce(CharityDbContext context, int organisationsQuantity)
    {
        IQueryable<Donor> allDonors = context.Donors.Include(d => d.Donations);
        var selectedDonors = allDonors
        .Where(d => d.Donations.GroupBy(don => don.OrganizationId).Count() > organisationsQuantity
            && d.Donations.GroupBy(don => don.OrganizationId).All(g => g.Count() == 1))
        .ToList();

        return selectedDonors;
    }

    // п.4 - Знайти донорів, які фінансували принаймні три різні організації, де 
    // кожна організація реалізувала хоча б два проєкти, і визначити
    // середній відсоток витрат на адміністративні потреби у цих проєктах
    public static IList<Tuple<Donor, decimal>> GetDonorsFundsAdminExpenses(CharityDbContext context)
    {
        var allDonors = context.Donors
            .Include(d => d.Donations)
            .ThenInclude(don => don.Organization)
            .ThenInclude(o => o.Fundings)
            .ThenInclude(f => f.Project)
            .ThenInclude(p => p.Report);

        var selectedDonors = allDonors
        .Where(d => d.Donations
            .Select(don => don.Organization)
            .Where(org => org.Fundings.Count >= 2).Distinct().Count() >= 3)
        .ToList();

        IList<Tuple<Donor, decimal>> result = [];
        IList<decimal> adminSpendingPercents = [];
        foreach (var donor in selectedDonors)
        {
            var reports = donor.Donations.SelectMany(d => d.Organization.Fundings.Select(f => f.Project.Report)); //SelectMany агрегує
            foreach (var report in reports)
            {
                var adminSpending = reports.Sum(r => r.AdministrativeSpending);
                var labourSpending = reports.Sum(r => r.LabourSpending);
                var materialsSpending = reports.Sum(r => r.MaterialsSpending);
                var totalSpending = adminSpending + labourSpending + materialsSpending;
                var adminSpendingPercent = adminSpending / totalSpending * 100;

                adminSpendingPercents.Add(adminSpendingPercent);
            }

            Tuple<Donor, decimal> resultTuple = Tuple.Create(donor, adminSpendingPercents.Average());

            result.Add(resultTuple);
        }

        return result;
    }

    // З ціллю демонстрації того, що ці функції існують
    public static void WriteTopDonorsToConsole(CharityDbContext context, int amount)
    {
        var donors = context.Donations
            .GroupBy(d => d.DonorId)
            .Select(g => new
            {
                DonorId = g.Key,
                TotalDonated = g.Sum(d => d.Amount)
            })
            .OrderByDescending(d => d.TotalDonated)
            .Take(amount)
            .Join(context.Donors, d => d.DonorId, donor => donor.Id, (d, donor) => new
            {
                donor.Name,
                d.TotalDonated
            })
            .ToList();

        Console.WriteLine($"\nTop-{amount} donors:");
        foreach (var donor in donors)
        {
            Console.WriteLine($"Name: {donor.Name}, Total donated: {donor.TotalDonated}");
        }
    }

    // 3 методи з цукром для такої ж демонстрації
    // Виводить статистику по донатам до кожного фонду
    public static void WriteDonationStatsToConsole(CharityDbContext context)
    {
        var orgDonationStats =
            (from donation in context.Donations
             group donation by donation.OrganizationId into fundGroup
             where fundGroup.Count() >= 5
             select new
             {
                 OrganizationId = fundGroup.Key,
                 TotalDonations = fundGroup.Sum(d => d.Amount),
                 MaxDonation = fundGroup.Max(d => d.Amount)
             })
             .ToList();

        Console.WriteLine("\nDonation stats for each fund that got more than 5 donations:");
        foreach (var entry in orgDonationStats)
        {
            Console.WriteLine($"Fund id: {entry.OrganizationId}, Total donations: {entry.TotalDonations}, Biggest donation: {entry.MaxDonation}");
        }
    }

    // Об'єднує донаторів і фонди
    public static void WriteDonorsAndOrganizationsToConsole(CharityDbContext context)
    {
        var donorFunds =
            (from donation in context.Donations
             join donor in context.Donors on donation.DonorId equals donor.Id
             join organization in context.Organizations on donation.OrganizationId equals organization.Id
             select new
             {
                 DonorName = donor.Name,
                 OrganizationName = organization.Name,
                 Amount = donation.Amount
             }).ToList();
        Console.WriteLine("\nDonors and funds they`ve donated to:");
        foreach (var entry in donorFunds)
        {
            Console.WriteLine($"Donor name: {entry.DonorName}, Fund name: {entry.OrganizationName}, Amount donated: {entry.Amount}");
        }
    }

    // Виводить всіх донаторів за останній місяць
    public static IList<Donor> GetLastMonthActivity(CharityDbContext context)
    {
        var donors =
            (from donor in context.Donors
             where donor.Donations.Any(don => don.TimeOfOperation <= DateTime.Now.AddMonths(-1))
             orderby donor.Name
             select donor).ToList();

        return donors;
    }

    // Виводить всі фонди, яким задонатили більше за amount
    public static IList<Organization> GetOrganizationsWithTotalDonationsAbove(CharityDbContext context, decimal amount)
    {
        var organizations = context.Donations
            .GroupBy(d => d.OrganizationId)
            .Where(g => g.Sum(d => d.Amount) > amount)
            .Select(g => g.First().Organization)
            .ToList();

        return organizations;
    }

    // Виводить середній розмір донату даній організації
    public static decimal AverageDonationForOrganization(CharityDbContext context, string id)
    {
        var allDonations = context.Donations;
        var avgDonationPerOrg = allDonations
            .Where(d => d.OrganizationId == id)
            .Select(d => d.Amount)
            .Average();

        return avgDonationPerOrg;
    }
}
