using DAL.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace BAL.Repositories
{
    public class ExecutiveDashboardRepository
    {
        private static readonly string connectionString = "data source=10.1.10.115\\PROD01;initial catalog=Production_Solb;persist security info=True;user id=WebReportViewer;password=WebReportViewer;";

        //public ExecutiveDashboardRepository(string connectionString)
        //{
        //    if (string.IsNullOrWhiteSpace(connectionString))
        //    {
        //        throw new ArgumentException("Database connection string is required.", "connectionString");
        //    }

        //    this.connectionString = connectionString;
        //}

        public ExecutiveOperationsDashboardVM GetExecutiveDashboard(
            DateTime fromDate,
            DateTime toDate)
        {
            fromDate = fromDate.Date;
            toDate = toDate.Date;

            if (fromDate > toDate)
            {
                DateTime swap = fromDate;
                fromDate = toDate;
                toDate = swap;
            }

            ExecutiveOperationsDashboardVM model =
                new ExecutiveOperationsDashboardVM
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    GeneratedOn = DateTime.Now
                };

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(
                "dbo.sp_GetExecutiveOperationsDashboard",
                connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 180;

                command.Parameters.Add("@FromDate", SqlDbType.Date).Value = fromDate;
                command.Parameters.Add("@ToDate", SqlDbType.Date).Value = toDate;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // Result set 1: executive summary
                    if (reader.Read())
                    {
                        model.Summary = new ExecutiveSummaryVM
                        {
                            TotalProduction = GetDecimal(reader, "TotalProduction"),
                            SMPYield = GetDecimal(reader, "SMPYield"),
                            SMPAvailability = GetDecimal(reader, "SMPAvailability"),
                            DispatchMTD = GetDecimal(reader, "DispatchMTD"),
                            DispatchMTDTarget = GetDecimal(reader, "DispatchMTDTarget"),
                            PowerConsumption = GetDecimal(reader, "PowerConsumption"),
                            PowerTarget = GetDecimal(reader, "PowerTarget"),
                            TotalDowntimeMinutes = GetDecimal(reader, "TotalDowntimeMinutes"),
                            MaintenanceClosure = GetDecimal(reader, "MaintenanceClosure"),
                            HealthScore = GetDecimal(reader, "HealthScore"),
                            SMPPlanVariancePercentage = GetDecimal(reader, "SMPPlanVariancePercentage"),
                            DataQualityScore = GetDecimal(reader, "DataQualityScore"),
                            DataWarningCount = GetInt(reader, "DataWarningCount"),
                            DataQualityStatus = GetString(reader, "DataQualityStatus")
                        };
                    }

                    // Result set 2: selected / MTD / YTD production
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            ProductionPeriodVM period = new ProductionPeriodVM
                            {
                                PeriodName = GetString(reader, "PeriodName"),
                                SMP = GetDecimal(reader, "SMP"),
                                RM1 = GetDecimal(reader, "RM1"),
                                RM2 = GetDecimal(reader, "RM2"),
                                SMPPlan = GetDecimal(reader, "SMPPlan")
                            };

                            if (period.PeriodName.Equals(
                                "Selected",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                model.SelectedProduction = period;
                            }
                            else if (period.PeriodName.Equals(
                                "MTD",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                model.MTDProduction = period;
                            }
                            else if (period.PeriodName.Equals(
                                "YTD",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                model.YTDProduction = period;
                            }
                        }
                    }

                    // Result set 3: last 12 months production
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            model.ProductionTrend.Add(new ProductionTrendVM
                            {
                                MonthStart = GetDateTime(reader, "MonthStart") ?? DateTime.MinValue,
                                MonthLabel = GetString(reader, "MonthLabel"),
                                SMP = GetNullableDecimal(reader, "SMP"),
                                RM1 = GetNullableDecimal(reader, "RM1"),
                                RM2 = GetNullableDecimal(reader, "RM2"),
                                Total = GetNullableDecimal(reader, "Total")
                            });
                        }
                    }

                    // Result set 4: supply chain snapshot
                    if (reader.NextResult() && reader.Read())
                    {
                        model.SupplyChain = new SupplyChainSnapshotVM
                        {
                            ReportDate = GetDateTime(reader, "ReportDate"),
                            RawMaterialStock = GetDecimal(reader, "RawMaterialStock"),
                            FinishedGoodsStock = GetDecimal(reader, "FinishedGoodsStock"),
                            DailyDispatch = GetDecimal(reader, "DailyDispatch"),
                            DailyDispatchTarget = GetDecimal(reader, "DailyDispatchTarget"),
                            DailyTruck = GetDecimal(reader, "DailyTruck"),
                            DailyTruckTarget = GetDecimal(reader, "DailyTruckTarget"),
                            MTDDispatch = GetDecimal(reader, "MTDDispatch"),
                            MTDDispatchTarget = GetDecimal(reader, "MTDDispatchTarget"),
                            MTDTruck = GetDecimal(reader, "MTDTruck"),
                            MTDTruckTarget = GetDecimal(reader, "MTDTruckTarget")
                        };
                    }

                    // Result set 5: utilities
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            model.Utilities.Add(new UtilityComplianceVM
                            {
                                UtilityName = GetString(reader, "UtilityName"),
                                Unit = GetString(reader, "Unit"),
                                Actual = GetDecimal(reader, "Actual"),
                                Target = GetDecimal(reader, "Target"),
                                VariancePercentage = GetDecimal(reader, "VariancePercentage"),
                                Status = GetString(reader, "Status")
                            });
                        }
                    }

                    // Result set 6: department performance
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            model.Departments.Add(new DepartmentPerformanceVM
                            {
                                DepartmentCode = GetString(reader, "DepartmentCode"),
                                DepartmentName = GetString(reader, "DepartmentName"),
                                Score = GetDecimal(reader, "Score"),
                                MetricLabel = GetString(reader, "MetricLabel"),
                                IsBenchmarkBased = GetBool(reader, "IsBenchmarkBased"),
                                PrimaryText = GetString(reader, "PrimaryText"),
                                SecondaryText = GetString(reader, "SecondaryText")
                            });
                        }
                    }

                    // Result set 7: risks
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            model.TopRisks.Add(new OperationalRiskVM
                            {
                                Rank = GetInt(reader, "Rank"),
                                RiskTitle = GetString(reader, "RiskTitle"),
                                RiskDetail = GetString(reader, "RiskDetail"),
                                RiskValue = GetString(reader, "RiskValue"),
                                RiskScore = GetDecimal(reader, "RiskScore"),
                                Severity = GetString(reader, "Severity")
                            });
                        }
                    }

                    // Result set 8: open management actions
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            model.ManagementActions.Add(new ManagementActionVM
                            {
                                Priority = GetString(reader, "Priority"),
                                Action = GetString(reader, "Action"),
                                Owner = GetString(reader, "Owner"),
                                DueDate = GetDateTime(reader, "DueDate"),
                                Plant = GetString(reader, "Plant")
                            });
                        }
                    }

                    // Result set 9: executive KPI trend
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            model.ExecutiveTrend.Add(new ExecutiveTrendVM
                            {
                                MonthStart = GetDateTime(reader, "MonthStart") ?? DateTime.MinValue,
                                MonthLabel = GetString(reader, "MonthLabel"),
                                ProductionScore = GetNullableDecimal(reader, "ProductionScore"),
                                AvailabilityScore = GetNullableDecimal(reader, "AvailabilityScore"),
                                MaintenanceScore = GetNullableDecimal(reader, "MaintenanceScore"),
                                SupplyChainScore = GetNullableDecimal(reader, "SupplyChainScore")
                            });
                        }
                    }

                    // Result set 10: data-validation issues
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            model.DataValidationIssues.Add(new DataValidationIssueVM
                            {
                                IssueID = GetInt(reader, "IssueID"),
                                SourceName = GetString(reader, "SourceName"),
                                FieldName = GetString(reader, "FieldName"),
                                IssueMessage = GetString(reader, "IssueMessage"),
                                AffectedRows = GetInt(reader, "AffectedRows"),
                                Severity = GetString(reader, "Severity")
                            });
                        }
                    }
                }
            }

            return model;
        }

        private static int GetOrdinal(SqlDataReader reader, string columnName)
        {
            for (int index = 0; index < reader.FieldCount; index++)
            {
                if (reader.GetName(index).Equals(
                    columnName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return index;
                }
            }

            return -1;
        }

        private static string GetString(SqlDataReader reader, string columnName)
        {
            int ordinal = GetOrdinal(reader, columnName);

            if (ordinal < 0 || reader.IsDBNull(ordinal))
            {
                return string.Empty;
            }

            return Convert.ToString(reader.GetValue(ordinal)).Trim();
        }

        private static decimal GetDecimal(SqlDataReader reader, string columnName)
        {
            int ordinal = GetOrdinal(reader, columnName);

            if (ordinal < 0 || reader.IsDBNull(ordinal))
            {
                return 0m;
            }

            decimal value;

            return decimal.TryParse(
                Convert.ToString(reader.GetValue(ordinal)),
                out value)
                ? value
                : 0m;
        }

        private static decimal? GetNullableDecimal(
            SqlDataReader reader,
            string columnName)
        {
            int ordinal = GetOrdinal(reader, columnName);

            if (ordinal < 0 || reader.IsDBNull(ordinal))
            {
                return null;
            }

            decimal value;

            return decimal.TryParse(
                Convert.ToString(reader.GetValue(ordinal)),
                out value)
                ? value
                : (decimal?)null;
        }

        private static bool GetBool(SqlDataReader reader, string columnName)
        {
            int ordinal = GetOrdinal(reader, columnName);

            if (ordinal < 0 || reader.IsDBNull(ordinal))
            {
                return false;
            }

            object rawValue = reader.GetValue(ordinal);

            if (rawValue is bool)
            {
                return (bool)rawValue;
            }

            int intValue;

            return int.TryParse(Convert.ToString(rawValue), out intValue)
                && intValue != 0;
        }

        private static int GetInt(SqlDataReader reader, string columnName)
        {
            int ordinal = GetOrdinal(reader, columnName);

            if (ordinal < 0 || reader.IsDBNull(ordinal))
            {
                return 0;
            }

            int value;

            return int.TryParse(
                Convert.ToString(reader.GetValue(ordinal)),
                out value)
                ? value
                : 0;
        }

        private static DateTime? GetDateTime(
            SqlDataReader reader,
            string columnName)
        {
            int ordinal = GetOrdinal(reader, columnName);

            if (ordinal < 0 || reader.IsDBNull(ordinal))
            {
                return null;
            }

            DateTime value;

            return DateTime.TryParse(
                Convert.ToString(reader.GetValue(ordinal)),
                out value)
                ? value
                : (DateTime?)null;
        }
    }
}
