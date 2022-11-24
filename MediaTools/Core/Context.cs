namespace MediaTools.Core {
    public interface IContext
    {
        object GetRuntime();
    }

    public class Context<C> : IContext
        where C: IContext
    {
        IRuntime<C> Runtime {get; set;}

        public Context(IRuntime<C> runtime) {
            Runtime = runtime;
        }

        public object GetRuntime() {
            return (object)Runtime;
        }
    }
}