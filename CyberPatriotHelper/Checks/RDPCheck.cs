using System;
using System.Diagnostics;
using CyberPatriotHelper.Models;

namespace CyberPatriotHelper.Checks
{
    /// <summary>
    /// Checks if Remote Desktop (RDP) is disabled
    /// </summary>
    public class RDPCheck : SecurityCheck
    {
        public RDPCheck()
        {
            Name = "Remote Desktop (RDP)";
            Description = "Ensures Remote Desktop Protocol is disabled when not needed";
            WhyItMatters = "RDP is a common target for attackers. If you don't need remote access, " +
                          "disabling it reduces your attack surface and prevents brute-force attacks.";
            ManualSteps = "1. Open System Properties (Win+Pause or SystemPropertiesRemote.exe)\n" +
                         "2. Click 'Remote' tab\n" +
                         "3. Select 'Don't allow remote connections to this computer'\n" +
                         "4. Click OK";
        }

        public override void Check()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "reg",
                        Arguments = "query \"HKLM\\System\\CurrentControlSet\\Control\\Terminal Server\" /v fDenyTSConnections",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                Evidence = output;

                // fDenyTSConnections = 1 means RDP is disabled (which is good)
                if (output.Contains("0x1"))
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
                Evidence = $"Error checking RDP: {ex.Message}";
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
                        FileName = "reg",
                        Arguments = "add \"HKLM\\System\\CurrentControlSet\\Control\\Terminal Server\" /v fDenyTSConnections /t REG_DWORD /d 1 /f",
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
