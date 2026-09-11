using System;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace cSharpRaylib
{
    public static class LevelTransform
    {
        // Pure isometric projection formula to calculate screen position bounds from map space coordinates
        public static void GetIsometricCoordinates(int x, int y, int heightValue, out int screenX, out int screenY)
        {
            // Base anchoring location offsets for centering the projection on our 800x600 layout window
            int originX = 400;
            int originY = 220;

            // Classic arcade conversion metrics mirroring original geometry stepping scales
            screenX = originX - (x * 12) + (y * 12);
            screenY = originY + (x * 6) + (y * 6) - (heightValue * 3);
        }
    }
}