using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace facadefinal
{
    public class EmployeeService
    {
        private readonly string _connectionString;

        public EmployeeService(string connectionString) => _connectionString = connectionString;

        public void InsertIfNotExists(Guid uuid)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM Employess WHERE DeviceID = @uuid", conn);
                cmd.Parameters.AddWithValue("@uuid", uuid);
                if ((int)cmd.ExecuteScalar() == 0)
                {
                    var insert = new SqlCommand(@"INSERT INTO Employess (DeviceID, EmpiName, Address, ContactNo, DepartmentID)
                                              VALUES (@uuid, 'Unknown', 'Unknown', 'Unknown', 1)", conn);
                    insert.Parameters.AddWithValue("@uuid", uuid);
                    insert.ExecuteNonQuery();
                }
            }
        }
    }

}
