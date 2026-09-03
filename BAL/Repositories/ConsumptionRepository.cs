using DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using WebAPICode.Helpers;
using static DAL.Models.ViewModel;

namespace BAL.Repositories
{
    public class ConsumptionRepository
    {
        public static DataTable _dt;
        public static DataSet _ds;
        private static readonly string connectionString = "data source=10.1.10.115\\PROD01;initial catalog=Production_Solb;persist security info=True;user id=WebReportViewer;password=WebReportViewer;";


        public ConsumptionRepository()
        {
            _dt =
                new DataTable();

            _ds =
                new DataSet();
        }

        public List<PlantConsumptionBLL>
            GetPlantConsumptionGroupByID(
                int id)
        {
            try
            {
                List<PlantConsumptionBLL> list =
                    new List<PlantConsumptionBLL>();


                SqlParameter[] p =
                {
                    new SqlParameter(
                        "@ID",
                        SqlDbType.Int
                    )
                    {
                        Value = id
                    }
                };


                DataTable dt =
                    new DBHelper()
                        .GetTableFromSP(
                            "sp_GetPlantConsumptionGroupByID",
                            p
                        );


                if (
                    dt != null &&
                    dt.Rows.Count > 0
                )
                {
                    list =
                        JArray
                            .Parse(
                                JsonConvert
                                    .SerializeObject(
                                        dt
                                    )
                            )
                            .ToObject<
                                List<PlantConsumptionBLL>
                            >();
                }


                return list;
            }
            catch
            {
                throw;
            }
        }


        // =====================================================
        // GET ALL PLANTS BY DATE
        // =====================================================

        public List<PlantConsumptionBLL>
            GetPlantConsumptionByDateAll(
                DateTime date)
        {
            try
            {
                List<PlantConsumptionBLL> list =
                    new List<PlantConsumptionBLL>();


                SqlParameter[] p =
                {
                    new SqlParameter(
                        "@Date",
                        SqlDbType.Date
                    )
                    {
                        Value =
                            date.Date
                    }
                };


                DataTable dt =
                    new DBHelper()
                        .GetTableFromSP(
                            "sp_GetPlantConsumptionByDateAll",
                            p
                        );


                if (
                    dt != null &&
                    dt.Rows.Count > 0
                )
                {
                    list =
                        JArray
                            .Parse(
                                JsonConvert
                                    .SerializeObject(
                                        dt
                                    )
                            )
                            .ToObject<
                                List<PlantConsumptionBLL>
                            >();
                }


                return list;
            }
            catch
            {
                throw;
            }
        }


        // =====================================================
        // SAVE SMP + RM1 + RM2
        // =====================================================

