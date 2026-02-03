using System.Collections.Generic;
using System.Threading.Tasks;
using LanceMudCapstone.Models;
using Dapper;
using Npgsql;

namespace LanceMudCapstone.Services
{
    public class RoomService
    {
        private readonly string _connectionString = string.Empty;

        public RoomService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Room?> GetRoomAsync(int roomId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            const string sql = @"SELECT roomid, name, description, x, y, z, createdat FROM rooms";

            return await conn.QueryFirstOrDefaultAsync<Room>(sql, new {roomId});
        }

        public async Task<IEnumerable<Room>> GetRoomsByCoordinatesAsync(int x, int y, int z)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            const string sql = @"SELECT roomid, name, description, x, y, z, createdat FROM rooms WHERRE x = @x AND y = @y AND z = @z";
            return await conn.QueryAsync<Room>(sql, new { x, y, z });
        }

    }
}

