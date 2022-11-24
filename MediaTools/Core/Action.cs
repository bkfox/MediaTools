using System.Collections.Generic;
using System.Linq;

namespace MediaTools.Core {
    /// Define interface for actions to be executed by runtime using specific user's command.
    public interface IAction<Context>
        where Context: IContext
    {
        public string GetName();

        public string GetHelp();

        public void Call(Context context, ref Command command);

        public IRuntime<Context> GetRuntime(Context context) {
            var runtime = context.GetRuntime();
            if(runtime is not IRuntime<Context>)
                throw new NotSupportedException("Context's runtime is not a IRuntime instance.");
            return (IRuntime<Context>)context.GetRuntime();
        }
    }

    public class Action<Context> : IAction<Context>
        where Context: IContext
    {
            public virtual string Name {get; set; } = "action";
            public virtual string Description {get; set;} = "";

            public Action() {}

            public Action(string name) {
                Name = name;
            }

            public Action(string name, string description) {
                Name = name;
                Description = description;
            }

            public string GetName() => Name;
            public virtual string GetHelp() => Description;

            public virtual void Call(Context context, ref Command command) {
                throw new NotImplementedException("call method not implemented");
            }
    }

    public class ArguedAction<Context> : Action<Context>
        where Context: IContext
    {
            public virtual Arguments Arguments {get; set;} = new Arguments();

            public override string GetHelp() {
                var description = base.GetHelp();
                if(this.Arguments.Count > 0) {
                    description += "Available Options:";
                    var done = new SortedDictionary<string, IArgument>();
                    foreach(var entry in this.Arguments) {
                        var name = entry.GetName();
                        if(!done.ContainsKey(name))
                            done.Add(name, entry);
                    }
                    foreach(var entry in done)
                        description += $"\n    {entry.Key}: {entry.Value.GetHelp()}";
                }
                return description;
            }
    }
}
