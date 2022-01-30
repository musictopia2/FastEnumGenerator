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
    public bool IsColor { get; set; } //has to be smart enough to see if its a color.  has to check the first one.
    //because if the first one is color, then needs to start with none.
    //on the other hand, if i specify none, then needs to allow that as well.
    //if i specify none, then the second one has to check for color.

}