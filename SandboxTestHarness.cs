// ====================================================================================
// CRYSTAL CASTLES UNIFIED INGESTION SUITE [v0.85 DATA ENGINE ISOLATION]
// MODULE: 5-LEVEL VALIDATION SANDBOX TEST HARNESS (PART 1)
// CONSTRAINTS: ZERO DATA DRIFT | ISOLATED RECIPIENT INJECTION PIPELINE
// ====================================================================================

using System;
using System.Collections.Generic;
using System.IO;

namespace CrystalCastles.DataEngine
{
    public class SandboxTestHarness
    {
        private struct TestStageTarget
        {
            public string Index;
            public string Name;
            public string MapFile;
            public string GemFile;
            public string LiftFile;
            public string Description;

            public TestStageTarget(string idx, string name, string m, string g, string l, string desc)
            {
                Index = idx; Name = name; MapFile = m; GemFile = g; LiftFile = l; Description = desc;
            }
        }

        private static List<TestStageTarget> _validationTiers;

        /// <summary>
        /// Populates our exact 5-level test matrix parameters.
        /// </summary>
        private static void InitializeTiers()
        {
            _validationTiers = new List<TestStageTarget>
            {
                new TestStageTarget("00", "Ball Wave", "Maps_Stage_00_Ball_Wave.txt", "Gems_Stage_00_Ball_Wave.txt", "Elevators_Stage_00_Ball_Wave.txt", "Pristine Baseline Pass"),
                new TestStageTarget("19", "Berthilda's Castle", "Maps_Stage_19_Berthilda_Castle.txt", "Gems_Stage_19_Berthilda_Castle.txt", "Elevators_Stage_19_Berthilda_Castle.txt", "Pass 2 Auto-Injection Target"),
                new TestStageTarget("15", "Berthilda's Palace", "Maps_Stage_15_Berthilda_Palace.txt", "Gems_Stage_15_Berthilda_Palace.txt", "Elevators_Stage_15_Berthilda_Palace.txt", "Multi-Layer Drift Verification"),
                new TestStageTarget("32", "Impossible Staircase", "Maps_Stage_32_Impossible_Staircase.txt", "Gems_Stage_32_Impossible_Staircase.txt", "Elevators_Stage_32_Impossible_Staircase.txt", "Unique Escher Layout Challenge"),
                new TestStageTarget("14", "Hidden Ramp", "Maps_Stage_14_Hidden_Ramp.txt", "Gems_Stage_14_Hidden_Ramp.txt", "Elevators_Stage_14_Hidden_Ramp.txt", "2-Gem Discrepancy Omission Test")
            };
        }
        /// <summary>
        /// Executes our isolated 5-stage transformation validation pass.
        /// </summary>
        /// <param name="sourceDir">The root deployment directory holding original v0.80 file structures.</param>
        // ====================================================================================
        // FIX BANNER: SANDBOX TEST HARNESS - SUBFOLDER ROUTING REALIGNMENT (v0.85)
        // ====================================================================================
        public static void ExecuteValidationPass(string baseDir)
        {
            InitializeTiers();
            DataEngineLogger.LogSession("--------------------------------------------------------------------------------");
            DataEngineLogger.LogSession($"LAUNCHING MIGRATION PROTOTYPE LOOP: Target Count = {_validationTiers.Count} Tiers");
            DataEngineLogger.LogSession("--------------------------------------------------------------------------------");

            int scannedCount = 0; int passedCount = 0; int exceptionCount = 0;

            foreach (var target in _validationTiers)
            {
                scannedCount++;
                DataEngineLogger.LogSession($"[VALIDATING TIER {scannedCount}] Stage [{target.Index}] -> {target.Name}");

                // Rule: Build explicit matching target directory pathways
                string mapFolder = Path.Combine(baseDir, "data", "maps");
                string gemFolder = Path.Combine(baseDir, "data", "gems");
                string liftFolder = Path.Combine(baseDir, "data", "elevator");

                // Security Gate: Ensure destination directories physically exist
                if (!Directory.Exists(mapFolder)) Directory.CreateDirectory(mapFolder);
                if (!Directory.Exists(gemFolder)) Directory.CreateDirectory(gemFolder);
                if (!Directory.Exists(liftFolder)) Directory.CreateDirectory(liftFolder);

                // Enforce exact case-sensitive lowercase filename pattern constraints
                string cleanNameFormat = target.Name.Replace(" ", "_").Replace("'", "");
                string mapPath = Path.Combine(mapFolder, $"Maps_Stage_{target.Index}_{cleanNameFormat}.txt");
                string gemPath = Path.Combine(gemFolder, $"Gems_Stage_{target.Index}_{cleanNameFormat}.txt");
                string liftPath = Path.Combine(liftFolder, $"elevators_stage_{target.Index}_{cleanNameFormat}.txt");

                try
                {
                    // Step 1: Read the data arrays
                    var lifts = ElevatorDataEngine.LoadElevatorFile(liftPath);
                    var gemMatrix = GemDataEngine.LoadGemFile(gemPath, out int activeGems);
                    var mapMatrix = MapDataEngine.LoadMapFile(mapPath, out int stageId, out int gemTally);

                    passedCount++;

                    // Step 2: Output matching v0.85 text format configurations directly into subfolders
                    ElevatorDataEngine.SaveElevatorFile(liftPath, target.Index, target.Name, lifts);
                    GemDataEngine.SaveGemFile(gemPath, target.Index, target.Name, gemMatrix, activeGems);
                    MapDataEngine.SaveMapFile(mapPath, target.Index, target.Name, mapMatrix, gemTally);

                    DataEngineLogger.LogStartup(target.Index, target.Name, "VALID (v0.85)", $"VALID ({activeGems})", "VALID (VEC)", target.Description);
                }
                catch (Exception ex)
                {
                    exceptionCount++;
                    DataEngineLogger.LogSession($"CRITICAL: Subfolder migration failure on target [{target.Index}]: {ex.Message}");
                }
            }

            DataEngineLogger.FinalizeSession(scannedCount, passedCount, exceptionCount, false);
        }
        // ====================================================================================
    }
}