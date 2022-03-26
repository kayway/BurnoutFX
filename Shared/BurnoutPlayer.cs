using System;
using System.Collections.Generic;
using System.Text;
#if CLIENT
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
#endif
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
        public uint Dosh { get; set; } = 0;

        public uint Rep { get; set; } = 0;

        public uint Infamy { get; set; } = 0;

        //Player's state ingame.
        public PlayerState State { get; set; } = PlayerState.None;
        
        public BurnoutPlayer()
        {
        }
    }
}
