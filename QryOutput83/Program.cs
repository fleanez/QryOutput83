#define USE_DATETIMESTRING

using EETDataFactory;
using EEUTILITY.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;



namespace QryOutput83
{
    class Program
    {

        private const int VALUE_DECIMAL = 2;
        private const bool USE_DATETIME_IN_CSV = false;

        private const int GENERATORGENERATION_PERIOD = 103;
        private const int GENERATORGENERATION_MONTH = 105;
        private const int GENERATORGENERATIONCOST_PERIOD = 106;
        private const int GENERATORFUELOFFTAKE_PERIOD = 107;
        private const int GENERATORCURTAILMENT_PERIOD = 108;
        private const int GENERATORCURTAILMENT_DAY = 109;

        private const int BATTNETGENERATION_PERIOD = 201;
        private const int BATTGENERATION_PERIOD = 202;
        private const int BATTLOAD_PERIOD = 203;

        private const int REGIONPRICE_PERIOD = 301;
        private const int REGIONENS_PERIOD = 302;
        private const int REGIONLOAD_PERIOD = 303;

        private const int NODEPRICE_PERIOD = 401;
        private const int NODELOAD_PERIOD = 402;

        private const int LINEFLOW_PERIOD = 501;

        private const int RESERVEPROVISION_PERIOD = 601;
        private const int RESERVESHORTAGE_PERIOD = 602;
        private const int RESERVEGENERATORPROVISION_PERIOD = 701;

        private static PLEXOS7_NET.Core.Solution InitConnection(string zipFilePath)
        {
            var zip = new PLEXOS7_NET.Core.Solution();
            zip.Connection(zipFilePath);
            return zip;
        }

        private static int ExecuteTestQueryToCsv(PLEXOS7_NET.Core.Solution zip, string csvFilePath, int action)
        {
            return ExecuteTestQueryToCsv(zip, csvFilePath, action, "0", 1);
        }

        private static int ExecuteTestQueryToCsv(PLEXOS7_NET.Core.Solution zip, string csvFilePath, int action, string sSampleList, int intervalLength)
        {
            //1--Execute query
            Console.WriteLine("Executing queries..");
            var qryTime = new Stopwatch();
            qryTime.Start();
            //string sSampleList = Utils.GetSampleListSansMedia(zip);
            SolutionResultList results = null;
            
            switch (action)
            {
                case GENERATORGENERATION_PERIOD:
                    results = Utils.QryToListGenerator(zip,SimulationPhaseEnum.STSchedule, PeriodEnum.Interval, SystemOutGeneratorsEnum.Generation, sSampleList);
                    break;
                case GENERATORGENERATIONCOST_PERIOD:
                    results = Utils.QryToListGenerator(zip, SimulationPhaseEnum.STSchedule, PeriodEnum.Interval, SystemOutGeneratorsEnum.GenerationCost, sSampleList);
                    break;
                case GENERATORFUELOFFTAKE_PERIOD:
                    results = Utils.QryToListGenerator(zip, SimulationPhaseEnum.STSchedule, PeriodEnum.Interval, SystemOutGeneratorsEnum.FuelOfftake, sSampleList);
                    break;
                case GENERATORCURTAILMENT_PERIOD:
                    results = Utils.QryToListGenerator(zip, SimulationPhaseEnum.STSchedule, PeriodEnum.Interval, SystemOutGeneratorsEnum.CapacityCurtailed, sSampleList);
                    break;
                case BATTNETGENERATION_PERIOD:
                    results = Utils.QryToListBattery(zip, SimulationPhaseEnum.STSchedule, PeriodEnum.Interval, SystemOutBatteriesEnum.NetGeneration, sSampleList);
                    break;
                case BATTGENERATION_PERIOD:
                    results = Utils.QryToListBattery(zip, SimulationPhaseEnum.STSchedule, PeriodEnum.Interval, SystemOutBatteriesEnum.Generation, sSampleList);
                    break;
                case BATTLOAD_PERIOD:
                    results = Utils.QryToListBattery(zip, SimulationPhaseEnum.STSchedule, PeriodEnum.Interval, SystemOutBatteriesEnum.Load, sSampleList);
                    break;
                case NODEPRICE_PERIOD:
                    results = Utils.QryToListNode(zip, SimulationPhaseEnum.STSchedule, PeriodEnum.Interval, SystemOutNodesEnum.Price, sSampleList);
                    break;
                case NODELOAD_PERIOD:
                    results = Utils.QryToListNode(zip, SimulationPhaseEnum.STSchedule, PeriodEnum.Interval, SystemOutNodesEnum.Load, sSampleList);
                    break;
                case LINEFLOW_PERIOD:
                    results = Utils.QryToListLine(zip, SimulationPhaseEnum.STSchedule, PeriodEnum.Interval, SystemOutLinesEnum.Flow, sSampleList);
                    break;
                case REGIONENS_PERIOD:
                    results = Utils.QryToListRegion(zip, SimulationPhaseEnum.STSchedule, PeriodEnum.Interval, SystemOutRegionsEnum.UnservedEnergy, sSampleList);
                    break;
                case RESERVESHORTAGE_PERIOD:
                    results = Utils.QryToListReserve(zip, SimulationPhaseEnum.STSchedule, PeriodEnum.Interval, SystemOutReservesEnum.Shortage, sSampleList);
                    break;
            }
            qryTime.Stop();
            Console.WriteLine($"Query time: {qryTime.ElapsedMilliseconds / 1000} sec.");
            if (results == null)
            {
                Console.WriteLine("No results found for the selected query");
                return 0;
            }

            //2--Write to stringbuilder:
            StringBuilder outLines = new StringBuilder();
            int count = 0;
            qryTime.Restart();

            outLines.AppendLine("NAME,SAMPLE,DATETIME,VALUE");
            File.WriteAllText(csvFilePath, outLines.ToString());
            outLines.Clear();
            
            foreach (var r in results)
            {
                if (intervalLength > 1)
                {
                    WriteCustomOutLine(outLines, r, USE_DATETIME_IN_CSV, intervalLength);
                }
                else
                {
                    WriteDefaultOutLine(outLines, r, USE_DATETIME_IN_CSV);
                }
                if (count % 1000000 == 0)
                {
                    File.AppendAllText(csvFilePath, outLines.ToString());
                    outLines.Clear();
                }
                count++;
            }
            File.AppendAllText(csvFilePath, outLines.ToString());
            outLines.Clear();

            qryTime.Stop();
            //Console.WriteLine($"Output preparation time: {qryTime.ElapsedMilliseconds / 1000.0} sec.");

            ////3--Write to file:
            //qryTime.Restart();
            //File.WriteAllText(csvFilePath, outLines.ToString());
            //qryTime.Stop();
            Console.WriteLine($"File {csvFilePath}. Write time: {qryTime.ElapsedMilliseconds / 1000} sec.");
            return 1;
        }

