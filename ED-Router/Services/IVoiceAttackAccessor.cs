using System;
using ED_Router.Events;

namespace ED_Router.Services
{
    public interface IVoiceAttackAccessor
    {
        void SendEvent(RouterEventArgs @event);
        void SetVariable<T>(string variableName, T content);
        void LogMessage(string message, MessageColor color = MessageColor.Blue);
    }
}
