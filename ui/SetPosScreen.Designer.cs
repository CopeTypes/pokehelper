using System.ComponentModel;

namespace PokeHelper.ui
{
    partial class SetPosScreen
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
            this.SetButton = new System.Windows.Forms.Button();
            this.NameLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SetButton
            // 
            this.SetButton.Font = new System.Drawing.Font("Microsoft YaHei UI Light", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SetButton.Location = new System.Drawing.Point(101, 78);
            this.SetButton.Name = "SetButton";
            this.SetButton.Size = new System.Drawing.Size(174, 39);
            this.SetButton.TabIndex = 0;
            this.SetButton.Text = "Set";
            this.SetButton.UseVisualStyleBackColor = true;
            this.SetButton.Click += new System.EventHandler(this.SetButton_Click);
            // 
            // NameLabel
            // 
            this.NameLabel.Font = new System.Drawing.Font("Segoe UI Symbol", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameLabel.Location = new System.Drawing.Point(15, 12);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(358, 52);
            this.NameLabel.TabIndex = 1;
            this.NameLabel.Text = "Pokeball Throw Start";
            this.NameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SetPosScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 129);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.SetButton);
            this.Name = "SetPosScreen";
            this.Text = "SetPosScreen";
            this.Shown += new System.EventHandler(this.SetPosScreen_Shown);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label NameLabel;

        private System.Windows.Forms.Button SetButton;

        #endregion
    }
}