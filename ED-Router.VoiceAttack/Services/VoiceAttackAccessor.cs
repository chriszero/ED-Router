using System;
using ED_Router.Events;
using ED_Router.Services;

namespace ED_Router.VoiceAttack.Services
{
    public class VoiceAttackAccessor: IVoiceAttackAccessor, IDisposable
    {
        public EventHandler<RouterEventArgs> EventSent;
        public EventHandler<LoggerEventArgs> LogSent;
        public EventHandler<SetVariableEventArgs> VariableSent;

        public void LogMessage(string message, MessageColor color = MessageColor.Blue)
        {
            LogSent?.Invoke(this, new LoggerEventArgs(){Message = message, Color = color});
        }

        public void SendEvent(RouterEventArgs @event)
        {
            EventSent?.Invoke(this, @event);
        }

        public void SetVariable<T>(string variableName, T content)
        {
            var @event = new SetVariableEventArgs()
            {
                Value = content,
                ValueType = typeof(T),
                VariableName = variableName
            };
            VariableSent?.Invoke(this, @event);
        }

        public void Dispose()
        {
            EventSent = null;
            LogSent = null;
            VariableSent = null;
        }
    }
}
