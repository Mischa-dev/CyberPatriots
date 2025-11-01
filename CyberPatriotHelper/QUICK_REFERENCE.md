# CyberPatriot Security Helper - Quick Reference

## Application Overview
A read-only security dashboard for Windows 10/11 that helps identify and fix security issues.

## Quick Start
1. Run as Administrator (right-click → "Run as administrator")
2. Review the security dashboard
3. Double-click any item for details
4. Click "Fix Now" or follow manual steps
5. Click "Verify" to confirm

## Security Checks

| Check | What It Does | Pass Condition |
|-------|--------------|----------------|
| 🔥 Firewall | Checks Windows Firewall is enabled | All profiles ON |
| 👤 Guest Account | Verifies Guest account is disabled | Account inactive |
| 🖥️ Remote Desktop | Checks RDP is disabled | RDP disabled |
| 🛡️ Defender | Confirms Windows Defender is active | Real-time protection ON |
| 📁 SMBv1 | Ensures SMBv1 protocol is disabled | SMBv1 disabled |

## Status Colors
- 🟢 **Green** = Pass (no action needed)
- 🔴 **Red** = Fail (requires attention)
- 🟠 **Orange** = Warning (review recommended)
- ⚪ **Gray** = Unknown (check permissions)

## Key Buttons

### Main Dashboard
- **Refresh All** - Re-run all security checks
- **File Sweep** - Scan for prohibited files (future)

### Detail Dialog
- **Fix Now** - Apply automatic fix (requires admin)
- **Verify** - Confirm the fix worked
- **Close** - Return to dashboard

## Dialog Sections
1. **Explanation** - What this setting controls
2. **Why It Matters** - Security importance
3. **Evidence** - Technical output/proof
4. **Manual Steps** - Alternative fix instructions

## Common Commands

### Check Firewall
```cmd
netsh advfirewall show allprofiles state
```

### Disable Guest Account
```cmd
net user guest /active:no
```

### Disable RDP
```cmd
reg add "HKLM\System\CurrentControlSet\Control\Terminal Server" /v fDenyTSConnections /t REG_DWORD /d 1 /f
```

### Enable Defender
```powershell
Set-MpPreference -DisableRealtimeMonitoring $false
```

### Disable SMBv1
```powershell
Disable-WindowsOptionalFeature -Online -FeatureName SMB1Protocol -NoRestart
```

## Tips
✅ Always run as Administrator
✅ Fix red items first
✅ Verify after each fix
✅ Read "Why It Matters"
✅ Use manual steps if auto-fix fails

## Workflow
```
Start Application
       ↓
  View Dashboard
       ↓
  Double-Click Item
       ↓
  Read Details
       ↓
  Click "Fix Now"
       ↓
  Auto Verify
       ↓
  Return to Dashboard
       ↓
  Click "Refresh All"
```

## Safety
- ✓ No automatic changes
- ✓ Confirmation required
- ✓ Shows exact commands
- ✓ Manual alternative provided
- ✓ Verify function included

## For Developers
- Base class: `SecurityCheck`
- Location: `Models/SecurityCheck.cs`
- Add new check: Inherit and implement `Check()` and `Fix()`
- See: `DEVELOPER_README.md`

## File Locations
- Main app: `CyberPatriotHelper.exe`
- User Guide: `USER_GUIDE.md`
- Developer Docs: `DEVELOPER_README.md`
- Source: `CyberPatriotHelper/` folder

## System Requirements
- Windows 10/11 or Windows Server
- .NET 8.0 Runtime
- Administrator privileges
- 10 MB disk space

## Build & Run
```bash
# Build
dotnet build

# Run (requires Windows)
dotnet run

# Publish
dotnet publish -c Release -r win-x64 --self-contained
```

---
**Remember:** Understand what you're changing and why. This tool helps you learn, not just click buttons!
