using BurnoutFX.Shared;
using CitizenFX.Core;
using System;
using static BurnoutFX.Client.ClientUI;
using static BurnoutFX.Client.StuntCounter;

namespace BurnoutFX.Client
{
    public class ClientMain : BaseScript
    {
        public static BurnoutPlayer CurrentBPlayer;
        public static BurnoutVehicle CurrentBPVehicle;

        public ClientMain()
        {
            CurrentBPlayer = new BurnoutPlayer();
            CurrentBPVehicle = new BurnoutVehicle();
            TriggerServerEvent("requestPlayerData");
            TriggerServerEvent("RequestMarkerData");
        }

        [EventHandler("retrievePlayerData")]
        private void retrievePlayerData(uint dosh, uint rep, uint infamy)
        {
            CurrentBPlayer.Dosh = dosh;
            CurrentBPlayer.Rep = rep;
            CurrentBPlayer.Infamy = infamy;
            SetDosh((int)dosh);
        }
    }
}