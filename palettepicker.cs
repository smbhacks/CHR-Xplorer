﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormsLearning
{
    public partial class palettepicker : Form
    {
        public palettepicker()
        {
            InitializeComponent();
        }
        public Color selected_color;
        private string hexa = "0123456789ABCDEF";
        private int color = 0;
        private int[,] basePaletteColors = {
{ 84, 84, 84}, { 0, 30, 116}, { 8, 16, 144}, { 48, 0, 136}, { 68, 0, 100}, { 92, 0, 48}, { 84, 4, 0}, { 60, 24, 0}, { 32, 42, 0}, { 8, 58, 0}, { 0, 64, 0}, { 0, 60, 0}, { 0, 50, 60}, { 0, 0, 0}, { 0, 0, 0}, { 0, 0, 0}, {152, 150, 152}, { 8, 76, 196}, { 48, 50, 236}, { 92, 30, 228}, {136, 20, 176}, {160, 20, 100}, {152, 34, 32}, {120, 60, 0}, { 84, 90, 0}, { 40, 114, 0}, { 8, 124, 0}, { 0, 118, 40}, { 0, 102, 120}, { 0, 0, 0}, { 0, 0, 0}, { 0, 0, 0}, {255, 255, 255}, { 76, 154, 236}, {120, 124, 236}, {176, 98, 236}, {228,  84, 236}, {236,  88, 180}, {236, 106, 100}, {212, 136,  32}, {160, 170,   0}, {116, 196,   0}, { 76, 208,  32}, { 56, 204, 108}, { 56, 180, 204}, { 60,  60,  60}, {  0,   0,   0}, {  0,   0,   0}, {255, 255, 255}, {168, 204, 236}, {188, 188, 236}, {212, 178, 236}, {236, 174, 236}, {236, 174, 212}, {236, 180, 176}, {228, 196, 144},{204, 210, 120}, {180, 222, 120}, {168, 226, 144}, {152, 226, 180}, {160, 214, 228}, {160, 162, 160}, {  0,   0,   0}, {  0,   0,   0},
};
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x = e.X / 16;
                int y = e.Y / 16;
                int index = x + y * 16;
                selected_color = Color.FromArgb(basePaletteColors[index, 0], basePaletteColors[index, 1], basePaletteColors[index, 2]);
                pictureBox2.BackColor = selected_color;
                label1.BackColor = selected_color;
                label1.ForeColor = Color.FromArgb(basePaletteColors[index, 0] ^ 255, basePaletteColors[index, 1] ^ 255, basePaletteColors[index, 2] ^ 255);
                label1.Text = hexa[y].ToString() + hexa[x].ToString();
            }
        }

        private void cancel_Click(object sender, EventArgs e)
        {

        }

        private void OK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
