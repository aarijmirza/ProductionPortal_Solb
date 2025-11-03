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
    public class LFMeltShopRepository
    {
        public static DataTable _dt;
        public static DataSet _ds;
        public LFMeltShopRepository() : base()
        {
            _dt = new DataTable();
            _ds = new DataSet();
        }

        public List<LFMeltShopBLL> GetAllRecord()
        {
            try
            {
                var lst = new List<LFMeltShopBLL>();
                _dt = (new DBHelper().GetTableFromSP)("sp_GetAllLFRecord");
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<LFMeltShopBLL>>();
                    }
                }
                return lst;
            }
            catch (Exception ex)
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
        public int InsertLF(LFMeltShopBLL data)
        {
            try
            {
                int rtn = 0;
                SqlParameter[] p = new SqlParameter[51];
                p[0] = new SqlParameter("@Date", data.Date);
                p[1] = new SqlParameter("@Shift", data.Shift);
                p[2] = new SqlParameter("@Grade", data.Grade);
                p[3] = new SqlParameter("@HeatNo", data.HeatNo);
                p[4] = new SqlParameter("@NoOfHeat", data.NoOfHeat);
                p[5] = new SqlParameter("@SequenceHeat", data.SequenceHeat);
                p[6] = new SqlParameter("@TemperatureC", data.TemperatureC);
                p[7] = new SqlParameter("@LMWeightApp", data.LMWeightApp);
                p[8] = new SqlParameter("@CarryOverSlag", data.CarryOverSlag);
                p[9] = new SqlParameter("@FreeBoard", data.FreeBoard);
                p[10] = new SqlParameter("@HeatStartTime", data.HeatStartTime);
                p[11] = new SqlParameter("@HeatEndTime", data.HeatEndTime);
                p[12] = new SqlParameter("@ArcingTime", data.ArcingTime);
                p[13] = new SqlParameter("@PowerConsumption", data.PowerConsumption);
                p[14] = new SqlParameter("@DeltaLife", data.DeltaLife);
                p[15] = new SqlParameter("@LadleNo", data.LadleNo);
                p[16] = new SqlParameter("@Ladlelife", data.Ladlelife);
                p[17] = new SqlParameter("@Returned", data.Returned);
                p[18] = new SqlParameter("@Preheated", data.Preheated);
                p[19] = new SqlParameter("@Slagzone", data.Slagzone);
                p[20] = new SqlParameter("@Purging", data.Purging);
                p[21] = new SqlParameter("@LadleTemperature", data.LadleTemperature);
                p[22] = new SqlParameter("@EAFCPC", data.EAFCPC);
                p[23] = new SqlParameter("@EAFFeSI", data.EAFFeSI);
                p[24] = new SqlParameter("@EAFSiMn", data.EAFSiMn);
                p[25] = new SqlParameter("@EAFFeMn", data.EAFFeMn);
                p[26] = new SqlParameter("@EAFAlBar", data.EAFAlBar);
                p[27] = new SqlParameter("@EAFLime", data.EAFLime);
                p[28] = new SqlParameter("@LFCPC", data.LFCPC);
                p[29] = new SqlParameter("@LFGCrush", data.LFGCrush);
                p[30] = new SqlParameter("@LFInjectedCoke", data.LFInjectedCoke);
                p[31] = new SqlParameter("@LFDOLOLIME", data.LFDOLOLIME);
                p[32] = new SqlParameter("@LFDOLOMITE", data.LFDOLOMITE);
                p[33] = new SqlParameter("@LFCaF2", data.LFCaF2);
                p[34] = new SqlParameter("@LFRiceHusk", data.LFRiceHusk);
                p[35] = new SqlParameter("@LFArgon", data.LFArgon);
                p[36] = new SqlParameter("@TT900MMImport", data.TT900MMImport);
                p[37] = new SqlParameter("@TT900MMLocal", data.TT900MMLocal);
                p[38] = new SqlParameter("@TT1200MM", data.TT1200MM);
                p[39] = new SqlParameter("@CeloxO2Probe", data.CeloxO2Probe);
                p[40] = new SqlParameter("@StatusID", data.StatusID);
                p[41] = new SqlParameter("@CreatedDate", data.CreatedDate);
                p[42] = new SqlParameter("@CreatedBy", data.CreatedBy);

                var outputParam = new SqlParameter("@NewRequestID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                p[43] = outputParam;

                new DBHelper().ExecuteNonQuery("sp_InsertLFData", p);

                int insertedId = (int)outputParam.Value;

                return insertedId;
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