using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdvancedSharpAdbClient.DeviceCommands;
using PokeHelper.config;

namespace PokeHelper.util
{
    public class PogoHelper
    {

        private AdbHelper _adb; // used to interface with ADB
        private PgsHelper _pgsHelper; // used to interface with PGSharp stuff
        private DeviceClient _client;
        private Positions _positions;
        private Config _config;

        private DialogHelper _dialogHelper;
        //private OcrHelper _ocrHelper; // used for reading text/detecting some elements

        private const string PackageName = "com.nianticlabs.pokemongo";
        private const string ActivityName = "com.nianticproject.holoholo.libholoholo.unity.UnityMainActivity";
        
        
        public PogoHelper()
        {
            _config = new Config();
            _adb = new AdbHelper();
            if (!_config.Load(_adb.GetModel())) throw new Exception("Unable to load config.json and/or positions.json");
            _adb.SetConfig(_config);
            //_adb = new AdbHelper(_config);
            _client = _adb.GetDeviceClient();
            _positions = _config.Positions;
            _pgsHelper = new PgsHelper(_client, _config);
            //_ocrHelper = new OcrHelper();
            _dialogHelper = new DialogHelper(_adb, _config);
        }

        public AdbHelper GetAdbHelper()
        {
            return _adb;
        }

        public PgsHelper GetPgsHelper()
        {
            return _pgsHelper;
        }

        public Config GetConfig()
        {
            return _config;
        }

        public async Task ThrowPokeball()
        {
            await _adb.SwipeTo(_positions.ThrowBallStart, _positions.ThrowBallEnd, _config.ThrowTime);
        }

        public async Task<bool> CatchPokemon()
        {
            var catching = true;
            while (catching)
            {
                Console.WriteLine("Throwing pokeball");
                await ThrowPokeball();
                Console.WriteLine("Waiting to see if pokemon was caught");
                await Task.Delay(10000); // longer delay only needed for free version
                catching = _pgsHelper.IsInCatchScreen();
            }
            
            Console.WriteLine("Initial catch part complete");

            if (_config.Premium) return true;
            Console.WriteLine("Non-premium version, dismissing catch screen and returning to home screen");
            await _adb.Click(_positions.CenterOk);
            await Task.Delay(500);
            await _adb.PressBackButton();
            return true;
        }
        
        /// <summary>
        /// Handles full init, start + wait for app load, clear all dialogs
        /// </summary>
        /// <returns>bool</returns>
        public async Task<bool> InitApp()
        {
            if (!await WaitForLoad())
            {
                MessageBox.Show(
                    "Startup failed, the app was unable to properly start or timed out. Please check your device and/or configuration (launch timeout)");
                return false;
            }
            try
            {
                await _dialogHelper.ClearAllDialogs();
            }
            catch (Exception)
            {
                Console.WriteLine("PogoHelper.InitApp failed, encountered unknown dialog, or unable to guarentee current screen.");
                MessageBox.Show(
                    "Startup failed, an unknown pop-up/dialog is obstruction operation. Please report this bug with a picture of what\'s currently on screen!");
                return false;
            }
            return true;
        }

        public async Task<bool> WaitForLoad()
        { // wait for pokemon to load, up to a minute
            var success = false;
            var tries = 0;
            if (!IsAppRunning()) await StartApp(); // if the app isn't running at all, start it
            else if (!IsShown()) BringToForeground(); // if it's already running but not open for some reason, do that
            
            while (true)
            {
                //Console.WriteLine("Waiting 15 seconds before checking if pogo is loaded");
                await Task.Delay(_config.AppLaunchWait);
                if (_pgsHelper.IsLoaded())
                {
                    success = true;
                    break;
                }
                tries++;
                if (tries > _config.AppLaunchTries) break;
                RestartApp();
            }

            return success;
        }

