using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace facadefinal
{
    public class ScoringService
    {
        private readonly string _connectionString;

        public ScoringService(string connectionString) => _connectionString = connectionString;

        public void InsertDefaultScoreIfNotExists(Guid uuid)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM Scoring WHERE DeviceID = @uuid", conn);
                cmd.Parameters.AddWithValue("@uuid", uuid);
                if ((int)cmd.ExecuteScalar() == 0)
                {
                    var insert = new SqlCommand("INSERT INTO Scoring (DeviceID, Score) VALUES (@uuid, 100)", conn);
                    insert.Parameters.AddWithValue("@uuid", uuid);
                    insert.ExecuteNonQuery();
                }
            }
        }

        public void DeductScore(Guid uuid, int penalty)
        {
            if (penalty <= 0) return;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var update = new SqlCommand("UPDATE Scoring SET Score = Score - @penalty WHERE DeviceID = @uuid", conn);
                update.Parameters.AddWithValue("@penalty", penalty);
                update.Parameters.AddWithValue("@uuid", uuid);
                update.ExecuteNonQuery();
            }
        }
    }

}
