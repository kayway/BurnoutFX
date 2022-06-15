using MySqlConnector;
using System;

namespace BurnoutFX.Server
{
    public class DatabaseApp : IDisposable
    {
        public readonly MySqlConnection Connection;

        public DatabaseApp()
        {
            Connection = new MySqlConnection("Server=localhost;User ID=root;Password=;Database=burnoutfx;");
        }

        public void Dispose()
        {
            Connection.Close();
        }
    }
}
