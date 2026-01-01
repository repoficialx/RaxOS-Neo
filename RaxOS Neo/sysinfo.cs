using Cosmos.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaxOS_Neo
{
    internal static class SysInfo
    {
        private static bool init = false;

        private static string version;
        private static string channel;
        private static bool installed;

        private static void Init()
        {
            if (init) return;
            init = true;

            string[] lines = File.ReadAllLines("0:\\RaxOS\\SYSTEM\\sysinfo.inf");

            foreach (var line in lines)
            {
                if (line.Contains("\"version\""))
                    version = ExtractValue(line);

                else if (line.Contains("\"channel\""))
                    channel = ExtractValue(line);

                else if (line.Contains("\"installed\""))
                    installed = ExtractValue(line) == "true";
            }
        }

        private static string ExtractValue(string line)
        {
            int start = line.IndexOf(":") + 1;
            return line.Substring(start)
                       .Replace("\"", "")
                       .Replace(",", "")
                       .Trim();
        }

        // ───── PROPIEDADES ─────

        public static string Version
        {
            get { Init(); return version; }
            set { Init(); version = value; }
        }

        public static int VersionInt
        {
            get
            {
                Init();
                int.TryParse(version, out int v);
                return v;
            }
            set
            {
                Init();
                version = value.ToString();
            }
        }

        public static string Channel
        {
            get { Init(); return channel; }
            set { Init(); channel = value; }
        }

        public static bool Installed
        {
            get { Init(); return installed; }
        }
    }
}
