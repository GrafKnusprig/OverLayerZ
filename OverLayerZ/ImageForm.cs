using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OverLayerZ
{
    public partial class ImageForm : Form
    {
        private byte m_opacity;

        public Image Image { get; set; }

        public byte ImageOpacity
        {
            get
            {
                return m_opacity;
            }
            set
            {
                SetLayeredWindowAttributes(this.Handle, 0, value, LWA.Alpha);
                m_opacity = value;
            }
        }

        #region click through and transparency https://stackoverflow.com/a/1524047
        public enum GWL
        {
            ExStyle = -20
        }

        public enum WS_EX
        {
            Transparent = 0x20,
            Layered = 0x80000
        }

        public enum LWA
        {
            ColorKey = 0x1,
            Alpha = 0x2
        }

        public ImageForm()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong(IntPtr hWnd, GWL nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte alpha, LWA dwFlags);

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            int wl = GetWindowLong(this.Handle, GWL.ExStyle);
            wl = wl | (int)WS_EX.Layered | (int)WS_EX.Transparent;
            SetWindowLong(this.Handle, GWL.ExStyle, wl);
            SetLayeredWindowAttributes(this.Handle, 0, m_opacity, LWA.Alpha);
        }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(Image, 0, 0, Width, Height);
        }
    }
}
