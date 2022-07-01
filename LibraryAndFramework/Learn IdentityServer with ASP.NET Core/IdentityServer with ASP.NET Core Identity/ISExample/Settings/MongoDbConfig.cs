namespace ISExample.Settings
{
    public class MongoDbConfig
    {
        public string? Name { get; set; }
        public string? Host { get; set; }
        public int Port { get; set; }
        public string ConnectionString => $"mongodb://{Host}:{Port}";
    }
}
