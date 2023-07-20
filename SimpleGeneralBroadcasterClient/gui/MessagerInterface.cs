using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
// ReSharper disable InconsistentNaming

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
            // Broadcast the message if the broadcast mode is enabled
            if (CheckBoxBroadcast.Checked)
                BroadcastMessage(TextBoxSubnet.Text, TextBoxMessage.Text, null);
            
            // Send the message to the specified IP address if the broadcast mode is disabled
            else SendToIP(TextBoxIPAddress.Text, TextBoxMessage.Text, null);
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
            // Parse the IP address and port
            IPAddress ip = IPAddress.Parse(ipAddress);
            int port = int.Parse(TextBoxPort.Text);
            
            // Send the message asynchronously and updates the broadcasting interface
            Task.Run(() => this.SendMessage(ip, port, message, inter));
        }
        
        /// <summary>
        /// Asynchronously connects to the server at the specified IP address and port,
        /// sends the message, waits for a response, and disconnects.
        /// </summary>
        /// <param name="ipAddress">The IPAddress to connect to</param>
        /// <param name="port">The port to use</param>
        /// <param name="message">The message to be sent</param>
        /// <param name="inter">The broadcasting interface to update with the IP response</param>
        private async Task SendMessage(IPAddress ipAddress, int port, string message, BroadcastingInterface inter)
        {
            // Create the socket and connect to it
            IPEndPoint endPoint = new (ipAddress, port);
            using Socket client = new (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            // Make sure that the message is according to the protocol by adding the EOF tag if it's not there
            message = message.EndsWith("<EOF>") ? message : message + "<EOF>";
            
            await client.ConnectAsync(endPoint);
            // TODO: Send the message to the server and wait for a response (On a cancellation token timeout of 12s)
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
            
            bool onlyBinaryOctets = this.TextBoxSubnet.Text.Split('.')
                .All(x => int.TryParse(x, out int y) && y < 256 && y >= 0);
            
            // The first two octets can't be 0, to allow only local networks
            bool firstOctet = int.TryParse(this.TextBoxSubnet.Text.Split('.')[0], out int evalFO) && evalFO != 0;
            bool secondOctet = int.TryParse(this.TextBoxSubnet.Text.Split('.')[1], out int evalSO) && evalSO != 0;
            bool onlyLocalNetworks = firstOctet && secondOctet;
            
            // Change the validity state of the text box
            ChangeTextBoxValidityState((TextBox) sender, pointCount && onlyNumbers && onlyBinaryOctets && onlyLocalNetworks);

        }

        /// <summary>
        /// Ensures that the port box is formatted correctly, as in a number between 0 and 65535
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        /// <exception cref="NotImplementedException"></exception>
        private void TextBoxPort_TextChanged(object sender, EventArgs e)
        {
            // Ensure that the port box is formatted correctly, as in a number between 0 and 65535
            bool onlyNumbers = this.TextBoxPort.Text.All(char.IsDigit);
            bool validPort = int.TryParse(this.TextBoxPort.Text, out int port) && port < 65536 && port >= 0;
            
            // Change the validity state of the text box
            ChangeTextBoxValidityState((TextBox) sender, onlyNumbers && validPort);
        }

        /// <summary>
        /// Changes the TextBox validity state to either valid or invalid.
        /// When valid, the text is black and the broadcast button is enabled.
        /// When invalid, the text is red and the broadcast button is disabled.
        /// </summary>
        /// <param name="sender">The command sender</param>
        /// <param name="state">The state</param>
        private void ChangeTextBoxValidityState(TextBox sender, bool state)
        {
            sender.ForeColor = state ? System.Drawing.Color.Black : System.Drawing.Color.Firebrick;
            ButtonBroadcast.Enabled = state;
        }
    }
}