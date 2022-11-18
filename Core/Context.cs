namespace MediaTools.Core {
    public interface IContext
    {
        object GetRuntime();
    }

    public class Context : IContext {
        IRuntime<Context> Runtime {get; set;}

        public Context(IRuntime<Context> runtime) {
            Runtime = runtime;
        }

        public object GetRuntime() {
            return (object)Runtime;
        }
    }
}