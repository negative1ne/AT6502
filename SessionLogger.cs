// ============================================================================
// SESSIONLOGGER.CS - DYNAMIC RUNTIME LOG FILE ROTATION ENGINE
// ============================================================================
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace cSharpRaylib
{
    public static class SessionLogger
    {
        private static readonly string LogFilename;
        private static readonly string LogPath;

        static SessionLogger()
        {
            // Create a unique file pointer for every execution instance using time-stamped parameters
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            LogFilename = $"session_audit_{timestamp}.log";
            LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogFilename);

            try
            {
                File.WriteAllText(LogPath, $"=== CRYSTAL CASTLES ISOLATED SESSION LOG TRACKER ===\nLaunched: {DateTime.Now}\nTarget File Name: {LogFilename}\n\n", Encoding.UTF8);
            }
            catch { }
        }

        public static void LogStageTransition(int stageNum, string stageName, int cityIndex, List<ElevatorData> elevators)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"\n================================================================================");
                sb.AppendLine($"STAGE LOAD SEQUENCE INITIALIZED: [{stageNum:D2}] {stageName.ToUpper()}");
                sb.AppendLine($"Execution Time: {DateTime.Now} | Underlying Reused City Parent ID: {cityIndex}");
                sb.AppendLine($"================================================================================");

                if (elevators == null || elevators.Count == 0)
                {
                    sb.AppendLine("  * Elevator Memory Trace: 0 active structures found.");
                }
                else
                {
                    sb.AppendLine($"  * Elevator Memory Trace: {elevators.Count} platform elements loaded:");
                    for (int i = 0; i < elevators.Count; i++)
                    {
                        var ev = elevators[i];
                        sb.AppendLine($"    - Platform [{i}]: ScreenX={ev.HorizontalPosition:D3}, ScreenY={ev.VerticalPosition:D3} | MappedCell=({ev.CellX:D2},{ev.CellY:D2}) | ActiveLatching={ev.IsMapped}");
                    }
                }
                sb.AppendLine("--------------------------------------------------------------------------------");

                File.AppendAllText(LogPath, sb.ToString(), Encoding.UTF8);
            }
            catch { }
        }

        /// <summary>
        /// Public safe pointer wrapper helper to allow other modules to stream verification blocks into this session file.
        /// </summary>
        public static void LogVerificationMessage(string message)
        {
            try
            {
                File.AppendAllText(LogPath, message + Environment.NewLine, Encoding.UTF8);
            }
            catch { }
        }
    }
}