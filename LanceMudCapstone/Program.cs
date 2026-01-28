using LanceMudCapstone.Components;
using LanceMudCapstone.Models;
using LanceMudCapstone.Services;

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
app.MapGet("/api/test-db", async (DatabaseService db) =>
{
    var result = await db.CanConnectAsync();
    return result ? Results.Ok("DB connection successful") : Results.Problem("DB connection failed");
});

app.MapGet("/api/users", async (DbHelper db) =>
{
    var sql = "SELECT userid, username, email, isactive FROM users ORDER BY userid;";
    var users = await db.QuryListAsync<dynamic>(sql);
    return Results.Ok(users);
});


app.Run();
