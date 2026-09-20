// ====================================================================================
// CRYSTAL CASTLES UNIFIED INGESTION SUITE [v0.85 DATA ENGINE ISOLATION]
// MODULE: ELEVATOR DATA ENGINE PARSER
// CONSTRAINTS: NO DOCK/STRIDE DRIFT | PURE SPATIAL VECTOR ENCAPSULATION
// ====================================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace CrystalCastles.DataEngine
{
    public class ElevatorEntity
    {
        public int GridX { get; set; }
        public int GridY { get; set; }
        public float Height { get; set; }
    }

    public class ElevatorDataEngine
    {
        private const string VersionBanner = "Pipeline Version Target Profile: v0.85 DATA ENGINE ISOLATION";
        private const string DashSeparator = "---------------------------------------------------------------";

        // ====================================================================================
        // FIX BANNER: ELEVATOR ENGINE PARSER - ATTEMPT 4 FORCE LINE-OFFSET ENGAGED (v0.85)
        // ====================================================================================
        public static List<ElevatorEntity> LoadElevatorFile(string filePath)
        {
            var elevators = new List<ElevatorEntity>();
            if (!File.Exists(filePath)) return elevators;

            string[] lines = File.ReadAllLines(filePath);
            DataEngineLogger.LogSession($"[MARKER DIAGNOSTIC]: Scanning {Path.GetFileName(filePath)} | Total Raw Lines = {lines.Length}");

            // Strategy: Force processing to execute starting exactly at line index 4 (5th line)
            for (int i = 4; i < lines.Length; i++)
            {
                string trimmed = lines[i].Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("==") || trimmed.StartsWith("--")) continue;

                try
                {
                    if (trimmed.Contains("MappedCell="))
                    {
                        int openP = trimmed.IndexOf('('); int closeP = trimmed.IndexOf(')');
                        if (openP != -1 && closeP != -1)
                        {
                            string[] coords = trimmed.Substring(openP + 1, closeP - openP - 1).Split(',');
                            elevators.Add(new ElevatorEntity
                            {
                                GridX = int.Parse(coords[0].Trim()),
                                GridY = int.Parse(coords[1].Trim()),
                                Height = 0.00f
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    DataEngineLogger.LogSession($"  [!] PARSE FAULT at Line {i + 1}: {ex.Message}");
                }
            }

            string reportStatus = elevators.Count > 0 ? $"[DATA FOUND - Count: {elevators.Count}]" : "[EMPTY ARRAY RECORDED]";
            DataEngineLogger.LogSession($"[MARKER REPORT]: Elevator Load Complete -> {reportStatus}");
            return elevators;
        }
        // ====================================================================================
        // ====================================================================================
        /// <summary>
        /// Automatically stamps and exports a list of elevator entities to disk under the v0.85 specification.
        /// </summary>
        /// <param name="filePath">Target destination file path.</param>
        /// <param name="stageNumber">Two-digit stage index indicator string (e.g., "00").</param>
        /// <param name="stageName">The capitalized level name string.</param>
        /// <param name="elevators">The live collection of elevator entities to export.</param>
        public static void SaveElevatorFile(string filePath, string stageNumber, string stageName, List<ElevatorEntity> elevators)
        {
            try
            {
                using (var writer = new StreamWriter(filePath, false))
                {
                    // Write v0.85 Layout Target Headers
                    writer.WriteLine($"=== CRYSTAL CASTLES ELEVATOR CONFIG: STAGE {stageNumber} ({stageName.ToUpper()}) ===");
                    writer.WriteLine(VersionBanner);
                    writer.WriteLine($"Active Configured Lift Count: {elevators.Count}");
                    writer.WriteLine(DashSeparator);

                    // Export formatted vector entries
                    for (int i = 0; i < elevators.Count; i++)
                    {
                        var e = elevators[i];
                        string xString = e.GridX.ToString("D2");
                        string yString = e.GridY.ToString("D2");
                        string hString = e.Height.ToString("F2", CultureInfo.InvariantCulture);

                        writer.WriteLine($"Lift_Index_{i:D2}: GridX={xString}, GridY={yString}, Height={hString}");
                    }
                }

                DataEngineLogger.LogSession($"SUCCESS: Synchronized and wrote v0.85 elevator asset to disk: {Path.GetFileName(filePath)}");
            }
            catch (Exception ex)
            {
                DataEngineLogger.LogSession($"ERROR: Failed to execute automated file write for {Path.GetFileName(filePath)}: {ex.Message}");
            }
        }
    }
}

