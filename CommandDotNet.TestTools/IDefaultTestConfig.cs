namespace CommandDotNet.TestTools
{
    /// <summary>Implement this to provide a TestConfig for tests</summary>
    public interface IDefaultTestConfig
    {
        /// <summary>The TestConfig to use as a default</summary>
        TestConfig Default { get; }
    }
}