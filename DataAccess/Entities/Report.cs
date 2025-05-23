using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DataAccess.Entities;
public class Report
{
    public required string Id { get; set; }

    public required string ProjectId { get; set; }

    public required decimal AdministrativeSpending { get; set; }

    public required decimal MaterialsSpending { get; set; }

    public required decimal LabourSpending { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public Project? Project {  get; set; }
}
