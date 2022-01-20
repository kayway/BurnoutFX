using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BurnoutFX.Shared;
using CitizenFX.Core;
using static BurnoutFX.Client.ClientMain;
using Newtonsoft.Json;
using static CitizenFX.Core.Native.API;
namespace BurnoutFX.Client
{
    class ClientManager : BaseScript
    {
        public static Dictionary<string, Marker> TrackMarkers = new Dictionary<string, Marker>();

        public static Dictionary<string, Marker> GameMarkers = new Dictionary<string, Marker>();
        
        // A List of all the Markers Blip IDs.
        private static List<int> blips = new List<int>();
        
        // Time before starting the game default is 10 seconds (10000ms)
        private int startDelay = 10000;
        
        //scaleform ID
        private static int scale = -1;
        
        // Current game the player is in.
        public static ActiveGame currentGame;
        
        public Race currentRace;
        
        public ClientManager()
        {
            TriggerServerEvent("RequestMarkerData");
            Tick += MarkerHandler;
        }
        /// <summary>
        /// Handles all of the track and game markers.
        /// </summary>
        private async Task MarkerHandler()
        {
            scale = RequestScaleformMovie("INSTRUCTIONAL_BUTTONS");
            while (!HasScaleformMovieLoaded(scale))
            {
                await Delay(0);
            }
            while (CurrentBPlayer.State == PlayerState.None)
            {
                foreach (KeyValuePair<string, Marker> kvp in GameMarkers)
                {
                    Marker marker = kvp.Value;
                    marker.drawMarker();
                    float markerDistance = Game.PlayerPed.Position.DistanceToSquared(new Vector3(marker.X, marker.Y, marker.Z));
                    if (markerDistance < 100.0f)
                    {
                        ClientMain.DrawGameText(kvp.Key, marker.X, marker.Y, 238, 198, 78, 255);
                        if (Game.IsControlPressed(0, Control.VehicleHorn))
                        {
                            JoinGame(kvp.Key);
                        }
                    }
                }
                foreach (KeyValuePair<string, Marker> kvp in TrackMarkers)
                {
                    Marker marker = kvp.Value;
                    marker.drawMarker();
                    float markerDistance = Game.PlayerPed.Position.DistanceToSquared(new Vector3(marker.X, marker.Y, marker.Z));
                    if (markerDistance < 100.0f)
                    {
                        ClientMain.DrawGameText(kvp.Key, marker.X, marker.Y, marker.Z + 0.45f);
                        
                        if (Game.IsControlPressed(0, Control.VehicleAccelerate) && Game.IsControlPressed(0, Control.VehicleBrake))
                        {
                            BeginScaleformMovieMethod(scale, "CLEAR_ALL");
                            EndScaleformMovieMethod();

                            BeginScaleformMovieMethod(scale, "SET_DATA_SLOT");
                            ScaleformMovieMethodAddParamInt(0);
                            PushScaleformMovieMethodParameterString("~INPUT_VEH_DUCK~");
                            PushScaleformMovieMethodParameterString($"Start Timetrial");
                            EndScaleformMovieMethod();

                            BeginScaleformMovieMethod(scale, "SET_DATA_SLOT");
                            ScaleformMovieMethodAddParamInt(1);
                            PushScaleformMovieMethodParameterString("~INPUT_VEH_CIN_CAM~");
                            PushScaleformMovieMethodParameterString($"Start Race");
                            EndScaleformMovieMethod();

                            BeginScaleformMovieMethod(scale, "DRAW_INSTRUCTIONAL_BUTTONS");
                            ScaleformMovieMethodAddParamInt(0);
                            EndScaleformMovieMethod();

                            Game.DisableControlThisFrame(0, Control.VehicleDuck);
                            Game.DisableControlThisFrame(0, Control.VehicleRocketBoost);
                            Game.DisableControlThisFrame(0, Control.VehicleDropProjectile);
                            Game.DisableControlThisFrame(0, Control.VehicleFlyDuck);
                            Game.DisableControlThisFrame(0, Control.ParachuteSmoke);
                            Game.DisableControlThisFrame(0, Control.VehicleHydraulicsControlToggle);
                            Game.DisableControlThisFrame(0, Control.VehicleBikeWings);
                            Game.DisableControlThisFrame(0, Control.VehicleFlyTransform);
                            Game.DisableControlThisFrame(0, Control.VehicleCinCam);
                            if (Game.IsDisabledControlJustPressed(0, Control.VehicleDuck)) // Controller: A Keyboard: X
                            {
                                currentGame = new ActiveGame(kvp.Key, GameMode.timetrial, Game.GameTime + startDelay);
                                CreateGame();
                            }
                            else if (Game.IsDisabledControlJustPressed(0, Control.VehicleCinCam)) // Controller: B Keyboard: R
                            {
                                currentGame = new ActiveGame(kvp.Key, GameMode.race, Game.GameTime + 15000);
                                currentGame.GameName = Game.Player.Name + "'s Race";
                                CreateGame(kvp.Value);
                            }
                        }
                    }
                }
                await Delay(0);
            }
            await Task.FromResult(0);
        }
        /// <summary>
        /// Processes requested marker data and converts them from JSON
        /// </summary>
        /// <param name="trackMarkerData"></param>
        /// <param name="gameMarkerData"></param>
        [EventHandler("RetreiveMarkerData")]
        private void ParseMarkerData(string trackMarkerData, string gameMarkerData)
        {
            Dictionary<string, Tuple<uint, uint, string>> trackMarkers = JsonConvert.DeserializeObject<Dictionary<string, Tuple<uint, uint, string>>>(trackMarkerData);
            foreach(KeyValuePair<string, Tuple<uint, uint, string>> kvp in trackMarkers)
            {
                string markerName = kvp.Key;
                int blipColour = (int) kvp.Value.Item1;
                int blipIcon = (int) kvp.Value.Item2;
                Marker marker = JsonConvert.DeserializeObject<Marker>(kvp.Value.Item3);
                int blipID = marker.AddBlip(markerName, blipColour, blipIcon);
                blips.Add(blipID);
                TrackMarkers.Add(markerName, marker);
            }
            Dictionary<string, string> gameMarkers = JsonConvert.DeserializeObject<Dictionary<string, string>>(gameMarkerData);
            foreach (KeyValuePair<string, string> kvp in gameMarkers)
            {
                Marker marker = JsonConvert.DeserializeObject<Marker>(kvp.Value);
                int blipID = marker.AddBlip(kvp.Key, 255, 255);
                blips.Add(blipID);
                GameMarkers.Add(kvp.Key, marker);
            }
        }
        /// <summary>
        /// Sends Game and marker data to the server.
        /// </summary>
        /// <param name="marker"></param>
        private void CreateGame(Marker marker = null)
        {
            string gameData = JsonConvert.SerializeObject(currentGame);
            string markerData = JsonConvert.SerializeObject(marker);
            Debug.WriteLine(markerData);
            TriggerServerEvent("CreateGame", gameData, markerData);
        }
        /// <summary>
        ///  requests server to join game.
        /// </summary>
        /// <param name="markerName"></param>
        private void JoinGame(string markerName)
        {
            TriggerServerEvent("JoinGame", markerName);
        }
        /// <summary>
        /// Retreives Game data from server and converts it from JSON.
        /// </summary>
        /// <param name="gameID"></param>
        /// <param name="checkpointRadius"></param>
        /// <param name="checkpointTransparency"></param>
        /// <param name="trackData"></param>
        [EventHandler("retreiveGameData")]
        private void ParseGameData(int gameID, float checkpointRadius, float checkpointTransparency, string trackData)
        {
            currentGame.GameID = gameID;
            Debug.WriteLine(trackData);
            Marker[] track = JsonConvert.DeserializeObject<Marker[]>(trackData);
            Debug.WriteLine(track.Length.ToString());
            currentRace = new Race(checkpointRadius, checkpointTransparency, track);

            currentRace.JoinRace(currentGame.StartTime);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="markerData"></param>
        [EventHandler("RetreiveNewGameMarker")]
        private void ParseGameMarker(string gameName, string markerData)
        {
            Debug.WriteLine("Marker: " + markerData + " Game: " + gameName);
            Marker newMarker = JsonConvert.DeserializeObject<Marker>(markerData);
            if(!GameMarkers.ContainsKey(gameName))
            {
                int blipID = newMarker.AddBlip(gameName, 255, 255);
                blips.Add(blipID);
                GameMarkers.Add(gameName, newMarker);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameName"></param>
        [EventHandler("RemoveGameMarker")]
        private void RemoveGameMarker(string gameName)
        {
            Marker marker;
            GameMarkers.TryGetValue(gameName, out marker);
            int blip = marker.BlipID;
            RemoveBlip(ref blip);
            blips.Remove(blip);
            GameMarkers.Remove(gameName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="winnerName"></param>
        [EventHandler("WonGame")]
        private async void WonGame(string winnerName)
        {
            int fadeTime = Game.GameTime + 10000;
            while(Game.GameTime < fadeTime)
            {
                DrawGameText(winnerName + "Won!!", 0.5f, 0.2f);
                await Delay(0);
            }
        }
    }
}
