using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace facadefinal
{
    public class DeviceInfoService
    {
        private readonly string _connectionString;

        public DeviceInfoService(string connectionString) => _connectionString = connectionString;

        public bool Exists(Guid uuid)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM DeviceInfo WHERE DeviceID = @uuid", conn);
                cmd.Parameters.AddWithValue("@uuid", uuid);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public void Insert(Guid uuid, string mac, string ram, string os, string desktop)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"INSERT INTO DeviceInfo (DeviceID, MacAddress, RamSize, OSVersion, DesktopName)
                                       VALUES (@uuid, @mac, @ram, @os, @desktop)", conn);
                cmd.Parameters.AddWithValue("@uuid", uuid);
                cmd.Parameters.AddWithValue("@mac", mac);
                cmd.Parameters.AddWithValue("@ram", ram);
                cmd.Parameters.AddWithValue("@os", os);
                cmd.Parameters.AddWithValue("@desktop", desktop);
                cmd.ExecuteNonQuery();
            }
        }
    }

}
