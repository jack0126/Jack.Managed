using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack.Managed
{
    public class ManagedException : Exception
    {
        public ManagedException()
        {
        }
        public ManagedException(string msg) : base(msg)
        {
        }
        public ManagedException(string msg, Exception e) : base(msg, e)
        {
        }
    }
}
