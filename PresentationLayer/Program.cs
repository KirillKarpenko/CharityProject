using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using PresentationLayer;

namespace PrsentationLayer;
public static class Program
{
    public static void Main()
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
            var donorExpensesTuples = Operations.GetDonorsFundsAdminExpenses(context);

            Console.WriteLine("\nAdministrative spending from donations of donors:");
            foreach (var tuple in donorExpensesTuples)
            {
                Console.WriteLine($"Id: {tuple.Item1.Id}, Name: {tuple.Item1.Name}, Avg administrative spending % of projects: {tuple.Item2}");
            }

            // З ціллю демонстрації того, що ці функції лінку існують
            // противні методи, які вертають анонімний клас
            // (рефакторить мені в падлу)
            // Знаходить топ amount донорів і виводить в консоль
            Operations.WriteTopDonorsToConsole(context, 5);

            // Виводить прикольну статистику в консоль
            Operations.WriteDonationStatsToConsole(context);

            // Групує донорів та орги і пише в консольку (з назви понятно, я знаю)
            Operations.WriteDonorsAndOrganizationsToConsole(context);

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
        }
    }
}