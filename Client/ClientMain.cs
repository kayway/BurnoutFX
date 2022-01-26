using BurnoutFX.Shared;
using CitizenFX.Core;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace BurnoutFX.Client
{
    public class ClientMain : BaseScript
    {
        public static BurnoutPlayer CurrentBPlayer;

        public ClientMain()
        {
            CurrentBPlayer = new BurnoutPlayer();
            TriggerServerEvent("requestPlayerData");
        }

        [EventHandler("retrievePlayerData")]
        private void retrievePlayerData(uint dosh, uint rep, uint infamy)
        {
            CurrentBPlayer.Dosh = dosh;
            CurrentBPlayer.Rep = rep;
            CurrentBPlayer.Infamy = infamy;
        }

        [Tick]
        public Task OnTick()
        {
            //DrawGameText("Dosh: " + CurrentBPlayer.Dosh, 0.8f, 0.1f);
            //DrawGameText("Gameplay Camera:" + GetGameplayCamCoords().ToString(), 0.8f, 0.2f);
            //DrawGameText("Player Ped: " + Game.PlayerPed.Position.ToString(), 0.8f, 0.3f);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Draws Specified text ingame.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="xCoords"></param>
        /// <param name="yCoords"></param>
        /// <param name="zCoords"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        public static void DrawGameText(string text, float xCoords, float yCoords, float zCoords = 0.0f, 
            int r = 255, int g = 255, int b = 255, int a = 255, float scaleX = 0.5f, float scaleY = 0.5f)
        {
            bool onScreen = true;
            float drawX = xCoords, drawY = yCoords;
            if (zCoords != 0.0f)
            {
                drawX = 0.0f; 
                drawY = 0.0f;
                onScreen = World3dToScreen2d(xCoords, yCoords, zCoords, ref drawX, ref drawY);
                float distance = GetGameplayCamCoords().DistanceToSquared(new Vector3(xCoords, yCoords, zCoords));
                float scale = ((1.0f / distance) * 2.0f) * ((1.0f /GetGameplayCamFov()) * 100.0f) * scaleX;
                SetTextScale(0, scale);
            }
            if (onScreen)
            {

                SetTextFont(4);
                SetTextProportional(true);
                SetTextColour(r, g, b, a);
                SetTextDropshadow(0, 0, 0, 0, 255);
                SetTextEdge(4, 0, 0, 0, 255);
                SetTextDropShadow();
                SetTextOutline();
                SetTextEntry("STRING");
                SetTextCentre(true);
                AddTextComponentString(text);
                DrawText(drawX, drawY);
            }
        }
    }
}