using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MonitorOff
{
    public partial class Form1 : Form
    {
        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }

        public int WM_SYSCOMMAND = 0x0112;
        public int SC_MONITORPOWER = 0xF170;

        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("kernel32.dll")]
        static extern int GetTickCount();

        bool bTurnedOff;

        public Form1()
        {
            InitializeComponent();
        }

        public long getLastInputTime()
        {
            long inputSince = long.MaxValue;

            LASTINPUTINFO info = new LASTINPUTINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);

            if (GetLastInputInfo(ref info))
            {
                try
                {
                    inputSince = ((Environment.TickCount) & Int32.MaxValue) - info.dwTime;
                }
                catch
                {
                    inputSince = long.MaxValue;
                }
            }

            return inputSince;
        }

        private void turnMonitorOff()
        {
            SendMessage(this.Handle.ToInt32(), WM_SYSCOMMAND, SC_MONITORPOWER, 2);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            long lastInput;
            long currentMilliseconds;

            lastInput = getLastInputTime();

            try
            {
                currentMilliseconds = Int64.Parse(textBox3.Text);
            }
            catch
            {
                currentMilliseconds = 60000;
            }

            if (lastInput > currentMilliseconds && !bTurnedOff)
            {
                textBox1.Text = getLastInputTime().ToString();
                turnMonitorOff();
                bTurnedOff = true;
            }
            else if (lastInput <= currentMilliseconds && bTurnedOff)
            {
                bTurnedOff = false;
            }

            textBox1.Text = lastInput.ToString();
        }
    }
}
