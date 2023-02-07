using EEUTILITY.Enums;
using EETDataFactory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QryOutput83
{
    class Utils
    {
        //public static String GetPropertyID (PLEXOS7_NET.Core.Solution zip, String property)
        //{
        //    var data = zip.GetSamplesToDataTable();
            
        //    return null;
        //}

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
