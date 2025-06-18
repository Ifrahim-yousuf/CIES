using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace facadefinal
{
    public class ViolationAnalyzer
    {
        private readonly string _connectionString;

        public ViolationAnalyzer(string connectionString) => _connectionString = connectionString;

        public int AnalyzeAndGetPenalty(string keystrokes)
        {
            var normalized = Regex.Replace(keystrokes, @"[^\w\s]", " ").ToLower();
            var totalPenalty = 0;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT Word, Severity FROM BlackListedWords", conn);
                var reader = cmd.ExecuteReader();

                var words = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                while (reader.Read())
                {
                    words[reader.GetString(0)] = reader.GetInt32(1);
                }

                foreach (var pair in words)
                {
                    int count = Regex.Matches(normalized, Regex.Escape(pair.Key.ToLower())).Count;
                    totalPenalty += count * pair.Value;
                }
            }

            return totalPenalty;
        }
    }

}
