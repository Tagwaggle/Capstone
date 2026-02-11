namespace LanceMudCapstone.Models;

public class NonPlayerCharacters
{
    public int NpcId { get; set; }
    public int CharacterId { get; set; }
    public string BehaviorType { get; set; } = string.Empty;
    public bool IsHostile { get; set; }
    public int? RespawnTime { get; set; }
    public int? LootTableId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? RoomId { get; set; }
    public int XpReward { get; set; }
    public DateTime? LastKilled { get; set; }
    public DateTime? RespawnAt { get; set; }



}
