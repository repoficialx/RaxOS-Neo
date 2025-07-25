using Cosmos.System.ScanMaps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using K = Cosmos.System.KeyboardManager;
using System.Threading.Tasks;

namespace RaxOS_Neo
{
    internal class InitCLI
    {

        static string username = "";
        static string password = "";
        public static void Init()
        {
        LOGIN:
            Console.Write("Please, log in with Password:");
            string input = Console.ReadLine();
            string[] userData = File.ReadAllLines("0:\\RaxOS\\SYSTEM\\users.db");
            string dbUser = userData[0];
            string dbPass = userData[1];

            if (username == dbUser && password == dbPass)
            {
                Console.WriteLine("Login successful!");
            }
            else
            {
                Console.WriteLine("Invalid credentials. Please try again.");
                goto LOGIN;
            }
        KEYB:
            Console.Write("Keyboard layout: [ES/EN/US/TR/DE/FR]");
            string keylay = Console.ReadLine();
            switch (keylay)
            {
                default:
                    Console.WriteLine("Don't Exist.");
                    goto KEYB;
                case "ES":
                    K.SetKeyLayout(new ESStandardLayout());
                    break;
                case "EN":
                    K.SetKeyLayout(new GBStandardLayout());
                    break;
                case "US":
                    K.SetKeyLayout(new USStandardLayout());
                    break;
                case "TR":
                    K.SetKeyLayout(new TRStandardLayout());
                    break;
                case "DE":
                    K.SetKeyLayout(new DEStandardLayout());
                    break;
                case "FR":
                    K.SetKeyLayout(new FRStandardLayout());
                    break;
            }
            return;
        }
    }
}
