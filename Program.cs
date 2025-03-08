using WoLPi.Components;

namespace WoLPi;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddSingleton<MainService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.MapGet("/api/status", async (HttpContext context, MainService service) =>
        {
            await context.Response.WriteAsJsonAsync(service.GetStatus());
        });

        app.MapGet("/api/wake", (MainService service) =>
        {
            service.SendWake();
            return "Magic Packet Sent";
        });

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
