# CyberPatriot Security Helper - User Guide

## What is this application?

CyberPatriot Security Helper is a Windows application designed to help you identify and fix common security issues on Windows 10/11 systems. It's particularly useful for CyberPatriot competitions.

## Important Notice

**This application is READ-ONLY by design.** It will NOT make any changes to your system automatically. All fixes require your explicit approval.

## Getting Started

### 1. Running the Application

**You MUST run this application as Administrator** to:
- Check system security settings
- Apply fixes to security issues

**How to run as Administrator:**
1. Right-click on `CyberPatriotHelper.exe`
2. Select "Run as administrator"
3. Click "Yes" when prompted by User Account Control

### 2. Understanding the Dashboard

When you open the application, you'll see a list of security checks:

- **Green items** ✓ - Security check is passing (no action needed)
- **Red items** ✗ - Security issue found (needs attention)
- **Orange items** ⚠ - Warning (review recommended)
- **Gray items** ? - Status unknown (may need admin rights)

### 3. Viewing Check Details

**Double-click any item** to open a detailed view showing:

1. **Explanation** - What this security setting is
2. **Why It Matters** - Why this is important for security
3. **Evidence** - Technical details about the current state
4. **Manual Steps** - How to fix it manually if needed

### 4. Fixing Issues

In the detail view, you have two options:

#### Option A: One-Click Fix
1. Click the **"Fix Now"** button
2. Confirm when prompted
3. Wait for the fix to apply
4. The application will automatically verify the fix

#### Option B: Manual Fix
1. Read the "Manual Steps" section
2. Follow the step-by-step instructions
3. Apply the fix manually using Windows settings
4. Click **"Verify"** to confirm it worked

### 5. Verifying Fixes

After applying a fix:
1. Click the **"Verify"** button
2. The application will re-check the security setting
3. You'll see a confirmation if the issue is resolved
4. The main dashboard will update automatically

## Security Checks Explained

### Windows Firewall
**What it checks:** Whether Windows Firewall is enabled for all network profiles (Domain, Private, Public)
**Why it matters:** The firewall blocks unauthorized network access

### Guest Account
**What it checks:** Whether the built-in Guest account is disabled
**Why it matters:** Guest accounts can allow unauthorized access

### Remote Desktop (RDP)
**What it checks:** Whether Remote Desktop is disabled
**Why it matters:** RDP is a common target for attackers. Disable it if you don't need remote access

### Windows Defender
**What it checks:** Whether Windows Defender real-time protection is enabled
**Why it matters:** Defender protects against viruses and malware

### SMBv1 Protocol
**What it checks:** Whether the outdated SMBv1 protocol is disabled
**Why it matters:** SMBv1 has known vulnerabilities exploited by ransomware like WannaCry

## Additional Features

### Refresh All
Click the **"Refresh All"** button to re-run all security checks and update the dashboard.

### File Sweep (Coming Soon)
This feature will scan user profiles for:
- Unauthorized media files
- Suspicious executables
- Prohibited software

## Troubleshooting

### "Fix Failed" Messages
**Cause:** The application may not have sufficient permissions
**Solution:** 
1. Close the application
2. Right-click and "Run as administrator"
3. Try the fix again

### Status Shows "Unknown"
**Cause:** Unable to check the setting (usually permissions)
**Solution:** Run the application as Administrator

### Fix Doesn't Work
**Solution:** Use the Manual Steps provided in the detail view

## Best Practices

1. ✅ **Always run as Administrator**
2. ✅ **Read the "Why It Matters" section** before applying fixes
3. ✅ **Use Verify after each fix** to confirm it worked
4. ✅ **Refresh the dashboard** after making multiple changes
5. ✅ **Keep the Evidence** - It shows what commands were run

## Tips for CyberPatriot Competitions

1. **Start with critical items** - Fix failing (red) checks first
2. **Document your actions** - The Evidence section shows what was done
3. **Verify everything** - Always confirm fixes worked
4. **Use manual steps** - Learn what the fixes actually do
5. **Keep this tool running** - Refresh periodically to catch new issues

## Safety Features

- ✓ No automatic changes without confirmation
- ✓ Shows exactly what commands will be run
- ✓ Provides manual rollback instructions
- ✓ Read-only dashboard prevents accidents
- ✓ Clear warnings before applying fixes

## Getting Help

If you encounter issues:
1. Check the Evidence section for error messages
2. Try the Manual Steps alternative
3. Consult Windows documentation for the specific setting
4. Review the Developer Documentation for technical details

## System Requirements

- Windows 10 or Windows 11
- .NET 8.0 Runtime or later
- Administrator privileges
- 10 MB disk space

---

**Remember:** This tool is designed to HELP you understand and fix security issues, not to do everything automatically. Always understand what you're changing and why!
