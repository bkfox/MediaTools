namespace MediaTools.Core {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provide argument interface used by `Arguments` class in order
    /// to parse user input.
    /// </summary>
    public interface IArgument {
        /// <summary>
        /// Return attribute name as list of '|' possible user option.
        /// Example: "-v|--verbose".
        /// </summary>
        public string GetName();
        /// <summary>
        /// Return attribute name on target options object.
        /// </summary>
        public string GetAttribute();
        /// <summary>
        /// Return true if argument expects many values.
        /// </summary>
        public bool ExpectMany();
        /// <summary>
        /// Return argument help string.
        /// </summary>
        public string GetHelp();
        /// <summary>
        /// Return expected type of value (as `List<T>` when many is true)
        /// </summary>
        /// <param name="baseType">If True, return T even when many is True</param>
        public Type GetValueType(bool baseType=false);

        /// <summary>
        /// Return true if argument expects a value.
        /// </summary>
        public bool ExpectValue() {
            return GetType() != typeof(bool);
        }

        /// <summary>
        /// Parse single input string value.
        /// </summary>
        public object Parse(string value) {
            var type = GetValueType(true);
            // TODO: handle boolean
            return type == typeof(string) ? value : Convert.ChangeType(value, type);
        }

        /// <summary></summary>
        public object CreateList();
        /// <summary>
        /// Parse input values into provided list.
        /// </summary>
        public int ParseList(IEnumerable<string> values, int index, ref object target);
    }

    public class Argument<T> : Attribute, IArgument {
        public string Name {get; set;} = "";
        public string Attribute {get;set;} = "";
        public bool Many {get;set;} = false;
        public string? Help {get; set;}

        public Argument() {}
        public Argument(string name, string attribute, bool many=false, 
                        string? help=null)
        {
            Name = name;
            Attribute = attribute;
            Many = many;
            Help = help;
        }

        public string GetName() => Name;
        public string GetAttribute() => Attribute;
        public bool ExpectMany() => Many;
        public string GetHelp() => Help == null ? "" : Help;
        public Type GetValueType(bool baseType=false)
            => !baseType && ExpectMany() ? typeof(List<T>) : typeof(T);

        public object CreateList() => new List<T>();

        public int ParseList(IEnumerable<string> values, int index, ref object target) {
            // TODO: take in account that a default value might follow and there can
            // be an error at converting input into option field
            if(target is not List<T>)
                throw new InvalidCastException("provided target object is not a List<T> where T is argument's target type");
            var list = (List<T>)target;
            var count = list.Count;
            var args = values.Skip(index).TakeWhile(s => !s.StartsWith('-'))
                             .Select(x => (T)((IArgument)this).Parse(x));
            list.AddRange(args);
            return list.Count - count;
        }
    }

    //[AttributeUsage(AttributeTargets.Field)]
    //ArgumentAttribute : Argument
}