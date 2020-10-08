using BuildChecker.Interfaces;
using BuildChecker.Classes.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Collections;

namespace BuildChecker.Classes.DeviceBuilderExtensions
{
    public abstract class BuilderExtension : IBuilder
    {
        protected string FetchUUP_String(string guid, string uupDevice, string uupEncryptedData, string created, string expires, string callerAttrib, string deviceAttributes, string products, bool syncCurrentVersion = false) => $"<s:Envelope xmlns:a=\"http://www.w3.org/2005/08/addressing\" xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\"><s:Header><a:Action s:mustUnderstand=\"1\">http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService/SyncUpdates</a:Action><a:MessageID>urn:uuid:{guid}</a:MessageID><a:To s:mustUnderstand=\"1\">https://fe3.delivery.mp.microsoft.com/ClientWebService/client.asmx</a:To><o:Security s:mustUnderstand=\"1\" xmlns:o=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\"><Timestamp xmlns=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\"><Created>{created}</Created><Expires>{expires}</Expires></Timestamp><wuws:WindowsUpdateTicketsToken wsu:id=\"ClientMSA\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" xmlns:wuws=\"http://schemas.microsoft.com/msus/2014/10/WindowsUpdateAuthorization\"><TicketType Name=\"MSA\" Version=\"1.0\" Policy=\"MBI_SSL\"></TicketType></wuws:WindowsUpdateTicketsToken></o:Security></s:Header><s:Body><SyncUpdates xmlns=\"http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService\"><cookie><Expiration>2046-02-18T21:29:10Z</Expiration><EncryptedData>{uupEncryptedData}</EncryptedData></cookie><parameters><ExpressQuery>false</ExpressQuery><InstalledNonLeafUpdateIDs><int>1</int><int>2</int><int>3</int><int>10</int><int>11</int><int>17</int><int>19</int><int>2359974</int><int>2359977</int><int>5143990</int><int>5169043</int><int>5169044</int><int>5169047</int><int>8788830</int><int>8806526</int><int>9125350</int><int>9154769</int><int>10809856</int><int>23110993</int><int>23110994</int><int>23110995</int><int>23110996</int><int>23110999</int><int>23111000</int><int>23111001</int><int>23111002</int><int>23111003</int><int>23111004</int><int>24513870</int><int>28880263</int><int>30077688</int><int>30486944</int><int>59830006</int><int>59830007</int><int>59830008</int><int>60484010</int><int>62450018</int><int>62450019</int><int>62450020</int><int>98959022</int><int>98959023</int><int>98959024</int><int>98959025</int><int>98959026</int><int>105939029</int><int>105995585</int><int>106017178</int><int>107825194</int><int>117765322</int><int>129905029</int><int>130040030</int><int>130040031</int><int>130040032</int><int>130040033</int><int>133399034</int><int>138372035</int><int>138372036</int><int>139536037</int><int>139536038</int><int>139536039</int><int>139536040</int><int>142045136</int><int>158941041</int><int>158941042</int><int>158941043</int><int>158941044</int><int>159776047</int><int>160733048</int><int>160733049</int><int>160733050</int><int>160733051</int><int>160733055</int><int>160733056</int><int>161870057</int><int>161870058</int><int>161870059</int></InstalledNonLeafUpdateIDs><OtherCachedUpdateIDs></OtherCachedUpdateIDs><SkipSoftwareSync>false</SkipSoftwareSync><NeedTwoGroupOutOfScopeUpdates>true</NeedTwoGroupOutOfScopeUpdates><AlsoPerformRegularSync>true</AlsoPerformRegularSync><ComputerSpec/><ExtendedUpdateInfoParameters><XmlUpdateFragmentTypes><XmlUpdateFragmentType>Extended</XmlUpdateFragmentType><XmlUpdateFragmentType>LocalizedProperties</XmlUpdateFragmentType><XmlUpdateFragmentType>Eula</XmlUpdateFragmentType></XmlUpdateFragmentTypes><Locales><string>en-US</string></Locales></ExtendedUpdateInfoParameters><ClientPreferredLanguages></ClientPreferredLanguages><ProductsParameters><SyncCurrentVersionOnly>{syncCurrentVersion.ToString().ToLower()}</SyncCurrentVersionOnly><DeviceAttributes>{deviceAttributes}</DeviceAttributes><CallerAttributes>{callerAttrib}</CallerAttributes><Products>{products}</Products></ProductsParameters></parameters></SyncUpdates></s:Body></s:Envelope>";
        protected string GetFiles_String(string guid, string uupDevice, string created, string expires, string uuid, string rev, string deviceAttributes) => $"<s:Envelope xmlns:a=\"http://www.w3.org/2005/08/addressing\" xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\"><s:Header><a:Action s:mustUnderstand=\"1\">http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService/GetExtendedUpdateInfo2</a:Action><a:MessageID>urn:uuid:{guid}</a:MessageID><a:To s:mustUnderstand=\"1\">https://fe3.delivery.mp.microsoft.com/ClientWebService/client.asmx/secured</a:To><o:Security s:mustUnderstand=\"1\" xmlns:o=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\"><Timestamp xmlns=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\"><Created>{created}</Created><Expires>{expires}</Expires></Timestamp><wuws:WindowsUpdateTicketsToken wsu:id=\"ClientMSA\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" xmlns:wuws=\"http://schemas.microsoft.com/msus/2014/10/WindowsUpdateAuthorization\"><TicketType Name=\"MSA\" Version=\"1.0\" Policy=\"MBI_SSL\"><Device>{uupDevice}</Device></TicketType></wuws:WindowsUpdateTicketsToken></o:Security></s:Header><s:Body><GetExtendedUpdateInfo2 xmlns=\"http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService\"><updateIDs><UpdateIdentity><UpdateID>{uuid}</UpdateID><RevisionNumber>{rev}</RevisionNumber></UpdateIdentity></updateIDs><infoTypes><XmlUpdateFragmentType>FileUrl</XmlUpdateFragmentType><XmlUpdateFragmentType>FileDecryption</XmlUpdateFragmentType><XmlUpdateFragmentType>EsrpDecryptionInformation</XmlUpdateFragmentType><XmlUpdateFragmentType>PiecesHashUrl</XmlUpdateFragmentType><XmlUpdateFragmentType>BlockMapUrl</XmlUpdateFragmentType></infoTypes><deviceAttributes>{deviceAttributes}</deviceAttributes></GetExtendedUpdateInfo2></s:Body></s:Envelope>";

