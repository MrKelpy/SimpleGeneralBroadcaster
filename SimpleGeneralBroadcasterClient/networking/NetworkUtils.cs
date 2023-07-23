using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SimpleGeneralBroadcasterClient.networking;

/// <summary>
/// This class contains all of the methods used to perform general networking tasks.
/// </summary>
public static class NetworkUtils
{
    /// <summary>
    /// Yields all of the IP addresses for a subnet.
    /// The octets for the number generation should be marked with a '0'.
    /// </summary>
    /// <param name="subnet">The subnet to use</param>
    /// <returns>Yields all the IP addresses in the subnet</returns>
    public static IEnumerable<string> GetAllIPAddressesForSubnet(string subnet)
    {
        // Initialize the list of IP addresses and checks if the subnet is valid
        if (!subnet.Contains(".0")) yield break;
            
        // Get the regex object for the subnet replacement pattern
        Regex replacementRegex = new (Regex.Escape("0"));
            
        // Generate all of the IP addresses for the subnet, replacing the rightmost '0' octet with the
        // every number from 1 to 254, recursively calling this method if there are more '0' octets.
        for (int i = 1; i < 255; i++)
        {
            // Replace the rightmost '0' octet with the current number
            string ip = replacementRegex.Replace(subnet, i.ToString(), 1);
                
            // If there are more '0' octets, recursively call this method and yield return the results
            if (ip.Contains(".0"))
            {
                foreach (string s in GetAllIPAddressesForSubnet(ip)) 
                    yield return s;
            }
            
            yield return ip;
        }
    }
}