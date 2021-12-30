namespace FastEnumGenerator;
[Generator]
public partial class MySourceGenerator : IIncrementalGenerator
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
    private void Execute(Compilation compilation, ImmutableArray<MainInfo> list, SourceProductionContext context)
    {
        var others = list.Distinct();
        Emitter emit = new(compilation, others, context);
        emit.Emit();
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