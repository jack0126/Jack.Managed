using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Jack.Managed.Util
{
    class VariableExpression
    {
        private static readonly Regex NameRegex = new Regex("^[a-zA-Z_][a-zA-Z_0-9]*$");

        private static bool IsValidName(string name)
        {
            return (!string.IsNullOrEmpty(name) && NameRegex.IsMatch(name));
        }

        private static bool IsValidEncodedValue(string value)
        {
            return !(value.Contains('{') || value.Contains('}'));
        }

        private static string EncodeValue(string s)
        {
            return s
                .Replace("$", "$s")
                .Replace("\\", "$/")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("{", "$l")
                .Replace("}", "$r")
                .ToString();
        }

        private static string DecodeValue(string s)
        {
            return s
                .Replace("$l", "{")
                .Replace("$r", "}")
                .Replace("\\r", "\r")
                .Replace("\\n", "\n")
                .Replace("$/", "\\")
                .Replace("$s", "$")
                .ToString();
        }

        private string _name;
        public string Name
        {
            get => _name;
            private set
            {
                if (IsValidName(value))
                {
                    _name = value;
                }
                else
                {
                    throw new IllegalExpressionNameException();
                }
            }
        }
        private string _value;
        public string Value
        {
            get => _value;
            set => _value = value ?? string.Empty;
        }

        public VariableExpression(string name) : this(name, string.Empty)
        {
        }
        public VariableExpression(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public static bool TryParse(string exp, out VariableExpression expression)
        {
            expression = null;
            if (exp != null && (exp = exp.Trim()).Length > 3 &&
                exp.ElementAt(0) == '{' && exp.ElementAt(exp.Length - 1) == '}' &&
                (exp = exp.Substring(1, exp.Length - 2)).ElementAt(0) != '=' &&
                exp.Contains('='))
            {
                var pos = exp.IndexOf('=');
                var name = exp.Substring(0, pos).Trim();
                pos++;

                if (IsValidName(name))
                {
                    var value = string.Empty;
                    if (pos < exp.Length)
                    {
                        if (IsValidEncodedValue(value = exp.Substring(pos).Trim()))
                        {
                            value = DecodeValue(value);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    expression = new VariableExpression(name, value);
                    return true;
                }
            }
            return false;
        }
        
        public override string ToString()
        {
            return '{' + Name + '=' + EncodeValue(Value).Trim() + '}';
        }

        public override bool Equals(object obj)
        {
            return (obj is VariableExpression exp) && Value == exp.Value;
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }
    }
    class IllegalExpressionNameException : ManagedException
    {
    }
}
