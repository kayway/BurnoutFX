using CitizenFX.Core;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static CitizenFX.Core.Native.API;
using static BurnoutFX.Client.ClientMain;

namespace BurnoutFX.Client
{
    public class ClientUI : BaseScript
    {
        private bool opened = false;
        public ClientUI()
        {
            Tick += drivingHUDHandler;
        }

        private Task drivingHUDHandler()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                if (!opened)
                {
                    SendNuiMessage(JsonConvert.SerializeObject(new { type = "enable-drivingUI", data = "true" }));
                    opened = true;
                }
            }
            else if (opened)
                SendNuiMessage(JsonConvert.SerializeObject(new { type = "enable-drivingUI", data = "false" }));
            
            return Task.FromResult(0);
        }
        public static void UpdateBoostHUD(int boost)
        {
            SendNuiMessage($"{{\"type\":\"boost\", \"amount\":{boost}}}");
        }
        public async void setDosh(int dosh)
        {
            StatSetInt((uint)GetHashKey("MP0_WALLET_BALANCE"), (int)dosh, true);
            N_0x170f541e1cadd1de(true);
            SetMultiplayerWalletCash();
            N_0x170f541e1cadd1de(false);
            await Delay(5000);
            RemoveMultiplayerWalletCash();
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