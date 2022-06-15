namespace BurnoutFX.Shared
{
    public enum GameMode
    {
        race,
        timetrial,
        demolition
    }
    
    public class ActiveGame
    {
        public int GameID { get; set; }
        
        public string TrackName { get; set; }
        
        // Usually name of the player and track unless its a timetrail then its the same as TrackName
        public string GameName { get; set; }
        
        // Time to atart the game, by default is 10 seconds (10000ms).
        public int StartTime { get; set; } = 10000;

        public GameMode Mode { get; set; }
        
        public ActiveGame(string name, GameMode mode, int startTime)
        {
            TrackName = name;
            Mode = mode;
            StartTime = startTime;
        }
    }
}
