using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jack.Managed.Util;

namespace Jack.Managed
{
    public class IntVariable
    {
        private readonly TextVariable variable;
        internal IntVariable(TextVariable variable)
        {
            this.variable = variable;
        }

        public string Comment => variable.Comment;

        public int Value
        {
            get => int.TryParse(variable.Value, out var val) ? val : 0;
            set => variable.Value = value.ToString();
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
