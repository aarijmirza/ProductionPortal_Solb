using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using WebAPICode.Helpers;
using static DAL.Models.ViewModel;

namespace BAL.Repositories
{
    public class DelayCounterMeasureRepository
    {
        public int SaveMultiple(int plantDelayID, List<DelayCounterMeasureBLL> counterMeasures, string createdBy)
        {
            if (plantDelayID <= 0)
                throw new ArgumentException("Invalid Plant Delay ID.");

            counterMeasures = (counterMeasures ?? new List<DelayCounterMeasureBLL>())
                .Where(x => x != null &&
                    (!string.IsNullOrWhiteSpace(x.CounterMeasure) ||
                     !string.IsNullOrWhiteSpace(x.CounterMeasureA) ||
                     !string.IsNullOrWhiteSpace(x.IncreaseMTBF) ||
                     !string.IsNullOrWhiteSpace(x.DecreaseMTTR)))
                .ToList();

            if (counterMeasures.Count == 0)
                throw new ArgumentException("At least one countermeasure is required.");

            int savedRecords = 0;

            foreach (var item in counterMeasures)
            {
                item.PlantDelayID = plantDelayID;

                if (string.IsNullOrWhiteSpace(item.CounterMeasureStatus))
                    item.CounterMeasureStatus = "Open";

                if (Save(item, createdBy) > 0)
                    savedRecords++;
            }

            return savedRecords;
        }

        public int Save(DelayCounterMeasureBLL model, string createdBy)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (model.PlantDelayID <= 0)
                throw new ArgumentException("Invalid Plant Delay ID.");

            SqlParameter[] parameters =
            {
                Param("@ID", SqlDbType.Int, model.ID),
                Param("@PlantDelayID", SqlDbType.Int, model.PlantDelayID),
                Param("@AnalysisID", SqlDbType.Int,
                    model.AnalysisID.HasValue ? (object)model.AnalysisID.Value : DBNull.Value),
                Param("@CounterMeasureCode", SqlDbType.NVarChar, DbValue(model.CounterMeasureCode), 50),
                Param("@CounterMeasure", SqlDbType.NVarChar, DbValue(model.CounterMeasure), -1),
                Param("@CounterMeasureA", SqlDbType.NVarChar, DbValue(model.CounterMeasureA), -1),
                Param("@IncreaseMTBF", SqlDbType.NVarChar, DbValue(model.IncreaseMTBF), -1),
                Param("@DecreaseMTTR", SqlDbType.NVarChar, DbValue(model.DecreaseMTTR), -1),
                Param("@IncreaseMTBF1", SqlDbType.NVarChar, DbValue(model.IncreaseMTBF1), -1),
                Param("@DecreaseMTTR1", SqlDbType.NVarChar, DbValue(model.DecreaseMTTR1), -1),
                Param("@RootCause", SqlDbType.NVarChar, DbValue(model.RootCause), -1),
                Param("@SAPBreakdownOrder", SqlDbType.NVarChar, DbValue(model.SAPBreakdownOrder), 100),
                Param("@FailureReportStatus", SqlDbType.NVarChar, DbValue(model.FailureReportStatus), 50),
                Param("@SAPOrderNo", SqlDbType.NVarChar, DbValue(model.SAPOrderNo), 100),
                Param("@SubOrderNumber", SqlDbType.NVarChar, DbValue(model.SubOrderNumber), 50),
                Param("@Responsible", SqlDbType.NVarChar, DbValue(model.Responsible), 150),
                Param("@TargetDate", SqlDbType.Date,
                    model.TargetDate.HasValue ? (object)model.TargetDate.Value.Date : DBNull.Value),
                Param("@CounterMeasureStatus", SqlDbType.NVarChar,
                    DbValue(string.IsNullOrWhiteSpace(model.CounterMeasureStatus) ? "Open" : model.CounterMeasureStatus), 50),
                Param("@EvidenceForCompletion", SqlDbType.NVarChar, DbValue(model.EvidenceForCompletion), -1),
                Param("@EvidenceFile", SqlDbType.NVarChar, DbValue(model.EvidenceFile), 500),
                Param("@EvidenceFileName", SqlDbType.NVarChar, DbValue(model.EvidenceFileName), 255),
                Param("@ReasonForNotClosing", SqlDbType.NVarChar, DbValue(model.ReasonForNotClosing), -1),
                Param("@CreatedBy", SqlDbType.NVarChar, DbValue(createdBy), 100)
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_SaveDelayCounterMeasure",
                CommandType.StoredProcedure,
                parameters
            );

