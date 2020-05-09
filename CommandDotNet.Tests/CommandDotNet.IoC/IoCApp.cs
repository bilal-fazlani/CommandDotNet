using System;
using System.Threading.Tasks;
using CommandDotNet.Execution;

namespace CommandDotNet.Tests.CommandDotNet.IoC
{
    public class SomeIoCService : ISomeIoCService { }

    public interface ISomeIoCService { }

    internal class IoCApp
    {
        public readonly ISomeIoCService FromCtor;
        public ISomeIoCService? FromInterceptor;

        public IoCApp(ISomeIoCService fromCtor)
        {
            FromCtor = fromCtor;
        }

        public Task<int> Intercept(CommandContext context, ExecutionDelegate next)
        {
            FromInterceptor = (ISomeIoCService?) context.AppConfig.DependencyResolver?.Resolve(typeof(ISomeIoCService));
            return next(context);
        }

        public void Do([Option(ShortName = "i")] bool expectFromInterceptor)
        {
            if(FromCtor == null)
                throw new Exception("SomeService was not injected via ctor");
            if (FromInterceptor == null)
                throw new Exception("SomeService was not resolved in Interceptor");
        }
    }
}