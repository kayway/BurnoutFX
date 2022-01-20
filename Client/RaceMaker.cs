using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
namespace BurnoutFX.Client
{
    public class RaceMaker : BaseScript
    {
        private static bool RacemakerActive { get; set; } = false;
        private static int Scale { get; set; } = -1;

        private SortedDictionary<int, Vector3> recordedCheckpoints;
        private int raceStartDelay = 0;
        private int bet = 0;
        private int blip = 0;
        private bool cleared;
        public RaceMaker()
        {
            Tick += RaceMakerHandler;
        }
        internal static void SetRacemakerActive(bool active)
        {
            RacemakerActive = active;
        }
        internal static bool IsRacemakerActive()
        {
            return RacemakerActive;
        }
        private async Task RaceMakerHandler()
        {
            if (RacemakerActive)
            {
                raceStartDelay = 10;
                bet = 0;
                blip = 0;
                cleared = true;
                recordedCheckpoints = new SortedDictionary<int, Vector3>();
                Scale = RequestScaleformMovie("INSTRUCTIONAL_BUTTONS");
                while (!HasScaleformMovieLoaded(Scale))
                {
                    await Delay(0);
                }
            }
            while (RacemakerActive)
            {
                if (!IsHudHidden())
                {
                    BeginScaleformMovieMethod(Scale, "CLEAR_ALL");
                    EndScaleformMovieMethod();

                    BeginScaleformMovieMethod(Scale, "SET_DATA_SLOT");
                    ScaleformMovieMethodAddParamInt(0);
                    PushScaleformMovieMethodParameterString("~INPUT_SPRINT~");
                    PushScaleformMovieMethodParameterString($"Start Race");
                    EndScaleformMovieMethod();

                    BeginScaleformMovieMethod(Scale, "SET_DATA_SLOT");
                    ScaleformMovieMethodAddParamInt(1);
                    PushScaleformMovieMethodParameterString("~INPUT_DUCK~");
                    PushScaleformMovieMethodParameterString($"Delete All Checkpoints");
                    EndScaleformMovieMethod();

                    BeginScaleformMovieMethod(Scale, "SET_DATA_SLOT");
                    ScaleformMovieMethodAddParamInt(2);
                    PushScaleformMovieMethodParameterString("~INPUT_PHONE~/~INPUT_PHONE_DOWN~");
                    PushScaleformMovieMethodParameterString($"Decrease/Increase Bet: ({bet})");
                    EndScaleformMovieMethod();

                    BeginScaleformMovieMethod(Scale, "SET_DATA_SLOT");
                    ScaleformMovieMethodAddParamInt(3);
                    PushScaleformMovieMethodParameterString("~INPUT_WEAPON_WHEEL_PREV~/~INPUT_WEAPON_WHEEL_NEXT~");
                    PushScaleformMovieMethodParameterString($"Decrease/Increase Start Delay: ({raceStartDelay})");
                    EndScaleformMovieMethod();

                    BeginScaleformMovieMethod(Scale, "SET_DATA_SLOT");
                    ScaleformMovieMethodAddParamInt(4);
                    PushScaleformMovieMethodParameterString("~INPUT_COVER~");
                    PushScaleformMovieMethodParameterString($"Delete Last Checkpoint");
                    EndScaleformMovieMethod();

                    BeginScaleformMovieMethod(Scale, "SET_DATA_SLOT");
                    ScaleformMovieMethodAddParamInt(5);
                    PushScaleformMovieMethodParameterString("~INPUT_MELEE_ATTACK_LIGHT~");
                    PushScaleformMovieMethodParameterString($"Place Checkpoint Here");
                    EndScaleformMovieMethod();

                    BeginScaleformMovieMethod(Scale, "SET_DATA_SLOT");
                    ScaleformMovieMethodAddParamInt(6);
                    PushScaleformMovieMethodParameterString(GetControlInstructionalButton(0, 170, 1));
                    PushScaleformMovieMethodParameterString($"Toggle RaceMaker");
                    EndScaleformMovieMethod();

                    BeginScaleformMovieMethod(Scale, "DRAW_INSTRUCTIONAL_BUTTONS");
                    ScaleformMovieMethodAddParamInt(0);
                    EndScaleformMovieMethod();

                    DrawScaleformMovieFullscreen(Scale, 255, 255, 255, 255, 0);
                }
                Game.DisableControlThisFrame(0, Control.Duck);
                Game.DisableControlThisFrame(0, Control.VehicleHorn);
                Game.DisableControlThisFrame(0, Control.Cover);
                Game.DisableControlThisFrame(0, Control.Jump);
                Game.DisableControlThisFrame(0, Control.Sprint);
                Game.DisableControlThisFrame(0, Control.WeaponWheelNext);
                Game.DisableControlThisFrame(0, Control.WeaponWheelPrev);
                Game.DisableControlThisFrame(0, Control.Phone);
                Game.DisableControlThisFrame(0, Control.PhoneDown);
                Game.DisableControlThisFrame(0, Control.MultiplayerInfo);
                Game.DisableControlThisFrame(0, Control.MeleeAttackLight);
                if (Game.PlayerPed.IsInVehicle())
                {
                    Game.DisableControlThisFrame(0, Control.VehicleRadioWheel);
                    Game.DisableControlThisFrame(0, Control.VehicleCinCam);
                }
                // Minimap Checkpoint Functions
                if (IsWaypointActive())
                {
                    Vector3 waypointPos = GetBlipInfoIdCoord(GetFirstBlipInfoId(8));
                    Vector3 pos = new Vector3();
                    bool deleted = false;
                    if (recordedCheckpoints != null)
                    {
                        for (int i = 0; i < recordedCheckpoints.Count; i++)
                        {
                            pos = recordedCheckpoints.ElementAt(i).Value;
                            blip = recordedCheckpoints.ElementAt(i).Key;
                            if (pos.DistanceToSquared2D(waypointPos) < 1)
                            {
                                deleted = true;
                                RemoveBlip(ref blip);
                                recordedCheckpoints.Remove(recordedCheckpoints.ElementAt(i).Key);
                                pos = new Vector3();
                                for (int j = i; j < recordedCheckpoints.Count; j++)
                                {
                                    blip = recordedCheckpoints.ElementAt(j).Key;
                                    ShowNumberOnBlip(blip, j);
                                }
                                SetWaypointOff();
                                break;
                            }
                        }
                    }
                    if (!deleted && GetClosestVehicleNode(waypointPos.X, waypointPos.Y, waypointPos.Z, ref pos, 1, 3.0f, 0.0f))
                    {
                        SetWaypointOff();
                        if (pos.DistanceToSquared2D(waypointPos) < 500)
                        {
                            blip = AddBlipForCoord(pos.X, pos.Y, pos.Z);
                            recordedCheckpoints.Add(blip, pos);
                        }
                        else
                        {
                            blip = AddBlipForCoord(waypointPos.X, waypointPos.Y, waypointPos.Z);
                            recordedCheckpoints.Add(blip, waypointPos);
                        }
                        SetBlipAsShortRange(blip, true);
                        ShowNumberOnBlip(blip, recordedCheckpoints.Count);
                        cleared = false;
                    }

                }
                // Place cheskpoint at Ped Location
                if (Game.IsDisabledControlJustPressed(0, Control.MeleeAttackLight))
                {
                    Vector3 pos = Game.PlayerPed.Position;
                    blip = AddBlipForCoord(pos.X, pos.Y, pos.Z);
                    SetBlipAsShortRange(blip, true);
                    recordedCheckpoints.Add(blip, pos);
                    ShowNumberOnBlip(blip, recordedCheckpoints.Count);
                    cleared = false;
                    Debug.WriteLine(String.Format("Blip = {0}", blip));
                }
                // Remove Last Checkpoint
                else if (!cleared && Game.IsDisabledControlJustPressed(0, Control.Cover))
                {
                    blip = recordedCheckpoints.Last().Key;
                    Debug.WriteLine(String.Format("Blip = {0}", blip));
                    RemoveBlip(ref blip);
                    bool success = recordedCheckpoints.Remove(recordedCheckpoints.Last().Key);
                    Debug.WriteLine(String.Format("removed: {0}", success));
                    if (recordedCheckpoints.Count < 1)
                        cleared = true;
                }
                // Remove all Checkpoints
                else if (!cleared && Game.IsDisabledControlJustPressed(0, Control.Duck) && recordedCheckpoints != null)
                {
                    foreach (KeyValuePair<int, Vector3> kvp in recordedCheckpoints)
                    {
                        blip = kvp.Key;
                        RemoveBlip(ref blip);
                    }
                    recordedCheckpoints.Clear();
                    cleared = true;
                }
                // Save Race
                else if (Game.IsDisabledControlJustPressed(0, Control.Jump) && recordedCheckpoints != null)
                {
                    string newName = await GetUserInput("Enter a name for this race.", "Poo",  30);
                    //TODO save race data
                }
                // Increase/Decrease Delay
                else if (Game.IsDisabledControlJustPressed(0, Control.WeaponWheelPrev) && raceStartDelay < 300)
                    raceStartDelay += 5;
                else if (Game.IsDisabledControlJustPressed(0, Control.WeaponWheelNext) && raceStartDelay > 10)
                    raceStartDelay -= 5;
                // Increase/Decrease Bet
                else if (Game.IsDisabledControlJustPressed(0, Control.Phone) && bet < 100000000)
                    bet += 10;
                else if (Game.IsDisabledControlJustPressed(0, Control.PhoneDown) && bet > 0)
                    bet -= 10;
                // Start Race
                else if (Game.IsDisabledControlJustPressed(0, Control.Sprint) && recordedCheckpoints != null)
                {
                    /*CreateGame(Game.Player, raceStartDelay, bet, recordedCheckpoints.Values.ToArray(), Game.PlayerPed.Position);
                    foreach (KeyValuePair<int, Vector3> kvp in recordedCheckpoints)
                    {
                        blip = kvp.Key;
                        RemoveBlip(ref blip);
                    }
                    recordedCheckpoints.Clear();
                    cleared = true;
                    RacemakerActive = false;*/
                }
                await Delay(0);
            }
            await Task.FromResult(0);
        }
        public static async Task<string> GetUserInput(string windowTitle, string defaultText, int maxInputLength)
        {
            // Create the window title string.
            var spacer = "\t";
            AddTextEntry($"{GetCurrentResourceName().ToUpper()}_WINDOW_TITLE", $"{windowTitle ?? "Enter"}:{spacer}(MAX {maxInputLength} Characters)");

            // Display the input box.
            DisplayOnscreenKeyboard(1, $"{GetCurrentResourceName().ToUpper()}_WINDOW_TITLE", "", defaultText ?? "", "", "", "", maxInputLength);
            await BaseScript.Delay(0);
            // Wait for a result.
            while (true)
            {
                int keyboardStatus = UpdateOnscreenKeyboard();

                switch (keyboardStatus)
                {
                    case 3: // not displaying input field anymore somehow
                    case 2: // cancelled
                        return null;
                    case 1: // finished editing
                        return GetOnscreenKeyboardResult();
                    default:
                        await BaseScript.Delay(0);
                        break;
                }
            }
        }
    }
}
