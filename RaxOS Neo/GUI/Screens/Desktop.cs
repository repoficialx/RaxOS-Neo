using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using RaxOS_Neo.GUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static RaxOS_Neo.GUI.Elements;
using Console = System.Console;
using Sys = Cosmos.System;

namespace RaxOS_Neo.GUI.Screens
{
    internal class Desktop
    {
        static List<Form> openForms = new List<Form>();
        static Canvas canvas;
        public static void InitializeComponent()
        {
            // Inicializa mouse y canvas  
            Sys.MouseManager.ScreenWidth = 800;
            Sys.MouseManager.ScreenHeight = 600;
            Sys.MouseManager.X = 100;
            Sys.MouseManager.Y = 100;
            
            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(800, 600, ColorDepth.ColorDepth32));
            canvas.Clear(Color.Green);
            canvas.Display();
        }
        private static int menuShown = 0;
        static void ShowMenu()
        {
            if (menuShown==0) { return; }
            // Fondo del menú
            int menuSize = 150;
            int menuW = 200;
            int menuH = 300;
            canvas.DrawFilledRectangle(Color.DarkBlue, 0, (int)canvas.Mode.Height-30-menuSize, 150, 150);
        }
        static long lastClickTime = 0;
        static readonly int clickCooldown = 500; // ms
        static void DrawTaskbar()
        {
            // Dibuja la barra de tareas en la parte inferior de la pantalla
            int taskbarHeight = 30;
            canvas.DrawFilledRectangle(Color.DarkGray, 0, (int)canvas.Mode.Height - taskbarHeight, (int)canvas.Mode.Width, taskbarHeight);
            canvas.DrawString("RaxOS NEO", PCScreenFont.Default, Color.White, (int)canvas.Mode.Width-85, (int)canvas.Mode.Height - taskbarHeight - 15);
            // Aquí puedes agregar más elementos a la barra de tareas
            // Ejemplo: un botón de inicio
            // Declaración global o en clase
            

            // En tu método de render o update:
            long currentTimeMs = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

            if (new Elements().DisplayButton("Start", 10, (int)canvas.Mode.Height - taskbarHeight + 5, 60, 20, canvas)
                && (currentTimeMs - lastClickTime) > clickCooldown)
            {
                lastClickTime = currentTimeMs; // Actualiza aquí para el cooldown
                menuShown = (menuShown == 1) ? 0 : 1;
            }


            ShowMenu();
            // Botón de apagado
            if (new Elements().DisplayButton("Shutdown", (int)canvas.Mode.Width - 110, (int)canvas.Mode.Height - taskbarHeight + 5, 100, 20, canvas))
            {
                // Acción al hacer clic en el botón de apagado
                Console.WriteLine("Shutdown button clicked");
                Sys.Power.Shutdown();
            }
        }
        Bitmap StretchBitmap(Bitmap source, uint newWidth, uint newHeight)
        {
            Bitmap stretched = new Bitmap(newWidth, newHeight, source.Depth);

            int srcW = (int)source.Width;
            int srcH = (int)source.Height;

            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    // Mapeo: píxel destino a píxel origen proporcional
                    int srcX = x * srcW / (int)newWidth;
                    int srcY = y * srcH / (int)newHeight;

                    int srcIndex = srcY * srcW + srcX;
                    int dstIndex = y * (int)newWidth + x;

                    stretched.RawData[dstIndex] = source.RawData[srcIndex];
                }
            }

            return stretched;
        }

        Bitmap InvertirRB(Bitmap bmp)
        {
            int w = (int)bmp.Width;
            int h = (int)bmp.Height;
            Bitmap nuevo = new Bitmap((uint)w, (uint)h, ColorDepth.ColorDepth24);

            for (int x = 0; x < w; x++)
            {
                for (int i = 0; i < nuevo.RawData.Length; i++)
                {
                    int pixel = bmp.RawData[i];
                    byte a = (byte)((pixel >> 24) & 0xFF);
                    byte r = (byte)((pixel >> 16) & 0xFF);
                    byte g = (byte)((pixel >> 8) & 0xFF);
                    byte b = (byte)(pixel & 0xFF);
                    
                    // ARGB a BGRA
                    int correctedPixel = (b << 24) | (g << 16) | (r << 8) | a;

                    nuevo.RawData[i] = correctedPixel;
                }

            }
            return nuevo;
        }

        public static void Show()
        {
            InitializeComponent();
            canvas.DrawString("RaxOS NEO Desktop", PCScreenFont.Default, Color.White, 50, 50);
            canvas.DrawString("Welcome to RaxOS NEO!", PCScreenFont.Default, Color.White, 50, 70);
            canvas.DrawString("", PCScreenFont.Default, Color.White, 50, 90);
            canvas.Display(); // <== Necesario aquí  
            Thread.Sleep(1000);
            // Bucle de renderizado del escritorio  
            while (true)
            {
                try
                {
                    
                    canvas.Clear(Color.Green);
                    //canvas.DrawImage(new Desktop().StretchBitmap(new Desktop().InvertirRB(new Bitmap(Resources.Bitmaps.RaxOSLogo)),800, 600),0,0);
                    DrawTaskbar();
                    
                    if (new Elements().DisplayButton("Settings", 10, 10, 90, 20, canvas)) {
                        var settings = new Settings.Settings();
                        settings.IsOpen = true;
                        settings.InitializeComponent();
                        openForms.Add(settings);
                    }
                    ;
                    foreach (var form in openForms)
                    {
                        if (form.IsOpen)
                        {
                            form.Draw(canvas);
                        }
                    }
                    MouseManagement.Display(Sys.MouseManager.X, Sys.MouseManager.Y, canvas);
                    canvas.Display();
                }
                catch (Exception ex)
                {
                    canvas.Disable();
                    Console.WriteLine("Error en renderizado: " + ex.Message);
                }
            }
        }
    }
}
