using System.Xml.Serialization;

namespace simur_backend.Hypermedia
{
    public class HypermediaLinks
    {
        [XmlAttribute]
        public string Rel { get; set; } = string.Empty;

        [XmlAttribute]
        public string Href { get; set; } = string.Empty;

        [XmlAttribute]
        public string Type { get; set; } = string.Empty;

        [XmlAttribute]
        public string Action { get; set; } = string.Empty;


    }
}
