using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using PresentationLayer;
using PresentationLayer.Dtos;
using PresentationLayer.Helpers;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace PrsentationLayer;
public static class Program
{
    public static async Task Main()
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
            Console.WriteLine("6 - Save Db to JSON files");
            Console.WriteLine("7 - Read JSON files");
            Console.WriteLine("8 - Add new Donor using Json Node");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ConsoleOutput();
                    break;
                case "2":
                    Console.WriteLine("Writing operations to XML");
                    WriteToXml();
                    Console.WriteLine("XML`s of operations successfully created");
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
                    Console.WriteLine("Saving Database to JSON");
                    await WriteToJsonAsync();
                    Console.WriteLine("Database successfully saved to JSON");
                    break;
                case "7":
                    await ReadJsonMenuAsync();
                    break;
                case "8":
                    await AddNewDonorWithJsonNodeAsync();
                    Console.WriteLine("New Donor successfully added");
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

    private static async Task ReadJsonMenuAsync()
    {
        Console.WriteLine("\nChoose method of reading:");
        Console.WriteLine("1 - Use Deserializer");
        Console.WriteLine("2 - use Json Document");
        Console.WriteLine("3 - Use Json Node");
        Console.WriteLine("Any other keys will kick you out of this menu");

        var choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                await ReadAllWithJsonDeserializerAsync();
                break;
            case "2":
                ReadAllWithJsonDocument();
                break;
            case "3":
                await ReadAllWithJsonNodeAsync();
                break;
            default:
                break;
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
            var topDonorsTotal = Operations.GetTopDonorsTotalDonations(context);
            Console.WriteLine($"\nTop donor total donations:");
            foreach (var donor in topDonorsTotal)
            {
                Console.WriteLine($"Name: {donor.Name}, Total donated: {donor.Donated}");
            }

            var topDonorsAverage = Operations.GetTopDonorsAverageDonations(context);
            Console.WriteLine($"\nTop donor average donation:");
            foreach (var donor in topDonorsAverage)
            {
                Console.WriteLine($"Name: {donor.Name}, Average donation: {donor.Donated}");
            }

            var topFundedOrganizations = Operations.GetTopFundedOrganizations(context);
            Console.WriteLine($"\nTop organization average donation:");
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
            var HighValueDonors = OperationsXml.GetHighValueDonorsXml(context);
            HighValueDonors.Save("XMLs/high-value-donors.xml");

            // 2. Фонди, які отримали найбільше коштів за останній квартал, але витратили менше 50%
            var recentTopOrganizations = OperationsXml.GetRecentTopOrganizationsXml(context);
            recentTopOrganizations.Save("XMLs/recent-top-orgs.xml");

            // 3. Донори, які зробили внески у більше ніж 5 фондів, але не більше одного разу в кожен фонд
            var preciseDonors = OperationsXml.GetDonorsWhoDonatedOnceXml(context, 4);
            preciseDonors.Save("XMLs/precise-donors.xml");

            // 4. Знайти донорів, які фінансували принаймні три різні організації, кожна з яких реалізувала хоча б два проєкти, і знайти середній відсоток адміністративних витрат у цих проєктах
            var donorAdminExpenses = OperationsXml.GetDonorsFundsAdminExpensesXml(context);
            donorAdminExpenses.Save("XMLs/donors-admin-exp.xml");

            // Виводить прикольну статистику в консоль
            var donationStats = OperationsXml.GetDonationStatsXml(context);
            donationStats.Save("XMLs/stats.xml");

            // Групує донорів та орги (з назви понятно, я знаю)
            var donorFunds = OperationsXml.GetDonorsAndOrganizationsXml(context);
            donorFunds.Save("XMLs/donor-funds.xml");

            // Видає донорів, які донатили хоч щось ласт місяць
            var activeDonors = OperationsXml.GetLastMonthActivityXml(context);
            activeDonors.Save("XMLs/active-donors.xml");

            // Видає орги, яким задонатили більше, ніж amount
            var highBalanceOrganizations = OperationsXml.GetOrganizationsWithTotalDonationsAboveXml(context, 10000);
            highBalanceOrganizations.Save("XMLs/high-balance-orgs.xml");

            // Знаходить топ amount донорів і виводить в консоль
            var topDonorsTotal = OperationsXml.GetTopDonorsTotalDonationsXml(context);
            topDonorsTotal.Save("XMLs/top-donors-total.xml");

            var topDonorsAverage = OperationsXml.GetTopDonorsAverageDonationsXml(context);
            topDonorsAverage.Save("XMLs/top-donors-avg.xml");

            var topFundedOrganizations = OperationsXml.GetTopFundedOrganizationsXml(context);
            topFundedOrganizations.Save("XMLs/top-funded-orgs.xml");
        }
    }

    private static void ReadFromXml()
    {
        Console.WriteLine("\nHigh value donors:");
        var highValueDoc = XDocument.Load("XMLs/high-value-donors.xml");
        foreach (var donor in highValueDoc.Descendants("Donor"))
        {
            Console.WriteLine($"Id: {donor.Attribute("Id")?.Value}, Name: {donor.Attribute("Name")?.Value}, " +
                              $"Total: {donor.Element("TotalAmount")?.Value}, " +
                              $"OrgsCount: {donor.Element("OrganizationsCount")?.Value}");
        }

        Console.WriteLine("\nOrganizations with the most funds for the last quarter, but spent less than 50% of it:");
        var topOrgsDoc = XDocument.Load("XMLs/recent-top-orgs.xml");
        foreach (var org in topOrgsDoc.Descendants("Organization"))
        {
            Console.WriteLine($"Id: {org.Attribute("Id")?.Value}, Name: {org.Attribute("Name")?.Value}, " +
                              $"TotalDonations: {org.Element("TotalDonations")?.Value}, " +
                              $"TotalFundings: {org.Element("TotalFundings")?.Value}");
        }

        Console.WriteLine("\nDonors, who have donated to one Fund at a time:");
        var preciseDonorsDoc = XDocument.Load("XMLs/precise-donors.xml");
        foreach (var donor in preciseDonorsDoc.Descendants("Donor"))
        {
            Console.WriteLine($"Id: {donor.Attribute("Id")?.Value}, Name: {donor.Attribute("Name")?.Value}, " +
                              $"UniqueOrgs: {donor.Element("UniqueOrganizations")?.Value}");
        }

        Console.WriteLine("\nAdministrative spending from donations of donors:");
        var donorAdminDoc = XDocument.Load("XMLs/donors-admin-exp.xml");
        foreach (var donor in donorAdminDoc.Descendants("Donor"))
        {
            Console.WriteLine($"Id: {donor.Attribute("Id")?.Value}, Name: {donor.Attribute("Name")?.Value}, " +
                              $"AvgAdminPercent: {donor.Element("AvgAdminPercent")?.Value}%");
        }

        Console.WriteLine("\nDonation stats for each fund that got more than 5 donations:");
        var statsDoc = XDocument.Load("XMLs/stats.xml");
        foreach (var org in statsDoc.Descendants("Organization"))
        {
            Console.WriteLine($"Id: {org.Attribute("Id")?.Value}, " +
                              $"Total: {org.Element("TotalDonations")?.Value}, " +
                              $"Max: {org.Element("MaxDonation")?.Value}");
        }

        Console.WriteLine("\nDonors and funds they`ve donated to:");
        var relationsDoc = XDocument.Load("XMLs/donor-funds.xml");
        foreach (var entry in relationsDoc.Descendants("Entry"))
        {
            Console.WriteLine($"Donor: {entry.Element("Donor")?.Value}, " +
                              $"Organization: {entry.Element("Organization")?.Value}, " +
                              $"Amount: {entry.Element("Amount")?.Value}");
        }

        Console.WriteLine("\nDonors who were active during last month:");
        var recentDonorsDoc = XDocument.Load("XMLs/active-donors.xml");
        foreach (var donor in recentDonorsDoc.Descendants("Donor"))
        {
            Console.WriteLine($"Id: {donor.Attribute("Id")?.Value}, Name: {donor.Attribute("Name")?.Value}");
        }

        Console.WriteLine("\nOrganizations with more than 10k:");
        var highBalanceDoc = XDocument.Load("XMLs/high-balance-orgs.xml");
        foreach (var org in highBalanceDoc.Descendants("Organization"))
        {
            Console.WriteLine($"Id: {org.Attribute("Id")?.Value}, Name: {org.Attribute("Name")?.Value}, " +
                              $"Total: {org.Element("TotalDonations")?.Value}");
        }

        Console.WriteLine($"\nTop donor total donations:");
        var topTotalDoc = XDocument.Load("XMLs/top-donors-total.xml");
        foreach (var donor in topTotalDoc.Descendants("Donor"))
        {
            Console.WriteLine($"Id: {donor.Attribute("Id")?.Value}, Name: {donor.Attribute("Name")?.Value}, " +
                              $"TotalDonated: {donor.Element("TotalDonated")?.Value}");
        }

        Console.WriteLine($"\nTop donor average donation:");
        var topAvgDoc = XDocument.Load("XMLs/top-donors-avg.xml");
        foreach (var donor in topAvgDoc.Descendants("Donor"))
        {
            Console.WriteLine($"Id: {donor.Attribute("Id")?.Value}, Name: {donor.Attribute("Name")?.Value}, " +
                              $"AvgDonation: {donor.Element("AvgDonation")?.Value}");
        }

        Console.WriteLine($"\nTop organization average donation:");
        var topFundedDoc = XDocument.Load("XMLs/top-funded-orgs.xml");
        foreach (var org in topFundedDoc.Descendants("Organization"))
        {
            Console.WriteLine($"Id: {org.Attribute("Id")?.Value}, Name: {org.Attribute("Name")?.Value}, " +
                              $"TotalFunded: {org.Element("TotalFunded")?.Value}");
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

    private async static Task WriteToJsonAsync()
    {
        using (var context = new CharityDbContext())
        {
            var donors = await context.Donors.ToListAsync();
            var jsonDonors = JsonSerializer.Serialize(donors);
            await File.WriteAllTextAsync("JSONs/donors.json", jsonDonors);

            var organizations = await context.Organizations.ToListAsync();
            var jsonOrgs = JsonSerializer.Serialize(organizations);
            await File.WriteAllTextAsync("JSONs/organizations.json", jsonOrgs);

            var donations = await context.Donations.ToListAsync();
            var jsonDonations = JsonSerializer.Serialize(donations);
            await File.WriteAllTextAsync("JSONs/donations.json", jsonDonations);

            var projects = await context.Projects.ToListAsync();
            var jsonProjects = JsonSerializer.Serialize(projects);
            await File.WriteAllTextAsync("JSONs/projects.json", jsonProjects);

            var fundings = await context.Fundings.ToListAsync();
            var jsonFunds = JsonSerializer.Serialize(fundings);
            await File.WriteAllTextAsync("JSONs/funds.json", jsonFunds);

            var reports = await context.Reports.ToListAsync();
            var jsonReports = JsonSerializer.Serialize(reports);
            await File.WriteAllTextAsync("JSONs/reports.json", jsonReports);
        }
    }

    private static async Task ReadAllWithJsonDeserializerAsync()
    {
        var files = Directory.GetFiles("JSONs", "*.json");

        foreach (var filePath in files)
        {
            var fileName = Path.GetFileName(filePath);
            Console.WriteLine($"\n=== {fileName} ===");

            var json = await File.ReadAllTextAsync(filePath);

            try
            {
                var items = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);
                if (items == null) continue;

                foreach (var item in items)
                {
                    foreach (var kvp in item)
                    {
                        Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                    }
                    Console.WriteLine(new string('-', 40));
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Failure during deserialization of: {fileName}: {ex.Message}");
            }
        }
    }

    private static void ReadAllWithJsonDocument()
    {
        var files = Directory.GetFiles("JSONs", "*.json");

        foreach (var filePath in files)
        {
            var fileName = Path.GetFileName(filePath);
            Console.WriteLine($"\n=== {fileName} ===");

            using var stream = File.OpenRead(filePath);
            using var doc = JsonDocument.Parse(stream);

            foreach (var item in doc.RootElement.EnumerateArray())
            {
                foreach (var property in item.EnumerateObject())
                {
                    Console.WriteLine($"{property.Name}: {property.Value}");
                }
                Console.WriteLine(new string('-', 40));
            }
        }
    }

    private static async Task ReadAllWithJsonNodeAsync()
    {
        var files = Directory.GetFiles("JSONs", "*.json");

        foreach (var filePath in files)
        {
            var fileName = Path.GetFileName(filePath);
            Console.WriteLine($"\n=== {fileName} ===");

            var json = await File.ReadAllTextAsync(filePath);
            var array = JsonNode.Parse(json)?.AsArray();
            if (array == null) continue;

            foreach (var item in array)
            {
                if (item is JsonObject obj)
                {
                    foreach (var property in obj)
                    {
                        Console.WriteLine($"{property.Key}: {property.Value}");
                    }
                    Console.WriteLine(new string('-', 40));
                }
            }
        }
    }

    private static async Task AddNewDonorWithJsonNodeAsync()
    {
        var filePath = "JSONs/donors.json";

        JsonArray donorsArray;

        if (File.Exists(filePath))
        {
            var json = await File.ReadAllTextAsync(filePath);
            var rootNode = JsonNode.Parse(json);

            donorsArray = rootNode as JsonArray ?? new JsonArray();
        }
        else
        {
            donorsArray = new JsonArray();
        }

        var id = Guid.NewGuid().ToString();

        Console.WriteLine("Enter full name: ");
        string name = Console.ReadLine();

        Console.WriteLine("Enter phone number: ");
        string phone = Console.ReadLine();

        Console.WriteLine("Enter email: ");
        string email = Console.ReadLine();

        var newDonor = new JsonObject
        {
            ["Id"] = id,
            ["Name"] = name,
            ["PhoneNumber"] = phone,
            ["Email"] = email
        };

        donorsArray.Add(newDonor);

        await File.WriteAllTextAsync(filePath, donorsArray.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
    }
}