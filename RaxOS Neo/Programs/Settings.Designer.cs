   using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaxOS_BETA.Programs
{
    internal partial class Settings
    {
        static void InitializeComponent()
        {

        }
        public static class DisplaySettings
        {
            public static class ResolutionAndColorSettings
            {
                /// <summary>
                /// 200p: 4/8 ColorDepth (si no: NotSupportedException())
                /// 480p: 4px ColorDepth (si no: NotSupportedException())
                /// 720x480p:16pxColorDepth(sino:NotSupportedException())
                /// </summary>
                public enum Resolution
                {
                    Size320x200,
                    Size640x480,
                    Size720x480
                }
                public enum ColorDepth
                {
                    ColorDepth4=4,
                    ColorDepth8=8,
                    ColorDepth16=16,
                    ColorDepth24=24,
                    ColorDepth32=32
                }
                public static void SetResolutionAndColor(Resolution res, ColorDepth cdf)
                {
                    Cosmos.System.Graphics.ColorDepth cdfEVO = Cosmos.System.Graphics.ColorDepth.ColorDepth4;
                    Cosmos.HAL.Drivers.Video.VGADriver.ScreenSize resEVO = Cosmos.HAL.Drivers.Video.VGADriver.ScreenSize.Size320x200;
                    if (cdf == ColorDepth.ColorDepth4) {
                        cdfEVO = Cosmos.System.Graphics.ColorDepth.ColorDepth4;
                    }
                    if (cdf == ColorDepth.ColorDepth8 )
                    {
                        cdfEVO = Cosmos.System.Graphics.ColorDepth.ColorDepth8;
                    }
                    if (cdf == ColorDepth.ColorDepth16 )
                    {
                        cdfEVO = Cosmos.System.Graphics.ColorDepth.ColorDepth16;
                    }
                    if (cdf == ColorDepth.ColorDepth24 )
                    {
                        cdfEVO = Cosmos.System.Graphics.ColorDepth.ColorDepth24;
                    }
                    if (cdf == ColorDepth.ColorDepth32 )
                    {
                        cdfEVO = Cosmos.System.Graphics.ColorDepth.ColorDepth32;
                    }

                    if (res == Resolution.Size320x200)
                    {
                        resEVO = Cosmos.HAL.Drivers.Video.VGADriver.ScreenSize.Size320x200;/*4/8-bit*/
                    }
                    if (res == Resolution.Size640x480)
                    {
                        resEVO = Cosmos.HAL.Drivers.Video.VGADriver.ScreenSize.Size640x480;/*2/4-bit*/
                    }  
                    if (res == Resolution.Size720x480)
                    {
                        resEVO = Cosmos.HAL.Drivers.Video.VGADriver.ScreenSize.Size720x480;/*16-bit*/
                    }


                    /*if (res == Resolution.Size320x200 && cdf == ColorDepth.ColorDepth4)
                    {
                        Console.WriteLine("Not compatible");
                        throw new NotSupportedException();
                    }*/
                    if (res == Resolution.Size320x200 && cdf == ColorDepth.ColorDepth4)
                    {
                        VGAScreen.SetGraphicsMode(resEVO, cdfEVO);
                    }
                    else if (res == Resolution.Size320x200 && cdf == ColorDepth.ColorDepth8)
                    {
                        VGAScreen.SetGraphicsMode(resEVO, cdfEVO);
                    }
                    else if (res == Resolution.Size320x200 && cdf == ColorDepth.ColorDepth16)
                    {
                        Console.WriteLine("Not compatible");
                        throw new NotSupportedException();
                    }
                    else if (res == Resolution.Size320x200 && cdf == ColorDepth.ColorDepth24)
                    {
                        Console.WriteLine("Not compatible");
                        throw new NotSupportedException();
                    }
                    else if (res == Resolution.Size320x200 && cdf == ColorDepth.ColorDepth32)
                    {
                        Console.WriteLine("Not compatible");
                        throw new NotSupportedException();
                    }
                    else if (res == Resolution.Size640x480 && cdf == ColorDepth.ColorDepth4)
                    {
                        VGAScreen.SetGraphicsMode(resEVO, cdfEVO);
                    }
                    else if (res == Resolution.Size640x480 && cdf == ColorDepth.ColorDepth8)
                    {
                        Console.WriteLine("Not compatible");
                        throw new NotSupportedException();
                    }
                    else if (res == Resolution.Size640x480 && cdf == ColorDepth.ColorDepth16)
                    {
                        Console.WriteLine("Not compatible");
                        throw new NotSupportedException();
                    }
                    else if (res == Resolution.Size640x480 && cdf == ColorDepth.ColorDepth24)
                    {
                        Console.WriteLine("Not compatible");
                        throw new NotSupportedException();
                    }
                    else if (res == Resolution.Size640x480 && cdf == ColorDepth.ColorDepth32)
                    {
                        Console.WriteLine("Not compatible");
                        throw new NotSupportedException();
                    }
                    else if (res == Resolution.Size720x480 && cdf == ColorDepth.ColorDepth4)
                    {
                        Console.WriteLine("Not compatible");
                        throw new NotSupportedException();
                    }
                    else if (res == Resolution.Size720x480 && cdf == ColorDepth.ColorDepth8)
                    {
                        Console.WriteLine("Not compatible");
                        throw new NotSupportedException();
                    }
                    else if (res == Resolution.Size720x480 && cdf == ColorDepth.ColorDepth16)
                    {
                        VGAScreen.SetGraphicsMode(resEVO, cdfEVO);
                    }
                    else if (res == Resolution.Size720x480 && cdf == ColorDepth.ColorDepth24)
                    {
                        Console.WriteLine("Not compatible");
                        throw new NotSupportedException();
                    }
                    else if (res == Resolution.Size720x480 && cdf == ColorDepth.ColorDepth32)
                    {
                        Console.WriteLine("Not compatible");
                        throw new NotSupportedException();
                    }
                }
            }
        }
    }
}
