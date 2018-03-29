using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GribData
{
    public class Site : DataClass
    {
        public int SiteID { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int XCell13km { get; set; }
        public int YCell13km { get; set; }
        public int XCell40km { get; set; }
        public int YCell40km { get; set; }
        public int XCell12km { get; set; }
        public int YCell12km { get; set; }
        public int XCell28km { get; set; }
        public int YCell28km { get; set; }
        public bool IsValid { get; set; }

        public Site()
            : base()
        {
        }

        public Site(int inID)
            : base(inID)
        {
        }

        public Site(DataRow dr)
            : base(dr)
        {

        }

        public static List<Site> GetList(string filter = "IsValid=1")
        {
            return GetList<Site>(filter);
        }
    }
}
