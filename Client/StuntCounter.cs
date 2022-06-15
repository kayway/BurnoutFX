using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

using static BurnoutFX.Client.ClientMain;
using static BurnoutFX.Client.ClientUI;
namespace BurnoutFX.Client
{

    class StuntCounter : BaseScript
    {
        private enum stunts
        {
            drift,
            air,
            slipstream,
            destruction,
            takedown,
            playerTakedown
        }
        public static bool DriftBoost = false, AirBoost = false, SlipBoost = false;
        private int driftScore = 0, slipstreamScore = 0, airScore = 0;
        private int driftTick = 0, slipTick = 0, airTick = 0;
        private bool driftOpened = false, airOpened = false, slipOpened = false;
        //Need to figure out how to make it send a nui message only once without having bools for everything <_>

        public StuntCounter()
        {
            SetEnableVehicleSlipstreaming(true);
            Tick += stuntHandler;
        }
        private async Task stuntHandler()
        {
            if (Game.PlayerPed.SeatIndex == VehicleSeat.Driver && Game.PlayerPed.IsAlive)
            {
                if (isDrifting())
                {
                    driftScore++;
                    driftTick = Game.GameTime + 3000;
                    if (!driftOpened)
                    {
                        driftOpened = true;
                        SendStunt("drift", driftOpened);
                    }
                }
                else if (driftScore != 0 && Game.GameTime >= driftTick)
                {
                    driftScore = 0;
                    if (driftOpened)
                    {
                        driftOpened = false;
                        SendStunt("drift", driftOpened);
                    }
                }
                if (isDriverValid(false))
                {
                    airScore++;
                    airTick = Game.GameTime + 3000;
                    if (!airOpened)
                    {
                        airOpened = true;
                        SendStunt("air", airOpened);
                    }
                }
                else if (airScore != 0 && Game.GameTime >= airTick)
                {
                    airScore = 0;
                    if (airOpened)
                    {
                        airOpened = false;
                        SendStunt("air", airOpened);
                    }
                }
                if (GetVehicleCurrentSlipstreamDraft(Game.PlayerPed.CurrentVehicle.NetworkId) > 0.0f)
                {
                    slipstreamScore++;
                    slipTick = Game.GameTime + 3000;
                    if (!slipOpened)
                    {
                        slipOpened = true;
                        SendStunt("slip", slipOpened);
                    }

                }
                else if (slipstreamScore != 0 && Game.GameTime >= slipTick)
                {
                    slipstreamScore = 0;
                    if (slipOpened)
                    {
                        slipOpened = false;
                        SendStunt("slip", slipOpened);
                    }
                }

                DriftBoost = driftScore > 10;
                SlipBoost = slipstreamScore > 10;
                AirBoost = airScore > 10;

                await Task.FromResult(0);
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
            if (!ped.IsInWater && !ped.IsInBoat && !ped.IsInFlyingVehicle)
            {
                if (onGround)
                    if (ped.CurrentVehicle.IsOnAllWheels && !ped.IsInAir)
                        return true;
                else if (!onGround)
                    if (ped.CurrentVehicle.IsInAir)
                        return true;
            }
            return false;
        }
        
        private bool isDrifting()
        {
            if (!isDriverValid(true))
                return false;
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