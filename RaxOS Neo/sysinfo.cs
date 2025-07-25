using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaxOS_Neo
{
    internal class sysinfo
    {
        private static string ver;
        private static bool init = false;
        private static void Init()
        {
            init = true;
            string[] SYSINFO = File.ReadAllLines("0:\\RaxOS\\SYSTEM\\sysinfo.inf");

            for (int i = 0; i < SYSINFO.Length - 1; i++)
            {
                if (SYSINFO[i].StartsWith("RaxOS_Version"))
                {
                    ver = SYSINFO[i + 1]; // asumimos que la siguiente línea es la versión
                    break;
                }
            }
        }

        public static string getChannel()
        {
            string channel = null;
            string[] SYSINFO = File.ReadAllLines("0:\\RaxOS\\SYSTEM\\sysinfo.inf");
            for (int i = 0; i < SYSINFO.Length - 1; i++)
            {
                if (SYSINFO[i].StartsWith("RaxOS_Channel"))
                {
                    channel = SYSINFO[i + 1]; // asumimos que la siguiente línea es la versión
                    break;
                }
            }
            return channel;
        }
        public static string versionString
        { 
            get 
            { 
                if (!init)
                {
                    Init();
                }
                return ver; 
            }
            set 
            {
                if (!init)
                {
                    Init();
                }
                ver = value;
            }
        }
        public static int versionInt
        {
            get
            {
                if (!init)
                {
                    Init();
                }
                int a;
                int.TryParse(versionString, out a);
                return a;
            }
            set
            {
                if (!init)
                {
                    Init();
                }
                ver = value.ToString();
            }
        }

    }
}
