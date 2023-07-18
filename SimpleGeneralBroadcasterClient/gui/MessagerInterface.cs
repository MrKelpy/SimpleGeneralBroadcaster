using System;
using System.Linq;
using System.Windows.Forms;

namespace SimpleGeneralBroadcasterClient.gui
{
    /// <summary>
    /// The messaging interface for the application; Allows the user to send either broadcast
    /// or ip-specific messages to a 
    /// </summary>
    public partial class MessagerInterface : Form
    {
        public MessagerInterface()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Inverts the state of the IP Address text box relative to the broadcast check box.
        /// Ensures that the broadcast checkbox's state is the same as the subnet box's state.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void CheckBoxBroadcast_CheckedChanged(object sender, EventArgs e)
        {
            this.TextBoxIPAddress.Enabled = !this.CheckBoxBroadcast.Checked;
            this.TextBoxSubnet.Enabled  = this.CheckBoxBroadcast.Checked;
        }

        /// <summary>
        /// Ensure that the subnet box is formatted correctly, as in XXX.XXX.XXX.XXX
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event argument</param>
        private void TextBoxSubnet_TextChanged(object sender, EventArgs e)
        {
            // Ensure that the subnet box is formatted correctly, as in an Ipv4 address format (XXX.XXX.XXX.XXX)
            bool pointCount = this.TextBoxSubnet.Text.Count(x => x == '.') == 3;
            bool onlyNumbers = this.TextBoxSubnet.Text.All(x => char.IsDigit(x) || x == '.');
            bool onlyBinaryOctets = this.TextBoxSubnet.Text.Split('.').All(x => Convert.ToInt32(x) < 256);
            
            if (pointCount && onlyNumbers && onlyBinaryOctets)
            {
                this.TextBoxSubnet.BackColor = System.Drawing.Color.White;
                this.ButtonBroadcast.Enabled = true;
            }
            else
            {
                this.TextBoxSubnet.BackColor = System.Drawing.Color.Red;
                this.ButtonBroadcast.Enabled = false;
            }
        }
    }
}