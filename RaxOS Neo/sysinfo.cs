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
    public class UserInfo
    {
        private static string username;
        private static string hashedPassword;
        private static bool _init;

        public static string Init(string var = "")
        {
            _init = true;
            string[] lines = File.ReadAllLines("0:\\RaxOS\\SYSTEM\\users.db");
            string dbUser = "", dbHash = "", algo = "SHA256";

            foreach (var line in lines)
            {
                if (line.StartsWith("username="))
                    dbUser = line.Substring("username=".Length).Trim();
                else if (line.StartsWith("password_hash="))
                    dbHash = line.Substring("password_hash=".Length).Trim();
                else if (line.StartsWith("hash_algo="))
                    algo = line.Substring("hash_algo=".Length).Trim();
            }
            username = dbUser;
            hashedPassword = dbHash;
            if (var == "usr")
            {
                return dbUser;
            }
            else if (var == "hsp")
            {
                return dbHash; 
            }
            return string.Empty;
        }

        public static string Username
        {
            get { return _init ? username : Init("usr"); }
            set { username = value; }
        }

        public static string HashedPassword
        {
            get { return _init ? hashedPassword : Init("hsp"); }
            set { hashedPassword = value; }
        }
    }
}