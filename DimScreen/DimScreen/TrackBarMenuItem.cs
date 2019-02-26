using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace DimScreen {
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ContextMenuStrip)]
    [DefaultEvent("Scroll")]
    [DefaultBindingProperty("Value")]
    [DefaultProperty("Value")]
    public class TrackBarMenuItem : ToolStripControlHost {
        private TrackBar trackBar;

        [Bindable(true)]
        [DefaultValue(0)]
        public int Value { get => trackBar.Value; set => trackBar.Value = value; }


        public int Minimum { get => trackBar.Minimum; }
        public int Maximum { get => trackBar.Maximum; }
        public bool HasRange(int value) {
            return value >= Minimum && value <= Maximum;
        }

        public TrackBarMenuItem() : base(new TrackBar()) {
            trackBar = Control as TrackBar;
            this.AutoSize = false;

            trackBar.AutoSize = false;
            trackBar.SetRange(0, 95);
            trackBar.TickFrequency = 1;
            trackBar.TickStyle = TickStyle.None;
            
            trackBar.Size = new Size(104, 20);
            this.Size = trackBar.Size;

            trackBar.BackColor = DarkMenuColorTable.backgroundColor;

            Scroll += OnScroll;
        }
        
        public event EventHandler Scroll {
            add { trackBar.Scroll += value; }
            remove { trackBar.Scroll -= value; }
        }
        
        protected virtual void OnScroll(object sender, EventArgs e) {
        }
    }
}