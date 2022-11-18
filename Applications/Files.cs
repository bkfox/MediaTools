/// <summary>
/// Provide actions and runtime in order to manage files.
/// </summary>
namespace MediaTools.Files {
    using MediaTools.Core;
    using Mono.Options;

    public class FilesContext : Context {
        /// <summary>
        /// Selected files path.
        /// </summary>
        SortedSet<string> Files;

        FilesContext(IRuntime<FilesContext> runtime)
            : base(runtime)
        {
            Files = new SortedSet<string>();              
        }
    }

    public class SelectAction : Action<FilesContext> {
        protected string Name = "select";
        protected string Description = "Select multiple files to work on.";
        protected Arguments? Arguments = new Arguments(new []{
            new Argument<string>() { Name="FILES", Attribute="Files", Many=true},
            new Argument<bool>() { Name="-r|--reset", Attribute="Reset"},
            new Argument<bool>() { Name="-p|--print", Attribute="Print",
                                   Help="Print selected files."},
        });

        record Options(bool Reset = false, List<string>? Files = null, bool Print=false);

        public void Call(Context context, ref Command command) {
            var options = new Options();
            Arguments.Parse(options, command.args);
            if(options.Reset)
                context.Files.Clear();
            if(options.Files != null)
                foreach(var file in options.Files)
                    foreach(var result in Directory.GetFiles("./", file))
                        context.Files.Add(result);
            if(options.Print)
                foreach(var file in context.Files)
                    Console.WriteLine(file);
        }
    }

    public class MoveAction : Action<FilesContext> {
        protected string Name {get;set;} = "mv";
        protected string Description = "Move files to target directory";
        protected public Arguments? Arguments = new Arguments(new []{
            new Argument<string>() { Name="FILES", Attribute="Files", Many=true, Help="Input files"},
            new Argument<string>() { Name="-t|--target", Attribute="Reset", Help="Target directory"},
            new Argument<bool>() { Name="-s|--use-selection", Attribute="UseSelect",
                                   Help="Use files selected by `select`."},
        });
    }
}
