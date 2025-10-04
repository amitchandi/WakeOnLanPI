using System.Net;
using System.Net.NetworkInformation;
using WoLPi.Components.Pages;

namespace WoLPi;

public class MainService(IConfiguration configuration)
{
    readonly string TargetIpAddress = configuration["TargetIp"] ?? throw new Exception("TargetIp is missing from config");
    readonly string TargetMacAddress = configuration["TargetMac"] ?? throw new Exception("TargetMac is missing from config");
    readonly int PingTimeout = configuration.GetValue<int>("PingTimeout");

    public async Task<Status> GetStatusAsync()
    {
        using Ping pingSender = new();
        var reply = await pingSender.SendPingAsync(TargetIpAddress, PingTimeout);
        return new()
        {
            IsOn = reply.Status == IPStatus.Success
        };
    }

    public async Task<Status> GetStatusAsync(string hostNameOrAddress)
    {
        Status status = new();
        try
        {
            using Ping pingSender = new();
            var reply = await pingSender.SendPingAsync(hostNameOrAddress, PingTimeout);
            status.IsOn = reply.Status == IPStatus.Success;
        }
        catch (Exception ex)
        {
            status.Error = ex.Message;
        }
        return status;
    }

    public async Task SendWake(string MacAddress)
    {
        try
        {
            if (PhysicalAddress.TryParse(MacAddress, out PhysicalAddress? macAddress))
            {
                await macAddress.SendWolAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public struct Status
    {
        public bool IsOn { get; set; }
        public string? Error { get; set; }
    }
}