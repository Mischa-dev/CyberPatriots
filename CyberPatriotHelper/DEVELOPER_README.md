# CyberPatriot Security Helper - Developer Documentation

## Overview
A Windows Forms application designed for CyberPatriot competitions. It provides a read-only dashboard that flags core security settings and offers one-click fixes with manual alternatives.

## Architecture

### Core Pattern: SecurityCheck
The foundation of the application is the `SecurityCheck` abstract class, which provides a repeatable pattern for adding new security checks:

```csharp
public abstract class SecurityCheck
{
    public string Name { get; }                // Check name
    public string Description { get; }         // Brief description
    public string WhyItMatters { get; }        // Explanation of importance
    public CheckStatus Status { get; }         // Pass/Fail/Warning/Unknown
    public string Evidence { get; }            // Technical output
    public string ManualSteps { get; }         // Manual fix instructions
    
    public abstract void Check();              // Run the check
    public abstract bool Fix();                // Apply automatic fix
    public virtual bool Verify();              // Verify the fix
}
```

### Current Security Checks
1. **FirewallCheck** - Ensures Windows Firewall is enabled
2. **GuestAccountCheck** - Verifies Guest account is disabled
3. **RDPCheck** - Checks if RDP is disabled when not needed
4. **DefenderCheck** - Confirms Windows Defender is active
5. **SMBv1Check** - Ensures outdated SMBv1 protocol is disabled

### UI Components
- **MainDashboard**: Read-only dashboard showing all security checks
- **CheckDetailDialog**: Reusable popup showing details with Fix/Verify buttons

## Adding New Security Checks

To add a new security check:

1. Create a new class in `Checks/` folder inheriting from `SecurityCheck`
2. Implement the required properties and methods:
   - Constructor: Set Name, Description, WhyItMatters, ManualSteps
   - `Check()`: Run diagnostic commands and set Status/Evidence
   - `Fix()`: Apply automatic fix (return true on success)
3. Add the check to `MainDashboard.InitializeChecks()`

Example:
```csharp
public class NewCheck : SecurityCheck
{
    public NewCheck()
    {
        Name = "Check Name";
        Description = "What this checks";
        WhyItMatters = "Why this is important";
        ManualSteps = "How to fix manually";
    }
    
    public override void Check()
    {
        // Run diagnostic commands
        // Set Status and Evidence
    }
    
    public override bool Fix()
    {
        // Apply fix
        // Return true if successful
    }
}
```

## Building

```bash
# Build
dotnet build

# Publish (Windows only)
dotnet publish -c Release -r win-x64 --self-contained
```

## Running

**Important**: Many security checks require administrator privileges. Run the application as Administrator for full functionality.

## File Structure
```
CyberPatriotHelper/
├── Models/
│   └── SecurityCheck.cs          # Base class for all checks
├── Checks/
│   ├── FirewallCheck.cs
│   ├── GuestAccountCheck.cs
│   ├── RDPCheck.cs
│   ├── DefenderCheck.cs
│   └── SMBv1Check.cs
├── UI/
│   └── CheckDetailDialog.cs      # Reusable detail dialog
├── MainDashboard.cs              # Main application window
├── Program.cs                    # Entry point
└── CyberPatriotHelper.csproj
```

## Future Enhancements

### File Sweep Feature
Scan user profiles for:
- Prohibited media files (mp3, mp4, avi, etc.)
- Unauthorized software
- Suspicious executables

Implementation approach:
```csharp
PowerShell: Get-ChildItem C:\Users -Recurse -Include *.mp3,*.mp4
```

### AI Integration (Optional)
- Parse README.md and challenge files
- Generate prioritized action plan
- Answer questions about security configurations

Integration point: Add `AIHelper` class with methods:
```csharp
public class AIHelper
{
    public string GenerateActionPlan(string readmeContent);
    public string AnswerQuestion(string question);
}
```

## Design Principles
1. **Read-Only Dashboard**: No automatic changes without user confirmation
2. **Repeatable Pattern**: All checks follow the same structure
3. **One-Click Fix + Manual Alternative**: Always provide both options
4. **Evidence-Based**: Show technical output for transparency
5. **Educational**: Explain why each check matters

## Testing on Windows
1. Build the project
2. Run as Administrator
3. Test each security check:
   - Click to view details
   - Try the Fix button
   - Verify the changes
   - Review evidence output

## Security Considerations
- Always request elevation for system changes
- Show clear warnings before applying fixes
- Provide rollback instructions where applicable
- Never store credentials or sensitive data
