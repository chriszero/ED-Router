using System;
using System.Text;
using ED_Router;

namespace ConsolTestApp
{
	class Program
	{
		static void Main(string[] args)
		{
			/*
            var Api = new SpanchApi();
            var result = Api.PlotRoute("Kashyapa", "Sol", 48);

             StringBuilder route = new StringBuilder();

             foreach (var system in result.SystemJumps)
             {
                 route.AppendLine(system.System);
             }

             Console.Write(route);*/

			/*
            var router = EdRouter.Instance;
            router.Start = "Sol";
            router.Destination = "Ao Shun";
            router.Range = 48;
            router.Efficiency = 60;

            router.CalculateRoute();
             
            Console.WriteLine("Start Sys: {0}", router.CurrentWaypoint);

            router.NextWaypoint();
            Console.WriteLine("Sys 1: {0}", router.CurrentWaypoint);

            router.NextWaypoint();
            Console.WriteLine("Sys 2: {0}", router.CurrentWaypoint);

            router.NextWaypoint();
            Console.WriteLine("Sys 3: {0}", router.CurrentWaypoint);

            router.PreviousWaypoint();
            Console.WriteLine("Sys 2: {0}", router.CurrentWaypoint);
            Console.ReadLine();
			*/

			//var l1 = JournalMonitor.GetLocaction("{ \"timestamp\":\"2018 - 03 - 17T20: 05:59Z\", \"event\":\"StartJump\", \"JumpType\":\"Hyperspace\", \"StarSystem\":\"Kyloarph IN-S d4-2021\", \"SystemAddress\":69449931411883, \"StarClass\":\"N\" }");
			//var l2 = JournalMonitor.GetLocaction("{ \"timestamp\":\"2018-03-17T20:03:25Z\", \"event\":\"Location\", \"Docked\":false, \"StarSystem\":\"Kyloarph DB-X e1-5559\", \"SystemAddress\":23876836747476, \"StarPos\":[-7504.34375,-727.28125,21081.59375], \"SystemAllegiance\":\"\", \"SystemEconomy\":\"$economy_None;\", \"SystemEconomy_Localised\":\"n/v\", \"SystemGovernment\":\"$government_None;\", \"SystemGovernment_Localised\":\"n/v\", \"SystemSecurity\":\"$GAlAXY_MAP_INFO_state_anarchy;\", \"SystemSecurity_Localised\":\"Anarchie\", \"Population\":0, \"Body\":\"Kyloarph DB-X e1-5559 1\", \"BodyID\":1, \"BodyType\":\"Planet\" }");
			var l3 = JournalMonitor.GetLocaction("{ \"timestamp\":\"2018-03-17T20:09:52Z\", \"event\":\"Scan\", \"ScanType\":\"Detailed\", \"BodyName\":\"Kyloarph PT-Q d5-3862 A 1\", \"BodyID\":7, \"Parents\":[ {\"Star\":1}, {\"Null\":0} ], \"DistanceFromArrivalLS\":239.561920, \"TidalLock\":false, \"TerraformState\":\"\", \"PlanetClass\":\"High metal content body\", \"Atmosphere\":\"thin sulfur dioxide atmosphere\", \"AtmosphereType\":\"SulphurDioxide\", \"AtmosphereComposition\":[ { \"Name\":\"SulphurDioxide\", \"Percent\":100.000000 } ], \"Volcanism\":\"\", \"MassEM\":0.134808, \"Radius\":3276403.500000, \"SurfaceGravity\":5.005291, \"SurfaceTemperature\":378.430450, \"SurfacePressure\":359.917908, \"Landable\":false, \"Composition\":{ \"Ice\":0.000000, \"Rock\":0.672977, \"Metal\":0.327023 }, \"SemiMajorAxis\":71812833280.000000, \"Eccentricity\":0.000085, \"OrbitalInclination\":-0.000587, \"Periapsis\":246.785690, \"OrbitalPeriod\":10816627.000000, \"RotationPeriod\":208069.937500, \"AxialTilt\":-0.499019 }");


		}
	}
}
