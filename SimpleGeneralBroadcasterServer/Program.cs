using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using MCSMLauncher.common;
using PgpsUtilsAEFC.common;
using PgpsUtilsAEFC.utils;
// ReSharper disable InvalidXmlDocComment
// ReSharper disable InconsistentNaming

namespace SimpleGeneralBroadcasterServer
{
    /// <summary>
    /// This server will be listening on port 62300 for incoming connections, and
    /// checking if those connections transmit a configured string from a whitelisted
    /// source IP address. If the string is matched with a configuration, then
    /// run the command associated with the message.
    ///
    /// <MessagingProtocol>
    /// - All messages should be finished by the flag "<|EOF|>".
    /// - Additional flags may be added before the EOF flag, such as "<|FLAG|EOF|...>".
    /// - The server should only respond using flags.
    /// - The client is only allowed to use the EOF flag.
    /// - The server is allowed to use any flag.
    /// </MessagingProtocol>
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Initialises a FileSystem for easy file management.
        /// </summary>
        private static FileManager FileSystem { get; } = new FileManager("./data");

        /// <summary>
        /// Sets up everything needed for the server to run and kick starts the server.
        /// 
        /// Command line arguments:
        /// "-i": Defines the IP Address to be used.
        /// "-p": Defines the port to be used.
        /// </summary>
        /// <param name="args">The command-line arguments for the server running</param>
        public static void Main(string[] args)
        {
            try
            {
                // Set up the configuration files for the server.
                Program.SetupConfiguration();
                
                // Get the IP address of the local machine.
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddr = ipHost.AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                int port = 62300;
                
                // If there is a command line argument specifying the ip address, use that instead.
                if (args.Length > 0 && args.Contains("-i"))
                    ipAddr = IPAddress.Parse(args[args.ToList().IndexOf("-i") + 1]);
                    
                // If there is a command line argument specifying the port, use that instead.
                if (args.Length > 0 && args.Contains("-p"))
                    port = int.Parse(args[args.ToList().IndexOf("-p") + 1]);

                // If no IPv4 address is found, throw an exception.
                if (ipAddr == null) throw new SystemException("No IPv4 address found for the local machine.");

                // Create a TCP/IP socket typed as a stream for the listener.
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port); // Local endpoint
                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the local endpoint, and listen for incoming connections.
                RunServer(localEndPoint, listener);
            }
            catch (Exception e) { Logging.LOGGER.Fatal(e); }
        }
        
        /// <summary>
        /// Creates the configuration files for the server if they don't exist.
        /// These will be populated by intructions from the app.config file.
        /// </summary>
        private static void SetupConfiguration()
        {
            // Sets the logging file path to the logs folder as a file with the current date as the name.
            Logging.LOGGER.LoggingFilePath = FileSystem.AddSection("logs").AddDocument(Logging.LOGGER.LoggingSession + ".log");
            
            // Get the configuration file instructions from the app.config file.
            string commands_config = ConfigurationManager.AppSettings.Get("commands_configuration_file");
            string ip_whitelist = ConfigurationManager.AppSettings.Get("ips_whitelist_file");

            // Check if the server configuration file exists, if not, create it.
            if (FileSystem.GetFirstDocumentNamed("commands.cfg") == null)
                FileUtils.DumpToFile(FileSystem.AddDocument("commands.cfg"), new List<string> { commands_config });
            
            // Check if the IP Whitelist file exists, if not, create it.
            if (FileSystem.GetFirstDocumentNamed("ip-whitelist.cfg") == null)
                FileUtils.DumpToFile(FileSystem.AddDocument("ip-whitelist.cfg"), new List<string> { ip_whitelist });
        }

        /// <summary>
        /// Runs the server on the specified endpoint
        /// </summary>
        private static void RunServer(IPEndPoint endpoint, Socket listener)
        {
            listener.Bind(endpoint);
            listener.Listen(5);
            Logging.LOGGER.Info($"Running server on {endpoint.Address}:{endpoint.Port}");

            while (true)
            {
                // Wait for a connection, suspending the thread until it arrives.
                Logging.LOGGER.Info("Waiting for a connection...", LoggingType.CONSOLE);
                Socket client = listener.Accept();
                    
                // Set up the client socket server-wise, before the connection
                client.ReceiveTimeout = 10000;
                string clientIPAddr = ((IPEndPoint) client.RemoteEndPoint).Address.ToString();
                Logging.LOGGER.Info($"Connection received from {clientIPAddr}");
                
                // Check if the IP Address is whitelisted, if not, close the connection.
                string whitelistPath = FileSystem.AddDocument("ip-whitelist.cfg");
                List<string> whitelist = FileUtils.ReadFromFile(whitelistPath);

                if (!whitelist.Contains(clientIPAddr))
                {
                    client.Send(Encoding.UTF8.GetBytes("<|BLOCKED|EOF|>"));
                    client.Close();
                    Logging.LOGGER.Warn($"Connection from {clientIPAddr} was blocked.");
                    continue;
                }
                
                // Log the connection acception
                Logging.LOGGER.Info($"Connection from {clientIPAddr} accepted.");

                // Once a connection is established, read the message, on a 1024 byte buffer.
                byte[] buffer = new byte[1024];
                string data = String.Empty;

                try
                {
                    // Receive the data into the buffer in chunks, breaking when the message is complete. (<|EOF|> is found)
                    while (true)
                    {
                        int byteData = client.Receive(buffer);
                        Logging.LOGGER.Info($"Received data from client with size {byteData} bytes.");
                        data += Encoding.UTF8.GetString(buffer, 0, byteData); // UTF-8 Message Support
                        
                        if (data.IndexOf("<|EOF|>", StringComparison.Ordinal) > -1)
                            break;
                    }
                }
                    
                // If the socket times out, close the connection, logging the event as a warning.
                catch (SocketException e)
                {
                    if (e.SocketErrorCode != SocketError.TimedOut) return;
                    Logging.LOGGER.Warn($"Socket from {clientIPAddr} timed out, closing connection.");
                    client.Send(Encoding.UTF8.GetBytes("<|TIMEOUT|EOF|>"));
                    client.Close();
                    continue;
                }
                
                // The message has been received, and the connection can be closed, and we can try for a command.
                Logging.LOGGER.Info($"Message received from {clientIPAddr}: {data}");
                client.Send(Encoding.UTF8.GetBytes("<|OK|EOF|>"));
                client.Close();
                
                // Remove the <|EOF|> from the message, so it can be mapped to a command.
                string cleanData = data.Replace("<|EOF|>", "").Trim();
                TryRunCommand(cleanData);
            }
        }

        /// <summary>
        /// Checks if the message received is a valid command, and if so, runs it.
        /// </summary>
        /// <param name="message">The message received from the client</param>
        private static void TryRunCommand(string message)
        {
            // Check if the message does not match a command, if so, return.
            Dictionary<string, string> commands = ParseCommandConfigurations();
            if (!commands.ContainsKey(message)) return;
            
            // Otherwise, the message is a valid command, and can be run.
            string command = commands[message];
            Process.Start("cmd.exe", "/c " +command);
        }

        /// <summary>
        /// Parses the command configuration file into a dictionary of mappings.
        /// </summary>
        /// <returns>A Dictionary(string, string) containing the messages mapped to a command.</returns>
        private static Dictionary<string, string> ParseCommandConfigurations()
        {
            // Get the command configuration file path, and read the file into a list of strings.
            string commandConfigPath = FileSystem.GetFirstDocumentNamed("commands.cfg");
            
            // Parse every line of the command configuration file into a dictionary.
            Dictionary<string, string> commandMappings = new Dictionary<string, string>();
            
            foreach (string line in FileUtils.ReadFromFile(commandConfigPath))
            {
                // Skip the line if it's a comment or doesn't contain a mapping.
                if (line.StartsWith("#") || !line.Contains("-=>")) continue;
                string[] splitLine = line.Split(new [] {"-=>"}, StringSplitOptions.RemoveEmptyEntries);

                // Ensure that the line is a valid mapping, and add it to the dictionary.
                if (splitLine.Length != 2) continue;
                commandMappings.Add(splitLine[0].Trim(), splitLine[1].Trim());
            }

            return commandMappings;
        }
    }
}