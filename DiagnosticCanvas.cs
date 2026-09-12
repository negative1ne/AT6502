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

                // --- LIVE D-KEY RE-WRITTEN INTERFACE TO PRINT VISUAL AND SPACE DETAILS ---
                if (Raylib.IsKeyPressed(KeyboardKey.D))
                {
                    try
                    {
                        string currentStageName = stageNames[currentRoom];
                        string filename = $"Diagnostic_Dump_Stage_{currentRoom:D2}_{currentStageName.Replace(" ", "_")}.txt";
                        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), filename);

                        using (StreamWriter writer = new StreamWriter(fullPath, false, Encoding.UTF8))
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
                                        // FLAT SPACE VIEW MAP: Look straight at the un-rotated memory arrays!
                                        foreach (var raw in mockList)
                                        {
                                            // Intercept raw memory array bytes indices cleanly
                                            int cellX = raw.HorizontalPosition % 22;
                                            int cellY = raw.VerticalPosition % 22;
                                            if (cellX == x && cellY == y) isRomFootprint = true;
                                        }
                                    }
                                    else
                                    {
                                        // SCREEN PROJECTION VIEW MAP
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
                        Console.Beep(1800, 200);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to write layout dump sheet: {ex.Message}");
                    }
                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

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
                            // Change layout background visualization color based on active mode
                            Color terrainColor = spaceMapView ? new Color(0, 30, 60, 255) : new Color(0, 50, 0, 255);
                            Raylib.DrawRectangle(posX + 2, posY + 2, cellSize - 4, cellSize - 4, terrainColor);
                        }

                        // Premapper data overlay box layer configuration
                        bool isPremappedCell = false;
                        foreach (var ev in mockList)
                        {
                            if (ev.IsMapped && ev.CellX == x && ev.CellY == y) isPremappedCell = true;
                        }

                        if (isPremappedCell)
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
    }
}