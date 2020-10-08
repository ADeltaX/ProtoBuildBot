using System;
using System.Xml.Serialization;

namespace BuildChecker.Classes.Structures
{
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    [XmlRoot(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
    public partial class CompDB
    {
        public CompDBTags Tags { get; set; }

        [XmlArrayItem("Feature", IsNullable = false)]
        public CompDBFeature[] Features { get; set; }

        [XmlArrayItem("Package", IsNullable = false)]
        public PkgType[] Packages { get; set; }

        [XmlArrayItem("ConditionalFeature", IsNullable = false)]
        public ConditionalFeatureType[] MSConditionalFeatures { get; set; }

        [XmlElement("AppX")]
        public AppXPkg AppX { get; set; }

        [XmlAttribute]
        public string CreatedDate { get; set; }

        [XmlAttribute]
        public string Revision { get; set; }

        [XmlAttribute]
        public string SchemaVersion { get; set; }

        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public string Product { get; set; }

        [XmlAttribute]
        public string ReleaseType { get; set; }

        [XmlAttribute]
        public string BuildID { get; set; }

        [XmlAttribute]
        public string BuildInfo { get; set; }

        [XmlAttribute]
        public string OSVersion { get; set; }

        [XmlAttribute]
        public string BuildArch { get; set; }

        [XmlAttribute]
        public string TargetBuildID { get; set; }

        [XmlAttribute]
        public string TargetBuildInfo { get; set; }

        [XmlAttribute]
        public string TargetOSVersion { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public partial class CompDBTags
    {
        [XmlElement("Tag")]
        public TagItemType[] Tag { get; set; }

        [XmlAttribute]
        public CompDBTagsType Type { get; set; }

        [XmlIgnore]
        public bool TypeSpecified { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public partial class AppXPkg
    {
        [XmlElement("AppXPackages")]
        public AppXPackage AppXPackage { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public partial class AppXPackage
    {
        [XmlElement("Package")]
        public PkgType[] Package { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public partial class TagItemType
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Value { get; set; }
    }

    [Serializable]   
    [XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public partial class ConditionType
    {
        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public string Operator { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Value { get; set; }

        [XmlAttribute]
        public string RegistryKey { get; set; }

        [XmlAttribute]
        public string RegistryKeyType { get; set; }

        [XmlAttribute]
        public string FMID { get; set; }

        [XmlAttribute]
        public string FeatureStatus { get; set; }

        [XmlAttribute]
        public string Status { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public partial class ConditionSetType
    {
        [XmlArrayItem("Condition", IsNullable = false)]
        public ConditionType[] Conditions { get; set; }

        [XmlArrayItem("ConditionSet", IsNullable = false)]
        public ConditionSetType[] ConditionSets { get; set; }

        [XmlAttribute]
        public string Operator { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public partial class ConditionalFeatureType
    {
        public ConditionSetType ConditionSet { get; set; }

        public ConditionType Condition { get; set; }

        [XmlAttribute]
        public string FeatureID { get; set; }

        [XmlAttribute]
        public string FMID { get; set; }

        [XmlAttribute]
        public string AppXID { get; set; }

        [XmlAttribute]
        public string OwnerType { get; set; }

        [XmlAttribute]
        public string UpdateAction { get; set; }

        [XmlAttribute]
        public string InstallAction { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public partial class PayloadItemType
    {
        [XmlAttribute]
        public string PayloadHash { get; set; }

        [XmlAttribute]
        public long PayloadSize { get; set; }

        [XmlIgnore()]
        public bool PayloadSizeSpecified { get; set; }

        [XmlAttribute]
        public string Path { get; set; }

        [XmlAttribute]
        public string PreviousPath { get; set; }

        [XmlAttribute]
        public string PayloadType { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public partial class PkgType
    {
        [XmlArrayItem("PayloadItem", IsNullable = false)]
        public PayloadItemType[] Payload { get; set; }

        [XmlAttribute]
        public string ID { get; set; }

        [XmlAttribute]
        public string FullName { get; set; }

        [XmlAttribute]
        public string Path { get; set; }

        [XmlAttribute]
        public string Partition { get; set; }

        [XmlAttribute]
        public string Version { get; set; }

        [XmlAttribute]
        public string ReleaseType { get; set; }

        [XmlAttribute]
        public string OwnerType { get; set; }

        [XmlAttribute]
        public string SatelliteType { get; set; }

        [XmlAttribute]
        public string SatelliteValue { get; set; }

        [XmlAttribute]
        public bool Encrypted { get; set; }

        [XmlIgnore()]
        public bool EncryptedSpecified { get; set; }

        [XmlAttribute]
        public string PublicKeyToken { get; set; }

        [XmlAttribute]
        public bool BinaryPartition { get; set; }

        [XmlIgnore()]
        public bool BinaryPartitionSpecified { get; set; }

        [XmlAttribute]
        public bool UserInstallable { get; set; }

        [XmlIgnore()]
        public bool UserInstallableSpecified { get; set; }

        [XmlAttribute]
        public string SourceFMFile { get; set; }

        [XmlAttribute]
        public string BuildArchOverride { get; set; }

        [XmlAttribute]
        public ulong InstalledSize { get; set; }

        [XmlIgnore()]
        public bool InstalledSizeSpecified { get; set; }

        [XmlAttribute]
        public ulong StagedSize { get; set; }

        [XmlIgnore()]
        public bool StagedSizeSpecified { get; set; }

        [XmlAttribute]
        public ulong CompressedSize { get; set; }

        [XmlIgnore()]
        public bool CompressedSizeSpecified { get; set; }

        [XmlAttribute]
        public string Type { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public partial class FeaturePkgType
    {
        [XmlAttribute]
        public string ID { get; set; }

        [XmlAttribute]
        public bool FIP { get; set; }

        [XmlIgnore()]
        public bool FIPSpecified { get; set; }

        [XmlAttribute]
        public string UpdateType { get; set; }

        [XmlAttribute]
        public string PackageType { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public enum CompDBTagsType
    {
        Edition,
        Neutral,
        Language,
        Tools,
        DeploymentCab,
        Internal,
        Retail,
        BSP,
        CumulativeUpdate,
        Baseless,
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public partial class CompDBFeature
    {
        [XmlArrayItem("Package", IsNullable = false)]
        public FeaturePkgType[] Packages { get; set; }

        [XmlAttribute]
        public string FeatureID { get; set; }

        [XmlAttribute]
        public string FMID { get; set; }

        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public string Group { get; set; }
    }
}
