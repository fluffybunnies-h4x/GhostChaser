# 👻 GhostChaser - Cyber Deception Toolkit

**GhostChaser** is a Windows-based cyber deception tool designed to deploy and manage canary accounts, files, and network shares (collectively called "Ghosts") across enterprise Windows environments. These Ghosts act as tripwires to detect unauthorized access, lateral movement, and malicious activity within your network.

## Overview

GhostChaser enables security teams to implement proactive defense-in-depth strategies using honey tokens and canary resources. When adversaries interact with these Ghosts, their activities can be detected and investigated.

### Key Features

- **Three Ghost Types:**
  - **👤 Account Ghosts**: Canary user accounts (local or domain)
  - **📄 File Ghosts**: Decoy documents with enticing names and content
  - **🗂️ Share Ghosts**: Honeypot network shares with bait files

- **Graphical User Interface**: Intuitive WPF-based GUI for easy Ghost management
- **Remote Deployment**: Target local or remote Windows systems on your network
- **Flexible Authentication**: Use current credentials or specify alternate credentials
- **Audit Logging**: Comprehensive logging of all Ghost creation and removal activities
- **File Auditing**: Optional Windows auditing integration for access detection

## Prerequisites

- **Operating System**: Windows 10/11 or Windows Server 2016+
- **.NET Runtime**: .NET 8.0 or higher
- **Permissions**: Administrator privileges on target systems
- **Domain Environment** (optional): Active Directory for domain Ghost accounts

## Installation

GhostChaser can be built in two ways: **self-contained** (recommended, no .NET required) or **framework-dependent** (requires .NET 8.0).

### Option 1: Self-Contained Build (Recommended)

Creates a single executable with everything included - no .NET installation required!

1. **Download** or clone the GhostChaser repository

2. **Publish** the self-contained application:
   ```bash
   dotnet publish GhostChaser\GhostChaser.csproj -c Release
   ```

3. **Run** the executable from:
   ```bash
   GhostChaser\bin\Release\net8.0-windows\win-x64\publish\GhostChaser.exe
   ```

   The entire `publish` folder is portable - copy it anywhere and run!

### Option 2: Framework-Dependent Build

Requires .NET 8.0 Desktop Runtime to be installed on the target machine.

1. **Download** or clone the GhostChaser repository

2. **Build** using one of these methods:

   **Using .NET CLI:**
   ```bash
   dotnet build GhostChaser.sln --configuration Release
   ```

   **Using MSBuild:**
   ```bash
   # Using MSBuild from Visual Studio installation
   "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" GhostChaser.sln /p:Configuration=Release

   # Or if MSBuild is in your PATH
   msbuild GhostChaser.sln /p:Configuration=Release
   ```

   **Using Visual Studio:**
   - Open `GhostChaser.sln` in Visual Studio 2022 or later
   - Set configuration to Release
   - Build → Build Solution (Ctrl+Shift+B)

