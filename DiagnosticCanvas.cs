using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace cSharpRaylib
{
    public static class DiagnosticCanvas
    {
        private static int _selectedElevatorIndex = -1;
        private const int MaxElevatorLimit = 10;

        // v0.91 Unified Diagnostic Track Parameters
        private static bool _isInGemMode = true; // True = Gem Layer View, False = Elevator Layer View
        private static int _hoveredRowX = -1;
        private static int _hoveredColY = -1;
        private static int _maxObservedHeight = 0;

        public static void LaunchDebugWindow(int startingRoom, string[] stageNames, byte[] roomToCityMap, List<CityData> cities)
        {
            // Canvas expanded to 1200 width to grant a dedicated 200px right-margin dashboard lane
            const int winW = 1200;
            const int winH = 1000;

            Raylib.InitWindow(winW, winH, "Diagnostic Grid Laboratory — Isolated View Suite [v0.91]");
            Raylib.SetTargetFPS(60);

            int currentRoom = startingRoom;
            bool shouldUpdateStage = true;
            bool spaceMapView = false;

            List<ElevatorData> mockList = new List<ElevatorData>();
            CityData activeCity = null;
            CrystalCastles.DataEngine.CCUnifiedParser.UnifiedStageProfile diagnosticProfile = null;

            // Tracking matrix latches cells checked during active troubleshooting session
            bool[,] sessionLoggedCells = new bool[22, 22];

            while (!Raylib.WindowShouldClose())
            {
                // Local Input Focus Routing: Locomotion strings are consumed inside this canvas scope exclusively
                if (Raylib.IsKeyPressed(KeyboardKey.Right)) { currentRoom = (currentRoom + 1) % 37; shouldUpdateStage = true; }
                if (Raylib.IsKeyPressed(KeyboardKey.Left)) { currentRoom = (currentRoom - 1 + 37) % 37; shouldUpdateStage = true; }
                if (Raylib.IsKeyPressed(KeyboardKey.S)) { spaceMapView = !spaceMapView; }

                // V Toggles the operational layer mode parameters explicitly
                if (Raylib.IsKeyPressed(KeyboardKey.V)) { _isInGemMode = !_isInGemMode; }

                // FIX BANNER: 1200x1000 DIAGNOSTIC CANVAS MOUSE BOUNDING LINE [v0.91]
                bool isMouseInsideGrid = (Raylib.GetMouseX() >= 0 && Raylib.GetMouseX() <= 1200 &&
                                          Raylib.GetMouseY() >= 0 && Raylib.GetMouseY() <= 1000);

                if (shouldUpdateStage)
                {
                    int cityIndex = roomToCityMap[currentRoom] & 0x0F;
                    activeCity = cities[cityIndex];

                    // Route relative path arrays to ingest our raw v0.91 datasets parallel
                    string revisedFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "revised");
                    string formattedName = stageNames[currentRoom].Replace(" ", "_").Replace("'", "").ToUpper();
                    string fileName = $"STAGE_{currentRoom:D2}_{formattedName}.txt";
                    string revisedFilePath = Path.Combine(revisedFolder, fileName);

                    // FIX BANNER: SYNC DIAGNOSTIC MONITOR STRIDES DIRECTLY TO DUAL LOGGER BUFFER [v0.91]
                    string unifiedSessionLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "session_audit.log");
                    using (StreamWriter localAuditWriter = new StreamWriter(unifiedSessionLog, true, Encoding.UTF8))
                    {
                        diagnosticProfile = CrystalCastles.DataEngine.CCUnifiedParser.LoadUnifiedStageFile(revisedFilePath, localAuditWriter);
                    }

                    // Force the active viewport city map values to update using the newly parsed text profile
                    if (diagnosticProfile != null && diagnosticProfile.Heights != null)
                    {
                        for (int r = 0; r < 22; r++)
                        {
                            for (int c = 0; c < 22; c++)
                            {
                                activeCity.Heights[r, c] = diagnosticProfile.Heights[r, c];
                                activeCity.Gems[r, c] = diagnosticProfile.Gems[r, c];
                            }
                        }
                    }

                    mockList.Clear();
                    for (int i = 0; i < activeCity.Elevators.Count; i++)
                    {
                        ElevatorData copy = new ElevatorData();
                        copy.HorizontalPosition = activeCity.Elevators[i].HorizontalPosition;
                        copy.VerticalPosition = activeCity.Elevators[i].VerticalPosition;
                        copy.BottomPosition = activeCity.Elevators[i].BottomPosition;
                        copy.TopPosition = activeCity.Elevators[i].TopPosition;
                        copy.WaitTime = activeCity.Elevators[i].WaitTime;
                        copy.CellX = activeCity.Elevators[i].CellX;
                        copy.CellY = activeCity.Elevators[i].CellY;
                        copy.IsMapped = activeCity.Elevators[i].IsMapped;
                        mockList.Add(copy);
                    }

                    ElevatorPremapper.ApplyOverrides(currentRoom, mockList);

                    // Scan the loaded heights map array to cache the absolute peak height value for the dashboard
                    _maxObservedHeight = 0;
                    for (int r = 0; r < 22; r++)
                    {
                        for (int c = 0; c < 22; c++)
                        {
                            if (activeCity.Heights[r, c] > _maxObservedHeight)
                                _maxObservedHeight = activeCity.Heights[r, c];
                        }
                    }

                    Array.Clear(sessionLoggedCells, 0, sessionLoggedCells.Length);
                    shouldUpdateStage = false;
                }

                int mousePixelX = Raylib.GetMouseX();
                int mousePixelY = Raylib.GetMouseY();

                // Shifting grid offsets down and right to permanently prevent title text collisions
                _hoveredColY = (mousePixelX - 150) / 36;
                _hoveredRowX = (mousePixelY - 150) / 36;

                // --- v0.91 Focus-Latching Click Interceptor & L-Key Log Matrix ---
                string telemetryOutputDisplayString = "ROW (X): OUT  |  COL (Y): OUT";

                if (isMouseInsideGrid)
                {
                    telemetryOutputDisplayString = $"ROW (X): {_hoveredRowX:D2}  |  COL (Y): {_hoveredColY:D2}";

                    // Click to latch cell coordinates natively into our active troubleshooting session
                    if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                    {
                        sessionLoggedCells[_hoveredRowX, _hoveredColY] = true;

                        if (_selectedElevatorIndex >= 0 && _selectedElevatorIndex < mockList.Count)
                        {
                            mockList[_selectedElevatorIndex].CellX = _hoveredRowX;
                            mockList[_selectedElevatorIndex].CellY = _hoveredColY;
                            mockList[_selectedElevatorIndex].IsMapped = true;
                            _selectedElevatorIndex = -1;
                        }
                        else
                        {
                            for (int i = 0; i < mockList.Count; i++)
                            {
                                if (mockList[i].IsMapped && mockList[i].CellX == _hoveredRowX && mockList[i].CellY == _hoveredColY)
                                {
                                    _selectedElevatorIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                }

                // Explicit 'L' key press logs current cell metrics directly to session_audit.log
                if (Raylib.IsKeyPressed(KeyboardKey.L) && isMouseInsideGrid)
                {
                    string unifiedSessionLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "session_audit.log");
                    try
                    {
                        using (StreamWriter appendWriter = new StreamWriter(unifiedSessionLog, true, Encoding.UTF8))
                        {
                            appendWriter.WriteLine($"[MANUAL TELEMETRY RECORD] Stage: {stageNames[currentRoom]} (ID: {currentRoom:D2})");
                            appendWriter.WriteLine($"  -> Targeted Coordinates: Row_X={_hoveredRowX:D2}, Col_Y={_hoveredColY:D2}");
                            appendWriter.WriteLine($"  -> Active Altitude Map : Value={activeCity.Heights[_hoveredRowX, _hoveredColY]}");
                            appendWriter.WriteLine($"  -> Active Mode Filter  : [{(_isInGemMode ? "GEM VIEW" : "ELEVATOR VIEW")}]");
                            appendWriter.WriteLine("--------------------------------------------------------------------------------\n");
                        }
                        Console.Beep(2100, 80); // Success tone response indicators
                    }
                    catch { /* Drive file locking protection safeguards */ }
                }

                // --- CRITICAL PERSISTENCE FIX: Return safely to main application loop thread without hard crashing ---
                if (Raylib.WindowShouldClose())
                {
                    // FIX BANNER: RE-FRAME ENGINE DRAW BOUNDARIES ON EXIT [v0.91]
                    RomManager.IsParserDiagnosticActive = false;
                    Raylib.BeginDrawing();
                    Raylib.ClearBackground(Color.Black);
                    Raylib.EndDrawing();
                    break;
                }

                Raylib.ClearBackground(Color.Black);

                // FIX BANNER: DEFERRING TEXT AND SIDEBAR TO DRAW ON TOP OF GRID OVERLAYS [v0.91]
                // (This block is left blank intentionally here, moving text logic below the grid loop!)

                // --- v0.91 Shifted 22x22 Grid Layout Drawing Engine ---
                int cellSize = 36;
                int startX = 150; // Pushed right to clear text collisions
                int startY = 150; // Pushed down to clear text collisions

                for (int x = 0; x < 22; x++)
                {
                    for (int y = 0; y < 22; y++)
                    {
                        int posX = startX + (y * cellSize);
                        int posY = startY + (x * cellSize);
                        int h = activeCity.Heights[x, y];

                        Raylib.DrawRectangleLines(posX, posY, cellSize, cellSize, new Color(45, 45, 40, 255));

                        if (h > 0)
                        {
                            Color terrainColor = spaceMapView ? new Color(0, 30, 60, 255) : new Color(0, 50, 0, 255);
                            Raylib.DrawRectangle(posX + 2, posY + 2, cellSize - 4, cellSize - 4, terrainColor);
                        }

                        // Gated Rendering Logic: Respects mode flags to show elements in complete isolation
                        if (!_isInGemMode)
                        {
                            bool isElevatorCell = false;
                            foreach (var ev in mockList)
                            {
                                if (ev.IsMapped && ev.CellX == x && ev.CellY == y) { isElevatorCell = true; break; }
                            }
                            if (isElevatorCell) Raylib.DrawText("E", posX + 12, posY + 8, 20, Color.White);
                        }
                        else if (diagnosticProfile != null && diagnosticProfile.Gems != null && diagnosticProfile.Gems[x, y])
                        {
                            Raylib.DrawCircle(posX + (cellSize / 2), posY + (cellSize / 2), 6, Color.Gold);
                        }

                        if (sessionLoggedCells[x, y])
                        {
                            Raylib.DrawRectangleLines(posX + 4, posY + 4, cellSize - 8, cellSize - 4, Color.Orange);
                        }
                    }
                }

                // --- v0.91 SCALED VECTOR TEXT DRAWING LAYER (Safely isolated from grid math) ---
                Raylib.DrawRectangle(150, 30, 900, 45, Color.DarkBlue);
                Raylib.DrawRectangleLines(150, 30, 900, 45, Color.White);
                // Active Telemetry: Scaled to 3x multiplier
                DrawVectorText(telemetryOutputDisplayString, 165, 42, 5, Color.Lime);

                // Header Labels: Main title at 3x scale, shifted slightly left (to 540) to prevent overflow bounding leaks
                DrawVectorText($"STAGE: {stageNames[currentRoom].Replace(" ", "_").ToUpper()} [V0.91]", 240, 90, 3, Color.Gold);
                DrawVectorText("V: TOGGLE MODE | L: LOG  | Le/Ri: STAGE", 140, 120, 4, Color.LightGray);

                // --- v0.91 Right-Margin Sidebar Dashboard (Text expanded to crisp 2x scale tracking layouts) ---
                Raylib.DrawRectangle(980, 150, 200, 792, new Color(20, 20, 25, 255));
                Raylib.DrawRectangleLines(980, 150, 200, 792, Color.DarkGray);

                DrawVectorText("DASH", 995, 170, 4, Color.Yellow);
                DrawVectorText("A MODE:", 1000, 210, 4, Color.White);
                DrawVectorText(_isInGemMode ? "[GEM MTRX]" : "[LIFT SHFT]", 1000, 250, 4, _isInGemMode ? Color.Gold : Color.White);

                DrawVectorText("STG MAX H:", 1000, 280, 4, Color.White);
                DrawVectorText($"{_maxObservedHeight:D3} U", 1000, 350, 4, Color.Lime);

                Raylib.EndDrawing();
            }
            // CRITICAL HOUSEKEEPING: Raylib.CloseWindow() completely removed to prevent root app crashes.
        }

        private static void AppendExpandedLabAuditLog(int stageNum, int cellX, int cellY, CityData city, List<ElevatorData> elevators)
        {
            // ============================================================================
            // FIXED BANNER: v0.85 DATA ENGINE ISOLATION DEACTIVATION GATE
            // ============================================================================
            return; // Early exit completely disables file generation and beeps globally.
                    // ============================================================================

            try
            {
                // ====================================================================================
                // FIX BANNER: RomManager.cs & InputHandler.cs SAFE DEACTIVATION GATE (v0.85)
                // ====================================================================================
                return; // Stops execution dead right here before any file stream is opened!
                        // ====================================================================================

                // Left completely untouched below so no downstream variables break:
                string stamp = RomManager.ActiveSessionTimestamp;
                string filename = $"LAB_TEST_LOG_STAGE_{stageNum:D2}_{stamp}.txt";
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

                byte romAttr = RomManager.BaseCities[stageNum % 16].Attributes[cellX, cellY];
                byte diskAttr = city.Attributes[cellX, cellY];
                int romHeight = RomManager.BaseCities[stageNum % 16].Heights[cellX, cellY];
                int diskHeight = city.Heights[cellX, cellY];

                // ============================================================================
                // DIAGNOSTICCANVAS.CS - PART 2: EXPANDED STREAM OUTPUT LAYER (v0.85)
                // ============================================================================
                using (StreamWriter sw = new StreamWriter(fullPath, true, Encoding.UTF8))
                {
                    sw.WriteLine($"[AUDIT POINT RECORDED - TIMESTAMP: {DateTime.Now:HH:mm:ss} | Engine: v0.85]");
                    sw.WriteLine($"  * Coordinate Vector : Row_X = {cellX:D2} , Col_Y = {cellY:D2}");

                    string terrainLabel = (diskHeight == 0) ? "VOID/PASSAGEWAY" : $"SOLID_DECK_PLATFORM (Height:{diskHeight})";
                    string pathLabel = ((diskAttr & 0x04) == 0x04) ? "ACTIVE_PATHWAY" : "STANDARD_TILES";
                    sw.WriteLine($"  * Text Classifiers  : [{terrainLabel} | {pathLabel}]");
                    sw.WriteLine($"  * Altitude Alignment: [Disk: {diskHeight:D2} vs ROM: {romHeight:D2}] -> {(diskHeight == romHeight ? "MATCH" : "DRIFT")}");

                    sw.WriteLine("  * Elevator Configuration Ledger:");
                    bool foundLift = false;
                    for (int i = 0; i < elevators.Count; i++)
                    {
                        var ev = elevators[i];
                        if (ev.CellX == cellX && ev.CellY == cellY)
                        {
                            foundLift = true;
                            sw.WriteLine($"    - Lift Index [{i}]: Status = VERIFIED_ATTACHED");
                            sw.WriteLine($"    - Motion State   : Mode_{ev.Mode} | SitTimer = {ev.CurrentSitTime} frames");
                            sw.WriteLine($"    - Range Limits   : Bottom = {ev.BottomPosition:D3} | Top = {ev.TopPosition:D3} | Live = {ev.CurrentPosition:D3}");
                        }
                    }
                    if (!foundLift) sw.WriteLine("    - Lift System    : No moving elevator elements mapped to this block.");

                    sw.WriteLine("  * Neighborhood Spatial Density Matrix (3x3 Surrounding Heights):");
                    sw.WriteLine("    [ DISK FILE LAYOUT ]             [ ARCADE ROM MEMORY ]");
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        StringBuilder dRow = new StringBuilder("    ");
                        StringBuilder rRow = new StringBuilder("    ");
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            int nx = cellX + dx; int ny = cellY + dy;
                            if (nx >= 0 && nx < 22 && ny >= 0 && ny < 22)
                            {
                                dRow.Append($" {city.Heights[nx, ny]:D2} ");
                                rRow.Append($" {RomManager.BaseCities[stageNum % 16].Heights[nx, ny]:D2} ");
                            }
                            else
                            {
                                dRow.Append(" XX "); rRow.Append(" XX ");
                            }
                        }
                        sw.WriteLine($"{dRow}        {rRow}");
                    }
                    sw.WriteLine("--------------------------------------------------------------------------------\n");
                }
                Console.Beep(1800, 100);
            }
            catch { }
        }
        // FIX BANNER: LIGHTWEIGHT GEOMETRIC VECTOR TEXT DRAWING UTILITY [v0.91]
        private static void DrawVectorChar(char c, int x, int y, int scale, Color color)
        {
            // 4x5 Dot Matrix Bitmask array mapping alphanumeric lines explicitly
            ushort glyph = 0;
            switch (char.ToUpper(c))
            {
                case 'A': glyph = 0xF99F; break;
                case 'B': glyph = 0xF9F9; break;
                case 'C': glyph = 0xF88F; break;
                case 'D': glyph = 0xF999; break;
                case 'E': glyph = 0xF8F8; break;
                case 'F': glyph = 0xF8F0; break;
                case 'G': glyph = 0xF8BF; break;
                case 'H': glyph = 0x99F9; break;
                case 'I': glyph = 0xF44F; break;
                case 'J': glyph = 0x119F; break;
                case 'K': glyph = 0x9AF9; break;
                case 'L': glyph = 0x888F; break;
                case 'M': glyph = 0x9FFF; break;
                case 'N': glyph = 0x9FFF; break;
                case 'O': glyph = 0xF99F; break;
                case 'P': glyph = 0xF9F0; break;
                case 'Q': glyph = 0xF9DF; break;
                case 'R': glyph = 0xF9F9; break;
                case 'S': glyph = 0xF8F7; break;
                case 'T': glyph = 0xF444; break;
                case 'U': glyph = 0x999F; break;
                case 'V': glyph = 0x9994; break;
                case 'W': glyph = 0x9FFF; break;
                case 'X': glyph = 0x9669; break;
                case 'Y': glyph = 0x9522; break;
                case 'Z': glyph = 0xF24F; break;
                case '0': glyph = 0xF99F; break;
                case '1': glyph = 0x4444; break;
                case '2': glyph = 0xF24F; break;
                case '3': glyph = 0xF17F; break;
                case '4': glyph = 0x99F1; break;
                case '5': glyph = 0xF8F7; break;
                case '6': glyph = 0xF8FF; break;
                case '7': glyph = 0xF111; break;
                case '8': glyph = 0xF9FF; break;
                case '9': glyph = 0xF9F1; break;
                case ':': glyph = 0x0404; break;
                case '-': glyph = 0x0060; break;
                case '(': glyph = 0x6446; break;
                case ')': glyph = 0x9229; break;
                case '[': glyph = 0xE88E; break;
                case ']': glyph = 0xB22B; break;
                case '|': glyph = 0x4444; break;
                case '_': glyph = 0x000F; break;
                default: glyph = 0x0000; break; // Blank space
            }

            int w = 4 * scale;
            int h = 5 * scale;
            for (int r = 0; r < 5; r++)
            {
                for (int col = 0; col < 4; col++)
                {
                    int bitIdx = 15 - (r * 4 + col);
                    if (bitIdx >= 0 && ((glyph >> bitIdx) & 1) == 1)
                    {
                        Raylib.DrawRectangle(x + (col * scale), y + (r * scale), scale, scale, color);
                    }
                }
            }
        }

        public static void DrawVectorText(string text, int x, int y, int scale, Color color)
        {
            int currentX = x;
            foreach (char c in text)
            {
                DrawVectorChar(c, currentX, y, scale, color);
                currentX += 5 * scale; // Uniform tracking space offset spacing per character block
            }
        }
    }
}
