using System;
using System.Collections.Generic;
using System.IO;
using c = System.Console;

namespace RaxOS_Neo.Programs
{
    internal class NanoSimple
    {
        public static void Launch()
        {
            c.Clear();
            c.WriteLine("SimpleNano v1.0");
            c.WriteLine("Type your document. Press ESC to finish and save.");
            c.Write("File path [0:\\myfolder\\myfile.txt]: ");
            string path = c.ReadLine() ?? "0:\\myfolder\\newfile.txt";

            List<string> lines = new List<string> { "" };
            int curX = 0;
            int curY = 0;

            c.Clear();
            Draw(lines, curX, curY);

            while (true)
            {
                var key = c.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
                {
                    // Guardar y salir
                    File.WriteAllLines(path, lines);
                    c.Clear();
                    c.WriteLine($"Saved {path}");
                    break;
                }
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

                Draw(lines, curX, curY);
            }
        }

        private static void Draw(List<string> lines, int curX, int curY)
        {
            c.Clear();
            for (int i = 0; i < lines.Count; i++)
            {
                c.WriteLine(lines[i]);
            }
            c.SetCursorPosition(curX, curY);
        }
    }
}
