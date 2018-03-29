using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Data;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Threading;
using System.Xml;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace GribData
{
    public static class Data
    {
        public static bool LogQueries = false;

        public static string DBServer
        {
            get { return ConfigurationManager.AppSettings["LocalDBServer"]; }
        }

        public static string DBUser
        {
            get { return ConfigurationManager.AppSettings["LocalDBUser"]; }
        }

        public static string DBPwd
        {
            get { return ConfigurationManager.AppSettings["LocalDBPwd"]; }
        }

        public static string DBName
        {
            get { return ConfigurationManager.AppSettings["LocalDBName"]; }
        }

        public static int DBPort
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["LocalDBPort"]); }
        }


        public static string ConnectionString
        {
            get
            {
                string connString = ConfigurationManager.ConnectionStrings["LocalConnectionString"].ConnectionString;
                connString = string.Format(connString,
                    DBServer,
                    DBUser,
                    DBPwd,
                    DBName,
                    DBPort);
                return connString;
            }
        }


        public static int Insert(string cmd)
        {
            int ID = 0;
            var command = new MySqlCommand();
            command.CommandText = cmd;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    command.Connection = conn;
                    command.ExecuteNonQuery();

                    //command.CommandText = "select last_insert_id()";
                    //var o = command.ExecuteScalar();
                    ID = Convert.ToInt32(command.LastInsertedId);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return ID;
        }


        public static void Execute(string cmd)
        {
            var command = new MySqlCommand();
            command.CommandText = cmd;
            DataSet ds = new DataSet();
            try
            {
                
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    command.Connection = conn;
                    using (MySqlDataAdapter da = new MySqlDataAdapter())
                    {
                        da.UpdateCommand = command;
                        da.UpdateCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //c.Close();
            }
        }

        public static DataTable Select(MySqlCommand command)
        {
            DataSet ds = new DataSet();
            Stopwatch watch = new Stopwatch();
            watch.Start();

            MySqlConnection conn = new MySqlConnection(ConnectionString);
            command.Connection = conn;
            using (conn)
            {
                conn.Open();
                using (MySqlDataAdapter da = new MySqlDataAdapter())
                {
                    da.SelectCommand = command;
                    ds = new DataSet();
                    da.Fill(ds);
                }
            }
            watch.Stop();

            return ds.Tables[0];
        }

        public static DataTable Select(string select_cmd)
        {
            select_cmd = select_cmd.Replace("\n", " ");
            select_cmd = select_cmd.Replace("\r", " ");
            MySqlCommand command = new MySqlCommand();
            command.CommandText = select_cmd;
            return Select(command);
        }

        public static object SelectField(string sql)
        {
            DataTable dt = Select(sql);
            if (dt.Rows.Count == 1 && dt.Rows[0][0] != DBNull.Value)
            {
                return dt.Rows[0][0];
            }
            else
            {
                return null;
            }
        }

    }


}
