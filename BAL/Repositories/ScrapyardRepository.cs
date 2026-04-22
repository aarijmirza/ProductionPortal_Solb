using DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class ScrapyardRepository
    {
        public static DataTable _dt;
        public static DataSet _ds;
        public ScrapyardRepository() : base()
        {
            _dt = new DataTable();
            _ds = new DataSet();
        }

        public List<ScrapyardBLL> GetAll()
        {
            try
            {
                var lst = new List<ScrapyardBLL>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetAllScrap_PP", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<ScrapyardBLL>>();
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int AddScrapyard(ScrapyardBLL bll)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[16];

                p[0] = new SqlParameter("@Date", bll.Date ?? DateTime.Now);
                p[1] = new SqlParameter("@Time", bll.Time);
                p[2] = new SqlParameter("@HeatNo", bll.HeatNo ?? "");

                p[3] = new SqlParameter("@Bucket", bll.Bucket ?? "");
                p[4] = new SqlParameter("@LightScrap", bll.LightScrap ?? 0);
                p[5] = new SqlParameter("@HMS", bll.HMS ?? 0);
                p[6] = new SqlParameter("@ReturnMetal", bll.ReturnMetal ?? 0);
                p[7] = new SqlParameter("@ReturnBar", bll.ReturnBar ?? 0);
                p[8] = new SqlParameter("@MetalSkull", bll.MetalSkull ?? 0);
                p[9] = new SqlParameter("@DRI", bll.DRI ?? 0);
                p[10] = new SqlParameter("@Coal", bll.Coal ?? 0);
                p[11] = new SqlParameter("@Lime", bll.Lime ?? 0);
                p[12] = new SqlParameter("@Dololime", bll.Dololime ?? 0);

                p[13] = new SqlParameter("@StatusID", bll.StatusID ?? 1);
                p[14] = new SqlParameter("@CreatedDate", bll.CreatedDate ?? DateTime.Now);
                p[15] = new SqlParameter("@CreatedBy", bll.CreatedBy ?? "System");

                return (new DBHelper().ExecuteNonQueryReturn)("sp_AddScrapyard", p);
            }
            catch
            {
                return 0;
            }
        }

        public List<ScrapyardBLL> GetScrapHeatDetails(string heatno)
        {
            try
            {
                // 1. Initialize the return object as a List
                var _objList = new List<ScrapyardBLL>();

                // 2. Prepare the parameter array
                SqlParameter[] p = new SqlParameter[1];
                // Note: The parameter name should match the parameter in your stored procedure
                p[0] = new SqlParameter("@id", heatno);

                // 3. Execute the stored procedure using DBHelper
                // NOTE: (new DBHelper().GetTableFromSP) is unusual syntax; ensure GetTableFromSP is correctly called.
                // Assuming it returns a DataTable (_dt)
                _dt = (new DBHelper().GetTableFromSP)("sp_GetScrapHeatDetail", p);

                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        // 4. CHANGE HERE: Convert the DataTable to a List<ScrapyardBLL>
                        // JArray.Parse().ToObject<List<T>>() is a standard pattern for this conversion
                        _objList = JArray.Parse(JsonConvert.SerializeObject(_dt)).ToObject<List<ScrapyardBLL>>();
                    }
                }

                // 5. Return the full list (which may be empty if no records were found)
                return _objList;
            }
            catch (Exception ex)
            {
                // In a real application, you should log the exception (ex) here
                return new List<ScrapyardBLL>(); // Return an empty list on failure
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

                return (new DBHelper().ExecuteNonQueryReturn)("sp_DeleteScrapData", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
