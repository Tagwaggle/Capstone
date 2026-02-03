using System.Collections.Generic;
using System.Threading.Tasks;
using LanceMudCapstone.Models;
using Dapper;
using Npgsql;

namespace LanceMudCapstone.Services
{
    public class ContainerService
    {
        private readonly string _connectionString;

        public ContainerService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Container>> GetContainersInRoom(int roomId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            const string sql = @"SELECT containerid, roomid, name, createdat
                                 FROM containers
                                 WHERE roomid = @roomId";

            return await conn.QueryAsync<Container>(sql, new { roomId });
        }

        public async Task<Container?> GetContainerAsync(int containerId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            const string sql = @"SELECT containerid, roomid, name, description, createdat
                                 FROM containers
                                 WHERE containerid = @containerId";

            return await conn.QueryFirstOrDefaultAsync<Container>(sql, new { containerId });
        }
    }
}
