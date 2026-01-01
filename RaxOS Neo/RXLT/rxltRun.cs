using Cosmos.System;
using System;
using System.Collections.Generic;
using System.IO;
using static RaxOS_Neo.Programs.Settings;
using Console = System.Console;

namespace RaxOS_Neo
{
    internal static class Variables
    {
        private static Dictionary<string, string> _vars = new();
        public static void Set(string name, string value) => _vars[name] = value;
        public static string Get(string name) => _vars.ContainsKey(name) ? _vars[name] : "";
    }

    internal static class RXLTRun
    {
        public static void ExecuteFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"RXLT file not found: {filePath}");
                return;
            }

            string[] lines = File.ReadAllLines(filePath);
            string currentCommand = null;

            foreach (var rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;

                if (line.StartsWith("command "))
                {
                    currentCommand = ExtractQuoted(line);
                    continue;
                }

                if (line.StartsWith("print "))
                {
                    string text = ExtractQuoted(line);
                    Console.WriteLine(ReplaceVars(text));
                    continue;
                }

                if (line.StartsWith("warn "))
                {
                    string text = ExtractQuoted(line);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("WARNING: " + ReplaceVars(text));
                    Console.ResetColor();
                    continue;
                }

                if (line.StartsWith("confirm "))
                {
                    string text = ExtractQuoted(line);
                    Console.WriteLine(ReplaceVars(text) + " [Y/N]");
                    var input = Console.ReadLine()?.ToUpper();
                    if (input != "Y")
                    {
                        Console.WriteLine("Action canceled.");
                        break;
                    }
                    continue;
                }

                if (line.StartsWith("call "))
                {
                    string action = line.Substring(5).Trim();
                    ExecuteCall(action);
                    continue;
                }

                if (line.StartsWith("label ")) continue; // para flujo futuro
            }
        }

        // ─── HELPERS ───
        private static string ExtractQuoted(string line)
        {
            int first = line.IndexOf('"');
            int last = line.LastIndexOf('"');
            if (first >= 0 && last > first) return line.Substring(first + 1, last - first - 1);
            return line;
        }

        private static string ReplaceVars(string text)
        {
            foreach (var kv in typeof(Variables).GetField("_vars", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                     .GetValue(null) as Dictionary<string, string>)
            {
                text = text.Replace(kv.Key, kv.Value);
            }
            return text;
        }

        private static void ExecuteCall(string action)
        {
            // SCIF helpers
            if (action.StartsWith("read_input "))
            {
                string varName = action.Substring(11).Trim();
                string? input = Console.ReadLine();
                Variables.Set(varName, input ?? "");
            }
            else if (action.StartsWith("read_file "))
            {
                string varName = action.Substring(10).Trim();
                string path = Variables.Get(varName);
                if (File.Exists(path))
                    Variables.Set(varName + "_contents", string.Join("\n", File.ReadAllLines(path)));
                else
                    Variables.Set(varName + "_contents", "File not found!");
            }
            else if (action.StartsWith("print_file "))
            {
                string varName = action.Substring(11).Trim();
                Console.WriteLine(Variables.Get(varName + "_contents"));
            }

            // SimpleNano helpers
            else if (action == "read_multiline")
            {
                Console.WriteLine("Type text line by line. ESC to finish.");
                var key = default(ConsoleKeyInfo);
                var lines = new List<string> { "" };
                int curX = 0, curY = 0;

                while (true)
                {
                    key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape) break;
                    else if (key.Key == ConsoleKey.Backspace)
                    {
                        if (curX > 0)
                        {
                            lines[curY] = lines[curY].Remove(curX - 1, 1);
                            curX--;
                        }
                        else if (curY > 0)
                        {
                            curX = lines[curY - 1].Length;
                            lines[curY - 1] += lines[curY];
                            lines.RemoveAt(curY);
                            curY--;
                        }
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        string rest = lines[curY].Substring(curX);
                        lines[curY] = lines[curY].Substring(0, curX);
                        lines.Insert(curY + 1, rest);
                        curY++;
                        curX = 0;
                    }
                    else
                    {
                        lines[curY] = lines[curY].Insert(curX, key.KeyChar.ToString());
                        curX++;
                    }
                    Console.Clear();
                    for (int i = 0; i < lines.Count; i++) Console.WriteLine(lines[i]);
                    Console.SetCursorPosition(curX, curY);
                }

                string path = Variables.Get("path");
                File.WriteAllLines(path, lines);
            }

            // RaxOS helpers
            else if (action == "wait_key") Console.ReadKey();
            else if (action == "clear_screen") Console.Clear();
            else if (action == "net_init") Cosmos.HAL.Network.NetworkInit.Init();
            else if (action == "factory_reset") SystemReserved.DONOTENTER.Format();
            else if (action.StartsWith("resolution"))
            {
                string[] parts = action.Split(' ');
                if (parts.Length == 3 &&
                    int.TryParse(parts[1], out int res) &&
                    int.TryParse(parts[2], out int bitColor))
                    ResolutionSettings.SetResolution(ResolutionSettings.Create(res, bitColor));
            }
            else Console.WriteLine($"Unknown call: {action}");
        }
    }
}
