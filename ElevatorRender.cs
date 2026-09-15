// ============================================================================
// ELEVATORRENDERER.CS - CORE 3D ISOMETRIC PARITY MIGRATION (v0.5)
// ============================================================================
using System;
using System.Collections.Generic;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace cSharpRaylib
{
    public static class ElevatorRenderer
    {
        /// <summary>
        /// Renders the entire collection of elevators safely on top of the 3D map workspace.
        /// Points exclusively to your verified, isolated 37-stage memory container blocks.
        /// </summary>
        public static void Render3DElevators(List<ElevatorData> elevators, Color[] palette,
            float scale, float heightScale, int offsetX, int offsetY,
            int rotationAngle, float tiltFactor, int renderStyle)
        {
            if (elevators == null) return;

            foreach (var elevator in elevators)
            {
                if (elevator.IsMapped)
                {
                    // FIX: Pass the true isolated CellX and CellY grid cells into the drawing 
                    // processor instead of the raw unmapped ROM screen bytes to secure 1-1 parity!
                    DrawStaticElevator(
                        elevator.CellX,
                        elevator.CellY,
                        elevator.BottomPosition,
                        elevator.TopPosition,
                        elevator.CurrentPosition, // Pass your live travel offset parameters
                        palette, scale, heightScale, offsetX, offsetY,
                        rotationAngle, tiltFactor, renderStyle
                    );
                }
            }
        }

        private static void DrawStaticElevator(int cellX, int cellY, int bottomH, int topH, int currentPos, Color[] palette,
            float scale, float heightScale, int offsetX, int offsetY,
            int rotationAngle, float tiltFactor, int renderStyle)
        {
            int originX = 500 + offsetX;
            int originY = 340 + offsetY;

            double rad = rotationAngle * Math.PI / 180.0;

            // Map dimensions centered around your master 11-step geometric pivots
            double cx = cellY - 11.0;
            double cy = cellX - 11.0;

            float rotX = (float)(cx * Math.Cos(rad) - cy * Math.Sin(rad));
            float rotY = (float)(cx * Math.Sin(rad) + cy * Math.Cos(rad)) * tiltFactor;

            // Generate screen-space projected coordinate steps matching your landscape grid tiles
            int screenX = (int)(originX - (rotX * 12 * scale) + (rotY * 12 * scale));
            int bY = (int)(originY + (rotX * 6 * scale * tiltFactor) + (rotY * 6 * scale * tiltFactor) - (bottomH * heightScale * scale));

            // Unified live vertical translation calculation tracking your state-machine height vectors
            int liveVerticalValue = (int)((bottomH + currentPos) * heightScale * scale);
            int tY = (int)(originY + (rotX * 6 * scale * tiltFactor) + (rotY * 6 * scale * tiltFactor) - liveVerticalValue);

            int sizeX = (int)(12 * scale);
            int sizeY = (int)(6 * scale * tiltFactor);

            // Render vertical tracking elevator rail line shaft
            Raylib.DrawLineV(new System.Numerics.Vector2(screenX, bY), new System.Numerics.Vector2(screenX, tY), Color.Red);

            // Calculate distinct platform coloring parameters matching your active theme styles
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