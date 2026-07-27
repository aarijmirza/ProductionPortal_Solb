using DAL.Models;
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
    public class RollingMillDailyTargetRepository
    {

        public int Save(RollingMillDailyTargetBLL model)
        {
            SqlParameter[] parameters =
            {
        new SqlParameter("@ID", model.ID),

        new SqlParameter(
            "@TargetDate",
            model.TargetDate.Date
        ),

        new SqlParameter(
            "@DailyProductionTarget",
            model.DailyProductionTarget
        ),

        new SqlParameter(
            "@FuelConsumption",
            model.FuelConsumption
        ),

        new SqlParameter(
            "@CreatedBy",
            string.IsNullOrWhiteSpace(model.CreatedBy)
                ? (object)DBNull.Value
                : model.CreatedBy
        )
    };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_SaveRollingMillDailyTarget",
                CommandType.StoredProcedure,
                parameters
            );

            if (dt != null && dt.Rows.Count > 0)
            {
                return Convert.ToInt32(
                    dt.Rows[0]["ID"]
                );
            }

            return 0;
        }

        public List<RollingMillDailyTargetBLL> GetAll(
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@FromDate",
                    fromDate.HasValue
                        ? (object)fromDate.Value.Date
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@ToDate",
                    toDate.HasValue
                        ? (object)toDate.Value.Date
                        : DBNull.Value
                )
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_GetRollingMillDailyTargets",
                CommandType.StoredProcedure, parameters
            );

            var list =
                new List<RollingMillDailyTargetBLL>();

            if (dt == null || dt.Rows.Count == 0)
                return list;

            foreach (DataRow row in dt.Rows)
            {
                list.Add(Map(row));
            }

            return list;
        }

        public RollingMillDailyTargetBLL GetByID(int id)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", id)
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_GetRollingMillDailyTargetByID",
                  CommandType.StoredProcedure,
                    parameters
            );

            if (dt == null || dt.Rows.Count == 0)
                return null;

            return Map(dt.Rows[0]);
        }

        public RollingMillDailyTargetBLL GetByDate(
            DateTime targetDate)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@TargetDate",
                    targetDate.Date
                )
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_GetRollingMillDailyTargetByDate",
                  CommandType.StoredProcedure,
                    parameters
            );

            if (dt == null || dt.Rows.Count == 0)
                return null;

            return Map(dt.Rows[0]);
        }

        public bool Delete(
            int id,
            string updatedBy)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", id),

                new SqlParameter(
                    "@UpdatedBy",
                    string.IsNullOrWhiteSpace(updatedBy)
                        ? (object)DBNull.Value
                        : updatedBy
                )
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_DeleteRollingMillDailyTarget",
                  CommandType.StoredProcedure,
                    parameters
            );

            if (dt != null && dt.Rows.Count > 0)
            {
                int affectedRows =
                    Convert.ToInt32(
                        dt.Rows[0]["AffectedRows"]
                    );

                return affectedRows > 0;
            }

            return false;
        }

        private RollingMillDailyTargetBLL Map(DataRow row)
        {
            return new RollingMillDailyTargetBLL
            {
                ID =
                    row.Table.Columns.Contains("ID") &&
                    row["ID"] != DBNull.Value
                        ? Convert.ToInt32(row["ID"])
                        : 0,

                TargetDate =
                    row.Table.Columns.Contains("TargetDate") &&
                    row["TargetDate"] != DBNull.Value
                        ? Convert.ToDateTime(
                            row["TargetDate"]
                        )
                        : DateTime.MinValue,

                DailyProductionTarget =
                    row.Table.Columns.Contains(
                        "DailyProductionTarget"
                    ) &&
                    row["DailyProductionTarget"] != DBNull.Value
                        ? Convert.ToDecimal(
                            row["DailyProductionTarget"]
                        )
                        : 0,

                FuelConsumption =
                    row.Table.Columns.Contains(
                        "FuelConsumption"
                    ) &&
                    row["FuelConsumption"] != DBNull.Value
                        ? Convert.ToDecimal(
                            row["FuelConsumption"]
                        )
                        : 0,

                StatusID =
                    row.Table.Columns.Contains("StatusID") &&
                    row["StatusID"] != DBNull.Value
                        ? Convert.ToInt32(row["StatusID"])
                        : 0,

                CreatedBy =
                    row.Table.Columns.Contains("CreatedBy") &&
                    row["CreatedBy"] != DBNull.Value
                        ? Convert.ToString(row["CreatedBy"])
                        : string.Empty,

                CreatedDate =
                    row.Table.Columns.Contains("CreatedDate") &&
                    row["CreatedDate"] != DBNull.Value
                        ? Convert.ToDateTime(
                            row["CreatedDate"]
                        )
                        : (DateTime?)null,

                UpdatedBy =
                    row.Table.Columns.Contains("UpdatedBy") &&
                    row["UpdatedBy"] != DBNull.Value
                        ? Convert.ToString(row["UpdatedBy"])
                        : string.Empty,

                UpdatedDate =
                    row.Table.Columns.Contains("UpdatedDate") &&
                    row["UpdatedDate"] != DBNull.Value
                        ? Convert.ToDateTime(
                            row["UpdatedDate"]
                        )
                        : (DateTime?)null
            };
        }
    }
}
