using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BuildChecker.Classes.DeviceBuilderExtensions
{
    public class MobileBuilderExtension : BuilderExtension
    {
        public MobileBuilderExtension(string Branch, string Build, string Arch, string Flight, string Ring)
            : base(Branch, Build, Arch, Flight, Ring, "")
        {       }

        public override string GetProducts()
        {
            var products = "";

            if (Branch == "rs2_release")
                products = "PN=Mobile.OS.rs2."+Arch+"&amp;V=10.0.15063.1000;PN=Mobile.BSP.NOKIA.RM-1062&amp;V=02177.00000.15184.36001;";
            else if (Branch == "feature2")
                products = "PN=Mobile.OS.rs2."+Arch+"&amp;V=10.0.15254.1;PN=Mobile.BSP.HP.FalconBaseUnit&amp;V=0002.0000.0037.0001";

            return products;
        }

        public override string GetDeviceAttributes()
        {
            var attributes = new string[0];

            if (Branch == "rs2_release")
            {
                attributes = new string[]
                {
                    $"BranchReadinessLevel=CB",
                    $"CurrentBranch=RS2_RELEASE",
                    $"OEMModel=RM-1062_1017",
                    $"FlightRing=Retail",
                    $"AttrDataVer=9",
                    $"InstallLanguage=en-US",
                    $"OSUILocale=en-US",
                    $"InstallationType=FactoryOS",
                    $"FirmwareVersion=02177.00000.15184.36001",
                    $"OSSkuId=104",
                    $"App=WPShift",
                    $"OEMName_Uncleaned=NOKIA",
                    $"AppVer=1.2.3.4",
                    $"ReleaseType=Test",
                    $"PhoneTargetingName=Lumia 640 XL LTE",
                    $"OSArchitecture={Arch}",
                    $"UpdateManagementGroup=0",
                    $"IsFlightingEnabled=0",
                    $"IsDeviceRetailDemo=0",
                    $"TelemetryLevel=3",
                    $"MobileOperatorCommercialized=000-IT",
                    $"OSVersion=10.0.15063.1000",
                    $"DeviceFamily=Windows.Mobile"
                };
            }
            else if (Branch == "feature2")
            {
                attributes = new string[]
                {
                    $"CurrentBranch=FEATURE2_RS3SVC",
                    $"OEMModel=Elite x3",
                    $"FlightRing=Retail",
                    $"AttrDataVer=27",
                    $"InstallLanguage=it-IT",
                    $"OSUILocale=it-IT",
                    $"InstallationType=FactoryOS",
                    $"FirmwareVersion=0002.0000.0037.0001",
                    $"OSSkuId=104",
                    $"App=WPShift",
                    $"ProcessorManufacturer=Qualcomm Inc",
                    $"OEMName_Uncleaned=HP",
                    $"InstallDate=0",
                    $"AppVer=1.2.3.4",
                    $"ReleaseType=Production", //No more test packages for Feature2.
                    $"PhoneTargetingName=Elite x3",
                    $"OSArchitecture={Arch}",
                    $"UpdateManagementGroup=0",
                    $"IsFlightingEnabled=0",
                    $"IsDeviceRetailDemo=0",
                    $"TelemetryLevel=1",
                    $"WuClientVer=10.0.15254.1",
                    $"MobileOperatorCommercialized=000-IT",
                    $"OSVersion=10.0.15254.1",
                    $"DeviceFamily=Windows.Mobile"
                };
            }
            return string.Join(';', attributes);
        }
    }
}
