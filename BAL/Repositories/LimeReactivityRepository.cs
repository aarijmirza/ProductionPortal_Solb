using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebAPICode.Helpers;

namespace BAL.Repositories
{
    public class LimeReactivityRepository
    {
        public List<QCLimeReactivityReportBLL> GetAll()
        {
            var list = new List<QCLimeReactivityReportBLL>();
            DataTable dt = new DBHelper().GetTableFromSP(
                "sp_QC_GetLimeReactivityList",
                new SqlParameter[0]
            );

            if (dt == null) return list;

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new QCLimeReactivityReportBLL
                {
                    ID = GetInt(row, "ID"),
                    AnalysisDateQC = GetDate(row, "AnalysisDateQC"),
                    ReferenceNo = GetString(row, "ReferenceNo"),
                    Remarks = GetString(row, "Remarks"),
                    FinalStatus = GetString(row, "FinalStatus"),
                    MRBNo = GetString(row, "MRBNo"),
                    StatusID = GetInt(row, "StatusID"),
                    CreatedDate = GetDate(row, "CreatedDate"),
                    CreatedBy = GetString(row, "CreatedBy"),
                    UpdatedDate = GetDate(row, "UpdatedDate"),
                    UpdatedBy = GetString(row, "UpdatedBy")
                });
            }

