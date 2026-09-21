// ====================================================================================
// FIX BLOCK 1: INPUTHANDLER.CS - FIELDS ALLOCATION & PERSISTENT LEVEL-RESTORE HOOK
// LOCATION: TARGET ALL SOURCE MATERIAL FROM LINE 12 DOWN TO LINE 45
// ====================================================================================
using Raylib_cs;
using System.Numerics;
using System.Text;

// ====================================================================================
// DIAGNOSTIC PART 1: INPUTHANDLER.CS - FIELDS, KEY-INTERCEPT & EVENT LOGGER ENGINE
// LOCATION: TARGET REGIONS FROM LINE 12 DOWN TO THE END OF THE HANDLEKEYS FUNCTION
// ====================================================================================
namespace cSharpRaylib
{
    public static class InputHandler
    {
        private static Vector2 _probeMouseScreenPos;
        private static bool _lastIs3DMode = false;
        private static int _globalPointIncrementer = 0;
        private static int _lastRecordedStageId = -1;

        public static int ProbeGridX { get; private set; } = -1;
        public static int ProbeGridY { get; private set; } = -1;
        public static bool IsProbeInsideWorkspace { get; private set; } = false;
        public static bool IsProbeLockedToGrid { get; private set; } = false;
        public static bool IsStageCommitted { get; private set; } = false;

        private static bool[,,] LevelMarkerArchive = new bool[37, 22, 22];
        public static bool[] StageCommitStatus { get; private set; } = new bool[37];

        // NEW v0.90 FOCUS-EVENT TELEMETRY TRACKER: Disables 430KB noise, captures raw state flips
        private static void LogDiagnosticEvent(string eventDescription)
        {
            try
            {
                string filename = $"session_audit_{RomManager.ActiveSessionTimestamp}.log";
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
                using (StreamWriter sw = new StreamWriter(fullPath, true, Encoding.UTF8))
                {
                    sw.WriteLine($"[DIAGNOSTIC EVENT - {DateTime.Now:HH:mm:ss.fff}] {eventDescription}");
                }
            }
            catch { }
        }

        private static int CountActiveRoomMarkers(int roomID)
        {
            if (roomID < 0 || roomID >= 37) return 0;
            int count = 0;
            for (int x = 0; x < 22; x++)
                for (int y = 0; y < 22; y++)
                    if (LevelMarkerArchive[roomID, x, y]) count++;
            return count;
        }

