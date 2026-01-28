using LanceMudCapstone.Services;
using LanceMudCapstone.DTOs;
using LanceMudCapstone.Models;

public static class AuthApi
{
    public static void MapAuthApi(this IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/api/auth");

        auth.MapPost("/register", async (CreateUserDto dto, UserService svc) =>
        {
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = PasswordHelper.HashPassword(dto.Password),
                IsActive = true
            };

            var id = await svc.AddUserAsync(user);
            return Results.Ok(new { userId = id });
        });

        auth.MapPost("/login", async (LoginDto dto, UserService svc, SessionState session) =>
        {
            var user = await svc.ValidateLoginAsync(dto.Username, dto.Password);
            if (user is null)
                return Results.Json(
                    new ApiErrorDto { Message = "Invalid credentials" },
                    statusCode: 401
                );

            session.SetUser(user.UserId, user.Username);

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
