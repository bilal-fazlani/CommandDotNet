using System.Threading.Tasks;

namespace CommandDotNet.Execution
{
    public delegate Task<int> ExecutionDelegate(CommandContext commandContext);

    public delegate Task<int> ExecutionMiddleware(
        CommandContext context,
        ExecutionDelegate next);
}