            if (dt == null || dt.Rows.Count == 0 || !dt.Columns.Contains("ID") || dt.Rows[0]["ID"] == DBNull.Value)
                return 0;

            return Convert.ToInt32(dt.Rows[0]["ID"]);
        }

        public DelayCounterMeasureVM GetPageData(int plantDelayID)
        {
            if (plantDelayID <= 0)
                return null;

            var delayDetail = GetFailureAnalysisRecord(plantDelayID);

            if (delayDetail == null)
                return null;

            return new DelayCounterMeasureVM
            {
                PlantDelayID = plantDelayID,
                DelayDetail = delayDetail,
                ExistingCounterMeasures = GetByPlantDelayID(plantDelayID),
                CounterMeasures = new List<DelayCounterMeasureBLL>
                {
                    new DelayCounterMeasureBLL
                    {
                        PlantDelayID = plantDelayID,
                        CounterMeasureStatus = "Open"
                    }
                }
            };
        }

        public PlantDelayBLL GetFailureAnalysisRecord(int plantDelayID)
        {
            SqlParameter[] parameters =
            {
                Param("@PlantDelayID", SqlDbType.Int, plantDelayID)
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_GetFailureAnalysisByID",
                CommandType.StoredProcedure,
                parameters
            );

            if (dt == null || dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new PlantDelayBLL
            {
                ID = GetInt(row, "ID"),
                Date = GetNullableDate(row, "Date"),
                Plant = GetString(row, "Plant"),
                Area = GetString(row, "Area"),
                Shift = GetString(row, "Shift"),
                StartTime = GetNullableTimeSpan(row, "StartTime"),
                EndTime = GetNullableTimeSpan(row, "EndTime"),
                TotalDuration = GetInt(row, "TotalDuration"),
                DelayType = GetString(row, "DelayType"),
                AgencyName = GetString(row, "AgencyName"),
                Delaycode = GetString(row, "Delaycode"),
                Equipments = GetString(row, "Equipments"),
                DelayDescription = GetString(row, "DelayDescription"),
                DelayReason = GetString(row, "DelayReason"),
                ReasonForOccurence = GetString(row, "ReasonForOccurence"),
                ActionTaken = GetString(row, "ActionTaken")
            };
        }

        public FailureAnalysisBLL GetFailureAnalysisByDelayID(int plantDelayID)
        {
            SqlParameter[] parameters =
            {
                Param("@DelayID", SqlDbType.Int, plantDelayID)
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_GetFailureAnalysisByDelayID",
                CommandType.StoredProcedure,
                parameters
            );

            if (dt == null || dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new FailureAnalysisBLL
            {
                ID = GetInt(row, "ID"),
                DelayID = GetInt(row, "DelayID"),
                AnalysisCode = GetString(row, "AnalysisCode"),
                LastPMDate = GetNullableDate(row, "LastPMDate"),
                FailureReportStatus = GetString(row, "FailureReportStatus"),
                IncreaseMTBF = GetString(row, "IncreaseMTBF"),
                IncreaseMTBF1 = GetString(row, "IncreaseMTBF1"),
                DecreaseMTTR = GetString(row, "DecreaseMTTR"),
                DecreaseMTTR1 = GetString(row, "DecreaseMTTR1"),
                SAPBreakdownOrder = GetString(row, "SAPBreakdownOrder"),
                FailureCategory1Component = GetString(row, "FailureCategory1Component"),
                FailureCategory2RootCause = GetString(row, "FailureCategory2RootCause")
            };
        }

        public List<DelayCounterMeasureBLL> GetByPlantDelayID(int plantDelayID)
        {
            SqlParameter[] parameters =
            {
                Param("@PlantDelayID", SqlDbType.Int, plantDelayID)
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_GetCounterMeasuresByPlantDelayID",
                CommandType.StoredProcedure,
                parameters
            );

            var list = new List<DelayCounterMeasureBLL>();

            if (dt == null || dt.Rows.Count == 0)
                return list;

            foreach (DataRow row in dt.Rows)
                list.Add(MapCounterMeasure(row));

            return list;
        }

        public DelayCounterMeasureBLL GetByID(int id)
        {
            SqlParameter[] parameters =
            {
                Param("@ID", SqlDbType.Int, id)
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_GetDelayCounterMeasureByID",
                CommandType.StoredProcedure,
                parameters
            );

            if (dt == null || dt.Rows.Count == 0)
                return null;

            return MapCounterMeasure(dt.Rows[0]);
        }

        public bool Delete(int id, string updatedBy)
        {
            SqlParameter[] parameters =
            {
                Param("@ID", SqlDbType.Int, id),
                Param("@UpdatedBy", SqlDbType.NVarChar, DbValue(updatedBy), 100)
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_DeleteDelayCounterMeasure",
                CommandType.StoredProcedure,
                parameters
            );

            return dt != null &&
                   dt.Rows.Count > 0 &&
                   dt.Columns.Contains("AffectedRows") &&
                   Convert.ToInt32(dt.Rows[0]["AffectedRows"]) > 0;
        }

        private DelayCounterMeasureBLL MapCounterMeasure(DataRow row)
        {
            return new DelayCounterMeasureBLL
            {
                ID = GetInt(row, "ID"),
                PlantDelayID = GetInt(row, "PlantDelayID"),
                AnalysisID = GetNullableInt(row, "AnalysisID"),
                CounterMeasureCode = GetString(row, "CounterMeasureCode"),
                CounterMeasure = GetString(row, "CounterMeasure"),
                CounterMeasureA = GetString(row, "CounterMeasureA"),
                IncreaseMTBF = GetString(row, "IncreaseMTBF"),
                DecreaseMTTR = GetString(row, "DecreaseMTTR"),
                IncreaseMTBF1 = GetString(row, "IncreaseMTBF1"),
                DecreaseMTTR1 = GetString(row, "DecreaseMTTR1"),
                RootCause = GetString(row, "RootCause"),
                SAPBreakdownOrder = GetString(row, "SAPBreakdownOrder"),
                FailureReportStatus = GetString(row, "FailureReportStatus"),
                SAPOrderNo = GetString(row, "SAPOrderNo"),
                SubOrderNumber = GetString(row, "SubOrderNumber"),
                Responsible = GetString(row, "Responsible"),
                TargetDate = GetNullableDate(row, "TargetDate"),
                CounterMeasureStatus = GetString(row, "CounterMeasureStatus"),
                EvidenceForCompletion = GetString(row, "EvidenceForCompletion"),
                EvidenceFile = GetString(row, "EvidenceFile"),
                EvidenceFileName = GetString(row, "EvidenceFileName"),
                ReasonForNotClosing = GetString(row, "ReasonForNotClosing"),
                StatusID = GetInt(row, "StatusID"),
                CreatedBy = GetString(row, "CreatedBy"),
                CreatedDate = GetNullableDate(row, "CreatedDate"),
                UpdatedBy = GetString(row, "UpdatedBy"),
                UpdatedDate = GetNullableDate(row, "UpdatedDate")
            };
        }

        private SqlParameter Param(string name, SqlDbType type, object value, int size = 0)
        {
            var parameter = size == 0
                ? new SqlParameter(name, type)
                : new SqlParameter(name, type, size);

            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }

        private object DbValue(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? (object)DBNull.Value
                : value.Trim();
        }

        private int GetInt(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value
                ? Convert.ToInt32(row[columnName])
                : 0;
        }

        private int? GetNullableInt(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName) &&
                   row[columnName] != DBNull.Value
                ? (int?)Convert.ToInt32(row[columnName])
                : null;
        }

        private string GetString(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value
                ? Convert.ToString(row[columnName]).Trim()
                : string.Empty;
        }

        private DateTime? GetNullableDate(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value
                ? Convert.ToDateTime(row[columnName])
                : (DateTime?)null;
        }

        private TimeSpan? GetNullableTimeSpan(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
                return null;

            if (row[columnName] is TimeSpan)
                return (TimeSpan)row[columnName];

            TimeSpan parsedValue;

            return TimeSpan.TryParse(
                Convert.ToString(row[columnName], CultureInfo.InvariantCulture),
                out parsedValue)
                ? parsedValue
                : (TimeSpan?)null;
        }
    }
}
