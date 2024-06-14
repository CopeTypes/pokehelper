using System;
using System.Diagnostics;
using System.IO;

namespace PokeHelper.util
{
    public class ProcessUtils
    {
        public static bool HasProcess(string match)
        {
            var processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                try
                {
                    process.Refresh();
                    if (process.HasExited || !process.ProcessName.Contains(match)) continue;
                    return true;
                } catch (Exception) { }
            }
            return false;
        }

        public static string RunCmd(string cmd, bool wait)
        {
            var process = StartProcess(cmd);
            if (wait) process.WaitForExit(5000);
            return process.StandardOutput.ReadToEnd();
        }

        public static StreamReader RunCmd(string cmd)
        {
            var process = StartProcess(cmd);
            return process.StandardOutput;
        }

        public static Process StartProcess(string cmd)
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/c " + cmd,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            process.StartInfo = startInfo;
            process.Start();
            return process;
        }
    }
}