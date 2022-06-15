namespace BurnoutFX.Client
{
    public enum BoostTypes
    {
        speed, //Boost Chains: Use boost until it runs out to refill it or fill it by stunts.
        stunt, //Increased Boost from jumps and drifting
        aggression, //Increased boost from 
        locked,
        Switch
    }
    public class BurnoutVehicle
    {
        
        public BoostTypes BoostType { get; set; } = BoostTypes.stunt;
    }
}