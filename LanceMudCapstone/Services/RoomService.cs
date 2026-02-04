using System.Collections.Generic;
using System.Threading.Tasks;
using LanceMudCapstone.Models;
using Dapper;
using Npgsql;
using System.Net.WebSockets;
using System.Text;
using System.Diagnostics.Tracing;

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
            const string sql = @"SELECT roomid, name, description, x, y, z, createdat FROM rooms WHERE roomid = @roomId";

            Console.WriteLine(sql + roomId);

            var Results = await conn.QueryFirstOrDefaultAsync<Room>(sql, new { roomId });
            Console.WriteLine(Results);
            return Results;
        }
        public async Task<RoomExit?> GetExitAsync(int fromRoomId, string direction)
        {
            const string sql = @"SELECT exitid, fromroomid, toroomid, direction
                    FROM roomexits WHERE fromroomid = @fromRoomId AND direction = @direction";
            using var conn = new NpgsqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<RoomExit>(sql, new { fromRoomId, direction });
        }
        public async Task<IEnumerable<RoomExit>?> GetExitsForRoomAsync(int fromRoomId)
        {
            const string sql = @"SELECT exitid, fromroomid, toroomid, direction
                    FROM roomexits WHERE fromroomid = @fromRoomId";
            using var conn = new NpgsqlConnection(_connectionString);
            return await conn.QueryAsync<RoomExit>(sql, new { fromRoomId });
        }

        public async Task<IEnumerable<Room>> GetRoomsByCoordinatesAsync(int x, int y, int z)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            const string sql = @"SELECT roomid, name, description, x, y, z, createdat FROM rooms WHERRE x = @x AND y = @y AND z = @z";
            return await conn.QueryAsync<Room>(sql, new { x, y, z });
        }

        public static IEnumerable<string> WordWrap(string text, int maxWidth)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var line = new StringBuilder();

                foreach (var word in words)
                {
                    if (line.Length == 0)
                    {
                        line.Append(word);
                    }
                    else if (line.Length + 1 + word.Length <= maxWidth)
                    {
                        line.Append(' ').Append(word);
                    }
                    else
                    {
                        yield return line.ToString();
                        line.Clear();
                        line.Append(word);
                    }
                }
                if ( line.Length > 0)
                {
                    yield return line.ToString();
                }
            }
        }

    }
}

