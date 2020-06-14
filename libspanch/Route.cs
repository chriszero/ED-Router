using System;
using System.Collections.Generic;

namespace libspanch
{
    public class Route
    {
        public List<SystemJump> SystemJumps { get; set; }
        public string DestinationSystem { get; set; }
        public double Distance { get; set; }
        public int Efficiency { get; set; }
        public double Range { get; set; }
        public string SourceSystem { get; set; }
        public int TotalJumps { get; set; }
        public string Id { get; set; }
        
        public string Uri
        {
            get
            {
                if (string.IsNullOrEmpty(SourceSystem) || string.IsNullOrEmpty(DestinationSystem))
                {
                    return null;
                }

                return $"https://spansh.co.uk/plotter/results/{Id}?efficiency={Efficiency}&from={SourceSystem}&range={Range}&to={DestinationSystem}";
            }
        }
    }
}