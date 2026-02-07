using System.Text.Json;
using LanceMudCapstone.Models;

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
