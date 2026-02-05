namespace MySQL_Master_to_Slave.DataBase
{
    /// <summary>
    /// 定义数据库操作: 读or写
    /// </summary>
    public enum DatabaseIntent { Write, Read }

    public interface IDatabaseIntentService
    {
        DatabaseIntent Intent { get; set; }
    }

    /// <summary>
    /// 在 EF Core 决定打开连接的那一刻，根据设置的连接意图（读或写）来提供正确的物理连接。
    /// </summary>
    public class DatabaseIntentService : IDatabaseIntentService
    {
        //使用 AsyncLocal 来确保连接意图在异步调用链中正确传递。
        private readonly AsyncLocal<DatabaseIntent> _intent = new();
        public DatabaseIntent Intent { get => _intent.Value; set => _intent.Value = value; }
    }
}
