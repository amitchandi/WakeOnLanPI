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

        builder.Services
            .AddScoped<MainService>()
            .AddOpenApi();

        var servers = ServerConfigLoader.Load("config.yaml");
        builder.Services.AddSingleton(servers);


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.MapOpenApi();

        app.MapGet("/api/status/{HostnameOrAddress}", async (HttpContext context, MainService service, string HostnameOrAddress) =>
        {
            var status = await service.GetStatusAsync(HostnameOrAddress);
            context.Response.StatusCode = status.Error == null ? 400 : 200;
            await context.Response.WriteAsJsonAsync(status);
        });

        app.MapGet("/api/wake/{MacAddress}", async (string MacAddress, MainService service) =>
        {
            try
            {
                await service.SendWake(MacAddress);
                return "Magic Packet Sent";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Something went wrong.";
            }
        });

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
