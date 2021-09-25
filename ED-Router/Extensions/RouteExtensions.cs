using System.Linq;
using ED_Router.Model;
using libspanch;

namespace ED_Router.Extensions
{
    public static class RouteExtensions
    {
        public static FlightPlan ToFlightPlan(this NeutronPlotterRoute neutronPlotter)
        {
            return new FlightPlan
            {
                DestinationSystem = neutronPlotter.DestinationSystem,
                Distance = neutronPlotter.Distance,
                Efficiency = neutronPlotter.Efficiency,
                Name = $"{neutronPlotter.SourceSystem} to {neutronPlotter.DestinationSystem}",
                SourceSystem = neutronPlotter.SourceSystem,
                PlanType = PlanType.NeutronPlotterAPI,
                Range = neutronPlotter.Range,
                SpanchId = neutronPlotter.Id,
                TotalJumps = neutronPlotter.TotalJumps,
                SystemsInRoute = neutronPlotter.SystemJumps.Select(ToSystemModel).ToArray()
            };
        }

        private static StarSystem ToSystemModel(NeutronPlotterSystem neutronPlotterSystem, int index)
        {
            return new StarSystem
            {
                DistanceToStar = neutronPlotterSystem.DistanceJumped,
                DistanceLeft = neutronPlotterSystem.DistanceLeft,
                HasNeutronStar = neutronPlotterSystem.NeutronStar,
                Jumps = neutronPlotterSystem.Jumps,
                Name = neutronPlotterSystem.System,
                Id = index+1
            };
        }
    }
}