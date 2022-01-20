using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace BurnoutFX.Client
{
    public class Marker : BaseScript
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Heading { get; set; }
        public int Type { get; set; }
        
        public int BlipID = 0;
        public void drawMarker()
        {
                DrawMarker(Type, X, Y, Z,
                    0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 3.0001f, 3.0001f, 1.5001f, 255, 165, 0, 165, false, false, 2, false, null, null, false);
        }
        /// <summary>
        /// Creates Blip for the map
        /// </summary>
        /// <param name="name"></param>
        /// <param name="blipColour"></param>
        /// <param name="blipIcon"></param>
        /// <returns>Id of Blip</returns>
        public int AddBlip(string name, int blipColour, int blipIcon)
        {
            if (BlipID == 0)
            {
                BlipID = AddBlipForCoord(X, Y, Z);
                SetBlipSprite(BlipID, blipIcon);
                SetBlipDisplay(BlipID, 4);
                SetBlipScale(BlipID, 1.0f);
                SetBlipColour(BlipID, blipColour);
                SetBlipAsShortRange(BlipID, true);
                BeginTextCommandSetBlipName("STRING");
                AddTextComponentString(name);
                EndTextCommandSetBlipName(BlipID);
            }
            return BlipID;
        }
    }
}
