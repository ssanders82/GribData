using System;
using System.Data;
using System.Net;
//using System.Net.FtpClient;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grib.Api;

namespace GribData
{
    public class FuncsOld
    {
        public static void TrimFile(FileInfo fi)
        {
            FileInfo fiOut = new FileInfo(fi.FullName.Replace(fi.Extension, "_short" + fi.Extension));
            if (fiOut.Exists) fiOut.Delete();
            using (GribFile file = new GribFile(fi.FullName))
            {
                Console.WriteLine("Processing " + fi.FullName + ", found " + file.MessageCount + " attributes");

                foreach (GribMessage msg in file)
                {
                    if (msg.ShortName == "dswrf" || msg.ShortName == "ugrd" || msg.Index == 43)
                    {
                        var a = 1;
                    }
                    bool goodLevel = msg.Level <= 10 && msg.TypeOfLevel == "heightAboveGround";
                    if (!goodLevel) goodLevel = msg.Level == 0 && msg.TypeOfLevel == "surface";

                    if (!goodLevel) continue;
                    if (msg.Name.ToLower().Contains("geopotential height") ||
                        msg.Name.ToLower().Contains("total snowfall") ||
                        msg.Name.ToLower().Contains("snow depth") ||
                        msg.Name == "Water equivalent of accumulated snow depth") continue;
                    if (msg.TypeOfLevel == "nominalTop" || msg.TypeOfLevel == "isothermZero" || msg.TypeOfLevel == "tropopause") continue;
                    if (msg.TypeOfLevel == "heightAboveGround"
                        || msg.TypeOfLevel == "heightAboveGroundLayer"
                        || msg.TypeOfLevel == "pressureFromGroundLayer"
                        || msg.TypeOfLevel == "isobaricInhPa")
                    {
                        if (msg.Level >= 50) continue;
                    }
                    /*if (msg.Units == msg.Name)
                    {
                        continue;
                    }*/
                    //if (!attributes.Contains(msg.ShortName)) continue;

                    string desc = msg.ShortName + ", " + msg.Name + ", " + msg.Units + ", " + msg.Level + ", " + msg.TypeOfLevel;
                    //Console.WriteLine(desc);
                    GribFile.Write(fiOut.FullName, msg, FileMode.Append);
                }

            }
        }

        public static void RunTrimmer(string fullPath, string matchingPattern)
        {
            DirectoryInfo di = new DirectoryInfo(fullPath);
            TrimDirectory(di, matchingPattern);
        }

