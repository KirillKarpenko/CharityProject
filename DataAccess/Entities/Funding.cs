namespace DataAccess.Entities;
public class Funding
{
    public required string Id { get; set; }

    public required string OrganizationId { get; set; }

    public required string ProjectId { get; set; }

    public required DateTime TimeOfOperation { get; set; }

    public required decimal Amount { get; set; }

    public Organization? Organization { get; set; }

    public Project? Project { get; set; }
}
