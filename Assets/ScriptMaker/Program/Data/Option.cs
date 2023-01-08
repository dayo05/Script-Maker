using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptMaker.Program.Data
{
    public class Option
    {
        private string _key;
        public string Key
        {
            get => _key.ToRawOptionString();
            private set => _key = value.ToSafeOptionString();
        }

        private string _value;
        public string Value
        {
            get => _value.ToRawOptionString();
            set => _value = value.ToSafeOptionString();
        }

        public int Int => int.Parse(Value);
        public string String => Value;
        public double Double => double.Parse(Value);
        public float Float => float.Parse(Value);
        public bool Bool => bool.Parse(Value);
        public long Long => long.Parse(Value);

        public readonly List<Option> subOptions = new();

        public Option(string key, object value) : this(key, value.ToString()) { }

        public Option(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public Option Append(string key, object value)
        {
            subOptions.Add((key, value.ToString()).CreateOption());
            return this;
        }
        public Option Append(string key, string value)
        {
            subOptions.Add((key, value).CreateOption());
            return this;
        }

        public Option Append(Option option)
        {
            subOptions.Add(option);
            return this;
        }

        public IEnumerable<Option> this[string key] => subOptions.Where(x => x.Key == key);

        public Option Set(string key, string value)
        {
            var opt = new Option(key, value);
            subOptions.RemoveAll(x => x.Key == key);
            subOptions.Add(opt);
            return opt;
        }

        public IEnumerable<Option> SetDefaultOf(string key, string defaultValue)
        {
            if (subOptions.All(x => x.Key != key))
                Append(key, defaultValue);
            return subOptions.Where(x => x.Key == key);
        }

        public Option Clone()
        {
            var opt = new Option(Key, Value);
            subOptions.ForEach(x => opt.Append(x.Clone()));
            return opt;
        }

        public string Export()
            => ToString(0);

        #pragma warning disable 0809 //Remove ToString method on option
        [Obsolete("Do not use ToString to option. Use Export() instead", true)]
        public override string ToString()
            => throw new InvalidOperationException("Do not use ToString to option. Use Export() instead");
        #pragma warning restore 0809

        private string ToString(int dim)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < dim; i++)
                sb.Append('>');
            sb.Append(_key);
            sb.Append('=');
            sb.AppendLine(_value);
            foreach (var opt in subOptions)
                sb.Append(opt.ToString(dim + 1));
            return sb.ToString();
        }
    }

    public static class OptionUtil
    {
        public static int Int(this IEnumerable<Option> o) => o.First().Int;
        public static float Float(this IEnumerable<Option> o) => o.First().Float;
        public static double Double(this IEnumerable<Option> o) => o.First().Double;
        public static bool Bool(this IEnumerable<Option> o) => o.First().Bool;
        public static string String(this IEnumerable<Option> o) => o.First().String;
        public static long Long(this IEnumerable<Option> o) => o.First().Long;
        public static Option CreateOption(this (string, string) k)
            => new(k.Item1, k.Item2);

        public static string ToSafeOptionString(this string s)
            => s.Replace("\r", "").Replace("\\", "\\\\").Replace("\n", "\\n").Replace(">", "\\>");

        public static string ToRawOptionString(this string s)
            => s.Replace("\r", "").Replace("\\>", ">").Replace("\\n", "\n").Replace("\\\\", "\\");
    }
}