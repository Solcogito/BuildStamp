# BuildStamp Templates

You can override built-in rendering by placing template files in:

```
.buildstamp/template.<format>.txt
```

Supported `<format>`: `cs`, `md`, `json`, `text`.

## Tokens

- {PROJECT}
- {VERSION}
- {BRANCH}
- {COMMIT}
- {TIMESTAMP}
- {TAGS}       (comma-joined)
- {TAGS_JSON}  (JSON array)
- {TAGS_CS}    (C# string[] literal)
- {NAMESPACE}
- {CLASS}
- {ATTR_FLAG}  (1 or 0, whether assembly attributes would be emitted)

## Example: template.cs.txt

```txt
// Auto-generated via template
namespace {NAMESPACE}
{
    internal static class {CLASS}
    {
        public const string Version = "{VERSION}";
        public const string Commit  = "{COMMIT}";
        public const string Branch  = "{BRANCH}";
        public const string Built   = "{TIMESTAMP}";
        public static readonly string[] Tags = {TAGS_CS};
    }
}
```

Place the file at `.buildstamp/template.cs.txt` then run:

```
buildstamp --format cs --out ./Builds/BuildInfo.cs
```
