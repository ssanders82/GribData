using System;
using System.Data;
using System.Net;
//using System.Net.FtpClient;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GribData
{
    public static class Hour24Funcs
    {
        /*
        public static void CSVDump()
        {
            FileInfo fiOutput = new FileInfo("output_rain.csv");
            if (fiOutput.Exists) fiOutput.Delete();

            System.IO.StreamWriter sw = new System.IO.StreamWriter(fiOutput.FullName);

            var srList = SolarRadiation.GetList("Year between 2003 and 2005");
            var data = File.ReadAllLines("grib.csv");
            Console.WriteLine(data.Length);
            // Create datatable
            DataTable dt = new DataTable();
            Dictionary<string, int[]> sites = Funcs.GetSites(40);
            Dictionary<string, int> siteAttributes = new Dictionary<string, int>();
            siteAttributes.Add("Temperature", 5);
            siteAttributes.Add("CloudCover", 6);
            siteAttributes.Add("PrecipitationProbability", 8);
            siteAttributes.Add("WindSpeed", 10);
            siteAttributes.Add("WindDirection", 11);
            siteAttributes.Add("MaxTemperature", 12);
            siteAttributes.Add("MinTemperature", 13);
            siteAttributes.Add("DewPointTemperature", 14);

            string[] srAttributes = new[] {"Year","Day","Time","AdjDate","AirTemp","Humidity","Dewpoint","VaporP","VaporPD","BarometricP","WindSpeed",
                "WindDir","SD","MaxWind","Pan","SR","TSR","PAR","Rain","Rain2"};

            foreach (var srAttribute in srAttributes)
            {
                dt.Columns.Add(new DataColumn(srAttribute, typeof(double)));
            }
            foreach (KeyValuePair<string, int[]> site in sites)
            {
                foreach (KeyValuePair<string, int> siteAttribute in siteAttributes)
                {
                    string columnName = site.Key + siteAttribute.Key;
                    dt.Columns.Add(new DataColumn(columnName, typeof(double)));
                }
            }
            dt.Columns.Add(new DataColumn("Target", typeof(double)));

            string lastYearDayTime = "";
            int matchingRows = 0;
            int desiredRows = sites.Count;
            DataRow dr = dt.NewRow();


            StringBuilder sb = new StringBuilder();
            int n = 0;
            foreach (DataColumn dc in dt.Columns)
            {
                if (n > 0) sb.Append(",");
                sb.Append(dc.ColumnName);
                n++;
            }
            sw.WriteLine(sb.ToString());
            sb = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            //for (int i = 0; i < 2000; i++)
            {
                if (i % 50000 == 0) Console.WriteLine("Done with " + i + " of " + data.Length);
                var row = data[i];
                var cells = row.Split(',').ToList();
                for (int j = 0; j < cells.Count; j++) cells[j] = cells[j].Replace("\"", "");

                string site = cells[23].ToString();
                if (!sites.ContainsKey(site)) continue;

                string yearDayTime = cells[20].ToString() + cells[21].ToString().PadLeft(3, '0') + cells[22].ToString().PadLeft(4, '0');
                if (yearDayTime != lastYearDayTime)
                {
                    if (matchingRows == desiredRows)
                    {
                        // Save to file
                        // Find matching sr
                        int year = Convert.ToInt32(lastYearDayTime.Substring(0, 4));
                        int day = Convert.ToInt32(lastYearDayTime.Substring(4, 3));
                        int time = Convert.ToInt32(lastYearDayTime.Substring(7, 4));
                        var sr = srList.FirstOrDefault(s => s.Year == year && s.Day == day && s.Time == time);
                        var sr24 = srList.FirstOrDefault(s => s.Year == year && s.Day == day + 1 && s.Time == time);
                        if (sr != null && sr24 != null)
                        {
                            for (int m = 0; m < srAttributes.Count(); m++)
                            {
                                var attr = srAttributes[m];
                                //if (m > 0) sb.Append(",");
                                //sb.Append(sr._Data[attr]);
                                dr[attr] = Convert.ToDouble(sr._Data[attr]);
                                dr["Target"] = sr24.Rain > 0 ? 100 : 0;// CelsiusToFahrenheit(sr24.AirTemp);
                            }

                            for (int m = 0; m < dt.Columns.Count; m++)
                            {
                                if (m > 0) sb.Append(",");
                                sb.Append(dr[m]);
                            }
                            //sb.Append(",");
                            //sb.Append(sr._Data["SR24"]);
                            sw.WriteLine(sb.ToString());
                            sb = new StringBuilder();

                        }
                    }
                    dr = dt.NewRow();
                    matchingRows = 0;
                    lastYearDayTime = yearDayTime;
                }

                Dictionary<string, double> vals = new Dictionary<string, double>();
                foreach (KeyValuePair<string, int> siteAttribute in siteAttributes)
                {
                    double dbl = Convert.ToDouble(cells[siteAttribute.Value]);
                    if (dbl < 0) break;
                    vals.Add(site + siteAttribute.Key, dbl);
                }
                if (vals.Count == siteAttributes.Count)
                {
                    foreach (var val in vals)
                    {
                        dr[val.Key] = val.Value;
                    }
                    matchingRows++;
                }
            }

            sw.Close();
            //File.WriteAllText("output.csv", sb.ToString());
        }
        */
    }
}
