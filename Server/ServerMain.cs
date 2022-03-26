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
            string licenseID = player.Identifiers["License"];
            BurnoutPlayer onlinePlayer = await DatabaseConnector.InitializePlayerData(licenseID, player.Name);
            if (!onlinePlayers.ContainsKey(licenseID))
            {
                await Delay(1000);
                onlinePlayers.Add(licenseID, onlinePlayer);
            }
            Debug.WriteLine(onlinePlayer.Dosh.ToString());
            TriggerClientEvent(player, "retrievePlayerData", onlinePlayer.Dosh, onlinePlayer.Rep, onlinePlayer.Infamy);
        }
    }
}