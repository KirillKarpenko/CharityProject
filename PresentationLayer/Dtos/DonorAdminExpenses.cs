namespace PresentationLayer.Dtos;
public class DonorAdminExpenses
{
    public required string DonorId { get; set; }

    public required string DonorName { get; set; }

    public decimal Amount { get; set; }
}
