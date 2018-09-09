using System;
using System.ComponentModel;

namespace WindowsFormsApp1
{
    public class ColorPoint
    {

        public ColorPoint(int x, int y, int color)
        {
            X = x;
            Y = y;
            Color = color;
        }

        [Browsable(false)]
        public bool IsEmpty { get; }
        //
        // Summary:
        //     Gets or sets the x-coordinate of this System.Drawing.Point.
        //
        // Returns:
        //     The x-coordinate of this System.Drawing.Point.
        public int X { get; set; }
        //
        // Summary:
        //     Gets or sets the y-coordinate of this System.Drawing.Point.
        //
        // Returns:
        //     The y-coordinate of this System.Drawing.Point.
        public int Y { get; set; }


        public int Color { get; set; }
        public bool checkstatus(int c = 10)
        {


            if (Getcolor.CheckColor(WindowHandler.appname, this, c))
            {
                return true;
            }
            else
            {
                Console.WriteLine("Color at : " + X + "," + Y + " :" + Getcolor.GETCOLORSTRING(WindowHandler.appname, X, Y) + "  : " + Getcolor.HexConverterOLD(System.Drawing.Color.FromArgb(Color)));
                return false;
            }


        }
    }
}