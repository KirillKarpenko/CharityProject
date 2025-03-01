namespace DataAccess.Entities;
public class Project
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public required string Location { get; set; }

    public ICollection<Funding> Fundings { get; set; } = [];

    public Report? Report { get; set; }
}
