namespace FastEnumGenerator;
internal class MainInfo
{
    public BasicList<EnumInfo> Enums { get; set; } = new();
    public string RecordName { get; set; } = ""; //needs this (especially for testing).
    public bool TooManyInstances { get; set; }
    public bool NotPrivate { get; set; }
    public bool NotReadOnly { get; set; }
    public string NameSpaceName { get; set; } = "";
    public EnumInfo? DefaultEnum { get; set; }
}