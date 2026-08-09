using DAL.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPICode.Helpers;

namespace BAL.Repositories
{
    public class SMPReportsRepository
    {
        public static DataTable _dt;
        public static DataSet _ds;
        public SMPReportsRepository() : base()
        {
            _dt = new DataTable();
            _ds = new DataSet();
        }

        //public SMPDailyPerformanceReportBLL GetDailyPerformanceReport(DateTime reportDate)
        //{
        //    SqlParameter[] p =
        //    {
        //        new SqlParameter("@ReportDate", SqlDbType.Date)
        //        {
        //            Value = reportDate.Date
        //        }
        //    };

        //    DataTable dt = DBHelper.ExecuteDataTable(
        //        "sp_GetSMPDailyPerformanceReport",
        //        CommandType.StoredProcedure,
        //        p
        //    );

        //    var m = new SMPDailyPerformanceReportBLL
        //    {
        //        ReportDate = reportDate.Date
        //    };

        //    if (dt == null || dt.Rows.Count == 0)
        //        return m;

        //    DataRow r = dt.Rows[0];

        //    m.NumberOfHeats = I(r, "NumberOfHeats");
        //    m.DRI = D(r, "DRI");
        //    m.Scrap = D(r, "Scrap");
        //    m.LiquidSteel = D(r, "LiquidSteel");
        //    m.CastedWeight = D(r, "CastedWeight");
        //    m.TapToTap = D(r, "TapToTap");
        //    m.Availability = D(r, "Availability");
        //    m.Performance = D(r, "Performance");
        //    m.Yield = D(r, "Yield");
        //    m.QualityYield = D(r, "QualityYield");
        //    m.EAFProductivity = D(r, "EAFProductivity");
        //    m.CCMProductivity = D(r, "CCMProductivity");
        //    m.DRIKgPerTon = D(r, "DRIKgPerTon");
        //    m.ScrapKgPerTon = D(r, "ScrapKgPerTon");
        //    m.FeSiKgPerTon = D(r, "FeSiKgPerTon");
        //    m.FeSiMnKgPerTon = D(r, "FeSiMnKgPerTon");
        //    m.FeMnKgPerTon = D(r, "FeMnKgPerTon");
        //    m.RiceHuskKgPerTon = D(r, "RiceHuskKgPerTon");
        //    m.LimeKgPerTon = D(r, "LimeKgPerTon");
        //    m.DoloLimeKgPerTon = D(r, "DoloLimeKgPerTon");
        //    m.ChargeCoalKgPerTon = D(r, "ChargeCoalKgPerTon");
        //    m.FluorsparKgPerTon = D(r, "FluorsparKgPerTon");
        //    m.CalcinedCarbonKgPerTon = D(r, "CalcinedCarbonKgPerTon");
        //    m.AluminiumKgPerTon = D(r, "AluminiumKgPerTon");
        //    m.PowerKwhPerTon = D(r, "PowerKwhPerTon");
        //    m.LPGNm3PerTon = D(r, "LPGNm3PerTon");
        //    m.OxygenNm3PerTon = D(r, "OxygenNm3PerTon");
        //    m.ArgonNm3PerTon = D(r, "ArgonNm3PerTon");
        //    m.NitrogenNm3PerTon = D(r, "NitrogenNm3PerTon");
        //    m.WaterM3 = D(r, "WaterM3");
        //    m.MechanicalDelay = D(r, "MechanicalDelay");
        //    m.ElectricalDelay = D(r, "ElectricalDelay");
        //    m.OperationDelay = D(r, "OperationDelay");
        //    m.RefractoryDelay = D(r, "RefractoryDelay");
        //    m.UtilityDelay = D(r, "UtilityDelay");
        //    m.CraneDelay = D(r, "CraneDelay");
        //    m.Remarks = S(r, "Remarks");

        //    return m;
        //}

        //private static decimal D(DataRow r, string c)
        //{
        //    decimal x;
        //    return r.Table.Columns.Contains(c) &&
        //           r[c] != DBNull.Value &&
        //           decimal.TryParse(Convert.ToString(r[c]), out x)
        //        ? x : 0M;
        //}

        //private static int I(DataRow r, string c)
        //{
        //    int x;
        //    return r.Table.Columns.Contains(c) &&
        //           r[c] != DBNull.Value &&
        //           int.TryParse(Convert.ToString(r[c]), out x)
        //        ? x : 0;
        //}

