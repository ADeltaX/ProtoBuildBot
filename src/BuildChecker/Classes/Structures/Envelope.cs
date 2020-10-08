using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace BuildChecker.Classes
{
    [XmlRoot(ElementName = "Action", Namespace = "http://www.w3.org/2005/08/addressing")]
    public class Action
    {
        [XmlAttribute(AttributeName = "mustUnderstand", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public string MustUnderstand { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Timestamp", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
    public class Timestamp
    {
        [XmlElement(ElementName = "Created", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
        public string Created { get; set; }
        [XmlElement(ElementName = "Expires", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
        public string Expires { get; set; }
        [XmlAttribute(AttributeName = "Id", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "Security", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
    public class Security
    {
        [XmlElement(ElementName = "Timestamp", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
        public Timestamp Timestamp { get; set; }
        [XmlAttribute(AttributeName = "mustUnderstand", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public string MustUnderstand { get; set; }
        [XmlAttribute(AttributeName = "o", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string O { get; set; }
    }

    [XmlRoot(ElementName = "Header", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    public class Header
    {
        [XmlElement(ElementName = "Action", Namespace = "http://www.w3.org/2005/08/addressing")]
        public Action Action { get; set; }
        [XmlElement(ElementName = "RelatesTo", Namespace = "http://www.w3.org/2005/08/addressing")]
        public string RelatesTo { get; set; }
        [XmlElement(ElementName = "Security", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
        public Security Security { get; set; }
    }

    [XmlRoot(ElementName = "Deployment", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
    public class Deployment
    {
        [XmlElement(ElementName = "ID", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string ID { get; set; }
        [XmlElement(ElementName = "Action", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string Action { get; set; }
        [XmlElement(ElementName = "IsAssigned", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string IsAssigned { get; set; }
        [XmlElement(ElementName = "LastChangeTime", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string LastChangeTime { get; set; }
        [XmlElement(ElementName = "AutoSelect", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string AutoSelect { get; set; }
        [XmlElement(ElementName = "AutoDownload", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string AutoDownload { get; set; }
        [XmlElement(ElementName = "SupersedenceBehavior", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string SupersedenceBehavior { get; set; }
        [XmlElement(ElementName = "Priority", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string Priority { get; set; }
        [XmlElement(ElementName = "HandlerSpecificAction", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string HandlerSpecificAction { get; set; }
        [XmlElement(ElementName = "FlightId", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string FlightId { get; set; }
        [XmlElement(ElementName = "FlightMetadata", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string FlightMetadata { get; set; }
    }

    [XmlRoot(ElementName = "Verification", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
    public class Verification
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

    [XmlRoot(ElementName = "UpdateInfo", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
    public class UpdateInfo
    {
        [XmlElement(ElementName = "ID", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string ID { get; set; }
        [XmlElement(ElementName = "Deployment", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public Deployment Deployment { get; set; }
        [XmlElement(ElementName = "IsLeaf", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string IsLeaf { get; set; }
        [XmlElement(ElementName = "IsShared", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string IsShared { get; set; }
        [XmlElement(ElementName = "Xml", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string Xml { get; set; }
        [XmlElement(ElementName = "Verification", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public Verification Verification { get; set; }
    }

    [XmlRoot(ElementName = "NewUpdates", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
    public class NewUpdates
    {
        [XmlElement(ElementName = "UpdateInfo", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public List<UpdateInfo> UpdateInfo { get; set; }
    }

    [XmlRoot(ElementName = "NewCookie", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
    public class NewCookie
    {
        [XmlElement(ElementName = "Expiration", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string Expiration { get; set; }
        [XmlElement(ElementName = "EncryptedData", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string EncryptedData { get; set; }
    }

    [XmlRoot(ElementName = "Update", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
    public class Update
    {
        [XmlElement(ElementName = "ID", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string ID { get; set; }
        [XmlElement(ElementName = "Xml", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string Xml { get; set; }
        [XmlElement(ElementName = "Verification", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public Verification Verification { get; set; }
    }

    [XmlRoot(ElementName = "Updates", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
    public class Updates
    {
        [XmlElement(ElementName = "Update", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public List<Update> Update { get; set; }
    }

    [XmlRoot(ElementName = "ExtendedUpdateInfo", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
    public class ExtendedUpdateInfo
    {
        [XmlElement(ElementName = "Updates", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public Updates Updates { get; set; }
    }

    [XmlRoot(ElementName = "SyncUpdatesResult", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
    public class SyncUpdatesResult
    {
        [XmlElement(ElementName = "NewUpdates", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public NewUpdates NewUpdates { get; set; }
        [XmlElement(ElementName = "Truncated", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string Truncated { get; set; }
        [XmlElement(ElementName = "NewCookie", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public NewCookie NewCookie { get; set; }
        [XmlElement(ElementName = "DriverSyncNotNeeded", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string DriverSyncNotNeeded { get; set; }
        [XmlElement(ElementName = "ExtendedUpdateInfo", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public ExtendedUpdateInfo ExtendedUpdateInfo { get; set; }
        [XmlElement(ElementName = "CountryCode", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string CountryCode { get; set; }
    }

    [XmlRoot(ElementName = "SyncUpdatesResponse", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
    public class SyncUpdatesResponse
    {
        [XmlElement(ElementName = "SyncUpdatesResult", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public SyncUpdatesResult SyncUpdatesResult { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "Body", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    public class Body
    {
        [XmlElement(ElementName = "SyncUpdatesResponse", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public SyncUpdatesResponse SyncUpdatesResponse { get; set; }
        [XmlElement(ElementName = "GetExtendedUpdateInfo2Response", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public GetExtendedUpdateInfo2Response GetExtendedUpdateInfo2Response { get; set; }
        [XmlElement(ElementName = "GetCookieResponse", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public GetCookieResponse GetCookieResponse { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsd { get; set; }
    }

    [XmlRoot(ElementName = "GetCookieResult", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
    public class GetCookieResult
    {
        [XmlElement(ElementName = "Expiration", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string Expiration { get; set; }
        [XmlElement(ElementName = "EncryptedData", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string EncryptedData { get; set; }
    }

    [XmlRoot(ElementName = "GetCookieResponse", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
    public class GetCookieResponse
    {
        [XmlElement(ElementName = "GetCookieResult", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public GetCookieResult GetCookieResult { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "Envelope", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    public class Envelope
    {
        [XmlElement(ElementName = "Header", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public Header Header { get; set; }
        [XmlElement(ElementName = "Body", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public Body Body { get; set; }
        [XmlAttribute(AttributeName = "s", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string S { get; set; }
        [XmlAttribute(AttributeName = "a", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string A { get; set; }
        [XmlAttribute(AttributeName = "u", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string U { get; set; }
    }




    //
    //  ================================
    //  \\SECURED FILELOCATION SECTION\\
    //  ================================
    //

    [XmlRoot(ElementName = "FileLocation", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
    public class FileLocation
    {
        [XmlElement(ElementName = "FileDigest", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string FileDigest { get; set; }
        [XmlElement(ElementName = "Url", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string Url { get; set; }
        [XmlElement(ElementName = "EsrpDecryptionInformation", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public string EsrpDecryptionInformation { get; set; }
    }

    [XmlRoot(ElementName = "FileLocations", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
    public class FileLocations
    {
        [XmlElement(ElementName = "FileLocation", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public List<FileLocation> FileLocation { get; set; }
    }

    [XmlRoot(ElementName = "GetExtendedUpdateInfo2Result", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
    public class GetExtendedUpdateInfo2Result
    {
        [XmlElement(ElementName = "FileLocations", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public FileLocations FileLocations { get; set; }
    }

    [XmlRoot(ElementName = "GetExtendedUpdateInfo2Response", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
    public class GetExtendedUpdateInfo2Response
    {
        [XmlElement(ElementName = "GetExtendedUpdateInfo2Result", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public GetExtendedUpdateInfo2Result GetExtendedUpdateInfo2Result { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }




    //
    //  =============================
    //  \\FRAGMENT FILES FROM <XML>\\
    //  =============================
    //

    [XmlRoot(ElementName = "InstallationBehavior")]
    public class InstallationBehavior
    {
        [XmlAttribute(AttributeName = "RebootBehavior")]
        public string RebootBehavior { get; set; }
    }

    [XmlRoot(ElementName = "ExtendedProperties")]
    public class ExtendedProperties
    {
        [XmlElement(ElementName = "InstallationBehavior")]
        public InstallationBehavior InstallationBehavior { get; set; }
        [XmlAttribute(AttributeName = "ProductName")]
        public string ProductName { get; set; }
        [XmlAttribute(AttributeName = "ReleaseVersion")]
        public string ReleaseVersion { get; set; }
        [XmlAttribute(AttributeName = "Handler")]
        public string Handler { get; set; }
        [XmlAttribute(AttributeName = "MaxDownloadSize")]
        public string MaxDownloadSize { get; set; }
        [XmlAttribute(AttributeName = "MinDownloadSize")]
        public string MinDownloadSize { get; set; }
        [XmlAttribute(AttributeName = "DefaultPropertiesLanguage")]
        public string DefaultPropertiesLanguage { get; set; }
        [XmlAttribute(AttributeName = "ContentType")]
        public string ContentType { get; set; }
        [XmlAttribute(AttributeName = "AutoSelectOnWebsites")]
        public string AutoSelectOnWebsites { get; set; }
        [XmlAttribute(AttributeName = "BrowseOnly")]
        public string BrowseOnly { get; set; }
    }

    [XmlRoot(ElementName = "AdditionalDigest")]
    public class AdditionalDigest
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "File")]
    public class FileF
    {
        [XmlElement(ElementName = "AdditionalDigest")]
        public AdditionalDigest AdditionalDigest { get; set; }
        [XmlAttribute(AttributeName = "FileName")]
        public string FileName { get; set; }
        [XmlAttribute(AttributeName = "Digest")]
        public string Digest { get; set; }
        [XmlAttribute(AttributeName = "DigestAlgorithm")]
        public string DigestAlgorithm { get; set; }
        [XmlAttribute(AttributeName = "Size")]
        public string Size { get; set; }
        [XmlAttribute(AttributeName = "Modified")]
        public string Modified { get; set; }
        [XmlAttribute(AttributeName = "PatchingType")]
        public string PatchingType { get; set; }
    }

    [XmlRoot(ElementName = "Files")]
    public class Files
    {
        [XmlElement(ElementName = "File")]
        public List<FileF> File { get; set; }
    }

    [XmlRoot(ElementName = "OldManYellsAtParcel")]
    public class OldManYellsAtParcel
    {
        [XmlElement(ElementName = "ExtendedProperties")]
        public ExtendedProperties ExtendedProperties { get; set; }
        [XmlElement(ElementName = "Files")]
        public Files Files { get; set; }
    }

    [XmlRoot(ElementName = "LocalizedProperties")]
    public class LocalizedProperties
    {
        [XmlElement(ElementName = "Language")]
        public string Language { get; set; }
        [XmlElement(ElementName = "Title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
    }

}
