// ============================================================================
// MAPRENDERER.CS - UPGRADED 2D BLUEPRINT GRAPHICS ENGINE (PART 1)
// ============================================================================
using System;
using System.Collections.Generic;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace cSharpRaylib
{
    public static class MapRenderer
    {
        public static void Draw2DBlueprint(CityData activeCity, Color[] activeTheme, bool displayPathOverlays, bool displayGems, bool displayElevators, int stageNum)
        {
            int cellSize = 16;
            int gridOffsetX = 380;
            int gridOffsetY = 150;

            // Fetch original un-altered baseline data context properties straight from parent ROM banks
            int parentCityIndex = (stageNum < 37) ? (RomManager.IsolatedStages[stageNum].NumElevators) : 0; // Quick cross reference link
            byte[,] romAttributes = RomManager.BaseCities[stageNum % 16].Attributes;

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
                    byte cellAttr = activeCity.Attributes[x, y];
                    int posX = gridOffsetX + (y * cellSize);
                    int posY = gridOffsetY + (x * cellSize);

                    // 1. CHASSIS CORRECTION RULE: Handle Height 0 Passageway Outlines First
                    if (tileHeight == 0 && !isElevatorCell)
                    {
                        // If paths are toggled on and this zero-height cell contains tracking bits, outline it cleanly
                        if (displayPathOverlays && ((cellAttr & 0x34) > 0))
                        {
                            Raylib.DrawRectangleLines(posX, posY, cellSize - 1, cellSize - 1, Color.DarkGray);
                        }

                        // Render gem cross-check comparisons over height-0 paths if active
                        if (displayGems) DrawVerifiedGemMarker2D(x, y, cellAttr, romAttributes, posX, posY);
                        continue;
                    }

                    // 2. STANDARD LANDSCAPE BLOCK DRAWING (Only processes if height > 0)
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

                    // Render collectible verification dots over valid solid deck coordinates
                    if (displayGems && !(isElevatorCell && displayElevators))
                    {
                        DrawVerifiedGemMarker2D(x, y, cellAttr, romAttributes, posX, posY);
                    }

                    if (isElevatorCell && displayElevators)
                    {
                        Raylib.DrawText("E", posX + 4, posY + 1, 12, Color.White);
                    }
                }
            }
        }

        private static void DrawVerifiedGemMarker2D(int x, int y, byte diskAttr, byte[,] romAttrs, int posX, int posY)
        {
            // Bit 0x10 tracks localized collectible presence triggers natively
            bool existsOnDisk = (diskAttr & 0x10) == 0x10;
            bool existsInRom = (romAttrs[x, y] & 0x10) == 0x10;

            if (!existsOnDisk && !existsInRom) return;

            Color validationColor;
            if (existsOnDisk && existsInRom) validationColor = Color.Yellow; // Perfect match!
            else if (existsOnDisk) validationColor = Color.Lime;   // Added via disk asset overlay
            else validationColor = Color.Red;    // Missing from disk file dump

            Raylib.DrawCircle(posX + 8, posY + 8, 3, validationColor);
        }
        // ============================================================================
        // MAPRENDERER.CS - UPDATED 3D WORKSPACE WITH GEM AUDITING GATES (PART 2)
        // ============================================================================
        public static void Draw3DWorkspace(CityData activeCity, Color[] activeTheme, float scale, float heightScale,
            int offsetX, int offsetY, int rotationAngle, float tiltFactor, int renderStyle, bool showPaths, bool displayGems, bool displayElevators, int stageNum)
        {
            Color[] goldBasePalette = new Color[] { Color.Gold, Color.Orange, Color.DarkBrown };
            byte[,] romAttributes = RomManager.BaseCities[stageNum % 16].Attributes;

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

                    // ========================================================================
                    // REPAIRED HEIGHT-0 FILTER PASS: EXEMPT ACTIVE LIFT CHASSIS SYSTEM FROM CONTINUES
                    // ========================================================================
                    if (currentHeight == 0)
                    {
                        if (!isElevatorCell)
                        {
                            // If paths are on and it's a standard flat empty walkway tile, draw a subtle gem cross-check
                            if (displayGems)
                            {
                                DrawVerifiedGemMarker3D(x, y, currentHeight, scale, heightScale, offsetX, offsetY, rotationAngle, tiltFactor, activeCity.Attributes[x, y], romAttributes);
                            }
                            continue; // Safely skip drawing solid blocks for standard empty space rows
                        }
                        // If it IS an elevator cell, we do NOT continue! We allow it to pass through and render its base column
                    }

                    byte cellAttr = activeCity.Attributes[x, y];

                    if (isElevatorCell && displayElevators)
                    {
                        int baseTileHeight = Math.Max(1, currentHeight);
                        LevelTransform.DrawIsometricBlock(
                            x, y, baseTileHeight, goldBasePalette, scale, heightScale, offsetX, offsetY,
                            rotationAngle, tiltFactor, renderStyle, false, cellAttr
                        );
                    }
                    else if (currentHeight > 0)
                    {
                        // Standard block projection drawing gate
                        LevelTransform.DrawIsometricBlock(
                            x, y, currentHeight, activeTheme, scale, heightScale, offsetX, offsetY,
                            rotationAngle, tiltFactor, renderStyle, showPaths, cellAttr
                        );
                    }

                    // Render our dual-verification collectible spheres into the active 3D view space
                    if (displayGems && !isElevatorCell)
                    {
                        DrawVerifiedGemMarker3D(x, y, currentHeight, scale, heightScale, offsetX, offsetY, rotationAngle, tiltFactor, cellAttr, romAttributes);
                    }
                }
            }
        }

        private static void DrawVerifiedGemMarker3D(int x, int y, int tileHeight, float scale, float heightScale,
            int offsetX, int offsetY, int rotationAngle, float tiltFactor, byte diskAttr, byte[,] romAttrs)
        {
            bool existsOnDisk = (diskAttr & 0x10) == 0x10;
            bool existsInRom = (romAttrs[x, y] & 0x10) == 0x10;

            if (!existsOnDisk && !existsInRom) return;

            Color validationColor;
            if (existsOnDisk && existsInRom) validationColor = Color.Yellow; // Perfect validation match
            else if (existsOnDisk) validationColor = Color.Lime;   // Unique custom workspace gem
            else validationColor = Color.Red;    // Missing data file definition

            LevelTransform.Draw3DGem(x, y, tileHeight, scale, heightScale, offsetX, offsetY, rotationAngle, tiltFactor, validationColor);
        }
    }
}