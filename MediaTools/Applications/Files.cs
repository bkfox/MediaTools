/// <summary>
/// Provide actions and runtime in order to manage files.
/// </summary>
namespace MediaTools.Files {
    using MediaTools.Core;

    public class FilesContext {
        /// <summary>
        /// Selected files path.
        /// </summary>
        public SortedSet<string> Files;

        FilesContext() {
            Files = new SortedSet<string>();              
        }
    }

    public class SelectAction : ArguedAction<FilesContext> {
        public override string Name {get;set;} = "select";
        public override string Label {get; set; } = "Select";
        public override string Description {get;set;} = "Select multiple files to work on.";
        public override Arguments Arguments {get;set;} = new Arguments(new IArgument[]{
            new Argument<string>() { Name="FILES", Attribute="Files", Count=-1},
            new Argument<bool>() { Name="-r|--reset", Attribute="Reset"},
            new Argument<bool>() { Name="-p|--print", Attribute="Print",
                                   Help="Print selected files."},
        });

        record Options(bool Reset = false, List<string>? Files = null, bool Print=false);

        public override void Call(ref IRuntime<FilesContext> runtime, ref FilesContext context,
                                  ref Command command)
        {
            var options = new Options();
            Arguments.Read(ref options, command.Args);
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

    // FIXME: we want to rename file one at a time, eventually using regex
    // Having something like:
    //
    // ```
    // - select *.py
    // - mv name_regex rename_expr_rx
    // - mv -s name_regex_in_select rename_expr_rx
    //
    // ```
    //
    // TODO:
    // - Arguments:
    //   - Many as a number: -1 (unlimited), 0 (none), n (n items)
    //   - multiple default arguments
    //
    public class MoveAction : ArguedAction<FilesContext> {
        public override string Name {get;set;} = "mv";
        public override string Label {get; set;} = "Move";
        public override string Description {get;set;} = "Move files to target directory";
        public override Arguments Arguments {get;set;} = new Arguments(new IArgument[]{
            new Argument<string>() { Name="FROM", Attribute="From", Help="Source path"},
            new Argument<string>() { Name="TO", Attribute="To", Help="Destination path"},
            new Argument<string>() { Name="-t|--target", Attribute="Target", Help="Target directory"},
            new Argument<bool>() { Name="-s|--use-selection", Attribute="UseSelection",
                                   Help="Use files selected by `select`."},
        });

        record Options(List<string> Files, string? From=null, string? To=null, bool UseSelection=false);

        public override void Call(ref IRuntime<FilesContext> runtime, ref FilesContext context, 
                                  ref Command command)
        {
            var options = new Options(new List<string>());
            Arguments.Read(ref options, command.Args);
            if(options.UseSelection)
                options.Files.AddRange(context.Files);
            var target = (options.To is null) ? Directory.GetCurrentDirectory()
                                                  : options.To;
            foreach(var file in options.Files)
                System.IO.File.Move(file, target);
        }
    }
}
