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
    public class SupplyChainRepository
    {
        public int SaveSupplyChainDaily(SupplyChainDailyBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[33];

                p[0] = new SqlParameter("@ID", model.ID);
                p[1] = new SqlParameter("@ReportDate", model.ReportDate.HasValue ? (object)model.ReportDate.Value : DBNull.Value);

                p[2] = new SqlParameter("@Scrap", model.Scrap.HasValue ? (object)model.Scrap.Value : DBNull.Value);
                p[3] = new SqlParameter("@DRI", model.DRI.HasValue ? (object)model.DRI.Value : DBNull.Value);
                p[4] = new SqlParameter("@HBI", model.HBI.HasValue ? (object)model.HBI.Value : DBNull.Value);

                p[5] = new SqlParameter("@Billet", model.Billet.HasValue ? (object)model.Billet.Value : DBNull.Value);
                p[6] = new SqlParameter("@Rebar", model.Rebar.HasValue ? (object)model.Rebar.Value : DBNull.Value);
                p[7] = new SqlParameter("@WireRodCoil", model.WireRodCoil.HasValue ? (object)model.WireRodCoil.Value : DBNull.Value);
                p[8] = new SqlParameter("@RebarInCoil", model.RebarInCoil.HasValue ? (object)model.RebarInCoil.Value : DBNull.Value);
                p[9] = new SqlParameter("@EpoxyRebar", model.EpoxyRebar.HasValue ? (object)model.EpoxyRebar.Value : DBNull.Value);

                p[10] = new SqlParameter("@DailyDispatch", model.DailyDispatch.HasValue ? (object)model.DailyDispatch.Value : DBNull.Value);
                p[11] = new SqlParameter("@DailyDispatchTarget", model.DailyDispatchTarget.HasValue ? (object)model.DailyDispatchTarget.Value : DBNull.Value);
                p[12] = new SqlParameter("@WTDDispatch", model.WTDDispatch.HasValue ? (object)model.WTDDispatch.Value : DBNull.Value);
                p[13] = new SqlParameter("@WTDDispatchTarget", model.WTDDispatchTarget.HasValue ? (object)model.WTDDispatchTarget.Value : DBNull.Value);
                p[14] = new SqlParameter("@MTDDispatch", model.MTDDispatch.HasValue ? (object)model.MTDDispatch.Value : DBNull.Value);
                p[15] = new SqlParameter("@MTDDispatchTarget", model.MTDDispatchTarget.HasValue ? (object)model.MTDDispatchTarget.Value : DBNull.Value);

                p[16] = new SqlParameter("@RawMaterialsReceived", model.RawMaterialsReceived.HasValue ? (object)model.RawMaterialsReceived.Value : DBNull.Value);
                p[17] = new SqlParameter("@SubRawMaterialsReceived", model.SubRawMaterialsReceived.HasValue ? (object)model.SubRawMaterialsReceived.Value : DBNull.Value);
                p[18] = new SqlParameter("@RefractoryMaterialsReceived", model.RefractoryMaterialsReceived.HasValue ? (object)model.RefractoryMaterialsReceived.Value : DBNull.Value);
                p[19] = new SqlParameter("@FuelOilReceived", model.FuelOilReceived.HasValue ? (object)model.FuelOilReceived.Value : DBNull.Value);
                p[20] = new SqlParameter("@OtherReceived", model.OtherReceived.HasValue ? (object)model.OtherReceived.Value : DBNull.Value);

                p[21] = new SqlParameter("@MillScale", model.MillScale.HasValue ? (object)model.MillScale.Value : DBNull.Value);
                p[22] = new SqlParameter("@Slag", model.Slag.HasValue ? (object)model.Slag.Value : DBNull.Value);
                p[23] = new SqlParameter("@Dust", model.Dust.HasValue ? (object)model.Dust.Value : DBNull.Value);
                p[24] = new SqlParameter("@Sludge", model.Sludge.HasValue ? (object)model.Sludge.Value : DBNull.Value);

                p[25] = new SqlParameter("@StatusID", model.StatusID.HasValue ? (object)model.StatusID.Value : 1);
                p[26] = new SqlParameter("@CreatedBy", string.IsNullOrWhiteSpace(model.CreatedBy) ? (object)DBNull.Value : model.CreatedBy);
                p[27] = new SqlParameter("@CreatedDate", model.CreatedDate.HasValue ? (object)model.CreatedDate.Value : DateTime.Now);
                p[28] = new SqlParameter("@UpdatedBy", string.IsNullOrWhiteSpace(model.UpdatedBy) ? (object)DBNull.Value : model.UpdatedBy);
                p[29] = new SqlParameter("@UpdatedDate", model.UpdatedDate.HasValue ? (object)model.UpdatedDate.Value : DBNull.Value);

                p[30] = new SqlParameter("@ShortBar", model.ShortBar.HasValue ? (object)model.ShortBar.Value : DBNull.Value);
                p[31] = new SqlParameter("@DailyTruck", model.ShortBar.HasValue ? (object)model.DailyTruck.Value : DBNull.Value);
                p[32] = new SqlParameter("@DailyTruckTarget", model.ShortBar.HasValue ? (object)model.DailyTruckTarget.Value : DBNull.Value);

                return new DBHelper().ExecuteNonQueryReturn("sp_SaveSupplyChainDaily", p);
            }
            catch
            {
                return 0;
            }
        }

        public SupplyChainDailyBLL GetSupplyChainDailyReport(DateTime? from, DateTime? to)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[2];
                p[0] = new SqlParameter("@FromDate", from.HasValue ? (object)from.Value.Date : DBNull.Value);
                p[1] = new SqlParameter("@ToDate", to.HasValue ? (object)to.Value.Date : DBNull.Value);

                DataTable dt = new DBHelper().GetTableFromSP("sp_GetSupplyChainDailyReport", p);

                if (dt == null || dt.Rows.Count == 0)
                    return new SupplyChainDailyBLL();

                DataRow r = dt.Rows[0];

                return new SupplyChainDailyBLL
                {
                    ID = r["ID"] == DBNull.Value ? 0 : Convert.ToInt32(r["ID"]),
                    ReportDate = r["ReportDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["ReportDate"]),
                    //ReportTime = r["ReportTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)r["ReportTime"],

                    Scrap = r["Scrap"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Scrap"]),
                    DRI = r["DRI"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["DRI"]),
                    HBI = r["HBI"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["HBI"]),

                    Billet = r["Billet"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Billet"]),
                    Rebar = r["Rebar"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Rebar"]),
                    WireRodCoil = r["WireRodCoil"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["WireRodCoil"]),
                    RebarInCoil = r["RebarInCoil"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["RebarInCoil"]),
                    EpoxyRebar = r["EpoxyRebar"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["EpoxyRebar"]),
                    ShortBar = r["ShortBar"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["ShortBar"]),

                    DailyDispatch = r["DailyDispatch"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["DailyDispatch"]),
                    DailyDispatchTarget = r["DailyDispatchTarget"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["DailyDispatchTarget"]),
                    DailyTruck = r["DailyTruck"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["DailyTruck"]),
                    DailyTruckTarget = r["DailyTruckTarget"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["DailyTruckTarget"]),
                    WTDDispatch = r["WTDDispatch"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["WTDDispatch"]),
                    WTDDispatchTarget = r["WTDDispatchTarget"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["WTDDispatchTarget"]),
                    MTDDispatch = r["MTDDispatch"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["MTDDispatch"]),
                    MTDDispatchTarget = r["MTDDispatchTarget"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["MTDDispatchTarget"]),

                    RawMaterialsReceived = r["RawMaterialsReceived"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["RawMaterialsReceived"]),
                    SubRawMaterialsReceived = r["SubRawMaterialsReceived"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["SubRawMaterialsReceived"]),
                    RefractoryMaterialsReceived = r["RefractoryMaterialsReceived"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["RefractoryMaterialsReceived"]),
                    FuelOilReceived = r["FuelOilReceived"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["FuelOilReceived"]),
                    OtherReceived = r["OtherReceived"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["OtherReceived"]),

                    MillScale = r["MillScale"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["MillScale"]),
                    Slag = r["Slag"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Slag"]),
                    Dust = r["Dust"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Dust"]),
                    Sludge = r["Sludge"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Sludge"])
                };
            }
            catch
            {
                return new SupplyChainDailyBLL();
            }
        }

        public List<SupplyChainDailyBLL> GetSupplyChainDailyList(DateTime? fromDate, DateTime? toDate)
        {
            var list = new List<SupplyChainDailyBLL>();

            try
            {
                SqlParameter[] p = new SqlParameter[2];

                p[0] = new SqlParameter("@FromDate",
                    fromDate.HasValue ? (object)fromDate.Value.Date : DBNull.Value);

                p[1] = new SqlParameter("@ToDate",
                    toDate.HasValue ? (object)toDate.Value.Date : DBNull.Value);

                DataTable dt = new DBHelper().GetTableFromSP("sp_GetSupplyChainDailyList", p);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        list.Add(new SupplyChainDailyBLL
                        {
                            ID = r["ID"] == DBNull.Value ? 0 : Convert.ToInt32(r["ID"]),

                            ReportDate = r["ReportDate"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(r["ReportDate"]),

                            ReportTime = r["ReportTime"] == DBNull.Value
                                ? (TimeSpan?)null
                                : (TimeSpan)r["ReportTime"],

                            Scrap = r["Scrap"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Scrap"]),
                            DRI = r["DRI"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["DRI"]),
                            HBI = r["HBI"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["HBI"]),

                            Billet = r["Billet"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Billet"]),
                            Rebar = r["Rebar"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Rebar"]),
                            WireRodCoil = r["WireRodCoil"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["WireRodCoil"]),
                            RebarInCoil = r["RebarInCoil"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["RebarInCoil"]),
                            EpoxyRebar = r["EpoxyRebar"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["EpoxyRebar"]),
                            ShortBar = r["ShortBar"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["ShortBar"]),

                            DailyDispatch = r["DailyDispatch"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["DailyDispatch"]),
                            DailyDispatchTarget = r["DailyDispatchTarget"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["DailyDispatchTarget"]),
                            DailyTruck = r["DailyTruck"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["DailyTruck"]),
                            DailyTruckTarget = r["DailyTruckTarget"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["DailyTruckTarget"]),
                            WTDDispatch = r["WTDDispatch"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["WTDDispatch"]),
                            WTDDispatchTarget = r["WTDDispatchTarget"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["WTDDispatchTarget"]),
                            MTDDispatch = r["MTDDispatch"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["MTDDispatch"]),
                            MTDDispatchTarget = r["MTDDispatchTarget"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["MTDDispatchTarget"]),

                            RawMaterialsReceived = r["RawMaterialsReceived"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["RawMaterialsReceived"]),
                            SubRawMaterialsReceived = r["SubRawMaterialsReceived"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["SubRawMaterialsReceived"]),
                            RefractoryMaterialsReceived = r["RefractoryMaterialsReceived"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["RefractoryMaterialsReceived"]),
                            FuelOilReceived = r["FuelOilReceived"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["FuelOilReceived"]),
                            OtherReceived = r["OtherReceived"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["OtherReceived"]),

                            MillScale = r["MillScale"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["MillScale"]),
                            Slag = r["Slag"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Slag"]),
                            Dust = r["Dust"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Dust"]),
                            Sludge = r["Sludge"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Sludge"]),

                            StatusID = r["StatusID"] == DBNull.Value ? 0 : Convert.ToInt32(r["StatusID"]),
                            CreatedBy = r["CreatedBy"] == DBNull.Value ? "" : Convert.ToString(r["CreatedBy"]),
                            CreatedDate = r["CreatedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["CreatedDate"]),
                            UpdatedBy = r["UpdatedBy"] == DBNull.Value ? "" : Convert.ToString(r["UpdatedBy"]),
                            UpdatedDate = r["UpdatedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["UpdatedDate"])
                        });
                    }
                }
            }
            catch
            {
                return list;
            }

            return list;
        }

        public SupplyChainDailyBLL GetSupplyChainDailyByID(int id)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[1];
                p[0] = new SqlParameter("@ID", id);

                DataTable dt = new DBHelper().GetTableFromSP("sp_GetSupplyChainDailyByID", p);

                if (dt == null || dt.Rows.Count == 0)
                    return new SupplyChainDailyBLL();

                DataRow r = dt.Rows[0];

                return new SupplyChainDailyBLL
                {
                    ID = r["ID"] == DBNull.Value ? 0 : Convert.ToInt32(r["ID"]),
                    ReportDate = r["ReportDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["ReportDate"]),
                    ReportTime = r["ReportTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)r["ReportTime"],

                    Scrap = r["Scrap"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Scrap"]),
                    DRI = r["DRI"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["DRI"]),
                    HBI = r["HBI"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["HBI"]),

                    Billet = r["Billet"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Billet"]),
                    Rebar = r["Rebar"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Rebar"]),
                    WireRodCoil = r["WireRodCoil"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["WireRodCoil"]),
                    RebarInCoil = r["RebarInCoil"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["RebarInCoil"]),
                    EpoxyRebar = r["EpoxyRebar"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["EpoxyRebar"]),

                    DailyDispatch = r["DailyDispatch"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["DailyDispatch"]),
                    DailyDispatchTarget = r["DailyDispatchTarget"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["DailyDispatchTarget"]),
                    DailyTruck = r["DailyTruck"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["DailyTruck"]),
                    DailyTruckTarget = r["DailyTruckTarget"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["DailyTruckTarget"]),
                    WTDDispatch = r["WTDDispatch"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["WTDDispatch"]),
                    WTDDispatchTarget = r["WTDDispatchTarget"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["WTDDispatchTarget"]),
                    MTDDispatch = r["MTDDispatch"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["MTDDispatch"]),
                    MTDDispatchTarget = r["MTDDispatchTarget"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["MTDDispatchTarget"]),

                    RawMaterialsReceived = r["RawMaterialsReceived"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["RawMaterialsReceived"]),
                    SubRawMaterialsReceived = r["SubRawMaterialsReceived"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["SubRawMaterialsReceived"]),
                    RefractoryMaterialsReceived = r["RefractoryMaterialsReceived"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["RefractoryMaterialsReceived"]),
                    FuelOilReceived = r["FuelOilReceived"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["FuelOilReceived"]),
                    OtherReceived = r["OtherReceived"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["OtherReceived"]),

                    MillScale = r["MillScale"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["MillScale"]),
                    Slag = r["Slag"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Slag"]),
                    Dust = r["Dust"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Dust"]),
                    Sludge = r["Sludge"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["Sludge"]),

                    StatusID = r["StatusID"] == DBNull.Value ? 0 : Convert.ToInt32(r["StatusID"]),
                    CreatedBy = r["CreatedBy"] == DBNull.Value ? "" : Convert.ToString(r["CreatedBy"]),
                    CreatedDate = r["CreatedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["CreatedDate"]),
                    UpdatedBy = r["UpdatedBy"] == DBNull.Value ? "" : Convert.ToString(r["UpdatedBy"]),
                    UpdatedDate = r["UpdatedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["UpdatedDate"])
                };
            }
            catch
            {
                return new SupplyChainDailyBLL();
            }
        }

    }
}
