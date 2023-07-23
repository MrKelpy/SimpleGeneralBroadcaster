using System;
using System.Windows.Forms;

namespace SimpleGeneralBroadcasterClient.gui
{
    /// <summary>
    /// This interface is used to display the IP addresses that the message was sent to.
    /// </summary>
    public partial class BroadcastingInterface : Form
    {
        
        /// <summary>
        /// Whether the user can send messages or not. (Serves as a stop button)
        /// </summary>
        public bool CanMessage { get; private set; } = true;
        
        public BroadcastingInterface()
        {
            InitializeComponent();
            CenterToParent();
        }
        
        /// <summary>
        /// Writes the specified text to the text box.
        /// </summary>
        /// <param name="text">The text to be written</param>
        public void Write(string text) => TextBoxConsole.AppendText(" " + text); 

        /// <summary>
        /// Mentions that a message was sent to the specified IP address in the text box.
        /// </summary>
        /// <param name="ip">The IP address the message was sent to</param>
        public void MentionIP(string ip) => Write($"Sending message to {ip}...{Environment.NewLine}");
        
        /// <summary>
        /// When the form is closed, the CanMessage property is set to false, so that the
        /// ips stop being sent messages.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void BroadcastingInterface_FormClosed(object sender, FormClosedEventArgs e) =>
            this.CanMessage = false;
    }
}