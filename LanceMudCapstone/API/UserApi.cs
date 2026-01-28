using LanceMudCapstone.Services;
using LanceMudCapstone.DTOs;

public static class UserApi
{
    public static void MapUserApi(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/api/users");

        users.MapGet("/", async (DbHelper db) =>
        {
            var sql = "SELECT userid, username, email, isactive FROM users ORDER BY userid;";
            var list = await db.QuryListAsync<dynamic>(sql);
            return Results.Ok(list);
        });

        users.MapGet("/{id:int}", async (int id, UserService svc) =>
        {
            var user = await svc.GetUserByIdAsync(id);
            if (user is null)
                return Results.NotFound(new ApiErrorDto { Message = "User not found" });

            return Results.Ok(new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive
            });
        });
    }
}
