// ============================================================================
// ELEVATORRENDERER.CS - RATIO-NORMALIZED HEIGHT PROJECTION ENGINE (v0.80 FIXED)
// ============================================================================
using System;
using System.Collections.Generic;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace cSharpRaylib
{
    public static class ElevatorRenderer
    {
        public static void Render3DElevators(List<ElevatorData> elevators, Color[] palette,
            float scale, float heightScale, int offsetX, int offsetY,
            int rotationAngle, float tiltFactor, int renderStyle)
        {
            if (elevators == null) return;

            foreach (var elevator in elevators)
            {
                if (elevator.IsMapped)
                {
                    DrawStaticElevator(
                        elevator.CellX, elevator.CellY,
                        elevator.BottomPosition, elevator.TopPosition, elevator.CurrentPosition,
                        palette, scale, heightScale, offsetX, offsetY,
                        rotationAngle, tiltFactor, renderStyle
                    );
                }
            }
        } // <--- Ensure Render3DElevators method closes cleanly right here

        private static void DrawStaticElevator(int cellX, int cellY, int bottomH, int topH, int currentPos, Color[] palette,
            float scale, float heightScale, int offsetX, int offsetY,
            int rotationAngle, float tiltFactor, int renderStyle)
        {
            int originX = 500 + offsetX;
            int originY = 340 + offsetY;

            double rad = rotationAngle * Math.PI / 180.0;
            double cx = cellX - 11.0;
            double cy = cellY - 11.0;

            float rotX = (float)(cx * Math.Cos(rad) - cy * Math.Sin(rad)) + 11f;
            float rotY = (float)(cx * Math.Sin(rad) + cy * Math.Cos(rad)) + 11f;

            int screenX = (int)(originX - (rotX * 12 * scale) + (rotY * 12 * scale));

            // CALCULATE THE INTERACTIVE TRAVEL RATIO PERCENTAGE (Safe from Division-by-Zero)
            float totalTravelRange = topH - bottomH;
            float currentProgressDistance = currentPos - bottomH;
            float travelRatio = (totalTravelRange > 0) ? (currentProgressDistance / totalTravelRange) : 0.0f;

            // v0.80 HEIGHT MATCH: Track real-time position increment ticks smoothly
            float visualBaseHeight = bottomH;
            float visualLiveHeight = currentPos;

            // Calculate baseline screen position using your true landscape deck scale factors
            int terrainBaseY = (int)(originY + (rotX * 6 * scale * tiltFactor) + (rotY * 6 * scale * tiltFactor));

            // Extrude the screen tracking coordinates using a standardized altitude ratio modifier
            int bY = (int)(terrainBaseY - (visualBaseHeight * heightScale * scale * 0.1f));
            int tY = (int)(terrainBaseY - (visualLiveHeight * heightScale * scale * 0.1f));

            int sizeX = (int)(12 * scale);
            int sizeY = (int)(6 * scale * tiltFactor);

            // Render vertical tracking elevator rail line shaft
            Raylib.DrawLineV(new System.Numerics.Vector2(screenX, bY), new System.Numerics.Vector2(screenX, tY), Color.Red);

            int baseShade = Math.Min(120 + (topH * 12), 255);
            Color platformColor = Color.Orange;
            Color topColor = new Color((byte)(platformColor.R * baseShade / 255), (byte)(platformColor.G * baseShade / 255), (byte)(platformColor.B * baseShade / 255), (byte)255);

            // Draw elevator plate platform diamond shell faces
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