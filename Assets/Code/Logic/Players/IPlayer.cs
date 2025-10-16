namespace Logic.Players
{
    public interface IPlayer
    {
        void StartPlayer();
        void UpdatePlayer();
        void StopPlayer();

        void Move(string uci);
    }
}