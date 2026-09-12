using System;
using System.Collections.Generic;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace cSharpRaylib
{
    public static class MapRenderer
    {
        public static void Draw2DBlueprint(CityData activeCity, Color[] activeTheme, bool displayPathOverlays, bool displayGems, bool displayElevators)
        {
            int cellSize = 16;
            int gridOffsetX = 380;
            int gridOffsetY = 150;

            for (int x = 0; x < 22; x++)
            {
                for (int y = 0; y < 22; y++)
                {
                    // FIXED OVERRIDE: If it's a hidden lift spot like Stage 32, draw it even if height is 0
                    bool isElevatorCell = false;
                    foreach (var ev in activeCity.Elevators)
                    {
                        if (ev.IsMapped && ev.CellX == x && ev.CellY == y)
                        {
                            isElevatorCell = true;
                            break;
                        }
                    }

                    int tileHeight = activeCity.Heights[x, y];

                    // Only escape if it's completely empty space AND not a lift
                    if (tileHeight == 0 && !isElevatorCell) continue;

                    int posX = gridOffsetX + (y * cellSize);
                    int posY = gridOffsetY + (x * cellSize);
                    byte cellAttr = activeCity.Attributes[x, y];

                    int baseShade = Math.Min(100 + (tileHeight * 12), 255);
                    Color blockColor;

                    // Dynamic B Key Visibility Layer Check
                    if (isElevatorCell && displayElevators)
                    {
                        blockColor = Color.Orange;
                    }
                    else if (displayPathOverlays)
                    {
                        if ((cellAttr & 0x20) == 0x20) blockColor = Color.Purple;
                        else if ((cellAttr & 0x04) == 0x04) blockColor = Color.Green;
                        else if ((cellAttr & 0x10) == 0x10) blockColor = Color.Yellow;
                        else blockColor = activeTheme[0];
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

                    if (displayGems && ((cellAttr & 0x10) == 0x10) && !(isElevatorCell && displayElevators))
                    {
                        Raylib.DrawCircle(posX + 8, posY + 8, 3, Color.Red);
                    }

                    if (isElevatorCell && displayElevators)
                    {
                        Raylib.DrawText("E", posX + 4, posY + 1, 12, Color.White);
                    }
                }
            }
        }


        // Updated method signature with displayElevators visibility tracking flag parameters added safely
        public static void Draw3DWorkspace(CityData activeCity, Color[] activeTheme, float scale, float heightScale,
            int offsetX, int offsetY, int rotationAngle, float tiltFactor, int renderStyle, bool showPaths, bool displayGems, bool displayElevators)
        {
            // MAIN 3D STRUCTURAL GRAPHICS GRID LOOPS
            for (int x = 0; x < 22; x++)
            {
                for (int y = 0; y < 22; y++)
                {
                    int currentHeight = activeCity.Heights[x, y];
                    bool isElevatorCell = false;
                    int drawHeight = currentHeight;

                    // Query our 100% verified pre-mapped database to look for a platform slot match
                    foreach (var ev in activeCity.Elevators)
                    {
                        if (ev.IsMapped && ev.CellX == x && ev.CellY == y)
                        {
                            isElevatorCell = true;
                            // INTERCEPT ANIMATION DEPTHS: Only override height if the B key toggle is ON
                            if (displayElevators)
                            {
                                drawHeight = ev.CurrentPosition;
                            }
                            break;
                        }
                    }

                    if (drawHeight == 0) continue;

                    byte cellAttr = activeCity.Attributes[x, y];

                    // Route column positions through your structural math transformations
                    LevelTransform.DrawIsometricBlock(
                        x, y, drawHeight, activeTheme, scale, heightScale, offsetX, offsetY,
                        rotationAngle, tiltFactor, renderStyle, showPaths, cellAttr
                    );

                    // Gem overlays pass
                    if (displayGems && ((cellAttr & 0x10) == 0x10) && !(isElevatorCell && displayElevators))
                    {
                        LevelTransform.Draw3DGem(x, y, drawHeight, scale, heightScale, offsetX, offsetY, rotationAngle, tiltFactor, Color.Yellow);
                    }
                }
            }
        }
    }
}
