using System;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace cSharpRaylib
{
    public static class LevelTransform
    {
        public static void DrawIsometricBlock(int x, int y, int tileHeight, Color[] palette, float scale, float heightScale, int offsetX, int offsetY)
        {
            // Base anchoring position modified by custom user screen panning offsets
            int originX = 400 + offsetX;
            int originY = 350 + offsetY;

            // Atari Asymmetrical Isometric Projection Transform Math Layer
            // Modifying scale handles overall size, while heightScale stretches or compresses vertical steps
            int screenX = (int)(originX - (x * 12 * scale) + (y * 12 * scale));
            int screenY = (int)(originY + (x * 6 * scale) + (y * 6 * scale) - (tileHeight * heightScale * scale));

            // Scaled structural geometry footprint bounds
            int sizeX = (int)(12 * scale);
            int sizeY = (int)(6 * scale);
            int sizeH = (int)(tileHeight * heightScale * scale);

            Color topColor = palette[0];
            Color frontColor = palette[1];
            Color sideColor = palette[2];

            // 1. Draw Front-Left Face (Vertical Wall Shading Layer)
            Raylib.DrawTriangle(
                new System.Numerics.Vector2(screenX - sizeX, screenY),
                new System.Numerics.Vector2(screenX, screenY + sizeY),
                new System.Numerics.Vector2(screenX, screenY + sizeY + (6 * scale)),
                frontColor);
            Raylib.DrawTriangle(
                new System.Numerics.Vector2(screenX - sizeX, screenY),
                new System.Numerics.Vector2(screenX, screenY + sizeY + (6 * scale)),
                new System.Numerics.Vector2(screenX - sizeX, screenY + (6 * scale)),
                frontColor);

            // 2. Draw Front-Right Face (Vertical Wall Shading Layer)
            Raylib.DrawTriangle(
                new System.Numerics.Vector2(screenX, screenY + sizeY),
                new System.Numerics.Vector2(screenX + sizeX, screenY),
                new System.Numerics.Vector2(screenX + sizeX, screenY + sizeY),
                sideColor);
            Raylib.DrawTriangle(
                new System.Numerics.Vector2(screenX, screenY + sizeY),
                new System.Numerics.Vector2(screenX + sizeX, screenY + sizeY),
                new System.Numerics.Vector2(screenX, screenY + sizeY + (6 * scale)),
                sideColor);

            // 3. Draw Top Walking Face (Diamond Cap Layer)
            Raylib.DrawTriangle(
                new System.Numerics.Vector2(screenX, screenY - sizeY),
                new System.Numerics.Vector2(screenX - sizeX, screenY),
                new System.Numerics.Vector2(screenX, screenY + sizeY),
                topColor);
            Raylib.DrawTriangle(
                new System.Numerics.Vector2(screenX, screenY - sizeY),
                new System.Numerics.Vector2(screenX + sizeX, screenY),
                new System.Numerics.Vector2(screenX, screenY + sizeY),
                topColor);
        }
    }
}