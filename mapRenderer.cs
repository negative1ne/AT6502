using System;
using System.Collections.Generic;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace cSharpRaylib
{
    public static class MapRenderer
    {
        public static void Draw2DBlueprint(CityData activeCity, Color[] activeTheme, bool displayPathOverlays, bool displayGems)
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

                    // Check if an elevator is configured on this tile position
                    bool isElevatorSpot = false;
                    foreach (var ev in activeCity.Elevators)
                    {
                        if (ev.HorizontalPosition == x && ev.VerticalPosition == y)
                        {
                            isElevatorSpot = true;
                            break;
                        }
                    }

                    int baseShade = Math.Min(100 + (tileHeight * 12), 255);
                    Color blockColor;

                    // 1. ELEVATOR GRAPHIC OVERLAY: Draw as a distinct color square (Orange)
                    if (isElevatorSpot)
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

                    // Render ruby gem indicator dot over tile center
                    if (displayGems && ((cellAttr & 0x10) == 0x10))
                    {
                        Raylib.DrawCircle(posX + 8, posY + 8, 3, Color.Red);
                    }
                }
            }
        }

        public static void Draw3DWorkspace(CityData activeCity, Color[] activeTheme, float scale, float heightScale,
            int offsetX, int offsetY, int rotationAngle, float tiltFactor, int renderStyle, bool showPaths, bool displayGems)
        {
            for (int x = 0; x < 22; x++)
            {
                for (int y = 0; y < 22; y++)
                {
                    int tileHeight = activeCity.Heights[x, y];
                    if (tileHeight == 0) continue;

                    byte cellAttr = activeCity.Attributes[x, y];

                    LevelTransform.DrawIsometricBlock(
                        x, y, tileHeight, activeTheme, scale, heightScale, offsetX, offsetY,
                        rotationAngle, tiltFactor, renderStyle, showPaths, cellAttr
                    );

                    if (displayGems && ((cellAttr & 0x10) == 0x10))
                    {
                        LevelTransform.Draw3DGem(x, y, tileHeight, scale, heightScale, offsetX, offsetY, rotationAngle, tiltFactor, Color.Yellow);
                    }
                }
            }
        }
    }
}