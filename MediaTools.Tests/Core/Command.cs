namespace MediaTools.Tests.Core {
    using System.Collections.Generic;
    using Xunit;
    using MediaTools.Core;

    public class CommandTests {
        string Input = "name arg1 \"arg 2\" arg\\ 3 \"arg\\\"4\" ";
        List<string> Args = new List<string>{"arg1","arg 2", "arg 3", "arg\"4"};

        [Fact]
        void TestReadString() {
            var command = new Command(Input);
            Assert.Equal(command.Name, "name");
            Assert.Equal(Args, command.Args);
        }

        [Fact]
        void TestGetSubCommand() {
            var command = new Command(Input);
            var subCommand = command.GetSubCommand();
            Assert.Equal(Args.Count-1, subCommand.Args.Count);
            Assert.Equal(Args[0], subCommand.Name);
            for(var i=0; i<subCommand.Args.Count; i++)
                Assert.Equal(Args[i+1], subCommand.Args[i]);
        }
    }
}