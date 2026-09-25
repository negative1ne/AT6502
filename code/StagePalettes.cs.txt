using System;
using System.Collections.Generic;
using System.IO;
using Raylib_cs;
// Add this exact line below to explicitly enforce Raylib's Color system
using Color = Raylib_cs.Color;
using Rectangle = Raylib_cs.Rectangle;

namespace cSharpRaylib
{
    public static class StagePalettes
    {
        public static Dictionary<int, Color[]> GetMasterPaletteMatrix()
        {
            return new Dictionary<int, Color[]>()
            {
                { 0, new Color[] { Color.White, Color.Gray, Color.DarkGray } },
                { 1, new Color[] { Color.SkyBlue, Color.Pink, Color.Maroon } },
                { 2, new Color[] { Color.RayWhite, Color.Blue, Color.DarkBlue } },
                { 3, new Color[] { Color.Violet, Color.Purple, Color.DarkPurple } },
                { 4, new Color[] { Color.LightGray, new Color(112, 128, 144, 255), Color.DarkBlue } },
                { 5, new Color[] { Color.Magenta, Color.Purple, Color.Black } },
                { 6, new Color[] { Color.Beige, Color.Brown, Color.DarkBrown } },
                { 7, new Color[] { Color.Yellow, Color.Orange, Color.Red } },
                { 8, new Color[] { Color.LightGray, new Color(112, 128, 144, 255), Color.DarkBlue } },
                { 9, new Color[] { Color.Lime, new Color(34, 139, 34, 255), Color.DarkGreen } },
                { 10, new Color[] { Color.White, Color.SkyBlue, Color.Blue } },
                { 11, new Color[] { Color.Purple, Color.DarkPurple, Color.Magenta } },
                { 12, new Color[] { Color.Gold, Color.Orange, Color.DarkBrown } },
                { 13, new Color[] { Color.SkyBlue, Color.Blue, Color.DarkBlue } },
                { 14, new Color[] { Color.LightGray, new Color(112, 128, 144, 255), Color.DarkBlue } },
                { 15, new Color[] { Color.DarkGray, Color.Maroon, Color.Black } },
                { 16, new Color[] { Color.Magenta, Color.Purple, Color.Black } },
                { 17, new Color[] { Color.Lime, new Color(34, 139, 34, 255), Color.DarkGreen } },
                { 18, new Color[] { Color.Beige, Color.Brown, Color.DarkBrown } },
                { 19, new Color[] { Color.Violet, Color.Purple, Color.DarkPurple } },
                { 20, new Color[] { Color.SkyBlue, Color.Blue, Color.DarkBlue } },
                { 21, new Color[] { Color.SkyBlue, Color.Pink, Color.Maroon } },
                { 22, new Color[] { Color.SkyBlue, Color.Pink, Color.Maroon } },
                { 23, new Color[] { Color.DarkGray, Color.Maroon, Color.Black } },
                { 24, new Color[] { Color.Magenta, Color.Purple, Color.Black } },
                { 25, new Color[] { Color.Gold, Color.Orange, Color.DarkBrown } },
                { 26, new Color[] { Color.White, Color.SkyBlue, Color.Blue } },
                { 27, new Color[] { Color.Purple, Color.DarkPurple, Color.Magenta } },
                { 28, new Color[] { Color.Magenta, Color.Purple, Color.Black } },
                { 29, new Color[] { Color.SkyBlue, Color.Blue, Color.DarkBlue } },
                { 30, new Color[] { Color.LightGray, new Color(112, 128, 144, 255), Color.DarkBlue } },
                { 31, new Color[] { Color.Yellow, Color.Orange, Color.Red } },
                { 32, new Color[] { Color.Red, new Color(139, 0, 0, 255), Color.Black } },
                { 33, new Color[] { Color.Lime, new Color(34, 139, 34, 255), Color.DarkGreen } },
                { 34, new Color[] { Color.White, Color.SkyBlue, Color.Blue } },
                { 35, new Color[] { Color.Purple, Color.DarkPurple, Color.Magenta } },
                { 36, new Color[] { Color.RayWhite, Color.Gold, new Color(204, 204, 0, 255) } }
            };
        }
    }
}