        private async Task GenericActionStart()
        { // to be used when starting a "loop" (ie. vpg farm loop, shiny hunt loop, etc)
            await Task.Delay(1000);
            if (!await _dialogHelper.ClearAllDialogs())
                throw new Exception(
                    "GenericActionStart failed, unable to clear dialogs, and/or unknown ui state present.");
        }
        
        
        public async Task<bool> StartFarming()
        {
            if (!_config.Premium)
            {
                MessageBox.Show("Farming requires the premium version of PGSharp, as it relies on the virtual go plus");
                return false;
            }
            await GenericActionStart();

            // todo redo this logic
            //if (_config.Premium && !await SendAndReceiveGifts()) return false; // auto gift sequence
            // todo manual free version auto gifter
            await StartVgp(TimeSpan.FromSeconds(_config.VgpStartWait));
            
            
            await _client.ClickAsync(_positions.GoPlusButton.ToPoint()); // start vgp
            await Task.Delay(15000); // there is usually some delay in vgp starting , especially on low spec phones

            if (!await _pgsHelper.StartAutoWalk()) return false; // start auto walk


            return true;
        }

        public async Task StopShinyHunting()
        {
            
        }

        private async Task<MonInfo> GetLastCaughtMonInfo()
        { // get MonInfo for the last pokemon caught

            if (!await _dialogHelper.ClearAllDialogs())
            {
                Console.WriteLine("GetLastCaughtMonInfo error, unknown ui state/unable to clear obstructing dialog(s)");
                return null;
            }
            
            await _adb.Click(_positions.MainMenuButton); // open main menu
            await Task.Delay(600);
            if (_config.LowSpecPhone) await Task.Delay(500);

            await _adb.Click(_positions.PokemonMenuButton); // open pokemon storage
            await Task.Delay(600);
            if (_config.LowSpecPhone) await Task.Delay(500);
            
            if (!await _adb.IsTextOnScreen("Search")) {
                Console.WriteLine("GetLastCaughtMonInfo error, failed to navigate to pokemon storage");
                return null;
            }
            
            await _adb.Click(_positions.FirstPokemonBox); // open first pokemon box
            await Task.Delay(600);
            if (_config.LowSpecPhone) await Task.Delay(500);

            var info = _pgsHelper.GetMonInfo();
            await _adb.PressBackButton(2); // get back to the main screen
            return info;
        }

