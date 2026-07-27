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
            var model = new CMDPerformanceDashboardVM
            {
                FromDate = fromDate.Date,
                ToDate = toDate.Date,

                DailyProduction = new ProductionSummaryVM(),
                MTDProduction = new ProductionSummaryVM(),
                YTDProduction = new ProductionSummaryVM(),

                Downtime = new List<DowntimeSummaryVM>(),
                EquipmentFailures = new List<TopFailureVM>(),
                RCAFailures = new List<TopFailureVM>(),
                ClosureRates = new List<ClosureRateVM>(),
                TopDelays = new List<CMDTopDelayVM>()
            };

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(
                "dbo.sp_GetCMDPerformanceDashboard",
                connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 120;

                command.Parameters.Add(
                    "@FromDate",
                    SqlDbType.DateTime
                ).Value = fromDate.Date;

                command.Parameters.Add(
                    "@ToDate",
                    SqlDbType.DateTime
                ).Value = toDate.Date;

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model.DailyProduction =
                            ReadProductionSummary(reader);
                    }

                    if (reader.NextResult() && reader.Read())
                    {
                        model.MTDProduction =
                            ReadProductionSummary(reader);
                    }

                    if (reader.NextResult() && reader.Read())
                    {
                        model.YTDProduction =
                            ReadProductionSummary(reader);
                    }

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            model.Downtime.Add(new DowntimeSummaryVM
                            {
                                Plant = GetString(reader, "Plant"),

                                DTDMechanical =
                                    GetDecimal(reader, "DTDMechanical"),

                                DTDElectrical =
                                    GetDecimal(reader, "DTDElectrical"),

                                DTDCranes =
                                    GetDecimal(reader, "DTDCranes"),

                                DTDUtilities =
                                    GetDecimal(reader, "DTDUtilities"),

                                MTDMechanical =
                                    GetDecimal(reader, "MTDMechanical"),

                                MTDElectrical =
                                    GetDecimal(reader, "MTDElectrical"),

                                MTDCranes =
                                    GetDecimal(reader, "MTDCranes"),

                                MTDUtilities =
                                    GetDecimal(reader, "MTDUtilities")
                            });
                        }
                    }

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            model.EquipmentFailures.Add(new TopFailureVM
                            {
                                Plant = GetString(reader, "Plant"),
                                Name = GetString(reader, "Name"),
                                DelayHours = GetDecimal(reader, "DelayHours"),
                                FailureType = GetString(reader, "FailureType")
                            });
                        }
                    }

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            model.RCAFailures.Add(new TopFailureVM
                            {
                                Plant = GetString(reader, "Plant"),
                                Name = GetString(reader, "Name"),
                                DelayHours = GetDecimal(reader, "DelayHours"),
                                FailureType = GetString(reader, "FailureType")
                            });
                        }
                    }

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            model.ClosureRates.Add(new ClosureRateVM
                            {
                                Plant = GetString(reader, "Plant"),
                                Department = GetString(reader, "Department"),
                                MonthName = GetString(reader, "MonthName"),
                                MonthNumber = GetInt(reader, "MonthNumber"),

                                ClosurePercentage =
                                    GetDecimal(reader, "ClosurePercentage")
                            });
                        }
                    }

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            model.TopDelays.Add(new CMDTopDelayVM
                            {
                                Plant = GetString(reader, "Plant"),
                                Shift = GetString(reader, "Shift"),

                                TotalDuration =
                                    GetDecimal(reader, "TotalDuration"),

                                DelayCode =
                                    GetString(reader, "DelayCode"),

                                EquipmentCode =
                                    GetString(reader, "EquipmentCode"),

                                Description =
                                    GetString(reader, "Description"),

                                ReasonForOccurrence =
                                    GetString(reader, "ReasonForOccurrence"),

                                ActionTaken =
                                    GetString(reader, "ActionTaken")
                            });
                        }
                    }
                }
            }

            return model;
        }

        private static ProductionSummaryVM ReadProductionSummary(
            SqlDataReader reader)
        {
            return new ProductionSummaryVM
            {
                SMP = GetDecimal(reader, "SMP"),

                RM1 = GetDecimal(reader, "RM1"),

                RM2 = GetDecimal(reader, "RM2"),

                ComparisonPercentage = GetDecimal(
                    reader,
                    "ComparisonPercentage"
                )
            };
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
