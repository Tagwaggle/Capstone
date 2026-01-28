namespace LanceMudCapstone.API;

public static class InventoryApi
{
    public static void MapInventoryApi(this IEndpointRouteBuilder app)
    {
        var inv = app.MapGroup("/api/inventory");

        // Future endpoints: add, remove, list
    }
}

