#define USE_DATETIMESTRING

using EETDataFactory;
using EEUTILITY.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

        public const string NONE = "";

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

        private static Dictionary<string, Dictionary<string, int>> mapOut = new Dictionary<string, Dictionary<string, int>>();

        private static Dictionary<CollectionEnum, ClassEnum> mapParentClass = new Dictionary<CollectionEnum, ClassEnum>();
        private static Dictionary<CollectionEnum, ClassEnum> mapChildClass = new Dictionary<CollectionEnum, ClassEnum>();
        private static Dictionary<string, CollectionEnum> mapCollectionEnums = new Dictionary<string, CollectionEnum>();
        private static Dictionary<string, SimulationPhaseEnum> mapPhaseEnums = new Dictionary<string, SimulationPhaseEnum>();
        private static Dictionary<string, PeriodEnum> mapPeriodEnums = new Dictionary<string, PeriodEnum>();

        private static Dictionary<string, int> mapGeneratorsEnums = new Dictionary<string, int>();
        private static Dictionary<string, int> mapBatteriesEnums = new Dictionary<string, int>();
        private static Dictionary<string, int> mapRegionsEnums = new Dictionary<string, int>();
        private static Dictionary<string, int> mapNodesEnums = new Dictionary<string, int>();
        private static Dictionary<string, int> mapLinesEnums = new Dictionary<string, int>();
        private static Dictionary<string, int> mapReservesEnums = new Dictionary<string, int>();
        private static Dictionary<string, int> mapGeneratorFuelsEnums = new Dictionary<string, int>();
        private static Dictionary<string, int> mapFuelsEnums = new Dictionary<string, int>();
        private static Dictionary<string, int> mapStoragesEnums = new Dictionary<string, int>();
        private static Dictionary<string, int> mapWaterwaysEnums = new Dictionary<string, int>();
        private static Dictionary<string, int> mapReserveGeneratorsEnums = new Dictionary<string, int>();
        private static Dictionary<string, int> mapZonesEnums = new Dictionary<string, int>();


        private static PLEXOS7_NET.Core.Solution InitConnection(string zipFilePath)
        {
            var zip = new PLEXOS7_NET.Core.Solution();
            zip.Connection(zipFilePath);
            return zip;
        }

        private static void InitDictionaries()
        {
            mapPhaseEnums.Add("st_schedule", SimulationPhaseEnum.STSchedule);
            mapPhaseEnums.Add("mt_schedule", SimulationPhaseEnum.MTSchedule);
            mapPhaseEnums.Add("pasa", SimulationPhaseEnum.PASA);
            mapPhaseEnums.Add("ltplan", SimulationPhaseEnum.LTPlan);
            
            mapPeriodEnums.Add("interval", PeriodEnum.Interval);
            mapPeriodEnums.Add("block", PeriodEnum.Block);
            mapPeriodEnums.Add("hour", PeriodEnum.Hour);
            mapPeriodEnums.Add("day", PeriodEnum.Day);
            mapPeriodEnums.Add("week", PeriodEnum.Week);
            mapPeriodEnums.Add("month", PeriodEnum.Month);
            mapPeriodEnums.Add("quarter", PeriodEnum.Quarter);
            mapPeriodEnums.Add("fiscal_year", PeriodEnum.FiscalYear);

            InitDictCollection();
            InitDictProperty();
        }

        private static void InitDictCollection()
        {

            //Map Parent Class:
            mapParentClass.Add(CollectionEnum.SystemGenerators, ClassEnum.System);
            mapParentClass.Add(CollectionEnum.SystemBatteries, ClassEnum.System);
            mapParentClass.Add(CollectionEnum.SystemRegions, ClassEnum.System);
            mapParentClass.Add(CollectionEnum.SystemNodes, ClassEnum.System);
            mapParentClass.Add(CollectionEnum.SystemLines, ClassEnum.System);
            mapParentClass.Add(CollectionEnum.SystemReserves, ClassEnum.System);
            mapParentClass.Add(CollectionEnum.GeneratorFuels, ClassEnum.Generator);
            mapParentClass.Add(CollectionEnum.SystemFuels, ClassEnum.System);
            mapParentClass.Add(CollectionEnum.SystemStorages, ClassEnum.System);
            mapParentClass.Add(CollectionEnum.SystemWaterways, ClassEnum.System);
            mapParentClass.Add(CollectionEnum.ReserveGenerators, ClassEnum.Reserve);
            mapParentClass.Add(CollectionEnum.SystemZones, ClassEnum.System);

            //Map Child Class:
            mapChildClass.Add(CollectionEnum.SystemGenerators, ClassEnum.Generator);
            mapChildClass.Add(CollectionEnum.SystemBatteries, ClassEnum.Battery);
            mapChildClass.Add(CollectionEnum.SystemRegions, ClassEnum.Region);
            mapChildClass.Add(CollectionEnum.SystemNodes, ClassEnum.Node);
            mapChildClass.Add(CollectionEnum.SystemLines, ClassEnum.Line);
            mapChildClass.Add(CollectionEnum.SystemReserves, ClassEnum.Reserve);
            mapChildClass.Add(CollectionEnum.GeneratorFuels, ClassEnum.Fuel);
            mapChildClass.Add(CollectionEnum.SystemFuels, ClassEnum.Fuel);
            mapChildClass.Add(CollectionEnum.SystemStorages, ClassEnum.Storage);
            mapChildClass.Add(CollectionEnum.SystemWaterways, ClassEnum.Waterway);
            mapChildClass.Add(CollectionEnum.ReserveGenerators, ClassEnum.Generator);
            mapChildClass.Add(CollectionEnum.SystemZones, ClassEnum.Zone);


            mapCollectionEnums.Add("generator", CollectionEnum.SystemGenerators);
            mapCollectionEnums.Add("battery", CollectionEnum.SystemBatteries);
            mapCollectionEnums.Add("region", CollectionEnum.SystemRegions);
            mapCollectionEnums.Add("node", CollectionEnum.SystemNodes);
            mapCollectionEnums.Add("line", CollectionEnum.SystemLines);
            mapCollectionEnums.Add("reserve", CollectionEnum.SystemReserves);
            mapCollectionEnums.Add("generator.fuels", CollectionEnum.GeneratorFuels);
            mapCollectionEnums.Add("fuel", CollectionEnum.SystemFuels);
            mapCollectionEnums.Add("storage", CollectionEnum.SystemStorages);
            mapCollectionEnums.Add("waterway", CollectionEnum.SystemWaterways);
            mapCollectionEnums.Add("reserve.generators", CollectionEnum.ReserveGenerators);
            mapCollectionEnums.Add("zone", CollectionEnum.SystemZones);

            mapCollectionEnums.Add("generators", CollectionEnum.SystemGenerators);
            mapCollectionEnums.Add("batteries", CollectionEnum.SystemBatteries);
            mapCollectionEnums.Add("regions", CollectionEnum.SystemRegions);
            mapCollectionEnums.Add("nodes", CollectionEnum.SystemNodes);
            mapCollectionEnums.Add("lines", CollectionEnum.SystemLines);
            mapCollectionEnums.Add("reserves", CollectionEnum.SystemReserves);
            mapCollectionEnums.Add("generatorfuels", CollectionEnum.GeneratorFuels);
            mapCollectionEnums.Add("fuels", CollectionEnum.SystemFuels);
            mapCollectionEnums.Add("storages", CollectionEnum.SystemStorages);
            mapCollectionEnums.Add("waterways", CollectionEnum.SystemWaterways);
            mapCollectionEnums.Add("reservegenerators", CollectionEnum.ReserveGenerators);
            mapCollectionEnums.Add("zones", CollectionEnum.SystemZones);

            mapCollectionEnums.Add("systemgenerators", CollectionEnum.SystemGenerators);
            mapCollectionEnums.Add("systembatteries", CollectionEnum.SystemBatteries);
            mapCollectionEnums.Add("systemregions", CollectionEnum.SystemRegions);
            mapCollectionEnums.Add("systemnodes", CollectionEnum.SystemNodes);
            mapCollectionEnums.Add("systemlines", CollectionEnum.SystemLines);
            mapCollectionEnums.Add("systemreserves", CollectionEnum.SystemReserves);
            //mapCollectionEnums.Add("generatorfuels", CollectionEnum.GeneratorFuels);
            mapCollectionEnums.Add("systemfuels", CollectionEnum.SystemFuels);
            mapCollectionEnums.Add("systemstorages", CollectionEnum.SystemStorages);
            mapCollectionEnums.Add("systemwaterways", CollectionEnum.SystemWaterways);
            //mapCollectionEnums.Add("reservegenerators", CollectionEnum.ReserveGenerators);
            mapCollectionEnums.Add("systemzones", CollectionEnum.SystemZones);

            mapOut.Add("generators", mapGeneratorsEnums);
            mapOut.Add("batteries", mapBatteriesEnums);
            mapOut.Add("regions", mapRegionsEnums);
            mapOut.Add("nodes", mapNodesEnums);
            mapOut.Add("lines", mapLinesEnums);
            mapOut.Add("reserves", mapReservesEnums);
            mapOut.Add("generatorfuels", mapGeneratorFuelsEnums);
            mapOut.Add("fuels", mapFuelsEnums);
            mapOut.Add("storages", mapStoragesEnums);
            mapOut.Add("waterways", mapWaterwaysEnums);
            mapOut.Add("reservegenerators", mapReserveGeneratorsEnums);
            mapOut.Add("zones", mapZonesEnums);
            
        }

        private static void InitDictProperty()
        {

            mapGeneratorsEnums.Add("abatement cost", (int)SystemOutGeneratorsEnum.AbatementCost);
            mapGeneratorsEnums.Add("age", (int)SystemOutGeneratorsEnum.Age);
            mapGeneratorsEnums.Add("annualized build cost", (int)SystemOutGeneratorsEnum.AnnualizedBuildCost);
            mapGeneratorsEnums.Add("auxiliary use", (int)SystemOutGeneratorsEnum.AuxiliaryUse);
            mapGeneratorsEnums.Add("available capacity", (int)SystemOutGeneratorsEnum.AvailableCapacity);
            mapGeneratorsEnums.Add("average cost", (int)SystemOutGeneratorsEnum.AverageCost);
            mapGeneratorsEnums.Add("average heat rate", (int)SystemOutGeneratorsEnum.AverageHeatRate);
            mapGeneratorsEnums.Add("average total cost", (int)SystemOutGeneratorsEnum.AverageTotalCost);
            mapGeneratorsEnums.Add("bid_cost mark_up", (int)SystemOutGeneratorsEnum.BidCostMarkup);
            mapGeneratorsEnums.Add("boiler fuel offtake", (int)SystemOutGeneratorsEnum.BoilerFuelOfftake);
            mapGeneratorsEnums.Add("boiler heat production", (int)SystemOutGeneratorsEnum.BoilerHeatProduction);
            mapGeneratorsEnums.Add("build cost", (int)SystemOutGeneratorsEnum.BuildCost);
            mapGeneratorsEnums.Add("capacity built", (int)SystemOutGeneratorsEnum.CapacityBuilt);
            mapGeneratorsEnums.Add("capacity curtailed", (int)SystemOutGeneratorsEnum.CapacityCurtailed);
            mapGeneratorsEnums.Add("capacity factor", (int)SystemOutGeneratorsEnum.CapacityFactor);
            mapGeneratorsEnums.Add("capacity price", (int)SystemOutGeneratorsEnum.CapacityPrice);
            mapGeneratorsEnums.Add("capacity reserves", (int)SystemOutGeneratorsEnum.CapacityReserves);
            mapGeneratorsEnums.Add("capacity retired", (int)SystemOutGeneratorsEnum.CapacityRetired);
            mapGeneratorsEnums.Add("capacity revenue", (int)SystemOutGeneratorsEnum.CapacityRevenue);
            mapGeneratorsEnums.Add("chp generation", (int)SystemOutGeneratorsEnum.CHPGeneration);
            mapGeneratorsEnums.Add("chp heat fuel offtake", (int)SystemOutGeneratorsEnum.CHPHeatFuelOfftake);
            mapGeneratorsEnums.Add("chp heat production", (int)SystemOutGeneratorsEnum.CHPHeatProduction);
            mapGeneratorsEnums.Add("chp heat surrogate fuel offtake", (int)SystemOutGeneratorsEnum.CHPHeatSurrogateFuelOfftake);
            mapGeneratorsEnums.Add("chp power fuel offtake", (int)SystemOutGeneratorsEnum.CHPPowerFuelOfftake);
            mapGeneratorsEnums.Add("clean spark spread", (int)SystemOutGeneratorsEnum.CleanSparkSpread);
            mapGeneratorsEnums.Add("cleared offer cost", (int)SystemOutGeneratorsEnum.ClearedOfferCost);
            mapGeneratorsEnums.Add("cleared offer price", (int)SystemOutGeneratorsEnum.ClearedOfferPrice);
            mapGeneratorsEnums.Add("cleared pump bid cost", (int)SystemOutGeneratorsEnum.ClearedPumpBidCost);
            mapGeneratorsEnums.Add("cleared pump bid price", (int)SystemOutGeneratorsEnum.ClearedPumpBidPrice);
            mapGeneratorsEnums.Add("cleared reserve offer cost", (int)SystemOutGeneratorsEnum.ClearedReserveOfferCost);
            mapGeneratorsEnums.Add("closing heat", (int)SystemOutGeneratorsEnum.ClosingHeat);
            mapGeneratorsEnums.Add("condense mode generation", (int)SystemOutGeneratorsEnum.CondenseModeGeneration);
            mapGeneratorsEnums.Add("constrained off revenue", (int)SystemOutGeneratorsEnum.ConstrainedOffRevenue);
            mapGeneratorsEnums.Add("constrained on revenue", (int)SystemOutGeneratorsEnum.ConstrainedOnRevenue);
            mapGeneratorsEnums.Add("cost price", (int)SystemOutGeneratorsEnum.CostPrice);
            mapGeneratorsEnums.Add("curtailment factor", (int)SystemOutGeneratorsEnum.CurtailmentFactor);
            mapGeneratorsEnums.Add("debt cost", (int)SystemOutGeneratorsEnum.DebtCost);
            mapGeneratorsEnums.Add("discrete maintenance", (int)SystemOutGeneratorsEnum.DiscreteMaintenance);
            mapGeneratorsEnums.Add("dispatchable capacity", (int)SystemOutGeneratorsEnum.DispatchableCapacity);
            mapGeneratorsEnums.Add("distributed maintenance", (int)SystemOutGeneratorsEnum.DistributedMaintenance);
            mapGeneratorsEnums.Add("efficiency", (int)SystemOutGeneratorsEnum.Efficiency);
            mapGeneratorsEnums.Add("emissions cost", (int)SystemOutGeneratorsEnum.EmissionsCost);
            mapGeneratorsEnums.Add("energy utilisation factor", (int)SystemOutGeneratorsEnum.EnergyUtilisationFactor);
            mapGeneratorsEnums.Add("equity cost", (int)SystemOutGeneratorsEnum.EquityCost);
            mapGeneratorsEnums.Add("firm capacity", (int)SystemOutGeneratorsEnum.FirmCapacity);
            mapGeneratorsEnums.Add("fixed costs", (int)SystemOutGeneratorsEnum.FixedCosts);
            mapGeneratorsEnums.Add("fixed load generation", (int)SystemOutGeneratorsEnum.FixedLoadGeneration);
            mapGeneratorsEnums.Add("fixed load violation", (int)SystemOutGeneratorsEnum.FixedLoadViolation);
            mapGeneratorsEnums.Add("fixed load violation cost", (int)SystemOutGeneratorsEnum.FixedLoadViolationCost);
            mapGeneratorsEnums.Add("fixed load violation hours", (int)SystemOutGeneratorsEnum.FixedLoadViolationHours);
            mapGeneratorsEnums.Add("fixed pump load", (int)SystemOutGeneratorsEnum.FixedPumpLoad);
            mapGeneratorsEnums.Add("fixed pump load violation", (int)SystemOutGeneratorsEnum.FixedPumpLoadViolation);
            mapGeneratorsEnums.Add("fixed pump load violation cost", (int)SystemOutGeneratorsEnum.FixedPumpLoadViolationCost);
            mapGeneratorsEnums.Add("fixed pump load violation hours", (int)SystemOutGeneratorsEnum.FixedPumpLoadViolationHours);
            mapGeneratorsEnums.Add("flexibility down", (int)SystemOutGeneratorsEnum.FlexibilityDown);
            mapGeneratorsEnums.Add("flexibility up", (int)SystemOutGeneratorsEnum.FlexibilityUp);
            mapGeneratorsEnums.Add("fo_m cost", (int)SystemOutGeneratorsEnum.FOMCost);
            mapGeneratorsEnums.Add("forced outage", (int)SystemOutGeneratorsEnum.ForcedOutage);
            mapGeneratorsEnums.Add("forced outage hours", (int)SystemOutGeneratorsEnum.ForcedOutageHours);
            mapGeneratorsEnums.Add("forced outage rate", (int)SystemOutGeneratorsEnum.ForcedOutageRate);
            mapGeneratorsEnums.Add("fuel cost", (int)SystemOutGeneratorsEnum.FuelCost);
            mapGeneratorsEnums.Add("fuel offtake", (int)SystemOutGeneratorsEnum.FuelOfftake);
            mapGeneratorsEnums.Add("fuel price", (int)SystemOutGeneratorsEnum.FuelPrice);
            mapGeneratorsEnums.Add("fuel transition cost", (int)SystemOutGeneratorsEnum.FuelTransitionCost);
            mapGeneratorsEnums.Add("fuel transport cost", (int)SystemOutGeneratorsEnum.FuelTransportCost);
            mapGeneratorsEnums.Add("generation", (int)SystemOutGeneratorsEnum.Generation);
            mapGeneratorsEnums.Add("generation cost", (int)SystemOutGeneratorsEnum.GenerationCost);
            mapGeneratorsEnums.Add("generation sent out", (int)SystemOutGeneratorsEnum.GenerationSentOut);
            mapGeneratorsEnums.Add("heat fuel offtake", (int)SystemOutGeneratorsEnum.HeatFuelOfftake);
            mapGeneratorsEnums.Add("heat injection", (int)SystemOutGeneratorsEnum.HeatInjection);
            mapGeneratorsEnums.Add("heat injection cost", (int)SystemOutGeneratorsEnum.HeatInjectionCost);
            mapGeneratorsEnums.Add("heat loss", (int)SystemOutGeneratorsEnum.HeatLoss);
            mapGeneratorsEnums.Add("heat market revenue", (int)SystemOutGeneratorsEnum.HeatMarketRevenue);
            mapGeneratorsEnums.Add("heat production", (int)SystemOutGeneratorsEnum.HeatProduction);
            mapGeneratorsEnums.Add("heat production cost", (int)SystemOutGeneratorsEnum.HeatProductionCost);
            mapGeneratorsEnums.Add("heat rate", (int)SystemOutGeneratorsEnum.HeatRate);
            mapGeneratorsEnums.Add("heat revenue", (int)SystemOutGeneratorsEnum.HeatRevenue);
            mapGeneratorsEnums.Add("heat shadow price", (int)SystemOutGeneratorsEnum.HeatShadowPrice);
            mapGeneratorsEnums.Add("heat withdrawal", (int)SystemOutGeneratorsEnum.HeatWithdrawal);
            mapGeneratorsEnums.Add("heat withdrawal cost", (int)SystemOutGeneratorsEnum.HeatWithdrawalCost);
            mapGeneratorsEnums.Add("hours at minimum", (int)SystemOutGeneratorsEnum.HoursatMinimum);
            mapGeneratorsEnums.Add("hours curtailed", (int)SystemOutGeneratorsEnum.HoursCurtailed);
            mapGeneratorsEnums.Add("hours down", (int)SystemOutGeneratorsEnum.HoursDown);
            mapGeneratorsEnums.Add("hours up", (int)SystemOutGeneratorsEnum.HoursUp);
            mapGeneratorsEnums.Add("inflexible generation", (int)SystemOutGeneratorsEnum.InflexibleGeneration);
            mapGeneratorsEnums.Add("installed capacity", (int)SystemOutGeneratorsEnum.InstalledCapacity);
            mapGeneratorsEnums.Add("levelized cost", (int)SystemOutGeneratorsEnum.LevelizedCost);
            mapGeneratorsEnums.Add("lower reserve", (int)SystemOutGeneratorsEnum.LowerReserve);
            mapGeneratorsEnums.Add("maintenance", (int)SystemOutGeneratorsEnum.Maintenance);
            mapGeneratorsEnums.Add("maintenance hours", (int)SystemOutGeneratorsEnum.MaintenanceHours);
            mapGeneratorsEnums.Add("maintenance rate", (int)SystemOutGeneratorsEnum.MaintenanceRate);
            mapGeneratorsEnums.Add("marginal fuel cost", (int)SystemOutGeneratorsEnum.MarginalFuelCost);
            mapGeneratorsEnums.Add("marginal heat rate", (int)SystemOutGeneratorsEnum.MarginalHeatRate);
            mapGeneratorsEnums.Add("marginal loss factor", (int)SystemOutGeneratorsEnum.MarginalLossFactor);
            mapGeneratorsEnums.Add("mark_up", (int)SystemOutGeneratorsEnum.Markup);
            mapGeneratorsEnums.Add("max capacity", (int)SystemOutGeneratorsEnum.MaxCapacity);
            mapGeneratorsEnums.Add("max energy violation", (int)SystemOutGeneratorsEnum.MaxEnergyViolation);
            mapGeneratorsEnums.Add("max energy violation cost", (int)SystemOutGeneratorsEnum.MaxEnergyViolationCost);
            mapGeneratorsEnums.Add("max generation", (int)SystemOutGeneratorsEnum.MaxGeneration);
            mapGeneratorsEnums.Add("max heat", (int)SystemOutGeneratorsEnum.MaxHeat);
            mapGeneratorsEnums.Add("max starts violation", (int)SystemOutGeneratorsEnum.MaxStartsViolation);
            mapGeneratorsEnums.Add("max starts violation cost", (int)SystemOutGeneratorsEnum.MaxStartsViolationCost);
            mapGeneratorsEnums.Add("min energy violation", (int)SystemOutGeneratorsEnum.MinEnergyViolation);
            mapGeneratorsEnums.Add("min energy violation cost", (int)SystemOutGeneratorsEnum.MinEnergyViolationCost);
            mapGeneratorsEnums.Add("min generation", (int)SystemOutGeneratorsEnum.MinGeneration);
            mapGeneratorsEnums.Add("min heat", (int)SystemOutGeneratorsEnum.MinHeat);
            mapGeneratorsEnums.Add("min load generation", (int)SystemOutGeneratorsEnum.MinLoadGeneration);
            mapGeneratorsEnums.Add("min load violation", (int)SystemOutGeneratorsEnum.MinLoadViolation);
            mapGeneratorsEnums.Add("min load violation cost", (int)SystemOutGeneratorsEnum.MinLoadViolationCost);
            mapGeneratorsEnums.Add("min load violation hours", (int)SystemOutGeneratorsEnum.MinLoadViolationHours);
            mapGeneratorsEnums.Add("minutes of ramp down", (int)SystemOutGeneratorsEnum.MinutesofRampDown);
            mapGeneratorsEnums.Add("minutes of ramp up", (int)SystemOutGeneratorsEnum.MinutesofRampUp);
            mapGeneratorsEnums.Add("monopoly rent", (int)SystemOutGeneratorsEnum.MonopolyRent);
            mapGeneratorsEnums.Add("net generation", (int)SystemOutGeneratorsEnum.NetGeneration);
            mapGeneratorsEnums.Add("net heat withdrawal", (int)SystemOutGeneratorsEnum.NetHeatWithdrawal);
            mapGeneratorsEnums.Add("net new capacity", (int)SystemOutGeneratorsEnum.NetNewCapacity);
            mapGeneratorsEnums.Add("net profit", (int)SystemOutGeneratorsEnum.NetProfit);
            mapGeneratorsEnums.Add("net reserves revenue", (int)SystemOutGeneratorsEnum.NetReservesRevenue);
            mapGeneratorsEnums.Add("net revenue", (int)SystemOutGeneratorsEnum.NetRevenue);
            mapGeneratorsEnums.Add("no cost capacity", (int)SystemOutGeneratorsEnum.NoCostCapacity);
            mapGeneratorsEnums.Add("offer base", (int)SystemOutGeneratorsEnum.OfferBase);
            mapGeneratorsEnums.Add("offer cleared", (int)SystemOutGeneratorsEnum.OfferCleared);
            mapGeneratorsEnums.Add("offer no load cost", (int)SystemOutGeneratorsEnum.OfferNoLoadCost);
            mapGeneratorsEnums.Add("offer price", (int)SystemOutGeneratorsEnum.OfferPrice);
            mapGeneratorsEnums.Add("offer quantity", (int)SystemOutGeneratorsEnum.OfferQuantity);
            mapGeneratorsEnums.Add("opening heat", (int)SystemOutGeneratorsEnum.OpeningHeat);
            mapGeneratorsEnums.Add("operating hours", (int)SystemOutGeneratorsEnum.OperatingHours);
            mapGeneratorsEnums.Add("pool revenue", (int)SystemOutGeneratorsEnum.PoolRevenue);
            mapGeneratorsEnums.Add("price received", (int)SystemOutGeneratorsEnum.PriceReceived);
            mapGeneratorsEnums.Add("pump bid base", (int)SystemOutGeneratorsEnum.PumpBidBase);
            mapGeneratorsEnums.Add("pump bid cleared", (int)SystemOutGeneratorsEnum.PumpBidCleared);
            mapGeneratorsEnums.Add("pump bid price", (int)SystemOutGeneratorsEnum.PumpBidPrice);
            mapGeneratorsEnums.Add("pump bid quantity", (int)SystemOutGeneratorsEnum.PumpBidQuantity);
            mapGeneratorsEnums.Add("pump cost", (int)SystemOutGeneratorsEnum.PumpCost);
            mapGeneratorsEnums.Add("pump load", (int)SystemOutGeneratorsEnum.PumpLoad);
            mapGeneratorsEnums.Add("pump operating hours", (int)SystemOutGeneratorsEnum.PumpOperatingHours);
            mapGeneratorsEnums.Add("pump price paid", (int)SystemOutGeneratorsEnum.PumpPricePaid);
            mapGeneratorsEnums.Add("pump units started", (int)SystemOutGeneratorsEnum.PumpUnitsStarted);
            mapGeneratorsEnums.Add("raise reserve", (int)SystemOutGeneratorsEnum.RaiseReserve);
            mapGeneratorsEnums.Add("ramp", (int)SystemOutGeneratorsEnum.Ramp);
            mapGeneratorsEnums.Add("ramp down", (int)SystemOutGeneratorsEnum.RampDown);
            mapGeneratorsEnums.Add("ramp down cost", (int)SystemOutGeneratorsEnum.RampDownCost);
            mapGeneratorsEnums.Add("ramp down price", (int)SystemOutGeneratorsEnum.RampDownPrice);
            mapGeneratorsEnums.Add("ramp down violation", (int)SystemOutGeneratorsEnum.RampDownViolation);
            mapGeneratorsEnums.Add("ramp down violation cost", (int)SystemOutGeneratorsEnum.RampDownViolationCost);
            mapGeneratorsEnums.Add("ramp down violation hours", (int)SystemOutGeneratorsEnum.RampDownViolationHours);
            mapGeneratorsEnums.Add("ramp flexibility down", (int)SystemOutGeneratorsEnum.RampFlexibilityDown);
            mapGeneratorsEnums.Add("ramp flexibility up", (int)SystemOutGeneratorsEnum.RampFlexibilityUp);
            mapGeneratorsEnums.Add("ramp up", (int)SystemOutGeneratorsEnum.RampUp);
            mapGeneratorsEnums.Add("ramp up cost", (int)SystemOutGeneratorsEnum.RampUpCost);
            mapGeneratorsEnums.Add("ramp up price", (int)SystemOutGeneratorsEnum.RampUpPrice);
            mapGeneratorsEnums.Add("ramp up violation", (int)SystemOutGeneratorsEnum.RampUpViolation);
            mapGeneratorsEnums.Add("ramp up violation cost", (int)SystemOutGeneratorsEnum.RampUpViolationCost);
            mapGeneratorsEnums.Add("ramp up violation hours", (int)SystemOutGeneratorsEnum.RampUpViolationHours);
            mapGeneratorsEnums.Add("rated capacity", (int)SystemOutGeneratorsEnum.RatedCapacity);
            mapGeneratorsEnums.Add("rating", (int)SystemOutGeneratorsEnum.Rating);
            mapGeneratorsEnums.Add("raw rating", (int)SystemOutGeneratorsEnum.RawRating);
            mapGeneratorsEnums.Add("regulation lower reserve", (int)SystemOutGeneratorsEnum.RegulationLowerReserve);
            mapGeneratorsEnums.Add("regulation raise reserve", (int)SystemOutGeneratorsEnum.RegulationRaiseReserve);
            mapGeneratorsEnums.Add("replacement reserve", (int)SystemOutGeneratorsEnum.ReplacementReserve);
            mapGeneratorsEnums.Add("reserves cost", (int)SystemOutGeneratorsEnum.ReservesCost);
            mapGeneratorsEnums.Add("reserves revenue", (int)SystemOutGeneratorsEnum.ReservesRevenue);
            mapGeneratorsEnums.Add("reserves vo_m cost", (int)SystemOutGeneratorsEnum.ReservesVOMCost);
            mapGeneratorsEnums.Add("retirement cost", (int)SystemOutGeneratorsEnum.RetirementCost);
            mapGeneratorsEnums.Add("scheduled generation", (int)SystemOutGeneratorsEnum.ScheduledGeneration);
            mapGeneratorsEnums.Add("scheduled generation cost", (int)SystemOutGeneratorsEnum.ScheduledGenerationCost);
            mapGeneratorsEnums.Add("scheduled offer cost", (int)SystemOutGeneratorsEnum.ScheduledOfferCost);
            mapGeneratorsEnums.Add("scheduled revenue", (int)SystemOutGeneratorsEnum.ScheduledRevenue);
            mapGeneratorsEnums.Add("scheduled start _ shutdown cost", (int)SystemOutGeneratorsEnum.ScheduledStartShutdownCost);
            mapGeneratorsEnums.Add("service factor", (int)SystemOutGeneratorsEnum.ServiceFactor);
            mapGeneratorsEnums.Add("shadow capacity built", (int)SystemOutGeneratorsEnum.ShadowCapacityBuilt);
            mapGeneratorsEnums.Add("shadow generation", (int)SystemOutGeneratorsEnum.ShadowGeneration);
            mapGeneratorsEnums.Add("shadow pool revenue", (int)SystemOutGeneratorsEnum.ShadowPoolRevenue);
            mapGeneratorsEnums.Add("shadow price received", (int)SystemOutGeneratorsEnum.ShadowPriceReceived);
            mapGeneratorsEnums.Add("spark spread", (int)SystemOutGeneratorsEnum.SparkSpread);
            mapGeneratorsEnums.Add("srmc", (int)SystemOutGeneratorsEnum.SRMC);
            mapGeneratorsEnums.Add("start _ shutdown cost", (int)SystemOutGeneratorsEnum.StartShutdownCost);
            mapGeneratorsEnums.Add("start _ shutdown penalty cost", (int)SystemOutGeneratorsEnum.StartShutdownPenaltyCost);
            mapGeneratorsEnums.Add("start fuel cost", (int)SystemOutGeneratorsEnum.StartFuelCost);
            mapGeneratorsEnums.Add("start fuel offtake", (int)SystemOutGeneratorsEnum.StartFuelOfftake);
            mapGeneratorsEnums.Add("strategic shadow price", (int)SystemOutGeneratorsEnum.StrategicShadowPrice);
            mapGeneratorsEnums.Add("sync cond cost", (int)SystemOutGeneratorsEnum.SyncCondCost);
            mapGeneratorsEnums.Add("sync cond load", (int)SystemOutGeneratorsEnum.SyncCondLoad);
            mapGeneratorsEnums.Add("sync cond operating hours", (int)SystemOutGeneratorsEnum.SyncCondOperatingHours);
            mapGeneratorsEnums.Add("sync cond price paid", (int)SystemOutGeneratorsEnum.SyncCondPricePaid);
            mapGeneratorsEnums.Add("total cost", (int)SystemOutGeneratorsEnum.TotalCost);
            mapGeneratorsEnums.Add("total generation cost", (int)SystemOutGeneratorsEnum.TotalGenerationCost);
            mapGeneratorsEnums.Add("total system cost", (int)SystemOutGeneratorsEnum.TotalSystemCost);
            mapGeneratorsEnums.Add("undispatched capacity", (int)SystemOutGeneratorsEnum.UndispatchedCapacity);
            mapGeneratorsEnums.Add("units", (int)SystemOutGeneratorsEnum.Units);
            mapGeneratorsEnums.Add("units built", (int)SystemOutGeneratorsEnum.UnitsBuilt);
            mapGeneratorsEnums.Add("units generating", (int)SystemOutGeneratorsEnum.UnitsGenerating);
            mapGeneratorsEnums.Add("units out", (int)SystemOutGeneratorsEnum.UnitsOut);
            mapGeneratorsEnums.Add("units pumping", (int)SystemOutGeneratorsEnum.UnitsPumping);
            mapGeneratorsEnums.Add("units retired", (int)SystemOutGeneratorsEnum.UnitsRetired);
            mapGeneratorsEnums.Add("units shutdown", (int)SystemOutGeneratorsEnum.UnitsShutdown);
            mapGeneratorsEnums.Add("units started", (int)SystemOutGeneratorsEnum.UnitsStarted);
            mapGeneratorsEnums.Add("units sync cond", (int)SystemOutGeneratorsEnum.UnitsSyncCond);
            mapGeneratorsEnums.Add("uos cost", (int)SystemOutGeneratorsEnum.UoSCost);
            mapGeneratorsEnums.Add("vo_m charge", (int)SystemOutGeneratorsEnum.VOMCharge);
            mapGeneratorsEnums.Add("vo_m cost", (int)SystemOutGeneratorsEnum.VOMCost);
            mapGeneratorsEnums.Add("waste heat", (int)SystemOutGeneratorsEnum.WasteHeat);
            mapGeneratorsEnums.Add("water consumption", (int)SystemOutGeneratorsEnum.WaterConsumption);
            mapGeneratorsEnums.Add("water cost", (int)SystemOutGeneratorsEnum.WaterCost);
            mapGeneratorsEnums.Add("water offtake", (int)SystemOutGeneratorsEnum.WaterOfftake);
            mapGeneratorsEnums.Add("water price paid", (int)SystemOutGeneratorsEnum.WaterPricePaid);
            mapGeneratorsEnums.Add("water pumped", (int)SystemOutGeneratorsEnum.WaterPumped);
            mapGeneratorsEnums.Add("water release", (int)SystemOutGeneratorsEnum.WaterRelease);
            mapGeneratorsEnums.Add("x", (int)SystemOutGeneratorsEnum.x);
            mapGeneratorsEnums.Add("y", (int)SystemOutGeneratorsEnum.y);
            mapGeneratorsEnums.Add("z", (int)SystemOutGeneratorsEnum.z);



            mapGeneratorFuelsEnums.Add("cost", (int)OutGeneratorFuelsEnum.Cost);
            mapGeneratorFuelsEnums.Add("cost price", (int)OutGeneratorFuelsEnum.CostPrice);
            mapGeneratorFuelsEnums.Add("generation", (int)OutGeneratorFuelsEnum.Generation);
            mapGeneratorFuelsEnums.Add("hours available", (int)OutGeneratorFuelsEnum.HoursAvailable);
            mapGeneratorFuelsEnums.Add("hours in use", (int)OutGeneratorFuelsEnum.HoursinUse);
            mapGeneratorFuelsEnums.Add("marginal heat rate", (int)OutGeneratorFuelsEnum.MarginalHeatRate);
            mapGeneratorFuelsEnums.Add("offer cleared", (int)OutGeneratorFuelsEnum.OfferCleared);
            mapGeneratorFuelsEnums.Add("offer price", (int)OutGeneratorFuelsEnum.OfferPrice);
            mapGeneratorFuelsEnums.Add("offer quantity", (int)OutGeneratorFuelsEnum.OfferQuantity);
            mapGeneratorFuelsEnums.Add("offtake", (int)OutGeneratorFuelsEnum.Offtake);
            mapGeneratorFuelsEnums.Add("offtake ratio", (int)OutGeneratorFuelsEnum.OfftakeRatio);
            mapGeneratorFuelsEnums.Add("price", (int)OutGeneratorFuelsEnum.Price);
            mapGeneratorFuelsEnums.Add("srmc", (int)OutGeneratorFuelsEnum.SRMC);
            mapGeneratorFuelsEnums.Add("transition cost", (int)OutGeneratorFuelsEnum.TransitionCost);
            mapGeneratorFuelsEnums.Add("transport charge", (int)OutGeneratorFuelsEnum.TransportCharge);
            mapGeneratorFuelsEnums.Add("transport cost", (int)OutGeneratorFuelsEnum.TransportCost);



            mapFuelsEnums.Add("average heat rate", (int)SystemOutFuelsEnum.AverageHeatRate);
            mapFuelsEnums.Add("closing inventory", (int)SystemOutFuelsEnum.ClosingInventory);
            mapFuelsEnums.Add("cost", (int)SystemOutFuelsEnum.Cost);
            mapFuelsEnums.Add("delivery", (int)SystemOutFuelsEnum.Delivery);
            mapFuelsEnums.Add("delivery cost", (int)SystemOutFuelsEnum.DeliveryCost);
            mapFuelsEnums.Add("fo_m cost", (int)SystemOutFuelsEnum.FOMCost);
            mapFuelsEnums.Add("generation", (int)SystemOutFuelsEnum.Generation);
            mapFuelsEnums.Add("installed capacity", (int)SystemOutFuelsEnum.InstalledCapacity);
            mapFuelsEnums.Add("inventory cost", (int)SystemOutFuelsEnum.InventoryCost);
            mapFuelsEnums.Add("max inventory", (int)SystemOutFuelsEnum.MaxInventory);
            mapFuelsEnums.Add("max offtake", (int)SystemOutFuelsEnum.MaxOfftake);
            mapFuelsEnums.Add("min inventory", (int)SystemOutFuelsEnum.MinInventory);
            mapFuelsEnums.Add("min offtake", (int)SystemOutFuelsEnum.MinOfftake);
            mapFuelsEnums.Add("net withdrawal", (int)SystemOutFuelsEnum.NetWithdrawal);
            mapFuelsEnums.Add("offtake", (int)SystemOutFuelsEnum.Offtake);
            mapFuelsEnums.Add("opening inventory", (int)SystemOutFuelsEnum.OpeningInventory);
            mapFuelsEnums.Add("price", (int)SystemOutFuelsEnum.Price);
            mapFuelsEnums.Add("reservation cost", (int)SystemOutFuelsEnum.ReservationCost);
            mapFuelsEnums.Add("shadow price", (int)SystemOutFuelsEnum.ShadowPrice);
            mapFuelsEnums.Add("tax", (int)SystemOutFuelsEnum.Tax);
            mapFuelsEnums.Add("tax cost", (int)SystemOutFuelsEnum.TaxCost);
            mapFuelsEnums.Add("time_weighted price", (int)SystemOutFuelsEnum.TimeweightedPrice);
            mapFuelsEnums.Add("total cost", (int)SystemOutFuelsEnum.TotalCost);
            mapFuelsEnums.Add("total price", (int)SystemOutFuelsEnum.TotalPrice);
            mapFuelsEnums.Add("withdrawal", (int)SystemOutFuelsEnum.Withdrawal);
            mapFuelsEnums.Add("withdrawal cost", (int)SystemOutFuelsEnum.WithdrawalCost);
            mapFuelsEnums.Add("x", (int)SystemOutFuelsEnum.x);
            mapFuelsEnums.Add("y", (int)SystemOutFuelsEnum.y);
            mapFuelsEnums.Add("z", (int)SystemOutFuelsEnum.z);



            mapStoragesEnums.Add("average utilization", (int)SystemOutStoragesEnum.AverageUtilization);
            mapStoragesEnums.Add("average working utilization", (int)SystemOutStoragesEnum.AverageWorkingUtilization);
            mapStoragesEnums.Add("downstream release", (int)SystemOutStoragesEnum.DownstreamRelease);
            mapStoragesEnums.Add("downstream release rate", (int)SystemOutStoragesEnum.DownstreamReleaseRate);
            mapStoragesEnums.Add("efficiency", (int)SystemOutStoragesEnum.Efficiency);
            mapStoragesEnums.Add("end level", (int)SystemOutStoragesEnum.EndLevel);
            mapStoragesEnums.Add("end volume", (int)SystemOutStoragesEnum.EndVolume);
            mapStoragesEnums.Add("generation", (int)SystemOutStoragesEnum.Generation);
            mapStoragesEnums.Add("generator release", (int)SystemOutStoragesEnum.GeneratorRelease);
            mapStoragesEnums.Add("generator release rate", (int)SystemOutStoragesEnum.GeneratorReleaseRate);
            mapStoragesEnums.Add("hours full", (int)SystemOutStoragesEnum.HoursFull);
            mapStoragesEnums.Add("inflow", (int)SystemOutStoragesEnum.Inflow);
            mapStoragesEnums.Add("inflow rate", (int)SystemOutStoragesEnum.InflowRate);
            mapStoragesEnums.Add("initial level", (int)SystemOutStoragesEnum.InitialLevel);
            mapStoragesEnums.Add("initial volume", (int)SystemOutStoragesEnum.InitialVolume);
            mapStoragesEnums.Add("loss", (int)SystemOutStoragesEnum.Loss);
            mapStoragesEnums.Add("marginal cost", (int)SystemOutStoragesEnum.MarginalCost);
            mapStoragesEnums.Add("marginal value", (int)SystemOutStoragesEnum.MarginalValue);
            mapStoragesEnums.Add("max release violation", (int)SystemOutStoragesEnum.MaxReleaseViolation);
            mapStoragesEnums.Add("max release violation cost", (int)SystemOutStoragesEnum.MaxReleaseViolationCost);
            mapStoragesEnums.Add("max release violation hours", (int)SystemOutStoragesEnum.MaxReleaseViolationHours);
            mapStoragesEnums.Add("max volume", (int)SystemOutStoragesEnum.MaxVolume);
            mapStoragesEnums.Add("min release violation", (int)SystemOutStoragesEnum.MinReleaseViolation);
            mapStoragesEnums.Add("min release violation cost", (int)SystemOutStoragesEnum.MinReleaseViolationCost);
            mapStoragesEnums.Add("min release violation hours", (int)SystemOutStoragesEnum.MinReleaseViolationHours);
            mapStoragesEnums.Add("min volume", (int)SystemOutStoragesEnum.MinVolume);
            mapStoragesEnums.Add("natural inflow", (int)SystemOutStoragesEnum.NaturalInflow);
            mapStoragesEnums.Add("natural inflow rate", (int)SystemOutStoragesEnum.NaturalInflowRate);
            mapStoragesEnums.Add("non_physical inflow", (int)SystemOutStoragesEnum.NonphysicalInflow);
            mapStoragesEnums.Add("non_physical spill", (int)SystemOutStoragesEnum.NonphysicalSpill);
            mapStoragesEnums.Add("potential energy", (int)SystemOutStoragesEnum.PotentialEnergy);
            mapStoragesEnums.Add("pump load", (int)SystemOutStoragesEnum.PumpLoad);
            mapStoragesEnums.Add("ramp", (int)SystemOutStoragesEnum.Ramp);
            mapStoragesEnums.Add("ramp price", (int)SystemOutStoragesEnum.RampPrice);
            mapStoragesEnums.Add("ramp violation", (int)SystemOutStoragesEnum.RampViolation);
            mapStoragesEnums.Add("ramp violation cost", (int)SystemOutStoragesEnum.RampViolationCost);
            mapStoragesEnums.Add("ramp violation hours", (int)SystemOutStoragesEnum.RampViolationHours);
            mapStoragesEnums.Add("release", (int)SystemOutStoragesEnum.Release);
            mapStoragesEnums.Add("release rate", (int)SystemOutStoragesEnum.ReleaseRate);
            mapStoragesEnums.Add("shadow price", (int)SystemOutStoragesEnum.ShadowPrice);
            mapStoragesEnums.Add("spill", (int)SystemOutStoragesEnum.Spill);
            mapStoragesEnums.Add("spill rate", (int)SystemOutStoragesEnum.SpillRate);
            mapStoragesEnums.Add("target violation", (int)SystemOutStoragesEnum.TargetViolation);
            mapStoragesEnums.Add("target violation cost", (int)SystemOutStoragesEnum.TargetViolationCost);
            mapStoragesEnums.Add("utilization", (int)SystemOutStoragesEnum.Utilization);
            mapStoragesEnums.Add("working utilization", (int)SystemOutStoragesEnum.WorkingUtilization);
            mapStoragesEnums.Add("working volume", (int)SystemOutStoragesEnum.WorkingVolume);
            mapStoragesEnums.Add("x", (int)SystemOutStoragesEnum.x);
            mapStoragesEnums.Add("y", (int)SystemOutStoragesEnum.y);
            mapStoragesEnums.Add("z", (int)SystemOutStoragesEnum.z);



            mapWaterwaysEnums.Add("flow", (int)SystemOutWaterwaysEnum.Flow);
            mapWaterwaysEnums.Add("hours flowing", (int)SystemOutWaterwaysEnum.HoursFlowing);
            mapWaterwaysEnums.Add("max flow", (int)SystemOutWaterwaysEnum.MaxFlow);
            mapWaterwaysEnums.Add("max flow violation", (int)SystemOutWaterwaysEnum.MaxFlowViolation);
            mapWaterwaysEnums.Add("max flow violation cost", (int)SystemOutWaterwaysEnum.MaxFlowViolationCost);
            mapWaterwaysEnums.Add("max flow violation hours", (int)SystemOutWaterwaysEnum.MaxFlowViolationHours);
            mapWaterwaysEnums.Add("max ramp", (int)SystemOutWaterwaysEnum.MaxRamp);
            mapWaterwaysEnums.Add("min flow", (int)SystemOutWaterwaysEnum.MinFlow);
            mapWaterwaysEnums.Add("min flow violation", (int)SystemOutWaterwaysEnum.MinFlowViolation);
            mapWaterwaysEnums.Add("min flow violation cost", (int)SystemOutWaterwaysEnum.MinFlowViolationCost);
            mapWaterwaysEnums.Add("min flow violation hours", (int)SystemOutWaterwaysEnum.MinFlowViolationHours);
            mapWaterwaysEnums.Add("ramp", (int)SystemOutWaterwaysEnum.Ramp);
            mapWaterwaysEnums.Add("ramp violation", (int)SystemOutWaterwaysEnum.RampViolation);
            mapWaterwaysEnums.Add("ramp violation cost", (int)SystemOutWaterwaysEnum.RampViolationCost);
            mapWaterwaysEnums.Add("ramp violation hours", (int)SystemOutWaterwaysEnum.RampViolationHours);
            mapWaterwaysEnums.Add("shadow price", (int)SystemOutWaterwaysEnum.ShadowPrice);
            mapWaterwaysEnums.Add("x", (int)SystemOutWaterwaysEnum.x);
            mapWaterwaysEnums.Add("y", (int)SystemOutWaterwaysEnum.y);
            mapWaterwaysEnums.Add("z", (int)SystemOutWaterwaysEnum.z);



            mapReservesEnums.Add("available response", (int)SystemOutReservesEnum.AvailableResponse);
            mapReservesEnums.Add("cleared offer cost", (int)SystemOutReservesEnum.ClearedOfferCost);
            mapReservesEnums.Add("cleared offer price", (int)SystemOutReservesEnum.ClearedOfferPrice);
            mapReservesEnums.Add("cost", (int)SystemOutReservesEnum.Cost);
            mapReservesEnums.Add("price", (int)SystemOutReservesEnum.Price);
            mapReservesEnums.Add("provision", (int)SystemOutReservesEnum.Provision);
            mapReservesEnums.Add("risk", (int)SystemOutReservesEnum.Risk);
            mapReservesEnums.Add("shortage", (int)SystemOutReservesEnum.Shortage);
            mapReservesEnums.Add("shortage cost", (int)SystemOutReservesEnum.ShortageCost);
            mapReservesEnums.Add("shortage hours", (int)SystemOutReservesEnum.ShortageHours);
            mapReservesEnums.Add("time_weighted price", (int)SystemOutReservesEnum.TimeweightedPrice);
            mapReservesEnums.Add("x", (int)SystemOutReservesEnum.x);
            mapReservesEnums.Add("y", (int)SystemOutReservesEnum.y);
            mapReservesEnums.Add("z", (int)SystemOutReservesEnum.z);



            mapReserveGeneratorsEnums.Add("available response", (int)OutReserveGeneratorsEnum.AvailableResponse);
            mapReserveGeneratorsEnums.Add("cleared offer cost", (int)OutReserveGeneratorsEnum.ClearedOfferCost);
            mapReserveGeneratorsEnums.Add("cleared offer price", (int)OutReserveGeneratorsEnum.ClearedOfferPrice);
            mapReserveGeneratorsEnums.Add("non_spinning reserve provision", (int)OutReserveGeneratorsEnum.NonspinningReserveProvision);
            mapReserveGeneratorsEnums.Add("provision", (int)OutReserveGeneratorsEnum.Provision);
            mapReserveGeneratorsEnums.Add("pump dispatchable load provision", (int)OutReserveGeneratorsEnum.PumpDispatchableLoadProvision);
            mapReserveGeneratorsEnums.Add("revenue", (int)OutReserveGeneratorsEnum.Revenue);
            mapReserveGeneratorsEnums.Add("spinning reserve provision", (int)OutReserveGeneratorsEnum.SpinningReserveProvision);
            mapReserveGeneratorsEnums.Add("sync cond reserve provision", (int)OutReserveGeneratorsEnum.SyncCondReserveProvision);



            mapBatteriesEnums.Add("age", (int)SystemOutBatteriesEnum.Age);
            mapBatteriesEnums.Add("available soc", (int)SystemOutBatteriesEnum.AvailableSoC);
            mapBatteriesEnums.Add("build cost", (int)SystemOutBatteriesEnum.BuildCost);
            mapBatteriesEnums.Add("capacity factor", (int)SystemOutBatteriesEnum.CapacityFactor);
            mapBatteriesEnums.Add("cleared reserve offer cost", (int)SystemOutBatteriesEnum.ClearedReserveOfferCost);
            mapBatteriesEnums.Add("cost to load", (int)SystemOutBatteriesEnum.CosttoLoad);
            mapBatteriesEnums.Add("discrete maintenance", (int)SystemOutBatteriesEnum.DiscreteMaintenance);
            mapBatteriesEnums.Add("distributed maintenance", (int)SystemOutBatteriesEnum.DistributedMaintenance);
            mapBatteriesEnums.Add("energy", (int)SystemOutBatteriesEnum.Energy);
            mapBatteriesEnums.Add("firm capacity", (int)SystemOutBatteriesEnum.FirmCapacity);
            mapBatteriesEnums.Add("fo_m cost", (int)SystemOutBatteriesEnum.FOMCost);
            mapBatteriesEnums.Add("generation", (int)SystemOutBatteriesEnum.Generation);
            mapBatteriesEnums.Add("generation revenue", (int)SystemOutBatteriesEnum.GenerationRevenue);
            mapBatteriesEnums.Add("hours charging", (int)SystemOutBatteriesEnum.HoursCharging);
            mapBatteriesEnums.Add("hours discharging", (int)SystemOutBatteriesEnum.HoursDischarging);
            mapBatteriesEnums.Add("hours idle", (int)SystemOutBatteriesEnum.HoursIdle);
            mapBatteriesEnums.Add("installed capacity", (int)SystemOutBatteriesEnum.InstalledCapacity);
            mapBatteriesEnums.Add("load", (int)SystemOutBatteriesEnum.Load);
            mapBatteriesEnums.Add("load factor", (int)SystemOutBatteriesEnum.LoadFactor);
            mapBatteriesEnums.Add("losses", (int)SystemOutBatteriesEnum.Losses);
            mapBatteriesEnums.Add("lower reserve", (int)SystemOutBatteriesEnum.LowerReserve);
            mapBatteriesEnums.Add("maintenance", (int)SystemOutBatteriesEnum.Maintenance);
            mapBatteriesEnums.Add("net generation", (int)SystemOutBatteriesEnum.NetGeneration);
            mapBatteriesEnums.Add("net generation revenue", (int)SystemOutBatteriesEnum.NetGenerationRevenue);
            mapBatteriesEnums.Add("net profit", (int)SystemOutBatteriesEnum.NetProfit);
            mapBatteriesEnums.Add("price paid", (int)SystemOutBatteriesEnum.PricePaid);
            mapBatteriesEnums.Add("price received", (int)SystemOutBatteriesEnum.PriceReceived);
            mapBatteriesEnums.Add("raise reserve", (int)SystemOutBatteriesEnum.RaiseReserve);
            mapBatteriesEnums.Add("regulation lower reserve", (int)SystemOutBatteriesEnum.RegulationLowerReserve);
            mapBatteriesEnums.Add("regulation raise reserve", (int)SystemOutBatteriesEnum.RegulationRaiseReserve);
            mapBatteriesEnums.Add("replacement reserve", (int)SystemOutBatteriesEnum.ReplacementReserve);
            mapBatteriesEnums.Add("reserves revenue", (int)SystemOutBatteriesEnum.ReservesRevenue);
            mapBatteriesEnums.Add("retirement cost", (int)SystemOutBatteriesEnum.RetirementCost);
            mapBatteriesEnums.Add("shadow price", (int)SystemOutBatteriesEnum.ShadowPrice);
            mapBatteriesEnums.Add("soc", (int)SystemOutBatteriesEnum.SoC);
            mapBatteriesEnums.Add("units", (int)SystemOutBatteriesEnum.Units);
            mapBatteriesEnums.Add("units built", (int)SystemOutBatteriesEnum.UnitsBuilt);
            mapBatteriesEnums.Add("units retired", (int)SystemOutBatteriesEnum.UnitsRetired);
            mapBatteriesEnums.Add("uos cost", (int)SystemOutBatteriesEnum.UoSCost);
            mapBatteriesEnums.Add("vo_m cost", (int)SystemOutBatteriesEnum.VOMCost);
            mapBatteriesEnums.Add("x", (int)SystemOutBatteriesEnum.x);
            mapBatteriesEnums.Add("y", (int)SystemOutBatteriesEnum.y);
            mapBatteriesEnums.Add("z", (int)SystemOutBatteriesEnum.z);



            mapRegionsEnums.Add("abatement cost", (int)SystemOutRegionsEnum.AbatementCost);
            mapRegionsEnums.Add("annualized build cost", (int)SystemOutRegionsEnum.AnnualizedBuildCost);
            mapRegionsEnums.Add("available capacity", (int)SystemOutRegionsEnum.AvailableCapacity);
            mapRegionsEnums.Add("available capacity margin", (int)SystemOutRegionsEnum.AvailableCapacityMargin);
            mapRegionsEnums.Add("available capacity reserves", (int)SystemOutRegionsEnum.AvailableCapacityReserves);
            mapRegionsEnums.Add("balancing area interchange exports", (int)SystemOutRegionsEnum.BalancingAreaInterchangeExports);
            mapRegionsEnums.Add("balancing area interchange exports revenue", (int)SystemOutRegionsEnum.BalancingAreaInterchangeExportsRevenue);
            mapRegionsEnums.Add("balancing area interchange imports", (int)SystemOutRegionsEnum.BalancingAreaInterchangeImports);
            mapRegionsEnums.Add("balancing area interchange imports cost", (int)SystemOutRegionsEnum.BalancingAreaInterchangeImportsCost);
            mapRegionsEnums.Add("battery generation", (int)SystemOutRegionsEnum.BatteryGeneration);
            mapRegionsEnums.Add("battery load", (int)SystemOutRegionsEnum.BatteryLoad);
            mapRegionsEnums.Add("capacity excess", (int)SystemOutRegionsEnum.CapacityExcess);
            mapRegionsEnums.Add("capacity excess cost", (int)SystemOutRegionsEnum.CapacityExcessCost);
            mapRegionsEnums.Add("capacity payments", (int)SystemOutRegionsEnum.CapacityPayments);
            mapRegionsEnums.Add("capacity price", (int)SystemOutRegionsEnum.CapacityPrice);
            mapRegionsEnums.Add("capacity reserve margin", (int)SystemOutRegionsEnum.CapacityReserveMargin);
            mapRegionsEnums.Add("capacity reserves", (int)SystemOutRegionsEnum.CapacityReserves);
            mapRegionsEnums.Add("capacity shadow price", (int)SystemOutRegionsEnum.CapacityShadowPrice);
            mapRegionsEnums.Add("capacity shortage", (int)SystemOutRegionsEnum.CapacityShortage);
            mapRegionsEnums.Add("capacity shortage cost", (int)SystemOutRegionsEnum.CapacityShortageCost);
            mapRegionsEnums.Add("charging station deferred load", (int)SystemOutRegionsEnum.ChargingStationDeferredLoad);
            mapRegionsEnums.Add("charging station generation", (int)SystemOutRegionsEnum.ChargingStationGeneration);
            mapRegionsEnums.Add("charging station hours deferred", (int)SystemOutRegionsEnum.ChargingStationHoursDeferred);
            mapRegionsEnums.Add("charging station load", (int)SystemOutRegionsEnum.ChargingStationLoad);
            mapRegionsEnums.Add("cleared dsp bid cost", (int)SystemOutRegionsEnum.ClearedDSPBidCost);
            mapRegionsEnums.Add("cleared dsp bid price", (int)SystemOutRegionsEnum.ClearedDSPBidPrice);
            mapRegionsEnums.Add("cleared offer cost", (int)SystemOutRegionsEnum.ClearedOfferCost);
            mapRegionsEnums.Add("cleared reserve offer cost", (int)SystemOutRegionsEnum.ClearedReserveOfferCost);
            mapRegionsEnums.Add("constrained off cost", (int)SystemOutRegionsEnum.ConstrainedOffCost);
            mapRegionsEnums.Add("constrained on cost", (int)SystemOutRegionsEnum.ConstrainedOnCost);
            mapRegionsEnums.Add("contract generation", (int)SystemOutRegionsEnum.ContractGeneration);
            mapRegionsEnums.Add("contract generation capacity", (int)SystemOutRegionsEnum.ContractGenerationCapacity);
            mapRegionsEnums.Add("contract load", (int)SystemOutRegionsEnum.ContractLoad);
            mapRegionsEnums.Add("contract load obligation", (int)SystemOutRegionsEnum.ContractLoadObligation);
            mapRegionsEnums.Add("contract settlement", (int)SystemOutRegionsEnum.ContractSettlement);
            mapRegionsEnums.Add("contract volume", (int)SystemOutRegionsEnum.ContractVolume);
            mapRegionsEnums.Add("cost of curtailment", (int)SystemOutRegionsEnum.CostofCurtailment);
            mapRegionsEnums.Add("cost of dump energy", (int)SystemOutRegionsEnum.CostofDumpEnergy);
            mapRegionsEnums.Add("cost of unserved energy", (int)SystemOutRegionsEnum.CostofUnservedEnergy);
            mapRegionsEnums.Add("cost to load", (int)SystemOutRegionsEnum.CosttoLoad);
            mapRegionsEnums.Add("curtailable load", (int)SystemOutRegionsEnum.CurtailableLoad);
            mapRegionsEnums.Add("customer load", (int)SystemOutRegionsEnum.CustomerLoad);
            mapRegionsEnums.Add("demand curtailed", (int)SystemOutRegionsEnum.DemandCurtailed);
            mapRegionsEnums.Add("discrete maintenance", (int)SystemOutRegionsEnum.DiscreteMaintenance);
            mapRegionsEnums.Add("dispatchable capacity", (int)SystemOutRegionsEnum.DispatchableCapacity);
            mapRegionsEnums.Add("distributed maintenance", (int)SystemOutRegionsEnum.DistributedMaintenance);
            mapRegionsEnums.Add("dsp bid cleared", (int)SystemOutRegionsEnum.DSPBidCleared);
            mapRegionsEnums.Add("dsp bid price", (int)SystemOutRegionsEnum.DSPBidPrice);
            mapRegionsEnums.Add("dsp bid quantity", (int)SystemOutRegionsEnum.DSPBidQuantity);
            mapRegionsEnums.Add("dump energy", (int)SystemOutRegionsEnum.DumpEnergy);
            mapRegionsEnums.Add("dump energy hours", (int)SystemOutRegionsEnum.DumpEnergyHours);
            mapRegionsEnums.Add("edns", (int)SystemOutRegionsEnum.EDNS);
            mapRegionsEnums.Add("eens", (int)SystemOutRegionsEnum.EENS);
            mapRegionsEnums.Add("emissions cost", (int)SystemOutRegionsEnum.EmissionsCost);
            mapRegionsEnums.Add("export capacity", (int)SystemOutRegionsEnum.ExportCapacity);
            mapRegionsEnums.Add("export capacity built", (int)SystemOutRegionsEnum.ExportCapacityBuilt);
            mapRegionsEnums.Add("export capacity retired", (int)SystemOutRegionsEnum.ExportCapacityRetired);
            mapRegionsEnums.Add("export cost", (int)SystemOutRegionsEnum.ExportCost);
            mapRegionsEnums.Add("exports", (int)SystemOutRegionsEnum.Exports);
            mapRegionsEnums.Add("firm generation capacity", (int)SystemOutRegionsEnum.FirmGenerationCapacity);
            mapRegionsEnums.Add("fixed generation", (int)SystemOutRegionsEnum.FixedGeneration);
            mapRegionsEnums.Add("fixed load", (int)SystemOutRegionsEnum.FixedLoad);
            mapRegionsEnums.Add("flexibility down", (int)SystemOutRegionsEnum.FlexibilityDown);
            mapRegionsEnums.Add("flexibility up", (int)SystemOutRegionsEnum.FlexibilityUp);
            mapRegionsEnums.Add("forced outage", (int)SystemOutRegionsEnum.ForcedOutage);
            mapRegionsEnums.Add("generation", (int)SystemOutRegionsEnum.Generation);
            mapRegionsEnums.Add("generation capacity", (int)SystemOutRegionsEnum.GenerationCapacity);
            mapRegionsEnums.Add("generation capacity built", (int)SystemOutRegionsEnum.GenerationCapacityBuilt);
            mapRegionsEnums.Add("generation capacity curtailed", (int)SystemOutRegionsEnum.GenerationCapacityCurtailed);
            mapRegionsEnums.Add("generation capacity retired", (int)SystemOutRegionsEnum.GenerationCapacityRetired);
            mapRegionsEnums.Add("generation cost", (int)SystemOutRegionsEnum.GenerationCost);
            mapRegionsEnums.Add("generation sent out", (int)SystemOutRegionsEnum.GenerationSentOut);
            mapRegionsEnums.Add("generation_weighted price", (int)SystemOutRegionsEnum.GenerationweightedPrice);
            mapRegionsEnums.Add("generator auxiliary use", (int)SystemOutRegionsEnum.GeneratorAuxiliaryUse);
            mapRegionsEnums.Add("generator build cost", (int)SystemOutRegionsEnum.GeneratorBuildCost);
            mapRegionsEnums.Add("generator fo_m cost", (int)SystemOutRegionsEnum.GeneratorFOMCost);
            mapRegionsEnums.Add("generator monopoly rent", (int)SystemOutRegionsEnum.GeneratorMonopolyRent);
            mapRegionsEnums.Add("generator net pool revenue", (int)SystemOutRegionsEnum.GeneratorNetPoolRevenue);
            mapRegionsEnums.Add("generator net profit", (int)SystemOutRegionsEnum.GeneratorNetProfit);
            mapRegionsEnums.Add("generator net revenue", (int)SystemOutRegionsEnum.GeneratorNetRevenue);
            mapRegionsEnums.Add("generator pool revenue", (int)SystemOutRegionsEnum.GeneratorPoolRevenue);
            mapRegionsEnums.Add("generator pump cost", (int)SystemOutRegionsEnum.GeneratorPumpCost);
            mapRegionsEnums.Add("generator retirement cost", (int)SystemOutRegionsEnum.GeneratorRetirementCost);
            mapRegionsEnums.Add("generator start _ shutdown cost", (int)SystemOutRegionsEnum.GeneratorStartShutdownCost);
            mapRegionsEnums.Add("hours at minimum", (int)SystemOutRegionsEnum.HoursatMinimum);
            mapRegionsEnums.Add("hours generation curtailed", (int)SystemOutRegionsEnum.HoursGenerationCurtailed);
            mapRegionsEnums.Add("import capacity", (int)SystemOutRegionsEnum.ImportCapacity);
            mapRegionsEnums.Add("import capacity built", (int)SystemOutRegionsEnum.ImportCapacityBuilt);
            mapRegionsEnums.Add("import capacity retired", (int)SystemOutRegionsEnum.ImportCapacityRetired);
            mapRegionsEnums.Add("import revenue", (int)SystemOutRegionsEnum.ImportRevenue);
            mapRegionsEnums.Add("imports", (int)SystemOutRegionsEnum.Imports);
            mapRegionsEnums.Add("inflexible generation", (int)SystemOutRegionsEnum.InflexibleGeneration);
            mapRegionsEnums.Add("interregional transmission losses", (int)SystemOutRegionsEnum.InterregionalTransmissionLosses);
            mapRegionsEnums.Add("interregional transmission rental", (int)SystemOutRegionsEnum.InterregionalTransmissionRental);
            mapRegionsEnums.Add("intraregional transmission losses", (int)SystemOutRegionsEnum.IntraregionalTransmissionLosses);
            mapRegionsEnums.Add("intraregional transmission rental", (int)SystemOutRegionsEnum.IntraregionalTransmissionRental);
            mapRegionsEnums.Add("levelized cost", (int)SystemOutRegionsEnum.LevelizedCost);
            mapRegionsEnums.Add("load", (int)SystemOutRegionsEnum.Load);
            mapRegionsEnums.Add("load_weighted price", (int)SystemOutRegionsEnum.LoadweightedPrice);
            mapRegionsEnums.Add("lole", (int)SystemOutRegionsEnum.LOLE);
            mapRegionsEnums.Add("lolp", (int)SystemOutRegionsEnum.LOLP);
            mapRegionsEnums.Add("lower reserve", (int)SystemOutRegionsEnum.LowerReserve);
            mapRegionsEnums.Add("lrmc", (int)SystemOutRegionsEnum.LRMC);
            mapRegionsEnums.Add("maintenance", (int)SystemOutRegionsEnum.Maintenance);
            mapRegionsEnums.Add("maintenance factor", (int)SystemOutRegionsEnum.MaintenanceFactor);
            mapRegionsEnums.Add("max capacity reserve margin", (int)SystemOutRegionsEnum.MaxCapacityReserveMargin);
            mapRegionsEnums.Add("max capacity reserves", (int)SystemOutRegionsEnum.MaxCapacityReserves);
            mapRegionsEnums.Add("max unserved energy", (int)SystemOutRegionsEnum.MaxUnservedEnergy);
            mapRegionsEnums.Add("min capacity reserve margin", (int)SystemOutRegionsEnum.MinCapacityReserveMargin);
            mapRegionsEnums.Add("min capacity reserves", (int)SystemOutRegionsEnum.MinCapacityReserves);
            mapRegionsEnums.Add("min load", (int)SystemOutRegionsEnum.MinLoad);
            mapRegionsEnums.Add("multi_area lole", (int)SystemOutRegionsEnum.MultiareaLOLE);
            mapRegionsEnums.Add("multi_area lolp", (int)SystemOutRegionsEnum.MultiareaLOLP);
            mapRegionsEnums.Add("native load", (int)SystemOutRegionsEnum.NativeLoad);
            mapRegionsEnums.Add("net balancing area interchange", (int)SystemOutRegionsEnum.NetBalancingAreaInterchange);
            mapRegionsEnums.Add("net balancing area interchange revenue", (int)SystemOutRegionsEnum.NetBalancingAreaInterchangeRevenue);
            mapRegionsEnums.Add("net capacity interchange", (int)SystemOutRegionsEnum.NetCapacityInterchange);
            mapRegionsEnums.Add("net contract load", (int)SystemOutRegionsEnum.NetContractLoad);
            mapRegionsEnums.Add("net cost of exports", (int)SystemOutRegionsEnum.NetCostofExports);
            mapRegionsEnums.Add("net cost to load", (int)SystemOutRegionsEnum.NetCosttoLoad);
            mapRegionsEnums.Add("net generation", (int)SystemOutRegionsEnum.NetGeneration);
            mapRegionsEnums.Add("net interchange", (int)SystemOutRegionsEnum.NetInterchange);
            mapRegionsEnums.Add("net market profit", (int)SystemOutRegionsEnum.NetMarketProfit);
            mapRegionsEnums.Add("net market sales", (int)SystemOutRegionsEnum.NetMarketSales);
            mapRegionsEnums.Add("no cost generation capacity", (int)SystemOutRegionsEnum.NoCostGenerationCapacity);
            mapRegionsEnums.Add("non_utility contract settlement", (int)SystemOutRegionsEnum.NonUtilityContractSettlement);
            mapRegionsEnums.Add("non_utility monopoly rent", (int)SystemOutRegionsEnum.NonUtilityMonopolyRent);
            mapRegionsEnums.Add("non_utility net revenue", (int)SystemOutRegionsEnum.NonUtilityNetRevenue);
            mapRegionsEnums.Add("peak load", (int)SystemOutRegionsEnum.PeakLoad);
            mapRegionsEnums.Add("planning peak load", (int)SystemOutRegionsEnum.PlanningPeakLoad);
            mapRegionsEnums.Add("price", (int)SystemOutRegionsEnum.Price);
            mapRegionsEnums.Add("price_cost mark_up", (int)SystemOutRegionsEnum.PriceCostMarkup);
            mapRegionsEnums.Add("pump generation", (int)SystemOutRegionsEnum.PumpGeneration);
            mapRegionsEnums.Add("pump load", (int)SystemOutRegionsEnum.PumpLoad);
            mapRegionsEnums.Add("purchaser load", (int)SystemOutRegionsEnum.PurchaserLoad);
            mapRegionsEnums.Add("raise reserve", (int)SystemOutRegionsEnum.RaiseReserve);
            mapRegionsEnums.Add("ramp flexibility down", (int)SystemOutRegionsEnum.RampFlexibilityDown);
            mapRegionsEnums.Add("ramp flexibility up", (int)SystemOutRegionsEnum.RampFlexibilityUp);
            mapRegionsEnums.Add("regulation lower reserve", (int)SystemOutRegionsEnum.RegulationLowerReserve);
            mapRegionsEnums.Add("regulation raise reserve", (int)SystemOutRegionsEnum.RegulationRaiseReserve);
            mapRegionsEnums.Add("replacement reserve", (int)SystemOutRegionsEnum.ReplacementReserve);
            mapRegionsEnums.Add("settlement surplus", (int)SystemOutRegionsEnum.SettlementSurplus);
            mapRegionsEnums.Add("shadow cost to load", (int)SystemOutRegionsEnum.ShadowCosttoLoad);
            mapRegionsEnums.Add("shadow generation", (int)SystemOutRegionsEnum.ShadowGeneration);
            mapRegionsEnums.Add("shadow generation capacity built", (int)SystemOutRegionsEnum.ShadowGenerationCapacityBuilt);
            mapRegionsEnums.Add("shadow load", (int)SystemOutRegionsEnum.ShadowLoad);
            mapRegionsEnums.Add("shadow price", (int)SystemOutRegionsEnum.ShadowPrice);
            mapRegionsEnums.Add("srmc", (int)SystemOutRegionsEnum.SRMC);
            mapRegionsEnums.Add("time_weighted price", (int)SystemOutRegionsEnum.TimeweightedPrice);
            mapRegionsEnums.Add("total cost", (int)SystemOutRegionsEnum.TotalCost);
            mapRegionsEnums.Add("total fixed costs", (int)SystemOutRegionsEnum.TotalFixedCosts);
            mapRegionsEnums.Add("total generation cost", (int)SystemOutRegionsEnum.TotalGenerationCost);
            mapRegionsEnums.Add("total generator revenue", (int)SystemOutRegionsEnum.TotalGeneratorRevenue);
            mapRegionsEnums.Add("total system cost", (int)SystemOutRegionsEnum.TotalSystemCost);
            mapRegionsEnums.Add("transmission build cost", (int)SystemOutRegionsEnum.TransmissionBuildCost);
            mapRegionsEnums.Add("transmission control rental", (int)SystemOutRegionsEnum.TransmissionControlRental);
            mapRegionsEnums.Add("transmission losses", (int)SystemOutRegionsEnum.TransmissionLosses);
            mapRegionsEnums.Add("transmission rental", (int)SystemOutRegionsEnum.TransmissionRental);
            mapRegionsEnums.Add("transmission retirement cost", (int)SystemOutRegionsEnum.TransmissionRetirementCost);
            mapRegionsEnums.Add("undispatched capacity", (int)SystemOutRegionsEnum.UndispatchedCapacity);
            mapRegionsEnums.Add("unserved energy", (int)SystemOutRegionsEnum.UnservedEnergy);
            mapRegionsEnums.Add("unserved energy factor", (int)SystemOutRegionsEnum.UnservedEnergyFactor);
            mapRegionsEnums.Add("unserved energy hours", (int)SystemOutRegionsEnum.UnservedEnergyHours);
            mapRegionsEnums.Add("uplift", (int)SystemOutRegionsEnum.Uplift);
            mapRegionsEnums.Add("utility contract settlement", (int)SystemOutRegionsEnum.UtilityContractSettlement);
            mapRegionsEnums.Add("utility monopoly rent", (int)SystemOutRegionsEnum.UtilityMonopolyRent);
            mapRegionsEnums.Add("utility net revenue", (int)SystemOutRegionsEnum.UtilityNetRevenue);
            mapRegionsEnums.Add("wheeling cost", (int)SystemOutRegionsEnum.WheelingCost);
            mapRegionsEnums.Add("wheeling revenue", (int)SystemOutRegionsEnum.WheelingRevenue);
            mapRegionsEnums.Add("x", (int)SystemOutRegionsEnum.x);
            mapRegionsEnums.Add("y", (int)SystemOutRegionsEnum.y);
            mapRegionsEnums.Add("z", (int)SystemOutRegionsEnum.z);



            mapZonesEnums.Add("abatement cost", (int)SystemOutZonesEnum.AbatementCost);
            mapZonesEnums.Add("annualized build cost", (int)SystemOutZonesEnum.AnnualizedBuildCost);
            mapZonesEnums.Add("available capacity", (int)SystemOutZonesEnum.AvailableCapacity);
            mapZonesEnums.Add("available capacity margin", (int)SystemOutZonesEnum.AvailableCapacityMargin);
            mapZonesEnums.Add("available capacity reserves", (int)SystemOutZonesEnum.AvailableCapacityReserves);
            mapZonesEnums.Add("balancing area interchange exports", (int)SystemOutZonesEnum.BalancingAreaInterchangeExports);
            mapZonesEnums.Add("balancing area interchange exports revenue", (int)SystemOutZonesEnum.BalancingAreaInterchangeExportsRevenue);
            mapZonesEnums.Add("balancing area interchange imports", (int)SystemOutZonesEnum.BalancingAreaInterchangeImports);
            mapZonesEnums.Add("balancing area interchange imports cost", (int)SystemOutZonesEnum.BalancingAreaInterchangeImportsCost);
            mapZonesEnums.Add("battery generation", (int)SystemOutZonesEnum.BatteryGeneration);
            mapZonesEnums.Add("battery load", (int)SystemOutZonesEnum.BatteryLoad);
            mapZonesEnums.Add("capacity excess", (int)SystemOutZonesEnum.CapacityExcess);
            mapZonesEnums.Add("capacity excess cost", (int)SystemOutZonesEnum.CapacityExcessCost);
            mapZonesEnums.Add("capacity payments", (int)SystemOutZonesEnum.CapacityPayments);
            mapZonesEnums.Add("capacity price", (int)SystemOutZonesEnum.CapacityPrice);
            mapZonesEnums.Add("capacity reserve margin", (int)SystemOutZonesEnum.CapacityReserveMargin);
            mapZonesEnums.Add("capacity reserves", (int)SystemOutZonesEnum.CapacityReserves);
            mapZonesEnums.Add("capacity shadow price", (int)SystemOutZonesEnum.CapacityShadowPrice);
            mapZonesEnums.Add("capacity shortage", (int)SystemOutZonesEnum.CapacityShortage);
            mapZonesEnums.Add("capacity shortage cost", (int)SystemOutZonesEnum.CapacityShortageCost);
            mapZonesEnums.Add("charging station deferred load", (int)SystemOutZonesEnum.ChargingStationDeferredLoad);
            mapZonesEnums.Add("charging station generation", (int)SystemOutZonesEnum.ChargingStationGeneration);
            mapZonesEnums.Add("charging station hours deferred", (int)SystemOutZonesEnum.ChargingStationHoursDeferred);
            mapZonesEnums.Add("charging station load", (int)SystemOutZonesEnum.ChargingStationLoad);
            mapZonesEnums.Add("cleared offer cost", (int)SystemOutZonesEnum.ClearedOfferCost);
            mapZonesEnums.Add("cleared reserve offer cost", (int)SystemOutZonesEnum.ClearedReserveOfferCost);
            mapZonesEnums.Add("congestion charge", (int)SystemOutZonesEnum.CongestionCharge);
            mapZonesEnums.Add("contract generation", (int)SystemOutZonesEnum.ContractGeneration);
            mapZonesEnums.Add("contract generation capacity", (int)SystemOutZonesEnum.ContractGenerationCapacity);
            mapZonesEnums.Add("contract load", (int)SystemOutZonesEnum.ContractLoad);
            mapZonesEnums.Add("contract load obligation", (int)SystemOutZonesEnum.ContractLoadObligation);
            mapZonesEnums.Add("cost of curtailment", (int)SystemOutZonesEnum.CostofCurtailment);
            mapZonesEnums.Add("cost of dump energy", (int)SystemOutZonesEnum.CostofDumpEnergy);
            mapZonesEnums.Add("cost of unserved energy", (int)SystemOutZonesEnum.CostofUnservedEnergy);
            mapZonesEnums.Add("cost to load", (int)SystemOutZonesEnum.CosttoLoad);
            mapZonesEnums.Add("curtailable load", (int)SystemOutZonesEnum.CurtailableLoad);
            mapZonesEnums.Add("customer load", (int)SystemOutZonesEnum.CustomerLoad);
            mapZonesEnums.Add("demand curtailed", (int)SystemOutZonesEnum.DemandCurtailed);
            mapZonesEnums.Add("discrete maintenance", (int)SystemOutZonesEnum.DiscreteMaintenance);
            mapZonesEnums.Add("dispatchable capacity", (int)SystemOutZonesEnum.DispatchableCapacity);
            mapZonesEnums.Add("distributed maintenance", (int)SystemOutZonesEnum.DistributedMaintenance);
            mapZonesEnums.Add("dump energy", (int)SystemOutZonesEnum.DumpEnergy);
            mapZonesEnums.Add("dump energy hours", (int)SystemOutZonesEnum.DumpEnergyHours);
            mapZonesEnums.Add("edns", (int)SystemOutZonesEnum.EDNS);
            mapZonesEnums.Add("eens", (int)SystemOutZonesEnum.EENS);
            mapZonesEnums.Add("emissions cost", (int)SystemOutZonesEnum.EmissionsCost);
            mapZonesEnums.Add("energy charge", (int)SystemOutZonesEnum.EnergyCharge);
            mapZonesEnums.Add("export capacity", (int)SystemOutZonesEnum.ExportCapacity);
            mapZonesEnums.Add("export capacity built", (int)SystemOutZonesEnum.ExportCapacityBuilt);
            mapZonesEnums.Add("export capacity retired", (int)SystemOutZonesEnum.ExportCapacityRetired);
            mapZonesEnums.Add("export cost", (int)SystemOutZonesEnum.ExportCost);
            mapZonesEnums.Add("exports", (int)SystemOutZonesEnum.Exports);
            mapZonesEnums.Add("firm generation capacity", (int)SystemOutZonesEnum.FirmGenerationCapacity);
            mapZonesEnums.Add("fixed generation", (int)SystemOutZonesEnum.FixedGeneration);
            mapZonesEnums.Add("fixed load", (int)SystemOutZonesEnum.FixedLoad);
            mapZonesEnums.Add("flexibility down", (int)SystemOutZonesEnum.FlexibilityDown);
            mapZonesEnums.Add("flexibility up", (int)SystemOutZonesEnum.FlexibilityUp);
            mapZonesEnums.Add("generation", (int)SystemOutZonesEnum.Generation);
            mapZonesEnums.Add("generation capacity", (int)SystemOutZonesEnum.GenerationCapacity);
            mapZonesEnums.Add("generation capacity built", (int)SystemOutZonesEnum.GenerationCapacityBuilt);
            mapZonesEnums.Add("generation capacity curtailed", (int)SystemOutZonesEnum.GenerationCapacityCurtailed);
            mapZonesEnums.Add("generation capacity retired", (int)SystemOutZonesEnum.GenerationCapacityRetired);
            mapZonesEnums.Add("generation cost", (int)SystemOutZonesEnum.GenerationCost);
            mapZonesEnums.Add("generation sent out", (int)SystemOutZonesEnum.GenerationSentOut);
            mapZonesEnums.Add("generation_weighted price", (int)SystemOutZonesEnum.GenerationweightedPrice);
            mapZonesEnums.Add("generator auxiliary use", (int)SystemOutZonesEnum.GeneratorAuxiliaryUse);
            mapZonesEnums.Add("generator build cost", (int)SystemOutZonesEnum.GeneratorBuildCost);
            mapZonesEnums.Add("generator fo_m cost", (int)SystemOutZonesEnum.GeneratorFOMCost);
            mapZonesEnums.Add("generator net profit", (int)SystemOutZonesEnum.GeneratorNetProfit);
            mapZonesEnums.Add("generator net revenue", (int)SystemOutZonesEnum.GeneratorNetRevenue);
            mapZonesEnums.Add("generator pool revenue", (int)SystemOutZonesEnum.GeneratorPoolRevenue);
            mapZonesEnums.Add("generator pump cost", (int)SystemOutZonesEnum.GeneratorPumpCost);
            mapZonesEnums.Add("generator retirement cost", (int)SystemOutZonesEnum.GeneratorRetirementCost);
            mapZonesEnums.Add("generator start _ shutdown cost", (int)SystemOutZonesEnum.GeneratorStartShutdownCost);
            mapZonesEnums.Add("hours at minimum", (int)SystemOutZonesEnum.HoursatMinimum);
            mapZonesEnums.Add("hours generation curtailed", (int)SystemOutZonesEnum.HoursGenerationCurtailed);
            mapZonesEnums.Add("import capacity", (int)SystemOutZonesEnum.ImportCapacity);
            mapZonesEnums.Add("import capacity built", (int)SystemOutZonesEnum.ImportCapacityBuilt);
            mapZonesEnums.Add("import capacity retired", (int)SystemOutZonesEnum.ImportCapacityRetired);
            mapZonesEnums.Add("import revenue", (int)SystemOutZonesEnum.ImportRevenue);
            mapZonesEnums.Add("imports", (int)SystemOutZonesEnum.Imports);
            mapZonesEnums.Add("inflexible generation", (int)SystemOutZonesEnum.InflexibleGeneration);
            mapZonesEnums.Add("levelized cost", (int)SystemOutZonesEnum.LevelizedCost);
            mapZonesEnums.Add("load", (int)SystemOutZonesEnum.Load);
            mapZonesEnums.Add("load_weighted price", (int)SystemOutZonesEnum.LoadweightedPrice);
            mapZonesEnums.Add("lole", (int)SystemOutZonesEnum.LOLE);
            mapZonesEnums.Add("lolp", (int)SystemOutZonesEnum.LOLP);
            mapZonesEnums.Add("lower reserve", (int)SystemOutZonesEnum.LowerReserve);
            mapZonesEnums.Add("lrmc", (int)SystemOutZonesEnum.LRMC);
            mapZonesEnums.Add("maintenance factor", (int)SystemOutZonesEnum.MaintenanceFactor);
            mapZonesEnums.Add("marginal loss charge", (int)SystemOutZonesEnum.MarginalLossCharge);
            mapZonesEnums.Add("marginal loss factor", (int)SystemOutZonesEnum.MarginalLossFactor);
            mapZonesEnums.Add("max capacity reserve margin", (int)SystemOutZonesEnum.MaxCapacityReserveMargin);
            mapZonesEnums.Add("max capacity reserves", (int)SystemOutZonesEnum.MaxCapacityReserves);
            mapZonesEnums.Add("max unserved energy", (int)SystemOutZonesEnum.MaxUnservedEnergy);
            mapZonesEnums.Add("min capacity reserve margin", (int)SystemOutZonesEnum.MinCapacityReserveMargin);
            mapZonesEnums.Add("min capacity reserves", (int)SystemOutZonesEnum.MinCapacityReserves);
            mapZonesEnums.Add("min load", (int)SystemOutZonesEnum.MinLoad);
            mapZonesEnums.Add("multi_area lole", (int)SystemOutZonesEnum.MultiareaLOLE);
            mapZonesEnums.Add("multi_area lolp", (int)SystemOutZonesEnum.MultiareaLOLP);
            mapZonesEnums.Add("native load", (int)SystemOutZonesEnum.NativeLoad);
            mapZonesEnums.Add("net balancing area interchange", (int)SystemOutZonesEnum.NetBalancingAreaInterchange);
            mapZonesEnums.Add("net balancing area interchange revenue", (int)SystemOutZonesEnum.NetBalancingAreaInterchangeRevenue);
            mapZonesEnums.Add("net capacity interchange", (int)SystemOutZonesEnum.NetCapacityInterchange);
            mapZonesEnums.Add("net contract load", (int)SystemOutZonesEnum.NetContractLoad);
            mapZonesEnums.Add("net cost of exports", (int)SystemOutZonesEnum.NetCostofExports);
            mapZonesEnums.Add("net generation", (int)SystemOutZonesEnum.NetGeneration);
            mapZonesEnums.Add("net interchange", (int)SystemOutZonesEnum.NetInterchange);
            mapZonesEnums.Add("net market profit", (int)SystemOutZonesEnum.NetMarketProfit);
            mapZonesEnums.Add("net market sales", (int)SystemOutZonesEnum.NetMarketSales);
            mapZonesEnums.Add("no cost generation capacity", (int)SystemOutZonesEnum.NoCostGenerationCapacity);
            mapZonesEnums.Add("peak load", (int)SystemOutZonesEnum.PeakLoad);
            mapZonesEnums.Add("planning peak load", (int)SystemOutZonesEnum.PlanningPeakLoad);
            mapZonesEnums.Add("price", (int)SystemOutZonesEnum.Price);
            mapZonesEnums.Add("pump generation", (int)SystemOutZonesEnum.PumpGeneration);
            mapZonesEnums.Add("pump load", (int)SystemOutZonesEnum.PumpLoad);
            mapZonesEnums.Add("purchaser load", (int)SystemOutZonesEnum.PurchaserLoad);
            mapZonesEnums.Add("raise reserve", (int)SystemOutZonesEnum.RaiseReserve);
            mapZonesEnums.Add("ramp flexibility down", (int)SystemOutZonesEnum.RampFlexibilityDown);
            mapZonesEnums.Add("ramp flexibility up", (int)SystemOutZonesEnum.RampFlexibilityUp);
            mapZonesEnums.Add("regulation lower reserve", (int)SystemOutZonesEnum.RegulationLowerReserve);
            mapZonesEnums.Add("regulation raise reserve", (int)SystemOutZonesEnum.RegulationRaiseReserve);
            mapZonesEnums.Add("replacement reserve", (int)SystemOutZonesEnum.ReplacementReserve);
            mapZonesEnums.Add("settlement surplus", (int)SystemOutZonesEnum.SettlementSurplus);
            mapZonesEnums.Add("srmc", (int)SystemOutZonesEnum.SRMC);
            mapZonesEnums.Add("time_weighted price", (int)SystemOutZonesEnum.TimeweightedPrice);
            mapZonesEnums.Add("total cost", (int)SystemOutZonesEnum.TotalCost);
            mapZonesEnums.Add("total fixed costs", (int)SystemOutZonesEnum.TotalFixedCosts);
            mapZonesEnums.Add("total generation cost", (int)SystemOutZonesEnum.TotalGenerationCost);
            mapZonesEnums.Add("total generator revenue", (int)SystemOutZonesEnum.TotalGeneratorRevenue);
            mapZonesEnums.Add("total system cost", (int)SystemOutZonesEnum.TotalSystemCost);
            mapZonesEnums.Add("transmission build cost", (int)SystemOutZonesEnum.TransmissionBuildCost);
            mapZonesEnums.Add("transmission losses", (int)SystemOutZonesEnum.TransmissionLosses);
            mapZonesEnums.Add("transmission rental", (int)SystemOutZonesEnum.TransmissionRental);
            mapZonesEnums.Add("transmission retirement cost", (int)SystemOutZonesEnum.TransmissionRetirementCost);
            mapZonesEnums.Add("undispatched capacity", (int)SystemOutZonesEnum.UndispatchedCapacity);
            mapZonesEnums.Add("unserved energy", (int)SystemOutZonesEnum.UnservedEnergy);
            mapZonesEnums.Add("unserved energy factor", (int)SystemOutZonesEnum.UnservedEnergyFactor);
            mapZonesEnums.Add("unserved energy hours", (int)SystemOutZonesEnum.UnservedEnergyHours);
            mapZonesEnums.Add("wheeling cost", (int)SystemOutZonesEnum.WheelingCost);
            mapZonesEnums.Add("wheeling revenue", (int)SystemOutZonesEnum.WheelingRevenue);
            mapZonesEnums.Add("x", (int)SystemOutZonesEnum.x);
            mapZonesEnums.Add("y", (int)SystemOutZonesEnum.y);
            mapZonesEnums.Add("z", (int)SystemOutZonesEnum.z);



            mapNodesEnums.Add("battery generation", (int)SystemOutNodesEnum.BatteryGeneration);
            mapNodesEnums.Add("battery load", (int)SystemOutNodesEnum.BatteryLoad);
            mapNodesEnums.Add("capacity reserves", (int)SystemOutNodesEnum.CapacityReserves);
            mapNodesEnums.Add("charging station deferred load", (int)SystemOutNodesEnum.ChargingStationDeferredLoad);
            mapNodesEnums.Add("charging station generation", (int)SystemOutNodesEnum.ChargingStationGeneration);
            mapNodesEnums.Add("charging station hours deferred", (int)SystemOutNodesEnum.ChargingStationHoursDeferred);
            mapNodesEnums.Add("charging station load", (int)SystemOutNodesEnum.ChargingStationLoad);
            mapNodesEnums.Add("cleared dsp bid cost", (int)SystemOutNodesEnum.ClearedDSPBidCost);
            mapNodesEnums.Add("cleared dsp bid price", (int)SystemOutNodesEnum.ClearedDSPBidPrice);
            mapNodesEnums.Add("congestion charge", (int)SystemOutNodesEnum.CongestionCharge);
            mapNodesEnums.Add("contract generation capacity", (int)SystemOutNodesEnum.ContractGenerationCapacity);
            mapNodesEnums.Add("contract load obligation", (int)SystemOutNodesEnum.ContractLoadObligation);
            mapNodesEnums.Add("curtailable load", (int)SystemOutNodesEnum.CurtailableLoad);
            mapNodesEnums.Add("customer load", (int)SystemOutNodesEnum.CustomerLoad);
            mapNodesEnums.Add("demand curtailed", (int)SystemOutNodesEnum.DemandCurtailed);
            mapNodesEnums.Add("discrete maintenance", (int)SystemOutNodesEnum.DiscreteMaintenance);
            mapNodesEnums.Add("distributed maintenance", (int)SystemOutNodesEnum.DistributedMaintenance);
            mapNodesEnums.Add("dsp bid cleared", (int)SystemOutNodesEnum.DSPBidCleared);
            mapNodesEnums.Add("dsp bid price", (int)SystemOutNodesEnum.DSPBidPrice);
            mapNodesEnums.Add("dsp bid quantity", (int)SystemOutNodesEnum.DSPBidQuantity);
            mapNodesEnums.Add("dump energy", (int)SystemOutNodesEnum.DumpEnergy);
            mapNodesEnums.Add("edns", (int)SystemOutNodesEnum.EDNS);
            mapNodesEnums.Add("eens", (int)SystemOutNodesEnum.EENS);
            mapNodesEnums.Add("energy charge", (int)SystemOutNodesEnum.EnergyCharge);
            mapNodesEnums.Add("export capacity", (int)SystemOutNodesEnum.ExportCapacity);
            mapNodesEnums.Add("exports", (int)SystemOutNodesEnum.Exports);
            mapNodesEnums.Add("firm generation capacity", (int)SystemOutNodesEnum.FirmGenerationCapacity);
            mapNodesEnums.Add("fixed generation", (int)SystemOutNodesEnum.FixedGeneration);
            mapNodesEnums.Add("fixed load", (int)SystemOutNodesEnum.FixedLoad);
            mapNodesEnums.Add("flow", (int)SystemOutNodesEnum.Flow);
            mapNodesEnums.Add("generation", (int)SystemOutNodesEnum.Generation);
            mapNodesEnums.Add("generation capacity", (int)SystemOutNodesEnum.GenerationCapacity);
            mapNodesEnums.Add("generation sent out", (int)SystemOutNodesEnum.GenerationSentOut);
            mapNodesEnums.Add("generator auxiliary use", (int)SystemOutNodesEnum.GeneratorAuxiliaryUse);
            mapNodesEnums.Add("import capacity", (int)SystemOutNodesEnum.ImportCapacity);
            mapNodesEnums.Add("imports", (int)SystemOutNodesEnum.Imports);
            mapNodesEnums.Add("injection mismatch", (int)SystemOutNodesEnum.InjectionMismatch);
            mapNodesEnums.Add("load", (int)SystemOutNodesEnum.Load);
            mapNodesEnums.Add("lole", (int)SystemOutNodesEnum.LOLE);
            mapNodesEnums.Add("lolp", (int)SystemOutNodesEnum.LOLP);
            mapNodesEnums.Add("losses", (int)SystemOutNodesEnum.Losses);
            mapNodesEnums.Add("maintenance factor", (int)SystemOutNodesEnum.MaintenanceFactor);
            mapNodesEnums.Add("marginal loss charge", (int)SystemOutNodesEnum.MarginalLossCharge);
            mapNodesEnums.Add("marginal loss factor", (int)SystemOutNodesEnum.MarginalLossFactor);
            mapNodesEnums.Add("min capacity reserves", (int)SystemOutNodesEnum.MinCapacityReserves);
            mapNodesEnums.Add("min load", (int)SystemOutNodesEnum.MinLoad);
            mapNodesEnums.Add("native load", (int)SystemOutNodesEnum.NativeLoad);
            mapNodesEnums.Add("net capacity interchange", (int)SystemOutNodesEnum.NetCapacityInterchange);
            mapNodesEnums.Add("net contract load", (int)SystemOutNodesEnum.NetContractLoad);
            mapNodesEnums.Add("net dc export", (int)SystemOutNodesEnum.NetDCExport);
            mapNodesEnums.Add("net injection", (int)SystemOutNodesEnum.NetInjection);
            mapNodesEnums.Add("net market sales", (int)SystemOutNodesEnum.NetMarketSales);
            mapNodesEnums.Add("peak load", (int)SystemOutNodesEnum.PeakLoad);
            mapNodesEnums.Add("phase angle", (int)SystemOutNodesEnum.PhaseAngle);
            mapNodesEnums.Add("price", (int)SystemOutNodesEnum.Price);
            mapNodesEnums.Add("pump generation", (int)SystemOutNodesEnum.PumpGeneration);
            mapNodesEnums.Add("pump load", (int)SystemOutNodesEnum.PumpLoad);
            mapNodesEnums.Add("purchaser load", (int)SystemOutNodesEnum.PurchaserLoad);
            mapNodesEnums.Add("unserved energy", (int)SystemOutNodesEnum.UnservedEnergy);
            mapNodesEnums.Add("voltage", (int)SystemOutNodesEnum.Voltage);
            mapNodesEnums.Add("x", (int)SystemOutNodesEnum.x);
            mapNodesEnums.Add("y", (int)SystemOutNodesEnum.y);
            mapNodesEnums.Add("z", (int)SystemOutNodesEnum.z);



            mapLinesEnums.Add("available transfer capability", (int)SystemOutLinesEnum.AvailableTransferCapability);
            mapLinesEnums.Add("available transfer capability back", (int)SystemOutLinesEnum.AvailableTransferCapabilityBack);
            mapLinesEnums.Add("build cost", (int)SystemOutLinesEnum.BuildCost);
            mapLinesEnums.Add("capacity reserves", (int)SystemOutLinesEnum.CapacityReserves);
            mapLinesEnums.Add("cleared offer cost", (int)SystemOutLinesEnum.ClearedOfferCost);
            mapLinesEnums.Add("cleared offer price", (int)SystemOutLinesEnum.ClearedOfferPrice);
            mapLinesEnums.Add("debt cost", (int)SystemOutLinesEnum.DebtCost);
            mapLinesEnums.Add("discrete maintenance", (int)SystemOutLinesEnum.DiscreteMaintenance);
            mapLinesEnums.Add("discrete maintenance back", (int)SystemOutLinesEnum.DiscreteMaintenanceBack);
            mapLinesEnums.Add("distributed maintenance", (int)SystemOutLinesEnum.DistributedMaintenance);
            mapLinesEnums.Add("distributed maintenance back", (int)SystemOutLinesEnum.DistributedMaintenanceBack);
            mapLinesEnums.Add("equity cost", (int)SystemOutLinesEnum.EquityCost);
            mapLinesEnums.Add("export capacity built", (int)SystemOutLinesEnum.ExportCapacityBuilt);
            mapLinesEnums.Add("export capacity retired", (int)SystemOutLinesEnum.ExportCapacityRetired);
            mapLinesEnums.Add("export cost", (int)SystemOutLinesEnum.ExportCost);
            mapLinesEnums.Add("export limit", (int)SystemOutLinesEnum.ExportLimit);
            mapLinesEnums.Add("export revenue", (int)SystemOutLinesEnum.ExportRevenue);
            mapLinesEnums.Add("firm capacity", (int)SystemOutLinesEnum.FirmCapacity);
            mapLinesEnums.Add("fixed flow", (int)SystemOutLinesEnum.FixedFlow);
            mapLinesEnums.Add("fixed flow violation", (int)SystemOutLinesEnum.FixedFlowViolation);
            mapLinesEnums.Add("fixed flow violation cost", (int)SystemOutLinesEnum.FixedFlowViolationCost);
            mapLinesEnums.Add("fixed flow violation hours", (int)SystemOutLinesEnum.FixedFlowViolationHours);
            mapLinesEnums.Add("flow", (int)SystemOutLinesEnum.Flow);
            mapLinesEnums.Add("flow back", (int)SystemOutLinesEnum.FlowBack);
            mapLinesEnums.Add("fo_m cost", (int)SystemOutLinesEnum.FOMCost);
            mapLinesEnums.Add("forced outage", (int)SystemOutLinesEnum.ForcedOutage);
            mapLinesEnums.Add("forced outage rate", (int)SystemOutLinesEnum.ForcedOutageRate);
            mapLinesEnums.Add("hours congested", (int)SystemOutLinesEnum.HoursCongested);
            mapLinesEnums.Add("hours congested back", (int)SystemOutLinesEnum.HoursCongestedBack);
            mapLinesEnums.Add("import capacity built", (int)SystemOutLinesEnum.ImportCapacityBuilt);
            mapLinesEnums.Add("import capacity retired", (int)SystemOutLinesEnum.ImportCapacityRetired);
            mapLinesEnums.Add("import cost", (int)SystemOutLinesEnum.ImportCost);
            mapLinesEnums.Add("import limit", (int)SystemOutLinesEnum.ImportLimit);
            mapLinesEnums.Add("import revenue", (int)SystemOutLinesEnum.ImportRevenue);
            mapLinesEnums.Add("loading", (int)SystemOutLinesEnum.Loading);
            mapLinesEnums.Add("loading back", (int)SystemOutLinesEnum.LoadingBack);
            mapLinesEnums.Add("loss", (int)SystemOutLinesEnum.Loss);
            mapLinesEnums.Add("loss back", (int)SystemOutLinesEnum.LossBack);
            mapLinesEnums.Add("maintenance", (int)SystemOutLinesEnum.Maintenance);
            mapLinesEnums.Add("maintenance back", (int)SystemOutLinesEnum.MaintenanceBack);
            mapLinesEnums.Add("maintenance rate", (int)SystemOutLinesEnum.MaintenanceRate);
            mapLinesEnums.Add("marginal loss", (int)SystemOutLinesEnum.MarginalLoss);
            mapLinesEnums.Add("marginal loss factor", (int)SystemOutLinesEnum.MarginalLossFactor);
            mapLinesEnums.Add("max flow", (int)SystemOutLinesEnum.MaxFlow);
            mapLinesEnums.Add("min flow", (int)SystemOutLinesEnum.MinFlow);
            mapLinesEnums.Add("net flow", (int)SystemOutLinesEnum.NetFlow);
            mapLinesEnums.Add("net profit", (int)SystemOutLinesEnum.NetProfit);
            mapLinesEnums.Add("offer base", (int)SystemOutLinesEnum.OfferBase);
            mapLinesEnums.Add("offer cleared", (int)SystemOutLinesEnum.OfferCleared);
            mapLinesEnums.Add("offer cleared back", (int)SystemOutLinesEnum.OfferClearedBack);
            mapLinesEnums.Add("offer price", (int)SystemOutLinesEnum.OfferPrice);
            mapLinesEnums.Add("offer price back", (int)SystemOutLinesEnum.OfferPriceBack);
            mapLinesEnums.Add("offer quantity", (int)SystemOutLinesEnum.OfferQuantity);
            mapLinesEnums.Add("offer quantity back", (int)SystemOutLinesEnum.OfferQuantityBack);
            mapLinesEnums.Add("ramp", (int)SystemOutLinesEnum.Ramp);
            mapLinesEnums.Add("ramp cost", (int)SystemOutLinesEnum.RampCost);
            mapLinesEnums.Add("rental", (int)SystemOutLinesEnum.Rental);
            mapLinesEnums.Add("rental back", (int)SystemOutLinesEnum.RentalBack);
            mapLinesEnums.Add("retirement cost", (int)SystemOutLinesEnum.RetirementCost);
            mapLinesEnums.Add("service factor", (int)SystemOutLinesEnum.ServiceFactor);
            mapLinesEnums.Add("shadow price", (int)SystemOutLinesEnum.ShadowPrice);
            mapLinesEnums.Add("shadow price back", (int)SystemOutLinesEnum.ShadowPriceBack);
            mapLinesEnums.Add("units", (int)SystemOutLinesEnum.Units);
            mapLinesEnums.Add("units built", (int)SystemOutLinesEnum.UnitsBuilt);
            mapLinesEnums.Add("units out", (int)SystemOutLinesEnum.UnitsOut);
            mapLinesEnums.Add("units retired", (int)SystemOutLinesEnum.UnitsRetired);
            mapLinesEnums.Add("violation", (int)SystemOutLinesEnum.Violation);
            mapLinesEnums.Add("violation back", (int)SystemOutLinesEnum.ViolationBack);
            mapLinesEnums.Add("violation back hours", (int)SystemOutLinesEnum.ViolationBackHours);
            mapLinesEnums.Add("violation cost", (int)SystemOutLinesEnum.ViolationCost);
            mapLinesEnums.Add("violation cost back", (int)SystemOutLinesEnum.ViolationCostBack);
            mapLinesEnums.Add("violation hours", (int)SystemOutLinesEnum.ViolationHours);
            mapLinesEnums.Add("voltage", (int)SystemOutLinesEnum.Voltage);
            mapLinesEnums.Add("wheeling cost", (int)SystemOutLinesEnum.WheelingCost);
            mapLinesEnums.Add("wheeling cost back", (int)SystemOutLinesEnum.WheelingCostBack);
            mapLinesEnums.Add("x", (int)SystemOutLinesEnum.x);
            mapLinesEnums.Add("y", (int)SystemOutLinesEnum.y);
            mapLinesEnums.Add("z", (int)SystemOutLinesEnum.z);

        }

        public int GetPropertyId (string collection, string property)
        {
            var map = mapOut[collection.ToLower()];
            return map[property.ToLower()];
        }

        public static SolutionResultList QryToListAllObjects(PLEXOS7_NET.Core.Solution zip, string phase_id, string collection, string period_id, string propertyList, string sSampleList = null)
        {
            var enumphase_id = mapPhaseEnums[phase_id.ToLower()];
            var enumCollection = mapCollectionEnums[collection.ToLower()];
            var enumperiod_id = mapPeriodEnums[period_id.ToLower()];
            var mapPropery = mapOut[collection.ToLower()];
            var enumPropery = mapPropery[propertyList.ToLower()];
            return zip.QueryToList(enumphase_id,
              enumCollection,
              "",
              "",
              enumperiod_id,
              SeriesTypeEnum.Properties,
              "" + enumPropery, null, null, null, sSampleList);
        }

        public static void WriteResultListToFile (SolutionResultList results, string csvFilePath, int intervalLength=1 )
        {
            //2--Write to stringbuilder:
            StringBuilder outLines = new StringBuilder();
            int count = 0;

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

            //3--Write to file:
            File.AppendAllText(csvFilePath, outLines.ToString());
            outLines.Clear();

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
                outLines.AppendLine($"{r.child_id},{r.category_id},{r.sample_id},{r.date_string},{r.interval_id},{r.value}");
            } else
            {
                outLines.AppendLine($"{r.child_id},{r.category_id},{r.sample_id},{r.interval_id},{r.value}");
            }
        }

        private static void WritePeriodsIdFile(PLEXOS7_NET.Core.Solution zip, string csvFilePath, PeriodEnum period)
        {
            string[] period_id = Utils.GetPeriodIds(zip, period);
            File.WriteAllLines(csvFilePath, period_id);
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

        private static void WriteObjectMapFile(PLEXOS7_NET.Core.Solution zip, string csvFilePath)
        {
            string[] objects = Utils.GetObjectIds(zip);
            File.WriteAllLines(csvFilePath, objects);
        }

            private static void WriteCategoryMap(PLEXOS7_NET.Core.Solution zip, ClassEnum classname, string csvFilePath)
        {
            StringBuilder outLines = new StringBuilder();
            outLines.AppendLine($"CATEGORY,NAME");
            //ClassEnum classEnum = mapClass[classname.ToLower()];
            string[] categories = zip.GetCategories(classname);
            if (categories != null)
            {
                foreach (String cat in categories)
                {
                    string[] names = zip.GetObjectsInCategory(classname, cat);
                    foreach (String name in names)
                    {
                        outLines.AppendLine($"{cat},{name}");
                    }
                }
            }
            File.WriteAllText(csvFilePath, outLines.ToString());
        }

        static void Main(string[] args)
        {
            string source;
            string dest;
            string phase_id;
            string collection;
            string property;
            string period_id;
            int interval_length = 1;
            int samples = -1;
            if (args.Length == 0)
            {
                Console.WriteLine("qryoutput83.exe [SOURCE_FILE] [DEST_FOLDER] [COLLECTION] [PROPERTY] [INTERVAL] [SAMPLES]");
                Console.WriteLine("  [SOURCE_FILE] Full path to PLEXOS zip output file (including file name)");
                Console.WriteLine("  [DEST_FOLDER] Full path to destination folder");
                Console.WriteLine("  [PHASE] Simulation phase name. Supported values: LTPlan, PASA, MT_Schedule, ST_Schedule");
                Console.WriteLine("  [COLLECTION] Collection name (do not use System). Eg. Generators, Reserve.Generators");
                Console.WriteLine("  [PROPERTY] Name of property");
                Console.WriteLine("  [INTERVAL] (Optional) Horizon interval length in hours");
                Console.WriteLine("  [SAMPLES] (Optional) Number of samples to be extrated");
                return;
            }
            else if (args.Length == 6)
            {
                source = args[0];
                dest = args[1];
                phase_id = args[2];
                collection = args[3];
                property = args[4];
                period_id = args[5];
            }
            else if (args.Length == 7)
            {
                source = args[0];
                dest = args[1];
                phase_id = args[2];
                collection = args[3];
                property = args[4];
                period_id = args[5];
                interval_length = int.Parse(args[6]);
            }
            else if (args.Length == 8)
            {
                source = args[0];
                dest = args[1];
                phase_id = args[2];
                collection = args[3];
                property = args[4];
                period_id = args[5];
                interval_length = int.Parse(args[6]);
                samples = int.Parse(args[7]);
            }
            else
            {
                Console.WriteLine($"Required arguments not provided. Execute 'queryout83.exe' (with no arguents) for help");
                return;
            }

            //Enum Validation:
            //TODO: We should do the map existance validation here
            InitDictionaries();
            CollectionEnum collectionEnum = mapCollectionEnums[collection];
            ClassEnum childParentEnum = mapParentClass[collectionEnum];
            ClassEnum childClassEnum = mapChildClass[collectionEnum];
            PeriodEnum periodEnum = mapPeriodEnums[period_id];
            
            //Initialize connection:
            var zip = InitConnection(source);
            var lReportProperties = zip.GetReportedProperties().ToList<string>();
            
            //TEMP CHECK:
            //int nEnumIdFromDB;
            //int nIdFromDB;
            //string strProp;
            //foreach ( var entry in mapGeneratorsEnums)
            //{
            //    strProp = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(entry.Key.ToLower());
            //    if (lReportProperties.Contains(strProp))
            //    {
            //        nEnumIdFromDB = zip.PropertyName2EnumId("System", "Generator", "Generators", strProp);
            //        nIdFromDB = zip.PropertyName2Id("System", "Generator", "Generators", strProp);
                    
            //        if (nIdFromDB != (int)entry.Value)
            //        {
            //            Console.WriteLine($"ID MISSMATCH {entry.Key}: enumInt={(int)entry.Value}, enumid={nIdFromDB}");
            //        }
            //        if (nEnumIdFromDB != (int)entry.Value)
            //        {
            //            Console.WriteLine($"ENUM MISSMATCH {entry.Key}: enumInt={(int)entry.Value}, enumid={nEnumIdFromDB}");
            //        }
            //    }
            //}

            //Figure out samples:
            string sSampleList;
            if (samples < 0)
            {
                sSampleList = Utils.GetSampleListSansMedia(zip);
            }
            else
            {
                int[] sequence = Enumerable.Range(0, samples).ToArray();
                sSampleList = String.Join(",", sequence);
            }

            //Create map files:
            string tblPeriod = Utils.PeriodEnumToTableName(periodEnum);
            //WriteCategoryMap(zip, childClassEnum, dest + Path.DirectorySeparatorChar + collection + "_id_map.csv");
            WriteObjectMapFile(zip, dest + Path.DirectorySeparatorChar + "object_id.csv");
            WritePeriodsIdFile(zip, dest + Path.DirectorySeparatorChar + tblPeriod + ".csv", periodEnum);
            //CreatePeriod0IdMap(zip, dest + Path.DirectorySeparatorChar + "t_period_0.csv");
            //CreateCustomPeriod0IdMap(zip, dest + Path.DirectorySeparatorChar + "period_0_id_map.csv", interval_lenght);

            //Execute queries:
            //for (int i = 1; i < nSamples; i++)
            //{
            //    //ExecuteTestQueries(zip, $"{csvFilePath}/genhr_{i}.csv", GENERATORGENERATION_PERIOD_ALLSAMPLES, ""+i, nIntervalLength);
            //    //ExecuteTestQueries(zip, $"{csvFilePath}/lineflowinterval_{i}s.csv", LINEFLOW_PERIOD_ALLSAMPLES, "" + i, nIntervalLength);
            //}
            //nFiles += ExecuteTestQueryToCsv(zip, $"{csvFilePath}/generatorgenerationinterval.csv", GENERATORGENERATION_PERIOD, "1,2,3,4", nIntervalLength);
            //nFiles += ExecuteTestQueryToCsv(zip, $"{csvFilePath}/lineflowinterval.csv", LINEFLOW_PERIOD, "1,2,3,4", nIntervalLength);
            //nFiles += ExecuteTestQueryToCsv(zip, $"{csvFilePath}/nodepriceinterval.csv", NODEPRICE_PERIOD, "1,2,3,4", nIntervalLength);
            var qryTime = new Stopwatch();
            qryTime.Start();
            SolutionResultList sol = QryToListAllObjects(zip, phase_id, collection, period_id, property, sSampleList);
            Console.WriteLine($"Finished executing queries. Time: {qryTime.ElapsedMilliseconds / 1000} sec.");

            //Write output file:
            string outputFile = dest + Path.DirectorySeparatorChar + $"{phase_id}_{collection}_{property}.csv";
            WriteResultListToFile(sol, outputFile);
            Console.WriteLine($"Finished writing csv files. Total time: {qryTime.ElapsedMilliseconds / 1000} sec.");
            qryTime.Stop();
            zip.Close();
            Console.ReadKey();
        }





    }


}
