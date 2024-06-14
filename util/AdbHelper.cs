using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.DeviceCommands;
using AdvancedSharpAdbClient.DeviceCommands.Models;
using AdvancedSharpAdbClient.Models;
using PokeHelper.config;

namespace PokeHelper.util
{
    public class AdbHelper
    {
        private readonly AdbClient _adb;
        private ExtendedDeviceClient _client;
        private DeviceData _device;
        private string _currentEventH;
        private Pos _currentEventMax;
        private Pos _currentScreenSize;

        private Config _config;
        private OcrHelper _ocr;

        public AdbHelper(/*Config config*/string ip = null)
        {
            _adb = new AdbClient();
            //_config = config;
            _ocr = new OcrHelper();
            if (!SetDevice(ip)) throw new Exception("No connected device");
        }

        public void SetConfig(Config config)
        {
            _config = config ?? throw new Exception("Null config provided to AdbHelper.");
        }

        public ExtendedDeviceClient GetDeviceClient()
        {
            return _client;
        }
        
        public bool SetDevice(string ip)
        {
            
            try
            {
                //todo ui and/or prompt when multiple devices are connected. show some type of identifier, then set the device here
                //this will allow multiple instances of pokehelper to run controlling multiple devices (one instance per device)
                //and allow simple loading/saving of per-device positions and config

                var d = new DeviceData();
                if (ip != null) // handle wireless connection
                {
                    try
                    {
                        Console.Write("Attempting wireless adb connection to " + ip);
                        _adb.Connect(ip);
                        d = _adb.GetDevices().First();
                        Console.WriteLine("Connected.");
                    }
                    catch (Exception e)
                    {
                        Console.Write($"Wireless adb connection to {ip} failed\n{e.StackTrace}");
                        return false;
                    }
                }
                else
                { // handle usb connection
                    var devices = _adb.GetDevices().ToList();
                    if (devices.Count > 1)
                    {
                        // todo handle multiple devices connected over usb
                        // todo checking if device is an emulator
                    }
                    else
                    { // only one device is connected
                        d = _adb.GetDevices().First();
                    }
                    
                }

                if (d == null)
                {
                    MessageBox.Show("There was a communication error trying to connect to your device\n" +
                                    "Please restart Pokehelper and try again.");
                    Console.WriteLine("Failed to connect to device (null DeviceData, shouldn't ever happen)");
                    return false;
                }
                
                _device = d;
                _client = new ExtendedDeviceClient(_adb, _device);
                return true;

            }
            catch (InvalidOperationException ex)
            {
                return false;
            }
        }

        public async Task Click(Pos pos) => await _client.ClickAsync(pos.X, pos.Y);

        public async Task SwipeTo(Pos first, Pos second, int time)
        {
            //var start = first.ToPoint();
            //var end = second.ToPoint();
            //await _client.SwipeAsync(start, end, time); not entirely sure why but this isn't working
            //hopefully this does
            await _client.SwipeAsync(first.X, first.Y, second.X, second.Y, time);
        }
        
        public async void SwipeTo(Pos first, Pos second)
        {
            await SwipeTo(first, second, _config.SwipeTime);
        }

        public Element FindElement(string name)
        {
            return _client.FindElement(name, TimeSpan.FromSeconds(3));
        }

        

        private static Pos GetScreenSize()
        {
            var reader = ProcessUtils.RunCmd("adb shell wm size");
            string str;
            while ((str = reader.ReadLine()) != null)
            {
                if (!str.Contains("Physical size:")) continue;
                var strArray = str.Replace("Physical size: ", "").Split('x');
                return new Pos(int.Parse(strArray[0]), int.Parse(strArray[1]), "screen_size");
            }

            return null;
        }

        public static Pos GetCenterScreen()
        { // automatically calculate the center of the screen
            var screenSize = GetScreenSize();
            if (screenSize == null) return null;
            var centerX = screenSize.X / 2;
            var centerY = screenSize.Y / 2;
            return new Pos(centerX, centerY, "center_screen");
        }
        

        private static string GetScreenEventHandler()
        {
            var reader = ProcessUtils.RunCmd("adb shell getevent -i");
            string eventHandler = null;
            var source = new List<string>
            {
                "touchscreen",
                "BlueStacks Virtual Touch"
            };
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains("add device ")) eventHandler = line.Split(':')[1].Trim();
                if (!string.IsNullOrEmpty(eventHandler) && source.Any(m => line.Contains(m))) return eventHandler;
            }

