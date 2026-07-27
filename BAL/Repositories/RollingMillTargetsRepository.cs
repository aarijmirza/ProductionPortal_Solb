using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebAPICode.Helpers;

namespace DAL.Repository
{
    public class RollingMillTargetsRepository
    {
        public List<RollingMillTargetsBLL> GetAll()
        {
            var list = new List<RollingMillTargetsBLL>();

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_GetAllRollingMillTargets",
                CommandType.StoredProcedure
            );

            if (dt == null || dt.Rows.Count == 0)
            {
                return list;
            }

            foreach (DataRow row in dt.Rows)
            {
                list.Add(Map(row));
            }

            return list;
        }

        public RollingMillTargetsBLL GetByID(int id)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", SqlDbType.Int)
                {
                    Value = id
                }
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_GetRollingMillTargetByID",
                CommandType.StoredProcedure,
                parameters
            );

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return Map(dt.Rows[0]);
        }

        public int Insert(RollingMillTargetsBLL model)
        {
            SqlParameter[] parameters =
                GetInsertParameters(model);

            object result = DBHelper.ExecuteScalar(
                "sp_InsertRollingMillTargets",
                CommandType.StoredProcedure,
                parameters
            );

            if (result == null || result == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(result);
        }

        public bool Update(RollingMillTargetsBLL model)
        {
            SqlParameter[] parameters =
                GetUpdateParameters(model);

            int affectedRows = DBHelper.ExecuteNonQuery(
                "sp_UpdateRollingMillTargets",
                CommandType.StoredProcedure,
                parameters
            );

            return affectedRows > 0;
        }

        public bool Delete(int id)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", SqlDbType.Int)
                {
                    Value = id
                }
            };

            int affectedRows = DBHelper.ExecuteNonQuery(
                "sp_DeleteRollingMillTarget",
                CommandType.StoredProcedure,
                parameters
            );

            return affectedRows > 0;
        }

        private SqlParameter[] GetInsertParameters(
            RollingMillTargetsBLL model)
        {
            return new[]
            {
                CreateNVarCharParameter(
                    "@Month",
                    model.Month,
                    20
                ),

                CreateNVarCharParameter(
                    "@Year",
                    model.Year,
                    4
                ),

                CreateNVarCharParameter(
                    "@Size",
                    model.Size,
                    50
                ),

                CreateNVarCharParameter(
                    "@Profile",
                    model.Profile,
                    50
                ),

                CreateDecimalParameter(
                    "@YeildPercentageRBTarget",
                    model.YeildPercentageRBTarget
                ),

                CreateDecimalParameter(
                    "@YeildPercentageWRTarget",
                    model.YeildPercentageWRTarget
                ),

                CreateDecimalParameter(
                    "@YeildPercentagePBTarget",
                    model.YeildPercentagePBTarget
                ),

                CreateDecimalParameter(
                    "@YeildPercentageRICTarget",
                    model.YeildPercentageRICTarget
                ),

                CreateDecimalParameter(
                    "@RRRPercentageTarget",
                    model.RRRPercentageTarget
                ),

                CreateDecimalParameter(
                    "@TonperhourTarget",
                    model.TonperhourTarget
                ),

                CreateDecimalParameter(
                    "@FuelOilTarget",
                    model.FuelOilTarget
                ),

                CreateDecimalParameter(
                    "@ElectricityTarget",
                    model.ElectricityTarget
                ),

                CreateDecimalParameter(
                    "@WaterTarget",
                    model.WaterTarget
                ),

                CreateDecimalParameter(
                    "@ProductionTarget",
                    model.ProductionTarget
                ),

                CreateDecimalParameter(
                    "@GuidePassTarget",
                    model.GuidePassTarget
                ),

                CreateDecimalParameter(
                    "@RollShopTarget",
                    model.RollShopTarget
                ),

                CreateDecimalParameter(
                    "@ElectricalTarget",
                    model.ElectricalTarget
                ),

                CreateDecimalParameter(
                    "@MechanicalTarget",
                    model.MechanicalTarget
                ),

                CreateDecimalParameter(
                    "@CraneTarget",
                    model.CraneTarget
                ),

                CreateDecimalParameter(
                    "@DispatchTarget",
                    model.DispatchTarget
                ),

                CreateDecimalParameter(
                    "@QualityTarget",
                    model.QualityTarget
                ),

                CreateDecimalParameter(
                    "@UtilityTarget",
                    model.UtilityTarget
                ),

                CreateDecimalParameter(
                    "@OthersTarget",
                    model.OthersTarget
                ),

                CreateDecimalParameter(
                    "@SizeChangeTarget",
                    model.SizeChangeTarget
                ),

                CreateDecimalParameter(
                    "@DownDayTarget",
                    model.DownDayTarget
                ),

                CreateDecimalParameter(
                    "@PowerFailureTarget",
                    model.PowerFailureTarget
                ),

                CreateDecimalParameter(
                    "@NoBilletTarget",
                    model.NoBilletTarget
                ),

                CreateDecimalParameter(
                    "@AnnualShutdownTarget",
                    model.AnnualShutdownTarget
                ),

                CreateNVarCharParameter(
                    "@CreatedBy",
                    model.CreatedBy,
                    100
                )
            };
        }

        private SqlParameter[] GetUpdateParameters(
            RollingMillTargetsBLL model)
        {
            return new[]
            {
                new SqlParameter("@ID", SqlDbType.Int)
                {
                    Value = model.ID
                },

                CreateNVarCharParameter(
                    "@Month",
                    model.Month,
                    20
                ),

                CreateNVarCharParameter(
                    "@Year",
                    model.Year,
                    4
                ),

                CreateNVarCharParameter(
                    "@Size",
                    model.Size,
                    50
                ),

                CreateNVarCharParameter(
                    "@Profile",
                    model.Profile,
                    50
                ),

                CreateDecimalParameter(
                    "@YeildPercentageRBTarget",
                    model.YeildPercentageRBTarget
                ),

                CreateDecimalParameter(
                    "@YeildPercentageWRTarget",
                    model.YeildPercentageWRTarget
                ),

                CreateDecimalParameter(
                    "@YeildPercentagePBTarget",
                    model.YeildPercentagePBTarget
                ),

                CreateDecimalParameter(
                    "@YeildPercentageRICTarget",
                    model.YeildPercentageRICTarget
                ),

                CreateDecimalParameter(
                    "@RRRPercentageTarget",
                    model.RRRPercentageTarget
                ),

                CreateDecimalParameter(
                    "@TonperhourTarget",
                    model.TonperhourTarget
                ),

                CreateDecimalParameter(
                    "@FuelOilTarget",
                    model.FuelOilTarget
                ),

                CreateDecimalParameter(
                    "@ElectricityTarget",
                    model.ElectricityTarget
                ),

                CreateDecimalParameter(
                    "@WaterTarget",
                    model.WaterTarget
                ),

                CreateDecimalParameter(
                    "@ProductionTarget",
                    model.ProductionTarget
                ),

                CreateDecimalParameter(
                    "@GuidePassTarget",
                    model.GuidePassTarget
                ),

                CreateDecimalParameter(
                    "@RollShopTarget",
                    model.RollShopTarget
                ),

                CreateDecimalParameter(
                    "@ElectricalTarget",
                    model.ElectricalTarget
                ),

                CreateDecimalParameter(
                    "@MechanicalTarget",
                    model.MechanicalTarget
                ),

                CreateDecimalParameter(
                    "@CraneTarget",
                    model.CraneTarget
                ),

                CreateDecimalParameter(
                    "@DispatchTarget",
                    model.DispatchTarget
                ),

                CreateDecimalParameter(
                    "@QualityTarget",
                    model.QualityTarget
                ),

                CreateDecimalParameter(
                    "@UtilityTarget",
                    model.UtilityTarget
                ),

                CreateDecimalParameter(
                    "@OthersTarget",
                    model.OthersTarget
                ),

                CreateDecimalParameter(
                    "@SizeChangeTarget",
                    model.SizeChangeTarget
                ),

                CreateDecimalParameter(
                    "@DownDayTarget",
                    model.DownDayTarget
                ),

                CreateDecimalParameter(
                    "@PowerFailureTarget",
                    model.PowerFailureTarget
                ),

                CreateDecimalParameter(
                    "@NoBilletTarget",
                    model.NoBilletTarget
                ),

                CreateDecimalParameter(
                    "@AnnualShutdownTarget",
                    model.AnnualShutdownTarget
                )
            };
        }

        private RollingMillTargetsBLL Map(DataRow row)
        {
            return new RollingMillTargetsBLL
            {
                ID = GetInt(row, "ID"),

                Month = GetString(row, "Month"),
                Year = GetString(row, "Year"),

                Size = GetString(row, "Size"),
                Profile = GetString(row, "Profile"),

                YeildPercentageRBTarget =
                    GetDecimal(
                        row,
                        "YeildPercentageRBTarget"
                    ),

                YeildPercentageWRTarget =
                    GetDecimal(
                        row,
                        "YeildPercentageWRTarget"
                    ),

                YeildPercentagePBTarget =
                    GetDecimal(
                        row,
                        "YeildPercentagePBTarget"
                    ),

                YeildPercentageRICTarget =
                    GetDecimal(
                        row,
                        "YeildPercentageRICTarget"
                    ),

                RRRPercentageTarget =
                    GetDecimal(
                        row,
                        "RRRPercentageTarget"
                    ),

                TonperhourTarget =
                    GetDecimal(
                        row,
                        "TonperhourTarget"
                    ),

                FuelOilTarget =
                    GetDecimal(
                        row,
                        "FuelOilTarget"
                    ),

                ElectricityTarget =
                    GetDecimal(
                        row,
                        "ElectricityTarget"
                    ),

                WaterTarget =
                    GetDecimal(
                        row,
                        "WaterTarget"
                    ),

                ProductionTarget =
                    GetDecimal(
                        row,
                        "ProductionTarget"
                    ),

                GuidePassTarget =
                    GetDecimal(
                        row,
                        "GuidePassTarget"
                    ),

                RollShopTarget =
                    GetDecimal(
                        row,
                        "RollShopTarget"
                    ),

                ElectricalTarget =
                    GetDecimal(
                        row,
                        "ElectricalTarget"
                    ),

                MechanicalTarget =
                    GetDecimal(
                        row,
                        "MechanicalTarget"
                    ),

                CraneTarget =
                    GetDecimal(
                        row,
                        "CraneTarget"
                    ),

                DispatchTarget =
                    GetDecimal(
                        row,
                        "DispatchTarget"
                    ),

                QualityTarget =
                    GetDecimal(
                        row,
                        "QualityTarget"
                    ),

                UtilityTarget =
                    GetDecimal(
                        row,
                        "UtilityTarget"
                    ),

                OthersTarget =
                    GetDecimal(
                        row,
                        "OthersTarget"
                    ),

                SizeChangeTarget =
                    GetDecimal(
                        row,
                        "SizeChangeTarget"
                    ),

                DownDayTarget =
                    GetDecimal(
                        row,
                        "DownDayTarget"
                    ),

                PowerFailureTarget =
                    GetDecimal(
                        row,
                        "PowerFailureTarget"
                    ),

                NoBilletTarget =
                    GetDecimal(
                        row,
                        "NoBilletTarget"
                    ),

                AnnualShutdownTarget =
                    GetDecimal(
                        row,
                        "AnnualShutdownTarget"
                    ),

                StatusID =
                    GetInt(
                        row,
                        "StatusID"
                    ),

                CreatedBy =
                    GetString(
                        row,
                        "CreatedBy"
                    ),

                CreatedDate =
                    GetNullableDateTime(
                        row,
                        "CreatedDate"
                    )
            };
        }

        private SqlParameter CreateDecimalParameter(
            string parameterName,
            decimal value)
        {
            var parameter = new SqlParameter(
                parameterName,
                SqlDbType.Decimal
            );

            parameter.Precision = 18;
            parameter.Scale = 2;
            parameter.Value = value;

            return parameter;
        }

        private SqlParameter CreateNVarCharParameter(
            string parameterName,
            string value,
            int size)
        {
            return new SqlParameter(
                parameterName,
                SqlDbType.NVarChar,
                size
            )
            {
                Value = string.IsNullOrWhiteSpace(value)
                    ? (object)DBNull.Value
                    : value.Trim()
            };
        }

        private string GetString(
            DataRow row,
            string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) ||
                row[columnName] == DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToString(row[columnName]);
        }

        private decimal GetDecimal(
            DataRow row,
            string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) ||
                row[columnName] == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToDecimal(row[columnName]);
        }

        private int GetInt(
            DataRow row,
            string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) ||
                row[columnName] == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(row[columnName]);
        }

        private DateTime? GetNullableDateTime(
            DataRow row,
            string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) ||
                row[columnName] == DBNull.Value)
            {
                return null;
            }

            return Convert.ToDateTime(row[columnName]);
        }
        public RollingMillTargetsBLL GetByMonthYear(
    string month,
    string year)
        {
            SqlParameter[] parameters =
            {
        new SqlParameter("@Month", SqlDbType.NVarChar, 20)
        {
            Value = string.IsNullOrWhiteSpace(month)
                ? (object)DBNull.Value
                : month.Trim()
        },

        new SqlParameter("@Year", SqlDbType.NVarChar, 4)
        {
            Value = string.IsNullOrWhiteSpace(year)
                ? (object)DBNull.Value
                : year.Trim()
        }
    };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_GetRollingMillTargetByMonthYear",
                CommandType.StoredProcedure,
                parameters
            );

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return Map(dt.Rows[0]);
        }
    }
}