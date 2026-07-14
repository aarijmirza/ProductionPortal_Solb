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
    public class StockRepository
    {
        // ==============================
        // MAIN INSERT
        // ==============================
        public int InsertSupplyChainStock(SupplyChainStockVM model, string createdBy)
        {
            try
            {
                int headerId = InsertHeader(model, createdBy);

                if (headerId <= 0)
                {
                    return 0;
                }

                if (model.DispatchDetails != null)
                {
                    foreach (var item in model.DispatchDetails)
                    {
                        InsertDispatchDetail(headerId, item, createdBy);
                    }
                }

                if (model.RebarStocks != null)
                {
                    foreach (var item in model.RebarStocks)
                    {
                        InsertRebarStock(headerId, item, createdBy);
                    }
                }

                if (model.WireRodStocks != null)
                {
                    foreach (var item in model.WireRodStocks)
                    {
                        InsertWireRodStock(headerId, item, createdBy);
                    }
                }

                if (model.BilletStocks != null)
                {
                    foreach (var item in model.BilletStocks)
                    {
                        InsertBilletStock(headerId, item, createdBy);
                    }
                }

                if (model.RawMaterialStocks != null)
                {
                    foreach (var item in model.RawMaterialStocks)
                    {
                        InsertRawMaterialStock(headerId, item, createdBy);
                    }
                }

                return -1;
            }
            catch
            {
                throw;
            }
        }

        private int InsertHeader(SupplyChainStockVM model, string createdBy)
        {
            SqlParameter[] param =
            {
            new SqlParameter("@ReportDate", model.ReportDate.HasValue ? (object)model.ReportDate.Value.Date : DBNull.Value),
            new SqlParameter("@CreatedBy", string.IsNullOrWhiteSpace(createdBy) ? "System" : createdBy)
        };

            DataTable dt = new DBHelper().GetTableFromSP("sp_InsertSupplyChainStockHeader", param);

            if (dt != null && dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0]["HeaderID"]);
            }

            return 0;
        }

        private int InsertDispatchDetail(int headerId, DispatchDetailBLL item, string createdBy)
        {
            SqlParameter[] param =
            {
            new SqlParameter("@HeaderID", headerId),
            new SqlParameter("@Material", string.IsNullOrWhiteSpace(item.Material) ? "" : item.Material),
            new SqlParameter("@Trucks", item.Trucks),
            new SqlParameter("@Tons", item.Tons),
            new SqlParameter("@MTD", item.MTD),
            new SqlParameter("@CreatedBy", string.IsNullOrWhiteSpace(createdBy) ? "System" : createdBy)
        };

            return new DBHelper().ExecuteNonQueryReturn("sp_InsertSupplyChainDispatchDetail", param);
        }

        private int InsertRebarStock(int headerId, RebarStockBLL item, string createdBy)
        {
            SqlParameter[] param =
            {
            new SqlParameter("@HeaderID", headerId),
            new SqlParameter("@Size", string.IsNullOrWhiteSpace(item.Size) ? "" : item.Size),
            new SqlParameter("@Prime", item.Prime),
            new SqlParameter("@Discolored", item.Discolored),
            new SqlParameter("@Epoxy", item.Epoxy),
            new SqlParameter("@ShortBars", item.ShortBars),
            new SqlParameter("@CreatedBy", string.IsNullOrWhiteSpace(createdBy) ? "System" : createdBy)
        };

            return new DBHelper().ExecuteNonQueryReturn("sp_InsertSupplyChainRebarStock", param);
        }

        private int InsertWireRodStock(int headerId, WireRodStockBLL item, string createdBy)
        {
            SqlParameter[] param =
            {
            new SqlParameter("@HeaderID", headerId),
            new SqlParameter("@Size", string.IsNullOrWhiteSpace(item.Size) ? "" : item.Size),
            new SqlParameter("@Grade", string.IsNullOrWhiteSpace(item.Grade) ? "" : item.Grade),
            new SqlParameter("@Prime", item.Prime),
            new SqlParameter("@CreatedBy", string.IsNullOrWhiteSpace(createdBy) ? "System" : createdBy)
        };

            return new DBHelper().ExecuteNonQueryReturn("sp_InsertSupplyChainWireRodStock", param);
        }

        private int InsertBilletStock(int headerId, BilletStockBLL item, string createdBy)
        {
            SqlParameter[] param =
            {
            new SqlParameter("@HeaderID", headerId),
            new SqlParameter("@Grade", string.IsNullOrWhiteSpace(item.Grade) ? "" : item.Grade),
            new SqlParameter("@QtyTon", item.QtyTon),
            new SqlParameter("@CreatedBy", string.IsNullOrWhiteSpace(createdBy) ? "System" : createdBy)
        };

            return new DBHelper().ExecuteNonQueryReturn("sp_InsertSupplyChainBilletStock", param);
        }

        private int InsertRawMaterialStock(int headerId, RawMaterialStockBLL item, string createdBy)
        {
            SqlParameter[] param =
            {
            new SqlParameter("@HeaderID", headerId),
            new SqlParameter("@MaterialDescription", string.IsNullOrWhiteSpace(item.MaterialDescription) ? "" : item.MaterialDescription),
            new SqlParameter("@QtyTon", item.QtyTon),
            new SqlParameter("@StockCategory", string.IsNullOrWhiteSpace(item.StockCategory) ? "" : item.StockCategory),
            new SqlParameter("@CreatedBy", string.IsNullOrWhiteSpace(createdBy) ? "System" : createdBy)
        };

            return new DBHelper().ExecuteNonQueryReturn("sp_InsertSupplyChainRawMaterialStock", param);
        }

        public List<SupplyChainStockHeaderBLL> GetSupplyChainStockHeaderList(DateTime? fromDate, DateTime? toDate)
        {
            List<SupplyChainStockHeaderBLL> list = new List<SupplyChainStockHeaderBLL>();

            SqlParameter[] param =
            {
            new SqlParameter("@FromDate", fromDate.HasValue ? (object)fromDate.Value.Date : DBNull.Value),
            new SqlParameter("@ToDate", toDate.HasValue ? (object)toDate.Value.Date : DBNull.Value)
        };

            DataTable dt = new DBHelper().GetTableFromSP("sp_GetSupplyChainStockHeaderList", param);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new SupplyChainStockHeaderBLL
                    {
                        ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]),
                        ReportDate = dr["ReportDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["ReportDate"]),
                        StatusID = dr["StatusID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["StatusID"]),
                        CreatedDate = dr["CreatedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["CreatedDate"]),
                        CreatedBy = dr["CreatedBy"] == DBNull.Value ? "" : dr["CreatedBy"].ToString(),
                        UpdatedDate = dr["UpdatedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["UpdatedDate"]),
                        UpdatedBy = dr["UpdatedBy"] == DBNull.Value ? "" : dr["UpdatedBy"].ToString()
                    });
                }
            }

            return list;
        }

        public SupplyChainStockVM GetSupplyChainStockDetailsByHeaderID(int headerId)
        {
            SupplyChainStockVM vm = new SupplyChainStockVM();

            SqlParameter[] param =
            {
            new SqlParameter("@HeaderID", headerId)
        };

            DataSet ds = new DBHelper().GetDatasetFromSP("sp_GetSupplyChainStockDetailsByHeaderID", param);

            if (ds == null || ds.Tables.Count == 0)
            {
                return vm;
            }

            // Header
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];

                vm.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                vm.ReportDate = dr["ReportDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["ReportDate"]);
            }

            // Dispatch
            if (ds.Tables.Count > 1)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    vm.DispatchDetails.Add(new DispatchDetailBLL
                    {
                        ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]),
                        HeaderID = dr["HeaderID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["HeaderID"]),
                        Material = dr["Material"] == DBNull.Value ? "" : dr["Material"].ToString(),
                        Trucks = dr["Trucks"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Trucks"]),
                        Tons = dr["Tons"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Tons"]),
                        MTD = dr["MTD"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MTD"])
                    });
                }
            }

            // Rebar
            if (ds.Tables.Count > 2)
            {
                foreach (DataRow dr in ds.Tables[2].Rows)
                {
                    vm.RebarStocks.Add(new RebarStockBLL
                    {
                        ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]),
                        HeaderID = dr["HeaderID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["HeaderID"]),
                        Size = dr["Size"] == DBNull.Value ? "" : dr["Size"].ToString(),
                        Prime = dr["Prime"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Prime"]),
                        Discolored = dr["Discolored"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Discolored"]),
                        Epoxy = dr["Epoxy"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Epoxy"]),
                        ShortBars = dr["ShortBars"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["ShortBars"])
                    });
                }
            }

            // Wire Rod
            if (ds.Tables.Count > 3)
            {
                foreach (DataRow dr in ds.Tables[3].Rows)
                {
                    vm.WireRodStocks.Add(new WireRodStockBLL
                    {
                        ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]),
                        HeaderID = dr["HeaderID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["HeaderID"]),
                        Size = dr["Size"] == DBNull.Value ? "" : dr["Size"].ToString(),
                        Grade = dr["Grade"] == DBNull.Value ? "" : dr["Grade"].ToString(),
                        Prime = dr["Prime"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Prime"])
                    });
                }
            }

            // Billet
            if (ds.Tables.Count > 4)
            {
                foreach (DataRow dr in ds.Tables[4].Rows)
                {
                    vm.BilletStocks.Add(new BilletStockBLL
                    {
                        ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]),
                        HeaderID = dr["HeaderID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["HeaderID"]),
                        Grade = dr["Grade"] == DBNull.Value ? "" : dr["Grade"].ToString(),
                        QtyTon = dr["QtyTon"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["QtyTon"])
                    });
                }
            }

            // Raw Material
            if (ds.Tables.Count > 5)
            {
                foreach (DataRow dr in ds.Tables[5].Rows)
                {
                    vm.RawMaterialStocks.Add(new RawMaterialStockBLL
                    {
                        ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]),
                        HeaderID = dr["HeaderID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["HeaderID"]),
                        MaterialDescription = dr["MaterialDescription"] == DBNull.Value ? "" : dr["MaterialDescription"].ToString(),
                        QtyTon = dr["QtyTon"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["QtyTon"]),
                        StockCategory = dr["StockCategory"] == DBNull.Value ? "" : dr["StockCategory"].ToString()
                    });
                }
            }

            return vm;
        }
    }
}
