
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ChameDrawer.Properties;
using GemBox.Pdf;
using GemBox.Pdf.Content;

namespace ChameDrawer
{
    public partial class MainForm : Form
    {
        private Point _mesh_center;
        private int _cel_size;
        private bool _middle_is_pressed;
        private Point _last_point;
        private Color _back_color;
        private int _border_width;

        private Size _mesh_size;
        private Image _selection_layer, _top_layer, _bottom_layer;
        private Point _hover_point;
        private Image _selection_icon;
        private Image _transparent_eraser;
        private Image _lightGray_eraser;

        private Image _left_numerick, _top_numerick;

        private string _file_name;

        private PointF MouseCanvasPosition => ScreenPoint_To_CanvasPoint(MousePosition);

        private PictureBox[] Icons_PB_List;
        private PictureBox _selected;
        private PictureBox Selected_PB { get => _selected; set { if (_selected != null) _selected.BackColor = Color.LightGray; _selected = value; value.BackColor = Color.SkyBlue; } }

        private bool _left_is_pressed;
        private Point _start_pos, _end_pos;
        private Rectangle _selection;

        private PointF ScreenPoint_To_CanvasPoint(Point screen_point) => new PointF(PointToClient(screen_point).X - Main_Panel.Location.X - Drawing_PBox.Location.X - _mesh_center.X, PointToClient(screen_point).Y - Main_Panel.Location.Y - Drawing_PBox.Location.Y - _mesh_center.Y);

        private Bitmap _bufer_top;
        private Bitmap _bufer_bott;

        private int _min_x, _min_y, _max_x, _max_y;

        public MainForm()
        {
            InitializeComponent();

            _middle_is_pressed = false;
            _left_is_pressed = false;
            _last_point = Point.Empty;
            _start_pos = Point.Empty;
            _end_pos = Point.Empty;
            _selection = Rectangle.Empty;


            _mesh_size = new Size(55, 40);
            _mesh_center = new Point(0, 0);
            _cel_size = 50;
            _border_width = 1;

            _max_x = 0;
            _max_y = 0;
            _min_x = -_mesh_size.Width * (_cel_size + _border_width);
            _min_y = -_mesh_size.Height * (_cel_size + _border_width);

            _back_color = Color.LightGray;
            Clear_Layers(_mesh_size.Width, _mesh_size.Height);
            _selection_icon = new Bitmap(_cel_size, _cel_size);
            Graphics.FromImage(_selection_icon).Clear(Color.FromArgb(100, 100, 100, 0));

            _transparent_eraser = new Bitmap(_cel_size, _cel_size);
            Graphics.FromImage(_transparent_eraser).Clear(Color.Transparent);

            _lightGray_eraser = new Bitmap(_cel_size, _cel_size);
            Graphics.FromImage(_lightGray_eraser).Clear(Color.LightGray);

            Color_PB.Image = new Bitmap(_cel_size, _cel_size);
            Graphics.FromImage(Color_PB.Image).Clear(Color.LightGray);

            _file_name = "";
            Icons_PB_List = new PictureBox[]
            {
                pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8, pictureBox9,
                pictureBox10, pictureBox11, pictureBox12, pictureBox13, pictureBox14, pictureBox15, pictureBox16, pictureBox17, pictureBox18, pictureBox19, pictureBox20,
                pictureBox21, pictureBox22, pictureBox23, pictureBox24, pictureBox25, pictureBox26, pictureBox27
            };
            Selected_PB = Icons_PB_List[0];

            LeftNumberic_PB.Image = new Bitmap(LeftNumberic_PB.Width, LeftNumberic_PB.Height);
            TopNumeric_PB.Image = new Bitmap(TopNumeric_PB.Width, TopNumeric_PB.Height);
            TopNumeric_PB.Padding = new Padding(LeftNumberic_PB.Width, 0, 0, 0);

            Draw_Numerick();
            Draw_Icons();
        }

