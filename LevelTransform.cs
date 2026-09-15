// ============================================================================
// LEVELTRANSFORM.CS - CLEANED CORE ISOMETRIC PROJECTIONS
// ============================================================================
using System;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace cSharpRaylib
{
    public static class LevelTransform
    {
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

            int baseShade = Math.Min(100 + (tileHeight * 12), 255);

            if (showPaths)
            {
                if ((cellAttr & 0x20) == 0x20) palette[0] = Color.Purple;
                else if ((cellAttr & 0x04) == 0x04) palette[0] = Color.Green;
                else if ((cellAttr & 0x10) == 0x10) palette[0] = Color.Yellow;
            }

            Color topColor = new Color((byte)(palette[0].R * baseShade / 255), (byte)(palette[0].G * baseShade / 255), (byte)(palette[0].B * baseShade / 255), (byte)255);
            Color frontColor = new Color((byte)(palette[1].R * baseShade * 0.85f / 255), (byte)(palette[1].G * baseShade * 0.85f / 255), (byte)(palette[1].B * baseShade * 0.85f / 255), (byte)255);
            Color sideColor = new Color((byte)(palette[2].R * baseShade * 0.65f / 255), (byte)(palette[2].G * baseShade * 0.65f / 255), (byte)(palette[2].B * baseShade * 0.65f / 255), (byte)255);

            int extensionHeight = (renderStyle == 0) ? (int)(tileHeight * heightScale * scale) : (int)(6 * scale);

            if (renderStyle == 0 || renderStyle == 1)
            {
                Raylib.DrawTriangle(new System.Numerics.Vector2(screenX - sizeX, screenY), new System.Numerics.Vector2(screenX, screenY + sizeY), new System.Numerics.Vector2(screenX, screenY + sizeY + extensionHeight), frontColor);
                Raylib.DrawTriangle(new System.Numerics.Vector2(screenX - sizeX, screenY), new System.Numerics.Vector2(screenX, screenY + sizeY + extensionHeight), new System.Numerics.Vector2(screenX - sizeX, screenY + extensionHeight), frontColor);

                Raylib.DrawTriangle(new System.Numerics.Vector2(screenX, screenY + sizeY), new System.Numerics.Vector2(screenX + sizeX, screenY), new System.Numerics.Vector2(screenX + sizeX, screenY + extensionHeight), sideColor);
                Raylib.DrawTriangle(new System.Numerics.Vector2(screenX, screenY + sizeY), new System.Numerics.Vector2(screenX + sizeX, screenY + extensionHeight), new System.Numerics.Vector2(screenX, screenY + sizeY + extensionHeight), sideColor);

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

        public static void SaveHardwareScreenshot(RenderTexture2D buffer, int rNum, int w, int h)
        {
            Raylib_cs.Image screenImg = Raylib.LoadImageFromTexture(buffer.Texture);
            Raylib.ImageFlipVertical(ref screenImg);

            string filename = $"ccView_Stage_{rNum:D2}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            Raylib.ExportImage(screenImg, fullPath);
            Raylib.UnloadImage(screenImg);
        }

        public static void Draw2DGem(int posX, int cellSize, Color gemColor)
        {
            int centerX = posX + (cellSize / 2);
            int centerY = posX + (cellSize / 2);
            Raylib.DrawCircle(centerX, centerY, 3, gemColor);
        }

        public static void Draw3DGem(int x, int y, int tileHeight, float scale, float heightScale, int offsetX, int offsetY, int rotationAngle, float tiltFactor, Color gemColor)
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

            Raylib.DrawCircle(screenX, screenY - 2, (int)(2.5f * scale), gemColor);
            Raylib.DrawCircleLines(screenX, screenY - 2, (int)(2.5f * scale), Color.White);
        }
    }
}