using DAL.Models;
using Newtonsoft.Json;
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

        public SMPDashboardVM GetSMPDashboard(
                    DateTime fromDate,
                    DateTime toDate)
        {
            var model =
                new SMPDashboardVM
                {
                    FromDate =
                        fromDate.Date,

                    ToDate =
                        toDate.Date
                };


            SqlParameter[] parameters =
            {
           new SqlParameter(
               "@FromDate",
               SqlDbType.Date
           )
           {
               Value =
                   fromDate.Date
           },

           new SqlParameter(
               "@ToDate",
               SqlDbType.Date
           )
           {
               Value =
                   toDate.Date
           }
       };


            DataSet ds =
                new DBHelper()
                    .GetDatasetFromSP(
                        "sp_GetSMPDashboardFromDayWiseProduction",
                        parameters
                    );


            if (
                ds == null ||
                ds.Tables.Count == 0
            )
            {
                return model;
            }


            /* =====================================================
               RESULT SET 1 - SUMMARY
               ===================================================== */

            if (
                ds.Tables.Count > 0 &&
                ds.Tables[0] != null &&
                ds.Tables[0].Rows.Count > 0
            )
            {
                DataRow row =
                    ds.Tables[0].Rows[0];


                model.FromDate =
                    GetDate(
                        row,
                        "FromDate",
                        fromDate.Date
                    );

                model.ToDate =
                    GetDate(
                        row,
                        "ToDate",
                        toDate.Date
                    );


                model.HeatCount =
                    GetDecimal(
                        row,
                        "HeatCount"
                    );

                model.BilletOutput =
                    GetDecimal(
                        row,
                        "BilletOutput"
                    );

                model.Yield =
                    GetDecimal(
                        row,
                        "Yield"
                    );

                model.Productivity =
                    GetDecimal(
                        row,
                        "Productivity"
                    );

                model.PowerConsumption =
                    GetDecimal(
                        row,
                        "PowerConsumption"
                    );

                model.TapToTap =
                    GetDecimal(
                        row,
                        "TapToTap"
                    );

                model.PowerOnTime =
                    GetDecimal(
                        row,
                        "PowerOnTime"
                    );

                model.HeatWeight =
                    GetDecimal(
                        row,
                        "HeatWeight"
                    );


                model.Performance =
                    GetDecimal(
                        row,
                        "Performance"
                    );

                model.Availability =
                    GetDecimal(
                        row,
                        "Availability"
                    );

                model.QualityYield =
                    GetDecimal(
                        row,
                        "QualityYield"
                    );


                model.DailyActual =
                    GetDecimal(
                        row,
                        "DailyActual"
                    );

                model.DailyTarget =
                    GetDecimal(
                        row,
                        "DailyTarget"
                    );

                model.PeriodTarget =
                    GetDecimal(
                        row,
                        "PeriodTarget"
                    );

                model.MTDActual =
                    GetDecimal(
                        row,
                        "MTDActual"
                    );

                model.MTDTarget =
                    GetDecimal(
                        row,
                        "MTDTarget"
                    );

                model.YTDActual =
                    GetDecimal(
                        row,
                        "YTDActual"
                    );

                model.YTDTarget =
                    GetDecimal(
                        row,
                        "YTDTarget"
                    );


                /*
                 * Yearly Achievement =
                 * YTD Actual / 700000 * 100
                 */
                model.YearlyTarget =
                    GetDecimal(
                        row,
                        "YearlyTarget"
                    );

                if (model.YearlyTarget <= 0)
                {
                    model.YearlyTarget =
                        700000m;
                }

                model.YearlyAchievement =
                    model.YearlyTarget > 0
                        ? Math.Round(
                            (
                                model.YTDActual /
                                model.YearlyTarget
                            ) * 100,
                            2
                        )
                        : 0;


                /* Raw Material */
                model.FeSi =
                    GetDecimal(row, "FeSi");

                model.SiMn =
                    GetDecimal(row, "SiMn");

                model.Fluorspar =
                    GetDecimal(row, "Fluorspar");

                model.CalcinedCarbon =
                    GetDecimal(row, "CalcinedCarbon");

                model.ChargeCoal =
                    GetDecimal(row, "ChargeCoal");

                model.RiceHusk =
                    GetDecimal(row, "RiceHusk");

                model.Lime =
                    GetDecimal(row, "Lime");

                model.DoloLime =
                    GetDecimal(row, "DoloLime");


                /* Utilities */
                model.LPG =
                    GetDecimal(row, "LPG");

                model.Oxygen =
                    GetDecimal(row, "Oxygen");

                model.Argon =
                    GetDecimal(row, "Argon");

                model.Nitrogen =
                    GetDecimal(row, "Nitrogen");


                /* ================================================
                   CHARGE MIX %
                   ================================================ */

                decimal driCharge =
                    GetDecimal(
                        row,
                        "DRICharge"
                    );

                decimal hbiCharge =
                    GetDecimal(
                        row,
                        "HBICharge"
                    );

                decimal scrapCharge =
                    GetDecimal(
                        row,
                        "ScrapCharge"
                    );

                decimal totalCharge =
                    driCharge
                    +
                    hbiCharge
                    +
                    scrapCharge;


                if (totalCharge > 0)
                {
                    model.DRIShare =
                        Math.Round(
                            (driCharge / totalCharge) * 100,
                            2
                        );

                    model.HBIShare =
                        Math.Round(
                            (hbiCharge / totalCharge) * 100,
                            2
                        );

                    model.ScrapShare =
                        Math.Round(
                            (scrapCharge / totalCharge) * 100,
                            2
                        );
                }


                /* ================================================
                   AVAILABILITY BREAKDOWN

                   Available part is taken directly from the
                   day-wise Availability KPI.

                   Remaining percentage is distributed between
                   delay categories according to delay minutes.
                   ================================================ */

                decimal operationDelay =
                    GetDecimal(
                        row,
                        "OperationDelayMinutes"
                    );

                decimal mechanicalDelay =
                    GetDecimal(
                        row,
                        "MechanicalDelayMinutes"
                    );

                decimal electricalDelay =
                    GetDecimal(
                        row,
                        "ElectricalDelayMinutes"
                    );

                decimal refractoryDelay =
                    GetDecimal(
                        row,
                        "RefractoryDelayMinutes"
                    );

                decimal otherDelay =
                    GetDecimal(
                        row,
                        "OtherDelayMinutes"
                    );


                decimal totalDelay =
                    operationDelay
                    +
                    mechanicalDelay
                    +
                    electricalDelay
                    +
                    refractoryDelay
                    +
                    otherDelay;


                decimal available =
                    model.Availability;

                if (available < 0)
                    available = 0;

                if (available > 100)
                    available = 100;


                model.AvailableShare =
                    Math.Round(
                        available,
                        2
                    );


                decimal unavailableShare =
                    100 - available;


                if (
                    totalDelay > 0 &&
                    unavailableShare > 0
                )
                {
                    model.OperationShare =
                        Math.Round(
                            unavailableShare
                            *
                            operationDelay
                            /
                            totalDelay,
                            2
                        );

                    model.MechanicalShare =
                        Math.Round(
                            unavailableShare
                            *
                            mechanicalDelay
                            /
                            totalDelay,
                            2
                        );

                    model.ElectricalShare =
                        Math.Round(
                            unavailableShare
                            *
                            electricalDelay
                            /
                            totalDelay,
                            2
                        );

                    model.RefractoryShare =
                        Math.Round(
                            unavailableShare
                            *
                            refractoryDelay
                            /
                            totalDelay,
                            2
                        );

                    model.OtherShare =
                        Math.Round(
                            unavailableShare
                            *
                            otherDelay
                            /
                            totalDelay,
                            2
                        );
                }
                else
                {
                    /*
                     * If there is no delay split,
                     * put the non-available balance under Other.
                     */
                    model.OtherShare =
                        Math.Round(
                            unavailableShare,
                            2
                        );
                }
            }


            /* =====================================================
               RESULT SET 2 - TOP 5 DELAYS
               ===================================================== */

            if (
                ds.Tables.Count > 1 &&
                ds.Tables[1] != null &&
                ds.Tables[1].Rows.Count > 0
            )
            {
                model.TopDelays =
                    JArray
                        .Parse(
                            JsonConvert.SerializeObject(
                                ds.Tables[1]
                            )
                        )
                        .ToObject<
                            List<SMPDelayItemBLL>
                        >();
            }


            /* =====================================================
               RESULT SET 3 - DAILY PRODUCTION
               ===================================================== */

            if (
                ds.Tables.Count > 2 &&
                ds.Tables[2] != null &&
                ds.Tables[2].Rows.Count > 0
            )
            {
                model.DailyProduction =
                    JArray
                        .Parse(
                            JsonConvert.SerializeObject(
                                ds.Tables[2]
                            )
                        )
                        .ToObject<
                            List<SMPDailyProductionPointBLL>
                        >();
            }


            return model;
        }


        private decimal GetDecimal(
            DataRow row,
            string columnName)
        {
            if (
                row == null ||
                row.Table == null ||
                !row.Table.Columns.Contains(columnName) ||
                row[columnName] == DBNull.Value
            )
            {
                return 0;
            }

            decimal value;

            if (
                decimal.TryParse(
                    Convert.ToString(
                        row[columnName]
                    ),
                    out value
                )
            )
            {
                return value;
            }

            return 0;
        }


        private DateTime GetDate(
            DataRow row,
            string columnName,
            DateTime defaultValue)
        {
            if (
                row == null ||
                row.Table == null ||
                !row.Table.Columns.Contains(columnName) ||
                row[columnName] == DBNull.Value
            )
            {
                return defaultValue;
            }

            DateTime value;

            if (
                DateTime.TryParse(
                    Convert.ToString(
                        row[columnName]
                    ),
                    out value
                )
            )
            {
                return value;
            }

            return defaultValue;
        }




        /* ============================================================
           HELPERS
           ============================================================ */

        private int GetInt(
            DataRow row,
            string columnName)
        {
            if (
                row == null ||
                !row.Table.Columns.Contains(
                    columnName
                ) ||
                row[columnName] == DBNull.Value ||
                row[columnName] == null
            )
            {
                return 0;
            }


            int value;

            if (
                int.TryParse(
                    Convert.ToString(
                        row[columnName]
                    ),
                    out value
                )
            )
            {
                return value;
            }


            return 0;
        }



        private string GetString(
            DataRow row,
            string columnName)
        {
            if (
                row == null ||
                !row.Table.Columns.Contains(
                    columnName
                ) ||
                row[columnName] == DBNull.Value ||
                row[columnName] == null
            )
            {
                return "";
            }


            return Convert.ToString(
                row[columnName]
            );
        }



        private DateTime GetDateTime(
            DataRow row,
            string columnName,
            DateTime defaultValue)
        {
            if (
                row == null ||
                !row.Table.Columns.Contains(
                    columnName
                ) ||
                row[columnName] == DBNull.Value ||
                row[columnName] == null
            )
            {
                return defaultValue;
            }


            DateTime value;

            if (
                DateTime.TryParse(
                    Convert.ToString(
                        row[columnName]
                    ),
                    out value
                )
            )
            {
                return value;
            }


            return defaultValue;
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