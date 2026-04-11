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

namespace CodeAnalysisTool.GUI;

partial class Form1
{
    // required designer variables
    private System.ComponentModel.IContainer components = null;

    // controls
    private System.Windows.Forms.SplitContainer splitMain;
    private System.Windows.Forms.SplitContainer splitOutput;
    private System.Windows.Forms.Panel          pnlToolbar;
    private System.Windows.Forms.Panel          pnlStatus;
    private System.Windows.Forms.Label          lblCodeEditor;
    private System.Windows.Forms.Label          lblOutput;
    private System.Windows.Forms.Label          lblErrors;
    private System.Windows.Forms.Label          lblExecutionTime;
    private System.Windows.Forms.Label          lblStatus;
    private System.Windows.Forms.RichTextBox    txtCodeEditor;
    private System.Windows.Forms.RichTextBox    txtOutput;
    private System.Windows.Forms.RichTextBox    txtErrors;
    private System.Windows.Forms.Button         btnRun;
    private System.Windows.Forms.Button         btnClear;
    private System.Windows.Forms.Button         btnLoadFile;

    // clean up any resources being used
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components     = new System.ComponentModel.Container();
        splitMain      = new SplitContainer();
        splitOutput    = new SplitContainer();
        pnlToolbar     = new Panel();
        pnlStatus      = new Panel();
        lblCodeEditor  = new Label();
        lblOutput      = new Label();
        lblErrors      = new Label();
        lblExecutionTime = new Label();
        lblStatus      = new Label();
        txtCodeEditor  = new RichTextBox();
        txtOutput      = new RichTextBox();
        txtErrors      = new RichTextBox();
        btnRun         = new Button();
        btnClear       = new Button();
        btnLoadFile    = new Button();

        // form
        Text            = "C# Code Analysis & Execution Tool";
        ClientSize      = new Size(1100, 700);
        MinimumSize     = new Size(800, 550);
        StartPosition   = FormStartPosition.CenterScreen;
        Font            = new Font("Segoe UI", 9F);
        BackColor       = Color.FromArgb(30, 30, 30);
        ForeColor       = Color.White;

        SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
        ((System.ComponentModel.ISupportInitialize)splitOutput).BeginInit();
        splitMain.Panel1.SuspendLayout();
        splitMain.Panel2.SuspendLayout();
        splitOutput.Panel1.SuspendLayout();
        splitOutput.Panel2.SuspendLayout();
        pnlToolbar.SuspendLayout();
        pnlStatus.SuspendLayout();

        // toolbar panel
        pnlToolbar.Dock      = DockStyle.Top;
        pnlToolbar.Height    = 44;
        pnlToolbar.BackColor = Color.FromArgb(45, 45, 45);
        pnlToolbar.Padding   = new Padding(6, 6, 6, 6);

        // run button
        btnRun.Text      = "▶  Run";
        btnRun.Size      = new Size(90, 30);
        btnRun.Location  = new Point(6, 7);
        btnRun.BackColor = Color.FromArgb(0, 122, 204);
        btnRun.ForeColor = Color.White;
        btnRun.FlatStyle = FlatStyle.Flat;
        btnRun.FlatAppearance.BorderSize = 0;
        btnRun.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnRun.Cursor    = Cursors.Hand;
        btnRun.Click    += btnRun_Click;

        // load file button
        btnLoadFile.Text      = "📂  Open File";
        btnLoadFile.Size      = new Size(110, 30);
        btnLoadFile.Location  = new Point(104, 7);
        btnLoadFile.BackColor = Color.FromArgb(62, 62, 66);
        btnLoadFile.ForeColor = Color.White;
        btnLoadFile.FlatStyle = FlatStyle.Flat;
        btnLoadFile.FlatAppearance.BorderSize = 0;
        btnLoadFile.Cursor    = Cursors.Hand;
        btnLoadFile.Click    += btnLoadFile_Click;

        // clear button
        btnClear.Text      = "🗑  Clear";
        btnClear.Size      = new Size(90, 30);
        btnClear.Location  = new Point(222, 7);
        btnClear.BackColor = Color.FromArgb(62, 62, 66);
        btnClear.ForeColor = Color.White;
        btnClear.FlatStyle = FlatStyle.Flat;
        btnClear.FlatAppearance.BorderSize = 0;
        btnClear.Cursor    = Cursors.Hand;
        btnClear.Click    += btnClear_Click;

        pnlToolbar.Controls.AddRange([btnRun, btnLoadFile, btnClear]);

        // status bar
        pnlStatus.Dock      = DockStyle.Bottom;
        pnlStatus.Height    = 28;
        pnlStatus.BackColor = Color.FromArgb(0, 122, 204);
        pnlStatus.Padding   = new Padding(8, 4, 8, 4);

