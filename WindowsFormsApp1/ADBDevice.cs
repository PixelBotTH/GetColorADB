using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class ADBDevice
    {


        DeviceData data { get; set; }
        static Image img = null;


        public ADBDevice(DeviceData data )
        {
            this.data = data;
        }


        public Color GetADBPixel(int x, int y)
        {
            ADBImg();
            ShellCommand("input tap " + (x+18) + " " + (y-18));
            Bitmap b = new Bitmap(img);
            Color color = b.GetPixel((x+18), (y-18));
            b.Dispose();
            img.Dispose();
            return color;
            //return Color.AliceBlue;
        }
    

        public static Tuple<Color,Point> PixelImg(Image img,int getx,int gety, int getw = 1, int geth = 1, int PixelColor = 0x000000)
        {
            var Dcolor = PixelColor.ToString();
            var PixelColor1 = Convert.ToInt32(Dcolor);
            Color Pixel_Color = Color.FromArgb(PixelColor1);
            Point Pixel_Coords = new Point(-1, -1);
            if (img == null)
            {
                Console.WriteLine("Null Point : Image");
                return new Tuple<Color, Point>(Color.Black, Pixel_Coords);
            }
            Bitmap b = new Bitmap(img);
            Color color = b.GetPixel(getx,gety);
            BitmapData data = b.LockBits(new Rectangle(getx, gety, getx + 1, gety + 1),
                ImageLockMode.ReadOnly, b.PixelFormat);
            if (getw>2&& geth>2)
            {
                data = b.LockBits(new Rectangle(getx, gety, getw,geth),
                ImageLockMode.ReadOnly, b.PixelFormat);  // make sure you check the pixel format as you will be looking directly at memory
            }

            

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
            img.Dispose();
            return new Tuple<Color, Point>(color, Pixel_Coords);
        }
    
        private void ADBImg()
        {

            using (SyncService service = new SyncService(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)), data))
                if (File.Exists(Application.StartupPath + @"\screentemp.png"))
                {
                   
                    ShellCommand("screencap /sdcard/screentemp.png");
                    using (Stream stream = File.OpenWrite(Application.StartupPath + @"\screentemp.png"))
                    {
                        service.Pull("/sdcard/screentemp.png", stream, null, CancellationToken.None);


                    }
                    img = Image.FromFile(Application.StartupPath + @"\screentemp.png");
                }


        }
        public void ShellCommand( string command)
        {
            if (data != null)
            {
                var receiver = new ConsoleOutputReceiver();

                AdbClient.Instance.ExecuteRemoteCommand(command, data, receiver);
                //AppendText(richTextBox1, "ADB Log : Execute Command " + command + "/n", Color.Gold);
                if (!receiver.ToString().Equals(""))
                {
                    //AppendText(richTextBox1, "ADB Log : Error Command " + receiver.ToString(), Color.Gold);
                }

                Console.WriteLine("The device responded:");
                Console.WriteLine(receiver.ToString());
            }

        }
    }
}
