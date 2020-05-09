# CommandDotNet.NameCasing

## 2.0.0 - prerelease

### Nullable Reference Types

The library has been updated to support Nullable Reference Types

## 1.0.1

improve stack traces with AppOutputBase and PathMap project settings
```
<AppOutputBase>$(MSBuildProjectDirectory)\</AppOutputBase>
<PathMap>$(AppOutputBase)=CommandDotNet/</PathMap>
```