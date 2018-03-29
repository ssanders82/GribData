using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GribData
{
    // https://github.com/0x1mason/GribApi.NET
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Start? ");
            Console.ReadLine();
            Funcs f = new Funcs();

            for (int i = 31; i <= 32; i++)
            {
                Process pr2 = new Process();
                //pr2.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                pr2.StartInfo.FileName = @"c:\windows\system32\cmd.exe";
                //pr2.StartInfo.Arguments = string.Format(@"/c java -cp ""C:\Program Files\Weka-3-8\weka.jar"" weka.classifiers.trees.RandomForest -t d:\grib\exports\{0}.csv -T d:\grib\exports\{0}.csv -d d:\grib\exports\{0}.model", i);
                pr2.StartInfo.Arguments = string.Format(@"/c java -cp ""C:\Program Files\Weka-3-8\weka.jar"" -Xmx8g weka.classifiers.trees.RandomForest -t d:\grib\exports\{0}.csv -T d:\grib\exports\{0}.csv -d d:\grib\exports\{0}.model", i);
                pr2.Start();
                pr2.WaitForExit();
            }
            //f.CSVDump();
            //f.DeleteOldTextFiles(new DirectoryInfo(@"D:\grib\hour24\"));
            //FuncsOld.RunTrimmer(@"D:\grib\gfs\", ".grb");
            //FuncsOld.RunTrimmer(@"D:\grib\hour1\2014on\201405\", ".grb");

            //f.CreateTextFiles(new DirectoryInfo(@"d:\grib\gfs\"), "_short.grb", 12);
            //f.CreateTextFiles(new DirectoryInfo(@"D:\grib\hour1\2014on\201405\"), "_000_short.grb", 13);
            //f.CreateTextFiles(new DirectoryInfo(@"\\work\grib\hour1\"), "_001_short.grb", 13);

            /*
            f.CreateTextFiles(new DirectoryInfo(@"D:\grib\hour24\"), "_024.grb", 40);
            f.CreateTextFiles(new DirectoryInfo(@"D:\grib\hour24\"), "_027.grb", 40);
            f.CreateTextFiles(new DirectoryInfo(@"D:\grib\hour24\"), "_030.grb", 40);
            f.CreateTextFiles(new DirectoryInfo(@"D:\grib\hour24\"), "_033.grb", 40);
            f.CreateTextFiles(new DirectoryInfo(@"D:\grib\hour24\"), "_036.grb", 40);
            */

            //f.ImportToMySQL(@"D:\grib\hour1\2014on\201405\", "RAP", "_000_short_");
            //f.ImportToMySQL(@"D:\grib\hour1\", "RAP", "_002_short_");
            //f.ImportToMySQL(@"D:\grib\hour24\", "NAM", "_");
            //f.ImportToMySQL(@"D:\grib\gfs\", "GFS", "_");
            //f.ImportToMySQL(@"D:\grib\nmm\", "NMM", "_");

            //f.Interpolate(1, "RAP", "hoursahead BETWEEN 1 AND 2 AND GridResolution=13");
            //f.Interpolate(24, "NAM", "hoursahead BETWEEN 24 AND 36 AND GridResolution=40");

            //f.RunTrimmer();
            //f.ImportToMySQL(new System.IO.FileInfo(@"D:\NAM\200306\20030608\early-eta_212_20030608_0000_009.txt"));



            //f.MakePredictions();
            //f.RunTrimmer();
            //f.ImportToMySQL();

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
