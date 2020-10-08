namespace BuildChecker.Interfaces
{
    public interface IChecker
    {
        bool DownloadCabs(DownloadInfo[] array, string path);
        FileRequests FetchBuild(bool updateAgentOnly, string ignoreUpdateID = null);
        BuildInfo ReadBuildVersion(FileRequests array, bool updateAgentOnly);
        bool ExportCSV(DownloadInfo[] array, string filenamepath);
        bool ExportJSON(FileRequests fileReq, string filenamepath, bool includeLinks);
    }
}
