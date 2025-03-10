using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Timers;

namespace WoLPi;

public class MainService : IDisposable
{
    IConfiguration configuration { get; set; }
    string TargetIpAddress;
    string TargetMacAddress;

    public System.Timers.Timer Timer { get; set; }

    public MainService(IConfiguration configuration)
    {
        this.configuration = configuration;
        TargetIpAddress = configuration["TargetIp"] ?? throw new Exception("TargetIp is missing from config");
        TargetMacAddress = configuration["TargetMac"] ?? throw new Exception("TargetMac is missing from config");
        Timer = new System.Timers.Timer(10 * 1000);
        Timer.Start();
    }

    public Status GetStatus()
    {
        // return some information about status of the server(s)
        Ping pingSender = new();
        int timeout = 120;
        PingReply reply = pingSender.Send(TargetIpAddress, timeout);

        return new()
        {
            IsOn = reply.Status == IPStatus.Success
        };
    }

    public void SendWake()
    {
        Process.Start($"wakeonlan", $"{TargetMacAddress}");
    }

    public struct Status
    {
        public bool IsOn { get; set; }
    }

    public void Dispose()
    {
        Timer.Dispose();
    }
}
