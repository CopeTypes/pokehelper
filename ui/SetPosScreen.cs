using System;
using System.Windows.Forms;
using PokeHelper.util;

namespace PokeHelper.ui
{
    public partial class SetPosScreen : Form
    {

        private AdbHelper _adbHelper;
        private Pos _pos;
        
        public SetPosScreen(AdbHelper adbHelper, Pos pos)
        {
            InitializeComponent();
            _adbHelper = adbHelper;
            _pos = pos;
        }

        public Pos GetPos()
        {
            return _pos;
        }

        private void SetButton_Click(object sender, EventArgs e)
        {
            var pos = _adbHelper.ReadTouch();
            if (pos == null)
            {
                MessageBox.Show("Error reading touch events from device.");
                return;
            }

            _pos.X = pos.X;
            _pos.Y = pos.Y;
            MessageBox.Show("Point set to: " + pos);
        }

        private void SetPosScreen_Shown(object sender, EventArgs e)
        {
            NameLabel.Text = _pos.Name;
        }
    }
}