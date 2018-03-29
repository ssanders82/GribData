using System;
using System.Reflection;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GribData
{
    public abstract class DataClass
    {
        /// <summary>
        /// Required for New, Read Only for Existing: The unique record identifier for the object.  Must be set to 0 for new record inserts.
        /// </summary>
        public int ID { get; set; }
        public DataRow _Data;

        public DataClass()
        {
        }

        public DataClass(int inID)
        {
            GetObject(inID);
        }

        public DataClass(DataRow dr)
        {
            Utilities.LoadObjectMembers(this, dr);
            _Data = dr;
        }

        public virtual string getTableName()
        {
            return GetType().Name;
        }

        public override string ToString()
        {
            return string.Format("ID {0}", ID);
        }

        /// <summary>
        /// http://stackoverflow.com/questions/1380087/whats-the-correct-alternative-to-static-method-inheritance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        protected static List<T> GetList<T>(string filter = "", string orderBy = "ID ASC") where T : DataClass, new()
        {
            List<T> lst = new List<T>();
            string sql = "SELECT * from `" + typeof(T).Name + "`";
            if (filter != "") sql += " WHERE (" + filter + ")";
            sql += " ORDER BY " + orderBy;
            DataTable dt = Data.Select(sql);
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                Utilities.LoadObjectMembers(t, dr);
                t._Data = dr;
                lst.Add(t);
            }
            return lst;
        }

        public static T Get<T>(string filter = "") where T : DataClass, new()
        {
            return GetList<T>(filter).FirstOrDefault();
        }

        public static T Get<T>(int inID) where T : DataClass, new()
        {
            return Get<T>("ID=" + inID);
        }

        public bool GetObject(int inID)
        {
            string cmd = "SELECT * FROM " + getTableName() + " WHERE ID = " + inID;
            DataTable dt = Data.Select(cmd);
            if (dt.Rows.Count == 0) return false;

            Utilities.LoadObjectMembers(this, dt.Rows[0]);
            _Data = dt.Rows[0];
            return true;
        }

        public List<PropertyInfo> PropsToUse()
        {
            PropertyInfo[] props = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<PropertyInfo> propsToUse = new List<PropertyInfo>();
            foreach (PropertyInfo p in props)
            {
                if (!p.Name.StartsWith("_") && p.Name != "ID") propsToUse.Add(p);
            }
            return propsToUse;
        }
        /*
        public List<string> GetColumns(string tableName)
        {
            List<string> lst = new List<string>();
            DataTable dt = Data.Select("describe " + tableName);
            foreach (DataRow dr in dt.Rows)
            {
            }
            return lst;
        }
        */

        /// <summary>
        /// Create a row to be used in mysql load data infile
        /// http://dev.mysql.com/doc/refman/5.1/en/load-data.html
        /// </summary>
        /// <returns></returns>
        public string GetMysqlDataRowForFile()
        {
            StringBuilder sb = new StringBuilder();
            List<PropertyInfo> propsToUse = PropsToUse();

            for (int i = 0; i < propsToUse.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append("\t");
                }
                sb.Append(Utilities.GetStringRepresentation(this, propsToUse[i], false));
            }
            return sb.ToString();
        }

        public virtual void Delete()
        {
            if (ID == 0) return;
            string sql = "DELETE FROM " + getTableName() + " WHERE ID=" + ID;
            Data.Execute(sql);
        }

        public int Save()
        {
            Dictionary<string, object> vals = new Dictionary<string, object>();

            List<PropertyInfo> propsToUse = PropsToUse();

            StringBuilder sql = new StringBuilder();
            if (ID == 0)
            {
                sql.Append("INSERT INTO " + getTableName() + " (");
                StringBuilder sb1 = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();

                for (int i = 0; i < propsToUse.Count; i++)
                {
                    if (i > 0)
                    {
                        sb1.Append(",");
                        sb2.Append(",");
                    }
                    sb1.Append(propsToUse[i].Name);
                    sb2.Append(Utilities.GetStringRepresentation(this, propsToUse[i]));
                }
                sql.Append(sb1.ToString());
                sql.Append(" ) VALUES (");
                sql.Append(sb2.ToString());
                sql.Append(")");

                ID = Data.Insert(sql.ToString());
            }
            else
            {
                sql.Append("UPDATE " + getTableName() + " SET ");
                for (int i = 0; i < propsToUse.Count; i++)
                {
                    if (i > 0)
                    {
                        sql.Append(",");
                    }
                    sql.Append(propsToUse[i].Name + "=" + Utilities.GetStringRepresentation(this, propsToUse[i]));
                }
                sql.Append(" WHERE ID=" + ID);
                Data.Execute(sql.ToString());
            }

            return ID;
        }
    }

}
