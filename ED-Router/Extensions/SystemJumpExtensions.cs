using System;
using System.Collections.Generic;
using ED_Router.Events;
using System = ED_Router.Model.System;

namespace ED_Router.Extensions
{
    public static class SystemExtensions
    {
        public static IEnumerable<VoiceAttackVariable> SystemJumpToVoiceAttackVariables(this Model.System system)
        {
            if (system == null)
            {
                yield break;
            }

            yield return VoiceAttackVariable.Create("current_waypoint", system.Name);
            yield return VoiceAttackVariable.Create("distance_left", Convert.ToDecimal(system.DistanceLeft));
            yield return VoiceAttackVariable.Create("distance_jumped", Convert.ToDecimal(system.DistanceJumped));
            yield return VoiceAttackVariable.Create("nb_jumps", system.Jumps);
        }
    }
}