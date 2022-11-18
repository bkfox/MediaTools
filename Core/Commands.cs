namespace MediaTools.Core {
    using System.Collections;
    using System.Collections.Generic;

    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    public class Commands : IEnumerable<Command> {
        [YamlMemberAttribute(Alias="run", SerializeAs=typeof(String))]
        List<Command> Items;

        public Commands() {
            Items = new List<Command>();
        }

        public Commands(List<Command> items) {
            Items = items;
        }

        /// Read YAML commands string
        static public Commands FromYAML(string input) {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
            return deserializer.Deserialize<Commands>(input);
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