        private void Draw_Numerick()
        {
            _left_numerick = new Bitmap(LeftNumberic_PB.Width + _border_width, _mesh_size.Height * (_cel_size + _border_width) + _border_width);
            _top_numerick = new Bitmap(_mesh_size.Width * (_cel_size + _border_width) + _border_width, TopNumeric_PB.Height);

            int font_size = 15;
            SolidBrush b = new SolidBrush(Color.Black);
            Pen p = new Pen(Color.Black, _border_width);
            Font f = new Font(FontFamily.GenericSansSerif, font_size);

            using (var drawer = Graphics.FromImage(_left_numerick))
            {
                drawer.Clear(Color.White);
                drawer.DrawLine(p, new Point(0, 0), new Point(0, _left_numerick.Height));
                int y = 0;
                for (int row = 0; row < _mesh_size.Height; row++)
                {
                    y = row * (_cel_size + _border_width);
                    string numerick = $"{row + 1}";
                    drawer.DrawString(numerick, f, b, new PointF((_left_numerick.Width - font_size * numerick.Length) / 2, y + (_cel_size - font_size) / 3));
                    drawer.DrawLine(p, new Point(0, y), new Point(_left_numerick.Width, y));
                }
                y += _cel_size + _border_width;
                drawer.DrawLine(p, new Point(0, y), new Point(_left_numerick.Width, y));
            }
            using (var drawer = Graphics.FromImage(_top_numerick))
            {
                drawer.Clear(Color.White);
                drawer.DrawLine(p, new Point(0, 0), new Point(_top_numerick.Width, 0));
                int x = 0;
                for (int col = 0; col < _mesh_size.Width; col++)
                {
                    x = col * (_cel_size + _border_width);
                    string numerick = $"{col + 1}";
                    drawer.DrawString(numerick, f, b, new PointF(x + (_cel_size - font_size * numerick.Length) / 2, (_top_numerick.Height - font_size)/3));
                    drawer.DrawLine(p, new Point(x, 0), new Point(x, _top_numerick.Height));
                }
                x += _cel_size + _border_width;
                drawer.DrawLine(p, new Point(x, 0), new Point(x, _top_numerick.Height));
            }
        }

