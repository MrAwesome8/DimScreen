using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DimScreen {

    public class DarkMenu : ToolStripProfessionalRenderer {

        public DarkMenu(ProfessionalColorTable professionalColorTable) : base(professionalColorTable) { }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
            base.OnRenderMenuItemBackground(e);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e) {
            e.TextColor = Color.WhiteSmoke;

            base.OnRenderItemText(e);
        }

    }

    public class DarkMenuColorTable : ProfessionalColorTable {
        private const string backgroundColorHex = "#404040";
        private const string selectedColorHex = "#4C4A48";

        public static readonly Color backgroundColor = ColorTranslator.FromHtml(backgroundColorHex);
        public static readonly Color selectedColor = ColorTranslator.FromHtml(selectedColorHex);

        public override Color ToolStripDropDownBackground => backgroundColor;

        public override Color ImageMarginGradientBegin => backgroundColor;

        public override Color ImageMarginGradientMiddle => backgroundColor;

        public override Color ImageMarginGradientEnd => backgroundColor;

        public override Color MenuBorder => Color.Black;

        public override Color MenuItemBorder => Color.Black;

        public override Color MenuItemSelected => selectedColor;

        public override Color MenuStripGradientBegin => backgroundColor;

        public override Color MenuStripGradientEnd => backgroundColor;

        public override Color MenuItemSelectedGradientBegin => selectedColor;

        public override Color MenuItemSelectedGradientEnd => selectedColor;

        public override Color MenuItemPressedGradientBegin => backgroundColor;

        public override Color MenuItemPressedGradientEnd => backgroundColor;
    }
}