        public static void HandleKeys(
            ref int currentRoom, ref bool is3DMode, ref float globalScale,
            ref float heightMultiplier, ref int panOffsetX, ref int panOffsetY,
            ref int rotationAngle, ref float tiltFactor, ref int renderStyleMode,
            ref bool displayPathOverlays, ref bool displayGems, ref bool showDanLegacyOverlay,
            ref bool invertBackground, ref bool exportTextFlag, ref bool trigger3DLabFlag)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Right) || Raylib.IsKeyPressed(KeyboardKey.Left))
            {
                int oldRoom = currentRoom;
                int preExitCount = CountActiveRoomMarkers(oldRoom);

                if (Raylib.IsKeyPressed(KeyboardKey.Right)) currentRoom = (currentRoom + 1) % 37;
                else currentRoom = (currentRoom - 1 + 37) % 37;

                IsStageCommitted = StageCommitStatus[currentRoom];
                LogDiagnosticEvent($"ROOM_MIGRATION_START: Swapping from Room [{oldRoom:D2}] (Cached Count: {preExitCount}) -> Room [{currentRoom:D2}]");

                RestoreActiveLevelMarkers(currentRoom);

                int postHydrateCount = 0;
                for (int x = 0; x < 22; x++)
                    for (int y = 0; y < 22; y++)
                        if (Program.MainLoggedCells[x, y]) postHydrateCount++;

                LogDiagnosticEvent($"ROOM_MIGRATION_END: Arrived at Room [{currentRoom:D2}]. Canvas Active 'L' Count: {postHydrateCount} | Archive Cache Count: {CountActiveRoomMarkers(currentRoom)}");
                return;
            }

            if (Raylib.IsKeyDown(KeyboardKey.KpAdd)) globalScale += 0.02f;
            if (Raylib.IsKeyDown(KeyboardKey.KpSubtract)) globalScale -= 0.02f;

            if (Raylib.IsKeyPressed(KeyboardKey.Up)) is3DMode = true;
            if (Raylib.IsKeyPressed(KeyboardKey.Down)) is3DMode = false;

            if (Raylib.IsKeyPressed(KeyboardKey.M)) { renderStyleMode = (renderStyleMode + 1) % 3; }
            if (Raylib.IsKeyPressed(KeyboardKey.P)) { displayPathOverlays = !displayPathOverlays; }
            if (Raylib.IsKeyPressed(KeyboardKey.G)) { displayGems = !displayGems; }
            if (Raylib.IsKeyPressed(KeyboardKey.V)) { invertBackground = !invertBackground; }

            if (Raylib.IsKeyPressed(KeyboardKey.C))
            {
                if (currentRoom >= 0 && currentRoom < 37)
                {
                    StageCommitStatus[currentRoom] = !StageCommitStatus[currentRoom];
                    IsStageCommitted = StageCommitStatus[currentRoom];
                    LogDiagnosticEvent($"COMMIT_KEY_TRIGGERED: Room [{currentRoom:D2}] Write-Lock Flag Set To [{IsStageCommitted.ToString().ToUpper()}] | Total Logged Active Elements: {CountActiveRoomMarkers(currentRoom)}");
                }
            }

            trigger3DLabFlag = false;
            exportTextFlag = false;

            if (is3DMode)
            {
                if (Raylib.IsKeyDown(KeyboardKey.W)) heightMultiplier += 0.05f;
                if (Raylib.IsKeyDown(KeyboardKey.S)) heightMultiplier -= 0.05f;
                if (Raylib.IsKeyDown(KeyboardKey.I)) panOffsetY -= 4;
                if (Raylib.IsKeyDown(KeyboardKey.K)) panOffsetY += 4;
                if (Raylib.IsKeyDown(KeyboardKey.J)) panOffsetX -= 4;
                if (Raylib.IsKeyDown(KeyboardKey.L)) panOffsetX += 4;
                if (Raylib.IsKeyDown(KeyboardKey.A)) rotationAngle = (rotationAngle - 2 + 360) % 360;
                if (Raylib.IsKeyDown(KeyboardKey.D)) rotationAngle = (rotationAngle + 2) % 360;
                if (Raylib.IsKeyDown(KeyboardKey.Q)) tiltFactor = Math.Max(0.4f, tiltFactor - 0.02f);
                if (Raylib.IsKeyDown(KeyboardKey.E)) tiltFactor = Math.Min(2.0f, tiltFactor + 0.02f);
            }

            if (Raylib.IsKeyPressed(KeyboardKey.R))
            {
                globalScale = 1.0f; heightMultiplier = 1.8f; panOffsetX = 0; panOffsetY = 0;
                rotationAngle = 0; tiltFactor = 1.0f; renderStyleMode = 0;
                displayPathOverlays = false; displayGems = true; showDanLegacyOverlay = false;
                invertBackground = false; IsProbeLockedToGrid = false;
                if (currentRoom >= 0 && currentRoom < 37) StageCommitStatus[currentRoom] = false;
                IsStageCommitted = false;
                LogDiagnosticEvent($"RESET_VIEW_EXECUTION: Repositioned coordinates for Room [{currentRoom:D2}] to baseline standards.");
            }
        }
        // ====================================================================================
        // END OF DIAGNOSTIC PART 1
        // ====================================================================================

        // ====================================================================================
        // DIAGNOSTIC PART 2: INPUTHANDLER.CS - RESTORE HOOK & DUAL-MIRROR MOUSE CLICK LOGGER
        // LOCATION: TARGET REGIONS FROM LINE 86 DOWN TO THE 2D VIEWPORT CELL CALCULATION LIMITS
        // ====================================================================================
        private static void RestoreActiveLevelMarkers(int targetRoom)
        {
            if (targetRoom < 0 || targetRoom >= 37) return;
            LogDiagnosticEvent($"ARCHIVE_RESTORATION_PASS: Hydrating active drawing canvas memory layer for Room [{targetRoom:D2}].");
            for (int x = 0; x < 22; x++)
            {
                for (int y = 0; y < 22; y++)
                {
                    Program.MainLoggedCells[x, y] = LevelMarkerArchive[targetRoom, x, y];
                }
            }
        }

        // ====================================================================================
        // FIX BANNER: INPUTHANDLER.CS - DIRECT MOUSE CLICK PIPELINE CORRECTION
        // LOCATION: REPLACES SUB-CONDITIONAL CLICK BLOCK IN TrackMouseProbeCoordinates (APPROX LINE 105)
        // ====================================================================================
        public static void TrackMouseProbeCoordinates(float screenX, float screenY,
            float scale, int offsetX, int offsetY, int rotationAngle, float tiltFactor,
            CityData activeCity, bool is3DMode, int currentRoom)
        {
            if (is3DMode != _lastIs3DMode) { _lastIs3DMode = is3DMode; return; }

            // Restored direct-action click pipeline: Decouples target locking flags from core data toggles
            if (Raylib.IsMouseButtonPressed(MouseButton.Left) && !IsStageCommitted)
            {
                if (IsProbeInsideWorkspace || IsProbeLockedToGrid)
                {
                    if (activeCity != null)
                    {
                        bool preClickState = Program.MainLoggedCells[ProbeGridX, ProbeGridY];

                        // Execute synchronized multi-stage retention writes immediately on click pass
                        Program.MainLoggedCells[ProbeGridX, ProbeGridY] = !Program.MainLoggedCells[ProbeGridX, ProbeGridY];
                        LevelMarkerArchive[currentRoom, ProbeGridX, ProbeGridY] = Program.MainLoggedCells[ProbeGridX, ProbeGridY];

                        LogDiagnosticEvent($"MOUSE_CLICK_EVENT: Room [{currentRoom:D2}] Grid Target [X:{ProbeGridX:D2}, Y:{ProbeGridY:D2}] Toggled. " +
                                           $"CanvasState: ({preClickState.ToString().ToUpper()} -> {Program.MainLoggedCells[ProbeGridX, ProbeGridY].ToString().ToUpper()}) | " +
                                           $"ArchiveState: ({LevelMarkerArchive[currentRoom, ProbeGridX, ProbeGridY].ToString().ToUpper()}) | " +
                                           $"Room Persistent Count: {CountActiveRoomMarkers(currentRoom)}");
                    }

                    // Toggle the viewport positioning lock cleanly after data assignment completes
                    IsProbeLockedToGrid = !IsProbeLockedToGrid;
                }
            }

            if (IsProbeLockedToGrid) return;
        }
