using System.ComponentModel;

namespace PokeHelper.ui;

partial class InputboxUI
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
        this.okButton = new System.Windows.Forms.Button();
        this.textBox = new System.Windows.Forms.TextBox();
        this.SuspendLayout();
        // 
        // okButton
        // 
        this.okButton.Font = new System.Drawing.Font("Microsoft YaHei UI Light", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.okButton.Location = new System.Drawing.Point(87, 51);
        this.okButton.Name = "okButton";
        this.okButton.Size = new System.Drawing.Size(181, 35);
        this.okButton.TabIndex = 0;
        this.okButton.Text = "Submit";
        this.okButton.UseVisualStyleBackColor = true;
        this.okButton.Click += new System.EventHandler(this.okButton_Click);
        // 
        // textBox
        // 
        this.textBox.Location = new System.Drawing.Point(87, 12);
        this.textBox.Name = "textBox";
        this.textBox.Size = new System.Drawing.Size(181, 20);
        this.textBox.TabIndex = 1;
        // 
        // InputboxUI
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(364, 98);
        this.Controls.Add(this.textBox);
        this.Controls.Add(this.okButton);
        this.Name = "InputboxUI";
        this.Text = "InputboxUI";
        this.Shown += new System.EventHandler(this.InputboxUI_Shown);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private System.Windows.Forms.TextBox textBox;

    private System.Windows.Forms.Button okButton;

    #endregion
}