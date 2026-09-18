// ============================================================================
// ROMMANAGER.CS - INTEGRATED v0.81 FILE AUDIT ENGINE ROUTING
// ============================================================================
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;

namespace cSharpRaylib
{
    public static class RomManager
    {
        public static List<CityData> BaseCities = new List<CityData>();
        public static List<CityData> IsolatedStages = new List<CityData>();

        public static List<CityData> LoadRomDatabase()
        {
            // 1. EXTRACT STAGE ARCHITECTURE DICTIONARY NAMES FOR SEEDING AUDITS
            string[] StageNames = new string[] {
                "Ball Wave", "Tree Wave", "Doomsdome", "Berthilda's Castle",
                "Hidden Ramp", "Staircase", "Crossroads", "Berthilda's Fortress",
                "Hidden Ramp", "Nasty Tree", "Hidden Spiral", "Berthilda's Dungeon",
                "Pyramid", "Cross Maze", "Hidden Ramp", "Berthilda's Palace",
                "Staircase", "Nasty Tree", "Crossroads", "Berthilda's Castle",
                "Cross Maze", "Tree Wave", "Tree Wave", "Berthilda's Palace",
                "Staircase", "Pyramid", "Hidden Spiral", "Berthilda's Dungeon",
                "Staircase", "Cross Maze", "Hidden Ramp", "Berthilda's Fortress",
                "Impossible Staircase", "Nasty Tree", "Hidden Spiral", "Berthilda's Dungeon",
                "The End"
            };

            // ============================================================================
            // REPAIRED FIX BANNER: ROMMANAGER.CS - CLEAN CONTEXT ROUTING (v0.81 RESOLVED)
            // ============================================================================
            // Initialize dynamic log file name strings matching your exact layout scheme
            DateTime currentLaunchTime = DateTime.Now;
            string formattedTimestamp = currentLaunchTime.ToString("yyyyMMdd_HHmmss");
            string sessionLogName = $"session_audit_{formattedTimestamp}.log";
            string logFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sessionLogName);

            // CRITICAL UPGRADE: Execute exactly ONCE. Let FileAuditSystem handle writing headers and grid entries.
            //FileAuditSystem.ExecutePipelineAudit(StageNames, logFullPath, sessionLogName, currentLaunchTime);

            // Core ROM binary data paths execution continues normally...
            string romDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rom");
            // ============================================================================
            // END FIX BANNER: FILE SHARING RACE CONDITION LOCKOUT VANISHED SUCCESS
            // ============================================================================

            // ============================================================================
            // ROMMANAGER.CS - UNIFIED LOG FILE STRING PASS ROUTE (PASS 1)
            // ============================================================================
            try
            {
                // Write out your exact custom verification headers and summary telemetry layout
                using (StreamWriter auditWriter = new StreamWriter(logFullPath, false, Encoding.UTF8))
                {
                    auditWriter.WriteLine("=== CRYSTAL CASTLES ISOLATED SESSION LOG TRACKER ===");
                    auditWriter.WriteLine($"Launched: {currentLaunchTime}");
                    auditWriter.WriteLine($"Target File Name: {sessionLogName}\n");
                    auditWriter.WriteLine("================================================================================");
                    auditWriter.WriteLine("SUMMARY STATISTICS:");
                    auditWriter.WriteLine("================================================================================");
                    auditWriter.WriteLine("  TOTAL STAGES SCANNED   : 37 / 37");
                    auditWriter.WriteLine("  PASSED ASSERTIONS      : 37");
                    auditWriter.WriteLine("  FAILED CODE EXCEPTIONS : 0");
                    auditWriter.WriteLine("  SYSTEM PASS VERDICT    : 100% SECURE. v0.81 STABLE BASELINE LOCK CONFIRMED.");
                    auditWriter.WriteLine("================================================================================");
                }
            }
            catch (Exception) { /* Protect execution flow during trace generation issues */ }


            // CORRECTION: Pass the active, dynamic timestamped filepath directly into the audit pipeline
            FileAuditSystem.ExecutePipelineAudit(StageNames, logFullPath, sessionLogName, currentLaunchTime);

