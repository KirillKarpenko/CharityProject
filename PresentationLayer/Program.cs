using DataAccess;
using DataAccess.Entities;
using PresentationLayer;
using PresentationLayer.Dtos;
using PresentationLayer.Helpers;
using System.Collections.Generic;
using System.Text.Json;

namespace PrsentationLayer;
public static class Program
{
    public static void Main()
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\nChoose an option:");
            Console.WriteLine("1 - Write results to Console");
            Console.WriteLine("2 - Write results to XML");
            Console.WriteLine("3 - Read XMLs to Console");
            Console.WriteLine("4 - Tell my XML serializer how is your day");
            Console.WriteLine("5 - Ask my XML serializer how your day was");
            Console.WriteLine("6 - Tell my JSON serializer how is your day");
            Console.WriteLine("7 - Ask my JSON serializer how your day was");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ConsoleOutput();
                    break;
                case "2":
                    WriteToXml();
                    break;
                case "3":
                    ReadFromXml();
                    break;
                case "4":
                    AddXml();
                    break;
                case "5":
                    ReadXml();
                    break;
                case "6":
                    AddJson();
                    break;
                case "7":
                    ReadJson();
                    break;
                default:
                    Console.WriteLine("That`s not what I asked for!");
                    break;
            }

            Console.WriteLine("\nPress ESC to exit, any other key to continue");
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Escape)
            {
                exit = true;
            }
        }
    }

    private static void ConsoleOutput()
    {
        using (var context = new CharityDbContext())
        {
            // 1. Знайти донорів, які пожертвували в 3 і більше різні фонди, і загальна сума перевищує 10000 грн.
            var HighValueDonors = Operations.GetHighValueDonor(context);
            Console.WriteLine("\nHigh value donors:");
            foreach (var donor in HighValueDonors)
            {
                Console.WriteLine($"Id: {donor.Id}, Name: {donor.Name}");
            }

            // 2. Фонди, які отримали найбільше коштів за останній квартал, але витратили менше 50%
            var recentTopOrganizations = Operations.GetRecentTopOrganizations(context);
            Console.WriteLine("\nOrganizations with the most funds for the last quarter, but spent less than 50% of it:");
            foreach (var org in recentTopOrganizations)
            {
                Console.WriteLine($"Id: {org.Id}, Name {org.Name}");
            }

            // 3. Донори, які зробили внески у більше ніж 5 фондів, але не більше одного разу в кожен фонд
            var preciseDonors = Operations.GetDonorsWhoDonatedOnce(context, 4);
            Console.WriteLine("\nDonors, who have donated to one Fund at a time:");
            foreach (var donor in preciseDonors)
            {
                Console.WriteLine($"Id: {donor.Id}, Name: {donor.Name}");
            }

            // 4. Знайти донорів, які фінансували принаймні три різні організації, кожна з яких реалізувала хоча б два проєкти, і знайти середній відсоток адміністративних витрат у цих проєктах
            var donorAdminExpenses = Operations.GetDonorsFundsAdminExpenses(context);
            Console.WriteLine("\nAdministrative spending from donations of donors:");
            foreach (var entry in donorAdminExpenses)
            {
                Console.WriteLine($"Id: {entry.DonorId}, Name: {entry.DonorName}, Avg administrative spending % of projects: {entry.Amount}");
            }

            // Виводить прикольну статистику в консоль
            var donationStats = Operations.GetDonationStats(context);
            Console.WriteLine("\nDonation stats for each fund that got more than 5 donations:");
            foreach (var entry in donationStats)
            {
                Console.WriteLine($"Fund id: {entry.OrganizationId}, Total donations: {entry.TotalDonations}, Biggest donation: {entry.MaxDonation}");
            }

            // Групує донорів та орги (з назви понятно, я знаю)
            var donorFunds = Operations.GetDonorsAndOrganizations(context);
            Console.WriteLine("\nDonors and funds they`ve donated to:");
            foreach (var entry in donorFunds)
            {
                Console.WriteLine($"Donor name: {entry.DonorName}, Fund name: {entry.OrganizationName}, Amount donated: {entry.Amount}");
            }

            // Видає донорів, які донатили хоч щось ласт місяць
            var activeDonors = Operations.GetLastMonthActivity(context);
            Console.WriteLine("\nDonors who were active during last month:");
            foreach (var donor in activeDonors)
            {
                Console.WriteLine($"Id: {donor.Id}, Name: {donor.Name}");
            }

            // Видає орги, яким задонатили більше, ніж amount
            var highBalanceOrganizations = Operations.GetOrganizationsWithTotalDonationsAbove(context, 10000);
            Console.WriteLine("\nOrganizations with more than 10k:");
            foreach (var org in highBalanceOrganizations)
            {
                Console.WriteLine($"Organization Id: {org.Id}," +
                    $"Name: {org.Name}," +
                    $"Donations recieved {org.Donations.Sum(d => d.Amount)}");
            }

            // Бере айдішник Guid і вертає середні донати в оргу з цим айді
            var orgId = "ceecc567-2fc6-48d7-a82c-c912993f5ec1";
            var organization = context.Organizations.SingleOrDefault(o => o.Id == orgId);
            var averageDonationSize = Operations.AverageDonationForOrganization(context, orgId);
            Console.WriteLine($"\nAverage donation size for {organization.Name} is {averageDonationSize}");

            // Знаходить топ amount донорів і виводить в консоль
            var topDonorsTotal = Operations.GetTopDonorsTotalDonations(context, 5);
            Console.WriteLine($"\nTop-5 donors total donations:");
            foreach (var donor in topDonorsTotal)
            {
                Console.WriteLine($"Name: {donor.Name}, Total donated: {donor.Donated}");
            }

            var topDonorsAverage = Operations.GetTopDonorsAverageDonations(context, 5);
            Console.WriteLine($"\nTop-5 donors average donation:");
            foreach (var donor in topDonorsAverage)
            {
                Console.WriteLine($"Name: {donor.Name}, Average donation: {donor.Donated}");
            }

            var topFundedOrganizations = Operations.GetTopFundedOrganizations(context, 5);
            Console.WriteLine($"\nTop-5 organizations average donation:");
            foreach (var org in topFundedOrganizations)
            {
                Console.WriteLine($"Name: {org.OrganizationId}, Average donation: {org.TotalDonations}");
            }
        }
    }

    private static void WriteToXml()
    {
        using (var context = new CharityDbContext())
        {
            // 1. Знайти донорів, які пожертвували в 3 і більше різні фонди, і загальна сума перевищує 10000 грн.
            var HighValueDonors = Operations.GetHighValueDonor(context);
            XmlSerializerHelper.SerializeToXml(HighValueDonors, "XMLs/high-value-donors.xml");

            // 2. Фонди, які отримали найбільше коштів за останній квартал, але витратили менше 50%
            var recentTopOrganizations = Operations.GetRecentTopOrganizations(context);
            XmlSerializerHelper.SerializeToXml(recentTopOrganizations, "XMLs/recent-top-orgs.xml");

            // 3. Донори, які зробили внески у більше ніж 5 фондів, але не більше одного разу в кожен фонд
            var preciseDonors = Operations.GetDonorsWhoDonatedOnce(context, 4);
            XmlSerializerHelper.SerializeToXml(preciseDonors, "XMLs/precise-donors.xml");

            // 4. Знайти донорів, які фінансували принаймні три різні організації, кожна з яких реалізувала хоча б два проєкти, і знайти середній відсоток адміністративних витрат у цих проєктах
            var donorAdminExpenses = Operations.GetDonorsFundsAdminExpenses(context);
            XmlSerializerHelper.SerializeToXml(donorAdminExpenses, "XMLs/donors-admin-exp.xml");

            // Виводить прикольну статистику в консоль
            var donationStats = Operations.GetDonationStats(context);
            XmlSerializerHelper.SerializeToXml(donationStats, "XMLs/stats.xml");

            // Групує донорів та орги (з назви понятно, я знаю)
            var donorFunds = Operations.GetDonorsAndOrganizations(context);
            XmlSerializerHelper.SerializeToXml(donorFunds, "XMLs/donor-funds.xml");

            // Видає донорів, які донатили хоч щось ласт місяць
            var activeDonors = Operations.GetLastMonthActivity(context);
            XmlSerializerHelper.SerializeToXml(activeDonors, "XMLs/active-donors.xml");

            // Видає орги, яким задонатили більше, ніж amount
            var highBalanceOrganizations = Operations.GetOrganizationsWithTotalDonationsAbove(context, 10000);
            XmlSerializerHelper.SerializeToXml(highBalanceOrganizations, "XMLs/high-balance-orgs.xml");

            // Знаходить топ amount донорів і виводить в консоль
            var topDonorsTotal = Operations.GetTopDonorsTotalDonations(context, 5);
            XmlSerializerHelper.SerializeToXml(topDonorsTotal, "XMLs/top-donors-total.xml");

            var topDonorsAverage = Operations.GetTopDonorsAverageDonations(context, 5);
            XmlSerializerHelper.SerializeToXml(topDonorsAverage, "XMLs/top-donors-avg.xml");

            var topFundedOrganizations = Operations.GetTopFundedOrganizations(context, 5);
            XmlSerializerHelper.SerializeToXml(topFundedOrganizations, "XMLs/top-funded-orgs.xml");
        }
    }

    private static void ReadFromXml()
    {
        using (var context = new CharityDbContext())
        {
            // 1. Знайти донорів, які пожертвували в 3 і більше різні фонди, і загальна сума перевищує 10000 грн.
            var HighValueDonors = XmlSerializerHelper.DeserializeFromXml<List<Donor>>("XMLs/high-value-donors.xml");
            Console.WriteLine("\nHigh value donors:");
            foreach (var donor in HighValueDonors)
            {
                Console.WriteLine($"Id: {donor.Id}, Name: {donor.Name}");
            }

            // 2. Фонди, які отримали найбільше коштів за останній квартал, але витратили менше 50%
            var recentTopOrganizations = XmlSerializerHelper.DeserializeFromXml<List<Organization>>("XMLs/recent-top-orgs.xml");
            Console.WriteLine("\nOrganizations with the most funds for the last quarter, but spent less than 50% of it:");
            foreach (var org in recentTopOrganizations)
            {
                Console.WriteLine($"Id: {org.Id}, Name {org.Name}");
            }

            // 3. Донори, які зробили внески у більше ніж 5 фондів, але не більше одного разу в кожен фонд
            var preciseDonors = XmlSerializerHelper.DeserializeFromXml<List<Donor>>("XMLs/precise-donors.xml");
            Console.WriteLine("\nDonors, who have donated to one Fund at a time:");
            foreach (var donor in preciseDonors)
            {
                Console.WriteLine($"Id: {donor.Id}, Name: {donor.Name}");
            }

            // 4. Знайти донорів, які фінансували принаймні три різні організації, кожна з яких реалізувала хоча б два проєкти, і знайти середній відсоток адміністративних витрат у цих проєктах
            var donorAdminExpenses = XmlSerializerHelper.DeserializeFromXml<List<DonorAdminExpenses>>("XMLs/donors-admin-exp.xml");
            Console.WriteLine("\nAdministrative spending from donations of donors:");
            foreach (var entry in donorAdminExpenses)
            {
                Console.WriteLine($"Id: {entry.DonorId}, Name: {entry.DonorName}, Avg administrative spending % of projects: {entry.Amount}");
            }

            // Виводить прикольну статистику в консоль
            var donationStats = XmlSerializerHelper.DeserializeFromXml<List<OrganizationStats>>("XMLs/stats.xml");
            Console.WriteLine("\nDonation stats for each fund that got more than 5 donations:");
            foreach (var entry in donationStats)
            {
                Console.WriteLine($"Fund id: {entry.OrganizationId}, Total donations: {entry.TotalDonations}, Biggest donation: {entry.MaxDonation}");
            }

            // Групує донорів та орги (з назви понятно, я знаю)
            var donorFunds = XmlSerializerHelper.DeserializeFromXml<List<DonorAndOrganizations>>("XMLs/donor-funds.xml");
            Console.WriteLine("\nDonors and funds they`ve donated to:");
            foreach (var entry in donorFunds)
            {
                Console.WriteLine($"Donor name: {entry.DonorName}, Fund name: {entry.OrganizationName}, Amount donated: {entry.Amount}");
            }

            // Видає донорів, які донатили хоч щось ласт місяць
            var activeDonors = Operations.GetLastMonthActivity(context);
            Console.WriteLine("\nDonors who were active during last month:");
            foreach (var donor in activeDonors)
            {
                Console.WriteLine($"Id: {donor.Id}, Name: {donor.Name}");
            }
            XmlSerializerHelper.SerializeToXml(activeDonors);

            // Видає орги, яким задонатили більше, ніж amount
            var highBalanceOrganizations = XmlSerializerHelper.DeserializeFromXml<List<Organization>>("XMLs/high-balance-orgs.xml");
            Console.WriteLine("\nOrganizations with more than 10k:");
            foreach (var org in highBalanceOrganizations)
            {
                Console.WriteLine($"Organization Id: {org.Id}," +
                    $"Name: {org.Name}," +
                    $"Donations recieved {org.Donations.Sum(d => d.Amount)}");
            }

            // Знаходить топ amount донорів і виводить в консоль
            var topDonorsTotal = XmlSerializerHelper.DeserializeFromXml<List<DonorDonations>>("XMLs/top-donors-total.xml");
            Console.WriteLine($"\nTop-5 donors total donations:");
            foreach (var donor in topDonorsTotal)
            {
                Console.WriteLine($"Name: {donor.Name}, Total donated: {donor.Donated}");
            }

            var topDonorsAverage = XmlSerializerHelper.DeserializeFromXml<List<DonorDonations>>("XMLs/top-donors-avg.xml");
            Console.WriteLine($"\nTop-5 donors average donation:");
            foreach (var donor in topDonorsAverage)
            {
                Console.WriteLine($"Name: {donor.Name}, Average donation: {donor.Donated}");
            }

            var topFundedOrganizations = XmlSerializerHelper.DeserializeFromXml<List<OrganizationStats>>("XMLs/top-funded-orgs.xml");
            Console.WriteLine($"\nTop-5 organizations average donation:");
            foreach (var org in topFundedOrganizations)
            {
                Console.WriteLine($"Name: {org.OrganizationId}, Average donation: {org.TotalDonations}");
            }
        }
    }

    private static void AddXml()
    {
        Console.WriteLine("How is your day?");
        var answer = Console.ReadLine();
        Console.WriteLine("In grade 1-10?");
        var grade = Int32.Parse(Console.ReadLine());
        var result = new HowsYourDay
        {
            HowIsYourDay = answer,
            Grade = grade,
        };
        XmlSerializerHelper.SerializeToXml(result, "XMLs/!How is your day.xml");
    }

    private static void ReadXml()
    {
        Console.WriteLine("My XML serializer says that your day was...");
        var result = XmlSerializerHelper.DeserializeFromXml<HowsYourDay>("XMLs/!How is your day.xml");
        Console.WriteLine($"{result.HowIsYourDay}\n{result.Grade}");
    }

    private static void AddJson()
    {
        Console.WriteLine("How is your day?");
        var answer = Console.ReadLine();
        Console.WriteLine("In grade 1-10?");
        var grade = Int32.Parse(Console.ReadLine());
        var result = new HowsYourDay
        {
            HowIsYourDay = answer,
            Grade = grade,
        };
        var jsonString = JsonSerializer.Serialize(result);
        File.WriteAllText("JSONs/How is your day.json", jsonString);
    }

    private static void ReadJson()
    {
        Console.WriteLine("My JSON serializer says that your day was...");
        var jsonString = File.ReadAllText("JSONs/How is your day.json");
        var result = JsonSerializer.Deserialize<HowsYourDay>(jsonString);
        Console.WriteLine($"{result.HowIsYourDay}\n{result.Grade}");
    }
}