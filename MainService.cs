using System.Net.NetworkInformation;
using MagicPacket;

namespace WoLPi;

public class MainService(IConfiguration configuration)
{
    readonly string TargetIpAddress = configuration["TargetIp"] ?? throw new Exception("TargetIp is missing from config");
    readonly string TargetMacAddress = configuration["TargetMac"] ?? throw new Exception("TargetMac is missing from config");
    readonly int PingTimeout = 120;

    public async Task<Status> GetStatusAsync()
    {
        // return some information about status of the server(s)
        Ping pingSender = new();
        var reply = await pingSender.SendPingAsync(TargetIpAddress, PingTimeout);
        return new()
        {
            IsOn = reply.Status == IPStatus.Success
        };
    }

    public async Task SendWake()
    {
        MagicPacketClient client = new();
        await client.BroadcastOnAllInterfacesAsync(TargetMacAddress);
    }

    public struct Status
    {
        public bool IsOn { get; set; }
    }
}
