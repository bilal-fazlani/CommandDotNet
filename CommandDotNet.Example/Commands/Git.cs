using System;

namespace CommandDotNet.Example.Commands
{
    [Command(Description = "Fake git application to demonstrate nested sub-commands. Does NOT interact with git.")]
    public class Git
    {
        [SubCommand]
        public Submodule? SubmoduleProperty { get; set; }

        [SubCommand]
        public class Remote
        {
            [SubCommand]
            public class Origin
            {
                public void Show()
                {
                    Console.WriteLine("remote origin: master");
                }
            }
        }
        
        [Command(Description = "Stashes all changes when executed without any arguments. " +
                                           "See stash --help for further information",
            Name = "stash")]
        [SubCommand]
        public class Stash
        {
            [DefaultMethod]
            public void DoStash()
            {
                Console.WriteLine("changes stashed");
            }
        
            [Command(Name = "pop", Description = "Applies last stashed changes")]
            public void Pop()
            {
                Console.WriteLine("stash popped");
            }

            [Command(Name = "list", Description = "Lists all saved stashed changes")]
            public void List()
            {
                Console.WriteLine("here's the list of stash");
            }
        }
        
        [Command(Name = "commit", Description = "Commits all staged changes")]
        public void Commit([Option(ShortName = "m")]string commitMessage)
        {
            Console.WriteLine("Commit successful");
        }
    }
    
    public class Submodule
    {     
        public void Add(string name)
        {
            Console.WriteLine($"submodule added: {name}");
        }

        public void Remove(string name)
        {
            Console.WriteLine($"submodule: {name} has been removed");
        }
    }
}