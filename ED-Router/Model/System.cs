namespace ED_Router.Model
{
    public class System
    {
        public string Name { get; set; }
        public bool HasNeutronStar { get; set; }
        public int Jumps { get; set; }
        public int Rank { get; set; }

        public double? DistanceJumped { get; set; }
        public double? DistanceLeft { get; set; }
    }
}