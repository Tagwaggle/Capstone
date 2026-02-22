using LanceMudCapstone.DTOs;
using LanceMudCapstone.Services;
using LanceMudCapstone.Enums;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

public static class AuthApi
{
    public static void MapAuthApi(this IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/api/auth");

        // POST /api/auth/register
        auth.MapPost("/register", async (CreateUserDto dto, IConfiguration config) =>
        {
            var connString = config["SupabaseDb"];

            try
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();

                var cmd = new NpgsqlCommand(
                    @"INSERT INTO users (username, email, passwordhash, isactive)
                      VALUES (@u, @e, @p, true)
                      RETURNING userid;",
                    conn
                );

                cmd.Parameters.AddWithValue("@u", dto.Username.Trim());
                cmd.Parameters.AddWithValue("@e", dto.Email);
                cmd.Parameters.AddWithValue("@p", PasswordHelper.HashPassword(dto.Password));

                var result = await cmd.ExecuteScalarAsync();

                if (result is int newId)
                {
                    return Results.Ok(new { userId = newId });
                }

                return Results.Problem("Failed to create user. No ID returned from database.");
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.ToString());
            }
        });

        // POST /api/auth/login
        auth.MapPost("/login", async (LoginDto dto, IConfiguration config) =>
        {
            var connString = config["SupabaseDb"];

            try
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();

                var cmd = new NpgsqlCommand(
                    @"SELECT userid, username, email, passwordhash, isactive, profilepictureid
                      FROM users
                      WHERE username = @u
                      LIMIT 1;",
                    conn
                );

                cmd.Parameters.AddWithValue("@u", dto.Username);

                var reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    return Results.BadRequest(new ApiErrorDto { Message = $"DTOuname{dto.Username}Invalid credentials" });
                }

                var storedHash = reader.GetString(reader.GetOrdinal("passwordhash"));

                if (!PasswordHelper.VerifyPassword(dto.Password, storedHash))
                {
                    return Results.BadRequest(new ApiErrorDto { Message = $"DTOuname{dto.Username}Invalid credentials" });
                }

                var user = new UserDto
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("userid")),
                    Username = reader.GetString(reader.GetOrdinal("username")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("isactive")),
                    ProfilePictureId =  (UserIcon)reader.GetInt32(reader.GetOrdinal("profilepictureid"))
                };

                return Results.Ok(user);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.ToString());
            }
        });

        // ⭐ GET /api/auth/validate/{userId}
        auth.MapGet("/validate/{userId}", async (int userId, IConfiguration config) =>
        {
            var connString = config["SupabaseDb"];
            Console.WriteLine("Validate Called");
            await using var conn = new NpgsqlConnection(connString);
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand(
                @"SELECT userid, username, profilepictureid 
                  FROM users 
                  WHERE userid = @id AND isactive = true
                  LIMIT 1;",
                conn
            );

            cmd.Parameters.AddWithValue("@id", userId);

            var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return Results.NotFound();

            var user = new UserDto
            {
                UserId = reader.GetInt32(reader.GetOrdinal("userid")),
                Username = reader.GetString(reader.GetOrdinal("username")),
                ProfilePictureId = (UserIcon)reader.GetInt32(reader.GetOrdinal("profilepictureid"))
            };

            return Results.Ok(user);
        });
    }
}