        private void Draw_Icons()
        {
            Bitmap[] icons = new Bitmap[]
            {
                Resources.Нет_витка,
                Resources._2_Вместе_изнаночной,
                Resources._2_Вместе_лицевой,
                Resources._2_Вместе_лицевой_скрещенной,
                Resources._2_Вместе_протяжкой,
                Resources._3_Вместе_изнаночной,
                Resources._3_Вместе_лицевой,
                Resources.Изнаночная_петля,
                Resources.Изнаночная_скрещенная_петля,
                Resources.Лицевая_петля,
                Resources.Лицевая_скрещенная_петля,
                Resources.Накид,
                Resources.Несколько_накидов,
                Resources.Обхватывающая_петля,
                Resources.Петля_платочной_вязки,
                Resources.Сбросить_накиды,
                Resources.Снятая_с_2_накидом,
                Resources.Снятая_с_3_накидами,
                Resources.Снять_2__как_изнаночную,
                Resources.Снять_2_как_лицевую,
                Resources.Снять_3__как_изнаночную,
                Resources.Снять_3_как_лицевую,
                Resources.Снять_как_изнаночную_за_работой,
                Resources.Снять_как_изнаночную_перед_работой,
                Resources.Снять_как_лицевую_за_работой,
                Resources.Снять_как_лицевую_перед_работой,
                Resources.Шишечка
            };
            for (int i = 0; i < icons.Length; i++)
            {
                Icons_PB_List[i].Image = icons[i];
                Icons_PB_List[i].SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void Clear_Layers(int rows, int cols)
        {
            Size mesh_size = new Size(rows * (_cel_size + _border_width) + _border_width, cols * (_cel_size + _border_width) + _border_width);
            _top_layer = new Bitmap(mesh_size.Width, mesh_size.Height);
            _bottom_layer = new Bitmap(mesh_size.Width, mesh_size.Height);
            _selection_layer = new Bitmap(mesh_size.Width, mesh_size.Height);

            Graphics.FromImage(_selection_layer).Clear(Color.Transparent);
            Graphics.FromImage(_top_layer).Clear(Color.Transparent);
            Graphics.FromImage(_bottom_layer).Clear(_back_color);

            Draw_Mesh(_bottom_layer);
        }

        private void Draw_Mesh(Image layer)
        {
            using (var drawer = Graphics.FromImage(layer))
            {
                int rows_count = layer.Height / _cel_size + 1;
                int columns_count = layer.Width / _cel_size + 1;
                Pen p = new Pen(Color.Black, _border_width);
                Brush b = new SolidBrush(Color.Red);

                for (int row = 0; row < rows_count; row++)
                {
                    int y = row * (_cel_size + _border_width) + _border_width / 2;
                    drawer.DrawLine(p, new Point(0, y), new Point(layer.Width, y));
                }
                for (int column = 0; column < columns_count; column++)
                {
                    int x = column * (_cel_size + _border_width) + _border_width / 2;
                    drawer.DrawLine(p, new Point(x, 0), new Point(x, layer.Height));
                }
            }
        }

        private void UpdateDrawer(object sender = null, EventArgs e = null)
        {
            Drawing_PBox.Image = new Bitmap(Drawing_PBox.Width, Drawing_PBox.Height);

            if (_mesh_center.X > _max_x)
                _mesh_center.X = _max_x;
            else if(_mesh_center.X < _min_x + Drawing_PBox.Width - _border_width)
                _mesh_center.X = _min_x + Drawing_PBox.Width - _border_width;

            if (_mesh_center.Y > _max_y)
                _mesh_center.Y = _max_y;
            else if (_mesh_center.Y < _min_y + Drawing_PBox.Height - _border_width)
                _mesh_center.Y = _min_y + Drawing_PBox.Height - _border_width;

            using (var selection_drawer = Graphics.FromImage(_selection_layer))
                DrawSelection(selection_drawer);

            using (var result_drawer = Graphics.FromImage(Drawing_PBox.Image))
                DrawLayers(result_drawer);

            LeftNumberic_PB.Image = new Bitmap(LeftNumberic_PB.Width, LeftNumberic_PB.Height);
            TopNumeric_PB.Image = new Bitmap(TopNumeric_PB.Width, TopNumeric_PB.Height);
            DrawNumerick();
        }

        private void DrawNumerick()
        {
            using (var drawer = Graphics.FromImage(LeftNumberic_PB.Image))
            {
                drawer.Clear(Color.White);
                drawer.DrawImage(_left_numerick, 0, _mesh_center.Y);
            }
            using (var drawer = Graphics.FromImage(TopNumeric_PB.Image))
            {
                drawer.Clear(Color.White);
                drawer.DrawImage(_top_numerick, _mesh_center.X, 0);
            }
            LeftNumberic_PB.Image = LeftNumberic_PB.Image;
            TopNumeric_PB.Image = TopNumeric_PB.Image;
        }

        private void DrawSelection(Graphics drawer)
        {
            drawer.Clear(Color.Transparent);
            Fill_Cells(drawer, _selection_icon);
        }

        private void DrawLayers(Graphics drawer)
        {
            drawer.DrawImage(_bottom_layer, _mesh_center.X, _mesh_center.Y);
            drawer.DrawImage(_top_layer, _mesh_center.X, _mesh_center.Y);
            drawer.DrawImage(_selection_layer, _mesh_center.X, _mesh_center.Y);
        }

        private void Fill_Cells(Graphics drawer, Image filler)
        {
            for (int col = _selection.X; col < _selection.X + _selection.Width+1; col++)
            {
                for (int row = _selection.Y; row < _selection.Y + _selection.Height+1; row++)
                {
                    Point cel_start = CelStartPos(col, row);
                    drawer.DrawImage(filler, cel_start.X, cel_start.Y, _cel_size, _cel_size);
                }
            }
        }

        private void Drawing_PBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                _middle_is_pressed = true;
                _last_point = e.Location;
            }
            else
            if (e.Button == MouseButtons.Left)
            {
                _left_is_pressed = true;
                _start_pos = MousePosition;
            }
            else
            if (e.Button == MouseButtons.Right)
            {
                Graphics.FromImage(_bottom_layer).DrawImage(Color_PB.Image, _hover_point.X, _hover_point.Y, _cel_size, _cel_size);
                UpdateDrawer();
            }
        }

        private void Drawing_PBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
                _middle_is_pressed = false;

            if (e.Button == MouseButtons.Left)
            {
                _left_is_pressed = false;
                _end_pos = MousePosition;
                SelectArea(Canvas_Point_ToCelPos(ScreenPoint_To_CanvasPoint(_start_pos)), Canvas_Point_ToCelPos(ScreenPoint_To_CanvasPoint(_end_pos)));
                if (_selection.Width == 0 && _selection.Height == 0)
                    using (var drawer = Graphics.FromImage(_top_layer))
                    {
                        drawer.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                        drawer.DrawImage(Selected_PB.Image, _hover_point.X, _hover_point.Y, _cel_size, _cel_size);
                        UpdateDrawer();
                    }
            }

        }

