using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Color = Raylib_cs.Color;

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

            // Initialize standard room navigation trackers cleanly in scope
            int currentRoom = 0;

            // Fixed color palette profiles using explicit RGB values for strict Raylib compatibility
    var stagePalettes = new System.Collections.Generic.Dictionary<int, Color[]>()
    {
        // WORLD 1
        { 0, new Color[] { Color.White, Color.Gray, Color.DarkGray } },
        { 1, new Color[] { Color.SkyBlue, Color.Pink, Color.Maroon } },
        { 2, new Color[] { Color.RayWhite, Color.Blue, Color.DarkBlue } },
        { 3, new Color[] { Color.Violet, Color.Purple, Color.DarkPurple } },         

        // WORLD 2
        { 4, new Color[] { Color.LightGray, new Color(112, 128, 144, 255), Color.DarkBlue } }, // SlateGray
        { 5, new Color[] { Color.Magenta, Color.Purple, Color.Black } },
        { 6, new Color[] { Color.Beige, Color.Brown, Color.DarkBrown } },
        { 7, new Color[] { Color.Yellow, Color.Orange, Color.Red } },                

        // WORLD 3
        { 8, new Color[] { Color.LightGray, new Color(112, 128, 144, 255), Color.DarkBlue } },
        { 9, new Color[] { Color.Lime, new Color(34, 139, 34, 255), Color.DarkGreen } },       // ForestGreen
        { 10, new Color[] { Color.White, Color.SkyBlue, Color.Blue } },
        { 11, new Color[] { Color.Purple, Color.DarkPurple, Color.Magenta } },       

        // WORLD 4
        { 12, new Color[] { Color.Gold, Color.Orange, Color.DarkBrown } },
        { 13, new Color[] { Color.SkyBlue, Color.Blue, Color.DarkBlue } },
        { 14, new Color[] { Color.LightGray, new Color(112, 128, 144, 255), Color.DarkBlue } },
        { 15, new Color[] { Color.DarkGray, Color.Maroon, Color.Black } },           

        // WORLD 5
        { 16, new Color[] { Color.Magenta, Color.Purple, Color.Black } },
        { 17, new Color[] { Color.Lime, new Color(34, 139, 34, 255), Color.DarkGreen } },
        { 18, new Color[] { Color.Beige, Color.Brown, Color.DarkBrown } },
        { 19, new Color[] { Color.Violet, Color.Purple, Color.DarkPurple } },        

        // WORLD 6
        { 20, new Color[] { Color.SkyBlue, Color.Blue, Color.DarkBlue } },
        { 21, new Color[] { Color.SkyBlue, Color.Pink, Color.Maroon } },
        { 22, new Color[] { Color.SkyBlue, Color.Pink, Color.Maroon } },
        { 23, new Color[] { Color.DarkGray, Color.Maroon, Color.Black } },           

        // WORLD 7
        { 24, new Color[] { Color.Magenta, Color.Purple, Color.Black } },
        { 25, new Color[] { Color.Gold, Color.Orange, Color.DarkBrown } },
        { 26, new Color[] { Color.White, Color.SkyBlue, Color.Blue } },
        { 27, new Color[] { Color.Purple, Color.DarkPurple, Color.Magenta } },       

        // WORLD 8
        { 28, new Color[] { Color.Magenta, Color.Purple, Color.Black } },
        { 29, new Color[] { Color.SkyBlue, Color.Blue, Color.DarkBlue } },
        { 30, new Color[] { Color.LightGray, new Color(112, 128, 144, 255), Color.DarkBlue } },
        { 31, new Color[] { Color.Yellow, Color.Orange, Color.Red } },               

        // WORLD 9
        { 32, new Color[] { Color.Red, new Color(139, 0, 0, 255), Color.Black } },             // DarkRed
        { 33, new Color[] { Color.Lime, new Color(34, 139, 34, 255), Color.DarkGreen } },
        { 34, new Color[] { Color.White, Color.SkyBlue, Color.Blue } },
        { 35, new Color[] { Color.Purple, Color.DarkPurple, Color.Magenta } },       

        // FINAL STAGE
        { 36, new Color[] { Color.RayWhite, Color.Gold, new Color(204, 204, 0, 255) } }         // DarkYellow
    };

            // The Interactive Video Loop
            while (!Raylib.WindowShouldClose())
            {
                // 1. INPUT HANDLING: Step rooms forward or backward safely
                if (Raylib.IsKeyPressed(KeyboardKey.Right))
                {
                    currentRoom++;
                    if (currentRoom > 36) currentRoom = 0;
                }
                if (Raylib.IsKeyPressed(KeyboardKey.Left))
                {
                    currentRoom--;
                    if (currentRoom < 0) currentRoom = 36;
                }

                // 2. GRAPHICS DRAWING ENVIRONMENT
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                // Fetch active theme color vectors safely using standard fallback
                Color[] activeTheme = stagePalettes.ContainsKey(currentRoom) ? stagePalettes[currentRoom] : stagePalettes[0];

                // Draw HUD diagnostics engine metrics
                Raylib.DrawText("ccSharpRaylib — Dynamic Stage Inspector Engine", 20, 20, 20, Color.RayWhite);
                Raylib.DrawText($"Current Focus: Stage ID [{currentRoom:D2}] (Use Left/Right Arrows to Flip)", 20, 55, 18, Color.Gold);

                // 3. THE 2D TOP-DOWN BLUEPRINT RENDERING ENGINE
                int rawRoomByte = RoomToCityMap[currentRoom];
                int cityIndex = rawRoomByte & 0x0F;
                CityData activeCity = cities[cityIndex];

                // Visual layout grid matrix alignment coordinates
                int cellSize = 16;
                int gridOffsetX = 220;
                int gridOffsetY = 150;

                for (int x = 0; x < 22; x++)
                {
                    for (int y = 0; y < 22; y++)
                    {
                        int tileHeight = activeCity.Heights[x, y];
                        if (tileHeight == 0) continue;

                        int posX = gridOffsetX + (y * cellSize);
                        int posY = gridOffsetY + (x * cellSize);

                        // Quality of life: Shade blocks lighter based on elevation height
                        byte baseShade = (byte)Math.Min(100 + (tileHeight * 2), 255);
                        Color blockColor = new Color(
                            (byte)(activeTheme[0].R * baseShade / 255),
                            (byte)(activeTheme[0].G * baseShade / 255),
                            (byte)(activeTheme[0].B * baseShade / 255),
                            (byte)255
                        );

                        Raylib.DrawRectangle(posX, posY, cellSize - 1, cellSize - 1, blockColor);
                    }
                }

                // Draw active palette indicators
                Raylib.DrawRectangle(20, 95, 40, 20, activeTheme[0]);
                Raylib.DrawRectangle(70, 95, 40, 20, activeTheme[1]);
                Raylib.DrawRectangle(120, 95, 40, 20, activeTheme[2]);
                Raylib.DrawText("Active Layout Palette Matrix Slots", 180, 98, 14, Color.LightGray);

                Raylib.EndDrawing();
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