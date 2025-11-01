using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CyberPatriotHelper.Checks;
using CyberPatriotHelper.Models;
using CyberPatriotHelper.UI;

namespace CyberPatriotHelper
{
    /// <summary>
    /// Main dashboard showing all security checks
    /// </summary>
    public partial class MainDashboard : Form
    {
        private List<SecurityCheck> _checks = null!;
        private ListView _listView = null!;
        private Button _btnRefresh = null!;
        private Button _btnFileSweep = null!;
        private Label _lblTitle = null!;
        private Label _lblSummary = null!;

        public MainDashboard()
        {
            InitializeComponents();
            InitializeChecks();
            RunAllChecks();
        }

        private void InitializeComponents()
        {
            this.Text = "CyberPatriot Security Helper";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 500);

            // Title
            _lblTitle = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(860, 40),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Text = "CyberPatriot Security Dashboard",
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(_lblTitle);

            // Summary
            _lblSummary = new Label
            {
                Location = new Point(10, 55),
                Size = new Size(860, 25),
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(_lblSummary);

            // Instructions
            Label lblInstructions = new Label
            {
                Location = new Point(10, 85),
                Size = new Size(860, 25),
                Font = new Font("Segoe UI", 9),
                Text = "Click any item below to view details, fix issues, and verify changes",
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray
            };
            this.Controls.Add(lblInstructions);

            // ListView for security checks
            _listView = new ListView
            {
                Location = new Point(10, 115),
                Size = new Size(860, 380),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 10)
            };
            _listView.Columns.Add("Security Check", 300);
            _listView.Columns.Add("Status", 100);
            _listView.Columns.Add("Description", 440);
            _listView.DoubleClick += ListView_DoubleClick;
            _listView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.Controls.Add(_listView);

            // Refresh button
            _btnRefresh = new Button
            {
                Location = new Point(10, 505),
                Size = new Size(150, 40),
                Text = "Refresh All",
                Font = new Font("Segoe UI", 10)
            };
            _btnRefresh.Click += BtnRefresh_Click;
            _btnRefresh.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(_btnRefresh);

            // File Sweep button
            _btnFileSweep = new Button
            {
                Location = new Point(170, 505),
                Size = new Size(150, 40),
                Text = "File Sweep",
                Font = new Font("Segoe UI", 10)
            };
            _btnFileSweep.Click += BtnFileSweep_Click;
            _btnFileSweep.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(_btnFileSweep);

            // Info label
            Label lblInfo = new Label
            {
                Location = new Point(330, 505),
                Size = new Size(550, 40),
                Text = "💡 This is a read-only dashboard. No changes are made automatically.\nDouble-click any item to view details and apply fixes.",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.DarkBlue,
                TextAlign = ContentAlignment.MiddleLeft
            };
            lblInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.Controls.Add(lblInfo);
        }

        private void InitializeChecks()
        {
            _checks = new List<SecurityCheck>
            {
                new FirewallCheck(),
                new GuestAccountCheck(),
                new RDPCheck(),
                new DefenderCheck(),
                new SMBv1Check()
            };
        }

        private void RunAllChecks()
        {
            _listView.Items.Clear();
            _btnRefresh.Enabled = false;
            _btnRefresh.Text = "Checking...";
            Application.DoEvents();

            int passCount = 0;
            int failCount = 0;

            foreach (var check in _checks)
            {
                try
                {
                    check.Check();
                    
                    var item = new ListViewItem(check.Name);
                    item.SubItems.Add(check.Status.ToString());
                    item.SubItems.Add(check.Description);
                    item.Tag = check;

                    // Color code by status
                    switch (check.Status)
                    {
                        case CheckStatus.Pass:
                            item.ForeColor = Color.Green;
                            passCount++;
                            break;
                        case CheckStatus.Fail:
                            item.ForeColor = Color.Red;
                            item.Font = new Font(_listView.Font, FontStyle.Bold);
                            failCount++;
                            break;
                        case CheckStatus.Warning:
                            item.ForeColor = Color.Orange;
                            failCount++;
                            break;
                        default:
                            item.ForeColor = Color.Gray;
                            break;
                    }

                    _listView.Items.Add(item);
                }
                catch (Exception ex)
                {
                    var item = new ListViewItem(check.Name);
                    item.SubItems.Add("Error");
                    item.SubItems.Add($"Error: {ex.Message}");
                    item.ForeColor = Color.Gray;
                    _listView.Items.Add(item);
                }
            }

            _lblSummary.Text = $"Security Status: {passCount} Passing, {failCount} Issues Found";
            _lblSummary.ForeColor = failCount == 0 ? Color.Green : Color.Red;
            _lblSummary.Font = new Font(_lblSummary.Font, FontStyle.Bold);

            _btnRefresh.Enabled = true;
            _btnRefresh.Text = "Refresh All";
        }

        private void ListView_DoubleClick(object? sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count > 0)
            {
                var selectedItem = _listView.SelectedItems[0];
                var check = selectedItem.Tag as SecurityCheck;
                
                if (check != null)
                {
                    var dialog = new CheckDetailDialog(check);
                    dialog.ShowDialog(this);
                    
                    // Refresh the specific item after dialog closes
                    RefreshCheck(check, selectedItem);
                }
            }
        }

        private void RefreshCheck(SecurityCheck check, ListViewItem item)
        {
            check.Check();
            item.SubItems[1].Text = check.Status.ToString();
            
            switch (check.Status)
            {
                case CheckStatus.Pass:
                    item.ForeColor = Color.Green;
                    item.Font = _listView.Font;
                    break;
                case CheckStatus.Fail:
                    item.ForeColor = Color.Red;
                    item.Font = new Font(_listView.Font, FontStyle.Bold);
                    break;
                case CheckStatus.Warning:
                    item.ForeColor = Color.Orange;
                    break;
                default:
                    item.ForeColor = Color.Gray;
                    break;
            }

            UpdateSummary();
        }

        private void UpdateSummary()
        {
            int passCount = _checks.Count(c => c.Status == CheckStatus.Pass);
            int failCount = _checks.Count(c => c.Status == CheckStatus.Fail || c.Status == CheckStatus.Warning);
            
            _lblSummary.Text = $"Security Status: {passCount} Passing, {failCount} Issues Found";
            _lblSummary.ForeColor = failCount == 0 ? Color.Green : Color.Red;
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            RunAllChecks();
        }

        private void BtnFileSweep_Click(object? sender, EventArgs e)
        {
            MessageBox.Show(
                "File Sweep will scan all user profiles for suspicious files.\n\n" +
                "This feature scans for:\n" +
                "- Media files in user directories\n" +
                "- Unauthorized software\n" +
                "- Suspicious executables\n\n" +
                "Implementation: Use Windows Search or PowerShell to scan C:\\Users\\ for file types specified in competition rules.",
                "File Sweep",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
