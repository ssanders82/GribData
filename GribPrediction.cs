using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GribData
{
    public class GribPrediction : DataClass
    {
        public int SiteID { get; set; }
        public DateTime PredictionMadeDate { get; set; }
        public long PredictionMadeTicks { get; set; }
        public DateTime PredictionForDate { get; set; }
        public long PredictionForTicks { get; set; }
        public double Temperature { get; set; }
        public double CloudCover { get; set; }
        public double PrecipitationRate { get; set; }
        public double PrecipitationProbability { get; set; }
        public double WindSpeed { get; set; }
        public double WindDirection { get; set; }
        public double Visibility { get; set; }

        public double DownwardShortWaveRadFlux { get; set; }
        public double DownwardLongWaveRadFlux { get; set; }
        public double UpwardShortWaveRadFlux { get; set; }
        public double UpwardLongWaveRadFlux { get; set; }

        public double MaxTemperature { get; set; }
        public double MinTemperature { get; set; }
        public double DewPointTemperature { get; set; }
        public double AirPressure { get; set; }
        public double RelativeHumidity { get; set; }
        public string Filename { get; set; }
        public int GridResolution { get; set; }
        public int HoursAhead { get; set; }
        public int SolarRadiationID { get; set; }
        public string Datasource { get; set; }

        public int Year { get; set; }
        public int Day { get; set; }
        public int Time { get; set; }
        public string Site { get; set; }

        public GribPrediction()
            : base()
        {
        }

        public GribPrediction(int inID)
            : base(inID)
        {
        }

        public GribPrediction(DataRow dr)
            : base(dr)
        {

        }

        public static GribPrediction Get(long madeTicks, long forTicks, int res, string site, int siteID, string datasource)
        {
            return Get<GribPrediction>(string.Format("PredictionMadeTicks={0} and PredictionForTicks={1} AND GridResolution={2} and Site='{3}' AND SiteID={4} AND DataSource='{5}'", 
                madeTicks, forTicks, res, site, siteID, datasource));
        }

        public static List<GribPrediction> GetList(string filter)
        {
            return GetList<GribPrediction>(filter);
        }
    }
}
