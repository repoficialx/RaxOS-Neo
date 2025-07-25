using Cosmos.HAL;
using Cosmos.System.Network;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaxOS_Neo.Net
{
    internal class Network
    {
        private static string BoolToString(bool @in)
        {
            return @in ? "yes" : "no";
        }
        public static void ShowNetworkDevices()
        {
            Console.WriteLine(string.Format("Device found: {0}", NetworkDevice.Devices.Count));
            foreach (var device in NetworkDevice.Devices)
            {
                Console.WriteLine( "================================");
                Console.WriteLine($"| Nombre       : {device.Name}");
                /*Console.WriteLine($"| Tipo         : {device.CardType}");
                  Console.WriteLine($"| MAC          : {device.MACAddress}");*/
                Console.WriteLine($"| ID del nombre : {device.NameID}");
                Console.WriteLine($"| Esta listo?   : {BoolToString(device.Ready)}");
                Console.WriteLine( "================================");
            }
        }
    }
}
