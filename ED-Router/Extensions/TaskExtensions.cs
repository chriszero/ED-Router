using System;
using System.Threading.Tasks;
using ED_Router.Services;

namespace ED_Router.Extensions
{
    public static class TaskExtensions
    {
        public static async void FireAndForget(this Task task)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                EdRouter.Instance.VoiceAttackAccessor.LogMessage($"Error {e.Message}", MessageColor.Red);
            }
        }
    }
}
