using System;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace cSharpRaylib
{
    public static class InputHandler
    {
        // Enforced ref signature on displayElevators to keep variable context alive
        public static void HandleKeys(
            ref int currentRoom, ref bool is3DMode, ref float globalScale,
            ref float heightMultiplier, ref int panOffsetX, ref int panOffsetY,
            ref int rotationAngle, ref float tiltFactor, ref int renderStyleMode,
            ref bool displayPathOverlays, ref bool displayGems, ref bool displayElevators, ref bool exportTextFlag)
        {
            // Room Selection Transitions
            if (Raylib.IsKeyPressed(KeyboardKey.Right)) { currentRoom = (currentRoom + 1) % 37; }
            if (Raylib.IsKeyPressed(KeyboardKey.Left)) { currentRoom = (currentRoom - 1 + 37) % 37; }
            // Insert this block right below your Left/Right arrow key checks:

            // GLOBAL ZOOM SCALING (Restored for both 2D and 3D views)
            if (Raylib.IsKeyDown(KeyboardKey.KpAdd)) globalScale += 0.02f;
            if (Raylib.IsKeyDown(KeyboardKey.KpSubtract)) globalScale -= 0.02f;

            // Perspective Dimension Toggles
            if (Raylib.IsKeyPressed(KeyboardKey.Up)) is3DMode = true;
            if (Raylib.IsKeyPressed(KeyboardKey.Down)) is3DMode = false;

            // Shared Layout View Toggles
            if (Raylib.IsKeyPressed(KeyboardKey.M)) { renderStyleMode = (renderStyleMode + 1) % 3; }
            if (Raylib.IsKeyPressed(KeyboardKey.P)) { displayPathOverlays = !displayPathOverlays; }
            if (Raylib.IsKeyPressed(KeyboardKey.G)) { displayGems = !displayGems; }

            // CONNECTING ELEVATOR VISIBILITY TO THE B KEY SAFELY BY REFERENCE
            if (Raylib.IsKeyPressed(KeyboardKey.B)) { displayElevators = !displayElevators; }

            // Global 3D Mode Calibrations
            if (is3DMode)
            {
                if (Raylib.IsKeyDown(KeyboardKey.KpAdd)) globalScale += 0.02f;
                if (Raylib.IsKeyDown(KeyboardKey.KpSubtract)) globalScale -= 0.02f;
                if (Raylib.IsKeyDown(KeyboardKey.W)) heightMultiplier += 0.05f;
                if (Raylib.IsKeyDown(KeyboardKey.S)) heightMultiplier -= 0.05f;

                if (Raylib.IsKeyDown(KeyboardKey.I)) panOffsetY -= 4;
                if (Raylib.IsKeyDown(KeyboardKey.K)) panOffsetY += 4;
                if (Raylib.IsKeyDown(KeyboardKey.J)) panOffsetX -= 4;
                if (Raylib.IsKeyDown(KeyboardKey.L)) panOffsetX += 4;

                // Rotation & Pitch Profiles
                if (Raylib.IsKeyDown(KeyboardKey.A)) rotationAngle = (rotationAngle - 2 + 360) % 360;
                if (Raylib.IsKeyDown(KeyboardKey.D)) rotationAngle = (rotationAngle + 2) % 360;
                if (Raylib.IsKeyDown(KeyboardKey.Q)) tiltFactor = Math.Max(0.4f, tiltFactor - 0.02f);
                if (Raylib.IsKeyDown(KeyboardKey.E)) tiltFactor = Math.Min(2.0f, tiltFactor + 0.02f);
            }

            if (!is3DMode)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.T)) exportTextFlag = true;
            }

            // Master Reset Sequence
            if (Raylib.IsKeyPressed(KeyboardKey.R))
            {
                globalScale = 1.0f;
                heightMultiplier = 1.8f;
                panOffsetX = 0;
                panOffsetY = 0;
                rotationAngle = 0;
                tiltFactor = 1.0f;
                renderStyleMode = 0;
                displayPathOverlays = false;
                displayGems = true;
                displayElevators = true;
            }
        }

        public static void DrawControlOverlay(bool is3DMode, int renderStyle, bool pathsOn, bool gemsOn, bool elevatorsOn,
            int currentRoom, string stageName, int totalElevators, Color[] activeTheme)
        {
            // 1. RENDER CLEAN HUD STRINGS HIGH ABOVE MAP BLOCKS
            Raylib.DrawText("ccSharpRaylib", 20, 20, 20, Color.RayWhite);

            int displayLevel = (currentRoom / 4) + 1;
            int displayWave = (currentRoom % 4) + 1;
            string viewModeLabel = is3DMode ? "3D Isometric" : "2D Flat";

            // Crisp un-conflicted sub-header string row tracking your dynamic data counts
            Raylib.DrawText($"Level {displayLevel} - {displayWave} [{stageName}] | Lifts: {totalElevators} | {viewModeLabel}", 20, 55, 18, Color.Gold);

            // Palette slot boxes layout paths
            Raylib.DrawRectangle(20, 95, 40, 20, activeTheme[0]);
            Raylib.DrawRectangle(70, 95, 40, 20, activeTheme[1]);
            Raylib.DrawRectangle(120, 95, 40, 20, activeTheme[2]);
            Raylib.DrawText("Active Layout Palette Matrix Slots", 180, 98, 14, Color.LightGray);

            // 2. RENDER THE CONTROLS CONTAINER MENU CARD PANEL
            int rectY = 130;
            int cardHeight = is3DMode ? 233 : 210;
            Raylib.DrawRectangle(15, rectY, 210, cardHeight, new Color(20, 20, 20, 200));
            Raylib.DrawRectangleLines(15, rectY, 210, cardHeight, Color.DarkGray);

            int startTextY = rectY + 10;
            Raylib.DrawText("CONTROLS QUICK MENU", 25, startTextY, 13, Color.Gold);

            Raylib.DrawText("Left/Right : Change Stage", 25, startTextY + 22, 11, Color.RayWhite);
            Raylib.DrawText("Up / Down  : Toggle 2D/3D", 25, startTextY + 40, 11, Color.RayWhite);
            Raylib.DrawText($"P          : Pathways [{(pathsOn ? "ON" : "OFF")}]", 25, startTextY + 58, 11, Color.RayWhite);
            Raylib.DrawText($"G          : Gems     [{(gemsOn ? "ON" : "OFF")}]", 25, startTextY + 76, 11, Color.RayWhite);
            Raylib.DrawText($"B          : Lifts    [{(elevatorsOn ? "ON" : "OFF")}]", 25, startTextY + 96, 11, Color.Yellow);

            if (!is3DMode)
            {
                Raylib.DrawText("T          : Export Matrix Text", 25, startTextY + 114, 11, Color.SkyBlue);
                Raylib.DrawText("R          : Reset View", 25, startTextY + 132, 11, Color.RayWhite);
            }
            else
            {
                string styleName = renderStyle == 0 ? "Original Filled" : (renderStyle == 1 ? "Cel Shaded" : "Wireframe");
                Raylib.DrawText($"M          : Style [{styleName}]", 25, startTextY + 114, 11, Color.Orange);
                Raylib.DrawText("R          : Reset View", 25, startTextY + 132, 11, Color.RayWhite);

                Raylib.DrawRectangle(15, rectY + 245, 210, 110, new Color(20, 20, 20, 200));
                Raylib.DrawRectangleLines(15, rectY + 245, 210, 110, Color.DarkGray);

                Raylib.DrawText("3D PARAMETERS", 25, rectY + 252, 12, Color.Gold);
                Raylib.DrawText("A / D   : Rotate Grid", 25, rectY + 272, 11, Color.LightGray);
                Raylib.DrawText("Q / E   : Perspective Tilt", 25, rectY + 290, 11, Color.LightGray);
                Raylib.DrawText("W / S   : Scale Height", 25, rectY + 308, 11, Color.LightGray);
                Raylib.DrawText("I/K/J/L : Pan Camera", 25, rectY + 326, 11, Color.LightGray);
            }
        }
    
    }
}