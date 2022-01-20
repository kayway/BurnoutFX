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
            DrawGameText("Dosh: " + CurrentBPlayer.Dosh, 0.8f, 0.1f);
            DrawGameText("Rep: " + CurrentBPlayer.Rep, 0.8f, 0.2f);
            DrawGameText("Infamy: " + CurrentBPlayer.Infamy, 0.8f, 0.3f);
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
            SetTextFont(4);
            SetTextColour(r, g, b, a);
            SetTextDropshadow(0, 0, 0, 0, 255);
            SetTextEdge(4, 0, 0, 0, 255);
            SetTextDropShadow();
            SetTextOutline();
            SetTextEntry("STRING");
            SetTextCentre(true);
            AddTextComponentString(text);
            if (zCoords != 0.0f)
            {
                //var onScreenLocation = new Vector2();
                //World3dToScreen2d(xCoords, yCoords, zCoords, ref onScreenLocation.X, ref onScreenLocation.Y);
                float distance = GetGameplayCamCoords().DistanceToSquared(new Vector3(xCoords, yCoords, zCoords));
                float scale = (1.0f / distance) * (1.0f / GetGameplayCamFov() * 100);
                SetTextScale(scaleX * scale, scaleY * scale);
                SetDrawOrigin(xCoords, yCoords, zCoords, 0);
                DrawText(0.0f, 0.0f);
            }
            else
            {
                SetTextScale(scaleX, scaleY);
                DrawText(xCoords, yCoords);
            }
        }
    }
}