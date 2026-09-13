using System;
using System.Collections.Generic;
using Raylib_cs;
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

            // Database parsed and auto-premapped entirely behind the scene boundary line
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

            // Inside Program.cs -> Main method variable block
            int currentRoom = 0;
            int lastRoomID = -1; // INJECT THIS VARIABLE LATCH TRACKER HERE
            
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
            bool invertBackground = false; // Add this line right here
            bool triggerTextExport = false;

            // ============================================================================
            // PROGRAM.CS - ROUTING VARIABLES TO YOUR ORIGINAL BASELINE
            // ============================================================================
            while (!Raylib.WindowShouldClose())
            {
                // Ensure the last argument matches the precise variable name 
                // expected by your original, working InputHandler.cs file
                InputHandler.HandleKeys(
                    ref currentRoom, ref is3DMode, ref globalScale, ref heightMultiplier,
                    ref panOffsetX, ref panOffsetY, ref rotationAngle, ref tiltFactor,
                    ref renderStyleMode, ref displayPathOverlays, ref displayGems, ref displayElevators,
                    ref invertBackground, ref triggerTextExport // Or ref exportTextFlag if that's what your baseline calls it
                );

                // --- PROTECTED SINGLE-SHOT SECOVERY RE-INITIALIZATION ---
                int cityIndex = RoomToCityMap[currentRoom] & 0x0F;
                CityData activeCity = cities[cityIndex];

                // Only executes once upon explicit room change key triggers
                if (currentRoom != lastRoomID)
                {
                    try
                    {
                        // Safely inject our hybrid split gate processor
                        ElevatorPremapper.ApplyOverrides(currentRoom, activeCity.Elevators);
                        lastRoomID = currentRoom;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Gated initialization fault: {ex.Message}");
                    }
                }

                // Continuous state machines update tick calls passed out
                foreach (var ev in activeCity.Elevators)
                {
                    ev.Update();
                }

                Raylib.BeginTextureMode(targetBuffer);
                Raylib.ClearBackground(Color.Black);

                Color[] activeTheme = stagePalettes.ContainsKey(currentRoom) ? stagePalettes[currentRoom] : stagePalettes[0];

                if (!is3DMode)
                {
                    MapRenderer.Draw2DBlueprint(activeCity, activeTheme, displayPathOverlays, displayGems, displayElevators);
                }
                else
                {
                    MapRenderer.Draw3DWorkspace(activeCity, activeTheme, globalScale, heightMultiplier, panOffsetX, panOffsetY, rotationAngle, tiltFactor, renderStyleMode, displayPathOverlays, displayGems, displayElevators);
                }

                // 5. HUD CONTROL OVERLAY AND LABELS PASS
                string currentStageName = (currentRoom < StageNames.Length) ? StageNames[currentRoom] : "Unknown Castle";

                InputHandler.DrawControlOverlay(
                    is3DMode, renderStyleMode, displayPathOverlays, displayGems, displayElevators,
                    invertBackground, globalScale, currentRoom, currentStageName, activeCity.NumElevators, activeTheme
                );

                if (triggerTextExport)
                {
                    // Passes arrays across the window boundary lines to allow live level flipping
                    DiagnosticCanvas.LaunchDebugWindow(currentRoom, StageNames, RoomToCityMap, cities);
                    triggerTextExport = false;
                }

                // === THE CRITICAL RE-ALIGNMENT FIX SEPARATION HOOKS ===
                Raylib.EndTextureMode(); // 1. CLOSE THE VIRTUAL BUFFER FIRST!

                // 6. NATIVE HARDWARE BLIT UP-SCALING CANVAS PASS (Fires cleanly onto your monitor)
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                Rectangle sourceRec = new Rectangle(0, 0, virtualWidth, -virtualHeight);
                Rectangle destRec = new Rectangle(0, 0, screenWidth, screenHeight);
                System.Numerics.Vector2 originPoint = new System.Numerics.Vector2(0, 0);

                Raylib.DrawTexturePro(targetBuffer.Texture, sourceRec, destRec, originPoint, 0.0f, Color.White);

                Raylib.EndDrawing(); // 2. CLOSE BUFFER DISPLAY COMPLETE
            }

            Raylib.UnloadRenderTexture(targetBuffer);
            Raylib.CloseWindow();
        }
    }
}