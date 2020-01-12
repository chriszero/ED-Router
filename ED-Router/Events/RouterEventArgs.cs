using System;
using System.Collections.Generic;
using System.Linq;
using ED_Router.Extensions;
using libspanch;

namespace ED_Router.Events
{
    public abstract class RouterEventArgs : EventArgs
    {
        public bool EmitEvent { get; protected set; }
        public IList<VoiceAttackVariable> EventArgs { get; } = new List<VoiceAttackVariable>();

        public abstract string EventName { get; }

        protected RouterEventArgs(){}
    }

    public class VoiceAttackVariable
    {
        public Type Type { get; private set; }
        public string VariableName { get; private set; }

        public object VariableValue { get; private set; }

        private VoiceAttackVariable() { }

        public static VoiceAttackVariable Create<T>(string name, T value)
        {
            return new VoiceAttackVariable
            {
                Type = typeof(T),
                VariableName = name,
                VariableValue = value
            };
        }
    }

    public abstract class RouterEventArgs<T>: RouterEventArgs where T: RouterEventArgs
    {
        public sealed override string EventName => typeof(T).Name.ToLowerInvariant();
    }

    public class Calculate_Route : RouterEventArgs<Calculate_Route>
    {
        public static Calculate_Route Create(Route route)
        {
            var @event = new Calculate_Route();

            @event.EventArgs.Add(VoiceAttackVariable.Create("total_jumps", route.TotalJumps));
            @event.EventArgs.Add(VoiceAttackVariable.Create("total_waypoints", route.SystemJumps.Count));

            if (route.SystemJumps.Count > 0)
            {
                var currentWaypoint = route.SystemJumps.ElementAt(0);
                foreach (var variable in currentWaypoint.SystemJumpToVoiceAttackVariables())
                {
                    @event.EventArgs.Add(variable);
                }
            }
            
            return @event;
        }
    }

    public interface IWaypointEvent
    {
        bool CopyToClipboard { get; }
    }

    public class Next_Waypoint : RouterEventArgs<Next_Waypoint>, IWaypointEvent
    {
        public bool CopyToClipboard { get; private set; }
        
        public static Next_Waypoint Create(SystemJump currentWaypoint, bool copyToClipboard = false, bool emitEvent = false)
        {
            var @event = new Next_Waypoint()
            {
                CopyToClipboard = copyToClipboard,
                EmitEvent = emitEvent
            };
            foreach (var variable in currentWaypoint.SystemJumpToVoiceAttackVariables())
            {
                @event.EventArgs.Add(variable);
            }

            return @event;
        }
    }

    public class Previous_Waypoint : RouterEventArgs<Previous_Waypoint>, IWaypointEvent
    {
        public bool CopyToClipboard { get; private set; }
        
        public static Previous_Waypoint Create(SystemJump currentWaypoint, bool copyToClipboard = false, bool emitEvent = false)
        {
            var @event = new Previous_Waypoint()
            {
                CopyToClipboard = copyToClipboard,
                EmitEvent = emitEvent
            };

            foreach (var variable in currentWaypoint.SystemJumpToVoiceAttackVariables())
            {
                @event.EventArgs.Add(variable);
            }

            return @event;
        }
    }

    public class Final_Waypoint : RouterEventArgs<Final_Waypoint>
    {
        public static Final_Waypoint Create()
        {
            return new Final_Waypoint();
        }
    }
}
