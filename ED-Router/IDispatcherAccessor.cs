using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED_Router
{
    public interface IDispatcherAccessor
    {
        void Invoke(Action action);
    }
}
