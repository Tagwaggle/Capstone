using LanceMudCapstone.Components;
using LanceMudCapstone.Models;
using LanceMudCapstone.Services;
using LanceMudCapstone.API;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Razor Components (correct registration)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRadzenComponents();

builder.Services.AddSingleton<SQLiteService>();
builder.Services.AddSingleton<SessionRepository>();

// App services
builder.Services.AddScoped<SessionState>();
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<CharacterService>();
builder.Services.AddScoped<LogoutService>();

builder.Services.AddScoped<MobService>();
builder.Services.AddScoped<EffectService>();
builder.Services.AddScoped<RespawnService>();
builder.Services.AddScoped<WorldEngine>();
builder.Services.AddScoped<AbilityEngine>();

builder.Services.AddScoped<ICombatService, CombatService>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IEquipRepository, EquipRepository>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<UserService>();

var supabaseConnString = builder.Configuration.GetConnectionString("SupabaseDb")
    ?? throw new InvalidOperationException("Missing SupabaseDb connection string");

builder.Services.AddScoped<DbHelper>();
builder.Services.AddScoped<ContainerService>();
builder.Services.AddScoped(sp => new RoomService(supabaseConnString));

// HttpClient setup
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
            baseUrl = fullUrl;
    }

    // Render or Docker
    if (baseUrl is null)
    {
        var publicUrl = builder.Configuration["PUBLIC_URL"];
        if (!string.IsNullOrWhiteSpace(publicUrl))
            baseUrl = publicUrl;
    }

    // Local dev fallback
    baseUrl ??= "https://localhost:7038/";

    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var sqlite = scope.ServiceProvider.GetRequiredService<SQLiteService>();
    using var conn = sqlite.GetConnection();

    var cmd = conn.CreateCommand();
    cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Sessions (
Token TEXT PRIMARY KEY,
UserId INTEGER NOT NULL,
CreatedAt TEXT NOT NULL,
ExpiresAt TEXT NOT NULL
);";
    cmd.ExecuteNonQuery();
}
// Start world engine
using (var scope = app.Services.CreateScope())
{
    var world = scope.ServiceProvider.GetRequiredService<WorldEngine>();
    world.Start();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

// API endpoints
app.MapUserApi();
app.MapCharacterApi();
app.MapTestApi();
app.MapAuthApi();
app.MapInventoryApi();
app.MapWorldApi();
app.MapContactApi();
app.MapPlayerCharacterApi();

// Static assets + Razor components
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Request logging
app.Use(async (context, next) =>
{
    Console.WriteLine($"Incoming {context.Request.Method} {context.Request.Path}, Content-Type: {context.Request.ContentType}");
    await next();
});

app.Run();
