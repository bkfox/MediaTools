
namespace MediaTools.Core
{
    using System.Collections.Generic;

    public interface IRuntime<Context>
        where Context: IContext
    {
        public Context GetContext();
        public void Run(Context context, Command command);
        public void Run(Context context, Commands commands);

        public void Run(Command command) {
            var context = GetContext();
            Run(context, command);
        }
        
        public void Run(Commands commands) {
            var context = GetContext();
            Run(context, commands);
        }
    }

    /// <summary>
    /// Provide runtime machine for commands, executing registered actions.
    /// </summary>
    public abstract class Runtime<Context> : IRuntime<Context>
        where Context: IContext
    {
        public delegate Context ContextBuilder(Runtime<Context> runtime);

        public string Label = "run";
        public string Name = "Run";
        public string Description = "Run set of commands";

        public ContextBuilder? CreateContext;
        public Dictionary<string, IAction<Context>> Actions = new Dictionary<string, IAction<Context>>();


        public Runtime(ContextBuilder? createContext=null) {
            Actions = new Dictionary<string, IAction<Context>>();
            CreateContext = createContext;
        }

        public Runtime(string name, string? label=null, string? description=null)
            : this()
        {
            Name = name;
            Label = label switch {
                null => string.Concat(name[0].ToString().ToUpper(), name.AsSpan(1)),
                _ => label,
            };
            Description = description switch {
                null => "",
                _ => description,
            };
        }

        /// <summary>
        /// Register an action.
        /// </summary>
        public void Register(IAction<Context> action, string? prefix=null) {
            var name = prefix switch {
                null => action.GetName(),
                _ => prefix + action.GetName(),
            };
            Register(name, action);
        }

        /// <summary>
        /// Register an action with provided name
        /// </summary>
        public void Register(string name, IAction<Context> action, string? prefix=null) {
            Actions.Add(name, action);
        }

        /// <summary>
        /// Register multiple actions at once
        /// </summary>
        public void Register(IEnumerable<IAction<Context>> actions, string? prefix=null) {
            foreach(var action in actions)
                Register(action);
        }

        /// <summary>
        /// Return a new context. Throws NotImplementedException when no method has been
        /// provided to build one.
        /// </summary>
        public virtual Context GetContext() => CreateContext switch {
            null => throw new NotImplementedException(
                "I don't know how to create a new Context. `CreateContext` or `GetContext`" +
                " must be provided."),
            _ => CreateContext(this),
        };

        /// <summary>
        /// Run a single command
        /// </summary>
        public virtual void Run(Context context, Command command) {
            var action = Actions[command.Name];
            if(action == null)
                throw new KeyNotFoundException($"unknown action '{command.Name}'.");
            var self = this;
            action.Call(context, ref command);
        }

        /// <summary>
        /// Run command set
        /// </summary>
        public virtual void Run(Context context, Commands commands) {
            foreach(var command in commands)
                Run(context, command);
        }
    }
}