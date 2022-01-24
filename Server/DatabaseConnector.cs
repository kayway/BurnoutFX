using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

using MySqlConnector;
using CitizenFX.Core;
using BurnoutFX.Shared;

namespace BurnoutFX.Server
{
    public static class DatabaseConnector
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="licenseID"></param>
        /// <param name="playerName"></param>
        /// <returns></returns>
        public static async Task<BurnoutPlayer> InitializePlayerData(string licenseID, string playerName)
        {
            DatabaseApp database = new DatabaseApp();
            BurnoutPlayer connectedPlayer = new BurnoutPlayer();
            try
            { 
                await database.Connection.OpenAsync();
                MySqlCommand command = new MySqlCommand("initialize_player", database.Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@player_id", licenseID);
                command.Parameters.AddWithValue("@player_name", playerName);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    connectedPlayer.Dosh = reader.GetUInt32(0);
                    connectedPlayer.Rep = reader.GetUInt32(1);
                    connectedPlayer.Infamy = reader.GetUInt32(2);
                    Debug.WriteLine(reader[0] + "    " + reader[1]);
                }
                reader.Close();
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Oh This happened: >> {exception} ");
            }
            return connectedPlayer;
        }
        /// <summary>
        /// Queries the Tracks database.
        /// </summary>
        /// <returns>Enabled Tracks Marker Data.</returns>
        public static async Task<Dictionary<string, Tuple<uint, uint, string>>> RetrieveTracks()
        {
            DatabaseApp database = new DatabaseApp();
            Dictionary<string, Tuple<uint, uint, string>> trackMarkers = new Dictionary<string, Tuple<uint, uint, string>>();
            try
            {
                await database.Connection.OpenAsync();
                MySqlCommand command = new MySqlCommand("SELECT title, colour, icon, start FROM tracks WHERE enabled=1", database.Connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    trackMarkers.Add(reader.GetString(0), Tuple.Create(reader.GetUInt32(1), reader.GetUInt32(2), reader.GetString(3)));
                    Debug.WriteLine("name: " + reader[0] + " marker: " + reader[3]);
                }
                reader.Close();
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Oh This happened: >> {exception} ");
            }
            return trackMarkers;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="raceName"></param>
        /// <returns></returns>
        public static async Task<Tuple<float, float, string>> RetreiveTrackData(string raceName)
        {
            DatabaseApp database = new DatabaseApp();
            string checkpoints = "";
            float checkpointRadius = 16.0f, checkpointTransparency = 1.0f;
            try
            {
                await database.Connection.OpenAsync();
                MySqlCommand command = new MySqlCommand("SELECT radius, transparency, checkpoints FROM tracks WHERE title = @race_name", database.Connection);
                command.Parameters.AddWithValue("@race_name", raceName);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    checkpointRadius = reader.GetFloat(0);
                    checkpointTransparency = reader.GetFloat(1);
                    checkpoints = reader.GetString(2);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Oh This happened: >> {exception} ");
            }
            Tuple<float, float, string> trackData = new Tuple<float, float, string>(checkpointRadius, checkpointTransparency, checkpoints);
            return trackData;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="time"></param>
        /// <param name="track"></param>
        /// <param name="vehicle"></param>
        public static async void SubmitTime(string playerID, int time, string track, string vehicle)
        {
            DatabaseApp database = new DatabaseApp();
            try
            {
                await database.Connection.OpenAsync();
                MySqlCommand bestTrackCommand = new MySqlCommand("SELECT MIN(time) FROM timetrials WHERE track = @track", database.Connection);
                bestTrackCommand.Parameters.AddWithValue("@track", track);
                
                MySqlCommand bestCarCommand = new MySqlCommand("SELECT MIN(time) FROM timetrials WHERE track = @track AND vehicle = @vehicle", database.Connection);
                bestCarCommand.Parameters.AddWithValue("@track", track);
                bestCarCommand.Parameters.AddWithValue("@vehicle", vehicle);
                
                MySqlCommand bestPersonalCommand = new MySqlCommand("SELECT MIN(time) FROM timetrials WHERE track = @track AND player_id = @player_id", database.Connection);
                bestPersonalCommand.Parameters.AddWithValue("@player_id", playerID);
                bestPersonalCommand.Parameters.AddWithValue("@track", track);
                
                MySqlCommand submitCommand = new MySqlCommand("REPLACE INTO timetrials (track, vehicle, player_id, time) VALUES(@track, @vehicle, @player_id, @finish)", database.Connection);
                submitCommand.Parameters.AddWithValue("@player_id", playerID);
                submitCommand.Parameters.AddWithValue("@track", track);
                submitCommand.Parameters.AddWithValue("@finish", time);
                submitCommand.Parameters.AddWithValue("@vehicle", vehicle);
                
                uint bestTrackTime = ConvertFromDBVal<uint>(bestTrackCommand.ExecuteScalar());
                if (bestTrackTime != 0)
                {
                    
                    Debug.WriteLine(bestTrackTime.ToString());
                    if(bestTrackTime > time)
                    {
                        Debug.WriteLine("Best time on track!!");
                        submitCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        uint bestCarTime = ConvertFromDBVal<uint>(bestCarCommand.ExecuteScalar());
                        if (bestCarTime == 0 || bestCarTime > time)
                        {
                            Debug.WriteLine("Best Time in this car!!");
                            submitCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            uint bestPersonalTime = ConvertFromDBVal<uint>(bestPersonalCommand.ExecuteScalar());
                            if (bestPersonalTime == 0 || bestPersonalTime > time)
                            {
                                Debug.WriteLine("Best personal time!!");
                                submitCommand.ExecuteNonQuery();
                            }
                            else
                            {
                                Debug.WriteLine("You tried...");
                            }
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("First time on this track!!");
                    submitCommand.ExecuteNonQuery();
                }
                
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Oh This happened: >> {exception} ");
            }
        }
        public static T ConvertFromDBVal<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default(T); 
            }
            else
            {
                return (T)obj;
            }
        }
    }
}
