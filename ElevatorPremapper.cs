// ============================================================================
// ELEVATORPREMAPPER.CS - RESTORED VERIFIED DISK MATRIX SLURPER (v0.80 PATHS)
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
        public static readonly Dictionary<int, List<(int X, int Y)>> FileCoordinateCache = new Dictionary<int, List<(int, int)>>();

        public static void InitializeFromDisk()
        {
            if (_isInitialized) return;

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            // v0.80 UPDATE: Direct point-of-entry into your active elevators subfolder
            string elevatorFolder = Path.Combine(baseDir, "data", "elevator");

            if (!Directory.Exists(elevatorFolder))
            {
                _isInitialized = true;
                return;
            }

            for (int stageNum = 0; stageNum < 37; stageNum++)
            {
                List<(int X, int Y)> stageCoords = new List<(int X, int Y)>();

                // v0.80 FORMAT COMPLIANCE: Match your explicit 'elevators_stage_[num]_levelname.txt' format rules
                string v080Pattern = $"elevators_stage_{stageNum:D2}_*.txt";
                string flatPattern = $"elevators_stage_{stageNum:D2}.txt";

                string[] files = Directory.GetFiles(elevatorFolder, v080Pattern);
                if (files.Length == 0) files = Directory.GetFiles(elevatorFolder, flatPattern);

                if (files.Length > 0)
                {
                    try
                    {
                        string[] lines = File.ReadAllLines(files[0]);
                        for (int r = 0; r < lines.Length; r++)
                        {
                            string currentLine = lines[r];

                            // Guard conditions to skip file headers, legends, or layout dividers cleanly
                            if (string.IsNullOrWhiteSpace(currentLine) || currentLine.Contains("===") || currentLine.Contains("Context") || currentLine.Contains("[Legend")) continue;

                            string[] stringTokens = currentLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            // Account for header tracking offsets to properly align the 22x22 grid boundaries
                            int currentRowX = r - 4;
                            if (currentRowX < 0 || currentRowX >= 22) continue;

                            for (int c = 0; c < stringTokens.Length && c < 22; c++)
                            {
                                string token = stringTokens[c].Trim();

                                // Parse our clean custom validation flags ("M" = Match, "O" = Override, "R" = ROM Anchor)
                                if (token == "M" || token == "O" || token == "R")
                                {
                                    stageCoords.Add((currentRowX, c));
                                }
                            }
                        }

                        if (stageCoords.Count > 0)
                        {
                            FileCoordinateCache[stageNum] = stageCoords;
                        }
                    }
                    catch
                    {
                        // Defensive block boundary to protect execution during file reading issues
                    }
                }
            }
            _isInitialized = true;
        }

        public static void ApplyOverrides(int stageNum, List<ElevatorData> elevators)
        {
            if (elevators == null || elevators.Count == 0) return;
            if (!_isInitialized) InitializeFromDisk();

            if (FileCoordinateCache.ContainsKey(stageNum))
            {
                var targetCoords = FileCoordinateCache[stageNum];
                int loopLimit = System.Math.Min(elevators.Count, targetCoords.Count);

                for (int i = 0; i < loopLimit; i++)
                {
                    elevators[i].CellX = targetCoords[i].X;
                    elevators[i].CellY = targetCoords[i].Y;
                    elevators[i].IsMapped = true;
                }
            }
        }
    }
}