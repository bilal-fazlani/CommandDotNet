# CommandDotNet.IoC.SimpleInjector


## 3.0.0 - prerelease

### Nullable Reference Types

The library has been updated to support Nullable Reference Types

### Obsoletes

remove `useLegacyInjectDependenciesAttribute` option as it's no longer supported as of CommandDotNet 4.0.0

## 2.0.1

improve stack traces with AppOutputBase and PathMap project settings
```
<AppOutputBase>$(MSBuildProjectDirectory)\</AppOutputBase>
<PathMap>$(AppOutputBase)=CommandDotNet/</PathMap>
```