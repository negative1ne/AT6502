// ============================================================================
// INPUTHANDLER.CS - v0.81 TELEMETRY LAYOUT HARNESS (PART 1 OF 2 - FIXED DRAW)
// ============================================================================
using Raylib_cs;
using System;
using System.IO;
using System.Text;
using System.Numerics;

namespace cSharpRaylib
{
    public static class InputHandler
    {
        private static Vector2 _probeMouseScreenPos;
        private static bool _lastIs3DMode = false;

        private static int _globalPointIncrementer = 0;
        private static int _lastRecordedStageId = -1;

        public static int ProbeGridX { get; private set; } = -1;
        public static int ProbeGridY { get; private set; } = -1;
        public static bool IsProbeInsideWorkspace { get; private set; } = false;
        public static bool IsProbeLockedToGrid { get; private set; } = false;

        public static void HandleKeys(
            ref int currentRoom, ref bool is3DMode, ref float globalScale,
            ref float heightMultiplier, ref int panOffsetX, ref int panOffsetY,
            ref int rotationAngle, ref float tiltFactor, ref int renderStyleMode,
            ref bool displayPathOverlays, ref bool displayGems, ref bool showDanLegacyOverlay,
            ref bool invertBackground, ref bool exportTextFlag, ref bool trigger3DLabFlag)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Right)) { currentRoom = (currentRoom + 1) % 37; }
            if (Raylib.IsKeyPressed(KeyboardKey.Left)) { currentRoom = (currentRoom - 1 + 37) % 37; }

            if (Raylib.IsKeyDown(KeyboardKey.KpAdd)) globalScale += 0.02f;
            if (Raylib.IsKeyDown(KeyboardKey.KpSubtract)) globalScale -= 0.02f;

            if (Raylib.IsKeyPressed(KeyboardKey.Up)) is3DMode = true;
            if (Raylib.IsKeyPressed(KeyboardKey.Down)) is3DMode = false;

            if (Raylib.IsKeyPressed(KeyboardKey.M)) { renderStyleMode = (renderStyleMode + 1) % 3; }
            if (Raylib.IsKeyPressed(KeyboardKey.P)) { displayPathOverlays = !displayPathOverlays; }
            if (Raylib.IsKeyPressed(KeyboardKey.G)) { displayGems = !displayGems; }
            if (Raylib.IsKeyPressed(KeyboardKey.B)) { showDanLegacyOverlay = !showDanLegacyOverlay; }
            if (Raylib.IsKeyPressed(KeyboardKey.V)) { invertBackground = !invertBackground; }
            if (Raylib.IsKeyPressed(KeyboardKey.X)) { trigger3DLabFlag = true; }

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

            if (!is3DMode && Raylib.IsKeyPressed(KeyboardKey.T)) exportTextFlag = true;

            if (Raylib.IsKeyPressed(KeyboardKey.R))
            {
                globalScale = 1.0f; heightMultiplier = 1.8f; panOffsetX = 0; panOffsetY = 0;
                rotationAngle = 0; tiltFactor = 1.0f; renderStyleMode = 0;
                displayPathOverlays = false; displayGems = true; showDanLegacyOverlay = false;
                invertBackground = false; IsProbeLockedToGrid = false;
            }
        }

        public static void DrawControlOverlay(bool is3DMode, int renderStyle, bool pathsOn, bool gemsOn, bool legacyOverlayOn,
        bool isInverted, float globalScale, int currentRoom, string stageName, int totalElevators, Raylib_cs.Color[] activeTheme)
        {
            Raylib_cs.Color cardBg = isInverted ? new Raylib_cs.Color(230, 230, 230, 220) : new Raylib_cs.Color(20, 20, 20, 200);
            Raylib_cs.Color cardBorder = isInverted ? Raylib_cs.Color.DarkGray : Raylib_cs.Color.LightGray;
            Raylib_cs.Color textClr = isInverted ? Raylib_cs.Color.Black : Raylib_cs.Color.RayWhite;

            Raylib.DrawText("ccSharpRaylib", 20, 20, 20, isInverted ? Raylib_cs.Color.Black : Raylib_cs.Color.RayWhite);

            int displayLevel = (currentRoom / 4) + 1;
            int displayWave = (currentRoom % 4) + 1;
            string viewModeLabel = is3DMode ? "3D Isometric" : "2D Flat";

            Raylib.DrawText($"Level {displayLevel} - {displayWave} [{stageName}] | Lifts: {totalElevators} | {viewModeLabel}", 20, 55, 18, Raylib_cs.Color.Gold);

            // FIXED LINES 88, 89, 90: Pull color element from activeTheme array index smoothly
            Raylib_cs.Color drawingColorSample = (activeTheme != null && activeTheme.Length > 0) ? activeTheme[0] : Raylib_cs.Color.White;
            Raylib.DrawRectangle(20, 95, 40, 20, drawingColorSample);
            Raylib.DrawRectangle(70, 95, 40, 20, drawingColorSample);
            Raylib.DrawRectangle(120, 95, 40, 20, drawingColorSample);

            Raylib.DrawText("Active Layout Palette Matrix Slots", 180, 98, 14, isInverted ? Raylib_cs.Color.DarkGray : Raylib_cs.Color.LightGray);

            int rectY = 130;
            int cardHeight = is3DMode ? 265 : 240;
            Raylib.DrawRectangle(15, rectY, 210, cardHeight, cardBg);
            Raylib.DrawRectangleLines(15, rectY, 210, cardHeight, cardBorder);

            int startTextY = rectY + 10;
            Raylib.DrawText("CONTROLS QUICK MENU", 25, startTextY, 13, Raylib_cs.Color.Gold);
            Raylib.DrawText("Left/Right : Change Stage", 25, startTextY + 22, 11, textClr);
            Raylib.DrawText("Up / Down  : Toggle 2D/3D", 25, startTextY + 40, 11, textClr);
            Raylib.DrawText($"P          : Pathways [{(pathsOn ? "ON" : "OFF")}]", 25, startTextY + 58, 11, textClr);
            Raylib.DrawText($"G          : Gems     [{(gemsOn ? "ON" : "OFF")}]", 25, startTextY + 76, 11, textClr);
            Raylib.DrawText($"B          : Dan Legacy [{(legacyOverlayOn ? "ON" : "OFF")}]", 25, startTextY + 96, 11, Raylib_cs.Color.SkyBlue);
            Raylib.DrawText($"V          : Invert   [{(isInverted ? "WHITE" : "BLACK")}]", 25, startTextY + 114, 11, Raylib_cs.Color.Yellow);
            Raylib.DrawText($"X          : Launch 3D Lab", 25, startTextY + 132, 11, Raylib_cs.Color.Lime);
            Raylib.DrawText($"+ / -      : Zoom [{globalScale:F2}]", 25, startTextY + 150, 11, textClr);

            if (!is3DMode)
            {
                Raylib.DrawText("T          : Export Matrix Text", 25, startTextY + 170, 11, Raylib_cs.Color.SkyBlue);
                Raylib.DrawText("R          : Reset View", 25, startTextY + 188, 11, textClr);
            }
            else
            {
                string styleName = renderStyle == 0 ? "Original Filled" : (renderStyle == 1 ? "Cel Shaded" : "Wireframe");
                Raylib.DrawText($"M          : Style [{styleName}]", 25, startTextY + 170, 11, Raylib_cs.Color.Orange);
                Raylib.DrawText("R          : Reset View", 25, startTextY + 188, 11, textClr);
            }

            var dRoom = RomManager.IsolatedStages[currentRoom];
            if (dRoom != null && dRoom.Elevators != null)
            {
                int terrainMismatches = MapRenderer.GetRomHeightDiscrepancyCount(dRoom, currentRoom);
                Raylib_cs.Color radarColor = (terrainMismatches == 0) ? Raylib_cs.Color.Lime : Raylib_cs.Color.Yellow;

                Raylib.DrawRectangle(15, rectY + 280, 210, 135, cardBg);
                Raylib.DrawRectangleLines(15, rectY + 280, 210, 135, cardBorder);
                Raylib.DrawText("LIVE REPOSITORY MONITOR", 25, rectY + 287, 12, Raylib_cs.Color.Gold);
                Raylib.DrawText($"  * Layout Drift: {terrainMismatches} cells", 25, rectY + 307, 11, radarColor);

                if (dRoom.Elevators.Count > 0)
                {
                    // FIXED: Added [0] index to pull fields from the first elevator element in the list smoothly
                    Raylib_cs.Color debugColor = dRoom.Elevators[0].IsMapped ? Raylib_cs.Color.Lime : Raylib_cs.Color.Red;
                    Raylib.DrawText($"  * Is Mapped Flag : {dRoom.Elevators[0].IsMapped}", 25, rectY + 327, 11, debugColor);
                    Raylib.DrawText($"  * Physics Mode   : Mode_{dRoom.Elevators[0].Mode}", 25, rectY + 345, 11, textClr);
                }
                else
                {
                    Raylib.DrawText("  * Lift Mechanics : Zero Active Elevators", 25, rectY + 327, 11, Raylib_cs.Color.DarkGray);
                }

                if (IsProbeInsideWorkspace)
                {
                    string lockLabel = IsProbeLockedToGrid ? "[LOCKED]" : "[FREE]";
                    Raylib_cs.Color trackClr = IsProbeLockedToGrid ? Raylib_cs.Color.Orange : Raylib_cs.Color.Yellow;
                    Raylib.DrawText($"  * Probe Target : X={ProbeGridX:D2} Y={ProbeGridY:D2} {lockLabel}", 25, rectY + 365, 11, trackClr);
                }
                else
                {
                    Raylib.DrawText("  * Probe Target : OUT OF BOUNDS", 25, rectY + 365, 11, Raylib_cs.Color.DarkGray);
                }
            }
        }
        // ============================================================================
        // INPUTHANDLER.CS - v0.81 LAB REPORT EXPORT PIPELINE (PART 2 OF 2 - FIXED DRAW)
        // ============================================================================
        public static void TrackMouseProbeCoordinates(float screenX, float screenY,
            float scale, int offsetX, int offsetY, int rotationAngle, float tiltFactor,
            CityData activeCity, bool is3DMode, int currentRoom)
        {
            if (is3DMode != _lastIs3DMode)
            {
                _lastIs3DMode = is3DMode;
                return;
            }

            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                if (IsProbeInsideWorkspace || IsProbeLockedToGrid)
                {
                    IsProbeLockedToGrid = !IsProbeLockedToGrid;

                    if (IsProbeLockedToGrid && activeCity != null)
                    {
                        AppendSelectedCellToLabLog(currentRoom, ProbeGridX, ProbeGridY, activeCity);
                    }
                }
            }

            if (IsProbeLockedToGrid) return;

            _probeMouseScreenPos.X = screenX;
            _probeMouseScreenPos.Y = screenY;

            if (activeCity == null) return;

            if (!is3DMode)
            {
                const int cellSize2D = 16;
                const int startX2D = 380;
                const int startY2D = 150;

                int relativeX = (int)screenX - startX2D;
                int relativeY = (int)screenY - startY2D;

                int gridCellY = relativeX >= 0 ? relativeX / cellSize2D : -1;
                int gridCellX = relativeY >= 0 ? relativeY / cellSize2D : -1;

                if (gridCellX >= 0 && gridCellX < 22 && gridCellY >= 0 && gridCellY < 22)
                {
                    ProbeGridX = gridCellX; ProbeGridY = gridCellY; IsProbeInsideWorkspace = true;
                }
                else
                {
                    ProbeGridX = -1; ProbeGridY = -1; IsProbeInsideWorkspace = false;
                }
                return;
            }

            int originX = 500 + offsetX;
            int originY = 340 + offsetY;
            float heightScale = 1.8f;

            float closestDistance = float.MaxValue;
            int bestX = -1; int bestY = -1;
            double rad = rotationAngle * Math.PI / 180.0;

            for (int x = 0; x < 22; x++)
            {
                for (int y = 0; y < 22; y++)
                {
                    double cx = x - 11.0; double cy = y - 11.0;
                    float rotX = (float)(cx * Math.Cos(rad) - cy * Math.Sin(rad)) + 11f;
                    float rotY = (float)(cx * Math.Sin(rad) + cy * Math.Cos(rad)) + 11f;

                    int tileHeight = activeCity.Heights[x, y];
                    float cellProjectedX = originX - (rotX * 12 * scale) + (rotY * 12 * scale);
                    float cellProjectedY = originY + (rotX * 6 * scale * tiltFactor) + (rotY * 6 * scale * tiltFactor) - (tileHeight * heightScale * scale);

                    float dx = screenX - cellProjectedX;
                    float dy = screenY - cellProjectedY;
                    float currentDist = (dx * dx) + (dy * dy);

                    if (currentDist < closestDistance)
                    {
                        closestDistance = currentDist; bestX = x; bestY = y;
                    }
                }
            }

            float thresholdDistance = 32.0f * scale;
            if (bestX != -1 && bestY != -1 && closestDistance < (thresholdDistance * thresholdDistance))
            {
                ProbeGridX = bestX; ProbeGridY = bestY; IsProbeInsideWorkspace = true;
            }
            else
            {
                ProbeGridX = -1; ProbeGridY = -1; IsProbeInsideWorkspace = false;
            }
        }

        private static void AppendSelectedCellToLabLog(int stageNum, int cellX, int cellY, CityData city)
        {
            try
            {
                string filename = $"LAB_TEST_LOG_STAGE_{stageNum:D2}.txt";
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

                if (_lastRecordedStageId != -1 && stageNum != _lastRecordedStageId)
                {
                    string fallbackOldFile = $"LAB_TEST_LOG_STAGE_{_lastRecordedStageId:D2}.txt";
                    string fallbackOldPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fallbackOldFile);
                    if (File.Exists(fallbackOldPath))
                    {
                        File.AppendAllText(fallbackOldPath, $"\n[AUDIT BREAK EVENT] Session Points Logged for Stage {_lastRecordedStageId:D2}: TotalCount = {_globalPointIncrementer}\n================================================================================\n");
                    }
                    _globalPointIncrementer = 0;
                }

                _lastRecordedStageId = stageNum;
                _globalPointIncrementer++;

                byte romAttribute = RomManager.BaseCities[stageNum % 16].Attributes[cellX, cellY];
                byte diskAttribute = city.Attributes[cellX, cellY];

                using (StreamWriter sw = new StreamWriter(fullPath, true, Encoding.UTF8))
                {
                    sw.WriteLine($"[POINT : {_globalPointIncrementer} - RECORDED TIMESTAMP: {DateTime.Now:HH:mm:ss}]");
                    sw.WriteLine($"  * Target Cell Row_X : {cellX:D2} , Col_Y: {cellY:D2}");
                    sw.WriteLine($"  * Active Altitude  : Height = {city.Heights[cellX, cellY]}");
                    sw.WriteLine($"  * Disk Attribute   : Byte = 0x{diskAttribute:X2} (Bits: {Convert.ToString(diskAttribute, 2).PadLeft(8, '0')})");
                    sw.WriteLine($"  * Raw ROM Baseline : Byte = 0x{romAttribute:X2} (Bits: {Convert.ToString(romAttribute, 2).PadLeft(8, '0')})");
                    sw.WriteLine("--------------------------------------------------------------------------------");
                }
                Console.Beep(2000, 150);
            }
            catch { }
        }
    }
}