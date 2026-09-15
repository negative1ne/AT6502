// ============================================================================
// PROGRAM.CS - CORE GRAPHICS UPDATE AND LABORATORY ROUTING
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

            List<CityData> cities = RomManager.LoadRomDatabase();
            var stagePalettes = StagePalettes.GetMasterPaletteMatrix();

            const int virtualWidth = 800;
            const int virtualHeight = 600;
            const int scaleMultiplier = 2;
            const int screenWidth = virtualWidth * scaleMultiplier;
            const int screenHeight = virtualHeight * scaleMultiplier;

            Raylib.InitWindow(screenWidth, screenHeight, "cSharpRaylib - Crystal Castles High-Res View");
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
            bool displayElevators = true;
            bool invertBackground = false;
            bool triggerTextExport = false;
            bool trigger3DLabWindow = false; // INJECT FLAG HERE

            while (!Raylib.WindowShouldClose())
            {
                // Pass parameter values stably with our new laboratory layout argument track safely mapped out
                InputHandler.HandleKeys(
                    ref currentRoom, ref is3DMode, ref globalScale, ref heightMultiplier,
                    ref panOffsetX, ref panOffsetY, ref rotationAngle, ref tiltFactor,
                    ref renderStyleMode, ref displayPathOverlays, ref displayGems, ref displayElevators,
                    ref invertBackground, ref triggerTextExport, ref trigger3DLabWindow
                );

                int cityIndex = RoomToCityMap[currentRoom] & 0x0F;
                CityData activeCity = cities[cityIndex];
                string currentStageName = (currentRoom < StageNames.Length) ? StageNames[currentRoom] : "Unknown Castle";

                if (currentRoom != lastRoomID)
                {
                    try
                    {
                        var isolatedTargetRoom = RomManager.IsolatedStages[currentRoom];

                        ElevatorPremapper.ApplyOverrides(currentRoom, isolatedTargetRoom.Elevators);
                        SessionLogger.LogStageTransition(currentRoom, currentStageName, cityIndex, isolatedTargetRoom.Elevators);

                        StringBuilder verificationBuffer = new StringBuilder();
                        verificationBuffer.AppendLine($"[DATA_VERIFICATION] Cross-examining 37-Stage Deep-Copy structural array for Wave [{currentRoom:D2}]...");

                        bool matricesAreIdentical = true;
                        for (int x = 0; x < 22; x++)
                        {
                            for (int y = 0; y < 22; y++)
                            {
                                if (isolatedTargetRoom.Heights[x, y] != activeCity.Heights[x, y] ||
                                    isolatedTargetRoom.Attributes[x, y] != activeCity.Attributes[x, y])
                                {
                                    matricesAreIdentical = false;
                                }
                            }
                        }

                        string resultMarkerStr = matricesAreIdentical ? "✅ VERIFIED MATCH: Deep-copy layout block matches parent city exactly." : "🛑 ALERT: Structural matrix drift detected inside copy layer!";
                        verificationBuffer.AppendLine($"    -> {resultMarkerStr}");
                        verificationBuffer.AppendLine($"    -> Parent City Array Elevators: {activeCity.Elevators.Count} | Isolated Array Elevators: {isolatedTargetRoom.Elevators.Count}");

                        verificationBuffer.AppendLine("\n[VIEWPORT_CROSS_CHECK] Analyzing coordinate synchronization with primary 2D viewports...");
                        for (int i = 0; i < isolatedTargetRoom.Elevators.Count; i++)
                        {
                            var ev = isolatedTargetRoom.Elevators[i];
                            if (ev.IsMapped)
                            {
                                verificationBuffer.AppendLine($"    - Elevator [{i}]: File Matrix Location Cell = ({ev.CellX:D2},{ev.CellY:D2}) | Terrain Height = {isolatedTargetRoom.Heights[ev.CellX, ev.CellY]:D2}");
                            }
                        }

                        verificationBuffer.AppendLine("\n[3D_PROJECT_AUDIT] Tracing Isometric 3D Space Projection Coordinates...");
                        for (int i = 0; i < isolatedTargetRoom.Elevators.Count; i++)
                        {
                            var ev = isolatedTargetRoom.Elevators[i];
                            if (ev.IsMapped)
                            {
                                int cx = ev.CellX;
                                int cy = ev.CellY;
                                int th = isolatedTargetRoom.Heights[cx, cy];

                                int projectedIsoX = 200 - (cx * 4) + (cy * 8);
                                int projectedIsoY = 100 + (cx * 4) + (cy * 2) - th;

                                verificationBuffer.AppendLine($"    - Elevator [{i}] Geometry Profile:");
                                verificationBuffer.AppendLine($"      * Hard ROM Anchors -> ArcadeX = {ev.HorizontalPosition:D3}, ArcadeY = {ev.VerticalPosition:D3}");
                                verificationBuffer.AppendLine($"      * Predicted 3D Box -> TargetX = {projectedIsoX:D3}, TargetY = {projectedIsoY:D3} | Ground Deck Altitude = {th:D2}");
                                verificationBuffer.AppendLine($"      * Motion Threshold -> BottomPos = {ev.BottomPosition} | TopPos = {ev.TopPosition} | TravelOffset = {ev.CurrentPosition}");
                            }
                        }
                        verificationBuffer.AppendLine("================================================================================");

                        SessionLogger.LogVerificationMessage(verificationBuffer.ToString());

                        lastRoomID = currentRoom;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Gated initialization fault: {ex.Message}");
                    }
                }

                var currentActiveIsolatedRoom = RomManager.IsolatedStages[currentRoom];

                int safeBoundLimit = Math.Min(activeCity.Elevators.Count, currentActiveIsolatedRoom.Elevators.Count);
                for (int i = 0; i < safeBoundLimit; i++)
                {
                    var romLift = activeCity.Elevators[i];
                    var isolatedLift = currentActiveIsolatedRoom.Elevators[i];

                    romLift.Update();

                    isolatedLift.HorizontalPosition = romLift.HorizontalPosition;
                    isolatedLift.VerticalPosition = romLift.VerticalPosition;
                    isolatedLift.CurrentPosition = romLift.CurrentPosition;
                    isolatedLift.Mode = romLift.Mode;
                    isolatedLift.CurrentSitTime = romLift.CurrentSitTime;
                }

                Raylib.BeginTextureMode(targetBuffer);
                Raylib.ClearBackground(invertBackground ? Color.RayWhite : Color.Black);

                Color[] activeTheme = stagePalettes.ContainsKey(currentRoom) ? stagePalettes[currentRoom] : stagePalettes[0];
                var drawingRoom = RomManager.IsolatedStages[currentRoom];

                if (!is3DMode)
                {
                    MapRenderer.Draw2DBlueprint(drawingRoom, activeTheme, displayPathOverlays, displayGems, displayElevators);
                }
                else
                {
                    MapRenderer.Draw3DWorkspace(drawingRoom, activeTheme, globalScale, heightMultiplier, panOffsetX, panOffsetY, rotationAngle, tiltFactor, renderStyleMode, displayPathOverlays, displayGems, displayElevators);
                    ElevatorRenderer.Render3DElevators(drawingRoom.Elevators, activeTheme, globalScale, heightMultiplier, panOffsetX, panOffsetY, rotationAngle, tiltFactor, renderStyleMode);
                }

                InputHandler.DrawControlOverlay(
                    is3DMode, renderStyleMode, displayPathOverlays, displayGems, displayElevators,
                    invertBackground, globalScale, currentRoom, currentStageName, drawingRoom.Elevators.Count, activeTheme
                );

                if (triggerTextExport)
                {
                    DiagnosticCanvas.LaunchDebugWindow(currentRoom, StageNames, RoomToCityMap, cities);
                    triggerTextExport = false;
                }

                // RUN DUAL INTERACTIVE SWITCHBOARD HOOK: 
                if (trigger3DLabWindow)
                {
                    System.Diagnostics.Debug.WriteLine("[SANDBOX] Diverting processing stream to 3D Diagnostic Canvas...");
                    DiagnosticCanvas.LaunchDebugWindow(currentRoom, StageNames, RoomToCityMap, cities);
                    trigger3DLabWindow = false;
                }

                Raylib.EndTextureMode();
                // --- HARDWARE CANVAS BLIT UP-SCALING PASS ---
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                Rectangle sourceRec = new Rectangle(0, 0, virtualWidth, -virtualHeight);
                Rectangle destRec = new Rectangle(0, 0, screenWidth, screenHeight);
                System.Numerics.Vector2 originPoint = new System.Numerics.Vector2(0, 0);

                Raylib.DrawTexturePro(targetBuffer.Texture, sourceRec, destRec, originPoint, 0.0f, Color.White);

                Raylib.EndDrawing();
            }

            Raylib.UnloadRenderTexture(targetBuffer);
            Raylib.CloseWindow();
        }
    }
}