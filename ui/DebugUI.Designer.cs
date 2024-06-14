using System.ComponentModel;

namespace PokeHelper.ui
{
    partial class DebugUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SetAllPosButton = new System.Windows.Forms.Button();
            this.ThrowBallButton = new System.Windows.Forms.Button();
            this.CatchMonButton = new System.Windows.Forms.Button();
            this.OcrDumpButton = new System.Windows.Forms.Button();
            this.FindGenButton = new System.Windows.Forms.Button();
            this.shinyLoopButton = new System.Windows.Forms.Button();
            this.stopShinyLoop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SetAllPosButton
            // 
            this.SetAllPosButton.Location = new System.Drawing.Point(12, 11);
            this.SetAllPosButton.Name = "SetAllPosButton";
            this.SetAllPosButton.Size = new System.Drawing.Size(146, 40);
            this.SetAllPosButton.TabIndex = 0;
            this.SetAllPosButton.Text = "Set Positions";
            this.SetAllPosButton.UseVisualStyleBackColor = true;
            this.SetAllPosButton.Click += new System.EventHandler(this.SetAllPosButton_Click);
            // 
            // ThrowBallButton
            // 
            this.ThrowBallButton.Location = new System.Drawing.Point(12, 69);
            this.ThrowBallButton.Name = "ThrowBallButton";
            this.ThrowBallButton.Size = new System.Drawing.Size(146, 40);
            this.ThrowBallButton.TabIndex = 1;
            this.ThrowBallButton.Text = "Throw Pokeball";
            this.ThrowBallButton.UseVisualStyleBackColor = true;
            this.ThrowBallButton.Click += new System.EventHandler(this.ThrowBallButton_Click);
            // 
            // CatchMonButton
            // 
            this.CatchMonButton.Location = new System.Drawing.Point(12, 125);
            this.CatchMonButton.Name = "CatchMonButton";
            this.CatchMonButton.Size = new System.Drawing.Size(146, 40);
            this.CatchMonButton.TabIndex = 2;
            this.CatchMonButton.Text = "Run Catch Sequence";
            this.CatchMonButton.UseVisualStyleBackColor = true;
            this.CatchMonButton.Click += new System.EventHandler(this.CatchMonButton_Click);
            // 
            // OcrDumpButton
            // 
            this.OcrDumpButton.Location = new System.Drawing.Point(12, 185);
            this.OcrDumpButton.Name = "OcrDumpButton";
            this.OcrDumpButton.Size = new System.Drawing.Size(146, 40);
            this.OcrDumpButton.TabIndex = 3;
            this.OcrDumpButton.Text = "Dump Screen OCR";
            this.OcrDumpButton.UseVisualStyleBackColor = true;
            this.OcrDumpButton.Click += new System.EventHandler(this.OcrDumpButton_Click);
            // 
            // FindGenButton
            // 
            this.FindGenButton.Location = new System.Drawing.Point(12, 245);
            this.FindGenButton.Name = "FindGenButton";
            this.FindGenButton.Size = new System.Drawing.Size(146, 40);
            this.FindGenButton.TabIndex = 4;
            this.FindGenButton.Text = "Find Generic Ok Button";
            this.FindGenButton.UseVisualStyleBackColor = true;
            this.FindGenButton.Click += new System.EventHandler(this.FindGenButton_Click);
            // 
            // shinyLoopButton
            // 
            this.shinyLoopButton.Location = new System.Drawing.Point(182, 12);
            this.shinyLoopButton.Name = "shinyLoopButton";
            this.shinyLoopButton.Size = new System.Drawing.Size(146, 40);
            this.shinyLoopButton.TabIndex = 5;
            this.shinyLoopButton.Text = "Start Shiny Hunt Loop";
            this.shinyLoopButton.UseVisualStyleBackColor = true;
            this.shinyLoopButton.Click += new System.EventHandler(this.shinyLoopButton_Click);
            // 
            // stopShinyLoop
            // 
            this.stopShinyLoop.Location = new System.Drawing.Point(182, 69);
            this.stopShinyLoop.Name = "stopShinyLoop";
            this.stopShinyLoop.Size = new System.Drawing.Size(146, 40);
            this.stopShinyLoop.TabIndex = 6;
            this.stopShinyLoop.Text = "Stop Shiny Hunt Loop";
            this.stopShinyLoop.UseVisualStyleBackColor = true;
            this.stopShinyLoop.Click += new System.EventHandler(this.stopShinyLoop_Click);
            // 
            // DebugUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 297);
            this.Controls.Add(this.stopShinyLoop);
            this.Controls.Add(this.shinyLoopButton);
            this.Controls.Add(this.FindGenButton);
            this.Controls.Add(this.OcrDumpButton);
            this.Controls.Add(this.CatchMonButton);
            this.Controls.Add(this.ThrowBallButton);
            this.Controls.Add(this.SetAllPosButton);
            this.Name = "DebugUI";
            this.Text = "DebugUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DebugUI_FormClosing);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button stopShinyLoop;

        private System.Windows.Forms.Button shinyLoopButton;

        private System.Windows.Forms.Button FindGenButton;

        private System.Windows.Forms.Button OcrDumpButton;

        private System.Windows.Forms.Button CatchMonButton;

        private System.Windows.Forms.Button ThrowBallButton;

        private System.Windows.Forms.Button SetAllPosButton;

        #endregion
    }
}