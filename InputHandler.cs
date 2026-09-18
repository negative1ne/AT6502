// ============================================================================
// INPUTHANDLER.CS - INTEGRATED DUAL-WINDOW ENGINE ROUTER (SANDBOX EXTENSION)
// ============================================================================
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
            ref bool displayPathOverlays, ref bool displayGems, ref bool showDanLegacyOverlay,
            ref bool invertBackground, ref bool exportTextFlag, ref bool trigger3DLabFlag)
        {
            // Room Selection Transitions
            if (Raylib.IsKeyPressed(KeyboardKey.Right)) { currentRoom = (currentRoom + 1) % 37; }
            if (Raylib.IsKeyPressed(KeyboardKey.Left)) { currentRoom = (currentRoom - 1 + 37) % 37; }

            // GLOBAL ZOOM SCALING (Universal)
            if (Raylib.IsKeyDown(KeyboardKey.KpAdd)) globalScale += 0.02f;
            if (Raylib.IsKeyDown(KeyboardKey.KpSubtract)) globalScale -= 0.02f;

            // Perspective Dimension Toggles
            if (Raylib.IsKeyPressed(KeyboardKey.Up)) is3DMode = true;
            if (Raylib.IsKeyPressed(KeyboardKey.Down)) is3DMode = false;

            // Shared Layout View Toggles
            if (Raylib.IsKeyPressed(KeyboardKey.M)) { renderStyleMode = (renderStyleMode + 1) % 3; }
            if (Raylib.IsKeyPressed(KeyboardKey.P)) { displayPathOverlays = !displayPathOverlays; }
            if (Raylib.IsKeyPressed(KeyboardKey.G)) { displayGems = !displayGems; }

            // SANDBOX EXPERIMENT: Repurposed 'B' key to handle Dan's legacy 3D math projections
            if (Raylib.IsKeyPressed(KeyboardKey.B)) { showDanLegacyOverlay = !showDanLegacyOverlay; }

            // CANVAS INVERSION HOOK (V Key)
            if (Raylib.IsKeyPressed(KeyboardKey.V)) { invertBackground = !invertBackground; }

            // NEW: INTERACTIVE 3D LAB CANVAS TRIGGER (X Key)
            if (Raylib.IsKeyPressed(KeyboardKey.X)) { trigger3DLabFlag = true; }

            // Global 3D Mode Calibrations
            if (is3DMode)
            {
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
                showDanLegacyOverlay = false;
                invertBackground = false;
            }
        }

        public static void DrawControlOverlay(bool is3DMode, int renderStyle, bool pathsOn, bool gemsOn, bool legacyOverlayOn,
        bool isInverted, float globalScale, int currentRoom, string stageName, int totalElevators, Color[] activeTheme)
        {
            Color cardBg = isInverted ? new Color(230, 230, 230, 220) : new Color(20, 20, 20, 200);
            Color cardBorder = isInverted ? Color.DarkGray : Color.LightGray;
            Color textClr = isInverted ? Color.Black : Color.RayWhite;

            Raylib.DrawText("ccSharpRaylib", 20, 20, 20, isInverted ? Color.Black : Color.RayWhite);

            int displayLevel = (currentRoom / 4) + 1;
            int displayWave = (currentRoom % 4) + 1;
            string viewModeLabel = is3DMode ? "3D Isometric" : "2D Flat";

            Raylib.DrawText($"Level {displayLevel} - {displayWave} [{stageName}] | Lifts: {totalElevators} | {viewModeLabel}", 20, 55, 18, Color.Gold);

            Raylib.DrawRectangle(20, 95, 40, 20, activeTheme[0]);
            Raylib.DrawRectangle(70, 95, 40, 20, activeTheme[1]);
            Raylib.DrawRectangle(120, 95, 40, 20, activeTheme[2]);
            Raylib.DrawText("Active Layout Palette Matrix Slots", 180, 98, 14, isInverted ? Color.DarkGray : Color.LightGray);

            int rectY = 130;
            int cardHeight = is3DMode ? 265 : 240;
            Raylib.DrawRectangle(15, rectY, 210, cardHeight, cardBg);
            Raylib.DrawRectangleLines(15, rectY, 210, cardHeight, cardBorder);

            int startTextY = rectY + 10;
            Raylib.DrawText("CONTROLS QUICK MENU", 25, startTextY, 13, Color.Gold);

            Raylib.DrawText("Left/Right : Change Stage", 25, startTextY + 22, 11, textClr);
            Raylib.DrawText("Up / Down  : Toggle 2D/3D", 25, startTextY + 40, 11, textClr);
            Raylib.DrawText($"P          : Pathways [{(pathsOn ? "ON" : "OFF")}]", 25, startTextY + 58, 11, textClr);
            Raylib.DrawText($"G          : Gems     [{(gemsOn ? "ON" : "OFF")}]", 25, startTextY + 76, 11, textClr);

            // HUD UPDATE: Clearly displays status of the legacy blue blueprint overlay injection
            Raylib.DrawText($"B          : Dan Legacy [{(legacyOverlayOn ? "ON" : "OFF")}]", 25, startTextY + 96, 11, Color.SkyBlue);
            Raylib.DrawText($"V          : Invert   [{(isInverted ? "WHITE" : "BLACK")}]", 25, startTextY + 114, 11, Color.Yellow);
            Raylib.DrawText($"X          : Launch 3D Lab", 25, startTextY + 132, 11, Color.Lime);
            Raylib.DrawText($"+ / -      : Zoom [{globalScale:F2}]", 25, startTextY + 150, 11, textClr);

            if (!is3DMode)
            {
                Raylib.DrawText("T          : Export Matrix Text", 25, startTextY + 170, 11, Color.SkyBlue);
                Raylib.DrawText("R          : Reset View", 25, startTextY + 188, 11, textClr);
            }
            else
            {
                string styleName = renderStyle == 0 ? "Original Filled" : (renderStyle == 1 ? "Cel Shaded" : "Wireframe");
                Raylib.DrawText($"M          : Style [{styleName}]", 25, startTextY + 170, 11, Color.Orange);
                Raylib.DrawText("R          : Reset View", 25, startTextY + 188, 11, textClr);

                Raylib.DrawRectangle(15, rectY + 280, 210, 110, cardBg);
                Raylib.DrawRectangleLines(15, rectY + 280, 210, 110, cardBorder);

                Raylib.DrawText("3D PARAMETERS", 25, rectY + 287, 12, Color.Gold);
                Raylib.DrawText("A / D   : Rotate Grid", 25, rectY + 307, 11, isInverted ? Color.DarkGray : Color.LightGray);
                Raylib.DrawText("Q / E   : Perspective Tilt", 25, rectY + 325, 11, isInverted ? Color.DarkGray : Color.LightGray);
                Raylib.DrawText("W / S   : Scale Height", 25, rectY + 343, 11, isInverted ? Color.DarkGray : Color.LightGray);
                Raylib.DrawText("I/K/J/L : Pan Camera", 25, rectY + 361, 11, isInverted ? Color.DarkGray : Color.LightGray);

                
            }
            // ============================================================================
            // FIX BANNER: INPUTHANDLER.CS - INTEGRATED METRIC RADAR COCKPIT (v0.81)
            // ============================================================================
            var diagnosticActiveRoom = RomManager.IsolatedStages[currentRoom];

            if (diagnosticActiveRoom.Elevators != null)
            {
                // Dynamic look up of live height matrix discrepancies against the raw parent ROM bytes
                int terrainMismatches = MapRenderer.GetRomHeightDiscrepancyCount(diagnosticActiveRoom, currentRoom);
                Color radarColor = (terrainMismatches == 0) ? Color.Lime : Color.Yellow;

                // Expand background block height container slightly (from 110 to 130) to house the radar stats cleanly
                Raylib.DrawRectangle(15, rectY + 280, 210, 130, cardBg);
                Raylib.DrawRectangleLines(15, rectY + 280, 210, 130, cardBorder);

                Raylib.DrawText("LIVE REPOSITORY MONITOR", 25, rectY + 287, 12, Color.Gold);
                Raylib.DrawText($"  * Layout Drift: {terrainMismatches} cells mismatched", 25, rectY + 307, 11, radarColor);

                // ============================================================================
                // REPAIRED HUD COUNTER: PROTECT EMPTY LIFT ARRAYS FROM CRASHES
                // ============================================================================
                if (diagnosticActiveRoom.Elevators.Count > 0)
                {
                    // Safe reference pass: inspect the first lift element without risking array bounds breaches
                    var monitorLift = diagnosticActiveRoom.Elevators[0];
                    Color debugColor = monitorLift.IsMapped ? Color.Lime : Color.Red;

                    Raylib.DrawText($"  * Is Mapped Flag : {monitorLift.IsMapped}", 25, rectY + 327, 11, debugColor);
                    Raylib.DrawText($"  * Physics Mode   : Mode_{monitorLift.Mode}", 25, rectY + 345, 11, textClr);
                    Raylib.DrawText($"  * Height Position: {monitorLift.CurrentPosition} / {monitorLift.TopPosition}", 25, rectY + 363, 11, textClr);
                    Raylib.DrawText($"  * Frame Sit Clock: {monitorLift.CurrentSitTime} / {monitorLift.WaitTime}", 25, rectY + 381, 11, textClr);
                }
                else
                {
                    Raylib.DrawText("  * Lift Mechanics : Zero Active Elevators", 25, rectY + 327, 11, Color.DarkGray);
                }
            }
            // ============================================================================
            // END FIX BANNER: TELEMETRY RADAR CONVERGENCE SUCCESSFULLY OPERATIONAL
            // ============================================================================
        }
    }
}

