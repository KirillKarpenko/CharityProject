using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using PresentationLayer.Dtos;

namespace PresentationLayer;
public static class Operations
{
    // Знайти донорів, які пожертвували в 3 і більше різні фонди, причому 
    // загальна сума перевищує 10000 грн.
    public static List<Donor> GetHighValueDonor(CharityDbContext context)
    {
        IQueryable<Donor> allDonors = context.Donors.Include(d => d.Donations);
        var selectedDonors = allDonors
            .Where(d => d.Donations
                .Select(don => don.OrganizationId).Distinct().Count() >= 3
                    && d.Donations.Sum(don => don.Amount) > 10000)
            .ToList();

        return selectedDonors;
    }

    // Визначити фонди, які отримали найбільше коштів за останній 
    // квартал, але витратили менше 50%.
    public static List<Organization> GetRecentTopOrganizations(CharityDbContext context)
    {
        var dateThreshold = DateTime.UtcNow.AddMonths(-3);
        IQueryable<Organization> allOrganizations = context.Organizations.Include(o => o.Fundings);
        var recentTopOrganizations = allOrganizations
            .OrderByDescending(o => o.Donations.Sum(d => d.Amount))
            .Where(o => o.Donations.Where(d => d.TimeOfOperation >= dateThreshold).Sum(d => d.Amount) > 5000 
                && o.Fundings.Sum(e => e.Amount) < o.Donations.Sum(d => d.Amount) * 0.5m)
            .ToList();

        return recentTopOrganizations;
    }

    // Визначити донорів, які зробили внески у більше ніж 5 фондів, але не 
    // більше одного разу в кожен фонд.
    public static List<Donor> GetDonorsWhoDonatedOnce(CharityDbContext context, int organisationsQuantity)
    {
        IQueryable<Donor> allDonors = context.Donors.Include(d => d.Donations);
        var selectedDonors = allDonors
        .Where(d => d.Donations.GroupBy(don => don.OrganizationId).Count() > organisationsQuantity
            && d.Donations.GroupBy(don => don.OrganizationId).All(g => g.Count() == 1))
        .ToList();

        return selectedDonors;
    }

    // Знайти донорів, які фінансували принаймні три різні організації, де 
    // кожна організація реалізувала хоча б два проєкти, і визначити
    // середній відсоток витрат на адміністративні потреби у цих проєктах
    public static List<DonorAdminExpenses> GetDonorsFundsAdminExpenses(CharityDbContext context)
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

        List<DonorAdminExpenses> result = [];
        List<decimal> adminSpendingPercents = [];
        foreach (var donor in selectedDonors)
        {
            var reports = donor.Donations.SelectMany(d => d.Organization.Fundings.Select(f => f.Project.Report));
            foreach (var report in reports)
            {
                var adminSpending = reports.Sum(r => r.AdministrativeSpending);
                var labourSpending = reports.Sum(r => r.LabourSpending);
                var materialsSpending = reports.Sum(r => r.MaterialsSpending);
                var totalSpending = adminSpending + labourSpending + materialsSpending;
                var adminSpendingPercent = adminSpending / totalSpending * 100;

                adminSpendingPercents.Add(adminSpendingPercent);
            }

            var donorAdminExpenses = new DonorAdminExpenses 
            { 
                DonorId = donor.Id,
                DonorName = donor.Name,
                Amount = adminSpendingPercents.Average()
            };

            result.Add(donorAdminExpenses);
        }

        return result;
    }

    // З ціллю демонстрації того, що ці функції існують
    public static List<DonorDonations> GetTopDonorsTotalDonations(CharityDbContext context)
    {
        var grouped = context.Donations
            .Include(d => d.Donor)
            .AsEnumerable()
            .GroupBy(d => d.DonorId)
            .Select(g => new
            {
                Donor = g.First().Donor,
                Total = g.Sum(d => d.Amount)
            });

        var maxTotal = grouped.Max(x => x.Total);

        return grouped
            .Where(x => x.Total == maxTotal)
            .Select(x => new DonorDonations
            {
                Name = x.Donor.Name,
                Donated = x.Total
            })
            .ToList();
    }

    // 3 методи з цукром для такої ж демонстрації
    // Виводить статистику по донатам до кожного фонду
    public static List<OrganizationStats> GetDonationStats(CharityDbContext context)
    {
        return (from donation in context.Donations
             group donation by donation.OrganizationId into fundGroup
             where fundGroup.Count() >= 5
             select new OrganizationStats
             {
                 OrganizationId = fundGroup.Key,
                 TotalDonations = fundGroup.Sum(d => d.Amount),
                 MaxDonation = fundGroup.Max(d => d.Amount)
             })
             .ToList();
    }

    // Об'єднує донаторів і фонди
    public static List<DonorAndOrganizations> GetDonorsAndOrganizations(CharityDbContext context)
    {
        return (from donation in context.Donations
             join donor in context.Donors on donation.DonorId equals donor.Id
             join organization in context.Organizations on donation.OrganizationId equals organization.Id
             select new DonorAndOrganizations
             {
                 DonorName = donor.Name,
                 OrganizationName = organization.Name,
                 Amount = donation.Amount
             }).ToList();
    }

    // Виводить всіх донаторів за останній місяць
    public static List<Donor> GetLastMonthActivity(CharityDbContext context)
    {
        var donors =
            (from donor in context.Donors
             where donor.Donations.Any(don => don.TimeOfOperation <= DateTime.Now.AddMonths(-1))
             orderby donor.Name
             select donor).ToList();

        return donors;
    }

    // Виводить всі фонди, яким задонатили більше за amount
    public static List<Organization> GetOrganizationsWithTotalDonationsAbove(CharityDbContext context, decimal amount)
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

    // п.1 - Знайти донорів, які жертвували найбільшу кількість коштів на 
    // проєкти, і для кожного донора вивести середній розмір жертви.
    public static List<DonorDonations> GetTopDonorsAverageDonations(CharityDbContext context)
    {
        var grouped = context.Donations
            .GroupBy(d => d.Donor)
            .Select(g => new
            {
                Donor = g.Key,
                Total = g.Sum(d => d.Amount),
                Avg = g.Average(d => d.Amount)
            })
            .ToList();

        var maxTotal = grouped.Max(x => x.Total);

        return grouped
            .Where(x => x.Total == maxTotal)
            .Select(x => new DonorDonations
            {
                Name = x.Donor.Name,
                Donated = x.Avg
            })
            .ToList();
    }

    // п.2 -  Визначити організації, які отримали найбільше фінансування від спонсорів.
    public static List<OrganizationStats> GetTopFundedOrganizations(CharityDbContext context)
    {
        var allOrgs = context.Organizations
            .Select(o => new
            {
                o.Id,
                Total = o.Fundings.Sum(f => f.Amount)
            })
            .ToList();

        var maxTotal = allOrgs.Max(x => x.Total);

        return allOrgs
            .Where(x => x.Total == maxTotal)
            .Select(x => new OrganizationStats
            {
                OrganizationId = x.Id,
                TotalDonations = x.Total
            })
            .ToList();
    }
}
