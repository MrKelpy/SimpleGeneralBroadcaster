using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        /// Checks whether the broadcast checkbox is checked or not, and calls
        /// the correct method to send the message.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void ButtonBroadcast_Click(object sender, EventArgs e)
        {
            
        }
        
        /// <summary>
        /// Broadcasts a message to all devices through permutations on a specified network
        /// according to the subnet mask.
        /// </summary>
        /// <param name="subnetMask">The subnet mask to use</param>
        /// <param name="message">The message to broadcast</param>
        /// <param name="inter">The broadcasting interface to update with the IP responses</param>
        private void BroadcastMessage(string subnetMask, string message, BroadcastingInterface inter)
        {
            // Get the subnet mask octets in an internally usable format (XXX.XXX.{O1}/XXX.{O2})
            string[] subnetOctets = subnetMask.Split('.');
            subnetOctets[2] = int.Parse(subnetOctets[2]) == 0 ? "{O1}" : subnetOctets[2];
            subnetOctets[3] = int.Parse(subnetOctets[3]) == 0 ? "{O2}" : subnetOctets[2];

            // Get all the subnet octet permutations
            List<string> ipAddresses = (List<string>)
                from i in Enumerable.Range(1, 254)
                from j in Enumerable.Range(1, 254)
                select TextBoxSubnet.Text.Replace("{O1}", i.ToString()).Replace("{O2}", j.ToString());
            
            // Send the message to all the ip addresses
            foreach (string ipAddress in ipAddresses)
                SendToIP(ipAddress, message, inter);
        }
        
        /// <summary>
        /// Sends a message to a specific IP address.
        /// </summary>
        /// <param name="ipAddress">The ip address to send the message to</param>
        /// <param name="message">The message to be sent</param>
        /// <param name="inter">The broadcasting interface to update with the IP response</param>

        private void SendToIP(string ipAddress, string message, BroadcastingInterface inter)
        {
            IPAddress ip = IPAddress.Parse(ipAddress);
            
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
            bool onlyBinaryOctets = this.TextBoxSubnet.Text.Split('.').All(x => Convert.ToInt32(x) < 256 && Convert.ToInt32(x) >= 0);
            
            // The first two octets can't be 0, to allow only local networks
            bool firstOctet = Convert.ToInt32(this.TextBoxSubnet.Text.Split('.')[0]) != 0;
            bool secondOctet = Convert.ToInt32(this.TextBoxSubnet.Text.Split('.')[1]) != 0;
            bool onlyLocalNetworks = firstOctet && secondOctet;
            
            if (pointCount && onlyNumbers && onlyBinaryOctets && onlyLocalNetworks)
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