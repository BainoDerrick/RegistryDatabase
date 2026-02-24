using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Services
{
    public class SequenceService
    {
        private readonly AppDbContext _db;

        public SequenceService(AppDbContext db) => _db = db;

        /// <summary>
        /// Atomically allocates the next sequence number for the given deptCode and year.
        /// Returns the new sequence number (1-based).
        /// </summary>
        public async Task<int> GetNextAsync(string deptCode, int year, string who = "system")
        {
            var seqId = $"{deptCode}-{year}";
            var conn = _db.Database.GetDbConnection();
            await conn.OpenAsync();
            try
            {
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
INSERT INTO `sequence` (`sequence_id`,`dept_code`,`year`,`sequence_number`,`created_by`,`create_date`)
VALUES (@seqId,@deptCode,@year,1,@who,NOW())
ON DUPLICATE KEY UPDATE sequence_number = LAST_INSERT_ID(sequence_number + 1);
SELECT LAST_INSERT_ID();";
                var p = cmd.CreateParameter(); p.ParameterName = "@seqId"; p.Value = seqId; cmd.Parameters.Add(p);
                p = cmd.CreateParameter(); p.ParameterName = "@deptCode"; p.Value = deptCode; cmd.Parameters.Add(p);
                p = cmd.CreateParameter(); p.ParameterName = "@year"; p.Value = year; cmd.Parameters.Add(p);
                p = cmd.CreateParameter(); p.ParameterName = "@who"; p.Value = who; cmd.Parameters.Add(p);

                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            finally
            {
                // do not close connection explicitly; let pooling handle it
                try { conn.Close(); } catch { }
            }
        }
    }
}
