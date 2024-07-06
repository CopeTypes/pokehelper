using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokeHelper.config;

namespace PokeHelper.util
{
    
    public class DialogHelper
    {

        private readonly AdbHelper _adb;
        private readonly Config _config;
        
        private const string StayAware = "Stay Aware of Your Surroundings"; // sometimes comes up when starting the game
        private const string AdvIncense = "caught while using Incense"; // adventure incense pop-up
        private const string FriendLvlUp = "USE ALUCKY EGG"; // friend level up pop-up
        private const string MedalLvlUp = "You earned a medal!"; // earning and/or leveling up a medal
        private const string WeatherWarning = "Weather warning"; // weather warning dialog
        private const string DrivingWarning = "You're going too fast!"; // warning about not playing while driving
        private const string GenericOk = "OK";
        private const string GenericBuddy = "Buddies!";
        private const string GenericPerks = "NEW PERKS!";

        private List<string> BackButtonTriggers = new() { AdvIncense, FriendLvlUp, MedalLvlUp };
        private List<string> OkButtonTriggers = new() { StayAware, WeatherWarning, DrivingWarning };
        private List<string> BuddyTriggers = new() { GenericBuddy, GenericPerks };

        public DialogHelper(AdbHelper adbHelper, Config config)
        {
            _adb = adbHelper;
            _config = config;
        }

        /// <summary>
        /// Attempts to clear all on-screen dialogs/pop-ups
        /// </summary>
        /// <returns>True if all dialogs are cleared, false on error</returns>
        public async Task<bool> ClearAllDialogs()
        {
            if (!await ClearTriggers(BackButtonTriggers)) return false;
            return await ClearTriggers(OkButtonTriggers, true);
        }

        public async Task<bool> ClearBuddyDialogs()
        {
            return await ClearTriggers(BuddyTriggers);
        }
        
        private async Task<bool> ClearTriggers(IReadOnlyCollection<string> triggers, bool click = false)
        {
            Console.WriteLine("Running DialogHelper.ClearTriggers()");
            var tries = 0;
            while (true)
            {
                if (await _adb.IsTextOnScreen(triggers))
                {
                    Console.WriteLine("One or more triggers on screen, handling");
                    if (click)
                    { // find and click generic ok button
                        if (!await _adb.ClickGenericOk())
                        {
                            Console.WriteLine("Click = true, ClickGenericOk error, failed.");
                            return false;
                        }
                    } else await _adb.PressBackButton();
                    Console.WriteLine("Delaying for UI to update");
                    await Task.Delay(2000);
                    if (_config.LowSpecPhone) await Task.Delay(800);
                    tries++;
                    if (tries > 5)
                    {
                        Console.WriteLine("DialogHelper.ClearTriggers() failed, exceeded 5 tries");
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("No trigger(s) on screen, success!");
                    break;
                }
            }

            return tries <= 5;
        }
        
    }
}