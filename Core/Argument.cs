namespace MediaTools.Core {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provide argument interface used by `Arguments` class in order
    /// to parse user input.
    /// </summary>
    protected interface IArgument {
        /// <summary>
        /// Return attribute name as list of '|' possible user option.
        /// Example: "-v|--verbose".
        /// </summary>
        public string GetName();
        /// <summary>
        /// Return argument help string.
        /// </summary>
        public string GetHelp();
        /// <summary>
        /// Return attribute name on target options object.
        /// </summary>
        public string GetAttribute();
        /// <summary>
        /// Return expected type of value (as `List<T>` when many is true)
        /// </summary>
        public Type GetType();
        /// <summary>
        /// Return true if argument expects many values.
        /// </summary>
        public bool ExpectMany();

        /// <summary>
        /// Return true if argument expects a value.
        /// </summary>
        public bool ExpectValue() {
            return GetValueType() != typeof(bool);
        }

        /// <summary>
        /// Parse single input string value.
        /// </summary>
        public object Parse(string value) {
            var type = GetValueType();
            // TODO: handle boolean
            return Convert.ChangeType(value, type);
        }

        /// <summary></summary>
        public object CreateList();
        /// <summary>
        /// Parse input values into provided list.
        /// </summary>
        public int ParseList(List<string> values, int index, object target);
    }

    public struct Argument<T> : IArgument {
        string Name {get; set;}
        string Attribute {get;set;}
        string? Help {get; set;}
        bool Many {get;set;} = false;

        public string GetName() => Name;
        public string GetHelp() => Help == null ? "" : Help;
        public string GetAttribute() => Attribute;
        public bool ExpectMany() => Many;
        public Type GetType() => ExpectMany() ? typeof(List<T>) : typeof(T);
        public object CreateList() => new List<T>();

        public int ParseList(List<string> values, int index, object target) {
            if(target is not T)
                throw InvalidCastException("provided target object is not a List<T> where T is argument's target type");
            var list = (List<T>)target;
            var count = list.Count;
            list.AddRange(values.Skip(index).TakeWhile(s => !s.StartsWith('-')));
            return list.Count - count;
        }
    }
}