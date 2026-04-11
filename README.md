# C# Code Analysis & Execution Tool

A lightweight MVP tool that reads C# source files, validates their syntax, compiles them **in-memory** with Roslyn, executes them, and captures the output — all without writing any external `.exe` files.

Available as both a **CLI** application and a **Windows Forms GUI**.

---

## Project Structure

```
CodeAnalysisTool.slnx
├── CodeAnalysisTool.Core/       # Shared library (Roslyn, execution engine)
│   ├── CodeReader.cs
│   ├── SyntaxChecker.cs
│   ├── CompilerService.cs
│   ├── ExecutionEngine.cs
│   └── ExecutionResult.cs
├── CodeAnalysisTool.CLI/        # Command-line interface
│   └── Program.cs
├── CodeAnalysisTool.GUI/        # Windows Forms GUI
│   ├── Form1.cs
│   └── Form1.Designer.cs
```

---

## Requirements

| Requirement | Version |
|-------------|---------|
| .NET SDK    | 8.0 +   |
| OS (CLI)    | Windows / macOS / Linux |
| OS (GUI)    | Windows (WinForms) |

---

## Building

```bash
# Restore dependencies
dotnet restore

# Build all projects
dotnet build CodeAnalysisTool.slnx
```

---

## CLI Usage

```bash
dotnet run --project CodeAnalysisTool.CLI -- [OPTION]... FILE
```

Or after publishing:

```bash
csanalysis [OPTION]... FILE
```

### Options

```
  -v, --verbose       print each processing step to stderr
  -q, --quiet         suppress timing summary; print only program output
  -V, --version       output version information and exit
  -h, --help          display this help and exit
      --              end of options; next argument is the input file
```

### Examples

```bash
# Basic execution
csanalysis <file>

# Verbose mode – shows each step on stderr
csanalysis --verbose <file>

# Quiet – program output only, no timing summary
csanalysis --quiet <file>

# Version / help
csanalysis --version
csanalysis --help
```

**Successful run (`--verbose`):**

```
csanalysis: read 273 bytes from 'Samples/HelloWorld.cs'
csanalysis: checking syntax...
csanalysis: syntax ok
csanalysis: compiling...
csanalysis: compilation ok
csanalysis: executing...
Hello, World!
This is the C# Code Analysis & Execution Tool.
csanalysis: Samples/HelloWorld.cs: executed in 0.10 ms
```

**Syntax error output (always on stderr):**

```
csanalysis: Samples/SyntaxError.cs:9:46: error: ; expected
csanalysis: Samples/SyntaxError.cs:11:27: error: ) expected
csanalysis: Samples/SyntaxError.cs:11:27: error: ; expected
```

**Runtime error:**

```
csanalysis: Samples/RuntimeError.cs: runtime error
System.DivideByZeroException: Attempted to divide by zero.
   at Program.Main()
```

### Exit Codes

| Code | Meaning |
|------|---------|
| 0    | Success |
| 1    | Invalid usage / unrecognized option |
| 2    | File read error |
| 3    | Syntax error |
| 4    | Compilation error |
| 5    | Runtime error |

---

## GUI Usage

Run the Windows Forms application:

```bash
dotnet run --project CodeAnalysisTool.GUI
```

The main window provides:

| Area | Description |
|------|-------------|
| **Code Editor** (left) | Paste or type C# code; use *Open File* to load a `.cs` file |
| **▶ Run** button | Validates, compiles, and executes the code |
| **Output** pane (top-right) | Captured `Console.Out` / `Console.Error` output |
| **Errors / Diagnostics** pane (bottom-right) | Syntax and compiler errors, runtime exceptions |
| **Status bar** | Current state and execution time |

---

## Core Services

### `CodeReader`
Reads C# source files from the filesystem.

### `SyntaxChecker`
Uses `CSharpSyntaxTree.ParseText` (Roslyn) to detect syntax errors without a full compilation pass.

### `CompilerService`
Compiles source code in-memory with `CSharpCompilation.Emit` into a `MemoryStream`.  
Includes standard runtime references (`System.Private.CoreLib`, `System.Console`, `System.Linq`, etc.).

