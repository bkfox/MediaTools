namespace MediaTools.Core {
    using System.Collections;
    using System.Collections.Generic;

    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    public class Commands /*: IEnumerable<Command> */ {
        /// <summary>Optional name of command set.</summary>
        public string? Name {get; set;}

        /// <summary>Required runtime by name.</summary>
        public string? Runtime {get; set;}

        /// <summary>List of commands to execute, as FIFO.</summary>
        [YamlIgnore]
        public List<Command> Items;

        // FIXME: this is a hack to avoid having to implement YamlDotNet interfaces
        //        -> lack of documentation
        [YamlMemberAttribute(Alias="run")]
        public List<string>? ItemsInput {get; set;}

        /// <summary>Serializer used to write Commands</summary>
        static public ISerializer Serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        /// <summary>Deserializer used to read Commands</summary>
        static public IDeserializer Deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        public Commands() {
            Items = new List<Command>();
        }

        public Commands(IEnumerable<Command> items) {
            Items = items.ToList();
        }

        /// <summary>Read YAML commands string</summary>
        static public Commands FromYAML(string input) {
            var commands = Deserializer.Deserialize<Commands>(input);
            if(commands.ItemsInput != null)
                commands.Items = commands.ItemsInput.Select(r => new Command(r))
                                    .ToList();
            commands.ItemsInput = null;
            return commands;
        }

        public string ToYAML() {
            ItemsInput = Items.Select(r => r.AsString()).ToList();
            var result = Serializer.Serialize(this);
            ItemsInput = null;
            return result;
        }

        /*
        // must be ignored because yaml serialization serialize this as
        // a list.
        public IEnumerator<Command> GetEnumerator() {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }*/
    }
}