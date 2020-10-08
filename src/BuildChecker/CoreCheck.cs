using BuildChecker.Classes.DeviceCheckers;
using BuildChecker.Enums;
using BuildChecker.Interfaces;
using System;
using System.Net.Http;

namespace BuildChecker
{
    public class CoreCheck : IChecker
    {
        private readonly IChecker _checker;

        public CoreCheck(string family, string ring, string arch, string branch, UUP newUUP = null)
        {
            if (!Enum.TryParse(family, true, out DeviceFamily _deviceFamily))
                throw new ArgumentException("Device Family can't be recognized.");

            if (!Enum.TryParse(ring, true, out Ring _ring))
                throw new ArgumentException("Ring can't be recognized.");

            if (!Enum.TryParse(arch, true, out Arch _arch))
                throw new ArgumentException("CPU Arch can't be recognized");

            if (!Enum.TryParse(branch, true, out Branch _branch))
                _branch = Branch.RS2;

            (string Fl, string Ri) = GetFlightAndRing(_ring);
            (string Br, string Bu) = GetFullBranchAndBuildVersion(_branch);

            switch (_deviceFamily)
            {
                case DeviceFamily.HoloLens:
                    _checker = new HololensChecker(Br, Bu, _arch.ToString(), Fl, Ri, newUUP);
                    break;
                case DeviceFamily.HoloLens2:
                    _checker = new HoloLens2Checker(Br, Bu, _arch.ToString(), Fl, Ri, newUUP);
                    break;
                case DeviceFamily.IoT:
                    _checker = new IoTChecker(Br, Bu, _arch.ToString(), Fl, Ri, newUUP);
                    break;
                case DeviceFamily.Mobile:
                    _checker = new MobileChecker(Br, Bu, _arch.ToString(), Fl, Ri, newUUP);
                    break;
                case DeviceFamily.Desktop:
                    _checker = new DesktopChecker(Br, Bu, _arch.ToString(), Fl, Ri, newUUP);
                    break;
                case DeviceFamily.Team:
                    _checker = new TeamChecker(Br, Bu, _arch.ToString(), Fl, Ri, newUUP);
                    break;
                case DeviceFamily.Server:
                    _checker = new ServerChecker(Br, Bu, _arch.ToString(), Fl, Ri, newUUP);
                    break;
                case DeviceFamily.Servicing:
                    _checker = new ServicingChecker(Br, Bu, _arch.ToString(), Fl, Ri, newUUP);
                    break;
                case DeviceFamily.IoTEnterprise:
                    _checker = new IoTEnterpriseChecker(Br, Bu, _arch.ToString(), Fl, Ri, newUUP);
                    break;
                case DeviceFamily.Unknown:
                    throw new ArgumentException("Unknown device family.");
            }
        }

        public static (string, string) GetFullBranchAndBuildVersion(Branch branch)
        {
            switch (branch)
            {
                case Branch.PRERS2:
                    return ("rs2_release", "10.0.14800.1000");
                case Branch.RS2:
                    return ("rs2_release", "10.0.15063.0");
                case Branch.FEATURE2:
                    return ("feature2", "10.0.15254.0"); 
                case Branch.RS3:
                    return ("rs3_release", "10.0.16299.15");
                case Branch.RS4:
                    return ("rs4_release", "10.0.17134.1");
                case Branch.RS5:
                    return ("rs5_release", "10.0.17763.1");
                case Branch.B19H1:
                    return ("19h1_release", "10.0.18362.1");
                case Branch.B20H1:
                    return ("rs_prerelease", "10.0.19041.1");
                case Branch.PRERELEASE:
                    return ("rs_prerelease", "10.0.19563.0");
                default:
                    throw new ArgumentException("Invalid branch.");
            }
        }

        public static (string, string) GetFlightAndRing(Ring ring)
        {
            switch (ring)
            {
                case Ring.SKIP:
                    return ("Skip", "WIF");
                case Ring.WIF:
                    return ("Active", "WIF");
                case Ring.WIS:
                    return ("Active", "WIS");
                case Ring.RP:
                    return ("Current", "RP");
                case Ring.RETAIL:
                    return ("Active", "Retail");
                case Ring.DEV:
                    return ("Mainline", "Dev");
                case Ring.BETA:
                    return ("Mainline", "Beta");
                case Ring.RELEASEPREVIEW:
                    return ("Mainline", "ReleasePreview");
                default:
                    return ("wat", "wat"); //What?
            }
        }

        public bool DownloadCabs(DownloadInfo[] array, string path) => _checker.DownloadCabs(array, path);

        public FileRequests FetchBuild(bool updateAgentOnly, string ignoreUpdateID = null) => _checker.FetchBuild(updateAgentOnly, ignoreUpdateID);

        public BuildInfo ReadBuildVersion(FileRequests fileRequests, bool updateAgentOnly) => _checker.ReadBuildVersion(fileRequests, updateAgentOnly);

        public bool ExportCSV(DownloadInfo[] array, string filenamepath) => _checker.ExportCSV(array, filenamepath);

        public bool ExportJSON(FileRequests fileReq, string filenamepath, bool includeLinks) => _checker.ExportJSON(fileReq, filenamepath, includeLinks);
    }
}
