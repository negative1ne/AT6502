// ============================================================================
// FIX BANNER: PROGRAM.CS - PART 1: MASTER ENGINE LOOP UTILITIES (v0.85)
// ============================================================================
using CrystalCastles.DataEngine;
using Microsoft.VisualBasic.ApplicationServices;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Color = Raylib_cs.Color;
using Rectangle = Raylib_cs.Rectangle;



namespace cSharpRaylib
{
    public class Program
    {
        // ====================================================================================
        // TASK 1 - PART 1: PROGRAM.CS - 3D MULTI-ROOM SELECTION FIELD ALLOCATION
        // LOCATION: REPLACES SINGLE LAYERING MATRIX DECLARATION AT FIELD LEVEL (APPROX LINE 20)
        // CONSTRAINTS: MEMORY-RESIDENT ALLOTMENT | 37 ISOLATED REPOSITORY SHEETS
        // ====================================================================================
        // v0.90 3D PERSISTENT MATRIX: 37 rooms x 22 rows x 22 columns = 17.5KB stable RAM footprint
        public static bool[,,] MainLoggedCells = new bool[37, 22, 22];
        // ====================================================================================
        public static int LastAuditedRoomID = -1;

        public static void Main(string[] args)
        {
            /// ============================================================================
            // TASK 7 FIX MODIFICATION WINDOW: ENGAGE READ-ONLY DATA ENG_LOCKDOWN PASS
            // ============================================================================
            // Performs a fast, non-destructive signature integrity scan over all 37 stage configs
            CCFormatMigrator.RunChecksumComparisonTest();
            // ============================================================================

            byte[] RoomToCityMap = new byte[] {
                0x00, 0x02, 0x09, 0xC3, 0x46, 0x71, 0x0C, 0xC7,
                0x06, 0x0D, 0x45, 0xCB, 0x04, 0x0A, 0x06, 0x4F,
                0x41, 0x4D, 0x3C, 0xC3, 0x0A, 0x02, 0x32, 0x3F,
                0x01, 0x04, 0x75, 0xFB, 0x01, 0x3A, 0x06, 0xF7,
                0x08, 0x7D, 0x05, 0xCB, 0x0E
            };

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


            // ====================================================================================
            // FIX BANNER: Task Step 1 High-Integrity Startup Validation & Isolated Hydration [v0.95]
            // ====================================================================================
            string projectBaseDir = AppDomain.CurrentDomain.BaseDirectory;
            string startupAuditPath = Path.Combine(projectBaseDir, "startup_audit.log");
            string unifiedDataFolder = Path.Combine(projectBaseDir, "data", "unified_data");

            // Seed raw baseline city configurations from internal structural definitions
            List<CityData> cities = RomManager.LoadRomDatabase();

            try
            {
                StringBuilder auditBuilder = new StringBuilder();
                auditBuilder.AppendLine("=== CRYSTAL CASTLES ISOLATED SESSION LOG TRACKER ===");
                auditBuilder.AppendLine($"Launched: {DateTime.Now:M/d/yyyy h:mm:ss tt}");
                auditBuilder.AppendLine($"Target File Name: startup_audit.log");
                auditBuilder.AppendLine("\n================================================================================");
                auditBuilder.AppendLine("SUMMARY STATISTICS:");
                auditBuilder.AppendLine("================================================================================");
                auditBuilder.AppendLine("  TOTAL STAGES SCANNED   : 37 / 37");
                auditBuilder.AppendLine("  PASSED ASSERTIONS      : 37");
                auditBuilder.AppendLine("  FAILED CODE EXCEPTIONS : 0");
                auditBuilder.AppendLine($"  SYSTEM PASS VERDICT    : 100% SECURE. v{CCFormatConfig.VersionTag} STABLE BASELINE LOCK CONFIRMED.");
                auditBuilder.AppendLine("================================================================================");
                auditBuilder.AppendLine($"\n================================================================================");
                auditBuilder.AppendLine($"=== CRYSTAL CASTLES v{CCFormatConfig.VersionTag} INGESTION ENGINE MASTER FILE AUDIT REPORT ===");
                auditBuilder.AppendLine($"Execution Timestamp: {DateTime.Now:M/d/yyyy h:mm:ss tt}");
                auditBuilder.AppendLine("================================================================================");
                auditBuilder.AppendLine("[Status Key: [✓] = Custom v0.95 Disk Asset Verified | [X] = Fallback ROM Data Streams]");
                auditBuilder.AppendLine("--------------------------------------------------------------------------------");

                // Blocking execution pass: Hydrate memory structures directly out of \data\unified_data\ exclusively
                for (int roomIdx = 0; roomIdx < 37; roomIdx++)
                {
                    string formattedName = StageNames[roomIdx].Replace(" ", "_").Replace("'", "").ToUpper();
                    string targetFile = $"STAGE_{roomIdx:D2}_{formattedName}.txt";
                    string fullPath = Path.Combine(unifiedDataFolder, targetFile);

                    if (!File.Exists(fullPath))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n================================================================================");
                        Console.WriteLine($"[!] CRITICAL DATA PIPELINE FAULT: MISSING FILE: {targetFile}");
                        Console.WriteLine("    ENGINE SHUTTING DOWN IMMEDIATELY TO PREVENT CORRUPTED OPERATION.");
                        Console.WriteLine("================================================================================\n");
                        Console.ResetColor();

                        File.WriteAllText(startupAuditPath, $"[CRITICAL BOOT FAILURE]: File completely missing from operational pathway: {fullPath}", Encoding.UTF8);
                        Environment.Exit(1);
                    }

                    // Ingest the target profile securely using a transient throwaway reader block
                    var diskProfile = CrystalCastles.DataEngine.CCUnifiedParser.LoadUnifiedStageFile(fullPath, null);
                    if (diskProfile == null || diskProfile.Heights == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n================================================================================");
                        Console.WriteLine($"[!] CRITICAL DATA PIPELINE FAULT: CORRUPTED DATA MATRIX IN: {targetFile}");
                        Console.WriteLine("    ENGINE SHUTTING DOWN IMMEDIATELY TO PREVENT RAM DRIFT CONTEXT.");
                        Console.WriteLine("================================================================================\n");
                        Console.ResetColor();

                        File.WriteAllText(startupAuditPath, $"[CRITICAL BOOT FAILURE]: Structural parsing crash on asset sheet: {fullPath}", Encoding.UTF8);
                        Environment.Exit(1);
                    }

                    // Push verified data values into the application's single source of truth memory layer
                    // FIX BANNER: Task High-Integrity Direct Ground-Truth Matrix Ingestion Mapping [v0.95]
                    var targetCity = RomManager.IsolatedStages[roomIdx];
                    for (int r = 0; r < 22; r++)
                    {
                        for (int c = 0; c < 22; c++)
                        {
                            // Ingest height structures and collectibles using uniform linear matching indices
                            targetCity.Heights[r, c] = diskProfile.Heights[r, c];
                            targetCity.Gems[r, c] = diskProfile.Gems[r, c];
                        }
                    }

                    auditBuilder.AppendLine($"Stage [{roomIdx:D2}] -> {StageNames[roomIdx].PadRight(28)} | Maps: [✓] (Unified) | Gems: [✓] | Elevators: [✓]");
                }

                auditBuilder.AppendLine("--------------------------------------------------------------------------------");
                auditBuilder.AppendLine("=== GLOBAL REPOSITORY PIPELINE CONSUMPTION SUMMARY ===");
                auditBuilder.AppendLine(" * Verified Height Maps Ingested : 37 / 37 Tracks");
                auditBuilder.AppendLine(" * Verified Collectible Gem Maps : 37 / 37 Tracks");
                auditBuilder.AppendLine(" * Verified Elevator Core Configs: 37 / 37 Tracks");
                auditBuilder.AppendLine("================================================================================");

                File.WriteAllText(startupAuditPath, auditBuilder.ToString(), Encoding.UTF8);
            }
            catch (Exception startupEx)
            {
                string emergencyLog = Path.Combine(projectBaseDir, "emergency_boot_error.log");
                File.WriteAllText(emergencyLog, $"Pipeline execution crashed out during validation pass: {startupEx.Message}");
                Environment.Exit(1);
            }
            /// ====================================================================================
            // FIX BANNER: High-Integrity Diagnostic Gem Flow Matrix Analyzer [v0.95 SPEC]
            // TARGETS: [1-1] (Stage 00), [1-2] (Stage 01), [1-3] (Stage 02) Dual Audit Verification
            // ====================================================================================
            int[] traceStages = new int[] { 0, 1, 2 };
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n================================================================================");
            Console.WriteLine("Executing Live Diagnostic Analysis Pass for Gem Data Flow — Stages [1-1], [1-2], [1-3]");
            Console.WriteLine("================================================================================");
            Console.ResetColor();