        public int SavePlantWiseConsumption(
            List<PlantConsumptionBLL> records)
        {
            try
            {
                if (
                    records == null ||
                    records.Count == 0
                )
                {
                    return 0;
                }


                int firstSavedID =
                    0;


                foreach (
                    PlantConsumptionBLL model
                    in records
                )
                {
                    if (model == null)
                    {
                        continue;
                    }


                    SqlParameter[] p =
                    {
                new SqlParameter(
                    "@ID",
                    SqlDbType.Int
                )
                {
                    Value =
                        model.ID
                },


                new SqlParameter(
                    "@Date",
                    SqlDbType.Date
                )
                {
                    Value =
                        model.Date.HasValue
                            ? (object)model.Date.Value.Date
                            : DBNull.Value
                },


                new SqlParameter(
                    "@Plant",
                    SqlDbType.NVarChar,
                    50
                )
                {
                    Value =
                        string.IsNullOrWhiteSpace(
                            model.Plant
                        )
                            ? (object)DBNull.Value
                            : model.Plant
                                .Trim()
                                .ToUpper()
                },


                /* ==========================================
                   ACTUAL VALUES
                   ========================================== */

                new SqlParameter(
                    "@TotalProductBillet",
                    SqlDbType.Decimal
                )
                {
                    Precision = 18,
                    Scale = 3,

                    Value =
                        model.TotalProductBillet.HasValue
                            ? (object)model.TotalProductBillet.Value
                            : DBNull.Value
                },


                new SqlParameter(
                    "@LPG",
                    SqlDbType.Decimal
                )
                {
                    Precision = 18,
                    Scale = 3,

                    Value =
                        model.LPG.HasValue
                            ? (object)model.LPG.Value
                            : DBNull.Value
                },


                new SqlParameter(
                    "@Oxygen",
                    SqlDbType.Decimal
                )
                {
                    Precision = 18,
                    Scale = 3,

                    Value =
                        model.Oxygen.HasValue
                            ? (object)model.Oxygen.Value
                            : DBNull.Value
                },


                new SqlParameter(
                    "@Nitrogen",
                    SqlDbType.Decimal
                )
                {
                    Precision = 18,
                    Scale = 3,

                    Value =
                        model.Nitrogen.HasValue
                            ? (object)model.Nitrogen.Value
                            : DBNull.Value
                },


                new SqlParameter(
                    "@Argon",
                    SqlDbType.Decimal
                )
                {
                    Precision = 18,
                    Scale = 3,

                    Value =
                        model.Argon.HasValue
                            ? (object)model.Argon.Value
                            : DBNull.Value
                },


                new SqlParameter(
                    "@WaterConsumption",
                    SqlDbType.Decimal
                )
                {
                    Precision = 18,
                    Scale = 3,

                    Value =
                        model.WaterConsumption.HasValue
                            ? (object)model.WaterConsumption.Value
                            : DBNull.Value
                },


                new SqlParameter(
                    "@PowerConsumption",
                    SqlDbType.Decimal
                )
                {
                    Precision = 18,
                    Scale = 3,

                    Value =
                        model.PowerConsumption.HasValue
                            ? (object)model.PowerConsumption.Value
                            : DBNull.Value
                },


                new SqlParameter(
                    "@FuelConsumption",
                    SqlDbType.Decimal
                )
                {
                    Precision = 18,
                    Scale = 3,

                    Value =
                        model.FuelConsumption.HasValue
                            ? (object)model.FuelConsumption.Value
                            : DBNull.Value
                },


                /* ==========================================
                   ACTUAL PER TON VALUES
                   ========================================== */

                new SqlParameter(
                    "@LPGm3ton",
                    SqlDbType.Decimal
                )
                {
                    Precision = 18,
                    Scale = 3,

                    Value =
                        model.LPGm3ton.HasValue
                            ? (object)model.LPGm3ton.Value
                            : DBNull.Value
                },


                new SqlParameter(
                    "@OxygenNm3ton",
                    SqlDbType.Decimal
                )
                {
                    Precision = 18,
                    Scale = 3,

                    Value =
                        model.OxygenNm3ton.HasValue
                            ? (object)model.OxygenNm3ton.Value
                            : DBNull.Value
                },


                new SqlParameter(
                    "@NitrogenNm3ton",
                    SqlDbType.Decimal
                )
                {
                    Precision = 18,
                    Scale = 3,

                    Value =
                        model.NitrogenNm3ton.HasValue
                            ? (object)model.NitrogenNm3ton.Value
                            : DBNull.Value
                },


                new SqlParameter(
                    "@ArgonNm3ton",
                    SqlDbType.Decimal
                )
                {
                    Precision = 18,
                    Scale = 3,

                    Value =
                        model.ArgonNm3ton.HasValue
                            ? (object)model.ArgonNm3ton.Value
                            : DBNull.Value
                },


                new SqlParameter(
                    "@PowerConsumptionKWHton",
                    SqlDbType.Decimal
                )
                {
                    Precision = 18,
                    Scale = 3,

                    Value =
                        model.PowerConsumptionKWHton.HasValue
                            ? (object)model.PowerConsumptionKWHton.Value
                            : DBNull.Value
                },


                new SqlParameter(
                    "@WaterConsumptionM3",
                    SqlDbType.Decimal
                )
                {
                    Precision = 18,
                    Scale = 3,

                    Value =
                        model.WaterConsumptionM3.HasValue
                            ? (object)model.WaterConsumptionM3.Value
                            : DBNull.Value
                },


                /* ==========================================
                   AUDIT
                   ========================================== */

                new SqlParameter(
                    "@StatusID",
                    SqlDbType.Int
                )
                {
                    Value =
                        model.StatusID ?? 1
                },


                new SqlParameter(
                    "@CreatedBy",
                    SqlDbType.NVarChar,
                    100
                )
                {
                    Value =
                        string.IsNullOrWhiteSpace(
                            model.CreatedBy
                        )
                            ? (object)DBNull.Value
                            : model.CreatedBy
                },


                new SqlParameter(
                    "@CreatedDate",
                    SqlDbType.DateTime
                )
                {
                    Value =
                        model.CreatedDate.HasValue
                            ? (object)model.CreatedDate.Value
                            : DBNull.Value
                },


                new SqlParameter(
                    "@UpdatedBy",
                    SqlDbType.NVarChar,
                    100
                )
                {
                    Value =
                        string.IsNullOrWhiteSpace(
                            model.UpdatedBy
                        )
                            ? (object)DBNull.Value
                            : model.UpdatedBy
                },


                new SqlParameter(
                    "@UpdatedDate",
                    SqlDbType.DateTime
                )
                {
                    Value =
                        model.UpdatedDate.HasValue
                            ? (object)model.UpdatedDate.Value
                            : DBNull.Value
                }
            };


                    int savedID =
                        new DBHelper()
                            .ExecuteNonQueryReturn(
                                "sp_SavePlantConsumption",
                                p
                            );


                    if (
                        firstSavedID == 0 &&
                        savedID > 0
                    )
                    {
                        firstSavedID =
                            savedID;
                    }
                }


                return firstSavedID;
            }
            catch
            {
                throw;
            }
        }



