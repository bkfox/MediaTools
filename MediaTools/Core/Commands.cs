namespace MediaTools.Core {
    using System.Collections;
    using System.Collections.Generic;

    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    public class Commands : IEnumerable<Command> {
        /// <summary>Optional name of command set.</summary>
        public string? Name;

        /// <summary>List of commands to execute, as FIFO.</summary>
        public List<Command> Items;

        // FIXME: this is a hack to avoid having to implement YamlDotNet interfaces
        //        -> lack of documentation
        [YamlMemberAttribute(Alias="run")]
        public List<string>? ItemsInput;

        /// <summary>Deserializer used to read Commands</summary>
        static public IDeserializer Deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        public Commands() {
            Items = new List<Command>();
        }

        public Commands(List<Command> items) {
            Items = items;
        }

        /// <summary>Read YAML commands string</summary>
        static public Commands FromYAML(string input) {
            var commands = Deserializer.Deserialize<Commands>(input);
            if(commands.ItemsInput != null)
                commands.Items = commands.ItemsInput.Select(r => new Command(r))
                                    .ToList();
            return commands;
            /*Commands commands = new Commands();
            commands.ReadYAML(input);
            return commands;
        }
        
        /// Read YAML commands file
        public void ReadYAML(string input) {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
            var inputs = deserializer.Deserialize<Commands>(input);
            foreach(var step in inputs) {
                foreach(var entry in step) {
                    var command = Command.Read(entry.Value, entry.Key);
                    if(command != null)
                        Items.Add((Command)command);
                }
            }*/
        }

        public IEnumerator<Command> GetEnumerator() {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}