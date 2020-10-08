using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BuildChecker.Classes.DeviceBuilderExtensions
{
    public sealed class IoTBuilderExtension : BuilderExtension
    {
        public IoTBuilderExtension(string Branch, string Build, string Arch, string Flight, string Ring)
            : base(Branch, Build, Arch, Flight, Ring, "133") //133
        {        }

        public override string GetProducts()
        {
            var productsArray = new string[]
            {
                $"PN=IoTCore.OS.rs2.{Arch}&amp;Branch={Branch}&amp;V={Build};"
            };

            return string.Join(';', productsArray);
        }

        public override string GetDeviceAttributes()
        {
            var attributes = new string[]
            {
                //$"IsTestLab=1",
                //$"IsRetailOS=0",
                $"AttrDataVer=25",
                $"ReleaseType=Test",
                //$"BranchReadinessLevel=CB",
                $"FlightContent={Flight}",
                $"FlightRing={Ring}",
                $"MobileOperatorCommercialized=000-88",
                $"FlightingBranchName={(Ring.ToUpper() == "RETAIL" ? "external" : Branch)}",
                $"IsFlightingEnabled={(Ring.ToUpper() == "RETAIL" ? "0" : "1")}",
                $"OSVersion={Build}",
                $"OSSkuId={Sku}",
                $"IsMsftOwned=1"
            };

            return string.Join(';', attributes);
        }
    }
}
