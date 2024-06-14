using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PokeHelper.util
{
    public class AdbNotif
    {
        private static readonly Regex PackageNameRegex = new Regex(@"pkg=([\w.]+)");
        private static readonly Regex TitleRegex = new Regex(@"android.title=String \(([^)]+)\)");
        private static readonly Regex TextRegex = new Regex(@"android.text=String \(([^)]+)\)");
        
        public string PackageName { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }

        public static IEnumerable<AdbNotif> GetNotifications()
        { // get a list of parsed notifications from the connected device
            var data = ProcessUtils.RunCmd("adb shell dumpsys notification --noredact", true);
            return data == null ? null : Parse(data);
        }
        
        private static List<AdbNotif> Parse(string dumpsysOutput)
        { // parse dumpsys output into AdbNotifs
            var notifications = new List<AdbNotif>();
            var blocks = dumpsysOutput.Split(new[] { "NotificationRecord" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var block in blocks)
            {
                try
                {
                    var packageName = PackageNameRegex.Match(block).Groups[1].Value;
                    var title = TitleRegex.Match(block).Groups[1].Value;
                    var text = TextRegex.Match(block).Groups[1].Value;
                    notifications.Add(new AdbNotif
                    {
                        PackageName = packageName,
                        Title = title,
                        Text = text
                    });
                }
                catch (Exception)
                {
                    // todo error handling
                }
            } 

            return notifications;
        }
    }
}