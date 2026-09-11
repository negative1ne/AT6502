using System;
using System.IO;
using System.Collections.Generic;
using Raylib_cs;
using Color = Raylib_cs.Color;
using Rectangle = Raylib_cs.Rectangle;

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

            // 4. Fire up Graphical Mode Sandbox Context with Fixed 3x Hardware Upscaling
            const int virtualWidth = 800;
            const int virtualHeight = 600;
            const int scaleMultiplier = 2;

            const int screenWidth = virtualWidth * scaleMultiplier;   // 2400 pixels wide
            const int screenHeight = virtualHeight * scaleMultiplier; // 1800 pixels high

            Raylib.InitWindow(screenWidth, screenHeight, "cSharpRaylib - Crystal Castles High-Res View");
            Raylib.SetTargetFPS(60);

            // Create the off-screen frame buffer for integer pixel perfect stretching
            RenderTexture2D targetBuffer = Raylib.LoadRenderTexture(virtualWidth, virtualHeight);

            // Disable texture filtering smoothing vectors to retain crisp arcade pixel edges
            Raylib.SetTextureFilter(targetBuffer.Texture, TextureFilter.Point);

            // 5. Initialize tracking states and the complete master color dictionary
            int currentRoom = 0;
            bool is3DMode = false;

            // 3D Viewport Calibration Modifiers
            float globalScale = 1.0f;
            float heightMultiplier = 1.8f;
            int panOffsetX = 0;
            int panOffsetY = 0;

            // NEW EXTENDED CONTROLS STATE DRIVERS
            int rotationAngle = 0;          // Tracks degrees of rotation (0-360)
            float tiltFactor = 1.0f;        // Tracks forward/backward projection tilt ratio
            int renderStyleMode = 0;        // 0 = Filled, 1 = Cel Shaded, 2 = Wireframe
            bool displayPathOverlays = false; // Toggles tunnel/path coloration highlights
            bool triggerScreenshotFlag = false; // Tracks screenshot demand requests

            

        var stagePalettes = new System.Collections.Generic.Dictionary<int, Color[]>()
            {
        { 0, new Color[] { Color.White, Color.Gray, Color.DarkGray } },
        { 1, new Color[] { Color.SkyBlue, Color.Pink, Color.Maroon } },
        { 2, new Color[] { Color.RayWhite, Color.Blue, Color.DarkBlue } },
        { 3, new Color[] { Color.Violet, Color.Purple, Color.DarkPurple } },
        { 4, new Color[] { Color.LightGray, new Color(112, 128, 144, 255), Color.DarkBlue } },
        { 5, new Color[] { Color.Magenta, Color.Purple, Color.Black } },
        { 6, new Color[] { Color.Beige, Color.Brown, Color.DarkBrown } },
        { 7, new Color[] { Color.Yellow, Color.Orange, Color.Red } },
        { 8, new Color[] { Color.LightGray, new Color(112, 128, 144, 255), Color.DarkBlue } },
        { 9, new Color[] { Color.Lime, new Color(34, 139, 34, 255), Color.DarkGreen } },
        { 10, new Color[] { Color.White, Color.SkyBlue, Color.Blue } },
        { 11, new Color[] { Color.Purple, Color.DarkPurple, Color.Magenta } },
        { 12, new Color[] { Color.Gold, Color.Orange, Color.DarkBrown } },
        { 13, new Color[] { Color.SkyBlue, Color.Blue, Color.DarkBlue } },
        { 14, new Color[] { Color.LightGray, new Color(112, 128, 144, 255), Color.DarkBlue } },
        { 15, new Color[] { Color.DarkGray, Color.Maroon, Color.Black } },
        { 16, new Color[] { Color.Magenta, Color.Purple, Color.Black } },
        { 17, new Color[] { Color.Lime, new Color(34, 139, 34, 255), Color.DarkGreen } },
        { 18, new Color[] { Color.Beige, Color.Brown, Color.DarkBrown } },
        { 19, new Color[] { Color.Violet, Color.Purple, Color.DarkPurple } },
        { 20, new Color[] { Color.SkyBlue, Color.Blue, Color.DarkBlue } },
        { 21, new Color[] { Color.SkyBlue, Color.Pink, Color.Maroon } },
        { 22, new Color[] { Color.SkyBlue, Color.Pink, Color.Maroon } },
        { 23, new Color[] { Color.DarkGray, Color.Maroon, Color.Black } },
        { 24, new Color[] { Color.Magenta, Color.Purple, Color.Black } },
        { 25, new Color[] { Color.Gold, Color.Orange, Color.DarkBrown } },
        { 26, new Color[] { Color.White, Color.SkyBlue, Color.Blue } },
        { 27, new Color[] { Color.Purple, Color.DarkPurple, Color.Magenta } },
        { 28, new Color[] { Color.Magenta, Color.Purple, Color.Black } },
        { 29, new Color[] { Color.SkyBlue, Color.Blue, Color.DarkBlue } },
        { 30, new Color[] { Color.LightGray, new Color(112, 128, 144, 255), Color.DarkBlue } },
        { 31, new Color[] { Color.Yellow, Color.Orange, Color.Red } },
        { 32, new Color[] { Color.Red, new Color(139, 0, 0, 255), Color.Black } },
        { 33, new Color[] { Color.Lime, new Color(34, 139, 34, 255), Color.DarkGreen } },
        { 34, new Color[] { Color.White, Color.SkyBlue, Color.Blue } },
        { 35, new Color[] { Color.Purple, Color.DarkPurple, Color.Magenta } },
        { 36, new Color[] { Color.RayWhite, Color.Gold, new Color(204, 204, 0, 255) } }
            };

            // The Interactive Video Loop starts immediately below this
            while (!Raylib.WindowShouldClose())
            {

                // The Interactive Video Loop
                while (!Raylib.WindowShouldClose())
                {
                    // 1. INPUT HANDLING: Room switching indices
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

                    // INPUT HANDLING: Viewpoint configuration switches
                    if (Raylib.IsKeyPressed(KeyboardKey.Up)) is3DMode = true;  // Toggle 3D isometric view
                    if (Raylib.IsKeyPressed(KeyboardKey.Down)) is3DMode = false; // Toggle 2D blueprint view

                    // 1.2 VIEWPORT CALIBRATION INPUTS (Runs only when 3D mode view is active)
                    if (is3DMode)
                    {
                        // Modify overall layout sizing scales using Numpad plus/minus keys
                        if (Raylib.IsKeyDown(KeyboardKey.KpAdd)) globalScale += 0.02f;
                        if (Raylib.IsKeyDown(KeyboardKey.KpSubtract)) globalScale -= 0.02f;

                        // Modify vertical column stepping thickness using W/S keys to compress or stretch heights
                        if (Raylib.IsKeyDown(KeyboardKey.W)) heightMultiplier += 0.05f;
                        if (Raylib.IsKeyDown(KeyboardKey.S)) heightMultiplier -= 0.05f;

                        // Pan map position inside screen space using standard I/K/J/L keys
                        if (Raylib.IsKeyDown(KeyboardKey.I)) panOffsetY -= 4; // Pan Up
                        if (Raylib.IsKeyDown(KeyboardKey.K)) panOffsetY += 4; // Pan Down
                        if (Raylib.IsKeyDown(KeyboardKey.J)) panOffsetX -= 4; // Pan Left
                        if (Raylib.IsKeyDown(KeyboardKey.L)) panOffsetX += 4; // Pan Right

                        // --- ADD THE NEW ROTATION, TILT, AND STYLE CONTROLS BELOW ---

                        // A / D Keys: Rotate the map grid smoothly
                        if (Raylib.IsKeyDown(KeyboardKey.A)) rotationAngle = (rotationAngle - 2 + 360) % 360;
                        if (Raylib.IsKeyDown(KeyboardKey.D)) rotationAngle = (rotationAngle + 2) % 360;

                        // Q / E Keys: Change the 3D projection tilt factor profile dynamically
                        if (Raylib.IsKeyDown(KeyboardKey.Q)) tiltFactor = Math.Max(0.4f, tiltFactor - 0.02f);
                        if (Raylib.IsKeyDown(KeyboardKey.E)) tiltFactor = Math.Min(2.0f, tiltFactor + 0.02f);

                        // M Key: Cycle through Render Styles (0 = Filled, 1 = Cel Shaded, 2 = Wireframe)
                        if (Raylib.IsKeyPressed(KeyboardKey.M))
                        {
                            renderStyleMode = (renderStyleMode + 1) % 3;
                        }

                        // P Key: Toggle path/tunnel visual color highlights
                        if (Raylib.IsKeyPressed(KeyboardKey.P))
                        {
                            displayPathOverlays = !displayPathOverlays;
                        }

                        // R Key: Instantly reset view metrics back to defaults
                        if (Raylib.IsKeyPressed(KeyboardKey.R))
                        {
                            globalScale = 1.0f;
                            heightMultiplier = 1.8f;
                            panOffsetX = 0;
                            panOffsetY = 0;
                            rotationAngle = 0;
                            tiltFactor = 1.0f;
                            renderStyleMode = 0;
                            displayPathOverlays = false;
                        }
                    }

                    // 2. GRAPHICS DRAWING ENVIRONMENT: Step A (Draw natively to our small virtual texture)
                    Raylib.BeginTextureMode(targetBuffer);
                    Raylib.ClearBackground(Color.Black);

                    // Fetch active theme color vectors safely using standard fallback
                    Color[] activeTheme = stagePalettes.ContainsKey(currentRoom) ? stagePalettes[currentRoom] : stagePalettes[0];

                    // Render HUD text information labels
                    Raylib.DrawText("ccSharpRaylib — Dynamic Stage Inspector Engine", 20, 20, 20, Color.RayWhite);
                    Raylib.DrawText($"Current Focus: Stage ID [{currentRoom:D2}] | View Mode: {(is3DMode ? "3D Isometric (Up)" : "2D Blueprint (Down)")}", 20, 55, 18, Color.Gold);

                    int rawRoomByte = RoomToCityMap[currentRoom];
                    int cityIndex = rawRoomByte & 0x0F;
                    CityData activeCity = cities[cityIndex];

                    // Route map coordinates out based on selected viewport mode profile
                    if (!is3DMode)
                    {
                        // === RENDER FLAT 2D BLUEPRINT ===
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
                                int baseShade = Math.Min(100 + (tileHeight * 2), 255);

                                // Explicitly pull from the top walk color element (Index 0) and use byte casting
                                Color blockColor = new Color(
                                    (byte)(activeTheme[0].R * baseShade / 255),
                                    (byte)(activeTheme[0].G * baseShade / 255),
                                    (byte)(activeTheme[0].B * baseShade / 255),
                                    (byte)255
                                );

                                Raylib.DrawRectangle(posX, posY, cellSize - 1, cellSize - 1, blockColor);
                            }
                        }
                    }
                    else
                    {
                        // === RENDER 3D ISOMETRIC BLOCKS ===
                        // Double nested loops step from back to front to handle depth sorting correctly
                        for (int x = 0; x < 22; x++)
                        {
                            for (int y = 0; y < 22; y++)
                            {
                                int tileHeight = activeCity.Heights[x, y];
                                if (tileHeight == 0) continue;

                                // Fetch the layout attribute byte for path highlights
                                byte cellAttr = activeCity.Attributes[x, y];

                                // Pass all 13 required arguments directly to the LevelTransform module
                                LevelTransform.DrawIsometricBlock(
                                    x,
                                    y,
                                    tileHeight,
                                    activeTheme,
                                    globalScale,
                                    heightMultiplier,
                                    panOffsetX,
                                    panOffsetY,
                                    rotationAngle,
                                    tiltFactor,
                                    renderStyleMode,
                                    displayPathOverlays,
                                    cellAttr
                                );
                            }
                        }
                    }

                    // Draw active palette indicators using explicit array indexes
                    Raylib.DrawRectangle(20, 95, 40, 20, activeTheme[0]);
                    Raylib.DrawRectangle(70, 95, 40, 20, activeTheme[1]);
                    Raylib.DrawRectangle(120, 95, 40, 20, activeTheme[2]);
                    Raylib.DrawText("Active Layout Palette Matrix Slots", 180, 98, 14, Color.LightGray);

                    Raylib.EndTextureMode(); // Hidden virtual canvas processing done

                    // GRAPHICS DRAWING ENVIRONMENT: Step B (Blit and upscale native texture directly to screen)
                    Raylib.BeginDrawing();
                    Raylib.ClearBackground(Color.Black);

                    // Blit virtual texture to window screen scaled exactly 3x larger cleanly
                    // Raylib textures are upside down natively due to OpenGL rules, so a negative height flips it right side up
                    Rectangle sourceRec = new Rectangle(0, 0, virtualWidth, -virtualHeight);
                    Rectangle destRec = new Rectangle(0, 0, screenWidth, screenHeight);
                    System.Numerics.Vector2 originPoint = new System.Numerics.Vector2(0, 0);

                    Raylib.DrawTexturePro(targetBuffer.Texture, sourceRec, destRec, originPoint, 0.0f, Color.White);

                    Raylib.EndDrawing();
                }

                // Unload texture buffers safely from GPU storage upon termination
                Raylib.UnloadRenderTexture(targetBuffer);
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
}