
using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
   
    public partial class Form1 : Form
    {
//        Best Way:
//This worked for my Droid X, .602+ Rooted was using adb shell, then:
//getprop net.hostname
//setprop net.hostname NAME
        /// <summary>
        ///  input text <string>
        ///input keyevent<event_code>
        ///0 -->  "KEYCODE_UNKNOWN" 
              //1 -->  "KEYCODE_MENU" 
              //2 -->  "KEYCODE_SOFT_RIGHT" 
              //3 -->  "KEYCODE_HOME" 
              //4 -->  "KEYCODE_BACK" 
              //5 -->  "KEYCODE_CALL" 
              //6 -->  "KEYCODE_ENDCALL" 
              //7 -->  "KEYCODE_0" 
              //8 -->  "KEYCODE_1" 
              //9 -->  "KEYCODE_2" 
              //10 -->  "KEYCODE_3" 
              //11 -->  "KEYCODE_4" 
              //12 -->  "KEYCODE_5" 
              //13 -->  "KEYCODE_6" 
              //14 -->  "KEYCODE_7" 
              //15 -->  "KEYCODE_8" 
              //16 -->  "KEYCODE_9" 
              //17 -->  "KEYCODE_STAR" 
              //18 -->  "KEYCODE_POUND" 
              //19 -->  "KEYCODE_DPAD_UP" 
              //20 -->  "KEYCODE_DPAD_DOWN" 
              //21 -->  "KEYCODE_DPAD_LEFT" 
              //22 -->  "KEYCODE_DPAD_RIGHT" 
              //23 -->  "KEYCODE_DPAD_CENTER" 
              //24 -->  "KEYCODE_VOLUME_UP" 
              //25 -->  "KEYCODE_VOLUME_DOWN" 
              //26 -->  "KEYCODE_POWER" 
              //27 -->  "KEYCODE_CAMERA" 
              //28 -->  "KEYCODE_CLEAR" 
              //29 -->  "KEYCODE_A" 
              //30 -->  "KEYCODE_B" 
              //31 -->  "KEYCODE_C" 
              //32 -->  "KEYCODE_D" 
              //33 -->  "KEYCODE_E" 
              //34 -->  "KEYCODE_F" 
              //35 -->  "KEYCODE_G" 
              //36 -->  "KEYCODE_H" 
              //37 -->  "KEYCODE_I" 
              //38 -->  "KEYCODE_J" 
              //39 -->  "KEYCODE_K" 
              //40 -->  "KEYCODE_L" 
              //41 -->  "KEYCODE_M" 
              //42 -->  "KEYCODE_N" 
              //43 -->  "KEYCODE_O" 
              //44 -->  "KEYCODE_P" 
              //45 -->  "KEYCODE_Q" 
              //46 -->  "KEYCODE_R" 
              //47 -->  "KEYCODE_S" 
              //48 -->  "KEYCODE_T" 
              //49 -->  "KEYCODE_U" 
              //50 -->  "KEYCODE_V" 
              //51 -->  "KEYCODE_W" 
              //52 -->  "KEYCODE_X" 
              //53 -->  "KEYCODE_Y" 
              //54 -->  "KEYCODE_Z" 
              //55 -->  "KEYCODE_COMMA" 
              //56 -->  "KEYCODE_PERIOD" 
              //57 -->  "KEYCODE_ALT_LEFT" 
              //58 -->  "KEYCODE_ALT_RIGHT" 
              //59 -->  "KEYCODE_SHIFT_LEFT" 
              //60 -->  "KEYCODE_SHIFT_RIGHT" 
              //61 -->  "KEYCODE_TAB" 
              //62 -->  "KEYCODE_SPACE" 
              //63 -->  "KEYCODE_SYM" 
              //64 -->  "KEYCODE_EXPLORER" 
              //65 -->  "KEYCODE_ENVELOPE" 
              //66 -->  "KEYCODE_ENTER" 
              //67 -->  "KEYCODE_DEL" 
              //68 -->  "KEYCODE_GRAVE" 
              //69 -->  "KEYCODE_MINUS" 
              //70 -->  "KEYCODE_EQUALS" 
              //71 -->  "KEYCODE_LEFT_BRACKET" 
              //72 -->  "KEYCODE_RIGHT_BRACKET" 
              //73 -->  "KEYCODE_BACKSLASH" 
              //74 -->  "KEYCODE_SEMICOLON" 
              //75 -->  "KEYCODE_APOSTROPHE" 
              //76 -->  "KEYCODE_SLASH" 
              //77 -->  "KEYCODE_AT" 
              //78 -->  "KEYCODE_NUM" 
              //79 -->  "KEYCODE_HEADSETHOOK" 
              //80 -->  "KEYCODE_FOCUS" 
              //81 -->  "KEYCODE_PLUS" 
              //82 -->  "KEYCODE_MENU" 
              //83 -->  "KEYCODE_NOTIFICATION" 
              //84 -->  "KEYCODE_SEARCH" 
              //85 -->  "TAG_LAST_KEYCODE"
              /// </summary>

              List<DeviceData> devices = new List<DeviceData>();

        ADBDevice adbcolor;
        public Form1()
        {
            InitializeComponent();

            AdbServer server = new AdbServer();
            server.StartServer(Application.StartupPath + "/adb/adb.exe", restartServerIfNewer: false);
        }
      
        private void Form1_Load(object sender, EventArgs e)
        {
            //timer1.Start();
            var devices = AdbClient.Instance.GetDevices();

            foreach (var device in devices)
            {
                this.devices.Add(device);
                comboBox1.Items.Add(device.Serial);
                
                Console.WriteLine(device.Product);
            }

        }
        
        public DeviceData GetByName(string s)
        {
            foreach (var device in devices)
            {

                if (s.Equals(device.Serial))
                {
                    return device;
                }
                else
                {
                    return null;
                }
            }
            return null;

        }


        private void button1_Click(object sender, EventArgs e)
        {
            EchoTest(comboBox1.Text,textBox1.Text);
        }
        public static void AppendText(RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
            box.ScrollToCaret();
        }
        void EchoTest(string s , string command )
        {
            var device = GetByName(s);
            if (device!=null)
            {
                var receiver = new ConsoleOutputReceiver();

                AdbClient.Instance.ExecuteRemoteCommand(command, device, receiver);
                AppendText(richTextBox1, "ADB Log : Execute Command " + command + "/n", Color.Gold);
                if (!receiver.ToString().Equals(""))
                {
                    AppendText(richTextBox1, "ADB Log : Error Command " + receiver.ToString(), Color.Gold);
                }

                Console.WriteLine("The device responded:");
                Console.WriteLine(receiver.ToString());
            }
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = ScreenCapAsync(comboBox1.Text).Result;
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

        }
        public async Task<Image> ScreenCapAsync(string s)
        {
            var device = GetByName(s);
            if (device!=null)
            {
                
                //devicescreen.Sta;

                return GetFramebufferAsyncPerformanceTest(device).Result;

            }
            return null;
        }
        public async Task<Image> GetFramebufferAsyncPerformanceTest(DeviceData device)
        {
            //Image img;
            Framebuffer framebuffer = AdbClient.Instance.CreateRefreshableFramebuffer(device);
            while (true)
            {
                await framebuffer.RefreshAsync(CancellationToken.None).ConfigureAwait(false);
                // Only call this method if you want to process the data in .NET. If you don't, you can save
                // the raw data to disk by using framebuffer.Data
                 var img = framebuffer.ToImage();
                return img;
                
            }
        }


       
      
        private void button3_Click(object sender, EventArgs e)
        {
            adbcolor = new ADBDevice(GetByName(comboBox1.Text));

            //int Color = Int32.Parse(textBox1.Text);
            int x = Int32.Parse(textBox3.Text);
            int y = Int32.Parse(textBox4.Text) ;
            //Color c = Getcolor.GetImagePixel(ScreenCapAsync(comboBox1.Text).Result, x, y);
            Color c = adbcolor.GetADBPixel(x, y);
            
            MessageBox.Show("Color : " + Getcolor.HexConverterOLD(c));


        }
        void PullFile()
        {
            var device = GetByName(comboBox1.Text);

            using (SyncService service = new SyncService(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)), device))
            if (File.Exists(Application.StartupPath + @"\screen.png"))
                {

                    EchoTest(comboBox1.Text, "screencap /sdcard/screentemp.png");
                    using (Stream stream = File.OpenWrite(Application.StartupPath + @"\screentemp.png"))
                    {
                        service.Pull("/sdcard/screentemp.png", stream, null, CancellationToken.None);
                        stream.Dispose();

                    }
                }
            else
                {

                    EchoTest(comboBox1.Text, "screencap /sdcard/screen.png");
                    using (Stream stream = File.OpenWrite(Application.StartupPath + @"\screen.png"))
                    {
                        service.Pull("/sdcard/screen.png", stream, null, CancellationToken.None);
                        stream.Dispose();
                    }
            }

        }
 
        private void button2_Click(object sender, EventArgs e)
        {
            var device = GetByName(comboBox1.Text);

            //PullFile();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ScreenCapAsync(comboBox1.Text) != null)
            {
                pictureBox1.Image = ScreenCapAsync(comboBox1.Text).Result;

            }

        }
    }

}
