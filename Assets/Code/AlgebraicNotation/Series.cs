namespace AlgebraicNotation
{
    [System.Serializable]
    public class Series
    {
        public int Count { get; set; }
        public string WhiteMove { get; set; }
        public string BlackMove { get; set; }

        public override string ToString()
        {
            string series = $"{Count}.<pos=15%>{WhiteMove}</pos><pos=60%>{BlackMove}</pos>";
            return series;
        }
    }
}