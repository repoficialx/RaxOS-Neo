using Cosmos.System.Graphics;
using static RaxOS_Neo.GUI.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaxOS_Neo.GUI.Screens.Settings
{
    public class Settings : Form
    {
        
        public override void InitializeComponent()
        {
            // Inicializa controles, posiciones, colores, etc.
            IsOpen = true;
        }

        public override void Draw(Canvas canvas)
        {
            // Dibujar la base de la ventana de configuración
            int formW = 200;
            int formH = 300;
            int formX = 75;
            int formY = 75;
            canvas.DrawFilledRectangle(System.Drawing.Color.LightGray, formX, formY, formW, formH);
            int buttonW=50;
            int buttonH=20;
            int buttonX=formW+formX-buttonW;
            int buttonY=formY;
            if (new Elements().DisplayButton("X", buttonX, buttonY, buttonW, buttonH, canvas))
            {
                IsOpen = false;
                return;
            }
        }
    }

}
