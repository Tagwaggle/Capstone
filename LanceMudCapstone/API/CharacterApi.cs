using LanceMudCapstone.DTOs;
using LanceMudCapstone.Services;
using Microsoft.AspNetCore.Routing;

namespace LanceMudCapstone.API;

public static class CharacterApi
{
    public static void MapCharacterApi(this IEndpointRouteBuilder app)
    {
        var chars = app.MapGroup("/api/characters");

        chars.MapPost("/create", async (CreateCharacterDto dto, CharacterService svc) =>
        {
            var characterId = await svc.CreateCharacterAsync(dto);
            return Results.Ok(new { characterId });
        });

        chars.MapPut("/update", async (UpdateCharacterDto dto, CharacterService svc) =>
        {
            var ok = await svc.UpdateCharacterAsync(dto);
            return ok ? Results.Ok() : Results.BadRequest(new ApiErrorDto { Message = "Update failed" });
        });

        chars.MapGet("/{id:int}", async (int id, CharacterService svc) =>
        {
            var character = await svc.GetCharacterByIdAsync(id);
            if (character is null)
                return Results.NotFound(new ApiErrorDto { Message = "Character not found" });

            return Results.Ok(character);
        });
    }
}
