namespace MediaTools.Tests.Core {
    using System.Collections.Generic;
    using Xunit;
    using MediaTools.Core;

    record Options {
        public string str = "";
        public int a = -1;
        public List<int>? b;
    }

    public class TestArguments {
        Arguments Args = new Arguments(new IArgument[] {
            new Argument<string>("Default","str"),
            new Argument<int>("-a|--arg", "a", false, "help a"),
            new Argument<int>("-b|--b", "b", true, "help b"),
        });

        [Fact]
        public void TestParseOptions() {
            var options = new Options();
            var input = new List<string>{"-b", "9", "10", "11",
                                         "-a", "12", "default"};
            Args.Parse(ref options, input);

            Assert.Equal(options.a, 12);
            Assert.Equal(options.b, new List<int>{9, 10, 11});
            Assert.Equal(options.str, "default");
        }
    }
}