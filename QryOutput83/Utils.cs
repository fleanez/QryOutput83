using EEUTILITY.Enums;
using EETDataFactory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;

namespace QryOutput83
{
    class Utils
    {

        public static int GetTotalSamples(PLEXOS7_NET.Core.Solution zip)
        {
            var data = zip.GetSamplesToDataTable();
            return data.Rows.Count;
        }

        public static int[] GetSampleIds(PLEXOS7_NET.Core.Solution zip)
        {
            int nTotalSamples = GetTotalSamples(zip);
            int[] ids = new int[nTotalSamples];
            var data = zip.GetSamplesToDataTable();

            int nCount = 0;
            foreach (DataRow item in data.Rows)
            {
                ids[nCount] = (int)item[0];
                nCount++;
            }
            return ids;
        }

        public static string PeriodEnumToTableName(PeriodEnum period)
        {
            switch (period)
            {
                case PeriodEnum.Interval:
                    return "t_period_0";
                case PeriodEnum.Day:
                    return "t_period_1";
                case PeriodEnum.Week:
                    return "t_period_2";
                case PeriodEnum.Month:
                    return "t_period_3";
                case PeriodEnum.FiscalYear:
                    return "t_period_4";
                case PeriodEnum.Custom:
                    return "t_period_5";
                case PeriodEnum.Hour:
                    return "t_period_6";
                case PeriodEnum.Quarter:
                    return "t_period_7";
                case PeriodEnum.Block:
                    return "t_period_8";
                default:
                    return "t_period_0";
            }
        }

        public static string PeriodEnumToTableHeader(PeriodEnum period)
        {
            switch (period)
            {
                case PeriodEnum.Interval:
                    return "inteval_id";
                case PeriodEnum.Day:
                    return "day_id";
                case PeriodEnum.Week:
                    return "week_id";
                case PeriodEnum.Month:
                    return "month_id";
                case PeriodEnum.FiscalYear:
                    return "fiscal_year_id";
                case PeriodEnum.Custom:
                    return "interval_id";
                case PeriodEnum.Hour:
                    return "hour_id";
                case PeriodEnum.Quarter:
                    return "quarter_id";
                case PeriodEnum.Block:
                    return "block_id";
                default:
                    return "t_period_0";
            }
        }

        public static Dictionary<int, string> CreateTableDictionary(PLEXOS7_NET.Core.Solution zip, string tblName, string tblKeyColumn, string tblValueColumn)
        {
            Dictionary<int, string> lRet = new Dictionary<int, string>();
            var data = zip.GetDataTable(tblName, "");
            foreach (DataRow item in data.Rows)
            {
                lRet.Add((int)item[tblKeyColumn], item[tblValueColumn].ToString());
            }
            return lRet;
        }

        public static string[] GetTableContent(PLEXOS7_NET.Core.Solution zip, string tblName)
        {
            string tblHeader = "";
            var data = zip.GetDataTable(tblName, tblHeader);
            List<string> lVal = new List<string>();
            string[] headers = data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
            lVal.Add(String.Join(",", headers));
            foreach (DataRow item in data.Rows)
            {
                lVal.Add(String.Join(",", item.ItemArray));
            }
            return lVal.ToArray();
        }

        public static string[] GetObjectIds(PLEXOS7_NET.Core.Solution zip)
        {
            string strClassId;
            string strCategoryId;
            //Create temp Class Map:
            Dictionary<string, string> mapClass = new Dictionary<string, string>();
            var data = zip.GetDataTable("t_class", "class_group_id");
            foreach (DataRow item in data.Rows)
            {
                mapClass.Add(item["class_id"].ToString(), item["name"].ToString());
            }

            //Create temp Category Map:
            Dictionary<string, string> mapCategory = new Dictionary<string, string>();
            data = zip.GetDataTable("t_category", "");
            foreach (DataRow item in data.Rows)
            {
                strCategoryId = $"{item["category_id"]},{item["class_id"]}";
                mapCategory.Add(strCategoryId, item["name"].ToString());
            }

            //Finally, we create the object list:
            data = zip.GetDataTable("t_object", "");
            string[] headers = data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
            
            List<string> lVal = new List<string>();
            lVal.Add("class_id,category_id,category_name,object_id,object_name");
            
            foreach (DataRow item in data.Rows)
            {
                strClassId = item["class_id"].ToString();
                strCategoryId = $"{item["category_id"]},{item["class_id"]}";
                lVal.Add($"{mapClass[strClassId]},{item["category_id"]},{mapCategory[strCategoryId]},{item["object_id"]},{item["name"]}");
            }
            return lVal.ToArray();
        }

        public static string[] GetPeriodIds(PLEXOS7_NET.Core.Solution zip, PeriodEnum period)
        {
            string tblName = PeriodEnumToTableName(period);
            string tblHead = PeriodEnumToTableHeader(period);
            return GetTableContent(zip, tblName);
        }

        public static string[] GetPeriod0Ids(PLEXOS7_NET.Core.Solution zip)
        {
            var data = zip.GetDataTable("t_period_0","inteval_id");
            List<string> lVal = new List<string>();
            foreach (DataRow item in data.Rows)
            {
                lVal.Add(String.Join(",", item.ItemArray));
            }
            return lVal.ToArray(); 
        }

