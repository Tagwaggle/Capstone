namespace LanceMudCapstone.API;

public static class WorldApi
{
    public static void MapWorldApi(this IEndpointRouteBuilder app)
    {
        var world = app.MapGroup("/api/world");

        // Future endpoints: get room, move to room, get exits
    }
}
