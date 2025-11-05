# ✅ RELEASE_CHECKLIST.md — BuildStamp v1.0.0 Verification

##### Objective:
Guarantee that the BuildStamp v1.0.0 (Lite) public release is stable, documented, reproducible, and installable via both CLI and CI/CD.

## 🧱 1. Project Integrity
|Task	|Check	|Status|
|-----|-----|-----|
|Repo builds successfully on all OS targets|	dotnet build	|☐|
|.gitignore, .gitattributes, .editorconfig present|	Root verified	|☐|
|MIT License text updated to current year|	LICENSE	|☐|
|CHANGELOG.md updated and accurate|	Last entry = v1.0.0	|☐|
|Version tag bump confirmed	|autoversion bump minor	|☐|
## ⚙️ 2. CLI Functionality
|Task	|Check	|Status|
|-----|-----|----|
|buildstamp --help shows all flags clearly|	CLI prints help text	|☐|
|buildstamp --out ./Builds/buildinfo.json works|	JSON file created	|☐|
|Auto-detection of commit, branch, version verified	|Output fields populated	|☐|
|--tag appends tags correctly	|Example: nightly,beta	|☐|
|--quiet suppresses console output	|No text printed	|☐|
|Invalid flag handling returns proper error code	|Exit code = 1	|☐|
## 🧩 3. Format Validation
|Format	|Test Command	|Expected Result	|Status|
|-----|-----|--------|------|
|JSON	|buildstamp --format json --out test.json	|Valid JSON, keys ordered	|☐|
|Markdown|	buildstamp --format md --out test.md|	Rendered properly	|☐|
|Text	|buildstamp --format text --out test.txt|	No placeholder text left	|☐|
|C#	|buildstamp --format cs --out test.cs|	Compiles successfully	|☐|
## 🧠 4. Configuration & Environment
|Task	|Check	|Status|
|-----|-----|----|
|buildstamp.config.json loads correctly|	CLI detects config	|☐|
|Environment variable overrides work|	BUILDSTAMP_VERSION, BUILDSTAMP_TAGS	|☐|
|CLI + config merge logic tested	|CLI flag overrides config	|☐|
|Missing config gracefully falls back|	Defaults applied	|☐|
|CI runner with detached HEAD resolves commit|	SHA detected from GITHUB_SHA	|☐|
## 🧰 5. Cross-Tool Integration
|Tool	|Integration Check	|Status|
|-----|-----|----|
|AutoVersion	|autoversion bump patch then buildstamp uses updated version	|☐|
|ZipRelease	|ziprelease --source ./Builds --include buildinfo.json includes correct file	|☐|
|CompleteRelease	|Pipeline runs full chain AutoVersion → BuildStamp → ZipRelease	|☐|
|LicenseKeyTiny	|N/A (Lite)	|✅|
## 🧪 6. Cross-Platform QA
|OS	|Environment	|Test Command	|Status|
|-----|-----|----|----|
|Windows 11|	PowerShell 7	|build.ps1	|☐|
|Ubuntu 22.04	|bash	|buildstamp basic run	|☐|
|macOS Sonoma|	zsh	|buildstamp --format md	|☐|
|GitHub Actions|	Ubuntu-latest|	Workflow test passes	|☐|
## 📚 7. Documentation Review
|File	|Content Verified	|Status|
|----|----|----|
|README.md|	Overview & install	|☐|
|BUILDSTAMP.md	|CLI usage & reference	|☐|
|CONFIG.md|	Full schema	|☐|
|TEMPLATES.md|	All formats & examples	|☐|
|WORKFLOWS.md|	CI/CD integration	|☐|
|QUICKSTART.md	|1-minute setup	|☐|
|FAQ.md	|Top 15 common issues	|☐|
|GUMROAD_PAGE.md|	Copy, SEO, tiers	|☐|
|CHANGELOG.md	|Matches commits	|☐|
|ROADMAP.md	|Up to v1.0.0 milestones	|☐|
## 📦 8. Packaging & Release Artifacts
|Task	|Check	|Status|
|----|----|----|
|Build output directory cleaned	|/Builds/ reset before packaging	|☐|
|buildstamp --format json generated metadata	|Valid JSON	|☐|
|ziprelease created archive with docs + binaries	|.zip valid	|☐|
|File integrity verified	|SHA256 hash recorded	|☐|
|Archive opens without errors	|Manual check	|☐|
|Archive includes: CLI binary, LICENSE, docs|	✅	|☐|
## 🔐 9. Tagging & Deployment
|Task	|Check	|Status|
|----|----|----|
|Commit all changes|	git add . && git commit -m "Release v1.0.0"	|☐|
|Tag the release|	git tag v1.0.0	|☐|
|Push with tags	|git push origin main --tags	|☐|
|GitHub Actions build passes	|Status: ✅	|☐|
|Release draft auto-generated	|CHANGELOG included	|☐|
|Download from GitHub verified	|Binary runs via CLI	|☐|
## 🪣 10. Post-Release Validation
|Task	|Description	|Status|
|----|----|----|
|dotnet tool install --global Solcogito.BuildStamp works globally	|CLI accessible	|☐|
|Run test command after install|	buildstamp --help outputs	|☐|
|Re-run in clean VM environment	|Output matches reference	|☐|
|Add version badge to README|	![v1.0.0]	|☐|
|Announce on Gumroad	|Free tier live	|☐|
## 🧠 11. Optional Extended QA (Pro Prep)
|Task	|Purpose	|Status|
|----|----|----|
|Test custom template parsing	|Prepare for Pro upgrade	|☐|
|Verify environment whitelisting|	Optional Pro config	|☐|
|Evaluate signing workflow (SHA256 hash)|	Pro security test	|☐|
|Confirm LicenseKeyTiny integration (dummy)|	Gumroad license check simulation	|☐|
## 🏁 Final Verification

#### Mark each box once validated.

- [ ] All CLI formats verified

- [ ] AutoVersion + ZipRelease chain passes

- [ ] CI/CD pipeline completes

- [ ] Cross-platform validation done

- [ ] Documentation finalized

- [ ] Archive integrity confirmed

- [ ] GitHub release created

- [ ] dotnet tool install verified globally

Once all are ✅, tag v1.0.0 and publish 🎉

## 🧾 References
|Document	|Purpose|
|----|----|
|ROADMAP.md|	Milestone overview|
|CHANGELOG.md|	Version history|
|WORKFLOWS.md|	CI pipeline details|
|GUMROAD_PAGE.md|	Marketing & tiers|

## 🪪 License

MIT (Lite)
© 2025 Solcogito S.E.N.C.
https://github.com/Solcogito/BuildStamp