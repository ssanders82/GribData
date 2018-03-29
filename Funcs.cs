using System;
using System.Diagnostics;
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
    public class Funcs
    {
        public static void Interpolate(int hoursAhead, string datasource, List<GribPrediction> lstThisDate, DateTime date, string distinctSite, int distinctSiteID)
        {
            for (int i = 0; i < lstThisDate.Count - 1; i++)
            {
                var thisItem = lstThisDate[i];
                var nextItem = lstThisDate[i + 1];
                var predictionForDate = thisItem.PredictionMadeDate.AddHours(thisItem.HoursAhead);
                while (predictionForDate < nextItem.PredictionForDate)
                {
                    double minutesThroughInterval = predictionForDate.Subtract(thisItem.PredictionForDate).TotalMinutes;
                    double totalIntervalMinutes = nextItem.PredictionForDate.Subtract(thisItem.PredictionForDate).TotalMinutes;
                    double ratioThroughInterval = (1.0 * minutesThroughInterval) / totalIntervalMinutes;

                    Console.WriteLine("trying to predict " + predictionForDate.ToString() + " for " + distinctSite);

                    GribPrediction gp = new GribPrediction();
                    gp.Datasource = minutesThroughInterval == 0 ? datasource : datasource + "Interpolation";

                    if (ratioThroughInterval == 0) gp.Filename = thisItem.Filename;
                    else if (ratioThroughInterval == 100) gp.Filename = nextItem.Filename;
                    else gp.Filename = thisItem.Filename + "," + nextItem.Filename;

                    gp.Site = distinctSite;
                    gp.SiteID = thisItem.SiteID;
                    gp.HoursAhead = hoursAhead;
                    gp.GridResolution = thisItem.GridResolution;

                    // Files are in UTC time!
                    gp.PredictionForDate = predictionForDate;
                    gp.PredictionForTicks = gp.PredictionForDate.Ticks;

                    gp.PredictionMadeDate = gp.PredictionForDate.AddHours(-gp.HoursAhead);
                    gp.PredictionMadeTicks = gp.PredictionMadeDate.Ticks;

                    GribPrediction gpDuplicate = GribPrediction.Get(gp.PredictionMadeTicks, gp.PredictionForTicks, gp.GridResolution, gp.Site, gp.SiteID, gp.Datasource);
                    if (gpDuplicate != null) gp.ID = gpDuplicate.ID;

                    gp.Temperature = -1;
                    gp.PrecipitationRate = -1;
                    gp.PrecipitationProbability = -1;
                    gp.CloudCover = -1;
                    gp.Visibility = -1;
                    gp.WindSpeed = -1;
                    gp.WindDirection = -1;
                    gp.MaxTemperature = -1;
                    gp.MinTemperature = -1;
                    gp.DewPointTemperature = -1;
                    gp.AirPressure = -1;
                    gp.RelativeHumidity = -1;

                    if (thisItem.Temperature >= 0 && nextItem.Temperature >= 0)
                    {
                        gp.Temperature = (ratioThroughInterval * nextItem.Temperature) + ((1 - ratioThroughInterval) * thisItem.Temperature);
                    }

                    if (thisItem.PrecipitationRate >= 0 && nextItem.PrecipitationRate >= 0)
                    {
                        gp.PrecipitationRate = (ratioThroughInterval * nextItem.PrecipitationRate) + ((1 - ratioThroughInterval) * thisItem.PrecipitationRate);
                    }

                    if (thisItem.PrecipitationProbability >= 0 && nextItem.PrecipitationProbability >= 0)
                    {
                        gp.PrecipitationProbability = (ratioThroughInterval * nextItem.PrecipitationProbability) + ((1 - ratioThroughInterval) * thisItem.PrecipitationProbability);
                    }

                    if (thisItem.CloudCover >= 0 && nextItem.CloudCover >= 0)
                    {
                        gp.CloudCover = (ratioThroughInterval * nextItem.CloudCover) + ((1 - ratioThroughInterval) * thisItem.CloudCover);
                    }

                    if (thisItem.Visibility >= 0 && nextItem.Visibility >= 0)
                    {
                        gp.Visibility = (ratioThroughInterval * nextItem.Visibility) + ((1 - ratioThroughInterval) * thisItem.Visibility);
                    }

                    if (thisItem.WindSpeed >= 0 && nextItem.WindSpeed >= 0)
                    {
                        gp.WindSpeed = (ratioThroughInterval * nextItem.WindSpeed) + ((1 - ratioThroughInterval) * thisItem.WindSpeed);
                    }

                    if (thisItem.WindDirection >= 0 && nextItem.WindDirection >= 0)
                    {
                        gp.WindDirection = (ratioThroughInterval * nextItem.WindDirection) + ((1 - ratioThroughInterval) * thisItem.WindDirection);
                    }

                    if (thisItem.MaxTemperature >= 0 && nextItem.MaxTemperature >= 0)
                    {
                        gp.MaxTemperature = (ratioThroughInterval * nextItem.MaxTemperature) + ((1 - ratioThroughInterval) * thisItem.MaxTemperature);
                    }

                    if (thisItem.MinTemperature >= 0 && nextItem.MinTemperature >= 0)
                    {
                        gp.MinTemperature = (ratioThroughInterval * nextItem.MinTemperature) + ((1 - ratioThroughInterval) * thisItem.MinTemperature);
                    }

                    if (thisItem.DewPointTemperature >= 0 && nextItem.DewPointTemperature >= 0)
                    {
                        gp.DewPointTemperature = (ratioThroughInterval * nextItem.DewPointTemperature) + ((1 - ratioThroughInterval) * thisItem.DewPointTemperature);
                    }

                    if (thisItem.AirPressure >= 0 && nextItem.AirPressure >= 0)
                    {
                        gp.AirPressure = (ratioThroughInterval * nextItem.AirPressure) + ((1 - ratioThroughInterval) * thisItem.AirPressure);
                    }

                    if (thisItem.RelativeHumidity >= 0 && nextItem.RelativeHumidity >= 0)
                    {
                        gp.RelativeHumidity = (ratioThroughInterval * nextItem.RelativeHumidity) + ((1 - ratioThroughInterval) * thisItem.RelativeHumidity);
                    }
                    gp.Year = gp.PredictionMadeDate.Year;
                    gp.Day = gp.PredictionMadeDate.DayOfYear;
                    gp.Time = (gp.PredictionMadeDate.Hour * 100) + gp.PredictionMadeDate.Minute;
                    if (gp.PredictionMadeDate.Hour == 0 && gp.PredictionMadeDate.Minute == 0)
                    {
                        gp.Year = gp.PredictionMadeDate.AddDays(-1).Year;
                        gp.Day = gp.PredictionMadeDate.AddDays(-1).DayOfYear;
                        gp.Time = 2400;
                    }
                    if (gp.ID == 0) gp.Save();
                    predictionForDate = predictionForDate.AddMinutes(15);
                }
            }
        }

        public void Interpolate(int hoursAhead, string datasource, string filter)
        {
            filter += " AND Datasource='" + datasource + "'";
            var lst = GribPrediction.GetList(filter);
            var distinctDates = lst.Select(g => g.PredictionMadeDate).Distinct().OrderBy(d => d).ToList();
            var distinctSites = lst.Select(g => g.Site).Distinct().OrderBy(d => d).ToList();
            var distinctSiteIDs = lst.Select(g => g.SiteID).Distinct().OrderBy(d => d).ToList();
            foreach (var distinctSite in distinctSites)
            {
                foreach (var distinctSiteID in distinctSiteIDs)
                {
                    foreach (var distinctDate in distinctDates)
                    {
                        Console.WriteLine("trying " + distinctDate.ToString() + " for " + distinctSite + "/" + distinctSiteID);
                        var lstThisDate = lst.Where(g => g.PredictionMadeDate == distinctDate && g.Site == distinctSite && g.SiteID == distinctSiteID)
                            .OrderBy(g => g.Time)
                            .ThenBy(g => g.HoursAhead).ToList();
                        if (lstThisDate.Count < 2) continue;

                        Interpolate(hoursAhead, datasource, lstThisDate, distinctDate, distinctSite, distinctSiteID);
                    }// date
                }// siteID
            }// site
        }

        public void DeleteOldTextFiles(DirectoryInfo di)
        {
            return;
            var cutoff = new DateTime(2017, 3, 15, 7, 0, 0);
            var dis = di.EnumerateDirectories().ToList();
            foreach (var sub in dis) DeleteOldTextFiles(sub);

            var files = di.EnumerateFiles("*.txt").ToList();
            foreach (var sub in files)
            {
                if (sub.LastWriteTime < cutoff)
                {
                    sub.Delete();
                }
            }
            Console.WriteLine("cleaned " + di.FullName);
        }

        public void CreateTextFiles(DirectoryInfo di, string matchingPattern, int cellSizeKM)
        {
            var dis = di.EnumerateDirectories().ToList();
            foreach (var sub in dis) CreateTextFiles(sub, matchingPattern, cellSizeKM);

            var files = di.EnumerateFiles("*.grb?").ToList();
            foreach (var sub in files)
            {
                if (!sub.Name.Contains(matchingPattern)) continue;
                CreateTextFiles(sub, cellSizeKM);
            }
        }

        public void CreateTextFiles(FileInfo fi, int cellSizeKM)
        {
            Dictionary<string, int[]> cells = GetCells(cellSizeKM, "SiteID=1000");
            Console.WriteLine("Generating for " + fi.FullName);
            Parallel.ForEach(cells, new ParallelOptions { MaxDegreeOfParallelism = 3 }, (cell, state) => {
            //foreach (var cell in cells) {
                string newFile = fi.FullName.Replace(fi.Extension, "") + "_" + cell.Key + ".txt";
                
                FileInfo fiExisting = new FileInfo(newFile);
                if (fiExisting.Exists && fiExisting.Length > 0 && fiExisting.LastWriteTime > new DateTime(2017, 3, 15, 7, 0, 0)) return;

                int xCell = cell.Value[0];
                int yCell = cell.Value[1];


                Process pr2 = new Process();
                pr2.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                pr2.StartInfo.FileName = @"c:\windows\system32\cmd.exe";
                pr2.StartInfo.Arguments = string.Format(@"/c C:\ndfd\degrib\bin\degrib.exe {0} -P -pnt {1},{2} -nLabel -pntStyle 1 -cells true > {3}", fi.FullName, yCell, xCell, newFile);
                pr2.Start();
                pr2.WaitForExit();
            //}
            });
        }


        public void ImportToMySQL(string fullPath, string datasource, string matchingPattern)
        {
            DirectoryInfo di = new DirectoryInfo(fullPath);
            List<SolarRadiation> cachedSR = null;
            //if (datasource == "NAM") cachedSR = SolarRadiation.GetList("dateandtime BETWEEN '2003-01-01' AND '2005-12-30 23:00:00'");
            //else if (datasource == "RAP") cachedSR = SolarRadiation.GetList("dateandtime BETWEEN '2011-06-21' AND '2012-04-30 23:00:00'");
            var sites = Site.GetList("SiteID=1000");
            ImportToMySQL(di, datasource, cachedSR, sites, matchingPattern);      
        }

        public void ImportToMySQL(DirectoryInfo di, string datasource, List<SolarRadiation> cachedSR, List<Site> sites, string matchingPattern)
        {
            var dis = di.EnumerateDirectories().ToList();
            foreach (var sub in dis) ImportToMySQL(sub, datasource, cachedSR, sites, matchingPattern);

            var files = di.EnumerateFiles("*.txt").ToList();
            foreach (var sub in files)
            {

                if (!sub.Name.Contains(matchingPattern)) continue;
                string fileName = sub.Name.ToLower();
                try
                {
                    var success = ImportToMySQL(sub, datasource, cachedSR, sites);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Wrong message length" || ex.Message == "This file is empty or invalid." || ex.Message == "End of resource reached when reading message")
                    {
                        var a = ex.Message;
                        //sub.MoveTo(sub.FullName + ".error");
                    }
                    else throw ex;
                }
            }
        }


        public bool ImportToMySQL(FileInfo fi, string datasource, List<SolarRadiation> cachedSR, List<Site> sites)
        {
            //if (!fi.Name.Contains("_Griffin")) return true;
            Console.WriteLine("Importing file " + fi.Name);
            var index = fi.FullName.IndexOf("_20");
            int siteIndex = fi.FullName.LastIndexOf("_");
            string name = fi.FullName.Substring(index + 1, siteIndex - index);
            string siteAndCell = fi.FullName.Substring(siteIndex + 1).Replace(fi.Extension, "");
            Site site = sites.FirstOrDefault(s => siteAndCell.StartsWith(s.Name));
            if (site == null) return true; // Not using this site anymore

            //if (name.Length != 23) throw new Exception("Bad file name"); // 17 + _short = 23
            int year = Convert.ToInt32(name.Substring(0, 4));
            int month = Convert.ToInt32(name.Substring(4, 2));
            int day = Convert.ToInt32(name.Substring(6, 2));
            int hour = Convert.ToInt32(name.Substring(9, 2));
            int hoursAhead = Convert.ToInt32(name.Substring(14, 3));

            GribPrediction gp = new GribPrediction();
            gp.Datasource = datasource;
            gp.Filename = fi.Name;
            gp.HoursAhead = hoursAhead;
            gp.SiteID = site.SiteID;
            gp.Site = fi.FullName.Substring(siteIndex + 1).Replace(site.Name, "").Replace(fi.Extension, "");

            if (fi.Name.Contains("_252_")) gp.GridResolution = 20;
            else if (fi.Name.Contains("_130_")) gp.GridResolution = 13;
            else if (fi.Name.Contains("_211_")) gp.GridResolution = 81;
            else if (fi.Name.Contains("_236_")) gp.GridResolution = 40;
            else if (fi.Name.Contains("_212_")) gp.GridResolution = 40;

            // Files are in UTC time!
            gp.PredictionMadeDate = new DateTime(year, month, day, hour, 0, 0, DateTimeKind.Utc).ToLocalTime();
            gp.PredictionMadeTicks = gp.PredictionMadeDate.Ticks;
            gp.PredictionForDate = gp.PredictionMadeDate.AddHours(hoursAhead);
            gp.PredictionForTicks = gp.PredictionForDate.Ticks;

            gp.Temperature = -1;
            gp.PrecipitationRate = -1;
            gp.CloudCover = -1;
            gp.Visibility = -1;
            gp.PrecipitationProbability = -1;
            gp.WindSpeed = -1;
            gp.WindDirection = -1;
            gp.MaxTemperature = -1;
            gp.MinTemperature = -1;
            gp.DewPointTemperature = -1;
            gp.AirPressure = -1;
            gp.RelativeHumidity = -1;

            gp.Year = year;
            gp.Day = gp.PredictionMadeDate.DayOfYear;
            gp.Time = (gp.PredictionMadeDate.Hour * 100) + gp.PredictionMadeDate.Minute;
            if (gp.PredictionMadeDate.Hour == 0 && gp.PredictionMadeDate.Minute == 0)
            {
                gp.Year = gp.PredictionMadeDate.AddDays(-1).Year;
                gp.Day = gp.PredictionMadeDate.AddDays(-1).DayOfYear;
                gp.Time = 2400;
            }

            GribPrediction gpDuplicate = GribPrediction.Get(gp.PredictionMadeTicks, gp.PredictionForTicks, gp.GridResolution, gp.Site, gp.SiteID, gp.Datasource);
            if (gpDuplicate != null) return true;
            if (gpDuplicate != null) gp.ID = gpDuplicate.ID;

            double? ucomp = null;
            double? vcomp = null;

            DataTable dt = Utilities.convertStringToDataTable(File.ReadAllText(fi.FullName));
            if (dt.Columns.Count != 8) return true;
            if (dt.Rows.Count < 5) return true;
            foreach (DataRow dr in dt.Rows)
            {
                var shortName = dr[4].ToString().ToUpper();
                if (shortName.IndexOf("[") == -1) continue;
                shortName = shortName.Substring(0, shortName.IndexOf("["));

                if (dr[7] == DBNull.Value) continue;
                double val = Convert.ToDouble(dr[7]);

                if (shortName == "TMP") gp.Temperature = val; // Get the last one
                else if (shortName == "PRATE" || shortName == "APCP") gp.PrecipitationRate = val;
                else if (shortName == "VIS") gp.Visibility = val;
                else if (shortName == "POP") gp.PrecipitationProbability = val;
                else if (shortName == "WIND") gp.WindSpeed = val;
                else if (shortName == "TCDC") gp.CloudCover = val;
                else if (shortName == "WDIR") gp.WindDirection = val;
                else if (shortName == "TMAX") gp.MaxTemperature = val;
                else if (shortName == "TMIN") gp.MinTemperature = val;
                else if (shortName == "DPT" && gp.DewPointTemperature == -1) gp.DewPointTemperature = val;
                else if (shortName == "PRES" && gp.AirPressure == -1) gp.AirPressure = val;
                else if (shortName == "RH") gp.RelativeHumidity = val;
                else if (shortName == "DSWRF")
                {
                    gp.DownwardShortWaveRadFlux = val;
                }
                else if (shortName == "DLWRF") gp.DownwardLongWaveRadFlux = val;
                else if (shortName == "USWRF") gp.UpwardShortWaveRadFlux = val;
                else if (shortName == "ULWRF") gp.UpwardLongWaveRadFlux = val;

                else if (shortName == "UGRD" && !ucomp.HasValue) ucomp = val;
                else if (shortName == "VGRD" && !vcomp.HasValue) vcomp = val;
                
            }

            if (ucomp.HasValue && vcomp.HasValue)
            {
                // Convert ucomp and vcomp to wind speed and direction
                gp.WindSpeed = Utilities.CalulateWindSpeed(ucomp.Value, vcomp.Value);
                gp.WindDirection = Utilities.CalulateWindDirection(ucomp.Value, vcomp.Value);
            }

            //SolarRadiation sr = SolarRadiation.Get<SolarRadiation>("DateAndTime='" + Utilities.DBDateTimeIn(gp.PredictionMadeDate) + "'");
            if (cachedSR != null)
            {
                var sr = cachedSR.FirstOrDefault(s => s.DateAndTime == gp.PredictionMadeDate && s.SiteID == site.SiteID);
                gp.SolarRadiationID = sr.ID;
            }
            gp.Save();
            return true;
        }

        /// <summary>
        /// value is int[2] with x cell, y cell
        /// </summary>
        /// <param name="cellSizeKM"></param>
        /// <returns></returns>
        public static Dictionary<string, int[]> GetCells(int cellSizeKM, string siteFilter = "")
        {
            Dictionary<string, int[]> cells = new Dictionary<string, int[]>();
            var sites = Site.GetList(siteFilter);
            foreach (var site in sites)
            {
                var siteCells = new int[] { };
                if (cellSizeKM == 13) siteCells = new int[] { site.XCell13km, site.YCell13km };
                else if (cellSizeKM == 40) siteCells = new int[] { site.XCell40km, site.YCell40km };
                else if (cellSizeKM == 12) siteCells = new int[] { site.XCell12km, site.YCell12km };
                else if (cellSizeKM == 28) siteCells = new int[] { site.XCell28km, site.YCell28km };
                else throw new Exception("don't know this one");

                if (siteCells[0] == 0) throw new Exception("Missing");

                cells.Add(site.Name, siteCells); // 24 hour: 130, 45. 1 hour: 321/115.

                if (site.SiteID > 0 && site.SiteID < 1000)
                {
                    cells.Add(site.Name + "West", new[] { siteCells[0], siteCells[1] - 1 });
                    cells.Add(site.Name + "East", new[] { siteCells[0], siteCells[1] + 1 });
                    cells.Add(site.Name + "North", new[] { siteCells[0] + 1, siteCells[1] });
                    cells.Add(site.Name + "South", new[] { siteCells[0] - 1, siteCells[1] });

                    cells.Add(site.Name + "Northwest", new[] { siteCells[0] + 1, siteCells[1] - 1 });
                    cells.Add(site.Name + "Northeast", new[] { siteCells[0] + 1, siteCells[1] + 1 });
                    cells.Add(site.Name + "Southwest", new[] { siteCells[0] - 1, siteCells[1] - 1 });
                    cells.Add(site.Name + "Southeast", new[] { siteCells[0] - 1, siteCells[1] + 1 });
                }
            }
            
            return cells;
        }

        public double KelvinToFahrenheit(double k)
        {
            double c = k - 273.15;
            return CelsiusToFahrenheit(c);
        }

        public double CelsiusToFahrenheit(double c)
        {
            double f = (c * 1.8) + 32;
            return f;
        }
    }
}
