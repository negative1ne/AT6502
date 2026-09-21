// ====================================================================================
// CRYSTAL CASTLES UNIFIED INGESTION SUITE [v0.90 DATA ENGINE SPEC]
// MODULE: CCUNIFIEDLOGGER.CS - STANDALONE DEFERRED EVENT & EXPORT LOG ENGINE
// CONSTRAINTS: MEMORY-RESIDENT TRANSIENT CACHE | ZERO 60FPS DISK UPDATE STUTTER
// ====================================================================================

using System;
using System.IO;
using System.Text;

namespace cSharpRaylib
{
    public static class CCUnifiedLogger
    {
        private static string _sessionTimestamp = "";

        // v0.90 LIFECYCLE INITIALIZER: Automatically binds to runtime startup contexts
        public static void Initialize(string projectBaseDir)
        {
            _sessionTimestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string rootRuntimeLog = Path.Combine(projectBaseDir, "startup_audit.log");

            try
            {
                // Clean append baseline registration to tracking root
                using (StreamWriter sw = new StreamWriter(rootRuntimeLog, true, Encoding.UTF8))
                {
                    sw.WriteLine($"\n================================================================================");
                    sw.WriteLine($"=== CRYSTAL CASTLES UNIFIED INGESTION SUITE ENGINE AUDIT LOG [v0.90 SPEC] ===");
                    sw.WriteLine($"================================================================================");
                    sw.WriteLine($"[RUN DETECTED]: {DateTime.Now:MM/dd/yyyy hh:mm:ss tt} | Root Context: {projectBaseDir}");
                    sw.WriteLine($"[VERDICT]     : INITIALIZING ISOLATED DIRECT MEMORY RUNTIME ARCHITECTURE PASS.");
                    sw.WriteLine($"--------------------------------------------------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LOGGER FAULT] Unable to instantiate base runtime audit stream: {ex.Message}");
            }
        }

        // v0.90 ISOLATED TRANSACTION DUMPER: Invoked solely on manual 'E' hotkey command passes
        public static void ExportActiveSessionSummary(int currentRoom, string stageName, bool[,] activeGridCells)
        {
            try
            {
                string outFileName = $"session_audit_stage_{currentRoom:D2}_{_sessionTimestamp}.log";
                string fullExportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, outFileName);

                using (StreamWriter sw = new StreamWriter(fullExportPath, true, Encoding.UTF8))
                {
                    sw.WriteLine($"================================================================================");
                    sw.WriteLine($"CRYSTAL CASTLES v0.90 UNIFIED REPOSITORY SINGLE SHEET SESSION SNAPSHOT");
                    sw.WriteLine($"Export Timestamp: {DateTime.Now:MM/dd/yyyy hh:mm:ss tt}");
                    sw.WriteLine($"Target Stage     : Room [{currentRoom:D2}] - {stageName}");
                    sw.WriteLine($"================================================================================");

                    int activeLockedCount = 0;
                    sw.WriteLine("\n[STAGE RETENTION MATRIX PROFILE LAYOUT]");
                    sw.WriteLine("    00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 20 21");

                    for (int x = 0; x < 22; x++)
                    {
                        StringBuilder rowText = new StringBuilder($"{x:D2} ");
                        for (int y = 0; y < 22; y++)
                        {
                            if (activeGridCells[x, y])
                            {
                                rowText.Append(" L ");
                                activeLockedCount++;
                            }
                            else
                            {
                                rowText.Append(" . ");
                            }
                        }
                        sw.WriteLine(rowText.ToString());
                    }

                    sw.WriteLine($"\n[METRIC SUMMARY]: Total inspection grid cells locked for validation = {activeLockedCount:D2}");
                    sw.WriteLine($"[STATUS]: Export processing completed successfully. Stream pipeline safely closed.\n");
                }

                // Clear hardware indicator signaling complete
                Console.Beep(1800, 250);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CRITICAL EXPORT FAULT] Failed writing session data profile to disk: {ex.Message}");
            }
        }
    }
}