namespace BurnoutFX.Client
{
    public class Track
    {
        public bool showWaypoints { get; set; } = true;

        public float checkpointRadius { get; set; } = 24.0f;
        
        public float checkpointTransparency { get; set; } = 1.0f;
        
        public int mapBlipID { get; set; } = 315;
        
        public int mapBlipColour { get; set; } = 5;
        
        public Marker[] checkpoints { get; set; }
        
        public Track(Marker[] trackCheckpoints)
        {
            checkpoints = trackCheckpoints;
        }
    }
}