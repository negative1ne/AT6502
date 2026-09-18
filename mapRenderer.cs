// ============================================================================
// MAPRENDERER.CS - WORKSPACE FILTERING AND LEGACY OVERLAY INJECTION
// ============================================================================
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
                    if (tileHeight == 0 && !isElevatorCell) continue;

                    int posX = gridOffsetX + (y * cellSize);
                    int posY = gridOffsetY + (x * cellSize);
                    byte cellAttr = activeCity.Attributes[x, y];

                    int baseShade = Math.Min(100 + (tileHeight * 12), 255);
                    Color blockColor;

                    if (isElevatorCell && displayElevators)
                    {
                        blockColor = Color.Orange;
                    }
                    else if (displayPathOverlays)
                    {
                        if ((cellAttr & 0x20) == 0x20) blockColor = Color.Purple;
                        else if ((cellAttr & 0x04) == 0x04) blockColor = Color.Green;
                        else if ((cellAttr & 0x10) == 0x10) blockColor = Color.Yellow;
                        else blockColor = activeTheme;
                    }
                    else
                    {
                        blockColor = new Color(
                            (byte)(activeTheme.R * baseShade / 255),
                            (byte)(activeTheme.G * baseShade / 255),
                            (byte)(activeTheme.B * baseShade / 255),
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

        // ============================================================================
        // MAPRENDERER.CS - UPDATED 3D WORKSPACE WITH SANDBOX BLUEPRINT EXTRACTIONS
        // ============================================================================
        public static void Draw3DWorkspace(CityData activeCity, Color[] activeTheme, float scale, float heightScale,
            int offsetX, int offsetY, int rotationAngle, float tiltFactor, int renderStyle, bool showPaths, bool displayGems, bool showDanLegacyOverlay)
        {
            Color[] goldBasePalette = new Color[] { Color.Gold, Color.Orange, Color.DarkBrown };

            for (int x = 0; x < 22; x++)
            {
                for (int y = 0; y < 22; y++)
                {
                    int currentHeight = activeCity.Heights[x, y];
                    bool isElevatorCell = false;

                    foreach (var ev in activeCity.Elevators)
                    {
                        if (ev.IsMapped && ev.CellX == x && ev.CellY == y)
                        {
                            isElevatorCell = true;
                            break;
                        }
                    }

                    if (currentHeight == 0 && !isElevatorCell) continue;
                    byte cellAttr = activeCity.Attributes[x, y];

                    if (isElevatorCell)
                    {
                        int baseTileHeight = Math.Max(1, currentHeight);
                        LevelTransform.DrawIsometricBlock(
                            x, y, baseTileHeight, goldBasePalette, scale, heightScale, offsetX, offsetY,
                            rotationAngle, tiltFactor, renderStyle, false, cellAttr
                        );
                    }
                    else if (currentHeight > 0)
                    {
                        LevelTransform.DrawIsometricBlock(
                            x, y, currentHeight, activeTheme, scale, heightScale, offsetX, offsetY,
                            rotationAngle, tiltFactor, renderStyle, showPaths, cellAttr
                        );
                    }

                    if (displayGems && ((cellAttr & 0x10) == 0x10) && !(isElevatorCell))
                    {
                        LevelTransform.Draw3DGem(x, y, currentHeight, scale, heightScale, offsetX, offsetY, rotationAngle, tiltFactor, Color.Yellow);
                    }

                    // ========================================================================
                    // SANDBOX ENGINE: INJECT DAN'S ORIGINAL "DIVIDE BY 8" INTERCEPT COORD OVERLAYS
                    // ========================================================================
                    if (showDanLegacyOverlay)
                    {
                        // Direct pull of Dan's historical formulas from DiagnosticCanvas.cs (Lines 109-112)
                        int legacyArcadeX = 200 - (x * 4) + (y * 8);
                        int legacyArcadeY = 100 + (x * 4) + (y * 2) - currentHeight;

                        // Apply standard global panning and scaling offsets so it renders relative to your window positions
                        int drawX = (int)((legacyArcadeX * scale) + offsetX + 200);
                        int drawY = (int)((legacyArcadeY * scale) + offsetY + 150);
                        int dotRadius = Math.Max(2, (int)(3 * scale));

                        // Render the legacy point matrix structure cleanly as distinct flat Blue disks
                        if (isElevatorCell)
                        {
                            // Highlight elevator locations along the blueprint array map
                            Raylib.DrawCircle(drawX, drawY, dotRadius + 2, Color.Blue);
                            Raylib.DrawCircleLines(drawX, drawY, dotRadius + 2, Color.SkyBlue);
                        }
                        else
                        {
                            // Draw standard layout outline grid dots
                            Raylib.DrawCircle(drawX, drawY, dotRadius, new Color(0, 100, 255, 180));
                        }
                    }
                }
            }
        }
    }
}