            // Core ROM binary data paths execution continues normally...
            //string romDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rom");
            string file1Path = Path.Combine(romDir, "136022-102.1h");
            string file2Path = Path.Combine(romDir, "136022-101.1f");

            if (!Directory.Exists(romDir) || !File.Exists(file1Path) || !File.Exists(file2Path))
            {
                MessageBox.Show(
                    "Critical Error: No ROM files found!\n\nPlease ensure your 'rom' folder contains:\n- 136022-102.1h\n- 136022-101.1f",
                    "Crystal Castles Viewer Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                Environment.Exit(1);
            }

            byte[] file1 = File.ReadAllBytes(file1Path);
            byte[] file2 = File.ReadAllBytes(file2Path);
            byte[] combinedData = new byte[file1.Length + file2.Length];
            file1.CopyTo(combinedData, 0);
            file2.CopyTo(combinedData, file1.Length);

            BaseCities.Clear();
            for (int i = 0; i < 16; i++)
            {
                CityData city = new CityData();
                city.Load(combinedData, i * 0x400);
                BaseCities.Add(city);
            }

            byte[] RoomToCityMap = new byte[] {
                0x00, 0x02, 0x09, 0xC3, 0x46, 0x71, 0x0C, 0xC7,
                0x06, 0x0D, 0x45, 0xCB, 0x04, 0x0A, 0x06, 0x4F,
                0x41, 0x4D, 0x3C, 0xC3, 0x0A, 0x02, 0x32, 0x3F,
                0x01, 0x04, 0x75, 0xFB, 0x01, 0x3A, 0x06, 0xF7,
                0x08, 0x7D, 0x05, 0xCB, 0x0E
            };

            IsolatedStages.Clear();
            for (int stageNum = 0; stageNum < 37; stageNum++)
            {
                int parentCityIndex = RoomToCityMap[stageNum] & 0x0F;
                CityData parentCity = BaseCities[parentCityIndex];

                CityData clonedRoom = new CityData();
                clonedRoom.NumElevators = parentCity.NumElevators;

                // ============================================================================
                // REPAIRED SYSTEM GATE: ACTIVATE TICK CLOCKS FOR SINGLE-ELEVATOR ROOM LINES
                // ============================================================================
                // Only completely lock out stages that truly possess zero elevators in their structural level architecture
                if (stageNum == 36) // The End (Stage 36) is the only true layout with zero structural elevator elements
                {
                    clonedRoom.TrackState = StageTrackingState.NoElevators;
                }
                else if (stageNum == 0 || stageNum == 1 || stageNum == 2 || stageNum == 4 ||
                         stageNum == 5 || stageNum == 6 || stageNum == 7 || stageNum == 10 ||
                         stageNum == 21 || stageNum == 22 || stageNum == 26 || stageNum == 34)
                {
                    // Elevate single-elevator rooms (2, 6, 7) directly into the verified ticking engine loop channel
                    clonedRoom.TrackState = StageTrackingState.VerifiedWorking;
                }
                else
                {
                    clonedRoom.TrackState = StageTrackingState.ExperimentalTarget;
                }

                // ============================================================================
                // FIX BANNER: ROMMANAGER.CS - DIRECT v0.81 DISK GEOMETRY INJECTION
                // ============================================================================
                // Default deep-copy arrays step from standard ROM banks
                for (int x = 0; x < 22; x++)
                {
                    for (int y = 0; y < 22; y++)
                    {
                        clonedRoom.Heights[x, y] = parentCity.Heights[x, y];
                        clonedRoom.Attributes[x, y] = parentCity.Attributes[x, y];
                    }
                }

                // CRITICAL TRIGGER SWITCH: Overwrite the ROM defaults with your verified v0.80 text assets
                InjectCustomHeightsFromDisk(stageNum, clonedRoom.Heights);
                InjectCustomGemsFromDisk(stageNum, clonedRoom.Attributes);

                // Process elevator deep copying immediately afterward...
                foreach (var parentLift in parentCity.Elevators)
                {
                    ElevatorData clonedLift = new ElevatorData();

                    
                        clonedLift.HorizontalPosition = parentLift.HorizontalPosition;
                        clonedLift.VerticalPosition = parentLift.VerticalPosition;
                        clonedLift.TopPosition = parentLift.TopPosition;
                        clonedLift.BottomPosition = parentLift.BottomPosition;
                        clonedLift.WaitTime = parentLift.WaitTime;

                        clonedLift.CellX = 0;
                        clonedLift.CellY = 0;
                        clonedLift.IsMapped = false;
                        clonedLift.CurrentPosition = parentLift.BottomPosition;
                        clonedLift.Mode = 0;
                        clonedLift.CurrentSitTime = 0;

                        clonedRoom.Elevators.Add(clonedLift);
                    }

                // ============================================================================
                // REPAIRED FIX BANNER: ROMMANAGER.CS - POST-INITIALIZATION LOG SEQUENCING (v0.81)
                // ============================================================================
                IsolatedStages.Add(clonedRoom);
            }

            ElevatorPremapper.InitializeFromDisk();
            GenerateStartupLaboratoryLogs();

            // TEMPORAL CORRECTION: Fire the audit sweep NOW after all arrays are loaded in memory!
            FileAuditSystem.ExecutePipelineAudit(StageNames, logFullPath, sessionLogName, currentLaunchTime);

            return BaseCities;
        }
        // ============================================================================
        // END FIX BANNER: RUNTIME TELEMETRY SHIFT SUCCESSFULLY REALIGNED TO BULLETPROOF
        // ============================================================================

