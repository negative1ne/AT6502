using System;
using Raylib_cs;

namespace cSharpRaylib
{
    public static class InputHandler
    {
        public static void HandleKeys(
            ref int currentRoom, ref bool is3DMode, ref float globalScale,
            ref float heightMultiplier, ref int panOffsetX, ref int panOffsetY,
            ref int rotationAngle, ref float tiltFactor, ref int renderStyleMode,
            ref bool displayPathOverlays, RenderTexture2D targetBuffer)
        {
            // Room Selection Transitions
            if (Raylib.IsKeyPressed(KeyboardKey.Right)) { currentRoom = (currentRoom + 1) % 37; }
            if (Raylib.IsKeyPressed(KeyboardKey.Left)) { currentRoom = (currentRoom - 1 + 37) % 37; }

            // Perspective Dimension Toggles
            if (Raylib.IsKeyPressed(KeyboardKey.Up)) is3DMode = true;
            if (Raylib.IsKeyPressed(KeyboardKey.Down)) is3DMode = false;

            // F12 Screenshot Generation
            if (Raylib.IsKeyPressed(KeyboardKey.F12))
            {
                LevelTransform.SaveHardwareScreenshot(targetBuffer, currentRoom, targetBuffer.Texture.Width, targetBuffer.Texture.Height);
            }

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

            // Shared Layout View Toggles
            if (Raylib.IsKeyPressed(KeyboardKey.M)) { renderStyleMode = (renderStyleMode + 1) % 3; }
            if (Raylib.IsKeyPressed(KeyboardKey.P)) { displayPathOverlays = !displayPathOverlays; }

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
            }
        }

        // Draws the semi-transparent left-side configuration overlay box
        public static void DrawControlOverlay(bool is3DMode, int renderStyle, bool pathsOn)
        {
            int rectY = 130;
            // Draw dark background plate overlay container
            Raylib.DrawRectangle(15, rectY, 210, 195, new Color(20, 20, 20, 200));
            Raylib.DrawRectangleLines(15, rectY, 210, 195, Color.DarkGray);

            int startTextY = rectY + 10;
            Raylib.DrawText("CONTROLS QUICK MENU", 25, startTextY, 13, Color.Gold);

            string[] universalMenus = {
                "Left/Right : Change Stage",
                "Up / Down  : Toggle 2D/3D",
                $"P          : Pathways [{(pathsOn ? "ON" : "OFF")}]",
                $"M          : Mode [{renderStyle}]",
                "F12        : Screenshot",
                "R          : Reset View"
            };

            for (int i = 0; i < universalMenus.Length; i++)
            {
                Raylib.DrawText(universalMenus[i], 25, startTextY + 22 + (i * 18), 11, Color.RayWhite);
            }

            // Show extended sub-options box if 3D workspace engine is active
            if (is3DMode)
            {
                Raylib.DrawRectangle(15, rectY + 205, 210, 110, new Color(20, 20, 20, 200));
                Raylib.DrawRectangleLines(15, rectY + 205, 210, 110, Color.DarkGray);

                Raylib.DrawText("3D PARAMETERS", 25, rectY + 212, 12, Color.Gold);
                Raylib.DrawText("A / D   : Rotate Grid", 25, rectY + 232, 11, Color.LightGray);
                Raylib.DrawText("Q / E   : Perspective Tilt", 25, rectY + 250, 11, Color.LightGray);
                Raylib.DrawText("W / S   : Scale Height", 25, rectY + 268, 11, Color.LightGray);
                Raylib.DrawText("I/K/J/L : Pan Camera", 25, rectY + 286, 11, Color.LightGray);
            }
        }
    }
}