using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jack.Managed.Util;

namespace Jack.Managed
{
    public class LongVariable
    {
        private readonly TextVariable variable;
        internal LongVariable(TextVariable variable)
        {
            this.variable = variable;
        }

        public string Comment => variable.Comment;

        public long Value
        {
            get => long.TryParse(variable.Value, out var val) ? val : 0L;
            set => variable.Value = value.ToString();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
