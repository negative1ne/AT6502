using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace CrystalCastles.DataEngine
{
    public static class CCFormatMigratorV091
    {
        public static void UpgradeQuarantineFilesToV091()
        {
            // Enforce relative directory tracking parameters
            string targetRelativePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "revised");

            if (!Directory.Exists(targetRelativePath))
            {
                Console.WriteLine($"[MIGRATION ERROR] Target relative path not found: {targetRelativePath}");
                return;
            }

            string[] filePaths = Directory.GetFiles(targetRelativePath, "STAGE_*.txt");
            Console.WriteLine($"[MIGRATION START] Upgrading {filePaths.Length} file layouts to v0.91 specification...");

            foreach (string filePath in filePaths)
            {
                string[] lines = File.ReadAllLines(filePath);
                List<string> processedLines = new List<string>();
                bool insideElevators = false;
                bool insideGems = false;
                bool insideMap = false;

                for (int i = 0; i < lines.Length; i++)
                {
                    string rawLine = lines[i];
                    string cleanLine = rawLine.Trim();

                    // Step 1: Informational Header Version Migration Pass
                    if (cleanLine.Contains("v0.90 DATA ENGINE SPEC") || cleanLine.Contains("v0.90 CONFIGURATION SPECIFICATION"))
                    {
                        rawLine = rawLine.Replace("v0.90", "v0.91");
                    }

                    // Step 2: Edge-Triggered Structural Section Boundaries Matching Injection
                    if (cleanLine.Equals("[ELEVATORS]", StringComparison.OrdinalIgnoreCase))
                    {
                        insideElevators = true;
                    }
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

                    // Append current tracking layout string line
                    processedLines.Add(rawLine);
                }

                // Step 3: Inject the absolute final layout file trailer tag wrapper
                if (insideMap)
                {
                    processedLines.Add("[END_MAP]");
                }

                // Write the updated string list directly back to our quarantined source file safely
                File.WriteAllLines(filePath, processedLines, Encoding.UTF8);
                Console.WriteLine($"  * Migration Complete: {Path.GetFileName(filePath)} -> Upgraded to v0.91");
            }

            Console.WriteLine("[✓] SUCCESS: All quarantine asset files successfully migrated to v0.91 boundaries.");
        }
    }
}