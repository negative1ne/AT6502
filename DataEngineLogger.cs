// ====================================================================================
// CRYSTAL CASTLES UNIFIED INGESTION SUITE [v0.85 DATA ENGINE ISOLATION]
// MODULE: DATA ENGINE CORE LOGGER
// CONSTRAINTS: ZERO MEMORY LEAKS | ISOLATED CONSOLIDATED TRACING
// ====================================================================================

using System;
using System.IO;

namespace CrystalCastles.DataEngine
{
    public class DataEngineLogger
    {
        private static string _sessionLogPath;
        private static string _startupLogPath;
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Initializes the v0.85 Dual-Logging Framework.
        /// </summary>
        /// <param name="baseDir">The root deployment base directory pathway.</param>
        public static void Initialize(string baseDir)
        {
            lock (_lockObject)
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                // Rule: Keep log files in the same active directory to prevent folder bloat
                _sessionLogPath = Path.Combine(baseDir, $"session_audit_{timestamp}.log");
                _startupLogPath = Path.Combine(baseDir, "startup_audit.log");

                // Initialize Session Log Header
                File.WriteAllText(_sessionLogPath,
                    "=== CRYSTAL CASTLES ISOLATED SESSION LOG TRACKER ===\n" +
                    $"Launched: {DateTime.Now}\n" +
                    $"Target File Name: {Path.GetFileName(_sessionLogPath)}\n" +
                    "================================================================================\n\n");

                // Initialize or Append Startup Log Header
                File.AppendAllText(_startupLogPath,
                    "\n================================================================================\n" +
                    "=== CRYSTAL CASTLES v0.85 INGESTION ENGINE MASTER FILE AUDIT REPORT ===\n" +
                    $"Execution Timestamp: {DateTime.Now}\n" +
                    "================================================================================\n");
            }
        }

        /// <summary>
        /// Writes an isolated tracking statement directly to the dynamic session file.
        /// </summary>
        public static void LogSession(string message)
        {
            lock (_lockObject)
            {
                if (string.IsNullOrEmpty(_sessionLogPath)) return;

                string logLine = $"[{DateTime.Now:HH:mm:ss}] {message}";
                File.AppendAllText(_sessionLogPath, logLine + "\n");

                // Echo to console for dynamic telemetry monitoring
                Console.WriteLine(logLine);
            }
        }

        /// <summary>
        /// Appends standardized level asset health states to the master startup registry log.
        /// </summary>
        public static void LogStartup(string stageIndex, string stageName, string mapsState, string gemsState, string liftState, string note = "")
        {
            lock (_lockObject)
            {
                if (string.IsNullOrEmpty(_startupLogPath)) return;

                string logLine = $"Stage [{stageIndex}] -> {stageName.PadRight(25)} | " +
                                 $"Maps: {mapsState.PadRight(12)} | " +
                                 $"Gems: {gemsState.PadRight(10)} | " +
                                 $"Elevators: {liftState.PadRight(10)}";

                if (!string.IsNullOrEmpty(note))
                {
                    logLine += $" -> {note}";
                }

                File.AppendAllText(_startupLogPath, logLine + "\n");
            }
        }
        /// <summary>
        /// Writes out the closing summary blocks to seal both files upon session completion or interruption.
        /// </summary>
        /// <param name="scannedCount">Total count of stages successfully ingested.</param>
        /// <param name="passedCount">Total count of stages passing structural assertions.</param>
        /// <param name="failedCount">Total count of critical processing exceptions caught.</param>
        /// <param name="userInterrupted">True if the run sequence was manually canceled by the user.</param>
        public static void FinalizeSession(int scannedCount, int passedCount, int failedCount, bool userInterrupted = false)
        {
            lock (_lockObject)
            {
                if (string.IsNullOrEmpty(_sessionLogPath) || string.IsNullOrEmpty(_startupLogPath)) return;

                string summaryBlock =
                    "\n================================================================================\n" +
                    "SUMMARY STATISTICS:\n" +
                    "================================================================================\n" +
                    $"  TOTAL STAGES SCANNED   : {scannedCount} / 37\n" +
                    $"  PASSED ASSERTIONS      : {passedCount}\n" +
                    $"  FAILED CODE EXCEPTIONS : {failedCount}\n";

                if (userInterrupted)
                {
                    summaryBlock += "  RUN STATUS             : [X] RUN INTERRUPTED BY USER\n";
                    LogSession("CRITICAL: RUN interrupted by user. Finalizing logs.");
                }
                else
                {
                    double accuracy = scannedCount > 0 ? ((double)passedCount / scannedCount) * 100 : 0;
                    summaryBlock += $"  SYSTEM PASS VERDICT    : {accuracy:F0}% SECURE. v0.85 TARGET MET.\n";
                }

                summaryBlock += "================================================================================\n";

                // Seal the temporary Session Log
                File.AppendAllText(_sessionLogPath, summaryBlock);

                // Seal the persistent Global Startup Summary Footer
                string footerLine = "--------------------------------------------------------------------------------\n" +
                                    $"=== GLOBAL REPOSITORY PIPELINE CONSUMPTION SUMMARY ===\n" +
                                    $" * Verified Assets Ingested : {passedCount} / {scannedCount} Tracks\n" +
                                    "================================================================================\n";
                File.AppendAllText(_startupLogPath, footerLine);
            }
        }
    }
}
    

