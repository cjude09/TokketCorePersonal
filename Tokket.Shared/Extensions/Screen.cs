using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Extensions
{
    public class Screen
    {
        public static void SetScreenSize(int h,int w) {
            Width = w;
            Heigth = h;
        }

        public static int Width { get; private set; }

        public static int Heigth { get; private set; }
    }
}
