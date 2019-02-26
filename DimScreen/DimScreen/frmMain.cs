using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DimScreen {
    public partial class frmMain : Form {
        public enum GWL {
            ExStyle = -20
        }

        public enum LWA {
            ColorKey = 0x1,
            Alpha = 0x2
        }

        public enum WS_EX {
            Transparent = 0x20,
            Layered = 0x80000
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong(IntPtr hWnd, GWL nIndex);

        [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte alpha, LWA dwFlags);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, int dwNewLong);

        private float currentValue;
        private float targetValue;
        private Timer timerPhase = new Timer() { Interval = 25, Enabled = false };

        private void applyTransparency() {
            float calculatedValue = currentValue + Math.Sign(targetValue - currentValue) * 0.02f;

            if(Math.Abs(targetValue - currentValue) < 0.02f * 2) {
                currentValue = targetValue;
                timerPhase.Stop();
            }

            int wl = GetWindowLong(Handle, GWL.ExStyle);
            wl = wl | 0x80000 | 0x20;
            SetWindowLong(Handle, GWL.ExStyle, wl);

            byte value = (byte)(calculatedValue * 255);
            SetLayeredWindowAttributes(Handle, 0x128, value, LWA.Alpha);

            currentValue = calculatedValue;
        }

        private void Form1_Load(object sender, EventArgs e) {
            // use working space rectangle info
            Size = new System.Drawing.Size(Area.Width, Area.Height);
            Location = new System.Drawing.Point(Area.X, Area.Y);

            applyTransparency();
        }

        private void timerPhase_Tick(object sender, EventArgs e) {
            applyTransparency();
        }

        protected override void OnShown(EventArgs e) {
            base.OnShown(e);
        }

        public System.Drawing.Rectangle Area { get; set; }

        public void Dim(float amount) => Dimness = amount / 100.0f;
        
        public float Dimness {
            get => targetValue;
            set {
                targetValue = value;
                timerPhase.Start();
            }
        }

        public frmMain() {
            InitializeComponent();
            timerPhase.Tick += timerPhase_Tick;
        }
    }
}