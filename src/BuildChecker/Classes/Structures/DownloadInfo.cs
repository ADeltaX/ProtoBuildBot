using System.Runtime.Serialization;

namespace BuildChecker
{
    [DataContract]
    public class FileRequests
    {
        [IgnoreDataMember]
        public bool IsUpdateIDIgnored { get; set; }

        [IgnoreDataMember]
        public string Title { get; set; }

        [IgnoreDataMember]
        public string Description { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string UpdateID { get; set; }

        [IgnoreDataMember]
        public string LastTimeChange { get; set; }

        [IgnoreDataMember]
        public string FlightID { get; set; }

        [IgnoreDataMember]
        public string RevisionNum { get; set; }

        [DataMember]
        public DownloadInfo[] DownloadInfo { get; set; }
    }

    [DataContract]
    public class DownloadInfo
    {
        [DataMember]
        public string SHA1Hash { get; set; }

        [DataMember]
        public string SHA256Hash { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public long Size { get; set; }

        [IgnoreDataMember]
        public bool Installable { get; set; }

        [IgnoreDataMember]
        public string ContentType { get; set; }

        [IgnoreDataMember]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string EsrpDecryptionInformation { get; set; }
    }

    //With URL

    [DataContract]
    public class FileRequestsWithURL
    {
        [DataMember(Name = "UpdateID", EmitDefaultValue = false)]
        public string UpdateID { get; set; }

        [DataMember(Name = "BuildLabExtended", EmitDefaultValue = false)]
        public string BuildLong { get; set; }

        [DataMember(Name = "DeviceFamily", EmitDefaultValue = false)]
        public string DeviceFamily { get; set; }

        [DataMember(Name = "Architecture", EmitDefaultValue = false)]
        public string Architecture { get; set; }

        [DataMember(Name = "ReleaseType", EmitDefaultValue = false)]
        public string ReleaseType { get; set; }

        [DataMember(Name = "FlightID", EmitDefaultValue = false)]
        public string FlightID { get; set; }

        [DataMember(Name = "RevisionNum")]
        public string RevisionNum { get; set; }

        [DataMember(Name = "DownloadInfo")]
        public DownloadInfoWithURL[] DownloadInfo { get; set; }
    }

    [DataContract]
    public class DownloadInfoWithURL
    {
        [DataMember(Name = "SHA1Hash")]
        public string SHA1Hash { get; set; }

        [DataMember(Name = "SHA256Hash")]
        public string SHA256Hash { get; set; }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "Size")]
        public long Size { get; set; }

        [IgnoreDataMember]
        public bool Installable { get; set; }

        [IgnoreDataMember]
        public string ContentType { get; set; }

        [DataMember(Name = "Url")]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string EsrpDecryptionInformation { get; set; }
    }
}
