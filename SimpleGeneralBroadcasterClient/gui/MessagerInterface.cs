using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleGeneralBroadcasterClient.networking;

// ReSharper disable InconsistentNaming
// ReSharper disable InvalidXmlDocComment

namespace SimpleGeneralBroadcasterClient.gui
{
    
    /// <summary>
    /// The messaging interface for the application; Allows the user to send either broadcast
    /// or ip-specific messages to a server.
    ///
    /// <MessagingProtocol>
    /// - All messages should be finished by the flag "<|EOF|>".
    /// - Additional flags may be added to the end EOF flag, such as "<|EOF|FLAG|...>".
    /// - The server should only respond using flags.
    /// - The client is only allowed to use the EOF flag.
    /// - The server is allowed to use any flag.
    /// </MessagingProtocol>
    /// </summary>
    /// <param name="consoleMode">Whether or not to run the application in the console only<param>
    public partial class MessagerInterface : Form
    {
        
        /// <summary>
        /// Whether the application is running in console mode or not.
        /// </summary>
        private bool ConsoleMode { get; }
        
        /// <summary>
        /// Main constructor for the messaging interface.
        /// Assigns the console mode to the class.
        /// </summary>
        /// <param name="consoleMode">Whether or not to run the application in the console only</param>
        public MessagerInterface(bool consoleMode)
        {
            InitializeComponent();
            CenterToScreen();
            this.ConsoleMode = consoleMode;
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
            int port = int.Parse(TextBoxPort.Text);
            if (!ConsoleMode) inter.Show();
            
            // Broadcast the message if the broadcast mode is enabled
            if (CheckBoxBroadcast.Checked)
                Messaging.BroadcastMessage(TextBoxSubnet.Text, port, TextBoxMessage.Text, ConsoleMode, inter);
            
            // Send the message to the specified IP address if the broadcast mode is disabled
            else Messaging.SendToIP(TextBoxIPAddress.Text, port, TextBoxMessage.Text, ConsoleMode, inter);
        }

        /// <summary>
        /// Inverts the state of the IP Address text box relative to the broadcast check box.
        /// Ensures that the broadcast check box's state is the same as the subnet box's state.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void CheckBoxBroadcast_CheckedChanged(object sender, EventArgs e)
        {
            this.TextBoxIPAddress.Enabled = !this.CheckBoxBroadcast.Checked;
            this.TextBoxSubnet.Enabled = this.CheckBoxBroadcast.Checked;
        }

        /// <summary>
        /// Ensure that the subnet box is formatted correctly, as in XXX.XXX.XXX/0.0 (Ipv4)
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event argument</param>
        private void TextBoxSubnet_TextChanged(object sender, EventArgs e)
        {
            // Cast the sender to a text box
            TextBox textBox = (TextBox) sender;
            string ip = textBox.Text;
                
            // Check if the IP is formatted correctly and change the validity state accordingly
            ChangeTextBoxValidityState((TextBox) sender, Formatting.IsSubnetFormatted(ip));
        }

        /// <summary>
        /// Ensures that the IP address box is formatted correctly, as in XXX.XXX.XXX.XXX (Ipv4)
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void TextBoxIPAddress_TextChanged(object sender, EventArgs e)
        {
            // Cast the sender to a text box
            TextBox textBox = (TextBox) sender;
            string ip = textBox.Text;
                
            // Check if the IP is formatted correctly and change the validity state accordingly
            ChangeTextBoxValidityState((TextBox) sender, Formatting.IsLocalIpv4Formatted(ip));
        }
        
        /// <summary>
        /// Prevent a disabled text box from being validated towards the button click. This
        /// way, the user will be able to click the button even if the mode they're not in
        /// is invalid.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void TextBoxIPAddress_EnabledChanged(object sender, EventArgs e)
        {
            // Get the text box from the sender
            TextBox textBox = (TextBox) sender;
            
            if (!textBox.Enabled) ChangeTextBoxValidityState(textBox, true);
            else TextBoxIPAddress_TextChanged(sender, e);
        }

        /// <summary>
        /// Prevent a disabled text box from being validated towards the button click. This
        /// way, the user will be able to click the button even if the mode they're not in
        /// is invalid.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void TextBoxSubnet_EnabledChanged(object sender, EventArgs e)
        {
            // Get the text box from the sender
            TextBox textBox = (TextBox) sender;
            
            if (!textBox.Enabled) ChangeTextBoxValidityState(textBox, true);
            else TextBoxSubnet_TextChanged(sender, e);
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
            bool validPort = int.TryParse(this.TextBoxPort.Text, out int port) && port is < 65536 and >= 0;
            
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
            if (sender.Enabled) ButtonBroadcast.Enabled = state;
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