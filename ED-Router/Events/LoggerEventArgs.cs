using System;
using ED_Router.Services;

namespace ED_Router.Events
{
    public class LoggerEventArgs : EventArgs
    {
        public string Message { get; set; }
        public MessageColor Color { get; set; }
    }
}