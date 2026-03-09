using LanceMudCapstone.Models;
using LanceMudCapstone.Services;

namespace LanceMudCapstone.API;

public static class ContactApi
{
    public static void MapContactApi(this WebApplication app)
    {
        Console.WriteLine("Test");
        app.MapPost("/api/contact", async (ContactFormModel form, EmailService emailService) =>
        {
            Console.WriteLine("Entered /api/contact endpoint");

            try
            {
                Console.WriteLine($"Form received: Name={form.Name}, Email={form.Email}, Message length={form.Message?.Length}");

                Console.WriteLine("About to call EmailService.SendContactFormEmail...");
                if (!string.IsNullOrEmpty(form.Message))
                    await emailService.SendContactFormEmail(form.Name, form.Email, form.Message);
                Console.WriteLine("EmailService completed successfully");

                return Results.Ok(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Contact form error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Results.Problem("Unable to send message at this time.");
            }
        });

    }
}
