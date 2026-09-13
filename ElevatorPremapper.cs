using System.Collections.Generic;

namespace cSharpRaylib
{
    public static class ElevatorPremapper
    {
        /// <summary>
        /// Applies 100% hand-verified 0-indexed matrix coordinates to the live elevator collection.
        /// Maps parameters directly by Stage ID to support high-density split layouts cleanly.
        /// </summary>
        public static void ApplyOverrides(int stageNum, List<ElevatorData> elevators)
        {
            if (elevators == null || elevators.Count == 0) return;

            // Define target tracking coordinates array for the current active stage context
            List<(int X, int Y)> targetCoords = new List<(int X, int Y)>();

            switch (stageNum)
            {
                case 3:  // Stage 03 - Berthilda's Castle
                case 19: // Stage 19 - Berthilda's Castle Variant
                    targetCoords.Add((4, 18));
                    targetCoords.Add((13, 2));
                    targetCoords.Add((17, 18));
                    targetCoords.Add((11, 10));
                    break;

                case 4:  // Stage 04 - Hidden Ramp
                case 8:  // Stage 08 - Hidden Ramp Variant
                case 14: // Stage 14 - Hidden Ramp Master
                case 30: // Stage 30 - Hidden Ramp Duplicate
                    targetCoords.Add((4, 18));
                    targetCoords.Add((19, 3));
                    targetCoords.Add((16, 16));
                    targetCoords.Add((8, 8));
                    break;

                case 6:  // Stage 06 - Crossroads
                case 18: // Stage 18 - Crossroads Master
                    targetCoords.Add((2, 18));
                    targetCoords.Add((6, 14));
                    targetCoords.Add((10, 10));
                    targetCoords.Add((14, 6));
                    break;

                case 11: // Stage 11 - Berthilda's Dungeon
                case 27: // Stage 27 - Berthilda's Dungeon Variant
                case 35: // Stage 35 - Berthilda's Dungeon Duplicate
                    targetCoords.Add((4, 16));
                    targetCoords.Add((8, 8));
                    targetCoords.Add((10, 3));
                    targetCoords.Add((11, 15));
                    targetCoords.Add((17, 5));
                    break;

                case 12: // Stage 12 - Pyramid
                case 25: // Stage 25 - Pyramid High-Density Variant (Fixed Missing Squares)
                    targetCoords.Add((2, 11));
                    targetCoords.Add((3, 12));
                    targetCoords.Add((11, 15));
                    targetCoords.Add((1, 18));
                    break;

                case 32: // Stage 32 - Impossible Staircase (Fixed Collision Tracker)
                    targetCoords.Add((16, 8));
                    break;

                default:
                    // Fallback pass: leave other un-audited stages to map their default structures cleanly
                    return;
            }

            // Safe assignment pass linking extracted data bounds to live array slots
            int loopLimit = System.Math.Min(elevators.Count, targetCoords.Count);
            for (int i = 0; i < loopLimit; i++)
            {
                elevators[i].CellX = targetCoords[i].X;
                elevators[i].CellY = targetCoords[i].Y;
                elevators[i].IsMapped = true;
            }
        }
    }
}