        // =====================================================
        // GET PRODUCTION BY DATE
        //
        // SMP  = SMPDayWiseProduction.TotalCastedTon
        // RM1  = BundlesSection.TotalWeight by RM1
        // RM2  = BundlesSection.TotalWeight by RM2
        // =====================================================

        public decimal GetSMPProductionByDate(
            DateTime date)
        {
            const string sql = @"
                SELECT
                    ISNULL(
                        SUM(
                            CONVERT(
                                DECIMAL(18,4),
                                TotalCastedTon
                            )
                        ),
                        0
                    )
                FROM dbo.SMPDayWiseProduction
                WHERE [Date] = @Date
                  AND StatusID = 1;";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        connectionString
                    )
            )
            using (
                SqlCommand command =
                    new SqlCommand(
                        sql,
                        connection
                    )
            )
            {
                command.CommandType =
                    CommandType.Text;

                command.Parameters.Add(
                    "@Date",
                    SqlDbType.Date
                ).Value = date.Date;


                connection.Open();

                object value =
                    command.ExecuteScalar();


                if (
                    value == null ||
                    value == DBNull.Value
                )
                {
                    return 0m;
                }


                return Convert.ToDecimal(
                    value
                );
            }
        }

        public DataTable GetProductionByDate(
            DateTime date)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter(
                        "@Date",
                        SqlDbType.Date
                    )
                    {
                        Value =
                            date.Date
                    }
                };


                DataTable dt =
                    new DBHelper()
                        .GetTableFromSP(
                            "sp_GetPlantProductionByDate",
                            p
                        );


                return dt
                    ?? new DataTable();
            }
            catch
            {
                throw;
            }
        }


        // =====================================================
        // DELETE
        // =====================================================

        public int DeletePlantConsumption(
            int id,
            string updatedBy)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter(
                        "@ID",
                        id
                    ),

                    new SqlParameter(
                        "@UpdatedBy",
                        DbValue(
                            updatedBy
                        )
                    )
                };


                return new DBHelper()
                    .ExecuteNonQueryReturn(
                        "sp_DeletePlantConsumption",
                        p
                    );
            }
            catch
            {
                throw;
            }
        }


        // =====================================================
        // COMMON DB NULL HANDLER
        // =====================================================

        private object DbValue(
            object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }


            string text =
                value as string;


            if (
                text != null &&
                string.IsNullOrWhiteSpace(
                    text
                )
            )
            {
                return DBNull.Value;
            }


            return value;
        }

        public UtilityDailyReportVM GetUtilityDailyReport(DateTime reportDate)
        {
            DateTime selectedDate = reportDate.Date;

            UtilityDailyReportVM vm = new UtilityDailyReportVM
            {
                ReportDate = selectedDate,
                Records = new List<PlantConsumptionBLL>(),
                SMP = new PlantConsumptionBLL { Plant = "SMP" },
                RM1 = new PlantConsumptionBLL { Plant = "RM1" },
                RM2 = new PlantConsumptionBLL { Plant = "RM2" }
            };

            try
            {
                SqlParameter[] utilityParameters =
                {
            new SqlParameter("@Date", SqlDbType.Date)
            {
                Value = selectedDate
            }
        };

                DataTable utilityTable = new DBHelper().GetTableFromSP(
                    "sp_GetUtilityDailyReport",
                    utilityParameters
                );

                if (utilityTable != null)
                {
                    foreach (DataRow row in utilityTable.Rows)
                    {
                        PlantConsumptionBLL item = new PlantConsumptionBLL
                        {
                            ID = GetInt(row, "ID"),
                            Date = GetDate(row, "Date"),
                            Plant = GetString(row, "Plant"),

                            // Production is overwritten below from
                            // sp_GetPlantProductionByDate.
                            TotalProductBillet = 0m,

                            LPG = GetNullableDecimal(row, "LPG"),
                            Oxygen = GetNullableDecimal(row, "Oxygen"),
                            Nitrogen = GetNullableDecimal(row, "Nitrogen"),
                            Argon = GetNullableDecimal(row, "Argon"),
                            WaterConsumption = GetNullableDecimal(row, "WaterConsumption"),
                            PowerConsumption = GetNullableDecimal(row, "PowerConsumption"),

                            LPGm3ton = GetNullableDecimal(row, "LPGm3ton"),
                            LPGNm3tonTarget = GetNullableDecimal(row, "LPGNm3tonTarget"),
                            OxygenNm3ton = GetNullableDecimal(row, "OxygenNm3ton"),
                            OxygenNm3tonTarget = GetNullableDecimal(row, "OxygenNm3tonTarget"),
                            NitrogenNm3ton = GetNullableDecimal(row, "NitrogenNm3ton"),
                            NitrogenNm3tonTarget = GetNullableDecimal(row, "NitrogenNm3tonTarget"),
                            ArgonNm3ton = GetNullableDecimal(row, "ArgonNm3ton"),
                            ArgonNm3tonTarget = GetNullableDecimal(row, "ArgonNm3tonTarget"),
                            PowerConsumptionKWHton = GetNullableDecimal(row, "PowerConsumptionKWHton"),
                            PowerConsumptionKWHtarget = GetNullableDecimal(row, "PowerConsumptionKWHtarget"),
                            WaterConsumptionM3 = GetNullableDecimal(row, "WaterConsumptionM3"),
                            WaterConsumptionTarget = GetNullableDecimal(row, "WaterConsumptionTarget"),
                            FuelConsumption = GetNullableDecimal(row, "FuelConsumption"),
                            FuelConsumptionTarget = GetNullableDecimal(row, "FuelConsumptionTarget"),

                            StatusID = GetNullableInt(row, "StatusID"),
                            CreatedBy = GetString(row, "CreatedBy"),
                            CreatedDate = GetDate(row, "CreatedDate"),
                            UpdatedBy = GetString(row, "UpdatedBy"),
                            UpdatedDate = GetDate(row, "UpdatedDate")
                        };

                        vm.Records.Add(item);

                        string plantCode = NormalizePlantCode(item.Plant);

                        if (plantCode == "SMP")
                        {
                            item.Plant = "SMP";
                            vm.SMP = item;
                        }
                        else if (plantCode == "RM1")
                        {
                            item.Plant = "RM1";
                            vm.RM1 = item;
                        }
                        else if (plantCode == "RM2")
                        {
                            item.Plant = "RM2";
                            vm.RM2 = item;
                        }
                    }
                }

                // Always execute this after utility binding. This replaces any old
                // TotalProductBillet value returned by sp_GetUtilityDailyReport.
                ApplyPlantProductionByDate(vm, selectedDate);

                return vm;
            }
            catch (Exception ex)
            {
                // Do not silently return incorrect/zero production.
                throw new DataException(
                    "Utility Daily Report could not bind production for " +
                    selectedDate.ToString("yyyy-MM-dd") + ".",
                    ex
                );
            }
        }

        private void ApplyPlantProductionByDate(
            UtilityDailyReportVM vm,
            DateTime reportDate)
        {
            if (vm == null)
            {
                throw new ArgumentNullException("vm");
            }

            SqlParameter[] productionParameters =
            {
        new SqlParameter("@Date", SqlDbType.Date)
        {
            Value = reportDate.Date
        }
    };

            DataTable productionTable = new DBHelper().GetTableFromSP(
                "sp_GetPlantProductionByDate",
                productionParameters
            );

            decimal smpProduction = 0m;
            decimal rm1Production = 0m;
            decimal rm2Production = 0m;

            if (productionTable != null && productionTable.Rows.Count > 0)
            {
                DataRow productionRow = productionTable.Rows[0];

                smpProduction = GetRequiredProductionDecimal(
                    productionRow,
                    "SMPProduction"
                );

                rm1Production = GetRequiredProductionDecimal(
                    productionRow,
                    "RM1Production"
                );

                rm2Production = GetRequiredProductionDecimal(
                    productionRow,
                    "RM2Production"
                );
            }

            if (vm.SMP == null)
            {
                vm.SMP = new PlantConsumptionBLL { Plant = "SMP" };
            }

            if (vm.RM1 == null)
            {
                vm.RM1 = new PlantConsumptionBLL { Plant = "RM1" };
            }

            if (vm.RM2 == null)
            {
                vm.RM2 = new PlantConsumptionBLL { Plant = "RM2" };
            }

            vm.SMP.TotalProductBillet = Math.Round(smpProduction, 3);
            vm.RM1.TotalProductBillet = Math.Round(rm1Production, 3);
            vm.RM2.TotalProductBillet = Math.Round(rm2Production, 3);
        }

        private static decimal GetRequiredProductionDecimal(
            DataRow row,
            string columnName)
        {
            if (row == null ||
                row.Table == null ||
                !row.Table.Columns.Contains(columnName))
            {
                throw new DataException(
                    "sp_GetPlantProductionByDate did not return column '" +
                    columnName + "'."
                );
            }

            if (row[columnName] == DBNull.Value)
            {
                return 0m;
            }

            decimal value;

            if (!decimal.TryParse(
                Convert.ToString(row[columnName]),
                out value))
            {
                throw new DataException(
                    "Invalid decimal value returned in column '" +
                    columnName + "'."
                );
            }

            return value;
        }

        private static string NormalizePlantCode(string plant)
        {
            string value = (plant ?? string.Empty)
                .Trim()
                .ToUpperInvariant()
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace("_", string.Empty);

            if (value == "SMP" || value == "STEELMAKINGPLANT")
            {
                return "SMP";
            }

            if (value == "RM1" || value == "ROLLINGMILL1")
            {
                return "RM1";
            }

            if (value == "RM2" || value == "ROLLINGMILL2")
            {
                return "RM2";
            }

            return value;
        }


        private string GetString(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return "";

            if (row[columnName] == DBNull.Value || row[columnName] == null)
                return "";

            return Convert.ToString(row[columnName]);
        }

        private int GetInt(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return 0;

            if (row[columnName] == DBNull.Value || row[columnName] == null)
                return 0;

            int result;
            return int.TryParse(row[columnName].ToString(), out result) ? result : 0;
        }

        private int? GetNullableInt(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return null;

            if (row[columnName] == DBNull.Value || row[columnName] == null)
                return null;

            int result;
            return int.TryParse(row[columnName].ToString(), out result) ? result : (int?)null;
        }

        private decimal? GetNullableDecimal(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return null;

            if (row[columnName] == DBNull.Value || row[columnName] == null)
                return null;

            decimal result;
            return decimal.TryParse(row[columnName].ToString(), out result) ? result : (decimal?)null;
        }

        private DateTime? GetDate(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return null;

            if (row[columnName] == DBNull.Value || row[columnName] == null)
                return null;

            DateTime result;
            return DateTime.TryParse(row[columnName].ToString(), out result) ? result : (DateTime?)null;
        }

        public List<PlantConsumptionBLL> GetPlantConsumption(
    DateTime fromDate,
    DateTime toDate)
        {
            try
            {
                var list =
                    new List<PlantConsumptionBLL>();

                SqlParameter[] p =
                {
            new SqlParameter(
                "@FromDate",
                SqlDbType.Date
            )
            {
                Value = fromDate.Date
            },

            new SqlParameter(
                "@ToDate",
                SqlDbType.Date
            )
            {
                Value = toDate.Date
            }
        };

                DataTable dt =
                    new DBHelper()
                        .GetTableFromSP(
                            "sp_GetPlantConsumption",
                            p
                        );

                if (
                    dt != null &&
                    dt.Rows.Count > 0
                )
                {
                    list =
                        JArray
                            .Parse(
                                Newtonsoft.Json
                                    .JsonConvert
                                    .SerializeObject(
                                        dt
                                    )
                            )
                            .ToObject<
                                List<PlantConsumptionBLL>
                            >();
                }

                return list;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public PlantConsumptionBLL GetUtilityDataByDate(
            DateTime fromDate,
            DateTime toDate,
            string plant)
        {
            var result = new PlantConsumptionBLL();

            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd =
                new SqlCommand(
                    "sp_GetUtilityDataByDate",
                    con))
            {
                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.Add(
                    "@FromDate",
                    SqlDbType.Date
                ).Value = fromDate.Date;

                cmd.Parameters.Add(
                    "@ToDate",
                    SqlDbType.Date
                ).Value = toDate.Date;

                cmd.Parameters.Add(
                    "@Plant",
                    SqlDbType.NVarChar,
                    50
                ).Value =
                    string.IsNullOrWhiteSpace(plant)
                        ? (object)DBNull.Value
                        : plant.Trim();

                con.Open();

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result.FuelConsumption =
                            reader["FuelConsumption"] ==
                            DBNull.Value
                                ? 0m
                                : Convert.ToDecimal(
                                    reader["FuelConsumption"]
                                );

                        result.PowerConsumption =
                            reader["PowerConsumption"] ==
                            DBNull.Value
                                ? 0m
                                : Convert.ToDecimal(
                                    reader["PowerConsumption"]
                                );

                        result.WaterConsumption =
                            reader["WaterConsumption"] ==
                            DBNull.Value
                                ? 0m
                                : Convert.ToDecimal(
                                    reader["WaterConsumption"]
                                );
                    }
                }
            }

            return result;
        }
    }
}