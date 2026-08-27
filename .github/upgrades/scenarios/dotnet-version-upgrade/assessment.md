# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NETCoreApp,Version=v10.0.

## Table of Contents

- [Executive Summary](#executive-Summary)
  - [Highlevel Metrics](#highlevel-metrics)
  - [Projects Compatibility](#projects-compatibility)
  - [Package Compatibility](#package-compatibility)
  - [API Compatibility](#api-compatibility)
  - [Binding Redirect Configuration](#binding-redirect-configuration)
- [Aggregate NuGet packages details](#aggregate-nuget-packages-details)
- [Top API Migration Challenges](#top-api-migration-challenges)
  - [Technologies and Features](#technologies-and-features)
  - [Most Frequent API Issues](#most-frequent-api-issues)
- [Projects Relationship Graph](#projects-relationship-graph)
- [Project Details](#project-details)

  - [PERSONAL-PROJECT-2.1.csproj](#personal-project-21csproj)


## Executive Summary

### Highlevel Metrics

| Metric | Count | Status |
| :--- | :---: | :--- |
| Total Projects | 1 | 0 require upgrade |
| Total NuGet Packages | 13 | All compatible |
| Total Code Files | 13 |  |
| Total Code Files with Incidents | 0 |  |
| Total Lines of Code | 620 |  |
| Total Number of Issues | 0 |  |
| Estimated LOC to modify | 0+ | at least 0.0% of codebase |

### Projects Compatibility

| Project | Target Framework | Difficulty | Package Issues | API Issues | Binding Issues | Est. LOC Impact | Description |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: | :--- |
| [PERSONAL-PROJECT-2.1.csproj](#personal-project-21csproj) | net10.0;net10.0-windows | ✅ None | 0 | 0 | 0 |  | Wpf, Sdk Style = True |

### Package Compatibility

| Status | Count | Percentage |
| :--- | :---: | :---: |
| ✅ Compatible | 13 | 100.0% |
| ⚠️ Incompatible | 0 | 0.0% |
| 🔄 Upgrade Recommended | 0 | 0.0% |
| ***Total NuGet Packages*** | ***13*** | ***100%*** |

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

## Aggregate NuGet packages details

| Package | Current Version | Suggested Version | Projects | Description |
| :--- | :---: | :---: | :--- | :--- |
| MetadataExtractor | 2.9.3 |  | [PERSONAL-PROJECT-2.1.csproj](#personal-project-21csproj) | ✅Compatible |
| Microsoft.Bcl.AsyncInterfaces | 10.0.11 |  | [PERSONAL-PROJECT-2.1.csproj](#personal-project-21csproj) | ✅Compatible |
| Microsoft.CSharp | 4.7.0 |  | [PERSONAL-PROJECT-2.1.csproj](#personal-project-21csproj) | ✅Compatible |
| Microsoft.Web.WebView2 | 1.0.4078.44 |  | [PERSONAL-PROJECT-2.1.csproj](#personal-project-21csproj) | ✅Compatible |
| Newtonsoft.Json | 13.0.4 |  | [PERSONAL-PROJECT-2.1.csproj](#personal-project-21csproj) | ✅Compatible |
| System.Data.DataSetExtensions | 4.5.0 |  | [PERSONAL-PROJECT-2.1.csproj](#personal-project-21csproj) | ✅Compatible |
| System.Drawing.Common | 10.0.11 |  | [PERSONAL-PROJECT-2.1.csproj](#personal-project-21csproj) | ✅Compatible |
| System.Net.Http | 4.3.4 |  | [PERSONAL-PROJECT-2.1.csproj](#personal-project-21csproj) | ✅Compatible |
| System.Runtime.CompilerServices.Unsafe | 6.1.2 |  | [PERSONAL-PROJECT-2.1.csproj](#personal-project-21csproj) | ✅Compatible |
| System.Text.Encoding.CodePages | 10.0.11 |  | [PERSONAL-PROJECT-2.1.csproj](#personal-project-21csproj) | ✅Compatible |
| System.Text.Encodings.Web | 10.0.11 |  | [PERSONAL-PROJECT-2.1.csproj](#personal-project-21csproj) | ✅Compatible |
| System.Text.Json | 10.0.11 |  | [PERSONAL-PROJECT-2.1.csproj](#personal-project-21csproj) | ✅Compatible |
| XmpCore | 6.1.10.1 |  | [PERSONAL-PROJECT-2.1.csproj](#personal-project-21csproj) | ✅Compatible |

## Top API Migration Challenges

### Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |

### Most Frequent API Issues

| API | Count | Percentage | Category |
| :--- | :---: | :---: | :--- |

## Projects Relationship Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR
    P1["<b>📦&nbsp;PERSONAL-PROJECT-2.1.csproj</b><br/><small>net10.0;net10.0-windows</small>"]
    click P1 "#personal-project-21csproj"

```

## Project Details

<a id="personal-project-21csproj"></a>
### PERSONAL-PROJECT-2.1.csproj

#### Project Info

- **Current Target Framework:** net10.0;net10.0-windows✅
- **SDK-style**: True
- **Project Kind:** Wpf
- **Dependencies**: 0
- **Dependants**: 0
- **Number of Files**: 18
- **Lines of Code**: 620
- **Estimated LOC to modify**: 0+ (at least 0.0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["PERSONAL-PROJECT-2.1.csproj"]
        MAIN["<b>📦&nbsp;PERSONAL-PROJECT-2.1.csproj</b><br/><small>net10.0;net10.0-windows</small>"]
        click MAIN "#personal-project-21csproj"
    end

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