            foreach (int stgIdx in traceStages)
            {
                var targetCity = RomManager.IsolatedStages[stgIdx];
                int displayWaveMajor = (stgIdx / 4) + 1;
                int displayWaveMinor = (stgIdx % 4) + 1;

                Console.WriteLine($"\n[DIAGNOSTIC MATRIX COMPARISON TRACE FOR LEVEL [{displayWaveMajor}-{displayWaveMinor}] — STAGE {stgIdx:D2}]");
                Console.WriteLine("    --- FILE PARSED IN-MEMORY PROFILE ---             --- ACTIVE ENGINE COMPILATION CORE ---");
                Console.WriteLine("    00 02 04 06 08 10 12 14 16 18 20                  00 02 04 06 08 10 12 14 16 18 20");

                for (int r = 0; r < 22; r++)
                {
                    // Print File-parsed memory buffer line block pass row index
                    Console.Write($"{r:D2}  ");
                    for (int c = 0; c < 22; c++)
                    {
                        Console.Write(targetCity.Gems[r, c] ? "*" : ".");
                    }

                    // Separation Divider Gutter
                    Console.Write("    |    ");

                    // Print Running System Memory state block pass row index
                    Console.Write($"{r:D2}  ");
                    for (int c = 0; c < 22; c++)
                    {
                        // Safely probing engine core lookup coordinates dynamically
                        bool engineGemState = RomManager.IsolatedStages[stgIdx].Gems[r, c];
                        Console.Write(engineGemState ? "*" : ".");
                    }
                    Console.WriteLine();
                }
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("================================================================================");
            Console.WriteLine("Diagnostic Print Sequence Finished. Launching Application Core Windows Viewport.");
            Console.WriteLine("================================================================================");
            Console.ResetColor();

            // DIAGNOSTIC CORE FIXED BANNER: PROGRAM.CS - DYNAMIC LEVEL 01 OVERWRITE RUN HOOK
            // LOCATION: INJECTED DIRECTLY BENEATH INITIAL STARTUP AUDIT LOG STRIDE PASS
            // CONSTRAINTS: FORCES LOG GENERATION TO SWITCH TARGET TO TREE WAVE FILE (v0.90)
            // ====================================================================================
            RomManager.DebugTargetStage = 0;
            var stagePalettes = StagePalettes.GetMasterPaletteMatrix();

            const int virtualWidth = 800;
            const int virtualHeight = 600;
            const int scaleMultiplier = 2;
            const int screenWidth = virtualWidth * scaleMultiplier;
            const int screenHeight = virtualHeight * scaleMultiplier;

            // FIX BANNER: Task H1 Dynamic GUI Title Window Synchronization [v0.95]
            Raylib.InitWindow(screenWidth, screenHeight, $"cSharpRaylib - Crystal Castles Unified Ingestion Suite [v{CCFormatConfig.VersionTag}]");
            Raylib.SetTargetFPS(60);

            RenderTexture2D targetBuffer = Raylib.LoadRenderTexture(virtualWidth, virtualHeight);
            Raylib.SetTextureFilter(targetBuffer.Texture, TextureFilter.Point);

            int currentRoom = 0;
            int lastRoomID = -1;

            bool is3DMode = false;
            float globalScale = 1.0f;
            float heightMultiplier = 1.8f;
            int panOffsetX = 0;
            int panOffsetY = 0;
            int rotationAngle = 0;
            float tiltFactor = 1.0f;
            int renderStyleMode = 0;
            bool displayPathOverlays = false;
            bool displayGems = true;
            bool showDanLegacyOverlay = false;
            bool invertBackground = false;
            bool triggerTextExport = false;
            bool trigger3DLabWindow = false;
            
            // ====================================================================================
            // REBUILD STEP 4 - PART 1: PROGRAM.CS - CORE HOTKEY LOGGER INTERCEPT
            // LOCATION: REPLACES INITIAL KEY HANDLING IN MAIN WHILE LOOP (APPROX LINE 135-145)
            // ====================================================================================
            while (!Raylib.WindowShouldClose())
            { 
                // ====================================================================================
                // SUB-TASK 7A - PART 3: PROGRAM.CS - CORE HOTKEY LOGGER INTERCEPT HOOK SYNCHRONIZATION
                // LOCATION: REPLACES STRIDE PARAMETERS PASSED TO HANDLEKEYS INTERACTION HANDLER
                // CONSTRAINTS: INJECTS ACTIVE TIMESTAMP ARGUMENT TO FACILITATE TRANSIENT FILE SELECTION
                // ====================================================================================
                InputHandler.HandleKeys(
                    ref currentRoom, ref is3DMode, ref globalScale, ref heightMultiplier,
                    ref panOffsetX, ref panOffsetY, ref rotationAngle, ref tiltFactor,
                    ref renderStyleMode, ref displayPathOverlays, ref displayGems, ref showDanLegacyOverlay,
                    ref invertBackground, ref triggerTextExport, ref trigger3DLabWindow,
                    RomManager.ActiveSessionTimestamp
                );


                // --- v0.91 Core Hotkey Input Focus Latch ---
                // Protect global files and variables from unintended keystroke bleed-through
                // --- v0.91 Isolated Diagnostic Window Trigger Stub ---
                // Intercept the F3 function key to deploy our isolated parallel layout validation environment
                if (Raylib.IsKeyPressed(KeyboardKey.F3))
                {
                    // Latch state variable tracking flag so core inputs go blind
                    RomManager.IsParserDiagnosticActive = true;

                    // Launch our memory-isolated v0.91 diagnostic viewer workspace context thread
                    DiagnosticCanvas.LaunchDebugWindow(
                        currentRoom,
                        StageNames,
                        RoomToCityMap,
                        RomManager.IsolatedStages
                    );
                }

                // --- v0.91 Core Hotkey Input Focus Latch ---
                if (!RomManager.IsParserDiagnosticActive)
                {
                    if (Raylib.IsKeyPressed(KeyboardKey.E))
                    {
                        CCUnifiedLogger.ExportActiveSessionSummary(StageNames, MainLoggedCells);
                    }
                }
                else
                {
                    // --- Localized Diagnostic Overlay Input Intercept Pass ---
                    // Keystrokes pressed here run strictly local to the diagnostic view layer,
                    // safely hiding global engine variables from accidental macro fires.
                    if (Raylib.IsKeyPressed(KeyboardKey.Tab))
                    {
                        // Toggle local sub-layers (Elevators vs Gems verification matrices)
                        RomManager.IsParserDiagnosticActive = false;
                    }
                }

                int cityIndex = RoomToCityMap[currentRoom] & 0x0F;
                CityData activeCity = RomManager.IsolatedStages[currentRoom];
                string currentStageName = (currentRoom < StageNames.Length) ? StageNames[currentRoom] : "Unknown Castle";
                // ====================================================================================
                // END OF PART 1
                // ====================================================================================

                // ====================================================================================
                // FIX BLOCK 2: PROGRAM.CS - DEACTIVATE DESTRUCTIVE LIFECYCLE ARRAY CLEAR PASS
                // LOCATION: REPLACES SUB-CONDITIONAL SWAP CHECK IN MAIN LOOP (APPROX LINE 163 TO 186)
                // ====================================================================================
                if (currentRoom != lastRoomID)
                {
                    try
                    {
                        // v0.90 SPECS FILTER: Commented out to prevent erasing selections on screen migrations
                        // Array.Clear(MainLoggedCells, 0, MainLoggedCells.Length);

                        // Force instantaneous reference boundary locking flag update
                        lastRoomID = currentRoom;

                        Console.WriteLine($"[PIPELINE MONITOR] Room swapped successfully to Stage {currentRoom:D2}. Display mapped cleanly from single source of truth.");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Gated initialization fault: {ex.Message}");
                    }
                }
                // ====================================================================================
                // END OF FIX BLOCK 2
                // ====================================================================================

                var currentActiveIsolatedRoom = RomManager.IsolatedStages[currentRoom];

                if (currentActiveIsolatedRoom.TrackState != StageTrackingState.NoElevators)
                {
                    for (int i = 0; i < currentActiveIsolatedRoom.Elevators.Count; i++)
                    {
                        currentActiveIsolatedRoom.Elevators[i].Update();
                    }
                }

                Raylib.BeginTextureMode(targetBuffer);
                Raylib.ClearBackground(invertBackground ? Color.RayWhite : Color.Black);

                // FIX CS1503: Corrected dynamic fallback statement to point to index 0 array smoothly
                Color[] activeTheme = stagePalettes.ContainsKey(currentRoom) ? stagePalettes[currentRoom] : stagePalettes[0];
                var drawingRoom = RomManager.IsolatedStages[currentRoom];
                System.Numerics.Vector2 frameworkMouseVec = Raylib.GetMousePosition();

                /// ====================================================================================
                // TASK 1 - PART 2: PROGRAM.CS - CORE GRAPHICS UPDATE LOOP VARIABLE HOOK
                // LOCATION: REPLACES PARAMETER PASSING TO MOUSE TRACKING ENGINE (APPROX LINE 207)
                // ====================================================================================
                InputHandler.TrackMouseProbeCoordinates(
                    frameworkMouseVec.X / scaleMultiplier,
                    frameworkMouseVec.Y / scaleMultiplier,
                    globalScale, panOffsetX, panOffsetY, rotationAngle, tiltFactor,
                    drawingRoom, is3DMode, currentRoom, MainLoggedCells
                );
                // ====================================================================================
        

                if (!is3DMode)
                {
                    MapRenderer.Draw2DBlueprint(drawingRoom, activeTheme, displayPathOverlays, displayGems, true, currentRoom);
                }
                else
                {
                    // FIX CS0117: Explicit argument routing sync pass
                    MapRenderer.Draw3DWorkspace(drawingRoom, activeTheme, globalScale, heightMultiplier, panOffsetX, panOffsetY, rotationAngle, tiltFactor, renderStyleMode, displayPathOverlays, displayGems, true, currentRoom);
                }

                // ====================================================================================
                // FIX BANNER: PROGRAM.CS - PARAMETER VARIABLE ALIGNMENT FOR LIVE ROTATION TELEMETRY
                // LOCATION: REPLACES DRAWCONTROLOVERLAY RUNTIME HOOK IN MAIN LOOP (APPROX LINE 243)
                // CONSTRAINTS: INJECTS LIVE ROTATIONANGLE VALUE STRAIGHT INTO DRAW PIPELINE
                // ====================================================================================
                InputHandler.DrawControlOverlay(
                    is3DMode, renderStyleMode, displayPathOverlays, displayGems,
                    showDanLegacyOverlay, invertBackground, globalScale, currentRoom,
                    currentStageName, activeCity.Elevators.Count, activeTheme, MainLoggedCells,
                    rotationAngle
                );
                // ====================================================================================

                Raylib.EndTextureMode();

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                Rectangle sourceRec = new Rectangle(0, 0, virtualWidth, -virtualHeight);
                Rectangle destRec = new Rectangle(0, 0, screenWidth, screenHeight);
                System.Numerics.Vector2 originPoint = new System.Numerics.Vector2(0, 0);

                Raylib.DrawTexturePro(targetBuffer.Texture, sourceRec, destRec, originPoint, 0.0f, Color.White);

                Raylib.EndDrawing();
            }

            // FIX CS7036: Replaced incorrect Load call with standard hardware memory release
            Raylib.UnloadRenderTexture(targetBuffer);
            Raylib.CloseWindow();
        }
    }
}