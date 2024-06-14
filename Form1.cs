using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokeHelper.util;

namespace PokeHelper
{
    public partial class Form1 : Form
    {

        private PogoHelper _pogoHelper;

        private bool _loop = false;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            // todo exception handling
            _pogoHelper = new PogoHelper();
        }

        private async Task Loop()
        {
            var sw = new Stopwatch();
            while (_loop)
            {
                var loaded = await _pogoHelper.WaitForLoad(); // wait for the app to launch
                if (!loaded)
                {
                    // todo error handling 
                    break;
                }
                await _pogoHelper.StartFarming(); // send + open gifts, start vgp, start auto walk
                if (!sw.IsRunning) sw.Start(); // start timer after all actions are completeDiDD
                
                while (sw.Elapsed < TimeSpan.FromMinutes(30)) // farm for 30 minutes, check for cancellation in between 
                {
                    await Task.Delay(500);
                    if (!_loop) break;
                }
                sw.Reset();
            }
            
        }
    }
}