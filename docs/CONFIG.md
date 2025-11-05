# ⚙️ CONFIG.md — BuildStamp Configuration Guide

BuildStamp can be fully automated using a configuration file.
You can define default metadata, output formats, and custom templates — ideal for CI/CD pipelines or multi-project workspaces.

## 🧱 Table of Contents

1. Overview

2. Default Behavior

3. Configuration File Locations

4. Schema Reference

5. JSON Example

6. YAML Example

7. Environment Overrides

8. Custom Templates

9. Multi-Project Workspaces

10. Pro Tips

## 🧭 Overview

By default, BuildStamp will read its settings from:
```
buildstamp.config.json

```

This allows you to predefine metadata and format rules so that buildstamp runs consistently in both local and automated environments.

## ⚡ Default Behavior

If no configuration file is found, BuildStamp auto-detects:

|Source	|Used for|
|--------|-------|
|AutoVersion’s version.json|	version number|
|git rev-parse HEAD	|commit hash|
|git rev-parse --abbrev-ref HEAD	|branch name|
|system clock|	timestamp (UTC)|

## 🧩 Configuration File Locations

BuildStamp searches for configuration files in this order:

1. ./buildstamp.config.json (project root)

2. ./.buildstamp/config.json (hidden folder)

3. ./.config/buildstamp/config.json (global config)

4. Environment variable: BUILDSTAMP_CONFIG_PATH

You can also override with:
```
buildstamp --config ./configs/stamp.json

```
## 🧾 Schema Reference
|Key	|Type	|Description|
|-------|--------|-----------|
|project	|string	|Project name to display in metadata
|version	|string	|Overrides version auto-detection
|branch	|string	|Overrides git branch detection
|commit	|string	|Overrides git commit hash
|timestamp|	string	|ISO timestamp (default: current UTC)
|tags|	array<string>	|Optional labels (nightly, rc, etc.)
|format|	string	|Output format: json, cs, text, md
|out|	string	|Output file path
|template	|string	|Path to a custom text template (Pro feature)
|includeGit	|bool|	Enables or disables git metadata collection
|includeEnv	|bool|	Capture selected environment variables
|envWhitelist	|array<string>	|Environment vars allowed in output
|caseStyle	|string	|Key naming: camelCase, PascalCase, snake_case
|precision	|string	|Time precision: seconds (default), milliseconds
|hashLength	|int|	Number of commit characters to include
|failOnMissingVersion	|bool	|Fails if no version found
|encoding	|string	|Output encoding (default: UTF-8)
## 🧩 JSON Example

buildstamp.config.json
```
{
  "project": "Solcogito.AutoVersion",
  "format": "cs",
  "out": "./src/BuildInfo.cs",
  "includeGit": true,
  "hashLength": 7,
  "tags": [ "stable", "release" ],
  "caseStyle": "PascalCase",
  "precision": "seconds",
  "includeEnv": true,
  "envWhitelist": [ "GITHUB_RUN_ID", "BUILD_NUMBER" ]
}

```
Usage:
```
buildstamp

```
Result:
A file BuildInfo.cs generated under ./src/, populated with metadata and selected environment variables.

## 🧩 YAML Example

buildstamp.yml
```
project: Solcogito.ZipRelease
format: md
out: ./Builds/BUILD_INFO.md
tags: [ beta, ci ]
includeGit: true
includeEnv: false
hashLength: 8
timestamp: 2025-11-04T15:42:00Z

```

Run:
```
buildstamp --config buildstamp.yml

```
## 🌍 Environment Overrides

Any key in the config file can be overridden by environment variables:

|Environment Variable	|Overrides|
|---|--------|
|BUILDSTAMP_PROJECT	|project|
|BUILDSTAMP_VERSION	|version|
|BUILDSTAMP_BRANCH	|branch|
|BUILDSTAMP_COMMIT	|commit|
|BUILDSTAMP_TAGS	|tags (comma-separated)|
|BUILDSTAMP_OUT	|out|
|BUILDSTAMP_FORMAT	|format|
|BUILDSTAMP_TIMESTAMP	|timestamp|

Example:
```
BUILDSTAMP_TAGS=nightly,internal buildstamp

```

## 🧩 Custom Templates (Pro Feature)

Pro users can define their own output structure using placeholders.
Example: custom-template.txt
```
Build Metadata
--------------
Project: {{ project }}
Version: {{ version }}
Branch:  {{ branch }}
Commit:  {{ commit }}
Date:    {{ timestamp }}
Tags:    {{ tags }}

```

Use it with:
```
buildstamp --template ./custom-template.txt --out ./Builds/custom.txt

```
## 🧱 Multi-Project Workspaces

In monorepos or multi-app solutions, you can use multiple configs:
```
/workspace
 ├─ /Client/
 │   ├─ buildstamp.config.json
 │   ├─ src/BuildInfo.cs
 │
 ├─ /Server/
 │   ├─ buildstamp.config.json
 │   ├─ src/BuildInfo.cs
 │
 └─ /Shared/
     ├─ buildstamp.config.json
     └─ src/BuildInfo.cs

```

Each module defines its own output, format, and metadata tags.

Example:
```
{
  "project": "Solcogito.Server",
  "tags": [ "api", "backend" ],
  "format": "json",
  "out": "./Builds/server-buildinfo.json"
}

```
## 🧠 Pro Tips

✅ Use .json configs for CI, .yml for readability in teams
✅ Keep timestamps in UTC for reproducibility
✅ Use short commit hashes (hashLength: 7) for concise build IDs
✅ Combine tags with AutoVersion for channel-specific builds
✅ Disable includeGit in isolated build environments

## 📄 Related Files
|File	|Purpose|
|---------|---|
|BUILDSTAMP.md|	CLI reference|
|WORKFLOWS.md|	CI/CD automation|
|README.md|	Overview & install|
|GUMROAD_PAGE.md|	Product marketing page|

## 🪪 License

MIT License (Lite)
Commercial License (Pro)
© 2025 Solcogito S.E.N.C.