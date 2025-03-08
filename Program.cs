using System.Net.NetworkInformation;
using System.Text;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

string targetIp = app.Configuration["TargetIp"] ?? throw new Exception("TargetIp is missing from config");
string targetMac = app.Configuration["TargetMac"] ?? throw new Exception("TargetMac is missing from config");

app.MapGet("/status", async (context) =>
{
    // return some information about status of the server(s)
    Ping pingSender = new Ping();
    PingOptions options = new PingOptions();

    // Use the default Ttl value which is 128,
    // but change the fragmentation behavior.
    options.DontFragment = true;

    // Create a buffer of 32 bytes of data to be transmitted.
    string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
    byte[] buffer = Encoding.ASCII.GetBytes(data);
    int timeout = 120;
    PingReply reply = pingSender.Send(targetIp, timeout, buffer, options);

    var resp = new
    {
        IsOn = reply.Status == IPStatus.Success
    };

    await context.Response.WriteAsJsonAsync(resp);
})
.WithName("GetStatus");

app.MapGet("/wake", () =>
{
    Process.Start($"wakeonlan", $"{targetMac}");
    return "Done";
})
.WithName("Wake");

app.Run();
