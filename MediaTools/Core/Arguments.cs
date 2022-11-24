namespace MediaTools.Core {
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class Arguments : IEnumerable<IArgument>
    {
        SortedDictionary<string, IArgument> Mapping;
        IArgument? Default;

        public int Count {
            get => Mapping.Count;
        }

        public Arguments() {
            Mapping = new SortedDictionary<string, IArgument>();
        }

        public Arguments(IEnumerable<IArgument> arguments)
            : this()
        {
            Add(arguments);
        }

        public Arguments(IArgument[] arguments)
            : this()
        {
            Add(arguments);
        }

        /*public static Arguments FromOptions<T>() {
            var fields = typeof(T).GetFields();
            var args = fields.Select(x => Attribute.GetCustomAttribute(x, typeof(ArgumentAttribute)));
            foreach(var field in fields) {
                
            }
        }*/

        public void Add<T>(T argument)
            where T: IArgument
        {
            Add((IArgument)argument);
        }

        /// <summary>
        /// Register an argument.
        /// </summary>
        public void Add(IArgument argument) {
            var keys = argument.GetName().Split("|");
            foreach(var key in keys) {
                if(key.StartsWith("-"))
                    Mapping.Add(key, argument);
                else if(Default == null) {
                    if(!argument.ExpectValue())
                        throw new ArgumentException($"Default argument must not be a flag" +
                                                     "for argument ({argument.GetName()})");
                    Default = argument;
                }
                else
                    throw new ArgumentException(
                        $"Default argument already passed (default={Default.GetName()}, " +
                        $"{argument.GetName()})");
            }
        }

        /// <summary>
        /// Register multiple arguments at once.
        /// </summary>
        public void Add(IEnumerable<IArgument> arguments) {
            foreach(var arg in arguments)
                Add(arg);
        }

        /// <summary>
        /// Parse input argument string iterable and assign values to target object.
        /// </summary>
        public void Parse<Target>(ref Target target, List<string> args)
        {
            int i = 0;
            while(i < args.Count()) {
                var arg = args[i];
                IArgument? mapping = (arg[0] == '-') ? Mapping[arg] : Default;
                if(mapping == null)
                    throw new KeyNotFoundException($"Invalid argument '{arg}'");
                if(arg[0] == '-')
                    i++;

                object? value = null;
                var field = typeof(Target).GetField(mapping.GetAttribute());
                if(field == null) {
                    i++;
                    continue;
                }
                if(mapping.GetValueType() != field.FieldType)
                    throw new NotSupportedException(
                        "Mapping and target property are not of the same type for " +
                        " argument '{arg}'");

                if(!mapping.ExpectValue())
                    value = (object)true;
                else if(!mapping.ExpectMany()) {
                    value = mapping.Parse(args[i]);
                    i++;
                }
                else {
                    value = field.GetValue(target);
                    if(value is null)
                        value = mapping.CreateList();
                    var count = mapping.ParseList(args, i, ref value);
                    i += count > 0 ? count : 1;
                }
                field.SetValue(target, value);
            }
        }

        public IEnumerator<IArgument> GetEnumerator() {
            return this.Mapping.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}