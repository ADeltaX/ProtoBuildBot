using BuildChecker.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BuildChecker.Classes.Extractors
{
    public class SevenZipExtractor : ICabExtractor
    {
        private static readonly bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private static readonly string SZipWindowsExe = Path.Combine(Directory.GetCurrentDirectory(), "libs", "7za.exe");

        public bool Extract(string sourceFile, string destFolder, string filter)
        {
            try
            {
                var escapedFile = "\"" + sourceFile.Replace("\"", "\\\"") + "\"";
                var escapedDir = "\"" + destFolder.Replace("\"", "\\\"") + "\"";

                var process = new Process()
                {

                    StartInfo = new ProcessStartInfo
                    {
                        FileName = isWindows ? SZipWindowsExe : "7za",
                        Arguments = $"e {escapedFile} -o{escapedDir} {filter} -y",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };

                process.Start();
                process.WaitForExit();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Error.");

                return false;
            }
        }
    }
}
