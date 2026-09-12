using System.Collections.Generic;

namespace cSharpRaylib
{
    public class CityData
    {
        // FIXED: Explicitly allocate the 22x22 dimensional grid arrays in system memory
        public byte[,] Heights = new byte[22, 22];
        public byte[,] Attributes = new byte[22, 22];
        public int NumElevators;
        public List<ElevatorData> Elevators = new List<ElevatorData>();

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
        public int TopPosition, BottomPosition;
        public int HorizontalPosition, VerticalPosition;
        public int WaitTime;

        public int Load(byte[] data, int offset)
        {
            offset += 5;
            TopPosition = data[offset++];
            BottomPosition = data[offset++];
            HorizontalPosition = data[offset++];
            VerticalPosition = data[offset++];
            WaitTime = data[offset++];
            offset++;
            return offset;
        }
    }
}