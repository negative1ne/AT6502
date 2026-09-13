using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Raylib_cs;
using Color = Raylib_cs.Color;

// ============================================================================
// DIAGNOSTICCANVAS.CS - INJECT STATE MANAGEMENT VARIABLES AT THE TOP
// ============================================================================
namespace cSharpRaylib
{
    public static class DiagnosticCanvas
    {
        // Tracks which elevator list item index is currently stuck to the mouse cursor
        private static int _selectedElevatorIndex = -1;

        // Target list configuration limit matching your multi-stage expansions
        private const int MaxElevatorLimit = 10;

        public static void LaunchDebugWindow(int startingRoom, string[] stageNames, byte[] roomToCityMap, List<CityData> cities)
        {
            const int winW = 1000;
            const int winH = 1000;

            Raylib.InitWindow(winW, winH, "Diagnostic Grid Laboratory — Interactive View Suite");
            Raylib.SetTargetFPS(60);

            int currentRoom = startingRoom;
            bool shouldUpdateStage = true;
            bool spaceMapView = false; // NEW: Toggle flag to bypass screen math calculations

            List<ElevatorData> mockList = new List<ElevatorData>();
            CityData activeCity = null;

            while (!Raylib.WindowShouldClose())
            {
                // Level cycling navigation triggers
                if (Raylib.IsKeyPressed(KeyboardKey.Right)) { currentRoom = (currentRoom + 1) % 37; shouldUpdateStage = true; }
                if (Raylib.IsKeyPressed(KeyboardKey.Left)) { currentRoom = (currentRoom - 1 + 37) % 37; shouldUpdateStage = true; }

                // NEW: Press S to swap between Projection View and Flat Space Map View!
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

                        // SYNC DATA: Pull the active grid assignments from the source cities collection array
                        copy.CellX = activeCity.Elevators[i].CellX;
                        copy.CellY = activeCity.Elevators[i].CellY;
                        copy.IsMapped = activeCity.Elevators[i].IsMapped;

                        mockList.Add(copy);
                    }

                    // Run our table overrides ONLY if the current room matches a problem stage entry
                    ElevatorPremapper.ApplyOverrides(currentRoom, mockList);
                    shouldUpdateStage = false;
                }

