## [0.3.0] - 2025-11-05

## [0.2.0] - 2025-11-05
### Maintenance
- Add multi-format output system (v0.4.0) (ab0bda6)

## 🧱 v0.4.0 — Format System

Date: 2025-11-05
Type: Feature Release

### ✨ Added

- Multi-format output system with dedicated formatters:

	- JsonFormatter

	- TextFormatter

	- MarkdownFormatter

	- CsFormatter

- CLI flag --format supporting json, text, md, and cs

- PowerShell test updated to verify all formats automatically

### 🧠 Changed

- Consolidated formatter classes under Solcogito.BuildStamp.Output

- Simplified project file (removed redundant <Compile Include> entries)

- CLI now shows output format in confirmation line

### 🧪 Verified

- ✅ JSON, MD, TXT, and CS outputs generated correctly

- ✅ Build passes cleanly across Windows and Linux

- ✅ AutoVersion validation successful

## ⚙️ v0.3.0 — Auto-Detection (Git + System)

Date: 2025-11-05
Type: Feature Release

### ✨ Added

- Automatic detection of:

	- Current Git branch (rev-parse --abbrev-ref HEAD)

	- Short commit hash (rev-parse --short HEAD)

	- Version via version.json

	- UTC timestamp auto-insertion

- Graceful fallbacks for missing Git/version contexts

### 🧠 Changed

- Introduced Utilities.cs for detection logic

- Improved PowerShell build script for integrated CLI test

- Structured JSON serialization for metadata

### 🧪 Verified

- ✅ Builds and runs in Git and non-Git directories

- ✅ Correct version and commit recorded

- ✅ Works with AutoVersion schema

## 🧩 v0.2.0 — Core CLI Command

Date: 2025-11-04
Type: Bootstrap Feature

### ✨ Added

- First working executable buildstamp

- Added CLI flags:

	- --out → specify output path

	- --help → show usage information

- Generates minimal buildinfo.json with version and timestamp

### 🧠 Changed

- Implemented clean C# .NET 8 console project structure

- Updated PowerShell test to call dotnet run for CLI validation

- Added README, LICENSE, and base documentation

### 🧪 Verified

- ✅ CLI outputs JSON file under /Builds/

- ✅ Basic argument parsing works

- ✅ Passes bootstrap validation script

## 🧱 v0.1.0 — Repository Bootstrap

Date: 2025-11-04
Type: Initial Setup

### ✨ Added

- Initialized BuildStamp repository structure

- Added:

	- .gitignore, .gitattributes, .editorconfig

	- README.md, LICENSE, CHANGELOG.md

	- _Infrastructure/build.ps1 (bootstrap validation)

	- docs/ folder with CONFIG.md, FAQ.md, WORKFLOWS.md, etc.

	- version.json with initial schema

### 🧠 Changed

- Standardized Solcogito documentation layout

- Linked with AutoVersion schema for future bumps

- Added internal test run and validation logs

### 🧪 Verified

- ✅ Repository builds successfully

- ✅ version.json validated via AutoVersion

- ✅ Initial test run passes bootstrap phase

## 🪐 v0.0.0 — Pre-Init Concept

Date: 2025-10-30
Type: Internal Prototype

### ✨ Added

- Design sketches for the BuildStamp tool concept

- Defined schema for embedding version and build metadata

- Established link between BuildStamp and AutoVersion pipeline

- Created initial GitHub repository placeholder

### 🧠 Notes

- No executable code in this stage

- Served as planning baseline for v0.1.0 repository bootstrap

## 🧩 Version History Summary
|Version	|Codename	|Focus	|Status|
|---|---|---|---|
|0.4.0	|Format System	|Multi-output support	|✅ Stable|
|0.3.0	|Auto Detection	|Git & version detection	|✅ Stable|
|0.2.0	|Core CLI	|Initial command-line	|✅ Stable|
|0.1.0	|Bootstrap	|Repository + infra	|✅ Complete|
|0.0.0	|Concept	|Design foundation	|✅ Archived|

✅ Maintained by: Solcogito S.E.N.C.
💡 Toolchain: AutoVersion · BuildStamp · CompleteRelease · ZipRelease
📦 License: MIT