        private static void WriteCustomOutLine(StringBuilder outLines, SolutionResultItem r, bool useDateTimeColumn, int intervalLength)
        {
            if (useDateTimeColumn)
            {
                DateTime date = DateTime.Parse(r.date_string);
                for (int i = 0; i < intervalLength; i++)
                {
                    //outLines.AppendLine($"{r.child_name},{r.sample_id},{r.interval_id},{date.AddHours((double)i).ToOADate()},{Math.Round((double)r.value, VALUE_DECIMAL)}");
                    outLines.AppendLine($"{r.child_name},{r.sample_id},{date.AddHours((double)i)},{Math.Round((double)r.value, VALUE_DECIMAL)}");
                }
            }
            else
            {
                for (int i = 0; i < intervalLength; i++)
                {
                    int nInterval = (r.period_id - 1) * intervalLength + i;
                    outLines.AppendLine($"{r.child_name},{r.sample_id},{nInterval},{Math.Round((double)r.value, VALUE_DECIMAL)}");
                }
            }
        }

        private static void WriteDefaultOutLine(StringBuilder outLines, SolutionResultItem r, bool useDateTimeColumn)
        {
            if (useDateTimeColumn)
            {
                outLines.AppendLine($"{ r.child_id},{r.category_id},{r.date_string},{r.interval_id},{r.value}");
            } else
            {
                outLines.AppendLine($"{r.child_id},{r.category_id},{r.sample_id},{r.interval_id},{r.value}");
            }
        }

        private static void CreateCategoryIdMap(ClassEnum childClass, PLEXOS7_NET.Core.Solution zip, string csvFilePath)
        {
            StringBuilder outLines = new StringBuilder();
            outLines.AppendLine($"GENERATOR,CATEGORY,GENERATOR_ID,CATEGORY_ID");
            string[] categories = zip.GetCategories(childClass);
            if (categories == null) { return; }
            foreach (string category in categories)
            {
                string[] objs = zip.GetObjectsInCategory(childClass, category);
                if (objs != null)
                {
                    foreach (string obj in objs)
                    {
                        outLines.AppendLine(obj + "," + category);
                    }
                }
            }
            zip.GetDataTable("", "");
            File.WriteAllText(csvFilePath, outLines.ToString());
        }

