using Raylib_cs;

using System;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace cSharpRaylib
{
    public static class LevelTransform
    {
        // Core rendering routine handling projection transformations, styling flags, and overlays
        public static void DrawIsometricBlock(int x, int y, int tileHeight, Color[] palette,
            float scale, float heightScale, int offsetX, int offsetY,
            int rotationAngle, float tiltFactor, int renderStyle, bool showPaths, byte cellAttr)
        {
            int originX = 400 + offsetX;
            int originY = 300 + offsetY;

            // 1. ROTATION MATRIX LAYER: Rotate around the center of the 22x22 grid (11, 11)
            double rad = rotationAngle * Math.PI / 180.0;
            double cx = x - 11.0;
            double cy = y - 11.0;

            float rotX = (float)(cx * Math.Cos(rad) - cy * Math.Sin(rad)) + 11f;
            float rotY = (float)(cx * Math.Sin(rad) + cy * Math.Cos(rad)) + 11f;

            // 2. TILT & PROJECTION LAYER: Translate grid points to screen space
            int screenX = (int)(originX - (rotX * 12 * scale) + (rotY * 12 * scale));
            int screenY = (int)(originY + (rotX * 6 * scale * tiltFactor) + (rotY * 6 * scale * tiltFactor) - (tileHeight * heightScale * scale));

            int sizeX = (int)(12 * scale);
            int sizeY = (int)(6 * scale * tiltFactor);

            // 3. COLOR SHADING CALCULATIONS (Ported from 2D view logic)
            int baseShade = Math.Min(100 + (tileHeight * 12), 255);

            // Override coloring completely if path visibility flags are toggled on
            Color baseColor = palette[0];
            if (showPaths)
            {
                if ((cellAttr & 0x20) == 0x20) baseColor = Color.Purple;       // Tunnel Highlight
                else if ((cellAttr & 0x04) == 0x04) baseColor = Color.Green;   // Path Highlight
                else if ((cellAttr & 0x10) == 0x10) baseColor = Color.Yellow;  // Gem Highlight
            }

            Color topColor   = new Color((byte)(palette[0].R * baseShade / 255), (byte)(palette[0].G * baseShade / 255), (byte)(palette[0].B * baseShade / 255), (byte)255);
            Color frontColor = new Color((byte)(palette[1].R * baseShade / 255), (byte)(palette[1].G * baseShade / 255), (byte)(palette[1].B * baseShade / 255), (byte)255);
            Color sideColor  = new Color((byte)(palette[2].R * baseShade / 255), (byte)(palette[2].G * baseShade / 255), (byte)(palette[2].B * baseShade / 255), (byte)255);

            // 4. DRAWING PATH STYLES (0 = Filled, 1 = Cel Shaded, 2 = Wireframe)
            if (renderStyle == 0 || renderStyle == 1) // Filled Faces
            {
                // Front-Left Face
                Raylib.DrawTriangle(new System.Numerics.Vector2(screenX - sizeX, screenY), new System.Numerics.Vector2(screenX, screenY + sizeY), new System.Numerics.Vector2(screenX, screenY + sizeY + (6 * scale)), frontColor);
                Raylib.DrawTriangle(new System.Numerics.Vector2(screenX - sizeX, screenY), new System.Numerics.Vector2(screenX, screenY + sizeY + (6 * scale)), new System.Numerics.Vector2(screenX - sizeX, screenY + (6 * scale)), frontColor);

                // Front-Right Face
                Raylib.DrawTriangle(new System.Numerics.Vector2(screenX, screenY + sizeY), new System.Numerics.Vector2(screenX + sizeX, screenY), new System.Numerics.Vector2(screenX + sizeX, screenY + sizeY), sideColor);
                Raylib.DrawTriangle(new System.Numerics.Vector2(screenX, screenY + sizeY), new System.Numerics.Vector2(screenX + sizeX, screenY + sizeY), new System.Numerics.Vector2(screenX, screenY + sizeY + (6 * scale)), sideColor);

                // Top Face
                Raylib.DrawTriangle(new System.Numerics.Vector2(screenX, screenY - sizeY), new System.Numerics.Vector2(screenX - sizeX, screenY), new System.Numerics.Vector2(screenX, screenY + sizeY), topColor);
                Raylib.DrawTriangle(new System.Numerics.Vector2(screenX, screenY - sizeY), new System.Numerics.Vector2(screenX + sizeX, screenY), new System.Numerics.Vector2(screenX, screenY + sizeY), topColor);
            }

            // Outline logic for Cel Shading and pure Wireframes
            if (renderStyle == 1 || renderStyle == 2)
            {
                Color lineClr = (renderStyle == 1) ? Color.Black : topColor;

                // Draw Diamond Top Cap Borders
                Raylib.DrawLineV(new System.Numerics.Vector2(screenX, screenY - sizeY), new System.Numerics.Vector2(screenX - sizeX, screenY), lineClr);
                Raylib.DrawLineV(new System.Numerics.Vector2(screenX - sizeX, screenY), new System.Numerics.Vector2(screenX, screenY + sizeY), lineClr);
                Raylib.DrawLineV(new System.Numerics.Vector2(screenX, screenY + sizeY), new System.Numerics.Vector2(screenX + sizeX, screenY), lineClr);
                Raylib.DrawLineV(new System.Numerics.Vector2(screenX + sizeX, screenY), new System.Numerics.Vector2(screenX, screenY - sizeY), lineClr);

                // Draw Vertical Drop Edges
                Raylib.DrawLineV(new System.Numerics.Vector2(screenX - sizeX, screenY), new System.Numerics.Vector2(screenX - sizeX, screenY + (6 * scale)), lineClr);
                Raylib.DrawLineV(new System.Numerics.Vector2(screenX, screenY + sizeY), new System.Numerics.Vector2(screenX, screenY + sizeY + (6 * scale)), lineClr);
                Raylib.DrawLineV(new System.Numerics.Vector2(screenX + sizeX, screenY), new System.Numerics.Vector2(screenX + sizeX, screenY + (6 * scale)), lineClr);
            }
        }

        // Draw explicit visual shaft indicators for elevator blocks
        public static void DrawElevatorIndicator(int startX, int startY, int bottomH, int topH, float scale, float heightScale, int offsetX, int offsetY, int rotationAngle, float tiltFactor)
        {
            double rad = rotationAngle * Math.PI / 180.0;
            float rotX = (float)((startX - 11.0) * Math.Cos(rad) - (startY - 11.0) * Math.Sin(rad)) + 11f;
            float rotY = (float)((startX - 11.0) * Math.Sin(rad) + (startY - 11.0) * Math.Cos(rad)) + 11f;

            int screenX = (int)((400 + offsetX) - (rotX * 12 * scale) + (rotY * 12 * scale));
            int bY = (int)((300 + offsetY) + (rotX * 6 * scale * tiltFactor) + (rotY * 6 * scale * tiltFactor) - (bottomH * heightScale * scale));
            int tY = (int)((300 + offsetY) + (rotX * 6 * scale * tiltFactor) + (rotY * 6 * scale * tiltFactor) - (topH * heightScale * scale));

            // Render a glowing neon tracking rail line to represent the elevator system shaft safely
            Raylib.DrawLineV(new System.Numerics.Vector2(screenX, bY), new System.Numerics.Vector2(screenX, tY), Color.Red);
            Raylib.DrawCircle(screenX, tY, (int)(4 * scale), Color.Gold);
        }

        // Captures the high-res viewport buffer and saves it straight to your executable root folder as a JPG
        public static void SaveHardwareScreenshot(RenderTexture2D buffer, int rNum, int w, int h)
        {
            // Pull raw pixel image array from GPU memory space cleanly
            Raylib_cs.Image screenImg = Raylib.LoadImageFromTexture(buffer.Texture);
            Raylib.ImageFlipVertical(ref screenImg); // Correct OpenGL coordinate orientation rule match

            string filename = $"ccView_Stage_{rNum:D2}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            Raylib.ExportImage(screenImg, fullPath);
            Raylib.UnloadImage(screenImg); // De-allocate tracking bitmap allocations instantly from system memory
        }
    }
}