        private bool shinyLoop = false;
        public async Task ShinyHuntLoop()
        { // automatic shiny/shundo hunting
            if (!_config.Premium)
            {
                MessageBox.Show(
                    "Shiny hunting requires the premium version of PGSharp, as it relies on the nearby rader to check (for) shinies.");
                return;
            }
            await GenericActionStart();
            
            if (!_pgsHelper.IsNearbyRadarOnScreen())
            {
                //Console.Write("Cannot start shiny hunt loop, nearby radar is not on/not visible");
                MessageBox.Show(
                    "Cannot shart shiny hunting, nearby radar is not visible or turned off. If you don't have premium, this feature CANNOT WORK.");
                return;
            }

            shinyLoop = true;
            while (shinyLoop)
            {
                // todo need to check if the player has pokeballs
                
                // i think the catch screen just doesn't open if there isn't any
                Console.WriteLine("Teleporting to first quick sniper position");
                await _adb.Click(_positions.FirstQSSlot); // click the first quick sniper slot to teleport
                Console.WriteLine("Waiting for pokemon spawns");
                if (_config.LowSpecPhone) await Task.Delay(2500); // wait a bit longer on low spec phones
                await Task.Delay(_config.SpawnWaitTime); // wait for spawns, todo setting for this
                Console.WriteLine("Checking for nearby shiny");
                if (IsShinyNearby()) 
                { // check for shiny via nearby radar notif
                    await ClickPokemonOnPlayer(); // the shiny should be exactly where we teleported, which should be the center of the screen
                    if (_config.LowSpecPhone) await Task.Delay(600);
                    await Task.Delay(1000);
                    if (_pgsHelper.IsShiny())
                    {
                        var monInfo = _pgsHelper.GetMonInfo();
                        if (_config.ShundoOnly && !monInfo.IsPerfect())
                        {
                            Console.WriteLine("Found target shiny but it isn't perfect, ignoring.");
                            await _adb.PressBackButton(); // close catch screen
                            continue; // teleport to next + continue loop
                        }
                        if (await CatchPokemon())
                        {
                            var slot1Info = await GetLastCaughtMonInfo();
                            if (monInfo.Iv.Equals(slot1Info.Iv) && monInfo.Level.Equals(slot1Info.Level))
                            {
                                Console.WriteLine("Caught target shiny! " + monInfo.Summary());
                            }
                            else
                            {
                                Console.WriteLine("Encountered target shiny " + monInfo.Summary() + " , catch failed.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Encountered target shiny " + monInfo);
                            Console.WriteLine("but something went wrong with the catch sequence (should never happen)");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Misclick or incorrect shiny data from PGSharp, current pokemon isn't shiny (should never happen)");
                        await _adb.PressBackButton();
                    }
                }
                else
                {
                    Console.WriteLine("No shiny nearby, continuing.");
                }
            }
        }

        private async Task StartVgp(TimeSpan timeout)
        { // start (and wait for) the virtual go plus
            await _adb.Click(_positions.GoPlusButton);
            //await _client.ClickAsync(_positions.GoPlusButton.ToPoint());
            await Task.Delay(5000);
            var ready = false;
            var sw = new Stopwatch();
            sw.Start();
            while (true)
            {
                if (sw.Elapsed > timeout) break;
                var notifs = AdbNotif.GetNotifications();
                // todo this needs to be tested, need to see what notification dump looks like with vgp active
                if (notifs.Any(n => n.PackageName.Equals(PackageName) && n.Title.Contains("Accessory Device"))) break;
                await Task.Delay(5000);
            }
        }

        private async Task<bool> SendAndReceiveGifts()
        {
            // todo free version doesn't have these buttons, need to implement manual send/receive loop
            // open trainer menu -> friends tab -> first friend, loop tapping once (to clear gift), tapping gift button, swiping left
            // user will have to manually set the friends count
            if (await OpenFriendsTab())
            {
                if (!_pgsHelper.CanAutoGift())
                {
                    Console.WriteLine("Gifting sequence failed, auto send/and or open button not found (or not visible?)");
                    return false;
                }
                await _pgsHelper.OpenGifts();
                await _pgsHelper.SendGifts();
                if (!await CloseFriendsTab())
                {
                    Console.WriteLine("Gifting sequence failed, unable to close friends tab");
                    return false;
                }
            }
            Console.WriteLine("Gifting sequence failed, unable to open friends tab");
            return false;
        }

        private async Task<bool> SendAndReceiveGiftsFree()
        {
            if (_config.FriendCount.Equals(0)) return false; // user hasn't configured friends count
            if (!await OpenFriendsTab()) return false;
            await _adb.Click(_positions.FirstFriendSlot);
            if (_config.LowSpecPhone) await Task.Delay(1200);
            else await Task.Delay(800);
            var sent = 0;
            while (sent < _config.FriendCount)
            {
                await _adb.Click(_positions.CenterScreen); // dismiss incoming gift, todo make sure this wont touch anything else
                if (_config.LowSpecPhone) await Task.Delay(1500);
                else await Task.Delay(1000);
                await SendGiftManual();
                sent++;
            }

            if (await GetBackToMainScreen()) return true;
            throw new Exception("Unknown Pogo UI State.");
        }

        private async Task SwapBuddy(int id)
        {
            // todo impl
            // first, need to add pos for the buddy icon (main screen), and handling swiping down to the swap buddy button. also need a pos for that button
            // then handling that sequence (dont recall offhand if there's some ok button)
            // need a position for the first pokemon slot (top left)
            // once the screen to select a pokemon is open, input "buddy[id]" into the search bar and send ok
            // this requires the user to tag up to 20 buddies with buddy1 - buddy20, need to implement a system to rotate them as well
            // tap the first pokemon slot pos and wait whatever the delay is
            // this sequence should be able to work the same for free or premium but have to test if any animation stuff is skipped with premium
        }

        private async Task BuddyRotate()
        {
            // todo impl
            // config value for how many buddies are tagged (max of 20)
            // to increase the success value, the main screen should never be shown during this sequence
            // there will be pop-ups for leveling up buddies that can't (yet) be detected, so it will be more reliable regardless i think
            // but we also have to come up with a way to detect those pop-ups anyways, for stuff like weather alerts, friend level up , all that shit
            // todo see if we can just check for "OK" with OCR because that should only be shown when there's visible buttons
            // that would be so cool and simple lmfao but i doubt it'll be that easy. also once we get into OCR stuff its another fucking rabbit hole, same as the ai SHIT
        }

        private async Task SendGiftManual()
        {
            await _adb.Click(_positions.SendGiftButton);
            if (!_config.LowSpecPhone) return;
            await Task.Delay(750);
            await _adb.Click(_positions.SendGiftButton);
        }

        private async Task<bool> OpenFriendsTab()
        {
            await Task.Delay(1000);
            await _adb.Click(_positions.TrainerMenuButton); // click the trainer icon in the bottom left
            await _adb.Click(_positions.TrainerFriendTab); // click the trainer tab (top center) within the trainer menu
            return _pgsHelper.IsInFriendsScreen(); // verify the friends tab is visible
        }

        private async Task<bool> CloseFriendsTab()
        {
            await Task.Delay(500);
            await _adb.PressBackButton();
            // this will slow things down a bit, but ensures it won't get stuck in the friends tab
            if (!_pgsHelper.IsInFriendsScreen()) return true;
            await _adb.PressBackButton();
            return _pgsHelper.IsInFriendsScreen();
        }

        private async Task<bool> GetBackToMainScreen()
        { // try to get back to the main screen, trying a max of four times. This won't work for dismissing pop-ups, only exiting sub-menus (ie. friends)
            var tries = 0;
            var ok = true;
            while (!_pgsHelper.IsNearbyRadarOnScreen())
            {
                await _adb.PressBackButton();
                if (_config.LowSpecPhone) await Task.Delay(1000);
                else await Task.Delay(650);
                tries++;
                if (tries <= 4) continue;
                ok = false;
                break;
            }

            return ok;
        }

        private async Task<bool> ClickPokemonOnPlayer()
        { // click the pokemon on the player's current position
            await _adb.Click(_positions.CenterScreen);
            await Task.Delay(1000); // let the catch screen load
            if (_config.LowSpecPhone) await Task.Delay(300);
            return _pgsHelper.IsInCatchScreen();
        }

        public bool IsShinyNearby()
        { // check if there is a shiny nearby using PGSharp's nearby radar
            var notifications = AdbNotif.GetNotifications();
            var hasShiny = false;
            var title = "";

            foreach (var n in notifications)
            {
                if (!n.PackageName.Equals(PackageName) || !n.Title.Contains("Nearby")) continue;
                hasShiny = true;
                title = n.Title;
                break;
            }

            if (!hasShiny) return false;
            // user configurable to prevent false triggers from zorua & ditto
            return !_config.IgnoreShinyBuddy || !title.Contains(_config.BuddyName);
        }

        private bool IsAppRunning()
        {
            return _adb.IsAppRunning(PackageName);
        }

        private bool IsShown()
        {
            return _adb.GetVisibleApp().Equals(PackageName);
        }
        
        public async Task StartApp()
        {
            await _client.StartAppAsync(PackageName);
        }

        public async Task StopApp()
        {
            await _client.StopAppAsync(PackageName);
        }

        private void BringToForeground()
        { // reopen pogo (from the background), does nothing if pogo is currently shown
            ProcessUtils.RunCmd("adb shell am start -n " + PackageName + "/" + ActivityName, true);
        }

        public async void RestartApp()
        {
            if (_adb.IsAppRunning(PackageName))
            {
                await StopApp();
                await Task.Delay(1000);
            }

            await StartApp();
        }
    }
}