namespace MediaTools.Core {
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class Arguments : IEnumerable<IArgument>
    {
        SortedDictionary<string, IArgument> Named;
        List<IArgument> Positional;

        public int Count {
            get => Named.Count;
        }

        public Arguments() {
            Named = new SortedDictionary<string, IArgument>();
            Positional = new List<IArgument>();
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
                    Named.Add(key, argument);
                else if(argument.GetCount() == 0)
                    throw new ArgumentException("Positional arguments must accept at least one value.");
                else
                    Positional.Add(argument);
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
        public void Read<Options>(ref Options options, List<string> args)
        {
            int i = 0;
            int posIndex = 0;
            while(i < args.Count()) {
                Console.WriteLine($"- loop {i} {args[i]}");
                var arg = args[i];
                (IArgument? mapping, bool isNamed) = arg[0] == '-' ? (Named[arg], true)
                        : posIndex < Positional.Count ? (Positional[posIndex], false)
                        : (null, false);
                if(mapping == null)
                    throw new KeyNotFoundException($"Invalid argument '{arg}'");
                if(isNamed)
                    i++;

                var field = typeof(Options).GetField(mapping.GetAttribute());
                if(field == null)
                    throw new FieldAccessException(
                        $"Field '{mapping.GetAttribute()}' not found on provided " +
                         "options object.");
    
                var castAllowed = this._CastAllowed(ref field, ref mapping);
                if(!castAllowed)
                    throw new NotSupportedException(
                        "Mapping and options' field must be of the same type for " +
                        $"argument '{arg}' ({field.FieldType} vs {mapping.GetValueType()})");

                var argCount = mapping.GetCount();
                var value = field.GetValue(options);
                if(value == null) {
                    if(argCount == 1)
                        throw new NotSupportedException(
                            "Option MUST provide a non-null value for field " +
                            $"{mapping.GetAttribute()}");
                    else if(argCount == 0)
                        continue;
                    else
                        value = mapping.CreateList();
                }

                var count = mapping.Read(args, i, ref value);
                field.SetValue(options, value);

                i += count;
                if(!isNamed && (!mapping.Many() || mapping.ListIsFull(value)))
                    posIndex++;
            }
        }

        protected bool _CastAllowed(ref System.Reflection.FieldInfo field, ref IArgument mapping) {
            var nullType = typeof(Nullable<>);
            var type = field.FieldType;
            var fieldType = type.IsGenericType && type.GetGenericTypeDefinition() == nullType
                            ? Nullable.GetUnderlyingType(type) : type;
            if(fieldType == null)
                throw new ArgumentNullException("field type is null");

            type = mapping.GetValueType();
            var mappingType = type.IsGenericType && type.GetGenericTypeDefinition() == nullType
                              ? Nullable.GetUnderlyingType(type) : type;
            if(mappingType == null)
                throw new ArgumentNullException("mapping type is null");
            return mappingType == fieldType;
        }

        public IEnumerator<IArgument> GetEnumerator() {
            return this.Named.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}