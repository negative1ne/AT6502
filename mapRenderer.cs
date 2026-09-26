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
         // ====================================================================================
        // FIX BANNER: MAPRENDERER.CS - TASK 6: THREE-LEVEL 90° 2D BLUEPRINT CANVAS RE-MAPPER
        // LOCATION: REPLACES DRAW2DBLUEPRINT METHOD COMPLETELY (LINES 12-105 APPROX)
        // CONSTRAINTS: KEEPS NATIVE STORAGE PURITY | ALIGNS STAGES 01, 21, AND 22 VISUALLY
        // ====================================================================================
        public static void Draw2DBlueprint(CityData activeCity, Color[] activeTheme, bool displayPathOverlays, bool displayGems, bool displayElevators, int stageNum)
        {
            // Phase 2 Pipeline Audit Pass: Trace mapping indices directly to find pointer drift
            try
            {
                byte[] RoomToCityLookupTable = new byte[] {
                    0x00, 0x02, 0x09, 0xC3, 0x46, 0x71, 0x0C, 0xC7,
                    0x06, 0x0D, 0x45, 0xCB, 0x04, 0x0A, 0x06, 0x4F,
                    0x41, 0x4D, 0x3C, 0xC3, 0x0A, 0x02, 0x32, 0x3F,
                    0x01, 0x04, 0x75, 0xFB, 0x01, 0x3A, 0x06, 0xF7,
                    0x08, 0x7D, 0x05, 0xCB, 0x0E
                };

                int resolvedParentBank = RoomToCityLookupTable[stageNum] & 0x0F;
                string diagnosticTracePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "session_audit.log");

                string alignmentReport = $"[{DateTime.Now:HH:mm:ss}] [VIEWPORT TRACKING TRACE] -> Active Stage ID: {stageNum:D2} | Render Modulo Index: {stageNum % 16:D2} | True ROM Parent Bank ID: {resolvedParentBank:D2}\n";
                System.IO.File.AppendAllText(diagnosticTracePath, alignmentReport, System.Text.Encoding.UTF8);
            }
            catch { /* Guard concurrent file locks */ }

            int cellSize = 16;
            int gridOffsetX = 380;
            int gridOffsetY = 150;

            // Unification Fix: Route attributes cleanly from the active in-memory v0.95 city tracking structure
            byte[,] activeAttributes = activeCity.Attributes;

            bool isRotatedStage = (stageNum == 1 || stageNum == 21 || stageNum == 22);

            for (int x = 0; x < 22; x++)
            {
                for (int y = 0; y < 22; y++)
                {
                    int srcX = isRotatedStage ? (21 - y) : x;
                    int srcY = isRotatedStage ? x : y;

                    bool isElevatorCell = false;
                    int tileHeight = activeCity.Heights[srcX, srcY];
                    byte cellAttr = activeCity.Attributes[srcX, srcY];

                    // Fetch real-time position to animate the 2D overview blueprint tiles
                    int currentLiveHeight = tileHeight;
                    foreach (var ev in activeCity.Elevators)
                    {
                        if (ev.IsMapped && ev.CellX == srcX && ev.CellY == srcY)
                        {
                            isElevatorCell = true;
                            currentLiveHeight = ev.CurrentPosition;
                            break;
                        }
                    }

                    int posX = gridOffsetX + (y * cellSize);
                    int posY = gridOffsetY + (x * cellSize);

                    if (tileHeight == 0 && !isElevatorCell)
                    {
                        if (displayPathOverlays && ((cellAttr & 0x04) == 0x04))
                        {
                            Raylib.DrawRectangleLines(posX, posY, cellSize - 1, cellSize - 1, Color.DarkGray);
                        }
                        if (displayGems && activeCity.Gems[srcX, srcY])
                        {
                            Raylib.DrawCircle(posX + 8, posY + 8, 3, Color.Yellow);
                        }
                        continue;
                    }
                    // ============================================================================
                    // TASK C3 REPAIR: HIGH-ACCURACY LINEAR 9-SHADE ALTITUDE HEATMAP ROUTER
                    // ============================================================================
                    Color blockColor;

                    if (isElevatorCell && displayElevators)
                    {
                        blockColor = Color.Orange;
                    }
                    else if (displayPathOverlays && ((cellAttr & 0x04) == 0x04))
                    {
                        blockColor = Color.Green;
                    }
                    else
                    {
                        // Safely evaluate local maximum height ranges natively in this layout row scope
                        int localMaxAltitudeValue = 1;
                        for (int r = 0; r < 22; r++)
                        {
                            for (int c = 0; c < 22; c++)
                            {
                                if (activeCity.Heights[r, c] > localMaxAltitudeValue)
                                {
                                    localMaxAltitudeValue = activeCity.Heights[r, c];
                                }
                            }
                        }

                        // Protect pipeline against division-by-zero bounds on flat maps
                        if (localMaxAltitudeValue < 1) localMaxAltitudeValue = 1;

                        // Map height levels evenly across 9 progressive shading intervals
                        float calculatedAltitudeFactor = (float)currentLiveHeight / localMaxAltitudeValue;
                        int altitudeStrideIndex = Math.Clamp((int)(calculatedAltitudeFactor * 8.99f), 0, 8);

                        // Extract active theme colors natively from the core array parameters
                        Color activeBaseTone = activeTheme[0];

                        // ============================================================================
                        // TASK C3 REPAIR: BRIGHTNESS BOOST SYSTEM FOR DARK ARCADE PALETTES
                        // ============================================================================
                        // Elevate the base brightness floor from 0.25 to 0.45 to prevent dark green blackouts
                        float linearScaleMultiplier = 0.45f + (altitudeStrideIndex * 0.06f);
                        // ============================================================================

                        blockColor = new Color(
                            (byte)Math.Clamp(activeBaseTone.R * linearScaleMultiplier, 0, 255),
                            (byte)Math.Clamp(activeBaseTone.G * linearScaleMultiplier, 0, 255),
                            (byte)Math.Clamp(activeBaseTone.B * linearScaleMultiplier, 0, 255),
                            (byte)255
                        );
                    }
                    // ============================================================================


                    Raylib.DrawRectangle(posX, posY, cellSize - 1, cellSize - 1, blockColor);

                    if (isElevatorCell && displayElevators)
                    {
                        Raylib.DrawText("E", posX + 4, posY + 1, 12, Color.White);
                    }
                    else if (Program.MainLoggedCells[stageNum, srcX, srcY])
                    {
                        Raylib.DrawText("L", posX + 4, posY + 1, 12, Color.Orange);
                    }
                    else if (displayGems && activeCity.Gems[srcX, srcY])
                    {
                        Raylib.DrawCircle(posX + 8, posY + 8, 3, Color.Yellow);
                    }
                }
            }

            // Interactive coordinate probe box alignment
            if (InputHandler.IsProbeInsideWorkspace && !displayPathOverlays)
            {
                int rawProbeX = InputHandler.ProbeGridX;
                int rawProbeY = InputHandler.ProbeGridY;

                // Adjust screen selection highlight vectors to display properly over rotated sheets
                int drawGridRow = isRotatedStage ? rawProbeY : rawProbeX;
                int drawGridCol = isRotatedStage ? (21 - rawProbeX) : rawProbeY;

                if (drawGridRow >= 0 && drawGridRow < 22 && drawGridCol >= 0 && drawGridCol < 22)
                {
                    int highlightX = gridOffsetX + (drawGridCol * 16);
                    int highlightY = gridOffsetY + (drawGridRow * 16);
                    Raylib.DrawRectangleLines(highlightX, highlightY, 15, 15, Color.SkyBlue);
                }
            }
        }

        // ============================================================================
        // MAPRENDERER.CS APPENDIX FIX: RESTORE 3D WORKSPACE METHOD HOOK (v0.85)
        // ============================================================================
        // ============================================================================
        // MAPRENDERER.CS APPENDIX FIX: RESTORE 3D WORKSPACE METHOD HOOK (v0.95 SYNC)
        // ============================================================================
        public static void Draw3DWorkspace(CityData activeCity, Color[] activeTheme, float scale, float heightScale,
            int offsetX, int offsetY, int rotationAngle, float tiltFactor, int renderStyle, bool showPaths, bool displayGems, bool displayElevators, int stageNum)
        {

            // Step 1 Isolated Calibration Pass: Inject known baseline test values into the first elevator on Stage 00 (Level 1-1)
            // Step 2 Dual Isolation Pass: Force completely asymmetric timing windows on Level 1-1
            if (stageNum == 0 && activeCity.Elevators.Count >= 2)
            {
                var liftE0 = activeCity.Elevators[0];
                var liftE1 = activeCity.Elevators[1];

                // Inject asymmetric bottom boundaries to completely split their timing loops
                liftE0.BottomPosition = 6;
                liftE1.BottomPosition = 14;

                // Safe initialization clip
                if (liftE0.CurrentPosition < liftE0.BottomPosition) liftE0.CurrentPosition = liftE0.BottomPosition;
                if (liftE1.CurrentPosition < liftE1.BottomPosition) liftE1.CurrentPosition = liftE1.BottomPosition;
            }

            Color[] goldBasePalette = new Color[] { Color.Gold, Color.Orange, Color.DarkBrown };
            for (int x = 0; x < 22; x++)
            {
                for (int y = 0; y < 22; y++)
                {
                    int currentHeight = activeCity.Heights[x, y];
                    bool isElevatorCell = false;

                    // Phase 2 Fix: Safely map drawHeight inside the elevator variable scope block
                    int drawHeight = currentHeight;

                    foreach (var ev in activeCity.Elevators)
                    {
                        if (ev.IsMapped && ev.CellX == x && ev.CellY == y)
                        {
                            isElevatorCell = true;
                            drawHeight = ev.CurrentPosition; // Safely reads the moving register inside scope
                            break;
                        }
                    }

                    if (drawHeight == 0 && !isElevatorCell)
                    {
                        if (displayGems && activeCity.Gems[x, y])
                        {
                            LevelTransform.Draw3DGem(x, y, currentHeight, scale, heightScale, offsetX, offsetY, rotationAngle, tiltFactor, Color.Yellow);
                        }
                        continue;
                    }

                    byte cellAttr = activeCity.Attributes[x, y];
                    

                    if (isElevatorCell && displayElevators)
                    {
                        // Use the active frame-driven height to extrude the elevator platform dynamically
                        int baseTileHeight = Math.Max(1, drawHeight);
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

                    // Task Step 2 Symmetric 3D Isometric Gem Marker Integration Pass [v0.95]
                    if (displayGems && !isElevatorCell && activeCity.Gems[x, y])
                    {
                        LevelTransform.Draw3DGem(x, y, currentHeight, scale, heightScale, offsetX, offsetY, rotationAngle, tiltFactor, Color.Yellow);
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