        private static void CreatePeriod0IdMap(PLEXOS7_NET.Core.Solution zip, string csvFilePath)
        {
            string[] period_id = Utils.GetPeriod0Ids(zip);
            File.WriteAllLines(csvFilePath, period_id);
        }

        private static void CreateCustomPeriod0IdMap(PLEXOS7_NET.Core.Solution zip, string csvFilePath, int intervalLength)
        {
            File.WriteAllText(csvFilePath, "inverval_id, datetime");
            string[] period_id = Utils.GetCustomPeriod0Ids(zip, intervalLength);
            File.WriteAllLines(csvFilePath, period_id);
        }

        private static void CreateSTGeneratorCategoryIdMap(PLEXOS7_NET.Core.Solution zip, string csvFilePath)
        {
            SolutionResultList results = zip.QueryToList(SimulationPhaseEnum.STSchedule,
                  CollectionEnum.SystemGenerators,
                  "",
                  "",
                  PeriodEnum.FiscalYear,
                  SeriesTypeEnum.Properties,
                  "" + ((int)SystemOutGeneratorsEnum.Generation), null, null, null, "0");
            StringBuilder outLines = new StringBuilder();
            outLines.AppendLine($"GENERATOR,CATEGORY,GENERATOR_ID,CATEGORY_ID");
            foreach (var r in results)
            {
                if (r.interval_id > 1) { continue; }
                outLines.AppendLine($"{r.child_name},{r.category_name},{r.child_id},{r.category_id}");
            }
            File.WriteAllText(csvFilePath, outLines.ToString());
        }

        static void Main(string[] args)
        {

            //var zipFilePath = "D:/WORK/2022.CEN-crf/06.2025/Model RP_base4s Solution 8.300 R09 2022-09-22 02.33.26/Model RP_base4s Solution.zip";
            var zipFilePath = "D:/WORK/2022.CEN-crf/06.2025-critic/Model RP_critic4s Solution 8.300 R09 2022-09-22 11.03.33/Model RP_critic4s Solution.zip";

            
            //var zipFilePath = "D:/WORK/2022.CEN-crf/Model RP_Test Solution.zip";
            var csvFilePath = "D:/WORK/2022.CEN-crf";
            int nIntervalLength = 4;

            var zip = InitConnection(zipFilePath);


            var qryTime = new Stopwatch();
            qryTime.Start();

            //Figure out samples:
            int nFiles = 0;
            
            string sSampleList = Utils.GetSampleListSansMedia(zip);
            int nSamples = Utils.GetTotalSamples(zip);
            int[] nSampleIds = Utils.GetSampleIds(zip);

            //Create map files:
            //CreateSTGeneratorCategoryIdMap(zip, csvFilePath + Path.DirectorySeparatorChar + "category_id_map.csv");
            CreateCategoryIdMap(ClassEnum.Generator, zip, csvFilePath + Path.DirectorySeparatorChar + "category_id_map.csv");
            CreatePeriod0IdMap(zip, csvFilePath + Path.DirectorySeparatorChar + "t_period_0.csv");
            CreateCustomPeriod0IdMap(zip, csvFilePath + Path.DirectorySeparatorChar + "period_0_id_map.csv", nIntervalLength);

            //Execute queries:
            for (int i = 1; i < nSamples; i++)
            {
                //ExecuteTestQueries(zip, $"{csvFilePath}/genhr_{i}.csv", GENERATORGENERATION_PERIOD_ALLSAMPLES, ""+i, nIntervalLength);
                //ExecuteTestQueries(zip, $"{csvFilePath}/lineflowinterval_{i}s.csv", LINEFLOW_PERIOD_ALLSAMPLES, "" + i, nIntervalLength);
            }
            nFiles += ExecuteTestQueryToCsv(zip, $"{csvFilePath}/generatorgenerationinterval.csv", GENERATORGENERATION_PERIOD, "1,2,3,4", nIntervalLength);
            nFiles += ExecuteTestQueryToCsv(zip, $"{csvFilePath}/lineflowinterval.csv", LINEFLOW_PERIOD, "1,2,3,4", nIntervalLength);
            nFiles += ExecuteTestQueryToCsv(zip, $"{csvFilePath}/nodepriceinterval.csv", NODEPRICE_PERIOD, "1,2,3,4", nIntervalLength);

            qryTime.Stop();
            Console.WriteLine($"Finshed writing {nFiles} csv files. Total time {qryTime.ElapsedMilliseconds / 1000} sec.");

            zip.Close();
            Console.ReadKey();
        }





    }


}
