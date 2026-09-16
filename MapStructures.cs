// ============================================================================
// MAPSTRUCTURES.CS - INJECTING TRI-STATE FRAMEWORK SWITCHES
// ============================================================================
using System;
using System.Collections.Generic;

namespace cSharpRaylib
{
    // FIX CS0103: Inject our defensive groups straight into the namespace!
    public enum StageTrackingState
    {
        NoElevators,       // 11 Levels completely devoid of lifts
        VerifiedWorking,   // 7 Levels structurally frozen under legacy rules
        ExperimentalTarget // 19 Levels targeted for height normalization calibrations
    }

    public class CityData
    {
        // FIXED: Explicitly allocate the 22x22 dimensional grid arrays in system memory
        public byte[,] Heights = new byte[22, 22];
        public byte[,] Attributes = new byte[22, 22];
        public int NumElevators;
        public List<ElevatorData> Elevators = new List<ElevatorData>();

        // FIX CS1061: Add the property field so RomManager can tag rooms on startup!
        public StageTrackingState TrackState { get; set; } = StageTrackingState.NoElevators;

        public void Load(byte[] data, int offset)
        {
            for (int x = 0; x < 22; x++)
                for (int y = 0; y < 22; y++)
                    Heights[x, y] = data[offset++];

            for (int x = 0; x < 22; x++)
                for (int y = 0; y < 22; y++)
                    Attributes[x, y] = data[offset++];

            NumElevators = data[offset++];

            for (int i = 0; i < NumElevators; i++)
            {
                ElevatorData ev = new ElevatorData();
                offset = ev.Load(data, offset);
                Elevators.Add(ev);
            }
        }
    }

    public class ElevatorData
    {
        public int HorizontalPosition, VerticalPosition;
        public int TopPosition, BottomPosition;
        public int WaitTime;

        // --- ADD THESE NEW SANDBOX FIELDS TO FIX THE COMPILER ERRORS ---
        public int CellX { get; set; }
        public int CellY { get; set; }
        public bool IsMapped { get; set; } = false;

        // --- ADD THESE NEW DYNAMIC ANIMATION TRACKERS FOR ENGINE INTEGRATION ---
        public int Mode { get; set; } = 0;           // 0=Idle Bottom, 1=Up, 2=Idle Top, 3=Down
        public int CurrentPosition { get; set; }     // Animated height tracking register value
        public int CurrentSitTime { get; set; }      // Wait state timer tracker

        public int Load(byte[] data, int offset)
        {
            offset += 5;
            TopPosition = data[offset++];
            BottomPosition = data[offset++];
            HorizontalPosition = data[offset++];
            VerticalPosition = data[offset++];
            WaitTime = data[offset++];
            offset++;

            // Initialize your starting parameters cleanly
            CellX = 0;
            CellY = 0;
            IsMapped = false;
            CurrentPosition = BottomPosition;
            Mode = 0;
            CurrentSitTime = 0;

            return offset;
        }

        // The unified state machine mechanics engine calculation block we verified
        public void Update()
        {
            const int SitTimeInc = 3;

            switch (Mode)
            {
                case 0: // Stationary at bottom boundary
                    CurrentSitTime += SitTimeInc;
                    if (CurrentSitTime > WaitTime)
                    {
                        CurrentSitTime = 0;
                        Mode = 1; // Change tracking state to Up
                    }
                    break;

                case 1: // Moving up column path
                    CurrentPosition++;
                    if (CurrentPosition >= TopPosition)
                    {
                        CurrentPosition = TopPosition;
                        Mode = 2; // Arrived at top ridge limit
                    }
                    break;

                case 2: // Stationary at top boundary
                    CurrentSitTime += SitTimeInc;
                    if (CurrentSitTime > WaitTime)
                    {
                        CurrentSitTime = 0;
                        Mode = 3; // Change tracking state to Down
                    }
                    break;

                case 3: // Moving down column path
                    CurrentPosition--;
                    if (CurrentPosition <= BottomPosition)
                    {
                        CurrentPosition = BottomPosition;
                        Mode = 0; // Arrived back at bottom
                    }
                    break;
            }
        }
    }
  
}