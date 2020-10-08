using BuildChecker.Classes.Structures;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BuildChecker.Classes.Helpers
{
    public class HttpDownloader
    {
        //Slice 8MB+64KB (8_388_608 + 65_536 = 8_454_144‬‬)
        private const long CHUNK_SIZE = 8_388_608 + 65_536;
        //private const long CHUNK_SIZE = 262_144 + 65_536;

        private readonly HttpClient _hc;
        public string DownloadPath { get; set; }

        public HttpDownloader(string downloadPath, IWebProxy proxy = null, bool useProxy = false)
        {
            var filter = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                Proxy = proxy,
                UseProxy = useProxy,
                MaxConnectionsPerServer = 1024
            };

            filter.AllowAutoRedirect = false;

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                filter.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; //For Linux ¯\_(ツ)_/¯

            _hc = new HttpClient(filter);
            _hc.Timeout = TimeSpan.FromSeconds(10);

            _hc.DefaultRequestHeaders.Add("User-Agent", "Windows-Update-Agent/10.0.10011.16384 Client-Protocol/2.1");
            _hc.DefaultRequestHeaders.Connection.Add("keep-alive");

            DownloadPath = downloadPath;
        }

        public async Task<string> DownloadAsync(string url, string relativePath, bool skip = false, string esrpDec = null, IProgress<double> downloadProgress = null)
        {
            if (string.IsNullOrWhiteSpace(DownloadPath))
                throw new InvalidOperationException("DownloadPath should be set before calling this method.");

            string text = Path.Combine(DownloadPath, relativePath);

            if (File.Exists(text))
                return text;
            Console.WriteLine("[DW]" + relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(text));

            try
            {
                bool result = await HttpDownload(text + ".dlTmp", url, downloadProgress);
                if (!result)
                {
                    if (File.Exists(text + ".dlTmp"))
                        File.Delete(text + ".dlTmp");
                }
                else
                {
                    if (File.Exists(text))
                    {
                        Console.WriteLine("[WARNING] Old file detected: " + text);
                        File.Move(text, text + ".old", true);
                    }

                    if (esrpDec != null)
                    {
                        EsrpDecrypt esrp = DeserializeEsrp(esrpDec);
                        EsrpDecryptor.Decrypt(text + ".dlTmp", text, Convert.FromBase64String(esrp.KeyData));
                        File.Delete(text + ".dlTmp");
                    }
                    else
                    {
                        File.Move(text + ".dlTmp", text, true);
                    }
                }
            }
            catch
            {
                return null;
            }
            return text;
        }

        public static EsrpDecrypt DeserializeEsrp(string json)
        {
            EsrpDecrypt deserializedEsrpDecrypt = new EsrpDecrypt();
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedEsrpDecrypt.GetType());
            deserializedEsrpDecrypt = ser.ReadObject(ms) as EsrpDecrypt;
            ms.Close();
            return deserializedEsrpDecrypt;
        }

        private async Task<bool> HttpDownload(string path, string url, IProgress<double> downloadProgress = null, int bufferSize = 65_536, CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await _hc.SendAsync(new HttpRequestMessage(HttpMethod.Head, new Uri(url)), HttpCompletionOption.ResponseHeadersRead);
                var contentLength = response.Content.Headers.ContentLength;

                if (!contentLength.HasValue)
                {
                    using Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();
                    using Stream streamToWriteTo = File.Open(path, FileMode.Create);
                    await streamToReadFrom.CopyToAsync(streamToWriteTo);
                    return true;
                }

                IProgress<long> IReportable = null;

                if (downloadProgress != null)
                {
                    IReportable = new Progress<long>(totalBytes => downloadProgress.Report((double)totalBytes / contentLength.Value));
                }

                using (FileStream streamToWriteTo = File.Open(path, FileMode.Create))
                {
                    streamToWriteTo.SetLength(contentLength.Value);

                    long currRange = 0;
                    long chunk = CHUNK_SIZE;
                    long totalBytesRead = 0;

                    while (currRange < contentLength.Value)
                    {
                        if (currRange + chunk >= contentLength.Value)
                            chunk = contentLength.Value - currRange - 1;

                        using var resp = await _hc.SendAsync(CreateRequestHeaderForRange(HttpMethod.Get, url, currRange, currRange + chunk),
                            HttpCompletionOption.ResponseHeadersRead);

                        currRange += (chunk + 1);

                        if (resp.IsSuccessStatusCode)
                        {
                            using Stream streamToReadFrom = await resp.Content.ReadAsStreamAsync();

                            var buffer = new byte[bufferSize];
                            int bytesRead;
                            while ((bytesRead = await streamToReadFrom.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
                            {
                                await streamToWriteTo.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                                totalBytesRead += bytesRead;
                                IReportable?.Report(totalBytesRead);
                            }
                        }
                        else
                        {
                            //TODO: Actually I should handle this.
                            Console.WriteLine("RIP");
                            throw new Exception("RIP");
                        }
                    }
                }

                downloadProgress?.Report(1);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private HttpRequestMessage CreateRequestHeaderForRange(HttpMethod method, string url, long from, long to)
        {
            var request = new HttpRequestMessage(method, new Uri(url));
            request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(from, to);
            return request;
        }
    }
}
