using System;
using System.IO;
//using System.Net.FtpClient;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;

namespace GribData
{
    public static class Utilities
    {
        public static double CalulateWindSpeed(double ucomp, double vcomp)
        {
            if (ucomp == 0 && vcomp == 0) return 0;
            double speed = Math.Pow((ucomp * ucomp) + (vcomp * vcomp), 0.5);
            return speed;
        }

        /// <summary>
        /// http://www.cactus2000.de/uk/unit/masswin.shtml
        /// https://answers.yahoo.com/question/index?qid=20130910172911AAKdpmt
        /// </summary>
        /// <param name="ucomp"></param>
        /// <param name="vcomp"></param>
        /// <returns></returns>
        public static double CalulateWindDirection(double ucomp, double vcomp)
        {
            if (vcomp == 0)
            {
                if (ucomp == 0) return 0;
                else if (ucomp > 0) return 270; // from west
                else return 90; // from east
            }
            double radians = Math.Atan(ucomp / vcomp);
            double degrees = radians * 180 / Math.PI;
            if (vcomp > 0) degrees += 180;
            if (degrees < 0) degrees += 360;
            
            return degrees;
        }

        public static double CalculateMedian(List<double> arrNums)
        {
            if (arrNums.Count == 0) return 0;
            arrNums.Sort();
            if (arrNums.Count % 2 == 1)
            {
                int index = (arrNums.Count - 1) / 2;
                return Convert.ToDouble(arrNums[index]);
            }
            else if (arrNums.Count > 0)
            {
                int index1 = (arrNums.Count - 2) / 2;
                int index2 = (arrNums.Count) / 2;
                return (Convert.ToDouble(arrNums[index1]) * 0.5) + (Convert.ToDouble(arrNums[index2]) * 0.5);
            }
            else return 0;
        }

        /*
        public static bool DownloadFile(this FtpClient ftpClient, string srcName, string destinationPath)
        {
            using (var ftpStream = ftpClient.OpenRead(srcName))
            using (var fileStream = File.Create(destinationPath, (int)ftpStream.Length))
            {
                var buffer = new byte[8 * 1024];
                int count;
                while ((count = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileStream.Write(buffer, 0, count);
                }

                // In this example, we're deleting the file after downloading it.
                //ftpClient.DeleteFile(ftpListItem.FullName);
            }
            return true;
        }*/

        public static void LoadObjectMembers(object obj, DataRow dr)
        {
            PropertyInfo[] props = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in props)
            {
                try
                {
                    if (!dr.Table.Columns.Contains(p.Name)) continue;
                    object val = dr[p.Name];
                    if (val != DBNull.Value)
                    {
                        if (p.PropertyType.IsEnum) val = Enum.Parse(p.PropertyType, val.ToString());
                        if (p.PropertyType == typeof(bool) || p.PropertyType == typeof(bool?)) val = Convert.ToBoolean(val);
                        else if (p.PropertyType == typeof(Int32) || p.PropertyType == typeof(Int32?)) val = Convert.ToInt32(val);
                        p.SetValue(obj, val, null);
                    }
                    else
                    {
                        if (p.PropertyType == typeof(string)) p.SetValue(obj, "", null);
                        else p.SetValue(obj, null, null);
                    }
                }
                catch (Exception ex)
                {
                    p.SetValue(obj, null, null);
                }
            }
        }

