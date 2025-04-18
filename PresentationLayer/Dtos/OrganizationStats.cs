namespace DataAccess.Entities;
public class OrganizationStats
{
    public required string OrganizationId { get; set; }

    public decimal TotalDonations { get; set; }

    public decimal MaxDonation { get; set; }
}
