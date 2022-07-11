using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChameDrawer
{
    public partial class MainForm : Form
    {
        private Graphics _drawer;
        private Point _center;
        private int _zoom;
        private int _default_cell_size;
        private int _zoom_step;
        private int _zoom_max;
        private int _zoom_min;
        private bool _middle_is_down;
        private Point _last_point;
        
        public MainForm()
        {
            InitializeComponent();
            _center = new Point(Width / 2, Height / 2);
            _zoom = 100;
            _zoom_step = 20;
            _zoom_max = 200;
            _zoom_min = 20;
            _default_cell_size = 50;
            Drawing_PBox.MouseWheel += ZoomScroll;
            _middle_is_down = false;
            _last_point = Point.Empty;
        }

        private void ZoomScroll(object sender, MouseEventArgs e)
        {
            _zoom += e.Delta > 0 ? _zoom_step : -_zoom_step;
            if (_zoom < _zoom_min)
                _zoom = _zoom_min;
            if (_zoom > _zoom_max)
                _zoom = _zoom_max;

            UpdateDrawer();
        }

        private void Draw_Mesh()
        {
            int cel_size = (int)(_default_cell_size * (_zoom / 100.0));
            if (cel_size == 0)
                cel_size = 1;
            int rows_count = Height / cel_size + 1;
            int columns_count = Width / cel_size + 1;
            Point padding = new Point(_center.X % cel_size, _center.Y % cel_size);
            Pen p = new Pen(Color.Black);
            Brush b = new SolidBrush(Color.Red);

            for (int row = 0; row < rows_count; row++)
            {
                int y = row * cel_size + padding.Y;
                _drawer.DrawLine(p, new Point(0, y), new Point(Width, y));
            }
            for (int column = 0; column < columns_count; column++)
            {
                int x = column * cel_size + padding.X;
                _drawer.DrawLine(p, new Point(x, 0), new Point(x, Height));
            }
            int center_dot_size = (int)(5 * _zoom / 100.0);
            _drawer.FillEllipse(b, new Rectangle(_center.X-center_dot_size, _center.Y- center_dot_size, center_dot_size*2, center_dot_size*2));
        }

        private void UpdateDrawer(object sender = null, EventArgs e = null)
        {
            Drawing_PBox.Image = new Bitmap(Width, Height);
            _drawer = Graphics.FromImage(Drawing_PBox.Image);
            _drawer.Clear(Color.White);
            Draw_Mesh();
        }

        private void Drawing_PBox_MouseHover(object sender, EventArgs e)
        {
        }

        private void Drawing_PBox_MouseDown(object sender, MouseEventArgs e)
        {
            _middle_is_down = true;
            _last_point = e.Location;
        }

        private void Drawing_PBox_MouseUp(object sender, MouseEventArgs e)
        {
            _middle_is_down = false;
        }

        private void Drawing_PBox_MouseLeave(object sender, EventArgs e)
        {
            _middle_is_down = false;
        }

        private void Drawing_PBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (_middle_is_down)
            {
                int x_delta = _last_point.X - e.Location.X;
                int y_delta = _last_point.Y - e.Location.Y;
                _center = new Point(_center.X - x_delta, _center.Y - y_delta);
                _last_point = e.Location;
                UpdateDrawer();
            }
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            UpdateDrawer();
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0112) // WM_SYSCOMMAND
            {
                // Check your window state here
                if (m.WParam == new IntPtr(0xF030)) // Maximize event - SC_MAXIMIZE from Winuser.h
                {
                    // THe window is being maximized
                    base.WndProc(ref m);
                    UpdateDrawer();
                    return;
                }
            }
            base.WndProc(ref m);
        }
    }
}
