using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleGeneralBroadcasterClient.gui;
using SimpleGeneralBroadcasterClient.networking;

// ReSharper disable InvalidXmlDocComment

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
        /// The socket to use to send messages.
        /// </summary>
        public static Socket Client = new (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 

        /// <summary>
        /// The port to use to send messages.
        /// Works in conjunction with the ConsoleMode property.
        /// </summary>
        private static string Port { get; set; } = ConfigurationManager.AppSettings.Get("default.port");
        
        /// <summary>
        /// The subnet to use to send messages.
        /// Works in conjunction with the ConsoleMode property.
        /// </summary>
        private static string Subnet { get; set; } = ConfigurationManager.AppSettings.Get("default.subnet");
        
        /// <summary>
        /// The message to send.
        /// Works in conjunction with the ConsoleMode property.
        /// </summary>
        private static string Message { get; set; } = String.Empty;
        
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int pid);

        /// <summary>
        /// The main entry point for the application.
        ///
        /// Command line arguments:
        /// -p <port> - The port to use to send messages.
        /// -s <subnet> - The subnet to use to send messages.
        /// -m <message> - The message to send.
        /// </summary>
        /// <param name="args">The command line arguments of the app</param>
        [STAThread]
        [SuppressMessage("ReSharper", "LocalizableElement")]
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
            
            // Attach the console to the process or allocate a new one.
            if ( !AttachConsole(-1) ) AllocConsole();
            
            // If there are command line arguments, run the app in console mode.
            Console.WriteLine(@"SGB Client - Console Mode");
            Console.WriteLine(@"-------------------------");
            Console.WriteLine($"Message: {Message}\nSubnet: {Subnet}\nPort: {Port}");
            Console.WriteLine(@"-------------------------");
            
            // If at least one of the inputs is invalid, return.
            if (string.IsNullOrWhiteSpace(Message) || !Formatting.IsSubnetFormatted(Subnet) || !Formatting.IsPortNumber(Port))
            {
                Console.WriteLine(@"Status: Messaging Failed");
                Console.WriteLine(@"One or more of the inputs are invalid. Please try again.");
                return;
            }
            
            // If all the inputs are valid, send the message.
            Console.WriteLine(@"Status: Messaging OK");
            Messaging.BroadcastMessage(Subnet, int.Parse(Port), Message, ConsoleMode, new BroadcastingInterface());
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
                Port = args[args.ToList().IndexOf("-p") + 1].Trim();

            // If there is a command line argument specifying the subnet, use that instead.
            if (args.Length > 0 && args.Contains("-s"))
                Subnet = args[args.ToList().IndexOf("-s") + 1].Trim();
            
            // If there is a command line argument specifying the message, use that instead.
            if (args.Length > 0 && args.Contains("-m"))
                Message = args[args.ToList().IndexOf("-m") + 1].Trim();
        }
    }
}