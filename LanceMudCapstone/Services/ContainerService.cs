using Dapper;
using LanceMudCapstone.DTOs;
using LanceMudCapstone.Models;
using LanceMudCapstone.Services;

public class ContainerService
{
    private readonly DbHelper _db;

    public ContainerService(DbHelper db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Container>> GetContainersInRoom(int roomId)
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        const string sql = @"SELECT containerid, name, roomid, ownernpcid, islootable, createdat
                             FROM containers
                             WHERE roomid = @roomId";

        return await conn.QueryAsync<Container>(sql, new { roomId });
    }

    public async Task<ContainerDto?> GetContainerAsync(int containerId)
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        const string sql = @"SELECT containerid, name, roomid, ownernpcid, islootable, createdat
                             FROM containers
                             WHERE containerid = @containerId";

        return await conn.QueryFirstOrDefaultAsync<ContainerDto>(sql, new { containerId });
    }

    public async Task<List<ItemDto>> GetLootAsync(int playerCharacterId, int containerId)
    {
        using var conn = await _db.CreateOpenConnectionAsync();

        // 1. Load container
        string containerSql = @"
            SELECT containerid, name, roomid
            FROM containers
            WHERE containerid = @Id;
        ";

        var container = await conn.QueryFirstOrDefaultAsync<ContainerDto>(containerSql, new { Id = containerId });

        if (container == null)
            return new List<ItemDto>();

        string itemsSql = @"
            SELECT ri.itemid, ri.quantity,
                   i.name, i.description, i.itemtype, i.itemcategory
            FROM roomitems ri
            INNER JOIN items i ON i.itemid = ri.itemid
            WHERE ri.containerid = @ContainerId;
        ";

        var items = (await conn.QueryAsync<ItemDto>(itemsSql, new { ContainerId = containerId })).ToList();

        foreach (var item in items)
        {
            string insertSql = @"
                    INSERT INTO playerinventory (playercharacterid, itemid, quantity)
                    VALUES (@PlayerId, @ItemId, @Quantity)
                    ON CONFLICT (playercharacterid, itemid)
                    DO UPDATE SET quantity = playerinventory.quantity + EXCLUDED.quantity;
                ";

            await conn.ExecuteAsync(insertSql, new
            {
                PlayerId = playerCharacterId,
                ItemId = item.ItemId,
                Quantity = item.Quantity
            });
        }

        string deleteItemsSql = @"
            DELETE FROM roomitems
            WHERE containerid = @ContainerId;
        ";

        await conn.ExecuteAsync(deleteItemsSql, new { ContainerId = containerId });

        if (container.Name.Contains("Corpse", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Corpse Delete");
            string deleteContainerSql = @"
                DELETE FROM containers
                WHERE containerid = @ContainerId;
            ";

            await conn.ExecuteAsync(deleteContainerSql, new { ContainerId = containerId });
        }

        return items;
    }
}
