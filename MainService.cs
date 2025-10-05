using System.Net;
using System.Net.NetworkInformation;

namespace WoLPi;

public class MainService(IConfiguration configuration)
{
    readonly int PingTimeout = configuration.GetValue<int>("PingTimeout");

    public async Task<Status> GetStatusAsync(string hostNameOrAddress)
    {
        Status status = new();
        try
        {
            using Ping pingSender = new();
            var reply = await pingSender.SendPingAsync(hostNameOrAddress, PingTimeout);
            status.IsOn = reply.Status == IPStatus.Success;
        }
        catch (PingException ex)
        {
            if (ex.InnerException?.Message == "No such host is known.")
            {
                status.Error = ex.InnerException?.Message;
            }
            else
            {
                status.Error = ex.Message;
            }
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