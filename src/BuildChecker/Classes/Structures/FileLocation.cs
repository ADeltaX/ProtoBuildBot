namespace BuildChecker
{
    public partial class LegacyUUP
    {
        public class FileLocation
        {
            public string FileName { get; set; }
            public long Size { get; set; }
            public string Language { get; set; }
            public string Digest { get; set; }
            public string ContentType { get; set; }
            public string Url { get; set; }
        }
    }
}