            return list;
        }

        public QCLimeReactivityReportBLL GetByID(int id)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ID", SqlDbType.Int) { Value = id }
            };

            DataSet ds = new DBHelper().GetDatasetFromSP(
                "sp_QC_GetLimeReactivityByID",
                p
            );

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return null;

            DataRow m = ds.Tables[0].Rows[0];
            var model = new QCLimeReactivityReportBLL
            {
                ID = GetInt(m, "ID"),
                AnalysisDateQC = GetDate(m, "AnalysisDateQC"),
                ReferenceNo = GetString(m, "ReferenceNo"),
                Remarks = GetString(m, "Remarks"),
                FinalStatus = GetString(m, "FinalStatus"),
                MRBNo = GetString(m, "MRBNo"),
                StatusID = GetInt(m, "StatusID"),
                CreatedDate = GetDate(m, "CreatedDate"),
                CreatedBy = GetString(m, "CreatedBy"),
                UpdatedDate = GetDate(m, "UpdatedDate"),
                UpdatedBy = GetString(m, "UpdatedBy"),
                Samples = new List<QCLimeReactivitySampleBLL>()
            };

            if (ds.Tables.Count > 1 && ds.Tables[1] != null)
            {
                foreach (DataRow r in ds.Tables[1].Rows)
                {
                    model.Samples.Add(MapSample(r));
                }
            }

            return model;
        }

        public int InsertMaster(QCLimeReactivityReportBLL model)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@AnalysisDateQC", SqlDbType.Date)
                {
                    Value = model.AnalysisDateQC.HasValue ? (object)model.AnalysisDateQC.Value.Date : DBNull.Value
                },
                new SqlParameter("@ReferenceNo", SqlDbType.NVarChar, 100) { Value = DbValue(model.ReferenceNo) },
                new SqlParameter("@Remarks", SqlDbType.NVarChar, -1) { Value = DbValue(model.Remarks) },
                new SqlParameter("@FinalStatus", SqlDbType.NVarChar, 30) { Value = DbValue(model.FinalStatus) },
                new SqlParameter("@MRBNo", SqlDbType.NVarChar, 100) { Value = DbValue(model.MRBNo) },
                new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100) { Value = DbValue(model.CreatedBy) }
            };

            object result = new DBHelper().ExecuteScalar(
                "sp_QC_InsertLimeReactivityReport",
                p
            );

            return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }

        public int UpdateMaster(QCLimeReactivityReportBLL model)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ID", SqlDbType.Int) { Value = model.ID },
                new SqlParameter("@AnalysisDateQC", SqlDbType.Date)
                {
                    Value = model.AnalysisDateQC.HasValue ? (object)model.AnalysisDateQC.Value.Date : DBNull.Value
                },
                new SqlParameter("@ReferenceNo", SqlDbType.NVarChar, 100) { Value = DbValue(model.ReferenceNo) },
                new SqlParameter("@Remarks", SqlDbType.NVarChar, -1) { Value = DbValue(model.Remarks) },
                new SqlParameter("@FinalStatus", SqlDbType.NVarChar, 30) { Value = DbValue(model.FinalStatus) },
                new SqlParameter("@MRBNo", SqlDbType.NVarChar, 100) { Value = DbValue(model.MRBNo) },
                new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = DbValue(model.UpdatedBy) }
            };

            object result = new DBHelper().ExecuteScalar(
                "sp_QC_UpdateLimeReactivityReport",
                p
            );

            return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }

        public int InsertSample(QCLimeReactivitySampleBLL model)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ReportID", SqlDbType.Int) { Value = model.ReportID },
                new SqlParameter("@SrNo", SqlDbType.Int) { Value = model.SrNo },
                new SqlParameter("@ShipmentCode", SqlDbType.NVarChar, 100) { Value = DbValue(model.ShipmentCode) },
                new SqlParameter("@ReceivedDate", SqlDbType.Date) { Value = model.ReceivedDate.HasValue ? (object)model.ReceivedDate.Value.Date : DBNull.Value },
                new SqlParameter("@Supplier", SqlDbType.NVarChar, 200) { Value = DbValue(model.Supplier) },
                DecimalParameter("@QuantityTons", model.QuantityTons),
                new SqlParameter("@SampleNo", SqlDbType.NVarChar, 100) { Value = DbValue(model.SampleNo) },
                DecimalParameter("@ActiveCaO", model.ActiveCaO),
                DecimalParameter("@TotalCaO", model.TotalCaO),
                DecimalParameter("@MgO", model.MgO),
                DecimalParameter("@LOI", model.LOI),
                DecimalParameter("@Temp0Sec", model.Temp0Sec),
                DecimalParameter("@Temp30Sec", model.Temp30Sec),
                DecimalParameter("@Temp60Sec", model.Temp60Sec),
                DecimalParameter("@Temp90Sec", model.Temp90Sec),
                DecimalParameter("@Temp120Sec", model.Temp120Sec),
                DecimalParameter("@Temp150Sec", model.Temp150Sec),
                DecimalParameter("@Temp180Sec", model.Temp180Sec),
                DecimalParameter("@Temp240Sec", model.Temp240Sec),
                DecimalParameter("@Temp360Sec", model.Temp360Sec),
                DecimalParameter("@Temp600Sec", model.Temp600Sec),
                new SqlParameter("@TestStatus", SqlDbType.NVarChar, 50) { Value = DbValue(model.TestStatus) },
                new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100) { Value = DbValue(model.CreatedBy) }
            };

            object result = new DBHelper().ExecuteScalar(
                "sp_QC_InsertLimeReactivitySample",
                p
            );

            return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }

        public int DeleteSamplesByReportID(int reportID, string updatedBy)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ReportID", SqlDbType.Int) { Value = reportID },
                new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = DbValue(updatedBy) }
            };

            return new DBHelper().ExecuteNonQueryReturn(
                "sp_QC_DeleteLimeReactivitySamplesByReportID",
                p
            );
        }

        public int DeleteReport(int id, string updatedBy)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ID", SqlDbType.Int) { Value = id },
                new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = DbValue(updatedBy) }
            };

            return new DBHelper().ExecuteNonQueryReturn(
                "sp_QC_DeleteLimeReactivityReport",
                p
            );
        }

        private static QCLimeReactivitySampleBLL MapSample(DataRow r)
        {
            return new QCLimeReactivitySampleBLL
            {
                ID = GetInt(r, "ID"),
                ReportID = GetInt(r, "ReportID"),
                SrNo = GetInt(r, "SrNo"),
                ShipmentCode = GetString(r, "ShipmentCode"),
                ReceivedDate = GetDate(r, "ReceivedDate"),
                Supplier = GetString(r, "Supplier"),
                QuantityTons = GetDecimal(r, "QuantityTons"),
                SampleNo = GetString(r, "SampleNo"),
                ActiveCaO = GetDecimal(r, "ActiveCaO"),
                TotalCaO = GetDecimal(r, "TotalCaO"),
                MgO = GetDecimal(r, "MgO"),
                LOI = GetDecimal(r, "LOI"),
                Temp0Sec = GetDecimal(r, "Temp0Sec"),
                Temp30Sec = GetDecimal(r, "Temp30Sec"),
                Temp60Sec = GetDecimal(r, "Temp60Sec"),
                Temp90Sec = GetDecimal(r, "Temp90Sec"),
                Temp120Sec = GetDecimal(r, "Temp120Sec"),
                Temp150Sec = GetDecimal(r, "Temp150Sec"),
                Temp180Sec = GetDecimal(r, "Temp180Sec"),
                Temp240Sec = GetDecimal(r, "Temp240Sec"),
                Temp360Sec = GetDecimal(r, "Temp360Sec"),
                Temp600Sec = GetDecimal(r, "Temp600Sec"),
                TestStatus = GetString(r, "TestStatus"),
                StatusID = GetInt(r, "StatusID")
            };
        }

        private static SqlParameter DecimalParameter(string name, decimal? value)
        {
            var p = new SqlParameter(name, SqlDbType.Decimal) { Precision = 18, Scale = 3 };
            p.Value = value.HasValue ? (object)value.Value : DBNull.Value;
            return p;
        }

        private static object DbValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value.Trim();
        }

        private static string GetString(DataRow row, string col)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(col) || row[col] == DBNull.Value)
                return string.Empty;
            return Convert.ToString(row[col]).Trim();
        }

        private static int GetInt(DataRow row, string col)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(col) || row[col] == DBNull.Value)
                return 0;
            return Convert.ToInt32(row[col]);
        }

        private static DateTime? GetDate(DataRow row, string col)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(col) || row[col] == DBNull.Value)
                return null;
            return Convert.ToDateTime(row[col]);
        }

        private static decimal? GetDecimal(DataRow row, string col)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(col) || row[col] == DBNull.Value)
                return null;
            return Convert.ToDecimal(row[col]);
        }
    }
}
