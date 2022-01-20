using System;
using System.Collections.Generic;
using System.Text;

namespace BurnoutFX.Shared
{
    public enum PlayerState
    {
        None,
        Joined,
        InGame
    }
    public class BurnoutPlayer
    {
        //Money.
        public uint Dosh { get; set; }

        //Reputation.
        public uint Rep { get; set; }
        
        public uint Infamy { get; set; }
        
        //Player's state ingame.
        public PlayerState State { get; set; }
        
        public BurnoutPlayer()
        {
            State = PlayerState.None;
        }
    }
}
