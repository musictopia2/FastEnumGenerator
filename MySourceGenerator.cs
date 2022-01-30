using System.Diagnostics;
namespace FastEnumGenerator;
[Generator]
public partial class MySourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
//#if DEBUG
//        if (Debugger.IsAttached == false)
//        {
//            Debugger.Launch();
//        }
//#endif
        IncrementalValuesProvider<MainInfo> declares = context.SyntaxProvider.CreateSyntaxProvider(
            (s, _) => IsSyntaxTarget(s),
            (t, _) => GetTarget(t))
            .Where(m => m != null)!;
        IncrementalValueProvider<(Compilation, ImmutableArray<MainInfo>)> compilation
            = context.CompilationProvider.Combine(declares.Collect());
        context.RegisterSourceOutput(compilation, (spc, source) =>
        {
            Execute(source.Item1, source.Item2, spc);
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
        var record = context.GetRecordNode();
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
            return null;
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
            return output;
        }
        int oldValue = 0;
        int index = 0;
        string transparentName = "#00FFFFFF";
        bool hadDefault = false;
        foreach (var item in nexts)
        {
            var fins = item.DescendantNodes().OfType<EqualsValueClauseSyntax>().SingleOrDefault();
            if (fins is not null)
            {
                var aa = fins.Value.ToString();
                oldValue = int.Parse(aa);
            }
            string name = item.Identifier.ValueText;

            string possibleColor = name.ToColor(false);
            if (possibleColor != "" && index == 0 && possibleColor != transparentName)
            {
                output.IsColor = true;
                EnumInfo temps = new();
                temps.Name = "None";
                temps.Words = "None";
                temps.Color = transparentName;
                temps.WebColor = "none";
                temps.Value = 0;
                output.DefaultEnum = temps;
                hadDefault = true;
                oldValue = 1; //should start with 1 now.
                output.Enums.Add(temps);
            }
            else if (possibleColor != "" && index == 1 && possibleColor != transparentName)
            {
                output.IsColor = true;
            }
            EnumInfo info = new();
            info.Value = oldValue;
            info.Name = item.Identifier.ValueText;
            info.Words = info.Name.GetWords();
            info.WebColor = "none";
            info.Color = transparentName;
            if (output.IsColor && possibleColor != transparentName && possibleColor != "")
            {
                info.Color = possibleColor;
                info.WebColor = possibleColor.ToWebColor();
            }
            output.Enums.Add(info);
            if (oldValue == 0 && hadDefault == false)
            {
                output.DefaultEnum = info;
            }
            oldValue++;
            index++;
        }
        if (output.DefaultEnum is null)
        {
            output.DefaultEnum = output.Enums.First();
        }
        return output;
    }
    private void Execute(Compilation compilation, ImmutableArray<MainInfo> list, SourceProductionContext context)
    {
        var others = list.Distinct();
        Emitter emit = new(compilation, others, context);
        emit.Emit();
    }
    //these are all the possible errors that will mean you cannot even create the custom enum since rules were violated.
    private static DiagnosticDescriptor TooManyEnums(string recordName) => new ("FirstID",
        "Could not create enum",
        $"The record {recordName} had too many enums",
        "EnumTest",
        DiagnosticSeverity.Error,
        true
        );

    private static DiagnosticDescriptor NeedsPrivateEnum(string recordName) => new ("SecondID",
        "Could not create enum",
        $"The record {recordName} needs to have the enum private",
        "EnumTest",
        DiagnosticSeverity.Error,
        true
        );
    private static DiagnosticDescriptor NoEnums(string recordName) => new ("ThirdID",
        "Could not create enum",
        $"The record {recordName} had blank enums",
        "EnumTest",
        DiagnosticSeverity.Error,
        true
        );
    private static DiagnosticDescriptor NotReadOnly(string recordName) => new ("FourthID",
        "Could not create enum",
        $"The record {recordName} must be readonly",
        "EnumTest",
        DiagnosticSeverity.Error,
        true
        );
    private static DiagnosticDescriptor NotStartingEnum(string recordName) => new ("FourthID",
        "Could not create enum",
        $"The record {recordName} must start with words Enum",
        "EnumTest",
        DiagnosticSeverity.Error,
        true
        );
}