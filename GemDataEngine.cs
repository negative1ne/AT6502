// ====================================================================================
// CRYSTAL CASTLES UNIFIED INGESTION SUITE [v0.85 DATA ENGINE ISOLATION]
// MODULE: GEM GEOMETRY DATA ENGINE PARSER
// CONSTRAINTS: DOUBLE-PADDING COORD STRIPES | ZERO ALIGNMENT SHIFT
// ====================================================================================

using System;
using System.IO;
using System.Text;

namespace CrystalCastles.DataEngine
{
    public class GemDataEngine
    {
        private const int GridSize = 22;
        private const string VersionBanner = "Pipeline Version Target Profile: v0.85 DATA ENGINE ISOLATION";
        private const string DashSeparator = "-----------------------------------------------------------------";

        // ====================================================================================
        // FIX BANNER: GEM GEOMETRY PARSER - KEYWORD EXTRACTOR PASS (v0.85)
        // ====================================================================================
        public static bool[,] LoadGemFile(string filePath, out int gemCount)
        {
            var matrix = new bool[GridSize, GridSize];
            gemCount = 0;
            if (!File.Exists(filePath)) return matrix;

            string[] lines = File.ReadAllLines(filePath);
            int currentGridRow = 0;

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("==") || trimmed.StartsWith("--") || trimmed.StartsWith("[")) continue;
                if (trimmed.StartsWith("00") && trimmed.Contains("01")) continue; // Skip column headers row
                if (currentGridRow >= GridSize) break;

                try
                {
                    string[] cells;
                    // Standard v0.85 Row Index Stripper
                    if (line.Length > 5 && char.IsDigit(line[0]) && char.IsDigit(line[1]) && line[2] == ' ')
                    {
                        cells = line.Substring(5).Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    else // Legacy v0.80 Naked Cells Row Parser
                    {
                        cells = trimmed.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    for (int c = 0; c < Math.Min(cells.Length, GridSize); c++)
                    {
                        if (cells[c] == "*") { matrix[currentGridRow, c] = true; gemCount++; }
                    }
                    currentGridRow++;
                }
                catch (Exception ex)
                {
                    DataEngineLogger.LogSession($"GEM MATRIX ERROR on row [{currentGridRow}]: {ex.Message}");
                }
            }
            return matrix;
        }
        // ====================================================================================
        /// <summary>
        /// Automatically formats, indexes, and exports a 22x22 gem collection matrix to disk under the v0.85 specification.
        /// </summary>
        /// <param name="filePath">Target destination file path.</param>
        /// <param name="stageNumber">Two-digit stage index indicator string (e.g., "00").</param>
        /// <param name="stageName">The capitalized level name string.</param>
        /// <param name="matrix">The live 22x22 collection array map to export.</param>
        /// <param name="gemCount">The total pre-calculated count of active gems populated in the matrix.</param>
        public static void SaveGemFile(string filePath, string stageNumber, string stageName, bool[,] matrix, int gemCount)
        {
            try
            {
                using (var writer = new StreamWriter(filePath, false))
                {
                    // Write v0.85 Layout Target Headers
                    writer.WriteLine($"=== CRYSTAL CASTLES GEM CONFIG: STAGE {stageNumber} ({stageName.ToUpper()}) ===");
                    writer.WriteLine(VersionBanner);
                    writer.WriteLine("[Core Collectible Telemetry Audit Layer]");
                    writer.WriteLine($"- Verified Ground Truth Active Count: {gemCount} ROM Gems Rendered");
                    writer.WriteLine("[Legend: * = Active Collectible Gem dot, . = Empty Floor Tile Slot]");
                    writer.WriteLine(DashSeparator);

                    // Build Top Column Header Labels Line (00 01 02...)
                    var columnHeader = new StringBuilder("     ");
                    for (int c = 0; c < GridSize; c++)
                    {
                        columnHeader.Append($"{c:D2} ");
                    }
                    writer.WriteLine(columnHeader.ToString().TrimEnd());

                    // Export Indexed Rows Matrix Layer
                    for (int r = 0; r < GridSize; r++)
                    {
                        var rowBuilder = new StringBuilder($"{r:D2}   ");
                        for (int c = 0; c < GridSize; c++)
                        {
                            string cellToken = matrix[r, c] ? "*" : ".";
                            rowBuilder.Append($"{cellToken}  ");
                        }
                        writer.WriteLine(rowBuilder.ToString().TrimEnd());
                    }
                }

                DataEngineLogger.LogSession($"SUCCESS: Synchronized and wrote v0.85 indexed gem sheet to disk: {Path.GetFileName(filePath)}");
            }
            catch (Exception ex)
            {
                DataEngineLogger.LogSession($"ERROR: Failed to execute automated gem file write for {Path.GetFileName(filePath)}: {ex.Message}");
            }
        }
    }
}