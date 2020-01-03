using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ED_Router.UI.Desktop.Services
{
    public class DispatcherAccessor: IDispatcherAccessor, IDisposable
    {
        private Dispatcher _dispatcher;

        public DispatcherAccessor(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;

            _dispatcher.ShutdownStarted += DispatcherOnShutdownStarted;
        }

        private void DispatcherOnShutdownStarted(object sender, EventArgs e)
        {
            Dispose();
        }

        public void Invoke(Action action)
        {
            if (_dispatcher.CheckAccess())
            {
                // We are already on the dispatcher. Execute the code immediately
                action();
                return;
            }

            _dispatcher.Invoke(action);
        }

        public void Dispose()
        {
            _dispatcher.ShutdownStarted -= DispatcherOnShutdownStarted;
            _dispatcher = null;
        }
    }
}