        private void Drawing_PBox_MouseLeave(object sender, EventArgs e)
        {
            _middle_is_pressed = false;
            _left_is_pressed = false;
        }

        private void Drawing_PBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (_middle_is_pressed)
            {
                SlideView(e);
            }
            else if (_left_is_pressed)
            {
                _end_pos = MousePosition;
                SelectArea(Canvas_Point_ToCelPos(ScreenPoint_To_CanvasPoint(_start_pos)), Canvas_Point_ToCelPos(ScreenPoint_To_CanvasPoint(_end_pos)));
            }
            else
            {
                Point cel = Canvas_Point_ToCelPos(MouseCanvasPosition);
                if (cel.X >= 0 && cel.X < _mesh_size.Width
                && cel.Y >= 0 && cel.Y < _mesh_size.Height)
                {
                    Point floating_pos = CelStartPos(cel.X, cel.Y);
                    if (floating_pos != _hover_point)
                    {
                        _hover_point = floating_pos;
                        UpdateDrawer();
                    }
                }
            }

        }

        private Point Canvas_Point_ToCelPos(PointF pos)
        {
            int row = (int)(pos.X / (_cel_size + _border_width));
            int col = (int)(pos.Y / (_cel_size + _border_width));
            return new Point(row, col);
        }
        private Point CelStartPos(int col, int row) => new Point(col * (_cel_size + _border_width) + _border_width, row * (_cel_size + _border_width) + _border_width);

        private void SelectArea(Point start_cel, Point end_cel)
        {
            if (start_cel.X > end_cel.X)
            {
                var x = start_cel.X;
                start_cel.X = end_cel.X;
                end_cel.X = x;
            }
            if (start_cel.Y >end_cel.Y)
            {
                var y = start_cel.Y;
                start_cel.Y = end_cel.Y;
                end_cel.Y = y;
            }
            _selection = new Rectangle(start_cel.X, start_cel.Y, end_cel.X - start_cel.X, end_cel.Y - start_cel.Y);
            UpdateDrawer();
        }

        private void SlideView(MouseEventArgs e)
        {
            int x_delta = _last_point.X - e.Location.X;
            int y_delta = _last_point.Y - e.Location.Y;
            double distance = Math.Sqrt(x_delta * x_delta + y_delta * y_delta);
            if (distance > 50)
            {
                _mesh_center = new Point(_mesh_center.X - x_delta, _mesh_center.Y - y_delta);
                _last_point = e.Location;
                UpdateDrawer();
            }
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            UpdateDrawer();
        }

        private void Drawing_PBox_MouseHover(object sender, EventArgs e)
        {
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clear_Layers(_mesh_size.Width, _mesh_size.Height);
            UpdateDrawer();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = _file_name;
            openFileDialog1.Filter = "Точечные рисунки|*.bmp|Все файлы|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _file_name = openFileDialog1.FileName.Split('.')[0];
                _top_layer = Bitmap.FromFile(_file_name + ".top.bmp");
                _bottom_layer = Bitmap.FromFile(_file_name + ".bottom.bmp");
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_file_name == "")
            {
                saveAsToolStripMenuItem_Click(sender, e);
                return;
            }
            _top_layer.Save(_file_name + ".top.bmp");
            _bottom_layer.Save(_file_name + ".bottom.bmp");
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = _file_name;
            saveFileDialog1.Filter = "Точечные рисунки|*.bmp|Все файлы|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _file_name = saveFileDialog1.FileName.Split('.')[0];
                saveToolStripMenuItem_Click(sender, e);
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = _file_name;
            saveFileDialog1.Filter = "PDF файлы|*.pdf|Все файлы|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string file_name = saveFileDialog1.FileName.Split('.')[0];
                ComponentInfo.SetLicense("FREE-LIMITED-KEY");

                string temp_file_name = "temp.bmp";
                Bitmap result = new Bitmap(_bottom_layer.Width + _left_numerick.Width, _bottom_layer.Height + _top_numerick.Height);
                using (var drawer = Graphics.FromImage(result))
                {
                    drawer.DrawImage(_top_numerick, new Point(_left_numerick.Width, 0));
                    drawer.DrawImage(_left_numerick, new Point(0, _top_numerick.Height));
                    drawer.DrawImage(_bottom_layer, new Point(_left_numerick.Width, _top_numerick.Height));
                    drawer.DrawImage(_top_layer, new Point(_left_numerick.Width, _top_numerick.Height));
                }
                result.RotateFlip(RotateFlipType.Rotate270FlipNone);
                result.Save(temp_file_name);

                var doc = new PdfDocument();
                var page = doc.Pages.Add();

                double pos_x, pos_y, size_x, size_y;
                double delta_width = result.Width / page.Size.Width;
                double delta_height = result.Height / page.Size.Height;
                double max_delta = delta_height > delta_width ? delta_height : delta_width;

                size_x = result.Width / max_delta;
                size_y = result.Height / max_delta;
                pos_x = (page.Size.Width - size_x) / 2;
                pos_y = (page.Size.Height - size_y) / 2;

                page.Rotate = 90;
                page.Content.DrawImage(PdfImage.Load(temp_file_name), new PdfPoint(pos_x, pos_y), new PdfSize(size_x, size_y));
                doc.Save(file_name + ".pdf");
                doc.Close();
                File.Delete(temp_file_name);
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyToolStripMenuItem_Click(sender, e);
            clearBack_ToolStripMenuItem_Click(sender, e);
            clearFront_ToolStripMenuItem_Click(sender, e);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _bufer_top = new Bitmap((_selection.Width + 1) * (_cel_size + _border_width), (_selection.Height + 1) * (_cel_size + _border_width));
            _bufer_bott = new Bitmap((_selection.Width + 1) * (_cel_size + _border_width), (_selection.Height + 1) * (_cel_size + _border_width));
            using (var drawer = Graphics.FromImage(_bufer_top))
            {
                drawer.Clear(Color.Transparent);
                drawer.DrawImage(_top_layer, -_selection.X * (_cel_size + _border_width), -_selection.Y * (_cel_size + _border_width));
            }
            using (var drawer = Graphics.FromImage(_bufer_bott))
            {
                drawer.Clear(Color.Transparent);
                drawer.DrawImage(_bottom_layer, -_selection.X * (_cel_size + _border_width), -_selection.Y * (_cel_size + _border_width));
            }

        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var drawer = Graphics.FromImage(_top_layer))
            {
                SelectArea(_selection.Location, new Point(_selection.Location.X + _bufer_top.Width / (_cel_size + _border_width) - 1, _selection.Location.Y + _bufer_top.Height / (_cel_size + _border_width) - 1));
                drawer.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                Fill_Cells(drawer, _transparent_eraser);
                drawer.DrawImage(_bufer_top, _selection.X * (_cel_size + _border_width), _selection.Y * (_cel_size + _border_width));
            }
            using (var drawer = Graphics.FromImage(_bottom_layer))
            {
                drawer.DrawImage(_bufer_bott, _selection.X * (_cel_size + _border_width), _selection.Y * (_cel_size + _border_width));
            }
            UpdateDrawer();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectArea(new Point(0, 0), new Point(_mesh_size.Width, _mesh_size.Height));
        }

        private void Color_PB_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Graphics.FromImage(Color_PB.Image).Clear(colorDialog1.Color);
                Color_PB.Image = Color_PB.Image;
            }
        }

        private void clearBack_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var drawer = Graphics.FromImage(_bottom_layer))
            {
                Fill_Cells(drawer, _lightGray_eraser);
                UpdateDrawer();
            }
        }

        private void fill_Top_toolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var drawer = Graphics.FromImage(_top_layer))
            {
                drawer.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                Fill_Cells(drawer, Selected_PB.Image);
                UpdateDrawer();
            }
        }

        private void fillBack_toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (var drawer = Graphics.FromImage(_bottom_layer))
            {
                drawer.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                Fill_Cells(drawer, Color_PB.Image);
                UpdateDrawer();
            }
        }

        private void clearFront_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var drawer = Graphics.FromImage(_top_layer))
            {
                Brush front_eraser = new SolidBrush(Color.Transparent);
                drawer.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                Fill_Cells(drawer, _transparent_eraser);
                UpdateDrawer();
            }
        }

        private void Icon_PB_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox icon_pb)
            {
                Selected_PB = icon_pb;
            }
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
