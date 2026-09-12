using System.Collections.Generic;

namespace cSharpRaylib
{
    public static class ElevatorPremapper
    {
        // IMMUTABLE GROUND-TRUTH MATRIX (Only tracks the 11 asymmetrical drift layouts)
        private static readonly Dictionary<int, (int X, int Y)> ProblemStagesMap = new Dictionary<int, (int, int)>()
        {
            // Stage 03: Berthilda's Castle (1-4)
            { 30, (11, 10) }, { 31, (10, 9) },  { 32, (12, 1) },  { 33, (16, 18) },

            // Stage 04: Hidden Ramp (2-1)
            { 40, (4, 18) },  { 41, (7, 7) },   { 42, (16, 16) }, { 43, (19, 3) },

            // Stage 06: Crossroads (2-3)
            { 60, (2, 18) },  { 61, (6, 14) },  { 62, (10, 10) }, { 63, (14, 6) },

            // Stage 11: Berthilda's Dungeon (3-4)
            { 110, (4, 16) }, { 111, (8, 7) },  { 112, (10, 3) }, { 113, (11, 15) }, { 114, (8, 8) },

            // Stage 12: Pyramid (4-1)
            { 120, (0, 18) }, { 121, (2, 11) }, { 122, (3, 12) }, { 123, (11, 15) }, { 124, (17, 5) },

            // Stage 18: Crossroads Twin (5-3)
            { 180, (2, 18) }, { 181, (6, 14) }, { 182, (10, 10) }, { 183, (14, 6) },

            // Stage 19: Berthilda's Castle Twin (5-4)
            { 190, (11, 10) }, { 191, (10, 9) }, { 192, (12, 1) }, { 193, (16, 18) },

            // Stage 25: Pyramid Twin (7-2)
            { 250, (0, 18) }, { 251, (2, 11) }, { 252, (3, 12) }, { 253, (11, 15) }, { 254, (17, 5) },

            // Stage 27: Berthilda's Dungeon Twin (7-4)
            { 270, (4, 16) }, { 271, (8, 7) },  { 272, (10, 3) }, { 273, (11, 15) }, { 274, (8, 8) },

            // Stage 32: Impossible Staircase (9-1)
            { 320, (16, 8) },

            // Stage 35: Berthilda's Dungeon Triplet (9-4)
            { 350, (4, 16) }, { 351, (8, 7) },  { 352, (10, 3) }, { 353, (11, 15) }, { 354, (8, 8) }
        };

        public static void ApplyOverrides(int stageNum, List<ElevatorData> elevators)
        {
            for (int i = 0; i < elevators.Count; i++)
            {
                int lookupKey = (stageNum * 10) + i;

                // If this is one of our 11 problem stages, use our hardcoded ground-truth positions
                if (ProblemStagesMap.ContainsKey(lookupKey))
                {
                    var coords = ProblemStagesMap[lookupKey];
                    elevators[i].CellX = coords.X;
                    elevators[i].CellY = coords.Y;
                    elevators[i].IsMapped = true; // Securely map our problem stage blocks
                }
                // SPLIT STRATEGY: If it's a working level, do absolutely nothing! 
                // This keeps it running on its native, verified ROM configurations.
            }
        }
    }
}