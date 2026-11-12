
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebAPICode.Helpers;

namespace BAL.Repositories
{
    public class RMMTCRepository
    {
        public string GetNextNumberPreview()
        {
            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_RMMTC_GetNextNumberPreview",
                CommandType.StoredProcedure,
                null
            );

            if (dt == null || dt.Rows.Count == 0)
                return "";

            return Convert.ToString(dt.Rows[0]["MTCNo"]);
        }

        public RMMTCHeaderBLL CreateNew(string createdBy)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100)
                {
                    Value = string.IsNullOrWhiteSpace(createdBy)
                        ? (object)DBNull.Value
                        : createdBy
                }
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_RMMTC_CreateNew",
                CommandType.StoredProcedure,
                parameters
            );

            if (dt == null || dt.Rows.Count == 0)
                return null;

            return new RMMTCHeaderBLL
            {
                ID = GetInt(dt.Rows[0], "ID"),
                MTCNo = GetString(dt.Rows[0], "MTCNo"),
                IssueDate = DateTime.Today
            };
        }

        public void CancelDraft(int id, string updatedBy)
        {
            SqlParameter[] parameters =
            {
                P("@ID", SqlDbType.Int, id),
                P("@UpdatedBy", SqlDbType.NVarChar, updatedBy, 100)
            };

            DBHelper.ExecuteDataTable(
                "sp_RMMTC_CancelDraft",
                CommandType.StoredProcedure,
                parameters
            );
        }

        public void SaveHeader(RMMTCHeaderBLL model, string updatedBy)
        {
            SqlParameter[] parameters =
            {
                P("@ID", SqlDbType.Int, model.ID),
                P("@CustomerName", SqlDbType.NVarChar, model.CustomerName, 250),
                P("@ShippingAddress", SqlDbType.NVarChar, model.ShippingAddress, 500),
                P("@DNNo", SqlDbType.NVarChar, model.DNNo, 100),
                P("@CustomerNo", SqlDbType.NVarChar, model.CustomerNo, 100),
                P("@SalesOrderNo", SqlDbType.NVarChar, model.SalesOrderNo, 100),
                P("@CustomerOrderNo", SqlDbType.NVarChar, model.CustomerOrderNo, 100),
                P("@IssueDate", SqlDbType.Date, model.IssueDate),
                P("@IsWireRodMTC", SqlDbType.Bit, model.IsWireRodMTC),
                P("@UpdatedBy", SqlDbType.NVarChar, updatedBy, 100)
            };

            DBHelper.ExecuteDataTable(
                "sp_RMMTC_SaveHeader",
                CommandType.StoredProcedure,
                parameters
            );
        }

        public List<RMMTCEligibleHeatBLL> GetEligibleHeats(string search = null)
        {
            var list = new List<RMMTCEligibleHeatBLL>();

            SqlParameter[] parameters =
            {
                P("@Search", SqlDbType.NVarChar, search, 100)
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_RMMTC_GetEligibleHeats",
                CommandType.StoredProcedure,
                parameters
            );

            if (dt == null) return list;

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new RMMTCEligibleHeatBLL
                {
                    QCInspectionRMID = GetInt(row, "QCInspectionRMID"),
                    HeatNo = GetString(row, "HeatNo"),
                    SteelGrade = GetString(row, "SteelGrade"),
                    Specification = GetString(row, "Specification"),
                    ProductType = GetString(row, "ProductType"),
                    BarSize = GetNullableDecimal(row, "BarSize"),
                    NoOfBundles = GetNullableInt(row, "NoOfBundles"),
                    IsWireRodOrCoil = GetBool(row, "IsWireRodOrCoil"),
                    YieldStrength = GetNullableDecimal(row, "YieldStrength"),
                    TensileStrength = GetNullableDecimal(row, "TensileStrength"),
                    TensileYieldRatio = GetNullableDecimal(row, "TensileYieldRatio")
                });
            }

            return list;
        }

        public void AddHeat(int mtcID, int qcInspectionRMID, int? actualBundles, string createdBy)
        {
            SqlParameter[] parameters =
            {
                P("@MTCID", SqlDbType.Int, mtcID),
                P("@QCInspectionRMID", SqlDbType.Int, qcInspectionRMID),
                P("@ActualNoOfBundles", SqlDbType.Int, actualBundles),
                P("@CreatedBy", SqlDbType.NVarChar, createdBy, 100)
            };

            DBHelper.ExecuteDataTable(
                "sp_RMMTC_AddHeat",
                CommandType.StoredProcedure,
                parameters
            );
        }

        public void UpdateDetail(int detailID, int? actualBundles)
        {
            SqlParameter[] parameters =
            {
                P("@DetailID", SqlDbType.Int, detailID),
                P("@ActualNoOfBundles", SqlDbType.Int, actualBundles)
            };

            DBHelper.ExecuteDataTable(
                "sp_RMMTC_UpdateDetail",
                CommandType.StoredProcedure,
                parameters
            );
        }

        public void RemoveHeat(int detailID)
        {
            SqlParameter[] parameters =
            {
                P("@DetailID", SqlDbType.Int, detailID)
            };

            DBHelper.ExecuteDataTable(
                "sp_RMMTC_RemoveHeat",
                CommandType.StoredProcedure,
                parameters
            );
        }

        public RMMTCHeaderBLL GetHeader(int id)
        {
            SqlParameter[] parameters =
            {
                P("@ID", SqlDbType.Int, id)
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_RMMTC_GetHeaderByID",
                CommandType.StoredProcedure,
                parameters
            );

            if (dt == null || dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new RMMTCHeaderBLL
            {
                ID = GetInt(row, "ID"),
                MTCNo = GetString(row, "MTCNo"),
                IssueDate = GetDate(row, "IssueDate"),
                CustomerName = GetString(row, "CustomerName"),
                ShippingAddress = GetString(row, "ShippingAddress"),
                DNNo = GetString(row, "DNNo"),
                CustomerNo = GetString(row, "CustomerNo"),
                SalesOrderNo = GetString(row, "SalesOrderNo"),
                CustomerOrderNo = GetString(row, "CustomerOrderNo"),
                IsWireRodMTC = GetBool(row, "IsWireRodMTC"),
                IsFinal = GetBool(row, "IsFinal"),
                CreatedBy = GetString(row, "CreatedBy"),
                CreatedDate = GetNullableDate(row, "CreatedDate")
            };
        }

        public List<RMMTCDetailBLL> GetDetails(int mtcID)
        {
            var list = new List<RMMTCDetailBLL>();

            SqlParameter[] parameters =
            {
                P("@MTCID", SqlDbType.Int, mtcID)
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_RMMTC_GetDetailsByID",
                CommandType.StoredProcedure,
                parameters
            );

            if (dt == null) return list;

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new RMMTCDetailBLL
                {
                    ID = GetInt(row, "ID"),
                    MTCID = GetInt(row, "MTCID"),
                    QCInspectionRMID = GetInt(row, "QCInspectionRMID"),
                    HeatNo = GetString(row, "HeatNo"),
                    SteelGrade = GetString(row, "SteelGrade"),
                    Specification = GetString(row, "Specification"),
                    ProductType = GetString(row, "ProductType"),
                    BarSize = GetNullableDecimal(row, "BarSize"),
                    ActualNoOfBundles = GetNullableInt(row, "ActualNoOfBundles"),
                    SortOrder = GetInt(row, "SortOrder"),
                    Length = GetNullableDecimal(row, "Length"),
                    WeightPerBundle = GetNullableDecimal(row, "WeightPerBundle"),
                    NominalWeight = GetNullableDecimal(row, "NominalWeight"),
                    CrossSectionArea = GetNullableDecimal(row, "CrossSectionArea"),
                    NoOfBarsPerBundle = GetNullableInt(row, "NoOfBarsPerBundle"),
                    NoOfBundles = GetNullableInt(row, "NoOfBundles"),
                    BendTestObserved = GetBool(row, "BendTestObserved"),
                    YieldStrength = GetNullableDecimal(row, "YieldStrength"),
                    TensileStrength = GetNullableDecimal(row, "TensileStrength"),
                    TensileYieldRatio = GetNullableDecimal(row, "TensileYieldRatio"),
                    Elongation = GetNullableDecimal(row, "Elongation"),
                    GaugeLength = GetNullableDecimal(row, "GaugeLength"),
                    C = GetNullableDecimal(row, "C"),
                    Si = GetNullableDecimal(row, "Si"),
                    Mn = GetNullableDecimal(row, "Mn"),
                    P = GetNullableDecimal(row, "P"),
                    S = GetNullableDecimal(row, "S"),
                    Cu = GetNullableDecimal(row, "Cu"),
                    Ni = GetNullableDecimal(row, "Ni"),
                    Cr = GetNullableDecimal(row, "Cr"),
                    Mo = GetNullableDecimal(row, "Mo"),
                    V = GetNullableDecimal(row, "V"),
                    B = GetNullableDecimal(row, "B"),
                    N = GetNullableDecimal(row, "N"),
                    Ceq = GetNullableDecimal(row, "Ceq")
                });
            }

            return list;
        }

        public List<RMMTCListRowBLL> GetMTCList(string search = null)
        {
            var list = new List<RMMTCListRowBLL>();

            SqlParameter[] parameters =
            {
                P("@Search", SqlDbType.NVarChar, search, 100)
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_RMMTC_GetList",
                CommandType.StoredProcedure,
                parameters
            );

            if (dt == null) return list;

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new RMMTCListRowBLL
                {
                    ID = GetInt(row, "ID"),
                    MTCNo = GetString(row, "MTCNo"),
                    IssueDate = GetDate(row, "IssueDate"),
                    CustomerName = GetString(row, "CustomerName"),
                    CreatedBy = GetString(row, "CreatedBy"),
                    IsFinal = GetBool(row, "IsFinal"),
                    HeatCount = GetInt(row, "HeatCount")
                });
            }

            return list;
        }

        public void FinalizeMTC(int id, string updatedBy)
        {
            SqlParameter[] parameters =
            {
                P("@ID", SqlDbType.Int, id),
                P("@UpdatedBy", SqlDbType.NVarChar, updatedBy, 100)
            };

            DBHelper.ExecuteDataTable(
                "sp_RMMTC_Finalize",
                CommandType.StoredProcedure,
                parameters
            );
        }

        private SqlParameter P(string name, SqlDbType type, object value, int size = 0)
        {
            SqlParameter p =
                size > 0
                    ? new SqlParameter(name, type, size)
                    : new SqlParameter(name, type);

            p.Value = value ?? DBNull.Value;
            return p;
        }

        private string GetString(DataRow row, string col)
        {
            return row.Table.Columns.Contains(col) && row[col] != DBNull.Value
                ? Convert.ToString(row[col])
                : "";
        }

        private int GetInt(DataRow row, string col)
        {
            return row.Table.Columns.Contains(col) && row[col] != DBNull.Value
                ? Convert.ToInt32(row[col])
                : 0;
        }

        private int? GetNullableInt(DataRow row, string col)
        {
            return row.Table.Columns.Contains(col) && row[col] != DBNull.Value
                ? (int?)Convert.ToInt32(row[col])
                : null;
        }

        private decimal? GetNullableDecimal(DataRow row, string col)
        {
            return row.Table.Columns.Contains(col) && row[col] != DBNull.Value
                ? (decimal?)Convert.ToDecimal(row[col])
                : null;
        }

        private bool GetBool(DataRow row, string col)
        {
            return row.Table.Columns.Contains(col) && row[col] != DBNull.Value
                ? Convert.ToBoolean(row[col])
                : false;
        }

        private DateTime GetDate(DataRow row, string col)
        {
            return row.Table.Columns.Contains(col) && row[col] != DBNull.Value
                ? Convert.ToDateTime(row[col])
                : DateTime.Today;
        }

        private DateTime? GetNullableDate(DataRow row, string col)
        {
            return row.Table.Columns.Contains(col) && row[col] != DBNull.Value
                ? (DateTime?)Convert.ToDateTime(row[col])
                : null;
        }
    }
}
