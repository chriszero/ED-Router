using System;
using System.Collections.Generic;

namespace libspanch
{
    public class NeutronPlotterRoute
    {
        public List<NeutronPlotterSystem> SystemJumps { get; set; }
        public string DestinationSystem { get; set; }
        public double Distance { get; set; }
        public int Efficiency { get; set; }
        public double Range { get; set; }
        public string SourceSystem { get; set; }
        public int TotalJumps { get; set; }
        public string Id { get; set; }
    }
}