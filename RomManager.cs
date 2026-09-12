using System;
using System.IO;
using System.Text;

namespace cSharpRaylib
{
    public static class RomManager
    {
        // Changed return type from List<Program.CityData> to plain List<CityData>
        public static List<CityData> LoadRomDatabase()
        {

            string romDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rom");
            string file1Path = Path.Combine(romDir, "136022-102.1h");
            string file2Path = Path.Combine(romDir, "136022-101.1f");

            if (!Directory.Exists(romDir) || !File.Exists(file1Path) || !File.Exists(file2Path))
            {
                MessageBox.Show(
                    "Critical Error: No ROM files found!\n\nPlease ensure your 'rom' folder contains:\n- 136022-102.1h\n- 136022-101.1f",
                    "Crystal Castles Viewer Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                Environment.Exit(1);
            }

            byte[] file1 = File.ReadAllBytes(file1Path);
            byte[] file2 = File.ReadAllBytes(file2Path);
            byte[] combinedData = new byte[file1.Length + file2.Length];
            file1.CopyTo(combinedData, 0);
            file2.CopyTo(combinedData, file1.Length);

            // Removed Program. from the list instantiation
            List<CityData> cities = new List<CityData>();
            for (int i = 0; i < 16; i++)
            {
                // Removed Program. from the object creation
                CityData city = new CityData();
                city.Load(combinedData, i * 0x400);
                cities.Add(city);
            }


            // === ISOLATED DATA LAYER INITIALIZATION OVERRIDES ===
            // This maps 37 waves to the 16 base cities automatically inside the loader module
            byte[] RoomToCityMap = new byte[] {
                0x00, 0x02, 0x09, 0xC3, 0x46, 0x71, 0x0C, 0xC7,
                0x06, 0x0D, 0x45, 0xCB, 0x04, 0x0A, 0x06, 0x4F,
                0x41, 0x4D, 0x3C, 0xC3, 0x0A, 0x02, 0x32, 0x3F,
                0x01, 0x04, 0x75, 0xFB, 0x01, 0x3A, 0x06, 0xF7,
                0x08, 0x7D, 0x05, 0xCB, 0x0E
            };

            for (int roomNum = 0; roomNum < 37; roomNum++)
            {
                int baseCityIndex = RoomToCityMap[roomNum] & 0x0F;
                CityData targetedCity = cities[baseCityIndex];

                // Securely lock the 100% validated coordinates into memory at boot time
                ElevatorPremapper.ApplyOverrides(roomNum, targetedCity.Elevators);
            }

            return cities; // Return the fully pre-mapped, bulletproof database object
        }



        public static void ExportStageTextFile(int stageNum, string stageName, CityData activeCity)
        {
            try
            {
                string filename = $"Stage_{stageNum:D2}_{stageName.Replace(" ", "_")}_Matrix.txt";
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

                using (StreamWriter writer = new StreamWriter(fullPath))
                {
                    writer.WriteLine($"=== VIEWER TEST 1 EXPORT LOG - RUN TIME: {DateTime.Now} ===");
                    writer.WriteLine($"Stage [{stageNum:D2}] - [{stageName}] | Lifts Configured: {activeCity.NumElevators}");

                    if (activeCity.NumElevators == 0)
                    {
                        writer.WriteLine("  * Lift Data: N/A (No lifts configured on this layout)");
                        writer.WriteLine("\nLOCATION : N/A\nBEHAVIOR : N/A\nRESULTS  : 0/0 passed");
                    }
                    else
                    {
                        int passedCount = 0;
                        for (int i = 0; i < activeCity.Elevators.Count; i++)
                        {
                            var ev = activeCity.Elevators[i];
                            // Simple layout validation pass verification strings tracking bounds
                            string statusStr = ev.IsMapped ? $"SUCCESS -> [CellX: {ev.CellX}, CellY: {ev.CellY}]" : "FAILED -> Out of bounds";
                            if (ev.IsMapped) passedCount++;

                            writer.WriteLine($"  * Lift [{i}]: ScreenX={ev.HorizontalPosition}, ScreenY={ev.VerticalPosition} | Map Position: {statusStr}");
                        }

                        writer.WriteLine("\nLOCATION : Pending Audit");
                        writer.WriteLine("BEHAVIOR : Pending Audit");
                        writer.WriteLine($"RESULTS  : {passedCount}/{activeCity.Elevators.Count} passed");
                    }

                    writer.WriteLine("\n--------------------------------------------------------------------------------");
                    writer.WriteLine("[Tile Height Grid Layout (22x22 Raw Blueprint View)]\n");

                    for (int i = 0; i < 22; i++)
                    {
                        StringBuilder rowLine = new StringBuilder();
                        for (int j = 0; j < 22; j++)
                        {
                            bool isElevatorSpot = false;
                            foreach (var ev in activeCity.Elevators)
                            {
                                if (ev.IsMapped && ev.CellX == i && ev.CellY == j)
                                {
                                    isElevatorSpot = true;
                                    break;
                                }
                            }

                            int heightVal = activeCity.Heights[i, j];

                            if (isElevatorSpot) rowLine.Append(" E  ");
                            else if (heightVal == 0) rowLine.Append("  . ");
                            else rowLine.Append($" {heightVal:D2} ");
                        }
                        writer.WriteLine(rowLine.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting layout report text matrix: {ex.Message}");
            }
        }
    }
  
}
