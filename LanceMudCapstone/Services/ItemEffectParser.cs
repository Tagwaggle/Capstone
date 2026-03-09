using LanceMudCapstone.Models;
using System.Text.Json;

namespace LanceMudCapstone.Services
{
    public static class ItemEffectParser
    {
        public static ItemEffect? Parse(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonSerializer.Deserialize<ItemEffect>(json);
        }
    }
}
