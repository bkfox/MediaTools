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
        public void TestFromYAML() {
            var commands = Commands.FromYAML(Schema);
            Assert.Equal("commands_1", commands.Name);
            Assert.Equal(CommandsItems.Count, commands.Items.Count);
            for(var i=0; i < CommandsItems.Count; i++)
                Assert.Equal(CommandsItems[i].AsString(), commands.Items[i].AsString());
        }
    }
}