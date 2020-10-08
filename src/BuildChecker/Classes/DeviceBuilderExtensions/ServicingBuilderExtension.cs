using System;
using System.Xml;

namespace BuildChecker.Classes.DeviceBuilderExtensions
{
    public sealed class ServicingBuilderExtension : BuilderExtension
    {
        public ServicingBuilderExtension(string Branch, string Build, string Arch, string Flight, string Ring)
            : base(Branch, Build, Arch, Flight, Ring, "72") //PRODUCT_ENTERPRISE_EVALUATION
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
                $"AttrDataVer=45",
                $"BranchReadinessLevel=CB",
                $"CurrentBranch={Branch}",
                $"FlightContent={Flight}",
                $"FlightRing={Ring}",
                $"IsFlightingEnabled={(Ring.ToUpper() == "RETAIL" ? "0" : "1")}",
                $"IsRetailOS={(Ring.ToUpper() == "RETAIL" ? "1" : "0")}",
                $"OSSkuId={Sku}",
                $"OSVersion={Build}",
            };

            return string.Join(';', attributes);
        }
    }
}
