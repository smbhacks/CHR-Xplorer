﻿namespace FormsLearning
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            loadbtn = new Button();
            savebtn = new Button();
            engpanel = new Panel();
            col1 = new Button();
            col2 = new Button();
            col3 = new Button();
            col4 = new Button();
            scrollbar = new VScrollBar();
            paldialog = new ColorDialog();
            comboBox1 = new ComboBox();
            tabcontrol = new TabControl();
            SuspendLayout();
            // 
            // loadbtn
            // 
            loadbtn.Location = new Point(931, 12);
            loadbtn.Name = "loadbtn";
            loadbtn.Size = new Size(44, 57);
            loadbtn.TabIndex = 1;
            loadbtn.Text = "Load file";
            loadbtn.UseVisualStyleBackColor = true;
            loadbtn.Click += loadbtn_Click;
            // 
            // savebtn
            // 
            savebtn.Location = new Point(931, 75);
            savebtn.Name = "savebtn";
            savebtn.Size = new Size(44, 65);
            savebtn.TabIndex = 2;
            savebtn.Text = "Save file";
            savebtn.UseVisualStyleBackColor = true;
            // 
            // engpanel
            // 
            engpanel.Location = new Point(481, 38);
            engpanel.Margin = new Padding(0);
            engpanel.Name = "engpanel";
            engpanel.Size = new Size(128, 128);
            engpanel.TabIndex = 7;
            // 
            // col1
            // 
            col1.BackColor = Color.Black;
            col1.FlatStyle = FlatStyle.Flat;
            col1.Location = new Point(471, 175);
            col1.Name = "col1";
            col1.Size = new Size(30, 30);
            col1.TabIndex = 12;
            col1.UseVisualStyleBackColor = false;
            col1.Click += col1_Click;
            col1.MouseDown += col1_MouseClick;
            // 
            // col2
            // 
            col2.BackColor = Color.DarkGray;
            col2.FlatStyle = FlatStyle.Flat;
            col2.Location = new Point(507, 175);
            col2.Name = "col2";
            col2.Size = new Size(30, 30);
            col2.TabIndex = 13;
            col2.UseVisualStyleBackColor = false;
            col2.Click += col2_Click;
            col2.MouseDown += col2_MouseClick;
            // 
            // col3
            // 
            col3.BackColor = Color.LightGray;
            col3.FlatStyle = FlatStyle.Flat;
            col3.Location = new Point(543, 175);
            col3.Name = "col3";
            col3.Size = new Size(30, 30);
            col3.TabIndex = 14;
            col3.UseVisualStyleBackColor = false;
            col3.Click += col3_Click;
            col3.MouseDown += col3_MouseClick;
            // 
            // col4
            // 
            col4.BackColor = Color.White;
            col4.FlatStyle = FlatStyle.Flat;
            col4.Location = new Point(579, 175);
            col4.Name = "col4";
            col4.Size = new Size(30, 30);
            col4.TabIndex = 15;
            col4.UseVisualStyleBackColor = false;
            col4.Click += col4_Click;
            col4.MouseDown += col4_MouseClick;
            // 
            // scrollbar
            // 
            scrollbar.LargeChange = 16;
            scrollbar.Location = new Point(448, 12);
            scrollbar.Maximum = 16;
            scrollbar.Name = "scrollbar";
            scrollbar.Size = new Size(20, 387);
            scrollbar.TabIndex = 16;
            scrollbar.Scroll += vScrollBar1_Scroll;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "8x8", "16x16", "32x32" });
            comboBox1.Location = new Point(481, 12);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(66, 23);
            comboBox1.TabIndex = 17;
            // 
            // tabcontrol
            // 
            tabcontrol.Location = new Point(12, 12);
            tabcontrol.Name = "tabcontrol";
            tabcontrol.SelectedIndex = 0;
            tabcontrol.Size = new Size(433, 433);
            tabcontrol.TabIndex = 18;
            tabcontrol.SelectedIndexChanged += tabcontrol_selected;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1034, 457);
            Controls.Add(tabcontrol);
            Controls.Add(comboBox1);
            Controls.Add(scrollbar);
            Controls.Add(col4);
            Controls.Add(col3);
            Controls.Add(col2);
            Controls.Add(col1);
            Controls.Add(engpanel);
            Controls.Add(savebtn);
            Controls.Add(loadbtn);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Name = "Form1";
            Text = "CHR-Xplorer";
            KeyDown += Form1_KeyDown;
            ResumeLayout(false);
        }

        #endregion
        private Button loadbtn;
        private Button savebtn;
        private Panel engpanel;
        private Button col1;
        private Button col2;
        private Button col3;
        private Button col4;
        private VScrollBar scrollbar;
        private ColorDialog paldialog;
        private ComboBox comboBox1;
        private TabControl tabcontrol;
    }
}