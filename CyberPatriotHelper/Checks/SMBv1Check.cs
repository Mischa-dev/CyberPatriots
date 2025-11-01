using System;
using System.Diagnostics;
using CyberPatriotHelper.Models;

namespace CyberPatriotHelper.Checks
{
    /// <summary>
    /// Checks if SMBv1 is disabled
    /// </summary>
    public class SMBv1Check : SecurityCheck
    {
        public SMBv1Check()
        {
            Name = "SMBv1 Protocol";
            Description = "Ensures the outdated and vulnerable SMBv1 protocol is disabled";
            WhyItMatters = "SMBv1 is an outdated file-sharing protocol with known security vulnerabilities. " +
                          "It was exploited by ransomware like WannaCry. Modern systems should use SMBv2 or SMBv3.";
            ManualSteps = "1. Open PowerShell as Administrator\n" +
                         "2. Run: Disable-WindowsOptionalFeature -Online -FeatureName SMB1Protocol -NoRestart\n" +
                         "3. Restart your computer for changes to take effect";
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
                        Arguments = "-Command \"Get-WindowsOptionalFeature -Online -FeatureName SMB1Protocol | Select-Object -Property State\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                Evidence = output;
                
                if (!string.IsNullOrEmpty(error))
                {
                    Status = CheckStatus.Unknown;
                    Evidence += "\nError: " + error;
                    return;
                }

                if (output.ToLower().Contains("disabled"))
                {
                    Status = CheckStatus.Pass;
                }
                else if (output.ToLower().Contains("enabled"))
                {
                    Status = CheckStatus.Fail;
                }
                else
                {
                    Status = CheckStatus.Unknown;
                }
            }
            catch (Exception ex)
            {
                Status = CheckStatus.Unknown;
                Evidence = $"Error checking SMBv1: {ex.Message}";
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
                        Arguments = "-Command \"Disable-WindowsOptionalFeature -Online -FeatureName SMB1Protocol -NoRestart\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Verb = "runas"
                    }
                };
                process.Start();
                process.WaitForExit();
                
                return process.ExitCode == 0;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // User cancelled UAC prompt or insufficient permissions
                return false;
            }
            catch (Exception)
            {
                // Other errors during fix attempt
                return false;
            }
        }
    }
}
