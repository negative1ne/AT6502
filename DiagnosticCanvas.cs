// ============================================================================
// FIX BANNER: DIAGNOSTICCANVAS.CS - INTERACTIVE AUDIT WORKSPACE (v0.85)
// ============================================================================
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

        public static void LaunchDebugWindow(int startingRoom, string[] stageNames, byte[] roomToCityMap, List<CityData> cities)
        {
            const int winW = 1000;
            const int winH = 1000;

            Raylib.InitWindow(winW, winH, "Diagnostic Grid Laboratory — Interactive View Suite [v0.85]");
            Raylib.SetTargetFPS(60);

            int currentRoom = startingRoom;
            bool shouldUpdateStage = true;
            bool spaceMapView = false;

            List<ElevatorData> mockList = new List<ElevatorData>();
            CityData activeCity = null;

            // v0.85 TRACKING LATCH MATRIX: Flags cells checked during the active view session
            bool[,] sessionLoggedCells = new bool[22, 22];

            while (!Raylib.WindowShouldClose())
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Right)) { currentRoom = (currentRoom + 1) % 37; shouldUpdateStage = true; }
                if (Raylib.IsKeyPressed(KeyboardKey.Left)) { currentRoom = (currentRoom - 1 + 37) % 37; shouldUpdateStage = true; }
                if (Raylib.IsKeyPressed(KeyboardKey.S)) { spaceMapView = !spaceMapView; }

                if (shouldUpdateStage)
                {
                    int cityIndex = roomToCityMap[currentRoom] & 0x0F;
                    activeCity = cities[cityIndex];

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

                    // Flush session latches when moving to a new level
                    Array.Clear(sessionLoggedCells, 0, sessionLoggedCells.Length);
                    shouldUpdateStage = false;
                }

                int mousePixelX = Raylib.GetMouseX();
                int mousePixelY = Raylib.GetMouseY();
                int calculatedColY = (mousePixelX - 100) / 36;
                int calculatedRowX = (mousePixelY - 100) / 36;

                string coordinateTelemetryString = "ROW (X): OUT  |  COL (Y): OUT";
                bool isMouseInsideGrid = (calculatedRowX >= 0 && calculatedRowX < 22 && calculatedColY >= 0 && calculatedColY < 22);

                if (isMouseInsideGrid)
                {
                    coordinateTelemetryString = $"ROW (X): {calculatedRowX:D2}  |  COL (Y): {calculatedColY:D2}";

                    if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                    {
                        // Latch and trigger all three automated point log audits simultaneously
                        sessionLoggedCells[calculatedRowX, calculatedColY] = true;
                        AppendExpandedLabAuditLog(currentRoom, calculatedRowX, calculatedColY, activeCity, mockList);

                        if (_selectedElevatorIndex >= 0 && _selectedElevatorIndex < mockList.Count)
                        {
                            mockList[_selectedElevatorIndex].CellX = calculatedRowX;
                            mockList[_selectedElevatorIndex].CellY = calculatedColY;
                            mockList[_selectedElevatorIndex].IsMapped = true;
                            _selectedElevatorIndex = -1;
                        }
                        else
                        {
                            for (int i = 0; i < mockList.Count; i++)
                            {
                                if (mockList[i].IsMapped && mockList[i].CellX == calculatedRowX && mockList[i].CellY == calculatedColY)
                                {
                                    _selectedElevatorIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                Raylib.DrawRectangle(100, 30, 380, 40, Color.DarkBlue);
                Raylib.DrawRectangleLines(100, 30, 380, 40, Color.White);
                Raylib.DrawText(coordinateTelemetryString, 120, 40, 20, Color.Lime);

                Raylib.DrawText($"STAGE: {stageNames[currentRoom].ToUpper()} [v0.85]", 500, 30, 20, Color.Gold);
                Raylib.DrawText("Click Cell to Run Unified Audit Pass  |  S: Toggle Space View", 500, 55, 13, Color.LightGray);

                int cellSize = 36;
                int startX = 100;
                int startY = 100;

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

                        // v0.85 VISUAL CELL MARKERS PASS
                        bool isElevatorCell = false;
                        foreach (var ev in mockList)
                        {
                            if (ev.IsMapped && ev.CellX == x && ev.CellY == y)
                            {
                                isElevatorCell = true;
                                break;
                            }
                        }

                        if (isElevatorCell)
                        {
                            Raylib.DrawText("E", posX + 12, posY + 8, 20, Color.White);
                        }
                        else if (sessionLoggedCells[x, y])
                        {
                            // Overlay indicator that cell telemetry was successfully pushed to text log
                            Raylib.DrawText("L", posX + 13, posY + 8, 20, Color.Orange);
                        }
                    }
                }

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }

        private static void AppendExpandedLabAuditLog(int stageNum, int cellX, int cellY, CityData city, List<ElevatorData> elevators)
        {
            try
            {
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
    }
}