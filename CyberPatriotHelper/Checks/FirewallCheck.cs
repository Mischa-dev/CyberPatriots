using System;
using System.Diagnostics;
using CyberPatriotHelper.Models;

namespace CyberPatriotHelper.Checks
{
    /// <summary>
    /// Checks Windows Firewall status
    /// </summary>
    public class FirewallCheck : SecurityCheck
    {
        public FirewallCheck()
        {
            Name = "Windows Firewall";
            Description = "Ensures Windows Firewall is enabled for all profiles (Domain, Private, Public)";
            WhyItMatters = "The firewall acts as your first line of defense against network-based attacks. " +
                          "It blocks unauthorized access to your computer while allowing legitimate communications.";
            ManualSteps = "1. Open Control Panel → System and Security → Windows Defender Firewall\n" +
                         "2. Click 'Turn Windows Defender Firewall on or off'\n" +
                         "3. Enable firewall for all network types (Domain, Private, Public)";
        }

        public override void Check()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "netsh",
                        Arguments = "advfirewall show allprofiles state",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                Evidence = output;

                // Check if all profiles are ON
                if (output.Contains("State") && !output.ToLower().Contains("off"))
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
                Evidence = $"Error checking firewall: {ex.Message}";
            }
        }

        public override bool Fix()
        {
            try
            {
                // Enable firewall for all profiles
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "netsh",
                        Arguments = "advfirewall set allprofiles state on",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Verb = "runas" // Request admin
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
