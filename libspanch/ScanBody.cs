namespace libspanch
{
    public class ScanBody
    {
        public int DistanceToArrival { get; set; }
        public int EstimatedScanValue { get; set; }
        public string Id { get; set; }
        public bool IsTerraformable { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}