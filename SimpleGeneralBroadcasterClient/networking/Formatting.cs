using System.Linq;
using System.Text.RegularExpressions;

namespace SimpleGeneralBroadcasterClient.networking;

/// <summary>
/// This class contains methods for network formatting.
/// </summary>
public static class Formatting
{
    /// <summary>
    /// Checks whether the given string is a valid IPv4 address, for local networks.
    /// </summary>
    /// <param name="ip">The IP address to check</param>
    /// <returns>Whether the string is valid or not</returns>
    public static bool IsLocalIpv4Formatted(string ip) => 
        Regex.IsMatch(ip, @"^(\d{1,3}\.){3}\d{1,3}$");

    /// <summary>
    /// Checks whether the given string is a valid subnet (has zeroes in the last octet), for local networks.
    /// </summary>
    /// <param name="ip">The IP to check for</param>
    /// <returns>Whether the string is valid or not</returns>
    public static bool IsSubnetFormatted(string ip)
    {
        return ip.EndsWith(".0") && Regex.IsMatch(ip, @"^(\d{1,3}\.){3}\d{1,3}$");
    }

    /// <summary>
    /// Checks whether the given string is a valid port number.
    /// </summary>
    /// <param name="portToCheck">The port to check</param>
    /// <returns>Whether the string is valid or not</returns>
    public static bool IsPortNumber(string portToCheck)
    {
        // Ensure that the port box is formatted correctly, as in a number between 0 and 65535
        bool onlyNumbers = portToCheck.All(char.IsDigit);
        bool validPort = int.TryParse(portToCheck, out int port) && port is < 65536 and >= 0;

        return onlyNumbers && validPort;
    }
}