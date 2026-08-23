using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repositories
{
    public class MaintenanceRepository
    {
        private static readonly string connectionString = "data source=10.1.10.115\\PROD01;initial catalog=Production_Solb;persist security info=True;user id=WebReportViewer;password=WebReportViewer;";

        public CMDPerformanceDashboardVM GetDashboard(
            DateTime fromDate,
            DateTime toDate)
        {
            fromDate = fromDate.Date;
            toDate = toDate.Date;

            if (fromDate > toDate)
            {
                DateTime temp = fromDate;
                fromDate = toDate;
                toDate = temp;
            }

            CMDPerformanceDashboardVM model =
                new CMDPerformanceDashboardVM
                {
                    FromDate = fromDate,
                    ToDate = toDate,

                    DailyProduction =
                        new ProductionSummaryVM(),

                    MTDProduction =
                        new ProductionSummaryVM(),

                    YTDProduction =
                        new ProductionSummaryVM(),

                    Downtime =
                        new List<DowntimeSummaryVM>(),

                    EquipmentFailures =
                        new List<TopFailureVM>(),

                    RCAFailures =
                        new List<TopFailureVM>(),

                    ClosureRates =
                        new List<ClosureRateVM>(),

                    TopDelays =
                        new List<CMDTopDelayVM>()
                };

            using (
                SqlConnection connection =
                    new SqlConnection(
                        connectionString
                    )
            )
            using (
                SqlCommand command =
                    new SqlCommand(
                        "dbo.sp_GetCMDPerformanceDashboard",
                        connection
                    )
            )
            {
                command.CommandType =
                    CommandType.StoredProcedure;

                command.CommandTimeout =
                    120;

                command.Parameters.Add(
                    "@FromDate",
                    SqlDbType.Date
                ).Value = fromDate;

                command.Parameters.Add(
                    "@ToDate",
                    SqlDbType.Date
                ).Value = toDate;

                connection.Open();

                using (
                    SqlDataReader reader =
                        command.ExecuteReader()
                )
                {
                    // 1 - Selected Date / Selected Period Production
                    if (reader.Read())
                    {
                        model.DailyProduction =
                            ReadProductionSummary(
                                reader
                            );
                    }

                    // 2 - MTD Production
                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            model.MTDProduction =
                                ReadProductionSummary(
                                    reader
                                );
                        }
                    }

                    // 3 - YTD Production
                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            model.YTDProduction =
                                ReadProductionSummary(
                                    reader
                                );
                        }
                    }

                    // 4 - Downtime %
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            model.Downtime.Add(
                                new DowntimeSummaryVM
                                {
                                    Plant =
                                        NormalizeCMDPlant(
                                            GetString(
                                                reader,
                                                "Plant"
                                            )
                                        ),

                                    DTDMechanical =
                                        GetDecimal(
                                            reader,
                                            "DTDMechanical"
                                        ),

                                    DTDElectrical =
                                        GetDecimal(
                                            reader,
                                            "DTDElectrical"
                                        ),

                                    DTDCranes =
                                        GetDecimal(
                                            reader,
                                            "DTDCranes"
                                        ),

                                    DTDUtilities =
                                        GetDecimal(
                                            reader,
                                            "DTDUtilities"
                                        ),

                                    MTDMechanical =
                                        GetDecimal(
                                            reader,
                                            "MTDMechanical"
                                        ),

                                    MTDElectrical =
                                        GetDecimal(
                                            reader,
                                            "MTDElectrical"
                                        ),

                                    MTDCranes =
                                        GetDecimal(
                                            reader,
                                            "MTDCranes"
                                        ),

                                    MTDUtilities =
                                        GetDecimal(
                                            reader,
                                            "MTDUtilities"
                                        )
                                }
                            );
                        }
                    }

                    // 5 - Equipment Failures
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            model.EquipmentFailures.Add(
                                new TopFailureVM
                                {
                                    Plant =
                                        NormalizeCMDPlant(
                                            GetString(
                                                reader,
                                                "Plant"
                                            )
                                        ),

                                    Name =
                                        GetString(
                                            reader,
                                            "Name"
                                        ),

                                    DelayHours =
                                        GetDecimal(
                                            reader,
                                            "DelayHours"
                                        ),

                                    FailureType =
                                        GetString(
                                            reader,
                                            "FailureType"
                                        )
                                }
                            );
                        }
                    }

                    // 6 - RCA Failures
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            model.RCAFailures.Add(
                                new TopFailureVM
                                {
                                    Plant =
                                        NormalizeCMDPlant(
                                            GetString(
                                                reader,
                                                "Plant"
                                            )
                                        ),

                                    Name =
                                        GetString(
                                            reader,
                                            "Name"
                                        ),

                                    DelayHours =
                                        GetDecimal(
                                            reader,
                                            "DelayHours"
                                        ),

                                    FailureType =
                                        GetString(
                                            reader,
                                            "FailureType"
                                        )
                                }
                            );
                        }
                    }

                    // 7 - Closure Rates
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            model.ClosureRates.Add(
                                new ClosureRateVM
                                {
                                    Plant =
                                        NormalizeCMDPlant(
                                            GetString(
                                                reader,
                                                "Plant"
                                            )
                                        ),

                                    Department =
                                        GetString(
                                            reader,
                                            "Department"
                                        ),

                                    MonthName =
                                        GetString(
                                            reader,
                                            "MonthName"
                                        ),

                                    MonthNumber =
                                        GetInt(
                                            reader,
                                            "MonthNumber"
                                        ),

                                    ClosurePercentage =
                                        GetDecimal(
                                            reader,
                                            "ClosurePercentage"
                                        )
                                }
                            );
                        }
                    }

                    // 8 - Top Delays
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            model.TopDelays.Add(
                                new CMDTopDelayVM
                                {
                                    Plant =
                                        NormalizeCMDPlant(
                                            GetString(
                                                reader,
                                                "Plant"
                                            )
                                        ),

                                    Shift =
                                        GetString(
                                            reader,
                                            "Shift"
                                        ),

                                    AgencyName =
                                        GetString(
                                            reader,
                                            "AgencyName"
                                        ),

                                    TotalDuration =
                                        GetDecimal(
                                            reader,
                                            "TotalDuration"
                                        ),

                                    DelayCode =
                                        GetString(
                                            reader,
                                            "DelayCode"
                                        ),

                                    EquipmentCode =
                                        GetString(
                                            reader,
                                            "EquipmentCode"
                                        ),

                                    Description =
                                        GetString(
                                            reader,
                                            "Description"
                                        ),

                                    ReasonForOccurrence =
                                        GetString(
                                            reader,
                                            "ReasonForOccurrence"
                                        ),

                                    ActionTaken =
                                        GetString(
                                            reader,
                                            "ActionTaken"
                                        )
                                }
                            );
                        }
                    }
                }
            }

            /*
               Defensive repository filter:
               the stored procedure already normalizes the delay source, but only
               the four CMD agencies are allowed to reach agency-labelled report
               sections even if an older/changed procedure returns extra rows.

               RCA rows are already restricted through #Delays in the procedure;
               their FailureType field contains RCA/report information, not agency.
            */
            HashSet<string> allowedAgencies =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase
                )
                {
                "Mechanical",
                "Electrical",
                "Cranes",
                "Utility"
                };

            HashSet<string> allowedClosureDepartments =
                new HashSet<string>(
                    allowedAgencies,
                    StringComparer.OrdinalIgnoreCase
                )
                {
                "Total"
                };

            model.EquipmentFailures =
                model.EquipmentFailures
                    .Where(x =>
                        x != null &&
                        !string.IsNullOrWhiteSpace(x.FailureType) &&
                        allowedAgencies.Contains(
                            x.FailureType.Trim()
                        )
                    )
                    .ToList();

            model.ClosureRates =
                model.ClosureRates
                    .Where(x =>
                        x != null &&
                        !string.IsNullOrWhiteSpace(x.Department) &&
                        allowedClosureDepartments.Contains(
                            x.Department.Trim()
                        )
                    )
                    .ToList();

            model.TopDelays =
                model.TopDelays
                    .Where(x =>
                        x != null &&
                        !string.IsNullOrWhiteSpace(x.AgencyName) &&
                        allowedAgencies.Contains(
                            x.AgencyName.Trim()
                        )
                    )
                    .ToList();

            return model;
        }

        private ProductionSummaryVM ReadProductionSummary(
            SqlDataReader reader)
        {
            return new ProductionSummaryVM
            {
                SMP =
                    GetDecimal(
                        reader,
                        "SMP"
                    ),

                RM1 =
                    GetDecimal(
                        reader,
                        "RM1"
                    ),

                RM2 =
                    GetDecimal(
                        reader,
                        "RM2"
                    ),

                ComparisonPercentage =
                    GetDecimal(
                        reader,
                        "ComparisonPercentage"
                    )
            };
        }
        private string NormalizeCMDPlant(
            string plant)
        {
            string value =
                (plant ?? "")
                    .Trim()
                    .ToUpper();

            if (
                value == "SMP" ||
                value == "STEEL MAKING" ||
                value == "STEEL MAKING PLANT" ||
                value == "MELT SHOP" ||
                value == "EAF" ||
                value == "LF" ||
                value == "CCM"
            )
            {
                return "SMP";
            }

            if (
                value == "RM1" ||
                value == "ROLLING MILL 1" ||
                value == "ROLLING MILL1"
            )
            {
                return "RM1";
            }

            if (
                value == "RM2" ||
                value == "ROLLING MILL 2" ||
                value == "ROLLING MILL2"
            )
            {
                return "RM2";
            }

            return value;
        }

        private static string GetString(
            IDataRecord reader,
            string columnName)
        {
            int ordinal;

            try
            {
                ordinal = reader.GetOrdinal(columnName);
            }
            catch (IndexOutOfRangeException)
            {
                return string.Empty;
            }

            if (reader.IsDBNull(ordinal))
            {
                return string.Empty;
            }

            return Convert.ToString(reader.GetValue(ordinal));
        }

        private static decimal GetDecimal(
            IDataRecord reader,
            string columnName)
        {
            int ordinal;

            try
            {
                ordinal = reader.GetOrdinal(columnName);
            }
            catch (IndexOutOfRangeException)
            {
                return 0;
            }

            if (reader.IsDBNull(ordinal))
            {
                return 0;
            }

            decimal value;

            return decimal.TryParse(
                Convert.ToString(reader.GetValue(ordinal)),
                out value
            )
                ? value
                : 0;
        }

        private static int GetInt(
            IDataRecord reader,
            string columnName)
        {
            int ordinal;

            try
            {
                ordinal = reader.GetOrdinal(columnName);
            }
            catch (IndexOutOfRangeException)
            {
                return 0;
            }

            if (reader.IsDBNull(ordinal))
            {
                return 0;
            }

            int value;

            return int.TryParse(
                Convert.ToString(reader.GetValue(ordinal)),
                out value
            )
                ? value
                : 0;
        }
    }
}