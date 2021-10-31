using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jack.Managed.Util;

namespace Jack.Managed
{
    public class BoolVariable
    {
        private readonly TextVariable variable;
        internal BoolVariable(TextVariable variable)
        {
            this.variable = variable;
        }

        public string Comment => variable.Comment;

        public bool Value
        {
            get => bool.TryParse(variable.Value, out var val) ? val : false;
            set => variable.Value = value.ToString();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
