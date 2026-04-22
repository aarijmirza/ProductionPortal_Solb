using DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Configuration;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebAPICode.Helpers;

namespace BAL.Repositories
{
    public class BoardingRepository
    {
        public static DataTable _dt;
        public static DataSet _ds;
        public BoardingRepository() : base()
        {
            _dt = new DataTable();
            _ds = new DataSet();
        }

        public List<BilletGrades> GetBilletGrade()
        {
            try
            {
                var lst = new List<BilletGrades>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetBilletGrades_PP", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<BilletGrades>>();
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<HeatChemistryBLL> GetChemsitryHeatDetails(string heatno)
        {
            try
            {
                // 1. Initialize the return object as a List
                var _objList = new List<HeatChemistryBLL>();

                // 2. Prepare the parameter array
                SqlParameter[] p = new SqlParameter[1];
                // Note: The parameter name should match the parameter in your stored procedure
                p[0] = new SqlParameter("@id", heatno);

                // 3. Execute the stored procedure using DBHelper
                // NOTE: (new DBHelper().GetTableFromSP) is unusual syntax; ensure GetTableFromSP is correctly called.
                // Assuming it returns a DataTable (_dt)
                _dt = (new DBHelper().GetTableFromSP)("sp_GetChemistryHeatDetail", p);

                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        // 4. CHANGE HERE: Convert the DataTable to a List<ScrapyardBLL>
                        // JArray.Parse().ToObject<List<T>>() is a standard pattern for this conversion
                        _objList = JArray.Parse(JsonConvert.SerializeObject(_dt)).ToObject<List<HeatChemistryBLL>>();
                    }
                }

                // 5. Return the full list (which may be empty if no records were found)
                return _objList;
            }
            catch (Exception ex)
            {
                // In a real application, you should log the exception (ex) here
                return new List<HeatChemistryBLL>(); // Return an empty list on failure
            }
        }

        public int InsertBilletBoarding(RollingMillMasterBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[16];

                var Profile = model.Profile  +  model.Size;

                p[0] = new SqlParameter("@Date", model.Date ?? DateTime.Now);
                p[1] = new SqlParameter("@HeatNo", model.HeatNo);
                p[2] = new SqlParameter("@BilletBoarding", model.BoardingNo);
                p[3] = new SqlParameter("@PlantName", model.Plant);
                p[4] = new SqlParameter("@Shift", model.Shift);
                p[5] = new SqlParameter("@ProductSpecs", model.ProductSpecs);
                p[6] = new SqlParameter("@BilletLength", model.BilletLength);
                p[7] = new SqlParameter("@CrossSection", model.CrossSection);
                p[8] = new SqlParameter("@Grade", model.SteelGrade);
                p[9] = new SqlParameter("@Size", model.Size);
                p[10] = new SqlParameter("@Remarks", model.Remarks);
                p[11] = new SqlParameter("@StatusID", model.StatusID);
                p[12] = new SqlParameter("@CreatedBy", model.CreatedBy);
                p[13] = new SqlParameter("@CreatedDate", model.CreatedDate);
                p[14] = new SqlParameter("@BilletWeight", model.BilletWeight);
                p[15] = new SqlParameter("@Profile", Profile);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_AddBilletBoarding", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int InsertChemicalAnalysisRM(RMChemicalAnalysisBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[13];

                p[0] = new SqlParameter("@HeatNo", model.HeatNo);
                p[1] = new SqlParameter("@NoOfBillets", model.NoOfBillets);
                p[2] = new SqlParameter("@C", model.C);
                p[3] = new SqlParameter("@Si", model.Si);
                p[4] = new SqlParameter("@Mn", model.Mn);
                p[5] = new SqlParameter("@S", model.S);
                p[6] = new SqlParameter("@P", model.P);
                p[7] = new SqlParameter("@N", model.N);
                p[8] = new SqlParameter("@Ceq", model.Ceq);
                p[9] = new SqlParameter("@HeatStatus", model.HeatStatus);
                p[10] = new SqlParameter("@StatusID", model.StatusID);
                p[11] = new SqlParameter("@CreatedBy", model.CreatedBy);
                p[12] = new SqlParameter("@CreatedDate", model.CreatedDate);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_AddRMChemicalAnalysis", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public List<BilletBoardBLL> GetAllBoarding()
        {
            try
            {
                var lst = new List<BilletBoardBLL>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetAllBilletBoarding", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<BilletBoardBLL>>();
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public List<HeatChemistryBLL> GetAllChemistry()
        {
            try
            {
                var lst = new List<HeatChemistryBLL>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetAllHeatChemistry", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<HeatChemistryBLL>>();
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
