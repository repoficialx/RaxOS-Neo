     using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaxOS_BETA.Programs
{
    static partial class Settings
    {
        public static class ResolutionSettings
        {
            public enum Mode
            {
                Px320Bt4,
                Px320Bt8,
                Px640Bt4,
                Px720Bt16
            }
            public static void SetResolution(Mode mode)
            {
                if (mode == Mode.Px320Bt4)
                {
                    DisplaySettings.
                        ResolutionAndColorSettings.
                        SetResolutionAndColor(
                        DisplaySettings.ResolutionAndColorSettings.Resolution.Size320x200,
                        DisplaySettings.ResolutionAndColorSettings.ColorDepth.ColorDepth4);
                }
                if (mode == Mode.Px320Bt8)
                {
                    DisplaySettings.
                        ResolutionAndColorSettings.
                        SetResolutionAndColor(
                        DisplaySettings.ResolutionAndColorSettings.Resolution.Size320x200,
                        DisplaySettings.ResolutionAndColorSettings.ColorDepth.ColorDepth8);
                }
                if (mode == Mode.Px640Bt4)
                {
                    DisplaySettings.
                        ResolutionAndColorSettings.
                        SetResolutionAndColor(
                        DisplaySettings.ResolutionAndColorSettings.Resolution.Size640x480,
                        DisplaySettings.ResolutionAndColorSettings.ColorDepth.ColorDepth4);
                }
                if (mode == Mode.Px720Bt16)
                {
                    DisplaySettings.
                        ResolutionAndColorSettings.
                        SetResolutionAndColor(
                        DisplaySettings.ResolutionAndColorSettings.Resolution.Size720x480,
                        DisplaySettings.ResolutionAndColorSettings.ColorDepth.ColorDepth16);
                }
            }
        }
    }
}
