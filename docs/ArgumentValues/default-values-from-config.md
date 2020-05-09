# AppSettings and Environment Variables

Populate default values using appSettings and environment variables. 

These defaults will be shown as the default values in help output.

The values are converted as they would be if entered in the shell.

Collections can be delimited with a comma. eg. `apple,banana,orange`

### Example App

We'll use the following app for examples below.
```c#
public class Creds
{
    [Option(LongName = "User", ShortName = "u")]
    public string Username{ get;set; }
    
    [Option(LongName = "Password", ShortName = "p")]
    public Password Password{ get;set; }

    [Option(LongName = "token", ShortName = "t")]
    public Password AccessToken{ get;set; }
}

public class Api
{
    public void Download(Creds creds, string url, FileInfo file) { ... }
    public void Upload(Creds creds, FileInfo file, string url){...}
}
```

In this app, the upload and download commands can authenticate with username/password or with an access token.

```bash
dotnet example.dll upload -u me -p my_password http://mysite.com c:\myfile.txt
```
or
```bash
dotnet example.dll upload -t my_token http://mysite.com c:\myfile.txt
```


## Environment Variables
Enable the feature with `appRunner.UseDefaultsFromEnvVar()`.

Use the `[EnvVar("key")]` to assign the environment variable as the default for the argument.

With our example, if we wanted to load the access token from an environment variable, decorate the argument as

```c#
    [EnvVar("ExampleAppAccessToken")]
    [Option(LongName = "token", ShortName = "t")]
    public Password AccessToken{ get;set; }
```

When the _ExampleAppAccessToken_ environment variable exists, the default will be that value.

## AppSettings
Enable the feature with `appRunner.UseDefaultsFromAppSetting(appSettings)`

Use the `[AppSetting("key")]` to assign the app setting as the default for the argument.

```c#
    [AppSetting("AccessToken")]
    [Option(LongName = "token", ShortName = "t")]
    public Password AccessToken{ get;set; }
```

When the _ExampleAppAccessToken_ app setting exists, the default will be that value.

### AppSetting By Convention
Enable the feature with `appRunner.UseDefaultsFromAppSetting(appSettings, includeNamingConventions = true)`

`includeNamingConventions` will enable users to provide defaults for all arguments in your app.

Users can set app settings, using argument names and option short or long names as shown in the help as the setting keys. 

```xml
<add key="--user" value="me"/>
<add key="-p" value="my_secret"/>
<add key="url" value="http://mysite.com"/>
```

Any command with these arguments will inherit the default values from the app settings.

To specify defaults for a specific command, prefix the key with the command name.

```xml
<add key="upload url" value="http://my-upload-site.com"/>
<add key="upload --password" value="secret1"/>
<add key="download url" value="http://my-download-site.com"/>
<add key="download -p" value="secret2"/>
```


## Other Configs
Enable the feature with `appRunner.UseDefaultsFromConfig(arg => ...)`

Example: Say we need git .config settings and have a `GitConfigService` to read them...

Given an app like this

```c#
[AttributeUsage(AttributeTargets.Property|AttributeTargets.Parameter)]
public class GitConfigAttribute : Attribute
{
    public string Key {get;}
    public GitConfigAttribute(string key) => Key = key;
}

public class GitConfig : IArgumentModel
{
    [Option]
    [GitConfig("user.name")]
    public string UserName {get;set;}

    [Option]
    [GitConfig("user.option")]
    public string Email {get;set;}
}

public class MyApp
{
    public void FindRepos(GitConfig gitConfig){...}
}
```

Use the defaults from `GitConfigService` like this

```c#
new AppRunner<MyApp>
    .UseDefaultsFromConfig(arg => 
    {
        var key = argument.CustomAttributes
                          .GetCustomAttribute<GitConfigAttribute>()
                          ?.Key
        return key != null && _gitConfigService.TryGetValue(key, out var value)
            ? new ArgumentDefault("git-config", key, value)
            : null;
    }
    .Run(args);
)
```

!!! Note
    If the config value returns null, it is assumed no default value was found for the argument and it is skipped.
    Returning null will __not__ clear default values.
