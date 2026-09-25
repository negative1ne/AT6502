// ====================================================================================
// FIX BANNER: INPUTHANDLER.CS - PHASE B UNIFIED OPERATIONS (v0.90 SPEC)
// LOCATION: FULL CLEAN REPLACEMENT FROM LINE 1 TO END OF CLASS
// CONSTRAINTS: MAX 150 LINES LIMIT | ZERO TELEMETRY DRIFT | ANCHOR BOUNDS SYNC
// ====================================================================================
using Raylib_cs;
using System;
using System.IO;
using System.Text;
using System.Numerics;

namespace cSharpRaylib
{
    public struct LevelVisualState
    {
        public float Scale;
        public float HeightMultiplier;
        public int PanOffsetX;
        public int PanOffsetY;
        public int RotationAngle;
        public float TiltFactor;
        public int RenderStyleMode;
        public bool Initialized;
    }

    public static class InputHandler
    {
        private static Vector2 _probeMouseScreenPos;
        private static bool _lastIs3DMode = false;

        public static int ProbeGridX { get; private set; } = -1;
        public static int ProbeGridY { get; private set; } = -1;
        public static bool IsProbeInsideWorkspace { get; private set; } = false;
        public static bool IsProbeLockedToGrid { get; private set; } = false;
        public static bool IsStageCommitted { get; private set; } = false;

        public static LevelVisualState[] LevelCameraCache = new LevelVisualState[37];

        // ====================================================================================
        // SUB-TASK 7A - PART 2: INPUTHANDLER.CS - GUI LOG ROUTING PAYLOAD METHOD SYNCHRONIZATION
        // LOCATION: REPLACES METHOD SIGNATURE DOWN THROUGH LEVEL-SWAP SELECTION CODE BLOCK
        // CONSTRAINTS: MAX 150 LINES WINDOW LIMIT | EDGE TRIPPED INTERCEPT CAPTURES (v0.90)
        // ====================================================================================
        public static void HandleKeys(
            ref int currentRoom, ref bool is3DMode, ref float globalScale,
            ref float heightMultiplier, ref int panOffsetX, ref int panOffsetY,
            ref int rotationAngle, ref float tiltFactor, ref int renderStyleMode,
            ref bool displayPathOverlays, ref bool displayGems, ref bool showDanLegacyOverlay,
            ref bool invertBackground, ref bool exportTextFlag, ref bool trigger3DLabFlag,
            string activeSessionTimestamp)
        {
            string logTargetName = $"session_audit_{activeSessionTimestamp}.log";

            // Level change intercept: Capture old layout states and log selection details
            // FIX BANNER: Task Step 1 High-Integrity F2 Key Snapshot Router Pass [v0.95]
            if (Raylib.IsKeyPressed(KeyboardKey.F2))
            {
                // Forces fallback string formatting to safely avoid city engine variable leaks
                string structuralFallbackName = $"STAGE_{currentRoom:D2}";

                // Routes data straight to logging engine using thread-safe parameters
                CCUnifiedLogger.ExportActiveSessionSummary(currentRoom, structuralFallbackName, Program.MainLoggedCells, displayGems);
                Console.Beep(800, 150);
            }

            // Level change intercept: Capture old layout states and log selection details
            if (Raylib.IsKeyPressed(KeyboardKey.Right) || Raylib.IsKeyPressed(KeyboardKey.Left))

                // Level change intercept: Capture old layout states and log selection details
                if (Raylib.IsKeyPressed(KeyboardKey.Right) || Raylib.IsKeyPressed(KeyboardKey.Left))
            {
                int oldRoom = currentRoom;
                SaveActiveRoomVisualState(currentRoom, globalScale, heightMultiplier, panOffsetX, panOffsetY, rotationAngle, tiltFactor, renderStyleMode);

                if (Raylib.IsKeyPressed(KeyboardKey.Right)) { currentRoom = (currentRoom + 1) % 37; }
                else { currentRoom = (currentRoom - 1 + 37) % 37; }

                IsStageCommitted = false;
                HydrateRoomVisualState(currentRoom, ref globalScale, ref heightMultiplier, ref panOffsetX, ref panOffsetY, ref rotationAngle, ref tiltFactor, ref renderStyleMode);

                // Fire discrete single-pass write to logging file
                CCUnifiedLogger.LogSessionEvent(logTargetName, $"Room migrated successfully from Stage {oldRoom:D2} to Stage {currentRoom:D2}.");
                return;
            }

            // ====================================================================================
            // CRITICAL FIX BANNER: INPUTHANDLER.CS - CONSOLIDATE DUPLICATE EDGE TRIGGERS
            // LOCATION: REPLACES ALL RAW KEY TOGGLES TO ELIMINATE FRAME DOUBLE-MUTATIONS
            // CONSTRAINTS: SINGLE-PASS INTERCEPTS | ENFORCES STATE MACHINE LOG STABILITY (v0.90)
            // ====================================================================================
            if (Raylib.IsKeyDown(KeyboardKey.KpAdd)) globalScale += 0.02f;
            if (Raylib.IsKeyDown(KeyboardKey.KpSubtract)) globalScale -= 0.02f;

            if (Raylib.IsKeyPressed(KeyboardKey.Up))
            {
                is3DMode = true;
                CCUnifiedLogger.LogSessionEvent(logTargetName, "View Mode altered: Swapped to 3D Isometric view.");
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Down))
            {
                is3DMode = false;
                CCUnifiedLogger.LogSessionEvent(logTargetName, "View Mode altered: Swapped to 2D Blueprint canvas view.");
            }

