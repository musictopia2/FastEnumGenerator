namespace FastEnumGenerator;
public partial class MySourceGenerator
{
    private partial class Emitter
    {
        private class Writer
        {
            private readonly MainInfo _info;
            public Writer(MainInfo info)
            {
                _info = info;
            }
            private void WriteConstructors(ICodeBlock w)
            {
                w.WriteLine(w =>
                {
                    w.Write("private ")
                    .Write(_info.RecordName);
                    if (_info.IsColor == false)
                    {
                        w.Write("(int value, string name, string words)");
                    }
                    else
                    {
                        w.Write("(int value, string name, string words, string color, string webColor)");
                    }
                }).WriteCodeBlock(w =>
                {
                    w.WriteLine("Value = value;")
                    .WriteLine("Name = name;")
                    .WriteLine("Words = words;");
                    w.WriteLine(w =>
                    {
                        w.Write("if (Name != ")
                        .AppendDoubleQuote("None")
                        .Write(")");
                    })
                    .WriteCodeBlock(w =>
                    {
                        w.WriteLine("CompleteList.Add(this);"); //i cannot think of any situation where the enum list should include None.  None represents nothing being chosen.
                    });
                    if (_info.IsColor)
                    {
                        //add more.
                        w.WriteLine("Color = color;")
                        .WriteLine("WebColor = webColor;")
                        .WriteLine(w =>
                        {
                            w.Write("if (webColor != ")
                            .AppendDoubleQuote("none")
                            .Write(")");
                        })
                        .WriteCodeBlock(w =>
                        {
                            w.WriteLine("ColorList.Add(this);");
                        });
                    }
                }).WriteLine(w =>
                {
                    w.Write("public ")
                    .Write(_info.RecordName)
                    .Write("()");
                }).WriteCodeBlock(w =>
                {
                    w.WriteLine(w =>
                    {
                        w.Write("Value = ")
                        .Write(_info.DefaultEnum!.Value)
                        .Write(";");
                    }).WriteLine(w =>
                    {
                        w.Write("Name = ")
                        .AppendDoubleQuote(w => w.Write(_info.DefaultEnum!.Name)).Write(";");
                    }).WriteLine(w =>
                    {
                        w.Write("Words = ")
                        .AppendDoubleQuote(w => w.Write(_info.DefaultEnum!.Words)).Write(";");
                    });
                    if (_info.IsColor)
                    {
                        w.WriteLine(w =>
                        {
                            w.Write("Color = ")
                            .AppendDoubleQuote("#00FFFFFF").Write(";");
                        })
                        .WriteLine(w =>
                        {
                            w.Write("WebColor = ")
                            .AppendDoubleQuote("none").Write(";");
                        });
                    }
                });
            }
            private void WriterAddConverterMethod(ICodeBlock w)
            {
                w.WriteLine("internal static void ZAddConverter()")
                    .WriteCodeBlock(w =>
                    {
                        w.WriteLine(w =>
                        {
                            w.Write("global::CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.ConvertersHelpers.AddConverter")
                            .SingleGenericWrite(AddConverter)
                            .Write("();");
                        });
                    });
            }
            private void WriteConverterClass(ICodeBlock w)
            {
                w.WriteLine(w =>
                {
                    w.Write("private class ");
                    AddConverter(w);
                    w.Write(": global::System.Text.Json.Serialization.JsonConverter")
                    .SingleGenericWrite(_info.RecordName);
                }).WriteCodeBlock(w =>
                {
                    w.WriteLine(w =>
                    {
                        w.Write("public override ")
                        .Write(_info.RecordName)
                        .Write(" Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)");
                    }).WriteCodeBlock(w =>
                    {
                        w.WriteLine("string value = reader.GetString()!;")
                        .WriteLine("return FromName(value);");
                    }).WriteLine(w =>
                    {
                        w.Write("public override void Write(global::System.Text.Json.Utf8JsonWriter writer, ")
                        .Write(_info.RecordName)
                        .Write(" value, global::System.Text.Json.JsonSerializerOptions options)");
                    }).WriteCodeBlock(w =>
                    {
                        w.WriteLine("if (value.IsNull)")
                        .WriteCodeBlock(w =>
                        {
                            w.WriteLine(w =>
                            {
                                w.Write("writer.WriteStringValue(")
                                .AppendDoubleQuote(w => w.Write(""))
                                .Write(");");
                            }).WriteLine("return;");
                        })
                        .WriteLine("writer.WriteStringValue(value.Name);");
                    });
                });
            }
            private void AddConverter(IWriter writer)
            {
                writer.Write(_info.RecordName)
                    .Write("Converter");
            }
            private void WritePartialRecordStructBeginnings(IWriter w)
            {
                w.Write("public partial record struct ")
                    .Write(_info.RecordName)
                    .Write(" : ")
                    .BasicProcessesWrite();
                if (_info.IsColor == false)
                {
                    w.Write("IFastEnumList");
                }
                else
                {
                    w.Write("IFastEnumColorList");
                }
                w.SingleGenericWrite(_info.RecordName)
                .Write(", ")
                .SystemWrite()
                .Write("IComparable")
                .SingleGenericWrite(_info.RecordName);
            }
            private void WriteCustomCollectionProcesses(ICodeBlock w)
            {
                w.WriteLine(w =>
                {
                    w.Write("public static ")
                    .SingleCollectionInfoWrite(_info.RecordName)
                    .Write(" CompleteList { get; } = new();");
                })
                .WriteLine(w =>
                {
                    w.SingleCollectionInfoWrite(_info.RecordName)
                    .Write(" ")
                    .BasicProcessesWrite()
                    .Write("IFastEnumList")
                    .SingleGenericWrite(_info.RecordName)
                    .Write(".CompleteList => CompleteList;");
                });
                if (_info.IsColor)
                {
                    w.WriteLine(w =>
                    {
                        w.Write("public static ")
                        .SingleCollectionInfoWrite(_info.RecordName)
                        .Write(" ColorList { get; } = new();");
                    })
                    .WriteLine(w =>
                    {
                        w.SingleCollectionInfoWrite(_info.RecordName)
                       .Write(" ")
                       .BasicProcessesWrite()
                       .Write("IFastEnumColorList")
                       .SingleGenericWrite(_info.RecordName)
                       .Write(".ColorList => ColorList;");
                    });
                }
            }
            private void WriteProperties(ICodeBlock w)
            {
                w.WriteLine(w =>
                {
                    w.Write("public string Name { get; }")
                    .EmptyEqualsEndString();
                })
                .WriteLine("public int Value { get; }")
                .WriteLine(w => w.Write("public string Words { get; }").EmptyEqualsEndString());
                if (_info.IsColor)
                {
                    w.WriteLine(w =>
                    {
                        w.Write("public string Color { get; } = ")
                        .AppendDoubleQuote("#00FFFFFF")
                        .Write(";");
                    })
                    .WriteLine(w =>
                    {
                        w.Write("public string WebColor { get; } = ")
                        .AppendDoubleQuote("none")
                        .Write(";");
                    });
                }
            }
            private void WriteCompare(ICodeBlock w)
            {
                w.WriteLine(w =>
                 {
                     w.Write("public int CompareTo(")
                     .Write(_info.RecordName)
                     .Write(" other)");
                 }).WriteCodeBlock(w =>
                 {
                     w.WriteLine("return Value.CompareTo(other.Value);");
                 });
            }
            public string ProcessSingleInfo()
            {
                SourceCodeStringBuilder builder = new();
                builder.WriteLine(w =>
                {
                    w.Write("namespace ")
                    .Write(_info.NameSpaceName).Write(";");
                })
                .WriteLine(w =>
                {
                    WritePartialRecordStructBeginnings(w);
                }).WriteCodeBlock(w =>
                {
                    WriteCustomCollectionProcesses(w);
                    WriteProperties(w);
                    WriteConstructors(w);
                    w.WriteLine("public override string ToString()")
                    .WriteCodeBlock(w =>
                    {
                        w.WriteLine("return Words;");
                    });
                    WriterAddConverterMethod(w);
                    WriteConverterClass(w);
                    w.WriteLine("public bool IsNull => string.IsNullOrWhiteSpace(Name);");
                    WriteCompare(w);
                    WriteStaticEnumMembers(w);
                    WriteFromValue(w);
                    WriteFromName(w);
                    WriteOperator(w, ">");
                    WriteOperator(w, "<");
                    WriteOperator(w, ">=");
                    WriteOperator(w, "<=");
                });
                return builder.ToString();
            }
            private void WriteStaticEnumMembers(ICodeBlock w)
            {
                //for double quote, have the ability to also be simple string (not always action)
                //because we don't always have something.
                //even the possibility of nothing (which means its empty).
                foreach (var item in _info.Enums)
                {
                    w.WriteLine("/// <summary>")
                        .WriteLine(w =>
                        {
                            w.Write("/// value is ")
                            .Write(item.Value);
                        }).
                        WriteLine("/// </summary>")
                        .WriteLine(w =>
                        {
                            w.Write("public static ")
                            .Write(_info.RecordName)
                            .Write(" ")
                            .Write(item.Name)
                            .Write("{ get; } = new(")
                            .Write(item.Value)
                            .Write(", ")
                            .AppendDoubleQuote(w =>
                            {
                                w.Write(item.Name);
                            }).Write(", ")
                            .AppendDoubleQuote(w =>
                            {
                                w.Write(item.Words);
                            });
                            if (_info.IsColor == false)
                            {
                                w.Write(");");
                            }
                            else
                            {
                                w.Write(", ")
                                .AppendDoubleQuote(w =>
                                {
                                    w.Write(item.Color);
                                }).Write(", ")
                                .AppendDoubleQuote(w =>
                                {
                                    w.Write(item.WebColor);
                                }).Write(");");
                            }
                        });
                }
            }
            private void WriteFromValue(ICodeBlock w)
            {
                w.WriteLine(w =>
                {
                    w.Write("public static ")
                    .Write(_info.RecordName)
                    .Write(" FromValue(int value, bool showErrors = false)");
                }).WriteCodeBlock(w =>
                {
                    foreach (var item in _info.Enums)
                    {
                        w.WriteLine(w =>
                        {
                            w.Write("if (value == ")
                            .Write(item.Value).Write(")");
                        }).WriteCodeBlock(w =>
                        {
                            w.WriteLine(w =>
                            {
                                w.Write("return ")
                                .Write(item.Name).Write(";");
                            });
                        });
                    }
                    w.WriteLine("if (showErrors)")
                    .WriteCodeBlock(w =>
                    {
                        w.WriteLine(w =>
                        {
                            w.CustomExceptionLine(w =>
                            {
                                w.Write("No value found for { value}");
                            });
                        });
                    }).WriteLine("return default;");
                });
            }
            private void WriteFromName(ICodeBlock w)
            {
                w.WriteLine(w =>
                {
                    w.Write("public static ")
                    .Write(_info.RecordName)
                    .Write(" FromName(string name, bool showErrors = false)");
                }).WriteCodeBlock(w =>
                {
                    foreach (var item in _info.Enums)
                    {
                        w.WriteLine(w =>
                        {
                            w.Write("if (name == ")
                            .AppendDoubleQuote(w => w.Write(item.Name)).Write(")");
                        }).WriteCodeBlock(w =>
                        {
                            w.WriteLine(w =>
                            {
                                w.Write("return ")
                                .Write(item.Name).Write(";");
                            });
                        });
                    }
                    w.WriteLine("if (showErrors)")
                    .WriteCodeBlock(w =>
                    {
                        w.WriteLine(w =>
                        {
                            w.CustomExceptionLine(w =>
                            {
                                w.Write("No name found for { name};");
                            });
                        });
                    }).WriteLine("return default;");
                });
            }
            private void WriteOperator(ICodeBlock w, string methodOperator)
            {
                w.WriteLine(w =>
                {
                    w.Write("public static bool operator ")
                    .Write(methodOperator)
                    .Write("(")
                    .Write(_info.RecordName)
                    .Write(" left, ")
                    .Write(_info.RecordName)
                    .Write(" right)");
                }).WriteCodeBlock(w =>
                {
                    w.WriteLine(w =>
                    {
                        w.Write("return left.Value ")
                        .Write(methodOperator)
                        .Write(" right.Value;");
                    });
                });
            }
        }
    }
}