using System;
using Raylib_cs;

namespace cSharpRaylib
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define basic window resolution targets
            const int screenWidth = 800;
            const int screenHeight = 600;

            // 1. Initialize the Hardware-Accelerated Graphics Context
            Raylib.InitWindow(screenWidth, screenHeight, "cSharpRaylib - Crystal Castles Renderer Verification");

            // Lock the frame rate precisely to 60 FPS to prevent core loop execution runaway
            Raylib.SetTargetFPS(60);

            Console.WriteLine("[SYSTEM] Raylib Window engine initiated successfully.");
            Console.WriteLine("[SYSTEM] Close the window or press ESC to exit safely.");

            // 2. The Main Video Rendering Loop
            while (!Raylib.WindowShouldClose())
            {
                // Prepare the GPU buffer for immediate color writing operations
                Raylib.BeginDrawing();

                // Clear out the previous frame graphics layer with a solid charcoal color
                Raylib.ClearBackground(Color.DarkGray);

                // Print a structural text metric placeholder directly inside the graphical pane
                Raylib.DrawText("Raylib Engine Operational", 20, 20, 20, Color.White);

                // Commit the drawing commands directly to the active hardware display buffer
                Raylib.EndDrawing();
            }

            // 3. De-allocate and tear down memory contexts safely upon exit
            Raylib.CloseWindow();
            Console.WriteLine("[SYSTEM] Graphics pipeline closed cleanly.");
        }
    }
}