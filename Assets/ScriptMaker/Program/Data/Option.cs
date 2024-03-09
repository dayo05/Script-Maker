using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptMaker.Program.Data
{
    public class Option
    {
        public readonly List<Option> subOptions = new();
        private string _key;

        private string _value;

        public Option(string key, object value) : this(key, value.ToString())
        {
        }

        public Option(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key
        {
            get => _key.ToRawOptionString();
            private set => _key = value.ToSafeOptionString();
        }

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

        public IEnumerable<Option> this[string key] => subOptions.Where(x => x.Key == key);

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
        {
            return ToString(0);
        }

#pragma warning disable 0809 //Remove ToString method on option
        [Obsolete("Do not use ToString to option. Use Export() instead", true)]
        public override string ToString()
        {
            throw new InvalidOperationException("Do not use ToString to option. Use Export() instead");
        }
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

    public class Options : List<Option>
    {
        public Option this[string s] => FindLast(x => x.Key == s);
    }

    public static class OptionUtil
    {
        public static int Int(this IEnumerable<Option> o)
        {
            return o.First().Int;
        }

        public static float Float(this IEnumerable<Option> o)
        {
            return o.First().Float;
        }

        public static double Double(this IEnumerable<Option> o)
        {
            return o.First().Double;
        }

        public static bool Bool(this IEnumerable<Option> o)
        {
            return o.First().Bool;
        }

        public static string String(this IEnumerable<Option> o)
        {
            return o.First().String;
        }

        public static long Long(this IEnumerable<Option> o)
        {
            return o.First().Long;
        }

        public static Option CreateOption(this (string, string) k)
        {
            return new Option(k.Item1, k.Item2);
        }

        public static string ToSafeOptionString(this string s)
        {
            return s.Replace("\r", "").Replace("\\", "\\\\").Replace("\n", "\\n").Replace(">", "\\>");
        }

        public static string ToRawOptionString(this string s)
        {
            return s.Replace("\r", "").Replace("\\>", ">").Replace("\\n", "\n").Replace("\\\\", "\\");
        }
    }
}