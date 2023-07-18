using System.ComponentModel;

namespace SimpleGeneralBroadcasterClient.gui
{
    partial class MessagerInterface
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessagerInterface));
            this.ButtonBroadcast = new System.Windows.Forms.Button();
            this.CheckBoxBroadcast = new System.Windows.Forms.CheckBox();
            this.TextBoxSubnet = new System.Windows.Forms.TextBox();
            this.TextBoxIPAddress = new System.Windows.Forms.TextBox();
            this.LabelSubnetMask = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TextBoxMessage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ButtonBroadcast
            // 
            this.ButtonBroadcast.Location = new System.Drawing.Point(12, 228);
            this.ButtonBroadcast.Name = "ButtonBroadcast";
            this.ButtonBroadcast.Size = new System.Drawing.Size(327, 49);
            this.ButtonBroadcast.TabIndex = 0;
            this.ButtonBroadcast.Text = "Start Broadcast";
            this.ButtonBroadcast.UseVisualStyleBackColor = true;
            // 
            // CheckBoxBroadcast
            // 
            this.CheckBoxBroadcast.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.CheckBoxBroadcast.Checked = true;
            this.CheckBoxBroadcast.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBoxBroadcast.Location = new System.Drawing.Point(12, 12);
            this.CheckBoxBroadcast.Name = "CheckBoxBroadcast";
            this.CheckBoxBroadcast.Size = new System.Drawing.Size(326, 44);
            this.CheckBoxBroadcast.TabIndex = 1;
            this.CheckBoxBroadcast.Text = "Broadcast Mode";
            this.CheckBoxBroadcast.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CheckBoxBroadcast.UseVisualStyleBackColor = true;
            this.CheckBoxBroadcast.CheckedChanged += new System.EventHandler(this.CheckBoxBroadcast_CheckedChanged);
            // 
            // TextBoxSubnet
            // 
            this.TextBoxSubnet.Location = new System.Drawing.Point(126, 73);
            this.TextBoxSubnet.Name = "TextBoxSubnet";
            this.TextBoxSubnet.Size = new System.Drawing.Size(212, 26);
            this.TextBoxSubnet.TabIndex = 2;
            this.TextBoxSubnet.Text = "192.168.1.0";
            this.TextBoxSubnet.TextChanged += new System.EventHandler(this.TextBoxSubnet_TextChanged);
            // 
            // TextBoxIPAddress
            // 
            this.TextBoxIPAddress.Enabled = false;
            this.TextBoxIPAddress.Location = new System.Drawing.Point(126, 112);
            this.TextBoxIPAddress.Name = "TextBoxIPAddress";
            this.TextBoxIPAddress.Size = new System.Drawing.Size(212, 26);
            this.TextBoxIPAddress.TabIndex = 3;
            // 
            // LabelSubnetMask
            // 
            this.LabelSubnetMask.Location = new System.Drawing.Point(12, 73);
            this.LabelSubnetMask.Name = "LabelSubnetMask";
            this.LabelSubnetMask.Size = new System.Drawing.Size(108, 29);
            this.LabelSubnetMask.TabIndex = 4;
            this.LabelSubnetMask.Text = "Subnet Mask";
            this.LabelSubnetMask.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 111);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 29);
            this.label1.TabIndex = 5;
            this.label1.Text = "IP Address";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(26, 154);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(302, 29);
            this.label2.TabIndex = 7;
            this.label2.Text = "Message";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TextBoxMessage
            // 
            this.TextBoxMessage.Location = new System.Drawing.Point(12, 186);
            this.TextBoxMessage.Name = "TextBoxMessage";
            this.TextBoxMessage.Size = new System.Drawing.Size(327, 26);
            this.TextBoxMessage.TabIndex = 6;
            // 
            // MessagerInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 297);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TextBoxMessage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LabelSubnetMask);
            this.Controls.Add(this.TextBoxIPAddress);
            this.Controls.Add(this.TextBoxSubnet);
            this.Controls.Add(this.CheckBoxBroadcast);
            this.Controls.Add(this.ButtonBroadcast);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(373, 353);
            this.MinimumSize = new System.Drawing.Size(373, 353);
            this.Name = "MessagerInterface";
            this.Text = "MessagerInterface";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TextBoxMessage;

        private System.Windows.Forms.TextBox TextBoxSubnet;
        private System.Windows.Forms.TextBox TextBoxIPAddress;
        private System.Windows.Forms.Label LabelSubnetMask;
        private System.Windows.Forms.Label label1;

        private System.Windows.Forms.Button ButtonBroadcast;
        private System.Windows.Forms.CheckBox CheckBoxBroadcast;

        #endregion
    }
}