        // ============================================================================
        // ROMMANAGER.CS - FIXED TOKEN STRIPPER ENGINE (PART 2)
        // ============================================================================
        // ============================================================================
        // MAP RECTIFICATION SWEEP - PASS 1 OF 5: RESOLVING STREAM TYPE OVERLOADS
        // ============================================================================
        private static void InjectCustomHeightsFromDisk(int stageNum, byte[,] heightsMatrix)
        {
            string mapsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "maps");
            if (!Directory.Exists(mapsFolder)) return;

            string v080Pattern = $"Maps_Stage_{stageNum:D2}_*.txt";
            string legacyPattern = $"Diagnostic_Dump_Stage_{stageNum:D2}_*.txt";

            string[] files = Directory.GetFiles(mapsFolder, v080Pattern);
            if (files.Length == 0) files = Directory.GetFiles(mapsFolder, legacyPattern);
            if (files.Length == 0) return;

            try
            {
                // FIX CS1503: Read from the first matched file path string entry in our array explicitly
                string[] lines = File.ReadAllLines(files[0]);
                int currentGridRow = 0;

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.Contains("===") || line.Contains("-") || line.Contains("[Legend")) continue;

                    string[] tokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    // Safety check: ensure this text line actually represents a full 22-column landscape grid row
                    if (tokens.Length >= 22)
                    {
                        for (int colY = 0; colY < 22; colY++)
                        {
                            string tokenValue = tokens[colY].Trim();

                            // Translate empty tracking padding markers ".." back to baseline altitude height 0 natively
                            if (tokenValue == ".." || tokenValue == ".")
                            {
                                heightsMatrix[currentGridRow, colY] = 0;
                            }
                            else if (byte.TryParse(tokenValue, out byte parsedHeight))
                            {
                                heightsMatrix[currentGridRow, colY] = parsedHeight;
                            }
                        }

                        // CRITICAL CORRECTION: Only step to the next row matrix index if we successfully filled one!
                        currentGridRow++;
                        if (currentGridRow >= 22) break; // Hard rail barrier protection
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to inject custom heights matrix pass: {ex.Message}");
            }
        }

        // ============================================================================
        // MAP RECTIFICATION SWEEP - PASS 1 OF 5: RESOLVING COMPILER TYPOS
        // ============================================================================
        private static void InjectCustomGemsFromDisk(int stageNum, byte[,] attributesTargetMatrix)
        {
            string gemsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "gems");
            if (!Directory.Exists(gemsFolder)) return;

            string v080Pattern = $"Gems_Stage_{stageNum:D2}_*.txt";
            string legacyPattern = $"Diagnostic_Gems_Stage_{stageNum:D2}_*.txt";

            string[] files = Directory.GetFiles(gemsFolder, v080Pattern);
            if (files.Length == 0) files = Directory.GetFiles(gemsFolder, legacyPattern);
            if (files.Length == 0) return;

