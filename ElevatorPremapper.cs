using System.Collections.Generic;

namespace cSharpRaylib
{
    public static class ElevatorPremapper
    {
        // 100% PROVEN RE-ALIGNED ARCADE DATA BLUEPRINT MATRIX (Mapped strictly to the 16 base cities)
        private static readonly Dictionary<int, (int X, int Y)> StaticMap = new Dictionary<int, (int, int)>()
        {
            // CITY 00: Ball Wave Baseline
            { 00, (18, 5) },  { 01, (5, 18) },

            // CITY 02: Doomsdome Baseline
            { 20, (17, 17) },

            // CITY 03: Berthilda's Castle (Asymmetrical Layout Fix)
            { 30, (11, 10) }, { 31, (10, 9) },  { 32, (12, 1) },  { 33, (16, 18) },

            // CITY 04: Pyramid / Hidden Ramp (Asymmetrical Layout Fix)
            { 40, (4, 18) },  { 41, (7, 7) },   { 42, (16, 16) }, { 43, (19, 3) },

            // CITY 05: Staircase Baseline
            { 50, (5, 19) },  { 51, (19, 5) },  { 52, (6, 10) },  { 53, (10, 6) },  { 54, (6, 6) },

            // CITY 06: Crossroads / Dungeon (Asymmetrical Layout Fix)
            { 60, (2, 18) },  { 61, (6, 14) },  { 62, (10, 10) }, { 63, (14, 6) },

            // CITY 07: Berthilda's Fortress Baseline
            { 70, (16, 16) },

            // CITY 08: Symmetrical Hidden Ramp Template
            { 80, (16, 16) }, { 81, (8, 8) },   { 82, (19, 4) },  { 83, (4, 19) },

            // CITY 09: Nasty Tree Baseline
            { 90, (7, 10) },  { 91, (10, 7) },  { 92, (9, 3) },   { 93, (3, 9) },   { 94, (5, 5) },

            // CITY 11: Unique Dungeon Tower Array Variations
            { 110, (4, 16) }, { 111, (8, 7) },  { 112, (10, 3) }, { 113, (11, 15) }, { 114, (8, 8) },

            // CITY 12: Unique Pyramid Tier Terrace Variations
            { 120, (0, 18) }, { 121, (2, 11) }, { 122, (3, 12) }, { 123, (11, 15) }, { 124, (17, 5) },

            // CITY 13: Center Cross Maze Intersection
            { 130, (4, 4) },

            // CITY 14: Symmetrical Base Runway Template
            { 140, (16, 16) }, { 141, (8, 8) },  { 142, (19, 4) }, { 143, (4, 19) },

            // CITY 16: Dynamic Staircase Variant Slabs
            { 160, (5, 19) }, { 161, (19, 5) }, { 162, (6, 10) }, { 163, (10, 6) }, { 164, (6, 6) },

            // CITY 17: Upper Tree Canopy Variant Slabs
            { 170, (7, 10) }, { 171, (10, 7) }, { 172, (9, 3) },  { 173, (3, 9) },  { 174, (5, 5) },

            // Dynamic tracking index placeholder for Stage 32 unique staircase slot channel
            { 320, (16, 8) }
        };

        // Upgraded signature to resolve indices based on city block ownership bounds
        public static void ApplyOverrides(int cityID, List<ElevatorData> elevators)
        {
            for (int i = 0; i < elevators.Count; i++)
            {
                // Multiply our stable 0-15 city ID to secure the mapping key fields cleanly
                int lookupKey = (cityID * 10) + i;
                if (StaticMap.ContainsKey(lookupKey))
                {
                    var coords = StaticMap[lookupKey];
                    elevators[i].CellX = coords.X;
                    elevators[i].CellY = coords.Y;
                    elevators[i].IsMapped = true;
                }
            }
        }
    }
}