using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace cSharpRaylib
{
    public static class RomManager
    {
        // Changed return type from List<Program.CityData> to plain List<CityData>
        public static List<CityData> LoadRomDatabase()
        {
            byte[] RoomToCityMap = new byte[] {
                0x00, 0x02, 0x09, 0xC3, 0x46, 0x71, 0x0C, 0xC7,
                0x06, 0x0D, 0x45, 0xCB, 0x04, 0x0A, 0x06, 0x4F,
                0x41, 0x4D, 0x3C, 0xC3, 0x0A, 0x02, 0x32, 0x3F,
                0x01, 0x04, 0x75, 0xFB, 0x01, 0x3A, 0x06, 0xF7,
                0x08, 0x7D, 0x05, 0xCB, 0x0E
            };

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

            return cities;
        }

        public static void ExportStageTextFile(int stageNum, string stageName, CityData activeCity)
        {
            try
            {
                string filename = $"Stage_{stageNum:D2}_{stageName.Replace(" ", "_")}_Matrix.txt";
                string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fullPath))
                {
                    writer.WriteLine($"=== STRUCTURAL AUDIT: STAGE {stageNum} ({stageName}) ===");
                    writer.WriteLine($"Elevators Configured: {activeCity.NumElevators}\n");
                    writer.WriteLine("[Tile Height Grid Layout (22x22 View)]");

                    // Look closely at the loop drivers: i represents X, j represents Y
                    for (int i = 0; i < 22; i++)
                    {
                        System.Text.StringBuilder rowLine = new System.Text.StringBuilder();
                        for (int j = 0; j < 22; j++)
                        {
                            // Check if an elevator matches this current cell coordinate position
                            bool isElevatorSpot = false;
                            foreach (var ev in activeCity.Elevators)
                            {
                                if (ev.HorizontalPosition == i && ev.VerticalPosition == j)
                                {
                                    isElevatorSpot = true;
                                    break;
                                }
                            }

                            int heightVal = activeCity.Heights[i, j];

                            if (isElevatorSpot)
                            {
                                rowLine.Append(" E  ");
                            }
                            else if (heightVal == 0)
                            {
                                rowLine.Append("  . ");
                            }
                            else
                            {
                                rowLine.Append($" {heightVal:D2} ");
                            }
                        }
                        writer.WriteLine(rowLine.ToString());
                    }
                }
                System.Diagnostics.Debug.WriteLine($"Exported matrix block successfully: {fullPath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to export data file: {ex.Message}");
            }
        }
     }
    }
