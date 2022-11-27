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
        /// Number of expected arguments (when not parsing a flag). Accepted values are:
        /// - `-1`: unlimited list of values
        /// - `0`: (only booleans) flag argument, no value;
        /// - `n>0`: accept up to `n` items.
        /// </summary>
        public int GetCount();
        /// <summary>
        /// Return argument help string.
        /// </summary>
        public string GetHelp();
        /// <summary>
        /// Return expected type of value (as `List<T>` when many is true)
        /// </summary>
        /// <param name="baseType">If True, return T even when many is True</param>
        public Type GetValueType(bool baseType=false);

        public bool Many() => GetCount() == -1 || GetCount() > 1;

        /// <summary>
        /// Read input value, index indicates where is the argument name.
        /// Return count of read values.
        /// </summary>
        public int Read(IEnumerable<string> values, int index, ref object target);

        /// <summary>
        /// Create a new list.
        /// </summary>
        public object CreateList();

        /// <summary>
        /// Return true if list is full filled.
        /// </summary>
        public bool ListIsFull(object list);

        /// <summary>
        /// Parse single input string value.
        /// </summary>
        public object Parse(string value) {
            var type = GetValueType(true);
            // TODO: handle boolean
            return type == typeof(string) ? value : Convert.ChangeType(value, type);
        }
    }

    public class Argument<T> : Attribute, IArgument
    {
        public string Name {get; set;} = "";
        public string Attribute {get;set;} = "";
        public int Count {get; set;} = typeof(T) == typeof(Boolean) ? 0 : 1;
        public string? Help {get; set;}

        public Argument() {}

        public Argument(string name, string attribute, int? count=null, string? help=null)
        {
            if(count == 0 && typeof(T) != typeof(Boolean))
                throw new ArgumentException("Non boolean types must accept at least " +
                                            "one value");
            Name = name;
            Attribute = attribute;
            Help = help;
            if(count != null)
                Count = (int)count;
        }

        public string GetName() => Name;
        public string GetAttribute() => Attribute;
        public int GetCount() => Count;
        public string GetHelp() => Help == null ? "" : Help;
        public Type GetValueType(bool baseType=false)
            => baseType || (-1 < GetCount() && GetCount() <= 1) ?
                typeof(T) : typeof(List<T>);

        public int Read(IEnumerable<string> values, int index, ref object target) {
            var count = GetCount();
            values = values.Skip(index);

            // Parse a no argument value
            if(count == 0) {
                if(typeof(T) == typeof(Boolean))
                    target = !(bool)target;
                return 0;
            }
            // Parse a single item
            if(count == 1) {
                target = ((IArgument)this).Parse(values.First());
                return 1;
            }

            // Parse a list of items
            var list = (List<T>)target;
            var initialCount = list.Count();

            values = values.TakeWhile(s => !s.StartsWith('-'));
            if(count > -1) {
                var delta = this.GetCount() - list.Count;
                if(delta == 0)
                    return 0;
                values = values.Take(delta);
            }

            var args = values.Select(x => (T)((IArgument)this).Parse(x));
            list.AddRange(args);
            return list.Count - initialCount;
        }

        public object CreateList() => new List<T>();

        public bool ListIsFull(object list) =>
            GetCount() >= 0 && ((List<T>)list).Count >= GetCount();
    }




    //[AttributeUsage(AttributeTargets.Field)]
    //ArgumentAttribute : Argument
}