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

using System.Reflection;
using CodeAnalysisTool.Core;

// Program name used in all diagnostic messages
const string Prog    = "csanalysis";
const string Version = "1.0.0";

/* 
 * Parse command-line arguments to extract flags (--verbose, --quiet, --version, --help)
 * and the required input file path. Exit early with appropriate status if help/version
 * is requested or if required arguments are missing
 */
bool verbose = false;
bool quiet   = false;
string? filePath = null;

for (int i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "-h":
        case "--help":
            PrintHelp();
            return 0;

        case "-V":
        case "--version":
            PrintVersion();
            return 0;

        case "-v":
        case "--verbose":
            verbose = true;
            break;

        case "-q":
        case "--quiet":
            quiet = true;
            break;

        case "--":
            // End of options; next argument is the file.
            if (i + 1 < args.Length)
                filePath = args[++i];
            break;

        default:
            if (args[i].StartsWith('-'))
            {
                Fatal($"unrecognized option '{args[i]}'");
                PrintTryHelp();
                return 1;
            }
            filePath = args[i];
            break;
    }
}

if (filePath is null)
{
    Fatal("no input file");
    PrintTryHelp();
    return 1;
}

/* 
 * Initialize the analysis pipeline: CodeReader loads source from disk, SyntaxChecker
 * validates syntax, CompilerService compiles to IL, and ExecutionEngine runs the code
 */
var codeReader    = new CodeReader();
var syntaxChecker = new SyntaxChecker();
var compiler      = new CompilerService();
var engine        = new ExecutionEngine();

/* 
 * Load the source file from the specified path. Handle missing files
 * and read errors gracefully with meaningful error messages
 */
string sourceCode;
try
{
    sourceCode = codeReader.ReadFile(filePath);
    Info($"read {sourceCode.Length} bytes from '{filePath}'");
}
catch (FileNotFoundException)
{
    Fatal($"{filePath}: {new System.IO.FileNotFoundException().Message}");
    return 2;
}
catch (Exception ex)
{
    Fatal($"{filePath}: {ex.Message}");
    return 2;
}

/*
 * Validate syntax using Roslyn. If syntax errors exist, report them like
 * format (file:line:col: error: message) and exit with status 3
 */
Info("checking syntax...");
var (syntaxValid, syntaxDiags) = syntaxChecker.CheckSyntax(sourceCode, filePath);

if (!syntaxValid)
{
    foreach (string d in syntaxDiags)
        Console.Error.WriteLine($"{Prog}: {d}");
    return 3;
}

Info("syntax ok");

/*
 * Compile the syntactically valid source code to a .NET assembly in memory.
 * If compilation fails (missing references, type errors), report errors and exit with status 4
 */
Info("compiling...");
var (compileOk, assembly, compileErrors) = compiler.Compile(sourceCode, sourcePath: filePath);

if (!compileOk)
{
    foreach (string e in compileErrors)
        Console.Error.WriteLine($"{Prog}: {e}");
    return 4;
}

Info("compilation ok");

/*
 * Execute the compiled assembly's Main() method. Capture console output
 * and any runtime exceptions then report success/failure with execution time summary
 */
Info("executing...");
ExecutionResult result = engine.Execute(assembly!);

// Program output goes to stdout unconditionally.
if (!string.IsNullOrEmpty(result.Output))
    Console.Write(result.Output);

if (!result.ExecutionSuccess)
{
    Console.Error.WriteLine($"{Prog}: {filePath}: runtime error");
    Console.Error.WriteLine(result.Errors);
    return 5;
}

if (!quiet)
    Console.Error.WriteLine(
        $"{Prog}: {filePath}: executed in {result.ExecutionTime.TotalMilliseconds:F2} ms");

return 0;

/* 
 * Helper functions for logging and output formatting.
 * Info() logs verbose messages to stderr conditionally (only if --verbose flag is set)
 */
void Info(string msg)
{
    if (verbose)
        Console.Error.WriteLine($"{Prog}: {msg}");
}

// Write a fatal error message to stderr (always shown).
void Fatal(string msg) { Console.Error.WriteLine($"{Prog}: {msg}"); }

void PrintTryHelp() { Console.Error.WriteLine($"Try '{Prog} --help' for more information."); }

void PrintVersion()
{
    Console.WriteLine($"{Prog} {Version}");
    Console.WriteLine("Copyright (C) 2026 UsiFX");
    Console.WriteLine("License Apache 2.0: <https://opensource.org/licenses/Apache-2.0>");
}

void PrintHelp()
{

    /*
     * get the assembly version from the informational version attribute
     * which can include pre-release tags or fall back to the hardcoded version if not available
     */
    string assemblyVersion = Assembly
        .GetExecutingAssembly()
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
        ?.InformationalVersion ?? Version;

    Console.WriteLine($"Usage: {Prog} [OPTION]... FILE");
    Console.WriteLine();
    Console.WriteLine("Validate, compile, and execute a C# source FILE in-memory.");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  -v, --verbose       print each processing step to stderr");
    Console.WriteLine("  -q, --quiet         suppress timing summary; print only program output");
    Console.WriteLine("  -V, --version       output version information and exit");
    Console.WriteLine("  -h, --help          display this help and exit");
    Console.WriteLine("      --              end of options; next argument is the input file");
    Console.WriteLine();
    Console.WriteLine("Exit status:");
    Console.WriteLine("  0  success");
    Console.WriteLine("  1  invalid usage / unrecognized option");
    Console.WriteLine("  2  file read error");
    Console.WriteLine("  3  syntax error");
    Console.WriteLine("  4  compilation error");
    Console.WriteLine("  5  runtime error");
    Console.WriteLine();
    Console.WriteLine($"csanalysis {assemblyVersion}");
    Console.WriteLine("Report bugs to: <https://github.com/UsiFX/csharp_analysis_tool/issues>");
}