// ====================================================================================
// END OF DIRECT MOUSE CLICK PIPELINE CORRECTION
// ====================================================================================
// ============================================================================
// FIX BANNER: INPUTHANDLER.CS - DRAWCONTROLOVERLAY FUNCTION (PART 1 OF 2)
// CONSTRAINTS: COMPACT LINE SAFETY CUTOFF PROTECTION | MAX 65 LINES
// ============================================================================
        public static void DrawControlOverlay(bool is3DMode, int renderStyle, bool pathsOn, bool gemsOn, bool legacyOverlayOn,
        bool isInverted, float globalScale, int currentRoom, string stageName, int totalElevators, Raylib_cs.Color[] activeTheme)
        {
            Raylib_cs.Color cardBg = isInverted ? new Raylib_cs.Color(230, 230, 230, 220) : new Raylib_cs.Color(20, 20, 20, 200);
            Raylib_cs.Color cardBorder = isInverted ? Raylib_cs.Color.DarkGray : Raylib_cs.Color.LightGray;
            Raylib_cs.Color textClr = isInverted ? Raylib_cs.Color.Black : Raylib_cs.Color.RayWhite;
            Vector2 guiAnchor = new Vector2(15, 10);

            Raylib.DrawText("ccSharpRaylib [v0.90]", (int)guiAnchor.X, (int)guiAnchor.Y, 20, textClr);
            Raylib.DrawText($"Level {(currentRoom / 4) + 1} - {(currentRoom % 4) + 1} [{stageName}] | Lifts: {totalElevators} | {(is3DMode ? "3D Isometric" : "2D Flat")}", (int)guiAnchor.X, (int)guiAnchor.Y + 35, 18, Raylib_cs.Color.Gold);

            if (activeTheme != null && activeTheme.Length >= 3)
            {
                // Un-indexed array loops to natively separate and display all 3 distinct stage palette chips
                Raylib.DrawRectangle((int)guiAnchor.X, (int)guiAnchor.Y + 70, 40, 20, activeTheme[0]);
                Raylib.DrawRectangle((int)guiAnchor.X + 50, (int)guiAnchor.Y + 70, 40, 20, activeTheme[1]);
                Raylib.DrawRectangle((int)guiAnchor.X + 100, (int)guiAnchor.Y + 70, 40, 20, activeTheme[2]);
            }
            Raylib.DrawText("Active Layout Palette Matrix Slots (v0.90)", (int)guiAnchor.X + 160, (int)guiAnchor.Y + 73, 14, isInverted ? Raylib_cs.Color.DarkGray : Raylib_cs.Color.LightGray);

            int box1Y = (int)guiAnchor.Y + 105;
            int box1Height = is3DMode ? 265 : 240;
            Raylib.DrawRectangle((int)guiAnchor.X, box1Y, 210, box1Height, cardBg);
            Raylib.DrawRectangleLines((int)guiAnchor.X, box1Y, 210, box1Height, cardBorder);

            int tY = box1Y + 10;
            Raylib.DrawText("CONTROLS QUICK MENU", (int)guiAnchor.X + 10, tY, 13, Raylib_cs.Color.Gold);
            Raylib.DrawText("Left/Right : Change Stage", (int)guiAnchor.X + 10, tY + 22, 11, textClr);
            Raylib.DrawText("Up / Down  : Toggle 2D/3D", (int)guiAnchor.X + 10, tY + 40, 11, textClr);
            Raylib.DrawText($"P          : Pathways [{(pathsOn ? "ON" : "OFF")}]", (int)guiAnchor.X + 10, tY + 58, 11, textClr);
            Raylib.DrawText($"G          : Gems     [{(gemsOn ? "ON" : "OFF")}]", (int)guiAnchor.X + 10, tY + 76, 11, textClr);
            Raylib.DrawText($"V          : Palette Mode Toggle", (int)guiAnchor.X + 10, tY + 96, 11, Raylib_cs.Color.Yellow);
            Raylib.DrawText($"C          : Commit Stage Data", (int)guiAnchor.X + 10, tY + 114, 11, Raylib_cs.Color.Lime);
            Raylib.DrawText($"+ / -      : Zoom [{globalScale:F2}]", (int)guiAnchor.X + 10, tY + 132, 11, textClr);

            // ============================================================================
            // FIX BANNER: INPUTHANDLER.CS - DRAWCONTROLOVERLAY FUNCTION (PART 2 OF 2)
            // CONSTRAINTS: COMPACT LINE SAFETY CUTOFF PROTECTION | MAX 80 LINES
            // ============================================================================
            Raylib.DrawText($"E          : Queue Output", (int)guiAnchor.X + 10, tY + 150, 11, Raylib_cs.Color.Gold);
            Raylib.DrawText($"R          : Reset View", (int)guiAnchor.X + 10, tY + 168, 11, textClr);

            var dRoom = RomManager.IsolatedStages[currentRoom];
            if (dRoom == null) return;
            int drift = MapRenderer.GetRomHeightDiscrepancyCount(dRoom, currentRoom);

            // ============================================================================
            // FIX BANNER: INPUTHANDLER.CS - ELEVATOR LIST INDEX TYPE RE-MAPPING
            // LOCATION: REPLACES BOX 2 HUD LINES IN DrawControlOverlay (SEGMENT 1 OF 1)
            // ============================================================================
            int box2Y = box1Y + box1Height + 15;
            Raylib.DrawRectangle((int)guiAnchor.X, box2Y, 210, 100, cardBg);
            Raylib.DrawRectangleLines((int)guiAnchor.X, box2Y, 210, 100, cardBorder);
            Raylib.DrawText("LIVE REPOSITORY MONITOR", (int)guiAnchor.X + 10, box2Y + 7, 12, Raylib_cs.Color.Gold);
            Raylib.DrawText($"  * Layout Drift: {drift} cells", (int)guiAnchor.X + 10, box2Y + 27, 11, drift == 0 ? Raylib_cs.Color.Lime : Raylib_cs.Color.Yellow);

            // Explicitly address index [0] to extract values from the collection instance correctly
            if (dRoom.Elevators.Count > 0)
            {
                Raylib_cs.Color debugColor = dRoom.Elevators[0].IsMapped ? Raylib_cs.Color.Lime : Raylib_cs.Color.Red;
                Raylib.DrawText($"  * Is Mapped Flag : {dRoom.Elevators[0].IsMapped}", (int)guiAnchor.X + 10, box2Y + 47, 11, debugColor);
                Raylib.DrawText($"  * Physics Mode   : Mode_{dRoom.Elevators[0].Mode}", (int)guiAnchor.X + 10, box2Y + 65, 11, textClr);
            }
            else
            {
                Raylib.DrawText("  * Lift Mechanics : Zero Active Elevators", (int)guiAnchor.X + 10, box2Y + 47, 11, Raylib_cs.Color.DarkGray);
            }

            int box3Y = box2Y + 115;
            Raylib.DrawRectangle((int)guiAnchor.X, box3Y, 210, 110, cardBg);
            Raylib.DrawRectangleLines((int)guiAnchor.X, box3Y, 210, 110, cardBorder);
            Raylib.DrawText("REAL-TIME CELL TELEMETRY", (int)guiAnchor.X + 10, box3Y + 7, 12, Raylib_cs.Color.Gold);

            if (IsProbeInsideWorkspace)
            {
                string lbl = IsStageCommitted ? "[COMMITTED]" : (IsProbeLockedToGrid ? "[LOCKED]" : "[FREE]");
                Raylib.DrawText($"  * Target : X={ProbeGridX:D2} Y={ProbeGridY:D2} {lbl}", (int)guiAnchor.X + 10, box3Y + 27, 11, IsStageCommitted ? Raylib_cs.Color.Lime : (IsProbeLockedToGrid ? Raylib_cs.Color.Orange : Raylib_cs.Color.Yellow));
                Raylib.DrawText($"  * Altitude  : H={dRoom.Heights[ProbeGridX, ProbeGridY]:D3}", (int)guiAnchor.X + 10, box3Y + 45, 11, Raylib_cs.Color.RayWhite);
                Raylib.DrawText($"  * Gem Layer : {(dRoom.Gems[ProbeGridX, ProbeGridY] ? "HAS GEM [✓]" : "EMPTY [.]")}", (int)guiAnchor.X + 10, box3Y + 63, 11, dRoom.Gems[ProbeGridX, ProbeGridY] ? Raylib_cs.Color.Lime : Raylib_cs.Color.DarkGray);

                string lNode = "NONE"; Raylib_cs.Color lClr = Raylib_cs.Color.DarkGray;
                for (int e = 0; e < dRoom.Elevators.Count; e++)
                {
                    var lf = dRoom.Elevators[e];
                    if (lf.IsMapped && lf.CellX == ProbeGridX && lf.CellY == ProbeGridY)
                    {
                        lNode = $"E{e} [H:{lf.CurrentPosition:D3}] [Max:{lf.TopPosition:D3}]"; lClr = Raylib_cs.Color.Orange; break;
                    }
                }
                Raylib.DrawText($"  * Lift Node : {lNode}", (int)guiAnchor.X + 10, box3Y + 81, 11, lClr);
            }
            else
            {
                Raylib.DrawText("  * Target : OUT OF BOUNDS", (int)guiAnchor.X + 10, box3Y + 27, 11, Raylib_cs.Color.DarkGray);
                Raylib.DrawText("  * Altitude  : H=000", (int)guiAnchor.X + 10, box3Y + 45, 11, Raylib_cs.Color.DarkGray);
                Raylib.DrawText("  * Gem Layer : N/A", (int)guiAnchor.X + 10, box3Y + 63, 11, Raylib_cs.Color.DarkGray);
                Raylib.DrawText("  * Lift Node : N/A", (int)guiAnchor.X + 10, box3Y + 81, 11, Raylib_cs.Color.DarkGray);
            }
        }
    }
}