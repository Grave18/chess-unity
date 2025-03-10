namespace AlgebraicNotation
{
    [System.Serializable]
    public class Series
    {
        public int Count;
        public string WhiteMove;
        public string BlackMove;

        public override string ToString()
        {
            return $"{Count}. {WhiteMove} {BlackMove}";
        }
    }
}
