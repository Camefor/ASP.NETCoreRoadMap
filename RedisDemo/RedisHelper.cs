using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisDemo {
    public class RedisHelper : IDisposable {

        private string _connectionString;

        private string _instanceName;

        private int _defaultDB;

        private ConcurrentDictionary<string, ConnectionMultiplexer> _connections;

        public RedisHelper(string connectionString, string instanceName, int defaultDB = 0) {
            _connectionString = connectionString;
            _instanceName = instanceName;
            _defaultDB = defaultDB;
            _connections = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        }

        /// <summary>
        /// 获取ConnectionMultiplexer
        /// </summary>
        /// <returns></returns>
        private ConnectionMultiplexer GetConnect() {
            return _connections.GetOrAdd(_instanceName, p => ConnectionMultiplexer.Connect(_connectionString));
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <returns></returns>
        public IDatabase GetDatabase() {
            return GetConnect().GetDatabase(_defaultDB);
        }

        public IServer GetServer(string configName = null, int endPointIndex = 0) {
            var confOption = ConfigurationOptions.Parse(_connectionString);
            return GetConnect().GetServer(confOption.EndPoints[endPointIndex]);
        }

        public ISubscriber GetSubscriber(string configName = null) {
            return GetConnect().GetSubscriber();
        }




        public void Dispose() {
            if (_connections != null && _connections.Count > 0) {
                foreach (var item in _connections.Values) {
                    item.Close();
                }
            }
        }
    }
}
