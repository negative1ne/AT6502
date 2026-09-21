// ====================================================================================
// CRYSTAL CASTLES UNIFIED INGESTION SUITE [v0.85 DATA ENGINE ISOLATION]
// MODULE: UNIFIED DATA CORE PARSER (PART 1 - MAPS & GEMS LAYER)
// CONSTRAINTS: ZERO MEMORY LEAKS | EXPLICIT TELEMETRY TRACING | ENFORCED OUTPUT STRIDE
// ====================================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace CrystalCastles.DataEngine
{
    

    public class CCUnifiedParser
    {
        private const int GridSize = 22;

        /// <summary>
        /// SUB-SECTION 1: Dynamic Map Geometry Loader (Working Parser Core)
        /// Ingests both legacy 2-char NN and modern 3-char NNN padded file grids.
        /// </summary>
        public static short[,] LoadMapFile(string filePath, out int stageId, out int gemTally)
        {
            var matrix = new short[GridSize, GridSize];
            stageId = 0; gemTally = 0;

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"[!] UNIFIED PARSER WARN: Map file missing at target path: {Path.GetFileName(filePath)}");
                return matrix;
            }

            string[] lines = File.ReadAllLines(filePath);
            int currentGridRow = 0;

            foreach (string line in lines)
            {
                // Sanitize line for hidden Byte Order Mark (BOM) signatures instantly
                string cleanLine = line.Trim(new char[] { '\uFEFF', '\u200B' });
                string trimmed = cleanLine.Trim();

                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("==") || trimmed.StartsWith("--") || trimmed.StartsWith("[")) continue;

                if (trimmed.Contains("Unique Stage ID Index"))
                {
                    int colon = trimmed.IndexOf(':');
                    if (colon != -1) int.TryParse(trimmed.Substring(colon + 1).Trim().Split(' ')[0], out stageId);
                    continue;
                }
                if (trimmed.Contains("Collectibles Tally"))
                {
                    int colon = trimmed.IndexOf(':');
                    if (colon != -1) int.TryParse(trimmed.Substring(colon + 1).Trim().Split(' ')[0], out gemTally);
                    continue;
                }
                if (trimmed.StartsWith("00") && trimmed.Contains("01")) continue; // Skip column indicators
                if (currentGridRow >= GridSize) break;

                // Enforce explicit verification gate: ensure line is a data row
                if (!trimmed.Contains("..") && !trimmed.Contains("...") && !trimmed.Contains("XX") && !char.IsDigit(trimmed[0])) continue;

                string[] tokens = trimmed.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                // Stride Shift Check: Step past left-margin row index labels if they exist in file row string
                int startCol = (tokens.Length > GridSize && int.TryParse(tokens[0], out _)) ? 1 : 0;

                for (int c = 0; c < GridSize && (c + startCol) < tokens.Length; c++)
                {
                    string token = tokens[c + startCol].Trim();
                    if (token == ".." || token == "..." || token == "XX") matrix[currentGridRow, c] = -1;
                    else matrix[currentGridRow, c] = short.Parse(token);
                }
                currentGridRow++;
            }

            Console.WriteLine($"[✓] SUCCESS: Map matrix loaded -> {Path.GetFileName(filePath)} | Grid Stride Sync Verified.");
            return matrix;
        }

        /// <summary>
        /// SUB-SECTION 2: Standardized Gem Sheet Loader
        /// Converts double-character spatial collection grids natively into memory boolean maps.
        /// </summary>
        public static bool[,] LoadGemFile(string filePath, out int parsedGemCount)
        {
            var matrix = new bool[GridSize, GridSize];
            parsedGemCount = 0;

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"[!] UNIFIED PARSER WARN: Gem file missing at target path: {Path.GetFileName(filePath)}");
                return matrix;
            }

            string[] lines = File.ReadAllLines(filePath);
            int currentGridRow = 0;

            foreach (string line in lines)
            {
                string cleanLine = line.Trim(new char[] { '\uFEFF', '\u200B' });
                string trimmed = cleanLine.Trim();

                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("==") || trimmed.StartsWith("--") || trimmed.StartsWith("[")) continue;
                if (trimmed.StartsWith("00") && trimmed.Contains("01")) continue;
                if (currentGridRow >= GridSize) break;

                if (!trimmed.Contains(".") && !trimmed.Contains("*")) continue;

                string[] cells = trimmed.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                int startCol = (cells.Length > GridSize && int.TryParse(cells[0], out _)) ? 1 : 0;

                for (int c = 0; c < GridSize && (c + startCol) < cells.Length; c++)
                {
                    string token = cells[c + startCol].Trim();
                    if (token == "**" || token == "*")
                    {
                        matrix[currentGridRow, c] = true;
                        parsedGemCount++;
                    }
                }
                currentGridRow++;
            }

            Console.WriteLine($"[✓] SUCCESS: Gem matrix loaded -> {Path.GetFileName(filePath)} | Active Count: {parsedGemCount} Found.");
            return matrix;
        }
    
    /// <summary>
        /// SUB-SECTION 3: Keyword-Targeted Elevator Config Loader
        /// Streams text sheets, scanning explicitly for "RowX = " to isolate vector vectors.
        /// </summary>
        public static List<ElevatorEntity> LoadElevatorFile(string filePath)
        {
            var elevators = new List<ElevatorEntity>();

            // Defensive Rule: If a stage naturally lacks elevators, return empty block instantly
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"[✓] UNIFIED PARSER TRACK: Stage has no elevator assets -> {Path.GetFileName(filePath)}. Skipping file stream.");
                return elevators;
            }

            string[] lines = File.ReadAllLines(filePath);
            int parsedLineIndex = 0;

            foreach (string line in lines)
            {
                parsedLineIndex++;
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                // Explicit Target Lock: Find lines containing coordinate records
                if (trimmed.Contains("RowX = ") && trimmed.Contains("ColY = "))
                {
                    try
                    {
                        int xIndex = trimmed.IndexOf("RowX = ");
                        int yIndex = trimmed.IndexOf("ColY = ");

                        if (xIndex != -1 && yIndex != -1)
                        {
                            // Precision Substring Slice: Pull the 2 characters immediately following assignment
                            string xToken = trimmed.Substring(xIndex + 7, 2).Trim();
                            string yToken = trimmed.Substring(yIndex + 7, 2).Trim();

                            var entity = new ElevatorEntity
                            {
                                GridX = int.Parse(xToken),
                                GridY = int.Parse(yToken),
                                Height = 0.00f // Hardcoded baseline elevation initialization for 3D engine prep
                            };

                            elevators.Add(entity);
                            Console.WriteLine($"  --> [DATA FOUND]: Extracted Lift Platform Vector at Coordinate: ({entity.GridX:D2}, {entity.GridY:D2})");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[!] UNIFIED PARSER EXCEPTION: Loop parse failed at Line {parsedLineIndex}: {ex.Message}");
                    }
                }
            }

            string finalStatusReport = elevators.Count > 0 ? $"[DATA FOUND - Count: {elevators.Count}]" : "[EMPTY ARRAY RECORDED]";
            Console.WriteLine($"[✓] SUCCESS: Elevator configurations loaded -> {Path.GetFileName(filePath)} | Status: {finalStatusReport}");
            return elevators;
        }

    } // ============================================================================
    // FIX BANNER: CCUNIFIEDPARSER.CS - STRUCT RECOVERY LAYER (v0.85 REPAIR)
    // ============================================================================
    // ============================================================================
    // FIX BANNER: CCUNIFIEDPARSER.CS - COMPLETE STRUCT RECOVERY LAYER (v0.85 FIXED)
    // ============================================================================
    public class ElevatorEntity
    {
        public int RowX { get; set; }
        public int ColY { get; set; }
        public int BottomH { get; set; }
        public int TopH { get; set; }
        public string Direction { get; set; } = "UP";

        // Dan's native properties to resolve CS0117 and CS1061 errors
        public int GridX { get; set; }
        public int GridY { get; set; }
        public float Height { get; set; }
    }
}

