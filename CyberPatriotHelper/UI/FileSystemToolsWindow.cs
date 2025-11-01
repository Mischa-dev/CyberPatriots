using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyberPatriotHelper.UI
{
    /// <summary>
    /// File System Tools Window - MVP for finding questionable files
    /// </summary>
    public partial class FileSystemToolsWindow : Form
    {
        private TabControl _tabControl = null!;
        private TabPage _tabUserExplorer = null!;
        private TabPage _tabPatternSearch = null!;
        private Panel _pnlElevationWarning = null!;

        // User Explorer controls
        private ComboBox _cmbUsers = null!;
        private ListView _lvUserFiles = null!;
        private Button _btnOpenExplorer = null!;
        private Button _btnOpenPowerShell = null!;
        private Label _lblUserExplorerStatus = null!;

        // Pattern Search controls
        private TextBox _txtExtensions = null!;
        private TextBox _txtMinSize = null!;
        private DateTimePicker _dtpModifiedSince = null!;
        private CheckBox _chkIncludeSystem = null!;
        private Button _btnStartSearch = null!;
        private Button _btnCancelSearch = null!;
        private Button _btnExportResults = null!;
        private ListView _lvSearchResults = null!;
        private ProgressBar _progressBar = null!;
        private Label _lblSearchStatus = null!;

        private CancellationTokenSource? _searchCancellation;
        private List<FileSearchResult> _searchResults = new List<FileSearchResult>();
        private List<FileSearchResult> _pendingUIUpdates = new List<FileSearchResult>();
        private System.Windows.Forms.Timer _uiUpdateTimer = null!;

        public FileSystemToolsWindow()
        {
            InitializeComponents();
            CheckElevation();
            LoadUsers();
            InitializeUIUpdateTimer();
        }

        private void InitializeUIUpdateTimer()
        {
            _uiUpdateTimer = new System.Windows.Forms.Timer
            {
                Interval = 200 // Update UI every 200ms
            };
            _uiUpdateTimer.Tick += (s, e) => ProcessPendingUIUpdates();
        }

        private void ProcessPendingUIUpdates()
        {
            lock (_pendingUIUpdates)
            {
                if (_pendingUIUpdates.Count == 0)
                    return;

                // Process all pending updates
                foreach (var result in _pendingUIUpdates)
                {
                    var item = new ListViewItem(result.Path);
                    item.SubItems.Add(FormatFileSize(result.Size));
                    item.SubItems.Add(result.Modified.ToString("yyyy-MM-dd HH:mm"));
                    item.SubItems.Add(result.Attributes);
                    item.SubItems.Add(result.IsReviewed ? "Yes" : "");
                    item.Tag = result;
                    _lvSearchResults.Items.Add(item);
                }

                _lblSearchStatus.Text = $"Searching... Found {_searchResults.Count} file(s) so far...";
                _pendingUIUpdates.Clear();
            }
        }

        private void CheckElevation()
        {
            bool isElevated = false;
            try
            {
                using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                var principal = new System.Security.Principal.WindowsPrincipal(identity);
                isElevated = principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
            catch
            {
                // If we can't check, assume not elevated
            }

            if (!isElevated)
            {
                _pnlElevationWarning.Visible = true;
            }
        }

        private void InitializeComponents()
        {
            this.Text = "File System Tools";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(900, 600);

            // Elevation warning panel (initially hidden)
            _pnlElevationWarning = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(1060, 40),
                BackColor = Color.LightYellow,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            Label lblWarning = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(1040, 20),
                Text = "⚠️ Not running as Administrator. Some files/folders may be inaccessible. Run as Admin for full access.",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.DarkOrange
            };
            _pnlElevationWarning.Controls.Add(lblWarning);
            this.Controls.Add(_pnlElevationWarning);

            _tabControl = new TabControl
            {
                Location = new Point(10, 55),
                Size = new Size(1060, 595),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(_tabControl);

            InitializeUserExplorerTab();
            InitializePatternSearchTab();
        }

        private void InitializeUserExplorerTab()
        {
            _tabUserExplorer = new TabPage("User Explorer");
            _tabControl.TabPages.Add(_tabUserExplorer);

            // Instructions
            Label lblInstructions = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(1020, 30),
                Text = "View user folders and files one level deep. Select a file or folder and click 'Open in Explorer' to navigate.",
                Font = new Font("Segoe UI", 9)
            };
            _tabUserExplorer.Controls.Add(lblInstructions);

            // User selection
            Label lblUser = new Label
            {
                Location = new Point(10, 50),
                Size = new Size(80, 25),
                Text = "Select User:",
                TextAlign = ContentAlignment.MiddleLeft
            };
            _tabUserExplorer.Controls.Add(lblUser);

            _cmbUsers = new ComboBox
            {
                Location = new Point(100, 50),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbUsers.SelectedIndexChanged += CmbUsers_SelectedIndexChanged;
            _tabUserExplorer.Controls.Add(_cmbUsers);

            // Status label
            _lblUserExplorerStatus = new Label
            {
                Location = new Point(410, 50),
                Size = new Size(600, 25),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.DarkBlue
            };
            _tabUserExplorer.Controls.Add(_lblUserExplorerStatus);

            // Files list
            _lvUserFiles = new ListView
            {
                Location = new Point(10, 85),
                Size = new Size(1020, 440),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 9),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            _lvUserFiles.Columns.Add("Type", 60);
            _lvUserFiles.Columns.Add("Folder", 150);
            _lvUserFiles.Columns.Add("Name", 350);
            _lvUserFiles.Columns.Add("Size", 100);
            _lvUserFiles.Columns.Add("Modified", 150);
            _lvUserFiles.Columns.Add("Attributes", 150);
            _lvUserFiles.DoubleClick += LvUserFiles_DoubleClick;
            _tabUserExplorer.Controls.Add(_lvUserFiles);

            // Buttons
            _btnOpenExplorer = new Button
            {
                Location = new Point(10, 535),
                Size = new Size(150, 35),
                Text = "Open in Explorer",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            _btnOpenExplorer.Click += BtnOpenExplorer_Click;
            _tabUserExplorer.Controls.Add(_btnOpenExplorer);

            _btnOpenPowerShell = new Button
            {
                Location = new Point(170, 535),
                Size = new Size(200, 35),
                Text = "Open Elevated PowerShell",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            _btnOpenPowerShell.Click += BtnOpenPowerShell_Click;
            _tabUserExplorer.Controls.Add(_btnOpenPowerShell);

            // Info label
            Label lblInfo = new Label
            {
                Location = new Point(380, 535),
                Size = new Size(640, 35),
                Text = "💡 Read-only view. Double-click items to open in File Explorer.\n" +
                       "⚠️ Use PowerShell for admin deletions/inspection if needed.",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.DarkBlue,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            _tabUserExplorer.Controls.Add(lblInfo);
        }

        private void InitializePatternSearchTab()
        {
            _tabPatternSearch = new TabPage("Global Pattern Search");
            _tabControl.TabPages.Add(_tabPatternSearch);

            // Instructions
            Label lblInstructions = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(1020, 30),
                Text = "Search for questionable files across the system. Results stream as they're found.",
                Font = new Font("Segoe UI", 9)
            };
            _tabPatternSearch.Controls.Add(lblInstructions);

            // Extensions filter
            Label lblExtensions = new Label
            {
                Location = new Point(10, 50),
                Size = new Size(80, 25),
                Text = "Extensions:",
                TextAlign = ContentAlignment.MiddleLeft
            };
            _tabPatternSearch.Controls.Add(lblExtensions);

            _txtExtensions = new TextBox
            {
                Location = new Point(100, 50),
                Size = new Size(400, 25),
                Text = ".mp3,.mp4,.avi,.mkv,.mov,.wmv,.flv,.exe,.msi,.iso,.dmg,.zip,.rar,.7z"
            };
            _tabPatternSearch.Controls.Add(_txtExtensions);

            Label lblExtensionsHelp = new Label
            {
                Location = new Point(510, 50),
                Size = new Size(500, 25),
                Text = "Comma-separated list (e.g., .mp3,.exe,.zip)",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _tabPatternSearch.Controls.Add(lblExtensionsHelp);

            // Min size filter
            Label lblMinSize = new Label
            {
                Location = new Point(10, 85),
                Size = new Size(80, 25),
                Text = "Min Size (MB):",
                TextAlign = ContentAlignment.MiddleLeft
            };
            _tabPatternSearch.Controls.Add(lblMinSize);

            _txtMinSize = new TextBox
            {
                Location = new Point(100, 85),
                Size = new Size(100, 25),
                Text = "0"
            };
            _tabPatternSearch.Controls.Add(_txtMinSize);

            // Modified since filter
            Label lblModified = new Label
            {
                Location = new Point(220, 85),
                Size = new Size(100, 25),
                Text = "Modified Since:",
                TextAlign = ContentAlignment.MiddleLeft
            };
            _tabPatternSearch.Controls.Add(lblModified);

            _dtpModifiedSince = new DateTimePicker
            {
                Location = new Point(325, 85),
                Size = new Size(200, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddMonths(-6),
                ShowCheckBox = true,
                Checked = false
            };
            _tabPatternSearch.Controls.Add(_dtpModifiedSince);

            // Include system locations checkbox
            _chkIncludeSystem = new CheckBox
            {
                Location = new Point(10, 120),
                Size = new Size(500, 25),
                Text = "⚠️ Include system locations (Windows, Program Files) - Use with caution",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.DarkRed
            };
            _tabPatternSearch.Controls.Add(_chkIncludeSystem);

            // Search results list
            _lvSearchResults = new ListView
            {
                Location = new Point(10, 155),
                Size = new Size(1020, 300),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 9),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            _lvSearchResults.Columns.Add("Path", 500);
            _lvSearchResults.Columns.Add("Size", 100);
            _lvSearchResults.Columns.Add("Modified", 150);
            _lvSearchResults.Columns.Add("Attributes", 120);
            _lvSearchResults.Columns.Add("Reviewed", 80);
            _lvSearchResults.DoubleClick += LvSearchResults_DoubleClick;
            _tabPatternSearch.Controls.Add(_lvSearchResults);

            // Progress bar
            _progressBar = new ProgressBar
            {
                Location = new Point(10, 465),
                Size = new Size(1020, 20),
                Style = ProgressBarStyle.Continuous,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            _tabPatternSearch.Controls.Add(_progressBar);

            // Status label
            _lblSearchStatus = new Label
            {
                Location = new Point(10, 490),
                Size = new Size(1020, 25),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.DarkBlue,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            _tabPatternSearch.Controls.Add(_lblSearchStatus);

            // Buttons
            _btnStartSearch = new Button
            {
                Location = new Point(10, 525),
                Size = new Size(120, 35),
                Text = "Start Search",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            _btnStartSearch.Click += BtnStartSearch_Click;
            _tabPatternSearch.Controls.Add(_btnStartSearch);

            _btnCancelSearch = new Button
            {
                Location = new Point(140, 525),
                Size = new Size(120, 35),
                Text = "Cancel",
                Enabled = false,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            _btnCancelSearch.Click += BtnCancelSearch_Click;
            _tabPatternSearch.Controls.Add(_btnCancelSearch);

            _btnExportResults = new Button
            {
                Location = new Point(270, 525),
                Size = new Size(120, 35),
                Text = "Export CSV",
                Enabled = false,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            _btnExportResults.Click += BtnExportResults_Click;
            _tabPatternSearch.Controls.Add(_btnExportResults);

            // Info label
            Label lblInfo = new Label
            {
                Location = new Point(400, 525),
                Size = new Size(620, 35),
                Text = "💡 Right-click results for actions: Open in Explorer, Copy Path, Mark Reviewed.\n" +
                       "Search is read-only and makes no file system changes.",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.DarkBlue,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            _tabPatternSearch.Controls.Add(lblInfo);

            // Context menu for search results
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Open in Explorer", null, (s, e) => OpenSelectedInExplorer(_lvSearchResults));
            contextMenu.Items.Add("Copy Path", null, (s, e) => CopySelectedPath(_lvSearchResults));
            contextMenu.Items.Add("Mark Reviewed", null, (s, e) => MarkSelectedReviewed());
            _lvSearchResults.ContextMenuStrip = contextMenu;
        }

        private void LoadUsers()
        {
            try
            {
                string usersPath = @"C:\Users";
                if (!Directory.Exists(usersPath))
                {
                    _lblUserExplorerStatus.Text = "⚠️ C:\\Users directory not found (not running on Windows)";
                    _lblUserExplorerStatus.ForeColor = Color.Red;
                    return;
                }

                var directories = Directory.GetDirectories(usersPath);
                var realUsers = directories
                    .Where(d => !Path.GetFileName(d).Equals("Public", StringComparison.OrdinalIgnoreCase) &&
                                !Path.GetFileName(d).Equals("Default", StringComparison.OrdinalIgnoreCase) &&
                                !Path.GetFileName(d).Equals("Default User", StringComparison.OrdinalIgnoreCase) &&
                                !Path.GetFileName(d).StartsWith("Default.", StringComparison.OrdinalIgnoreCase))
                    .Select(d => Path.GetFileName(d))
                    .ToList();

                if (realUsers.Count == 0)
                {
                    _lblUserExplorerStatus.Text = "⚠️ No user profiles found";
                    _lblUserExplorerStatus.ForeColor = Color.Orange;
                    return;
                }

                _cmbUsers.Items.Clear();
                foreach (var user in realUsers)
                {
                    _cmbUsers.Items.Add(user);
                }

                if (_cmbUsers.Items.Count > 0)
                {
                    _cmbUsers.SelectedIndex = 0;
                }

                _lblUserExplorerStatus.Text = $"✓ Found {realUsers.Count} user profile(s)";
                _lblUserExplorerStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                _lblUserExplorerStatus.Text = $"⚠️ Error loading users: {ex.Message}";
                _lblUserExplorerStatus.ForeColor = Color.Red;
            }
        }

        private void CmbUsers_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_cmbUsers.SelectedItem == null) return;

            string userName = _cmbUsers.SelectedItem.ToString()!;
            LoadUserFolders(userName);
        }

        private void LoadUserFolders(string userName)
        {
            _lvUserFiles.Items.Clear();
            _lblUserExplorerStatus.Text = "Loading...";

            string userPath = Path.Combine(@"C:\Users", userName);
            string[] defaultFolders = {
                "Desktop",
                "Documents",
                "Downloads",
                "Pictures",
                "Music",
                "Videos"
            };

            // Also check OneDrive\Documents
            string oneDriveDocs = Path.Combine(userPath, "OneDrive", "Documents");

            int totalFiles = 0;
            int totalFolders = 0;

            foreach (string folderName in defaultFolders)
            {
                string folderPath = Path.Combine(userPath, folderName);
                ScanFolderOneLevel(folderPath, folderName, ref totalFiles, ref totalFolders);
            }

            // Check OneDrive\Documents if it exists
            if (Directory.Exists(oneDriveDocs))
            {
                ScanFolderOneLevel(oneDriveDocs, "OneDrive\\Documents", ref totalFiles, ref totalFolders);
            }

            _lblUserExplorerStatus.Text = $"✓ Loaded {totalFolders} folders, {totalFiles} files from {userName}'s default locations";
            _lblUserExplorerStatus.ForeColor = Color.Green;
        }

        private void ScanFolderOneLevel(string folderPath, string folderDisplayName, ref int totalFiles, ref int totalFolders)
        {
            if (!Directory.Exists(folderPath))
                return;

            try
            {
                // Get directories (folders) - one level only
                var directories = Directory.GetDirectories(folderPath);
                foreach (var dir in directories)
                {
                    try
                    {
                        var dirInfo = new DirectoryInfo(dir);
                        var item = new ListViewItem("Folder");
                        item.SubItems.Add(folderDisplayName);
                        item.SubItems.Add(dirInfo.Name);
                        item.SubItems.Add("");  // No size for folders
                        item.SubItems.Add(dirInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));
                        item.SubItems.Add(GetAttributesString(dirInfo.Attributes));
                        item.Tag = dir;
                        item.ForeColor = Color.Blue;
                        _lvUserFiles.Items.Add(item);
                        totalFolders++;
                    }
                    catch
                    {
                        // Skip folders we can't access
                    }
                }

                // Get files - one level only
                var files = Directory.GetFiles(folderPath);
                foreach (var file in files)
                {
                    try
                    {
                        var fileInfo = new FileInfo(file);
                        var item = new ListViewItem("File");
                        item.SubItems.Add(folderDisplayName);
                        item.SubItems.Add(fileInfo.Name);
                        item.SubItems.Add(FormatFileSize(fileInfo.Length));
                        item.SubItems.Add(fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));
                        item.SubItems.Add(GetAttributesString(fileInfo.Attributes));
                        item.Tag = file;
                        _lvUserFiles.Items.Add(item);
                        totalFiles++;
                    }
                    catch
                    {
                        // Skip files we can't access
                    }
                }
            }
            catch
            {
                // Skip if we can't access the folder
            }
        }

        private string GetAttributesString(FileAttributes attributes)
        {
            List<string> attrs = new List<string>();
            if ((attributes & FileAttributes.Hidden) != 0) attrs.Add("Hidden");
            if ((attributes & FileAttributes.System) != 0) attrs.Add("System");
            if ((attributes & FileAttributes.ReadOnly) != 0) attrs.Add("ReadOnly");
            if ((attributes & FileAttributes.Archive) != 0) attrs.Add("Archive");
            return attrs.Count > 0 ? string.Join(", ", attrs) : "Normal";
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private void LvUserFiles_DoubleClick(object? sender, EventArgs e)
        {
            OpenSelectedInExplorer(_lvUserFiles);
        }

        private void BtnOpenExplorer_Click(object? sender, EventArgs e)
        {
            OpenSelectedInExplorer(_lvUserFiles);
        }

        private void OpenSelectedInExplorer(ListView listView)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an item first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedItem = listView.SelectedItems[0];
            string? path = selectedItem.Tag as string;

            if (string.IsNullOrEmpty(path))
                return;

            try
            {
                // Open File Explorer with the item selected
                Process.Start("explorer.exe", $"/select,\"{path}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Explorer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnOpenPowerShell_Click(object? sender, EventArgs e)
        {
            if (_lvUserFiles.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an item first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedItem = _lvUserFiles.SelectedItems[0];
            string? path = selectedItem.Tag as string;

            if (string.IsNullOrEmpty(path))
                return;

            string directory = File.Exists(path) ? Path.GetDirectoryName(path)! : path;

            try
            {
                // Start elevated PowerShell in that directory
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoExit -Command \"Set-Location '{directory}'\"",
                    Verb = "runas",  // Request elevation
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open PowerShell: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnStartSearch_Click(object? sender, EventArgs e)
        {
            // Validate inputs
            if (!decimal.TryParse(_txtMinSize.Text, out decimal minSizeMB) || minSizeMB < 0)
            {
                MessageBox.Show("Please enter a valid minimum size (0 or greater).", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Parse extensions
            var extensions = _txtExtensions.Text
                .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim().ToLowerInvariant())
                .Where(e => !string.IsNullOrEmpty(e))
                .Select(e => e.StartsWith(".") ? e : "." + e)
                .ToHashSet();

            if (extensions.Count == 0)
            {
                MessageBox.Show("Please enter at least one file extension.", "No Extensions", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Warn if including system locations
            if (_chkIncludeSystem.Checked)
            {
                var result = MessageBox.Show(
                    "⚠️ WARNING: Including system locations (Windows, Program Files) may:\n\n" +
                    "- Take significantly longer to complete\n" +
                    "- Return many legitimate system files\n" +
                    "- Require elevated permissions\n\n" +
                    "Are you sure you want to proceed?",
                    "System Locations Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                    return;
            }

            // Start search
            _lvSearchResults.Items.Clear();
            _searchResults.Clear();
            _btnStartSearch.Enabled = false;
            _btnCancelSearch.Enabled = true;
            _btnExportResults.Enabled = false;
            _progressBar.Style = ProgressBarStyle.Marquee;
            _lblSearchStatus.Text = "Searching...";
            _lblSearchStatus.ForeColor = Color.Blue;

            _searchCancellation = new CancellationTokenSource();
            _uiUpdateTimer.Start(); // Start the batched UI update timer

            try
            {
                await Task.Run(() => PerformSearch(extensions, minSizeMB, _searchCancellation.Token));

                // Process any remaining pending updates
                ProcessPendingUIUpdates();

                if (!_searchCancellation.Token.IsCancellationRequested)
                {
                    _lblSearchStatus.Text = $"✓ Search complete. Found {_searchResults.Count} file(s).";
                    _lblSearchStatus.ForeColor = Color.Green;
                }
            }
            catch (OperationCanceledException)
            {
                _lblSearchStatus.Text = "Search cancelled by user.";
                _lblSearchStatus.ForeColor = Color.Orange;
            }
            catch (Exception ex)
            {
                _lblSearchStatus.Text = $"Error during search: {ex.Message}";
                _lblSearchStatus.ForeColor = Color.Red;
            }
            finally
            {
                _uiUpdateTimer.Stop(); // Stop the timer
                _btnStartSearch.Enabled = true;
                _btnCancelSearch.Enabled = false;
                _btnExportResults.Enabled = _searchResults.Count > 0;
                _progressBar.Style = ProgressBarStyle.Continuous;
                _progressBar.Value = 0;
            }
        }

        private void PerformSearch(HashSet<string> extensions, decimal minSizeMB, CancellationToken cancellationToken)
        {
            long minSizeBytes = (long)(minSizeMB * 1024 * 1024);
            DateTime? modifiedSince = _dtpModifiedSince.Checked ? _dtpModifiedSince.Value : null;

            // Define search locations
            List<string> searchPaths = new List<string>();

            // Always search user locations
            string usersPath = @"C:\Users";
            if (Directory.Exists(usersPath))
            {
                searchPaths.Add(usersPath);
            }

            // Add Temp
            string tempPath = Path.GetTempPath();
            if (!string.IsNullOrEmpty(tempPath) && Directory.Exists(tempPath))
            {
                searchPaths.Add(tempPath);
            }

            // Optionally add system locations
            if (_chkIncludeSystem.Checked)
            {
                if (Directory.Exists(@"C:\Windows"))
                    searchPaths.Add(@"C:\Windows");
                if (Directory.Exists(@"C:\Program Files"))
                    searchPaths.Add(@"C:\Program Files");
                if (Directory.Exists(@"C:\Program Files (x86)"))
                    searchPaths.Add(@"C:\Program Files (x86)");
            }

            int foundCount = 0;

            foreach (var basePath in searchPaths)
            {
                SearchDirectory(basePath, extensions, minSizeBytes, modifiedSince, cancellationToken, ref foundCount);
            }
        }

        private void SearchDirectory(string path, HashSet<string> extensions, long minSizeBytes, DateTime? modifiedSince, CancellationToken cancellationToken, ref int foundCount)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            try
            {
                // Search files in current directory
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    try
                    {
                        var fileInfo = new FileInfo(file);
                        string ext = fileInfo.Extension.ToLowerInvariant();

                        // Check if matches criteria
                        if (extensions.Contains(ext) &&
                            fileInfo.Length >= minSizeBytes &&
                            (!modifiedSince.HasValue || fileInfo.LastWriteTime >= modifiedSince.Value))
                        {
                            var result = new FileSearchResult
                            {
                                Path = file,
                                Size = fileInfo.Length,
                                Modified = fileInfo.LastWriteTime,
                                Attributes = GetAttributesString(fileInfo.Attributes),
                                IsReviewed = false
                            };

                            _searchResults.Add(result);
                            foundCount++;

                            // Add to pending updates for batched UI update
                            lock (_pendingUIUpdates)
                            {
                                _pendingUIUpdates.Add(result);
                            }
                        }
                    }
                    catch
                    {
                        // Skip files we can't access
                    }
                }

                // Recurse into subdirectories
                var directories = Directory.GetDirectories(path);
                foreach (var dir in directories)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    SearchDirectory(dir, extensions, minSizeBytes, modifiedSince, cancellationToken, ref foundCount);
                }
            }
            catch
            {
                // Skip directories we can't access
            }
        }

        private void BtnCancelSearch_Click(object? sender, EventArgs e)
        {
            _searchCancellation?.Cancel();
            _lblSearchStatus.Text = "Cancelling search...";
            _lblSearchStatus.ForeColor = Color.Orange;
        }

        private void LvSearchResults_DoubleClick(object? sender, EventArgs e)
        {
            OpenSelectedInExplorer(_lvSearchResults);
        }

        private void CopySelectedPath(ListView listView)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an item first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedItem = listView.SelectedItems[0];
            string? path = null;

            if (selectedItem.Tag is string pathTag)
            {
                path = pathTag;
            }
            else if (selectedItem.Tag is FileSearchResult result)
            {
                path = result.Path;
            }

            if (!string.IsNullOrEmpty(path))
            {
                Clipboard.SetText(path);
                _lblSearchStatus.Text = $"Path copied to clipboard: {path}";
                _lblSearchStatus.ForeColor = Color.Green;
            }
        }

        private void MarkSelectedReviewed()
        {
            if (_lvSearchResults.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an item first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (ListViewItem item in _lvSearchResults.SelectedItems)
            {
                if (item.Tag is FileSearchResult result)
                {
                    result.IsReviewed = true;
                    item.SubItems[4].Text = "Yes";
                    item.BackColor = Color.LightGray;
                }
            }

            int reviewedCount = _searchResults.Count(r => r.IsReviewed);
            _lblSearchStatus.Text = $"Marked {_lvSearchResults.SelectedItems.Count} item(s) as reviewed. Total reviewed: {reviewedCount}/{_searchResults.Count}";
            _lblSearchStatus.ForeColor = Color.Green;
        }

        private void BtnExportResults_Click(object? sender, EventArgs e)
        {
            if (_searchResults.Count == 0)
            {
                MessageBox.Show("No results to export.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = "csv",
                FileName = $"FileSearchResults_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (saveDialog.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    {
                        // Export as JSON (basic implementation)
                        var json = JsonSerializer.Serialize(_searchResults, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(saveDialog.FileName, json);
                    }
                    else
                    {
                        // Export as CSV
                        using StreamWriter writer = new StreamWriter(saveDialog.FileName);
                        writer.WriteLine("Path,Size,SizeFormatted,Modified,Attributes,Reviewed");
                        foreach (var result in _searchResults)
                        {
                            writer.WriteLine($"\"{result.Path}\",{result.Size},\"{FormatFileSize(result.Size)}\",\"{result.Modified:yyyy-MM-dd HH:mm:ss}\",\"{result.Attributes}\",{result.IsReviewed}");
                        }
                    }

                    MessageBox.Show($"Results exported successfully to:\n{saveDialog.FileName}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _lblSearchStatus.Text = $"Results exported to: {Path.GetFileName(saveDialog.FileName)}";
                    _lblSearchStatus.ForeColor = Color.Green;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to export results: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private class FileSearchResult
        {
            public string Path { get; set; } = string.Empty;
            public long Size { get; set; }
            public DateTime Modified { get; set; }
            public string Attributes { get; set; } = string.Empty;
            public bool IsReviewed { get; set; }
        }
    }
}
