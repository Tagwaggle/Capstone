using LanceMudCapstone.Models;

namespace LanceMudCapstone.DTOs;

public class RoomNpcDto
{
    public Character Base { get; set; } = new();
    public NonPlayerCharacters Instance { get; set; } = new();
}
