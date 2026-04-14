
/*
* Copyright (C) 2026 UsiFX. All rights reserved.
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalysisTool.Core;

// Result of a compilation attempt
public class CompileResult
{
    public bool Success { get; set; }
    public Assembly? Assembly { get; set; }
    public IReadOnlyList<string> Errors { get; set; } = [];

    // Allow tuple deconstruction for backwards compatibility
    public void Deconstruct(out bool success, out Assembly? assembly, out IReadOnlyList<string> errors)
    {
        success = Success;
        assembly = Assembly;
        errors = Errors;
    }
}

// Compiles C# source code in-memory using Roslyn, producing an in-memory assembly
public class CompilerService
{
    // Runtime reference list from the current .NET runtime.
    private static readonly IReadOnlyList<MetadataReference> RuntimeReferences =
        LoadRuntimeReferences();

/*
    * Compiles the provided C# source code into an in-memory assembly.
    * sourceCode: C# source code to compile.
    * assemblyName: Optional name for the resulting assembly.
    * sourcePath: Path used in diagnostic messages (e.g. the file name).
    *             Defaults to <input> for in-memory code.
    * Returns: A CompileResult containing the success flag, compiled Assembly or null, and any diagnostic errors.
    */
    public CompileResult Compile(
        string sourceCode,
        string assemblyName = "UserCode",
        string sourcePath   = "<input>")
    {
        if (string.IsNullOrWhiteSpace(sourceCode))
            return new CompileResult 
            { 
                Success = false, 
                Assembly = null, 
                Errors = [$"{sourcePath}: error: source code is empty"] 
            };

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode, path: sourcePath);

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees: [syntaxTree],
            references: RuntimeReferences,
            options: new CSharpCompilationOptions(OutputKind.ConsoleApplication));

        using var ms = new MemoryStream();
        Microsoft.CodeAnalysis.Emit.EmitResult result = compilation.Emit(ms);

        if (!result.Success)
        {
            var errors = result.Diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => FormatDiagnostic(d, sourcePath))
                .ToList();

            return new CompileResult 
            { 
                Success = false, 
                Assembly = null, 
                Errors = errors 
            };
        }

        ms.Seek(0, SeekOrigin.Begin);
        Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
        return new CompileResult 
        { 
            Success = true, 
            Assembly = assembly, 
            Errors = [] 
        };
    }

    private static IReadOnlyList<MetadataReference> LoadRuntimeReferences()
    {
        var referencePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        string? tpa = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
        if (!string.IsNullOrWhiteSpace(tpa))
        {
            foreach (string path in tpa.Split(Path.PathSeparator))
            {
                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                    referencePaths.Add(path);
            }
        }

        // Add already loaded assemblies as a fallback when TPA is unavailable.
        foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (asm.IsDynamic)
                continue;

            string? location = asm.Location;
            if (!string.IsNullOrWhiteSpace(location) && File.Exists(location))
                referencePaths.Add(location);
        }

        // Include all runtime assemblies from the current runtime directory.
        string runtimeDir = RuntimeEnvironment.GetRuntimeDirectory();
        if (!string.IsNullOrWhiteSpace(runtimeDir) && Directory.Exists(runtimeDir))
        {
            foreach (string dll in Directory.EnumerateFiles(runtimeDir, "*.dll"))
                referencePaths.Add(dll);
        }

        if (referencePaths.Count == 0)
            throw new InvalidOperationException("Could not load runtime assembly references.");

        return referencePaths
            .Select(path => MetadataReference.CreateFromFile(path))
            .ToArray();
    }

// Format a Roslyn diagnostic as GCC-style: file:line:col: error: message
    private static string FormatDiagnostic(Diagnostic d, string sourcePath)
    {
        FileLinePositionSpan span = d.Location.GetLineSpan();
        string file = string.IsNullOrEmpty(span.Path) ? sourcePath : span.Path;
        int line = span.StartLinePosition.Line + 1;
        int col  = span.StartLinePosition.Character + 1;
        return $"{file}:{line}:{col}: error: {d.GetMessage()}";
    }
}