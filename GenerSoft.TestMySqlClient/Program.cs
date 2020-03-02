using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace GenerSoft.TestMySqlClient
{
    public class testclass
    {
        public static void testGetData()
        {
            // 数据库配置
            string connStr = "Database=DeviceMonitor;datasource=172.22.15.132;port=10065;user=root;pwd=root!2018;SslMode = none;";
            MySqlConnection conn = new MySqlConnection(connStr);

            //设置查询命令
            MySqlCommand cmd = new MySqlCommand("select * from U_User", conn);
            //查询结果读取器
            MySqlDataReader reader = null;

            try
            {
                //打开连接
                conn.Open();
                Console.WriteLine("连接成功!!!");
                //执行查询，并将结果返回给读取器
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string username = reader.GetString("Name");
                    string password = reader.GetString("PassWord");
                    Console.WriteLine(username + ":" + password);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            finally
            {
                reader.Close();
                conn.Close();
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            testclass.testGetData();
        }
    }
}
