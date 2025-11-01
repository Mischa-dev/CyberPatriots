# GitHub Copilot Instructions for CyberPatriots

## Project Overview

This repository contains **CyberPatriot Security Helper**, a Windows Forms application designed for CyberPatriot competitions. It helps identify and fix common Windows security issues through a read-only dashboard with one-click fixes.

### Core Purpose
- Provide a security assessment dashboard for Windows 10/11 systems
- Flag security issues without making automatic changes
- Offer one-click fixes with UAC elevation for identified issues
- Educate users about security best practices

## Technology Stack

- **Framework**: .NET 8.0
- **UI Framework**: Windows Forms
- **Language**: C# 12
- **Target OS**: Windows 10/11, Windows Server 2016+
- **Build System**: dotnet CLI / Visual Studio 2022

## Architecture

### Core Pattern: SecurityCheck Abstract Class

The application is built around a repeatable pattern using the `SecurityCheck` abstract base class:

```csharp
public abstract class SecurityCheck
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public string WhyItMatters { get; protected set; }
    public CheckStatus Status { get; protected set; }
    public string Evidence { get; protected set; }
    public string ManualSteps { get; protected set; }
    
    public abstract void Check();
    public abstract bool Fix();
    public virtual bool Verify();
}
```

### Adding New Security Checks

When adding a new security check:
1. Create a new class inheriting from `SecurityCheck` in the `Checks/` directory
2. Implement `Check()` method to diagnose the security issue
3. Implement `Fix()` method to remediate the issue (with UAC elevation if needed)
4. Set all required properties (Name, Description, WhyItMatters, ManualSteps)
5. Add the check to the dashboard initialization in `MainDashboard.cs`

### Project Structure

```
CyberPatriotHelper/
├── Program.cs              # Entry point
├── MainDashboard.cs        # Main UI dashboard
├── Models/
│   └── SecurityCheck.cs    # Base pattern class
├── Checks/                 # Security check implementations
│   ├── FirewallCheck.cs
│   ├── GuestAccountCheck.cs
│   ├── RDPCheck.cs
│   ├── DefenderCheck.cs
│   └── SMBv1Check.cs
└── UI/
    └── CheckDetailDialog.cs # Detail popup dialog
```

## Coding Standards

### General Guidelines

1. **No Automatic Changes**: The application is read-only by design. No security settings should be changed without explicit user confirmation.

2. **UAC Elevation**: Any operation that modifies system settings must:
   - Request UAC elevation
   - Show confirmation dialog before executing
   - Provide feedback on success/failure

3. **Evidence Display**: Always show raw command output or system state to provide transparency.

4. **Error Handling**: 
   - Use specific exception types (avoid empty catch blocks)
   - Provide meaningful error messages to users
   - Log errors with sufficient context

5. **Security**: 
   - Never bypass UAC or security checks
   - Validate all input from external sources
   - Use parameterized commands when executing PowerShell/cmd

### C# Conventions

- Use C# 12 features appropriately
- Follow Microsoft's C# coding conventions
- Use null-coalescing and null-conditional operators
- Prefer `async`/`await` for I/O operations where appropriate
- Use proper disposal patterns for system resources

### Windows API Interactions

- Prefer executing standard Windows commands (netsh, PowerShell cmdlets) over P/Invoke when possible
- Always check for command execution success
- Parse output carefully and handle unexpected formats gracefully

## Building and Testing

### Build Commands

```bash
# Build the solution
dotnet build

# Build in Release mode
dotnet build -c Release

# Run the application
dotnet run --project CyberPatriotHelper/CyberPatriotHelper.csproj
```

### Testing Considerations

- **Administrator Rights Required**: Most security checks require elevated privileges
- **Windows-Only**: This application only runs on Windows operating systems
- **Manual Testing**: Focus on manual testing of UI interactions and security fixes
- **No Unit Tests Yet**: The project doesn't have automated tests; focus on integration testing

### Running Locally

1. Open `CyberPatriotHelper.sln` in Visual Studio 2022
2. Build the solution (Ctrl+Shift+B)
3. Run as Administrator (required for most security checks)
4. Test security checks in the dashboard

## Security Considerations

### Important Security Requirements

1. **Privilege Escalation**: Always use proper UAC elevation, never attempt to bypass
2. **Command Injection**: Sanitize any dynamic inputs used in commands
3. **Registry Safety**: When modifying registry, verify paths and values
4. **Rollback**: Consider implementing rollback capabilities for critical changes
5. **Audit Trail**: Log all security changes made by the application

### Windows Security APIs

When working with Windows security:
- Use .NET's built-in classes where possible (e.g., `WindowsPrincipal`, `WindowsIdentity`)
- For PowerShell commands, use `PowerShell.Create()` from `System.Management.Automation`
- For command-line tools, use `Process.Start()` with proper security settings

## Common Tasks

### Adding a New Security Check

1. Create a new class file in `Checks/` directory
2. Inherit from `SecurityCheck`
3. Implement the pattern:
   ```csharp
   public class MyNewCheck : SecurityCheck
   {
       public MyNewCheck()
       {
           Name = "Descriptive Name";
           Description = "What this checks";
           WhyItMatters = "Security implications";
           ManualSteps = "Step-by-step fix instructions";
       }

       public override void Check()
       {
           // Diagnose the issue
           // Set Status (Pass, Fail, Warning, Unknown)
           // Set Evidence with diagnostic output
       }

       public override bool Fix()
       {
           // Fix the issue with UAC elevation
           // Return true if successful, false otherwise
           // Check() is automatically called by Verify()
           return true;
       }
   }
   ```
4. Add to dashboard in `MainDashboard.cs`:
   ```csharp
   private void InitializeChecks()
   {
       _checks = new List<SecurityCheck>
       {
           new FirewallCheck(),
           new GuestAccountCheck(),
           new MyNewCheck(),  // Add your new check here
           // ... other checks
       };
   }
   ```

### Modifying the UI

- Use Windows Forms Designer in Visual Studio for layout changes
- Keep the UI simple and focused on the security dashboard
- Maintain color-coding: Green (Pass), Red (Fail), Orange (Warning), Gray (Unknown)
- Follow the existing pattern for consistency

### Documentation Updates

When making significant changes:
1. Update `README.md` if user-facing features change
2. Update `DEVELOPER_README.md` for architectural changes
3. Update `USER_GUIDE.md` for new security checks or UI changes
4. Keep `PROJECT_SUMMARY.md` current with major milestones

## Future Enhancements

### Planned Features

1. **File Sweep**: Scan user profiles for unauthorized files (media, executables)
2. **AI Integration**: Parse challenge README files to generate action plans
3. **Additional Checks**: Password policies, Windows Update, network sharing, audit policies
4. **Export Reports**: Generate security assessment reports

When implementing these features, maintain the same pattern-based architecture.

## References

- [CyberPatriot Competition Rules](https://www.uscyberpatriot.org/)
- [Windows Security Baseline](https://docs.microsoft.com/en-us/windows/security/threat-protection/windows-security-baselines)
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Windows Forms Documentation](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/)

## Notes for AI Assistance

- This is a **Windows-only** application; don't suggest cross-platform solutions
- The application is **intentionally simple**; don't over-engineer solutions
- **Security is paramount**; always consider the security implications of changes
- **User education** is a goal; help users understand what they're changing
- The **repeatable pattern** is the core design principle; preserve it in all changes