        public static string[] GetCustomPeriod0Ids(PLEXOS7_NET.Core.Solution zip, int intervalLength)
        {
            var data = zip.GetDataTable("t_period_0", "inteval_id");
            List<string> lVal = new List<string>();
            foreach (DataRow item in data.Rows)
            {
                for (int i = 0; i < intervalLength; i++)
                {
                    int nInterval = ((int)item["interval_id"] - 1) * intervalLength + i;
                    DateTime date = DateTime.Parse(item["datetime"].ToString());
                    lVal.Add($"{nInterval},{date.AddHours(i)}");
                }
            }
            return lVal.ToArray();
        }

        public static String GetSampleListSansMedia(PLEXOS7_NET.Core.Solution zip)
        {
            return String.Join(",", GetSampleIds(zip).Skip(1));
        }

        public static String CreateFileName(string phase_id, string collection, string property, string period_id, string sample_list, string aggregation)
        {
            string outputFile = $"{phase_id.ToLower()}_{collection.ToLower()}_{property.ToLower()}";
            if (!period_id.ToLower().Equals("interval",StringComparison.OrdinalIgnoreCase))
            {
                outputFile += "_" + period_id.ToLower();
            }
            if (sample_list.Contains(","))
            {
                outputFile += $"_({sample_list.ToLower()})";
            }
            if (!aggregation.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                outputFile += "_" + aggregation.ToLower();
            }
            return outputFile + ".csv";
        }

        //GENERATOR TEST QUERIES:
        public static SolutionResultList QryToListGenerator(PLEXOS7_NET.Core.Solution zip, SimulationPhaseEnum phase_id, PeriodEnum period_id, SystemOutGeneratorsEnum propertyList, string sSampleList=null)
        {
            return zip.QueryToList(phase_id,
                  EEUTILITY.Enums.CollectionEnum.SystemGenerators,
                  "",
                  "",
                  period_id,
                  EEUTILITY.Enums.SeriesTypeEnum.Properties,
                  "" + ((int)propertyList), null, null, null, sSampleList);
        }

        //BATTERIES TEST QUERIES:
        public static SolutionResultList QryToListBattery(PLEXOS7_NET.Core.Solution zip, SimulationPhaseEnum phase_id, PeriodEnum period_id, SystemOutBatteriesEnum propertyList, string sSampleList)
        {
            return zip.QueryToList(phase_id,
                  CollectionEnum.SystemBatteries,
                  "",
                  "",
                  period_id,
                  SeriesTypeEnum.Properties,
                  "" + ((int)propertyList), null, null, null, sSampleList);
        }

        //REGIONAL TEST QUERIES:
        public static SolutionResultList QryToListRegion(PLEXOS7_NET.Core.Solution zip, SimulationPhaseEnum phase_id, PeriodEnum period_id, SystemOutRegionsEnum propertyList,string sSampleList)
        {
            return zip.QueryToList(phase_id,
                  EEUTILITY.Enums.CollectionEnum.SystemRegions,
                  "",
                  "",
                  period_id,
                  EEUTILITY.Enums.SeriesTypeEnum.Properties,
                  "" + ((int)propertyList), null, null, null, sSampleList);
        }

        //NODAL TEST QUERIES:
        public static SolutionResultList QryToListNode(PLEXOS7_NET.Core.Solution zip, SimulationPhaseEnum phase_id, PeriodEnum period_id, SystemOutNodesEnum propertyList, string sSampleList)
        {
            return zip.QueryToList(phase_id,
                  EEUTILITY.Enums.CollectionEnum.SystemNodes,
                  "",
                  "",
                  period_id,
                  EEUTILITY.Enums.SeriesTypeEnum.Properties,
                  "" + ((int)propertyList), null, null, null, sSampleList);
        }

        //TRANSMISSION LINE TEST QUERIES:
        public static SolutionResultList QryToListLine(PLEXOS7_NET.Core.Solution zip, SimulationPhaseEnum phase_id, PeriodEnum period_id, SystemOutLinesEnum propertyList, string sSampleList)
        {
            return zip.QueryToList(phase_id,
                  CollectionEnum.SystemLines,
                  "",
                  "",
                  period_id,
                  SeriesTypeEnum.Properties,
                  "" + ((int)propertyList), null, null, null, sSampleList);
        }

        //RESERVE TEST QUERIES:
        public static SolutionResultList QryToListReserve(PLEXOS7_NET.Core.Solution zip, SimulationPhaseEnum phase_id, PeriodEnum period_id, SystemOutReservesEnum propertyList, string sSampleList)
        {
            return zip.QueryToList(phase_id,
                  CollectionEnum.SystemReserves,
                  "",
                  "",
                  period_id,
                  SeriesTypeEnum.Properties,
                  "" + ((int)propertyList), null, null, null, sSampleList);
        }

        public static SolutionResultList QryToListReserveGeneratorProvision(PLEXOS7_NET.Core.Solution zip, SimulationPhaseEnum phase_id, PeriodEnum period_id, string sSampleList)
        {
            return zip.QueryToList(phase_id,
                  CollectionEnum.ReserveGenerators,
                  "",
                  "",
                  period_id,
                  SeriesTypeEnum.Properties,
                  "" + ((int)OutReserveGeneratorsEnum.Provision), null, null, null, sSampleList);
        }

    }
}
