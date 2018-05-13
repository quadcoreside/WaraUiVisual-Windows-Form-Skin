using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace WaraUi.Controls
{
    public class WaraTabControl : TabControl, IWaraControl
    {
        [Browsable(false)]
        public int Depth { get; set; }
        [Browsable(false)]
        public WaraSkinManager SkinManager { get { return WaraSkinManager.Instance; } }
        [Browsable(false)]
        public MouseState MouseState { get; set; }
        
		protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x1328 && !DesignMode) m.Result = (IntPtr)1;
            else base.WndProc(ref m);
        }
    }
}
