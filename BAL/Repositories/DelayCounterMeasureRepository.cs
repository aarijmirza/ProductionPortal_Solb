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
    public class DelayCounterMeasureRepository
    {
        public int Insert(
                 DelayCounterMeasureBLL model)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@PlantDelayID",
                    model.PlantDelayID
                ),

                new SqlParameter(
                    "@CounterMeasure",
                    model.CounterMeasure
                ),

                new SqlParameter(
                    "@SAPOrderNo",
                    string.IsNullOrWhiteSpace(
                        model.SAPOrderNo
                    )
                        ? (object)DBNull.Value
                        : model.SAPOrderNo
                ),

                new SqlParameter(
                    "@Responsible",
                    string.IsNullOrWhiteSpace(
                        model.Responsible
                    )
                        ? (object)DBNull.Value
                        : model.Responsible
                ),

                new SqlParameter(
                    "@TargetDate",
                    model.TargetDate.HasValue
                        ? (object)model.TargetDate.Value.Date
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@EvidenceForCompletion",
                    string.IsNullOrWhiteSpace(
                        model.EvidenceForCompletion
                    )
                        ? (object)DBNull.Value
                        : model.EvidenceForCompletion
                ),

                new SqlParameter(
                    "@CounterMeasureStatus",
                    string.IsNullOrWhiteSpace(
                        model.CounterMeasureStatus
                    )
                        ? "Open"
                        : model.CounterMeasureStatus
                ),

                new SqlParameter(
                    "@ReasonForNotClosing",
                    string.IsNullOrWhiteSpace(
                        model.ReasonForNotClosing
                    )
                        ? (object)DBNull.Value
                        : model.ReasonForNotClosing
                ),

                new SqlParameter(
                    "@CreatedBy",
                    string.IsNullOrWhiteSpace(
                        model.CreatedBy
                    )
                        ? (object)DBNull.Value
                        : model.CreatedBy
                )
            };

            DataTable dt =
                new DBHelper().GetTableFromSP(
                    "sp_InsertDelayCounterMeasure",
                    parameters
                );

            if (dt == null ||
                dt.Rows.Count == 0)
            {
                return 0;
            }

            return Convert.ToInt32(
                dt.Rows[0]["NewID"]
            );
        }

        public List<DelayCounterMeasureBLL>
            GetByPlantDelayID(
                int plantDelayID)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@PlantDelayID",
                    plantDelayID
                )
            };

            DataTable dt =
                new DBHelper().GetTableFromSP(
                    "sp_GetDelayCounterMeasures",
                    parameters
                );

            List<DelayCounterMeasureBLL> list =
                new List<DelayCounterMeasureBLL>();

            if (dt == null)
            {
                return list;
            }

            foreach (DataRow row in dt.Rows)
            {
                list.Add(Map(row));
            }

            return list;
        }

        public DelayCounterMeasureBLL GetByID(
            int id)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", id)
            };

            DataTable dt =
                new DBHelper().GetTableFromSP(
                    "sp_GetDelayCounterMeasureByID",
                    parameters
                );

            if (dt == null ||
                dt.Rows.Count == 0)
            {
                return null;
            }

            return Map(dt.Rows[0]);
        }

        public bool Update(
            DelayCounterMeasureBLL model)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@ID",
                    model.ID
                ),

                new SqlParameter(
                    "@CounterMeasure",
                    model.CounterMeasure
                ),

                new SqlParameter(
                    "@SAPOrderNo",
                    DbValue(model.SAPOrderNo)
                ),

                new SqlParameter(
                    "@Responsible",
                    DbValue(model.Responsible)
                ),

                new SqlParameter(
                    "@TargetDate",
                    model.TargetDate.HasValue
                        ? (object)model.TargetDate.Value.Date
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@EvidenceForCompletion",
                    DbValue(
                        model.EvidenceForCompletion
                    )
                ),

                new SqlParameter(
                    "@CounterMeasureStatus",
                    string.IsNullOrWhiteSpace(
                        model.CounterMeasureStatus
                    )
                        ? "Open"
                        : model.CounterMeasureStatus
                ),

                new SqlParameter(
                    "@ReasonForNotClosing",
                    DbValue(
                        model.ReasonForNotClosing
                    )
                ),

                new SqlParameter(
                    "@UpdatedBy",
                    DbValue(model.UpdatedBy)
                )
            };

            DataTable dt =
                new DBHelper().GetTableFromSP(
                    "sp_UpdateDelayCounterMeasure",
                    parameters
                );

            return
                dt != null &&
                dt.Rows.Count > 0 &&
                Convert.ToInt32(
                    dt.Rows[0]["AffectedRows"]
                ) > 0;
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
                    DbValue(updatedBy)
                )
            };

            DataTable dt =
                new DBHelper().GetTableFromSP(
                    "sp_DeleteDelayCounterMeasure",
                    parameters
                );

            return
                dt != null &&
                dt.Rows.Count > 0 &&
                Convert.ToInt32(
                    dt.Rows[0]["AffectedRows"]
                ) > 0;
        }

        private DelayCounterMeasureBLL Map(
            DataRow row)
        {
            return new DelayCounterMeasureBLL
            {
                ID =
                    GetInt(row, "ID"),

                PlantDelayID =
                    GetInt(
                        row,
                        "PlantDelayID"
                    ),

                CounterMeasure =
                    GetString(
                        row,
                        "CounterMeasure"
                    ),

                SAPOrderNo =
                    GetString(
                        row,
                        "SAPOrderNo"
                    ),

                Responsible =
                    GetString(
                        row,
                        "Responsible"
                    ),

                TargetDate =
                    GetNullableDate(
                        row,
                        "TargetDate"
                    ),

                EvidenceForCompletion =
                    GetString(
                        row,
                        "EvidenceForCompletion"
                    ),

                CounterMeasureStatus =
                    GetString(
                        row,
                        "CounterMeasureStatus"
                    ),

                ReasonForNotClosing =
                    GetString(
                        row,
                        "ReasonForNotClosing"
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
                    GetNullableDate(
                        row,
                        "CreatedDate"
                    ),

                UpdatedBy =
                    GetString(
                        row,
                        "UpdatedBy"
                    ),

                UpdatedDate =
                    GetNullableDate(
                        row,
                        "UpdatedDate"
                    ),

                DelayCode =
                    GetString(
                        row,
                        "DelayCode"
                    ),

                DelayDate =
                    GetNullableDate(
                        row,
                        "DelayDate"
                    ),

                Plant =
                    GetString(
                        row,
                        "Plant"
                    ),

                Area =
                    GetString(
                        row,
                        "Area"
                    ),

                Equipment =
                    GetString(
                        row,
                        "Equipment"
                    ),

                DelayDescription =
                    GetString(
                        row,
                        "DelayDescription"
                    )
            };
        }

        private object DbValue(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? (object)DBNull.Value
                : value.Trim();
        }

        private string GetString(
            DataRow row,
            string columnName)
        {
            if (!row.Table.Columns.Contains(
                    columnName
                ) ||
                row[columnName] == DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToString(
                row[columnName]
            ).Trim();
        }

        private int GetInt(
            DataRow row,
            string columnName)
        {
            if (!row.Table.Columns.Contains(
                    columnName
                ) ||
                row[columnName] == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(
                row[columnName]
            );
        }

        private DateTime? GetNullableDate(
            DataRow row,
            string columnName)
        {
            if (!row.Table.Columns.Contains(
                    columnName
                ) ||
                row[columnName] == DBNull.Value)
            {
                return null;
            }

            return Convert.ToDateTime(
                row[columnName]
            );
        }
    }
}