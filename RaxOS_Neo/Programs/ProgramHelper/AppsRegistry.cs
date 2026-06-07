using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RaxOS_Neo
{
    public class AppInfo
    {
        public string name = "";
        public string code = "";
        public string path = "";
        public string indt = "";
    }

    public class AppsInstalled
    {
        public string list = "";
        public Dictionary<string, AppInfo> apps = new();
    }

    public class AppsRegistry
    {
        private static string regPath = @"0:\RaxOS\SYSTEM\apps.reg";
        private static AppsInstalled appsInstalled = new();

        public static void Init()
        {
            if (!File.Exists(regPath))
            {
                Save();
                return;
            }

            Load();
        }

        private static void Load()
        {
            string section = "";

            foreach (var lineRaw in File.ReadAllLines(regPath))
            {
                var line = lineRaw.Trim();
                if (line == "" || line.StartsWith("#")) continue;

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    section = line.Substring(1, line.Length - 2);
                    continue;
                }

                if (section == "list")
                {
                    appsInstalled.list = line;
                    continue;
                }

                var parts = line.Split('=');
                if (parts.Length != 2) continue;

                if (!appsInstalled.apps.ContainsKey(section))
                    appsInstalled.apps[section] = new AppInfo();

                var app = appsInstalled.apps[section];

                if (parts[0] == "name") app.name = parts[1];
                else if (parts[0] == "code") app.code = parts[1];
                else if (parts[0] == "path") app.path = parts[1];
                else if (parts[0] == "indt") app.indt = parts[1];
            }
        }

        public static void RegisterApp(string key, string name, string code, string path)
        {
            var app = new AppInfo
            {
                name = name,
                code = code,
                path = path,
                indt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'")
            };

            appsInstalled.apps[key] = app;
            appsInstalled.list = string.Join(",", appsInstalled.apps.Keys);

            Save();
        }

        private static void Save()
        {
            /*
            using var sw = new StreamWriter(regPath);

            sw.WriteLine("[list]");
            sw.WriteLine(appsInstalled.list);
            sw.WriteLine();

            foreach (var kv in appsInstalled.apps)
            {
                var app = kv.Value;

                sw.WriteLine($"[{kv.Key}]");
                sw.WriteLine($"name={app.name}");
                sw.WriteLine($"code={app.code}");
                sw.WriteLine($"path={app.path}");
                sw.WriteLine($"indt={app.indt}");
                sw.WriteLine();
            }*/
            var sb = new StringBuilder();

            sb.AppendLine("[list]");
            sb.AppendLine(appsInstalled.list);
            sb.AppendLine();

            foreach (var kv in appsInstalled.apps)
            {
                var app = kv.Value;

                sb.AppendLine($"[{kv.Key}]");
                sb.AppendLine($"name={app.name}");
                sb.AppendLine($"code={app.code}");
                sb.AppendLine($"path={app.path}");
                sb.AppendLine($"indt={app.indt}");
                sb.AppendLine();
            }

            File.WriteAllText(regPath, sb.ToString());
        }

        public static AppInfo GetApp(string key)
        {
            if (appsInstalled.apps.ContainsKey(key))
                return appsInstalled.apps[key];

            return null;
        }

        public static IEnumerable<string> ListApps()
        {
            return appsInstalled.apps.Keys;
        }
    }
}
