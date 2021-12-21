namespace FastEnumGenerator;
[Generator] //this is important so it knows this class is a generator which will generate code for a class using it.
public class MySourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<MainInfo> declares = context.SyntaxProvider.CreateSyntaxProvider(
            (s, _) => IsSyntaxTarget(s),
            (t, _) => GetTarget(t))
            .Where(m => m != null)!;
        IncrementalValueProvider<(Compilation, ImmutableArray<MainInfo>)> compilation
            = context.CompilationProvider.Combine(declares.Collect());
        context.RegisterSourceOutput(compilation, (spc, source) =>
        {
            Execute(source.Item2, spc);
        });
    }
    private bool IsSyntaxTarget(SyntaxNode syntax)
    {
        return syntax is RecordDeclarationSyntax r
            && r.IsPartial()
            && r.IsPublic()
            && r.IsRecordStruct();
    }
    private bool IsEnumPrivate(EnumDeclarationSyntax node)
    {
        foreach (var item in node.Modifiers)
        {
            if (item.IsKind(SyntaxKind.PrivateKeyword))
            {
                return true;
            }
        }
        return false;
    }
    private MainInfo? GetTarget(GeneratorSyntaxContext context)
    {
        var record = context.GetRecordNode(); //can use the sematic model at this stage
        var symbol = context.GetRecordSymbol(record);
        var list = record.DescendantNodes().OfType<EnumDeclarationSyntax>();
        MainInfo output = new();
        output.RecordName = record.Identifier.ValueText;
        output.NameSpaceName = symbol.ContainingNamespace.ToDisplayString();
        if (record.IsReadOnly() == false)
        {
            output.NotReadOnly = true;
            return output;
        }
        if (list.Count() == 0)
        {
            return null; //because no enums.
        }
        if (list.Count() > 1)
        {
            output.TooManyInstances = true;
            return output;
        }
        var singleEnum = list.Single();
        if (IsEnumPrivate(singleEnum) == false)
        {
            output.NotPrivate = true;
            return output;
        }
        var nexts = singleEnum.DescendantNodes().OfType<EnumMemberDeclarationSyntax>().ToBasicList();
        if (nexts.Count == 0)
        {
            return output; //because there was none.  means later will raise diagnostic error
        }
        int oldValue = 0;
        foreach (var item in nexts)
        {
            var fins = item.DescendantNodes().OfType<EqualsValueClauseSyntax>().SingleOrDefault();
            if (fins is not null)
            {
                var aa = fins.Value.ToString();
                oldValue = int.Parse(aa);
            }
            EnumInfo info = new();
            info.Value = oldValue;
            info.Name = item.Identifier.ValueText;
            info.Words = info.Name.GetWords();
            output.Enums.Add(info);
            if (oldValue == 0)
            {
                output.DefaultEnum = info;
            }
            oldValue++;
        }
        if (output.DefaultEnum is null)
        {
            output.DefaultEnum = output.Enums.First();
        }
        return output;
    }
    private BasicList<string> GetStaticList(MainInfo info)
    {
        BasicList<string> output = new();
        foreach (var item in info.Enums)
        {
            output.Add("    /// <summary>");
            output.Add($"    /// value is {item.Value}");
            output.Add("    /// </summary>");
            output.Add($@"    public static {info.RecordName} {item.Name} {{get; }} = new({item.Value}, ""{item.Name}"", ""{item.Words}"");");
        }
        return output;
    }
    private BasicList<string> GetNameList(BasicList<EnumInfo> enums)
    {
        BasicList<string> output = new();
        foreach (var item in enums)
        {
            output.Add($@"        if (name == ""{item.Name}"")");
            output.Add("        {");
            output.Add($"            return {item.Name};");
            output.Add("        }");
        }
        return output;
    }
    private BasicList<string> GetValueList(BasicList<EnumInfo> enums)
    {
        BasicList<string> output = new();
        foreach (var item in enums)
        {
            output.Add($@"        if (value == {item.Value})");
            output.Add("        {");
            output.Add($"            return {item.Name};");
            output.Add("        }");
        }
        return output;
    }
    private void Execute(ImmutableArray<MainInfo> list, SourceProductionContext context)
    {
        var others = list.Distinct();
        foreach (var info in others)
        {
            if (info.RecordName.StartsWith("Enum") == false)
            {
                context.ReportDiagnostic(Diagnostic.Create(NotStartingEnum(info.RecordName), Location.None));
                return;
            }
            if (info.NotReadOnly)
            {
                context.ReportDiagnostic(Diagnostic.Create(NotReadOnly(info.RecordName), Location.None));
                return;
            }
            if (info.TooManyInstances == true)
            {
                context.ReportDiagnostic(Diagnostic.Create(TooManyEnums(info.RecordName), Location.None));
                return;
            }
            if (info.NotPrivate)
            {
                context.ReportDiagnostic(Diagnostic.Create(NeedsPrivateEnum(info.RecordName), Location.None));
                return;
            }
            if (info.Enums.Count == 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(NoEnums(info.RecordName), Location.None));
                return;
            }
            BasicList<string> staticList = GetStaticList(info);
            BasicList<string> nameList = GetNameList(info.Enums);
            BasicList<string> valueList = GetValueList(info.Enums);
            string source = $@"using System.Text.Json;
using System.Text.Json.Serialization;
namespace {info.NameSpaceName};
public partial record struct {info.RecordName} : IFastEnumList<{info.RecordName}>, IComparable<{info.RecordName}>
{{
    public static BasicList<{info.RecordName}> CompleteList {{ get; }} = new();
    BasicList<{info.RecordName}> IFastEnumList<{info.RecordName}>.CompleteList => CompleteList;
    public string Name {{ get; }} = """";
    public int Value {{ get; }}
    public string Words {{ get; }} = """";
    private {info.RecordName}(int value, string name, string words)
    {{
        Value = value;
        Name = name;
        Words = words;
        CompleteList.Add(this);
        ZAddConverter();
    }}
    public {info.RecordName}()
    {{
        Value = {info.DefaultEnum!.Value};
        Name = ""{info.DefaultEnum!.Name}"";
        Words = ""{info.DefaultEnum!.Words}"";
        ZAddConverter();
    }}
    static bool _didAdd;
    internal static void ZAddConverter()
    {{
        if (_didAdd)
        {{
            return;
        }}
        js.GetCustomJsonSerializerOptions().Converters.Add(new {info.RecordName}Converter());
        _didAdd = true;
    }}
    public override string ToString()
    {{
        return Words;
    }}
    private class {info.RecordName}Converter : JsonConverter<{info.RecordName}>
    {{
        public override {info.RecordName} Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {{
            string value = reader.GetString()!;
            return FromName(value);
        }}
        public override void Write(Utf8JsonWriter writer, {info.RecordName} value, JsonSerializerOptions options)
        {{
            if (value.IsNull)
            {{
                writer.WriteStringValue("""");
                return;
            }}
            writer.WriteStringValue(value.Name);
        }}
    }}
    public bool IsNull => string.IsNullOrWhiteSpace(Name);
    public int CompareTo({info.RecordName} other)
    {{
        return Value.CompareTo(other.Value);
    }}
{string.Join(Environment.NewLine, staticList)}
    public static {info.RecordName} FromValue(int value, bool showErrors = false)
    {{
{string.Join(Environment.NewLine, valueList)}
        if (showErrors)
        {{
            throw new Exception($""No value found for {{ value}}"");
        }}
        return default;
    }}
    public static {info.RecordName} FromName(string name, bool showErrors = false)
    {{
{string.Join(Environment.NewLine, nameList)}
        if (showErrors)
        {{
            throw new Exception($""No name found for {{ name}}"");
        }}
        return default;
    }}
    public static bool operator > ({info.RecordName} left, {info.RecordName} right)
    {{
        return left.Value > right.Value;
    }}
    public static bool operator < ({info.RecordName} left, {info.RecordName} right)
    {{
        return left.Value < right.Value;
    }}
    public static bool operator >= ({info.RecordName} left, {info.RecordName} right)
    {{
        return left.Value >= right.Value;
    }}
    public static bool operator <= ({info.RecordName} left, {info.RecordName} right)
    {{
        return left.Value <= right.Value;
    }}
}}";
            IAddSource finals = new IncrementalExecuteAddSource(context);
            finals.AddSource($"generatedSource{info.RecordName}.g", source);
        }
    }
    //these are all the possible errors that will mean you cannot even create the custom enum since rules were violated.
#pragma warning disable RS2008 // Enable analyzer release tracking
    private static DiagnosticDescriptor TooManyEnums(string recordName) => new DiagnosticDescriptor("FirstID",
#pragma warning restore RS2008 // Enable analyzer release tracking
        "Could not create enum",
        $"The record {recordName} had too many enums",
        "EnumTest",
        DiagnosticSeverity.Error,
        true
        );

#pragma warning disable RS2008 // Enable analyzer release tracking
    private static DiagnosticDescriptor NeedsPrivateEnum(string recordName) => new DiagnosticDescriptor("SecondID",
#pragma warning restore RS2008 // Enable analyzer release tracking
        "Could not create enum",
        $"The record {recordName} needs to have the enum private",
        "EnumTest",
        DiagnosticSeverity.Error,
        true
        );
#pragma warning disable RS2008 // Enable analyzer release tracking
    private static DiagnosticDescriptor NoEnums(string recordName) => new DiagnosticDescriptor("ThirdID",
#pragma warning restore RS2008 // Enable analyzer release tracking
        "Could not create enum",
        $"The record {recordName} had blank enums",
        "EnumTest",
        DiagnosticSeverity.Error,
        true
        );
#pragma warning disable RS2008 // Enable analyzer release tracking
    private static DiagnosticDescriptor NotReadOnly(string recordName) => new DiagnosticDescriptor("FourthID",
#pragma warning restore RS2008 // Enable analyzer release tracking
        "Could not create enum",
        $"The record {recordName} must be readonly",
        "EnumTest",
        DiagnosticSeverity.Error,
        true
        );
#pragma warning disable RS2008 // Enable analyzer release tracking
    private static DiagnosticDescriptor NotStartingEnum(string recordName) => new DiagnosticDescriptor("FourthID",
#pragma warning restore RS2008 // Enable analyzer release tracking
        "Could not create enum",
        $"The record {recordName} must start with words Enum",
        "EnumTest",
        DiagnosticSeverity.Error,
        true
        );
}