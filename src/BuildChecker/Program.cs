using System;
using System.IO;
using BuildChecker.Classes.Helpers;

namespace BuildChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "BuildChecker Standalone";
            Console.WriteLine("BuildChecker Standalone -- Private version, do not share");
            Console.WriteLine("Copyright (c) 2018-2020 ADeltaX and woachk");
            Console.WriteLine("Portions from Gustave M.\n");

#if RELEASE
            
            if (args.Length != 5)
            {
                Console.WriteLine("Arguments: SKU (IoT/HoloLens/HoloLens2/Team/Desktop/Server/Mobile), RING (OSG/WIF/WIS), ARCH (x86/amd64/arm/arm64), BRANCH (RS2, FEATURE2, RS3, RS4, RS5, B19H1, PRERELEASE), DL_PATH\n");
                Console.WriteLine("Supported combinations:");
                Console.WriteLine("IoT [WIF/Retail] [x86/amd64/arm/arm64] [RS2/RS3/RS4/RS5]");
                Console.WriteLine("Hololens [WIF/WIS] [x86/amd64/arm/arm64] [RS3/RS4/RS5]");
                Console.WriteLine("Hololens2 [WIF/WIS] [arm64] [B19H1]");
                Console.WriteLine("Team [WIF/WIS] [x86/amd64/arm64] [RS2]");
                Console.WriteLine("Desktop [Retail/RP/WIS/WIF/SKIP] [x86/amd64/arm64] [RS4/RS5/B19H1/PRERELEASE]");
                Console.WriteLine("Mobile [WIF/WIS] [arm] [RS2/FEATURE2]");
                Console.WriteLine("Server [Retail] [amd64/arm64] [RS5]");
                Console.WriteLine("\nExample: BuildChecker.exe IoT WIF arm64 PRERELEASE \"C:\\Downloads\"");
                return;
            }

            Run(args);
#else
            DoTest();
