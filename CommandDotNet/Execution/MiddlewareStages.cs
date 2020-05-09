using CommandDotNet.Parsing;
using CommandDotNet.Tokens;

namespace CommandDotNet.Execution
{
    /// <summary>
    /// Defines stages that will exit with specific guarantees
    /// </summary>
    public enum MiddlewareStages
    {
        /// <summary>
        /// Use the <see cref="PreTokenize"/> stage for middleware that will configure the rest of the app
        /// or need to register event handlers early in the pipeline.
        /// <br/>Guarantees:<br/>
        /// - <see cref="CommandContext.Original"/> is populated<br/>
        /// - <see cref="CommandContext.Tokens"/> is set to <see cref="OriginalInput.Tokens"/>.
        ///  The first pass of Tokenization has been performed but <see cref="TokenTransformation"/>s have not been applied.<br/>
        /// - <see cref="CommandContext.AppConfig"/> and <see cref="CommandContext.Console"/> are set<br/>
        /// </summary>
        PreTokenize,

        /// <summary>
        /// In the <see cref="Tokenize"/> stage, all <see cref="TokenTransformation"/>s are applied to <see cref="CommandContext.Tokens"/>.
        /// Any transformations to <see cref="CommandContext.Tokens"/> applied after this stage will not be reported by the
        /// parse directive.
        /// <br/>Guarantees:<br/>
        /// - <see cref="CommandContext.Tokens"/> is the result of all <see cref="TokenTransformation"/>s<br/>
        /// - <see cref="CommandContext.RootCommand"/> is set for the type specified in <see cref="AppRunner{T}"/>.<br/>
        ///   The entire command hierarchy may be loaded or commands may lazy load during parse.
        /// </summary>
        Tokenize,
        PostTokenizePreParseInput,

        /// <summary>
        /// In the <see cref="ParseInput"/> stage Tokens are parsed to determine the
        /// intended command, options and operands and separated arguments.
        /// <br/>Guarantees:<br/>
        /// - <see cref="CommandContext.ParseResult"/> is populated<br/>
        /// - <see cref="ParseResult.TargetCommand"/> is set to the target command<br/>
        /// - <see cref="IArgument.InputValues"/> contains the collection of values assigned to arguments<br/>
        /// - <see cref="IArgument.Default"/> contains the default value assigned to arguments<br/>
        /// - <see cref="InvocationStep.Invocation"/> and <see cref="InvocationStep.Command"/> are set for the target command and its ancestral interceptor commands<br/>
        /// not set yet (see <see cref="BindValues"/>):<br/>
        /// - <see cref="InvocationStep.Instance"/><br/>
        /// - <see cref="IInvocation.ParameterValues"/><br/>
        /// </summary>
        ParseInput,
        PostParseInputPreBindValues,

        /// <summary>
        /// In the <see cref="BindValues"/> stage the argument string values are bound to the parameters
        /// of the command method and the instance of the type containing the method is instantiated.
        /// <br/>Guarantees:<br/>
        /// - <see cref="InvocationStep.Instance"/> are set for the target command and its ancestral interceptor commands<br/>
        /// - <see cref="IInvocation.ParameterValues"/> are set<br/>
        /// - Properties attributed with <see cref="SubCommandAttribute"/> are populated IF the subcommand is included in the user request and the property is settable.
        /// </summary>
        BindValues,
        PostBindValuesPreInvoke,

        /// <summary>
        /// In the <see cref="Invoke"/> stage the <see cref="InvocationPipeline"/> are invoked.
        /// This is the final stage.
        /// </summary>
        Invoke
    }
}