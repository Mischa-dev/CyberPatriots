using System;
using System.Diagnostics;
using CyberPatriotHelper.Models;

namespace CyberPatriotHelper.Checks
{
    /// <summary>
    /// Checks if Guest account is disabled
    /// </summary>
    public class GuestAccountCheck : SecurityCheck
    {
        public GuestAccountCheck()
        {
            Name = "Guest Account";
            Description = "Ensures the Guest account is disabled";
            WhyItMatters = "The Guest account provides anonymous access to your system. " +
                          "Leaving it enabled can allow unauthorized users to access your computer without credentials.";
            ManualSteps = "1. Open Computer Management (compmgmt.msc)\n" +
                         "2. Navigate to Local Users and Groups → Users\n" +
                         "3. Right-click Guest account → Properties\n" +
                         "4. Check 'Account is disabled'";
        }

        public override void Check()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "net",
                        Arguments = "user guest",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                Evidence = output;

                if (output.ToLower().Contains("account active") && output.ToLower().Contains("no"))
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
                Evidence = $"Error checking guest account: {ex.Message}";
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
                        FileName = "net",
                        Arguments = "user guest /active:no",
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
