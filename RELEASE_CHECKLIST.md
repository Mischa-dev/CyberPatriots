# Release Checklist

This checklist should be completed before releasing the application to users.

## Pre-Release Testing (Windows Required)

### Build and Installation
- [ ] Build in Release configuration: `dotnet build -c Release`
- [ ] Publish self-contained: `dotnet publish -c Release -r win-x64 --self-contained`
- [ ] Verify executable runs on Windows 10
- [ ] Verify executable runs on Windows 11
- [ ] Verify executable runs on Windows Server
- [ ] Test without .NET installed (self-contained build)

### Functionality Testing

#### Main Dashboard
- [ ] Application starts successfully
- [ ] Dashboard displays all 5 security checks
- [ ] Status colors are correct (Green/Red/Orange/Gray)
- [ ] Summary counts are accurate
- [ ] Refresh button works correctly
- [ ] Window resizing works properly
- [ ] Window can be maximized/minimized

#### Security Checks
- [ ] **Firewall Check**: Detects current state correctly
- [ ] **Guest Account Check**: Detects current state correctly
- [ ] **RDP Check**: Detects current state correctly
- [ ] **Defender Check**: Detects current state correctly
- [ ] **SMBv1 Check**: Detects current state correctly

#### Detail Dialog
- [ ] Double-click opens detail dialog
- [ ] All sections display correctly (Explanation, Why It Matters, Evidence, Manual Steps)
- [ ] Evidence shows actual command output
- [ ] Dialog is properly sized and formatted
- [ ] Close button returns to dashboard

#### Fix Functionality
- [ ] UAC prompt appears when clicking "Fix Now"
- [ ] Fixes work when UAC is accepted
- [ ] Fixes fail gracefully when UAC is declined
- [ ] Success message shows when fix works
- [ ] Error message shows when fix fails
- [ ] Auto-verify runs after successful fix

#### Verify Functionality
- [ ] Verify re-runs the check
- [ ] Status updates correctly after verify
- [ ] Evidence updates with new output
- [ ] Success/failure message is appropriate

### Administrator Privileges
- [ ] Application warns if not run as admin
- [ ] Fix buttons require admin (via UAC)
- [ ] Manual steps work without admin (user can follow them)
- [ ] Evidence can be collected without admin

### Security Testing
- [ ] No credentials are stored
- [ ] No sensitive data in logs
- [ ] UAC elevation works correctly
- [ ] Commands are executed safely (no injection possible)
- [ ] Fix operations can be undone manually if needed

### Edge Cases
- [ ] Network disconnected - app doesn't crash
- [ ] Windows Defender disabled by policy - handled gracefully
- [ ] SMBv1 not available - handled gracefully
- [ ] Multiple rapid refreshes - no crashes
- [ ] Spam clicking Fix/Verify - handled correctly

### UI/UX Testing
- [ ] Text is readable and clear
- [ ] Colors provide good contrast
- [ ] Font sizes are appropriate
- [ ] Button sizes are clickable
- [ ] No UI elements overlap
- [ ] No truncated text
- [ ] Tooltips work (if any)

### Performance
- [ ] Dashboard loads in < 2 seconds
- [ ] Refresh completes in < 5 seconds
- [ ] Fix operations complete in < 10 seconds
- [ ] No memory leaks on repeated operations
- [ ] Application uses < 50MB RAM

## Documentation Review

### User Documentation
- [ ] USER_GUIDE.md is accurate
- [ ] QUICK_REFERENCE.md is helpful
- [ ] README.md has correct instructions
- [ ] Screenshots added (if applicable)

### Developer Documentation
- [ ] DEVELOPER_README.md is complete
- [ ] ARCHITECTURE.md is accurate
- [ ] Code comments are helpful
- [ ] API documentation is correct

## Screenshots to Add

Take and add screenshots of:
- [ ] Main dashboard with all checks passing (green)
- [ ] Main dashboard with some checks failing (red)
- [ ] Detail dialog showing a check
- [ ] Fix confirmation dialog
- [ ] UAC elevation prompt
- [ ] Successful fix message
- [ ] Verify confirmation message

## Release Artifacts

Create and test:
- [ ] Self-contained Windows x64 build
- [ ] Self-contained Windows ARM64 build (optional)
- [ ] Installer package (optional)
- [ ] ZIP archive with executable and documentation
- [ ] Release notes document

## Final Checks

- [ ] Version number is set correctly
- [ ] Copyright/license information is included
- [ ] README.md has installation instructions
- [ ] All documentation links work
- [ ] No test/debug code remains
- [ ] No hardcoded paths remain
- [ ] .gitignore excludes build artifacts
- [ ] Solution builds without warnings
- [ ] All dependencies are documented

## Distribution

- [ ] Create GitHub release
- [ ] Upload artifacts
- [ ] Write release notes
- [ ] Tag the release in git
- [ ] Update README with download link
- [ ] Announce in appropriate channels

## Post-Release

- [ ] Monitor for bug reports
- [ ] Respond to user questions
- [ ] Track feature requests
- [ ] Plan next version
- [ ] Update documentation as needed

---

## Quick Test Scenarios

### Scenario 1: First Time User
1. Download and run the application
2. See all security checks fail
3. Double-click Firewall check
4. Read the information
5. Click "Fix Now"
6. Accept UAC
7. Verify the fix
8. Return to dashboard
9. See Firewall check now passes

### Scenario 2: CyberPatriot Competition
1. Start with unsecured system
2. Run application as admin
3. Review all failing checks
4. Fix them one by one using the app
5. Verify each fix
6. Click Refresh to see progress
7. All checks should pass
8. System is now more secure

### Scenario 3: Learning Mode
1. Open application
2. Double-click a check
3. Read "Why It Matters"
4. Read "Manual Steps"
5. Follow manual steps instead of auto-fix
6. Click "Verify"
7. Confirm manual fix worked
8. Understand what was changed and why

---

**Testing Notes**: Document any issues found during testing in GitHub Issues.

**Sign-off**: All items must be checked before release.
