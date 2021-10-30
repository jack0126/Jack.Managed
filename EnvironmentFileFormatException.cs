using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack.Managed
{
    public class EnvironmentFileFormatException : Exception
    {
        public EnvironmentFileFormatException()
        {
        }
        public EnvironmentFileFormatException(string message) : base(message)
        {
        }
    }
}
