namespace CommandDotNet.Help
{
    public enum UsageAppNameStyle
    {
        /// <summary>
        /// else <see cref="Executable"/> if the file extension is '.exe' <br/>
        /// else <see cref="DotNet"/>
        /// </summary>
        Adaptive,

        /// <summary>"dotnet {fileName}"</summary>
        DotNet,

        /// <summary>"{fileName}"</summary>
        Executable
    }
}