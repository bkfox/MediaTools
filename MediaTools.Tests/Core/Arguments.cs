namespace MediaTools.Tests.Core {
    using System.Collections.Generic;
    using Xunit;
    using MediaTools.Core;

    record Options {
        // positional
        public string first = "";
        public List<int>? second = null;
        public List<int>? third = null;
        // named
        public int a = -1;
        public List<int>? b;
    }

    public class TestArguments {
        Arguments Args = new Arguments(new IArgument[] {
            new Argument<string>("Positional 1", "first"),
            new Argument<int>("Positional 2", "second", 2),
            new Argument<int>("Position 3", "third", -1),
            new Argument<int>("-a|--arg", "a"),
            new Argument<int>("-b|--b", "b", -1),
        });

        [Fact]
        public void TestReadOptions() {
            var options = new Options();
            var input = new List<string>{"first", "-b", "9", "10", "11",
                                         "-a", "12",
                                         // positional values
                                         "20", "21", "30", "31", "32"};
            Args.Read(ref options, input);

            Assert.Equal("first", options.first);
            Assert.Equal(new List<int>{20, 21}, options.second);
            Assert.Equal(new List<int>{30, 31, 32}, options.third);

            Assert.Equal(12, options.a);
            Assert.Equal(new List<int>{9, 10, 11}, options.b);
        }
    }
}