using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BurnoutFX.Shared;
using CitizenFX.Core;
using static BurnoutFX.Client.ClientMain;
using static BurnoutFX.Client.ClientUI;
using static CitizenFX.Core.Native.API;

namespace BurnoutFX.Client
{
    public class Race : BaseScript
    {
        public float CheckpointRadius { get; set; } = 16.0f;
        public bool Checkpoints { get; set; } = true;

        public float CheckpointTransparency { get; set; } = 1.0f;
        //public List<Player> Racers { get; set; }
        public Marker[] TrackData { get; set; }
        public int Prize { get; set; } = 1000;
        public int Bet { get; set; } = 0;

        public Race(float checkpointRadius, float checkpointTransparency, Marker[] trackData)
        {
            CheckpointRadius = checkpointRadius;
            CheckpointTransparency = checkpointTransparency;
            TrackData = trackData;
        }
        public async void JoinRace(int startTime)
        {
            bool closedRace = false;
            while (Game.GameTime < startTime)
            {
                string drawTime = (Math.Floor(startTime - Game.GameTime) / 1000).ToString("F3");
                DrawGameText(drawTime, 0.5f, 0.5f, 0, 238, 198, 78, 255);
                Game.DisableAllControlsThisFrame(0);
                Game.DisableControlThisFrame(0, Control.VehicleAccelerate);
                Game.DisableControlThisFrame(0, Control.VehicleBrake);
                Game.DisableControlThisFrame(0, Control.VehicleDuck);
                Game.DisableControlThisFrame(0, Control.VehicleRocketBoost);
                if(startTime - Game.GameTime < 5000 && closedRace == false)
                {
                    TriggerServerEvent("CloseGame");
                    closedRace = true;
                }
                await Delay(0);

            }
            CurrentBPlayer.State = PlayerState.InGame;
            startRacing(startTime);
        }
        private async void startRacing(int startTime)
        {
            TriggerServerEvent("StartedGame");
            int checkpointID = 0;

            Marker currentCheckpoint = TrackData[checkpointID], nextCheckpoint = TrackData[checkpointID + 1], previousCheckpoint = TrackData[checkpointID - 1];

            int checkpointHandle = CreateCheckpoint(currentCheckpoint.Type, currentCheckpoint.X, currentCheckpoint.Y, currentCheckpoint.Z,
                nextCheckpoint.X, nextCheckpoint.Y, nextCheckpoint.Z, CheckpointRadius, 255, 255, 0, 127, 0);
            SetCheckpointCylinderHeight(checkpointHandle, 1, 10, 10);
            int checkpointBlip = AddBlipForCoord(currentCheckpoint.X, currentCheckpoint.Y, currentCheckpoint.Z);
            SetNewWaypoint(currentCheckpoint.X, currentCheckpoint.Y);
            while (CurrentBPlayer.State == PlayerState.InGame)
            {  
                float drawTime = (float)Game.GameTime - (float)startTime;
                Vector3 queryVector = new Vector3(currentCheckpoint.X, currentCheckpoint.Y, currentCheckpoint.Z);
                float checkpointDistance = Game.PlayerPed.Position.DistanceToSquared(queryVector);
                DrawGameText((drawTime / 1000).ToString(), 0.1f, 0.025f, 0, 238, 198, 78, 255, 0.7f, 0.7f);
                string checkpointText = string.Format("Checkpoint {0} / {1} ({2:F3} m)", checkpointID, TrackData.Length, checkpointDistance / 1000);
                DrawGameText(checkpointText, 0.1f, 0.065f, 0, 238, 198, 78, 255);
                DrawGameText(checkpointID.ToString(), 0.5f, 0.5f);
                if (Game.PlayerPed.Position.DistanceToSquared(queryVector) < CheckpointRadius)
                {
                    DeleteCheckpoint(checkpointHandle);
                    RemoveBlip(ref checkpointBlip);
                    if (checkpointID +1 == TrackData.Length)
                    {
                        PlaySoundFrontend(-1, "ScreenFlash", "WastedSounds", true);
                        string vehicleName = Game.PlayerPed.CurrentVehicle.LocalizedName;
                        if (vehicleName.Contains("NULL"))
                            vehicleName = Game.PlayerPed.CurrentVehicle.DisplayName;
                        TriggerServerEvent("FinishedGame", Game.GameTime - startTime, GetLabelText(vehicleName));
                        CurrentBPlayer.State = PlayerState.None;
                    }
                    else
                    {
                        checkpointID++;
                        currentCheckpoint = TrackData[checkpointID];
                        nextCheckpoint = TrackData[checkpointID + 1];
                        PlaySoundFrontend(-1, "RACE_PLACED", "HUD_AWARDS", true);
                        checkpointHandle = CreateCheckpoint(currentCheckpoint.Type, currentCheckpoint.X, currentCheckpoint.Y, currentCheckpoint.Z,
                            nextCheckpoint.X, nextCheckpoint.Y, nextCheckpoint.Z, CheckpointRadius, 255, 255, 0, 127, 0);
                        SetCheckpointCylinderHeight(checkpointHandle, 1, 10, 10);
                        checkpointBlip = AddBlipForCoord(currentCheckpoint.X, currentCheckpoint.Y, currentCheckpoint.Z);
                        SetNewWaypoint(currentCheckpoint.X, currentCheckpoint.Y);
                    }
                }
                await Delay(0);
            }
        }
    }
}
