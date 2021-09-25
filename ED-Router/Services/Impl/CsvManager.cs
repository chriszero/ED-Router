using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ED_Router.Model;

namespace ED_Router.Services.Impl
{
    public class CsvManager: ICsvManager
    {
        public async Task<FlightPlan> ImportCsv(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var csv = new FileInfo(filePath);
            if (!csv.Exists)
            {
                throw new InvalidOperationException("File must exist");
            }

            using(var streamReader = csv.OpenText())
            {
                var headers = await streamReader.ParseCsvHeader();

                return await streamReader.ParseFlightPlanFromCSV(headers);
            }
        }
    }

    static class CsvManagerExtensions
    {
        private static async Task<IEnumerable<string>> GetNextLine(this StreamReader reader)
        {
            var line = await reader.ReadLineAsync();
            return line
                .Split(',')
                .Select(x => x.Remove(x.Length - 1).Remove(0, 1));
        }


        public static async Task<IDictionary<int, PropertyInfo>> ParseCsvHeader(this StreamReader reader)
        {
            var line = await reader.GetNextLine();
            var type = typeof(StarSystem);
            return line
                .Select((column, index) =>
                {
                    switch (column)
                    {
                        case "System Name":
                            return new KeyValuePair<int, PropertyInfo>(index, type.GetProperty(nameof(StarSystem.Name)));
                        case "Distance To Arrival":
                        case "Distance":
                            return new KeyValuePair<int, PropertyInfo>(index, type.GetProperty(nameof(StarSystem.DistanceToStar)));
                        case "Distance Remaining":
                            return new KeyValuePair<int, PropertyInfo>(index, type.GetProperty(nameof(StarSystem.DistanceLeft)));
                        case "Fuel Left":
                            return new KeyValuePair<int, PropertyInfo>(index, type.GetProperty(nameof(StarSystem.FuelLeft)));
                        case "Fuel Used":
                            return new KeyValuePair<int, PropertyInfo>(index, type.GetProperty(nameof(StarSystem.FuelUsed)));
                        case "Neutron Star":
                            return new KeyValuePair<int, PropertyInfo>(index, type.GetProperty(nameof(StarSystem.HasNeutronStar)));
                        case "Refuel":
                            return new KeyValuePair<int, PropertyInfo>(index, type.GetProperty(nameof(StarSystem.Refuel)));
                        case "Jumps":
                            return new KeyValuePair<int, PropertyInfo>(index, type.GetProperty(nameof(StarSystem.Jumps)));
                        default:
                            return (KeyValuePair<int, PropertyInfo>?)null;
                    }
                })
                .Where(x => x.HasValue)
                .ToDictionary(x => x.Value.Key, x => x.Value.Value);
        }


        public static async Task<FlightPlan> ParseFlightPlanFromCSV(this StreamReader reader, IDictionary<int, PropertyInfo> headers)
        {
            var isGalaxyPlotter = headers.Values.Any(x => x.Name == nameof(StarSystem.Refuel));
            var flightPlan = new FlightPlan
            {
                PlanType = isGalaxyPlotter
                    ? PlanType.GalaxyPlotterCSV
                    : PlanType.NeutronPlotterCSV
            };

            var systems = new List<StarSystem>();

            while (!reader.EndOfStream)
            {
                var line = await reader.GetNextLine();
                var star = new StarSystem
                {
                    Id = systems.Count + 1
                };

                foreach (var (value, index) in line.Select((col, index) => (col, index)))
                {
                    var property = headers[index];
                    if (property.PropertyType == typeof(string))
                    {
                        property.SetValue(star, value);
                        continue;
                    }
                    if (property.PropertyType == typeof(double) || property.PropertyType == typeof(double?))
                    {
                        property.SetValue(star, Convert.ToDouble(value));
                        continue;
                    }
                    if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                    {
                        property.SetValue(star, Convert.ToInt32(value));
                        continue;
                    }
                    if (property.PropertyType == typeof(long) || property.PropertyType == typeof(long?))
                    {
                        property.SetValue(star, Convert.ToInt64(value));
                        continue;
                    }
                    if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                    {
                        if (!bool.TryParse(value, out var boolValue))
                        {
                            boolValue = value.Equals("yes", StringComparison.InvariantCultureIgnoreCase) || value.Equals("1");
                        }
                        
                        property.SetValue(star, boolValue);
                    }
                }

                systems.Add(star);
            }

            var firstSystem = systems.FirstOrDefault();
            var lastSystem = systems.LastOrDefault();
            var distance = systems.Count;

            flightPlan.Distance = firstSystem?.DistanceLeft ?? 0;
            flightPlan.SourceSystem = firstSystem?.Name;
            flightPlan.DestinationSystem = lastSystem?.Name;
            flightPlan.TotalJumps = distance;

            flightPlan.SystemsInRoute = systems.ToArray();

            return flightPlan;
        }
    }


}
