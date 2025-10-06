using System.Collections.ObjectModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace WoLPi;

public class ServerConfigService : IDisposable
{
    private readonly string _configPath;
    private readonly IDeserializer _deserializer;
    private readonly ISerializer _serializer;
    private FileSystemWatcher? _watcher;
    private bool _isReloading;

    public ObservableCollection<Server> Servers { get; private set; } = [];

    public event Action? OnConfigChanged;

    public ServerConfigService(IWebHostEnvironment env)
    {
        _configPath = Path.Combine(env.ContentRootPath, "config.yaml");

        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        _serializer = new SerializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();

        Load();
        StartWatcher();
    }

    private void StartWatcher()
    {
        var dir = Path.GetDirectoryName(_configPath)!;
        var file = Path.GetFileName(_configPath)!;

        _watcher = new FileSystemWatcher(dir)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
            Filter = file
        };

        _watcher.Changed += (_, _) => OnFileChanged();
        _watcher.EnableRaisingEvents = true;
    }

    public void Load()
    {
        if (!File.Exists(_configPath))
        {
            Servers = new();
            Save(); // create empty file
            return;
        }

        using var reader = File.OpenText(_configPath);
        var servers = _deserializer.Deserialize<List<Server>>(reader) ?? new List<Server>();

        // Replace contents so UI binding stays intact
        if (Servers.Count > 0)
            Servers.Clear();

        foreach (var s in servers)
            Servers.Add(s);
    }

    public void Save()
    {
        var yaml = _serializer.Serialize(Servers);
        File.WriteAllText(_configPath, yaml);
    }

    private void OnFileChanged()
    {
        if (_isReloading) return;
        
        _isReloading = true;
        try
        {
            Thread.Sleep(200); // allow file write to finish
            Load();
            OnConfigChanged?.Invoke();
        }
        finally
        {
            _isReloading = false;
        }
    }

    public void Dispose()
    {
        _watcher?.Dispose();
    }
}