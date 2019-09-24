using Memory;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace skillBot
{
    public partial class Form1 : Form
    {

        System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"danger.wav");


        public static Process proc = Process.GetProcessesByName("DBLClient").FirstOrDefault();
        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_SHOWWINDOW = 0x0040;

        private const uint Restore = 9;
        private const int SW_MAXIMIZE = 3;
        private const int SW_MINIMIZE = 6;

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, uint Msg);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool Beep(uint dwFreq, uint dwDuration);

        public static Mem m = new Mem();
        static int pID;


        static TextBox staticTextBox1, staticTextBox3, staticTextBox4, staticTextBox5;
        static Button staticButton1, staticButton2;
        static Color pxl;



        public Form1()
        {
            

            InitializeComponent();

            pID = m.getProcIDFromName("DBLCLient");
            bool openProc = false;
            if (pID > 0)
            {
                openProc = m.OpenProcess(pID);
                int numerID = m.getProcIDFromName("DBLCLient");
                textBox2.Text = "DBLClient.exe is running with pID " + numerID + ".";
                textBox2.ForeColor = Color.Lime;
                this.Text = numerID.ToString();
            }
            else
            {
                string message = "DBLClient.exe is not running." + "\n" + "Run it first.";
                string caption = "Client error";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.
                result = MessageBox.Show(message, caption, buttons);
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // Closes the parent form.
                    System.Environment.Exit(1);
                }
            }
            staticTextBox1 = textBox1;
            staticButton1 = button1;
            label1.Text = "Reference point1 choosing";
            label2.Text = "Click [X]";
            button1.Text = "START";

            textBox1.TextAlign = HorizontalAlignment.Center;
            textBox2.TextAlign = HorizontalAlignment.Center;
            textBox3.TextAlign = HorizontalAlignment.Center;
            textBox1.Text = "[ 0,0 ]";

            pxl = GetColorFromScreen(new Point(0,0));
            textBox3.Text = pxl.ToString();

            staticTextBox3 = textBox3;

            staticTextBox4 = textBox4;
            staticTextBox5 = textBox5;
            staticButton2 = button2;
            label4.Text = "Reference point 2 choosing";
            label3.Text = "Click [Z]";
            button2.Text = "START";

            textBox5.Text = "[ 0,0 ]";
            textBox4.TextAlign = HorizontalAlignment.Center;
            textBox5.TextAlign = HorizontalAlignment.Center;
            textBox4.Text = pxl.ToString();

            button3.Text = "START";

            textBox6.TextAlign = HorizontalAlignment.Center;
            textBox7.TextAlign = HorizontalAlignment.Center;

            checkBox1.Text = "Food [F8]";
            checkBox2.Text = "Eq repair 1920x1080";
            checkBox3.Text = "slow repair";
            checkBox4.Text = "alarm sound";
            checkBox5.Text = "kill process";
            

            checkBox4.Checked = true;
            checkBox1.Checked = true;
            checkBox5.Checked = true;

            radioButton1.Text = "WE"; //lewo prawo
            radioButton2.Text = "EW"; //prawo lewo
            radioButton3.Text = "NS"; // gora dol
            radioButton4.Text = "SN"; // dol gora

            radioButton3.Checked = true;
            button4.Text = "Check!";
            

            ActivateWindow(proc.MainWindowHandle);

            checkIfOnline();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "START")
            {
                button1.Text = "STOP";
                _hookID = SetHook(_proc);
            }
            else
            {
                button1.Text = "START";
                UnhookWindowsHookEx(_hookID);
            }
        }

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public void sendEmail()
        {
            DateTime localDate = DateTime.Now;
            var fromAddress = new MailAddress("dblbotemail@gmail.com", "BOT ALERT");
            var toAddress = new MailAddress("adwokatdbl@gmail.com", "botuser");
            const string fromPassword = "Global98";
            const string subject = "BOT IS OFF";
             string body = "This is alert message " + localDate.ToString();

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
        private void Button3_Click(object sender, EventArgs e)
        {

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Text = "BOT RUNNING!";
            button3.Enabled = false;
            if (timer1.Enabled == false) timer1.Enabled = true;

            ActivateWindow(proc.MainWindowHandle);
            checkIfOnline();



            

            pxl1 = GetColorFromScreen(new Point(X1, Y1));
            pxl2 = GetColorFromScreen(new Point(X2, Y2));
            textBox6.Text = pxl1.ToString();
            textBox7.Text = pxl2.ToString();


            if (textBox6.Text != textBox3.Text || textBox7.Text != textBox4.Text || textBox8.Text == "Offline")
            {
                button3.Text = "TURNING OFF";
                if (timer2.Enabled == false) timer2.Enabled = true;
            }
            else
            {
                
                if (radioButton1.Checked) antyKick(" ^{LEFT}", "^{RIGHT}");
                else if (radioButton2.Checked) antyKick(" ^{RIGHT}", "^{LEFT}");
                else if (radioButton3.Checked) antyKick(" ^{UP}", "^{DOWN}");
                else antyKick(" ^{DOWN}", "^{UP}");


                MinimizeWindow(proc.MainWindowHandle);
            }
        }


        bool wasItOffline = false;

        private void checkIfOnline()
        {
            Thread.Sleep(500);

            Color pxl3 = GetColorFromScreen(new Point(15, 963));

            if (pxl3.ToString() != "Color [A=255, R=223, G=223, B=223]")
            {
                textBox8.Text = "Offline";
                textBox8.ForeColor = Color.Red;
                wasItOffline = true;
            }
            else
            {
                textBox8.Text = "Online";
                textBox8.ForeColor = Color.Green;
            }
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
           

            ActivateWindow(proc.MainWindowHandle);
            checkIfOnline();

            var date = DateTime.Now;
            int godzina = date.Hour;
            int minuta = date.Minute;
            if ( (godzina == 12 || godzina == 00 || godzina == 24) && (minuta > 9 && minuta < 12) ) {
                Thread.Sleep(180000);
            }

            if(textBox8.Text == "Offline")
            {
                SendKeys.SendWait("{ENTER}");
                textBox8.Text = "Trying to log in...";
                Thread.Sleep(5000);
                SendKeys.Send("{ENTER}");
                Thread.Sleep(5000);
                SendKeys.Send("{ENTER}");
                Thread.Sleep(5000);

                checkIfOnline();
            }

            pxl1 = GetColorFromScreen(new Point(X1, Y1));
            pxl2 = GetColorFromScreen(new Point(X2, Y2));
            textBox6.Text = pxl1.ToString();
            textBox7.Text = pxl2.ToString();

           
            if (textBox6.Text != textBox3.Text || textBox7.Text != textBox4.Text || textBox8.Text == "Offline")
            {
                button3.Text = "TURNING OFF";
                if (timer2.Enabled == false) timer2.Enabled = true;
            }
            else
            {
                if(wasItOffline)TransformAndOpenBackpack();
                if (radioButton1.Checked) antyKick(" ^{LEFT}", "^{RIGHT}");
                else if (radioButton2.Checked) antyKick(" ^{RIGHT}", "^{LEFT}");
                else if (radioButton3.Checked) antyKick(" ^{UP}", "^{DOWN}");
                else antyKick(" ^{DOWN}", "^{UP}");


                MinimizeWindow(proc.MainWindowHandle);
            }


        }
        
        private void antyKick(String s1, String s2)
        {

            Thread.Sleep(50);
            SendKeys.SendWait(s1);
            Thread.Sleep(50);
            SendKeys.SendWait(s2);
            Thread.Sleep(50);
            if (checkBox1.Checked) eatFood();
            if (checkBox2.Checked) repairEQ();
        }
        private void eatFood()
        {
            for (int i = 0; i < 20; i++) SendKeys.SendWait("{F8}");
        }

        Random rnd = new Random();
        private void TransformAndOpenBackpack()
        {
            randomLag();
            int X = 850;
            int Y = 580;
            SetCursorPos(X, Y);
            Thread.Sleep(50);
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
            randomLag();
            SendKeys.SendWait("{F2}");
            randomLag();
            SendKeys.SendWait("{F6}");
            randomLag();
            FurieFive();
            randomLag();
            TransformFive();
            wasItOffline = false;

        }
        private void randomLag()
        {
            int lag = rnd.Next(0, 300);
            Thread.Sleep(100 + lag);
        }
        private void FurieFive()
        {
            for (int i = 0; i < 5; i++)
            {
                SendKeys.SendWait("{F5}");
                randomLag();
                Thread.Sleep(1500);
            }
        }

        private void TransformFive()
        {
            for (int i = 0; i < 7; i++)
            {
                SendKeys.SendWait("{F12}");
                randomLag();
                Thread.Sleep(1500);
            }
        }
        private void repairEQ()
        {
            Clicker(1750, 115); // amulet
            Clicker(1750, 146); // left hand
            Clicker(1830, 147); // right hand
            Clicker(1750, 183); // belt
            Clicker(1790, 140); // armor
            Clicker(1790, 177); // legs
            Clicker(1790, 215); // boots
        }

        private void Clicker(int x, int y)
        {
            SetCursorPos(x, y);
            Thread.Sleep(50);
            int X = Cursor.Position.X;
            int Y = Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
            Thread.Sleep(50);
            if (checkBox3.Checked) Thread.Sleep(1000);
        }
        static Color pxl1, pxl2;
        static int X1 = 0, X2 = 0, Y1 = 0, Y2 = 0;

        private void TextBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void Button4_Click(object sender, EventArgs e)
        {
            ActivateWindow(proc.MainWindowHandle);
            Thread.Sleep(1500);
            pxl1 = GetColorFromScreen(new Point(X1, Y1));
            pxl2 = GetColorFromScreen(new Point(X2, Y2));
            textBox6.Text = pxl1.ToString();
            textBox7.Text = pxl2.ToString();

            
            if (textBox6.Text != textBox3.Text || textBox7.Text != textBox4.Text)
            {
                button3.Text = "TURNING OFF";
                if (timer2.Enabled == false) timer2.Enabled = true;
            }
            else
            {
                if (radioButton1.Checked) antyKick(" ^{LEFT}", "^{RIGHT}");
                else if (radioButton2.Checked) antyKick(" ^{RIGHT}", "^{LEFT}");
                else if (radioButton3.Checked) antyKick(" ^{UP}", "^{DOWN}");
                else antyKick(" ^{DOWN}", "^{UP}");

                MinimizeWindow(proc.MainWindowHandle);
            }
        }

        int failLoop = 0;
        private void Timer2_Tick(object sender, EventArgs e)
        {
            
            if (checkBox4.Checked)player.Play();
            failLoop++;
            if (failLoop == 5)
            {
                sendEmail();
                //Process.Start("shutdown", "/s /t 120");
                if (checkBox5.Checked)
                {
                    try { proc.Kill(); }
                    catch(Exception)
                    {

                    }
                }
                System.Environment.Exit(1);
                
            }
            
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
               // Console.Write((Keys)vkCode);
               // Console.Write(vkCode);
               // Console.WriteLine();

                switch (vkCode)
                {
                    case 88:
                        {
                            if (staticButton1.Text == "STOP")
                            {
                                int X = Cursor.Position.X;
                                int Y = Cursor.Position.Y;
                                X1 = X;
                                Y1 = Y;
                                staticTextBox1.Text = "[ " + X + ", " + Y + " ]";
                                pxl = GetColorFromScreen(new Point(X, Y));
                                staticTextBox3.Text = pxl.ToString();
                                staticButton1.PerformClick();
                            }
                        }
                        break;
                    case 90:
                        {
                            if (staticButton2.Text == "STOP")
                            {
                                int X = Cursor.Position.X;
                                int Y = Cursor.Position.Y;
                                X2 = X;
                                Y2 = Y;
                                staticTextBox5.Text = "[ " + X + ", " + Y + " ]";
                                pxl = GetColorFromScreen(new Point(X, Y));
                                staticTextBox4.Text = pxl.ToString();
                                staticButton2.PerformClick();
                            }
                        }
                        break;

                }

            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        static public Color GetColorFromScreen(Point p)
        {
            Rectangle rect = new Rectangle(p, new Size(2, 2));

            Bitmap map = CaptureFromScreen(rect);

            Color c = map.GetPixel(0, 0);

            map.Dispose();

            return c;
        }
        static public Bitmap CaptureFromScreen(Rectangle rect)
        {
            Bitmap bmpScreenCapture = null;

            if (rect == Rectangle.Empty)//capture the whole screen
            {
                rect = Screen.PrimaryScreen.Bounds;
            }

            bmpScreenCapture = new Bitmap(rect.Width, rect.Height);

            Graphics p = Graphics.FromImage(bmpScreenCapture);


            p.CopyFromScreen(rect.X,
                     rect.Y,
                     0, 0,
                     rect.Size,
                     CopyPixelOperation.SourceCopy);


            p.Dispose();

            return bmpScreenCapture;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "START")
            {
                button2.Text = "STOP";
                _hookID = SetHook(_proc);
            }
            else
            {
                button2.Text = "START";
                UnhookWindowsHookEx(_hookID);
            }
        }

        public static void ActivateWindow(IntPtr mainWindowHandle)
        {
            //check if already has focus
            if (mainWindowHandle == GetForegroundWindow()) return;

            //check if window is minimized
            if (IsIconic(mainWindowHandle))
            {
                ShowWindow(mainWindowHandle, Restore);
            }

            // Simulate a key press
            keybd_event(0, 0, 0, 0);

            SetForegroundWindow(mainWindowHandle);
        }

        public static void MinimizeWindow(IntPtr mainWindowHandle)
        {
            ShowWindow(mainWindowHandle, SW_MINIMIZE);
        }

    }



   
}

