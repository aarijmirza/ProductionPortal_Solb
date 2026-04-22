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
    public class CCMRespository
    {
        public static DataTable _dt;
        public static DataSet _ds;
        public CCMRespository() : base()
        {
            _dt = new DataTable();
            _ds = new DataSet();
        }

        public List<CCMBLL> GetAllCCMHeat()
        {
            try
            {
                var lst = new List<CCMBLL>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetAllCCMHeat", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<CCMBLL>>();
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
               
        public List<CCMYeildBLL> GetAllCCMBreakdown()
        {
            try
            {
                var lst = new List<CCMYeildBLL>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetAllCCMYeildBreakdown", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<CCMYeildBLL>>();
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public CCMBLL GetCCMHeatByID(int? id)
        {
            var _obj = new CCMBLL();
            DataTable _dt;

            // Assume DBHelper is accessible and correctly instantiated
            var dbHelper = new DBHelper();

            try
            {
                SqlParameter[] p = new SqlParameter[1];
                p[0] = new SqlParameter("@id", id);

                // --- 1. Fetch the main CCM Heat record ---
                // You MUST create a new SP (e.g., sp_GetCCMHeatMain) that returns only ONE row/table
                _dt = dbHelper.GetTableFromSP("sp_GetCCMHeatbyID", p);

                if (_dt != null && _dt.Rows.Count > 0)
                {
                    // Deserialize the single heat record
                    string jsonHeat = Newtonsoft.Json.JsonConvert.SerializeObject(_dt);
                    _obj = JArray.Parse(jsonHeat).ToObject<List<CCMBLL>>().FirstOrDefault();

                    // --- 2. Fetch the list of Chemical Analysis records ---
                    // You MUST create a second SP (e.g., sp_GetCCMAnalysisList) 
                    // that returns only the chemical analysis rows/table

                    // Re-use the parameter
                    DataTable analysisDt = dbHelper.GetTableFromSP("sp_GetCCMAnalysisList", p);

                    if (analysisDt != null && analysisDt.Rows.Count > 0)
                    {
                        // Deserialize the list of Analysis records and attach them
                        string jsonAnalysis = Newtonsoft.Json.JsonConvert.SerializeObject(analysisDt);
                        // Attach the List<CCMChemicalAnalysisBLL> to the main object
                        _obj.Analysis = JArray.Parse(jsonAnalysis).ToObject<List<CCMChemicalAnalysisBLL>>();
                    }
                }
                return _obj;
            }
            catch (Exception ex)
            {
                // Log exception details
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

        public int AddChemicalAnalysis(CCMChemicalAnalysisBLL bll)
        {
            try
            {
                int rtn = 0;
                SqlParameter[] p = new SqlParameter[11];

                p[0] = new SqlParameter("@HeatNo", bll.HeatNo);
                p[1] = new SqlParameter("@Sample", bll.Sample);
                p[2] = new SqlParameter("@C", bll.C);
                p[3] = new SqlParameter("@Si", bll.Si);
                p[4] = new SqlParameter("@Mn", bll.Mn);
                p[5] = new SqlParameter("@P", bll.P);
                p[6] = new SqlParameter("@S", bll.S);
                p[7] = new SqlParameter("@TE", bll.TE);
                p[8] = new SqlParameter("@StatusID", bll.StatusID);
                p[9] = new SqlParameter("@CreatedBy", bll.CreatedBy);
                p[10] = new SqlParameter("@CreatedDate", bll.CreatedDate);

                rtn = (new DBHelper().ExecuteNonQueryReturn)("sp_InsertCCMChemicalAnalysis", p);
                return rtn;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int InsertCCMHeat(CCMBLL bll)
        {
            try
            {
                int rtn = 0;
                SqlParameter[] p = new SqlParameter[40];

                p[0] = new SqlParameter("@Date", bll.Date);
                p[1] = new SqlParameter("@Shift", bll.Shift);
                p[2] = new SqlParameter("@Grade", bll.Grade);
                p[3] = new SqlParameter("@HeatNo", bll.HeatNo);
                p[4] = new SqlParameter("@SequenceHeat", bll.SequenceHeat);
                p[5] = new SqlParameter("@LadleNo", bll.LadleNo);
                p[6] = new SqlParameter("@LadleTemperature", bll.LadleTemperature);
                p[7] = new SqlParameter("@MoltenSteel", bll.MoltenSteel);
                p[8] = new SqlParameter("@TimeOnTurret", bll.TimeOnTurret);
                p[9] = new SqlParameter("@PlateLife", bll.PlateLife);
                p[10] = new SqlParameter("@OpenTime", bll.OpenTime);
                p[11] = new SqlParameter("@CloseTime", bll.CloseTime);
                p[12] = new SqlParameter("@TundishNo", bll.TundishNo);
                p[13] = new SqlParameter("@TundishLife", bll.TundishLife);
                p[14] = new SqlParameter("@ShroudLife", bll.ShroudLife);
                p[15] = new SqlParameter("@Strand1", bll.Strand1);
                p[16] = new SqlParameter("@BilletSize", bll.BilletSize);
                p[17] = new SqlParameter("@BilletLength", bll.BilletLength);
                p[18] = new SqlParameter("@CastingTime", bll.CastingTime);
                p[19] = new SqlParameter("@BilletNumber", bll.BilletNumber);
                p[20] = new SqlParameter("@BilletTotalWeight", bll.BilletTotalWeight);
                p[21] = new SqlParameter("@Productivitytonhr", bll.Productivitytonhr);
                p[22] = new SqlParameter("@Yeild", bll.Yeild);
                p[23] = new SqlParameter("@LF_C", bll.LF_C);
                p[24] = new SqlParameter("@LF_Si", bll.LF_Si);
                p[25] = new SqlParameter("@LF_Mn", bll.LF_Mn);
                p[26] = new SqlParameter("@LF_S", bll.LF_S);
                p[27] = new SqlParameter("@LF_TE", bll.LF_TE);
                p[28] = new SqlParameter("@LF_MnSi", bll.LF_MnSi);
                p[29] = new SqlParameter("@LF_MnS", bll.LF_MnS);
                p[30] = new SqlParameter("@StatusID", bll.StatusID);
                p[31] = new SqlParameter("@CreatedBy", bll.CreatedBy);
                p[32] = new SqlParameter("@CreatedDate", bll.CreatedDate);
                p[33] = new SqlParameter("@SteelTemperature1", bll.SteelTemperature1);
                p[34] = new SqlParameter("@SteelTemperature2", bll.SteelTemperature2);
                p[35] = new SqlParameter("@SteelTemperature3", bll.SteelTemperature3);
                p[36] = new SqlParameter("@SteelTemperature4", bll.SteelTemperature4);
                p[37] = new SqlParameter("@Strand2", bll.Strand2);
                p[38] = new SqlParameter("@Strand3", bll.Strand3);
                p[39] = new SqlParameter("@Strand4", bll.Strand4);

                rtn = (new DBHelper().ExecuteNonQueryReturn)("sp_InsertCCMHeatDetail", p);
                return rtn;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int Delete(string heatno, string UpdatedBy)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[4];
                int i = 0;

                p[i++] = new SqlParameter("@HeatNo", heatno);
                p[i++] = new SqlParameter("@StatusID", 3);
                p[i++] = new SqlParameter("@UpdatedDate", DateTime.Now);
                p[i++] = new SqlParameter("@UpdatedBy", UpdatedBy);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_DeleteCCMData", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int InsertCCMYeild(CCMYeildBLL data)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[10];
                int i = 0;

                p[i++] = new SqlParameter("@Date", data.Date);
                p[i++] = new SqlParameter("@HeatNo", data.HeatNo);
                p[i++] = new SqlParameter("@TundishSkull", data.TundishSkull);
                p[i++] = new SqlParameter("@ProcessRejectedBillet", data.ProcessRejectedBillet);
                p[i++] = new SqlParameter("@ShortBillet6m", data.ShortBillet6m);
                p[i++] = new SqlParameter("@HeadTail", data.HeadTail);
                p[i++] = new SqlParameter("@Comment", data.Comment);
                p[i++] = new SqlParameter("@StatusID", data.StatusID);
                p[i++] = new SqlParameter("@CreatedBy", data.CreatedBy);
                p[i++] = new SqlParameter("@CreatedDate", data.CreatedDate);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_InsertCCMYeildBreakdown", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int UpdateCCMHeat(CCMBLL bll)
        {
            try
            {
                int rtn = 0;
                SqlParameter[] p = new SqlParameter[40];

                p[0] = new SqlParameter("@Date", bll.Date);
                p[1] = new SqlParameter("@Shift", bll.Shift);
                p[2] = new SqlParameter("@Grade", bll.GRADE_ID);
                p[3] = new SqlParameter("@HeatNo", bll.HeatNo);
                p[4] = new SqlParameter("@SequenceHeat", bll.SequenceHeat);
                p[5] = new SqlParameter("@LadleNo", bll.LadleNo);
                p[6] = new SqlParameter("@LadleTemperature", bll.LadleTemperature);
                p[7] = new SqlParameter("@MoltenSteel", bll.MoltenSteel);
                p[8] = new SqlParameter("@TimeOnTurret", bll.TimeOnTurret);
                p[9] = new SqlParameter("@PlateLife", bll.PlateLife);
                p[10] = new SqlParameter("@OpenTime", bll.OpenTime);
                p[11] = new SqlParameter("@CloseTime", bll.CloseTime);
                p[12] = new SqlParameter("@TundishNo", bll.TundishNo);
                p[13] = new SqlParameter("@TundishLife", bll.TundishLife);
                p[14] = new SqlParameter("@ShroudLife", bll.ShroudLife);
                p[15] = new SqlParameter("@Strand1", bll.Strand1);
                p[16] = new SqlParameter("@BilletSize", bll.BilletSize);
                p[17] = new SqlParameter("@BilletLength", bll.BilletLength);
                p[18] = new SqlParameter("@CastingTime", bll.CastingTime);
                p[19] = new SqlParameter("@BilletNumber", bll.BilletNumber);
                p[20] = new SqlParameter("@BilletTotalWeight", bll.BilletTotalWeight);
                p[21] = new SqlParameter("@Productivitytonhr", bll.Productivitytonhr);
                p[22] = new SqlParameter("@Yeild", bll.Yeild);
                p[23] = new SqlParameter("@LF_C", bll.LF_C);
                p[24] = new SqlParameter("@LF_Si", bll.LF_Si);
                p[25] = new SqlParameter("@LF_Mn", bll.LF_Mn);
                p[26] = new SqlParameter("@LF_S", bll.LF_S);
                p[27] = new SqlParameter("@LF_TE", bll.LF_TE);
                p[28] = new SqlParameter("@LF_MnSi", bll.LF_MnSi);
                p[29] = new SqlParameter("@LF_MnS", bll.LF_MnS);
                p[30] = new SqlParameter("@StatusID", bll.StatusID);
                p[31] = new SqlParameter("@UpdateBy", bll.UpdatedBy);
                p[32] = new SqlParameter("@UpdatedDate", bll.UpdatedDate);
                p[33] = new SqlParameter("@SteelTemperature1", bll.SteelTemperature1);
                p[34] = new SqlParameter("@SteelTemperature2", bll.SteelTemperature2);
                p[35] = new SqlParameter("@SteelTemperature3", bll.SteelTemperature3);
                p[36] = new SqlParameter("@SteelTemperature4", bll.SteelTemperature4);
                p[37] = new SqlParameter("@Strand2", bll.Strand2);
                p[38] = new SqlParameter("@Strand3", bll.Strand3);
                p[39] = new SqlParameter("@Strand4", bll.Strand4);

                rtn = (new DBHelper().ExecuteNonQueryReturn)("sp_UpdateCCMHeatDetail", p);
                return rtn;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
