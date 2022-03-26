using BurnoutFX.Shared;
using CitizenFX.Core;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;
using CitizenFX.Core.Native;
using System;

namespace BurnoutFX.Client
{
    public class ClientMain : BaseScript
    {
        public static BurnoutPlayer CurrentBPlayer;

        public static int Boost = 0;
        private int currentBoost = 0;
        public ClientMain()
        {
            CurrentBPlayer = new BurnoutPlayer();
            TriggerServerEvent("requestPlayerData");
        }

        [EventHandler("retrievePlayerData")]
        private async void retrievePlayerData(uint dosh, uint rep, uint infamy)
        {
            CurrentBPlayer.Dosh = dosh;
            CurrentBPlayer.Rep = rep;
            CurrentBPlayer.Infamy = infamy;
            StatSetInt((uint)GetHashKey("MP0_WALLET_BALANCE"), (int)dosh, true);
            N_0x170f541e1cadd1de(true);
            SetMultiplayerWalletCash();
            N_0x170f541e1cadd1de(false);
            await Delay(5000);
            RemoveMultiplayerWalletCash();
            //SendNuiMessage(JsonConvert.SerializeObject(new{type = "open"}));
            Debug.WriteLine(JsonConvert.SerializeObject(new { type = "boost" }));
            Tick += boostHandler;
        }
        private Task boostHandler()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                if (Game.IsControlPressed(0, Control.VehicleRocketBoost) && Boost > 0)
                {
                    Boost--;
                }
            }
            if(currentBoost != Boost)
            {
                ClientUI.UpdateBoostHUD(Boost);
                currentBoost = Boost;
            }
            
            return Task.FromResult(0);
        }
    }
}