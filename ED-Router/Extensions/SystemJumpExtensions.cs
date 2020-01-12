using System.Collections.Generic;
using ED_Router.Events;
using libspanch;

namespace ED_Router.Extensions
{
    public static class SystemJumpExtensions
    {
        public static IEnumerable<VoiceAttackVariable> SystemJumpToVoiceAttackVariables(this SystemJump systemJump)
        {
            if (systemJump == null)
            {
                yield break;
            }

            yield return VoiceAttackVariable.Create("current_waypoint", systemJump.System);
            yield return VoiceAttackVariable.Create("distance_left", systemJump.DistanceLeft);
            yield return VoiceAttackVariable.Create("distance_jumped", systemJump.DistanceJumped);
            yield return VoiceAttackVariable.Create("nb_jumps", systemJump.Jumps);
        }
    }
}