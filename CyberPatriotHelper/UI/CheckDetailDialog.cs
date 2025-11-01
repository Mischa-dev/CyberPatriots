using System;
using System.Drawing;
using System.Windows.Forms;
using CyberPatriotHelper.Models;

namespace CyberPatriotHelper.UI
{
    /// <summary>
    /// Popup dialog showing details for a security check with Fix and Verify buttons
    /// </summary>
    public partial class CheckDetailDialog : Form
    {
        private readonly SecurityCheck _check;
        private Label lblName = null!;
        private Label lblDescription = null!;
        private GroupBox grpExplanation = null!;
        private TextBox txtExplanation = null!;
        private GroupBox grpWhyItMatters = null!;
        private TextBox txtWhyItMatters = null!;
        private GroupBox grpEvidence = null!;
        private TextBox txtEvidence = null!;
        private GroupBox grpManualSteps = null!;
        private TextBox txtManualSteps = null!;
        private Button btnFix = null!;
        private Button btnVerify = null!;
        private Button btnClose = null!;
        private Label lblStatus = null!;

        public CheckDetailDialog(SecurityCheck check)
        {
            _check = check;
            InitializeComponents();
            LoadCheckDetails();
        }

        private void InitializeComponents()
        {
            this.Text = "Security Check Details";
            this.Size = new Size(700, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int yPos = 10;

            // Name Label
            lblName = new Label
            {
                Location = new Point(10, yPos),
                Size = new Size(660, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Text = _check.Name
            };
            this.Controls.Add(lblName);
            yPos += 35;

            // Status Label
            lblStatus = new Label
            {
                Location = new Point(10, yPos),
                Size = new Size(660, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblStatus);
            yPos += 25;

            // Description
            lblDescription = new Label
            {
                Location = new Point(10, yPos),
                Size = new Size(660, 30),
                Text = _check.Description
            };
            this.Controls.Add(lblDescription);
            yPos += 35;

            // Explanation group
            grpExplanation = new GroupBox
            {
                Location = new Point(10, yPos),
                Size = new Size(660, 80),
                Text = "Explanation"
            };
            txtExplanation = new TextBox
            {
                Location = new Point(10, 20),
                Size = new Size(640, 50),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Text = _check.Description
            };
            grpExplanation.Controls.Add(txtExplanation);
            this.Controls.Add(grpExplanation);
            yPos += 85;

            // Why It Matters group
            grpWhyItMatters = new GroupBox
            {
                Location = new Point(10, yPos),
                Size = new Size(660, 80),
                Text = "Why It Matters"
            };
            txtWhyItMatters = new TextBox
            {
                Location = new Point(10, 20),
                Size = new Size(640, 50),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Text = _check.WhyItMatters
            };
            grpWhyItMatters.Controls.Add(txtWhyItMatters);
            this.Controls.Add(grpWhyItMatters);
            yPos += 85;

            // Evidence group
            grpEvidence = new GroupBox
            {
                Location = new Point(10, yPos),
                Size = new Size(660, 120),
                Text = "Evidence"
            };
            txtEvidence = new TextBox
            {
                Location = new Point(10, 20),
                Size = new Size(640, 90),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 8)
            };
            grpEvidence.Controls.Add(txtEvidence);
            this.Controls.Add(grpEvidence);
            yPos += 125;

            // Manual Steps group
            grpManualSteps = new GroupBox
            {
                Location = new Point(10, yPos),
                Size = new Size(660, 100),
                Text = "Manual Steps (if Fix doesn't work)"
            };
            txtManualSteps = new TextBox
            {
                Location = new Point(10, 20),
                Size = new Size(640, 70),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Text = _check.ManualSteps
            };
            grpManualSteps.Controls.Add(txtManualSteps);
            this.Controls.Add(grpManualSteps);
            yPos += 105;

            // Buttons
            btnFix = new Button
            {
                Location = new Point(10, yPos),
                Size = new Size(150, 35),
                Text = "Fix Now",
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnFix.Click += BtnFix_Click;
            this.Controls.Add(btnFix);

            btnVerify = new Button
            {
                Location = new Point(170, yPos),
                Size = new Size(150, 35),
                Text = "Verify",
                Font = new Font("Segoe UI", 10)
            };
            btnVerify.Click += BtnVerify_Click;
            this.Controls.Add(btnVerify);

            btnClose = new Button
            {
                Location = new Point(520, yPos),
                Size = new Size(150, 35),
                Text = "Close",
                Font = new Font("Segoe UI", 10)
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void LoadCheckDetails()
        {
            UpdateStatus();
            txtEvidence.Text = _check.Evidence;
        }

        private void UpdateStatus()
        {
            string statusText = $"Status: {_check.Status}";
            lblStatus.Text = statusText;

            switch (_check.Status)
            {
                case CheckStatus.Pass:
                    lblStatus.ForeColor = Color.Green;
                    btnFix.Enabled = false;
                    break;
                case CheckStatus.Fail:
                    lblStatus.ForeColor = Color.Red;
                    btnFix.Enabled = true;
                    break;
                case CheckStatus.Warning:
                    lblStatus.ForeColor = Color.Orange;
                    btnFix.Enabled = true;
                    break;
                default:
                    lblStatus.ForeColor = Color.Gray;
                    btnFix.Enabled = false;
                    break;
            }
        }

        private void BtnFix_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show(
                "This will attempt to automatically fix the issue. This requires administrator privileges.\n\nContinue?",
                "Confirm Fix",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    btnFix.Enabled = false;
                    btnFix.Text = "Fixing...";
                    // Note: Application.DoEvents() is used here to update UI immediately
                    // This is acceptable for simple UI feedback in a Windows Forms app
                    // For production, consider using async/await pattern
                    Application.DoEvents();

                    bool success = _check.Fix();

                    if (success)
                    {
                        MessageBox.Show("Fix applied successfully! Click Verify to confirm.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        BtnVerify_Click(sender, e); // Auto-verify after fix
                    }
                    else
                    {
                        MessageBox.Show("Fix failed. You may need to run this application as Administrator, or apply the fix manually using the steps provided.", "Fix Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        btnFix.Enabled = true;
                        btnFix.Text = "Fix Now";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error applying fix: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnFix.Enabled = true;
                    btnFix.Text = "Fix Now";
                }
            }
        }

        private void BtnVerify_Click(object? sender, EventArgs e)
        {
            try
            {
                btnVerify.Enabled = false;
                btnVerify.Text = "Verifying...";
                // Note: Application.DoEvents() is used here to update UI immediately
                // This is acceptable for simple UI feedback in a Windows Forms app
                Application.DoEvents();

                bool verified = _check.Verify();
                txtEvidence.Text = _check.Evidence;
                UpdateStatus();

                if (verified)
                {
                    MessageBox.Show("Verification successful! This security check is now passing.", "Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Verification failed. The issue still exists. Try the manual steps if automatic fix didn't work.", "Not Verified", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                btnVerify.Enabled = true;
                btnVerify.Text = "Verify";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during verification: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnVerify.Enabled = true;
                btnVerify.Text = "Verify";
            }
        }
    }
}
