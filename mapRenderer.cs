// ============================================================================
// FIX BANNER: MAPRENDERER.CS - PART 1: MAIN VIEWPORT SESSION MARKERS (v0.85)
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

                    // 1. VOID FILTER PASS
                    if (tileHeight == 0 && !isElevatorCell)
                    {
                        if (displayPathOverlays && ((cellAttr & 0x04) == 0x04))
                        {
                            Raylib.DrawRectangleLines(posX, posY, cellSize - 1, cellSize - 1, Color.DarkGray);
                        }
                        if (displayGems) DrawVerifiedGemMarker2D(x, y, cellAttr, romAttributes, posX, posY);
                        continue;
                    }

                    // 2. DYNAMIC CELL RENDERING DETERMINATION
                    int baseShade = Math.Min(100 + (tileHeight * 12), 255);
                    Color blockColor;

                    if (isElevatorCell && displayElevators)
                    {
                        blockColor = Color.Orange;
                    }
                    else if (displayPathOverlays)
                    {
                        if ((cellAttr & 0x04) == 0x04) blockColor = Color.Green;
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

                    // v0.85 MASTER INTERACTIVE OVERLAY RULE
                    if (isElevatorCell && displayElevators)
                    {
                        Raylib.DrawText("E", posX + 4, posY + 1, 12, Color.White);
                    }
                    // Read the shared global matrix to display session progress markers live
                    else if (Program.MainLoggedCells[x, y])
                    {
                        Raylib.DrawText("L", posX + 4, posY + 1, 12, Color.Orange);
                    }
                    // ============================================================================
                    // FIX BANNER: MAPRENDERER.CS - 2D BLUEPRINT GEM REFERENCE UNLEASHED (v0.85)
                    // ============================================================================
                    else if (displayGems && activeCity.Gems[x, y])
                    {
                        DrawVerifiedGemMarker2D(x, y, cellAttr, romAttributes, posX, posY);
                    }
                }
            }

            if (InputHandler.IsProbeInsideWorkspace && !displayPathOverlays)
            {
                int probeX = InputHandler.ProbeGridX;
                int probeY = InputHandler.ProbeGridY;

                if (probeX >= 0 && probeX < 22 && probeY >= 0 && probeY < 22)
                {
                    int highlightX = gridOffsetX + (probeY * 16);
                    int highlightY = gridOffsetY + (probeX * 16);
                    Raylib.DrawRectangleLines(highlightX, highlightY, 15, 15, Color.SkyBlue);
                }
            }
        }
         
        // ============================================================================
        // MAPRENDERER.CS APPENDIX FIX: RESTORE 3D WORKSPACE METHOD HOOK (v0.85)
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

                    if (currentHeight == 0 && !isElevatorCell)
                    {
                        if (displayGems)
                        {
                            DrawVerifiedGemMarker3D(x, y, currentHeight, scale, heightScale, offsetX, offsetY, rotationAngle, tiltFactor, activeCity.Attributes[x, y], romAttributes);
                        }
                        continue;
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
                        LevelTransform.DrawIsometricBlock(
                            x, y, currentHeight, activeTheme, scale, heightScale, offsetX, offsetY,
                            rotationAngle, tiltFactor, renderStyle, showPaths, cellAttr
                        );
                    }

                    // ============================================================================
                    // FIX BANNER: MAPRENDERER.CS - 3D WORKSPACE GEM REFERENCE UNLEASHED (v0.85)
                    // ============================================================================
                    if (displayGems && !isElevatorCell && activeCity.Gems[x, y])
                    {
                        DrawVerifiedGemMarker3D(x, y, currentHeight, scale, heightScale, offsetX, offsetY, rotationAngle, tiltFactor, cellAttr, romAttributes);
                    }
                }
            }

            if (InputHandler.IsProbeInsideWorkspace)
            {
                int targetX = InputHandler.ProbeGridX;
                int targetY = InputHandler.ProbeGridY;

                if (targetX >= 0 && targetX < 22 && targetY >= 0 && targetY < 22)
                {
                    int currentAltitude = activeCity.Heights[targetX, targetY];
                    byte cellAttributes = activeCity.Attributes[targetX, targetY];
                    Color[] probeIndicatorPalette = new Color[] { Color.SkyBlue, Color.Blue, Color.DarkBlue };

                    LevelTransform.DrawIsometricBlock(
                        targetX, targetY, currentAltitude, probeIndicatorPalette,
                        scale, heightScale, offsetX, offsetY, rotationAngle, tiltFactor, 2, true, cellAttributes
                    );
                }
            }
        
        }
        // ============================================================================
        // MAPRENDERER.CS - PART 2: DISPLAY HOOKS & DISCREPANCY COMPARATORS (v0.85)
        // ============================================================================
        private static void DrawVerifiedGemMarker2D(int x, int y, byte diskAttr, byte[,] romAttrs, int posX, int posY)
        {
            bool existsOnDisk = (diskAttr & 0x10) == 0x10;
            bool existsInRom = (romAttrs[x, y] & 0x10) == 0x10;

            if (!existsOnDisk && !existsInRom) return;

            Color validationColor;
            if (existsOnDisk && existsInRom) validationColor = Color.Yellow;
            else if (existsOnDisk) validationColor = Color.Lime;
            else validationColor = Color.Red;

            Raylib.DrawCircle(posX + 8, posY + 8, 3, validationColor);
        }

        private static void DrawVerifiedGemMarker3D(int x, int y, int tileHeight, float scale, float heightScale,
            int offsetX, int offsetY, int rotationAngle, float tiltFactor, byte diskAttr, byte[,] romAttrs)
        {
            bool existsOnDisk = (diskAttr & 0x10) == 0x10;
            bool existsInRom = (romAttrs[x, y] & 0x10) == 0x10;

            if (!existsOnDisk && !existsInRom) return;

            Color validationColor;
            if (existsOnDisk && existsInRom) validationColor = Color.Yellow;
            else if (existsOnDisk) validationColor = Color.Lime;
            else validationColor = Color.Red;

            LevelTransform.Draw3DGem(x, y, tileHeight, scale, heightScale, offsetX, offsetY, rotationAngle, tiltFactor, validationColor);
        }

        public static int GetRomHeightDiscrepancyCount(CityData activeCity, int stageNum)
        {
            if (activeCity == null) return 0;

            int mismatchCount = 0;

            try
            {
                byte[,] romHeights = RomManager.BaseCities[stageNum % 16].Heights;
                byte[,] romAttributes = RomManager.BaseCities[stageNum % 16].Attributes;

                for (int x = 0; x < 22; x++)
                {
                    for (int y = 0; y < 22; y++)
                    {
                        int customDiskHeight = activeCity.Heights[x, y];
                        int originalRomHeight = romHeights[x, y];

                        // v0.85 SPATIAL DENSITY LEDGER: Compare heights AND pathing attributes (0x0F lower nibble)
                        int customDiskLowerNibble = activeCity.Attributes[x, y] & 0x0F;
                        int originalRomLowerNibble = romAttributes[x, y] & 0x0F;

                        if (customDiskHeight != originalRomHeight || customDiskLowerNibble != originalRomLowerNibble)
                        {
                            mismatchCount++;
                        }
                    }
                }
            }
            catch (Exception) { }

            return mismatchCount;
        }
    }
}