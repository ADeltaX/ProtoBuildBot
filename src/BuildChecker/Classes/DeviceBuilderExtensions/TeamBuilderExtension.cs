using System;
using System.Xml;

namespace BuildChecker.Classes.DeviceBuilderExtensions
{
    public sealed class TeamBuilderExtension : BuilderExtension
    {
        public TeamBuilderExtension(string branch, string build, string arch, string flight, string ring) 
            : base(branch, build, arch, flight, ring, "119")
        {        }

        public override string GetProducts()
        {
            var productsArray = new string[]
            {
                $"PN=Client.OS.rs2.{Arch}&amp;Branch={Branch}&amp;PrimaryOSProduct=1&amp;V={Build}"
            };

            return string.Join(';', productsArray);
        }

        public override string GetDeviceAttributes()
        {
            var attributes = new string[]
            {
                $"AppVer={Build}",
                $"AttrDataVer=45",
                $"BranchReadinessLevel=CB",
                $"CurrentBranch={Branch}",
                $"FlightContent={Flight}",
                $"FlightRing={Ring}",
                $"FlightingBranchName=external",
                $"IsFlightingEnabled={(Ring.ToUpper() == "RETAIL" ? "0" : "1")}",
                $"IsRetailOS={(Ring.ToUpper() == "RETAIL" ? "1" : "0")}",
                $"OSSkuId={Sku}",
                $"OSVersion={Build}",

                //$"AppVer={Build}",
                ////$"IsTestLab=True",
                //$"AttrDataVer=45",
                ////$"ReleaseType=Test",
                //$"BranchReadinessLevel=CB",
                //$"CurrentBranch={Branch}",
                //$"FlightContent={Flight}",
                //$"FlightRing={Ring}",
                //$"FlightingBranchName=vb_release",
                //$"OneCoreFwV=10.0.19041.1",
                //$"OneCoreSwV=10.0.19041.1",
                //$"OneCoreManufacturerModelName=Surface Hub 2S",
                //$"OneCoreManufacturer=Microsoft Corporation",
                //$"OneCoreOperatorName=000-88",
                //$"IsFlightingEnabled={(Ring.ToUpper() == "RETAIL" ? "0" : "1")}",
                //$"IsRetailOS={(Ring.ToUpper() == "RETAIL" ? "1" : "0")}",
                //$"OSSkuId={Sku}",
                //$"OSVersion={Build}",
                ////$"IsMsftOwned=1"
            };

            return string.Join(';', attributes);
        }
    }
}
