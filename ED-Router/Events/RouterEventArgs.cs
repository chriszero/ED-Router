using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED_Router.Events
{
    public class RouterEventArgs: EventArgs
    {
        public string System { get; set; }
        public string EventName { get; set; }
    }
}
