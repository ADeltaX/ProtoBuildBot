using BuildChecker.Classes.Extractors;
using BuildChecker.Classes.Helpers;
using BuildChecker.Classes.Structures;
using BuildChecker.Interfaces;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BuildChecker.Classes.DeviceCheckers
{
    public abstract class BaseChecker : IChecker
    {
        protected abstract string DeviceFamily { get; }
        public string Branch { get; set; }
        public string Build { get; set; }
        public string Arch { get; set; }
        public string Flight { get; set; }
        public string Ring { get; set; }

        protected UUP uup;

        private readonly ICabExtractor cabExtractor;

        public BaseChecker(string branch, string build, string arch, string flight, string ring, UUP newUUP)
        {
            Branch = branch;
            Build = build;
            Arch = arch;
            Flight = flight;
            Ring = ring;

            uup = newUUP ?? new UUP();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                cabExtractor = new WinCabinetExtractor();
            else
                cabExtractor = new SevenZipExtractor();
        }

        public virtual bool DownloadCabs(DownloadInfo[] array, string path)
        {
            HttpDownloader httpDownloader = new HttpDownloader(path);

            bool success = true;
            // Serial execution because of the file refresh
            Parallel.ForEach(array, new ParallelOptions { MaxDegreeOfParallelism = 16 }, dw =>
            {
                Console.WriteLine("[DW] " + dw.Name); //TODO: replace this
                try
                {
                    httpDownloader.DownloadAsync(dw.Url, Path.Combine(path, dw.Name), true, dw.EsrpDecryptionInformation).GetAwaiter().GetResult();
#if false
                    FileInfo info = new FileInfo(path);
                    if (info.Length != dw.Size) 
                    {
                        Console.WriteLine("[!] : Refreshing download links.");
                        array = newUUP.RefreshFiles().GetAwaiter().GetResult().DownloadInfo;
                        File.Delete(Path.Combine(path, dw.Name));
                        httpDownloader.Download(dw.Url, Path.Combine(path, dw.Name), true, dw.EsrpDecryptionInformation);
                    }
#endif
                }
                catch (Exception)
                {
                    success = false;
                }
            });

            return success;
        }

        public bool ExportCSV(DownloadInfo[] array, string filenamepath)
        {
            bool success = true;

            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("Package name, Size (bytes), Url, SHA1, SHA256,\n");
                for (int i = 0; i < array.Length; i++)
                {
                    sb.Append(array[i].Name + ",");
                    sb.Append(array[i].Size + ",");
                    sb.Append(array[i].Url + ",");
                    sb.Append(array[i].SHA1Hash + ",");
                    sb.Append(array[i].SHA256Hash + ",");

                    if (array[i].EsrpDecryptionInformation != null)
                        sb.Append(array[i].EsrpDecryptionInformation + ",");

                    sb.Append("\n");
                }
                File.WriteAllText(filenamepath, sb.ToString());

                Console.WriteLine("[OK] Links exported!"); //TODO: Remove this
            }
            catch (Exception)
            {
                success = false;
            }

            return success;
        }

        public bool ExportJSON(FileRequests fileReq, string filenamepath, bool includeLinks)
        {
            bool success = true;

            try
            {
                MemoryStream stream = new MemoryStream();
                DataContractJsonSerializer ser;

                //TODO: Add "remove links" support

                ser = new DataContractJsonSerializer(typeof(FileRequests));
                ser.WriteObject(stream, fileReq);

                stream.Seek(0, SeekOrigin.Begin);
                Compress(stream, filenamepath);
            }
            catch (Exception)
            {
                success = false;
            }

            return success;
        }

        public abstract FileRequests FetchBuild(bool updateAgentOnly, string ignoreUpdateID = null);

        public static void Compress(Stream input, string filenamepath)
        {
            using FileStream fs = new FileStream(filenamepath + ".gz", FileMode.Create, FileAccess.ReadWrite);
            using var output = new GZipStream(fs, CompressionMode.Compress);
            Write(input, output);
        }

        public static MemoryStream DecompressFileInMemory(string path)
        {
            MemoryStream str = new MemoryStream();

            string fileIn = path;
            using (var input = new GZipStream(File.OpenRead(fileIn), CompressionMode.Decompress))
                Write(input, str);

            str.Seek(0, SeekOrigin.Begin);

            return str;
        }

        static void Write(Stream input, Stream output, int bufferSize = 10 * 1024 * 1024)
        {
            var buffer = new byte[bufferSize];
            for (int readCount; (readCount = input.Read(buffer, 0, buffer.Length)) > 0;)
                output.Write(buffer, 0, readCount);
        }

        public virtual BuildInfo ReadBuildVersion(FileRequests fileRequests, bool updateAgentOnly)
        {
            var tmpPath = Path.Combine(Directory.GetCurrentDirectory(), "tmp", Guid.NewGuid().ToString("n").Substring(0, 8));
            BuildInfo buildInfo = GetBuildInfoFromUpdateAgent(fileRequests, updateAgentOnly, tmpPath);

            if (updateAgentOnly)
                return buildInfo;

            var compDB = GetCompDB(fileRequests, tmpPath);

            if (compDB != null)
            {
                var build = compDB.BuildInfo;
                var crDate = compDB.CreatedDate;
                var buildID = compDB.BuildID;
                var relType = compDB.ReleaseType;

                buildInfo.BuildID = buildID;
                buildInfo.CreatedDate = (crDate != null) ? DateTime.Parse(crDate) : (DateTime?)null;
                buildInfo.ReleaseType = relType;
                buildInfo.Build = build;
                buildInfo.UpdateID = fileRequests.UpdateID;
                buildInfo.LastTimeChanged = fileRequests.LastTimeChange;
                buildInfo.RevisionNumber = fileRequests.RevisionNum;
            }
            else
            {
                return null;
            }

            return buildInfo;
        }

        public BuildInfo GetBuildInfoFromUpdateAgent(FileRequests fileRequests, bool updateAgentOnly, string tmpPath)
        {
            const string updateAgentCabName = "upd.cab";

            BuildInfo buildInfo = null;
            DownloadInfo dwUpdateAgent = null;

            try
            {
                dwUpdateAgent = fileRequests.DownloadInfo.ToList().FirstOrDefault(deployment => deployment.ContentType == "ServicingStack");

                if (dwUpdateAgent != null)
                {
                    HttpDownloader httpDownloader = new HttpDownloader(tmpPath);
                    httpDownloader.DownloadAsync(dwUpdateAgent.Url, updateAgentCabName, false, dwUpdateAgent.EsrpDecryptionInformation).GetAwaiter().GetResult();

                    cabExtractor.Extract(Path.Combine(tmpPath, updateAgentCabName), Path.Combine(tmpPath, "tmpUPDCAB"), "UpdateAgent.dll");

                    var updateAgentFile = Path.Combine(tmpPath, "tmpUPDCAB", "UpdateAgent.dll");

                    if (File.Exists(updateAgentFile))
                    {
                        buildInfo = new BuildInfo
                        {
                            BuildLong = GetBuildStringFromUpdateAgent(File.ReadAllBytes(updateAgentFile)),
                            Architecture = Arch.ToUpper(),
                            Ring = Ring,
                            DeviceFamily = DeviceFamily,
                            UpdateID = fileRequests.UpdateID,
                            LastTimeChanged = fileRequests.LastTimeChange,
                            FlightID = fileRequests.FlightID,
                            Title = fileRequests.Title,
                            Description = fileRequests.Description
                        };

                        File.Delete(updateAgentFile);
                        File.Delete(Path.Combine(tmpPath, updateAgentCabName));
                    }                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
            
            return buildInfo;
        }

        private CompDB GetCompDB(FileRequests fileRequests, string tmpPath)
        {
            const string aggregatedMetadataCabName = "aggMeta.cab";
            const string metadataCabName = "meta.cab";

            string unparsedCompDB = null;

            try
            {
                var metadataRequest = fileRequests.DownloadInfo.Where(compdbDownloadInfo => compdbDownloadInfo.ContentType == "Metadata").ToList();

                if (metadataRequest.Count > 0)
                {
                    HttpDownloader httpDownloader = new HttpDownloader(tmpPath);

                    var aggregatedMetadata = metadataRequest.FirstOrDefault(meta => meta.Name.ToLower() == (fileRequests.UpdateID + ".AggregatedMetadata.cab").ToLower());
                    if (aggregatedMetadata != null)
                    {
                        httpDownloader.DownloadAsync(aggregatedMetadata.Url, aggregatedMetadataCabName, false, aggregatedMetadata.EsrpDecryptionInformation).GetAwaiter().GetResult();
                        unparsedCompDB = ExtractCabAndRead(tmpPath, Path.Combine(tmpPath, aggregatedMetadataCabName), aggregatedMetadataCabName, true);
                    }
                    else
                    {
                        var metadataFilter = metadataRequest.SingleOrDefault(meta =>
                        meta.Name.EndsWith("neutral.xml.cab", StringComparison.InvariantCultureIgnoreCase) ||
                        meta.Name.EndsWith("targetcompdb.xml.cab", StringComparison.InvariantCultureIgnoreCase));

                        if (metadataFilter != null)
                        {
                            httpDownloader.DownloadAsync(metadataFilter.Url, metadataCabName, false, metadataFilter.EsrpDecryptionInformation).GetAwaiter().GetResult();
                            unparsedCompDB = ExtractCabAndRead(tmpPath, Path.Combine(tmpPath, metadataCabName), metadataCabName, false);
                        }
                        else
                        {
                            Console.WriteLine("[ERROR] Can't find metadata");
                            return null;
                        }
                    }

                    if (Directory.Exists(tmpPath))
                        Directory.Delete(tmpPath, true);
                }

                if (unparsedCompDB == null)
                    return null;

                CompDB compDB = null;

                XmlSerializer ser = new XmlSerializer(typeof(CompDB));
                using TextReader reader = new StringReader(unparsedCompDB);
                compDB = (CompDB)ser.Deserialize(reader);

                return compDB;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] In GetCompDB: " + ex.Message);
                return null;
            }
        }

        private string ExtractCabAndRead(string tmpPath, string filenamepath, string cabName, bool isAggregated)
        {
            if (isAggregated)
            {
                var tmpCabExtractionPath = Path.Combine(tmpPath, "tmpCAB");

                cabExtractor.Extract(filenamepath, Path.Combine(tmpPath, "tmpCAB"), "*TargetCompDB_Neutral.xml.cab");

                if (File.Exists(Path.Combine(tmpPath, cabName)))
                    File.Delete(Path.Combine(tmpPath, cabName));

                if (Directory.Exists(tmpCabExtractionPath))
                {
                    var files = Directory.GetFiles(tmpCabExtractionPath);
                    if (files.Length > 0)
                    {
                        cabExtractor.Extract(files[0], tmpPath, "");
                        Directory.Delete(tmpCabExtractionPath, true);
                    }
                }
            }
            else
            {
                cabExtractor.Extract(filenamepath, tmpPath, "");
                if (File.Exists(Path.Combine(tmpPath, cabName)))
                    File.Delete(Path.Combine(tmpPath, cabName));
            }

            var xmlFiles = Directory.GetFiles(tmpPath, "*.xml");
            if (xmlFiles.Length == 0)
                return null;

            using var fileStream = File.OpenRead(xmlFiles[0]);
            using var strReader = new StreamReader(fileStream);

            return strReader.ReadToEnd();
        }

        public static string GetBuildStringFromUpdateAgent(byte[] updateAgentFile)
        {
            byte[] sign = new byte[] {
                0x46, 0x00, 0x69, 0x00, 0x6c, 0x00, 0x65, 0x00, 0x56, 0x00, 0x65, 0x00, 0x72,
                0x00, 0x73, 0x00, 0x69, 0x00, 0x6f, 0x00, 0x6e, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            var fIndex = IndexOf(updateAgentFile, sign) + sign.Length;
            var lIndex = IndexOf(updateAgentFile, new byte[] { 0x00, 0x00, 0x00 }, fIndex) + 1;

            var sliced = SliceByteArray(updateAgentFile, lIndex - fIndex, fIndex);

            return Encoding.Unicode.GetString(sliced);
        }

        static byte[] SliceByteArray(byte[] source, int length, int offset)
        {
            byte[] destfoo = new byte[length];
            Array.Copy(source, offset, destfoo, 0, length);
            return destfoo;
        }

        private static int IndexOf(byte[] searchIn, byte[] searchFor, int offset = 0)
        {
            if ((searchIn != null) && (searchIn != null))
            {
                if (searchFor.Length > searchIn.Length) return 0;
                for (int i = offset; i < searchIn.Length; i++)
                {
                    int startIndex = i;
                    bool match = true;
                    for (int j = 0; j < searchFor.Length; j++)
                    {
                        if (searchIn[startIndex] != searchFor[j])
                        {
                            match = false;
                            break;
                        }
                        else if (startIndex < searchIn.Length)
                        {
                            startIndex++;
                        }

                    }
                    if (match)
                        return startIndex - searchFor.Length;
                }
            }
            return -1;
        }
    }
}