        protected string _uupDevice;
        protected string _expCookie;

        public string Branch { get; set; }
        public string Build { get; set; }
        public string Arch { get; set; }
        public string Flight { get; set; }
        public string Ring { get; set; }
        public string Sku { get; set; }

        protected BuilderExtension(string branch, string build, string arch, string flight, string ring, string sku)
        {
            Branch = branch;
            Build = build;
            Arch = arch;
            Flight = flight;
            Ring = ring;
            Sku = sku;
        }

        public virtual DownloadInfo[] ExtractDownloadInfo(UpdateIdentity updateInfo, SyncUpdatesResponse requestEnv, GetExtendedUpdateInfo2Response filesEnv, bool updateAgentOnly = false)
        {
            try
            {
                List<DownloadInfo> downloadInfos = new List<DownloadInfo>();
                Update extendedUpdate = null;
                foreach (var extUpdate in requestEnv.SyncUpdatesResult.ExtendedUpdateInfo.Updates.Update)
                {
                    if (extUpdate.ID == updateInfo.ID && extUpdate.Xml.StartsWith("<ExtendedProperties"))
                    {
                        extendedUpdate = extUpdate;
                        break;
                    }
                }

                Files files = DeserializeFragmentFiles(extendedUpdate.Xml);

                if (updateAgentOnly)
                {
                    //Seek for UpdateAgent
                    var updateAgentFile = files.File.AsParallel().Single(f => f.PatchingType == "ServicingStack");
                    var updateAgentLocation = filesEnv.GetExtendedUpdateInfo2Result.FileLocations.FileLocation.AsParallel().Single(f => f.FileDigest == updateAgentFile.Digest);

                    downloadInfos.Add(new DownloadInfo
                    {
                        ContentType = updateAgentFile.PatchingType,
                        Installable = true,
                        Name = updateAgentFile.FileName,
                        Size = long.Parse(updateAgentFile.Size),
                        SHA1Hash = updateAgentFile.Digest,
                        Url = updateAgentLocation.Url,
                        EsrpDecryptionInformation = updateAgentLocation.EsrpDecryptionInformation,
                        SHA256Hash = updateAgentFile.AdditionalDigest.Text
                    });
                    downloadInfos.Sort(delegate (DownloadInfo file1, DownloadInfo file2)
                    {
                        return file1.Size.CompareTo(file2.Size);
                    });
                    return downloadInfos.ToArray();
                }
                else
                {
                    if (filesEnv.GetExtendedUpdateInfo2Result.FileLocations == null)
                        throw new Exception("Check deviceAttributes of both.");

                    Stopwatch stopwatch = Stopwatch.StartNew();

                    //Build hashtable Digest-FileLocation
                    Hashtable htDigestFileLocation = new Hashtable(filesEnv.GetExtendedUpdateInfo2Result.FileLocations.FileLocation.Count + 1);

                    //Build hashtable Digest-File
                    Hashtable htDigestFile = new Hashtable(files.File.Count + 1);

                    DownloadInfo[] dw = new DownloadInfo[files.File.Count];

                    filesEnv.GetExtendedUpdateInfo2Result.FileLocations.FileLocation.ForEach(FL => htDigestFileLocation.Add(FL.FileDigest, FL));
                    files.File.ForEach(FF => htDigestFile.Add(FF.Digest, FF));


                    int i = 0;
                    foreach (DictionaryEntry item in htDigestFileLocation)
                    {
                        var fileTmp = (FileF)htDigestFile[item.Key];
                        var fileLocationTmp = (item.Value as FileLocation);

                        var dwTmp = new DownloadInfo
                        {
                            ContentType = fileTmp.PatchingType,
                            Installable = true,
                            Name = fileTmp.FileName,
                            Size = long.Parse(fileTmp.Size),
                            SHA1Hash = (string)item.Key,
                            Url = fileLocationTmp.Url,
                            EsrpDecryptionInformation = fileLocationTmp.EsrpDecryptionInformation,
                            SHA256Hash = fileTmp.AdditionalDigest.Text
                        };

                        dw[i++] = dwTmp;
                    }

                    stopwatch.Stop();

                    Console.WriteLine($"Paired in: {stopwatch.Elapsed}ms");

                    Array.Sort(dw, (file1, file2) => file1.Size.CompareTo(file2.Size));
                    return dw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Internal server error.");
                return null;
            }
        }

        public virtual UpdateIdentity ExtractUpdateIdentity(Envelope cont)
        {
            if (!(cont.Body.SyncUpdatesResponse?.SyncUpdatesResult?.NewUpdates?.UpdateInfo != null)) return null;

            UpdateIdentity srlzd = null;

            foreach (var update in cont.Body.SyncUpdatesResponse.SyncUpdatesResult.NewUpdates.UpdateInfo)
            {
                if (update.Deployment.Action == "Install")
                {
                    var serIden = new XmlSerializer(typeof(UpdateIdentity));
                    using var strReader = new StringReader(update.Xml);
                    using var reader = XmlReader.Create(strReader, new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment });

                    srlzd = (UpdateIdentity)serIden.Deserialize(reader);
                    srlzd.LastChangeTime = update.Deployment.LastChangeTime;
                    srlzd.ID = update.ID;
                    srlzd.FlightID = update.Deployment.FlightId;
                    break;
                }
            }

            if (srlzd != null)
            {
                foreach (var extUpdate in cont.Body.SyncUpdatesResponse.SyncUpdatesResult.ExtendedUpdateInfo.Updates.Update)
                {
                    if (extUpdate.ID == srlzd.ID && extUpdate.Xml.StartsWith("<LocalizedProperties"))
                    {
                        srlzd.LocalizedProperties = DeserializeLocalizedProperties(extUpdate.Xml);
                        break;
                    }
                }
            }

            return srlzd;
        }

