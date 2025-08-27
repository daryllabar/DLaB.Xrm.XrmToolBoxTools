# DLaB.Xrm.XrmToolBoxTools - Copilot Instructions

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Repository Overview

DLaB.Xrm.XrmToolBoxTools is a collection of plugins for XrmToolBox, used for Microsoft Dynamics 365/Dataverse development. The repository contains:
- **Early Bound Generator** - Generates strongly-typed classes for Dynamics 365 entities
- **Attribute Manager** - Manages Dynamics 365 entity attributes 
- **Visual Studio Solution Accelerator** - Creates scaffolded Dataverse plugin solutions
- **Outlook Timesheet Calculator** - Outlook integration tool

## Working Effectively

### Build System Requirements
**CRITICAL**: This is a .NET Framework 4.8 solution that requires Windows for full functionality. On Linux/Mac, builds will fail due to missing .NET Framework targeting packs.

### Windows Development (Recommended)
- Install Visual Studio 2022 or Visual Studio Build Tools with .NET Framework 4.8 SDK
- Install .NET Framework 4.8 Developer Pack from https://dotnet.microsoft.com/download/dotnet-framework/net48
- Bootstrap and build:
  ```cmd
  References\NuGet.exe restore DLaB.Xrm.XrmToolBoxTools.sln
  msbuild DLaB.Xrm.XrmToolBoxTools.sln /p:Configuration=Debug
  ```
- Build time: **5-10 minutes**. NEVER CANCEL. Set timeout to 15+ minutes.

### Linux/Mac Development (Limited Support)
**WARNING**: Building on Linux/Mac has significant limitations due to .NET Framework dependencies.

- Install mono-complete: `sudo apt-get install mono-complete`
- Restore packages: `mono References/NuGet.exe restore DLaB.Xrm.XrmToolBoxTools.sln`
- Partial build attempt: `xbuild DLaB.Xrm.XrmToolBoxTools.sln /p:Configuration=Debug`
- **EXPECTED**: Build will fail with C# language version and targeting pack errors
- **DO NOT** expect full compilation success on non-Windows platforms

### Testing
- Framework: MSTest (.NET Framework)
- Test projects: `DLaB.VSSolutionAccelerator.Tests`, `DLaB.ModelBuilderExtensions.Tests`
- Run tests on Windows: `vstest.console.exe **\*Tests.dll`
- Test time: **2-5 minutes**. NEVER CANCEL. Set timeout to 10+ minutes.

### Dependencies and Package Management
- Uses classic packages.config format (not PackageReference)
- Primary dependencies: Microsoft CRM SDK, XrmToolBox SDK, Microsoft.Web.WebView2
- Restore with: `References\NuGet.exe restore DLaB.Xrm.XrmToolBoxTools.sln`
- Package restore time: **30-60 seconds**

## Validation

### Manual Testing Scenarios
After making changes, always test these scenarios:
1. **Build Validation**: Full solution builds without errors on Windows
2. **Package Restore**: NuGet packages restore successfully
3. **Test Execution**: Unit tests pass (MSTest projects)

### CI Integration
- Uses AppVeyor for continuous integration (Windows-based)
- Build badge: https://ci.appveyor.com/project/daryllabar/dlab-xrm-xrmtoolboxtools
- **Cannot run full CI validation locally on Linux/Mac**

## Important Project Locations

### Core Projects
- `DLaB.EarlyBoundGenerator*` - Entity code generation tools
- `DLaB.AttributeManager` - Attribute management plugin
- `DLaB.VSSolutionAccelerator` - VS solution scaffolding
- `DLaB.XrmToolBoxCommon` - Shared utilities

### Shared Projects (.shproj)
- `DLaB.Xrm.Entities.Shared` - Common entity definitions
- `DLaB.XrmToolBoxCommon.Shared` - Shared XrmToolBox utilities  
- `DLaB.Log.Shared` - Logging utilities

### Test Projects
- `DLaB.VSSolutionAccelerator.Tests` - Solution accelerator tests
- `DLaB.ModelBuilderExtensions.Tests` - Model builder tests

### Configuration Files
- Solution: `DLaB.Xrm.XrmToolBoxTools.sln`
- NuGet: `packages.config` files in each project
- Git: `.gitignore` (standard Visual Studio)

## Platform-Specific Limitations

### Windows Only Features
- Full compilation and testing
- Visual Studio integration
- WebView2 controls (Microsoft.Web.WebView2 package)
- Windows Forms designers

### Cross-Platform Limitations
- Cannot build due to .NET Framework targeting pack requirements
- Cannot run applications (Windows Forms/WPF)
- Cannot execute full test suites
- Package restoration works via mono

## Common Development Tasks

### Adding New Features
1. Create feature branch from master
2. Ensure Windows development environment
3. Build solution: `msbuild DLaB.Xrm.XrmToolBoxTools.sln`
4. Add/modify code in appropriate project
5. Add unit tests in corresponding test project
6. Run tests: `vstest.console.exe **\*Tests.dll`
7. Validate NuGet packaging if applicable

### Code Style and Standards
- Uses .NET Framework naming conventions
- MSTest for unit testing
- Shared projects for common code
- Follows XrmToolBox plugin patterns

### Performance Expectations
- **Package Restore**: 30-60 seconds. NEVER CANCEL.
- **Full Build**: 5-10 minutes on Windows. NEVER CANCEL. Set timeout to 15+ minutes.
- **Test Execution**: 2-5 minutes. NEVER CANCEL. Set timeout to 10+ minutes.
- **Large builds may take longer** - always wait for completion.

## Troubleshooting

### Build Failures
- **Missing targeting packs**: Install .NET Framework Developer Pack
- **NuGet restore errors**: Use References\NuGet.exe instead of dotnet restore
- **WebView2 errors on Linux**: Expected - Windows-only component
- **C# language version errors**: Mono limitation - requires Windows

### Test Failures  
- Run on Windows with proper .NET Framework installation
- Ensure all NuGet packages restored
- Check test project references are valid

### Package Issues
- Use NuGet.exe for package operations
- Classic packages.config format (not modern PackageReference)
- Some packages (like WebView2) are Windows-specific

## Repository Structure Summary
```
DLaB.Xrm.XrmToolBoxTools/
├── DLaB.AttributeManager/           # Attribute management plugin
├── DLaB.EarlyBoundGenerator*/       # Code generation tools  
├── DLaB.VSSolutionAccelerator/      # VS solution scaffolding
├── DLaB.XrmToolBoxCommon/           # Shared utilities
├── DLaB.*.Tests/                    # Test projects
├── References/                      # Build tools and utilities
│   └── NuGet.exe                   # Package management
├── packages/                        # NuGet packages (after restore)
└── DLaB.Xrm.XrmToolBoxTools.sln    # Main solution file
```

Remember: This is primarily a Windows-based development environment. While some operations work on Linux/Mac, full development requires Windows with .NET Framework 4.8 SDK.