# ❓ FAQ.md — Frequently Asked Questions

Everything you need to know about using BuildStamp, troubleshooting common issues, and understanding advanced Pro features.

## 🧱 Table of Contents

1. General Questions

2. Version & Commit Detection

3. Output & Templates

4. Integration & CI/CD

5. Advanced & Pro Features

6. Licensing & Distribution

## 🧩 General Questions
### ❓ What is BuildStamp?

BuildStamp is a lightweight CLI that embeds version, commit, timestamp, and other metadata into your builds. It ensures every artifact is identifiable — perfect for CI/CD, Unity projects, or CLI apps.

### ❓ Do I need AutoVersion to use BuildStamp?

No.
BuildStamp works standalone.
However, if you have AutoVersion Lite installed, it will automatically detect and use the version from your version.json.

### ❓ What platforms are supported?

✅ Windows
✅ macOS
✅ Linux
✅ Compatible with .NET 8.0+, PowerShell 7+, and any CI runner (GitHub, GitLab, Azure, Bitbucket).

### ❓ How do I install it?
```
dotnet tool install --global Solcogito.BuildStamp

```
Or via PowerShell:
```
iwr https://raw.githubusercontent.com/Solcogito/BuildStamp/main/install.ps1 -useb | iex

```
### ❓ Where is the config file located?

By default:
```
./buildstamp.config.json

```
You can override with:
```
buildstamp --config ./configs/custom.json

```
See CONFIG.md for details.

## 🧭 Version & Commit Detection
### ❓ Why is my commit hash missing?

Check the following:

- You’re running BuildStamp inside a valid Git repository

- The .git folder isn’t excluded in your CI environment

- Git is installed and accessible in your $PATH

If Git is unavailable, you can manually specify:
```
buildstamp --commit abc1234

```
### ❓ Can I override the version manually?

Yes:
```
buildstamp --version 1.0.0

```

This value will override AutoVersion or any config-detected version.

### ❓ Can BuildStamp handle detached HEAD states (e.g. CI builds)?

Yes.
When running in detached HEAD mode (like CI runners), BuildStamp uses:

- GITHUB_SHA (GitHub)

- CI_COMMIT_SHA (GitLab)

- BUILD_SOURCEVERSION (Azure)

If those aren’t available, it defaults to manual commit override.

### ❓ What format is the timestamp?

All timestamps use ISO 8601 UTC:
```
2025-11-04T15:42:11Z

```
You can override it:
```
buildstamp --timestamp 2025-11-04T12:00:00Z

```
## 🧾 Output & Templates
### ❓ What output formats are supported?

- json → for CI/CD and automation

- cs → for embedding in .NET/Unity

- text → for human-readable logs

- md → for documentation or changelogs

- custom → (Pro only) define your own templates

See TEMPLATES.md for examples.

### ❓ Can I change the output file path?

Yes, using the --out option:
```
buildstamp --format md --out ./Docs/BUILD_INFO.md

```
### ❓ What if I want multiple output formats?

You can call BuildStamp multiple times in a single script:
```
buildstamp --format json --out ./Builds/buildinfo.json
buildstamp --format md --out ./Docs/BUILD_INFO.md

```
In Pro, a single config can define multiple outputs.

### ❓ Why do my tags show as null?

Tags only appear if you specify them:
```
buildstamp --tag beta --tag internal

```
or define them in buildstamp.config.json:
```
"tags": ["beta", "internal"]

```
### ❓ Can I localize the timestamp or use local time?

By default, timestamps are always UTC for reproducibility.
To format custom timestamps, use Pro template filters:
```
{{ timestamp | date:"yyyy-MM-dd HH:mm:ss zzz" }}

```
## ⚙️ Integration & CI/CD
### ❓ How do I use BuildStamp with GitHub Actions?

Add to .github/workflows/build.yml:
```
- name: Generate Build Info
  run: buildstamp --format json --out ./Builds/buildinfo.json

```

See WORKFLOWS.md for full YAML examples.

### ❓ Can I use BuildStamp in PowerShell scripts?

Yes. Example build.ps1:
```
autoversion bump patch
dotnet build
buildstamp --format json --out ./Builds/buildinfo.json

```
### ❓ Does BuildStamp work with ZipRelease?

Yes — that’s its intended flow:
```
autoversion bump patch
buildstamp --format json --out ./Builds/buildinfo.json
ziprelease --source ./Builds --include buildinfo.json

```
### ❓ Can BuildStamp trigger version bumps?

No.
BuildStamp reads version data, but only AutoVersion can modify it.
Use both together for complete automation.

## ⚡ Advanced & Pro Features
### ❓ What is included in BuildStamp Pro?
|Feature	|Descriptio|
|----------|-----|
|🔐 Signed Stamps	|Generate cryptographic SHA256 signatures|
|🧩 Custom Template|	Define your own text/JSON layouts|
|🪶 Environment Capture	|Include selected environment vars|
|🪣 Multi-Output| Config	Generate multiple formats in one run|
|🌍 Team License |Commercial use for studios and teams|

### ❓ Can I define custom templates?

Yes (Pro only). Example:
```
Build {{ project }} (v{{ version }})
Commit: {{ commit }} — {{ timestamp }}

```
Save as build-template.txt, then run:
```
buildstamp --template ./build-template.txt --out ./Builds/custom.txt

```
See TEMPLATES.md for more.

### ❓ How do environment captures work?

Pro supports whitelisting safe environment variables:
```
{
  "includeEnv": true,
  "envWhitelist": ["GITHUB_RUN_ID", "BUILD_NUMBER"]
}

```
Result (JSON):
```
{
  "env": {
    "GITHUB_RUN_ID": "4711",
    "BUILD_NUMBER": "22"
  }
}

```
### ❓ Can I run BuildStamp without Git installed?

Yes — manually specify commit and branch:
```
buildstamp --branch main --commit abc1234

```
## 💰 Licensing & Distribution
### ❓ Is the Lite version free?

Yes.
BuildStamp Lite is open-source under the MIT License.
You can use it freely for both personal and commercial projects.

### ❓ What does the Pro license include?

- Unlocks Pro features

- Perpetual license per organization

- Includes minor updates and patches

- Distributed via Gumroad with key verification (LicenseKeyTiny)

### ❓ How is licensing verified?

The Pro binary validates your key through LicenseKeyTiny, Solcogito’s lightweight key verifier.
Offline validation is supported for CI/CD.

### ❓ Can I redistribute BuildStamp?

You may include Lite binaries in your own repos or pipelines under MIT terms.
Pro binaries require a valid license.

### ❓ Where can I get support?

- 💬 Discord (invite via solcogito.com
)

- 🧠 Documentation: https://github.com/Solcogito/BuildStamp/docs

- 📧 Email: solcogito@gmail.com

## 🧾 Related Docs
|File	|Purpose|
|----|-------|
|BUILDSTAMP.md|	Core CLI reference|
|CONFIG.md|	Configuration schema|
|TEMPLATES.md|	Format templates|
|WORKFLOWS.md|	CI/CD integration|
|README.md|Overview & install|
|GUMROAD_PAGE.md|	Product marketing page|

## 🪪 License

MIT (Lite) • Commercial (Pro)
© 2025 Solcogito S.E.N.C.
https://github.com/Solcogito/BuildStamp