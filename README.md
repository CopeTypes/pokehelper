# pokehelper
pokehelper is a (very wip) pokemon go bot for Android in C#, relying on [PGSharp](https://www.pgsharp.com/)<br> The main reason for this tool is fully automating shiny/shundo hunting.<br><br>
As PGSharp doesn't require root, neither does pokehelper. Rooted and non-rooted devices have the same functionality.

# How does it work?
pokehelper connects to your phone (or emulator 😉) via adb, either wirelessly or over usb to automate actions<br>pokehelper utilizes pixel detection, OCR, and the [screen layout](https://developer.android.com/training/testing/other-components/ui-automator) to detect what's happening in-game.

# How do I set it up?
- Download the release exe, run it and connect your phone. You will have to set some positions manually on the first run. Positions are saved per-device, allowing multiple device support.
- After setting up the positions, enable the features you want and leave your phone connected. <br>The device screen needs to remain on during operation, so OLED screens are not reccomended.
- pokehelper will run for the time you set, and shut the screen off upon exiting.


# What is automated/What are the features?
Most features rely on the [paid](https://www.pgsharp.com/feature-comparison/) version of PGSharp.
- Dismissing any dialogs that might interrupt operation. pokefarm will automatically clear any warning dialogs (weather warning, you're moving too fast, pay attention, etc), the adventure incense summary, and friend/medal level up dialogs.
- Cooldown detection, pokehelper will wait for your cooldown to expire before starting any actions that will trigger a soft ban
- Start/restarting the app and auto walking, along with starting the [virtual go plus](https://www.pgsharp.com/features/#virtual-go-plus) to automate spinning pokestops/catching pokemon
- Automatic shiny/shundo hunting, simply set your desired shiny/shundo and pokehelper will teleport and check pokemon until the shiny/shundo is found. <br>It can be configured to catch automatically (for complete afk), or send a notification and wait for the user.<br> Additionally, it can be configured to ignore "transformation" (ie Zorua or Ditto) which may trigger false positives.
- Automatic sending and recieving of gifts, supporting both free and premium versions. <br>With the free version, it will manually open and send a gift to each of your friends (friend count must be set in config). <br>The premium version has built-in methods for this, so it will be much quicker and more reliable.
  
# When will there be a release / code published?
  - I'm currently working on cleaning up a bunch of code before the first commit, but once that is done (2-3 weeks) everything will be pushed at regular intervals.
  - A proper working release is a ways out, this requires a good bit of testing, lots of things aren't 100% implemented/working, and this is a free time project I'm doing for fun.


  # Credits / Shoutouts
  - Huge shoutout to [PGSharp](https://www.pgsharp.com/) as none of this would be possible without their amazing app.
  


