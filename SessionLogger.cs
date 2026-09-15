using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace cSharpRaylib
{
    public static class SessionLogger
    {
        private static readonly string LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "session_execution_audit.log");

        static SessionLogger()
        {
            // Clear any stale historical text sheets on new boot launch passes
            try
            {
                File.WriteAllText(LogPath, $"=== CRYSTAL CASTLES CORE SESSION LOG INITIALIZED ===\nStarted: {DateTime.Now}\n\n", Encoding.UTF8);
            }
            catch { }
        }

        /// <summary>
        /// Single-shot telemetry recorder. Appends clean state metrics down onto disk file.
        /// </summary>
        public static void LogStageTransition(int stageNum, string stageName, int cityIndex, List<ElevatorData> elevators)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"\n================================================================================");
                sb.AppendLine($"STAGE LOAD INITIALIZED: [{stageNum:D2}] {stageName.ToUpper()}");
                sb.AppendLine($"Timestamp: {DateTime.Now} | Underlying Shared City Base Index: {cityIndex}");
                sb.AppendLine($"================================================================================");

                if (elevators == null || elevators.Count == 0)
                {
                    sb.AppendLine("  * Elevator Inventory: 0 active hardware descriptors detected.");
                }
                else
                {
                    sb.AppendLine($"  * Elevator Inventory: {elevators.Count} tracking structures active in memory array:");
                    for (int i = 0; i < elevators.Count; i++)
                    {
                        var ev = elevators[i];
                        sb.AppendLine($"    - Lift [{i}]: RawArcadeX={ev.HorizontalPosition:D3}, RawArcadeY={ev.VerticalPosition:D3} | CachedCell=({ev.CellX:D2},{ev.CellY:D2}) | IsMapped={ev.IsMapped}");
                    }
                }
                sb.AppendLine("--------------------------------------------------------------------------------");

                File.AppendAllText(LogPath, sb.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Session logger write failure: {ex.Message}");
            }
        }
    }
}