using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebAPICode.Helpers;

namespace BAL.Repositories
{
    public class CokeCoalRepository
    {
        public List<QCCokeCoalReportBLL> GetAll()
        {
            var list = new List<QCCokeCoalReportBLL>();

            DataTable dt = new DBHelper().GetTableFromSP(
                "sp_QC_GetCokeCoalList",
                new SqlParameter[0]
            );

            if (dt == null)
                return list;

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapMaster(row));
            }

            return list;
        }

        public QCCokeCoalReportBLL GetByID(int id)
        {
            if (id <= 0)
                return null;

            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", SqlDbType.Int) { Value = id }
            };

            DataSet ds = new DBHelper().GetDatasetFromSP(
                "sp_QC_GetCokeCoalByID",
                parameters
            );

            if (
                ds == null ||
                ds.Tables.Count == 0 ||
                ds.Tables[0].Rows.Count == 0
            )
            {
                return null;
            }

            var model = MapMaster(ds.Tables[0].Rows[0]);

            model.Samples = new List<QCCokeCoalSampleBLL>();

            if (ds.Tables.Count > 1 && ds.Tables[1] != null)
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    model.Samples.Add(MapSample(row));
                }
            }

            return model;
        }

        public int Insert(QCCokeCoalReportBLL model)
        {
            SqlParameter[] parameters =
            {
                Str("@Supplier", 200, model.Supplier),
                Dec("@Quantity", model.Quantity, 3),
                Str("@ShipmentCodeNo", 100, model.ShipmentCodeNo),
                Date("@ReceivingDate", model.ReceivingDate),
                Date("@AnalysisDate", model.AnalysisDate),
                Str("@Material", 100, model.Material),
                Str("@Description", 500, model.Description),
                Str("@DeliveryPO", 100, model.DeliveryPO),
                Str("@LotSize", 100, model.LotSize),
                Str("@CertificateNo", 100, model.CertificateNo),
                Str("@FinalStatus", 30, model.FinalStatus),
                Str("@MRBNo", 100, model.MRBNo),
                StrMax("@DecisionDetails", model.DecisionDetails),
                StrMax("@Comments", model.Comments),
                Str("@CreatedBy", 100, model.CreatedBy)
            };

            object result = new DBHelper().ExecuteScalar(
                "sp_QC_InsertCokeCoalReport",
                parameters
            );

            return ToInt(result);
        }

        public int Update(QCCokeCoalReportBLL model)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", SqlDbType.Int) { Value = model.ID },
                Str("@Supplier", 200, model.Supplier),
                Dec("@Quantity", model.Quantity, 3),
                Str("@ShipmentCodeNo", 100, model.ShipmentCodeNo),
                Date("@ReceivingDate", model.ReceivingDate),
                Date("@AnalysisDate", model.AnalysisDate),
                Str("@Material", 100, model.Material),
                Str("@Description", 500, model.Description),
                Str("@DeliveryPO", 100, model.DeliveryPO),
                Str("@LotSize", 100, model.LotSize),
                Str("@CertificateNo", 100, model.CertificateNo),
                Str("@FinalStatus", 30, model.FinalStatus),
                Str("@MRBNo", 100, model.MRBNo),
                StrMax("@DecisionDetails", model.DecisionDetails),
                StrMax("@Comments", model.Comments),
                Str("@UpdatedBy", 100, model.UpdatedBy)
            };

            object result = new DBHelper().ExecuteScalar(
                "sp_QC_UpdateCokeCoalReport",
                parameters
            );

            return ToInt(result);
        }

        public int InsertSample(QCCokeCoalSampleBLL sample)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ReportID", SqlDbType.Int)
                {
                    Value = sample.ReportID
                },

                new SqlParameter("@SrNo", SqlDbType.Int)
                {
                    Value = sample.SrNo
                },

                Str("@SampleCode", 100, sample.SampleCode),
                Dec("@Ash", sample.Ash, 4),
                Dec("@Moisture", sample.Moisture, 4),
                Dec("@Volatile", sample.Volatile, 4),
                Dec("@S", sample.S, 4),
                Dec("@FixedCarbon", sample.FixedCarbon, 4),
                Str("@GrainSizeMM", 100, sample.GrainSizeMM),
                Str("@Remark", 1000, sample.Remark),
                Str("@CreatedBy", 100, sample.CreatedBy)
            };

            object result = new DBHelper().ExecuteScalar(
                "sp_QC_InsertCokeCoalSample",
                parameters
            );

            return ToInt(result);
        }

        public int DeleteSamplesByReportID(int reportID, string updatedBy)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ReportID", SqlDbType.Int) { Value = reportID },
                Str("@UpdatedBy", 100, updatedBy)
            };

            return new DBHelper().ExecuteNonQueryReturn(
                "sp_QC_DeleteCokeCoalSamplesByReportID",
                parameters
            );
        }

        public int Delete(int id, string updatedBy)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", SqlDbType.Int) { Value = id },
                Str("@UpdatedBy", 100, updatedBy)
            };

            object result = new DBHelper().ExecuteScalar(
                "sp_QC_DeleteCokeCoalReport",
                parameters
            );

            return ToInt(result);
        }

        private static QCCokeCoalReportBLL MapMaster(DataRow row)
        {
            return new QCCokeCoalReportBLL
            {
                ID = GetInt(row, "ID"),
                Supplier = GetString(row, "Supplier"),
                Quantity = GetDecimal(row, "Quantity"),
                ShipmentCodeNo = GetString(row, "ShipmentCodeNo"),
                ReceivingDate = GetDate(row, "ReceivingDate"),
                AnalysisDate = GetDate(row, "AnalysisDate"),
                Material = GetString(row, "Material"),
                Description = GetString(row, "Description"),
                DeliveryPO = GetString(row, "DeliveryPO"),
                LotSize = GetString(row, "LotSize"),
                CertificateNo = GetString(row, "CertificateNo"),
                FinalStatus = GetString(row, "FinalStatus"),
                MRBNo = GetString(row, "MRBNo"),
                DecisionDetails = GetString(row, "DecisionDetails"),
                Comments = GetString(row, "Comments"),
                StatusID = GetInt(row, "StatusID"),
                CreatedDate = GetDate(row, "CreatedDate"),
                CreatedBy = GetString(row, "CreatedBy"),
                UpdatedDate = GetDate(row, "UpdatedDate"),
                UpdatedBy = GetString(row, "UpdatedBy")
            };
        }

        private static QCCokeCoalSampleBLL MapSample(DataRow row)
        {
            return new QCCokeCoalSampleBLL
            {
                ID = GetInt(row, "ID"),
                ReportID = GetInt(row, "ReportID"),
                SrNo = GetInt(row, "SrNo"),
                SampleCode = GetString(row, "SampleCode"),
                Ash = GetDecimal(row, "Ash"),
                Moisture = GetDecimal(row, "Moisture"),
                Volatile = GetDecimal(row, "Volatile"),
                S = GetDecimal(row, "S"),
                FixedCarbon = GetDecimal(row, "FixedCarbon"),
                GrainSizeMM = GetString(row, "GrainSizeMM"),
                Remark = GetString(row, "Remark"),
                StatusID = GetInt(row, "StatusID"),
                CreatedDate = GetDate(row, "CreatedDate"),
                CreatedBy = GetString(row, "CreatedBy"),
                UpdatedDate = GetDate(row, "UpdatedDate"),
                UpdatedBy = GetString(row, "UpdatedBy")
            };
        }

        private static SqlParameter Str(string name, int size, string value)
        {
            return new SqlParameter(name, SqlDbType.NVarChar, size)
            {
                Value = string.IsNullOrWhiteSpace(value)
                    ? (object)DBNull.Value
                    : value.Trim()
            };
        }

        private static SqlParameter StrMax(string name, string value)
        {
            return new SqlParameter(name, SqlDbType.NVarChar, -1)
            {
                Value = string.IsNullOrWhiteSpace(value)
                    ? (object)DBNull.Value
                    : value.Trim()
            };
        }

        private static SqlParameter Dec(string name, decimal? value, byte scale)
        {
            var p = new SqlParameter(name, SqlDbType.Decimal);
            p.Precision = 18;
            p.Scale = scale;
            p.Value = value.HasValue ? (object)value.Value : DBNull.Value;
            return p;
        }

        private static SqlParameter Date(string name, DateTime? value)
        {
            return new SqlParameter(name, SqlDbType.Date)
            {
                Value = value.HasValue
                    ? (object)value.Value.Date
                    : DBNull.Value
            };
        }

        private static int ToInt(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            int result;
            return int.TryParse(Convert.ToString(value), out result)
                ? result
                : 0;
        }

        private static int GetInt(DataRow row, string col)
        {
            if (!Has(row, col))
                return 0;

            int result;
            return int.TryParse(Convert.ToString(row[col]), out result)
                ? result
                : 0;
        }

        private static string GetString(DataRow row, string col)
        {
            return Has(row, col)
                ? Convert.ToString(row[col]).Trim()
                : string.Empty;
        }

        private static decimal? GetDecimal(DataRow row, string col)
        {
            if (!Has(row, col))
                return null;

            try
            {
                return Convert.ToDecimal(row[col]);
            }
            catch
            {
                return null;
            }
        }

        private static DateTime? GetDate(DataRow row, string col)
        {
            if (!Has(row, col))
                return null;

            DateTime result;
            return DateTime.TryParse(Convert.ToString(row[col]), out result)
                ? (DateTime?)result
                : null;
        }

        private static bool Has(DataRow row, string col)
        {
            return
                row != null &&
                row.Table != null &&
                row.Table.Columns.Contains(col) &&
                row[col] != DBNull.Value;
        }
    }
}
