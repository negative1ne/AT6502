using System;
using System.Collections.Generic;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace cSharpRaylib
{
    public static class ElevatorRenderer
    {
        // Renders the entire collection of elevators safely on top of the 3D map workspace
        public static void Render3DElevators(List<ElevatorData> elevators, Color[] palette,
            float scale, float heightScale, int offsetX, int offsetY,
            int rotationAngle, float tiltFactor, int renderStyle)
        {
            foreach (var elevator in elevators)
            {
                DrawStaticElevator(
                    elevator.HorizontalPosition,
                    elevator.VerticalPosition,
                    elevator.BottomPosition,
                    elevator.TopPosition,
                    palette, scale, heightScale, offsetX, offsetY,
                    rotationAngle, tiltFactor, renderStyle
                );
            }
        }

        // Low-level mathematical isometric projection for a single elevator platform
        private static void DrawStaticElevator(int x, int y, int bottomH, int topH, Color[] palette,
            float scale, float heightScale, int offsetX, int offsetY,
            int rotationAngle, float tiltFactor, int renderStyle)
        {
            int originX = 500 + offsetX;
            int originY = 340 + offsetY;

            double rad = rotationAngle * Math.PI / 180.0;
            double cx = x - 11.0;
            double cy = y - 11.0;

            float rotX = (float)(cx * Math.Cos(rad) - cy * Math.Sin(rad)) + 11f;
            float rotY = (float)(cx * Math.Sin(rad) + cy * Math.Cos(rad)) + 11f;

            int screenX = (int)(originX - (rotX * 12 * scale) + (rotY * 12 * scale));
            int bY = (int)(originY + (rotX * 6 * scale * tiltFactor) + (rotY * 6 * scale * tiltFactor) - (bottomH * heightScale * scale));
            int tY = (int)(originY + (rotX * 6 * scale * tiltFactor) + (rotY * 6 * scale * tiltFactor) - (topH * heightScale * scale));

            int sizeX = (int)(12 * scale);
            int sizeY = (int)(6 * scale * tiltFactor);

            // Draw vertical track rail
            Raylib.DrawLineV(new System.Numerics.Vector2(screenX, bY), new System.Numerics.Vector2(screenX, tY), Color.Red);

            // Calculate distinct platform coloring
            int baseShade = Math.Min(120 + (topH * 12), 255);
            Color platformColor = Color.Gold;
            Color topColor = new Color((byte)(platformColor.R * baseShade / 255), (byte)(platformColor.G * baseShade / 255), (byte)(platformColor.B * baseShade / 255), (byte)255);

            // Draw elevator plate platform diamond
            Raylib.DrawTriangle(new System.Numerics.Vector2(screenX, tY - sizeY), new System.Numerics.Vector2(screenX - sizeX, tY), new System.Numerics.Vector2(screenX, tY + sizeY), topColor);
            Raylib.DrawTriangle(new System.Numerics.Vector2(screenX, tY - sizeY), new System.Numerics.Vector2(screenX + sizeX, tY), new System.Numerics.Vector2(screenX, tY + sizeY), topColor);

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