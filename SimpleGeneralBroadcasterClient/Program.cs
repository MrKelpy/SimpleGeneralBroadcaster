using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleGeneralBroadcasterClient.gui;

namespace SimpleGeneralBroadcasterClient
{
    static class Program
    {
        /// <summary>
        /// Whether the application is running in console mode or not.
        /// The console mode is used to send messages from the command line, whilst
        /// the GUI mode is used to send messages from the GUI interface provided.
        /// </summary>
        private static bool ConsoleMode { get; set; } = false;

        /// <summary>
        /// The port to use to send messages.
        /// Works in conjunction with the ConsoleMode property.
        /// </summary>
        private static string Port { get; set; } = "63200";
        
        /// <summary>
        /// The subnet mask to use to send messages.
        /// Works in conjunction with the ConsoleMode property.
        /// </summary>
        private static string SubnetMask { get; set; } = "192.168.1.0";
        
        /// <summary>
        /// The message to send.
        /// Works in conjunction with the ConsoleMode property.
        /// </summary>
        private static string Message { get; set; } = String.Empty;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The command line arguments of the app</param>
        [STAThread]
        static void Main(string[] args)
        {
            // Process the command line arguments if there are any.
            Program.ProcessCommandLineArguments(args);
            
            // If there are no command line arguments, run the GUI.
            if (!ConsoleMode) {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MessagerInterface(ConsoleMode));
                return;
            }
            
            // If there are command line arguments, run the app in console mode.
            Console.WriteLine(@"SGB Client - Console Mode");
            Console.WriteLine(@"-------------------------");
            Console.WriteLine(@$"Message: {Message}\nSubnet Mask: {SubnetMask}\nPort: {Port}");
            Console.WriteLine(@"-------------------------");

            // Initialise the messager interface and validate the inputs.
            MessagerInterface messager = new MessagerInterface(ConsoleMode);
            messager.TextBoxMessage.Text = Message;
            messager.TextBoxIPAddress.Text = SubnetMask;
            messager.TextBoxPort.Text = Port;

            // If at least one of the inputs are invalid, return.
            if (!messager.ButtonBroadcast.Enabled)
            {
                Console.WriteLine(@"Status: Message not sent.");
                Console.WriteLine(@"One or more of the inputs are invalid. Please try again.");
                return;
            }
            
            // If all the inputs are valid, send the message.
            messager.ButtonBroadcast.PerformClick();
            Console.WriteLine(@"Status: Message sent.");
        }

        /// <summary>
        /// Processes the command line arguments passed to the application, setting the
        /// properties of the class accordingly.
        /// </summary>
        /// <param name="args">The command line arguments of the app</param>
        private static void ProcessCommandLineArguments(string[] args)
        {
            // If there are no arguments, return.
            if (args.Length <= 0) return;
            
            // If there are arguments, set the console mode to true.
            ConsoleMode = true; 

            // If there is a command line argument specifying the port, use that instead.
            if (args.Length > 0 && args.Contains("-p"))
                Port = args[args.ToList().IndexOf("-p") + 1];

            // If there is a command line argument specifying the subnet mask, use that instead.
            if (args.Length > 0 && args.Contains("-s"))
                SubnetMask = args[args.ToList().IndexOf("-s") + 1];
            
            // If there is a command line argument specifying the message, use that instead.
            if (args.Length > 0 && args.Contains("-m"))
                Message = args[args.ToList().IndexOf("-m") + 1];
        }
    }
}