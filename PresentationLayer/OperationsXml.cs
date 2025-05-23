using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace PresentationLayer;

public class OperationsXml
{
    public static XDocument GetHighValueDonorsXml(CharityDbContext context)
    {
        var donors = context.Donors
            .Include(d => d.Donations)
            .AsEnumerable()
            .Where(d => d.Donations.Select(x => x.OrganizationId).Distinct().Count() >= 3
                        && d.Donations.Sum(x => x.Amount) > 10000)
            .Select(d => new XElement("Donor",
                new XAttribute("Id", d.Id),
                new XAttribute("Name", d.Name),
                new XElement("TotalAmount", d.Donations.Sum(x => x.Amount)),
                new XElement("OrganizationsCount", d.Donations.Select(x => x.OrganizationId).Distinct().Count())
            ));

        return new XDocument(new XElement("Donors", donors));
    }

    public static XDocument GetRecentTopOrganizationsXml(CharityDbContext context)
    {
        var dateThreshold = DateTime.UtcNow.AddMonths(-3);
        var organizations = context.Organizations
            .Include(o => o.Fundings)
            .Include(o => o.Donations)
            .AsEnumerable()
            .Where(o => o.Donations.Where(d => d.TimeOfOperation >= dateThreshold).Sum(d => d.Amount) > 5000
                        && o.Fundings.Sum(e => e.Amount) < o.Donations.Sum(d => d.Amount) * 0.5m)
            .Select(o => new XElement("Organization",
                new XAttribute("Id", o.Id),
                new XAttribute("Name", o.Name),
                new XElement("TotalDonations", o.Donations.Sum(d => d.Amount)),
                new XElement("TotalFundings", o.Fundings.Sum(f => f.Amount))
            ));

        return new XDocument(new XElement("Organizations", organizations));
    }

    public static XDocument GetDonorsWhoDonatedOnceXml(CharityDbContext context, int minOrgs)
    {
        var donors = context.Donors
            .Include(d => d.Donations)
            .AsEnumerable()
            .Where(d => d.Donations.GroupBy(x => x.OrganizationId).Count() > minOrgs
                     && d.Donations.GroupBy(x => x.OrganizationId).All(g => g.Count() == 1))
            .Select(d => new XElement("Donor",
                new XAttribute("Id", d.Id),
                new XAttribute("Name", d.Name),
                new XElement("UniqueOrganizations", d.Donations.Select(x => x.OrganizationId).Distinct().Count())
            ));

        return new XDocument(new XElement("Donors", donors));
    }

    public static XDocument GetDonorsFundsAdminExpensesXml(CharityDbContext context)
    {
        var donors = context.Donors
            .Include(d => d.Donations)
                .ThenInclude(don => don.Organization)
                    .ThenInclude(org => org.Fundings)
                        .ThenInclude(f => f.Project)
                            .ThenInclude(p => p.Report)
            .AsEnumerable()
            .Where(d => d.Donations
                        .Select(don => don.Organization)
                        .Where(org => org.Fundings.Count >= 2).Distinct().Count() >= 3);

        var results = donors.Select(d => {
            var reports = d.Donations.SelectMany(don => don.Organization.Fundings)
                .Select(f => f.Project?.Report)
                .Where(r => r != null).AsEnumerable();

            var admin = reports.Sum(r => r.AdministrativeSpending);
            var labour = reports.Sum(r => r.LabourSpending);
            var materials = reports.Sum(r => r.MaterialsSpending);
            var total = admin + labour + materials;
            var percent = total > 0 ? (admin / total) * 100 : 0;

            return new XElement("Donor",
                new XAttribute("Id", d.Id),
                new XAttribute("Name", d.Name),
                new XElement("AvgAdminPercent", percent.ToString("F2"))
            );
        });

        return new XDocument(new XElement("Donors", results));
    }

    public static XDocument GetDonationStatsXml(CharityDbContext context)
    {
        var stats = context.Donations
            .AsEnumerable()
            .GroupBy(d => d.OrganizationId)
            .Where(g => g.Count() >= 5)
            .Select(g => new XElement("Organization",
                new XAttribute("Id", g.Key),
                new XElement("TotalDonations", g.Sum(d => d.Amount)),
                new XElement("MaxDonation", g.Max(d => d.Amount))
            ));

        return new XDocument(new XElement("Organizations", stats));
    }

    public static XDocument GetDonorsAndOrganizationsXml(CharityDbContext context)
    {
        var entries = context.Donations
            .Include(d => d.Donor)
            .Include(d => d.Organization)
            .AsEnumerable()
            .Select(d => new XElement("Entry",
                new XElement("Donor", d.Donor.Name),
                new XElement("Organization", d.Organization.Name),
                new XElement("Amount", d.Amount)
            ));

        return new XDocument(new XElement("DonorOrganizationRelations", entries));
    }

    public static XDocument GetLastMonthActivityXml(CharityDbContext context)
    {
        var threshold = DateTime.Now.AddMonths(-3); // It was -1, but now it`s -3 to show smth (i`m too lazy to change db)
        var donors = context.Donors
            .Include(d => d.Donations)
            .AsEnumerable()
            .Where(d => d.Donations.Any(don => don.TimeOfOperation >= threshold))
            .OrderBy(d => d.Name)
            .Select(d => new XElement("Donor",
                new XAttribute("Id", d.Id),
                new XAttribute("Name", d.Name)
             ));

        return new XDocument(new XElement("Donors", donors));
    }

    public static XDocument GetOrganizationsWithTotalDonationsAboveXml(CharityDbContext context, decimal amount)
    {
        var organizations = context.Donations
            .Include(d => d.Organization)
            .AsEnumerable()
            .GroupBy(d => d.OrganizationId)
            .Where(g => g.Sum(d => d.Amount) > amount)
            .Select(g => new XElement("Organization",
                new XAttribute("Id", g.First().Organization.Id),
                new XAttribute("Name", g.First().Organization.Name),
                new XElement("TotalDonations", g.Sum(d => d.Amount))
            ));

        return new XDocument(new XElement("Organizations", organizations));
    }

    public static XDocument GetTopDonorsTotalDonationsXml(CharityDbContext context)
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

        var donors = grouped
            .Select(x => new XElement("Donor",
                new XAttribute("Id", x.Donor.Id),
                new XAttribute("Name", x.Donor.Name),
                new XElement("TotalDonated", x.Total)
            ));

        return new XDocument(new XElement("Donors", donors));
    }

    public static XDocument GetTopDonorsAverageDonationsXml(CharityDbContext context)
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

        var donors =  grouped
            .Where(x => x.Total == maxTotal)
            .Select(x => new XElement("Donor",
                new XAttribute("Id", x.Donor.Id),
                new XAttribute("Name", x.Donor.Name),
                new XElement("AvgDonation", x.Avg)
            ));

        return new XDocument(new XElement("Donors", donors));
    }

    public static XDocument GetTopFundedOrganizationsXml(CharityDbContext context)
    {
        var allOrgs = context.Organizations
            .Select(o => new
            {
                o.Id,
                o.Name,
                Total = o.Fundings.Sum(f => f.Amount)
            })
            .ToList();

        var maxTotal = allOrgs.Max(x => x.Total);

        var orgs = allOrgs
            .Where(x => x.Total == maxTotal)
            .Select(x => new XElement("Organization",
                new XAttribute("Id", x.Id),
                new XAttribute("Name", x.Name),
                new XElement("TotalFunded", x.Total)
            ));

        return new XDocument(new XElement("Organizations", orgs));
    }
}
