namespace MediaTools.Core {
    using System.Collections.Generic;
    using System.Text.RegularExpressions;


    /// <summary>
    /// Describe a user input command passed down to an IAction in order to
    /// execute an action.
    /// </summary>
    public struct Command {
        /// <summary>Regular expression used to parse user input string</summary>
        static Regex RegWords = new Regex(
                @"\s*(""(?<str>(\\""|[^""])+)""|(?<expr>(\\\s|[^\s])+))(\s+|\s*$)?",
                RegexOptions.Compiled);

        /// <summary>Command name</summary>
        public string Name { get; set; }
        /// <summary>Command arguments</summary>
        public List<String> Args {get; set; }

        /// <summary>
        /// Return True if command is empty.
        /// </summary>
        public bool IsEmpty {
            get => Name.Length == 0;
        }

        public Command() {
            Name = "";
            Args = new List<string>();
        }

        /// <summary>
        /// Instanciate command using provided name and args.
        /// </summary>
        public Command(string name, List<string> args)
        {
            Name = name;
            Args = args;
        }

        /// <summary>
        /// Instanciate command reading from input string.
        /// </summary>
        public Command(string input) : this() {
            ReadString(input);
        }

        /// <summary>
        /// Read command values from input string.
        /// </summary>
        void ReadString(string input) {
            Name = "";
            Args.Clear();

            input = input.Trim();
            if(input == string.Empty)
                return;

            var match = RegWords.Match(input);
            while(match.Success) {
                var groups = match.Groups;
                string arg;
                if(!string.IsNullOrEmpty(groups["expr"].Value))
                    arg = groups["expr"].Value.Replace("\\ ", " ");
                else
                    arg = groups["str"].Value.Replace("\\\"", "\"");
                if(Name == String.Empty)
                    Name = arg;
                else
                    Args.Add(arg);
                match = match.NextMatch();
            }
        }

        /// <summary>
        /// Return a new command whose name is this first arg, and args
        /// this next arguments
        /// </summary>
        public Command GetSubCommand() {
            var name = Args[0];
            var args = Args.Count switch {
                >= 2 => Args.GetRange(1, Args.Count-1),
                _ => new List<string>(),
            };
            return new Command(name, args);
        }

        /// <summary>Return Command as string</summary>
        public string AsString() {
            var str = Name + " ";
            foreach(var arg in Args)
                if(arg.Contains(" "))
                    str += $"\"{arg.Replace("\"", "\\\"")}\" ";
                else
                    str += $"{arg} ";
            return str;
        }
    }
}