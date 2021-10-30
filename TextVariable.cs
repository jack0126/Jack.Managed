using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using Jack.Managed.Util;

namespace Jack.Managed
{
    public class TextVariable
    {
        private static readonly object IOLock = new object();
        internal const string __at_end__ = "__at_end__";
        internal static readonly string __at_end__Expression = "{__at_end__=}" + Environment.NewLine;

        private readonly VariableExpression expression;

        internal TextVariable(VariableExpression expression)
        {
            this.expression = expression;
        }

        internal string Name => expression.Name;

        internal string EnvironmentPath { private get; set; }

        internal string Expression => expression.ToString() + Environment.NewLine;
        
        internal static TextVariable Parse(string text)
        {
            return VariableExpression.TryParse(text, out var exp) ? new TextVariable(exp) : null;
        }

        private string _comment;
        public string Comment
        {
            get => _comment;
            internal set
            {
                _comment = value;
                if (_comment != null)
                {
                    var desc = new StringBuilder(32);
                    using (var sr = new StringReader(_comment))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Length != 0 && line.ElementAt(0) != '#')
                            {
                                desc.Append('#');
                            }
                            desc.Append(line).Append(Environment.NewLine);
                        }
                    }
                    _comment = desc.ToString();
                }
            }
        }
        
        public string Value
        {
            get => expression.Value;
            set
            {
                expression.Value = value;
                if (!string.IsNullOrEmpty(EnvironmentPath))
                {
                    var contents = Comment + Expression + __at_end__Expression;
                    lock (IOLock)
                    {
                        File.AppendAllText(EnvironmentPath, contents);
                    }
                }
            }
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
