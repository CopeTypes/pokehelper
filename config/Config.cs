using System;
using System.IO;
using Newtonsoft.Json;

namespace PokeHelper.config
{
    public class Config
    {
        
        //public int ActionDelay { get; set; } // global action delay todo check on this but i think it'll be better hard-coded with the LowSpecPhone flag for extra delays
        public int SwipeTime { get; set; } // how long to swipe for
        public int ThrowTime { get; set; } // how long to swipe for , when throwing a pokeball
        
        public int AppLaunchWait { get; set; } // how long to wait for the app to load
        public int AppLaunchTries { get; set; } // how many times to try launching the app before aborting
        
        public int VgpStartWait { get; set; } // how long (in seconds) to wait for virtual go plus to start
        
        public int FriendCount { get; set; } // how many friends you have

        public bool Rooted { get; set; }
        
        public bool Premium { get; set; }
        
        public bool LowSpecPhone { get; set; } // used to change some behavior to increase success on lower spec phones
        public bool IgnoreShinyBuddy { get; set; } // ignore your own buddy when shiny hunting (zorua, ditto, etc) this may result in missed shinies but the chances are extremely low
        public bool ShundoOnly { get; set; }
        public int SpawnWaitTime { get; set; }
        public string BuddyName { get; set; }
        
        private static string GetPath() => Path.Combine(Program.WorkDir, $"config_{_deviceName}.json");
        
        [JsonIgnore]
        public Positions Positions; //= new Positions();

        [JsonIgnore] private static string _deviceName;

        public bool Load(string deviceName)
        {
            _deviceName = deviceName; // set the device identifier for this instance
            Positions = new Positions(_deviceName); // load positions for this device
            if (!Positions.Load()) return false;
            var path = GetPath();
            if (!File.Exists(path))
            { // create default config if it doesn't exist
                //ActionDelay = 1000;
                SwipeTime = 800;
                ThrowTime = 700;
                Rooted = false;
                Premium = false;
                AppLaunchWait = 20000;
                AppLaunchTries = 6;
                VgpStartWait = 30000;
                FriendCount = 0;
                LowSpecPhone = false; // rather the user enable this if they have issue, than it be enabled by default.
                IgnoreShinyBuddy = false;
                BuddyName = "BuddyNameHere";
                ShundoOnly = false;
                SpawnWaitTime = 25000;
                if (!Save()) return false;
            }
            try
            {
                LoadFromJson(GetPath());
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception loading config.json:");
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }

        public bool Save()
        {
            if (!Positions.Save()) return false;
            try
            {
                SaveToJson(GetPath());
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Config save exception:");
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }
        
        private void SaveToJson(string filePath)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        private void LoadFromJson(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var cjson = JsonConvert.DeserializeObject<Config>(json);
            //ActionDelay = cjson.ActionDelay;
            SwipeTime = cjson.SwipeTime;
            ThrowTime = cjson.ThrowTime;
            Rooted = cjson.Rooted;
            Premium = cjson.Premium;
            AppLaunchWait = cjson.AppLaunchWait;
            AppLaunchTries = cjson.AppLaunchTries;
            VgpStartWait = cjson.VgpStartWait;
            FriendCount = cjson.FriendCount;
            LowSpecPhone = cjson.LowSpecPhone;
            IgnoreShinyBuddy = cjson.IgnoreShinyBuddy;
            BuddyName = cjson.BuddyName;
            ShundoOnly = cjson.ShundoOnly;
            SpawnWaitTime = cjson.SpawnWaitTime;
        }

    }
}