# 🧪 TEST_PLAN.md — BuildStamp v1.0.0

Project: BuildStamp (Lite)
Maintainer: Solcogito S.E.N.C.
Target Version: 1.0.0 (Free Release)
Test Environments: Windows 11, Ubuntu 22.04, macOS Sonoma
Tools: .NET 8, PowerShell 7, GitHub Actions, AutoVersion, ZipRelease

## 🧱 1. Objectives

- Verify all CLI features function as documented

- Ensure consistent outputs across platforms and formats

- Validate integration with AutoVersion, ZipRelease, and CompleteRelease

- Confirm deterministic metadata behavior for reproducible builds

- Certify release readiness for public distribution via dotnet tool install

## 🧩 2. Test Categories
|Category	|Description|
|----|----|
|🧪 Unit Tests|	Isolated function logic (formatters, timestamp, CLI args)|
|🔗 Integration Tests	|Interaction with config, Git, AutoVersion, and file IO|
|🧠 Regression Tests	|Stability and consistency across past versions|
|🧰 Manual QA	|Human validation of CLI UX and output readability|
|☁️ CI/CD Tests	|GitHub Actions pipeline execution verification|
## 🧪 3. Unit Tests
### 🧩 3.1 CLI Argument Parsing
|Case	|Input	|Expected Output|
|----|----|----|
|Basic run	|buildstamp	|Creates default buildinfo.json|
|With explicit version	|--version 1.0.0	|Version field = “1.0.0”|
|With custom output path	|--out ./temp/info.json	|File created at path|
|Invalid flag	|--bogus	|Error message + exit code 1|
|Quiet mode	|--quiet	|No console output|
### 🧩 3.2 Format Writers
|Format	|Input Command	|Validation|
|----|----|----|
|JSON|	--format json	|Valid JSON, all keys present|
|Markdown	|--format md	|Proper Markdown syntax|
|Text	|--format text	|No missing placeholders|
|C#	|--format cs	|File compiles under dotnet build|
### 🧩 3.3 Timestamp & Hash
|Case	|Input	|Expected|
|----|----|----|
|Default timestamp|	none	|UTC ISO format (Z suffix)|
|Custom timestamp|	--timestamp 2025-11-04T10:00:00Z	|Output matches override|
|Git hash detection	|repo present|	7-char short hash|
|Git missing|	no .git|	Empty commit field, warning shown|
## 🔗 4. Integration Tests
### 🔧 4.1 AutoVersion Compatibility
|Step	|Command	|Expected|
|----|----|----|
|Run AutoVersion bump	|autoversion bump patch	|Updates version.json|
|Run BuildStamp	|buildstamp	|Uses version from version.json|
|Verify output	|cat ./Builds/buildinfo.json	|Version matches AutoVersion output|
### 🔧 4.2 Config & Environment Variables
|Step	|Config	|Expected|
|----|----|----|
|Run with config|	buildstamp.config.json	|CLI reads values|
|Override by env|	BUILDSTAMP_VERSION=9.9.9 |buildstamp	Version = “9.9.9”|
|Merge CLI + config	|Config has branch, CLI adds tag	|Both appear in output|
### 🔧 4.3 ZipRelease Integration
|Step	|Command	|Expected|
|----|----|----|
|Generate metadata	|buildstamp --format json	|JSON present|
|Package release	|ziprelease --include buildinfo.json|	File inside zip|
|Verify	|Unzip artifact	|Metadata file matches source|
### 🔧 4.4 Unity / C# Embedder
|Step	|Command	|Validation|
|----|----|----|
|Generate embed file	|buildstamp --format cs --out ./Assets/Scripts/BuildInfo.cs|	File created|
|Compile Unity project	|via Editor or CLI|	No compiler errors|
|Log build info	|Debug.Log(BuildInfo.Version)|	Prints correct version|
## 🧠 5. Regression Tests
|Case	|Description	|Expected|
|----|----|----|
|Re-run v0.6.0 configs|	Old config still valid	|Output identical|
|Multi-tag stamping	|--tag beta --tag nightly	|Tags array consistent|
|Missing config fallback	|Delete config file	|CLI defaults applied|
|Cross-format consistency	|Compare all formats	|Same metadata values|
## ☁️ 6. CI/CD Tests
### 🧩 6.1 GitHub Actions Validation
|Step	|Command	|Expected Result|
|----|----|----|
|Push to main	|triggers workflow|	✅ Build completes|
|AutoVersion step	|autoversion bump	|✅ Version bumped|
|BuildStamp step	|buildstamp	|✅ Metadata generated|
|ZipRelease step	|ziprelease|	✅ Archive created|
|Upload Artifact	|auto step	|✅ buildinfo.json included|
|Action duration	|< 60s	|✅ within target|
### 🧩 6.2 Cross-Platform Matrix
|OS	|Shell	|Command	|Expected|
|----|----|----|----|
|Windows|	PowerShell	|build.ps1	|Success|
|macOS	|zsh	|buildstamp	|Success|
|Ubuntu	|bash	|buildstamp	|Success|
|CI runner	|default	|buildstamp	|Output matches reference|
## 🧰 7. Manual QA Checklist
|Area	|Test	|Verified|
|----|----|----|
|CLI help screen	|buildstamp --help readable	|☐|
|Error output|	Missing path triggers message	|☐|
|Console colors	|Color-coded sections visible	|☐|
|Version bump chain	|AutoVersion → BuildStamp → ZipRelease runs smoothly	|☐|
|Output readability	|Markdown + text formats human-friendly	|☐|
|Docs accuracy|	All examples produce expected output	|☐|
## 🧾 8. Automation Coverage Summary
|Test Layer	|Coverage Goal	|Status|
|----|----|----|
|Unit Tests	|90%+ of CLI and format logic	|☐|
|Integration|	100% of supported formats + tools	|☐|
|Regression	|Previous versions stable	|☐|
|Manual QA|	100% verified by human tester	|☐|
|CI/CD	|Full matrix passing	|☐|
## 🧠 9. Known Limitations (Lite)
|Limitation	|Description|
|----|----|
|No multi-output templates	|Planned for v1.1.0 Pro|
|No signed manifests	|Pro-only feature|
|Limited environment capture	|Whitelist in Pro|
|Static namespace in C# embedder	|Fixed in Lite, configurable in Pro|
## 🧭 10. Acceptance Criteria (v1.0.0)

- [ ] CLI works in isolation
- [ ] Outputs are valid in all formats
- [ ] Config and env overrides consistent
- [ ] Documentation examples tested
- [ ] Full pipeline (AutoVersion → BuildStamp → ZipRelease) succeeds
- [ ] CI passes on Windows/Linux/macOS
- [ ] Artifacts reproducible and verifiable

Once all conditions are green, tag and publish:
```
git tag v1.0.0
git push origin main --tags

```
## 🪪 License

MIT (Lite)
© 2025 Solcogito S.E.N.C.
https://github.com/Solcogito/BuildStamp