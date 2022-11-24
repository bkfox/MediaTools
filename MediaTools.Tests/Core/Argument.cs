
namespace MediaTools.Tests.Core {
    using System.Collections.Generic;
    using Xunit;
    using MediaTools.Core;

    public class TestArgument {
        string Name = "-a|--arg";
        string Attr = "arg";
        string Help = "help";
        Argument<int> Arg;

        public TestArgument() {
            Arg = new Argument<int>(Name, Attr, true, Help);
        }

        [Fact]
        public void TestGetters() {
            Assert.Equal(Arg.GetName(), Name);
            Assert.Equal(Arg.GetAttribute(), Attr);
            Assert.Equal(Arg.ExpectMany(), true);
            Assert.Equal(Arg.GetHelp(), Help);
            Assert.Equal(Arg.GetValueType(), typeof(List<int>));

            Arg.Many = false;
            Assert.Equal(Arg.GetValueType(), typeof(int));
        }

        [Fact]
        public void TestCreateList() {
            var list = Arg.CreateList();
            Assert.True(list is List<int>);
        }

        [Fact]
        public void TestParseList() {
            var input = new string[]{"-a", "13", "14", "15", "-b", "13"};
            var expected = new List<int>{13,14,15};

            var target = Arg.CreateList();
            var count = Arg.ParseList(input, 1, ref target);

            var list = (List<int>)target;
            Assert.Equal(list.Count, count);
            Assert.Equal(list, expected);
        }
    }
}