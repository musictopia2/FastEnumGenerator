namespace FastEnumGenerator;
public partial class MySourceGenerator
{
    private partial class Emitter
    {
        private readonly Compilation _compilation;
        private readonly IEnumerable<MainInfo> _list;
        private readonly SourceProductionContext _context;
        public Emitter(Compilation compilation, IEnumerable<MainInfo> list, SourceProductionContext context)
        {
            _compilation = compilation;
            _list = list;
            _context = context;
        }
        public void Emit()
        {
            if (_list.Count() == 0)
            {
                return;
            }
            foreach (var info in _list)
            {
                if (info.RecordName.StartsWith("Enum") == false)
                {
                    _context.ReportDiagnostic(Diagnostic.Create(NotStartingEnum(info.RecordName), Location.None));
                    return;
                }
                if (info.NotReadOnly)
                {
                    _context.ReportDiagnostic(Diagnostic.Create(NotReadOnly(info.RecordName), Location.None));
                    return;
                }
                if (info.TooManyInstances == true)
                {
                    _context.ReportDiagnostic(Diagnostic.Create(TooManyEnums(info.RecordName), Location.None));
                    return;
                }
                if (info.NotPrivate)
                {
                    _context.ReportDiagnostic(Diagnostic.Create(NeedsPrivateEnum(info.RecordName), Location.None));
                    return;
                }
                if (info.Enums.Count == 0)
                {
                    _context.ReportDiagnostic(Diagnostic.Create(NoEnums(info.RecordName), Location.None));
                    return;
                }
                Writer writes = new(info);
                string text = writes.ProcessSingleInfo();
                _context.AddSource($"generatedSource{info.RecordName}.g", text);
            }
            GlobalConverterProcesses();
        }
        private void GlobalConverterProcesses()
        {
            string ns = $"{_compilation.AssemblyName!}.JsonConverterProcesses";
            SourceCodeStringBuilder builder = new();
            builder.WriteLine(w =>
            {
                w.Write("namespace ")
                .Write(ns)
                .Write(";");
            });
            builder.WriteLine("public static class GlobalJsonConverterClass");
            builder.WriteCodeBlock(w =>
            {
                w.WriteLine("public static void AddEnumConverters()");
                w.WriteCodeBlock(w =>
                {
                    WriteGlobalConverters(w);
                });
            });
            _context.AddSource("generatedglobal.g", builder.ToString());
        }
        private void WriteGlobalConverters(ICodeBlock w)
        {
            foreach (var info in _list)
            {
                w.WriteLine(w =>
                {
                    w.Write(info.RecordName)
                    .Write(".ZAddConverter();");
                });
            }
        }
    }
}