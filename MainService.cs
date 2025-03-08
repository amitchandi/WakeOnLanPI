using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using WoLPi.Components;

namespace WoLPi;

public class MainService
{
    IConfiguration configuration { get; set; }
    string TargetIpAddress;
    string TargetMacAddress;

    public MainService(IConfiguration configuration)
    {
        this.configuration = configuration;
        TargetIpAddress = configuration["TargetIp"] ?? throw new Exception("TargetIp is missing from config");
        TargetMacAddress = configuration["TargetMac"] ?? throw new Exception("TargetMac is missing from config");
    }

    public Status GetStatus()
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
        PingReply reply = pingSender.Send(TargetIpAddress, timeout, buffer, options);

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
}
