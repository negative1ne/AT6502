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
            bool triggerTextExport = false;

            // Simple Execution Video Frame Loop
            while (!Raylib.WindowShouldClose())
            {
                // 1. Core Input Routing Step
                InputHandler.HandleKeys(
                    ref currentRoom, ref is3DMode, ref globalScale, ref heightMultiplier,
                    ref panOffsetX, ref panOffsetY, ref rotationAngle, ref tiltFactor,
                    ref renderStyleMode, ref displayPathOverlays, ref displayGems, ref displayElevators, ref triggerTextExport
                );

                Raylib.BeginTextureMode(targetBuffer);
                Raylib.ClearBackground(Color.Black);

                Color[] activeTheme = stagePalettes.ContainsKey(currentRoom) ? stagePalettes[currentRoom] : stagePalettes[0];

                // 2. HUD text drawing pass
                Raylib.DrawText("ccSharpRaylib", 20, 20, 20, Color.RayWhite);
                string activeStageName = (currentRoom < StageNames.Length) ? StageNames[currentRoom] : "Unknown Castle";
                int displayLevel = (currentRoom / 4) + 1;
                int displayWave = (currentRoom % 4) + 1;
                string viewModeLabel = is3DMode ? "3D Isometric" : "2D Flat";
                Raylib.DrawText($"Level {displayLevel} - {displayWave} [{activeStageName}] | {viewModeLabel}", 20, 55, 18, Color.Gold);

                int cityIndex = RoomToCityMap[currentRoom] & 0x0F;
                CityData activeCity = cities[cityIndex];

                // 3. CLEAN REFACTORED WORKSPACE RENDER CALLS
                if (!is3DMode)
                {
                    MapRenderer.Draw2DBlueprint(activeCity, activeTheme, displayPathOverlays, displayGems);
                }
                else
                {
                    MapRenderer.Draw3DWorkspace(activeCity, activeTheme, globalScale, heightMultiplier, panOffsetX, panOffsetY, rotationAngle, tiltFactor, renderStyleMode, displayPathOverlays, displayGems);

                    if (displayElevators)
                    {
                        ElevatorRenderer.Render3DElevators(activeCity.Elevators, activeTheme, globalScale, heightMultiplier, panOffsetX, panOffsetY, rotationAngle, tiltFactor, renderStyleMode);
                    }
                }

                // 4. Panel Overlay Elements Drawing Pass
                Raylib.DrawRectangle(20, 95, 40, 20, activeTheme[0]);
                Raylib.DrawRectangle(70, 95, 40, 20, activeTheme[1]);
                Raylib.DrawRectangle(120, 95, 40, 20, activeTheme[2]);
                Raylib.DrawText("Active Layout Palette Matrix Slots", 180, 98, 14, Color.LightGray);

                InputHandler.DrawControlOverlay(is3DMode, renderStyleMode, displayPathOverlays, displayGems, displayElevators);

                if (triggerTextExport)
                {
                    RomManager.ExportStageTextFile(currentRoom, StageNames[currentRoom], activeCity);
                    triggerTextExport = false;
                }

                Raylib.EndTextureMode();

                // 5. Native hardware blit upscaling canvas pass
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                Rectangle sourceRec = new Rectangle(0, 0, virtualWidth, -virtualHeight);
                Rectangle destRec = new Rectangle(0, 0, screenWidth, screenHeight);
                System.Numerics.Vector2 originPoint = new System.Numerics.Vector2(0, 0);

                Raylib.DrawTexturePro(targetBuffer.Texture, sourceRec, destRec, originPoint, 0.0f, Color.White);

                Raylib.EndDrawing();
            }

            // GPU Memory De-allocations on Exit
            Raylib.UnloadRenderTexture(targetBuffer);
            Raylib.CloseWindow();
        }
    }
}