        public static void SaveObjectMembers(object obj, DataRow dr, bool isNew)
        {
            //this is a simplified version of 
            //the concept found here:
            //http://www.codeproject.com/cs/database/dal3.asp

            PropertyInfo[] props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in props)
            {
                try
                {
                    if (p.Name == "ID") continue;
                    if (dr.Table.Columns.Contains(p.Name))
                    {
                        object val = p.GetValue(obj, null);
                        if (val != null)
                        {
                            if (p.PropertyType.IsEnum) val = val.ToString();
                            if (p.PropertyType == typeof(bool)) val = Convert.ToBoolean(val) ? "1" : "0";

                            if (p.PropertyType == typeof(DateTime))
                            {
                                DateTime dt = (DateTime)val;
                                if (dt <= DateTime.MinValue) val = DBNull.Value;
                            }
                        }
                        else
                        {
                            if (p.PropertyType == typeof(string)) val = "";
                            else val = DBNull.Value;
                        }
                        dr[p.Name] = val;

                    }
                }
                catch (Exception ex)
                {
                    //the object can and probably will have more properties than the record
                    //has columns.  this catches & ignores the lookup for columns with no matching name.
                }
            }
        }

        public static string GetStringRepresentation(object obj, PropertyInfo p, bool includeQuotes = true)
        {
            object val = p.GetValue(obj, null);
            if (val != null)
            {
                if (p.PropertyType.IsEnum) val = Convert.ToInt32(val);
                else if (p.PropertyType == typeof(bool)) val = Convert.ToBoolean(val) ? "1" : "0";
                else if (p.PropertyType == typeof(DateTime))
                {
                    DateTime dt = (DateTime)val;
                    if (dt < DateTime.MinValue) val = DBNull.Value;
                    else val = Utilities.DBDateTimeIn(dt);
                }
            }
            else
            {
                val = DBNull.Value;
            }
            if (val == DBNull.Value) return "NULL";

            return includeQuotes ? "'" + DBIn(val.ToString()) + "'" : DBIn(val.ToString());
        }


        public static DateTime DBDateOut(string dateTime)
        {
            if (dateTime == null) return DateTime.MinValue;
            if (dateTime == "") return DateTime.MinValue;
            return Convert.ToDateTime(dateTime);
            //return (DateTime)dateTime;
        }

        public static string DBIn(string str)
        {
            if (str == null) return "";
            if (str.Length == 0) return "";
            // TODO fix this below?
            while (str.Contains("\\'")) str = str.Replace("\\'", "'");

            str = str.Replace("'", "''");
            str = str.Replace("\r\n", "<br>");
            return str;
        }

        public static string DBDateIn(DateTime dt)
        {
            //if (dateTime == DateTime.MinValue) return "";
            //return dt.ToString();
            string date = dt.Year + "-" + dt.Month.ToString("D2") + "-" + dt.Day.ToString("D2");// +" 00:00:00";
            return date;
        }

        public static string DBDateTimeIn(DateTime dt)
        {
            if (dt == DateTime.MinValue) return "";
            return dt.Year + "-" + dt.Month + "-" + dt.Day + " " + dt.Hour + ":" + dt.Minute + ":" + dt.Second;
        }

        public static bool IsNumeric(object Expression)
        {
            if (Expression == null) return false;
            if (Expression.ToString() == "") return false;
            double retNum;
            return Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
        }

        public static bool IsInteger(object Expression)
        {
            if (!IsNumeric(Expression)) return false;
            // Cannot have a decimal
            return (Expression.ToString().IndexOf(".") == -1);
        }

        public static DataTable convertStringToDataTable(string data)
        {
            DataTable dataTable = new DataTable();
            bool columnsAdded = false;
            var rows = data.Split('\r').ToList();
            for (int i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                DataRow dataRow = dataTable.NewRow();
                var cells = row.Split(',').ToList();
                for (int j = 0; j < cells.Count; j++)
                {
                    if (!columnsAdded)
                    {
                        DataColumn dataColumn = new DataColumn();
                        dataTable.Columns.Add(dataColumn);
                    }
                    dataRow[j] = cells[j].Trim();
                }
                columnsAdded = true;
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }

        public static DataTable DataTableFromCSV(string path, string header = "Yes")
        {
            Console.WriteLine("Reading file " + path + "...");
            if (!File.Exists(path)) return new DataTable();

            string full = Path.GetFullPath(path);
            string file = Path.GetFileName(full);
            string dir = Path.GetDirectoryName(full);

            //create the "database" connection string
            string connString = "Provider=Microsoft.Jet.OLEDB.4.0;"
              + "Data Source=\"" + dir + "\\\";"
              + "Extended Properties=\"text;HDR=" + header + ";FMT=CSVDelimited\"";

            //create the database query
            string query = "SELECT * FROM [" + file + "]";

            //create a DataTable to hold the query results
            DataTable dTable = new DataTable();

            //create an OleDbDataAdapter to execute the query
            OleDbDataAdapter dAdapter = new OleDbDataAdapter(query, connString);

            try
            {
                //fill the DataTable
                dAdapter.Fill(dTable);
            }
            catch (InvalidOperationException ex)
            {
                string exception = ex.Message;
            }

            dAdapter.Dispose();

            return dTable;
        }
    }
}
