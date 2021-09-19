namespace UnrealJassScriptHelper
{
    partial class JassScriptHelperMainWindow
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.inputscript = new System.Windows.Forms.TextBox();
            this.OutOptimizedScript = new System.Windows.Forms.TextBox();
            this.LogLeakDetected = new System.Windows.Forms.TextBox();
            this.LogUnusedLocals = new System.Windows.Forms.TextBox();
            this.LogUnusedFunctions = new System.Windows.Forms.TextBox();
            this.LogUnusedGlobals = new System.Windows.Forms.TextBox();
            this.OutOptimizeHelper = new System.Windows.Forms.TextBox();
            this.LoadLeakForScan = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.STARTSTARTSTART = new System.Windows.Forms.Button();
            this.SelectInputScript = new System.Windows.Forms.Button();
            this.SelectOutputScript = new System.Windows.Forms.Button();
            this.SelectLeakLogFile = new System.Windows.Forms.Button();
            this.SelectLeakListFile = new System.Windows.Forms.Button();
            this.SelectUnusedGLobalsLogFile = new System.Windows.Forms.Button();
            this.SelectUnusedFunctionsLogFile = new System.Windows.Forms.Button();
            this.SelectUnusedLocalsLogFile = new System.Windows.Forms.Button();
            this.SelectUnusedOptimizationsLogFile = new System.Windows.Forms.Button();
            this.ScriptProcessProgress = new System.Windows.Forms.ProgressBar();
            this.UpdateProgressBar = new System.Windows.Forms.Timer(this.components);
            this.WriteOptimizedScript = new System.Windows.Forms.CheckBox();
            this.WriteLeakList = new System.Windows.Forms.CheckBox();
            this.HaveCustomLeakList = new System.Windows.Forms.CheckBox();
            this.WriteUnusedGlobals = new System.Windows.Forms.CheckBox();
            this.WriteUnusedFunctions = new System.Windows.Forms.CheckBox();
            this.WriteUnusedLocals = new System.Windows.Forms.CheckBox();
            this.WriteOptimizations = new System.Windows.Forms.CheckBox();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.SaveDebugInfo = new System.Windows.Forms.CheckBox();
            this.NeedSaveCommentLines = new System.Windows.Forms.CheckBox();
            this.AboutBtn = new System.Windows.Forms.CheckBox();
            this.OptimizeFuncUsageFile = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // inputscript
            // 
            this.inputscript.Location = new System.Drawing.Point(122, 12);
            this.inputscript.Name = "inputscript";
            this.inputscript.Size = new System.Drawing.Size(152, 20);
            this.inputscript.TabIndex = 0;
            this.inputscript.Text = "war3map.j";
            // 
            // OutOptimizedScript
            // 
            this.OutOptimizedScript.Location = new System.Drawing.Point(122, 38);
            this.OutOptimizedScript.Name = "OutOptimizedScript";
            this.OutOptimizedScript.Size = new System.Drawing.Size(152, 20);
            this.OutOptimizedScript.TabIndex = 0;
            this.OutOptimizedScript.Text = "war3map_opt.j";
            // 
            // LogLeakDetected
            // 
            this.LogLeakDetected.Location = new System.Drawing.Point(122, 64);
            this.LogLeakDetected.Name = "LogLeakDetected";
            this.LogLeakDetected.Size = new System.Drawing.Size(152, 20);
            this.LogLeakDetected.TabIndex = 0;
            this.LogLeakDetected.Text = "LeakDetected.txt";
            // 
            // LogUnusedLocals
            // 
            this.LogUnusedLocals.Location = new System.Drawing.Point(122, 168);
            this.LogUnusedLocals.Name = "LogUnusedLocals";
            this.LogUnusedLocals.Size = new System.Drawing.Size(152, 20);
            this.LogUnusedLocals.TabIndex = 0;
            this.LogUnusedLocals.Text = "UnusedLocals.txt";
            // 
            // LogUnusedFunctions
            // 
            this.LogUnusedFunctions.Location = new System.Drawing.Point(122, 142);
            this.LogUnusedFunctions.Name = "LogUnusedFunctions";
            this.LogUnusedFunctions.Size = new System.Drawing.Size(152, 20);
            this.LogUnusedFunctions.TabIndex = 0;
            this.LogUnusedFunctions.Text = "UnusedFunctions.txt";
            // 
            // LogUnusedGlobals
            // 
            this.LogUnusedGlobals.Location = new System.Drawing.Point(122, 116);
            this.LogUnusedGlobals.Name = "LogUnusedGlobals";
            this.LogUnusedGlobals.Size = new System.Drawing.Size(152, 20);
            this.LogUnusedGlobals.TabIndex = 0;
            this.LogUnusedGlobals.Text = "UnusedGlobals.txt";
            // 
            // OutOptimizeHelper
            // 
            this.OutOptimizeHelper.Location = new System.Drawing.Point(122, 222);
            this.OutOptimizeHelper.Name = "OutOptimizeHelper";
            this.OutOptimizeHelper.Size = new System.Drawing.Size(152, 20);
            this.OutOptimizeHelper.TabIndex = 0;
            this.OutOptimizeHelper.Text = "Optimizations.txt";
            // 
            // LoadLeakForScan
            // 
            this.LoadLeakForScan.Location = new System.Drawing.Point(139, 90);
            this.LoadLeakForScan.Name = "LoadLeakForScan";
            this.LoadLeakForScan.Size = new System.Drawing.Size(135, 20);
            this.LoadLeakForScan.TabIndex = 0;
            this.LoadLeakForScan.Text = "ScanLeaks.txt";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Input script:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Output script:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Out leaks:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(29, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Scan only this leaks:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 123);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Out unused globals:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 149);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(111, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Out unused functions:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 229);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Out optimizations:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 175);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(95, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Out unused locals:";
            // 
            // STARTSTARTSTART
            // 
            this.STARTSTARTSTART.Location = new System.Drawing.Point(242, 248);
            this.STARTSTARTSTART.Name = "STARTSTARTSTART";
            this.STARTSTARTSTART.Size = new System.Drawing.Size(75, 23);
            this.STARTSTARTSTART.TabIndex = 2;
            this.STARTSTARTSTART.Text = "Start!";
            this.STARTSTARTSTART.UseVisualStyleBackColor = true;
            this.STARTSTARTSTART.Click += new System.EventHandler(this.STARTSTARTSTART_Click);
            // 
            // SelectInputScript
            // 
            this.SelectInputScript.Location = new System.Drawing.Point(280, 12);
            this.SelectInputScript.Name = "SelectInputScript";
            this.SelectInputScript.Size = new System.Drawing.Size(37, 20);
            this.SelectInputScript.TabIndex = 3;
            this.SelectInputScript.Text = "...";
            this.SelectInputScript.UseVisualStyleBackColor = true;
            this.SelectInputScript.Click += new System.EventHandler(this.OpenFileForJassScriptHelper);
            // 
            // SelectOutputScript
            // 
            this.SelectOutputScript.Location = new System.Drawing.Point(280, 37);
            this.SelectOutputScript.Name = "SelectOutputScript";
            this.SelectOutputScript.Size = new System.Drawing.Size(37, 20);
            this.SelectOutputScript.TabIndex = 3;
            this.SelectOutputScript.Text = "...";
            this.SelectOutputScript.UseVisualStyleBackColor = true;
            this.SelectOutputScript.Click += new System.EventHandler(this.OpenFileForJassScriptHelper);
            // 
            // SelectLeakLogFile
            // 
            this.SelectLeakLogFile.Location = new System.Drawing.Point(280, 64);
            this.SelectLeakLogFile.Name = "SelectLeakLogFile";
            this.SelectLeakLogFile.Size = new System.Drawing.Size(37, 20);
            this.SelectLeakLogFile.TabIndex = 3;
            this.SelectLeakLogFile.Text = "...";
            this.SelectLeakLogFile.UseVisualStyleBackColor = true;
            this.SelectLeakLogFile.Click += new System.EventHandler(this.OpenFileForJassScriptHelper);
            // 
            // SelectLeakListFile
            // 
            this.SelectLeakListFile.Location = new System.Drawing.Point(280, 90);
            this.SelectLeakListFile.Name = "SelectLeakListFile";
            this.SelectLeakListFile.Size = new System.Drawing.Size(37, 20);
            this.SelectLeakListFile.TabIndex = 3;
            this.SelectLeakListFile.Text = "...";
            this.SelectLeakListFile.UseVisualStyleBackColor = true;
            this.SelectLeakListFile.Click += new System.EventHandler(this.OpenFileForJassScriptHelper);
            // 
            // SelectUnusedGLobalsLogFile
            // 
            this.SelectUnusedGLobalsLogFile.Location = new System.Drawing.Point(280, 116);
            this.SelectUnusedGLobalsLogFile.Name = "SelectUnusedGLobalsLogFile";
            this.SelectUnusedGLobalsLogFile.Size = new System.Drawing.Size(37, 20);
            this.SelectUnusedGLobalsLogFile.TabIndex = 3;
            this.SelectUnusedGLobalsLogFile.Text = "...";
            this.SelectUnusedGLobalsLogFile.UseVisualStyleBackColor = true;
            this.SelectUnusedGLobalsLogFile.Click += new System.EventHandler(this.OpenFileForJassScriptHelper);
            // 
            // SelectUnusedFunctionsLogFile
            // 
            this.SelectUnusedFunctionsLogFile.Location = new System.Drawing.Point(280, 142);
            this.SelectUnusedFunctionsLogFile.Name = "SelectUnusedFunctionsLogFile";
            this.SelectUnusedFunctionsLogFile.Size = new System.Drawing.Size(37, 20);
            this.SelectUnusedFunctionsLogFile.TabIndex = 3;
            this.SelectUnusedFunctionsLogFile.Text = "...";
            this.SelectUnusedFunctionsLogFile.UseVisualStyleBackColor = true;
            this.SelectUnusedFunctionsLogFile.Click += new System.EventHandler(this.OpenFileForJassScriptHelper);
            // 
            // SelectUnusedLocalsLogFile
            // 
            this.SelectUnusedLocalsLogFile.Location = new System.Drawing.Point(280, 168);
            this.SelectUnusedLocalsLogFile.Name = "SelectUnusedLocalsLogFile";
            this.SelectUnusedLocalsLogFile.Size = new System.Drawing.Size(37, 20);
            this.SelectUnusedLocalsLogFile.TabIndex = 3;
            this.SelectUnusedLocalsLogFile.Text = "...";
            this.SelectUnusedLocalsLogFile.UseVisualStyleBackColor = true;
            this.SelectUnusedLocalsLogFile.Click += new System.EventHandler(this.OpenFileForJassScriptHelper);
            // 
            // SelectUnusedOptimizationsLogFile
            // 
            this.SelectUnusedOptimizationsLogFile.Location = new System.Drawing.Point(280, 222);
            this.SelectUnusedOptimizationsLogFile.Name = "SelectUnusedOptimizationsLogFile";
            this.SelectUnusedOptimizationsLogFile.Size = new System.Drawing.Size(37, 20);
            this.SelectUnusedOptimizationsLogFile.TabIndex = 3;
            this.SelectUnusedOptimizationsLogFile.Text = "...";
            this.SelectUnusedOptimizationsLogFile.UseVisualStyleBackColor = true;
            this.SelectUnusedOptimizationsLogFile.Click += new System.EventHandler(this.OpenFileForJassScriptHelper);
            // 
            // ScriptProcessProgress
            // 
            this.ScriptProcessProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ScriptProcessProgress.Location = new System.Drawing.Point(0, 294);
            this.ScriptProcessProgress.Name = "ScriptProcessProgress";
            this.ScriptProcessProgress.Size = new System.Drawing.Size(356, 23);
            this.ScriptProcessProgress.Step = 5;
            this.ScriptProcessProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ScriptProcessProgress.TabIndex = 4;
            // 
            // UpdateProgressBar
            // 
            this.UpdateProgressBar.Enabled = true;
            this.UpdateProgressBar.Interval = 50;
            this.UpdateProgressBar.Tick += new System.EventHandler(this.UpdateProgressBar_Tick);
            // 
            // WriteOptimizedScript
            // 
            this.WriteOptimizedScript.AutoSize = true;
            this.WriteOptimizedScript.Location = new System.Drawing.Point(329, 44);
            this.WriteOptimizedScript.Name = "WriteOptimizedScript";
            this.WriteOptimizedScript.Size = new System.Drawing.Size(15, 14);
            this.WriteOptimizedScript.TabIndex = 5;
            this.WriteOptimizedScript.UseVisualStyleBackColor = true;
            // 
            // WriteLeakList
            // 
            this.WriteLeakList.AutoSize = true;
            this.WriteLeakList.Location = new System.Drawing.Point(329, 70);
            this.WriteLeakList.Name = "WriteLeakList";
            this.WriteLeakList.Size = new System.Drawing.Size(15, 14);
            this.WriteLeakList.TabIndex = 5;
            this.WriteLeakList.UseVisualStyleBackColor = true;
            // 
            // HaveCustomLeakList
            // 
            this.HaveCustomLeakList.AutoSize = true;
            this.HaveCustomLeakList.Checked = true;
            this.HaveCustomLeakList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.HaveCustomLeakList.Location = new System.Drawing.Point(329, 96);
            this.HaveCustomLeakList.Name = "HaveCustomLeakList";
            this.HaveCustomLeakList.Size = new System.Drawing.Size(15, 14);
            this.HaveCustomLeakList.TabIndex = 5;
            this.HaveCustomLeakList.UseVisualStyleBackColor = true;
            // 
            // WriteUnusedGlobals
            // 
            this.WriteUnusedGlobals.AutoSize = true;
            this.WriteUnusedGlobals.Checked = true;
            this.WriteUnusedGlobals.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WriteUnusedGlobals.Location = new System.Drawing.Point(329, 122);
            this.WriteUnusedGlobals.Name = "WriteUnusedGlobals";
            this.WriteUnusedGlobals.Size = new System.Drawing.Size(15, 14);
            this.WriteUnusedGlobals.TabIndex = 5;
            this.WriteUnusedGlobals.UseVisualStyleBackColor = true;
            // 
            // WriteUnusedFunctions
            // 
            this.WriteUnusedFunctions.AutoSize = true;
            this.WriteUnusedFunctions.Checked = true;
            this.WriteUnusedFunctions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WriteUnusedFunctions.Location = new System.Drawing.Point(329, 148);
            this.WriteUnusedFunctions.Name = "WriteUnusedFunctions";
            this.WriteUnusedFunctions.Size = new System.Drawing.Size(15, 14);
            this.WriteUnusedFunctions.TabIndex = 5;
            this.WriteUnusedFunctions.UseVisualStyleBackColor = true;
            // 
            // WriteUnusedLocals
            // 
            this.WriteUnusedLocals.AutoSize = true;
            this.WriteUnusedLocals.Checked = true;
            this.WriteUnusedLocals.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WriteUnusedLocals.Location = new System.Drawing.Point(329, 174);
            this.WriteUnusedLocals.Name = "WriteUnusedLocals";
            this.WriteUnusedLocals.Size = new System.Drawing.Size(15, 14);
            this.WriteUnusedLocals.TabIndex = 5;
            this.WriteUnusedLocals.UseVisualStyleBackColor = true;
            // 
            // WriteOptimizations
            // 
            this.WriteOptimizations.AutoSize = true;
            this.WriteOptimizations.Location = new System.Drawing.Point(329, 228);
            this.WriteOptimizations.Name = "WriteOptimizations";
            this.WriteOptimizations.Size = new System.Drawing.Size(15, 14);
            this.WriteOptimizations.TabIndex = 5;
            this.WriteOptimizations.UseVisualStyleBackColor = true;
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Location = new System.Drawing.Point(11, 274);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(23, 13);
            this.StatusLabel.TabIndex = 6;
            this.StatusLabel.Text = "idle";
            // 
            // SaveDebugInfo
            // 
            this.SaveDebugInfo.AutoSize = true;
            this.SaveDebugInfo.Location = new System.Drawing.Point(160, 254);
            this.SaveDebugInfo.Name = "SaveDebugInfo";
            this.SaveDebugInfo.Size = new System.Drawing.Size(58, 17);
            this.SaveDebugInfo.TabIndex = 7;
            this.SaveDebugInfo.Text = "Debug";
            this.SaveDebugInfo.UseVisualStyleBackColor = true;
            // 
            // NeedSaveCommentLines
            // 
            this.NeedSaveCommentLines.AutoSize = true;
            this.NeedSaveCommentLines.Checked = true;
            this.NeedSaveCommentLines.CheckState = System.Windows.Forms.CheckState.Checked;
            this.NeedSaveCommentLines.Location = new System.Drawing.Point(32, 254);
            this.NeedSaveCommentLines.Name = "NeedSaveCommentLines";
            this.NeedSaveCommentLines.Size = new System.Drawing.Size(102, 17);
            this.NeedSaveCommentLines.TabIndex = 7;
            this.NeedSaveCommentLines.Text = "Save comments";
            this.NeedSaveCommentLines.UseVisualStyleBackColor = true;
            // 
            // AboutBtn
            // 
            this.AboutBtn.AutoSize = true;
            this.AboutBtn.Checked = true;
            this.AboutBtn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AboutBtn.Location = new System.Drawing.Point(329, 18);
            this.AboutBtn.Name = "AboutBtn";
            this.AboutBtn.Size = new System.Drawing.Size(15, 14);
            this.AboutBtn.TabIndex = 5;
            this.AboutBtn.UseVisualStyleBackColor = true;
            this.AboutBtn.CheckedChanged += new System.EventHandler(this.AboutBtn_CheckedChanged);
            // 
            // OptimizeFuncUsageFile
            // 
            this.OptimizeFuncUsageFile.Location = new System.Drawing.Point(139, 196);
            this.OptimizeFuncUsageFile.Name = "OptimizeFuncUsageFile";
            this.OptimizeFuncUsageFile.Size = new System.Drawing.Size(135, 20);
            this.OptimizeFuncUsageFile.TabIndex = 0;
            this.OptimizeFuncUsageFile.Text = "FunctionsForOptimize.txt";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(60, 203);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(73, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Input func list:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(280, 196);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(37, 20);
            this.button1.TabIndex = 3;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OpenFileForJassScriptHelper);
            // 
            // JassScriptHelperMainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 317);
            this.Controls.Add(this.NeedSaveCommentLines);
            this.Controls.Add(this.SaveDebugInfo);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.WriteOptimizations);
            this.Controls.Add(this.WriteUnusedFunctions);
            this.Controls.Add(this.HaveCustomLeakList);
            this.Controls.Add(this.AboutBtn);
            this.Controls.Add(this.WriteOptimizedScript);
            this.Controls.Add(this.WriteUnusedLocals);
            this.Controls.Add(this.WriteUnusedGlobals);
            this.Controls.Add(this.WriteLeakList);
            this.Controls.Add(this.ScriptProcessProgress);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.SelectUnusedOptimizationsLogFile);
            this.Controls.Add(this.SelectUnusedLocalsLogFile);
            this.Controls.Add(this.SelectUnusedFunctionsLogFile);
            this.Controls.Add(this.SelectUnusedGLobalsLogFile);
            this.Controls.Add(this.SelectLeakListFile);
            this.Controls.Add(this.SelectLeakLogFile);
            this.Controls.Add(this.SelectOutputScript);
            this.Controls.Add(this.SelectInputScript);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.STARTSTARTSTART);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.OptimizeFuncUsageFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OutOptimizeHelper);
            this.Controls.Add(this.LogUnusedGlobals);
            this.Controls.Add(this.LogUnusedLocals);
            this.Controls.Add(this.LogUnusedFunctions);
            this.Controls.Add(this.LoadLeakForScan);
            this.Controls.Add(this.LogLeakDetected);
            this.Controls.Add(this.OutOptimizedScript);
            this.Controls.Add(this.inputscript);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "JassScriptHelperMainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Jass Script Helper         1.4e";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JassScriptHelperMainWindow_FormClosing);
            this.Load += new System.EventHandler(this.JassScriptHelperMainWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox inputscript;
        private System.Windows.Forms.TextBox OutOptimizedScript;
        private System.Windows.Forms.TextBox LogLeakDetected;
        private System.Windows.Forms.TextBox LogUnusedLocals;
        private System.Windows.Forms.TextBox LogUnusedFunctions;
        private System.Windows.Forms.TextBox LogUnusedGlobals;
        private System.Windows.Forms.TextBox OutOptimizeHelper;
        private System.Windows.Forms.TextBox LoadLeakForScan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button STARTSTARTSTART;
        private System.Windows.Forms.Button SelectInputScript;
        private System.Windows.Forms.Button SelectOutputScript;
        private System.Windows.Forms.Button SelectLeakLogFile;
        private System.Windows.Forms.Button SelectLeakListFile;
        private System.Windows.Forms.Button SelectUnusedGLobalsLogFile;
        private System.Windows.Forms.Button SelectUnusedFunctionsLogFile;
        private System.Windows.Forms.Button SelectUnusedLocalsLogFile;
        private System.Windows.Forms.Button SelectUnusedOptimizationsLogFile;
        private System.Windows.Forms.ProgressBar ScriptProcessProgress;
        private System.Windows.Forms.Timer UpdateProgressBar;
        private System.Windows.Forms.CheckBox WriteOptimizedScript;
        private System.Windows.Forms.CheckBox WriteLeakList;
        private System.Windows.Forms.CheckBox HaveCustomLeakList;
        private System.Windows.Forms.CheckBox WriteUnusedGlobals;
        private System.Windows.Forms.CheckBox WriteUnusedFunctions;
        private System.Windows.Forms.CheckBox WriteUnusedLocals;
        private System.Windows.Forms.CheckBox WriteOptimizations;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.CheckBox SaveDebugInfo;
        private System.Windows.Forms.CheckBox NeedSaveCommentLines;
        private System.Windows.Forms.CheckBox AboutBtn;
        private System.Windows.Forms.TextBox OptimizeFuncUsageFile;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button button1;
    }
}