        private static LocalizedProperties DeserializeLocalizedProperties(string content)
        {
            var serFiles = new XmlSerializer(typeof(LocalizedProperties));
            using var strReader = new StringReader(content);
            using XmlReader reader = XmlReader.Create(strReader, new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment });
            return (LocalizedProperties)serFiles.Deserialize(reader);
        }

        private static Files DeserializeFragmentFiles(string content)
        {
            var serFiles = new XmlSerializer(typeof(OldManYellsAtParcel));
            using var strReader = new StringReader("<OldManYellsAtParcel>" + content + "</OldManYellsAtParcel>");
            using XmlReader reader = XmlReader.Create(strReader, new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment });
            return ((OldManYellsAtParcel)serFiles.Deserialize(reader)).Files;
        }

        public virtual string BuildFetchUUPRequest(string uupEncryptedData)
        {
            var guid = Guid.NewGuid().ToString();
            var created = XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Local);
            var expires = XmlConvert.ToString(DateTime.Now + TimeSpan.FromSeconds(120), XmlDateTimeSerializationMode.Local);

            var products = GetProducts();

            var callerAttrib = GetCallerAttributes();
            var deviceAttributes = GetDeviceAttributes();
            return FetchUUP_String(guid, _uupDevice, uupEncryptedData, created, expires, callerAttrib, deviceAttributes, products);
        }

        public virtual string BuildFileGetRequest(string uuid, string rev, string uupEncryptedData)
        {
            var guid = Guid.NewGuid().ToString();
            var created = XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Local);
            var expires = XmlConvert.ToString(DateTime.Now + TimeSpan.FromSeconds(120), XmlDateTimeSerializationMode.Local);

            var deviceAttributes = GetDeviceAttributes();
            return GetFiles_String(guid, _uupDevice, created, expires, uuid, rev, deviceAttributes);
        }

        public virtual string GetCallerAttributes()
        {
            var callerAttribArray = new string[]
            {
                "Id=UpdateOrchestrator",
                "SheddingAware=1",
                "Interactive=1",
                "IsSeeker=1"
            };

            return string.Join(";", callerAttribArray);
        }

        public abstract string GetDeviceAttributes();
        public abstract string GetProducts();
    }
}
