using System.Xml.Serialization;

namespace PresentationLayer.Helpers;
public static class XmlSerializerHelper
{
    public static void SerializeToXml<T>(T data, string? filePath = null)
    {
        var type = typeof(T);
        filePath ??= $"{type.Name}.xml";

        var rootAttr = new XmlRootAttribute(type.Name);
        var serializer = new XmlSerializer(type, rootAttr);

        using var stream = new FileStream(filePath, FileMode.Create);
        serializer.Serialize(stream, data);
    }

    public static T DeserializeFromXml<T>(string? filePath = null)
    {
        var type = typeof(T);
        filePath ??= $"{type.Name}.xml";

        var rootAttr = new XmlRootAttribute(type.Name);
        var serializer = new XmlSerializer(type, rootAttr);

        using var stream = new FileStream(filePath, FileMode.Open);
        return (T)serializer.Deserialize(stream)!;
    }
}