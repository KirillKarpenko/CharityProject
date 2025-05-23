using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DataAccess.Entities;
public class Project
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public required string Location { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public ICollection<Funding> Fundings { get; set; } = [];

    [XmlIgnore]
    [JsonIgnore]
    public Report? Report { get; set; }
}
