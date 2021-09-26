﻿namespace ED_Router.Model
{
    public class FlightPlan
    {
        public string SpanchId { get; set; }
        public string Name { get; set; }

        public PlanType PlanType { get; set; }

        public StarSystem[] SystemsInRoute { get; set; }

        public string DestinationSystem { get; set; }
        public double Distance { get; set; }
        public int Efficiency { get; set; }
        public double Range { get; set; }
        public string SourceSystem { get; set; }
        public int TotalJumps { get; set; }

        public string Uri
        {
            get
            {
                if (PlanType < PlanType.NeutronPlotterAPI || PlanType > PlanType.GalaxyPlotterAPI)
                {
                    return null;
                }

                if (string.IsNullOrEmpty(SourceSystem) || string.IsNullOrEmpty(DestinationSystem))
                {
                    return null;
                }

                return $"https://spansh.co.uk/plotter/results/{SpanchId}?efficiency={Efficiency}&from={SourceSystem}&range={Range}&to={DestinationSystem}";
            }
        }

        public bool RefuelDataAvailable => PlanType == PlanType.GalaxyPlotterAPI || PlanType == PlanType.GalaxyPlotterCSV;
    }
}