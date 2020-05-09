# App Version

Enable the feature with `appRunner.UseVersionMiddleware()` or `appRunner.UseDefaultMiddleware()`.

Example:

```bash
$ dotnet example.dll -v
example.dll
1.0.0
```

By default, the version info is taken from the entry assembly's metadata.

A version can be provided via 

```c#
var version = new VersionInfo("blah", "1.0.0");
appRunner.Configure(c => c.Services.AddOrUpdate(version))
```