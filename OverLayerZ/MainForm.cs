using System;
using System.Drawing;
using System.Windows.Forms;

namespace OverLayerZ
{
    public partial class MainForm : Form
    {
        private ImageForm m_imageForm;
        private bool m_imageShown;
        private Mode m_mode;
        private Timer m_resizeTimer;
        private Point m_cursorPosition;

        private enum Mode
        {
            Positioning = 0,
            Resizing = 1
        }

        public MainForm()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            m_imageForm = new ImageForm();
            m_imageForm.TopMost = true;
            m_imageForm.FormBorderStyle = FormBorderStyle.None;

            m_cursorPosition = new Point(0, 0);
            m_mode = Mode.Positioning;
            m_imageShown = false;

            m_resizeTimer = new Timer();
            m_resizeTimer.Interval = 100;
            m_resizeTimer.Tick += ResizeImageForm;

            tbOpacity.ValueChanged += OnOpacityChanged;
            btnPosition.Click += OnPositionClick;
            btnSize.Click += OnSizeClick;
            btnOriginal.Click += OnOriginalRatioClick;
            btnOpenFile.Click += OnOpenFileClick;
            btnShow.Click += OnShowClick;

            MouseHook.MouseClickAction += OnMouseClickAction;
            MouseHook.MouseMoveAction += OnMouseMoveAction;
        }

        private void SetImage()
        {
            try
            {
                m_imageForm.Image = Image.FromFile(tbPath.Text);
            }
            catch
            {
                Bitmap bmp = new Bitmap(600, 400);
                RectangleF rectf = new RectangleF(5, 150, 595, 100);
                Graphics g = Graphics.FromImage(bmp);
                g.DrawString("No image loaded...", new Font("Tahoma", 50), Brushes.Red, rectf);
                g.Flush();

                m_imageForm.Image = bmp;
            }
            m_imageForm.ImageOpacity = Convert.ToByte(tbOpacity.Value);
            m_imageForm.Width = m_imageForm.Image.Width * m_imageForm.Height / m_imageForm.Image.Height;
        }

        private void ResizeImageForm(object sender, EventArgs e)
        {
            m_imageForm.Size = new Size(m_cursorPosition.X - m_imageForm.Location.X, m_cursorPosition.Y - m_imageForm.Location.Y);
        }

        private void OnOpenFileClick(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                dialog.Title = @"Open Image";
                dialog.Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp";
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    tbPath.Text = dialog.FileName;
                    SetImage();
                }
            }
        }

        private void OnOriginalRatioClick(object sender, EventArgs e)
        {
            if (!m_imageShown)
                return;
            m_imageForm.Width = m_imageForm.Image.Width * m_imageForm.Height / m_imageForm.Image.Height;
        }

        private void OnSizeClick(object sender, EventArgs e)
        {
            if (!m_imageShown)
                return;
            Cursor.Position = new Point(m_imageForm.Location.X + m_imageForm.Width, m_imageForm.Location.Y + m_imageForm.Height);
            m_cursorPosition = Cursor.Position;
            m_mode = Mode.Resizing;
            MouseHook.Start();
            m_resizeTimer.Start();
        }

        private void OnPositionClick(object sender, EventArgs e)
        {
            if (!m_imageShown)
                return;
            Cursor.Position = m_imageForm.Location;
            m_mode = Mode.Positioning;
            MouseHook.Start();
        }

        private void OnMouseMoveAction(object sender, EventArgs e)
        {
            if (m_mode == Mode.Positioning)
                m_imageForm.Location = Cursor.Position;
            else if (m_mode == Mode.Resizing)
                m_cursorPosition = Cursor.Position;
        }

        private void OnMouseClickAction(object sender, EventArgs e)
        {
            MouseHook.Stop();
            m_resizeTimer.Stop();
        }

        private void OnOpacityChanged(object sender, EventArgs e)
        {
            if (!m_imageShown)
                return;
            m_imageForm.ImageOpacity = Convert.ToByte(tbOpacity.Value);
        }

        private void OnShowClick(object sender, EventArgs e)
        {
            if (!m_imageShown)
            {
                SetImage();
                m_imageShown = true;
                m_imageForm.Show();
                btnShow.Text = "hide";
            }
            else
            {
                m_imageShown = false;
                m_imageForm.Hide();
                btnShow.Text = "show";
            }
        }
    }
}
