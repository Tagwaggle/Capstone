using LanceMudCapstone.Components;
using LanceMudCapstone.Models;
using LanceMudCapstone.Services;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<SupabaseService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<DbHelper>();
builder.Services.AddScoped<SessionState>();
builder.Services.AddScoped<EmailService>();

builder.Services.AddHttpClient();
builder.Services.AddHttpClient("ServerAPI", client =>
{
    string? baseUrl = null;

    // Azure App Service
    var azureHost = builder.Configuration["WEBSITE_HOSTNAME"];
    if (!string.IsNullOrWhiteSpace(azureHost))
    {
        var fullUrl = $"https://{azureHost}/";
        if (Uri.IsWellFormedUriString(fullUrl, UriKind.Absolute))
        {
            baseUrl = fullUrl;
        }
    }

    // Local dev
    if (baseUrl is null)
    {
        var urls = builder.Configuration["ASPNETCORE_URLS"];
        if (!string.IsNullOrWhiteSpace(urls))
        {
            baseUrl = urls
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault(u => Uri.IsWellFormedUriString(u, UriKind.Absolute));
        }
    }

    // Final fallback
    baseUrl ??= "https://localhost:7038/";

    client.BaseAddress = new Uri(baseUrl);
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapPost("/api/contact", async (
    ContactFormModel form,
    EmailService emailService) =>
{
    await emailService.SendContactFormEmail(form.Name, form.Email, form.Message);
    return Results.Ok();
});
app.MapGet("/api/test-db", async (IConfiguration config) =>
{
    var connString = config["SupabaseDb"];

    try
    {
        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();
        return Results.Ok($"DB connection successful. ConnString={connString}");
    }
    catch (Exception ex)
    {
        return Results.Problem($"DB connection failed. ConnString={connString}. Error={ex.Message}");
    }
});

app.MapGet("/api/users", async (DbHelper db) =>
{
    var sql = "SELECT userid, username, email, isactive FROM users ORDER BY userid;";
    var users = await db.QuryListAsync<dynamic>(sql);
    return Results.Ok(users);
});


app.Run();