                // ============================================================================
                // DIAGNOSTICCANVAS.CS - RE-MAPPED ISOLATED FILE EXPORT SHORTCUT
                // ============================================================================
                // --- LIVE E-KEY MODULE INTERFACE TO EXPORT VERIFIED DETAILS ---
                // FIX: Changed from KeyboardKey.D to KeyboardKey.E to protect your backups from overwrites
                if (Raylib.IsKeyPressed(KeyboardKey.E))
                {
                    try
                    {
                        string currentStageName = stageNames[currentRoom];

                        using (StreamWriter writer = new StreamWriter(currentStageName, false, Encoding.UTF8))
                        {
                            writer.WriteLine($"=== DIAGNOSTIC GRID SHEET: STAGE {currentRoom:D2} ({currentStageName.ToUpper()}) ===");
                            writer.WriteLine($"Mode Target Context Profile: {(spaceMapView ? "FLAT UN-ROTATED SPACE DATA VIEW" : "ISOMETRIC PROJECTION SCREEN VIEW")}");
                            writer.WriteLine("[Legend: NN = Height, O = Premapper Box, R = ROM Footprint, M = Perfect Match, . = Empty Space]\n");

                            for (int x = 0; x < 22; x++)
                            {
                                StringBuilder rowLine = new StringBuilder();
                                for (int y = 0; y < 22; y++)
                                {
                                    bool isPremapped = false;
                                    foreach (var ev in mockList)
                                    {
                                        if (ev.IsMapped && ev.CellX == x && ev.CellY == y) isPremapped = true;
                                    }

                                    bool isRomFootprint = false;
                                    if (spaceMapView)
                                    {
                                        foreach (var raw in mockList)
                                        {
                                            int cellX = raw.HorizontalPosition % 22;
                                            int cellY = raw.VerticalPosition % 22;
                                            if (cellX == x && cellY == y) isRomFootprint = true;
                                        }
                                    }
                                    else
                                    {
                                        int xp = 200 - (x * 4) + (y * 8);
                                        int yp = 100 + (x * 4) + (y * 2) - activeCity.Heights[x, y];

                                        foreach (var raw in mockList)
                                        {
                                            int footprintX = raw.HorizontalPosition + 112;
                                            int footprintY = raw.VerticalPosition - 28 - raw.BottomPosition;
                                            if (footprintX == xp && footprintY == yp) isRomFootprint = true;
                                        }
                                    }

                                    int h = activeCity.Heights[x, y];

                                    if (isPremapped && isRomFootprint) rowLine.Append(" M  ");
                                    else if (isPremapped) rowLine.Append(" O  ");
                                    else if (isRomFootprint) rowLine.Append(" R  ");
                                    else if (h == 0) rowLine.Append("  . ");
                                    else rowLine.Append($" {h:D2} ");
                                }
                                writer.WriteLine(rowLine.ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Terrain dump failed: {ex.Message}");
                    }

                    // --- 2. NEW DETACHED PASSIVE FILE IMPORT VERIFIER LOG ---
                    try
                    {
                        string importDebugFilename = $"file_import_stage_{currentRoom:D2}.txt";
                        string importDebugPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, importDebugFilename);

                        string searchPattern = $"Diagnostic_Dump_Stage_{currentRoom:D2}_*.txt";
                        string[] matchingFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, searchPattern);

                        using (StreamWriter debugWriter = new StreamWriter(importDebugPath, false, Encoding.UTF8))
                        {
                            debugWriter.WriteLine("================================================================================");
                            debugWriter.WriteLine($"=== DEFENSIVE FILE IMPORT AUDIT REPORT: STAGE {currentRoom:D2} ===");
                            debugWriter.WriteLine($"Time of execution pass: {DateTime.Now}");
                            debugWriter.WriteLine("================================================================================");
                            debugWriter.WriteLine($" Target Directory Scan: {AppDomain.CurrentDomain.BaseDirectory}");
                            debugWriter.WriteLine($" Pattern Search Target: {searchPattern}");

                            bool fileExists = matchingFiles.Length > 0;
                            debugWriter.WriteLine($" Source file physically exists on disk: {(fileExists ? "YES (Success)" : "NO (Failed)")}");

                            if (fileExists)
                            {
                                debugWriter.WriteLine($" Found exact filename matching pattern: {Path.GetFileName(matchingFiles[0])}");
                                List<(int RowX, int ColY)> parsedCoords = LoadVerifiedCoordsFromDisk(currentRoom);

                                debugWriter.WriteLine($" Total coordinate positions successfully parsed: {parsedCoords.Count}");
                                for (int i = 0; i < parsedCoords.Count; i++)
                                {
                                    debugWriter.WriteLine($"  * Match Index [{i}]: Mapped to absolute Grid RowX = {parsedCoords[i].RowX:D2}, ColY = {parsedCoords[i].ColY:D2}");
                                }
                            }
                            else
                            {
                                debugWriter.WriteLine("\n[ERROR] Diagnostic reader can't find source text layout sheet.");
                            }
                            debugWriter.WriteLine("================================================================================");
                        }
                        Console.Beep(1800, 100);
                    }
                    catch (Exception) { }
                }

                // ------------------------------------------------------------
                // 1) RESOLVE ON-SCREEN GRID COORDINATES & MOUSE CLICK MECHANICS
                // ------------------------------------------------------------
                int mousePixelX = Raylib.GetMouseX();
                int mousePixelY = Raylib.GetMouseY();

                // Inverse math matching your startX=100, startY=100, cellSize=36 footprints exactly
                int calculatedColY = (mousePixelX - 100) / 36;
                int calculatedRowX = (mousePixelY - 100) / 36;

                string coordinateTelemetryString = "ROW (X): OUT  |  COL (Y): OUT";
                bool isMouseInsideGrid = (calculatedRowX >= 0 && calculatedRowX < 22 && calculatedColY >= 0 && calculatedColY < 22);

                if (isMouseInsideGrid)
                {
                    coordinateTelemetryString = $"ROW (X): {calculatedRowX:D2}  |  COL (Y): {calculatedColY:D2}";

                    // 2) INTEGRATE THE LEVEL EDITOR INTERACTION ENGINE
                    if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                    {
                        // STATE A: An elevator is currently grabbed. Relocate it and release the mouse lock.
                        if (_selectedElevatorIndex >= 0 && _selectedElevatorIndex < mockList.Count)
                        {
                            mockList[_selectedElevatorIndex].CellX = calculatedRowX;
                            mockList[_selectedElevatorIndex].CellY = calculatedColY;
                            mockList[_selectedElevatorIndex].IsMapped = true;

                            _selectedElevatorIndex = -1; // Release focus cleanly
                        }
                        // STATE B: Cursor is free. Scan for an existing yellow/orange elevator cell click hook.
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
               
                // --- ITEM 1: RENDER FIXED CO-ORDINATE DISPLAY PANEL ONSCREEN ---
                Raylib.DrawRectangle(100, 30, 380, 40, Color.DarkBlue);
                Raylib.DrawRectangleLines(100, 30, 380, 40, Color.White);
                Raylib.DrawText(coordinateTelemetryString, 120, 40, 20, Color.Lime);

                // ============================================================================
                // DIAGNOSTICCANVAS.CS - HUD INSTRUCTION TEXT UPDATE
                // ============================================================================
                Raylib.DrawText($"STAGE: {stageNames[currentRoom].ToUpper()}", 500, 30, 20, Color.Gold);

                // FIX: Updated user messaging instruction labels to match your new isolated key binding
                Raylib.DrawText("Click Lift to Select -> 2nd Click Moves It  |  S: Toggle Space View", 500, 55, 13, Color.LightGray);
                Raylib.DrawText("Press 'E' to Export Clean Diagnostic Reports and File Audits Safely", 500, 75, 13, Color.SkyBlue);


                int cellSize = 36;
                int startX = 100;
                int startY = 100;
                // Inside your while loop, right above the "for (int x = 0; x < 22; x++)" block:
                List<(int RowX, int ColY)> diskMappedElevators = LoadVerifiedCoordsFromDisk(currentRoom);
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
                            // Change layout background visualization color based on active mode
                            Color terrainColor = spaceMapView ? new Color(0, 30, 60, 255) : new Color(0, 50, 0, 255);
                            Raylib.DrawRectangle(posX + 2, posY + 2, cellSize - 4, cellSize - 4, terrainColor);
                        }

                        // --- PASSIVE DIRECT FILE OVERLAP OVERLAY PASS ---
                        bool existsInTextFile = false;
                        foreach (var coord in diskMappedElevators)
                        {
                            if (coord.RowX == x && coord.ColY == y)
                            {
                                existsInTextFile = true;
                                break;
                            }
                        }

                        // Map any entries found in the file onto the screen grid cleanly as Orange boxes
                        if (existsInTextFile)
                        {
                            Raylib.DrawRectangleLines(posX + 4, posY + 4, cellSize - 8, cellSize - 8, Color.Orange);
                            Raylib.DrawRectangleLines(posX + 5, posY + 5, cellSize - 10, cellSize - 10, Color.Yellow);
                        }

                        // ROM Footprints Layer configuration checks
                        bool drawRomDot = false;

                        if (spaceMapView)
                        {
                            foreach (var raw in mockList)
                            {
                                int cellX = raw.HorizontalPosition % 22;
                                int cellY = raw.VerticalPosition % 22;
                                if (cellX == x && cellY == y) drawRomDot = true;
                            }
                        }
                        else
                        {
                            int xp = 200 - (x * 4) + (y * 8);
                            int yp = 100 + (x * 4) + (y * 2) - h;

                            foreach (var raw in mockList)
                            {
                                int footprintX = raw.HorizontalPosition + 112;
                                int footprintY = raw.VerticalPosition - 28 - raw.BottomPosition;
                                if (footprintX == xp && footprintY == yp) drawRomDot = true;
                            }
                        }

                        if (drawRomDot)
                        {
                            Raylib.DrawRectangle(posX + 12, posY + 12, cellSize - 24, cellSize - 24, Color.Red);
                        }
                    }
                }

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
        /// <summary>
        /// Reads absolute coordinate tuples directly from hand-edited text files.
        /// Bypasses all engine variables and dictionary limitations.
        /// </summary>
        /// <summary>
        /// Scans your active directory for Diagnostic_Dump files, handles dynamic 
        /// stage names, and reads coordinates directly from the hand-edited text grid layout.
        /// </summary>
        private static List<(int RowX, int ColY)> LoadVerifiedCoordsFromDisk(int stageNum)
        {
            List<(int RowX, int ColY)> customCoords = new List<(int RowX, int ColY)>();
            string targetDir = AppDomain.CurrentDomain.BaseDirectory;

            try
            {
                // Find any file in your folder that begins with your specific stage prefix format rules
                string searchPattern = $"Diagnostic_Dump_Stage_{stageNum:D2}_*.txt";
                string[] matchingFiles = Directory.GetFiles(targetDir, searchPattern);

                // If no matching dump sheet exists yet for this wave, return empty list safely
                if (matchingFiles.Length == 0) return customCoords;

                string fullPath = matchingFiles[0]; // Isolate the first matching file found
                string[] lines = File.ReadAllLines(fullPath);

                // Start reading after line 4 to skip the header and legends text lines safely
                for (int x = 0; x < lines.Length; x++)
                {
                    string line = lines[x];
                    if (string.IsNullOrWhiteSpace(line) || line.Contains("===") || line.Contains("Mode") || line.Contains("[Legend")) continue;

                    // Split your 22-column space-separated text characters cleanly
                    string[] tokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    // Track row coordinates processing index
                    int currentRowX = customCoords.Count / 22;

                    for (int currentColY = 0; currentColY < tokens.Length && currentColY < 22; currentColY++)
                    {
                        string token = tokens[currentColY].Trim();

                        // O = Premapper Box, M = Perfect Match, R = ROM Footprint
                        // If you edited a square to 'O' or 'M' by hand, extract it instantly!
                        if (token == "O" || token == "M")
                        {
                            customCoords.Add((x - 4, currentColY)); // Offset header lines index to track 0-21 grid bounds
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Defensive catch boundary to keep frame rendering loops completely stable
            }

            return customCoords;
        }
    }

}