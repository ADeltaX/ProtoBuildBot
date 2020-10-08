using System.Xml.Serialization;

namespace BuildChecker.Classes
{
    //What's the difference? No xmlns.

    [XmlRoot(ElementName = "Deployment")]
    public class DeploymentXml
    {
        [XmlElement(ElementName = "ID")]
        public string ID { get; set; }
        [XmlElement(ElementName = "Action")]
        public string Action { get; set; }
        [XmlElement(ElementName = "IsAssigned")]
        public string IsAssigned { get; set; }
        [XmlElement(ElementName = "LastChangeTime")]
        public string LastChangeTime { get; set; }
        [XmlElement(ElementName = "AutoSelect")]
        public string AutoSelect { get; set; }
        [XmlElement(ElementName = "AutoDownload")]
        public string AutoDownload { get; set; }
        [XmlElement(ElementName = "SupersedenceBehavior")]
        public string SupersedenceBehavior { get; set; }
        [XmlElement(ElementName = "Priority")]
        public string Priority { get; set; }
        [XmlElement(ElementName = "HandlerSpecificAction")]
        public string HandlerSpecificAction { get; set; }
    }

    [XmlRoot(ElementName = "Verification")]
    public class VerificationXml
    {
        [XmlAttribute(AttributeName = "Timestamp")]
        public string Timestamp { get; set; }
        [XmlAttribute(AttributeName = "LeafCertificateId")]
        public string LeafCertificateId { get; set; }
        [XmlAttribute(AttributeName = "Signature")]
        public string Signature { get; set; }
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "UpdateInfo")]
    public class UpdateInfoXml
    {
        [XmlElement(ElementName = "ID")]
        public string ID { get; set; }
        [XmlElement(ElementName = "Deployment")]
        public DeploymentXml Deployment { get; set; }
        [XmlElement(ElementName = "IsLeaf")]
        public string IsLeaf { get; set; }
        [XmlElement(ElementName = "IsShared")]
        public string IsShared { get; set; }
        [XmlElement(ElementName = "Xml")]
        public string Xml { get; set; }
        [XmlElement(ElementName = "Verification")]
        public VerificationXml Verification { get; set; }
    }

    [XmlRoot(ElementName = "UpdateIdentity")]
    public class UpdateIdentity
    {
        [XmlAttribute(AttributeName = "UpdateID")]
        public string UpdateID { get; set; }
        [XmlAttribute(AttributeName = "RevisionNumber")]
        public string RevisionNumber { get; set; }

        public string ID { get; set; }
        public string LastChangeTime { get; set; }
        public string FlightID { get; set; }

        public LocalizedProperties LocalizedProperties { get; set; }

    }
}
