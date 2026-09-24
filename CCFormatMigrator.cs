// ============================================================================
// FIX BANNER: CCFormatMigrator.cs - CENTRAL CONFIG VERSIONING MIGRATION [v0.95]
// ============================================================================
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace cSharpRaylib
{
    public static class CCFormatMigrator
    {
        // Master ground truth coordinates lookup dictionary array ledger
        public static readonly Dictionary<int, List<(int X, int Y)>> GroundTruthCoordinates = new Dictionary<int, List<(int, int)>>
        {
            { 0, new List<(int, int)> { (18, 5), (5, 18) } },
            { 2, new List<(int, int)> { (17, 17) } },
            { 3, new List<(int, int)> { (4, 19), (11, 11), (13, 2), (17, 19) } },
            { 5, new List<(int, int)> { (5, 19), (19, 5), (6, 10), (10, 6), (6, 6) } },
            { 6, new List<(int, int)> { (2, 13), (6, 9), (10, 5), (14, 7) } },
            { 7, new List<(int, int)> { (16, 16) } },
            { 8, new List<(int, int)> { (4, 19), (11, 11), (13, 2), (17, 19) } },
            { 11, new List<(int, int)> { (4, 17), (8, 9), (10, 4), (11, 16), (17, 5) } },
            { 12, new List<(int, int)> { (2, 12), (2, 19), (3, 13), (11, 16), (17, 6) } },
            { 13, new List<(int, int)> { (4, 4) } },
            { 14, new List<(int, int)> { (4, 19), (8, 8), (16, 16), (19, 4) } }
        };

        public static void InjectGroundTruthElevators()
        {
            // Task 8 Isolated Custom Editor Relative Data Path Split Integration Pass
            string targetFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "revised");
            string startupAuditPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startup_audit.log");

            try
            {
                // Format a highly clear, clean structural context header text block utilizing the global config variable
                StringBuilder startupHeader = new StringBuilder();
                startupHeader.AppendLine("================================================================================");
                startupHeader.AppendLine($"[STARTUP TRACE] Execution Run Initiated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                startupHeader.AppendLine($"[ENGINE VERSION ENVIRONMENT] -> Active Build Spec: v{CCFormatConfig.VersionTag}");
                startupHeader.AppendLine($"[I/O TARGET PATH] -> Fully Resolved Base Directory Location:");
                startupHeader.AppendLine($"                  {targetFolder}");
                startupHeader.AppendLine("================================================================================");

                File.WriteAllText(startupAuditPath, startupHeader.ToString(), Encoding.UTF8);
                File.AppendAllText(startupAuditPath, $"[PATH SPLIT INITIALIZED] -> Editor Asset Target Isolated to: \\data\\revised\\\n", Encoding.UTF8);
            }
            catch (Exception logPathEx)
            {
                string sessionAuditFallback = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "session_audit.log");
                File.AppendAllText(sessionAuditFallback, $"[!] PATH TRACE ERROR: {logPathEx.Message}\n", Encoding.UTF8);
            }

            if (!Directory.Exists(targetFolder)) return;

            try
            {
                // Reconstruct an explicit unique subdirectory lane dynamically injecting our VersionTag string field
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "backups");
                string backupFolder = Path.Combine(backupBase, $"bk_{timestamp}_v{CCFormatConfig.VersionTag.Replace(".", "")}_pre_run");

                if (!Directory.Exists(backupFolder))
                {
                    Directory.CreateDirectory(backupFolder);
                }

                string[] activeSheets = Directory.GetFiles(targetFolder, "STAGE_*.txt");
                foreach (string activeFile in activeSheets)
                {
                    string fileName = Path.GetFileName(activeFile);
                    string destPath = Path.Combine(backupFolder, fileName);
                    File.Copy(activeFile, destPath, true);
                }
            }
            catch (Exception backupEx)
            {
                string sessionAuditFallback = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "session_audit.log");
                File.AppendAllText(sessionAuditFallback, $"[!] SAFETY CRITICAL ERROR: Backup sequence failed: {backupEx.Message}\n", Encoding.UTF8);
                return;
            }

            // FIX BANNER: v0.95 THE "READ-ONLY" IN-MEMORY VALIDATION SCANNER GATE [v0.95]
            string[] validationPaths = Directory.GetFiles(targetFolder, "STAGE_*.txt");
            bool systemHasAnomalies = false;

            foreach (string valPath in validationPaths)
            {
                string[] checkLines = File.ReadAllLines(valPath);
                int elevatorsHeader = 0; int elevatorsFooter = 0;
                int gemsHeader = 0; int gemsFooter = 0;
                int mapHeader = 0; int mapFooter = 0;

                foreach (string line in checkLines)
                {
                    string token = line.Trim().ToUpper();
                    if (token == "[ELEVATORS]") elevatorsHeader++;
                    if (token == "[END_ELEVATORS]") elevatorsFooter++;
                    if (token == "[GEMS]") gemsHeader++;
                    if (token == "[END_GEMS]") gemsFooter++;
                    if (token == "[MAP]") mapHeader++;
                    if (token == "[END_MAP]") mapFooter++;
                }

                // Verify that every single asset layout file contains an exact matching pair of section tags
                bool isSheetPristine = (elevatorsHeader == 1 && elevatorsFooter == 1 &&
                                        gemsHeader == 1 && gemsFooter == 1 &&
                                        mapHeader == 1 && mapFooter == 1) ||
                                       (checkLines.Contains("[NONE]") && gemsHeader == 1 && mapHeader == 1);

                if (!isSheetPristine)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[!] CORRUPTION WARNING: Structural tag anomaly detected inside: {Path.GetFileName(valPath)}");
                    Console.WriteLine($"    Metrics -> [ELEVATORS]: {elevatorsHeader}/{elevatorsFooter} [GEMS]: {gemsHeader}/{gemsFooter} [MAP]: {mapHeader}/{mapFooter}");
                    Console.ResetColor();
                    systemHasAnomalies = true;
                }
            }

            // Defensive Abort Trigger: Block file processing entirely if any sheet is structurally suspect
            if (systemHasAnomalies)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[!] SAFETY HALT: Utility run blocked completely. No text files were modified on disk.");
                Console.WriteLine("    Please check the logged files above inside your text editor to repair double-tag headers.");
                Console.ResetColor();
                return;
            }

            // Safe Gateway Cleared: Proceed with actual file writing execution loops
            string[] filePaths = Directory.GetFiles(targetFolder, "STAGE_*.txt");
            foreach (string filePath in filePaths)
            {
                string name = Path.GetFileNameWithoutExtension(filePath);
                if (!int.TryParse(name.Substring(6, 2), out int stageID)) continue;

                string[] lines = File.ReadAllLines(filePath);
                // FIX BANNER: SECTION BUILDER ENGINE WITH NATIVE GEMS RESTORER [v0.95]
                List<string> output = new List<string>();
                bool insideElevatorBlock = false;
                bool insideGemsBlock = false;

                for (int lineIdx = 0; lineIdx < lines.Length; lineIdx++)
                {
                    string currentLine = lines[lineIdx];
                    string clean = currentLine.Trim();

                    // 1. ELEVATOR GENERATION CONTROLLER
                    if (clean.Equals("[ELEVATORS]", StringComparison.OrdinalIgnoreCase))
                    {
                        output.Add(currentLine);
                        insideElevatorBlock = true;

                        if (GroundTruthCoordinates.ContainsKey(stageID))
                        {
                            var coords = GroundTruthCoordinates[stageID];
                            for (int liftIdx = 0; liftIdx < coords.Count; liftIdx++)
                            {
                                var cell = coords[liftIdx];
                                output.Add($"LIFT_{liftIdx}: RowX={cell.X:D2} | ColY={cell.Y:D2} | BottomH=004 | TopH=032 | Direction=UP");
                            }
                        }
                        else
                        {
                            output.Add("[NONE]");
                        }

                        // FIX BANNER: v0.95 COMPONENT OVERRIDE MUTATION TRACE STAMP [v0.95]
                        try
                        {
                            string sessionAuditPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "session_audit.log");
                            string traceStamp = $"[MUTATION TRACE] STAGE_{stageID:D2} -> Generated {GroundTruthCoordinates[stageID].Count} active elevators slots. Origin Caller: CCFormatMigratorV091.InjectGroundTruthElevators\n";
                            File.AppendAllText(sessionAuditPath, traceStamp, Encoding.UTF8);
                        }
                        catch { /* Soft ignore if file lock is busy */ }
                        continue;
                    }

                    if (clean.Equals("[END_ELEVATORS]", StringComparison.OrdinalIgnoreCase))
                    {
                        insideElevatorBlock = false;
                        output.Add(currentLine);
                        continue;
                    }

                    if (insideElevatorBlock) continue;

                    // 2. GEMS RESTORER CONTROLLER
                    if (clean.Equals("[GEMS]", StringComparison.OrdinalIgnoreCase))
                    {
                        output.Add(currentLine);
                        insideGemsBlock = true;

                        // Inject clean double-spaced data loops to restore the missing rooms
                        if (stageID == 1) output.AddRange(GetStage01GemsBlueprint());
                        else if (stageID == 2) output.AddRange(GetStage02GemsBlueprint());
                        else if (stageID == 3) output.AddRange(GetStage03GemsBlueprint());
                        else if (stageID == 4) output.AddRange(GetStage04GemsBlueprint());
                        continue;
                    }

                    if (clean.Equals("[END_GEMS]", StringComparison.OrdinalIgnoreCase))
                    {
                        insideGemsBlock = false;
                        output.Add(currentLine);
                        continue;
                    }

                    // FIX BANNER: v0.95 DROP LEGACY GEMS MATRIX PASS STRIDE
                    // Completely drops legacy character records for targeted stages until hitting the section boundary tag
                    if (insideGemsBlock && (stageID == 1 || stageID == 2 || stageID == 3 || stageID == 4)) continue;

                    output.Add(currentLine);
                }
                File.WriteAllLines(filePath, output, Encoding.UTF8);
            }
            Console.WriteLine("[✓] SUCCESS: All active elevator level configurations updated with clean ground-truth values.");
        }
        public static void UpgradeQuarantineFilesToV091()
        {
            string targetRelativePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "revised");
            if (!Directory.Exists(targetRelativePath)) return;

            string[] filePaths = Directory.GetFiles(targetRelativePath, "STAGE_*.txt");
            foreach (string filePath in filePaths)
            {
                string[] lines = File.ReadAllLines(filePath);
                List<string> processedLines = new List<string>();
                bool insideElevators = false; bool insideGems = false; bool insideMap = false;

                for (int i = 0; i < lines.Length; i++)
                {
                    string rawLine = lines[i]; string cleanLine = rawLine.Trim();
                    if (cleanLine.Contains("v0.90 DATA ENGINE SPEC") || cleanLine.Contains("v0.90 CONFIGURATION SPECIFICATION"))
                        rawLine = rawLine.Replace("v0.90", "v0.91");

                    if (cleanLine.Equals("[ELEVATORS]", StringComparison.OrdinalIgnoreCase)) insideElevators = true;
                    else if (cleanLine.Equals("[GEMS]", StringComparison.OrdinalIgnoreCase))
                    {
                        if (insideElevators) { processedLines.Add("[END_ELEVATORS]"); insideElevators = false; }
                        insideGems = true;
                    }
                    else if (cleanLine.Equals("[MAP]", StringComparison.OrdinalIgnoreCase))
                    {
                        if (insideElevators) { processedLines.Add("[END_ELEVATORS]"); insideElevators = false; }
                        if (insideGems) { processedLines.Add("[END_GEMS]"); insideGems = false; }
                        insideMap = true;
                    }

                    if (cleanLine.Equals("[END_ELEVATORS]", StringComparison.OrdinalIgnoreCase) ||
                        cleanLine.Equals("[END_GEMS]", StringComparison.OrdinalIgnoreCase) ||
                        cleanLine.Equals("[END_MAP]", StringComparison.OrdinalIgnoreCase)) continue;

                    processedLines.Add(rawLine);
                }
                if (insideMap) processedLines.Add("[END_MAP]");

                // FIX BANNER: DEFENSIVE ANTI-CORRUPTION DOUBLE-TAG SAFEGUARD GATE [v0.95]
                int elevatorsCount = 0; int endElevatorsCount = 0;
                int gemsCount = 0; int endGemsCount = 0;
                int mapCount = 0; int endMapCount = 0;

                foreach (string line in processedLines)
                {
                    string upperToken = line.Trim().ToUpper();
                    if (upperToken == "[ELEVATORS]") elevatorsCount++;
                    if (upperToken == "[END_ELEVATORS]") endElevatorsCount++;
                    if (upperToken == "[GEMS]") gemsCount++;
                    if (upperToken == "[END_GEMS]") endGemsCount++;
                    if (upperToken == "[MAP]") mapCount++;
                    if (upperToken == "[END_MAP]") endMapCount++;
                }

                bool isStructureValid = (elevatorsCount == 1 && endElevatorsCount == 1 &&
                                         gemsCount == 1 && endGemsCount == 1 &&
                                         mapCount == 1 && endMapCount == 1) ||
                                        (processedLines.Contains("[NONE]") && gemsCount == 1 && mapCount == 1);

                if (isStructureValid)
                {
                    File.WriteAllLines(filePath, processedLines, Encoding.UTF8);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[!] SAFETY BLOCK: Aborted write to {Path.GetFileName(filePath)} to prevent tag corruption.");
                    Console.ResetColor();
                }
            }
        }
    
    private static List<string> GetStage01GemsBlueprint()
        {
            return new List<string> {
                "     00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 20 21",
                "00   .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ",
                "01   .  .  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  .  ",
                "02   .  *  .  .  .  .  .  .  *  .  .  .  .  *  .  .  .  .  .  .  *  .  ",
                "03   .  *  .  .  .  .  .  .  *  .  .  .  .  *  .  .  .  .  .  .  *  .  ",
                "04   .  *  .  .  .  .  .  .  *  .  .  .  .  *  .  .  .  .  .  .  *  .  ",
                "05   .  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  .  ",
                "06   .  *  .  .  .  *  .  .  .  .  .  .  .  .  .  .  *  .  .  .  *  .  ",
                "07   .  *  .  .  .  *  .  .  .  .  .  .  .  .  .  .  *  .  .  .  *  .  ",
                "08   .  *  .  .  .  *  .  .  .  .  .  .  .  .  .  .  *  .  .  .  *  .  ",
                "09   .  *  .  .  .  *  .  .  .  .  .  .  .  .  .  .  *  .  .  .  *  .  ",
                "10   .  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  .  ",
                "11   .  *  .  .  .  .  .  .  *  .  .  .  .  *  .  .  .  .  .  .  *  .  ",
                "12   .  *  .  .  .  .  .  .  *  .  .  .  .  *  .  .  .  .  .  .  *  .  ",
                "13   .  *  .  .  .  .  .  .  *  .  .  .  .  *  .  .  .  .  .  .  *  .  ",
                "14   .  *  .  .  .  .  .  .  *  .  .  .  .  *  .  .  .  .  .  .  *  .  ",
                "15   .  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  .  ",
                "16   .  *  .  .  .  .  .  .  .  .  *  *  .  .  .  .  .  .  .  .  *  .  ",
                "17   .  *  .  .  .  .  .  .  .  .  *  *  .  .  .  .  .  .  .  .  *  .  ",
                "18   .  *  .  .  .  .  .  .  .  .  *  *  .  .  .  .  .  .  .  .  *  .  ",
                "19   .  *  .  .  .  .  .  .  .  .  *  *  .  .  .  .  .  .  .  .  *  .  ",
                "20   .  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  .  ",
                "21   .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  "
            };
        }

        private static List<string> GetStage02GemsBlueprint()
        {
            return new List<string> {
                "     00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 20 21",
                "00   .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ",
                "01   .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ",
                "02   .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  *  .  ",
                "03   .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  *  .  ",
                "04   .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  *  .  *  .  ",
                "05   .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  *  .  *  .  ",
                "06   .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  *  .  *  .  ",
                "07   .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  *  .  *  .  ",
                "08   .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  *  .  *  .  ",
                "09   .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  *  *  *  .  ",
                "10   .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  *  .  *  .  ",
                "11   .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  *  .  *  .  ",
                "12   .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  *  *  *  .  ",
                "13   .  .  .  .  .  .  .  .  .  .  .  .  .  .  *  *  *  *  *  .  *  .  ",
                "14   .  .  .  .  .  .  .  .  .  .  .  .  .  *  *  *  *  *  *  .  *  .  ",
                "15   .  .  .  .  .  .  .  .  .  .  .  .  .  *  *  .  *  *  *  .  *  .  ",
                "16   .  .  .  .  .  .  .  .  .  .  .  .  .  *  *  *  *  *  *  .  *  .  ",
                "17   .  .  .  .  .  .  .  .  .  .  .  .  .  *  *  *  *  .  *  .  *  .  ",
                "18   .  .  .  .  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  .  *  .  ",
                "19   .  .  .  .  .  .  .  .  .  *  .  .  *  .  .  .  .  .  .  .  *  .  ",
                "20   .  .  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  .  ",
                "21   .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  "
            };
        }

        private static List<string> GetStage03GemsBlueprint()
        {
            return new List<string> {
                "     00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 20 21",
                "00   .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ",
                "01   .  .  .  .  .  .  .  .  .  .  .  .  *  *  *  *  *  *  *  *  *  .  ",
                "02   .  .  .  .  .  .  .  .  .  .  .  .  *  *  .  .  *  .  .  *  *  .  ",
                "03   .  .  .  .  .  .  .  .  .  .  .  .  *  *  .  .  *  .  .  *  *  .  ",
                "04   .  .  .  .  .  .  .  .  .  .  .  .  *  *  .  .  *  *  *  .  *  .  ",
                "05   .  .  .  .  .  .  .  .  .  .  .  .  *  *  .  .  .  .  *  *  *  .  ",
                "06   .  .  .  .  .  .  .  .  .  .  .  .  *  *  .  .  .  .  *  .  .  .  ",
                "07   .  .  .  .  .  .  .  .  .  .  .  .  .  *  .  .  .  .  *  .  .  .  ",
                "08   .  .  .  .  .  .  .  .  .  .  .  .  *  *  *  *  *  *  *  .  .  .  ",
                "09   .  .  .  .  .  .  .  .  .  *  *  .  *  .  *  .  *  .  *  *  *  .  ",
                "10   .  .  .  .  .  .  .  .  .  *  *  .  *  *  *  *  *  .  .  *  *  .  ",
                "11   .  .  .  .  .  .  .  .  .  .  .  .  *  .  .  .  .  .  .  *  *  .  ",
                "12   .  *  *  *  *  *  *  .  .  .  .  *  *  .  .  .  .  .  .  *  *  .  ",
                "13   .  *  .  *  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  *  *  .  ",
                "14   .  *  *  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  *  *  .  ",
                "15   .  *  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  *  *  .  ",
                "16   .  *  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  *  *  .  ",
                "17   .  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  .  *  .  ",
                "18   .  *  .  .  .  .  .  .  .  .  *  *  *  *  *  *  *  *  *  *  *  .  ",
                "19   .  *  .  .  .  .  .  .  .  .  *  .  .  .  .  .  .  .  .  .  *  .  ",
                "20   .  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  .  ",
                "21   .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  "
            };
        }
        // FIX BANNER: v0.95 STAGE 04 HIDDEN RAMP GEM bluePRINT [v0.95]
        private static List<string> GetStage04GemsBlueprint()
        {
            return new List<string> {
                "    00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 20 21",
                "00  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  . ",
                "01  .  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  .  . ",
                "02  .  *  *  *  *  .  .  .  .  .  .  .  .  .  .  .  *  *  *  *  *  . ",
                "03  .  *  *  .  .  .  .  .  .  .  .  .  .  .  .  .  *  *  *  *  *  . ",
                "04  .  *  *  .  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  .  *  . ",
                "05  .  *  .  .  *  *  *  *  *  *  .  .  .  .  .  .  *  *  *  *  *  . ",
                "06  .  *  .  .  *  *  .  *  *  *  .  .  .  .  .  .  *  .  *  *  *  . ",
                "07  .  *  .  .  *  *  *  *  *  *  .  .  .  .  .  .  *  .  *  *  *  . ",
                "08  .  *  .  .  *  *  *  *  .  .  .  .  .  .  .  .  *  .  *  *  *  . ",
                "09  .  *  .  .  *  *  *  *  .  .  .  .  .  .  .  .  *  .  .  .  *  . ",
                "10  .  *  .  .  *  .  .  .  .  .  .  .  .  .  .  .  *  .  .  .  *  . ",
                "11  .  *  .  .  *  .  .  .  .  .  .  .  .  .  .  .  *  .  .  .  *  . ",
                "12  .  *  .  .  *  .  .  .  .  .  .  .  .  .  .  .  *  .  .  .  *  . ",
                "13  .  *  .  .  *  .  .  .  .  .  .  .  .  .  .  .  *  *  .  *  *  . ",
                "14  .  *  .  .  *  .  .  .  .  .  .  .  .  .  .  .  *  *  .  *  *  . ",
                "15  .  *  .  .  *  .  .  .  .  .  .  .  .  .  .  .  *  *  .  *  *  . ",
                "16  .  *  *  *  *  *  *  *  *  *  *  *  *  *  *  .  *  .  *  *  .  . ",
                "17  .  *  *  *  *  *  .  .  .  .  .  .  .  *  .  *  *  *  *  *  *  . ",
                "18  .  *  *  *  *  *  *  *  *  .  .  .  .  .  .  .  *  *  *  *  *  . ",
                "19  .  *  *  *  .  *  *  *  *  .  .  .  .  *  *  *  *  *  *  *  *  . ",
                "20  .  .  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  . ",
                "21  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  . "
            };
        }
    }
}
    

