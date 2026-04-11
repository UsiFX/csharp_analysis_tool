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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalysisTool.Core;

// Result of a syntax check
public class SyntaxCheckResult
{
    public bool IsValid { get; set; }
    public IReadOnlyList<string> Diagnostics { get; set; } = [];

    // Allow tuple deconstruction for backwards compatibility
    public void Deconstruct(out bool isValid, out IReadOnlyList<string> diagnostics)
    {
        isValid = IsValid;
        diagnostics = Diagnostics;
    }
}

// Validates C# source code syntax using Roslyn
public class SyntaxChecker
{
    /*
     * Checks the syntax of the given C# source code.
     * sourceCode: C# source code string to analyse.
     * sourcePath: Path used in diagnostic messages (e.g. the file name).
     *             Defaults to <input> for in-memory code.
     * Returns: A SyntaxCheckResult containing a boolean indicating whether the syntax is valid
     *          and a list of GCC-style diagnostic messages.
     */
    public SyntaxCheckResult CheckSyntax(
        string sourceCode,
        string sourcePath = "<input>")
    {
        if (string.IsNullOrWhiteSpace(sourceCode))
            return new SyntaxCheckResult 
            { 
                IsValid = false, 
                Diagnostics = [$"{sourcePath}: error: source code is empty"] 
            };

        SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceCode, path: sourcePath);
        string[] errors = tree
            .GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .Select(d => FormatDiagnostic(d, sourcePath))
            .ToArray();

        return new SyntaxCheckResult 
        { 
            IsValid = errors.Length == 0, 
            Diagnostics = errors 
        };
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

