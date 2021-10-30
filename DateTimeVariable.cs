using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Jack.Managed
{
    public class DateTimeVariable
    {
        private TextVariable variable;
        internal DateTimeVariable(TextVariable variable, string format = null)
        {
            this.variable = variable;
            this.format = format ?? "yyyy-MM-dd HH:mm:ss";
        }

        private string format;

        public string Comment => variable.Comment;

        public DateTime Value
        {
            get => DateTime.ParseExact(variable.Value, format, null, DateTimeStyles.AssumeLocal);
            set => variable.Value = value.ToString(format);
        }
        public override string ToString()
        {
            return Value.ToString(format);
        }
        
    }
}
