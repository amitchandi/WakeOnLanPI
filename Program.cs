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

        builder.Services.AddScoped<MainService>();

        builder.Services.AddSingleton<ServerConfigService>();

        builder.Services.AddSwaggerGen();
        builder.Services.AddEndpointsApiExplorer();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.MapGet("/api/status/{HostnameOrAddress}", async (HttpContext context, MainService service, string HostnameOrAddress) =>
        {
            var status = await service.GetStatusAsync(HostnameOrAddress);
            context.Response.StatusCode = status.Error == null ? 400 : 200;
            await context.Response.WriteAsJsonAsync(status);
        })
        .WithDescription("Ping {HostnameOrAddress} for status.")
        .Produces(200, typeof(MainService.Status));

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
        })
        .WithDescription("Send Wake on Lan magic packet to {MacAddress}.")
        .Produces(200, typeof(string));

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
