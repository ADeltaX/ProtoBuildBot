using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BuildChecker.Classes.Helpers
{
    public static class DownloadInfoFilter
    {
        public static DownloadInfo[] DesktopCompFilter(this DownloadInfo[] dwInfo)
        {
            var tmp = new List<DownloadInfo>();

            Regex regex = new Regex(@"~~|en\-us|\.ESD|\.exe|Metadata(\_|\.)|deployment|ModernApps", RegexOptions.ECMAScript | RegexOptions.Compiled);
            foreach (var item in dwInfo)
            {
                if (!regex.IsMatch(item.Name))
                    continue;
                else
                    tmp.Add(item);
            }

            return tmp.ToArray();
        }

        public static DownloadInfo[] MobileFilterRemoveDeltas(this DownloadInfo[] dwInfo) =>
            dwInfo.Where(dw => !dw.Name.EndsWith(".cbsu.cab", StringComparison.InvariantCultureIgnoreCase)).ToArray();

        public static DownloadInfo[] DesktopFilterRemoveDeltas(this DownloadInfo[] dwInfo) =>
            dwInfo.Where(dw => !dw.Name.EndsWith(".psf", StringComparison.InvariantCultureIgnoreCase)).ToArray();
    }
}
