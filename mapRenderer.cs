// ============================================================================
// FIX BANNER: MAPRENDERER.CS - PART 1: MAIN VIEWPORT SESSION MARKERS (v0.85)
// ============================================================================
using Raylib_cs;
using System;
using System.Collections.Generic;
using static System.Windows.Forms.AxHost;
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
                    // ====================================================================================
                    // FIX BANNER: MAPRENDERER.CS - 3D RENDERING MATRIX SYNCHRONIZATION
                    // LOCATION: REPLACES THE ELSE IF (Program.MainLoggedCells) SELECTION TARGET (APPROX LINE 80)
                    // CONSTRAINTS: COMPACT LINE OVERRUN PREVENTER | ALIGNS NATIVE STAGENUM & POSX/Y LABELS
                    // ====================================================================================
                    // Read the 3D persistent matrix layer cleanly using your native stageNum index
                    else if (Program.MainLoggedCells[stageNum, x, y])
                    {
                        Raylib.DrawText("L", posX + 4, posY + 1, 12, Color.Orange);
                    }
                    // ====================================================================================

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

        // ============================================================================
        // FIX BANNER: MAPRENDERER.CS - UNIFIED MASTER TRACK CROSS-REFERENCE (v0.85 SUCCESS)
        // ============================================================================
        public static int GetRomHeightDiscrepancyCount(CityData activeCity, int stageNum)
        {
            if (activeCity == null) return 0;

            int mismatchCount = 0;

            byte[] RoomToCityMap = new byte[] {
                0x00, 0x02, 0x09, 0xC3, 0x46, 0x71, 0x0C, 0xC7,
                0x06, 0x0D, 0x45, 0xCB, 0x04, 0x0A, 0x06, 0x4F,
                0x41, 0x4D, 0x3C, 0xC3, 0x0A, 0x02, 0x32, 0x3F,
                0x01, 0x04, 0x75, 0xFB, 0x01, 0x3A, 0x06, 0xF7,
                0x08, 0x7D, 0x05, 0xCB, 0x0E
            };

            try
            {
                // Isolate the true parent city bank tracking index cleanly from the lookup matrix
                int parentCityBankID = RoomToCityMap[stageNum] & 0x0F;

                byte[,] romHeights = RomManager.BaseCities[parentCityBankID].Heights;
                byte[,] romAttributes = RomManager.BaseCities[parentCityBankID].Attributes;
                // ============================================================================

                for (int x = 0; x < 22; x++)
                {
                    for (int y = 0; y < 22; y++)
                    {
                        // ============================================================================
                        // FIX BANNER: MAPRENDERER.CS - MASKED VOID CALCULATION CHECK (v0.85 SUCCESS)
                        // ============================================================================
                        int customDiskHeight = activeCity.Heights[x, y];
                        int originalRomHeight = romHeights[x, y];

                        // v0.85 SPATIAL DENSITY LEDGER: Compare lower nibbles for structural attributes
                        int customDiskLowerNibble = activeCity.Attributes[x, y] & 0x0F;
                        int originalRomLowerNibble = romAttributes[x, y] & 0x0F;

                        // ============================================================================
                        // FIX BANNER: MAPRENDERER.CS - CORE VOID FILTER CORRECTION (v0.85 SUCCESS)
                        // ============================================================================
                        bool isHeightMismatch = customDiskHeight != originalRomHeight;
                        bool isAttributeMismatch = customDiskLowerNibble != originalRomLowerNibble;

                        // CORRECTION: If disk maps a 0 void and ROM matches that structural air footprint, skip it!
                        if (isHeightMismatch && customDiskHeight == 0)
                        {
                            // Examine the native hardware ROM lower nibble grid structure data.
                            // If the ROM lower nibble is 0, it means the original game logic also maps a true void cell here.
                            if (originalRomLowerNibble == 0)
                            {
                                isHeightMismatch = false;

                                // Sync pathing attributes so empty air spaces don't flag an attribute mismatch error
                                isAttributeMismatch = false;
                            }

                            // ============================================================================
                            // FIX BANNER: MAPRENDERER.CS - DEEP STRIDE MONITOR LOGGER TRACE (v0.85 ANALYSIS)
                            // ============================================================================
                            if (isHeightMismatch || isAttributeMismatch)
                            {
                                mismatchCount++;

                                // Debug telemetry trace prints the exact data layout factors to Visual Studio's Output Panel
                                System.Diagnostics.Debug.WriteLine($"[DRIFT TRACE] Stage {stageNum:D2} Cell [{x:D2},{y:D2}] -> DiskHeight: {customDiskHeight:D3} | RomHeight: {originalRomHeight:D3} | DiskAttr: {customDiskLowerNibble:X2} | RomAttr: {originalRomLowerNibble:X2}");
                            }
                            // ============================================================================
                        }
                    }
                }
            }

            catch (Exception) { }

            return mismatchCount;
        }
    }
}