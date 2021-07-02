using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TPLConsole.Redis;
using System.Timers;


namespace TPLConsole
{

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using MySql.Data.MySqlClient;
    using StackExchange.Redis;
    using TPLConsole.DataBase;
    using TPLConsole.RedisDemo;

    namespace RedisDemo
    {
        public class RedisConnectHelper : IDisposable
        {

            private string _connectionString;

            private string _instanceName;

            private int _defaultDB;

            private ConcurrentDictionary<string, ConnectionMultiplexer> _connections;

            public RedisConnectHelper(string connectionString, string instanceName, int defaultDB = 0)
            {
                _connectionString = connectionString;
                _instanceName = instanceName;
                _defaultDB = defaultDB;
                _connections = new ConcurrentDictionary<string, ConnectionMultiplexer>();
            }

            /// <summary>
            /// 获取ConnectionMultiplexer
            /// </summary>
            /// <returns></returns>
            private ConnectionMultiplexer GetConnect()
            {
                return _connections.GetOrAdd(_instanceName, p => ConnectionMultiplexer.Connect(_connectionString));
            }

            /// <summary>
            /// 获取数据库
            /// </summary>
            /// <returns></returns>
            public IDatabase GetDatabase()
            {
                return GetConnect().GetDatabase(_defaultDB);
            }

            public IServer GetServer(string configName = null, int endPointIndex = 0)
            {
                var confOption = ConfigurationOptions.Parse(_connectionString);
                return GetConnect().GetServer(confOption.EndPoints[endPointIndex]);
            }

            public ISubscriber GetSubscriber(string configName = null)
            {
                return GetConnect().GetSubscriber();
            }



            public void Dispose()
            {
                if (_connections != null && _connections.Count > 0)
                {
                    foreach (var item in _connections.Values)
                    {
                        item.Close();
                    }
                }
            }
        }
    }



    class Program
    {

        /// <summary>
        /// 获取站点（设备）最新监测数据(建筑工地)
        /// </summary>
        private const string getdevicerealdata = "http://115.227.32.36:8823/common/getdevicerealdata";
        static RedisHelperOptions _redisHelperOptions = new RedisHelperOptions {
            //ConnectionString = "127.0.0.1:6379,connectTimeout=1000,connectRetry=1,syncTimeout=10000,allowadmin=true",
            ConnectionString = "192.168.0.202:6379,allowadmin=true",
            DbNumber = 10
        };
        static IRedisHelper _redisClient = null;


        static string _connectionString = "127.0.0.1:6379,connectTimeout=1000,connectRetry=1,syncTimeout=10000,allowadmin=true";
        static string _instanceName = "local";
        static int _defaultDB = 10;
        static RedisConnectHelper _redisConnect = null;
        static IDatabase _redis = null;
        static Stopwatch sw = null;

        static void Main(string[] args)
        {
            _redisClient = new RedisHelper(new RedisHelperOptions {
                ConnectionString = "127.0.0.1:6379,connectTimeout=1000,connectRetry=1,syncTimeout=10000,allowadmin=true",
                DbNumber = 1
            });

            _redisConnect = new RedisConnectHelper(_connectionString, _instanceName, _defaultDB);
            _redis = _redisConnect.GetDatabase();

            #region "开始任务"
            sw = new Stopwatch();
            sw.Start();
            sw.Reset();
            sw.Restart();
            #endregion "开始任务"



            JobTask();


            //定时
            //System.Timers.Timer timer = new System.Timers.Timer();
            //timer.Enabled = true;
            //timer.Interval = 60000; //执行间隔时间,单位为毫秒; 这里实际间隔为1分钟  
            //timer.Start();
            //timer.Elapsed += Timer_Elapsed;




            #region "结束任务"
            sw.Stop();
            //获取运行时间间隔  
            TimeSpan ts = sw.Elapsed;
            //获取运行时间[毫秒]  
            long times = sw.ElapsedMilliseconds;
            //获取运行的总时间  
            long times2 = sw.ElapsedTicks;
            //判断计时是否正在进行[true为计时]  
            bool isrun = sw.IsRunning;
            //获取计时频率  
            long frequency = Stopwatch.Frequency;
            AnsiConsole.Markup("[red]{0}[/]", Markup.Escape($@"程序结束:{DateTime.Now}" + "\r\n"));
            AnsiConsole.Markup("[red]{0}[/]", Markup.Escape($@"耗时: {times / 1000} 秒" + "\r\n"));
            AnsiConsole.Markup("[blue]{0}[/]", Markup.Escape($@"任务运行完成；等待下次运行" + "\r\n"));
            Console.ReadKey();
            #endregion "结束任务"
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            JobTask();
        }

