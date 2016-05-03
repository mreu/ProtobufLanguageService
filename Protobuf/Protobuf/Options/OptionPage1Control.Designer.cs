namespace MichaelReukauff.Protobuf.Options
{
    partial class OptionPage1Control
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkAutoConvert = new System.Windows.Forms.CheckBox();
            this.lblAutoConvert = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // chkAutoConvert
            // 
            this.chkAutoConvert.AutoSize = true;
            this.chkAutoConvert.Location = new System.Drawing.Point(17, 17);
            this.chkAutoConvert.Name = "chkAutoConvert";
            this.chkAutoConvert.Size = new System.Drawing.Size(15, 14);
            this.chkAutoConvert.TabIndex = 0;
            this.chkAutoConvert.UseVisualStyleBackColor = true;
            this.chkAutoConvert.CheckedChanged += new System.EventHandler(this.chkAutoConvert_CheckedChanged);
            // 
            // lblAutoConvert
            // 
            this.lblAutoConvert.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblAutoConvert.Location = new System.Drawing.Point(38, 3);
            this.lblAutoConvert.Multiline = true;
            this.lblAutoConvert.Name = "lblAutoConvert";
            this.lblAutoConvert.ReadOnly = true;
            this.lblAutoConvert.Size = new System.Drawing.Size(184, 48);
            this.lblAutoConvert.TabIndex = 2;
            this.lblAutoConvert.Text = "Convert .proto files automatically to codepage compatible to googles protobuf com" +
    "piler (protoc.exe)";
            this.lblAutoConvert.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lblAutoConvert_MouseClick);
            // 
            // OptionPage1Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblAutoConvert);
            this.Controls.Add(this.chkAutoConvert);
            this.Name = "OptionPage1Control";
            this.Size = new System.Drawing.Size(614, 150);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkAutoConvert;
        private System.Windows.Forms.TextBox lblAutoConvert;

    }
}
