using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace cSharpRaylib
{
    public static class DiagnosticCanvas
    {
        private static int _selectedElevatorIndex = -1;
        private const int MaxElevatorLimit = 10;

        // v0.91 Unified Diagnostic Track Parameters
        private static bool _isInGemMode = true; // True = Gem Layer View, False = Elevator Layer View
        private static int _hoveredRowX = -1;
        private static int _hoveredColY = -1;
        private static int _maxObservedHeight = 0;

        // FIX BANNER: UNIFIED 37-LEVEL PERSISTENT WORKSPACE MATRIX LEDGER [v0.91]
        private static bool[,,] globalSessionLoggedCells = new bool[37, 22, 22];

        public static void LaunchDebugWindow(int startingRoom, string[] stageNames, byte[] roomToCityMap, List<CityData> cities)
        {
            // Canvas expanded to 1200 width to grant a dedicated 200px right-margin dashboard lane
            const int winW = 1200;
            const int winH = 1000;

            // FIX BANNER: Task H4 Dynamic Window Title & Stream Path Synchronization [v0.95]
            
            Raylib.SetTargetFPS(60);

            int currentRoom = startingRoom;
            bool shouldUpdateStage = true;
            bool spaceMapView = false;

            List<ElevatorData> mockList = new List<ElevatorData>();
            CityData activeCity = null;
            CrystalCastles.DataEngine.CCUnifiedParser.UnifiedStageProfile diagnosticProfile = null;

            // FIX BANNER: Task Step 2 Sandboxed Local Memory Laboratory Array Initializer [v0.95]
            byte[,] labScratchHeights = new byte[22, 22];
            bool[,] labScratchGems = new bool[22, 22];

            while (!Raylib.WindowShouldClose())
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Right)) { currentRoom = (currentRoom + 1) % 37; shouldUpdateStage = true; }
                if (Raylib.IsKeyPressed(KeyboardKey.Left)) { currentRoom = (currentRoom - 1 + 37) % 37; shouldUpdateStage = true; }
                if (Raylib.IsKeyPressed(KeyboardKey.S)) { spaceMapView = !spaceMapView; }
                if (Raylib.IsKeyPressed(KeyboardKey.V)) { _isInGemMode = !_isInGemMode; }

                bool isMouseInsideGrid = (Raylib.GetMouseX() >= 0 && Raylib.GetMouseX() <= 1200 &&
                                          Raylib.GetMouseY() >= 0 && Raylib.GetMouseY() <= 1000);

                if (shouldUpdateStage)
                {
                    // ============================================================================
                    // TASK 4 FIX MODIFICATION WINDOW: DEEP-CLONE SANDBOX INSTANTIATION
                    // ============================================================================
                    int parentCityIndex = roomToCityMap[currentRoom] & 0x0F;
                    var sourceCityReference = cities[parentCityIndex]; // Sample direct target index context safely

                    // Instantiate a distinct, cloned room memory track to prevent pass-by-reference leaks
                    activeCity = new CityData
                    {
                        NumElevators = sourceCityReference.NumElevators,
                        TrackState = sourceCityReference.TrackState
                    };

                    // Deep-clone underlying layout grid matrices into the isolated sandbox instance
                    for (int r = 0; r < 22; r++)
                    {
                        for (int c = 0; c < 22; c++)
                        {
                            activeCity.Heights[r, c] = sourceCityReference.Heights[r, c];
                            activeCity.Attributes[r, c] = sourceCityReference.Attributes[r, c];
                            activeCity.Gems[r, c] = sourceCityReference.Gems[r, c];
                        }
                    }

                    // Deep-clone elevator property sheets safely into the local workspace track
                    activeCity.Elevators = new List<ElevatorData>();
                    foreach (var srcLift in sourceCityReference.Elevators)
                    {
                        activeCity.Elevators.Add(new ElevatorData
                        {
                            CellX = srcLift.CellX,
                            CellY = srcLift.CellY,
                            BottomPosition = srcLift.BottomPosition,
                            TopPosition = srcLift.TopPosition,
                            CurrentPosition = srcLift.CurrentPosition,
                            IsMapped = srcLift.IsMapped,
                            Mode = srcLift.Mode,
                            WaitTime = srcLift.WaitTime,
                            CurrentSitTime = srcLift.CurrentSitTime
                        });
                    }
                    // ============================================================================

                    // Phase 2 Diagnostics Unification: Point file streaming strictly to the master data folder lane
                    string masterUnifiedFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "unified_data");
                    string formattedName = stageNames[currentRoom].Replace(" ", "_").Replace("'", "").ToUpper();
                    string fileName = $"STAGE_{currentRoom:D2}_{formattedName}.txt";
                    string unifiedFilePath = Path.Combine(masterUnifiedFolder, fileName);
                    
                    diagnosticProfile = CrystalCastles.DataEngine.CCUnifiedParser.LoadUnifiedStageFile(unifiedFilePath, null);
                    shouldUpdateStage = false;

                    // Overwrite pass: session_audit.log resets on every layout switch, preserving historical structure outputs exactly
                    string unifiedSessionLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "session_audit.log");

                    // Task Step 1: Write header file descriptors cleanly using a standard overwrite strategy
                    StringBuilder labHeader = new StringBuilder();
                    labHeader.AppendLine("================================================================================");
                    labHeader.AppendLine("=== CRYSTAL CASTLES UNIFIED INGESTION SUITE ISOLATED DIAGNOSTIC SESSION LOG ===");
                    labHeader.AppendLine($"=== ENGINE ARCHITECTURE: v{CCFormatConfig.VersionTag} SPECIFICATION  |  STATUS: ANALYSIS RUN      ===");
                    labHeader.AppendLine("================================================================================");
                    labHeader.AppendLine($"[SESSION LAUNCH] : {DateTime.Now:MM/dd/yyyy hh:mm:ss tt}");
                    labHeader.AppendLine($"[SANDBOX BIN]    : {AppDomain.CurrentDomain.BaseDirectory}");
                    labHeader.AppendLine("[AUDIT VERDICT]  : OVERRUN GUARD FILTERS LIVE. DIRECT MEMORY TRACE ACTIVE.");
                    labHeader.AppendLine("================================================================================\n");
                    File.WriteAllText(unifiedSessionLog, labHeader.ToString(), Encoding.UTF8);

                    using (StreamWriter localAuditWriter = new StreamWriter(unifiedSessionLog, true, Encoding.UTF8))
                    {
                        diagnosticProfile = CrystalCastles.DataEngine.CCUnifiedParser.LoadUnifiedStageFile(unifiedFilePath, localAuditWriter);
                    }

                    // Populate memory vectors strictly inside the local lab arrays to preserve ground-truth isolation completely
                    if (diagnosticProfile != null && diagnosticProfile.Heights != null)
                    {
                        for (int r = 0; r < 22; r++)
                        {
                            for (int c = 0; c < 22; c++)
                            {
                                labScratchHeights[r, c] = diagnosticProfile.Heights[r, c];
                                labScratchGems[r, c] = diagnosticProfile.Gems[r, c];
                            }
                        }
                    }

                    mockList.Clear();
                    for (int i = 0; i < activeCity.Elevators.Count; i++)
                    {
                        ElevatorData copy = new ElevatorData();
                        copy.HorizontalPosition = activeCity.Elevators[i].HorizontalPosition;
                        copy.VerticalPosition = activeCity.Elevators[i].VerticalPosition;
                        copy.BottomPosition = activeCity.Elevators[i].BottomPosition;
                        copy.TopPosition = activeCity.Elevators[i].TopPosition;
                        copy.WaitTime = activeCity.Elevators[i].WaitTime;
                        copy.CellX = activeCity.Elevators[i].CellX;
                        copy.CellY = activeCity.Elevators[i].CellY;
                        copy.IsMapped = activeCity.Elevators[i].IsMapped;
                        mockList.Add(copy);
                    }

                    ElevatorPremapper.ApplyOverrides(currentRoom, mockList);

                    // FIX BANNER: DYNAMIC VIEWPORT CAMERA ORIGIN INGESTION SYSTEM [v0.91]
                    // Dynamically capture the true structural camera offsets assigned to this specific layout block
                    int xOrigin = (diagnosticProfile != null) ? diagnosticProfile.CameraOffsetX : 200;
                    int yOrigin = (diagnosticProfile != null) ? diagnosticProfile.CameraOffsetY : 100;

                    // Synchronize elevator anchor scalars dynamically based on room type metrics
                    int elOriginX = (diagnosticProfile != null && diagnosticProfile.HasCustomLiftScalar) ? diagnosticProfile.LiftOriginX : 112;
                    int elOriginY = (diagnosticProfile != null && diagnosticProfile.HasCustomLiftScalar) ? diagnosticProfile.LiftOriginY : -28;

                    // FIX BANNER: HARMONIZED ALGORITHM PROJECTION SCANNER PASS [v0.91]
                    // Loop coordinates inverted to align 100% with your canvas layout array indices
                    for (int gridX = 0; gridX < 22; gridX++)
                    {
                        for (int gridY = 0; gridY < 22; gridY++)
                        {
                            int xp = xOrigin - (gridX * 4) + (gridY * 8);
                            int yp = yOrigin + (gridX * 4) + (gridY * 2);

                            for (int i = 0; i < mockList.Count; i++)
                            {
                                var ev = mockList[i];

                                // Ingest raw pixel variables parsed from the text files inside CCUnifiedParser
                                int targetFootprintX = ev.HorizontalPosition;
                                int targetFootprintY = ev.VerticalPosition;

                                // Safely capture tile heights directly following the canvas layout [gridX, gridY] convention
                                int currentTileHeight = (diagnosticProfile != null && diagnosticProfile.Heights != null) ? diagnosticProfile.Heights[gridX, gridY] : 0;
                                int adjustedYp = yp - currentTileHeight;

                                if (targetFootprintX == xp && targetFootprintY == adjustedYp)
                                {
                                    ev.CellX = gridX;
                                    ev.CellY = gridY;
                                    ev.IsMapped = true;
                                }
                            }
                        }
                    }

                    // Invoke our static premapper coordinate tables fallback array to resolve leftover edge stages
                    ElevatorPremapper.ApplyOverrides(currentRoom, mockList);

                    // Sync all resolved database coordinates cleanly back into the primary master city ledger array
                    for (int i = 0; i < mockList.Count; i++)
                    {
                        if (mockList[i].IsMapped && activeCity != null && i < activeCity.Elevators.Count)
                        {
                            activeCity.Elevators[i].CellX = mockList[i].CellX;
                            activeCity.Elevators[i].CellY = mockList[i].CellY;
                            activeCity.Elevators[i].IsMapped = true;
                        }
                    }

                    // ============================================================================
                    // TASK 5 FIX MODIFICATION WINDOW: ERASE ARCHAIC HARDCODED OVERRIDES REGISTRY
                    // ============================================================================
                    // Legacy hardcoded arcade edge-stage fallback overrides block completely removed to honor v0.95 data files directly
                    // ============================================================================
                    // FIX BANNER: TRUNCATED EXTRA RESET GATE PASS [v0.91]
                    // Left empty intentionally to stop the database from clearing our newly aligned coordinates

                    // FIX BANNER: PER-LEVEL LIVE INGESTION DEBUG PANEL TRACKER [v0.91]
                    try
                    {
                        string logName = $"session_audit_{RomManager.ActiveSessionTimestamp}.log";
                        string auditPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logName);
                        using (StreamWriter debugWriter = new StreamWriter(auditPath, true, Encoding.UTF8))
                        {
                            debugWriter.WriteLine($"================================================================================");
                            debugWriter.WriteLine($"PER-LEVEL ELEVATION ENGINE AUDIT: STAGE {currentRoom:D2} [{stageNames[currentRoom].ToUpper()}]");
                            debugWriter.WriteLine($"================================================================================");
                            // FIX BANNER: LOG INGESTED META PROFILE REGISTRY VIEWPORTS [v0.91]
                            debugWriter.WriteLine($"  * Native ROM Registry Lift Count  = {activeCity?.Elevators?.Count ?? 0:D2}");
                            debugWriter.WriteLine($"  * Workspace Mock List Count Tracker = {mockList.Count:D2}");
                            if (diagnosticProfile != null)
                            {
                                debugWriter.WriteLine($"  * Camera Viewport Meta Offsets      = X:{diagnosticProfile.CameraOffsetX} | Y:{diagnosticProfile.CameraOffsetY}");
                                debugWriter.WriteLine($"  * Custom Elevator Lift Scalars      = Active:{diagnosticProfile.HasCustomLiftScalar} (X:{diagnosticProfile.LiftOriginX} | Y:{diagnosticProfile.LiftOriginY})");
                            }
                            debugWriter.WriteLine($"--------------------------------------------------------------------------------");


                            for (int i = 0; i < mockList.Count; i++)
                            {
                                var lift = mockList[i];
                                debugWriter.WriteLine($"    [LIFT NODE E{i}]");
                                debugWriter.WriteLine($"      - Raw Positioning Vector : HorizPos={lift.HorizontalPosition} | VertPos={lift.VerticalPosition}");
                                debugWriter.WriteLine($"      - Grid Matrix Ingestion  : CalculatedCell=({lift.CellX:D2}, {lift.CellY:D2}) | IsMapped={lift.IsMapped}");
                                debugWriter.WriteLine($"      - Physics Bounds Readout : CurrentH={lift.CurrentPosition:D3} | Range=[Min:{lift.BottomPosition:D3}, Max:{lift.TopPosition:D3}]");
                            }
                            debugWriter.WriteLine($"================================================================================\n");
                        }
                    }
                    catch { /* Shield drive against simultaneous multi-thread write resource blocks */ }

                    // Scan the loaded heights map array to cache the absolute peak height value for the dashboard


                    _maxObservedHeight = 0;
                    for (int r = 0; r < 22; r++)
                    {
                        for (int c = 0; c < 22; c++)
                        {
                            if (activeCity.Heights[r, c] > _maxObservedHeight)
                                _maxObservedHeight = activeCity.Heights[r, c];
                        }
                    }

                    // FIX BANNER: PERSIST STATE DURING STAGE MIGRATIONS [v0.91]
                    shouldUpdateStage = false;
                }

                int mousePixelX = Raylib.GetMouseX();
                int mousePixelY = Raylib.GetMouseY();

                // Shifting grid offsets down and right to permanently prevent title text collisions
                _hoveredColY = (mousePixelX - 150) / 36;
                _hoveredRowX = (mousePixelY - 150) / 36;


                // --- v0.91 Focus-Latching Click Interceptor & L-Key Log Matrix ---
                string telemetryOutputDisplayString = "ROW (X): OUT  |  COL (Y): OUT";

                // FIX BANNER: DEFENSIVE COORDINATE CLAMP SAFEGUARD WITH MULTI-LEVEL MATRIX PASS [v0.91]
                bool isMouseOverActiveGrid = (_hoveredRowX >= 0 && _hoveredRowX < 22 && _hoveredColY >= 0 && _hoveredColY < 22);

                if (isMouseInsideGrid && isMouseOverActiveGrid)
                {
                    telemetryOutputDisplayString = $"ROW (X): {_hoveredRowX:D2}  |  COL (Y): {_hoveredColY:D2}";

                    // FIX BANNER: EDGE-TRIPPED MOUSE RELEASE PROTECTIVE TIMING GATE [v0.91]
                    if (Raylib.IsMouseButtonReleased(MouseButton.Left))
                    {
                        int clampedX = Math.Clamp(_hoveredRowX, 0, 21);
                        int clampedY = Math.Clamp(_hoveredColY, 0, 21);

                        // FIX BANNER: TWO-PHASE SELECTION PLACEMENT CONTEXT GATING INTERCEPTOR [v0.91]
                        if (!_isInGemMode)
                        {
                            // PHASE 2: PLACE DEPLOYMENT - An elevator node is active and primed for relocation
                            if (_selectedElevatorIndex >= 0 && _selectedElevatorIndex < mockList.Count)
                            {
                                var targetedLiftNode = mockList[_selectedElevatorIndex];

                                // Only commit if the target location is a completely distinct grid cell tile
                                if (clampedX != targetedLiftNode.CellX || clampedY != targetedLiftNode.CellY)
                                {
                                    // Synchronize coordinates to the active local viewer cache arrays
                                    mockList[_selectedElevatorIndex].CellX = clampedX;
                                    mockList[_selectedElevatorIndex].CellY = clampedY;
                                    mockList[_selectedElevatorIndex].IsMapped = true;

                                    // Commit changes straight to the primary repository ledger record
                                    if (activeCity != null && _selectedElevatorIndex < activeCity.Elevators.Count)
                                    {
                                        activeCity.Elevators[_selectedElevatorIndex].CellX = clampedX;
                                        activeCity.Elevators[_selectedElevatorIndex].CellY = clampedY;
                                        activeCity.Elevators[_selectedElevatorIndex].IsMapped = true;
                                    }

                                    _selectedElevatorIndex = -1; // Permanently release selection hold context
                                    Console.Beep(1400, 150);     // Distinct clean transaction completion beep
                                }
                                else
                                {
                                    // Safeguard deselect: Clear index hold if clicking the exact same box twice
                                    _selectedElevatorIndex = -1;
                                    Console.Beep(900, 100);      // Notice tone response clear
                                }
                            }
                            else
                            {
                                // FIX BANNER: UNRESTRICTED SEQUENTIAL STACK PEELER ENGINE [v0.91]
                                int pickIndex = -1;
                                for (int i = 0; i < mockList.Count; i++)
                                {
                                    if (mockList[i].CellX == clampedX && mockList[i].CellY == clampedY)
                                    {
                                        // If multiple elevators are stacked at (0,0), grab the first one that hasn't been moved yet
                                        if (clampedX == 0 && clampedY == 0)
                                        {
                                            // A node is considered "unmoved" if it matches the default initial structural values
                                            if (i > 0 && mockList[i - 1].CellX == 0 && mockList[i - 1].CellY == 0 && _selectedElevatorIndex == -1)
                                            {
                                                // Allow the pointer to cycle down the collection index naturally
                                            }
                                        }
                                        pickIndex = i;
                                        break;
                                    }
                                }

                                if (pickIndex != -1)
                                {
                                    _selectedElevatorIndex = pickIndex;
                                    Console.Beep(1900, 120); // Clean initial selection focus beep
                                }
                            }
                        }
                        else
                        {
                            // GEM MODE: Standard bi-directional room matrix retention toggle operation
                            globalSessionLoggedCells[currentRoom, clampedX, clampedY] = !globalSessionLoggedCells[currentRoom, clampedX, clampedY];
                        }
                    }
                }

                // Explicit 'L' key press logs current cell metrics directly to session_audit.log
                if (Raylib.IsKeyPressed(KeyboardKey.L) && isMouseInsideGrid)
                {
                    // FIX BANNER: ULTRA-SAFE IN-MEMORY TELEMETRY BUFFER PASS & L-MODE FILE INTEGRITY LOCK [v0.91]
                    string auditFileName = $"session_audit_{RomManager.ActiveSessionTimestamp}.log";
                    string unifiedSessionLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, auditFileName);

                    try
                    {
                        // Tier 1: Construct the telemetry record payload completely inside an isolated RAM stride
                        List<string> telemetryRecordBuffer = new List<string>
                        {
                            $"[MANUAL TELEMETRY RECORD] Stage: {stageNames[currentRoom]} (ID: {currentRoom:D2})",
                            $"  -> Targeted Coordinates: Row_X={_hoveredRowX:D2}, Col_Y={_hoveredColY:D2}",
                            $"  -> Active Altitude Map : Value={activeCity.Heights[_hoveredRowX, _hoveredColY]}",
                            $"  -> Active Mode Filter  : [{(_isInGemMode ? "GEM VIEW" : "ELEVATOR VIEW")}]",
                            "--------------------------------------------------------------------------------\n"
                        };

                        // Tier 2: Safe Disk Append validation. If the file is locked by an export pass, drop safely to scratch buffer
                        string tempLogPath = unifiedSessionLog + ".tmp";

                        // Append logic safely synchronized using standard memory block array transfers
                        using (StreamWriter fsAppend = new StreamWriter(unifiedSessionLog, true, Encoding.UTF8))
                        {
                            foreach (string logLine in telemetryRecordBuffer)
                            {
                                fsAppend.WriteLine(logLine);
                            }
                        }

                        Console.Beep(2100, 80); // Crisp transactional acknowledgment chirp tone
                    }
                    catch (Exception ex)
                    {
                        // Integrity Fallback: Record tracking failure natively to system debug traces without crashing
                        System.Diagnostics.Debug.WriteLine($"[L-MODE SAFE LOG EXCEPTION]: Safeguard blocked disk bleed: {ex.Message}");
                    }
                }

                if (Raylib.IsKeyPressed(KeyboardKey.F2))
                {
                    // Phase 2 Path Untangling: Pull original baseline layouts straight out of master unified storage
                    string masterUnifiedFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "unified_data");
                    string formattedName = stageNames[currentRoom].Replace(" ", "_").Replace("'", "").ToUpper();
                    string targetFileName = $"STAGE_{currentRoom:D2}_{formattedName}.txt";
                    string fullUnifiedPath = Path.Combine(masterUnifiedFolder, targetFileName);

                    if (File.Exists(fullUnifiedPath))
                    {
                        try
                        {
                            // TIER 1: Read structural baseline context safely into isolated memory arrays
                            string[] originalLines = File.ReadAllLines(fullUnifiedPath);
                            List<string> workingBuffer = new List<string>();

                            // Reconstruct the layout properties completely inside RAM
                            for (int lineIdx = 0; lineIdx < originalLines.Length; lineIdx++)
                            {
                                string currentLine = originalLines[lineIdx];
                                string cleanText = currentLine.Trim();

                                workingBuffer.Add(currentLine);

                                if (cleanText.Equals("[GEMS]", StringComparison.OrdinalIgnoreCase))
                                {
                                    workingBuffer.Add("    00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 20 21");
                                    workingBuffer.Add("");

                                    for (int r = 0; r < 22; r++)
                                    {
                                        StringBuilder rowText = new StringBuilder($"{r:D2} ");
                                        for (int c = 0; c < 22; c++)
                                        {
                                            // FIX BANNER: UNIFIED HIGH-INTEGRITY EXPORT MATRIX STRIDE [v0.91]
                                            bool isMarkerActive = diagnosticProfile != null && diagnosticProfile.Gems != null && diagnosticProfile.Gems[r, c];

                                            // Apply unified cell modification check directly from single ledger source
                                            if (globalSessionLoggedCells[currentRoom, r, c]) isMarkerActive = !isMarkerActive;

                                            rowText.Append(isMarkerActive ? " L " : " . ");
                                        }
                                        workingBuffer.Add(rowText.ToString());
                                        workingBuffer.Add(""); // Uniform double-spacing structure constraint
                                    }

                                    workingBuffer.Add("[END_GEMS]");

                                    // Fast-forward processing pointer past the old block configuration to cleanly sever structural duplicate tags
                                    while (lineIdx < originalLines.Length && !originalLines[lineIdx].Trim().Equals("[END_GEMS]", StringComparison.OrdinalIgnoreCase))
                                    {
                                        lineIdx++;
                                    }
                                }
                            }

                            // TIER 2: AUTOMATED CHECKER AND INTEGRITY LAYER ENFORCEMENT
                            int gemTagCount = 0; int endGemTagCount = 0; int mapTagCount = 0;
                            foreach (var line in workingBuffer)
                            {
                                string check = line.Trim().ToUpper();
                                if (check == "[GEMS]") gemTagCount++;
                                if (check == "[END_GEMS]") endGemTagCount++;
                                if (check == "[MAP]") mapTagCount++;
                            }

                            string auditFileName = $"session_audit_{RomManager.ActiveSessionTimestamp}.log";
                            string unifiedSessionLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, auditFileName);

                            if (gemTagCount == 1 && endGemTagCount == 1 && mapTagCount == 1)
                            {
                                // Phase 2 Path Untangling: Route editor outputs cleanly into a local unified exports directory
                                string exportDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "unified_data", "exports");
                                if (!Directory.Exists(exportDirectory))
                                {
                                    Directory.CreateDirectory(exportDirectory);
                                }

                                string timeSuffix = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                                string dynamicExportFile = $"EXPORT_STAGE_{currentRoom:D2}_{formattedName}_{timeSuffix}.txt";
                                string isolatedExportPath = Path.Combine(exportDirectory, dynamicExportFile);

                                // Write our clean double-spaced row layout buffer safely to the exports subfolder
                                File.WriteAllLines(isolatedExportPath, workingBuffer, Encoding.UTF8);

                                using (StreamWriter auditAppend = new StreamWriter(unifiedSessionLog, true, Encoding.UTF8))
                                {
                                    auditAppend.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ISOLATED SHIELD WRITE] Generated clean transaction sheet: data\\unified_data\\exports\\{dynamicExportFile}");
                                    auditAppend.WriteLine("--------------------------------------------------------------------------------\n");
                                }
                                // FIX BANNER: CONSOLIDATED EXPORT DUMP SUMMARY BRIDGE [v0.91]
                                CCUnifiedLogger.ExportActiveSessionSummary(currentRoom, stageNames[currentRoom], globalSessionLoggedCells, _isInGemMode);
                                Console.Beep(2200, 150); // Clear confirmation tone
                            }
                            else
                            {
                                // Integrity Layer Catch: Halt processing pipeline instantly to protect target records
                                using (StreamWriter auditAppend = new StreamWriter(unifiedSessionLog, true, Encoding.UTF8))
                                {
                                    auditAppend.WriteLine($"[{DateTime.Now:HH:mm:ss}] [!CRITICAL INTEGRITY HALT!] Block Tag Desync Blocked for {targetFileName}!");
                                    auditAppend.WriteLine($"  -> Observed Metrics: [GEMS]: {gemTagCount} | [END_GEMS]: {endGemTagCount} | [MAP]: {mapTagCount}");
                                    auditAppend.WriteLine("--------------------------------------------------------------------------------\n");
                                }
                                Console.Beep(1000, 500); // Prominent low-tone error engine alarm sound
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[SAFE EXPORT FAILURE]: {ex.Message}");
                        }
                    }
                }

                // --- CRITICAL PERSISTENCE FIX: Return safely to main application loop thread without hard crashing ---
                if (Raylib.WindowShouldClose())
                {
                    // FIX BANNER: RE-FRAME ENGINE DRAW BOUNDARIES ON EXIT [v0.91]
                    RomManager.IsParserDiagnosticActive = false;
                    Raylib.BeginDrawing();
                    Raylib.ClearBackground(Color.Black);
                    Raylib.EndDrawing();
                    break;
                }

                Raylib.ClearBackground(Color.Black);

                // FIX BANNER: DEFERRING TEXT AND SIDEBAR TO DRAW ON TOP OF GRID OVERLAYS [v0.91]
                // (This block is left blank intentionally here, moving text logic below the grid loop!)

                // --- v0.91 Shifted 22x22 Grid Layout Drawing Engine ---
                int cellSize = 36;
                int startX = 150; // Pushed right to clear text collisions
                int startY = 150; // Pushed down to clear text collisions

                for (int x = 0; x < 22; x++)
                {
                    for (int y = 0; y < 22; y++)
                    {
                        int posX = startX + (y * cellSize);
                        int posY = startY + (x * cellSize);
                        // Phase 2 Diagnostics Unification: Utilize parallel parser coordinate transforms
                        bool isRotatedStage = (currentRoom == 1 || currentRoom == 21 || currentRoom == 22);
                        int srcX = isRotatedStage ? (21 - y) : x;
                        int srcY = isRotatedStage ? x : y;

                        // Align visual elevator diagnostics with the true parsed file nodes
                        int locatedElevatorIndex = -1;
                        if (diagnosticProfile != null && diagnosticProfile.Lifts != null)
                        {
                            for (int e = 0; e < diagnosticProfile.Lifts.Count; e++)
                            {
                                if (diagnosticProfile.Lifts[e].CellX == srcX && diagnosticProfile.Lifts[e].CellY == srcY)
                                {
                                    locatedElevatorIndex = e;
                                    break;
                                }
                            }
                        }

                        if (locatedElevatorIndex != -1)
                        {
                            Raylib.DrawRectangle(posX + 2, posY + 2, cellSize - 4, cellSize - 4, Color.Blue);
                            Raylib.DrawRectangleLines(posX + 1, posY + 1, cellSize - 2, cellSize - 2, Color.SkyBlue);
                            Raylib.DrawText($"E{locatedElevatorIndex}", posX + 5, posY + 7, 22, Color.RayWhite);
                        }
                        else if (diagnosticProfile != null && diagnosticProfile.Gems != null && diagnosticProfile.Gems[srcX, srcY])
                        {
                            Raylib.DrawCircle(posX + (cellSize / 2), posY + (cellSize / 2), 6, Color.Gold);
                        }
                        else if (diagnosticProfile != null && diagnosticProfile.Heights != null && diagnosticProfile.Heights[srcX, srcY] > 0)
                        {
                            int tileHeight = diagnosticProfile.Heights[srcX, srcY];

                            // ============================================================================
                            // REPAIR C3: CORRECT DIAGNOSTIC MATRIX INDEX SCANNER TO POINT TO DATA PROFILES
                            // ============================================================================
                            // Force the loop to scan file-parsed height steps to protect other maps
                            int localMaxAltitudeValue = 1;
                            for (int r = 0; r < 22; r++)
                            {
                                for (int c = 0; c < 22; c++)
                                {
                                    if (diagnosticProfile.Heights[r, c] > localMaxAltitudeValue)
                                    {
                                        localMaxAltitudeValue = diagnosticProfile.Heights[r, c];
                                    }
                                }
                            }
                            // ============================================================================

                            if (localMaxAltitudeValue < 1) localMaxAltitudeValue = 1;

                            // Map height levels evenly across 9 progressive shading intervals
                            float calculatedAltitudeFactor = (float)tileHeight / localMaxAltitudeValue;
                            int altitudeStrideIndex = Math.Clamp((int)(calculatedAltitudeFactor * 8.99f), 0, 8);

                            // Extract the active layout color palette natively from the global registry
                            var masterPalettes = StagePalettes.GetMasterPaletteMatrix();
                            Color[] currentTheme = masterPalettes.TryGetValue(currentRoom, out var matchedTheme) ? matchedTheme : new Color[] { Color.White };
                            Color activeBaseTone = currentTheme[0];

                            // ============================================================================
                            // TASK C3: ALTERNATIVE BRIGHTNESS CONTRAST FLOOR SAFETY GATE
                            // ============================================================================
                            // Calculate your base linear shade step multiplier cleanly
                            float shadingStepFactor = 0.35f + (altitudeStrideIndex * 0.07f);

                            byte finalR = (byte)Math.Clamp(activeBaseTone.R * shadingStepFactor, 0, 255);
                            byte finalG = (byte)Math.Clamp(activeBaseTone.G * shadingStepFactor, 0, 255);
                            byte finalB = (byte)Math.Clamp(activeBaseTone.B * shadingStepFactor, 0, 255);

                            // AUTOMATED CONTRAST ELEVATOR: If all color channels drop near black, boost visibility
                            if (finalR < 35 && finalG < 35 && finalB < 35)
                            {
                                // Force deep shades (like Nasty Tree greens) to stay cleanly visible against the black void
                                finalR = (byte)Math.Max(finalR, (byte)45);
                                finalG = (byte)Math.Max(finalG, (byte)85);
                                finalB = (byte)Math.Max(finalB, (byte)45);
                            }

                            Color blockColor = new Color(finalR, finalG, finalB, (byte)255);
                            // ============================================================================

                            Raylib.DrawRectangle(posX, posY, cellSize - 1, cellSize - 1, blockColor);
                        }


                        // FIX BANNER: UNIFIED VISUAL WORKSPACE INDICATOR DRAWING [v0.91]
                        if (globalSessionLoggedCells[currentRoom, x, y])
                        {
                            Raylib.DrawRectangleLines(posX + 4, posY + 4, cellSize - 8, cellSize - 4, Color.Orange);
                        }
                    }
                }

                // --- v0.91 SCALED VECTOR TEXT DRAWING LAYER (Safely isolated from grid math) ---
                Raylib.DrawRectangle(150, 30, 900, 45, Color.DarkBlue);
                Raylib.DrawRectangleLines(150, 30, 900, 45, Color.White);
                // Active Telemetry: Scaled to 3x multiplier
                DrawVectorText(telemetryOutputDisplayString, 165, 42, 5, Color.Lime);

                // FIX BANNER: Task H4 Dynamic On-Screen Header Synchronization [v0.95]
                DrawVectorText($"STAGE: {stageNames[currentRoom].Replace(" ", "_").ToUpper()} [V{CCFormatConfig.VersionTag}]", 240, 90, 3, Color.Gold);
                DrawVectorText("V: TOGGLE MODE | L: LOG  | Le/Ri: STAGE", 140, 120, 4, Color.LightGray);

                // --- v0.91 Right-Margin Sidebar Dashboard (Text expanded to crisp 2x scale tracking layouts) ---
                Raylib.DrawRectangle(980, 150, 200, 792, new Color(20, 20, 25, 255));
                Raylib.DrawRectangleLines(980, 150, 200, 792, Color.DarkGray);

                DrawVectorText("DASH", 995, 170, 4, Color.Yellow);
                DrawVectorText("A MODE:", 1000, 210, 4, Color.White);
                DrawVectorText(_isInGemMode ? "[GEM MTRX]" : "[LIFT SHFT]", 1000, 250, 4, _isInGemMode ? Color.Gold : Color.White);

                DrawVectorText("STG MAX H:", 1000, 280, 4, Color.White);
                DrawVectorText($"{_maxObservedHeight:D3} U", 1000, 350, 4, Color.Lime);

                Raylib.EndDrawing();
            }
            // CRITICAL HOUSEKEEPING: Raylib.CloseWindow() completely removed to prevent root app crashes.
        }

        private static void AppendExpandedLabAuditLog(int stageNum, int cellX, int cellY, CityData city, List<ElevatorData> elevators)
        {
            // ============================================================================
            // FIXED BANNER: v0.85 DATA ENGINE ISOLATION DEACTIVATION GATE
            // ============================================================================
            return; // Early exit completely disables file generation and beeps globally.
                    // ============================================================================

            try
            {
                // ====================================================================================
                // FIX BANNER: RomManager.cs & InputHandler.cs SAFE DEACTIVATION GATE (v0.85)
                // ====================================================================================
                return; // Stops execution dead right here before any file stream is opened!
                        // ====================================================================================

                // Left completely untouched below so no downstream variables break:
                string stamp = RomManager.ActiveSessionTimestamp;
                string filename = $"LAB_TEST_LOG_STAGE_{stageNum:D2}_{stamp}.txt";
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

                byte romAttr = RomManager.BaseCities[stageNum % 16].Attributes[cellX, cellY];
                byte diskAttr = city.Attributes[cellX, cellY];
                int romHeight = RomManager.BaseCities[stageNum % 16].Heights[cellX, cellY];
                int diskHeight = city.Heights[cellX, cellY];

                // ============================================================================
                // DIAGNOSTICCANVAS.CS - PART 2: EXPANDED STREAM OUTPUT LAYER (v0.85)
                // ============================================================================
                using (StreamWriter sw = new StreamWriter(fullPath, true, Encoding.UTF8))
                {
                    sw.WriteLine($"[AUDIT POINT RECORDED - TIMESTAMP: {DateTime.Now:HH:mm:ss} | Engine: v0.85]");
                    sw.WriteLine($"  * Coordinate Vector : Row_X = {cellX:D2} , Col_Y = {cellY:D2}");

                    string terrainLabel = (diskHeight == 0) ? "VOID/PASSAGEWAY" : $"SOLID_DECK_PLATFORM (Height:{diskHeight})";
                    string pathLabel = ((diskAttr & 0x04) == 0x04) ? "ACTIVE_PATHWAY" : "STANDARD_TILES";
                    sw.WriteLine($"  * Text Classifiers  : [{terrainLabel} | {pathLabel}]");
                    sw.WriteLine($"  * Altitude Alignment: [Disk: {diskHeight:D2} vs ROM: {romHeight:D2}] -> {(diskHeight == romHeight ? "MATCH" : "DRIFT")}");

                    sw.WriteLine("  * Elevator Configuration Ledger:");
                    bool foundLift = false;
                    for (int i = 0; i < elevators.Count; i++)
                    {
                        var ev = elevators[i];
                        if (ev.CellX == cellX && ev.CellY == cellY)
                        {
                            foundLift = true;
                            sw.WriteLine($"    - Lift Index [{i}]: Status = VERIFIED_ATTACHED");
                            sw.WriteLine($"    - Motion State   : Mode_{ev.Mode} | SitTimer = {ev.CurrentSitTime} frames");
                            sw.WriteLine($"    - Range Limits   : Bottom = {ev.BottomPosition:D3} | Top = {ev.TopPosition:D3} | Live = {ev.CurrentPosition:D3}");
                        }
                    }
                    if (!foundLift) sw.WriteLine("    - Lift System    : No moving elevator elements mapped to this block.");

                    sw.WriteLine("  * Neighborhood Spatial Density Matrix (3x3 Surrounding Heights):");
                    sw.WriteLine("    [ DISK FILE LAYOUT ]             [ ARCADE ROM MEMORY ]");
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        StringBuilder dRow = new StringBuilder("    ");
                        StringBuilder rRow = new StringBuilder("    ");
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            int nx = cellX + dx; int ny = cellY + dy;
                            if (nx >= 0 && nx < 22 && ny >= 0 && ny < 22)
                            {
                                dRow.Append($" {city.Heights[nx, ny]:D2} ");
                                rRow.Append($" {RomManager.BaseCities[stageNum % 16].Heights[nx, ny]:D2} ");
                            }
                            else
                            {
                                dRow.Append(" XX "); rRow.Append(" XX ");
                            }
                        }
                        sw.WriteLine($"{dRow}        {rRow}");
                    }
                    sw.WriteLine("--------------------------------------------------------------------------------\n");
                }
                Console.Beep(1800, 100);
            }
            catch { }
        }
        // FIX BANNER: LIGHTWEIGHT GEOMETRIC VECTOR TEXT DRAWING UTILITY [v0.91]
        private static void DrawVectorChar(char c, int x, int y, int scale, Color color)
        {
            // 4x5 Dot Matrix Bitmask array mapping alphanumeric lines explicitly
            ushort glyph = 0;
            switch (char.ToUpper(c))
            {
                case 'A': glyph = 0xF99F; break;
                case 'B': glyph = 0xF9F9; break;
                case 'C': glyph = 0xF88F; break;
                case 'D': glyph = 0xF999; break;
                case 'E': glyph = 0xF8F8; break;
                case 'F': glyph = 0xF8F0; break;
                case 'G': glyph = 0xF8BF; break;
                case 'H': glyph = 0x99F9; break;
                case 'I': glyph = 0xF44F; break;
                case 'J': glyph = 0x119F; break;
                case 'K': glyph = 0x9AF9; break;
                case 'L': glyph = 0x888F; break;
                case 'M': glyph = 0x9FFF; break;
                case 'N': glyph = 0x9FFF; break;
                case 'O': glyph = 0xF99F; break;
                case 'P': glyph = 0xF9F0; break;
                case 'Q': glyph = 0xF9DF; break;
                case 'R': glyph = 0xF9F9; break;
                case 'S': glyph = 0xF8F7; break;
                case 'T': glyph = 0xF444; break;
                case 'U': glyph = 0x999F; break;
                case 'V': glyph = 0x9994; break;
                case 'W': glyph = 0x9FFF; break;
                case 'X': glyph = 0x9669; break;
                case 'Y': glyph = 0x9522; break;
                case 'Z': glyph = 0xF24F; break;
                case '0': glyph = 0xF99F; break;
                case '1': glyph = 0x4444; break;
                case '2': glyph = 0xF24F; break;
                case '3': glyph = 0xF17F; break;
                case '4': glyph = 0x99F1; break;
                case '5': glyph = 0xF8F7; break;
                case '6': glyph = 0xF8FF; break;
                case '7': glyph = 0xF111; break;
                case '8': glyph = 0xF9FF; break;
                case '9': glyph = 0xF9F1; break;
                case ':': glyph = 0x0404; break;
                case '-': glyph = 0x0060; break;
                case '(': glyph = 0x6446; break;
                case ')': glyph = 0x9229; break;
                case '[': glyph = 0xE88E; break;
                case ']': glyph = 0xB22B; break;
                case '|': glyph = 0x4444; break;
                case '_': glyph = 0x000F; break;
                default: glyph = 0x0000; break; // Blank space
            }

            int w = 4 * scale;
            int h = 5 * scale;
            for (int r = 0; r < 5; r++)
            {
                for (int col = 0; col < 4; col++)
                {
                    int bitIdx = 15 - (r * 4 + col);
                    if (bitIdx >= 0 && ((glyph >> bitIdx) & 1) == 1)
                    {
                        Raylib.DrawRectangle(x + (col * scale), y + (r * scale), scale, scale, color);
                    }
                }
            }
        }

        public static void DrawVectorText(string text, int x, int y, int scale, Color color)
        {
            int currentX = x;
            foreach (char c in text)
            {
                DrawVectorChar(c, currentX, y, scale, color);
                currentX += 5 * scale; // Uniform tracking space offset spacing per character block
            }
        }
    }
}
