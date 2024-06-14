using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AdvancedSharpAdbClient.DeviceCommands;
using AdvancedSharpAdbClient.DeviceCommands.Models;
using PokeHelper.config;

namespace PokeHelper.util
{
    public class PgsHelper
    {
        
        
        public string SummaryWindowId = "//node[@resource-id='me.underworld.helaplugin:id/hl_ec_sum']"; // check for iv summary box (catch screen)
        public string ShinyId = "//node[@resource-id='me.underworld.helaplugin:id/hl_ec_sum_shiny']"; // check for shiny
        public string LevelId = "//node[@resource-id='me.underworld.helaplugin:id/hl_ec_sum_lvv']"; // pokemon level : 16
        public string IvId = "//node[@resource-id='me.underworld.helaplugin:id/hl_ec_sum_ivv']"; // iv : 64
        public string IvSummary = "//node[@resource-id='me.underworld.helaplugin:id/hl_ec_sum_ads']"; // full iv : 9/5/15

        public string NearbyRadarId = "//node[@resource-id='me.underworld.helaplugin:id/hl_sri_icon']"; // nearby radar, element 1 or 2 is the first slot, need to check
        public string CooldownIconId = "//node[@resource-id='me.underworld.helaplugin:id/hl_cd_icon']"; // cooldown text icon
        public string CooldownTextId = "//node[@resource-id='me.underworld.helaplugin:id/hl_cd_text']"; // cooldown text : 0:00:00

        // these are only shown when in the friends tab (inside the trainer menu)
        public string SearchFriendsId = "//node[@text='Search Friends(Beta)']"; // search friends button
        public string OpenGiftsId = "//node[@text='Open Gifts(Beta)']"; // open gifts button
        public string SendGiftsId = "//node[@text='Send Gifts']"; // send gifts button

        public string AutoWalkId = "//node[@text='AutoWalk']"; // auto walk button
        public string GlobalOkId = "//node[@text='OK']"; // generic ok button used in PGSharp dialogs (ie autowalk)

        
        private ExtendedDeviceClient _client;
        private Config _config;

        public PgsHelper(ExtendedDeviceClient client, Config config)
        {
            _client = client;
            _config = config;
            if (!_config.Rooted)
            {
                LevelId = LevelId.Replace("me.underworld.helaplugin", "me.underw.hp");
                IvId = IvId.Replace("me.underworld.helaplugin", "me.underw.hp");
                IvSummary = IvSummary.Replace("me.underworld.helaplugin", "me.underw.hp");
                ShinyId = ShinyId.Replace("me.underworld.helaplugin", "me.underw.hp");
            }
        }

        public bool IsNearbyRadarOnScreen()
        {
            var radar = FindElement(NearbyRadarId);
            return radar != null;
        }
        
        public async Task<bool> StartAutoWalk()
        { // start auto walking with the maximum # of stops
            var awButton = FindElement(AutoWalkId);
            if (awButton == null) return false;
            await awButton.ClickAsync();
            return await ClickOK();
        }

        private async Task<bool> ClickOK()
        { // click the generic OK button on a PGSharp dialog
            await Task.Delay(300); // give the dialog time to pop up
            var okButton = FindElement(GlobalOkId);
            if (okButton == null) return false;
            await okButton.ClickAsync();
            return true;
        }

        public async Task<bool> OpenGifts()
        { // send gifts to all friends that can receive one
            var openButton = FindElement(OpenGiftsId);
            if (openButton == null) return false;
            await openButton.ClickAsync();
            if (!await ClickOK()) return false;
            //await Task.Delay(30000);
            await WaitForElementDispose(OpenGiftsId, TimeSpan.FromSeconds(30));
            return true;
        }

        public async Task<bool> SendGifts()
        { // open all pending gifts (if possible)
            var sendButton = FindElement(SendGiftsId);
            if (sendButton == null) return false;
            await sendButton.ClickAsync();
            if (!await ClickOK()) return false;
            await WaitForElementDispose(SendGiftsId, TimeSpan.FromSeconds(30));
            //await Task.Delay(30000);
            return true;
        }

        public bool CanAutoGift()
        {
            var send = FindElement(SendGiftsId);
            if (send == null) return false;
            return FindElement(OpenGiftsId) != null;
        }
        
        private Element FindElement(string value)
        {
            if (!_config.Rooted) value = value.Replace("me.underworld.helaplugin", "me.underw.hp");
            return _client.FindElement(value);
            //var elem = _client.FindElement(value);
            //return elem == null ? _client.FindElement(value.Replace("me.underworld.helaplugin", "me.underw.hp")) : elem;
        }

        private async Task WaitForElementDispose(string value, TimeSpan timeout)
        { // wait for an on screen element to "dispose/despawn". in this case mainly to detect when the send/open gifts button is hidden, so we know those functions have finished.
            var sw = new Stopwatch();
            sw.Start();
            while (true)
            {
                if (FindElement(value) == null || sw.Elapsed > timeout) break;
                await Task.Delay(1000);
            }
        }

        private bool IsElementOnScreen(string value)
        {
            return FindElement(value) != null;
        }

        public bool IsLoaded()
        { // check if pogo (and pgsharp) are loaded
            // todo this relies on the user having the cooldown timer on (which they should), there should be a fallback anyways.
            return IsElementOnScreen(CooldownIconId);
        }
        
        public bool IsInCatchScreen()
        { // check if the catch screen is currently on screen
            return IsElementOnScreen(SummaryWindowId);
        }

        public bool IsInFriendsScreen()
        { // check if the friends tab is currently on screen
            return IsElementOnScreen(SearchFriendsId);
        }
        
        private string GetIvFull()
        { // gets the full iv, ie: 1/2/3
            var iv = FindElement(IvSummary);
            return iv == null ? null : iv.Text;
        }

        private string GetIv()
        { // gets the iv, ie: 64
            var iv = FindElement(IvId);
            return iv == null ? null : iv.Text;
        }

        private string GetLvl()
        {
            var lvl = FindElement(LevelId);
            return lvl == null ? null : lvl.Text;
        }
        
        public bool IsPerfect()
        { // check if the current pokemon in the catch screen is perfect
            var iv = GetIvFull();
            return iv != null && iv.Equals("15/15/15");
        }

        public bool IsShiny()
        { // check if the current pokemon in the catch screen is shiny
            return IsElementOnScreen(ShinyId);
        }

        public MonInfo GetMonInfo()
        { // gets a summary for the current pokemon on screen (catch screen)
            var d = new List<string> { LevelId, IvId, IvSummary, ShinyId };
            var dd = _client.FindElements(d);
            if (dd == null) return null;
            var lvl = dd[LevelId];
            var iv = dd[IvId];
            var ivFull = dd[IvSummary];
            if (lvl != null && iv != null && ivFull != null) return new MonInfo(lvl.Text, iv.Text, ivFull.Text, dd.ContainsKey(ShinyId));
            return null;
        }
    }
}