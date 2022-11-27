
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
            Arg = new Argument<int>(Name, Attr, -1, Help);
        }

        [Fact]
        public void TestGetters() {
            Assert.Equal(Arg.GetName(), Name);
            Assert.Equal(Arg.GetAttribute(), Attr);
            Assert.Equal(Arg.GetCount(), -1);
            Assert.Equal(Arg.GetHelp(), Help);
            Assert.Equal(Arg.GetValueType(), typeof(List<int>));

            Arg.Count = 0;
            Assert.Equal(Arg.GetValueType(), typeof(int));
        }

        [Fact]
        public void TestReadFlag() {
            var arg = new Argument<bool>("arg", "arg");
            var input = new string[]{"-a"};
            var target = (object)false;
            var count = arg.Read(input, 0, ref target);
            Assert.Equal(0, count);
            Assert.True((bool)target);
        }

        [Fact]
        public void TestReadValue() {
            var arg = new Argument<int>("arg", "arg");
            var input = new string[]{"-a", "13"};
            var target = (object)0;
            var count = arg.Read(input, 1, ref target);
            Assert.Equal(1, count);
            Assert.Equal((int)target, 13);
        }

        [Fact]
        public void TestReadList() {
            var input = new string[]{"-a", "13", "14", "15", "-b", "13"};
            var expected = new List<int>{13,14,15};

            object target = new List<int>();
            var count = Arg.Read(input, 1, ref target);

            var list = (List<int>)target;
            Assert.Equal(count, list.Count);
            Assert.Equal(expected, list);
        }
    }
}