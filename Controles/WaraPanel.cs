using WaraUi.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WaraUi.Controls
{
    public class WaraPanel : Panel, IWaraControl
    {
        [Browsable(false)]
        public int Depth { get; set; }
        [Browsable(false)]
        public WaraSkinManager SkinManager { get { return WaraSkinManager.Instance; } }
        [Browsable(false)]
        public MouseState MouseState { get; set; }

        private Font font;
        public WaraPanel()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);
            Height = 200;
            Width = 200;
            font = new Font(SkinManager.LoadFont(Resources.Roboto_Medium), 12f);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(SkinManager.ColorScheme.PrimaryColor);
            BackColor = SkinManager.ColorScheme.PrimaryColor;
            ForeColor = SkinManager.ColorScheme.TextColor;
        }

    }
}
