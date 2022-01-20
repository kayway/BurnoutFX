using System;
using System.Collections.Generic;
using CitizenFX.Core;
using BurnoutFX.Shared;

namespace BurnoutFX.Server
{
    public class ServerMain : BaseScript
    {
        public static Dictionary<string, BurnoutPlayer> onlinePlayers = new Dictionary<string, BurnoutPlayer>();

        /*[EventHandler("playerConnecting")]
        private async void onPlayerConnected([FromSource] Player player, string playerName, dynamic deferrals)
        {
            deferrals.defer();
            await Delay(0);
            string licenseID = player.Identifiers["License"];
            onlinePlayers.Add(licenseID, await DatabaseConnector.initializePlayerData(licenseID, playerName));
            deferrals.done();
        }*/
        [EventHandler("requestPlayerData")]
        private async void sendPlayerData([FromSource] Player player)
        {
            Debug.WriteLine("hello");
            string licenseID = player.Identifiers["License"];
            if (!onlinePlayers.ContainsKey(licenseID))
            {
                onlinePlayers.Add(licenseID, await DatabaseConnector.InitializePlayerData(licenseID, player.Name));
            }
            BurnoutPlayer onlinePlayer = onlinePlayers[licenseID];
            TriggerClientEvent(player, "retrievePlayerData", onlinePlayer.Dosh, onlinePlayer.Rep, onlinePlayer.Infamy);
        }
    }
}