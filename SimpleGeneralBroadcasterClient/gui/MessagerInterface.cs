﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
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
            CenterToScreen();
        }

        /// <summary>
        /// Checks whether the broadcast checkbox is checked or not, and calls
        /// the correct method to send the message.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void ButtonBroadcast_Click(object sender, EventArgs e)
        {
            // Create a new broadcasting interface to update with the IP responses
            BroadcastingInterface inter = new ();
            inter.Show();
            
            // Broadcast the message if the broadcast mode is enabled
            if (CheckBoxBroadcast.Checked)
                BroadcastMessage(TextBoxSubnet.Text, TextBoxMessage.Text, inter);
            
            // Send the message to the specified IP address if the broadcast mode is disabled
            else SendToIP(TextBoxIPAddress.Text, TextBoxMessage.Text, inter);
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
            // Send the message to all the ip addresses in the subnet
            foreach (string ipAddress in this.GetAllIPAddressesForSubnet(subnetMask))
            {
                if (!inter.CanMessage) return;  // Stop sending messages if the user pressed the stop button
                SendToIP(ipAddress, message, inter);
            }
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
            inter.MentionIP(ipAddress);
            
            // Send the message asynchronously and updates the broadcasting interface
            Task.Run(() => this.SendMessage(ip, port, message));
        }
        
        /// <summary>
        /// Asynchronously connects to the server at the specified IP address and port,
        /// sends the message, waits for a response, and disconnects.
        /// </summary>
        /// <param name="ipAddress">The IPAddress to connect to</param>
        /// <param name="port">The port to use</param>
        /// <param name="message">The message to be sent</param>
        private async Task SendMessage(IPAddress ipAddress, int port, string message)
        {
            // Create the socket and connect to it
            IPEndPoint endPoint = new (ipAddress, port);
            using Socket client = new (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            // Make sure that the message is according to the protocol by adding the EOF tag if it's not there
            // Encode the message to bytes in UTF-8
            message = message.EndsWith("<|EOF|>") ? message : message + "<|EOF|>";
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            
            // Connect to the server, send the message, and close the connection
            await client.ConnectAsync(endPoint);
            await client.SendAsync(new ArraySegment<byte>(messageBytes), SocketFlags.None);
            client.Close();
        }
        
        /// <summary>
        /// Returns all of the IP addresses for a subnet mask.
        /// The octets for the number generation should be marked with a '0'.
        /// </summary>
        /// <param name="subnetMask">The subnet mask to use</param>
        /// <returns>A list with all of the IP addresses in the subnet</returns>
        private List<string> GetAllIPAddressesForSubnet(string subnetMask)
        {
            // Initialize the list of IP addresses and checks if the subnet mask is valid
            List<string> ipAddresses = new ();
            if (!subnetMask.Contains(".0")) return ipAddresses;
            
            // Get the regex object for the subnet mask replacement pattern
            Regex replacementRegex = new (Regex.Escape("0"));
            
            // Generate all of the IP addresses for the subnet, replacing the rightmost '0' octet with the
            // every number from 1 to 254, recursively calling this method if there are more '0' octets.
            for (int i = 1; i < 255; i++)
            {
                // Replace the rightmost '0' octet with the current number
                string ip = replacementRegex.Replace(subnetMask, i.ToString(), 1);
                
                // If there are more '0' octets, recursively call this method
                if (ip.Contains(".0")) ipAddresses.AddRange(GetAllIPAddressesForSubnet(ip));
                else ipAddresses.Add(ip);
            }

            return ipAddresses;
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

        /// <summary>
        /// Prevent the message box from being empty
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void TextBoxMessage_TextChanged(object sender, EventArgs e) =>
            ButtonBroadcast.Enabled = !string.IsNullOrWhiteSpace(this.TextBoxMessage.Text);
    }
}