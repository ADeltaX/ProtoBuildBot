using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BuildChecker.Classes.DeviceBuilderExtensions
{
    public class HololensBuilderExtension : BuilderExtension
    {
        public HololensBuilderExtension(string Branch, string Build, string Arch, string Flight, string Ring)
            : base(Branch, Build, Arch, Flight, Ring, "135")
        {        }

        public override string GetProducts()
        {
            var productsArray = new string[]
            {
                $"PN=HoloLens.OS.rs2.{Arch}&amp;Branch={Branch}&amp;V={Build};"
            };

            return string.Join(';', productsArray);
        }

        public override string GetDeviceAttributes()
        {
            string[] attributes;

            // if newer than 19H1 and arm64, WCOS path!
            if (Branch != "rs5_release" && Branch != "rs4_release" && Branch != "rs3_release" && Arch == "arm64")
            {
                attributes = new string[]
                {
                    $"AttrDataVer=41",
                    $"ReleaseType={(Ring.ToUpper() == "RETAIL" ? "Production" : "Test")}",
                    $"BranchReadinessLevel=CB",
                    $"FlightContent={Flight}",
                    $"FlightRing={Ring}",
                    $"OneCoreFwV=10.0.18362.1",
                    $"OneCoreSwV=10.0.18362.1",
                    $"OneCoreManufacturerModelName=HoloLens",
                    $"OneCoreManufacturer=Microsoft Corporation",
                    $"OneCoreOperatorName=000-88",
                    $"FlightingBranchName={(Ring.ToUpper() == "RETAIL" ? "external" : Branch)}",
                    $"IsFlightingEnabled={(Ring.ToUpper() == "RETAIL" ? "0" : "1")}",
                    $"OSVersion={Build}",
                    $"OSSkuId={Sku}",
                    $"IsMsftOwned=1"
                };
            }
            // Otherwise...
            else
            {
                attributes = new string[]
                {
                    $"AttrDataVer=9",
                    $"ReleaseType={(Ring.ToUpper() == "RETAIL" ? "Production" : "Test")}",
                    $"BranchReadinessLevel=CB",
                    $"FlightContent={Flight}",
                    $"FlightRing={Ring}",
                    $"FlightingBranchName={(Ring.ToUpper() == "RETAIL" ? "external" : Branch)}",
                    $"IsFlightingEnabled={(Ring.ToUpper() == "RETAIL" ? "0" : "1")}",
                    $"OSVersion={Build}",
                    $"OSSkuId={Sku}",
                    $"IsMsftOwned=1"
                };
            }
            return string.Join(';', attributes);
        }
    }
}
