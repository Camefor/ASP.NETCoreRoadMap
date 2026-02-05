using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MySQL_Master_to_Slave.DataBase
{
    public static class DbContextExtensions
    {
        public static MyDbContext AsReadOnly(this MyDbContext context, IDatabaseIntentService intentService)
        {
            intentService.Intent = DatabaseIntent.Read;
            return context;
        }
    }

    public class MyDbContext : DbContext
    {
        private readonly IDatabaseIntentService _intentService;
        private readonly string _masterConn;
        private readonly string _slaveConn;

        public MyDbContext(
        DbContextOptions<MyDbContext> options,
        IDatabaseIntentService intentService,
        IConfiguration config) : base(options)
        {
            _intentService = intentService;
            _masterConn = config.GetConnectionString("MasterConnection");
            _slaveConn = config.GetConnectionString("SlaveConnection");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // 只有在 Options 还没配置时才进入
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _intentService.Intent == DatabaseIntent.Read
                                       ? _slaveConn
                                       : _masterConn;

                // 调试打印，看看它到底什么时候跑的
                Console.WriteLine($"[DB_DEBUG] Using Connection: {(connectionString == _slaveConn ? "SLAVE" : "MASTER")}");

                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
        }


        // 定义数据表
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Log>().ToTable("log");
        }
    }

    //sample demo entity

    [Table("log")]
    public class Log
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("LogLevelId")]
        public int LogLevelId { get; set; }

        [Required]
        [Column("ShortMessage", TypeName = "longtext")]
        public string ShortMessage { get; set; }

        [Column("FullMessage", TypeName = "longtext")]
        public string? FullMessage { get; set; }

        [StringLength(200)]
        [Column("IpAddress")]
        public string? IpAddress { get; set; }

        [Column("UserId")]
        public int? UserId { get; set; }

        [Column("PageUrl", TypeName = "longtext")]
        public string? PageUrl { get; set; }

        [Column("ReferrerUrl", TypeName = "longtext")]
        public string? ReferrerUrl { get; set; }

        [Required]
        [Column("CreateTime")]
        public DateTime CreateTime { get; set; }
    }
}