        lblStatus.AutoSize  = true;
        lblStatus.Text      = "Ready";
        lblStatus.ForeColor = Color.White;
        lblStatus.Location  = new Point(8, 6);
        lblStatus.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);

        lblExecutionTime.AutoSize  = true;
        lblExecutionTime.Text      = "Execution time: —";
        lblExecutionTime.ForeColor = Color.White;
        lblExecutionTime.Dock      = DockStyle.Right;
        lblExecutionTime.TextAlign = ContentAlignment.MiddleRight;
        lblExecutionTime.Padding   = new Padding(0, 0, 8, 0);

        pnlStatus.Controls.AddRange([lblStatus, lblExecutionTime]);

        // horizontal split (code | results)
        splitMain.Dock        = DockStyle.Fill;
        splitMain.Orientation = Orientation.Vertical;
        splitMain.SplitterDistance = 480;
        splitMain.BackColor   = Color.FromArgb(30, 30, 30);
        splitMain.Panel1MinSize = 200;
        splitMain.Panel2MinSize = 200;

        // left panel (code editor)
        lblCodeEditor.Text      = "C# Code Editor";
        lblCodeEditor.Dock      = DockStyle.Top;
        lblCodeEditor.Height    = 22;
        lblCodeEditor.TextAlign = ContentAlignment.MiddleLeft;
        lblCodeEditor.BackColor = Color.FromArgb(37, 37, 38);
        lblCodeEditor.ForeColor = Color.FromArgb(0, 122, 204);
        lblCodeEditor.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblCodeEditor.Padding   = new Padding(4, 0, 0, 0);

        txtCodeEditor.Dock        = DockStyle.Fill;
        txtCodeEditor.Font        = new Font(MonospaceFont, 10F, FontStyle.Regular,
                                             GraphicsUnit.Point, 0);
        txtCodeEditor.BackColor   = Color.FromArgb(30, 30, 30);
        txtCodeEditor.ForeColor   = Color.FromArgb(220, 220, 170);
        txtCodeEditor.BorderStyle = BorderStyle.None;
        txtCodeEditor.AcceptsTab  = true;
        txtCodeEditor.ScrollBars  = RichTextBoxScrollBars.Both;
        txtCodeEditor.WordWrap    = false;

        splitMain.Panel1.Controls.AddRange([txtCodeEditor, lblCodeEditor]);

        // right panel split vertically into output (top) and errors (bottom)
        splitOutput.Dock        = DockStyle.Fill;
        splitOutput.Orientation = Orientation.Horizontal;
        splitOutput.SplitterDistance = 350;
        splitOutput.BackColor   = Color.FromArgb(30, 30, 30);
        splitOutput.Panel1MinSize = 100;
        splitOutput.Panel2MinSize = 80;

        // output area
        lblOutput.Text      = "Output";
        lblOutput.Dock      = DockStyle.Top;
        lblOutput.Height    = 22;
        lblOutput.TextAlign = ContentAlignment.MiddleLeft;
        lblOutput.BackColor = Color.FromArgb(37, 37, 38);
        lblOutput.ForeColor = Color.FromArgb(0, 122, 204);
        lblOutput.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblOutput.Padding   = new Padding(4, 0, 0, 0);

        txtOutput.Dock        = DockStyle.Fill;
        txtOutput.Font        = new Font(MonospaceFont, 10F);
        txtOutput.BackColor   = Color.FromArgb(30, 30, 30);
        txtOutput.ForeColor   = Color.FromArgb(212, 212, 212);
        txtOutput.BorderStyle = BorderStyle.None;
        txtOutput.ReadOnly    = true;
        txtOutput.ScrollBars  = RichTextBoxScrollBars.Both;
        txtOutput.WordWrap    = false;

        splitOutput.Panel1.Controls.AddRange([txtOutput, lblOutput]);

        // errors area
        lblErrors.Text      = "Errors / Diagnostics";
        lblErrors.Dock      = DockStyle.Top;
        lblErrors.Height    = 22;
        lblErrors.TextAlign = ContentAlignment.MiddleLeft;
        lblErrors.BackColor = Color.FromArgb(37, 37, 38);
        lblErrors.ForeColor = Color.FromArgb(0, 122, 204);
        lblErrors.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblErrors.Padding   = new Padding(4, 0, 0, 0);

        txtErrors.Dock        = DockStyle.Fill;
        txtErrors.Font        = new Font(MonospaceFont, 9.5F);
        txtErrors.BackColor   = Color.FromArgb(30, 30, 30);
        txtErrors.ForeColor   = Color.FromArgb(244, 135, 113);
        txtErrors.BorderStyle = BorderStyle.None;
        txtErrors.ReadOnly    = true;
        txtErrors.ScrollBars  = RichTextBoxScrollBars.Both;
        txtErrors.WordWrap    = false;

        splitOutput.Panel2.Controls.AddRange([txtErrors, lblErrors]);

        splitMain.Panel2.Controls.Add(splitOutput);

        // wire everything to the form
        Controls.AddRange([splitMain, pnlStatus, pnlToolbar]);

        pnlToolbar.ResumeLayout(false);
        pnlStatus.ResumeLayout(false);
        pnlStatus.PerformLayout();
        splitOutput.Panel1.ResumeLayout(false);
        splitOutput.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitOutput).EndInit();
        splitOutput.ResumeLayout(false);
        splitMain.Panel1.ResumeLayout(false);
        splitMain.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
        splitMain.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion
}
