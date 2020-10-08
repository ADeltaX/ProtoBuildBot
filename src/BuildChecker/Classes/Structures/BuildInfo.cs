using System;

namespace BuildChecker
{
    public class BuildInfo
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string DeviceFamily { get; set; }
        public string Build { get; set; }
        public string Ring { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string BuildID { get; set; }
        public string UpdateID { get; set; }
        public string ReleaseType { get; set; }
        public string Architecture { get; set; }
        public string BuildLong { get; set; }
        public string LastTimeChanged { get; set; }
        public string FlightID { get; set; }
        public string RevisionNumber { get; set; }
    }
}
