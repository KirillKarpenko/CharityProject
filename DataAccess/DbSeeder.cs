using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;
public static class DbSeeder
{
    public static void SeedDb(ModelBuilder modelBuilder)
    {
        var organizations = new List<Organization>();
        var projects = new List<Project>();
        var donors = new List<Donor>();
        var donations = new List<Donation>();
        var fundings = new List<Funding>();
        var reports = new List<Report>();

        var rnd = new Random();

        var donor1Id = Guid.NewGuid();
        donors.Add(new Donor
        {
            Id = donor1Id.ToString(),
            Name = "One fund donor",
            PhoneNumber = "380599000000",
            Email = "1t1mer@mail.com",
        });
        var orgs = organizations;
        foreach (var org in orgs)
        {
            donations.Add(new Donation
            {
                Id = Guid.NewGuid().ToString(),
                DonorId = donor1Id.ToString(),
                OrganizationId = org.Id,
                TimeOfOperation = DateTime.Now,
                Amount = rnd.Next(1000, 10000),
            });
        }

        var donor2Id = Guid.NewGuid();
        donors.Add(new Donor
        {
            Id = donor2Id.ToString(),
            Name = "Mr. White",
            PhoneNumber = "380699000000",
            Email = "Org3lover@mail.com",
        });

        for (int i = 1; i <= 5; i++)
        {
            var orgId = Guid.NewGuid();
            organizations.Add(new Organization
            {
                Id = orgId.ToString(),
                Name = $"Organization {i}",
                Description = $"Description for Org {i}",
                PhoneNumber = $"38044{i}000000",
                Email = $"org{i}@charity.com",
                Address = $"Street {i}, City",
            });

            for (int j = 1; j <= 3; j++)
            {
                var projId = Guid.NewGuid();
                projects.Add(new Project
                {
                    Id = projId.ToString(),
                    Name = $"Project {j} for Org {i}",
                    Description = $"Project {j} description",
                    Location = $"Region {j}",
                });

                fundings.Add(new Funding
                {
                    Id = Guid.NewGuid().ToString(),
                    ProjectId = projId.ToString(),
                    OrganizationId = orgId.ToString(),
                    Amount = rnd.Next(1000, 20000),
                    TimeOfOperation = DateTime.Now.AddDays(-rnd.Next(10, 100)),
                });

                reports.Add(new Report
                {
                    Id = Guid.NewGuid().ToString(),
                    ProjectId = projId.ToString(),
                    AdministrativeSpending = rnd.Next(1000, 5000),
                    MaterialsSpending = rnd.Next(2000, 7000),
                    LabourSpending = rnd.Next(3000, 8000),
                });
            }

            if (i == 1)
            {
                donations.Add(new Donation
                {
                    Id = Guid.NewGuid().ToString(),
                    DonorId = donor2Id.ToString(),
                    OrganizationId = orgId.ToString(),
                    TimeOfOperation = DateTime.Now,
                    Amount = 150000,
                });
            }
        }

        for (int i = 1; i <= 6; i++)
        {
            var donorId = Guid.NewGuid();
            donors.Add(new Donor
            {
                Id = donorId.ToString(),
                Name = $"Donor {i}",
                PhoneNumber = $"38050{i}000000",
                Email = $"donor{i}@mail.com",
            });

            foreach (var org in organizations.OrderBy(_ => rnd.Next()).Take(rnd.Next(2, 5)))
            {
                donations.Add(new Donation
                {
                    Id = Guid.NewGuid().ToString(),
                    DonorId = donorId.ToString(),
                    OrganizationId = org.Id,
                    Amount = rnd.Next(1000, 10000),
                    TimeOfOperation = DateTime.Now.AddDays(-rnd.Next(10, 365))
                });
            }
        }

        modelBuilder.Entity<Organization>().HasData(organizations);
        modelBuilder.Entity<Project>().HasData(projects);
        modelBuilder.Entity<Donor>().HasData(donors);
        modelBuilder.Entity<Donation>().HasData(donations);
        modelBuilder.Entity<Funding>().HasData(fundings);
        modelBuilder.Entity<Report>().HasData(reports);
    }
}
