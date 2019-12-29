﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Voxine
{
    public partial class GameWindow : Form
    {
        internal static GameWindow Instance { get; private set; }

        public GameWindow()
        {
            InitializeComponent();
            Instance = this;
        }

        private void RenderPictureBox_Click(object sender, EventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            Input.AddClicked(args.Button);
        }

        private void GameWindow_Resize(object sender, EventArgs e)
        {
            Size s = Instance.Size;
            Graphics.Ratio = (double)(s.Width - Graphics._BordersSize.x) / (s.Height - Graphics._BordersSize.y);
        }

        private void GameWindow_LostFocus(object sender, EventArgs e)
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            Cursor.Position = new Point(Cursor.Position.X - 50, Cursor.Position.Y - 50);
            Cursor.Clip = new Rectangle(this.Location, this.Size);


            if (Graphics.RenderMode == Graphics.WindowMode.FullScreen)
            {
                Instance.WindowState = FormWindowState.Minimized;
            }
        }

        private void GameWindow_Closing(object sender, CancelEventArgs e)
        {
            
            //e.Cancel = true;  
        }
    }
}
