using BuildChecker.Classes;
using BuildChecker.Classes.DeviceBuilderExtensions;
using BuildChecker.Classes.Helpers;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BuildChecker
{
    public class UUP
    {
        protected string GetCookie_String() => "<s:Envelope xmlns:a=\"http://www.w3.org/2005/08/addressing\" xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\"><s:Header><a:Action s:mustUnderstand=\"1\">http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService/GetCookie</a:Action><a:To s:mustUnderstand=\"1\">https://fe3.delivery.mp.microsoft.com/ClientWebService/client.asmx</a:To><o:Security s:mustUnderstand=\"1\" xmlns:o=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\"><wuws:WindowsUpdateTicketsToken wsu:id=\"ClientMSA\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" xmlns:wuws=\"http://schemas.microsoft.com/msus/2014/10/WindowsUpdateAuthorization\"><TicketType Name=\"MSA\" Version=\"1.0\" Policy=\"MBI_SSL\"></TicketType></wuws:WindowsUpdateTicketsToken></o:Security></s:Header><s:Body><GetCookie xmlns=\"http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService\"><protocolVersion>2.0</protocolVersion></GetCookie></s:Body></s:Envelope>";

        private const string fe2URL = "https://fe2cr.update.microsoft.com/v6/ClientWebService/client.asmx";
        private const string fe3URL = "https://fe3cr.delivery.mp.microsoft.com/ClientWebService/client.asmx";
        private const string fe3SecuredURL = fe3URL + "/secured";

        private readonly HttpClient _hc;
        private CorrelationVector correlationVector = new CorrelationVector();
        private string _uupEncryptedData = "l7+HKNuROJPWJaJh3mKSSE/WnnoTIyHbrZ9eC3dciMUHE24soYZPIuDyfgVtUMSfvkmF+dDRgwUCVeVCeqayozk3EPuTxj9cx3J1Uf8GLzkGhd1d+aUGtict0xCZmywG9IxMfLR7HBlWybjDldJ73CdypszJwx+IqqCVMmuV3tG3996BXvq2BpjYm4H6cJca+fhx8ZxNNKdZj76OwkVj2w==";

        private UpdateIdentity updIden;
        private Envelope envelopeUUPRequestResponse;
        
        public UUP()
        {
            var filter = new HttpClientHandler { AllowAutoRedirect = false, AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            filter.AllowAutoRedirect = false;
            filter.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; //For Linux ¯\_(ツ)_/¯
            _hc = new HttpClient(filter);
            AddDefaultHeaders(_hc);
            correlationVector.GetValue();
        }

        public async Task<FileRequests> RefreshFiles(BuilderExtension builderExt, bool updateAgentOnly = false) {
            //Send the second request --> Get links to download packages.

            var envelopeUUPFilesResponse = await GetFiles(builderExt, updIden.UpdateID, updIden.RevisionNumber);

            //Now merge and get DownloadInfo

            var dwInfo = builderExt.ExtractDownloadInfo(updIden, envelopeUUPRequestResponse.Body.SyncUpdatesResponse, envelopeUUPFilesResponse.Body.GetExtendedUpdateInfo2Response, updateAgentOnly);
            return new FileRequests { DownloadInfo = dwInfo, UpdateID = updIden.UpdateID, LastTimeChange = updIden.LastChangeTime, FlightID = updIden.FlightID, RevisionNum = updIden.RevisionNumber, Title = updIden.LocalizedProperties.Title, Description = updIden.LocalizedProperties.Description };
        }

        public async Task<FileRequests> GetFileRequests(BuilderExtension builderExt, bool updateAgentOnly = false, string ignoreUpdateID = null)
        {
            try
            {
                //Send THE first request.
                envelopeUUPRequestResponse = await GetRequest(builderExt);

                //Extract useful data from response
                updIden = builderExt.ExtractUpdateIdentity(envelopeUUPRequestResponse);

                if (updIden != null)
                {
                    if (ignoreUpdateID != null && updIden.UpdateID.ToLower() == ignoreUpdateID.ToLower())
                    {
                        return new FileRequests { IsUpdateIDIgnored = true };
                    }

                    //Send the second request --> Get links to download packages.
                    var envelopeUUPFilesResponse = await GetFiles(builderExt, updIden.UpdateID, updIden.RevisionNumber);

                    //Now merge and get DownloadInfo
                    var dwInfo = builderExt.ExtractDownloadInfo(updIden, envelopeUUPRequestResponse.Body.SyncUpdatesResponse, envelopeUUPFilesResponse.Body.GetExtendedUpdateInfo2Response, updateAgentOnly);
                    return new FileRequests { DownloadInfo = dwInfo, UpdateID = updIden.UpdateID, LastTimeChange = updIden.LastChangeTime, FlightID = updIden.FlightID, RevisionNum = updIden.RevisionNumber, Title = updIden.LocalizedProperties.Title, Description = updIden.LocalizedProperties.Description };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] " + ex.Message);
                Console.ResetColor();
                return null;
            }
        }

        private async Task<Envelope> GetRequest(BuilderExtension baseFetcher)
        {
            var composed = baseFetcher.BuildFetchUUPRequest(_uupEncryptedData);

            var req = CreateRequestHeader(HttpMethod.Post, fe3URL);
            req.Content = CreateStringContentSoapXml(composed);
            req.Headers.Add("MS-CV", correlationVector.Increment());
            var resp = await _hc.SendAsync(req);

            var envelope = DeserializeEnvelope(await resp.Content.ReadAsStringAsync());
            UpdateCookieFromEnvelope(envelope);

            return envelope;
        }

        public async Task<Envelope> GetFiles(BuilderExtension baseFetcher, string updateId, string rev)
        {
            var composed = baseFetcher.BuildFileGetRequest(updateId, rev, _uupEncryptedData);
            var req = CreateRequestHeader(HttpMethod.Post, fe3SecuredURL);

            req.Content = CreateStringContentSoapXml(composed);
            req.Headers.Add("MS-CV", correlationVector.Increment());
            var resp = await _hc.SendAsync(req);

            var envelope = DeserializeEnvelope(await resp.Content.ReadAsStringAsync());
            UpdateCookieFromEnvelope(envelope);

            return envelope;
        }

        private void UpdateCookieFromEnvelope(Envelope envelope)
        {
            if (envelope?.Body?.GetCookieResponse?.GetCookieResult?.EncryptedData != null)
                _uupEncryptedData = envelope.Body.GetCookieResponse.GetCookieResult.EncryptedData;
        }

        public async Task UpdateCookie()
        {
            try
            {
                var req = CreateRequestHeader(HttpMethod.Post, fe3URL);

                req.Content = CreateStringContentSoapXml(GetCookie_String());
                var resp = await _hc.SendAsync(req);
                var envelope = DeserializeEnvelope(await resp.Content.ReadAsStringAsync());
                UpdateCookieFromEnvelope(envelope);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] " + ex.Message);
                Console.ResetColor();
            }
        }

        public void GenerateNewCV()
            => correlationVector = new CorrelationVector();

        #region HttpClient Helpers

        private static HttpRequestMessage CreateRequestHeader(HttpMethod method, string url) 
            => new HttpRequestMessage(method, new Uri(url));

        private static StringContent CreateStringContentSoapXml(string composedString) 
            => new StringContent(composedString, System.Text.Encoding.UTF8, "application/soap+xml");

        private static void AddDefaultHeaders(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Windows-Update-Agent/10.0.10011.16384 Client-Protocol/2.0");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "no-cache");
            httpClient.DefaultRequestHeaders.Pragma.Add(new System.Net.Http.Headers.NameValueHeaderValue("no-cache"));
            httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
        }

        #endregion

        //INTERNAL

        private Envelope DeserializeEnvelope(string content)
        {
            try
            {
                var ser = new XmlSerializer(typeof(Envelope));
                var envelope = (Envelope)ser.Deserialize(new StringReader(content));
                return envelope;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
