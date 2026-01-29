using LanceMudCapstone.Models;
using LanceMudCapstone.Services;
using Microsoft.AspNetCore.Builder;

namespace LanceMudCapstone.API;

public static class ContactApi
{
    public static void MapContactApi(this WebApplication app)
    {
        app.MapPost("/api/contact", async (
            ContactFormModel form,
            EmailService emailService) =>
        {

            try
            {
                await emailService.SendContactFormEmail(form.Name, form.Email, form.Message);
                return Results.Ok(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Contact form error: {ex.Message}");
                return Results.Problem("Unable to send message at this time.");
            }
        });
    }
}
