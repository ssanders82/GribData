using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GribData
{
    public static class Hour1Funcs
    {
        public static void Interpolate(List<GribPrediction> lstThisDate, DateTime date, string distinctSite, int distinctSiteID)
        {
            for (int i = 0; i < lstThisDate.Count - 1; i++)
            {
                var thisItem = lstThisDate[i];
                var nextItem = lstThisDate[i + 1];
                var predictionForDate = thisItem.PredictionMadeDate.AddHours(thisItem.HoursAhead);
                while (predictionForDate < nextItem.PredictionForDate)
                {
                    Console.WriteLine("trying to predict " + predictionForDate.ToString());

                    GribPrediction gp = new GribPrediction();
                    gp.Datasource = "RAPInterpolation";
                    gp.Filename = "";
                    gp.Site = distinctSite;
                    gp.HoursAhead = 1;
                    gp.GridResolution = thisItem.GridResolution;
                    gp.SiteID = distinctSiteID;
                    // Files are in UTC time!
                    gp.PredictionForDate = predictionForDate;
                    gp.PredictionForTicks = gp.PredictionForDate.Ticks;

                    gp.PredictionMadeDate = gp.PredictionForDate.AddHours(-gp.HoursAhead);
                    gp.PredictionMadeTicks = gp.PredictionMadeDate.Ticks;

                    GribPrediction gpDuplicate = GribPrediction.Get(gp.PredictionMadeTicks, gp.PredictionForTicks, gp.GridResolution, gp.Site, gp.SiteID, gp.Datasource);
                    if (gpDuplicate != null) gp.ID = gpDuplicate.ID;

                    gp.Temperature = -1;
                    gp.PrecipitationRate = -1;
                    gp.CloudCover = -1;
                    gp.Visibility = -1;
                    gp.PrecipitationProbability = -1;
                    gp.WindSpeed = -1;
                    gp.WindDirection = -1;

                    double minutesThroughInterval = predictionForDate.Subtract(thisItem.PredictionForDate).TotalMinutes;
                    double totalIntervalMinutes = nextItem.PredictionForDate.Subtract(thisItem.PredictionForDate).TotalMinutes;
                    double percentThroughInterval = (1.0 * minutesThroughInterval) / totalIntervalMinutes;

                    if (thisItem.Temperature >= 0 && nextItem.Temperature >= 0)
                    {
                        gp.Temperature = (percentThroughInterval * nextItem.Temperature) + ((1 - percentThroughInterval) * thisItem.Temperature);
                    }

                    if (thisItem.PrecipitationRate >= 0 && nextItem.PrecipitationRate >= 0)
                    {
                        gp.PrecipitationRate = (percentThroughInterval * nextItem.PrecipitationRate) + ((1 - percentThroughInterval) * thisItem.PrecipitationRate);
                    }

                    if (thisItem.Visibility >= 0 && nextItem.Visibility >= 0)
                    {
                        gp.Visibility = (percentThroughInterval * nextItem.Visibility) + ((1 - percentThroughInterval) * thisItem.Visibility);
                    }

                    if (thisItem.WindSpeed >= 0 && nextItem.WindSpeed >= 0)
                    {
                        gp.WindSpeed = (percentThroughInterval * nextItem.WindSpeed) + ((1 - percentThroughInterval) * thisItem.WindSpeed);
                    }

                    if (thisItem.WindDirection >= 0 && nextItem.WindDirection >= 0)
                    {
                        gp.WindDirection = (percentThroughInterval * nextItem.WindDirection) + ((1 - percentThroughInterval) * thisItem.WindDirection);
                    }

                    if (thisItem.DewPointTemperature >= 0 && nextItem.DewPointTemperature >= 0)
                    {
                        gp.DewPointTemperature = (percentThroughInterval * nextItem.DewPointTemperature) + ((1 - percentThroughInterval) * thisItem.DewPointTemperature);
                    }

                    if (thisItem.AirPressure >= 0 && nextItem.AirPressure >= 0)
                    {
                        gp.AirPressure = (percentThroughInterval * nextItem.AirPressure) + ((1 - percentThroughInterval) * thisItem.AirPressure);
                    }

                    if (thisItem.RelativeHumidity >= 0 && nextItem.RelativeHumidity >= 0)
                    {
                        gp.RelativeHumidity = (percentThroughInterval * nextItem.RelativeHumidity) + ((1 - percentThroughInterval) * thisItem.RelativeHumidity);
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
                    gp.Save();
                    predictionForDate = predictionForDate.AddMinutes(15);
                } // while
            } // for
        }

        public static void Interpolate(string filter)
        {
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
                        Console.WriteLine("trying " + distinctDate.ToString() + " for " + distinctSite);
                        var lstThisDate = lst.Where(g => g.PredictionMadeDate == distinctDate && g.Site == distinctSite && g.SiteID == distinctSiteID)
                            .OrderBy(g => g.Time)
                            .ThenBy(g => g.HoursAhead).ToList();
                        if (lstThisDate.Count < 2) continue;

                        Interpolate(lstThisDate, distinctDate, distinctSite, distinctSiteID);
                    }// date
                }// siteID
            }// site
        }// end Interpolate
    } // end Hour1Funcs
} // end namespace
