using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Jack.Managed
{
    public class EnvironmentFile
    {
        internal readonly Dictionary<string, TextVariable> variables = new Dictionary<string, TextVariable>(64);
        private readonly string path;

        private EnvironmentFile(string path)
        {
            this.path = path;
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
        }

        public static EnvironmentFile Open(string path)
        {
            return Open(path, Encoding.UTF8);
        }

        public static EnvironmentFile Open(string path, Encoding encoding)
        {
            return Open(path, encoding, false);
        }

        internal static EnvironmentFile Open(string path, Encoding encoding, bool bindLocal)
        {
            var file = new EnvironmentFile(path);
            string comment = null;
            TextVariable variable = null;

            foreach (var line in File.ReadAllLines(path, encoding))
            {
                variable = TextVariable.Parse(line);
                if (variable == null)
                {
                    comment += line + Environment.NewLine;
                    continue;
                }

                if (file.variables.ContainsKey(variable.Name))
                {
                    var newValue = variable.Value;
                    variable = file.variables[variable.Name];
                    variable.EnvironmentPath = null;
                    variable.Value = newValue;
                }
                else
                {
                    variable.Comment = comment;
                    file.variables.Add(variable.Name, variable);
                }

                comment = null;
                if (bindLocal)
                {
                    variable.EnvironmentPath = path;
                }
            }

            if (variable != null)
            {
                if (variable.Name != TextVariable.__at_end__)
                {
                    throw new EnvironmentFileFormatException($"'{TextVariable.__at_end__}' tag validation failed.");
                }
                file.variables.Remove(TextVariable.__at_end__);
            }

            return file;
        }
        
        public void SaveTo(string path)
        {
            SaveTo(path, Encoding.UTF8);
        }

        public void SaveTo(string path, Encoding encoding)
        {
            StringBuilder contents = new StringBuilder(1024);
            variables.Values.ToList().ForEach(e => contents.Append(e.Comment).Append(e.Expression));
            contents.Append(TextVariable.__at_end__Expression);
            File.WriteAllText(path, contents.ToString(), encoding);
        }

        public string this[string name]
        {
            get
            {
                if (variables.ContainsKey(name))
                {
                    return variables[name].Value;
                }
                else
                {
                    throw new Exception("can't read or write a variable that doesn't exist.");
                }
            }
            set
            {
                if (variables.ContainsKey(name))
                {
                    variables[name].Value = value;
                }
                else
                {
                    throw new Exception("can't read or write a variable that doesn't exist.");
                }
            }
        }

        public void ForEach(Action<string, string> action)
        {
            foreach(var val in variables.Values)
            {
                action(val.Name, val.Value);
            }
        }
    }
}
