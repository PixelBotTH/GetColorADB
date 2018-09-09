using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class PixelSearch
    {
        ////// พัฒนาโดยเพจ เขียนโปรแกรมยามว่าง ////////
        ////// พัฒนาโดยเพจ เขียนโปรแกรมยามว่าง ////////
        [DllImport("user32")]
        private static extern int PrintWindow(IntPtr hWnd, IntPtr dc, uint flags);
        [DllImport("user32")]
        private static extern int GetWindowRect(IntPtr hWnd, ref RECT rect);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "GetDC")]
        internal extern static IntPtr GetDC(IntPtr hWnd);
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        internal extern static IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        internal extern static IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        internal extern static IntPtr DeleteDC(IntPtr hDc);
        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        internal extern static IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        internal extern static bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, int RasterOp);
        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        internal extern static IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        internal extern static IntPtr DeleteObject(IntPtr hDc);
        public const int SRCCOPY = 0x00CC0020;
        private struct RECT
        {
            int left;
            int top;
            int right;
            int bottom;

            public int Left
            {
                get { return this.left; }
            }

            public int Top
            {
                get { return this.top; }
            }

            public int Width
            {
                get { return right - left; }
            }

            public int Height
            {
                get { return bottom - top; }
            }

            public static implicit operator Rectangle(RECT rect)
            {
                return new Rectangle(rect.left, rect.top, rect.Width, rect.Height);
            }
        }

        public static int Status;

        public static bool hasColor(Point p)
        {
            if (p.X > 0 || p.Y > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static Point PixelSearchsByImg(Image img, int PixelColor)
        {
            Bitmap b = new Bitmap(img);
            BitmapData data = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
            ImageLockMode.ReadOnly, b.PixelFormat);  // make sure you check the pixel format as you will be looking directly at memory
            var Dcolor = PixelColor.ToString();
            var PixelColor1 = Convert.ToInt32(Dcolor);
            Color Pixel_Color = Color.FromArgb(PixelColor1);
            Point Pixel_Coords = new Point(-1, -1);

            unsafe
            {
                // example assumes 24bpp image.  You need to verify your pixel depth
                // loop by row for better data locality 
                int[] Formatted_Color = new int[3] { Pixel_Color.B, Pixel_Color.G, Pixel_Color.R };
                for (int y = 0; y < data.Height; ++y)
                {
                    //byte* pRow = (byte*)data.Scan0 + y * data.Stride;
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);

                    for (int x = 0; x < data.Width; ++x)
                    {
                        Pixel_Coords = new Point(x, y);
                        goto end;
                    }
                }
            }
            end:
            b.UnlockBits(data);
            b.Dispose();

            return Pixel_Coords;
        }

        public static Point PixelSearchs(IntPtr iHandle, int L, int Top, int R, int Bottom, int PixelColor, int Shade_Variation)
        {
            Status = 1;
            Rectangle rect = Rectangle.FromLTRB(L, Top, R, Bottom);
            var Dcolor = PixelColor.ToString();
            var PixelColor1 = Convert.ToInt32(Dcolor);
            Color Pixel_Color = Color.FromArgb(PixelColor1);
            Point Pixel_Coords = new Point(-1, -1);
            IntPtr hdcSrc = GetDC(iHandle);
            IntPtr hdcDest = CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = CreateCompatibleBitmap(hdcSrc, rect.Width, rect.Height);
            IntPtr hOld = SelectObject(hdcDest, hBitmap);
            BitBlt(hdcDest, 0, 0, rect.Width, rect.Height, hdcSrc, rect.X, rect.Y, SRCCOPY);
            SelectObject(hdcDest, hOld);
            DeleteDC(hdcDest);
            ReleaseDC(iHandle, hdcSrc);
            Bitmap bmp = Bitmap.FromHbitmap(hBitmap);
            BitmapData RegionIn_BitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int[] Formatted_Color = new int[3] { Pixel_Color.B, Pixel_Color.G, Pixel_Color.R };
            unsafe
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    byte* row = (byte*)RegionIn_BitmapData.Scan0 + (y * RegionIn_BitmapData.Stride);
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        if (row[x * 3] >= (Formatted_Color[0] - Shade_Variation) & row[x * 3] <= (Formatted_Color[0] + Shade_Variation))//b
                        {
                            if (row[(x * 3) + 1] >= (Formatted_Color[1] - Shade_Variation) & row[(x * 3) + 1] <= (Formatted_Color[1] + Shade_Variation))//g
                            {
                                if (row[(x * 3) + 2] >= (Formatted_Color[2] - Shade_Variation) & row[(x * 3) + 2] <= (Formatted_Color[2] + Shade_Variation))//r
                                {
                                    Pixel_Coords = new Point(x + rect.X, y + rect.Y);
                                    Status = 0;
                                    goto end;
                                }
                            }
                        }
                    }
                }
            }
            end:
            bmp.UnlockBits(RegionIn_BitmapData);
            DeleteObject(hBitmap);
            bmp.Dispose();
            return Pixel_Coords;
        }
        public static Bitmap ResizeImage(Bitmap imgToResize, Size size)
        {
            try
            {
                Bitmap b = new Bitmap(size.Width, size.Height);
                using (Graphics g = Graphics.FromImage((Image)b))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
                }
                return b;
            }
            catch
            {
                Console.WriteLine("Bitmap could not be resized");
                return imgToResize;
            }
        }

        public static bool FindBitmap(Bitmap bmpNeedle, Bitmap bmpHaystack, Point location)
        {
            for (int outerX = 0; outerX <= bmpHaystack.Width - bmpNeedle.Width - 1; outerX++)
            {
                for (int outerY = 0; outerY <= bmpHaystack.Height - bmpNeedle.Height - 1; outerY++)
                {
                    for (int innerX = 0; innerX <= bmpNeedle.Width - 1; innerX++)
                    {
                        for (int innerY = 0; innerY <= bmpNeedle.Height - 1; innerY++)
                        {
                            Color cNeedle = bmpNeedle.GetPixel(innerX, innerY);
                            Color cHaystack = bmpHaystack.GetPixel(innerX + outerX, innerY + outerY);

                            if (cNeedle.R != cHaystack.R || cNeedle.G != cHaystack.G || cNeedle.B != cHaystack.B)
                                goto notFound;
                        }
                    }
                    location = new Point(outerX, outerY);
                    return true;
                    notFound:
                    ;
                    continue;
                }
            }
            location = Point.Empty;
            return false;
        }




        public static Bitmap GetWindowImage(IntPtr hWnd, Size size)
        {
            try
            {
                if (size.IsEmpty || size.Height < 0 || size.Width < 0) return null;

                Bitmap bmp = new Bitmap(size.Width, size.Height);
                Graphics g = Graphics.FromImage(bmp);
                IntPtr dc = g.GetHdc();

                PrintWindow(hWnd, dc, 0);

                g.ReleaseHdc();
                g.Dispose();

                return bmp;
            }
            catch { return null; }
        }
        public static Rectangle GetWindowPlacement(IntPtr hWnd)
        {
            RECT rect = new RECT();

            GetWindowRect(hWnd, ref rect);

            return rect;
        }
    }
}