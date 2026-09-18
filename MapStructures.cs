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

        // ============================================================================
        // MAPSTRUCTURES.CS / ELEVATORDATA - CLEAN FRAME-DRIVEN PHYSICS ENGINE (v0.80)
        // ============================================================================
        public void Update()
        {
            // Safety check: Skip movement checks completely if the lift isn't bound to your text grid
            if (!this.IsMapped) return;

            // TARGET MOTION STATE MACHINE RULES:
            // Mode 0: Platform traveling UP toward TopPosition
            // Mode 1: Platform sitting/waiting at TopPosition
            // Mode 2: Platform traveling DOWN toward BottomPosition
            // Mode 3: Platform sitting/waiting at BottomPosition

            switch (this.Mode)
            {
                case 0: // Moving UP
                    this.CurrentPosition += 1; // Smooth linear upward translation step per frame
                    if (this.CurrentPosition >= this.TopPosition)
                    {
                        this.CurrentPosition = this.TopPosition;
                        this.Mode = 1; // Transition to waiting at top deck
                        this.CurrentSitTime = 0;
                    }
                    break;

                case 1: // Waiting at Top
                    this.CurrentSitTime++;
                    if (this.CurrentSitTime >= this.WaitTime)
                    {
                        this.Mode = 2; // Begin moving back down
                    }
                    break;

                case 2: // Moving DOWN
                    this.CurrentPosition -= 1; // Smooth linear downward translation step per frame
                    if (this.CurrentPosition <= this.BottomPosition)
                    {
                        this.CurrentPosition = this.BottomPosition;
                        this.Mode = 3; // Transition to waiting at bottom ground deck
                        this.CurrentSitTime = 0;
                    }
                    break;

                case 3: // Waiting at Bottom
                    this.CurrentSitTime++;
                    if (this.CurrentSitTime >= this.WaitTime)
                    {
                        this.Mode = 0; // Recycle loop: begin moving up again
                    }
                    break;
            }
        }
    }
    
  
}