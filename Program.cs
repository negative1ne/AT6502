using System;
using Raylib_cs;
using System.Collections.Generic;
using System.IO;

namespace cSharpRaylib
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Core Map Translation Array Mapping 37 Waves to 16 Base Cities
            byte[] RoomToCityMap = new byte[] {
         0x00, 0x02, 0x09, 0xC3, 0x46, 0x71, 0x0C, 0xC7,
         0x06, 0x0D, 0x45, 0xCB, 0x04, 0x0A, 0x06, 0x4F,
         0x41, 0x4D, 0x3C, 0xC3, 0x0A, 0x02, 0x32, 0x3F,
         0x01, 0x04, 0x75, 0xFB, 0x01, 0x3A, 0x06, 0xF7,
         0x08, 0x7D, 0x05, 0xCB, 0x0E
         };

            // 2. Load and Combine Arcade Hardware Memory Buffers Safely
            string romDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rom");
            byte[] file1 = File.ReadAllBytes(Path.Combine(romDir, "136022-102.1h"));
            byte[] file2 = File.ReadAllBytes(Path.Combine(romDir, "136022-101.1f"));
            byte[] combinedData = new byte[file1.Length + file2.Length];
            file1.CopyTo(combinedData, 0);
            file2.CopyTo(combinedData, file1.Length);

            // 3. Process Buffers Into Unique Map Layout Blocks
            List<CityData> cities = new List<CityData>();
            for (int i = 0; i < 16; i++)
            {
                CityData city = new CityData();
                city.Load(combinedData, i * 0x400);
                cities.Add(city);
            }

            // 4. Fire up Graphical Mode Sandbox Context
            const int screenWidth = 800;
            const int screenHeight = 600;
            Raylib.InitWindow(screenWidth, screenHeight, "cSharpRaylib - Crystal Castles Data Active");
            Raylib.SetTargetFPS(60);

            // Active tracking targets for our upcoming loop tests
            int currentRoom = 0;

            while (!Raylib.WindowShouldClose())
            {
                // Define color palette dictionary structure for quality-of-life visuals
                var stagePalettes = new System.Collections.Generic.Dictionary<int, Color[]>()
    {
        { 0, new Color[] { Color.White, Color.Gray, Color.DarkGray } },     // 1-1: Ball Wave
        { 1, new Color[] { Color.SkyBlue, Color.Pink, Color.Maroon } },    // 1-2: Tree Wave
        { 2, new Color[] { Color.Blue, Color.DarkBlue, Color.Red } },      // 1-3: Doomsdome
        { 3, new Color[] { Color.Violet, Color.Purple, Color.DarkPurple } }// 1-4: Berthilda's Castle
    };

                // The Interactive Video Loop
                while (!Raylib.WindowShouldClose())
                {
                    // 1. INPUT HANDLING: Step rooms forward or backward with arrow keys safely
                    if (Raylib.IsKeyPressed(KeyboardKey.Right))
                    {
                        currentRoom++;
                        if (currentRoom > 36) currentRoom = 0; // Wrap back to stage 1-1
                    }
                    if (Raylib.IsKeyPressed(KeyboardKey.Left))
                    {
                        currentRoom--;
                        if (currentRoom < 0) currentRoom = 36; // Wrap around to the final stage
                    }

                    // 2. GRAPHICS DRAWING ENVIRONMENT
                    Raylib.BeginDrawing();
                    Raylib.ClearBackground(Color.Black); // Dark cinema backdrop matching VGMaps templates

                    // Pull active theme profile, fallback to Gray if index is not explicitly filled yet
                    Color[] activeTheme = stagePalettes.ContainsKey(currentRoom) ? stagePalettes[currentRoom] : stagePalettes[0];

                    // Draw HUD diagnostics strings securely
                    Raylib.DrawText("ccSharpRaylib — Dynamic Stage Inspector Engine", 20, 20, 20, Color.RayWhite);
                    Raylib.DrawText($"Current Focus: Stage ID [{currentRoom:D2}] (Use Left/Right Arrows to Flip)", 20, 55, 18, Color.Gold);

                    // Visual indicator of active loaded colors
                    Raylib.DrawRectangle(20, 95, 40, 20, activeTheme[0]);
                    Raylib.DrawRectangle(70, 95, 40, 20, activeTheme[1]);
                    Raylib.DrawRectangle(120, 95, 40, 20, activeTheme[2]);
                    Raylib.DrawText("Active Layout Palette Matrix Slots", 180, 98, 14, Color.LightGray);

                    Raylib.EndDrawing();
                }
            }

            Raylib.CloseWindow();
        }
    }

    public class CityData
    {
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
            offset += 5; // Skip animation mode metrics
            TopPosition = data[offset++];
            BottomPosition = data[offset++];
            HorizontalPosition = data[offset++];
            VerticalPosition = data[offset++];
            WaitTime = data[offset++];
            offset++; // Skip structural structural padding byte
            return offset;
        }
    }


}