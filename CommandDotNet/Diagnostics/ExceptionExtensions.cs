using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using CommandDotNet.Extensions;
using CommandDotNet.Rendering;

namespace CommandDotNet.Diagnostics
{
    public static class ExceptionExtensions
    {
        internal static void SetCommandContext(this Exception ex, CommandContext ctx)
        {
            ex.Data[typeof(CommandContext)] = ctx;
        }

        public static CommandContext? GetCommandContext(this Exception ex)
        {
            return ex.Data.Contains(typeof(CommandContext))
                ? (CommandContext)ex.Data[typeof(CommandContext)]
                : ex.InnerException?.GetCommandContext();
        }

        public static void Print(this Exception ex, IConsole console, 
            Indent indent = null, bool includeProperties = false, bool includeData = false)
        {
            void WriteLine(string? s) => console.Error.WriteLine(s);
            
            indent ??= new Indent();
            WriteLine($"{indent}{ex.Message}");
            
            if (includeProperties)
            {
                var properties = ex.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                if (properties.Any())
                {
                    WriteLine($"{indent}Properties:");
                    indent = indent.Increment();
                    foreach (var property in properties)
                    {
                        WriteLine($"{indent}{property.Name}:{property.GetValue(ex).ToIndentedString(indent)}");
                    }

                    foreach (DictionaryEntry entry in ex.Data)
                    {
                        WriteLine($"{indent}{entry.Key}:{entry.Value.ToIndentedString(indent)}");
                    }

                    indent = indent.Decrement();
                }
            }

            if (includeData && ex.Data.Count > 0)
            {
                WriteLine($"{indent}Data:");
                indent = indent.Increment();
                foreach (DictionaryEntry entry in ex.Data)
                {
                    WriteLine($"{indent}{entry.Key}:{entry.Value.ToIndentedString(indent)}");
                }
                indent = indent.Decrement();
            }
        }

        [Conditional("DEBUG")]
        internal static void PrintStackTrace(this Exception ex, IConsole console)
        {
            console.Error.WriteLine(ex.StackTrace);
        }

        internal static Exception EscapeWrappers(this Exception exception)
        {
            if (exception is AggregateException aggEx)
            {
                var original = exception;
                exception = aggEx.GetBaseException().WithDataFrom(original);

                if (exception is AggregateException)
                {
                    // will be AggregateException where there are multiple inner exceptions
                    return exception;
                }
            }

            if (exception is TargetInvocationException tie && tie.InnerException != null)
            {
                exception = EscapeWrappers(tie.InnerException).WithDataFrom(tie);
            }

            return exception;
        }

        private static Exception WithDataFrom(this Exception exception, Exception original)
        {
            // copy exception.Data that can be used store debug context
            foreach (DictionaryEntry entry in original.Data)
            {
                if (!exception.Data.Contains(entry.Key))
                {
                    exception.Data[entry.Key] = entry.Value;
                }
            }

            return exception;
        }
    }
}