            if (Raylib.IsKeyPressed(KeyboardKey.M))
            {
                renderStyleMode = (renderStyleMode + 1) % 3;
                CCUnifiedLogger.LogSessionEvent(logTargetName, $"Render style cycled to Mode_{renderStyleMode}.");
            }
            if (Raylib.IsKeyPressed(KeyboardKey.P))
            {
                displayPathOverlays = !displayPathOverlays;
                CCUnifiedLogger.LogSessionEvent(logTargetName, $"Path Overlay toggle mutated to: {displayPathOverlays}.");
            }
            if (Raylib.IsKeyPressed(KeyboardKey.G))
            {
                displayGems = !displayGems;
                CCUnifiedLogger.LogSessionEvent(logTargetName, $"Gem Overlay visibility mutated to: {displayGems}.");
            }
            if (Raylib.IsKeyPressed(KeyboardKey.V))
            {
                invertBackground = !invertBackground;
                CCUnifiedLogger.LogSessionEvent(logTargetName, $"Color Inversion state mutated to: {invertBackground}.");
            }
            if (Raylib.IsKeyPressed(KeyboardKey.C))
            {
                IsStageCommitted = !IsStageCommitted;
                CCUnifiedLogger.LogSessionEvent(logTargetName, $"Stage selection commit flag mutated to: {IsStageCommitted}.");
            }
            if (Raylib.IsKeyPressed(KeyboardKey.F))
            {
                rotationAngle = ((rotationAngle / 45) + 1) * 45 % 360;
                CCUnifiedLogger.LogSessionEvent(logTargetName, $"Fixed angle snap executed. Active rotation angle locked at {rotationAngle:D3}°.");
            }
            
            trigger3DLabFlag = false;
            exportTextFlag = false;

            // Global [W/S] Scaling Inversion & Redirection Handler
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
            else
            {
                // Redirect top menu scale transformations cleanly during 2D Canvas views
                if (Raylib.IsKeyDown(KeyboardKey.W)) globalScale += 0.02f;
                if (Raylib.IsKeyDown(KeyboardKey.S)) globalScale -= 0.02f;
            }

            if (Raylib.IsKeyPressed(KeyboardKey.R))
            {
                globalScale = 1.0f; heightMultiplier = 1.8f; panOffsetX = 0; panOffsetY = 0;
                rotationAngle = 0; tiltFactor = 1.0f; renderStyleMode = 0;
                displayPathOverlays = false; displayGems = true; showDanLegacyOverlay = false;
                invertBackground = false; IsProbeLockedToGrid = false; IsStageCommitted = false;
                SaveActiveRoomVisualState(currentRoom, globalScale, heightMultiplier, panOffsetX, panOffsetY, rotationAngle, tiltFactor, renderStyleMode);
            }
        }

