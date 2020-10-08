using BuildChecker;
using BuildChecker.Classes;
using BuildChecker.Classes.DeviceBuilderExtensions;
using BuildChecker.Classes.DeviceCheckers;
using BuildChecker.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using ProtoBuildBot.Attributes;
using ProtoBuildBot.Classes.Automation;
using ProtoBuildBot.DataStore;
using ProtoBuildBot.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ProtoBuildBot.Controllers
{
    [Route("v1")]
    [SubdomainFilter(SecretKeys.BaseHost, "meme", "api.lol")]
    public class ProtoBuildController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        public ProtoBuildController(IMemoryCache cache)
        {
            _cache = cache;
        }

        //TODO 2.0 - remake all of this

        [ValidateTimeLimit]
        [HttpGet("GetAllRingStates")]
        public ActionResult GetAllRingStates()
        {
            //TODO REPLACE THIS WITH THE NEWWWW (1.6)
            var rngs = SharedDBcmd.GetAllRingStates(); //Item1 = Device | Item2 = Ring | Item3 = BuildLab

            var RingStatesList = new List<RingState>(rngs.Count);

            rngs.ForEach(element => RingStatesList.Add(new RingState() { DeviceFamily = element.Item1, Ring = element.Item2, BuildLab = element.Item3 }));

            return new JsonResult(RingStatesList);
        }

        [HttpGet("GenerateLink")]
        public ActionResult GenerateLink([FromQuery] string id)
        {
            if (string.IsNullOrWhiteSpace(id) && !Guid.TryParse(id, out Guid _))
                return new JsonResult(new ErrorContent() { ErrorCode = 0, Message = "Missing parameters" })
                {  StatusCode = (int)HttpStatusCode.BadRequest };

            if (_cache.TryGetValue("t_" + id, out JsonResult res))
                return res;

            var path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + TelegramBotSettings.SaveJSONPath + Path.DirectorySeparatorChar + id + ".json.gz";

            //1             2     3            4             5         6               7
            //DeviceFamily, Ring, ReleaseType, Architecture, FlightID, RevisionNumber, BuildLong
            var el = SharedDBcmd.GetBuildInfo(id);

            return _cache.GetOrCreate("t_" + id, factory =>
            {
                factory.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(180);

                if (el.Count > 0 && System.IO.File.Exists(path))
                {
                    var ext = ProtoBuildControllerHelper.GetBuilderExtension(el[0].Item1, el[0].Item4, el[0].Item2, el[0].Item7.Split(' ').First());
                    var lol = (SearchAutomation.thisUUP.GetFiles(ext, id, el[0].Item6 ?? "1")).GetAwaiter().GetResult();

                    var strm = BaseChecker.DecompressFileInMemory(path);
                    using StreamReader reader = new StreamReader(strm);
                    string text = reader.ReadToEnd();
                    var des = JsonConvert.DeserializeObject<FileRequestsWithURL>(text);

                    var fileLocations = lol.Body?.GetExtendedUpdateInfo2Response?.GetExtendedUpdateInfo2Result?.FileLocations?.FileLocation;

                    if (fileLocations != null && fileLocations.Count > 0)
                    {
                        for (int i = 0; i < des.DownloadInfo.Length; i++)
                        {
                            var url = fileLocations.AsParallel().First(file => file.FileDigest == des.DownloadInfo[i].SHA1Hash);
                            des.DownloadInfo[i].Url = url.Url;
                        }

                        des.FlightID = el[0].Item5;
                        des.RevisionNum = el[0].Item6;

                        var x = el[0].Item1;
                        if (el[0].Item1.Contains('-', StringComparison.Ordinal))
                            x = el[0].Item1.Split('-')[0];

                        des.DeviceFamily = x;
                        des.Architecture = el[0].Item4;
                        des.BuildLong = el[0].Item7;

                        if (el[0].Item3 != null && (el[0].Item3.StartsWith("TEST", StringComparison.OrdinalIgnoreCase) || el[0].Item3.StartsWith("PRODUCTION", StringComparison.OrdinalIgnoreCase)))
                            des.ReleaseType = el[0].Item3;

                        var jsn = new JsonResult(des);


                        return jsn;
                    }
                    else
                    {
                        return new JsonResult(new ErrorContent() { ErrorCode = 2, Message = "No results from Windows Update" })
                        { StatusCode = (int)HttpStatusCode.BadRequest };
                    }
                }
                else
                {
                    return new JsonResult(new ErrorContent() { ErrorCode = 1, Message = "UpdateID not saved locally." })
                    { StatusCode = (int)HttpStatusCode.BadRequest };
                }
            });

            
        }

        [HttpGet("Delete")]
        public ActionResult Delete()
        {
            return new RedirectResult("https://i.kym-cdn.com/photos/images/original/001/185/519/64c.gif", true);
        }
    }

    public static class ProtoBuildControllerHelper
    {
        public static BuilderExtension GetBuilderExtension(string deviceFamily, string arch, string ring, string build)
        {
            if (deviceFamily.Contains("-", StringComparison.Ordinal))
                deviceFamily = deviceFamily.Split('-')[0];

            if (!Enum.TryParse(deviceFamily, true, out DeviceFamily _deviceFamily))
                throw new ArgumentException("Device Family can't be recognized.");

            if (!Enum.TryParse(ring, true, out Ring _ring))
                throw new ArgumentException("Ring can't be recognized.");

            (string Fl, string Ri) = CoreCheck.GetFlightAndRing(_ring);

            switch (_deviceFamily)
            {
                case DeviceFamily.HoloLens:
                    return new HololensBuilderExtension("rs_prerelease", build, arch, Fl, Ri);
                case DeviceFamily.HoloLens2:
                    return new HoloLens2BuilderExtension("rs_prerelease", build, arch, Fl, Ri);
                case DeviceFamily.IoT:
                    return new IoTBuilderExtension("rs_prerelease", build, arch, Fl, Ri);
                case DeviceFamily.Mobile:
                    return new MobileBuilderExtension("rs2_release", "10.0.15063.0", arch, Fl, Ri);
                case DeviceFamily.Desktop:
                    return new DesktopBuilderExtension("rs_prerelease", build, arch, Fl, Ri);
                case DeviceFamily.Team:
                    return new TeamBuilderExtension("rs_prerelease", build, arch, Fl, Ri);
                case DeviceFamily.Server:
                case DeviceFamily.Servicing:
                    return new ServerBuilderExtension("rs_prerelease", build, arch, Fl, Ri);
                case DeviceFamily.IoTEnterprise:
                    return new IoTEnterpriseBuilderExtension("rs_prerelease", build, arch, Fl, Ri);
                case DeviceFamily.Unknown:
                    throw new ArgumentException("Unknown device family.");
            }

            return null;
        }
    }

    public class ErrorContent
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
