using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BuildChecker.Classes.DeviceBuilderExtensions
{
    public sealed class HoloLens2BuilderExtension : BuilderExtension
    {
        public HoloLens2BuilderExtension(string Branch, string Build, string Arch, string Flight, string Ring)
            : base(Branch, Build, Arch, Flight, Ring, "")
        { }

        public override string GetProducts()
        {
            var productsArray = new string[]
            {
                $"PN=HOLOLENS.OS.RS2.{Arch}&amp;Branch={Branch}&amp;V={Build};"
            };

            return string.Join(';', productsArray);
        }

        public override string GetDeviceAttributes()
        {
            var attributes = new string[]
            {
                $"AttrDataVer=41",
                $"IsTestLab=False",
                $"ReleaseType=Test",
                $"BranchReadinessLevel=CB",
                $"FlightContent={Flight}",
                $"FlightRing={Ring}",
                $"InstallationType=FactoryOS",
                $"OneCoreFwV=10.0.18362.1",
                $"OneCoreSwV=10.0.18362.1",
                $"OneCoreManufacturerModelName=HoloLens",
                $"OneCoreManufacturer=Microsoft Corporation",
                $"OneCoreOperatorName=000-88",
                $"FlightingBranchName={(Ring.ToUpper() == "RETAIL" ? "external" : Branch)}",
                $"IsFlightingEnabled={(Ring.ToUpper() == "RETAIL" ? "0" : "1")}",
                $"OSVersion={Build}",
                $"OSSkuId=135",
                $"IsMsftOwned=0"
            };

            return string.Join(';', attributes);
        }
    }
}
