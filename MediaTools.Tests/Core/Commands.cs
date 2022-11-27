namespace MediaTools.Tests.Core {
    using System;
    using System.Collections.Generic;
    using Xunit;
    using MediaTools.Core;

    public class CommandsTests {
        List<Command> CommandsItems = new List<Command> {
            new Command("command_1 a b c"),
            new Command("command_2 d e f"),
        };
        string Schema = @"name: commands_1";

        public CommandsTests() {
            Schema += "\nrun:";
            foreach(var command in CommandsItems)
                Schema += $"\n    - {command.AsString()}";
        }

        [Fact]
        public void TestToFromYAML() {
            var commands = new Commands(CommandsItems);
            var schema = commands.ToYAML();
            var result = Commands.FromYAML(schema);

            Assert.Equal(commands.Name, result.Name);
            Assert.Equal(commands.Items.Count, result.Items.Count);
            for(var i=0; i < commands.Items.Count; i++)
                Assert.Equal(commands.Items[i].AsString(), result.Items[i].AsString());
        }
    }
}