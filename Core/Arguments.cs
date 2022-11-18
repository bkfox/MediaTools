namespace MediaTools.Core {
    using System;
    using System.Collections.Generic;

    public class Arguments
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
        public Add(IEnumerable<IArgument> arguments) {
            foreach(var arg in arguments)
                Add(arg);
        }

        /// <summary>
        /// Parse input argument string iterable and assign values to target object.
        /// </summary>
        public void Parse<Target>(out Target target, IEnumerable<string> args)
        {
            IArgument? current = null;
            int (i, step) = (0, 1);
            while(i < args.Count) {
                var arg = args[i];
                IArgument? mapping = (arg[0] == "-") ? Mapping[arg] : Default;
                if(mapping == null)
                    throw KeyNotFoundException($"Invalid argument '{arg}'");
                if(arg[0] == '-')
                    i++;

                object? value = null;
                var property = typeof(target).GetProperty(mapping.GetAttribute());
                if(mapping.GetType() != property.PropertyType)
                    throw new NotSupportedException(
                        "Mapping and target property are not of the same type for " +
                        " argument '{arg}'");

                if(!mapping.ExpectValue())
                    value = (object)true;
                else if(!mapping.ExpectMany()) {
                    value = mapping.Parse(args[i+1]);
                    i++;
                }
                else {
                    value = property.GetValue(target, null);
                    if(value is null)
                        value = mapping.CreateList();
                    i += mapping.ParseList(args, i, value);
                }
                property.SetValue(target, value);
            }
        }
    }
}