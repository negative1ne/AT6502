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

            // 4. Initialize tracking states inside Program.cs
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

            // Ensure this matches the 12th variable passed to your InputHandler
            bool triggerTextExport = false;

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

            while (!Raylib.WindowShouldClose())
            {
                // Inside the update loop of Program.cs
                InputHandler.HandleKeys(
                    ref currentRoom,
                    ref is3DMode,
                    ref globalScale,
                    ref heightMultiplier,
                    ref panOffsetX,
                    ref panOffsetY,
                    ref rotationAngle,
                    ref tiltFactor,
                    ref renderStyleMode,
                    ref displayPathOverlays,
                    ref displayGems,
                    ref triggerTextExport // <-- Pass the updated flag name here
                );

                Raylib.BeginTextureMode(targetBuffer);
                Raylib.ClearBackground(Color.Black);

                Color[] activeTheme = stagePalettes.ContainsKey(currentRoom) ? stagePalettes[currentRoom] : stagePalettes[0];

                Raylib.DrawText("ccSharpRaylib", 20, 20, 20, Color.RayWhite);
                string activeStageName = (currentRoom < StageNames.Length) ? StageNames[currentRoom] : "Unknown Castle";
                int displayLevel = (currentRoom / 4) + 1;
                int displayWave = (currentRoom % 4) + 1;
                string viewModeLabel = is3DMode ? "3D Isometric" : "2D Flat";
                Raylib.DrawText($"Level {displayLevel} - {displayWave} [{activeStageName}] | {viewModeLabel}", 20, 55, 18, Color.Gold);

                int cityIndex = RoomToCityMap[currentRoom] & 0x0F;
                CityData activeCity = cities[cityIndex];

                if (!is3DMode)
                {
                    int cellSize = 16;
                    int gridOffsetX = 380;
                    int gridOffsetY = 150;

                    for (int x = 0; x < 22; x++)
                    {
                        for (int y = 0; y < 22; y++)
                        {
                            int tileHeight = activeCity.Heights[x, y];
                            if (tileHeight == 0) continue;

                            int posX = gridOffsetX + (y * cellSize);
                            int posY = gridOffsetY + (x * cellSize);
                            byte cellAttr = activeCity.Attributes[x, y];

                            int baseShade = Math.Min(100 + (tileHeight * 12), 255);
                            Color blockColor = activeTheme[0];

                            if (displayPathOverlays)
                            {
                                if ((cellAttr & 0x20) == 0x20) blockColor = Color.Purple;
                                else if ((cellAttr & 0x04) == 0x04) blockColor = Color.Green;
                                else if ((cellAttr & 0x10) == 0x10) blockColor = Color.Yellow;
                            }
                            else
                            {
                                blockColor = new Color(
                                    (byte)(activeTheme[0].R * baseShade / 255),
                                    (byte)(activeTheme[0].G * baseShade / 255),
                                    (byte)(activeTheme[0].B * baseShade / 255),
                                    (byte)255
                                );
                            }

                            Raylib.DrawRectangle(posX, posY, cellSize - 1, cellSize - 1, blockColor);

                            // 2D GEM RENDERING STUB LOOKUP CALL
                            if (displayGems && ((cellAttr & 0x10) == 0x10))
                            {
                                int cx = gridOffsetX + (y * cellSize) - (cellSize / 2) + 8;
                                int cy = gridOffsetY + (x * cellSize) - (cellSize / 2) + 8;
                                Raylib.DrawCircle(cx + 4, cy + 4, 3, Color.Red); // Classic arcade ruby dot
                            }
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < 22; x++)
                    {
                        for (int y = 0; y < 22; y++)
                        {
                            int tileHeight = activeCity.Heights[x, y];
                            if (tileHeight == 0) continue;

                            byte cellAttr = activeCity.Attributes[x, y];

                            LevelTransform.DrawIsometricBlock(
                                x, y, tileHeight, activeTheme, globalScale, heightMultiplier, panOffsetX, panOffsetY,
                                rotationAngle, tiltFactor, renderStyleMode, displayPathOverlays, cellAttr
                            );

                            // 3D GEM RENDERING STUB LOOKUP CALL
                            if (displayGems && ((cellAttr & 0x10) == 0x10))
                            {
                                LevelTransform.Draw3DGem(x, y, tileHeight, globalScale, heightMultiplier, panOffsetX, panOffsetY, rotationAngle, tiltFactor, Color.Yellow);
                            }
                        }
                    }
                }

                Raylib.DrawRectangle(20, 95, 40, 20, activeTheme[0]);
                Raylib.DrawRectangle(70, 95, 40, 20, activeTheme[1]);
                Raylib.DrawRectangle(120, 95, 40, 20, activeTheme[2]);
                Raylib.DrawText("Active Layout Palette Matrix Slots", 180, 98, 14, Color.LightGray);

                InputHandler.DrawControlOverlay(is3DMode, renderStyleMode, displayPathOverlays, displayGems);

                // CRITICAL CAPTURE LAYER INTERCEPT POINT
                // Intercept the text file matrix compilation request cleanly on demand
                if (triggerTextExport)
                {
                    RomManager.ExportStageTextFile(currentRoom, StageNames[currentRoom], activeCity);
                    triggerTextExport = false; // Reset intercept driver flag instantly
                }

                Raylib.EndTextureMode();

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