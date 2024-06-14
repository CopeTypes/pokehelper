using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using PokeHelper.util;

namespace PokeHelper.config
{
    public class Positions
    {
        public Pos CenterOk { get; set; }
        public Pos CenterScreen { get; set; } // todo some way to automatically calculate this, although that may introduce catch errors with shiny hunting
        
        // the dismiss button that comes up sometimes when opening the app
        // the dismiss 'ok' on the weather dialog that comes up sometimes
        
        public Pos MainMenuButton { get; set; } // pokeball
        
        
        public Pos GoPlusButton { get; set; }
        
        public Pos PgsMenu { get; set; } // pgsharp main menu button
        
        
        //public Pos PgsAutoWalk { get; set; } these should be found dynamically with PgsHelper
        //public Pos PgsAutoWalkOk { get; set; }
        //public Pos PgsSearchFriends { get; set; }
        //public Pos PgsOpenGifts { get; set; }
        //public Pos PgsSendGifts { get; set; }
        //public Pos PgsNearbyRadarSlot1 { get; set; } // first slot in the nearby radar, for automated shiny hunting
        
        public Pos TabSwipeStart { get; set; } // swipe start/end for changing tabs
        public Pos TabSwipeEnd { get; set; }
        
        public Pos TrainerMenuButton { get; set; } // bottom left trainer icon
        //public Pos TrainerMeTab { get; set; }
        public Pos TrainerFriendTab { get; set; } // friends tab in the trainer menu
        
        public Pos FirstFriendSlot { get; set; } // position of the first friend "slot" in the friends tab
        
        public Pos SendGiftButton { get; set; } // send gift button (on the friend overview screen)
        
        
        public Pos ThrowBallStart { get; set; } // swipe start for pokeball throw
        public Pos ThrowBallEnd { get; set; } // swipe end for pokeball throw
        
        public Pos FirstQSSlot { get; set; } // first "quick sniper" slot (can't be detected via layout xml)
        
        // these *should* all be able to be handled by UIHelper.FindGenericOkButton
        //public Pos StayAwareButton { get; set; } // the "ok" on the stay aware dialog that sometimes pops up when loading
        //public Pos WeatherWarningButton { get; set; } // the "ok" on the weather warning dialog, this may be in the same position as the StayAwareButton
        //public Pos DrivingWarningButton { get; set; } // the "ok" on the driving warning dialog (you're going too fast!)
        
        public Pos PokemonMenuButton { get; set; } // the pokemon storage button inside the trainer menu
        public Pos FirstPokemonBox { get; set; } // the first (top left) pokemon storage box

        [JsonIgnore] private static string _deviceName;

        private static string GetPath()
        {
            return Path.Combine(Program.WorkDir, $"positions-{_deviceName}.json");
        }

        [JsonIgnore]
        public List<Pos> PosList = new List<Pos>();
        
        public Positions(string deviceName)
        {
            _deviceName = deviceName;
        }

        public bool Load()
        {
            var path = GetPath();
            var newCfg = false;
            if (!File.Exists(path))
            { // create default config if it doesn't exist
                newCfg = true;
                MessageBox.Show("No existing config found for " + _deviceName);
                SetDefaults();
                if (!Save()) return false;
            }
            try
            {
                LoadFromJson(GetPath(), newCfg);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show($"There was an error loading positions for {_deviceName}, check the logs.");
                Console.WriteLine("Exception loading positions.json:");
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }
        
        private void LoadFromJson(string filePath, bool newCfg)
        {
            var json = File.ReadAllText(filePath);
            var pjson = JsonConvert.DeserializeObject<Positions>(json);
            
            CenterOk = pjson.CenterOk;
            CenterScreen = pjson.CenterScreen;
            MainMenuButton = pjson.MainMenuButton;
            GoPlusButton = pjson.GoPlusButton;
            PgsMenu = pjson.PgsMenu;
            TabSwipeStart = pjson.TabSwipeStart;
            TabSwipeEnd = pjson.TabSwipeEnd;
            TrainerMenuButton = pjson.TrainerMenuButton;
            TrainerFriendTab = pjson.TrainerFriendTab;
            FirstFriendSlot = pjson.FirstFriendSlot;
            ThrowBallStart = pjson.ThrowBallStart;
            ThrowBallEnd = pjson.ThrowBallEnd;
            SendGiftButton = pjson.SendGiftButton;
            FirstQSSlot = pjson.FirstQSSlot;
            //StayAwareButton = pjson.StayAwareButton;
            //WeatherWarningButton = pjson.WeatherWarningButton;
            //DrivingWarningButton = pjson.DrivingWarningButton;
            PokemonMenuButton = pjson.PokemonMenuButton;
            FirstPokemonBox = pjson.FirstPokemonBox;
            
            PosList.Add(CenterScreen);
            PosList.Add(CenterOk);
            PosList.Add(MainMenuButton);
            PosList.Add(GoPlusButton);
            PosList.Add(PgsMenu);
            PosList.Add(TabSwipeStart);
            PosList.Add(TabSwipeEnd);
            PosList.Add(TrainerMenuButton);
            PosList.Add(FirstFriendSlot);
            PosList.Add(TrainerFriendTab);
            PosList.Add(ThrowBallStart);
            PosList.Add(ThrowBallEnd);
            PosList.Add(SendGiftButton);
            PosList.Add(FirstQSSlot);
            //PosList.Add(StayAwareButton);
            //PosList.Add(WeatherWarningButton);
            //PosList.Add(DrivingWarningButton);
            PosList.Add(PokemonMenuButton);
            PosList.Add(FirstPokemonBox);

            if (!newCfg && MissingValues())
                MessageBox.Show("Some (or all) positions are not set, the config may be corrupt.");
        }

        private void SetDefaults()
        { // set default config values
            CenterOk = new Pos(0, 0, "center_ok");
            CenterScreen = new Pos(0, 0, "center_screen");
            MainMenuButton = new Pos(0, 0, "main_menu_button");
            GoPlusButton = new Pos(0, 0, "go_plus_button");
            PgsMenu = new Pos(0, 0, "pgs_menu_button");
            TabSwipeStart = new Pos(0, 0, "tab_swipe_start");
            TabSwipeEnd = new Pos(0, 0, "tab_swipe_end");
            TrainerMenuButton = new Pos(0, 0, "trainer_menu_button");
            TrainerFriendTab = new Pos(0, 0, "trainer_friends_tab");
            FirstFriendSlot = new Pos(0, 0, "first_friends_slot");
            ThrowBallStart = new Pos(0, 0, "throw_pokeball_start");
            ThrowBallEnd = new Pos(0, 0, "throw_pokeball_end");
            SendGiftButton = new Pos(0, 0, "send_gift_button");
            FirstQSSlot = new Pos(0, 0, "first_quick_sniper_slot");
            //StayAwareButton = new Pos(0, 0, "stay_aware_ok_button");
            //WeatherWarningButton = new Pos(0, 0, "weather_warning_ok_button");
            //DrivingWarningButton = new Pos(0, 0, "driving_warning_ok_button");
            PokemonMenuButton = new Pos(0, 0, "pokemon_storage_button");
            FirstPokemonBox = new Pos(0, 0, "first_pokemon_storage_box");
        }

        public bool Save()
        {
            try
            {
                SaveToJson(GetPath());
                return true;
            }
            catch (Exception e)
            { // todo error handling
                Console.WriteLine("Exception saving postions.json:");
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }

        public bool MissingValues()
        { // check if there's any missing values in the config (should only happen on first run/new device)
            return PosList.Any(pos => !pos.IsSet());
        }
        
        private void SaveToJson(string filePath)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}