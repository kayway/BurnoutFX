using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

using static BurnoutFX.Client.ClientMain;
namespace BurnoutFX.Client
{
    class StuntCounter : BaseScript
    {
        private int driftScore, slipstreamScore, airScore, updateRate = 1;

        public StuntCounter()
        {
            SetEnableVehicleSlipstreaming(true);
        }
        [Tick]
        public Task OnTick()
        {
            if (driftScore > 10)
            DrawGameText("Drift: " + driftScore.ToString(), 0.4f, 0.1f);
            if (slipstreamScore > 10)
            DrawGameText("Slipstream: " + slipstreamScore.ToString(), 0.5f, 0.1f);
            if (airScore > 10)
            DrawGameText("Air: " + airScore.ToString(), 0.6f, 0.1f);
            return Task.FromResult(0);
        }
        [EventHandler("onClientResourceStart")]
        private async void counters()
        {
            while (true)
            {
                if (!isDriverValid(true))
                {
                    driftScore = 0;
                    slipstreamScore = 0;
                }
                else if (GetVehicleCurrentSlipstreamDraft(Game.PlayerPed.CurrentVehicle.NetworkId) == 0.0f)
                    slipstreamScore = 0;
                if (!isDrifting())
                    driftScore = 0;
                if (!isDriverValid(false))
                    airScore = 0; 
                airScore++;
                slipstreamScore++;
                driftScore++;
                await Delay(updateRate);
            }
        }
        /// <summary>
        /// processes player peds current situation
        /// </summary>
        /// <param name="onGround"></param>
        /// <returns>Whether driver is allowed to get score</returns>
        private bool isDriverValid(bool onGround)
        {
            Ped ped = Game.PlayerPed;
            if (!ped.IsDead && ped.SeatIndex == VehicleSeat.Driver && !ped.IsInWater && !ped.IsInBoat && !ped.IsInFlyingVehicle)
            {
                if (onGround)
                {
                    if (ped.CurrentVehicle.IsOnAllWheels && !ped.IsInAir)
                        return true;
                }
                else if (!onGround)
                {
                    if (ped.CurrentVehicle.IsInAir)
                        return true;
                }
            }
            return false;
        }
        
        private bool isDrifting()
        {
  
            Vehicle vehicle = Game.PlayerPed.CurrentVehicle;
            if (vehicle == null)
                return false;
            //whether the car has started or speed is below 30km/h
            if (vehicle.CurrentGear == 0 || vehicle.Speed * 3.6 < 30)
                return false;
            Vector3 velocity = vehicle.Velocity;
            double magnitude = Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y);
            Vector3 vehicleRot = vehicle.Rotation;
            double sin1 = -Math.Sin(degrees2Radians(vehicleRot.Z));
            double cos1 = Math.Cos(degrees2Radians(vehicleRot.Z));
            double cosX = (sin1 * velocity.X + cos1 * velocity.Y) / magnitude;
            //angle tolerance
            if (cosX > 0.966 || cosX < 0)
                return false;
            return true;
        }
        private double degrees2Radians(double degrees)
        {
            return ((Math.PI / 180) * degrees);
        }

        private double radians2Degrees(double radians)
        {
            return ((180 / Math.PI) * radians);
        }
    }
}
