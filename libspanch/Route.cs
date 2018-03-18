using System.Collections.Generic;

namespace libspanch
{
    public class Route
    {
        public List<SystemJump> SystemJumps { get; set; }
        public string DestinationSystem { get; set; }
        public double distance { get; set; }
        public int Efficiency { get; set; }
        public double Range { get; set; }
        public string SourceSystem { get; set; }
        public int TotalJumps { get; set; }
    }
}