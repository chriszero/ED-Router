using ED_Router.Events;
using ED_Router.Services;

namespace ED_Router.UI.Desktop.Services
{
    public class DummyVoiceAttackAccessor: IVoiceAttackAccessor
    {
        public void LogMessage(string message, MessageColor color = MessageColor.Blue)
        {
        }

        public void SendEvent(RouterEventArgs @event)
        {
            
        }

        public void SetVariable<T>(string variableName, T content)
        {
        }
    }
}
