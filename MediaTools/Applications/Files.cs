/// <summary>
/// Provide actions and runtime in order to manage files.
/// </summary>
namespace MediaTools.Files {
    using MediaTools.Core;

    public class FilesContext : Context<FilesContext> {
        /// <summary>
        /// Selected files path.
        /// </summary>
        public SortedSet<string> Files;

        FilesContext(IRuntime<FilesContext> runtime)
            : base(runtime)
        {
            Files = new SortedSet<string>();              
        }
    }

    public class SelectAction : ArguedAction<FilesContext> {
        public override string Name {get;set;} = "select";
        public override string Description {get;set;} = "Select multiple files to work on.";
        public override Arguments Arguments {get;set;} = new Arguments(new IArgument[]{
            new Argument<string>() { Name="FILES", Attribute="Files", Many=true},
            new Argument<bool>() { Name="-r|--reset", Attribute="Reset"},
            new Argument<bool>() { Name="-p|--print", Attribute="Print",
                                   Help="Print selected files."},
        });

        record Options(bool Reset = false, List<string>? Files = null, bool Print=false);

        public override void Call(FilesContext context, ref Command command) {
            var options = new Options();
            Arguments.Parse(ref options, command.Args);
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

    public class MoveAction : ArguedAction<FilesContext> {
        public override string Name {get;set;} = "mv";
        public override string Description {get;set;} = "Move files to target directory";
        public override Arguments Arguments {get;set;} = new Arguments(new IArgument[]{
            new Argument<string>() { Name="FILES", Attribute="Files", Many=true, Help="Input files"},
            new Argument<string>() { Name="-t|--target", Attribute="Reset", Help="Target directory"},
            new Argument<bool>() { Name="-s|--use-selection", Attribute="UseSelect",
                                   Help="Use files selected by `select`."},
        });
    }
}
