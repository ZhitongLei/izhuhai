using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Diagnostics;

namespace iZhuHai.Data
{
     public class SQLHelper:IDisposable
    {
        private SqlConnection sqlConnection;
        private SqlCommand sqlCommand;
        private SqlDataAdapter DataAdapter;

        private string connectionString;
        private static string DefaultConnString =ConfigurationManager.
            ConnectionStrings["SqlConnectionString"].ConnectionString;
        //"server=.;database=stu;uid=lei;pwd=8202980"; //Integrated Security=SSPI

        private bool disposed = false;
        private bool isOpen = false;

        //异常处理的日志类
        //EventLog log = new EventLog();

        public SQLHelper():this(DefaultConnString)
        {
            this.ConnectionString= DefaultConnString;
            //CreateLog("WebApplication", "WebApplicationLog");
        }

        public SQLHelper(string connectionString)
        {
            this.ConnectionString = connectionString;
            //CreateLog("WebApplication", "WebApplicationLog");
        }

        public string ConnectionString
        {
            get { return this.connectionString; }
            set { this.connectionString = value; }
        }

        #region 公共方法
        //打开数据库连接，注意在执行SQL指令前调用此方法
        public void Open()
        {
            if (!isOpen)
            {
                sqlConnection = new SqlConnection(ConnectionString);
                try
                {
                    sqlConnection.Open();
                    sqlCommand = new SqlCommand();
                    sqlCommand.Connection = sqlConnection;
                    isOpen = true;
                }
                catch (InvalidOperationException ex)
                {
                    //log.WriteEntry(ex.Message, EventLogEntryType.FailureAudit);
                }
                catch (SqlException ex)
                {
                    //log.WriteEntry(ex.Message,EventLogEntryType.FailureAudit);
                }
            }
        }

        //关闭数据库连接
        public void Close()
        {
            Dispose(false);
        }
        #endregion
        
        #region 执行简单SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的的行数
        /// </summary>
        /// <param name="sqlCommand">SQL语句</param>
        /// <returns>影响的行数</returns>
        public  int ExecuteCommand(string CommandText)
        {
            int rows=0;
            try
            {
                this.Open(); //记得要先连接数据库
                sqlCommand.CommandText = CommandText;
                rows = sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                //log.WriteEntry(ex.Message, EventLogEntryType.Error);
            }
            return rows;
        }

        //返回查询结果的数据集
        public DataSet GetDataSet(string storeProduceName)
        {
            DataSet ds = new DataSet();
            BuildStoredProc(storeProduceName);
            DataAdapter = new SqlDataAdapter(sqlCommand);
            DataAdapter.Fill(ds);
            return ds;
        }

        //返回查询结果的数据集，带参数
        public DataSet GetDataSet(string storeProduceName,
            IEnumerable<string> parameters, IEnumerable<string> values)
        {
            DataSet ds = new DataSet();
            BuildStoredProc(storeProduceName,parameters,values);
            DataAdapter = new SqlDataAdapter(sqlCommand);
            DataAdapter.Fill(ds);
            return ds;
        }

        
        public bool Update(DataTable dt, string SelectCommand)
        {
            bool SucceendFlag = false;
            DataAdapter = new SqlDataAdapter(
                SelectCommand, sqlConnection);
            SqlCommandBuilder builder = new SqlCommandBuilder(
                DataAdapter);
            try
            {
                this.Open();
                DataAdapter.Update(dt);
                SucceendFlag = true;
            }
            catch (InvalidOperationException ex)
            {
                //log.WriteEntry(ex.Message, EventLogEntryType.Error);
            }
            catch (DBConcurrencyException ex)
            {
                //log.WriteEntry(ex.Message, EventLogEntryType.Error);
            }
            return SucceendFlag;
        }
        #endregion

        #region 执行带参数SQL语句
        public int ExecuteCommand
            (string CommandTest, params SqlParameter[] cmdParams)
        {
             int rows = 0;
             try
             {
                 AddParamters(sqlCommand,sqlConnection,null,CommandTest,cmdParams);
                 rows = sqlCommand.ExecuteNonQuery();
             }
             catch (SqlException ex)
             {
                 //log.WriteEntry(ex.Message, EventLogEntryType.Error);
             }
             return rows;
        }

        private  void AddParamters(SqlCommand cmd,SqlConnection conn,
            SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;
            if (cmdParms!=null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        #endregion

        #region 执行储存过程
        public int ExecuteStoredProcedure(string storedProcName,
            IEnumerable<string> parameters, IEnumerable<string> values)
        {
            BuildStoredProc(storedProcName, parameters, values);
            int rows = sqlCommand.ExecuteNonQuery();
            return rows;
        }

        /// <summary>
        /// 查询数据库中是否有要查询的记录
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="values">参数的实际值</param>
        /// <returns>是否有记录</returns>
        public SqlDataReader Query(string storedProcName,
            IEnumerable<string> parameters, IEnumerable<string> values)
        {
            BuildStoredProc(storedProcName, parameters,values);
            SqlDataReader dataReader;
            dataReader = sqlCommand.ExecuteReader(CommandBehavior.SingleRow);
            //return dataReader.Read();
            return dataReader;
        }
        #endregion

        #region 私有方法
        private void BuildStoredProc(string storedProcName)
        {
            this.Open();
            sqlCommand.CommandText = storedProcName;
            sqlCommand.CommandType = CommandType.StoredProcedure;
        }

        //利用AddWithValues方法构造存储过程
        private void BuildStoredProc(string storedProcName,
            IEnumerable<string> parameters,IEnumerable<string> values)
        {
            if (BuildCommand(storedProcName,parameters,values))
                sqlCommand.CommandType = CommandType.StoredProcedure;
        }

        //构造SQL命令
        private bool BuildCommand(string commandText,
            IEnumerable<string> parameters, IEnumerable<string> values)
        {
            if (parameters.Count() == values.Count())
            {
                sqlCommand.CommandText = commandText;
                sqlCommand.Parameters.Clear();

                IEnumerator<string> parameter =
                    (IEnumerator<string>)parameters.GetEnumerator();

                IEnumerator<string> value =
                    (IEnumerator<string>)values.GetEnumerator();

                while (parameter.MoveNext() && value.MoveNext())
                {
                    sqlCommand.Parameters.AddWithValue(parameter.Current, value.Current);
                }
                return true;
            }
            return false;
        }

        //创建程序的日志
        /*private void CreateLog(string source,string logName)
        {
            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source,logName);
            log.Source = source;
        }*/

        #endregion

        #region 实现IDispose接口
        //当对象不再使用时应调用此方法
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                //清理资源
                if (disposing)
                {
                    sqlCommand.Dispose();
                    DataAdapter.Dispose();
                }

                //关闭连接
                if (isOpen && sqlConnection != null)
                {
                    try
                    {
                        sqlConnection.Close();
                    }
                    catch (SqlException ex)
                    {
                        //log.WriteEntry(ex.Message);
                    }
                }
                this.disposed = true;
                isOpen = false;
            }
        }
        #endregion
    }
}
