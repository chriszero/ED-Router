using System.ComponentModel;

namespace ED_Router.Model
{
    public class StarSystem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool HasNeutronStar { get; set; }
        public int? Jumps { get; set; }
        
        public double? FuelLeft{ get; set; }
        public double? FuelUsed { get; set; }
        public bool? Refuel { get; set; }

        public double? DistanceToStar { get; set; }
        public double? DistanceLeft { get; set; }
    }
}