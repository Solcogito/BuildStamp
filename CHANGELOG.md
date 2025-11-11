# 🧱 BuildStamp — CHANGELOG

Comprehensive version history of Solcogito.BuildStamp, part of the Solcogito DevOps Suite.  
Each milestone represents a verified, tagged release in the versioning pipeline.

---

## [0.8.0] - 2025-11-12
### Added
- GitHub Actions CI/CD pipeline for automatic build and test.
- Release workflow auto-generates `.zip` artifacts from BuildStamp CLI.
- PowerShell `release.ps1` for manual packaging.
- Artifact upload to GitHub Releases on version tags.

### Verified
- ✅ All CI steps pass successfully on `windows-latest`.
- ✅ BuildInfo.cs generated automatically during workflow run.

## [0.7.1] - 2025-11-11
### Other
- **buildstamp**: v0.7.1 CHANGELOG (ba16838)

## [0.7.0] - 2025-11-11
### Added
- User-defined template system under `.buildstamp/`.
- Multi-tag metadata array for JSON and C#.
- Environment variable overrides (`BUILDSTAMP_*`).
- Autoload from `.buildstamp.config.json`.
- `--config <path>` CLI override.
- Unit tests for template parsing and config merge.

### Changed
- Unified config resolution order (env → CLI → file).
- Updated documentation and examples for template tokens.

### Verified
- ✅ All formats and template overrides tested via xUnit and build.ps1.

## [0.6.1] - 2025-11-11
### Other
- **buildstamp**: v0.6.1 Changelog modifications (b19f0fa)

## [0.6.0] - 2025-11-11
### ✨ Added
- Integrated **Solcogito.Common.Versioning v0.1.1** for unified version resolution.
- Introduced **Embedded Metadata API** generating `BuildInfo.cs` automatically for .NET/Unity.
- Added `buildstamp.json` configuration schema and `_Infrastructure/build.ps1` verification script.
- Added initial **xUnit** test suite under `BuildStamp.Tests`.

### 🧠 Changed
- Reorganized project into **Core + Cli** architecture for modularity and CI integration.
- Removed legacy `Config` and `Utilities` classes — replaced with `BuildInfoEmitter`.
- CLI reimplemented to load configuration and delegate generation to Core.

### 🧪 Verified
- ✅ `build.ps1` validation passed successfully on Windows (PowerShell 7).
- ✅ `dotnet test` suite confirmed metadata file generation and integrity.
- ✅ Output confirmed under `/Builds/BuildInfo.cs` and sample project build.

---

## [0.5.0] - 2025-11-05
### ✨ Added
- Added configuration file support (`buildstamp.config.json`).
- CLI now merges configuration values with CLI arguments.
- Improved PowerShell validation script to support config-based runs.

### 🧠 Changed
- Centralized CLI parsing and argument override logic.
- Cleaned redundant formatter registration in CLI.
- Added robust directory resolution for output path.

### 🧪 Verified
- ✅ Config-driven and CLI override outputs verified.
- ✅ Compatible with AutoVersion and ZipRelease pipelines.

---

## [0.4.0] - 2025-11-05  
**Codename:** *Format System*  
**Type:** Feature Release  

### ✨ Added
- Multi-format output system with dedicated formatters:
  - `JsonFormatter`
  - `TextFormatter`
  - `MarkdownFormatter`
  - `CsFormatter`
- CLI flag `--format` supporting `json`, `text`, `md`, and `cs`.
- PowerShell test updated to verify all formats automatically.

### 🧠 Changed
- Consolidated formatter classes under `Soleria.BuildStamp.Output`.
- Simplified project file (removed redundant `<Compile Include>` entries).
- CLI now shows output format in confirmation line.

### 🧪 Verified
- ✅ JSON, MD, TXT, and CS outputs generated correctly.
- ✅ Build passes cleanly across Windows and Linux.
- ✅ AutoVersion validation successful.

---

## [0.3.0] - 2025-11-05  
**Codename:** *Auto Detection*  
**Type:** Feature Release  

### ✨ Added
- Automatic detection of:
  - Current Git branch (`rev-parse --abbrev-ref HEAD`)
  - Short commit hash (`rev-parse --short HEAD`)
  - Version via `version.json`
  - UTC timestamp auto-insertion
- Graceful fallbacks for missing Git/version contexts.

### 🧠 Changed
- Introduced `Utilities.cs` for detection logic.
- Improved PowerShell build script for integrated CLI test.
- Structured JSON serialization for metadata.

### 🧪 Verified
- ✅ Works in Git and non-Git directories.
- ✅ Correct version and commit recorded.
- ✅ Compatible with AutoVersion schema.

---

## [0.2.0] - 2025-11-04  
**Codename:** *Core CLI Command*  
**Type:** Bootstrap Feature  

### ✨ Added
- First working executable `buildstamp`.
- CLI flags:
  - `--out` → specify output path
  - `--help` → show usage information
- Generates minimal `buildinfo.json` with version and timestamp.

### 🧠 Changed
- Implemented clean C# .NET 8 console structure.
- Added README, LICENSE, and PowerShell validation script.
- Prepared for multi-format support in future versions.

### 🧪 Verified
- ✅ CLI outputs JSON file under `/Builds/`.
- ✅ Argument parsing works.
- ✅ Passes bootstrap validation.

---

## [0.1.0] - 2025-11-04  
**Codename:** *Repository Bootstrap*  
**Type:** Initial Setup  

### ✨ Added
- Initialized repository structure with:
  - `.gitignore`, `.gitattributes`, `.editorconfig`
  - `README.md`, `LICENSE`, `CHANGELOG.md`
  - `_Infrastructure/build.ps1` bootstrap validator
  - `/docs/` folder with `CONFIG.md`, `FAQ.md`, `WORKFLOWS.md`
  - `version.json` with initial schema

### 🧠 Changed
- Standardized Solcogito documentation layout.
- Linked with AutoVersion schema for future semantic bumps.
- Added internal test run and validation logs.

### 🧪 Verified
- ✅ Repository builds successfully.
- ✅ `version.json` validated via AutoVersion.
- ✅ Initial bootstrap test passes.

---

## [0.0.0] - 2025-10-30  
**Codename:** *Pre-Init Concept*  
**Type:** Prototype Stage  

### ✨ Added
- Conceptual design for BuildStamp.
- Defined schema for embedding version/build metadata.
- Established pipeline link with AutoVersion and ZipRelease.
- Created initial GitHub repository placeholder.

### 🧠 Notes
- No executable code at this stage.
- Served as design baseline for v0.1.0 bootstrap.

---

## 🧩 Version History Summary
| Version | Codename | Focus | Status |
|----------|-----------|--------|--------|
| 0.6.0 | Embedded Metadata API | Versioning integration + BuildInfo.cs | ✅ Stable |
| 0.5.0 | Config File Support | Config-driven CLI | ✅ Stable |
| 0.4.0 | Format System | Multi-output formatting | ✅ Stable |
| 0.3.0 | Auto Detection | Git & version resolution | ✅ Stable |
| 0.2.0 | Core CLI | Executable command base | ✅ Stable |
| 0.1.0 | Bootstrap | Repository + infrastructure | ✅ Complete |
| 0.0.0 | Concept | Design foundation | ✅ Archived |

---

✅ **Maintained by:** Solcogito S.E.N.C.  
💡 **Toolchain:** AutoVersion · BuildStamp · CompleteRelease · ZipRelease  
📦 **License:** MIT  
🌍 **Website:** [https://github.com/Solcogito/BuildStamp](https://github.com/Solcogito/BuildStamp)
