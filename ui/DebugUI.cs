using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokeHelper.util;

namespace PokeHelper.ui
{
    public partial class DebugUI : Form
    {

        private PogoHelper _pogoHelper;
        
        public DebugUI(PogoHelper pogoHelper)
        {
            _pogoHelper = pogoHelper;
            InitializeComponent();
        }

        private void SetAllPosButton_Click(object sender, EventArgs e)
        {
            foreach (var pos in _pogoHelper.GetConfig().Positions.PosList.Where(pos => !pos.IsSet()))
            {
                new SetPosScreen(_pogoHelper.GetAdbHelper(), pos).Show();
            }
        }

        private void DebugUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            _pogoHelper.GetConfig().Save();
        }

        private async void ThrowBallButton_Click(object sender, EventArgs e)
        {
            await _pogoHelper.ThrowPokeball();
        }

        private async void CatchMonButton_Click(object sender, EventArgs e)
        {
            await _pogoHelper.CatchPokemon();
        }

        private async void OcrDumpButton_Click(object sender, EventArgs e)
        {
            var text = await _pogoHelper.GetAdbHelper().GetScreenText();
            MessageBox.Show(text);
        }

        private async void FindGenButton_Click(object sender, EventArgs e)
        {
            var pos = await _pogoHelper.GetAdbHelper().GetGenericOkButton();
            MessageBox.Show(pos != null ? pos.ToString() : "Button not found");
        }

        private async void shinyLoopButton_Click(object sender, EventArgs e)
        {
            await _pogoHelper.ShinyHuntLoop();
        }

        private async void stopShinyLoop_Click(object sender, EventArgs e)
        {
            await _pogoHelper.StopShinyHunting();
        }

        private void monInfoButton_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();
            var monInfo = _pogoHelper.GetPgsHelper().GetMonInfo();
            var shiny = false;
            shiny = _pogoHelper.GetPgsHelper().IsShiny();
            sw.Stop();
            MessageBox.Show(monInfo != null ? $"Pokemon Stats: {monInfo} Shiny: {shiny.ToString()}\nCheck took {sw.ElapsedMilliseconds}ms" : "No pokemon on screen, or an error occured.");
        }
    }
}