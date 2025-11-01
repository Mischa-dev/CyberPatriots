# File System Tools - User Guide

## Overview

The File System Tools feature helps CyberPatriot competitors quickly identify questionable files on a Windows system. It provides two main capabilities:

1. **User Explorer** - View user folders one level deep
2. **Global Pattern Search** - Hunt for specific file types across the system

## Important: Read-Only by Default

⚠️ **This feature is read-only.** It does not delete or modify any files automatically. You must manually review findings and take appropriate action using File Explorer or PowerShell.

## User Explorer

### Purpose
Quickly review the contents of each user's default folders to identify suspicious files.

### How It Works
1. Click "File Sweep" button on the main dashboard
2. Select the "User Explorer" tab
3. Choose a user from the dropdown
4. Review files and folders from these locations (one level deep):
   - Desktop
   - Documents
   - Downloads
   - Pictures
   - Music
   - Videos
   - OneDrive\Documents (if present)

### Features
- **Hidden/System Files**: All files are shown, including hidden and system files
- **One Level Deep**: Shows only immediate contents (no subfolders within subfolders)
- **File Attributes**: Displays file size, modified date, and attributes (Hidden, System, etc.)

### Actions
- **Double-click** any item to open it in File Explorer
- **Open in Explorer** button: Opens the selected file/folder location
- **Open Elevated PowerShell** button: Opens PowerShell as Administrator at the selected location (for manual deletions/inspection)

## Global Pattern Search

### Purpose
Search the entire system for specific file types that might be prohibited in CyberPatriot competitions.

### Default Search Settings
- **Extensions**: `.mp3,.mp4,.avi,.mkv,.mov,.wmv,.flv,.exe,.msi,.iso,.dmg,.zip,.rar,.7z`
- **Scope**: User-writable locations only (C:\Users\, Temp)
- **Min Size**: 0 MB (all files)
- **Modified Since**: Disabled (all dates)

### How to Use
1. Click "File Sweep" button on the main dashboard
2. Select the "Global Pattern Search" tab
3. Customize filters as needed:
   - **Extensions**: Comma-separated list (e.g., `.mp3,.exe,.zip`)
   - **Min Size**: Minimum file size in MB (useful for ignoring small files)
   - **Modified Since**: Only show files modified after a specific date
   - **Include System Locations**: ⚠️ Check to search Windows, Program Files (slower, may return many legitimate files)
4. Click "Start Search"
5. Results stream in real-time as they're found
6. Click "Cancel" to stop the search early

### Search Actions
- **Double-click** a result to open it in File Explorer
- **Right-click** for additional options:
  - **Open in Explorer**: Navigate to the file location
  - **Copy Path**: Copy the full file path to clipboard
  - **Mark Reviewed**: Mark the file as reviewed (visual indicator)
- **Export CSV**: Save all results to a CSV file for later analysis

### Search Scope

#### Default (User-writable locations)
- C:\Users\ (all user profiles)
- C:\Users\Public
- Windows Temp folders

#### With "Include System Locations" enabled
⚠️ **WARNING**: This option:
- Takes significantly longer
- Returns many legitimate system files
- May require Administrator privileges
- Searches:
  - C:\Windows
  - C:\Program Files
  - C:\Program Files (x86)

## Tips

### User Explorer
- Start with the Downloads folder - often contains unauthorized files
- Check Desktop for media files and games
- Look for Hidden/System attributes on suspicious files in Documents

### Pattern Search
- Use the "Modified Since" filter to focus on recent files
- Start with default scope first, only enable system locations if needed
- Export results to CSV for reporting or documentation
- Mark files as "Reviewed" to track your progress

### Performance
- Searches run on a background thread and won't freeze the UI
- Cancel button stops the search immediately
- Results stream as they're found (no waiting for full completion)

## Security Notes

1. **No Automatic Changes**: This tool makes NO changes to the file system
2. **Administrator Rights**: Some files/folders may be inaccessible without admin rights
3. **Elevation Banner**: A warning appears if not running as Administrator
4. **Manual Actions Only**: Use File Explorer or PowerShell to delete/modify files

## Common Workflows

### Finding Unauthorized Media Files
1. Open Global Pattern Search
2. Set extensions: `.mp3,.mp4,.avi,.mkv,.mov`
3. Set min size: `1` (MB) to ignore tiny files
4. Search default scope
5. Review results and mark legitimate files as reviewed
6. Export to CSV for documentation

### Checking New User Downloads
1. Open User Explorer
2. Select the user
3. Focus on Desktop and Downloads folders
4. Double-click suspicious items to open in Explorer
5. Use PowerShell option if you need to delete files

### Pre-Competition System Scan
1. Run Pattern Search with default settings
2. Export results to CSV
3. Review offline
4. Return to mark files as reviewed as you address them
5. Re-run search to verify cleanup

## Troubleshooting

### "Not running as Administrator" warning
- Some files and folders may be inaccessible
- Relaunch the application as Administrator for full access
- Right-click CyberPatriotHelper.exe → Run as Administrator

### No users found in User Explorer
- Verify you're running on Windows
- Check that C:\Users exists
- Try running as Administrator

### Search returns no results
- Verify your extension filters are correct (include the dot, e.g., `.mp3`)
- Check that files matching your criteria exist
- Try reducing the minimum size filter
- Disable the "Modified Since" filter

### Search is slow
- Reduce the number of extensions
- Increase the minimum size filter
- Avoid enabling "Include System Locations"
- Cancel and restart with more restrictive filters

## Keyboard Shortcuts
- **Double-click**: Open in Explorer
- **Right-click**: Context menu (Pattern Search results)
- **Escape**: Close dialog (if modal)

## Related Features
- Use the main dashboard security checks to address system hardening
- Use File Explorer for manual file operations
- Use PowerShell for batch operations or scripting
