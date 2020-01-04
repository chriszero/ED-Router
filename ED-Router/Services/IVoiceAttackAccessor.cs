namespace ED_Router.Services
{
    public interface IVoiceAttackAccessor
    {
        void SendEvent(string @event);
        void SetVariable<T>(string variableName, T content);
        void LogMessage(string message, MessageColor color = MessageColor.Blue);
    }
}