### `ExecutionEngine`
- Loads the compiled assembly into the default `AssemblyLoadContext`.
- Locates the `Main` entry point via reflection.
- Redirects `Console.Out` / `Console.Error` to a `StringWriter` for output capture.
- Measures wall-clock execution time with `System.Diagnostics.Stopwatch`.
- Catches `TargetInvocationException` (and its inner exception) to report runtime errors cleanly.

### `ExecutionResult`
Plain data class carrying: `Output`, `Errors`, `CompilationSuccess`, `ExecutionSuccess`, `ExecutionTime`, `CompilationErrors`.

---

## TODO — Future Enhancements

The items below are grouped by area and ordered roughly by implementation effort.

### CLI / UX
- [ ] `--output`/`-o FILE` flag to write program stdout to a file
- [ ] `--timeout`/`-t SECONDS` flag to kill execution after N seconds
- [ ] `--no-execute` / `--check-only` flag: validate and compile without running
- [ ] `--stdin` flag: read source code from standard input (pipe-friendly)
- [ ] Machine-readable JSON output mode (`--format json`)
- [ ] Color output opt-in (`--color=always|never|auto`)
- [ ] `--define SYMBOL` preprocessor symbol injection
- [ ] Batch mode: accept multiple files, report a summary at the end

### Core / Compilation
- [ ] Execution timeout / `CancellationToken` support in `ExecutionEngine`
- [ ] Isolated `AssemblyLoadContext` per run to allow unloading and avoid assembly conflicts
- [ ] `--reference` / `-r` flag to add extra DLL references at compile time
- [ ] NuGet reference support: resolve and cache packages at runtime
- [ ] Support for multi-file compilation (pass a list of `.cs` files or a directory)
- [ ] Top-level statements support: auto-wrap bare expressions in a `Main()` method
- [ ] Warnings reporting alongside errors (currently only errors surface)
- [ ] Treat warnings-as-errors option (`--Werror`)

### Analysis / Diagnostics
- [ ] Roslyn code metrics: lines of code, cyclomatic complexity, maintainability index
- [ ] Dead code / unreachable code detection
- [ ] IL code dump (`--emit-il`) using `ILSpy` or `System.Reflection.Metadata`
- [ ] Symbol table listing: classes, methods, and their signatures
- [ ] Dependency graph output (which types depend on which)
- [ ] Code style / formatting check via `dotnet-format` integration

### Performance / Profiling
- [ ] Execution time breakdown: JIT warm-up vs actual runtime
- [ ] Allocation count via `GC.GetAllocatedBytesForCurrentThread()`
- [ ] Peak working-set / managed heap size snapshot after execution
- [ ] Repeated-run benchmarking (`--bench N`): run N times, report min/avg/max

### Debugging
- [ ] Variable tracking: inject logging shims around assignments using Roslyn source rewriters
- [ ] Step-through execution using the Roslyn scripting API (`Microsoft.CodeAnalysis.CSharp.Scripting`)
- [ ] Exception breakpoints: pause and dump locals when an exception is thrown
- [ ] Conditional tracing: emit a trace line for every method entry/exit

### GUI (Windows Forms)
- [ ] Syntax highlighting in the code editor (custom `RichTextBox` painter)
- [ ] Line numbers gutter
- [ ] Error squiggles: underline diagnostics inline in the editor
- [ ] Recent files menu
- [ ] Split view: source on left, IL / decompiled output on right
- [ ] Output clear button and auto-scroll toggle
- [ ] Font size zoom (Ctrl+scroll)
- [ ] Light / dark theme toggle

### Project / Ecosystem
- [ ] Publish a standalone `csanalysis` binary via `dotnet publish -r linux-x64 --self-contained`
- [ ] Homebrew formula / apt package / Scoop manifest
- [ ] REPL mode (`csanalysis --repl`): interactive read-eval-print loop
- [ ] Unit test project (`CodeAnalysisTool.Tests`) covering Core services

---

## License

[Apache License, Version 2.0](LICENSE)
