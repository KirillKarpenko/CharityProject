namespace DataAccess.Entities;
public class Organization
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public required string PhoneNumber { get; set; }

    public required string Email { get; set; }

    public required string Address { get; set;}

    public ICollection<Donation> Donations { get; set; } = [];

    public ICollection<Funding> Fundings { get; set; } = [];
}
