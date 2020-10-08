using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BuildChecker.Classes.DeviceBuilderExtensions
{
    public sealed class ServerBuilderExtension : BuilderExtension
    {
        //Why the hell ONLY Windows.Server uses &amp; as separator and others as ;? It rejects the request if it's ;....

        public ServerBuilderExtension(string Branch, string Build, string Arch, string Flight, string Ring)
            : base(Branch, Build, Arch, Flight, Ring, "145")  // 145: ServerDatacenterACor, 120: ServerARM64, 168: Azure ServerCore
        {   }

        public override string GetProducts()
        {
            var productsArray = new string[]
            {
                $"PN=Server.OS.{Arch}",
                $"Branch={Branch}",
                $"PrimaryOSProduct=1",
                $"Repairable=1",
                $"V={Build};PN=Windows.EmergencyUpdate.{Arch}",
                $"V={Build};PN=Windows.UpdateStackPackage.{Arch}",
                $"Name=Update Stack Package",
                $"V={Build};PN=Hammer.{Arch}",
                $"Source=UpdateOrchestrator",
                $"V=0.0.0.0;PN=MSRT.{Arch}",
                $"Source=UpdateOrchestrator",
                $"V=0.0.0.0;PN=SedimentPack.{Arch}",
                $"Source=UpdateOrchestrator",
                $"V=0.0.0.0;PN={{5a2e40f9-307b-5bcb-aa0a-51d10be05565}}_{Arch}",
                $"Source=SMBIOS;PN={{6d4071db-0b8f-520b-806c-81e804431336}}_{Arch}",
                $"Source=SMBIOS;PN={{d5f9f684-700b-53c8-a3dc-b364d9c54f9f}}_{Arch}",
                $"Source=SMBIOS;PN={{f4c1fd58-0de8-56b5-95be-d96c0765a89e}}_{Arch}"
            };

            return string.Join("&amp;", productsArray);
        }

        public override string GetCallerAttributes()
        {
            var caller = new string[]
            {
                $"E:Interactive=1",
                $"SheddingAware=1",
                $"Id=%3C%3CPROCESS%3E%3E%3A%20cscript.exe"
            };

            return string.Join("&amp;", caller);
        }

        public override string GetDeviceAttributes()
        {
            var attributes = new string[]
            {
                $"E:BranchReadinessLevel=CB",
                $"ProcessorIdentifier=Intel64%20Family%206%20Model%20158%20Stepping%209",
                $"CurrentBranch={Branch}",
                $"OEMModel=Virtual%20Machine",
                $"FlightRing={Ring}",
                $"AttrDataVer=52",
                $"InstallLanguage=en-US",
                $"OSUILocale=en-US",
                $"InstallationType=Server%20Core",
                $"FlightingBranchName={(Ring.ToUpper() == "RETAIL" ? "external" : Branch)}",
                $"IsFlightingEnabled={(Ring.ToUpper() == "RETAIL" ? "0" : "1")}",
                $"OSSkuId={Sku}",
                $"App=WU_OS",
                $"InstallDate=1546423446",
                $"ProcessorManufacturer=GenuineIntel",
                $"OEMName_Uncleaned=Microsoft%20Corporation",
                $"AppVer={Build}",
                $"OSArchitecture={Arch}",
                $"UpdateManagementGroup=2",
                $"IsDeviceRetailDemo=4294967295",
                $"TelemetryLevel=0",
                $"DefaultUserRegion=244",
                $"WuClientVer={Build}",
                $"OSVersion={Build}",
                $"DeviceFamily=Windows.Server"
            };

            return string.Join("&amp;", attributes);
        }
    }
}
