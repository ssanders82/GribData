using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GribData
{
    public class SolarRadiation : DataClass
    {
        public int SiteID { get; set; }
        public int Year { get; set; }
        public int Day { get; set; }
        public int Time { get; set; }
        public double AdjDate { get; set; }
        public double AirTemp { get; set; }
        public double Humidity { get; set; }
        public double DewPoint { get; set; }
        public double VaporP { get; set; }
        public double VaporPD { get; set; }
        public double BarometricP { get; set; }
        public double WindSpeed { get; set; }
        public double WindDir { get; set; }
        public double SD { get; set; }
        public double MaxWind { get; set; }
        public double Pan { get; set; }
        public double SR { get; set; }
        public double SR24 { get; set; }
        public double TSR { get; set; }
        public double Par { get; set; }
        public double Rain { get; set; }
        public double Rain2 { get; set; }
        public DateTime DateAndTime { get; set; }
        public double? SRPrediction = null;
        

        public SolarRadiation()
            : base()
        {
        }

        public SolarRadiation(int inID)
            : base(inID)
        {
        }

        public SolarRadiation(DataRow dr)
            : base(dr)
        {

        }

        public static List<SolarRadiation> GetList(string filter)
        {
            return GetList<SolarRadiation>(filter);
        }
    }
}
