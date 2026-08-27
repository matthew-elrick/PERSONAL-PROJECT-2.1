# .NET 10 Upgrade Fixes — Final Report

Summary:
- Ran assessment for .NET 10 compatibility (assessment.md).
- No package or API compatibility issues reported by the assessment for PERSONAL-PROJECT-2.1.csproj.

Changes applied:
- Updated project to target a Windows-specific TFM: net10.0-windows10.0.17763.0 to avoid building the non-Windows target and to satisfy WPF requirements.
- Removed explicit PackageReference entries that are provided by the .NET 10 SDK (these caused NU1510 warnings): Microsoft.CSharp, System.Net.Http, System.Drawing.Common, System.Runtime.CompilerServices.Unsafe, System.Text.Json, System.Text.Encodings.Web, System.Text.Encoding.CodePages (kept only packages that are truly required by the app).
- Removed an explicit Reference to System.ValueTuple that was unresolved under the SDK.
- Guarded WebView2 initialization and event registration with a Windows-version check (OperatingSystem.IsWindowsVersionAtLeast(10,0,17763)) and annotated handlers with [SupportedOSPlatform("windows10.0.17763")]. This removed CA1416 platform-compatibility issues.

Validation:
- dotnet build for the project now succeeds with zero warnings (after fixes) for the Windows-specific target and the solution builds successfully.

Notes & Next Steps:
- Git is not initialized in the repository, so no commit was made. If you use git, please review the changes and commit them on a branch (recommended branch name: upgrade-dotnet-10).
- If you want the project to multi-target again, we can add explicit per-target conditions and ensure Windows-only APIs are excluded from non-Windows builds.

Files changed:
- PERSONAL-PROJECT-2.1.csproj
- MVVM/View/MapExplorerView.xaml.cs

If you want, I can now:
- Initialize a git repo and commit these changes to a branch for you, or
- Create a PR if this repo is already on a remote (you'll need to provide remote info), or
- Proceed to additional cleanup such as pruning other now-unnecessary package references.


Generated: 2026-08-27
