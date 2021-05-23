using System.ComponentModel;

namespace ED_Router.Model
{
    public class System
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool HasNeutronStar { get; set; }
        public int Jumps { get; set; }
        
        public double? DistanceJumped { get; set; }
        public double? DistanceLeft { get; set; }
    }
}