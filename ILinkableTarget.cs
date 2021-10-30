using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack.Managed
{
    public interface ILinkableTarget
    {
        void Linkup(object src);
    }
}
