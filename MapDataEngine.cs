// ====================================================================================
// CRYSTAL CASTLES UNIFIED INGESTION SUITE [v0.85 DATA ENGINE ISOLATION]
// MODULE: MAP GEOMETRY DATA ENGINE PARSER
// CONSTRAINTS: NNN STRIDE SLICING | 4-CHAR STEP INCREMENTS | BOTTOM SEPARATOR CHECK
// ====================================================================================

using System;
using System.IO;
using System.Text;

namespace CrystalCastles.DataEngine
{
    public class MapDataEngine
    {
        private const int GridSize = 22;
        private const string VersionBanner = "Pipeline Version Target Profile: v0.85 DATA ENGINE ISOLATION";
        private const string DashSeparator = "------------------------------------------------------------------------------------------";

        /// <summary>
        /// Parses a v0.85 triple-digit padded geometry layout file into a flat 22x22 height matrix.
        /// </summary>
        // ====================================================================================
        // FIX BANNER: MAP GEOMETRY PARSER - KEYWORD EXTRACTOR PASS (v0.85)
        // ====================================================================================
        public static short[,] LoadMapFile(string filePath, out int stageId, out int gemTally)
        {
            var matrix = new short[GridSize, GridSize];
            stageId = 0; gemTally = 0;
            if (!File.Exists(filePath)) return matrix;

            string[] lines = File.ReadAllLines(filePath);
            int currentGridRow = 0;

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("==") || trimmed.StartsWith("--") || trimmed.StartsWith("[")) continue;

                // Track metadata properties inline while streaming past headers
                if (trimmed.StartsWith("- Unique Stage ID Index"))
                {
                    int colon = trimmed.IndexOf(':');
                    if (colon != -1) int.TryParse(trimmed.Substring(colon + 1).Split('[')[0].Trim(), out stageId);
                    continue;
                }
                if (trimmed.StartsWith("- Dynamic Collectibles Tally"))
                {
                    int colon = trimmed.IndexOf(':');
                    if (colon != -1) int.TryParse(trimmed.Substring(colon + 1).Split(' ')[0].Trim(), out gemTally);
                    continue;
                }
                if (trimmed.StartsWith("00") && trimmed.Contains("01")) continue;
                if (currentGridRow >= GridSize) break;

                try
                {
                    string[] tokens;
                    if (line.Length > 4 && char.IsDigit(line[0]) && char.IsDigit(line[1]) && line[2] == ' ')
                    {
                        // Standard v0.85 4-Character Stride Block Processing
                        string activeLine = line.Substring(4);
                        tokens = new string[GridSize];
                        for (int c = 0; c < GridSize; c++)
                        {
                            int start = c * 4;
                            if (start + 3 <= activeLine.Length) tokens[c] = activeLine.Substring(start, 3);
                        }
                    }
                    else // Legacy v0.80 Double Character `NN` Space Splitting Parser
                    {
                        tokens = trimmed.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    for (int c = 0; c < Math.Min(tokens.Length, GridSize); c++)
                    {
                        string token = tokens[c].Trim();
                        if (token == ".." || token == "..." || token == "XX") matrix[currentGridRow, c] = -1;
                        else matrix[currentGridRow, c] = short.Parse(token);
                    }
                    currentGridRow++;
                }
                catch (Exception ex)
                {
                    DataEngineLogger.LogSession($"MAP HEIGHT ERROR on row [{currentGridRow}]: {ex.Message}");
                }
            }
            return matrix;
        }
        // ====================================================================================
        // ====================================================================================
        /// <summary>
        /// Automatically formats, pads, and exports a 22x22 map geometry matrix to disk under the v0.85 specification.
        /// </summary>
        /// <param name="filePath">Target destination file path.</param>
        /// <param name="stageNumber">Two-digit stage index indicator string (e.g., "00").</param>
        /// <param name="stageName">The capitalized level name string.</param>
        /// <param name="matrix">The live 22x22 height array map to export.</param>
        /// <param name="gemTally">The active count of collectibles matching this stage map asset.</param>
        public static void SaveMapFile(string filePath, string stageNumber, string stageName, short[,] matrix, int gemTally)
        {
            try
            {
                using (var writer = new StreamWriter(filePath, false))
                {
                    int numericStageId = int.Parse(stageNumber);

                    // Write v0.85 Layout Target Headers & Metadata
                    writer.WriteLine($"=== CRYSTAL CASTLES MAP CONFIG: STAGE {stageNumber} ({stageName.ToUpper()}) ===");
                    writer.WriteLine(VersionBanner);
                    writer.WriteLine("[Core Map Data Tracking Layer]");
                    writer.WriteLine($"- Unique Stage ID Index   : {numericStageId} [Reused Parent City ROM Data Bank ID: 0]");
                    writer.WriteLine($"- Dynamic Collectibles Tally: {gemTally} Active ROM Gems Counted");
                    writer.WriteLine("[Active Viewport Tracking Flags Enforced In Memory]");
                    writer.WriteLine("- TrackState Engine Gate  : StageTrackingState.VerifiedWorking");
                    writer.WriteLine("[Legend: NNN = Height, ... = Empty Space]");
                    writer.WriteLine(DashSeparator);

                    // Build Top Column Header Labels Line (00  01  02...)
                    var columnHeader = new StringBuilder("    ");
                    for (int c = 0; c < GridSize; c++)
                    {
                        columnHeader.Append($"{c:D2}  ");
                    }
                    writer.WriteLine(columnHeader.ToString().TrimEnd());

                    // Export Indexed Rows Matrix Layer with Padded NNN Entries
                    for (int r = 0; r < GridSize; r++)
                    {
                        var rowBuilder = new StringBuilder($"{r:D2}  ");
                        for (int c = 0; c < GridSize; c++)
                        {
                            short val = matrix[r, c];
                            string cellToken = (val == -1) ? "..." : val.ToString("D3");
                            rowBuilder.Append($"{cellToken} ");
                        }
                        writer.WriteLine(rowBuilder.ToString().TrimEnd());
                    }

                    // Write Bottom Separator Line to Seal the Data Stream Boundary
                    writer.WriteLine(DashSeparator);
                }

                DataEngineLogger.LogSession($"SUCCESS: Synchronized and wrote v0.85 padded map geometry to disk: {Path.GetFileName(filePath)}");
            }
            catch (Exception ex)
            {
                DataEngineLogger.LogSession($"ERROR: Failed to execute automated map file write for {Path.GetFileName(filePath)}: {ex.Message}");
            }
        }
    }
}