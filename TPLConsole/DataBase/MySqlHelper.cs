using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace TPLConsole.DataBase
{
    /// <summary>
    /// MYSQL数据库操作辅助方法
    /// </summary>
    public class MySqlHelper
    {
        private static string connString;
        public MySqlHelper(string conn)
        {
            connString = ConfigurationManager.AppSettings[conn].ToString();
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList"></param>
        public void ExecuteSqlTran(List<string> SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                MySqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            int k = cmd.ExecuteNonQuery();
                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "[" + n + "]" + "=>" + k);
                        }
                        if (n > 0 && (n % 500 == 0 || n == SQLStringList.Count - 1))
                        {
                            tx.Commit();
                            tx = conn.BeginTransaction();
                        }
                    }
                }
                catch (Exception E)
                {
                    if (conn.State == System.Data.ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    tx.Rollback();
                    Console.WriteLine(E.Message);
                }
            }
        }

        public void ExecuteSql(string asql)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandTimeout = 60000;
                cmd.Connection = conn;
                try
                {
                    cmd.CommandText = asql;
                    int k = cmd.ExecuteNonQuery();
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "=>" + k);

                }
                catch (Exception ex)
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                    }
                    Console.WriteLine(ex.Message);
                }
            }
        }

    }
}
