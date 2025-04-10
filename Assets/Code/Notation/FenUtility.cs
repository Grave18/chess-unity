namespace Notation
{
    public static class FenUtility
    {
        /// Fen string must be valid at this point
        public static FenSplit GetFenSplit(string fen)
        {
            string[] splitPreset = fen.Split(' ');

            var parsedPreset = new FenSplit
            {
                PiecesPreset = splitPreset[0],
                TurnColor = splitPreset[1],
                Castling = splitPreset[2],
                EnPassant = splitPreset[3],
                HalfMove = splitPreset[4],
                FullMove = splitPreset[5]
            };

            return parsedPreset;
        }
    }
}