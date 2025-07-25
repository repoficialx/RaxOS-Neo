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
            canvas.DrawFilledRectangle(System.Drawing.Color.LightGray, 75, 75, 200, 300);
        }
    }

}