            return null;
        }

        private static int GetEventHex(string line)
        { // parse an event line containing hex coordinates to the decimal value
            foreach (var s in SplitEmpty(line))
            {
                if (s.Contains("0000")) return int.Parse(s.Replace("0000", ""), NumberStyles.HexNumber);
            }
            return -1;
        }

        private static IEnumerable<string> SplitEmpty(string str)
        {
            return str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private const string MaxRegex = @"max (\d+)";
        private static int ExtractMax(string str)
        {
            var match = Regex.Match(str, MaxRegex);
            if (match.Success) return int.Parse(match.Groups[1].Value);
            return -1;
        }
        
        private static Pos GetScreenMax(string eventHandler)
        { // calculate the maximum values for screen event x and y
            var reader = ProcessUtils.RunCmd("adb shell getevent -il " + eventHandler);

            string line;
            var x = 0;
            var y = 0;
            var hasX = false;
            var hasY = false;
            while ((line = reader.ReadLine()) != null)
            {
                if (!hasX && line.Contains("ABS_") && line.Contains("X"))
                { // extract X maximum
                    var max = ExtractMax(line);
                    if (max == -1) continue;
                    //Console.WriteLine("Screen max for X: " + max);
                    hasX = true;
                    x = max;
                }

                if (!hasY && line.Contains("ABS_") && line.Contains("Y"))
                { // extract Y maximum
                    var max = ExtractMax(line);
                    if (max == -1) continue;
                    //Console.WriteLine("Screen max for Y: " + max);
                    hasY = true;
                    y = max;
                }
            }

            if (!hasX || !hasY) return null;
            return new Pos(x, y, "auto_touch");
        }

        public Pos ReadTouch()
        {
            // get the touchscreen event handler
            if (_currentEventH == null) _currentEventH = GetScreenEventHandler();
            if (_currentEventH == null) return null;
            
            // get the max value for a touch event
            if (_currentEventMax == null) _currentEventMax = GetScreenMax(_currentEventH);
            if (_currentEventMax == null) return null;
            
            // get the screen size to calculate touch coordinates
            if (_currentScreenSize == null) _currentScreenSize = GetScreenSize();
            if (_currentScreenSize == null) return null;

            // start listening to touch events
            var reader = ProcessUtils.RunCmd("adb shell getevent -l " + _currentEventH);
            
            string line;
            var x = 0;
            var y = 0;

            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains("ABS_MT_POSITION_X") && x == 0)
                { // get x coordinate
                    var result = GetEventHex(line);
                    if (result != -1) x = result;
                }

                if (line.Contains("ABS_MT_POSITION_Y") && y == 0)
                { // get y coordinate
                    var result = GetEventHex(line);
                    if (result != -1) y = result;
                }

                if (x != 0 && y != 0) break;
            }
            
            // calculate the 'actual' touch coordinates
            //Console.WriteLine("NonParsed: " + x + "," + y);
            //Console.WriteLine("ScreenSize: " + _currentScreenSize.X + "," + _currentScreenSize.Y);
            //Console.WriteLine("ScreenMax: " + _currentEventMax.X + "," + _currentEventMax.Y);
            var realX = x * _currentScreenSize.X / _currentEventMax.X;
            var realY = y * _currentScreenSize.Y / _currentEventMax.Y;

            return new Pos(realX, realY, "adb_touch");
        }

        

        public bool IsAppRunning(string packageName)
        {
            var pid = ProcessUtils.RunCmd("adb shell pidof " + packageName, false);
            return string.IsNullOrWhiteSpace(pid);
        }

        public string GetVisibleApp()
        { // get the package name of the app currently on screen
            return ProcessUtils.RunCmd(
                "adb shell \"dumpsys activity activities | grep mResumedActivity | cut -d \"{\" -f2 | cut -d ' ' -f3 | cut -d \"/\" -f1\"", true);
        }


        public async Task<Image> GetScreen()
        {
            using (var img = await _adb.GetFrameBufferAsync(_device, CancellationToken.None))
            {
                var i = img.ToImage();
                img.Dispose();
                return i;
            }
        }

        public async Task<string> GetScreenText()
        {
            using (var img = await GetScreen())
            {
                return _ocr.DumpScreen(img);
            }
        }

        public async Task<Pos> GetGenericOkButton()
        { // find the generic ok button on screen
            // todo probably make private once debugging is done
            using (var img = await GetScreen())
            {
                return UIHelper.FindGenericOkButton(img);
            }
        }

        public async Task<bool> ClickGenericOk()
        { // find the generic ok button on screen, and click it
            var pos = await GetGenericOkButton();
            if (pos == null || !pos.IsSet()) return false;
            await Click(pos);
            return true;
        }

        public async Task<bool> IsTextOnScreen(string str)
        {
            var result = await GetScreenText();
            return result.Contains(str);
        }

        public async Task<bool> IsTextOnScreen(IEnumerable<string> strs)
        {
            var result = await GetScreenText();
            return strs.Any(str => result.Contains(str));
        }
        
        public async Task PressBackButton()
        {
            await _client.SendKeyEventAsync("KEYCODE_BACK");
        }

        public async Task PressBackButton(int times)
        {
            for (var i = 0; i < times; i++)
            {
                await PressBackButton();
                if (_config.LowSpecPhone) await Task.Delay(1000);
                else await Task.Delay(650);
            }
        }

        public string GetModel()
        {
            return _device.Model;
        }
        
        // keys
        // https://developer.android.com/reference/android/view/KeyEvent#constants
        // KEYCODE_BACK KEYCODE_NAVIGATE_NEXT KEYCODE_NAVIGATE_PREVIOUS
        
        // get android version : adb shell getprop ro.build.version.release
        // get sdk version : adb shell getprop ro.build.version.sdk
        // [ro.product.model]: [SM-S911B]
        // [ro.product.cpu.abilist64]: [arm64-v8a] arch
    }
}