using System;
using System.Diagnostics;
using CyberPatriotHelper.Models;

namespace CyberPatriotHelper.Checks
{
    /// <summary>
    /// Checks Windows Defender status
    /// </summary>
    public class DefenderCheck : SecurityCheck
    {
        public DefenderCheck()
        {
            Name = "Windows Defender";
            Description = "Ensures Windows Defender antivirus is enabled and active";
            WhyItMatters = "Windows Defender provides real-time protection against viruses, malware, and other threats. " +
                          "Having it disabled leaves your system vulnerable to malicious software.";
            ManualSteps = "1. Open Windows Security (type 'Windows Security' in Start menu)\n" +
                         "2. Go to Virus & threat protection\n" +
                         "3. Ensure Real-time protection is ON\n" +
                         "4. Run a Quick scan to verify functionality";
        }

        public override void Check()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell",
                        Arguments = "-Command \"Get-MpComputerStatus | Select-Object -Property RealTimeProtectionEnabled\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                Evidence = output;

                if (output.ToLower().Contains("true"))
                {
                    Status = CheckStatus.Pass;
                }
                else
                {
                    Status = CheckStatus.Fail;
                }
            }
            catch (Exception ex)
            {
                Status = CheckStatus.Unknown;
                Evidence = $"Error checking Windows Defender: {ex.Message}";
            }
        }

        public override bool Fix()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell",
                        Arguments = "-Command \"Set-MpPreference -DisableRealtimeMonitoring $false\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Verb = "runas"
                    }
                };
                process.Start();
                process.WaitForExit();
                
                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
