using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jack.Managed.Util;

namespace Jack.Managed
{
    public class DoubleVariable
    {
        private readonly TextVariable variable;
        internal DoubleVariable(TextVariable variable)
        {
            this.variable = variable;
        }

        public string Comment => variable.Comment;

        public double Value
        {
            get => double.TryParse(variable.Value, out var val) ? val : 0.0D;
            set => variable.Value = value.ToString();
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