        private static void JobTask()
        {
            AnsiConsole.Markup("[red]{0}[/]", Markup.Escape($@"程序运行:{DateTime.Now}" + "\r\n"));


            string resStr = ApiHelper.HttpGet(getdevicerealdata);
            if (resStr == null)
            {
                return;
            }
            else
            {


                var response = JsonHelper.TryDeserializeObject<OutBaseDto<List<GetDeviceRealDataOutDto>>>(resStr);
                if (response?.code == 0)
                {
                    var sourceCollection = response.data;

                    AnsiConsole.Markup("[red]{0}[/]", Markup.Escape($@"获取:{sourceCollection?.Count} 条数据" + "\r\n"));



                    #region "生成sql语句"
                    int _counter = 0;
                    StringBuilder sqlBuilder = new StringBuilder();
                    sqlBuilder.AppendLine($@"
                            INSERT INTO 
                            `dust`.`dustinfo_today_auto` 
                            (`id`, `mncode`, `data_time`, `upload_time`, `dust`, `dust_flag`, `temperature`, `temp_flag`, `humidity`, `hum_flag`, `wind_speed`, `wind_speed_flag`, `atm`, `atm_flag`, `wind_dir`, `wind_dir_flag`, `pm25`, `pm25_flag`, `ref_dust`, `OperationCompanyName`, `DeviceModel`, `scene`, `DistrictName`, `ProjectName`, `ref_error`, `qualify`, `standard`) 
                            VALUES ");


                    StringBuilder sqlBuilder_History = new StringBuilder();
                    sqlBuilder_History.AppendLine($@"
                            INSERT INTO 
                            `dust`.`dustinfo_history_auto` 
                            (`id`, `mncode`, `data_time`, `upload_time`, `dust`, `dust_flag`, `temperature`, `temp_flag`, `humidity`, `hum_flag`, `wind_speed`, `wind_speed_flag`, `atm`, `atm_flag`, `wind_dir`, `wind_dir_flag`, `pm25`, `pm25_flag`, `ref_dust`, `OperationCompanyName`, `DeviceModel`, `scene`, `DistrictName`, `ProjectName`, `ref_error`, `qualify`, `standard`) 
                            VALUES ");


                    List<string> newSql = new List<string>();
                    List<string> historySql = new List<string>();

                    #endregion "生成sql语句"

                    //顺序执行
                    foreach (var item in sourceCollection)
                    {
                        //生成合并sql语句//
                        _counter++;
                        var _sql = CreateRawSql_Value(item);
                        if (_counter == 1)
                        {
                            sqlBuilder.AppendLine(_sql);
                            sqlBuilder_History.AppendLine(_sql);
                        }
                        else
                        {
                            sqlBuilder.AppendLine(" , " + _sql);
                            sqlBuilder_History.AppendLine(" , " + _sql);
                        }



                        //生成单条sql语句
                        newSql.Add(CreateRawSingleSqlWithInsert(item, "dustinfo_today_auto"));
                        historySql.Add(CreateRawSingleSqlWithInsert(item, "dustinfo_history_auto"));

                        //生成sql语句 结束

                        //ProcessData(item);
                    }

                    /**
                     * https://docs.microsoft.com/zh-cn/dotnet/standard/parallel-programming/data-parallelism-task-parallel-library
                     * 并行循环运行时，TPL 将数据源进行分区，以便该循环可以同时对多个部分进行作用。
                     * 在后台，任务计划程序基于系统资源和工作负荷来划分任务。 
                     * 如有可能，如果工作负荷变得不平衡了，计划程序将重新分配多个线程与处理器之间的工作。
                     **/
                    var db = new TPLConsole.DataBase.MySqlHelper("Dust");

                    //并行处理
                    Parallel.ForEach(sourceCollection, item =>
                    {
                        // error code: 并行处理中不要有外部变量参与

                        //处理数据
                        ProcessData(item);

                    });

                    //============写入数据库2============//



                    //============写入数据库============//



                    #region "合并数据"

                    //顺序执行
                    db.ExecuteSql(sqlBuilder.ToString());
                    //db.ExecuteSql(sqlBuilder_History.ToString());

                    //开启线程处理
                    //Task.Run(() =>
                    //{
                    //    db.ExecuteSql(sqlBuilder.ToString());
                    //});
                    //Task.Run(() =>
                    // {
                    //     db.ExecuteSql(sqlBuilder_History.ToString());
                    // });

                    #endregion "合并数据"



                    #region "单条数据组合为List<string>集合"
                    //顺序执行
                    //db.ExecuteSqlTran(newSql);//67秒//58秒
                    //db.ExecuteSqlTran(historySql);


                    //开启线程处理
                    //Task.Run(() =>
                    //{
                    //    db.ExecuteSqlTran(newSql);
                    //});
                    //Task.Run(() =>
                    // {
                    //     db.ExecuteSqlTran(historySql);
                    // });
                    #endregion "单条数据组合为List<string>集合"

                }
                else
                {
                    //error no data
                }



            }
        }

        /// <summary>
        /// 处理数据，
        /// </summary>
        /// <param name="item"></param>
        private static void ProcessData(GetDeviceRealDataOutDto item)
        {
            AnsiConsole.Render(new Markup("[bold yellow]正在执行[/] [red]……![/]" + "\r\n"));

            #region "异常判定"
            string key = "prev_" + item.mnCode;
            var _obj = new {
                item.mnCode,
                item.dataTime,
                item.dust
            };
            //读取上一次
            var prev_content = _redis.StringGet(key);/*_redisClient.StringGet<string>(key);*/

            if (prev_content == _obj.ToJson())
            {
                AnsiConsole.Render(new Markup("[bold red]haha == haha [/] [red]![/]" + "\r\n"));
            }

            //写入redis 记为上一次数据
            _redis.StringSet(key, _obj.ToJson(), TimeSpan.FromSeconds(70));
            #endregion "异常判定"

            // todo  输出 异常数据 sql 

        }

        /// <summary>
        /// 生成sql语句 只带Values 后面的部分 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static string CreateRawSql_Value(GetDeviceRealDataOutDto item)
        {
            return $@" 
                    ('{Guid.NewGuid()}', '{item.mnCode}', '{item.dataTime.ToString("yyyy-MM-dd HH:mm:ss")}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', {item.dust}, '{item.dustFlag}', {item.temperature}, '{item.temperatureFlag}', {item.humidity}, '{item.humidityFlag}',{item.windSpeed}, '{item.windSpeedFlag}', {item.atm}, '{item.atmFlag}', {item.windDirection}, '{item.windDirectionFlag}', {-1}, NULL, {-1}, '{"品牌字段,工地接口没有，需要另外查询"}', '{"DeviceModel-工地接口没有，需要另外查询"}', '工地', '{"区域字段没有 "}', '{item.siteName}', 0, 0, 0) 
             ";
        }

        /// <summary>
        /// 单条sql
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static string CreateRawSingleSqlWithInsert(GetDeviceRealDataOutDto item, string table = "dustinfo_today_auto")
        {
            var d = $@" INSERT INTO 
                            `dust`.`{table}` 
                            (`id`, `mncode`, `data_time`, `upload_time`, `dust`, `dust_flag`, `temperature`, `temp_flag`, `humidity`, `hum_flag`, `wind_speed`, `wind_speed_flag`, `atm`, `atm_flag`, `wind_dir`, `wind_dir_flag`, `pm25`, `pm25_flag`, `ref_dust`, `OperationCompanyName`, `DeviceModel`, `scene`, `DistrictName`, `ProjectName`, `ref_error`, `qualify`, `standard`) 
                            VALUES ";
            var d2 = $@" 
                    ('{Guid.NewGuid()}', '{item.mnCode}', '{item.dataTime}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', {item.dust}, '{item.dustFlag}', {item.temperature}, '{item.temperatureFlag}', {item.humidity}, '{item.humidityFlag}',{item.windSpeed}, '{item.windSpeedFlag}', {item.atm}, '{item.atmFlag}', {item.windDirection}, '{item.windDirectionFlag}', {-1}, NULL, {-1}, '{"品牌字段,工地接口没有，需要另外查询"}', '{"DeviceModel-工地接口没有，需要另外查询"}', '工地', '{"区域字段没有 "}', '{item.siteName}', 0, 0, 0) 
             ";

            return d + d2;
        }
    }
}
