using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static BurnoutFX.Client.StuntCounter;
namespace BurnoutFX.Client
{
    public class Boost : BaseScript
    {
        public static int CurrentBoost = 0;
        public static int BoostRate = 0;

        private int oldBoostRate = 0;
        private int DriftRate = 1;
        private int AirTimeRate = 1;
        private int SlipstreamRate = 1;
        private int DestructionRate = 1;
        private int TakedownRate = 1;
        private int PlayerTakedownRate = 1;
        private string vehicleName;
        private int lastVehicle = 0;

        public Boost()
        {
            BoostTypes vehicleBoostType = BoostTypes.stunt;
            switch(vehicleBoostType)
            {
                case BoostTypes.stunt :
                    DriftRate = 2;
                    AirTimeRate = 2;
                    break;
                case BoostTypes.speed:
                    SlipstreamRate = 2;
                    break;
                case BoostTypes.aggression:
                    TakedownRate = 2;
                    PlayerTakedownRate = 2;
                    break;
                case BoostTypes.locked:
                case BoostTypes.Switch:
                    break;
            }
            Tick += boostHandler;
            Tick += boostRateHandler;
        }
        private async Task boostHandler()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                if (Game.PlayerPed.CurrentVehicle.NetworkId != lastVehicle)
                {
                    CurrentBoost = 100;
                    SendNuiMessage($"{{\"type\":\"boost\", \"amount\":{CurrentBoost}}}");
                    lastVehicle = Game.PlayerPed.CurrentVehicle.NetworkId;
                }
                CurrentBoost = MathUtil.Clamp(CurrentBoost += BoostRate, 0, 100);
            }
            await Task.FromResult(1000);
        }
        private async Task boostRateHandler()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                bool isBoosting = Game.IsControlPressed(0, Control.VehicleRocketBoost) && CurrentBoost > 0;
                int playerVehicle = Game.PlayerPed.CurrentVehicle.Handle;
                if (isBoosting)
                {
                    SetVehicleBoostActive(playerVehicle, true);
                    SetVehicleForwardSpeed(playerVehicle, 80.0f);
                    StartScreenEffect("RaceTurbo", 0, false);
                }
                else
                {
                    SetVehicleBoostActive(playerVehicle, false);
                }
                BoostRate = (isBoosting ? -2 : 0) + (DriftBoost ? DriftRate : 0) + (SlipBoost ? SlipstreamRate : 0) + (AirBoost ? AirTimeRate : 0);
                if (oldBoostRate != BoostRate)
                {
                    SendNuiMessage($"{{\"type\":\"boostRate\", \"amount\":{BoostRate}}}");
                    oldBoostRate = BoostRate;
                }
            }
            await Task.FromResult(0);
        }
        //Native Implementation
        private async void addBoost()
        {
            await Delay(3000);
            int playerVehicle = Game.PlayerPed.CurrentVehicle.Handle;
            SetVehicleBoostActive(playerVehicle, true);
            SetVehicleForwardSpeed(playerVehicle, 80.0f);
            StartScreenEffect("RaceTurbo", 0, false);
            SetVehicleBoostActive(playerVehicle, false);

        }
        // HardCoded Implementation.
        private async void addBoost2()
        {
            int playerVehicle = Game.PlayerPed.CurrentVehicle.Handle;
            Vector3 forwardVector = GetEntityForwardVector(playerVehicle);
            float scale = 50;
            Vector3 targetVelocity = (forwardVector * scale) + GetEntityVelocity(playerVehicle);
            SetEntityVelocity(playerVehicle, targetVelocity.X, targetVelocity.Y, targetVelocity.Z);
        }
        // HardCoded Implementation 2.
        private async void addBoost3()
        {
            int playerVehicle = Game.PlayerPed.CurrentVehicle.Handle;
            Vector3 forwardVector = GetEntityForwardVector(playerVehicle);
            float scale = 10;
            Vector3 targetVelocity = GetEntityVelocity(playerVehicle) + scale;
            SetEntityVelocity(playerVehicle, targetVelocity.X, targetVelocity.Y, targetVelocity.Z);
        }
    }
}