3. **Install .NET 8.0 Desktop Runtime** on target machine: [Download Here](https://dotnet.microsoft.com/download/dotnet/8.0)

4. **Run** the executable from:
   ```bash
   GhostChaser\bin\Release\net8.0-windows\win-x64\GhostChaser.exe
   ```

   > **Note:** GhostChaser is a C# .NET project and cannot be compiled with cl.exe (C/C++ compiler). Use dotnet CLI or MSBuild instead.

## Usage Guide

### Creating Ghost Accounts

Ghost accounts are canary user accounts designed to detect credential theft and unauthorized access attempts.

**Steps:**
1. Select **Account (User)** as the Ghost type
2. Enter a **Ghost Name** (descriptive identifier)
3. Specify the **Target System** (hostname or IP)
4. Configure **Deployment Credentials**
5. Enter the **Username** for the Ghost account
6. Choose between **Local Account** or **Domain Account**
   - For domain accounts, specify the domain and optional OU
7. Click **Deploy Ghost**

**Best Practices:**
- Use realistic but slightly suspicious usernames (e.g., `svc_backup`, `admin_temp`, `dbadmin`)
- Deploy to multiple systems for broader coverage
- Document Ghost accounts to avoid confusion during incident response
- Never use Ghost accounts for legitimate purposes

### Creating Ghost Files

Ghost files are decoy documents designed to detect file system access and data exfiltration attempts.

**Steps:**
1. Select **File (Document)** as the Ghost type
2. Enter a **Ghost Name**
3. Specify the **Target System**
4. Configure **Deployment Credentials**
5. Optionally specify a **File Path** (defaults to shared location)
6. Select a **File Extension** (e.g., `.txt`, `.xlsx`, `.sql`)
7. Enable **File Auditing** if supported
8. Click **Deploy Ghost**

**Best Practices:**
- Use enticing file names (e.g., `passwords.txt`, `credentials.xlsx`, `vpn_config.txt`)
- Place files in common shared directories
- Enable auditing to detect access attempts via Windows Event Logs
- Mix file types to appear legitimate

### Creating Ghost Shares

Ghost shares are honeypot network shares designed to detect lateral movement and unauthorized network access.

**Steps:**
1. Select **Share (Network)** as the Ghost type
2. Enter a **Ghost Name**
3. Specify the **Target System**
4. Configure **Deployment Credentials**
5. Enter a **Share Name** (visible on the network)
6. Optionally specify a **Share Path** (defaults to local directory)
7. Add a **Description** (makes the share more enticing)
8. Enable **Share Auditing** if supported
9. Click **Deploy Ghost**

**Best Practices:**
- Use realistic share names (e.g., `Backups`, `Finance`, `HR_Documents`)
- Add descriptive text to make shares appear legitimate
- The tool automatically populates shares with bait files
- Enable auditing to track access attempts

## Remote System Deployment

GhostChaser supports deploying Ghosts to remote Windows systems on the same network.

**Requirements:**
- Network connectivity to target system
- Administrative credentials for the target system
- File and Printer Sharing enabled on target
- Remote Registry service enabled (for shares)
- WMI access enabled on target

**Configuration:**
1. Enter the **hostname** or **IP address** in the Target System field
2. Uncheck **Use Current User Credentials**
3. Enter **Username** with admin rights on the remote system
4. Enter the **Password**
5. Specify the **Domain** if applicable

## Credential Management

GhostChaser handles credentials securely:

- Passwords are stored in memory using `SecureString`
- Credentials are cleared after each operation
- No credentials are logged or persisted to disk
- Always use least-privilege accounts when possible

## Audit Logging

All Ghost operations are logged to:
```
C:\ProgramData\GhostChaser\Logs\GhostChaser_Audit_YYYYMMDD.log
```

**Logged Events:**
- Ghost creation (with full details)
- Ghost removal
- Deployment successes and failures
- Errors and exceptions

**Log Format:** JSON (one event per line)

## Monitoring and Detection

### Windows Event Logs

When auditing is enabled, Ghost access attempts generate events in:
- **Security Log**: Event IDs 4663, 4656 (File/Share access)
- **Security Log**: Event IDs 4624, 4625 (Account logon attempts)

### SIEM Integration

Forward Windows Security logs to your SIEM and create alerts for:
- Logon attempts using Ghost account usernames
- File access events for Ghost file paths
- Network share access to Ghost shares

### Manual Verification

Use the GhostChaser GUI to track deployed Ghosts and verify their status.

## Security Considerations

### For Defenders

**Best Practices:**
- Deploy Ghosts as part of a layered defense strategy
- Document all Ghost deployments internally
- Integrate Ghost access detection into your incident response procedures
- Regularly review Ghost audit logs
- Remove unused Ghosts to reduce false positives

**Limitations:**
- Sophisticated attackers may identify and avoid Ghosts
- Ghosts require monitoring to be effective
- False positives can occur from legitimate admin activity
- Auditing may generate significant log volume

### Authorized Use Only

**IMPORTANT:** GhostChaser is designed for authorized defensive security operations only.

**Legitimate Use Cases:**
- Enterprise cyber deception programs
- Red team/blue team exercises with proper authorization
- Security research in controlled lab environments
- Penetration testing with written permission
- Educational cybersecurity training

**Prohibited Uses:**
- Deploying Ghosts on systems without authorization
- Creating Ghosts in networks you don't own or manage
- Using the tool for malicious purposes
- Violating organizational policies or laws

**Legal Notice:** Users are responsible for ensuring their use of GhostChaser complies with all applicable laws, regulations, and organizational policies. Always obtain proper authorization before deploying Ghosts.

## Removal and Cleanup

**To Remove a Ghost:**
1. Select the Ghost in the "Deployed Ghosts" list
2. Click the **Remove** button
3. Confirm the removal

**Complete Cleanup:**
- Use the GUI to remove all deployed Ghosts
- Delete Ghost account objects from Active Directory if needed
- Remove Ghost files and directories manually if necessary
- Delete Ghost shares using Computer Management or WMI
- Review audit logs at: `C:\ProgramData\GhostChaser\Logs\`

## Troubleshooting

### "Access Denied" Errors
- Verify you have administrator privileges on the target system
- Check that credentials are correct
- Ensure Windows Firewall allows WMI and SMB traffic
- Confirm Remote Registry service is running

### Ghost Account Creation Fails
- Verify Active Directory permissions for domain accounts
- Check that the username doesn't already exist
- Ensure password complexity requirements are met
- Review audit logs for specific error details

### Ghost Share Creation Fails
- Confirm the share name isn't already in use
- Verify the local path exists or can be created
- Check SMB services are running
- Review WMI permissions on the target system

### Auditing Not Working
- Verify you have permissions to modify audit policies
- Check that object access auditing is enabled via Group Policy
- Confirm Security Event Log is not full
- Review Windows audit policy with `auditpol /get /category:*`

## Architecture

GhostChaser uses a clean architecture with separation of concerns:

```
GhostChaser/
├── Models/              # Domain models (Ghost types, credentials, status)
├── Services/            # Business logic (deployment services, logging)
├── ViewModels/          # MVVM view models
├── Views/               # WPF UI views
└── Converters/          # UI data converters
```

**Key Technologies:**
- **WPF**: Modern Windows UI framework
- **C# / .NET 8.0**: Core programming language and framework
- **System.DirectoryServices**: Active Directory integration
- **System.Management**: WMI for remote management
- **MVVM Pattern**: Clean separation of UI and business logic

## Contributing

Contributions are welcome! Please ensure all contributions:
- Follow the existing code style and architecture
- Include appropriate error handling
- Update documentation as needed
- Are intended for defensive security purposes only

## License

This project is provided for educational and authorized defensive security purposes.

## Disclaimer

This tool creates user accounts, files, and network shares on Windows systems. Users are solely responsible for:
- Obtaining proper authorization before use
- Compliance with organizational policies
- Adherence to applicable laws and regulations
- Managing and removing deployed Ghosts
- Monitoring and responding to Ghost triggers

The authors assume no liability for misuse or unauthorized use of this software.

## Support

For issues, questions, or feature requests:
- Review the Troubleshooting section
- Check audit logs for error details
- Verify prerequisites and permissions
- Consult Windows Event Logs for system-level errors

## Version History

**v1.0.0** - Initial Release
- Ghost account creation (local and domain)
- Ghost file creation with multiple templates
- Ghost share creation with bait files
- WPF GUI with MVVM pattern
- Remote system targeting
- Comprehensive audit logging
- Windows auditing integration

---

**Remember:** Ghosts are only effective when properly monitored and integrated into your security operations workflow. Deploy responsibly and maintain situational awareness of all deployed Ghosts.
