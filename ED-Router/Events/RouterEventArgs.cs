using System;
using System.Collections.Generic;
using System.Linq;
using ED_Router.Extensions;
using libspanch;

namespace ED_Router.Events
{
    public abstract class RouterEventArgs : EventArgs
    {
        public bool EmitEvent { get; protected set; } = true;
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
        public static Calculate_Route Create(NeutronPlotterRoute neutronPlotterRoute)
        {
            var @event = new Calculate_Route();

            @event.EventArgs.Add(VoiceAttackVariable.Create("total_jumps", neutronPlotterRoute.TotalJumps));

            if (neutronPlotterRoute.SystemJumps.Count > 0)
            {
                var currentWaypoint = neutronPlotterRoute.SystemJumps.ElementAt(0);
                foreach (var variable in currentWaypoint.SystemJumpToVoiceAttackVariables())
                {
                    @event.EventArgs.Add(variable);
                }
            }
            @event.EventArgs.Add(VoiceAttackVariable.Create("travel_percent", (decimal)0));
            @event.EventArgs.Add(VoiceAttackVariable.Create("jump_number", 1));
            @event.EventArgs.Add(VoiceAttackVariable.Create("spansh_uri", neutronPlotterRoute.Uri));

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
        
        public static Next_Waypoint Create(NeutronPlotterSystem currentWaypoint, int jumpIndex, double travelPercent,bool copyToClipboard = false, bool emitEvent = false)
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

            @event.EventArgs.Add(VoiceAttackVariable.Create("travel_percent", Convert.ToDecimal(travelPercent)));
            @event.EventArgs.Add(VoiceAttackVariable.Create("jump_number", jumpIndex+1));

            return @event;
        }
    }

    public class Previous_Waypoint : RouterEventArgs<Previous_Waypoint>, IWaypointEvent
    {
        public bool CopyToClipboard { get; private set; }
        
        public static Previous_Waypoint Create(NeutronPlotterSystem currentWaypoint, int jumpIndex, double travelPercent, bool copyToClipboard = false, bool emitEvent = false)
        {
            var @event = new Previous_Waypoint()
            {
                CopyToClipboard = copyToClipboard,
                EmitEvent = emitEvent
            };

            var variables = currentWaypoint.SystemJumpToVoiceAttackVariables()
                .Concat(new[]
                {
                    VoiceAttackVariable.Create("travel_percent", Convert.ToDecimal(travelPercent)),
                    VoiceAttackVariable.Create("jump_number", jumpIndex + 1)
                });

            foreach (var variable in variables)
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
