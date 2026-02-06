using LanceMudCapstone.Components;
using LanceMudCapstone.Models;
using LanceMudCapstone.Services;
using LanceMudCapstone.API;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddScoped<SessionState>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<CharacterService>();
builder.Services.AddSingleton<MobService>();
builder.Services.AddSingleton<EffectService>();
builder.Services.AddSingleton<RespawnService>();
builder.Services.AddSingleton<WorldEngine>();
builder.Services.AddSingleton<AbilityEngine>();
builder.Services.AddSingleton<ICombatService, CombatService>();

var supabaseConnString = builder.Configuration.GetConnectionString("SupabaseDb") ??
    throw new InvalidOperationException("Missing SupabaseDb connection string");

builder.Services.AddScoped<DbHelper>();
builder.Services.AddScoped(sp => new ContainerService(supabaseConnString));
builder.Services.AddScoped(sp => new RoomService(supabaseConnString));

builder.Services.AddHttpClient();
builder.Services.AddHttpClient("ServerAPI", client =>
{
    string? baseUrl = null;

    var azureHost = builder.Configuration["WEBSITE_HOSTNAME"];
    if (!string.IsNullOrWhiteSpace(azureHost))
    {
        var fullUrl = $"https://{azureHost}/";
        if (Uri.IsWellFormedUriString(fullUrl, UriKind.Absolute))
            baseUrl = fullUrl;
    }

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

    baseUrl ??= "https://localhost:7038/";
    client.BaseAddress = new Uri(baseUrl);
});


var app = builder.Build();

var world = app.Services.GetRequiredService<WorldEngine>();
world.Start();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapUserApi();
app.MapCharacterApi();
app.MapTestApi();
app.MapAuthApi();
app.MapInventoryApi();
app.MapWorldApi();
app.MapContactApi();
app.MapPlayerCharacterApi();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Use(async (context, next) =>
{
    Console.WriteLine($"Incoming {context.Request.Method} {context.Request.Path}, Content-Type: {context.Request.ContentType}");
    await next();
});

 
app.Run();
