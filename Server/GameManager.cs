using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using CitizenFX.Core;
using BurnoutFX.Shared;

namespace BurnoutFX.Server
{
    public class GameManager : BaseScript
    {
        public static Dictionary<string, Tuple<uint, uint, string>> MarkerData = new Dictionary<string, Tuple<uint, uint, string>>();

        public static Dictionary<ActiveGame, List<Player>> activeGames = new Dictionary<ActiveGame, List<Player>>();
        
        public static Dictionary<string, string> availableGameMarkers = new Dictionary<string, string>();
       
        public GameManager()
        {
            GetGameMarkers();
        }
        /// <summary>
        /// Processes player's completed game
        /// </summary>
        /// <param name="player"></param>
        /// <param name="time"></param>
        /// <param name="vehicleName"></param>
        [EventHandler("CloseGame")]
        private static void CloseGame([FromSource] Player player)
        {
            foreach (KeyValuePair<ActiveGame, List<Player>> kvp in activeGames)
            {
                if (kvp.Value.Contains(player) && kvp.Key.Mode != GameMode.timetrial)
                {
                    TriggerClientEvent("RemoveGameMarker", -1, kvp.Key.GameName);
                    availableGameMarkers.Remove(kvp.Key.GameName);
                    break;
                }
            }
        }
        /// <summary>
        /// Finishes the game and processes the player results
        /// </summary>
        /// <param name="player"></param>
        /// <param name="time"></param>
        /// <param name="vehicleName"></param>
        [EventHandler("FinishedGame")]
        private static void FinishedGame([FromSource] Player player, int time, string vehicleName)
        {
            bool everyoneFinished = true;
            bool firstFinished = true;
            foreach (KeyValuePair<ActiveGame, List<Player>> kvp in activeGames)
            {
                Debug.WriteLine("HI");
                if (kvp.Value.Contains(player))
                { 
                    if (kvp.Key.Mode != GameMode.timetrial)
                    {
                        foreach(Player participant in kvp.Value)
                        {
                            string licenseID = participant.Identifiers["License"];
                            BurnoutPlayer burnoutPlayer;
                            if (ServerMain.onlinePlayers.TryGetValue(licenseID, out burnoutPlayer))
                            {
                                if (burnoutPlayer.State == PlayerState.InGame)
                                    everyoneFinished = false;
                                else if (burnoutPlayer.State == PlayerState.None && player != participant)
                                    firstFinished = false;
                            }
                        }
                        if (everyoneFinished)
                        {
                            activeGames.Remove(kvp.Key);
                            break;
                        }
                        else if (firstFinished)
                            TriggerClientEvent("WonGame", player.Name);
                        //Add Client announce event here
                    }
                    else
                    {
                        DatabaseConnector.SubmitTime(player.Identifiers["License"], time, kvp.Key.TrackName, vehicleName);
                        //Add some display function
                    }
                }
            }
        }
        /// <summary>
        /// Requests Track Data and creates a joinable game
        /// </summary>
        /// <param name="player"></param>
        /// <param name="gameData"></param>
        /// <param name="markerData"></param>
        [EventHandler("CreateGame")]
        public async void CreateGame([FromSource] Player player, string gameData, string markerData = "")
        {
            
            ActiveGame newGame = JsonConvert.DeserializeObject<ActiveGame>(gameData);
            List<Player> players = new List<Player>(1);
            players.Add(player);
            newGame.GameID = activeGames.Count + 1;
            activeGames.Add(newGame, players);
            var trackData = await DatabaseConnector.RetreiveTrackData(newGame.TrackName);
            TriggerClientEvent(player, "retreiveGameData", newGame.GameID, trackData.Item1, trackData.Item2, trackData.Item3);
            if (newGame.Mode != GameMode.timetrial)
            {
                availableGameMarkers.Add(newGame.GameName, markerData);
                Debug.WriteLine(markerData);
                TriggerClientEvent("RetreiveNewGameMarker", newGame.GameName, markerData);
            }
        }
        /// <summary>
        /// Adds the player to particapants in active game and sends game data
        /// </summary>
        /// <param name="player"></param>
        /// <param name="markerName"></param>
        [EventHandler("JoinGame")]
        public async void JoinGame([FromSource] Player player, string markerName)
        {
            string gameName = "";
            foreach (KeyValuePair<ActiveGame, List<Player>> kvp in activeGames)
            {
                if (kvp.Key.GameName == markerName)
                {
                    gameName = kvp.Key.GameName;
                    kvp.Value.Add(player);
                    var trackData = await DatabaseConnector.RetreiveTrackData(kvp.Key.TrackName);
                   TriggerClientEvent(player, "retreiveGameData", kvp.Key.GameID, trackData.Item1, trackData.Item2, trackData.Item3);
                    break;
                }
            }
        }
        private static async void GetGameMarkers()
        {
            MarkerData = await DatabaseConnector.RetreiveTracks();
            Debug.WriteLine(MarkerData.Count.ToString());
        }
        [EventHandler("RequestMarkerData")]
        private static void SendGameMarkers([FromSource]Player player)
        {
            string trackMarkerData = JsonConvert.SerializeObject(MarkerData);
            string gameMarkerData = JsonConvert.SerializeObject(availableGameMarkers);
            TriggerClientEvent(player, "RetreiveMarkerData", trackMarkerData, gameMarkerData);
        }
    }
}
