using DAL.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPICode.Helpers;
using static DAL.Models.ViewModel;

namespace BAL.Repositories
{
    public class EAFMeltShopRepository
    {
        public static DataTable _dt;
        public static DataSet _ds;
        public EAFMeltShopRepository() : base()
        {
            _dt = new DataTable();
            _ds = new DataSet();
        }
        public List<EAFMeltShopBLL> GetAllRecord()
        {
            try
            {
                var lst = new List<EAFMeltShopBLL>();
                _dt = (new DBHelper().GetTableFromSP)("sp_GetAllEAFRecord");
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<EAFMeltShopBLL>>();
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public EAFMeltShopVM GetRecordByID(int? id)
        {
            try
            {
                var _obj = new EAFMeltShopVM();
                SqlParameter[] p = new SqlParameter[1];
                p[0] = new SqlParameter("@id", id);

                _dt = (new DBHelper().GetTableFromSP)("sp_GetEAFRecordbyID", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        _obj = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<EAFMeltShopVM>>().FirstOrDefault();
                    }
                }
                return _obj;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public List<GradeBLL> GetAllGrade()
        {
            try
            {
                var lst = new List<GradeBLL>();
                var dt = new OracleDBHelper().GetTableFromSP("SP_GetAllGrades_PP");
                if (dt != null && dt.Rows.Count > 0)
                {
                    lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(dt))
                                .ToObject<List<GradeBLL>>();
                }
                return lst;
            }
            catch (Exception ex)
            {
                // log exception
                return null;
            }
        }
        public List<DelaySectionBLL> GetAllDelays()
        {
            try
            {
                var lst = new List<DelaySectionBLL>();
                var dt = new OracleDBHelper().GetTableFromSP("SP_GetAllDelaysSection_PP");
                if (dt != null && dt.Rows.Count > 0)
                {
                    lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(dt))
                                .ToObject<List<DelaySectionBLL>>();
                }
                return lst;
            }
            catch (Exception ex)
            {
                // log exception
                return null;
            }
        }
        public int Insert(EAFMeltShopBLL data)
        {
            try
            {
                int rtn = 0;
                SqlParameter[] p = new SqlParameter[51];
                p[0] = new SqlParameter("@Date", data.Date);
                p[1] = new SqlParameter("@Shift", data.Shift);
                p[2] = new SqlParameter("@GRADE_ID", data.GRADE_ID);
                p[3] = new SqlParameter("@HeatNo", data.HeatNo);
                p[4] = new SqlParameter("@NoOfHeat", data.NoOfHeat);
                p[5] = new SqlParameter("@SequenceHeat", data.SequenceHeat);
                p[6] = new SqlParameter("@PrevTappTime", data.PrevTappTime);
                p[7] = new SqlParameter("@EAFShellNo", data.EAFShellNo);
                p[8] = new SqlParameter("@HeatStartTime", data.HeatStartTime);
                p[9] = new SqlParameter("@HeatEndTime", data.HeatEndTime);
                p[10] = new SqlParameter("@TaptoTapTime", data.TaptoTapTime);
                p[11] = new SqlParameter("@NetArcingTime", data.NetArcingTime);
                p[12] = new SqlParameter("@TurnAroundTime", data.TurnAroundTime);
                p[13] = new SqlParameter("@LiningLife", data.LiningLife);
                p[14] = new SqlParameter("@BottomLife", data.BottomLife);
                p[15] = new SqlParameter("@EBTLife", data.EBTLife);
                p[16] = new SqlParameter("@DeltaLife", data.DeltaLife);
                p[17] = new SqlParameter("@BucketChargingWeight", data.BucketChargingWeight);
                p[18] = new SqlParameter("@ISACChargeWeight", data.ISACChargeWeight);
                p[19] = new SqlParameter("@LimeISAC", data.LimeISAC);
                p[20] = new SqlParameter("@DoloISAC", data.DoloISAC);
                p[21] = new SqlParameter("@CokeISAC", data.CokeISAC);
                p[22] = new SqlParameter("@ISACFluxWeight", data.ISACFluxWeight);
                p[23] = new SqlParameter("@TotalChargeWeight", data.TotalChargeWeight);
                p[24] = new SqlParameter("@LMTAPWTApp", data.LMTAPWTApp);
                p[25] = new SqlParameter("@TappingTemp", data.TappingTemp);
                p[26] = new SqlParameter("@OpeningCarbon", data.OpeningCarbon);
                p[27] = new SqlParameter("@TappingDuration", data.TappingDuration);
                p[28] = new SqlParameter("@PowerConsumption", data.PowerConsumption);
                p[29] = new SqlParameter("@FeSi", data.FeSi);
                p[30] = new SqlParameter("@SiMn", data.SiMn);
                p[31] = new SqlParameter("@FeMn", data.FeMn);
                p[32] = new SqlParameter("@AlBAR", data.AlBAR);
                p[33] = new SqlParameter("@LIME", data.LIME);
                p[34] = new SqlParameter("@DOLOLIME", data.DOLOLIME);
                p[35] = new SqlParameter("@DOLOMITE", data.DOLOMITE);
                p[36] = new SqlParameter("@INJECTIONCARBON", data.INJECTIONCARBON);
                p[37] = new SqlParameter("@HARD_COKE", data.HARD_COKE);
                p[38] = new SqlParameter("@GUNNING", data.GUNNING);
                p[39] = new SqlParameter("@FETTLING", data.FETTLING);
                p[40] = new SqlParameter("@TEMP_TIPS", data.TEMP_TIPS);
                p[41] = new SqlParameter("@LOLLYPOP_SAMPLER", data.LOLLYPOP_SAMPLER);
                p[42] = new SqlParameter("@TOTAL_OXY", data.TOTAL_OXY);
                p[43] = new SqlParameter("@CELOXO2_PROBE", data.CELOXO2_PROBE);
                p[44] = new SqlParameter("@EBT_FILLER", data.EBT_FILLER);
                p[45] = new SqlParameter("@O2LANCING_PIPE", data.O2LANCING_PIPE);
                p[46] = new SqlParameter("@CERAMIC_COATED_PIPE", data.CERAMIC_COATED_PIPE);
                p[47] = new SqlParameter("@StatusID", data.StatusID);
                p[48] = new SqlParameter("@CreatedBy", data.CreatedBy);
                p[49] = new SqlParameter("@CreatedDate", data.CreatedDate);

                var outputParam = new SqlParameter("@NewRequestID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                p[50] = outputParam;

                new DBHelper().ExecuteNonQuery("sp_InsertEAFData", p);

                int insertedId = (int)outputParam.Value;

                return insertedId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int AddElectrode(ElectrodeBLL bll)
        {
            try
            {
                int rtn = 0;
                SqlParameter[] p = new SqlParameter[10];
                p[0] = new SqlParameter("@PlantID", bll.PlantID);
                p[1] = new SqlParameter("@PlantName", bll.PlantName);
                p[2] = new SqlParameter("@HeatID", bll.HeatID);
                p[3] = new SqlParameter("@Adjusted", bll.Adjusted);
                p[4] = new SqlParameter("@Addition", bll.ElectrodeAddition);
                p[5] = new SqlParameter("@Break", bll.Break);
                p[6] = new SqlParameter("@StubEndLoss", bll.StubEndLoss);
                p[7] = new SqlParameter("@StatusID", bll.StatusID);
                p[8] = new SqlParameter("@CreatedDate", bll.CreatedDate);
                p[9] = new SqlParameter("@CreatedBy", bll.CreatedBy);

                rtn = (new DBHelper().ExecuteNonQueryReturn)("sp_InsertElectrode", p);
                return rtn;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int AddDelays(DelaysBLL bll)
        {
            try
            {
                int rtn = 0;
                SqlParameter[] p = new SqlParameter[11];
                p[0] = new SqlParameter("@PlantID", bll.PlantID);
                p[1] = new SqlParameter("@PlantName", bll.PlantName);
                p[2] = new SqlParameter("@HeatID", bll.HeatID);
                p[3] = new SqlParameter("@StartTime", bll.StartTime);
                p[4] = new SqlParameter("@EndTime", bll.EndTime);
                p[5] = new SqlParameter("@TotalDuration", bll.TotalDuration);
                p[6] = new SqlParameter("@Reason", bll.Reason);
                p[7] = new SqlParameter("@StatusID", bll.StatusID);
                p[8] = new SqlParameter("@CreatedDate", bll.CreatedDate);
                p[9] = new SqlParameter("@CreatedBy", bll.CreatedBy);
                p[10] = new SqlParameter("@HeatNo", bll.HeatNo);

                rtn = (new DBHelper().ExecuteNonQueryReturn)("sp_InsertDelays", p);
                return rtn;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
