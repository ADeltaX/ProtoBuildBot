using BuildChecker.Interfaces;
using Microsoft.Deployment.Compression.Cab;
using System;
using System.Runtime.InteropServices;

namespace BuildChecker.Classes.Extractors
{
    public class WinCabinetExtractor : ICabExtractor
    {
        private static readonly bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public WinCabinetExtractor()
        {
            if (!isWindows)
                throw new InvalidOperationException("This class is supported only on Windows.");
        }

        public bool Extract(string sourceFile, string destFolder, string filter)
        {
            try
            {
                CabInfo info = new CabInfo(sourceFile);

                if (string.IsNullOrWhiteSpace(filter))
                {
                    System.IO.Directory.CreateDirectory(destFolder);
                    info.Unpack(destFolder);
                }
                else
                {
                    var files = info.GetFiles(filter);
                    if (files.Count > 0)
                    {
                        System.IO.Directory.CreateDirectory(destFolder);
                        info.UnpackFile(files[0].Name, System.IO.Path.Combine(destFolder, files[0].Name));
                        return true;
                    }
                    else
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Error: " + ex.Message);
                return false;
            }

        }
    }
}
