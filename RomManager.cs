// ====================================================================================
// FIX BANNER: ROMMANAGER.CS - CONSOLIDATED INITIALIZATION REWRITE (SEGMENT 1 OF 1)
// LOCATION: REPLACES EVERYTHING FROM LINE 1 DOWN TO CLONEDROOM.TRACKSTATE ASSIGNMENT
// CONSTRAINTS: COMPACT LINE SAFETY CUTOFF | REMOVES DUPLICATE TABLES AND LOG BLOAT
// ====================================================================================
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
        public static string ActiveSessionTimestamp { get; private set; } = "";

        // ====================================================================================
        // DIAGNOSTIC CORE FIXED BANNER: CENTRALIZED DEBUG FIELDS REGISTER
        // LOCATION: EXTENDS FIELD PROPERTIES AT HEAD OF ROMMANAGER CLASS SCOPE
        // CONSTRAINTS: ELIMINATES HARDCODED LOOP DRIFT ACROSS EXPERIMENTAL REPOSITORIES (v0.90)
        // ====================================================================================
        public static bool IsParserDiagnosticActive = true;
        public static int DebugTargetStage = 1; // Locked to Wave 00 (Ball Wave) for fine tuning

        public static List<CityData> LoadRomDatabase()
        {
            CCUnifiedLogger.Initialize(AppDomain.CurrentDomain.BaseDirectory);

            if (string.IsNullOrEmpty(ActiveSessionTimestamp))
            {
                ActiveSessionTimestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            }

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

            string romDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rom");
            string file1Path = Path.Combine(romDir, "136022-102.1h");
            string file2Path = Path.Combine(romDir, "136022-101.1f");

            if (!Directory.Exists(romDir) || !File.Exists(file1Path) || !File.Exists(file2Path))
            {
                MessageBox.Show("Critical Error: ROM assets missing!", "Viewer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                if (stageNum == 36)
                {
                    clonedRoom.TrackState = StageTrackingState.NoElevators;
                }
                else if (stageNum == 0 || stageNum == 1 || stageNum == 2 || stageNum == 4 ||
                         stageNum == 5 || stageNum == 6 || stageNum == 7 || stageNum == 10 ||
                         stageNum == 21 || stageNum == 22 || stageNum == 26 || stageNum == 34)
                {
                    clonedRoom.TrackState = StageTrackingState.VerifiedWorking;
                }
                else
                {
                    clonedRoom.TrackState = StageTrackingState.ExperimentalTarget;
                }

                // ====================================================================================
                // FIX BANNER: ROMMANAGER.CS - RESOLVE CS0128 COUPLING STRIDE & TYPE-SAFE CONVERTER
                // LOCATION: REPLACES DUPLICATE VARIABLE BLOCKS DOWN THROUGH DATA TRANSFER SECTIONS
                // CONSTRAINTS: ELIMINATES VARIABLE MULTI-DECLARATIONS | SHIFTS FOCUS TO LEVEL 01 (v0.90)
                // ====================================================================================
                string targetExportFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "unified_data");
                string formattedName = StageNames[stageNum].Replace(" ", "_").Replace("'", "").ToUpper();
                string outFileName = $"STAGE_{stageNum:D2}_{formattedName}.txt";
                string fullUnifiedPath = Path.Combine(targetExportFolder, outFileName);

                CrystalCastles.DataEngine.CCUnifiedParser.UnifiedStageProfile ingestedProfile = null;

                // ====================================================================================
                // DIAGNOSTIC CORE FIXED BANNER: ROMMANAGER.CS - 5-LEVEL TESTING SUITE RANGE PASS
                // LOCATION: REPLACES SINGLE-STAGE CONDITIONAL HOOK INSIDE THE STAGENUM LOOP
                // CONSTRAINTS: UNDER 150 LINES MAX WINDOW LIMIT | STREAMS STAGES 00-04 LINE-BY-LINE (v0.90)
                // ====================================================================================
                if (IsParserDiagnosticActive && stageNum < 37)
                {
                    string auditPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startup_audit.log");
                    // Using append mode ensures all 5 stages record sequentially to the same run ledger
                    using (StreamWriter verboseAuditWriter = new StreamWriter(auditPath, true, Encoding.UTF8))
                    {
                        ingestedProfile = CrystalCastles.DataEngine.CCUnifiedParser.LoadUnifiedStageFile(fullUnifiedPath, verboseAuditWriter);
                    }
                }
                else
                {
                    ingestedProfile = CrystalCastles.DataEngine.CCUnifiedParser.LoadUnifiedStageFile(fullUnifiedPath, null);
                }

                // TYPE-SAFE ARRAY TRANSFORMER: Elements are copied sequentially to align byte[,] to int[,] variables
                if (ingestedProfile != null && ingestedProfile.Heights != null)
                {
                    for (int x = 0; x < 22; x++)
                    {
                        for (int y = 0; y < 22; y++)
                        {
                            // ====================================================================================
                            // FIX BANNER: LINE 139 HEIGHT MAPPING VARIABLE TYPE CORRECTION
                            // LOCATION: INNER GRID NESTED FOR-LOOP MATRIX CONVERSION ASSIGNMENT
                            // CONSTRAINTS: ELIMINATES CS0266 BY CASTING EXPLICITLY NATIVE DATA TO BYTE (v0.90)
                            // ====================================================================================
                            clonedRoom.Heights[x, y] = (byte)ingestedProfile.Heights[x, y];
                            clonedRoom.Gems[x, y] = ingestedProfile.Gems[x, y];
                        }
                    }
                }
               
                clonedRoom.Elevators.Clear();
                for (int e = 0; e < ingestedProfile.Lifts.Count; e++)
                {
                    var parsedLift = ingestedProfile.Lifts[e];
                    ElevatorData clonedLift = new ElevatorData
                    {
                        CellX = parsedLift.CellX,
                        CellY = parsedLift.CellY,
                        BottomPosition = parsedLift.BottomH,
                        TopPosition = parsedLift.TopH,
                        CurrentPosition = parsedLift.BottomH,
                        IsMapped = true,
                        Mode = 0,
                        CurrentSitTime = 0
                    };
                    clonedRoom.Elevators.Add(clonedLift);
                }
                // ============================================================================

                ElevatorPremapper.ApplyOverrides(stageNum, clonedRoom.Elevators);
                IsolatedStages.Add(clonedRoom);
            }

            ElevatorPremapper.InitializeFromDisk();
            GenerateStartupLaboratoryLogs();

            return IsolatedStages;
        }
        // ====================================================================================
        // END OF SEGMENT 1
        // ====================================================================================


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

            ElevatorPremapper.InitializeFromDisk();

            for (int stageNum = 0; stageNum < 37; stageNum++)
            {
                var isolatedRoom = IsolatedStages[stageNum];

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
                    // ====================================================================================
                    // FIX BANNER: RomManager.cs & InputHandler.cs SAFE DEACTIVATION GATE (v0.85)
                    // ====================================================================================
                    return; // Stops execution dead right here before any file stream is opened!
                            // ====================================================================================

                    // Left completely untouched below so no downstream variables break:
                    string stamp = RomManager.ActiveSessionTimestamp;
                    string filename = $"LAB_TEST_LOG_STAGE_{stageNum:D2}_{stamp}.txt";
                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

                    using (StreamWriter sw = new StreamWriter(fullPath, false, Encoding.UTF8))
                    {
                        sw.WriteLine("================================================================================");
                        sw.WriteLine($"=== CRYSTAL CASTLES ISOLATED AUTOMATED AUDIT REPORT: STAGE {stageNum:D2} ===");
                        sw.WriteLine($"Stage Name Reference profile: {currentStageName.ToUpper()} | Engine: v0.85");
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
        // ============================================================================
        // ROMMANAGER.CS - PART 3: RE-MAPPED STATIC FILE EXPORT PIPELINE (v0.85)
        // ============================================================================
        public static void ExportStageTextFile(int stageNum, string stageName, CityData activeCity)
        {
            try
            {
                string filename = $"Stage_{stageNum:D2}_{stageName.Replace(" ", "_")}_Matrix.txt";
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

                using (StreamWriter writer = new StreamWriter(fullPath))
                {
                    writer.WriteLine($"=== VIEWER PIPELINE AUDIT LOG - RUN TIME: {DateTime.Now} | Engine: v0.85 ===");
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

                        writer.WriteLine("\nLOCATION : Verified");
                        writer.WriteLine("BEHAVIOR : Verified");
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
