using DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebAPICode.Helpers;

namespace BAL.Repositories
{
    public class SMPProductionDelayRepository
    {
        public List<SMPProductionDelayBLL> GetAll(
            DateTime? fromDate,
            DateTime? toDate)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@FromDate", SqlDbType.Date)
                {
                    Value = fromDate.HasValue
                        ? (object)fromDate.Value.Date
                        : DBNull.Value
                },
                new SqlParameter("@ToDate", SqlDbType.Date)
                {
                    Value = toDate.HasValue
                        ? (object)toDate.Value.Date
                        : DBNull.Value
                }
            };

            DataTable dt = new DBHelper().GetTableFromSP(
                "sp_GetSMPDelayDaywises",
                parameters
            );

            if (dt == null || dt.Rows.Count == 0)
            {
                return new List<SMPProductionDelayBLL>();
            }

            return JArray
                .Parse(JsonConvert.SerializeObject(dt))
                .ToObject<List<SMPProductionDelayBLL>>();
        }

        public SMPProductionDelayImportResultBLL ImportExcelRows(
            List<SMPProductionDelayUploadBLL> rows,
            string createdBy)
        {
            if (rows == null || rows.Count == 0)
            {
                throw new ArgumentException(
                    "At least one SMP delay row is required.",
                    "rows"
                );
            }

            DataTable uploadTable = BuildUploadTable(rows);

            SqlParameter rowsParameter =
                new SqlParameter("@Rows", SqlDbType.Structured)
                {
                    TypeName = "dbo.SMPDelayDaywiseUploadType",
                    Value = uploadTable
                };

            SqlParameter createdByParameter =
                new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100)
                {
                    Value = string.IsNullOrWhiteSpace(createdBy)
                        ? (object)DBNull.Value
                        : createdBy.Trim()
                };

            DataTable resultTable =
                new DBHelper().GetTableFromSP(
                    "sp_ImportSMPDelayDaywises",
                    new[]
                    {
                        rowsParameter,
                        createdByParameter
                    }
                );

            if (resultTable == null || resultTable.Rows.Count == 0)
            {
                throw new InvalidOperationException(
                    "The database did not return an Excel import result."
                );
            }

            DataRow result = resultTable.Rows[0];

            return new SMPProductionDelayImportResultBLL
            {
                ProcessedRows = ReadInt(result, "ProcessedRows"),
                InsertedPlantDelays = ReadInt(result, "InsertedPlantDelays"),
                UpdatedPlantDelays = ReadInt(result, "UpdatedPlantDelays"),
                DeactivatedPlantDelays = ReadInt(result, "DeactivatedPlantDelays"),

                InsertedSMPProductionDelays = ReadInt(
                    result,
                    "InsertedSMPProductionDelays"
                ),
                UpdatedSMPProductionDelays = ReadInt(
                    result,
                    "UpdatedSMPProductionDelays"
                ),
                DeactivatedSMPProductionDelays = ReadInt(
                    result,
                    "DeactivatedSMPProductionDelays"
                ),

                InsertedFailureAnalyses = ReadInt(
                    result,
                    "InsertedFailureAnalyses"
                ),
                UpdatedFailureAnalyses = ReadInt(
                    result,
                    "UpdatedFailureAnalyses"
                ),

                GeneratedDelayCodes = ReadInt(
                    result,
                    "GeneratedDelayCodes"
                ),
                FirstGeneratedDelayCode = ReadString(
                    result,
                    "FirstGeneratedDelayCode"
                ),
                LastGeneratedDelayCode = ReadString(
                    result,
                    "LastGeneratedDelayCode"
                )
            };
        }

        private static DataTable BuildUploadTable(
            IEnumerable<SMPProductionDelayUploadBLL> rows)
        {
            var table = new DataTable();

            // Must match dbo.SMPDelayDaywiseUploadType in this exact order.
            table.Columns.Add("RowNo", typeof(int));
            table.Columns.Add("Plant", typeof(string));
            table.Columns.Add("ShiftGroup", typeof(string));
            table.Columns.Add("ProductionDate", typeof(DateTime));
            table.Columns.Add("DelayStart", typeof(TimeSpan));
            table.Columns.Add("DelayFinish", typeof(TimeSpan));
            table.Columns.Add("TotalMinutes", typeof(int));
            table.Columns.Add("Agency", typeof(string));
            table.Columns.Add("Area", typeof(string));
            table.Columns.Add("Equipment", typeof(string));
            table.Columns.Add("DelayDescription", typeof(string));
            table.Columns.Add("ReasonForOccurrence", typeof(string));
            table.Columns.Add("ActionTaken", typeof(string));
            table.Columns.Add("LastPMDate", typeof(DateTime));
            table.Columns.Add("FailureReportStatus", typeof(string));
            table.Columns.Add("IncreaseMTBF", typeof(string));
            table.Columns.Add("DecreaseMTTR", typeof(string));
            table.Columns.Add("SAPBreakdownOrder", typeof(string));
            table.Columns.Add(
                "FailureCategory1Component",
                typeof(string)
            );
            table.Columns.Add(
                "FailureCategory2RootCause",
                typeof(string)
            );

            foreach (SMPProductionDelayUploadBLL row in rows)
            {
                DataRow dataRow = table.NewRow();

                dataRow["RowNo"] = row.ExcelRowNo;
                dataRow["Plant"] = DbValue(row.Plant);
                dataRow["ShiftGroup"] = DbValue(row.ShiftGroup);
                dataRow["ProductionDate"] = row.ProductionDate.Date;
                dataRow["DelayStart"] = row.DelayStart.HasValue
                    ? (object)row.DelayStart.Value
                    : DBNull.Value;
                dataRow["DelayFinish"] = row.DelayFinish.HasValue
                    ? (object)row.DelayFinish.Value
                    : DBNull.Value;
                dataRow["TotalMinutes"] = row.TotalMinutes;
                dataRow["Agency"] = DbValue(row.Agency);
                dataRow["Area"] = DbValue(row.Area);
                dataRow["Equipment"] = DbValue(row.Equipment);
                dataRow["DelayDescription"] = DbValue(
                    row.DelayDescription
                );
                dataRow["ReasonForOccurrence"] = DbValue(
                    row.ReasonForOccurrence
                );
                dataRow["ActionTaken"] = DbValue(row.ActionTaken);
                dataRow["LastPMDate"] = row.LastPMDate.HasValue
                    ? (object)row.LastPMDate.Value.Date
                    : DBNull.Value;
                dataRow["FailureReportStatus"] = DbValue(
                    row.FailureReportStatus
                );
                dataRow["IncreaseMTBF"] = DbValue(row.IncreaseMTBF);
                dataRow["DecreaseMTTR"] = DbValue(row.DecreaseMTTR);
                dataRow["SAPBreakdownOrder"] = DbValue(
                    row.SAPBreakdownOrder
                );
                dataRow["FailureCategory1Component"] = DbValue(
                    row.FailureCategory1Component
                );
                dataRow["FailureCategory2RootCause"] = DbValue(
                    row.FailureCategory2RootCause
                );

                table.Rows.Add(dataRow);
            }

            return table;
        }

        private static object DbValue(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? (object)DBNull.Value
                : value.Trim();
        }

        private static int ReadInt(DataRow row, string columnName)
        {
            return !row.Table.Columns.Contains(columnName) ||
                   row[columnName] == DBNull.Value
                ? 0
                : Convert.ToInt32(row[columnName]);
        }

        private static string ReadString(
            DataRow row,
            string columnName)
        {
            return !row.Table.Columns.Contains(columnName) ||
                   row[columnName] == DBNull.Value
                ? null
                : Convert.ToString(row[columnName]);
        }
    }
}
