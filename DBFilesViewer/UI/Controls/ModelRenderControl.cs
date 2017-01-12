using System;
using System.Windows.Forms;

namespace DBFilesViewer.UI.Controls
{
    public partial class ModelRenderControl : UserControl
    {
        public event Action OnRenderFrame;
        public event Action<int> OnVerticalScroll;
        public new event Action OnKeyDown;

        public int Interval
        {
            get { return timer1.Interval; }
            set { timer1.Interval = value; }
        }

        public ModelRenderControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.Selectable, true);
            InitializeComponent();

            timer1.Interval = 10;
            timer1.Tick += (sender, args) => OnRenderFrame?.Invoke();
            timer1.Start();
        }

        protected override void OnClick(EventArgs e)
        {
            Focus();
            base.OnClick(e);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x20A: // WM_MOUSEWHEEL 
                {
                    OnVerticalScroll?.Invoke((m.WParam.ToInt32() >> 16) / 120);
                    break;
                }
                case 0x100: // WM_KEYDOWN
                {
                    OnKeyDown?.Invoke();
                    break;
                }
            }

            base.WndProc(ref m);
        }
    }
}
