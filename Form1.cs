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
            var pal = m_bitmap.Palette;
            BWColors.CopyTo(pal.Entries, 0);
            m_bitmap.Palette = pal;
            InitializeComponent();
        }

        private Color[] BWColors =
        {
            Color.Black, Color.LightGray, Color.DarkGray, Color.White
        };
        private string m_filename;
        private int m_offset;
        private int m_previous_offset;
        private Bitmap m_bitmap = new Bitmap(128, 128, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
        private Bitmap m_selected_bitmap = new Bitmap(128, 128);
        private byte[] NES_Signature = { 0x4e, 0x45, 0x53, 0x1a };
        private byte[] file_data;
        private byte[] chr_data;
        private int m_selected_tile;
        private int color;
        private bool nesfile;
        private int GetNESOffset()
        {
            FileStream fileStream = new FileStream(m_filename, FileMode.Open, FileAccess.Read);
            for (int x = 0; x < NES_Signature.Length; x++)
            {
                if (fileStream.ReadByte() != NES_Signature[x]) return -1;
            }
            int prg_size = fileStream.ReadByte();
            return prg_size * 128 * 128 * 4;
        }
        private void DrawToPanel()
        {
            if (m_filename == null)
            {
                MessageBox.Show("Please open a file first", "Error");
                return;
            }
            m_previous_offset = m_offset;
            using (Graphics graphics = panel.CreateGraphics())
            {
                System.Drawing.Imaging.BitmapData bmpData = 
                    m_bitmap.LockBits(new Rectangle(0, 0, 128, 128), System.Drawing.Imaging.ImageLockMode.ReadWrite, m_bitmap.PixelFormat);
                IntPtr ptr = bmpData.Scan0;
                System.Runtime.InteropServices.Marshal.Copy(chr_data, m_offset, ptr, 128 * 128);
                m_bitmap.UnlockBits(bmpData);
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                graphics.DrawImage(m_bitmap, 0, 0, 128 * 3, 128 * 3);
            }
        }
        private void loadbtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CHR|*.chr|NES|*.nes|Any filetype|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filepath = openFileDialog.FileName;
                    try
                    {
                        m_filename = filepath;
                        string extension = Path.GetExtension(m_filename).Trim();
                        if (extension == ".nes")
                        {
                            m_offset = GetNESOffset();
                            nesfile = true;
                        }
                        else 
                        {
                            m_offset = 0;
                            nesfile = false;
                        }   
                        if (m_offset == -1)
                        {
                            MessageBox.Show("Not a valid NES file");
                            return;
                        }
                        using (FileStream filestream = new FileStream(m_filename, FileMode.Open, FileAccess.Read))
                        {
                            if (filestream.Length < 4096) file_data = new byte[4096];
                            else file_data = new byte[filestream.Length];
                            filestream.Seek(nesfile ? 16 : 0, SeekOrigin.Begin);
                            filestream.Read(file_data, 0, nesfile ? (int)filestream.Length - 16 : (int)filestream.Length);
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
                                            int plane1 = file_data[tile_in_row * 16 + tile_row + row * 256] & mask_value;
                                            int plane2 = file_data[tile_in_row * 16 + tile_row + row * 256 + 8] & mask_value;
                                            plane1 = plane1 >> bits;
                                            plane2 = plane2 >> bits;
                                            int index = (plane1 + 2 * plane2);
                                            chr_data_temp.Add((byte)index);
                                        }
                                    }
                                }
                            }
                            chr_data = chr_data_temp.ToArray();
                        }
                        scrollbar.Maximum = (chr_data.Length / 1024) - 1;
                        scrollbar.Value = m_offset / 1024;
                        DrawToPanel();
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
            m_offset -= 256;
            DrawToPanel();
        }

        private void downrow_Click(object sender, EventArgs e)
        {
            m_offset += 256;
            DrawToPanel();
        }

        private void uppage_Click(object sender, EventArgs e)
        {

            m_offset -= 4096;
            DrawToPanel();
        }

        private void downpage_Click(object sender, EventArgs e)
        {
            m_offset += 4096;
            DrawToPanel();
        }

        private void uptile_Click(object sender, EventArgs e)
        {
            m_offset -= 16;
            DrawToPanel();
        }
        private void downtile_Click(object sender, EventArgs e)
        {
            m_offset += 16;
            DrawToPanel();
        }

        private void panel_Paint(object sender, PaintEventArgs e)
        {
            if (file_data == null) return;
            if (m_filename != null || (m_offset + 4096 > file_data.Length || m_offset < 0)) DrawToPanel();
        }

        private void panel_Mouse(object sender, MouseEventArgs e)
        {
            if (file_data == null) return;
            if (e.Button == MouseButtons.Left)
            {
                DrawToPanel();
                using (Graphics graphics = panel.CreateGraphics())
                {
                    int x = e.X - (e.X % (8 * 3));
                    int y = e.Y - (e.Y % (8 * 3));
                    m_selected_tile = x + y * 16;
                    graphics.DrawRectangle(new Pen(new SolidBrush(Color.Red), 5), x, y, 8 * 3, 8 * 3);
                }
                DrawToSelection();
            }
        }

        private void DrawToSelection()
        {

        }

        private void ScrollMainPanel(int scrollvalue)
        {
            if (scrollvalue < 0) scrollvalue = 0;
            if (scrollvalue > (scrollbar.Maximum - scrollbar.LargeChange)) scrollvalue = scrollbar.Maximum - scrollbar.LargeChange + 1;
            scrollbar.Value = scrollvalue;
            m_offset = scrollbar.Value * 1024;
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
                    var pal = m_bitmap.Palette;
                    pal.Entries[color] = colorDialog.Color;
                    m_bitmap.Palette = pal;
                    DrawToPanel();
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