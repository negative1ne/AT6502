using System;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace cSharpRaylib
{
    public static class InputHandler
    {
        public static void HandleKeys(
            ref int currentRoom, ref bool is3DMode, ref float globalScale,
            ref float heightMultiplier, ref int panOffsetX, ref int panOffsetY,
            ref int rotationAngle, ref float tiltFactor, ref int renderStyleMode,
            ref bool displayPathOverlays, ref bool displayGems, ref bool displayElevators, ref bool exportTextFlag)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Right)) { currentRoom = (currentRoom + 1) % 37; }
            if (Raylib.IsKeyPressed(KeyboardKey.Left)) { currentRoom = (currentRoom - 1 + 37) % 37; }

            if (Raylib.IsKeyPressed(KeyboardKey.Up)) is3DMode = true;
            if (Raylib.IsKeyPressed(KeyboardKey.Down)) is3DMode = false;

            if (Raylib.IsKeyPressed(KeyboardKey.G)) displayGems = !displayGems;
            if (Raylib.IsKeyPressed(KeyboardKey.P)) displayPathOverlays = !displayPathOverlays;
            if (Raylib.IsKeyPressed(KeyboardKey.B)) displayElevators = !displayElevators; // FIXED: Changed from L to B to clear pan conflict

            if (!is3DMode)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.T)) exportTextFlag = true;
            }
            else
            {
                if (Raylib.IsKeyPressed(KeyboardKey.M)) { renderStyleMode = (renderStyleMode + 1) % 3; }

                if (Raylib.IsKeyDown(KeyboardKey.KpAdd)) globalScale += 0.02f;
                if (Raylib.IsKeyDown(KeyboardKey.KpSubtract)) globalScale -= 0.02f;
                if (Raylib.IsKeyDown(KeyboardKey.W)) heightMultiplier += 0.05f;
                if (Raylib.IsKeyDown(KeyboardKey.S)) heightMultiplier -= 0.05f;

                if (Raylib.IsKeyDown(KeyboardKey.I)) panOffsetY -= 4;
                if (Raylib.IsKeyDown(KeyboardKey.K)) panOffsetY += 4;
                if (Raylib.IsKeyDown(KeyboardKey.J)) panOffsetX -= 4;
                if (Raylib.IsKeyDown(KeyboardKey.L)) panOffsetX += 4;

                if (Raylib.IsKeyDown(KeyboardKey.A)) rotationAngle = (rotationAngle - 2 + 360) % 360;
                if (Raylib.IsKeyDown(KeyboardKey.D)) rotationAngle = (rotationAngle + 2) % 360;
                if (Raylib.IsKeyDown(KeyboardKey.Q)) tiltFactor = Math.Max(0.4f, tiltFactor - 0.02f);
                if (Raylib.IsKeyDown(KeyboardKey.E)) tiltFactor = Math.Min(2.0f, tiltFactor + 0.02f);
            }

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

        public static void DrawControlOverlay(bool is3DMode, int renderStyle, bool pathsOn, bool gemsOn, bool elevatorsOn)
        {
            int rectY = 130;
            int rectHeight = is3DMode ? 233 : 195;
            Raylib.DrawRectangle(15, rectY, 210, rectHeight, new Color(20, 20, 20, 200));
            Raylib.DrawRectangleLines(15, rectY, 210, rectHeight, Color.DarkGray);

            int startTextY = rectY + 10;
            Raylib.DrawText("CONTROLS QUICK MENU", 25, startTextY, 13, Color.Gold);

            Raylib.DrawText("Left/Right : Change Stage", 25, startTextY + 22, 11, Color.RayWhite);
            Raylib.DrawText("Up / Down  : Toggle 2D/3D", 25, startTextY + 40, 11, Color.RayWhite);
            Raylib.DrawText($"P          : Pathways [{(pathsOn ? "ON" : "OFF")}]", 25, startTextY + 58, 11, Color.RayWhite);
            Raylib.DrawText($"G          : Gems     [{(gemsOn ? "ON" : "OFF")}]", 25, startTextY + 76, 11, Color.RayWhite);

            if (!is3DMode)
            {
                Raylib.DrawText("T          : Export Matrix Text", 25, startTextY + 94, 11, Color.SkyBlue);
                Raylib.DrawText("R          : Reset View", 25, startTextY + 112, 11, Color.RayWhite);
            }
            else
            {
                string styleName = renderStyle == 0 ? "Original Filled" : (renderStyle == 1 ? "Cel Shaded" : "Wireframe");
                Raylib.DrawText($"B          : Elevators [{(elevatorsOn ? "ON" : "OFF")}]", 25, startTextY + 94, 11, Color.Yellow); // Updated text label
                Raylib.DrawText($"M          : Style [{styleName}]", 25, startTextY + 112, 11, Color.Orange);
                Raylib.DrawText("R          : Reset View", 25, startTextY + 130, 11, Color.RayWhite);

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