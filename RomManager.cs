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

            // Centralized v0.91 Session Summary Header Initialization
            if (IsParserDiagnosticActive)
            {
                string unifiedSessionLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "session_audit.log");
                try
                {
                    // Create a fresh file (false) at boot to establish our unified dashboard header
                    using (StreamWriter headerWriter = new StreamWriter(unifiedSessionLog, false, Encoding.UTF8))
                    {
                        headerWriter.WriteLine("================================================================================");
                        headerWriter.WriteLine("=== CRYSTAL CASTLES UNIFIED INGESTION SUITE ISOLATED DIAGNOSTIC SESSION LOG ===");
                        headerWriter.WriteLine($"=== ENGINE ARCHITECTURE: v0.91 SPECIFICATION  |  STATUS: ANALYSIS RUN      ===");
                        headerWriter.WriteLine("================================================================================");
                        headerWriter.WriteLine($"[SESSION LAUNCH] : {DateTime.Now:MM/dd/yyyy hh:mm:ss tt}");
                        headerWriter.WriteLine($"[SANDBOX BIN]    : {AppDomain.CurrentDomain.BaseDirectory}");
                        headerWriter.WriteLine($"[AUDIT VERDICT]  : OVERRUN GUARD FILTERS LIVE. DIRECT MEMORY TRACE ACTIVE.");
                        headerWriter.WriteLine("================================================================================\n");
                    }
                }
                catch { /* Prevent disk write access locks */ }
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

                // --- v0.91 Isolated Parallel Ingestion Test Leg ---
                // Live Path: Feeds the visible display variables safely from production records
                ingestedProfile = CrystalCastles.DataEngine.CCUnifiedParser.LoadUnifiedStageFile(fullUnifiedPath, null);

                if (IsParserDiagnosticActive)
                {
                    // FIX BANNER: ESCAPE BIN CONTEXT ONLY FOR ISOLATED PARALLEL DIAGNOSTIC FILES [v0.91]
                    string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                    string projectRoot = Path.GetFullPath(Path.Combine(exeDir, "..", "..", "..", ".."));

                    // FIX BANNER: TARGET DETACHED REVISED SUBDIRECTORY LOCALLY TO PROTECT GRAPHICS [v0.91]
                    string revisedFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "revised");
                    string parallelTestPath = Path.Combine(revisedFolder, outFileName);
                    string unifiedSessionLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "session_audit.log");

                    using (StreamWriter parallelAuditWriter = new StreamWriter(unifiedSessionLog, true, Encoding.UTF8))
                    {
                        // Parse the local revised layout file and extract its profile properties safely
                        var diagnosticProfile = CrystalCastles.DataEngine.CCUnifiedParser.LoadUnifiedStageFile(parallelTestPath, parallelAuditWriter);

                        if (diagnosticProfile != null && diagnosticProfile.Heights != null)
                        {
                            // Bind the revised altitude arrays strictly to the isolated target buffer profile
                            ingestedProfile = diagnosticProfile;
                        }
                    }
                }
                else
                {
                    // Production Mode: Zero disk overhead, 100% direct-to-RAM ingestion
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

                // --- Replaces the old method calls right before return statement ---
                ElevatorPremapper.ApplyOverrides(stageNum, clonedRoom.Elevators);
                IsolatedStages.Add(clonedRoom);
            }

            // EXCISE: Removed old GenerateStartupLaboratoryLogs() trigger line completely

            return IsolatedStages;
        }
        
        // ====================================================================================
        // END OF SEGMENT 1
        // ====================================================================================


       
    }
}
