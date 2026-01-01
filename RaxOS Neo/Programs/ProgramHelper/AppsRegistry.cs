using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace RaxOS_Neo
{
    public class AppInfo
    {
        public string name { get; set; }
        public string code { get; set; }
        public string path { get; set; }
        public string indt { get; set; }
    }

    public class AppsInstalled
    {
        public string list { get; set; } = "";
        public Dictionary<string, AppInfo> apps { get; set; } = new();
    }

    public class AppsRegistry
    {
        private static string jsonPath = "0:\\RaxOS\\SYSTEM\\apps.json";
        private static AppsInstalled appsInstalled = new();

        public static void Init()
        {
            if (File.Exists(jsonPath))
            {
                string json = File.ReadAllText(jsonPath);
                try
                {
                    var doc = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, JsonElement>>>(json);
                    if (doc != null && doc.ContainsKey("appsInstalled"))
                    {
                        appsInstalled.list = doc["appsInstalled"]["list"].GetString() ?? "";
                        foreach (var kv in doc["appsInstalled"])
                        {
                            if (kv.Key == "list") continue;
                            appsInstalled.apps[kv.Key] = JsonSerializer.Deserialize<AppInfo>(kv.Value.GetRawText());
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Error parsing apps.json, recreating file...");
                    Save();
                }
            }
            else
            {
                Save(); // crear archivo vacío
            }
        }

        public static void RegisterApp(string key, string name, string code, string path)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'K");
            var app = new AppInfo
            {
                name = name,
                code = code,
                path = path,
                indt = timestamp
            };
            appsInstalled.apps[key] = app;

            // actualizar lista
            appsInstalled.list = string.Join(",", appsInstalled.apps.Keys);

            Save();
        }

        private static void Save()
        {
            var dict = new Dictionary<string, object>();
            var appsDict = new Dictionary<string, object>();
            appsDict["list"] = appsInstalled.list;
            foreach (var kv in appsInstalled.apps)
                appsDict[kv.Key] = kv.Value;

            dict["appsInstalled"] = appsDict;

            string json = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(jsonPath, json);
        }

        public static AppInfo? GetApp(string key)
        {
            if (appsInstalled.apps.ContainsKey(key)) return appsInstalled.apps[key];
            return null;
        }

        public static IEnumerable<string> ListApps()
        {
            return appsInstalled.apps.Keys;
        }
    }
}
