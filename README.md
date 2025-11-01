# CyberPatriots

## CyberPatriot Windows Helper

A lightweight Windows 10/11 security dashboard application for CyberPatriot competitions.

### Features

**Read-Only Dashboard**
- Flags core security settings without making automatic changes
- Visual status indicators (Green = Pass, Red = Fail)
- Real-time security status summary

**Core Security Checks**
- ✅ Windows Firewall status
- ✅ Guest Account enabled/disabled
- ✅ Remote Desktop (RDP) configuration
- ✅ Windows Defender status
- ✅ SMBv1 protocol (vulnerable legacy protocol)

**For Each Security Check**
- **Explanation**: What this setting controls
- **Why It Matters**: Security implications explained clearly
- **Evidence**: Technical output showing current state
- **One-Click Fix**: Automatic remediation with admin privileges
- **Manual Steps**: Alternative instructions if automatic fix fails
- **Verify**: Confirm the fix was applied correctly

**Repeatable Pattern**
- Easy to add new security checks
- Consistent user experience
- Extensible architecture

**Future Features**
- File Sweep: Scan user profiles for unauthorized files
- AI Integration: Parse README/challenges to generate action plans

### Getting Started

#### Prerequisites
- Windows 10/11 or Windows Server
- .NET 8.0 Runtime
- Administrator privileges (for applying fixes)

#### Running the Application
1. Download the latest release
2. Right-click `CyberPatriotHelper.exe` → Run as Administrator
3. Review security checks on the dashboard
4. Double-click any item to view details and apply fixes

#### Building from Source
```bash
cd CyberPatriotHelper
dotnet build
dotnet run
```

For more details, see [Developer Documentation](CyberPatriotHelper/DEVELOPER_README.md)

### Screenshots

The application provides:
- Main dashboard with security check list
- Detailed popup for each check with Fix and Verify buttons
- Color-coded status (Green/Red/Orange/Gray)
- Read-only by design - nothing changes automatically

### Architecture

The core pattern is the `SecurityCheck` abstract class that makes adding new checks easy:
- Each check knows how to diagnose itself
- Each check can fix itself automatically
- Each check can verify the fix
- All checks display the same UI pattern

### Contributing

To add a new security check:
1. Create a new class inheriting from `SecurityCheck`
2. Implement `Check()`, `Fix()`, and set appropriate properties
3. Add to the dashboard initialization

See [Developer Documentation](CyberPatriotHelper/DEVELOPER_README.md) for details.

### License

Open source for CyberPatriot training and education.
