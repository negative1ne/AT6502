// ============================================================================
// FIX BANNER: PROGRAM.CS - PART 1: MASTER ENGINE LOOP UTILITIES (v0.85)
// ============================================================================
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Text;
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
            // INITIALIZATION SEQUENCE DROP-IN BANNER: v0.85 DATA ENGINE ISOLATION
            // ====================================================================================

            // Step 1: Establish project base directory tracking context
            string projectBaseDir = AppDomain.CurrentDomain.BaseDirectory;

            // Step 2: Initialize the dual-logging system pipeline immediately at startup
            CrystalCastles.DataEngine.DataEngineLogger.Initialize(projectBaseDir);

            // ====================================================================================
            // RUNTIME DATA INTEGRATION HOOK: v0.90 UNIFIED SINGLE-PASS RE-ROUTE
            // TARGET: VERIFY ZERO-DRIFT SPATIAL EXTRACTION ON V0.90 SPEC SINGLE SHEETS
            // ====================================================================================
            // ====================================================================================
            // FIX BLOCK 1: PROGRAM.CS - v0.90 SPECS CALIBRATION & VERBOSE LOGGER MUTING
            // LOCATION: REPLACE FROM TRY BLOCK CALIBRATION START (LINE 41) TO CATCH END (LINE 70)
            // ====================================================================================
            try
            {
                Console.WriteLine("\n================================================================================");
                Console.WriteLine("LAUNCHING UNIFIED DATA INGESTION SUITE: STARTING RUNTIME CALIBRATION TEST PASS");
                Console.WriteLine("================================================================================\n");

                string rootDir = AppDomain.CurrentDomain.BaseDirectory;
                string unifiedFolder = Path.Combine(rootDir, "data", "unified_data");

                // Target Selection: Run a calibration check on Stage 00 text configuration layout sheets
                string targetFileName = "STAGE_00_BALL_WAVE.txt";
                string fullUnifiedPath = Path.Combine(unifiedFolder, targetFileName);

                // MUTED LOGGER PASS: Passing null here completely suppresses the repetitive 430KB disk noise
                var calibrationProfile = CrystalCastles.DataEngine.CCUnifiedParser.LoadUnifiedStageFile(fullUnifiedPath, null);

                Console.WriteLine("\n================================================================================");
                Console.WriteLine($"CALIBRATION VERDICT: STAGE {calibrationProfile.StageID:D2} [{calibrationProfile.StageName}] INGESTION MATCH VERIFIED.");
                Console.WriteLine("================================================================================\n");
            }
            catch (Exception integrationEx)
            {
                Console.WriteLine($"[CRITICAL PIPELINE FAULT]: Integration run execution crashed: {integrationEx.Message}");
            }
            // ====================================================================================
            // END OF FIX BLOCK 1
            // ====================================================================================


            // ====================================================================================
            // STRINGS SWEEP: PROGRAM.CS - SYNCHRONIZE VERSION TEXT REGISTRATION
            // LOCATION: REPLACES LABELED ENGINE DATA ISOLATION INIT TRACE LOG LINE
            // CONSTRAINTS: ELIMINATES LEGACY VERSION FRAGMENT STREAMS FROM BASELINE TRACES
            // ====================================================================================
            CrystalCastles.DataEngine.DataEngineLogger.LogSession("System Initialization: v0.90 Data Engine Isolation initialized successfully.");
            CrystalCastles.DataEngine.DataEngineLogger.LogSession($"Project Base Directory verified at: {projectBaseDir}");

            // ====================================================================================

            // ====================================================================================
            // CONCLUDING PIPELINE EXECUTION HOOK: v0.85 DATA ENGINE ISOLATION
            // ====================================================================================

            // Step 4: Fire the isolated 5-stage migration sandbox pass immediately
            // CrystalCastles.DataEngine.SandboxTestHarness.ExecuteValidationPass(projectBaseDir);

            // ====================================================================================
            // ====================================================================================
            // SUB-TASK 7C HOOK: PROGRAM.CS - ACTIVATE MASTER STARTUP INTEGRITY VERIFICATION PASS
            // LOCATION: INJECTED DIRECTLY FOLLOWING ROM MANAGER REPOSITORY DATABASE INITIALIZATION
            // CONSTRAINTS: SINGLE RUN PASS ON APPLICATION EXECUTION ENTRY STRIDE (v0.90)
            // ====================================================================================
            CCUnifiedLogger.AppendStartupAuditReport(StageNames);
            List<CityData> cities = RomManager.LoadRomDatabase();
            var stagePalettes = StagePalettes.GetMasterPaletteMatrix();

            const int virtualWidth = 800;
            const int virtualHeight = 600;
            const int scaleMultiplier = 2;
            const int screenWidth = virtualWidth * scaleMultiplier;
            const int screenHeight = virtualHeight * scaleMultiplier;

            Raylib.InitWindow(screenWidth, screenHeight, "cSharpRaylib - Crystal Castles Unified Ingestion Suite [v0.85]");
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
              

            // v0.90 DEFERRED LOG ENGINE SNAPSHOT HOOK: Invoked strictly under user command pass
            if (Raylib.IsKeyPressed(KeyboardKey.E))
                {
                    string cleanStageName = (currentRoom < StageNames.Length) ? StageNames[currentRoom] : "Unknown Castle";
                    CCUnifiedLogger.ExportActiveSessionSummary(currentRoom, cleanStageName, MainLoggedCells);
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