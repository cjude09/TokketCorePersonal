using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokket.Shared.Models;

namespace Tokket.Shared.Helpers
{
    public static class PointsSymbolsHelper
    {
        private const string BASE_URL = "https://tokketcontent.blob.core.windows.net";
        private static string BASE_SIZE = "/pointssymbol48/";
        private static string BASE_FILE_TYPE = ".jpg";
        public static List<PointsSymbolModel> PointsSymbols { get; set; } = new List<PointsSymbolModel>();
        private static string[] colors = new string[]{ "black", "gold", "blue", "green", "orange", "purple", "pink" };//new string[] { "black" };
        private static string[] birds = new string[] { "Parakeet", "Blue Jay", "Eagle", "Falcon", "Owl" };
        private static int[] points = new int[]
        {
            10,30,60,100,150,250,400,650,1000,1500,2500,4000,6500,10000,15000,21000,28000,36000,45000,55000
        };

        public static string UserCurrentRank { get; set; }
        static PointsSymbolsHelper()
        {
            List<string> NewColors = new List<string>();
            string setColor = colors[0];
            if (Settings.GetTokketUser().PointsSymbolColor != null)
               setColor = Settings.GetTokketUser().PointsSymbolColor;

            NewColors.Add(setColor);
            foreach (var color in NewColors)
            {
                //Egg
                PointsSymbols.Add(new PointsSymbolModel()
                {
                    Name = $"Egg",
                    Description = "Egg",
                    Image = $"{BASE_URL}{BASE_SIZE}set1-{color} (0){BASE_FILE_TYPE}",
                    PointsRequired = 0,
                    Level = $"L1",
                    index = 1
                });
                //Each set
                for (int i = 0, x = 2; i < 4; ++i)
                {
                    //Each bird
                    for (int j = 0; j < 5; ++j)
                    {
                        PointsSymbols.Add(new PointsSymbolModel()
                        {
                            Name = birds[j],
                            Description = "Patch for earning points",
                            Image = $"{BASE_URL}{BASE_SIZE}set{i + 1}-{color} ({j + 1}){BASE_FILE_TYPE}",
                            PointsRequired = points[(5 * i) + (j)],
                            Degree = i,
                            Level = "L" + x,
                            index = x
                        });
                        x++;
                    }
                }
            }
        }

        public static void ResetColor(string colorName)
        {
            List<string> NewColors = new List<string>();
            string setColor = colorName;

            NewColors.Add(setColor);
            PointsSymbols = new List<PointsSymbolModel>();
            foreach (var color in NewColors)
            {
                //Egg
                PointsSymbols.Add(new PointsSymbolModel()
                {
                    Name = $"Egg",
                    Description = "Egg",
                    Image = $"{BASE_URL}{BASE_SIZE}set1-{color} (0){BASE_FILE_TYPE}",
                    PointsRequired = 0,
                    Level = $"L1",
                    index = 1
                });
                //Each set
                for (int i = 0, x = 2; i < 4; ++i)
                {
                    //Each bird
                    for (int j = 0; j < 5; ++j)
                    {
                        PointsSymbols.Add(new PointsSymbolModel()
                        {
                            Name = birds[j],
                            Description = "Patch for earning points",
                            Image = $"{BASE_URL}{BASE_SIZE}set{i + 1}-{color} ({j + 1}){BASE_FILE_TYPE}",
                            PointsRequired = points[(5 * i) + (j)],
                            Degree = i,
                            Level = "L" + x,
                            index = x
                        });
                        x++;
                    }
                }
            }
        }

        public static PointsSymbolModel GetPatchExactResult(long? points)
        {
            IEnumerable<PointsSymbolModel> Items = PointsSymbols;
            PointsSymbolModel getItem = PointsSymbols.FirstOrDefault(x => x.PointsRequired == points);
            List<PointsSymbolModel> LastItem = Items.ToList();
            PointsSymbolModel getter = new PointsSymbolModel();
            if (getItem == null)
            {
                if (points >= 0 && points < 10)
                {
                    getter = LastItem[0];
                }
                else if (points >= 10 && points < 30)
                {
                    getter = LastItem[1];
                }
                else if (points >= 30 && points < 60)
                {
                    getter = LastItem[2];
                }
                else if (points >= 60 && points < 100)
                {
                    getter = LastItem[3];
                }
                else if (points >= 100 && points < 150)
                {
                    getter = LastItem[4];
                }
                else if (points >= 150 && points < 250)
                {
                    getter = LastItem[5];
                }
                else if (points >= 250 && points < 400)
                {
                    getter = LastItem[6];
                }
                else if (points >= 400 && points < 650)
                {
                    getter = LastItem[7];
                }
                else if (points >= 650 && points < 1000)
                {
                    getter = LastItem[8];
                }
                else if (points >= 1000 && points < 1500)
                {
                    getter = LastItem[9];
                }
                else if (points >= 1500 && points < 2500)
                {
                    getter = LastItem[10];
                }
                else if (points >= 2500 && points < 4000)
                {
                    getter = LastItem[11];
                }
                else if (points >= 4000 && points < 6500)
                {
                    getter = LastItem[12];
                }
                else if (points >= 6500 && points < 10000)
                {
                    getter = LastItem[13];
                }
                else if (points >= 10000 && points < 15000)
                {
                    getter = LastItem[14];
                }
                else if (points >= 15000 && points < 21000)
                {
                    getter = LastItem[15];
                }
                else if (points >= 21000 && points < 28000)
                {
                    getter = LastItem[16];
                }
                else if (points >= 28000 && points < 36000)
                {
                    getter = LastItem[17];
                }
                else if (points >= 36000 && points < 45000)
                {
                    getter = LastItem[18];
                }
                else if (points >= 45000 && points < 55000)
                {
                    getter = LastItem[19];
                }
                else if (points >= 55000)
                {
                    getter = LastItem[20];
                }
                return getter;
            }
            else
            {
                getter = getItem;
                return getter;
            }
        }
        public static List<PointsSymbolModel> PatchesColors()
        {
            List<PointsSymbolModel> ListPatchesColors = new List<PointsSymbolModel>();
            int indexer = birds.ToList().IndexOf(UserCurrentRank);
            for (int c = 0; c < colors.Count() - 1; ++c)
            {
                //if (c == 0)
                //    indexer = 1;
                //else
                //    indexer = 0;
                ListPatchesColors.Add(new PointsSymbolModel()
                {
                    Name = colors[c].Substring(0,1).ToUpper() + colors[c].Substring(1, colors[c].Length - 1),
                    Description = "",
                    Image = $"{BASE_URL}{BASE_SIZE}set{4}-{colors[c]} ({indexer+1}){BASE_FILE_TYPE}",
                    PointsRequired = 0,
                    Degree = 0,
                    Level = "",
                    index = c
                });

              
            }

            return ListPatchesColors;
        }
    }
}