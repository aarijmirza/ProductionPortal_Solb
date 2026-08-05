using DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebAPICode.Helpers;

namespace BAL.Repositories
{
    public class MeltshopRepository
    {
        public static DataTable _dt;
        public static DataSet _ds;
        public MeltshopRepository() : base()
        {
            _dt = new DataTable();
            _ds = new DataSet();
        }
        public List<ElectricArcFurnaceBLL> GetAllEAFRecord()
        {
            try
            {
                var lst = new List<ElectricArcFurnaceBLL>();
                _dt = (new DBHelper().GetTableFromSP)("sp_GetAllElectricArcFurnaceRecord");
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<ElectricArcFurnaceBLL>>();
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<ElectricArcFurnaceBLL> GetEAFListByDate(DateTime fromDate, DateTime toInclusive)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[2];
                p[0] = new SqlParameter("@FromDate", fromDate);
                p[1] = new SqlParameter("@ToDate", toInclusive);

                DataTable dt = (new DBHelper().GetTableFromSP)("sp_GetEAF_ByDate", p);

                if (dt != null && dt.Rows.Count > 0)
                    return JArray.Parse(JsonConvert.SerializeObject(dt)).ToObject<List<ElectricArcFurnaceBLL>>();

                return new List<ElectricArcFurnaceBLL>();
            }
            catch
            {
                return new List<ElectricArcFurnaceBLL>();
            }
        }


