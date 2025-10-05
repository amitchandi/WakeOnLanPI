namespace WoLPi;

public class ServerCardInfo
{
    public int Index { get; set; } = 0;
    public required Server? server { get; set; }
    public bool IsLoading { get; set; } = true;
    public bool IsOn { get; set; } = false;
    public bool IsValid { get; set; } = true;
}
