// ============================================================================
// ELEVATORPREMAPPER.CS - SIMPLIFIED TEXT TOKEN STRUCT SLURPER (v0.80)
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
            string elevatorFolder = Path.Combine(baseDir, "data", "elevator");

            if (!Directory.Exists(elevatorFolder))
            {
                _isInitialized = true;
                return;
            }

            for (int stageNum = 0; stageNum < 37; stageNum++)
            {
                List<(int X, int Y)> stageCoords = new List<(int X, int Y)>();

                string v080Pattern = $"elevators_stage_{stageNum:D2}_*.txt";
                string flatPattern = $"elevators_stage_{stageNum:D2}.txt";

                string[] files = Directory.GetFiles(elevatorFolder, v080Pattern);
                if (files.Length == 0) files = Directory.GetFiles(elevatorFolder, flatPattern);

                if (files.Length > 0)
                {
                    try
                    {
                        string[] lines = File.ReadAllLines(files[0]);
                        foreach (string line in lines)
                        {
                            if (string.IsNullOrWhiteSpace(line)) continue;

                            // v0.80 SIMPLIFIED STRIPPER: Isolate rows containing coordinate entries
                            if (line.Contains("MappedCell=("))
                            {
                                // Split at the opening parenthesis token to isolate values block
                                string[] mainParts = line.Split(new string[] { "MappedCell=(" }, StringSplitOptions.None);
                                if (mainParts.Length < 2) continue;

                                // Split at closing parenthesis to discard trailing metadata characters
                                string coordBlock = mainParts[1].Split(')')[0];
                                string[] coordinates = coordBlock.Split(',');

                                if (coordinates.Length == 2 &&
                                    int.TryParse(coordinates[0].Trim(), out int rowX) &&
                                    int.TryParse(coordinates[1].Trim(), out int colY))
                                {
                                    // Protect the matrix cache from unexpected text-editing overflow limits
                                    if (rowX >= 0 && rowX < 22 && colY >= 0 && colY < 22)
                                    {
                                        stageCoords.Add((rowX, colY));
                                    }
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
                        // Defensive block boundary to keep engine compilation loops running smoothly
                    }
                }
            }
            _isInitialized = true;
        }

        // ============================================================================
        // ELEVATORPREMAPPER.CS - EXPANDED MOTION WINDOW CALIBRATION (v0.80)
        // ============================================================================
        public static void ApplyOverrides(int stageNum, List<ElevatorData> elevators)
        {
            if (elevators == null || elevators.Count == 0) return;
            if (!_isInitialized) InitializeFromDisk();

            if (FileCoordinateCache.ContainsKey(stageNum))
            {
                var targetCoords = FileCoordinateCache[stageNum];
                int loopLimit = System.Math.Min(elevators.Count, targetCoords.Count);
                var isolatedRoom = RomManager.IsolatedStages[stageNum];

                for (int i = 0; i < loopLimit; i++)
                {
                    elevators[i].CellX = targetCoords[i].X;
                    elevators[i].CellY = targetCoords[i].Y;
                    elevators[i].IsMapped = true;

                    // Pull home altitude directly from your v0.80 hand-edited text map
                    int terrainTileHeight = isolatedRoom.Heights[elevators[i].CellX, elevators[i].CellY];

                    // Read original ROM range delta, but establish a strict minimum 32-unit tracking track
                    int originalRomDelta = System.Math.Abs(elevators[i].TopPosition - elevators[i].BottomPosition);
                    int forcedMotionRange = System.Math.Max(32, originalRomDelta);

                    // Re-bind physics boundaries directly to our new dynamic deck coordinates
                    elevators[i].BottomPosition = terrainTileHeight;
                    elevators[i].TopPosition = terrainTileHeight + forcedMotionRange;

                    // Launch setup pointers cleanly from home base deck
                    elevators[i].CurrentPosition = elevators[i].BottomPosition;
                    elevators[i].Mode = 0; // Set to climb upward instantly on load
                    elevators[i].CurrentSitTime = 0;
                }
            }
        }
    }
}