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

using CodeAnalysisTool.Core;
using System.Drawing.Text;

namespace CodeAnalysisTool.GUI;

public partial class Form1 : Form
{
    private readonly CodeReader      _codeReader      = new();
    private readonly SyntaxChecker   _syntaxChecker   = new();
    private readonly CompilerService _compilerService = new();
    private readonly ExecutionEngine _executionEngine = new();

    /*
     * Returns the best available monospace font, falling back to "Courier New" when
     * preferred fonts are not installed on the system.
     */
    private static string MonospaceFont
    {
        get
        {
            using var fonts = new InstalledFontCollection();
            var installed = new HashSet<string>(
                fonts.Families.Select(f => f.Name),
                StringComparer.OrdinalIgnoreCase);

            foreach (string candidate in new[] { "Cascadia Code", "Cascadia Mono", "Consolas", "Courier New" })
            {
                if (installed.Contains(candidate))
                    return candidate;
            }
            return "Courier New"; // always available on Windows
        }
    }

    public Form1()
    {
        InitializeComponent();
        LoadSampleCode();
    }

    // button handlers
    private void btnRun_Click(object sender, EventArgs e)
    {
        ClearResults();
        SetStatus("Running…", Color.Orange);
        btnRun.Enabled = false;
        Application.DoEvents();

        try
        {
            string sourceCode = txtCodeEditor.Text;

            // 1. Syntax check
            var (syntaxValid, syntaxDiags) = _syntaxChecker.CheckSyntax(sourceCode);
            if (!syntaxValid)
            {
                ShowErrors(syntaxDiags);
                SetStatus("Syntax error", Color.Red);
                return;
            }

            // 2. Compile
            var (compileOk, assembly, compileErrors) = _compilerService.Compile(sourceCode);
            if (!compileOk)
            {
                ShowErrors(compileErrors);
                SetStatus("Compilation failed", Color.Red);
                return;
            }

            // 3. Execute
            ExecutionResult result = _executionEngine.Execute(assembly!);

            txtOutput.Text = result.Output;
            lblExecutionTime.Text = $"Execution time: {result.ExecutionTime.TotalMilliseconds:F2} ms";

            if (!result.ExecutionSuccess)
            {
                ShowErrors([result.Errors]);
                SetStatus("Runtime error", Color.Red);
            }
            else
            {
                SetStatus("Success ✔", Color.Green);
            }
        }
        catch (Exception ex)
        {
            ShowErrors([$"Unexpected error: {ex.Message}"]);
            SetStatus("Error", Color.Red);
        }
        finally
        {
            btnRun.Enabled = true;
        }
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
        txtCodeEditor.Text = string.Empty;
        ClearResults();
        SetStatus("Ready", Color.Gray);
    }

    private void btnLoadFile_Click(object sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog
        {
            Filter = "C# Files (*.cs)|*.cs|All Files (*.*)|*.*",
            Title   = "Open C# Source File"
        };

        if (dlg.ShowDialog() == DialogResult.OK)
        {
            try
            {
                txtCodeEditor.Text = _codeReader.ReadFile(dlg.FileName);
                SetStatus($"Loaded: {Path.GetFileName(dlg.FileName)}", Color.Gray);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load file:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // helpers
    private void ClearResults()
    {
        txtOutput.Clear();
        txtErrors.Clear();
        lblExecutionTime.Text = "Execution time: —";
    }

    private void ShowErrors(IEnumerable<string> errors)
    {
        txtErrors.Text = string.Join(Environment.NewLine, errors);
    }

    private void SetStatus(string text, Color color)
    {
        lblStatus.Text      = text;
        lblStatus.ForeColor = color;
    }

    private void LoadSampleCode()
    {
        txtCodeEditor.Text =
            """
            using System;

            class Program
            {
                static void Main()
                {
                    Console.WriteLine("Hello from the C# Analysis Tool!");
                    for (int i = 1; i <= 5; i++)
                    {
                        Console.WriteLine($"  Line {i}");
                    }
                }
            }
            """;
    }
}

