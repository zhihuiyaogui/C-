using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Text;

namespace Common.Config
{
    /// <summary>
    /// 使用拦截器记录SQL执行语句
    /// 另一种方法context.Database.Log = log.Info; 可是记录SQL执行的详细信息
    /// </summary>
    public class CustomEFInterceptor : IDbCommandInterceptor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static readonly ConcurrentDictionary<DbCommand, DateTime> MStartTime = new ConcurrentDictionary<DbCommand, DateTime>();

        //记录开始执行时的时间
        private static void OnStart(DbCommand command)
        {
            MStartTime.TryAdd(command, DateTime.Now);
        }


        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            WriteLog(command, interceptionContext);
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            OnStart(command);
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            WriteLog(command, interceptionContext);
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            OnStart(command);
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            WriteLog(command, interceptionContext);
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            OnStart(command);
        }

        private void WriteLog<T>(DbCommand command, DbCommandInterceptionContext<T> interceptionContext)
        {
            DateTime startTime;
            TimeSpan duration;
            //得到此command的开始时间
            MStartTime.TryRemove(command, out startTime);
            if (startTime != default(DateTime))
            {
                duration = DateTime.Now - startTime;
            }
            else
                duration = TimeSpan.Zero;

            var parameters = new StringBuilder();
            //循环获取执行语句的参数值
            foreach (DbParameter param in command.Parameters)
            {
                parameters.AppendLine(param.ParameterName + " " + param.DbType + " = " + param.Value);
            }

            //判断语句是否执行时间超过1秒或是否有错
            if (duration.TotalSeconds > 1 || interceptionContext.Exception != null)
            {
                log.WarnFormat("[SQL]:IsAsync: {0},Cost {1}s, Command Text: {2}, Parameters: {3}", interceptionContext.IsAsync, duration.TotalSeconds, command.CommandText, parameters);
            }
            else
            {
                //记录普通SQL语句
                log.DebugFormat("[SQL]:IsAsync: {0},Cost {1}s, Command Text: {2}, Parameters: {3}", interceptionContext.IsAsync, duration.TotalSeconds, command.CommandText, parameters);
            }
        }

    }
}