#endif
            Console.ReadKey();
        }

        private static void Run(string[] args)
        {
            var extractor = new CoreCheck(args[0], args[1], args[2], args[3]);
            var downloads = extractor.FetchBuild(false);

            if (downloads != null && downloads.DownloadInfo.Length > 0)
            {
                downloads.DownloadInfo = downloads.DownloadInfo.MobileFilterRemoveDeltas().DesktopFilterRemoveDeltas();

                var buildInfo = extractor.ReadBuildVersion(downloads, false);
                if (buildInfo != null)
                    OutputVals(buildInfo);
                else
                    Console.WriteLine("[Error] BuildInfo is null :/");

                Console.WriteLine("Wanna continue? Press enter.");
                Console.ReadLine();

                extractor.ExportCSV(downloads.DownloadInfo, Path.Combine(args[4], "LINKS.csv"));
                extractor.DownloadCabs(downloads.DownloadInfo, args[4]);
            }
        }

        #region Tests

        static int _successes = 0;
        static int _failures = 0;
        static void DoTest()
        {
            Console.WriteLine("Beginning test(s):\n");

            var uup = new UUP();

            uup.UpdateCookie().GetAwaiter().GetResult();

            CompactTest("Servicing", "Retail", "amd64", "RS3", uup);
            CompactTest("Servicing", "Retail", "amd64", "RS4", uup);
            CompactTest("Servicing", "Retail", "amd64", "RS5", uup);
            CompactTest("Servicing", "Retail", "amd64", "B19H1", uup);

            CompactTest("Desktop", "Retail", "x86", "RS2", uup);
            CompactTest("Desktop", "RP", "x86", "B19H1", uup);
            CompactTest("Desktop", "Beta", "x86", "B19H1", uup);
            CompactTest("Desktop", "Dev", "amd64", "B19H1", uup);
            CompactTest("Desktop", "ReleasePreview", "x86", "B20H1", uup);
            CompactTest("Desktop", "SKIP", "x86", "PRERELEASE", uup);

            CompactTest("Server", "RETAIL", "arm64", "B19H1", uup);

            CompactTest("IoT", "WIF", "x86", "RS5", uup);
            CompactTest("IoT", "Retail", "x86", "RS5", uup);

            CompactTest("HoloLens", "WIF", "arm", "RS4", uup);
            CompactTest("HoloLens", "WIS", "arm", "RS4", uup);
            CompactTest("HoloLens", "RETAIL", "arm", "RS4", uup);

            CompactTest("HoloLens2", "WIF", "arm64", "B19H1", uup);
            CompactTest("HoloLens2", "WIS", "arm64", "B19H1", uup);
            CompactTest("HoloLens2", "Retail", "arm", "B19H1", uup);

           // CompactTest("Team", "Dev", "amd64", "B19H1", uup);

            CompactTest("Team", "WIF", "arm64", "RS2", uup);
            //CompactTest("Team", "Beta", "amd64", "RS2", uup);


            CompactTest("Mobile", "WIF", "arm", "FEATURE2", uup);
            CompactTest("Mobile", "WIF", "arm", "RS2", uup);

            Console.WriteLine("Test(s) terminated.\nResults: ");
            Console.WriteLine($"Successes: {_successes}");
            Console.WriteLine($"Failures:  {_failures}");
            Console.WriteLine("\nProgram terminated. Press any key to exit.");
        }

        static void CompactTest(string family, string ring, string arch, string branch, UUP uup, string testIgnoreUpdateID = null)
        {
            Console.WriteLine("TESTING: " + family + " - " + ring + " - " + arch + " - (" + branch + ")");

            const bool updateAgentOnly = false;

            var extractor = new CoreCheck(family, ring, arch, branch, uup);
            var downloads = extractor.FetchBuild(updateAgentOnly, testIgnoreUpdateID);

            if (downloads != null)
            {
                if (downloads.IsUpdateIDIgnored)
                {
                    Console.WriteLine("[LOG] UpdateID ignored");
                    _successes++;
                }
                else
                {
                    if (downloads.DownloadInfo?.Length > 0)
                    {
                        Console.WriteLine("[LOG] Number of files: " + downloads.DownloadInfo.Length);

                        var buildInfo = extractor.ReadBuildVersion(downloads, updateAgentOnly);
                        if (buildInfo != null)
                        {
                            OutputVals(buildInfo);
                            _successes++;
                        }
                        else
                        {
                            Console.WriteLine("[ERROR] BuildInfo is null :/");
                            _failures++;
                        }
                    }
                    else
                    {
                        Console.WriteLine("[ERROR] No DownloadInfo :/");
                        _failures++;
                    }
                }
            }
            else
            {
                Console.WriteLine("[WARNING] No updates found");
                _failures++;
            }

            Console.WriteLine("\n\n-----------------------------------------------------------\n");
        }

        #endregion

        static void OutputVals(BuildInfo bi)
        {

            if (bi.Title != null)
                Console.WriteLine("TITLE            : " + bi.Title);

            if (bi.Description != null && bi.Title != null && bi.Description != bi.Title)
                Console.WriteLine("DESCRIPTION      : " + bi.Description);

            Console.WriteLine("ARCH             : " + bi.Architecture);
            Console.WriteLine("RING             : " + bi.Ring);
            Console.WriteLine("DEVICE FAMILY    : " + bi.DeviceFamily);
            Console.WriteLine("BUILD LONG       : " + bi.BuildLong);

            //For CompDB only.
            if (bi.Build != null)
                Console.WriteLine("BUILD            : " + bi.Build);

            if (bi.BuildID != null)
                Console.WriteLine("BUILD ID         : " + bi.BuildID);

            if (bi.CreatedDate.HasValue)
                Console.WriteLine("CREATED DATE     : " + bi.CreatedDate);

            if (bi.ReleaseType != null)
                Console.WriteLine("RELEASE TYPE     : " + bi.ReleaseType);


            //For NewUUP only.
            if (bi.UpdateID != null)
                Console.WriteLine("UPDATE ID        : " + bi.UpdateID);

            if (bi.LastTimeChanged != null)
                Console.WriteLine("LAST TIME CHANGED: " + bi.LastTimeChanged);

            if (bi.FlightID != null)
                Console.WriteLine("FLIGHT ID        : " + bi.FlightID);

            if (bi.RevisionNumber != null)
                Console.WriteLine("REVISION NUMBER  : " + bi.RevisionNumber);
        }
    }
}
