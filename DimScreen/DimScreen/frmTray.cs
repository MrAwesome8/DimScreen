using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DimScreen {
    public partial class frmTray : Form {
        private frmMain[] overlays;
        private bool[] activeMonitors;

        private void SetValuesOnSubItems(List<ToolStripMenuItem> items) {
            //https://stackoverflow.com/a/32579262
            items.ForEach(item => {
                if(!"Skip".Equals(item.Tag)) {
                    var dropdown = (ToolStripDropDownMenu)item.DropDown;
                    if(dropdown != null) {
                        dropdown.ShowImageMargin = false;
                        SetValuesOnSubItems(item.DropDownItems.OfType<ToolStripMenuItem>().ToList());
                    }
                }
            });
        }

        public frmTray() {
            InitializeComponent();
            contextMenuStrip1.Renderer = new DarkMenu(new DarkMenuColorTable());

            SetValuesOnSubItems(contextMenuStrip1.Items.OfType<ToolStripMenuItem>().ToList());
        }

        private void configureOverlays(float initialValue) {
            clearOverlays();

            if(overlays == null || overlays.Length != Screen.AllScreens.Length) {
                int i = 0;

                activeMonitors = new bool[Screen.AllScreens.Length];
                overlays = new frmMain[Screen.AllScreens.Length];
                foreach(var screen in Screen.AllScreens) {
                    frmMain overlay = new frmMain {
                        Dimness = 0,
                        Area = screen.WorkingArea
                    };
                    overlay.Show();

                    overlays[i] = overlay;
                    activeMonitors[i] = true;

                    ToolStripMenuItem monitor = new ToolStripMenuItem($"Monitor {++i}") {
                        Checked = true,
                        CheckState = CheckState.Checked,
                        CheckOnClick = true
                    };
                    monitor.CheckStateChanged += Monitor_CheckStateChanged;
                    monitorsToolStripMenuItem.DropDownItems.Add(monitor);

                    monitor.MouseEnter += (o, e) => monitorsToolStripMenuItem.DropDown.AutoClose = false;
                    monitor.MouseLeave += (o, e) => {
                        monitorsToolStripMenuItem.DropDown.AutoClose = true;

                        contextMenuStrip1.Show(); //fix hiding behind taskbar
                    };
                }
            }

            foreach(frmMain form in overlays) {
                form.Dimness = initialValue;
            }
        }

        private void Monitor_CheckStateChanged(object sender, EventArgs e) {
            for(int i = 0; i < activeMonitors.Length; ++i) {
                var tmp = ((ToolStripMenuItem)monitorsToolStripMenuItem.DropDownItems[i]).CheckState == CheckState.Checked;

                if(activeMonitors[i] != tmp) {
                    if(tmp) {
                        overlays[i].Dim(dimnessBar.Value);
                    } else {
                        overlays[i].Dim(0.0f);
                    }
                    activeMonitors[i] = tmp;
                }
            }
        }

        private void clearOverlays() {
            if(overlays == null) { return; }

            foreach(var form in overlays) {
                form.Close();
            }

            Array.Clear(overlays, 0, overlays.Length);
        }

        private void frmTray_Load(object sender, EventArgs e) {
            var arg = "";
            var args = Environment.GetCommandLineArgs();
            if(args.Length > 1) {
                arg = args[1];
            }

            if(!string.IsNullOrEmpty(arg)) {
                try {
                    var val = float.Parse(arg) / 100;
                    if(val > 100 || val < 0) {
                        throw new Exception("Out of range");
                    }

                    configureOverlays(val);
                } catch(Exception) {
                    MessageBox.Show("Expecting number from 0 to 100 to represent percentage of dimming. 0 means no change, 100 being totally dark.");
                    configureOverlays(0);
                }
            } else {
                configureOverlays(0);
            }
        }

        private void menuExit_Click(object sender, EventArgs e) {
            clearOverlays();
            Close();
        }

        private void Restart() {
            var exePath = Application.ExecutablePath;
            System.Diagnostics.Process.Start(exePath, (overlays[0].Dimness * 100).ToString());
            Application.Exit();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e) {
            contextMenuStrip1.Show();
        }

        private void dimnessBar_Scroll(object sender, EventArgs e) {
            ChangeDimness();

            contextMenuStrip1.Focus();
        }

        private void ChangeDimness() {
            dimnessLbl.Text = $"Dimness: {dimnessBar.Value}%";

            for(int i = 0; i < overlays.Length; ++i) {
                if(activeMonitors[i]) {
                    overlays[i].Dim(dimnessBar.Value);
                } else {
                    overlays[i].Dim(0.0f);
                }
            }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e) {
            var txt = toolStripTextBox1.Text;
            if(int.TryParse(txt, out int value) && dimnessBar.HasRange(value)) {
                dimnessBar.Value = value;
                ChangeDimness();
            }
        }

        private void toolStripTextBox2_TextChanged(object sender, EventArgs e) {
            var txt = toolStripTextBox2.Text.TrimStart('#').Trim();
            if(txt.Length > 6) { return; }

            Color color = Color.Black;

            if(Regex.Match(txt, "^[a-fA-F0-9]{1,6}$").Success) {
                color = ColorTranslator.FromHtml($"#{txt.PadRight(6, '0')}");
            } else {
                var tmp = Color.FromName(txt);
                if(tmp.IsKnownColor) {
                    color = tmp;
                }
            }

            foreach(frmMain form in overlays) {
                form.BackColor = color;
            }
        }
    }
}