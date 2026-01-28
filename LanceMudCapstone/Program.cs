using LanceMudCapstone.Components;
using LanceMudCapstone.Models;
using LanceMudCapstone.Services;
using LanceMudCapstone.API;

var builder = WebApplication.CreateBuilder(args);

// Razor Components
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Only keep services you actually use
builder.Services.AddScoped<SessionState>();
builder.Services.AddScoped<EmailService>();

// HttpClient
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

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

// API groups
app.MapUserApi();
app.MapCharacterApi();
app.MapTestApi();
app.MapAuthApi();
app.MapInventoryApi();
app.MapWorldApi();

// Static + Razor
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// Contact form
app.MapPost("/api/contact", async (ContactFormModel form, EmailService emailService) =>
{
    await emailService.SendContactFormEmail(form.Name, form.Email, form.Message);
    return Results.Ok();
});

app.Run();
