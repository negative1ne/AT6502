using cSharpRaylib;
using System.Collections.Generic;

namespace cViewerTest
{
    public static class ElevatorPremapper
    {
        // 100% VALIDATED MASTER DATA MATRIX FROM THE 2D BLUEPRINT SHEETS
        private static readonly Dictionary<int, (int X, int Y)> StaticMap = new Dictionary<int, (int, int)>()
        {
            { 00, (18, 5) },  { 01, (5, 18) },                                    // Stage 00
            { 20, (17, 17) },                                                     // Stage 02
            { 30, (19, 4) },  { 31, (4, 19) },  { 32, (13, 2) },  { 33, (11, 11) }, // Stage 03 - FIXED INDICES SWAP
            { 40, (19, 3) },  { 41, (4, 18) },  { 42, (16, 15) }, { 43, (8, 17) },  // Stage 04 - FIXED TRANSPOSED SHIFTS
            { 50, (5, 19) },  { 51, (19, 5) },  { 52, (6, 10) },  { 53, (10, 6) },  { 54, (6, 6) }, // Stage 05
            { 60, (2, 12) },  { 61, (6, 8) },   { 62, (10, 10) }, { 63, (14, 6) },  // Stage 06 - FIXED OFF-BY-ONE SHIFTS
            { 70, (16, 16) },                                                     // Stage 07
            { 80, (16, 16) }, { 81, (8, 8) },   { 82, (19, 4) },  { 83, (4, 19) },  // Stage 08
            { 90, (7, 10) },  { 91, (10, 7) },  { 92, (9, 3) },   { 93, (3, 9) },   { 94, (5, 5) }, // Stage 09
            { 110, (2, 15) }, { 111, (14, 5) }, { 112, (16, 6) }, { 113, (6, 17) }, { 114, (8, 8) }, // Stage 11 - Verified
            { 120, (0, 18) }, { 121, (2, 11) }, { 122, (5, 14) }, { 123, (11, 15) }, { 124, (17, 5) },// Stage 12 - FIXED PYRAMID SHIFT
            { 130, (4, 4) },                                                      // Stage 13
            { 140, (16, 16) }, { 141, (8, 8) },  { 142, (19, 4) }, { 143, (4, 19) }, // Stage 14
            { 160, (5, 19) }, { 161, (19, 5) }, { 162, (6, 10) }, { 163, (10, 6) }, { 164, (6, 6) }, // Stage 16
            { 170, (7, 10) }, { 171, (10, 7) }, { 172, (9, 3) },  { 173, (3, 9) },  { 174, (5, 5) }, // Stage 17
            { 180, (2, 12) }, { 181, (6, 8) },  { 182, (10, 10) }, { 183, (14, 6) }, // Stage 18 - TWIN REPAIR
            { 190, (19, 4) }, { 191, (4, 19) }, { 192, (13, 2) }, { 193, (11, 11) }, // Stage 19 - TWIN REPAIR
            { 200, (4, 4) },                                                      // Stage 20
            { 240, (5, 19) }, { 241, (19, 5) }, { 242, (6, 10) }, { 243, (10, 6) }, { 244, (6, 6) }, // Stage 24
            { 250, (0, 18) }, { 251, (2, 11) }, { 252, (5, 14) }, { 253, (11, 15) }, { 254, (17, 5) },// Stage 25 - TWIN REPAIR
            { 270, (2, 15) }, { 271, (14, 6) }, { 272, (16, 6) }, { 273, (6, 17) }, { 274, (8, 8) }, // Stage 27 - PERFECTLY CORRECTED
            { 280, (5, 19) }, { 281, (19, 5) }, { 282, (6, 10) }, { 283, (10, 6) }, { 284, (6, 6) }, // Stage 28
            { 290, (4, 4) },                                                      // Stage 29
            { 300, (16, 16) }, { 301, (8, 8) },  { 302, (19, 4) }, { 303, (4, 19) }, // Stage 30
            { 310, (16, 16) },                                                    // Stage 31
            { 320, (12, 6) },                                                     // Stage 32 - STAIRCASE PLATFORM FIXED
            { 330, (7, 10) }, { 331, (10, 7) }, { 332, (9, 3) },  { 333, (3, 9) },  { 334, (5, 5) }, // Stage 33
            { 350, (2, 15) }, { 351, (14, 5) }, { 352, (16, 6) }, { 353, (6, 17) }, { 354, (8, 8) }, // Stage 35 - Verified
        };

        public static void ApplyOverrides(int stageNum, List<ElevatorData> elevators)
        {
            for (int i = 0; i < elevators.Count; i++)
            {
                int lookupKey = (stageNum * 10) + i;
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