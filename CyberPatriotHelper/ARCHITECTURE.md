# Application Architecture

## Component Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                      CyberPatriot Security Helper               │
│                         Windows Forms App                        │
└─────────────────────────────────────────────────────────────────┘
                                 │
                                 ▼
        ┌────────────────────────────────────────────┐
        │           Program.cs (Entry Point)          │
        └────────────────────────────────────────────┘
                                 │
                                 ▼
        ┌────────────────────────────────────────────┐
        │         MainDashboard.cs (Main UI)          │
        │  ┌──────────────────────────────────────┐  │
        │  │  ListView - Shows all checks         │  │
        │  │  - Color coded status                │  │
        │  │  - Pass/Fail indicators              │  │
        │  └──────────────────────────────────────┘  │
        │  ┌──────────────┐  ┌──────────────────┐   │
        │  │ Refresh All  │  │   File Sweep     │   │
        │  └──────────────┘  └──────────────────┘   │
        └────────────────────────────────────────────┘
                                 │
                   ┌─────────────┴─────────────┐
                   │  Double-Click on Item     │
                   └─────────────┬─────────────┘
                                 ▼
        ┌────────────────────────────────────────────┐
        │    CheckDetailDialog.cs (Popup Dialog)      │
        │  ┌──────────────────────────────────────┐  │
        │  │  Explanation                         │  │
        │  │  Why It Matters                      │  │
        │  │  Evidence (Technical Output)         │  │
        │  │  Manual Steps                        │  │
        │  └──────────────────────────────────────┘  │
        │  ┌──────────┐  ┌──────────┐  ┌────────┐  │
        │  │ Fix Now  │  │  Verify  │  │ Close  │  │
        │  └──────────┘  └──────────┘  └────────┘  │
        └────────────────────────────────────────────┘
                                 │
                                 ▼
        ┌────────────────────────────────────────────┐
        │     SecurityCheck (Abstract Base Class)     │
        │  ┌──────────────────────────────────────┐  │
        │  │  Properties:                         │  │
        │  │  - Name, Description                 │  │
        │  │  - WhyItMatters, Status             │  │
        │  │  - Evidence, ManualSteps            │  │
        │  └──────────────────────────────────────┘  │
        │  ┌──────────────────────────────────────┐  │
        │  │  Methods:                            │  │
        │  │  - Check()  : Run diagnostic        │  │
        │  │  - Fix()    : Apply automatic fix   │  │
        │  │  - Verify() : Confirm fix worked    │  │
        │  └──────────────────────────────────────┘  │
        └────────────────────────────────────────────┘
                                 │
                ┌────────────────┼────────────────┐
                │                │                │
                ▼                ▼                ▼
    ┌──────────────────┐ ┌──────────────┐ ┌─────────────┐
    │ FirewallCheck    │ │ GuestAccount │ │  RDPCheck   │
    │                  │ │    Check     │ │             │
    │ - Check firewall │ │ - Check guest│ │ - Check RDP │
    │ - Enable it      │ │ - Disable it │ │ - Disable it│
    └──────────────────┘ └──────────────┘ └─────────────┘
    
                ▼                ▼
    ┌──────────────────┐ ┌──────────────┐
    │  DefenderCheck   │ │  SMBv1Check  │
    │                  │ │              │
    │ - Check Defender │ │ - Check SMBv1│
    │ - Enable it      │ │ - Disable it │
    └──────────────────┘ └──────────────┘
```

## Data Flow

```
User Action                  System Response
───────────                  ───────────────
   
1. Start App         →       Load MainDashboard
                             Initialize SecurityChecks
                             
2. App Loads         →       Run Check() on all items
                             Display status in ListView
                             Update summary counts
                             
3. Double-Click      →       Open CheckDetailDialog
   Item                      Load check details
                             Show Evidence, Steps
                             
4. Click "Fix Now"   →       Show confirmation
                             Call check.Fix()
                             Request Admin elevation
                             
5. Fix Applies       →       Execute system commands
                             Update registry/settings
                             Return success/failure
                             
6. Auto Verify       →       Call check.Verify()
                             Run Check() again
                             Update status
                             Show result message
                             
7. Close Dialog      →       Refresh ListView item
                             Update status color
                             Recalculate summary
                             
8. Click "Refresh"   →       Re-run all Check() methods
                             Update entire dashboard
                             Show new status
```

## Class Hierarchy

```
System.Windows.Forms.Form
    │
    ├── MainDashboard
    │   ├── Contains: List<SecurityCheck>
    │   ├── Contains: ListView (displays checks)
    │   └── Creates: CheckDetailDialog (on double-click)
    │
    └── CheckDetailDialog
        └── Contains: SecurityCheck (reference)

Abstract SecurityCheck
    │
    ├── FirewallCheck
    │   └── Uses: netsh commands
    │
    ├── GuestAccountCheck
    │   └── Uses: net user commands
    │
    ├── RDPCheck
    │   └── Uses: reg commands
    │
    ├── DefenderCheck
    │   └── Uses: PowerShell Get-MpComputerStatus
    │
    └── SMBv1Check
        └── Uses: PowerShell WindowsOptionalFeature
```

## File Structure

```
CyberPatriots/
│
├── README.md                           # Main project README
├── .gitignore                          # Git ignore rules
├── RunAsAdmin.bat                      # Helper script
├── CyberPatriotHelper.sln             # Visual Studio solution
│
└── CyberPatriotHelper/                # Main project folder
    │
    ├── CyberPatriotHelper.csproj      # Project file
    ├── Program.cs                      # Entry point
    ├── MainDashboard.cs               # Main UI form
    │
    ├── DEVELOPER_README.md            # Technical docs
    ├── USER_GUIDE.md                  # User manual
    ├── QUICK_REFERENCE.md             # Quick ref card
    └── ARCHITECTURE.md                # This file
    │
    ├── Models/                        # Data models
    │   └── SecurityCheck.cs           # Base class (pattern)
    │
    ├── Checks/                        # Security check implementations
    │   ├── FirewallCheck.cs
    │   ├── GuestAccountCheck.cs
    │   ├── RDPCheck.cs
    │   ├── DefenderCheck.cs
    │   └── SMBv1Check.cs
    │
    └── UI/                            # UI components
        └── CheckDetailDialog.cs       # Detail popup
```

## Extension Points

To add a new security check:

```
1. Create new class in Checks/
   
   public class NewCheck : SecurityCheck
   {
       public NewCheck() { /* Set properties */ }
       public override void Check() { /* Implement */ }
       public override bool Fix() { /* Implement */ }
   }

2. Add to MainDashboard.InitializeChecks()
   
   _checks = new List<SecurityCheck>
   {
       new FirewallCheck(),
       new GuestAccountCheck(),
       new NewCheck(),  // ← Add here
       // ...
   };

3. Build and run - it automatically appears in the UI!
```

## Design Patterns Used

1. **Template Method Pattern**
   - SecurityCheck defines the pattern
   - Each check implements specific behavior
   - UI treats all checks uniformly

2. **Strategy Pattern**
   - Different checks use different strategies
   - Check(), Fix(), Verify() methods
   - Swappable implementations

3. **Factory Pattern** (Simple)
   - MainDashboard creates SecurityCheck instances
   - Centralized initialization

4. **Model-View Pattern**
   - SecurityCheck = Model
   - MainDashboard = View
   - CheckDetailDialog = Detail View
