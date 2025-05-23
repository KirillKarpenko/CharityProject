using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DataAccess.Entities;
public class Donor
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public ICollection<Donation> Donations { get; set; } = [];
}