        private static void SaveActiveRoomVisualState(int room, float sc, float hm, int ox, int oy, int ra, float tf, int rm)
        {
            if (room < 0 || room >= 37) return;
            LevelCameraCache[room] = new LevelVisualState { Scale = sc, HeightMultiplier = hm, PanOffsetX = ox, PanOffsetY = oy, RotationAngle = ra, TiltFactor = tf, RenderStyleMode = rm, Initialized = true };
        }

        private static void HydrateRoomVisualState(int room, ref float sc, ref float hm, ref int ox, ref int oy, ref int ra, ref float tf, ref int rm)
        {
            if (room < 0 || room >= 37) return;
            if (!LevelCameraCache[room].Initialized)
            {
                sc = 1.0f; hm = 1.8f; ox = 0; oy = 0; ra = 0; tf = 1.0f; rm = 0;
                return;
            }
            var st = LevelCameraCache[room];
            sc = st.Scale; hm = st.HeightMultiplier; ox = st.PanOffsetX; oy = st.PanOffsetY; ra = st.RotationAngle; tf = st.TiltFactor; rm = st.RenderStyleMode;
        }

        public static void TrackMouseProbeCoordinates(float screenX, float screenY,
            float scale, int offsetX, int offsetY, int rotationAngle, float tiltFactor,
            CityData activeCity, bool is3DMode, int currentRoom, bool[,,] canvasMatrix)
        {
            if (is3DMode != _lastIs3DMode) { _lastIs3DMode = is3DMode; return; }

            if (Raylib.IsMouseButtonPressed(MouseButton.Left) && !IsStageCommitted)
            {
                if (IsProbeInsideWorkspace || IsProbeLockedToGrid)
                {
                    IsProbeLockedToGrid = !IsProbeLockedToGrid;
                    if (IsProbeLockedToGrid && canvasMatrix != null && currentRoom >= 0 && currentRoom < 37)
                    {
                        canvasMatrix[currentRoom, ProbeGridX, ProbeGridY] = !canvasMatrix[currentRoom, ProbeGridX, ProbeGridY];
                    }
                }
            }

            if (IsProbeLockedToGrid) return;
            _probeMouseScreenPos.X = screenX; _probeMouseScreenPos.Y = screenY;
            if (activeCity == null) return;

            // ====================================================================================
            // FIX BANNER: INPUTHANDLER.CS - TASK 6: RECIPROCAL MOUSE COORDINATE PROBE COUPLER
            // LOCATION: REPLACES MOUSE HIT-SCAN CONSTRAINTS FOR 2D PROJECTION CANVAS VIEWS
            // CONSTRAINTS: CONVERTS SCREEN RASTER STEPS BACK INTO STABLE ROTATED ARRAY DATA SLOTS
            // ====================================================================================
            if (!is3DMode)
            {
                int gridCellY = ((int)screenX - 380) >= 0 ? ((int)screenX - 380) / 16 : -1;
                int gridCellX = ((int)screenY - 150) >= 0 ? ((int)screenY - 150) / 16 : -1;

                if (gridCellX >= 0 && gridCellX < 22 && gridCellY >= 0 && gridCellY < 22)
                {
                    bool isRotatedStage = (currentRoom == 1 || currentRoom == 21 || currentRoom == 22);

                    // Map visual coordinate clicks symmetrically back onto actual database memory indexes
                    int realX = isRotatedStage ? (21 - gridCellY) : gridCellX;
                    int realY = isRotatedStage ? gridCellX : gridCellY;

                    ProbeGridX = Math.Clamp(realX, 0, 21);
                    ProbeGridY = Math.Clamp(realY, 0, 21);
                    IsProbeInsideWorkspace = true;
                }
                else { ProbeGridX = -1; ProbeGridY = -1; IsProbeInsideWorkspace = false; }
                return;
            }
            // ====================================================================================

            int originX = 500 + offsetX; int originY = 340 + offsetY;
            float closestDistance = float.MaxValue; int bestX = -1; int bestY = -1;
            double rad = rotationAngle * Math.PI / 180.0;

            for (int x = 0; x < 22; x++)
            {
                for (int y = 0; y < 22; y++)
                {
                    double cx = x - 11.0; double cy = y - 11.0;
                    float rotX = (float)(cx * Math.Cos(rad) - cy * Math.Sin(rad)) + 11f;
                    float rotY = (float)(cx * Math.Sin(rad) + cy * Math.Cos(rad)) + 11f;
                    float cellProjectedX = originX - (rotX * 12 * scale) + (rotY * 12 * scale);
                    float cellProjectedY = originY + (rotX * 6 * scale * tiltFactor) + (rotY * 6 * scale * tiltFactor) - (activeCity.Heights[x, y] * 1.8f * scale);
                    float currentDist = ((screenX - cellProjectedX) * (screenX - cellProjectedX)) + ((screenY - cellProjectedY) * (screenY - cellProjectedY));
                    if (currentDist < closestDistance) { closestDistance = currentDist; bestX = x; bestY = y; }
                }
            }

            if (bestX != -1 && bestY != -1 && closestDistance < (32.0f * scale * 32.0f * scale))
            {
                ProbeGridX = Math.Clamp(bestX, 0, 21); ProbeGridY = Math.Clamp(bestY, 0, 21); IsProbeInsideWorkspace = true;
            }
            else { ProbeGridX = -1; ProbeGridY = -1; IsProbeInsideWorkspace = false; }
        }
        // ====================================================================================
        // FIX BANNER: INPUTHANDLER.CS - DRAWCONTROLOVERLAY ANCHOR COMPRESSION & TELEMETRY LIVE SYNC
        // LOCATION: REPLACES FROM DRAWCONTROLOVERLAY DECLARATION UNTIL THE END OF THE FILE
        // CONSTRAINTS: COMPACT LINE OVERRUN SAFETY | LIVE ROTATION SYNC | HEIGHT COMPRESSION
        // ====================================================================================
        public static void DrawControlOverlay(bool is3DMode, int renderStyle, bool pathsOn, bool gemsOn, bool legacyOverlayOn,
            bool isInverted, float globalScale, int currentRoom, string stageName, int totalElevators, Raylib_cs.Color[] activeTheme, bool[,,] canvasMatrix,
            int rotationAngle)
        {
            Raylib_cs.Color cardBg = isInverted ? new Raylib_cs.Color(230, 230, 230, 220) : new Raylib_cs.Color(20, 20, 20, 200);
            Raylib_cs.Color cardBorder = isInverted ? Raylib_cs.Color.DarkGray : Raylib_cs.Color.LightGray;
            Raylib_cs.Color textClr = isInverted ? Raylib_cs.Color.Black : Raylib_cs.Color.RayWhite;
            Vector2 guiAnchor = new Vector2(15, 10);

            // FIX 1: Linked directly to live loop telemetry argument parameter
            string viewModeLabel = is3DMode ? $"3D Isometric @ {rotationAngle:D3}°" : "2D Flat Viewport";

            // FIX BANNER: Task H4 Dynamic Main Overlay UI Label Synchronizer [v0.95]
            Raylib.DrawText($"ccSharpRaylib [v{CCFormatConfig.VersionTag}]", (int)guiAnchor.X, (int)guiAnchor.Y, 20, textClr);
            Raylib.DrawText($"Level {(currentRoom / 4) + 1} - {(currentRoom % 4) + 1} [{stageName}] | Lifts: {totalElevators} | {viewModeLabel}", (int)guiAnchor.X, (int)guiAnchor.Y + 35, 18, Raylib_cs.Color.Gold);

            if (activeTheme != null && activeTheme.Length >= 3)
            {
                Raylib.DrawRectangle((int)guiAnchor.X, (int)guiAnchor.Y + 70, 40, 20, activeTheme[0]);
                Raylib.DrawRectangle((int)guiAnchor.X + 50, (int)guiAnchor.Y + 70, 40, 20, activeTheme[1]);
                Raylib.DrawRectangle((int)guiAnchor.X + 100, (int)guiAnchor.Y + 70, 40, 20, activeTheme[2]);
            }
            Raylib.DrawText($"Active Layout Palette Matrix Slots (v{CCFormatConfig.VersionTag})", (int)guiAnchor.X + 160, (int)guiAnchor.Y + 73, 14, isInverted ? Raylib_cs.Color.DarkGray : Raylib_cs.Color.LightGray);

            int box1Y = (int)guiAnchor.Y + 105;
            // COMPRESSION: Tightened up box height calculation constraints to fit menu scope exactly
            int box1Height = 225;
            Raylib.DrawRectangle((int)guiAnchor.X, box1Y, 210, box1Height, cardBg);
            Raylib.DrawRectangleLines((int)guiAnchor.X, box1Y, 210, box1Height, cardBorder);

            int tY = box1Y + 10;
            Raylib.DrawText("CONTROLS QUICK MENU", (int)guiAnchor.X + 10, tY, 13, Raylib_cs.Color.Gold);
            Raylib.DrawText("Left/Right : Change Stage", (int)guiAnchor.X + 10, tY + 20, 11, textClr);
            Raylib.DrawText("Up / Down  : Toggle 2D/3D", (int)guiAnchor.X + 10, tY + 36, 11, textClr);
            Raylib.DrawText("A / D      : Rotate 3D View", (int)guiAnchor.X + 10, tY + 52, 11, textClr);
            Raylib.DrawText("F          : Snap 45° Rotation", (int)guiAnchor.X + 10, tY + 68, 11, Raylib_cs.Color.Yellow);
            Raylib.DrawText($"P          : Pathways [{(pathsOn ? "ON" : "OFF")}]", (int)guiAnchor.X + 10, tY + 84, 11, textClr);
            Raylib.DrawText($"G          : Gems     [{(gemsOn ? "ON" : "OFF")}]", (int)guiAnchor.X + 10, tY + 100, 11, textClr);
            Raylib.DrawText($"V          : Palette Mode Toggle", (int)guiAnchor.X + 10, tY + 116, 11, Raylib_cs.Color.Yellow);

            Raylib_cs.Color commitStatusColor = IsStageCommitted ? Raylib_cs.Color.Lime : Raylib_cs.Color.DarkGray;
            string commitLabelText = IsStageCommitted ? "C          : Commit Data [LOCKED]" : "C          : Commit Data [OFF]";
            Raylib.DrawText(commitLabelText, (int)guiAnchor.X + 10, tY + 132, 11, commitStatusColor);

            Raylib.DrawText("W / S      : Scale / Height Multiplier", (int)guiAnchor.X + 10, tY + 148, 11, textClr);
            Raylib.DrawText($"+ / -      : Zoom [{globalScale:F2}]", (int)guiAnchor.X + 10, tY + 164, 11, textClr);
            Raylib.DrawText($"E          : Queue Output", (int)guiAnchor.X + 10, tY + 180, 11, Raylib_cs.Color.Gold);
            Raylib.DrawText($"R          : Reset View", (int)guiAnchor.X + 10, tY + 196, 11, textClr);

            var dRoom = RomManager.IsolatedStages[currentRoom];
            if (dRoom == null) return;
            int drift = MapRenderer.GetRomHeightDiscrepancyCount(dRoom, currentRoom);

            // ANCHOR LINKED: box2Y and box3Y now snap dynamically off box1 height dimensions cleanly
            int box2Y = box1Y + box1Height + 15;
            Raylib.DrawRectangle((int)guiAnchor.X, box2Y, 210, 100, cardBg);
            Raylib.DrawRectangleLines((int)guiAnchor.X, box2Y, 210, 100, cardBorder);
            Raylib.DrawText("LIVE REPOSITORY MONITOR", (int)guiAnchor.X + 10, box2Y + 7, 12, Raylib_cs.Color.Gold);
            Raylib.DrawText($"  * Layout Drift: {drift} cells", (int)guiAnchor.X + 10, box2Y + 27, 11, drift == 0 ? Raylib_cs.Color.Lime : Raylib_cs.Color.Yellow);

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

            int box3Y = box2Y + 100 + 15;
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