using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimpleGeneralBroadcasterClient.gui;

namespace SimpleGeneralBroadcasterClient.networking;

/// <summary>
/// This class contains all of the methods used to perform network messaging operations.
/// </summary>
public static class Messaging
{
    /// <summary>
    /// Broadcasts a message to all devices through permutations on a specified network
    /// according to the subnet.
    /// </summary>
    /// <param name="subnet">The subnet to use</param>
    /// <param name="port">The port to use</param>
    /// <param name="message">The message to broadcast</param>
    /// <param name="consoleMode">IP Mentioning mode</param>
    /// <param name="inter">The broadcasting interface to update with the IP responses</param>
    public static void BroadcastMessage(string subnet, int port, string message, bool consoleMode, BroadcastingInterface inter)
    {
        // Send the message to all the ip addresses in the subnet
        foreach (string ipAddress in NetworkUtils.GetAllIPAddressesForSubnet(subnet))
        {
            if (!inter.CanMessage) return;  // Stop sending messages if the user pressed the stop button
            SendToIP(ipAddress, port, message, consoleMode, inter);
        }
        
        if (consoleMode) Console.WriteLine(@"Broadcasting finished.");
        else inter.Write(@"Broadcasting finished.");
    }
    
    /// <summary>
    /// Sends a message to a specific IP address.
    /// </summary>
    /// <param name="ipAddress">The ip address to send the message to</param>
    /// <param name="port">The port to use</param>
    /// <param name="message">The message to be sent</param>
    /// <param name="consoleMode">IP Mentioning mode</param>
    /// <param name="inter">The broadcasting interface to update with the IP response</param>
    public static void SendToIP(string ipAddress, int port, string message, bool consoleMode, BroadcastingInterface inter)
    {
        // Parse the IP address and port
        IPAddress ip = IPAddress.Parse(ipAddress);
        
        // Update the broadcasting interface with the IP address or mention it in the console
        // Depending on whether the application is running in console mode or not
        if (!consoleMode) inter.MentionIP(ipAddress);
        else Console.WriteLine($@"Sending message to {ipAddress}...");

        // Send the message asynchronously and updates the broadcasting interface
        Task.Run(() => Messaging.SendMessage(ip, port, message));
    }
    
    /// <summary>
    /// Asynchronously connects to the server at the specified IP address and port,
    /// sends the message, waits for a response, and disconnects.
    /// </summary>
    /// <param name="ipAddress">The IPAddress to connect to</param>
    /// <param name="port">The port to use</param>
    /// <param name="message">The message to be sent</param>
    private static async Task SendMessage(IPAddress ipAddress, int port, string message)
    {
        // Create the socket and connect to it
        IPEndPoint endPoint = new (ipAddress, port);
        
        // Make sure that the message is according to the protocol by adding the EOF tag if it's not there
        // Encode the message to bytes in UTF-8
        message = message.EndsWith("<|EOF|>") ? message : message + "<|EOF|>";
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        
        // Connect to the server, send the message, and close the connection
        await Program.Client.ConnectAsync(endPoint);
        await Program.Client.SendAsync(new ArraySegment<byte>(messageBytes), SocketFlags.None);
        Program.Client.Disconnect(true);
    }
}