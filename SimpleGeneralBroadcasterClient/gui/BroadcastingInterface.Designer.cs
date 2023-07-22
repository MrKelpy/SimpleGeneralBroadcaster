using System.ComponentModel;

namespace SimpleGeneralBroadcasterClient.gui
{
    partial class BroadcastingInterface
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BroadcastingInterface));
            this.TextBoxConsole = new System.Windows.Forms.RichTextBox();
            this.ButtonEnd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TextBoxConsole
            // 
            this.TextBoxConsole.BackColor = System.Drawing.Color.Black;
            this.TextBoxConsole.ForeColor = System.Drawing.Color.Lime;
            this.TextBoxConsole.Location = new System.Drawing.Point(12, 12);
            this.TextBoxConsole.Name = "TextBoxConsole";
            this.TextBoxConsole.ReadOnly = true;
            this.TextBoxConsole.Size = new System.Drawing.Size(456, 244);
            this.TextBoxConsole.TabIndex = 0;
            this.TextBoxConsole.Text = " Loading...\n";
            // 
            // ButtonEnd
            // 
            this.ButtonEnd.Location = new System.Drawing.Point(12, 263);
            this.ButtonEnd.Name = "ButtonEnd";
            this.ButtonEnd.Size = new System.Drawing.Size(456, 39);
            this.ButtonEnd.TabIndex = 1;
            this.ButtonEnd.Text = "Stop";
            this.ButtonEnd.UseVisualStyleBackColor = true;
            this.ButtonEnd.Click += new System.EventHandler(this.ButtonEnd_Click);
            // 
            // BroadcastingInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 307);
            this.Controls.Add(this.ButtonEnd);
            this.Controls.Add(this.TextBoxConsole);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BroadcastingInterface";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BroadcastingInterface_FormClosed);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button ButtonEnd;

        private System.Windows.Forms.RichTextBox TextBoxConsole;

        #endregion
    }
}