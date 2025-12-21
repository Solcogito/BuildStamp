# Changelog

All notable changes to **BuildStamp Core** are documented here.

This project follows [Semantic Versioning](https://semver.org/).

---

## [0.1.0] – 2025-12-21

### Added
- API-first `BuildStampEngine` with explicit request/response contract.
- Deterministic output generation with no filesystem, git, or environment access.
- Supported output formats:
  - Text
  - JSON
  - Markdown
  - C# source
- Explicit `BuildStampRequest` input model.
- Explicit `BuildStampResult` output model.
- Minimal error model (`BuildStampException`, error codes).
- Fully isolated Core with zero side effects.
- Thin CLI adapter built on top of Core.

### Removed
- Configuration files.
- Implicit Git discovery.
- Environment variable inference.
- Template engines.
- Multi-command CLI surface.
- Inspect / schema / resolve commands.
- ArgForge dependency from Core.

### Notes
- This release intentionally resets the BuildStamp architecture.
- Legacy behavior is documented in `CHANGELOG_LEGACY.md`.
