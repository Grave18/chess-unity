using Logic;

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

        public static PieceColor GetTurnColorFromFenSplit(FenSplit fenSplit)
        {
            PieceColor turnColor = fenSplit.TurnColor switch
            {
                "w" => PieceColor.White,
                "b" => PieceColor.Black,
                _ => PieceColor.None
            };

            return turnColor;
        }

        public static int GetHalfMoveClock(string fen)
        {
            string[] splitPreset = fen.Split(' ');
            return int.TryParse(splitPreset[4], out int halfMoveClock) ? halfMoveClock : 0;
        }

        public static int GetFullMoveCounter(string fen)
        {
            string[] splitPreset = fen.Split(' ');
            return int.TryParse(splitPreset[5], out int fullMoveCounter) ? fullMoveCounter : 1;
        }
    }
}