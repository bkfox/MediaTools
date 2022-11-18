using System;

namespace MediaTools.Core {
    /// Define interface for actions to be executed by runtime using specific user's command.
    public interface IAction<Context>
        where Context: IContext
    {
        public string GetName();

        public string GetDescription();

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
            protected string Name {get; set; } = "action";
            protected string? Description {get; set;}
            protected Arguments? Arguments {get; set;} = null;

            public Action() {}

            public Action(string name, string? description=null) {
                Name = name;
                Description = description;
            }

            public string GetName() {
                return Name;
            }

            public string GetDescription() {
                var description = Description switch {
                    null => "",
                    _ => Description,
                };
                if(this.Arguments != null && this.Arguments.Count > 0) {
                    description += "Available Options:";
                    var done = new SortedDictionnary<string, IArgument>();
                    foreach(var entry in this.Arguments) {
                        var name = entry.Value.GetName();
                        if(!done.Contains(name))
                            done.Add(name, entry.Value);
                    }
                    foreach(var entry in done)
                        description += $"\n    {arg.Value.GetName()}: {arg.Value.GetHelp()}";
                }
                return description;
            }


            public virtual void Call(Context context, ref Command command) {
                throw new NotImplementedException("call method not implemented");
            }
    }
}
