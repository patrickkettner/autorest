// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// 
using AutoRest.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoRest.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.Rest.CSharp.Compiler.Compilation;
using Xunit.Abstractions;
using OutputKind = Microsoft.Rest.CSharp.Compiler.Compilation.OutputKind;

namespace AutoRest.CSharp.Unit.Tests
{
    public class BugTest
    {
        private ITestOutputHelper _output;
        internal static string[] SuppressWarnings = {"CS1701", "CS1591" , "CS1573"};
        //Todo: Remove CS1591 when issue https://github.com/Azure/autorest/issues/1387 is fixed


        internal static string[] VsCode = new string[] {
            @"C:\Program Files (x86)\Microsoft VS Code Insiders\Code - Insiders.exe",
            @"C:\Program Files (x86)\Microsoft VS Code\Code.exe"
        };

        
        internal static char Q = '"';
        internal static string Quote(string text) => $"{Q}{text}{Q}";

        /// <summary>
        ///  Tries to run VSCode 
        /// </summary>
        /// <param name="args"></param>
        internal bool StartVsCode(params object[] args)
        {
            ProcessStartInfo startInfo = null;
            foreach (var exe in VsCode)
            {
                if (File.Exists(exe))
                {
                    startInfo =
                        new ProcessStartInfo(exe,
                            args.Aggregate(
                                // $@"""{Path.Combine(exe, @"..\resources\app\out\cli.js")}""",
                                "",
                                (s, o) => $"{s} {Q}{o}{Q}"));
                    startInfo.EnvironmentVariables.Add("ATOM_SHELL_INTERNAL_RUN_AS_NODE", "1");
                    startInfo.UseShellExecute = false;
                    break;
                }
            }
            if (startInfo != null)
            {
                return Process.Start(startInfo) != null;
            }

            return false;
        }

        internal void ShowGeneratedCode(IFileSystem fileSystem)
        {
            InspectWithFavoriteCodeEditor(fileSystem.SaveFilesToTemp(GetType().Name));
        }

        internal void InspectWithFavoriteCodeEditor(string folder, FileLinePositionSpan? span = null)
        {
            if (span != null)
            {
                FileLinePositionSpan s = (FileLinePositionSpan)span;
                // when working locally on windows we can pop up vs code to see if the code failure.
                if (!StartVsCode(
                    folder,
                    "-g",
                    $"{Path.Combine(folder, s.Path)}:{s.StartLinePosition.Line + 1}:{s.StartLinePosition.Character + 1}"))
                {
                    // todo: add code here to try another editor?
                }
            }
            else
            {
                StartVsCode(folder);
            }
        }
        public BugTest(ITestOutputHelper output)
        {
            _output = output;
        }

        public BugTest()
        {
        }

        protected virtual MemoryFileSystem CreateMockFilesystem()
        {
            var fs = new MemoryFileSystem();
            fs.CopyFile(Path.Combine("Resource", "AutoRest.json"), "AutoRest.json");
            return fs;
        }

        protected virtual MemoryFileSystem GenerateCodeForTestFromSpec(string codeGenerator = "CSharp", string modeler = "Swagger")
        {
           return GenerateCodeForTestFromSpec($"{GetType().Name}", codeGenerator, modeler);
        }

        protected virtual MemoryFileSystem GenerateCodeForTestFromSpec(string dirName, string codeGenerator="CSharp", string modeler = "Swagger")
        {
            var fs = CreateMockFilesystem();
            dirName.GenerateCodeInto(fs, codeGenerator, modeler);
            return fs;
        }

        protected virtual void WriteLine(object value)
        {
            if (value != null)
            {
                _output?.WriteLine(value.ToString());
                Debug.WriteLine(value.ToString());
            }
            else
            {
                _output?.WriteLine("<null>");
                Debug.WriteLine("<null>");
            }
        }

        protected virtual void WriteLine(string format, params object[] values)
        {
            if (format != null)
            {
                if (values != null && values.Length > 0)
                {
                    _output?.WriteLine(format, values);
                    Debug.WriteLine(format, values);
                }
                else
                {
                    _output?.WriteLine(format);
                    Debug.WriteLine(format);
                }
            }
            else
            {
                _output?.WriteLine("<null>");
                Debug.WriteLine("<null>");
            }
        }

        protected void Write(IEnumerable<Diagnostic> messages, MemoryFileSystem fileSystem)
        {
            if (messages.Any())
            {
                foreach (var file in messages.GroupBy(each => each.Location?.SourceTree?.FilePath, each => each))
                {
                    var text = file.Key != null ? fileSystem.VirtualStore[file.Key].ToString() : string.Empty;

                    foreach (var error in file)
                    {
                        WriteLine(error.ToString());
                        // WriteLine(text.Substring(error.Location.SourceSpan.Start, error.Location.SourceSpan.Length));
                    }
                }
            }
        }

        protected async Task<CompilationResult> Compile(IFileSystem fileSystem)
        {
            string dllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var assemblies = new[]
                        {
                            Path.Combine(dllPath, "Microsoft.Rest.ClientRuntime.dll"),
                            Path.Combine(dllPath, "Microsoft.Rest.ClientRuntime.Azure.dll")
                        };
            assemblies = assemblies.ToList().Concat(System.IO.Directory.GetFiles(dllPath, "*.dll", System.IO.SearchOption.TopDirectoryOnly).Where(f => Path.GetFileName(f).StartsWith("Microsoft.AspNetCore."))).ToArray();
                
            var compiler = new CSharpCompiler(
                fileSystem.GetFiles("GeneratedCode", "*.cs", SearchOption.AllDirectories)
                    .Select(each => new KeyValuePair<string, string>(each, fileSystem.ReadFileAsText(each))).ToArray(),
                ManagedAssets.FrameworkAssemblies.Concat(
                    AppDomain.CurrentDomain.GetAssemblies()
                        .Where(each => !each.IsDynamic && !string.IsNullOrEmpty(each.Location) )
                        .Select(each => each.Location)
                        .Concat(assemblies)
                    ));
            
            var result = await compiler.Compile(OutputKind.DynamicallyLinkedLibrary);
            
            // if it failed compiling and we're in an interactive session
            if (!result.Succeeded && System.Environment.OSVersion.Platform == PlatformID.Win32NT && System.Environment.UserInteractive)
            {
                var error = result.Messages.FirstOrDefault(each => each.Severity == DiagnosticSeverity.Error);
                if (error != null)
                {
                    // use this to dump the files to disk for examination
                    // open in Favorite Code Editor
                    InspectWithFavoriteCodeEditor(fileSystem.SaveFilesToTemp(GetType().Name), error.Location.GetMappedLineSpan());
                }
            }

            return result;
        }
    }
}