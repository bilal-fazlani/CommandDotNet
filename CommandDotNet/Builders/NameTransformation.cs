using System.Reflection;

namespace CommandDotNet.Builders
{
    /// <summary>
    /// Add a name transformation to enforce name consistency across commands, operands and options.<br/>
    /// To enforce casing rules, configure `appRunner.UseNameCasing(...)` from the nuget package CommandDotNet.NameCasing
    /// </summary>
    /// <param name="attributes">the <see cref="ICustomAttributeProvider"/> for the parameter or member the name is for.</param>
    /// <param name="memberName">The name of the class, method, property or parameter defining the class or argument</param>
    /// <param name="nameOverride">The name provided via attribute or other extensibility point</param>
    /// <param name="commandNodeType">the <see cref="CommandNodeType"/> the name is for</param>
    public delegate string NameTransformation(ICustomAttributeProvider attributes, string memberName, string? nameOverride, CommandNodeType commandNodeType);
}