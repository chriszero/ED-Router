using System;
using System.Collections.Generic;
using ED_Router.Events;
using ED_Router.Model;

namespace ED_Router.Extensions
{
    public static class SystemExtensions
    {
        public static IEnumerable<VoiceAttackVariable> SystemJumpToVoiceAttackVariables(this StarSystem starSystem)
        {
            if (starSystem == null)
            {
                yield break;
            }

            yield return VoiceAttackVariable.Create("current_waypoint", starSystem.Name);
            yield return VoiceAttackVariable.Create("distance_left", Convert.ToDecimal(starSystem.DistanceLeft));
            yield return VoiceAttackVariable.Create("distance_jumped", Convert.ToDecimal(starSystem.DistanceToStar));
            if (starSystem.Jumps.HasValue)
            {
                yield return VoiceAttackVariable.Create("nb_jumps", starSystem.Jumps);
            }
            yield return VoiceAttackVariable.Create("has_neutron", starSystem.HasNeutronStar);
            if (starSystem.Refuel.HasValue)
            {
                yield return VoiceAttackVariable.Create("refuel", starSystem.Refuel);
            }
        }
    }
}