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
            int originX = 500 + offsetX;
            int originY = 340 + offsetY;

            double rad = rotationAngle * Math.PI / 180.0;
            double cx = x - 11.0;
            double cy = y - 11.0;

            float rotX = (float)(cx * Math.Cos(rad) - cy * Math.Sin(rad)) + 11f;
            float rotY = (float)(cx * Math.Sin(rad) + cy * Math.Cos(rad)) + 11f;

            int screenX = (int)(originX - (rotX * 12 * scale) + (rotY * 12 * scale));
            int screenY = (int)(originY + (rotX * 6 * scale * tiltFactor) + (rotY * 6 * scale * tiltFactor) - (tileHeight * heightScale * scale));

            int sizeX = (int)(12 * scale);
            int sizeY = (int)(6 * scale * tiltFactor);

            // Inside LevelTransform.cs -> DrawIsometricBlock function
            // Replace your color calculation and polygon drawing block with this:

            // 3. 3D SHADING LAYER (Applies distinct brightness multipliers to separate faces)
            int baseShade = Math.Min(100 + (tileHeight * 12), 255);

            if (showPaths)
            {
                if ((cellAttr & 0x20) == 0x20) palette[0] = Color.Purple;
                else if ((cellAttr & 0x04) == 0x04) palette[0] = Color.Green;
                else if ((cellAttr & 0x10) == 0x10) palette[0] = Color.Yellow;
            }

            // Top cap face: Main base tone
            Color topColor = new Color((byte)(palette[0].R * baseShade / 255), (byte)(palette[0].G * baseShade / 255), (byte)(palette[0].B * baseShade / 255), (byte)255);

            // Front-Left Face: Multiplied down slightly (85% shading)
            Color frontColor = new Color((byte)(palette[1].R * baseShade * 0.85f / 255), (byte)(palette[1].G * baseShade * 0.85f / 255), (byte)(palette[1].B * baseShade * 0.85f / 255), (byte)255);

            // Front-Right Face: Multiplied down deeply (65% shading) to give a powerful 3D depth pop effect
            Color sideColor = new Color((byte)(palette[2].R * baseShade * 0.65f / 255), (byte)(palette[2].G * baseShade * 0.65f / 255), (byte)(palette[2].B * baseShade * 0.65f / 255), (byte)255);

            // Determine vertical extrusion extension depth parameters based on styling profiles selected
            // Style 0 = Filled solid walls down to baseline grid plane coordinates
            int extensionHeight = (renderStyle == 0) ? (int)(tileHeight * heightScale * scale) : (int)(6 * scale);

            if (renderStyle == 0 || renderStyle == 1)
            {
                // Front-Left Face Extrusion Polygons
                Raylib.DrawTriangle(new System.Numerics.Vector2(screenX - sizeX, screenY), new System.Numerics.Vector2(screenX, screenY + sizeY), new System.Numerics.Vector2(screenX, screenY + sizeY + extensionHeight), frontColor);
                Raylib.DrawTriangle(new System.Numerics.Vector2(screenX - sizeX, screenY), new System.Numerics.Vector2(screenX, screenY + sizeY + extensionHeight), new System.Numerics.Vector2(screenX - sizeX, screenY + extensionHeight), frontColor);

                // Front-Right Face Extrusion Polygons
                Raylib.DrawTriangle(new System.Numerics.Vector2(screenX, screenY + sizeY), new System.Numerics.Vector2(screenX + sizeX, screenY), new System.Numerics.Vector2(screenX + sizeX, screenY + extensionHeight), sideColor);
                Raylib.DrawTriangle(new System.Numerics.Vector2(screenX, screenY + sizeY), new System.Numerics.Vector2(screenX + sizeX, screenY + extensionHeight), new System.Numerics.Vector2(screenX, screenY + sizeY + extensionHeight), sideColor);

                // Top Horizontal Diamond Face Cap 
                Raylib.DrawTriangle(new System.Numerics.Vector2(screenX, screenY - sizeY), new System.Numerics.Vector2(screenX - sizeX, screenY), new System.Numerics.Vector2(screenX, screenY + sizeY), topColor);
                Raylib.DrawTriangle(new System.Numerics.Vector2(screenX, screenY - sizeY), new System.Numerics.Vector2(screenX + sizeX, screenY), new System.Numerics.Vector2(screenX, screenY + sizeY), topColor);
            }

            if (renderStyle == 1 || renderStyle == 2)
            {
                Color lineClr = (renderStyle == 1) ? Color.Black : topColor;

                Raylib.DrawLineV(new System.Numerics.Vector2(screenX, screenY - sizeY), new System.Numerics.Vector2(screenX - sizeX, screenY), lineClr);
                Raylib.DrawLineV(new System.Numerics.Vector2(screenX - sizeX, screenY), new System.Numerics.Vector2(screenX, screenY + sizeY), lineClr);
                Raylib.DrawLineV(new System.Numerics.Vector2(screenX, screenY + sizeY), new System.Numerics.Vector2(screenX + sizeX, screenY), lineClr);
                Raylib.DrawLineV(new System.Numerics.Vector2(screenX + sizeX, screenY), new System.Numerics.Vector2(screenX, screenY - sizeY), lineClr);

                Raylib.DrawLineV(new System.Numerics.Vector2(screenX - sizeX, screenY), new System.Numerics.Vector2(screenX - sizeX, screenY + extensionHeight), lineClr);
                Raylib.DrawLineV(new System.Numerics.Vector2(screenX, screenY + sizeY), new System.Numerics.Vector2(screenX, screenY + sizeY + extensionHeight), lineClr);
                Raylib.DrawLineV(new System.Numerics.Vector2(screenX + sizeX, screenY), new System.Numerics.Vector2(screenX + sizeX, screenY + extensionHeight), lineClr);
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

        // Captures the high-res viewport buffer and saves it straight to your executable root folder as a PNG
        public static void SaveHardwareScreenshot(RenderTexture2D buffer, int rNum, int w, int h)
        {
            // Explicitly enforce Raylib namespace matching to avoid assembly conflicts
            Raylib_cs.Image screenImg = Raylib.LoadImageFromTexture(buffer.Texture);
            Raylib.ImageFlipVertical(ref screenImg);

            string filename = $"ccView_Stage_{rNum:D2}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            Raylib.ExportImage(screenImg, fullPath);
            Raylib.UnloadImage(screenImg);
        }
        // Renders a flat 2D gem dot exactly at the center of a grid tile coordinate
        public static void Draw2DGem(int posX, int cellSize, Color gemColor)
        {
            int centerX = posX + (cellSize / 2);
            int centerY = posX + (cellSize / 2); // Calculated safely relative to cells
            Raylib.DrawCircle(centerX, centerY, 3, gemColor);
        }

        // Calculates the precise 3D floating screen projection coordinates to render a gemstone indicator
        public static void Draw3DGem(int x, int y, int tileHeight, float scale, float heightScale, int offsetX, int offsetY, int rotationAngle, float tiltFactor, Color gemColor)
        {
            int originX = 500 + offsetX;
            int originY = 340 + offsetY;

            double rad = rotationAngle * Math.PI / 180.0;
            double cx = x - 11.0;
            double cy = y - 11.0;

            float rotX = (float)(cx * Math.Cos(rad) - cy * Math.Sin(rad)) + 11f;
            float rotY = (float)(cx * Math.Sin(rad) + cy * Math.Cos(rad)) + 11f;

            // Projected coordinate math matching top face centers
            int screenX = (int)(originX - (rotX * 12 * scale) + (rotY * 12 * scale));
            int screenY = (int)(originY + (rotX * 6 * scale * tiltFactor) + (rotY * 6 * scale * tiltFactor) - (tileHeight * heightScale * scale));

            // Render gem dot hovering slightly above the block top cap face
            Raylib.DrawCircle(screenX, screenY - 2, (int)(2.5f * scale), gemColor);
            Raylib.DrawCircleLines(screenX, screenY - 2, (int)(2.5f * scale), Color.White);
        }
        // Renders a static elevator shaft tracking rail and its platform cap bound to level projection math
        public static void DrawStaticElevator(int x, int y, int bottomH, int topH, Color[] palette,
            float scale, float heightScale, int offsetX, int offsetY,
            int rotationAngle, float tiltFactor, int renderStyle)
        {
            int originX = 500 + offsetX;
            int originY = 340 + offsetY;

            // 1. ROTATION MATRIX LAYER: Match the exact core map rotation transformation sequence
            double rad = rotationAngle * Math.PI / 180.0;
            double cx = x - 11.0;
            double cy = y - 11.0;

            float rotX = (float)(cx * Math.Cos(rad) - cy * Math.Sin(rad)) + 11f;
            float rotY = (float)(cx * Math.Sin(rad) + cy * Math.Cos(rad)) + 11f;

            // Calculate screen base coordinates for both the bottom and top bounds
            int screenX = (int)(originX - (rotX * 12 * scale) + (rotY * 12 * scale));
            int bY = (int)(originY + (rotX * 6 * scale * tiltFactor) + (rotY * 6 * scale * tiltFactor) - (bottomH * heightScale * scale));
            int tY = (int)(originY + (rotX * 6 * scale * tiltFactor) + (rotY * 6 * scale * tiltFactor) - (topH * heightScale * scale));

            int sizeX = (int)(12 * scale);
            int sizeY = (int)(6 * scale * tiltFactor);

            // 2. RENDER THE VERTICAL SHAFT RAIL (Glowing neon track representation)
            Raylib.DrawLineV(new System.Numerics.Vector2(screenX, bY), new System.Numerics.Vector2(screenX, tY), Color.Red);

            // 3. RENDER THE PLATFORM BLOCK (Sitting stationary at the Top Height Boundary)
            int baseShade = Math.Min(120 + (topH * 12), 255);
            Color platformColor = Color.Gold; // Distinctive styling color signature for elevator platforms
            Color topColor = new Color((byte)(platformColor.R * baseShade / 255), (byte)(platformColor.G * baseShade / 255), (byte)(platformColor.B * baseShade / 255), (byte)255);

            // Top horizontal cap diamond of the elevator lift plate
            Raylib.DrawTriangle(new System.Numerics.Vector2(screenX, tY - sizeY), new System.Numerics.Vector2(screenX - sizeX, tY), new System.Numerics.Vector2(screenX, tY + sizeY), topColor);
            Raylib.DrawTriangle(new System.Numerics.Vector2(screenX, tY - sizeY), new System.Numerics.Vector2(screenX + sizeX, tY), new System.Numerics.Vector2(screenX, tY + sizeY), topColor);

            // Draw dark border trim guidelines if wireframe or cel styles are active
            if (renderStyle > 0)
            {
                Color lineClr = (renderStyle == 1) ? Color.Black : Color.White;
                Raylib.DrawLineV(new System.Numerics.Vector2(screenX, tY - sizeY), new System.Numerics.Vector2(screenX - sizeX, tY), lineClr);
                Raylib.DrawLineV(new System.Numerics.Vector2(screenX - sizeX, tY), new System.Numerics.Vector2(screenX, tY + sizeY), lineClr);
                Raylib.DrawLineV(new System.Numerics.Vector2(screenX, tY + sizeY), new System.Numerics.Vector2(screenX + sizeX, tY), lineClr);
                Raylib.DrawLineV(new System.Numerics.Vector2(screenX + sizeX, tY), new System.Numerics.Vector2(screenX, tY - sizeY), lineClr);
            }
        }
    }
    }