        //private static string S(DataRow r, string c)
        //{
        //    return r.Table.Columns.Contains(c) && r[c] != DBNull.Value
        //        ? Convert.ToString(r[c]).Trim()
        //        : "";
        //}
        public SMPDashboardVM GetDashboard(
                    DateTime fromDate,
                    DateTime toDate)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@FromDate", SqlDbType.Date)
                {
                    Value = fromDate.Date
                },
                new SqlParameter("@ToDate", SqlDbType.Date)
                {
                    Value = toDate.Date
                }
            };

            DataSet ds = DBHelper.ExecuteDataSet(
                "sp_GetSMPDashboard",
                CommandType.StoredProcedure,
                p
            );

            var model = new SMPDashboardVM
            {
                FromDate = fromDate.Date,
                ToDate = toDate.Date
            };

            if (ds == null)
                return model;

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow r = ds.Tables[0].Rows[0];

                model.HeatCount = I(r, "HeatCount");
                model.BilletOutput = D(r, "BilletOutput");
                model.Yield = D(r, "Yield");
                model.Productivity = D(r, "Productivity");
                model.PowerConsumption = D(r, "PowerConsumption");
                model.TapToTap = D(r, "TapToTap");
                model.PowerOnTime = D(r, "PowerOnTime");
                model.HeatWeight = D(r, "HeatWeight");
                model.Availability = D(r, "Availability");
                model.Performance = D(r, "Performance");
                model.QualityYield = D(r, "QualityYield");

                model.DailyActual = D(r, "DailyActual");
                model.DailyTarget = D(r, "DailyTarget");
                model.MTDActual = D(r, "MTDActual");
                model.MTDTarget = D(r, "MTDTarget");
                model.YTDActual = D(r, "YTDActual");
                model.YTDTarget = D(r, "YTDTarget");
                model.YearlyAchievement = D(r, "YearlyAchievement");

                model.FeSi = D(r, "FeSi");
                model.SiMn = D(r, "SiMn");
                model.Fluorspar = D(r, "Fluorspar");
                model.CalcinedCarbon = D(r, "CalcinedCarbon");
                model.ChargeCoal = D(r, "ChargeCoal");
                model.RiceHusk = D(r, "RiceHusk");
                model.Lime = D(r, "Lime");
                model.DoloLime = D(r, "DoloLime");

                model.LPG = D(r, "LPG");
                model.Oxygen = D(r, "Oxygen");
                model.Argon = D(r, "Argon");
                model.Nitrogen = D(r, "Nitrogen");

                model.DRIShare = D(r, "DRIShare");
                model.HBIShare = D(r, "HBIShare");
                model.ScrapShare = D(r, "ScrapShare");

                model.AvailableShare = D(r, "AvailableShare");
                model.OperationShare = D(r, "OperationShare");
                model.MechanicalShare = D(r, "MechanicalShare");
                model.ElectricalShare = D(r, "ElectricalShare");
                model.RefractoryShare = D(r, "RefractoryShare");
                model.OtherShare = D(r, "OtherShare");
            }

            if (ds.Tables.Count > 1)
            {
                foreach (DataRow r in ds.Tables[1].Rows)
                {
                    model.TopDelays.Add(new SMPDelayItemBLL
                    {
                        DelayName = S(r, "DelayName"),
                        Area = S(r, "Area"),
                        Minutes = D(r, "Minutes")
                    });
                }
            }

            if (ds.Tables.Count > 2)
            {
                foreach (DataRow r in ds.Tables[2].Rows)
                {
                    model.DailyProduction.Add(new SMPDailyProductionPointBLL
                    {
                        DayNo = I(r, "DayNo"),
                        ProductionDate = DT(r, "ProductionDate"),
                        DateLabel = S(r, "DateLabel"),
                        Actual = D(r, "Actual"),
                        Plan = D(r, "Plan")
                    });
                }
            }

            return model;
        }

        private static decimal D(DataRow r, string c)
        {
            decimal x;
            return r.Table.Columns.Contains(c) &&
                   r[c] != DBNull.Value &&
                   decimal.TryParse(Convert.ToString(r[c]), out x)
                ? x : 0M;
        }

        private static int I(DataRow r, string c)
        {
            int x;
            return r.Table.Columns.Contains(c) &&
                   r[c] != DBNull.Value &&
                   int.TryParse(Convert.ToString(r[c]), out x)
                ? x : 0;
        }

        private static string S(DataRow r, string c)
        {
            return r.Table.Columns.Contains(c) && r[c] != DBNull.Value
                ? Convert.ToString(r[c]).Trim()
                : "";
        }

        private static DateTime DT(DataRow r, string c)
        {
            DateTime x;

            return r.Table.Columns.Contains(c) &&
                   r[c] != DBNull.Value &&
                   DateTime.TryParse(Convert.ToString(r[c]), out x)
                ? x
                : DateTime.MinValue;
        }
    }
}