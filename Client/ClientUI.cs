using CitizenFX.Core;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
using static BurnoutFX.Client.ClientMain;

namespace BurnoutFX.Client
{
    public class ClientUI : BaseScript
    {
        //scaleform ID
        private static int scale = -1;
        private bool opened = false;
        public static bool NearMarker = false;
        public ClientUI()
        {
            queueOperations();
        }
        private async void queueOperations()
        {
            await Delay(5000);
            Tick += drivingHUDHandler;
        } 
        private async Task drivingHUDHandler()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                scale = RequestScaleformMovie("INSTRUCTIONAL_BUTTONS");
                while (!HasScaleformMovieLoaded(scale)) { await Delay(0); }
                if (NearMarker)
                {
                    scale = RequestScaleformMovie("INSTRUCTIONAL_BUTTONS");
                    while (!HasScaleformMovieLoaded(scale))
                    {
                        await Delay(0);
                    }
                    DrawScaleformMovieFullscreen(scale, 255, 255, 255, 0, 0);
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
                }
                while (NearMarker)
                {
                    DrawScaleformMovieFullscreen(scale, 255, 255, 255, 255, 0);
                    await Delay(0);
                }
                if (!opened)
                {
                    SendNuiMessage("{\"type\":\"enabledrivingUI\", \"enable\":true}");
                    opened = true;
                    Debug.WriteLine("DrivingUI open");
                }
                DrawGameText("Boost: " + Boost.CurrentBoost, 0.9f, 0.2f);
            }
            else if (opened)
            {
                SendNuiMessage("{\"type\":\"enabledrivingUI\", \"enable\":false}");
                opened = false;
            }
            await Task.FromResult(0);
        }
        public static void UpdateBoostHUD(int boost)
        {
            Debug.WriteLine("Boost Triggered: " + boost);
            SendNuiMessage($"{{\"type\":\"boost\", \"amount\":{boost}}}");
        }
        public static void SendStunt(string type, bool enabled)
        {
            SendNuiMessage($"{{\"type\":\"stunt\", \"stunt\":{type}\", \"enabled\":{enabled}}}");
        }
        public static async void SetDosh(int dosh)
        {
            StatSetInt((uint)GetHashKey("MP0_WALLET_BALANCE"), dosh, true);
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