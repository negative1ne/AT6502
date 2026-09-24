// ====================================================================================
// CRYSTAL CASTLES UNIFIED INGESTION SUITE [v0.90 DATA ENGINE CORE]
// MODULE: UNIFIED DATA CORE PARSER - PART 1 (STATE ENGINE MECHANICS)
// CONSTRAINTS: SINGLE-PASS DATA INGESTION | VERBOSE STRIDE LEDGER TELEMETRY
// ====================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CrystalCastles.DataEngine
{
    public enum ParserBlockState
    {
        None,
        Metadata,
        Elevators,
        Gems,
        Map
    }

    public class CCUnifiedParser
    {
        private const int GridSize = 22;

        public class UnifiedStageProfile
        {
            public int StageID { get; set; }
            public string StageName { get; set; } = "Unknown Wave";
            public string TrackState { get; set; } = "StageTrackingState.ExperimentalTarget";
            public byte[,] Heights { get; set; } = new byte[GridSize, GridSize];
            public bool[,] Gems { get; set; } = new bool[GridSize, GridSize];
            public List<UnifiedLiftEntity> Lifts { get; set; } = new List<UnifiedLiftEntity>();
            // FIX BANNER: v0.91 DYNAMIC VIEWPORT CAMERA TELEMETRY MODEL DECLARATIONS [v0.91]
            // Explicitly declare missing scene origin offsets to clear Downstream Canvas compilation errors
            public int CameraOffsetX { get; set; } = 200;
            public int CameraOffsetY { get; set; } = 100;

            // Elevator tracking scalars matching arcade hardware configuration layouts
            public bool HasCustomLiftScalar { get; set; } = false;
            public int LiftOriginX { get; set; } = 112;
            public int LiftOriginY { get; set; } = -28;
        }

        public class UnifiedLiftEntity
        {
            public int CellX { get; set; }
            public int CellY { get; set; }
            public int BottomH { get; set; }
            public int TopH { get; set; }
            public string Direction { get; set; } = "UP";

            // FIX BANNER: v0.91 REBUILT HARDWARE POSITION TELEMETRY FIELDS [v0.91]
            // Explicitly track calculated arcade mapping scalars to resolve upstream structural calculations
            public int HorizontalPosition { get; set; } = 0;
            public int VerticalPosition { get; set; } = 0;
        
        }

        public static UnifiedStageProfile LoadUnifiedStageFile(string filePath, StreamWriter sessionLogger)
        {
            UnifiedStageProfile profile = new UnifiedStageProfile();
            if (!File.Exists(filePath))
            {
                if (sessionLogger != null)
                {
                    sessionLogger.WriteLine($"[CRITICAL ERROR] Unified level target missing: {Path.GetFileName(filePath)}");
                }
                return profile;
            }

            string[] lines = File.ReadAllLines(filePath);
            ParserBlockState currentState = ParserBlockState.None;
            int currentGemRow = 0;
            int currentMapRow = 0;

            if (sessionLogger != null)
            {
                sessionLogger.WriteLine($"\n==================== [STATE SWITCH DIAGNOSTIC: STAGE {Path.GetFileNameWithoutExtension(filePath)}] ====================");
                sessionLogger.WriteLine($"Resource: {filePath}");
            }

            for (int i = 0; i < lines.Length; i++)
            {
                string rawLine = lines[i];
                string cleanLine = rawLine.Trim(new char[] { '\uFEFF', '\u200B' }).Trim();

                if (string.IsNullOrEmpty(cleanLine) || cleanLine.StartsWith("//")) continue;

                // State Toggle Condition Blocks
                if (cleanLine.Equals("[METADATA]", StringComparison.OrdinalIgnoreCase))
                {
                    currentState = ParserBlockState.Metadata;
                    if (sessionLogger != null) sessionLogger.WriteLine($"  Line {i + 1:D2} [STATE -> METADATA]: Header Found. Engaging Ingestion Mode.");
                    continue;
                }
                if (cleanLine.Equals("[ELEVATORS]", StringComparison.OrdinalIgnoreCase))
                {
                    currentState = ParserBlockState.Elevators;
                    if (sessionLogger != null) sessionLogger.WriteLine($"  Line {i + 1:D2} [STATE -> ELEVATORS]: Section Found. Parsing Vector Node Registers.");
                    continue;
                }
                if (cleanLine.Equals("[GEMS]", StringComparison.OrdinalIgnoreCase))
                {
                    currentState = ParserBlockState.Gems;
                    if (sessionLogger != null) sessionLogger.WriteLine($"  Line {i + 1:D2} [STATE -> GEMS]: Section Found. Ingesting Spatial Boolean Matrix.");
                    continue;
                }
                if (cleanLine.Equals("[MAP]", StringComparison.OrdinalIgnoreCase))
                {
                    currentState = ParserBlockState.Map;
                    if (sessionLogger != null) sessionLogger.WriteLine($"  Line {i + 1:D2} [STATE -> MAP]: Section Found. Processing 3-Digit Altitude Values.");
                    continue;
                }

                // Strict v0.91 Explicit End Token Interceptors
                if (cleanLine.Equals("[END_ELEVATORS]", StringComparison.OrdinalIgnoreCase) ||
                    cleanLine.Equals("[END_GEMS]", StringComparison.OrdinalIgnoreCase) ||
                    cleanLine.Equals("[END_MAP]", StringComparison.OrdinalIgnoreCase))
                {
                    currentState = ParserBlockState.None;
                    if (sessionLogger != null) sessionLogger.WriteLine($"  Line {i + 1:D2} [STATE -> NONE]: Boundary Tag Detected. Re-locking State Pointer.");
                    continue;
                }

                // Divert processing lines based on the active mode with row index validation guards
                switch (currentState)
                {
                    case ParserBlockState.Metadata:
                        ParseMetadataLine(cleanLine, profile, i + 1, sessionLogger);
                        break;
                    case ParserBlockState.Elevators:
                        ParseElevatorLine(cleanLine, profile, i + 1, sessionLogger);
                        break;
                         case ParserBlockState.Gems:
                        ParseGemsLine(cleanLine, profile, ref currentGemRow, i + 1, sessionLogger);
                        break;
                    case ParserBlockState.Map:
                        ParseMapLine(cleanLine, profile, ref currentMapRow, i + 1, sessionLogger);
                        break;
                }
            }

            if (sessionLogger != null)
            {
                sessionLogger.WriteLine($"[✓] SUCCESS: Unified level processing completed for {profile.StageName}.");
                sessionLogger.WriteLine($"==================== [STRIDE MONITOR: END OF STAGE {profile.StageID:D2} LOG] ====================\n");
            }

            return profile;
        }

        private static void ParseMetadataLine(string line, UnifiedStageProfile profile, int lineNum, StreamWriter logger)
        {
            int eqIdx = line.IndexOf('=');
            if (eqIdx == -1) return;

            string key = line.Substring(0, eqIdx).Trim();
            string val = line.Substring(eqIdx + 1).Trim();

            if (key.Equals("StageID", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(val, out int id)) profile.StageID = id;
                if (logger != null) logger.WriteLine($"  Line {lineNum:D2} [DATA INGEST] -> Field: StageID = {profile.StageID:D2}");
            }
            else if (key.Equals("StageName", StringComparison.OrdinalIgnoreCase))
            {
                profile.StageName = val;
                if (logger != null) logger.WriteLine($"  Line {lineNum:D2} [DATA INGEST] -> Field: StageName = {profile.StageName}");
            }
            else if (key.Equals("TrackState", StringComparison.OrdinalIgnoreCase))
            {
                profile.TrackState = val;
                if (logger != null) logger.WriteLine($"  Line {lineNum:D2} [DATA INGEST] -> Field: TrackState = {profile.TrackState}");
            }
            // FIX BANNER: METADATA HEADER CAMERA OFFSET INGESTION PASS [v0.91]
            else if (key.Equals("CameraOffsetX", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(val, out int cx)) profile.CameraOffsetX = cx;
                if (logger != null) logger.WriteLine($"  Line {lineNum:D2} [DATA INGEST] -> Field: CameraOffsetX = {profile.CameraOffsetX}");
            }
            else if (key.Equals("CameraOffsetY", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(val, out int cy)) profile.CameraOffsetY = cy;
                if (logger != null) logger.WriteLine($"  Line {lineNum:D2} [DATA INGEST] -> Field: CameraOffsetY = {profile.CameraOffsetY}");
            }
            else if (key.Equals("LiftOriginX", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(val, out int lox))
                {
                    profile.LiftOriginX = lox;
                    profile.HasCustomLiftScalar = true;
                }
                if (logger != null) logger.WriteLine($"  Line {lineNum:D2} [DATA INGEST] -> Field: LiftOriginX = {profile.LiftOriginX}");
            }
            else if (key.Equals("LiftOriginY", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(val, out int loy))
                {
                    profile.LiftOriginY = loy;
                    profile.HasCustomLiftScalar = true;
                }
                if (logger != null) logger.WriteLine($"  Line {lineNum:D2} [DATA INGEST] -> Field: LiftOriginY = {profile.LiftOriginY}");
            }
        }

        private static void ParseElevatorLine(string line, UnifiedStageProfile profile, int lineNum, StreamWriter logger)
        {
            if (line.Equals("[NONE]", StringComparison.OrdinalIgnoreCase))
            {
                if (logger != null) logger.WriteLine($"  Line {lineNum:D2} [DEFENSIVE BYPASS] -> Keyword [NONE] Captured. Terminating Elevator Sub-Loop Safely.");
                return;
            }

            if (!line.Contains("RowX=") || !line.Contains("ColY=")) return;

            try
            {
                int xIdx = line.IndexOf("RowX=") + 5;
                int yIdx = line.IndexOf("ColY=") + 5;
                int bhIdx = line.IndexOf("BottomH=") + 8;
                int thIdx = line.IndexOf("TopH=") + 5;

                var lift = new UnifiedLiftEntity
                {
                    CellX = int.Parse(line.Substring(xIdx, 2)),
                    CellY = int.Parse(line.Substring(yIdx, 2)),
                    BottomH = int.Parse(line.Substring(bhIdx, 3)),
                    TopH = int.Parse(line.Substring(thIdx, 3))
                };

                // FIX BANNER: REVERSE-PROJECTION HARDWARE SCALAR INGESTION PASS [v0.91]
                // Intercept the row and column fields to mathematically reconstruct missing arcade coordinates in RAM
                int xOrigin = profile.CameraOffsetX;
                int yOrigin = profile.CameraOffsetY;
                int elOriginX = profile.HasCustomLiftScalar ? profile.LiftOriginX : 112;
                int elOriginY = profile.HasCustomLiftScalar ? profile.LiftOriginY : -28;

                // Re-calculate raw screen pixel offsets based on target row/col tiles
                int xp = xOrigin - (lift.CellX * 4) + (lift.CellY * 8);
                int yp = yOrigin + (lift.CellX * 4) + (lift.CellY * 2);

                // Populate UnifiedLiftEntity arcade coordinate trackers cleanly to satisfy downstream mapping equations
                lift.HorizontalPosition = xp - elOriginX;
                lift.VerticalPosition = yp - yOrigin + lift.BottomH - elOriginY;

                profile.Lifts.Add(lift);
                if (logger != null)
                {
                    logger.WriteLine($"  Line {lineNum:D2} [DATA INGEST] -> LIFT_{profile.Lifts.Count - 1}: Bounds Loaded (RowX={lift.CellX:D2} | ColY={lift.CellY:D2})");
                    logger.WriteLine($"    * Rebuilt Arcade Scalars : HorizPos={lift.HorizontalPosition} | VertPos={lift.VerticalPosition}");
                }
            }
            catch (Exception ex)
            {
                if (logger != null) logger.WriteLine($"  Line {lineNum:D2} [!] ELEVATOR PARSE EXCEPTION: {ex.Message}");
            }
        }

        // ====================================================================================
        // PASS 2 - PART 2: CCUNIFIEDPARSER.CS - SPATIAL DATA TOKEN STRIDE INDEXER FIXED
        // LOCATION: REPLACES PARSEGEMSLINE INTERIOR IMPLEMENTATION COMPLETELY
        // CONSTRAINTS: COMPACT LINE OVERRUN SAFETY | ELIMINATES WHITESPACE FRAGMENT DRIFT (v0.90)
        // ====================================================================================
        private static void ParseGemsLine(string line, UnifiedStageProfile profile, ref int gemRow, int lineNum, StreamWriter logger)
        {
            if (line.Contains("00 01 02") || gemRow >= GridSize)
            {
                if (logger != null && line.Contains("00 01 02")) logger.WriteLine($"  Line {lineNum:D2} [COLUMN LABEL SKIP]: '{line}'");
                return;
            }

            // Split on spaces and remove all empty element fragments cleanly
            string[] rawTokens = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (rawTokens.Length == 0) return;

            // DYNAMIC STRIDE FILTER: Isolate the row prefix securely to lock accurate array offsets
            int cleanStartCol = 0;
            if (int.TryParse(rawTokens[0], out int labelCheck) && labelCheck == gemRow)
            {
                cleanStartCol = 1;
            }

            if (logger != null)
            {
                logger.WriteLine($"  Line {lineNum:D2} [DATA INGEST] -> Raw Char Length: {line.Length} | Active Row: {gemRow:D2} | Tokens: {rawTokens.Length}");
            }

            int activeGemsInRow = 0;
            for (int colY = 0; colY < GridSize && (colY + cleanStartCol) < rawTokens.Length; colY++)
            {
                string targetToken = rawTokens[colY + cleanStartCol].Trim();

                // Track both single-pass marker profiles to lock gem occupancy records natively
                if (targetToken == "*" || targetToken == "**" || targetToken == "L")
                {
                    profile.Gems[gemRow, colY] = true;
                    activeGemsInRow++;
                }
                else
                {
                    profile.Gems[gemRow, colY] = false;
                }
            }

            if (logger != null)
            {
                logger.WriteLine($"  Line {lineNum:D2} [MATRIX CELL MAP] -> Row {gemRow:D2} Gems Extracted (Active Count: {activeGemsInRow})");
            }

            gemRow++;
        }

        private static void ParseMapLine(string line, UnifiedStageProfile profile, ref int mapRow, int lineNum, StreamWriter logger)
        {
            if (line.Contains("00  01  02") || mapRow >= GridSize)
            {
                if (logger != null && line.Contains("00  01  02")) logger.WriteLine($"  Line {lineNum:D2} [COLUMN LABEL SKIP]: '{line}'");
                return;
            }

            string[] tokens = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0) return;

            int startCol = 0;
            if (int.TryParse(tokens[0], out int rowPrefix) && rowPrefix == mapRow)
            {
                startCol = 1;
                if (logger != null && mapRow == 0) logger.WriteLine($"  Line {lineNum:D2} [STRIDE SHIFT] -> Line {lineNum:D2} Row Label '{tokens[0]}' Detected. Stripping Left-Margin Index Prefix.");
            }

            if (logger != null) logger.WriteLine($"  Line {lineNum:D2} [DATA INGEST] -> Raw Char Length: {line.Length} | Text: '{line}' | Tokens: {tokens.Length}");

            for (int colY = 0; colY < GridSize && (colY + startCol) < tokens.Length; colY++)
            {
                string token = tokens[colY + startCol].Trim();
                if (token == ".." || token == "..." || token == "XX")
                {
                    profile.Heights[mapRow, colY] = 0;
                }
                else if (byte.TryParse(token, out byte height))
                {
                    profile.Heights[mapRow, colY] = height > 99 ? (byte)99 : height;
                }
            }

            if (logger != null) logger.WriteLine($"  Line {lineNum:D2} [HEIGHT VERIFY] -> Row {mapRow:D2} Heights Loaded Natively. Void Tile Filters Synced.");
            mapRow++;
        }
    }
}
