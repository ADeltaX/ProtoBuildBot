using System;
using System.Xml;

namespace BuildChecker.Classes.DeviceBuilderExtensions
{
    public sealed class DesktopBuilderExtension : BuilderExtension
    {
        public DesktopBuilderExtension(string Branch, string Build, string Arch, string Flight, string Ring)
            : base(Branch, Build, Arch, Flight, Ring, "48")
        {        }

        public override string GetProducts()
        {
            var productsArray = new string[]
            {
                $"PN=Client.OS.rs2.{Arch}&amp;Branch={Branch}&amp;V={Build}",
            };

            return string.Join(';', productsArray);
        }

        public override string GetDeviceAttributes()
        {
            var attributes = new string[]
            {
                $"AppVer={Build}",
                $"AttrDataVer=98",
                $"ReleaseType=Production",
                $"BranchReadinessLevel=CB",
                $"CurrentBranch={Branch}",
                $"FlightingBranchName={Ring}",
                $"FlightContent={Flight}",
                $"FlightRing=External",
                $"IsFlightingEnabled={(Ring.ToUpper() == "RETAIL" ? "0" : "1")}",
                $"IsRetailOS={(Ring.ToUpper() == "RETAIL" ? "1" : "0")}",
                $"OSSkuId={Sku}",
                $"OSVersion={Build}",
                //$"ProcessorIdentifier=GenuineIntel Family 23 Model 1 Stepping 1",
                //$"OEMModel=System Product Name",
                $"ProcessorManufacturer=GenuineIntel",
                $"UpgEx_20H1=Green",
                $"UpgEx_21H1=Green",
                $"UpgEx_22H1=Green",
                $"DataExpDateEpoch_20H1=0",
                $"GStatus_20H1=2"
            };

            return string.Join(';', attributes);
        }
    }
}
