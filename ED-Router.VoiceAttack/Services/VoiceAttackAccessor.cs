using System;
using ED_Router.Services;
using ED_Router.VoiceAttack.Extensions;

namespace ED_Router.VoiceAttack.Services
{
    public class VoiceAttackAccessor: IVoiceAttackAccessor, IDisposable
    {
        private dynamic _vaProxy;
        private static readonly object _vaProxyLock = new object();
        
        public VoiceAttackAccessor(dynamic vaProxy)
        {
            _vaProxy = vaProxy;
        }

        public void LogMessage(string message, MessageColor color = MessageColor.Blue)
        {
            if (_vaProxy == null) return;
            
            lock (_vaProxyLock)
            {
                _vaProxy.WriteToLog(message, color.ToLogColor());
            }
        }

        public void SendEvent(string @eventName)
        {
            if (_vaProxy == null) return;
            var vaCommandName = $"((EDRouter {@eventName.ToLowerInvariant()}))";
            lock (_vaProxyLock)
            {
                if (_vaProxy.CommandExists(vaCommandName))
                {
                    _vaProxy.ExecuteCommand(vaCommandName);
                }
            }
        }

        public void SetVariable<T>(string variableName, T content)
        {
            if (_vaProxy == null) return;
            // To be implemented if needed.
        }

        public void Dispose()
        {
            _vaProxy = null;
        }
    }
}
