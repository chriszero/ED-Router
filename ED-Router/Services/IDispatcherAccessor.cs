using System;

namespace ED_Router.Services
{
    public interface IDispatcherAccessor
    {
        void Invoke(Action action);
    }
}
