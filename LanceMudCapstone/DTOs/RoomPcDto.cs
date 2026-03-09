using LanceMudCapstone.Models;

namespace LanceMudCapstone.DTOs;

public class RoomPcDto
{
    public Character Base { get; set; } = new();
    public PlayerCharacter Instance { get; set; } = new();
}
