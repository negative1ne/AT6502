// ============================================================================
// ROMMANAGER.CS - COMPLETE COMPONENT UPGRADE (37-STAGE DEEP COPY FACTORY)
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
        // STEP 3: Retain the old 16-room structure exactly as-is to preserve working viewports
        public static List<CityData> BaseCities = new List<CityData>();

        // STEP 1: Implement the pristine, isolated 37-stage deep-copy database array container
        public static List<CityData> IsolatedStages = new List<CityData>();

        public static List<CityData> LoadRomDatabase()
        {
            string romDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rom");
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

            // 1. Populate the old 16 base cities natively from the file layout bytes
            BaseCities.Clear();
            for (int i = 0; i < 16; i++)
            {
                CityData city = new CityData();
                city.Load(combinedData, i * 0x400);
                BaseCities.Add(city);
            }

            // Master arcade hardcoded wave matching array matrix pointers
            byte[] RoomToCityMap = new byte[] {
                0x00, 0x02, 0x09, 0xC3, 0x46, 0x71, 0x0C, 0xC7,
                0x06, 0x0D, 0x45, 0xCB, 0x04, 0x0A, 0x06, 0x4F,
                0x41, 0x4D, 0x3C, 0xC3, 0x0A, 0x02, 0x32, 0x3F,
                0x01, 0x04, 0x75, 0xFB, 0x01, 0x3A, 0x06, 0xF7,
                0x08, 0x7D, 0x05, 0xCB, 0x0E
            };

            // ============================================================================
            // ROMMANAGER.CS - FRAGMENT 1: AUTO-TRI-STATE FILTER CLASSIFICATION ENGINE
            // ============================================================================
            // 2. STAGE SETUP & DEEP COPY: Allocate 37 completely separate, distinct room containers
            IsolatedStages.Clear();
            for (int stageNum = 0; stageNum < 37; stageNum++)
            {
                int parentCityIndex = RoomToCityMap[stageNum] & 0x0F;
                CityData parentCity = BaseCities[parentCityIndex];

                // Create a completely detached, un-linked object container instance in RAM
                CityData clonedRoom = new CityData();
                clonedRoom.NumElevators = parentCity.NumElevators;

                // --- INJECT TRI-STATE GATING SWITCHBOARD FILTER RULES ---
                // Boundary Group A: NO ELEVATORS CONFIGURATIONS
                if (stageNum == 2 || stageNum == 6 || stageNum == 7 || stageNum == 13 ||
                    stageNum == 15 || stageNum == 18 || stageNum == 20 || stageNum == 23 ||
                    stageNum == 29 || stageNum == 31 || stageNum == 36)
                {
                    clonedRoom.TrackState = StageTrackingState.NoElevators;
                }
                // Boundary Group B: VERIFIED WORKING (Freeze and lock completely under legacy rules!)
                else if (stageNum == 0 || stageNum == 1 || stageNum == 10 ||
                         stageNum == 21 || stageNum == 22 || stageNum == 26 || stageNum == 34)
                {
                    clonedRoom.TrackState = StageTrackingState.VerifiedWorking;
                }
                // Boundary Group C: EXPERIMENTAL TEST TARGETS (Isolate height calibrations here)
                else
                {
                    clonedRoom.TrackState = StageTrackingState.ExperimentalTarget;
                }

                // ============================================================================
                // ROMMANAGER.CS - FRAGMENT 2: DEEP COPY MEMORY UNLINKING & BOOT TELESCOPES
                // ============================================================================
                // Deep-copy byte matrices row-by-row to break memory pointer cross-talk references
                for (int x = 0; x < 22; x++)
                {
                    for (int y = 0; y < 22; y++)
                    {
                        clonedRoom.Heights[x, y] = parentCity.Heights[x, y];
                        clonedRoom.Attributes[x, y] = parentCity.Attributes[x, y];
                    }
                }

                // Isolate elevator configurations safely into pristine memory vectors
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

                IsolatedStages.Add(clonedRoom);
            }

            // Keep exactly ONE copy of the premapper initialization pass
            ElevatorPremapper.InitializeFromDisk();

            // Run our automated height verification text reporter pass cleanly
            GenerateStartupLaboratoryLogs();

            // PIPELINE INITIALIZATION AUDIT LOG: Print data structure states to our active session log
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\n================================================================================");
            sb.AppendLine("=== CC_FRAMEWORK_UPDATE: 37-STAGE DATA ISOLATION PIPELINE ENGINE INITIALIZED ===");
            sb.AppendLine($"Verification Check -> Total Rooms Registered in Isolated Cache: {IsolatedStages.Count}/37");
            sb.AppendLine("================================================================================");
            SessionLogger.LogVerificationMessage(sb.ToString());

            return BaseCities;

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