        public static void TrimDirectory(DirectoryInfo di, string matchingPattern)
        {
            var dis = di.EnumerateDirectories().ToList();
            foreach (var sub in dis) TrimDirectory(sub, matchingPattern);
            List<FileInfo> lstToMove = new List<FileInfo>();
            var files = di.EnumerateFiles("*.grb?").ToList();
            foreach (var sub in files)
            {
                if (!sub.Name.Contains(matchingPattern)) continue;
                if (sub.Name.Contains("_short_short"))
                {
                    sub.MoveTo(sub.FullName.Replace("_short_short", "_short"));
                }

                if (sub.Name.Contains("_short")) continue;
                try
                {
                    TrimFile(sub);
                    sub.Delete();
                }
                catch (Exception ex)
                {
                    lstToMove.Add(sub);
                }
            }

            foreach (var sub in lstToMove)
            {
                Console.WriteLine("Error with " + sub.FullName);
                //sub.MoveTo(sub.FullName + ".error");
            }
        }

        
        /*
        public bool ImportToMySQLOLD(FileInfo fi)
        {
            var index = fi.FullName.IndexOf("_20");
            string name = fi.FullName.Substring(index + 1).Replace(fi.Extension, "");
            //if (name.Length != 23) throw new Exception("Bad file name"); // 17 + _short = 23
            int year = Convert.ToInt32(name.Substring(0, 4));
            int month = Convert.ToInt32(name.Substring(4, 2));
            int day = Convert.ToInt32(name.Substring(6, 2));
            int hour = Convert.ToInt32(name.Substring(9, 2));
            int hoursAhead = Convert.ToInt32(name.Substring(14, 3));

            // 214 x 77
            double latitude = 33.197;
            double longitude = 275.815;

            GribPrediction gp = new GribPrediction();
            gp.Datasource = "NAM";
            gp.Filename = fi.Name;
            gp.HoursAhead = hoursAhead;

            if (fi.Name.Contains("_252_")) gp.GridResolution = 20;
            else if (fi.Name.Contains("_130_")) gp.GridResolution = 13;
            else if (fi.Name.Contains("_211_")) gp.GridResolution = 81;
            else if (fi.Name.Contains("_236_")) gp.GridResolution = 40;
            else if (fi.Name.Contains("_212_")) gp.GridResolution = 40;

            gp.PredictionMadeDate = new DateTime(year, month, day, hour, 0, 0, DateTimeKind.Local).ToLocalTime();
            gp.PredictionMadeTicks = gp.PredictionMadeDate.Ticks;
            gp.PredictionForDate = gp.PredictionMadeDate.AddHours(hoursAhead);
            gp.PredictionForTicks = gp.PredictionForDate.Ticks;

            gp.Temperature = -1;
            gp.PrecipitationRate = -1;
            gp.CloudCover = -1;
            gp.Visibility = -1;
            gp.PrecipitationProbability = -1;
            gp.WindSpeed = -1;
            gp.Year = year;
            gp.Day = gp.PredictionMadeDate.DayOfYear;
            gp.Time = (gp.PredictionMadeDate.Hour * 100) + gp.PredictionMadeDate.Minute;

            if (GribPrediction.Get(gp.PredictionMadeTicks, gp.PredictionForTicks, gp.GridResolution, gp.Site) != null)
            {
                return true;
            }

            var attributes = new string[] { "prate", "vis", "2t", "pop", "tp", "tmp", "tcc", "ws" };

            FileStream fs = new FileStream(fi.FullName, FileMode.Open);
            Grib1Input gi = new Grib1Input(fs);
            gi.scan(false, false);
            byte[] b = new byte[100];
            fs.Seek(129400, SeekOrigin.Begin);
            fs.Read(b, 0, 100);
            for (int i = 0; i < gi.Records.Count; i++)
            {

                Grib1Record rec = (Grib1Record)gi.Records[i];
                IGrib1IndicatorSection iis = rec.Is;
                IGrib1ProductDefinitionSection pdsv = rec.PDS;
                IGrib1GridDefinitionSection gdsv = rec.GDS;

                Grib1Data gd = new Grib1Data(fs);
                float[] data = gd.getData(rec.DataOffset,
                                            rec.PDS.DecimalScale, rec.PDS.bmsExists());
                System.Console.WriteLine( "Param=" + rec.PDS.ParameterNumber);
                System.Console.WriteLine("Time=" + rec.PDS.ForecastTime);

                //if ((iis.Discipline == 0) && (pdsv.ParameterCategory == 1) && (pdsv.ParameterNumber == 1))
                //{
                    int c = 0;
                    for (double lat = gdsv.La1; lat >= gdsv.La2; lat = lat - gdsv.Dy)
                    {
                        for (double lon = gdsv.Lo1; lon <= gdsv.Lo2; lon = lon + gdsv.Dx)
                        {
                            Console.WriteLine("RH " + lat + "\t" + lon + "\t" + data[c]);
                            c++;
                        }
                    }
                //}

            }
            using (GribFile file = new GribFile(fi.FullName))
            {
                Console.WriteLine("Processing " + fi.FullName + ", found " + file.MessageCount + " attributes");
                var lst1 = file.Where(m => attributes.Contains(m.ShortName.ToLower())).ToList();
                //if (lst1.Count() != attributes.Count()) return false; //throw new Exception("Bad attribute count for " + fi.FullName);
                //if (lst1.Count() < attributes.Count()) return false;

                foreach (GribMessage msg in file)
                {
                    var shortName = msg.ShortName.ToLower();
                    if (!attributes.Contains(shortName)) continue;

                    var lst = msg.GeoSpatialValues.ToList();

                    var matches = lst.Where(g => Math.Abs(g.Latitude - latitude) < .5 && Math.Abs(g.Longitude - longitude) < .5).ToList();
                    if (matches.Count == 0) throw new Exception("Null data for " + fi.FullName);
                    matches = matches.OrderBy(g => Math.Abs(g.Latitude - latitude) + Math.Abs(g.Longitude - longitude)).ToList();

                    var data = matches[0];

                    if (shortName == "2t") gp.Temperature = KelvinToFahrenheit(data.Value);
                    else if (shortName == "prate" || shortName == "tp") gp.PrecipitationRate = data.Value;
                    else if (shortName == "vis") gp.Visibility = data.Value;
                    else if (shortName == "pop") gp.PrecipitationProbability = data.Value;
                    else if (shortName == "ws") gp.WindSpeed = data.Value;
                    else if (shortName == "tcc") gp.CloudCover = data.Value;
                }
            }
            gp.Save();
            return true;
        }
        */
    }
}
