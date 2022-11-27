/// Note: see Runtime namespace for the IAction interface definition.
using System;
using System.IO;
using System.Diagnostics;

namespace MediaTools.Core.Actions {
    using MediaTools.Core;

    /// Execute an external program.
    class ExecAction<Context> : Action<Context>
    {
        public string? Program {get; set;}
        public string? Args {get; set;}
        public bool UseShell {get; set; } = false;

        public ExecAction(string name, string? program, string? args=null, string? description=null)
            : base(name)
        {
            if(description != null)
                Description = description;
            Program = program;
            Args = args;
        }

        protected virtual (string, string?) GetProgram(string? defaultProgram=null) {
            string? program = Program switch {
                null => defaultProgram,
                _ => Program,
            };
            if(program == null)
                throw new Exception("No editor found");
            return (program, Args);
        }

        protected virtual Process GetProcess(string program, string? args) {
            var process = new Process();
            process.StartInfo.FileName = program;
            if(args != null)
                process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = UseShell;
            return process;
        }

        public override void Call(ref IRuntime<Context> runtime, ref Context context, ref Command command)
        {
            var (program, args) = GetProgram();
            var process = GetProcess(program, args);
            process.Start();
        }
    }

    /// Provide base YAML template file used in order to generate commands.
    /// It runs as follow:
    /// - Call from Runtime as a Command
    /// - Get file data and write it into temporary file
    /// - Launch user editor
    /// - Call subsequent actions from Application
    class EditRunAction<Context> : ExecAction<Context>
    {
        public delegate string DataGetter(ref Context app, ref Command command);

        public DataGetter GetData;

        public EditRunAction(string name, DataGetter getData, string? program, string? programArgs=null, string? description=null)
            : base(name, program, programArgs, description)
        {
            GetData = getData;
        }

        protected override (string, string?) GetProgram(string? defaultProgram=null) {
            if(defaultProgram is null)
                defaultProgram = Environment.GetEnvironmentVariable("EDITOR");
            return base.GetProgram(defaultProgram);
        }

        public override void Call(ref IRuntime<Context> runtime, ref Context context, ref Command command)
        {
            var path = Path.Combine(
                Path.GetTempPath(),
                Path.ChangeExtension(Guid.NewGuid().ToString(), "yaml")
            );
            var data = GetData(ref context, ref command);
            File.WriteAllText(path, data);

            // call editor
            var (program, args) = GetProgram();
            if(args == null)
                args = path;
            else
                args = $"{args} {path}";
            var process = GetProcess(program, args);
            process.Start();

            // read data and execute
            var result = File.ReadAllText(path);
            Commands commands = Commands.FromYAML(result);
            runtime.Run(context, commands);
        }
    }

}
