using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Numerics;
using System.Windows.Forms;

namespace FormsLearning
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Color[] BWColors =
        {
            Color.Black, Color.LightGray, Color.DarkGray, Color.White
        };
        private byte[] NES_Signature = { 0x4e, 0x45, 0x53, 0x1a };
        private int color;
        private CHRFile[] files = new CHRFile[256];
        bool dontinitCHR;
        private class CHRFile
        {
            public Bitmap m_bitmap = new Bitmap(128, 128, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            public Bitmap m_selected_bitmap = new Bitmap(8, 8, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            public string m_filename;
            public int m_offset;
            public int m_previous_offset;
            public byte[] file_data;
            public byte[] chr_data;
            public int m_selected_tile;
            public int m_sel_x;
            public int m_sel_y;
            public int m_sel_prev_x;
            public int m_sel_prev_y;
            public int m_graphically_x;
            public int m_graphically_y;
        }
        private CHRFile get_chr_file()
        {
            //When I need a CHR file, return it with the proper index
            return files[tabcontrol.SelectedIndex];
        }
        private CHRFile make_chr_file()
        {
            //Disable the tabcontrol_selected control 
            dontinitCHR = true;

            //Make a new tab
            TabPage tab = new TabPage();
            tab.MouseClick += panel_Mouse;
            tab.MouseMove += panel_Mouse;
            tab.MouseWheel += panel_Scroll;
            tabcontrol.TabPages.Add(tab);
            tabcontrol.SelectTab(tab);
            
            //Create new insstance and save it with the selected index
            CHRFile file = new CHRFile();
            files[tabcontrol.SelectedIndex] = file;

            //Set default palette (could put this in the constructor)
            var palA = file.m_bitmap.Palette;
            var palB = file.m_selected_bitmap.Palette;
            BWColors.CopyTo(palA.Entries, 0);
            BWColors.CopyTo(palB.Entries, 0);
            file.m_bitmap.Palette = palA;
            file.m_selected_bitmap.Palette = palB;

            //Reenable the tabcontrol_selected control
            dontinitCHR = false;
            return file;
        }
        private void tabcontrol_selected(object sender, EventArgs e)
        {
            if (dontinitCHR) return;
            CHRFile f = get_chr_file();
            scrollbar.Maximum = (f.chr_data.Length / 1024) - 1;
            scrollbar.Value = f.m_offset / 1024;
            DrawToPanel();
            DrawToSelection();
            using(Graphics graphics = tabcontrol.SelectedTab.CreateGraphics())
            {
                graphics.DrawRectangle(new Pen(new SolidBrush(Color.Red), 5), f.m_graphically_x, f.m_graphically_y, 8 * 3, 8 * 3);
            }
            col1.BackColor = f.m_bitmap.Palette.Entries[0];
            col2.BackColor = f.m_bitmap.Palette.Entries[1];
            col3.BackColor = f.m_bitmap.Palette.Entries[2];
            col4.BackColor = f.m_bitmap.Palette.Entries[3];
        }

        private int GetNESOffset()
        {
            CHRFile f = get_chr_file();
            FileStream fileStream = new FileStream(f.m_filename, FileMode.Open, FileAccess.Read);
            for (int x = 0; x < NES_Signature.Length; x++)
            {
                if (fileStream.ReadByte() != NES_Signature[x]) return -1;
            }
            int prg_size = fileStream.ReadByte();
            return prg_size * 128 * 128 * 4;
        }
        private void DrawToPanel()
        {
            CHRFile f = get_chr_file();
            if (f.m_filename == null)
            {
                MessageBox.Show("Please open a file first", "Error");
                return;
            }
            f.m_previous_offset = f.m_offset;
            TabPage tab = tabcontrol.SelectedTab;
            using (Graphics graphics = tab.CreateGraphics())
            {
                System.Drawing.Imaging.BitmapData bmpData =
                    f.m_bitmap.LockBits(new Rectangle(0, 0, 128, 128), System.Drawing.Imaging.ImageLockMode.ReadWrite, f.m_bitmap.PixelFormat);
                IntPtr ptr = bmpData.Scan0;
                System.Runtime.InteropServices.Marshal.Copy(f.chr_data, f.m_offset, ptr, 128 * 128);
                f.m_bitmap.UnlockBits(bmpData);
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                graphics.DrawImage(f.m_bitmap, 0, 0, 128 * 3, 128 * 3);
            }
        }
        private void loadbtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "NES or CHR|*.chr;*.nes|CHR|*.chr|NES|*.nes";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filepath = openFileDialog.FileName;
                    try
                    {
                        CHRFile f = make_chr_file();
                        tabcontrol.SelectedTab.Text = Path.GetFileName(filepath);
                        bool nesfile;
                        f.m_filename = filepath;
                        string extension = Path.GetExtension(f.m_filename).Trim();
                        if (extension == ".nes")
                        {
                            f.m_offset = GetNESOffset();
                            nesfile = true;
                        }
                        else
                        {
                            f.m_offset = 0;
                            nesfile = false;
                        }
                        if (f.m_offset == -1)
                        {
                            MessageBox.Show("Not a valid NES file");
                            return;
                        }
                        using (FileStream filestream = new FileStream(f.m_filename, FileMode.Open, FileAccess.Read))
                        {
                            if (filestream.Length < 4096) f.file_data = new byte[4096];
                            else f.file_data = new byte[filestream.Length];
                            filestream.Seek(nesfile ? 16 : 0, SeekOrigin.Begin);
                            filestream.Read(f.file_data, 0, nesfile ? (int)filestream.Length - 16 : (int)filestream.Length);
                            List<byte> chr_data_temp = new List<byte>();
                            for (int row = 0; row < (filestream.Length / 256); row++)
                            {
                                for (int tile_row = 0; tile_row < 8; tile_row++)
                                {
                                    for (int tile_in_row = 0; tile_in_row < 16; tile_in_row++)
                                    {
                                        for (int bits = 7; bits >= 0; bits--)
                                        {
                                            int mask_value = 1 << bits;
                                            int plane1 = f.file_data[tile_in_row * 16 + tile_row + row * 256] & mask_value;
                                            int plane2 = f.file_data[tile_in_row * 16 + tile_row + row * 256 + 8] & mask_value;
                                            plane1 = plane1 >> bits;
                                            plane2 = plane2 >> bits;
                                            int index = (plane1 + 2 * plane2);
                                            chr_data_temp.Add((byte)index);
                                        }
                                    }
                                }
                            }
                            f.chr_data = chr_data_temp.ToArray();
                        }
                        scrollbar.Maximum = (f.chr_data.Length / 1024) - 1;
                        scrollbar.Value = f.m_offset / 1024;
                        tabcontrol_selected(sender, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error!");
                    }
                }
            }
        }

        private void uprow_Click(object sender, EventArgs e)
        {
            CHRFile f = get_chr_file();
            f.m_offset -= 256;
            DrawToPanel();
        }

        private void downrow_Click(object sender, EventArgs e)
        {
            CHRFile f = get_chr_file();
            f.m_offset += 256;
            DrawToPanel();
        }

        private void uppage_Click(object sender, EventArgs e)
        {
            CHRFile f = get_chr_file();
            f.m_offset -= 4096;
            DrawToPanel();
        }

        private void downpage_Click(object sender, EventArgs e)
        {
            CHRFile f = get_chr_file();
            f.m_offset += 4096;
            DrawToPanel();
        }

        private void uptile_Click(object sender, EventArgs e)
        {
            CHRFile f = get_chr_file();
            f.m_offset -= 16;
            DrawToPanel();
        }
        private void downtile_Click(object sender, EventArgs e)
        {
            CHRFile f = get_chr_file();
            f.m_offset += 16;
            DrawToPanel();
        }

        private void panel_Paint(object sender, PaintEventArgs e)
        {
            CHRFile f = get_chr_file();
            if (f.file_data == null) return;
            if (f.m_filename != null || (f.m_offset + 4096 > f.file_data.Length || f.m_offset < 0)) DrawToPanel();
        }

        private void panel_Mouse(object sender, MouseEventArgs e)
        {
            CHRFile f = get_chr_file();
            if (f.file_data == null) return;
            if (e.Button == MouseButtons.Left)
            {
                TabPage tab = tabcontrol.SelectedTab;
                using (Graphics graphics = tab.CreateGraphics())
                {
                    int x = e.X - (e.X % (8 * 3));
                    int y = e.Y - (e.Y % (8 * 3));
                    if (x < 0) x = 0;
                    if (x > 120 * 3) x = 120 * 3;
                    if (y < 0) y = 0;
                    if (y > 120 * 3) y = 120 * 3;
                    f.m_sel_x = x / (8 * 3);
                    f.m_sel_y = y / (8 * 3);
                    f.m_selected_tile = x + y * 16;
                    if (f.m_sel_prev_x != f.m_sel_x || f.m_sel_prev_y != f.m_sel_y)
                    {
                        DrawToPanel();
                        graphics.DrawRectangle(new Pen(new SolidBrush(Color.Red), 5), x, y, 8 * 3, 8 * 3);
                        f.m_graphically_x = x;
                        f.m_graphically_y = y;
                    }
                    f.m_sel_prev_x = f.m_sel_x;
                    f.m_sel_prev_y = f.m_sel_y;
                }
                DrawToSelection();
            }
        }

        private void DrawToSelection()
        {
            try
            {
                using (Graphics graphics = engpanel.CreateGraphics())
                {
                    CHRFile f = get_chr_file();
                    System.Drawing.Imaging.BitmapData bmpData =
                    f.m_selected_bitmap.LockBits(new Rectangle(0, 0, 8, 8), System.Drawing.Imaging.ImageLockMode.ReadWrite, f.m_selected_bitmap.PixelFormat);
                    IntPtr ptr = bmpData.Scan0;
                    for (int y = 0; y < 8; y++)
                    {
                        System.Runtime.InteropServices.Marshal.Copy(f.chr_data, f.m_offset + f.m_sel_x * 8 + y * 8 * 16 + f.m_sel_y * 8 * 8 * 16, ptr, 8);
                        ptr += 8;
                    }
                    f.m_selected_bitmap.UnlockBits(bmpData);
                    graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    graphics.DrawImage(f.m_selected_bitmap, 0, 0, 128, 128);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ScrollMainPanel(int scrollvalue)
        {
            CHRFile f = get_chr_file();
            if (scrollvalue < 0) scrollvalue = 0;
            if (scrollvalue > (scrollbar.Maximum - scrollbar.LargeChange)) scrollvalue = scrollbar.Maximum - scrollbar.LargeChange + 1;
            scrollbar.Value = scrollvalue;
            f.m_offset = scrollbar.Value * 1024;
            DrawToPanel();
        }

        private void panel_Scroll(object sender, MouseEventArgs e)
        {
            int temp = scrollbar.Value - e.Delta / 120;
            ScrollMainPanel(temp);
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PageDown) ScrollMainPanel(scrollbar.Value + 16);
            if (e.KeyCode == Keys.PageUp) ScrollMainPanel(scrollbar.Value - 16);
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollMainPanel(scrollbar.Value);
        }

        private void palselect_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    CHRFile f = get_chr_file();
                    var palA = f.m_bitmap.Palette;
                    var palB = f.m_selected_bitmap.Palette;
                    palA.Entries[color] = colorDialog.Color;
                    f.m_bitmap.Palette = palA;
                    palB.Entries[color] = colorDialog.Color;
                    f.m_bitmap.Palette = palB;
                    DrawToPanel();
                    DrawToSelection();
                    switch (color)
                    {
                        default:
                        case 0: col1.BackColor = colorDialog.Color; break;
                        case 1: col2.BackColor = colorDialog.Color; break;
                        case 2: col3.BackColor = colorDialog.Color; break;
                        case 3: col4.BackColor = colorDialog.Color; break;
                    }
                }
            }
        }

        private void do_palette_dialog(object sender, MouseEventArgs e, int color)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    using (palettepicker pp = new palettepicker())
                    {
                        if (pp.ShowDialog() == DialogResult.OK)
                        {
                            CHRFile f = get_chr_file();
                            var palA = f.m_bitmap.Palette;
                            var palB = f.m_selected_bitmap.Palette;
                            palA.Entries[color] = pp.selected_color;
                            f.m_bitmap.Palette = palA;
                            palB.Entries[color] = pp.selected_color;
                            f.m_selected_bitmap.Palette = palB;
                            DrawToPanel();
                            DrawToSelection();
                            switch (color)
                            {
                                default:
                                case 0: col1.BackColor = pp.selected_color; break;
                                case 1: col2.BackColor = pp.selected_color; break;
                                case 2: col3.BackColor = pp.selected_color; break;
                                case 3: col4.BackColor = pp.selected_color; break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void col1_MouseClick(object sender, MouseEventArgs e)
        {
            do_palette_dialog(sender, e, 0);
        }
        private void col2_MouseClick(object sender, MouseEventArgs e)
        {
            do_palette_dialog(sender, e, 1);
        }
        private void col3_MouseClick(object sender, MouseEventArgs e)
        {
            do_palette_dialog(sender, e, 2);
        }
        private void col4_MouseClick(object sender, MouseEventArgs e)
        {
            do_palette_dialog(sender, e, 3);
        }

        private void col1_Click(object sender, EventArgs e)
        {
            color = 0;
        }

        private void col2_Click(object sender, EventArgs e)
        {
            color = 1;
        }

        private void col3_Click(object sender, EventArgs e)
        {
            color = 2;
        }

        private void col4_Click(object sender, EventArgs e)
        {
            color = 3;
        }
    }
}