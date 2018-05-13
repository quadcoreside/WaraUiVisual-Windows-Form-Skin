using System.ComponentModel;
using System.Windows.Forms;

namespace WaraUi.Controls
{
    public sealed class WaraDivider : Control, IWaraControl
    {
        [Browsable(false)]
        public int Depth { get; set; }
        [Browsable(false)]
        public WaraSkinManager SkinManager { get { return WaraSkinManager.Instance; } }
        [Browsable(false)]
        public MouseState MouseState { get; set; }
        
        public WaraDivider()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            Height = 1;
            BackColor = SkinManager.GetDividersColor();
        }
    }
}
