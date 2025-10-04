using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace WoLPi;

public static class ServerConfigLoader
{
    public static List<Server> Load(string path = "config.yaml")
    {
        if (!File.Exists(path))
            return [];

        var yaml = File.ReadAllText(path);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();

        return deserializer.Deserialize<List<Server>>(yaml) ?? [];
    }

    public static void Save(List<Server> servers, string path = "config.yaml")
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        var yaml = serializer.Serialize(servers);
        File.WriteAllText(path, yaml);
    }
}
