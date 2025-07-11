﻿using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RaxOS_Neo.GUI.Screens
{
    public static class BootScreen
    {
        static Canvas canvas;
        public static void Display()
        {
            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(800, 600, ColorDepth.ColorDepth32));
            canvas.Clear(Color.Green);
            canvas.DrawString("Booting RaxOS NEO", PCScreenFont.Default, Color.White, 50, 50);
            canvas.DrawString("Thank you for using RaxOS", PCScreenFont.Default, Color.White, 50, 70);
            canvas.DrawString("Visit repoficialx.xyz/raxos/ for more information", PCScreenFont.Default, Color.White, 50, 90);
            canvas.DrawString("Contribute on github.com/repoficialx/RaxOS_Neo", PCScreenFont.Default, Color.White, 50, 110);
            canvas.Display();
            bootProcess();
            Thread.Sleep(3500);
            canvas.Clear();
            canvas.Disable();
            return;
        }
        public static void bootProcess()
        {
            try { 
            if (!File.Exists(System.IO.Path.Combine(Kernel.getPath(Path.SystemFolder),"\\resol.conf")))
            {
                Directory.CreateDirectory("0:\\RaxOS\\SYSTEM\\");
                File.WriteAllText("0:\\RaxOS\\SYSTEM\\resol.conf", "1");
            }
            if (!File.Exists("0:\\RaxOS\\SYSTEM\\color.conf"))
            {
                Directory.CreateDirectory("0:\\RaxOS\\SYSTEM\\");
                File.WriteAllText("0:\\RaxOS\\SYSTEM\\color.conf", "1");
            }
                return;
            }
            catch
            {
                RaxOS_BETA.ExceptionHelper.Exception exception = new("BOOT_NOT_COMPLETED");
                exception.Source = "BOOTSCREEN";
                exception.Code = 0x101;
                RaxOS_BETA.ExceptionHelper.ExceptionHandler.GraphicalHandler.BSOD_GHandler(exception);
            }
        }
    }
}
