// ============================================================================
// ELEVATORPREMAPPER.CS - TEXT MATRIX SHEET SLURPER OVERHAUL (v0.5)
// ============================================================================
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace cSharpRaylib
{
    public static class ElevatorPremapper
    {
        private static bool _isInitialized = false;
        private static readonly Dictionary<int, List<(int X, int Y)>> _fileCoordinateCache = new Dictionary<int, List<(int, int)>>();

        /// <summary>
        /// Reads absolute coordinate tokens straight from your hand-edited Map sheets.
        /// Bypasses string report requirements and reads grid indices natively.
        /// </summary>
        public static void InitializeFromDisk()
        {
            if (_isInitialized) return;

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string mapsFolder = Path.Combine(baseDir, "data", "maps");

            SessionLogger.LogVerificationMessage("[PREMAPPER_INIT] Initializing matrix sheet slurper pass... Target folder: .\\data\\maps\\");

            if (!Directory.Exists(mapsFolder))
            {
                SessionFolderErrorTrace();
                _isInitialized = true;
                return;
            }

            for (int stageNum = 0; stageNum < 37; stageNum++)
            {
                List<(int X, int Y)> stageCoords = new List<(int X, int Y)>();

                string customPattern = $"Diagnostic_Dump_Stage_{stageNum:D2}_*.txt";
                string flatPattern = $"Diagnostic_Dump_Stage_{stageNum:D2}.txt";

                string[] files = Directory.GetFiles(mapsFolder, customPattern);
                if (files.Length == 0) files = Directory.GetFiles(mapsFolder, flatPattern);

                if (files.Length > 0)
                {
                    try
                    {
                        string targetFilePath = files[0];
                        string[] lines = File.ReadAllLines(targetFilePath);

                        for (int r = 0; r < lines.Length; r++)
                        {
                            string currentLine = lines[r];
                            // Skip text header lines, profile names, and legends safely
                            if (string.IsNullOrWhiteSpace(currentLine) || currentLine.Contains("===") || currentLine.Contains("Context") || currentLine.Contains("[Legend")) continue;

                            string[] stringTokens = currentLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            // Account for the edge padding marker lines inside the files
                            int currentRowX = r - 4;
                            if (currentRowX < 0 || currentRowX >= 22) continue;

                            // ============================================================================
                            // ELEVATORPREMAPPER.CS - ENFORCING TOTAL CHARACTER TOKEN VALIDATION
                            // ============================================================================
                            // Loop across all 22 columns inside the text matrix grid row array path
                            for (int c = 0; c < stringTokens.Length && c < 22; c++)
                            {
                                string token = stringTokens[c].Trim();

                                // FIX: Process 'R' tokens identically to 'M' and 'O' markers!
                                // This immunizes your directories against previous background overwrites.
                                if (token == "M" || token == "O" || token == "R")
                                {
                                    stageCoords.Add((currentRowX, c));
                                }
                            }
                        }

                        if (stageCoords.Count > 0)
                        {
                            _fileCoordinateCache[stageNum] = stageCoords;
                            SessionLogger.LogVerificationMessage($"    -> Stage [{stageNum:D2}]: Successfully extracted {stageCoords.Count} matrix markers from flat file grid.");
                        }
                    }
                    catch (Exception ex)
                    {
                        SessionLogger.LogVerificationMessage($"    -> Stage [{stageNum:D2}][ERROR] File parsing matrix failure: {ex.Message}");
                    }
                }
            }

            _isInitialized = true;
            SessionLogger.LogVerificationMessage("[PREMAPPER_INIT] Matrix memory cache layer sealed completely.");
        }

        private static void SessionFolderErrorTrace()
        {
            SessionLogger.LogVerificationMessage("    -> [CRITICAL WARNING]: Directory '.\\data\\maps\\' is missing from disk tree path configuration layout!");
        }

        /// <summary>
        /// Binds extracted text sheet grid markers directly straight onto your running game object containers.
        /// </summary>
        public static void ApplyOverrides(int stageNum, List<ElevatorData> elevators)
        {
            if (elevators == null || elevators.Count == 0) return;

            if (!_isInitialized) InitializeFromDisk();

            if (_fileCoordinateCache.ContainsKey(stageNum))
            {
                var targetCoords = _fileCoordinateCache[stageNum];
                int loopLimit = System.Math.Min(elevators.Count, targetCoords.Count);

                for (int i = 0; i < loopLimit; i++)
                {
                    elevators[i].CellX = targetCoords[i].X;
                    elevators[i].CellY = targetCoords[i].Y;
                    elevators[i].IsMapped = true;
                }

                SessionLogger.LogVerificationMessage($"[MATRIX_BRIDGE] Overwrote {loopLimit} live coordinates on Stage [{stageNum:D2}] using flat text sheet tokens.");
            }
        }
    }
}