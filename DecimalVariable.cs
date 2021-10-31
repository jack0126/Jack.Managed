using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jack.Managed.Util;

namespace Jack.Managed
{
    public class DecimalVariable
    {
        private readonly TextVariable variable;
        internal DecimalVariable(TextVariable variable)
        {
            this.variable = variable;
        }

        public string Comment => variable.Comment;

        public decimal Value
        {
            get => decimal.TryParse(variable.Value, out var val) ? val : 0.0M;
            set => variable.Value = value.ToString();
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
