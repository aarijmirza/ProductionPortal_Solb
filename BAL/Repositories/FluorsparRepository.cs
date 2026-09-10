using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebAPICode.Helpers;

namespace BAL.Repositories
{
    public class FluorsparRepository
    {
        public List<QCFluorsparReportBLL> GetAll()
        {
            var list = new List<QCFluorsparReportBLL>();
            DataTable dt = new DBHelper().GetTableFromSP("sp_QC_GetFluorsparList", new SqlParameter[0]);
            if (dt == null) return list;
            foreach (DataRow row in dt.Rows) list.Add(MapMaster(row));
            return list;
        }

        public QCFluorsparReportBLL GetByID(int id)
        {
            if (id <= 0) return null;
            SqlParameter[] p = { new SqlParameter("@ID", SqlDbType.Int) { Value = id } };
            DataSet ds = new DBHelper().GetDatasetFromSP("sp_QC_GetFluorsparByID", p);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return null;

            var model = MapMaster(ds.Tables[0].Rows[0]);
            model.Samples = new List<QCFluorsparSampleBLL>();
            if (ds.Tables.Count > 1 && ds.Tables[1] != null)
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                    model.Samples.Add(MapSample(row));
            }
            return model;
        }

        public int Insert(QCFluorsparReportBLL model)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@Supplier", SqlDbType.NVarChar, 200){ Value = DbString(model.Supplier) },
                Dec("@Quantity", model.Quantity, 3),
                new SqlParameter("@ShipmentCodeNo", SqlDbType.NVarChar, 100){ Value = DbString(model.ShipmentCodeNo) },
                new SqlParameter("@ReceivingDate", SqlDbType.Date){ Value = model.ReceivingDate.HasValue ? (object)model.ReceivingDate.Value.Date : DBNull.Value },
                new SqlParameter("@AnalysisDate", SqlDbType.Date){ Value = model.AnalysisDate.HasValue ? (object)model.AnalysisDate.Value.Date : DBNull.Value },
                new SqlParameter("@Material", SqlDbType.NVarChar, 100){ Value = DbString(model.Material) },
                new SqlParameter("@ReferenceNo", SqlDbType.NVarChar, 100){ Value = DbString(model.ReferenceNo) },
                new SqlParameter("@DeliveryPO", SqlDbType.NVarChar, 100){ Value = DbString(model.DeliveryPO) },
                new SqlParameter("@LotSize", SqlDbType.NVarChar, 100){ Value = DbString(model.LotSize) },
                new SqlParameter("@CertificateNo", SqlDbType.NVarChar, 100){ Value = DbString(model.CertificateNo) },
                new SqlParameter("@FinalStatus", SqlDbType.NVarChar, 30){ Value = DbString(model.FinalStatus) },
                new SqlParameter("@MRBNo", SqlDbType.NVarChar, 100){ Value = DbString(model.MRBNo) },
                new SqlParameter("@Remarks", SqlDbType.NVarChar, -1){ Value = DbString(model.Remarks) },
                new SqlParameter("@Comments", SqlDbType.NVarChar, -1){ Value = DbString(model.Comments) },
                new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100){ Value = DbString(model.CreatedBy) }
            };
            return ToInt(new DBHelper().ExecuteScalar("sp_QC_InsertFluorsparReport", p));
        }

        public int Update(QCFluorsparReportBLL model)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ID", SqlDbType.Int){ Value = model.ID },
                new SqlParameter("@Supplier", SqlDbType.NVarChar, 200){ Value = DbString(model.Supplier) },
                Dec("@Quantity", model.Quantity, 3),
                new SqlParameter("@ShipmentCodeNo", SqlDbType.NVarChar, 100){ Value = DbString(model.ShipmentCodeNo) },
                new SqlParameter("@ReceivingDate", SqlDbType.Date){ Value = model.ReceivingDate.HasValue ? (object)model.ReceivingDate.Value.Date : DBNull.Value },
                new SqlParameter("@AnalysisDate", SqlDbType.Date){ Value = model.AnalysisDate.HasValue ? (object)model.AnalysisDate.Value.Date : DBNull.Value },
                new SqlParameter("@Material", SqlDbType.NVarChar, 100){ Value = DbString(model.Material) },
                new SqlParameter("@ReferenceNo", SqlDbType.NVarChar, 100){ Value = DbString(model.ReferenceNo) },
                new SqlParameter("@DeliveryPO", SqlDbType.NVarChar, 100){ Value = DbString(model.DeliveryPO) },
                new SqlParameter("@LotSize", SqlDbType.NVarChar, 100){ Value = DbString(model.LotSize) },
                new SqlParameter("@CertificateNo", SqlDbType.NVarChar, 100){ Value = DbString(model.CertificateNo) },
                new SqlParameter("@FinalStatus", SqlDbType.NVarChar, 30){ Value = DbString(model.FinalStatus) },
                new SqlParameter("@MRBNo", SqlDbType.NVarChar, 100){ Value = DbString(model.MRBNo) },
                new SqlParameter("@Remarks", SqlDbType.NVarChar, -1){ Value = DbString(model.Remarks) },
                new SqlParameter("@Comments", SqlDbType.NVarChar, -1){ Value = DbString(model.Comments) },
                new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100){ Value = DbString(model.UpdatedBy) }
            };
            return ToInt(new DBHelper().ExecuteScalar("sp_QC_UpdateFluorsparReport", p));
        }

        public int InsertSample(QCFluorsparSampleBLL s)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ReportID", SqlDbType.Int){ Value = s.ReportID },
                new SqlParameter("@SrNo", SqlDbType.Int){ Value = s.SrNo },
                new SqlParameter("@SampleNo", SqlDbType.NVarChar, 100){ Value = DbString(s.SampleNo) },
                Dec("@CaF2", s.CaF2, 4), Dec("@SiO2", s.SiO2, 4), Dec("@P", s.P, 4), Dec("@S", s.S, 4),
                Dec("@Fe2O3", s.Fe2O3, 4), Dec("@Al2O3", s.Al2O3, 4), Dec("@Na2O", s.Na2O, 4),
                Dec("@K2O", s.K2O, 4), Dec("@BaO", s.BaO, 4), Dec("@Pb", s.Pb, 4),
                new SqlParameter("@Comment", SqlDbType.NVarChar, 1000){ Value = DbString(s.Comment) },
                new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100){ Value = DbString(s.CreatedBy) }
            };
            return ToInt(new DBHelper().ExecuteScalar("sp_QC_InsertFluorsparSample", p));
        }

        public int DeleteSamplesByReportID(int reportID, string updatedBy)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ReportID", SqlDbType.Int){ Value = reportID },
                new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100){ Value = DbString(updatedBy) }
            };
            return new DBHelper().ExecuteNonQueryReturn("sp_QC_DeleteFluorsparSamplesByReportID", p);
        }

        public int Delete(int id, string updatedBy)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ID", SqlDbType.Int){ Value = id },
                new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100){ Value = DbString(updatedBy) }
            };
            return ToInt(new DBHelper().ExecuteScalar("sp_QC_DeleteFluorsparReport", p));
        }

        private static QCFluorsparReportBLL MapMaster(DataRow r)
        {
            return new QCFluorsparReportBLL
            {
                ID = I(r, "ID"),
                Supplier = S(r, "Supplier"),
                Quantity = D(r, "Quantity"),
                ShipmentCodeNo = S(r, "ShipmentCodeNo"),
                ReceivingDate = DT(r, "ReceivingDate"),
                AnalysisDate = DT(r, "AnalysisDate"),
                Material = S(r, "Material"),
                ReferenceNo = S(r, "ReferenceNo"),
                DeliveryPO = S(r, "DeliveryPO"),
                LotSize = S(r, "LotSize"),
                CertificateNo = S(r, "CertificateNo"),
                FinalStatus = S(r, "FinalStatus"),
                MRBNo = S(r, "MRBNo"),
                Remarks = S(r, "Remarks"),
                Comments = S(r, "Comments"),
                StatusID = I(r, "StatusID"),
                CreatedDate = DT(r, "CreatedDate"),
                CreatedBy = S(r, "CreatedBy"),
                UpdatedDate = DT(r, "UpdatedDate"),
                UpdatedBy = S(r, "UpdatedBy")
            };
        }

        private static QCFluorsparSampleBLL MapSample(DataRow r)
        {
            return new QCFluorsparSampleBLL
            {
                ID = I(r, "ID"),
                ReportID = I(r, "ReportID"),
                SrNo = I(r, "SrNo"),
                SampleNo = S(r, "SampleNo"),
                CaF2 = D(r, "CaF2"),
                SiO2 = D(r, "SiO2"),
                P = D(r, "P"),
                S = D(r, "S"),
                Fe2O3 = D(r, "Fe2O3"),
                Al2O3 = D(r, "Al2O3"),
                Na2O = D(r, "Na2O"),
                K2O = D(r, "K2O"),
                BaO = D(r, "BaO"),
                Pb = D(r, "Pb"),
                Comment = S(r, "Comment"),
                StatusID = I(r, "StatusID"),
                CreatedDate = DT(r, "CreatedDate"),
                CreatedBy = S(r, "CreatedBy"),
                UpdatedDate = DT(r, "UpdatedDate"),
                UpdatedBy = S(r, "UpdatedBy")
            };
        }

        private static SqlParameter Dec(string name, decimal? value, byte scale)
        {
            var p = new SqlParameter(name, SqlDbType.Decimal) { Precision = 18, Scale = scale, Value = value.HasValue ? (object)value.Value : DBNull.Value };
            return p;
        }
        private static object DbString(string value) => string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value.Trim();
        private static int ToInt(object v) { int x; return v != null && v != DBNull.Value && int.TryParse(Convert.ToString(v), out x) ? x : 0; }
        private static int I(DataRow r, string c) { int x; return r.Table.Columns.Contains(c) && r[c] != DBNull.Value && int.TryParse(Convert.ToString(r[c]), out x) ? x : 0; }
        private static string S(DataRow r, string c) => r.Table.Columns.Contains(c) && r[c] != DBNull.Value ? Convert.ToString(r[c]).Trim() : "";
        private static decimal? D(DataRow r, string c) { if (!r.Table.Columns.Contains(c) || r[c] == DBNull.Value) return null; try { return Convert.ToDecimal(r[c]); } catch { return null; } }
        private static DateTime? DT(DataRow r, string c) { DateTime x; return r.Table.Columns.Contains(c) && r[c] != DBNull.Value && DateTime.TryParse(Convert.ToString(r[c]), out x) ? (DateTime?)x : null; }
    }
}