        public ElectricArcFurnaceBLL GetEAFRecordByID(string heatno)
        {
            try
            {
                var _obj = new ElectricArcFurnaceBLL();
                SqlParameter[] p = new SqlParameter[1];
                p[0] = new SqlParameter("@id", heatno);
                _dt = (new DBHelper().GetTableFromSP)("sp_GetEAFHeatDetailByID", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        _obj = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<ElectricArcFurnaceBLL>>().FirstOrDefault();
                    }
                }
                return _obj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int InsertEAF(ElectricArcFurnaceBLL model)
        {
            try
            {
                int rtn = 0;
                SqlParameter[] p = new SqlParameter[60];

                p[0] = new SqlParameter("@Date", (object)(model.Date));

                p[1] = new SqlParameter("@HeatNo", (object)model.HeatNo ?? DBNull.Value);
                p[2] = new SqlParameter("@Group", (object)model.Group ?? DBNull.Value);
                p[3] = new SqlParameter("@Shift", (object)model.Shift ?? DBNull.Value);
                p[4] = new SqlParameter("@Grade", (object)model.Grade ?? DBNull.Value);

                p[5] = new SqlParameter("@NoofBaskets", (object)model.NoofBaskets ?? DBNull.Value);
                p[6] = new SqlParameter("@Scrap", (object)model.Scrap ?? DBNull.Value);
                p[7] = new SqlParameter("@DRI", (object)model.DRI ?? DBNull.Value);
                p[8] = new SqlParameter("@OldHBIDRI", (object)model.OldHBIDRI ?? DBNull.Value);
                p[9] = new SqlParameter("@HBI", (object)model.HBI ?? DBNull.Value);
                p[10] = new SqlParameter("@PB", (object)model.PB ?? DBNull.Value);
                p[11] = new SqlParameter("@Dumped", (object)model.Dumped ?? DBNull.Value);

                p[12] = new SqlParameter("@Lime", (object)model.Lime ?? DBNull.Value);
                p[13] = new SqlParameter("@Dololime", (object)model.Dololime ?? DBNull.Value);
                p[14] = new SqlParameter("@ChargeCoal", (object)model.ChargeCoal ?? DBNull.Value);
                p[15] = new SqlParameter("@TotalChargeWeight", (object)model.TotalChargeWeight ?? DBNull.Value);
                p[16] = new SqlParameter("@TotalTappingWeight", (object)model.TotalTappingWeight ?? DBNull.Value);

                p[17] = new SqlParameter("@FeMn", (object)model.FeMn ?? DBNull.Value);
                p[18] = new SqlParameter("@FeSi", (object)model.FeSi ?? DBNull.Value);
                p[19] = new SqlParameter("@SiMn", (object)model.SiMn ?? DBNull.Value);
                p[20] = new SqlParameter("@TotalAlloys", (object)model.TotalAlloys ?? DBNull.Value);

                p[21] = new SqlParameter("@LPG", (object)model.LPG ?? DBNull.Value);
                p[22] = new SqlParameter("@Oxygen", (object)model.Oxygen ?? DBNull.Value);
                p[23] = new SqlParameter("@InjCarbon", (object)model.InjCarbon ?? DBNull.Value);
                p[24] = new SqlParameter("@EnergyKWH", (object)model.EnergyKWH ?? DBNull.Value);

                p[25] = new SqlParameter("@TaptoTap", (object)model.TaptoTap ?? DBNull.Value);
                p[26] = new SqlParameter("@TAT", (object)model.TAT ?? DBNull.Value);
                p[27] = new SqlParameter("@Carbon", (object)model.Carbon ?? DBNull.Value);

                p[28] = new SqlParameter("@FlourSpar", (object)model.FlourSpar ?? DBNull.Value);
                p[29] = new SqlParameter("@AluminumLamps", (object)model.AluminumLamps ?? DBNull.Value);
                p[30] = new SqlParameter("@Lime312mm", (object)model.Lime312mm ?? DBNull.Value);
                p[31] = new SqlParameter("@Lime550mm", (object)model.Lime550mm ?? DBNull.Value);
                p[32] = new SqlParameter("@Coke", (object)model.Coke ?? DBNull.Value);
                p[33] = new SqlParameter("@Magnisia", (object)model.Magnisia ?? DBNull.Value);
                p[34] = new SqlParameter("@Electrode", (object)model.Electrode ?? DBNull.Value);

                p[35] = new SqlParameter("@TotalTls", (object)model.TotalTls ?? DBNull.Value);
                p[36] = new SqlParameter("@NetTls", (object)model.NetTls ?? DBNull.Value);

                p[37] = new SqlParameter("@HeatStart", (object)model.HeatStart ?? DBNull.Value);
                p[38] = new SqlParameter("@HeatStop", (object)model.HeatStop ?? DBNull.Value);

                p[39] = new SqlParameter("@Yeild", (object)model.Yeild ?? DBNull.Value); // (spelling same as DB)
                p[40] = new SqlParameter("@PowerOn", (object)model.PowerOn ?? DBNull.Value);
                p[41] = new SqlParameter("@TappingC", (object)model.TappingC ?? DBNull.Value);
                p[42] = new SqlParameter("@TapTemp", (object)model.TapTemp ?? DBNull.Value);

                p[43] = new SqlParameter("@UnscheduleDelay", (object)model.UnscheduleDelay ?? DBNull.Value);
                p[44] = new SqlParameter("@EffectiveDelay", (object)model.EffectiveDelay ?? DBNull.Value);
                p[45] = new SqlParameter("@TotalDelay", (object)model.TotalDelay ?? DBNull.Value);

                p[46] = new SqlParameter("@Hearth", (object)model.Hearth ?? DBNull.Value);
                p[47] = new SqlParameter("@Wall", (object)model.Wall ?? DBNull.Value);
                p[48] = new SqlParameter("@Roof", (object)model.Roof ?? DBNull.Value);
                p[49] = new SqlParameter("@EBT", (object)model.EBT ?? DBNull.Value);
                p[50] = new SqlParameter("@Gunning", (object)model.Gunning ?? DBNull.Value);
                p[51] = new SqlParameter("@Fettling", (object)model.Fettling ?? DBNull.Value);

                p[52] = new SqlParameter("@LadleNo", (object)model.LadleNo ?? DBNull.Value);
                p[53] = new SqlParameter("@LadleLife", (object)model.LadleLife ?? DBNull.Value);

                p[54] = new SqlParameter("@StatusID", (object)model.StatusID ?? DBNull.Value);
                p[55] = new SqlParameter("@CreatedBy", (object)model.CreatedBy ?? DBNull.Value);
                p[56] = new SqlParameter("@CreatedDate", (object)model.CreatedDate ?? DBNull.Value);

                p[57] = new SqlParameter("@SpecEnergy", model.EnergyKWH / model.TotalTappingWeight);
                p[58] = new SqlParameter("@SpecCarbon", model.InjCarbon + model.ChargeCoal);
                p[59] = new SqlParameter("@SpecOxygen", model.Oxygen / model.TotalTappingWeight);


                return (new DBHelper().ExecuteNonQueryReturn)("sp_AddElectricArcFurnace", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public List<LaddleFurnaceBLL> GetAllLFRecord(
            DateTime? fromDate,
            DateTime? toDate)
        {
            try
            {
                var parameters =
                    new[]
                    {
                new SqlParameter(
                    "@FromDate",
                    SqlDbType.Date
                )
                {
                    Value =
                        fromDate.HasValue
                            ? (object)fromDate.Value.Date
                            : DBNull.Value
                },

                new SqlParameter(
                    "@ToDate",
                    SqlDbType.Date
                )
                {
                    Value =
                        toDate.HasValue
                            ? (object)toDate.Value.Date
                            : DBNull.Value
                }
                    };

                DataTable dt =
                    DBHelper.ExecuteDataTable(
                        "sp_GetAllLaddleFurnaceRecord",
                        CommandType.StoredProcedure,
                        parameters
                    );

                if (
                    dt == null ||
                    dt.Rows.Count == 0
                )
                {
                    return new List<LaddleFurnaceBLL>();
                }

                return JArray
                    .Parse(
                        JsonConvert.SerializeObject(
                            dt
                        )
                    )
                    .ToObject<
                        List<LaddleFurnaceBLL>
                    >();
            }
            catch
            {
                return new List<LaddleFurnaceBLL>();
            }
        }
        public LaddleFurnaceBLL GetLFRecordByID(string heatno)
        {
            try
            {
                var _obj = new LaddleFurnaceBLL();
                SqlParameter[] p = new SqlParameter[1];
                p[0] = new SqlParameter("@id", heatno);
                _dt = (new DBHelper().GetTableFromSP)("sp_GetLFHeatDetailByID", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        _obj = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<LaddleFurnaceBLL>>().FirstOrDefault();
                    }
                }
                return _obj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int InsertLF(LaddleFurnaceBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[26];
                int i = 0;

                p[i++] = new SqlParameter("@Date", model.Date ?? DateTime.Now);
                p[i++] = new SqlParameter("@HeatNo", model.HeatNo);
                p[i++] = new SqlParameter("@Grade", model.Grade);
                p[i++] = new SqlParameter("@RecarburizerSilo", (object)model.RecarburizerSilo ?? DBNull.Value);
                p[i++] = new SqlParameter("@RecarburizerBags", (object)model.RecarburizerBags ?? DBNull.Value);
                p[i++] = new SqlParameter("@FeSi", (object)model.FeSi ?? DBNull.Value);
                p[i++] = new SqlParameter("@FeMn", (object)model.FeMn ?? DBNull.Value);
                p[i++] = new SqlParameter("@SiMn", (object)model.SiMn ?? DBNull.Value);
                p[i++] = new SqlParameter("@TotalAlloys", (object)model.TotalAlloys ?? DBNull.Value);
                p[i++] = new SqlParameter("@Lime312mm", (object)model.Lime312mm ?? DBNull.Value);
                p[i++] = new SqlParameter("@Dololime", (object)model.Dololime ?? DBNull.Value);
                p[i++] = new SqlParameter("@MgO", (object)model.MgO ?? DBNull.Value);
                p[i++] = new SqlParameter("@FlourSpar", (object)model.FlourSpar ?? DBNull.Value);
                p[i++] = new SqlParameter("@RiceHusk", (object)model.RiceHusk ?? DBNull.Value);
                p[i++] = new SqlParameter("@SynSlag", (object)model.SynSlag ?? DBNull.Value);
                p[i++] = new SqlParameter("@CaSi", (object)model.CaSi ?? DBNull.Value);
                p[i++] = new SqlParameter("@CafeWire", (object)model.CafeWire ?? DBNull.Value);
                p[i++] = new SqlParameter("@Cac2", (object)model.Cac2 ?? DBNull.Value);
                p[i++] = new SqlParameter("@Electrode", (object)model.Electrode ?? DBNull.Value);
                p[i++] = new SqlParameter("@KWH", (object)model.KWH ?? DBNull.Value);
                p[i++] = new SqlParameter("@Nitrogen", (object)model.Nitrogen ?? DBNull.Value);
                p[i++] = new SqlParameter("@Argon", (object)model.Argon ?? DBNull.Value);
                p[i++] = new SqlParameter("@LMForCasting", (object)model.LMForCasting ?? DBNull.Value);
                p[i++] = new SqlParameter("@StatusID", model.StatusID);
                p[i++] = new SqlParameter("@CreatedBy", model.CreatedBy);
                p[i++] = new SqlParameter("@CreatedDate", model.CreatedDate);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_AddLaddleFurnace", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int DeleteEAFHeat(string heatno, string UpdatedBy)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[4];
                int i = 0;

                p[i++] = new SqlParameter("@HeatNo", heatno);
                p[i++] = new SqlParameter("@StatusID", 3);
                p[i++] = new SqlParameter("@UpdatedDate", DateTime.Now);
                p[i++] = new SqlParameter("@UpdatedBy", UpdatedBy);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_DeleteEAFData", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int DeleteLFHeat(string heatno, string UpdatedBy)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[4];
                int i = 0;

                p[i++] = new SqlParameter("@HeatNo", heatno);
                p[i++] = new SqlParameter("@StatusID", 3);
                p[i++] = new SqlParameter("@UpdatedDate", DateTime.Now);
                p[i++] = new SqlParameter("@UpdatedBy", UpdatedBy);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_DeleteLFData", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public List<LaddleFurnaceBLL> GetLFListByDate(DateTime fromDate, DateTime toInclusive)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[2];
                p[0] = new SqlParameter("@FromDate", fromDate);
                p[1] = new SqlParameter("@ToDate", toInclusive);

                DataTable dt = (new DBHelper().GetTableFromSP)("sp_GetLF_ByDate", p);

                if (dt != null && dt.Rows.Count > 0)
                    return JArray.Parse(JsonConvert.SerializeObject(dt)).ToObject<List<LaddleFurnaceBLL>>();

                return new List<LaddleFurnaceBLL>();
            }
            catch
            {
                return new List<LaddleFurnaceBLL>();
            }
        }

    }
}
