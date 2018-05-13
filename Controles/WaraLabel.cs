using System.ComponentModel;
using System.Windows.Forms;

namespace WaraUi.Controls
{
    public class WaraLabel : Label, IWaraControl
    {
        [Browsable(false)]
        public int Depth { get; set; }
        [Browsable(false)]
        public WaraSkinManager SkinManager { get { return WaraSkinManager.Instance; } }
        [Browsable(false)]
        public MouseState MouseState { get; set; }
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            ForeColor = SkinManager.GetPrimaryTextColor();
            Font = SkinManager.ROBOTO_REGULAR_11;

            BackColorChanged += (sender, args) => ForeColor = SkinManager.GetPrimaryTextColor();
        }
    }
}