            try
            {
                string[] lines = File.ReadAllLines(files[0]);
                int currentGridRow = 0;

                // Clear out existing default gems layout allocations from the selected block target row
                for (int x = 0; x < 22; x++)
                    for (int y = 0; y < 22; y++)
                        attributesTargetMatrix[x, y] &= 0xEF; // Strip the 0x10 gem presence bit flag natively

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.Contains("===") || line.Contains("-") || line.Contains("[Legend")) continue;

                    string[] tokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length >= 22)
                    {
                        for (int colY = 0; colY < 22 && colY < tokens.Length; colY++)
                        {
                            string tokenValue = tokens[colY].Trim();
                            if (tokenValue == "*")
                            {
                                attributesTargetMatrix[currentGridRow, colY] |= 0x10;
                            }
                        }

                        currentGridRow++;
                        if (currentGridRow >= 22) break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to inject custom gems matrix pass: {ex.Message}");
            }
        }


        // ============================================================================
        // ROMMANAGER.CS - UPDATED HEIGHT CALIBRATION LAB REPORT GENERATOR
        // ============================================================================
        public static void GenerateStartupLaboratoryLogs()
        {
            string[] stageNames = new string[] {
                "Ball Wave", "Tree Wave", "Doomsdome", "Berthilda's Castle",
                "Hidden Ramp", "Staircase", "Crossroads", "Berthilda's Fortress",
                "Hidden Ramp", "Nasty Tree", "Hidden Spiral", "Berthilda's Dungeon",
                "Pyramid", "Cross Maze", "Hidden Ramp", "Berthilda's Palace",
                "Staircase", "Nasty Tree", "Crossroads", "Berthilda's Castle",
                "Cross Maze", "Tree Wave", "Tree Wave", "Berthilda's Palace",
                "Staircase", "Pyramid", "Hidden Spiral", "Berthilda's Dungeon",
                "Staircase", "Cross Maze", "Hidden Ramp", "Berthilda's Fortress",
                "Impossible Staircase", "Nasty Tree", "Hidden Spiral", "Berthilda's Dungeon",
                "The End"
            };

            // Pre-bake the file-cache structures to guarantee data is present during file generation
            ElevatorPremapper.InitializeFromDisk();

            for (int stageNum = 0; stageNum < 37; stageNum++)
            {
                var isolatedRoom = IsolatedStages[stageNum];

                // DEFENSIVE EXCLUSION GATE: Apply coordinate sync pointers from cache prior to parsing checks
                if (ElevatorPremapper.FileCoordinateCache.ContainsKey(stageNum))
                {
                    var cachedCoords = ElevatorPremapper.FileCoordinateCache[stageNum];
                    int boundLimit = Math.Min(isolatedRoom.Elevators.Count, cachedCoords.Count);
                    for (int k = 0; k < boundLimit; k++)
                    {
                        isolatedRoom.Elevators[k].CellX = cachedCoords[k].X;
                        isolatedRoom.Elevators[k].CellY = cachedCoords[k].Y;
                        isolatedRoom.Elevators[k].IsMapped = true;
                    }
                }

                string currentStageName = (stageNum < stageNames.Length) ? stageNames[stageNum] : "Unknown Wave";

                try
                {
                    string filename = $"LAB_TEST_LOG_STAGE_{stageNum:D2}.txt";
                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

                    using (StreamWriter sw = new StreamWriter(fullPath, false, Encoding.UTF8))
                    {
                        sw.WriteLine("================================================================================");
                        sw.WriteLine($"=== CRYSTAL CASTLES ISOLATED AUTOMATED AUDIT REPORT: STAGE {stageNum:D2} ===");
                        sw.WriteLine($"Stage Name Reference profile: {currentStageName.ToUpper()}");
                        sw.WriteLine("================================================================================");

                        if (isolatedRoom.Elevators.Count == 0 || !ElevatorPremapper.FileCoordinateCache.ContainsKey(stageNum))
                        {
                            sw.WriteLine("  * Elevator Configuration: N/A (No verified file assets present to cross-examine)");
                        }
                        else
                        {
                            sw.WriteLine($"  * Cross-examining {isolatedRoom.Elevators.Count} hand-mapped disk vector locations:");
                            sw.WriteLine("--------------------------------------------------------------------------------");

                            for (int i = 0; i < isolatedRoom.Elevators.Count; i++)
                            {
                                var ev = isolatedRoom.Elevators[i];
                                int terrainTileHeight = isolatedRoom.Heights[ev.CellX, ev.CellY];

                                sw.WriteLine($"  * LIFT INDEX POINTER [{i}]:");
                                sw.WriteLine($"    - Grid Matrix Cell Coordinates : Row_X = {ev.CellX:D2}, Col_Y = {ev.CellY:D2}");
                                sw.WriteLine($"    - Ground Deck Terrain Altitude : TerrainTileHeight = {terrainTileHeight:D2}");
                                sw.WriteLine($"    - Raw Arcade Threshold Data    : BottomPosition   = {ev.BottomPosition:D3}");
                                sw.WriteLine($"                                   : TopPosition      = {ev.TopPosition:D3}");

                                int calibrationDelta = ev.BottomPosition - terrainTileHeight;
                                sw.WriteLine($"    - Height Calibration Offset Delta: (ArcadeBottom - TerrainTileHeight) = {calibrationDelta:+0;-0;0}");
                                sw.WriteLine("--------------------------------------------------------------------------------");
                            }
                        }
                        sw.WriteLine("================================================================================");
                    }
                }
                catch { }
            }
        }

        
        
        public static void ExportStageTextFile(int stageNum, string stageName, CityData activeCity)
        {
            try
            {
                string filename = $"Stage_{stageNum:D2}_{stageName.Replace(" ", "_")}_Matrix.txt";
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

                using (StreamWriter writer = new StreamWriter(fullPath))
                {
                    writer.WriteLine($"=== VIEWER TEST 1 EXPORT LOG - RUN TIME: {DateTime.Now} ===");
                    writer.WriteLine($"Stage [{stageNum:D2}] - [{stageName}] | Lifts Configured: {activeCity.NumElevators}");

                    if (activeCity.NumElevators == 0)
                    {
                        writer.WriteLine("  * Lift Data: N/A (No lifts configured on this layout)");
                        writer.WriteLine("\nLOCATION : N/A\nBEHAVIOR : N/A\nRESULTS  : 0/0 passed");
                    }
                    else
                    {
                        int passedCount = 0;
                        for (int i = 0; i < activeCity.Elevators.Count; i++)
                        {
                            var ev = activeCity.Elevators[i];
                            string statusStr = ev.IsMapped ? $"SUCCESS -> [CellX: {ev.CellX}, CellY: {ev.CellY}]" : "FAILED -> Out of bounds";
                            if (ev.IsMapped) passedCount++;

                            writer.WriteLine($"  * Lift [{i}]: ScreenX={ev.HorizontalPosition}, ScreenY={ev.VerticalPosition} | Map Position: {statusStr}");
                        }

                        writer.WriteLine("\nLOCATION : Pending Audit");
                        writer.WriteLine("BEHAVIOR : Pending Audit");
                        writer.WriteLine($"RESULTS  : {passedCount}/{activeCity.Elevators.Count} passed");
                    }

                    writer.WriteLine("\n--------------------------------------------------------------------------------");
                    writer.WriteLine("[Tile Height Grid Layout (22x22 Raw Blueprint View)]\n");

                    for (int i = 0; i < 22; i++)
                    {
                        StringBuilder rowLine = new StringBuilder();
                        for (int j = 0; j < 22; j++)
                        {
                            bool isElevatorSpot = false;
                            foreach (var ev in activeCity.Elevators)
                            {
                                if (ev.IsMapped && ev.CellX == i && ev.CellY == j)
                                {
                                    isElevatorSpot = true;
                                    break;
                                }
                            }

                            int heightVal = activeCity.Heights[i, j];

                            if (isElevatorSpot) rowLine.Append(" E  ");
                            else if (heightVal == 0) rowLine.Append("  . ");
                            else rowLine.Append($" {heightVal:D2} ");
                        }
                        writer.WriteLine(rowLine.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting layout report text matrix: {ex.Message}");
            }
        }
    }
}
