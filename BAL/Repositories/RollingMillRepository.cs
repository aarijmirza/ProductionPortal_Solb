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
    public class RollingMillRepository
    {
        public static DataTable _dt;
        public static DataSet _ds;
        public RollingMillRepository() : base()
        {
            _dt = new DataTable();
            _ds = new DataSet();
        }

        public List<BilletDischargingBLL> GetDichargedHeat()
        {
            try
            {
                var lst = new List<BilletDischargingBLL>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetDichargedHeat", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<BilletDischargingBLL>>();
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<BundlingSectionBLL> GetBundlesHeats()
        {
            try
            {
                var lst = new List<BundlingSectionBLL>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetBundleSectionHeat", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<BundlingSectionBLL>>();
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<BilletChargingBLL> GetAllBoardingToday()
        {
            DateTime today = DateTime.Today;

            var dt = new DBHelper().GetTableFromQuery(@"
                SELECT *
                FROM BilletBoarding
                WHERE CAST(CreatedDate AS DATE) = @Today",
                new SqlParameter("@Today", today)
            );

            return JArray.Parse(JsonConvert.SerializeObject(dt))
                         .ToObject<List<BilletChargingBLL>>();
        }



        public bool IsHeatOnCharging(string heatNo)
        {
            SqlParameter[] p = new SqlParameter[1];
            p[0] = new SqlParameter("@HeatNo", heatNo);

            DataTable dt = new DBHelper().GetTableFromSP("sp_IsHeatOnCharging", p);

            if (dt != null && dt.Rows.Count > 0)
                return Convert.ToInt32(dt.Rows[0]["IsCharging"]) == 1;

            return false;
        }

        public int InsertBilletCharging(BilletChargingBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[15];

                p[0] = new SqlParameter("@Date", model.Date);
                p[1] = new SqlParameter("@Shift", model.Shift);
                p[2] = new SqlParameter("@HeatNo", model.HeatNo);
                p[3] = new SqlParameter("@BoardingNo", model.BoardingNo);
                p[4] = new SqlParameter("@SteelGrade", model.SteelGrade);
                p[5] = new SqlParameter("@BilletSize", model.BilletSize);
                p[6] = new SqlParameter("@BilletLength", model.BilletLength);
                p[7] = new SqlParameter("@Weight", model.Weight);
                p[8] = new SqlParameter("@TotalBillet", model.TotalBillet);
                p[9] = new SqlParameter("@TotalWeight", model.TotalWeight);
                p[10] = new SqlParameter("@HeatSequence", model.HeatSequence);
                p[11] = new SqlParameter("@HeatStatus", 102);
                p[12] = new SqlParameter("@StatusID", model.StatusID);
                p[13] = new SqlParameter("@CreatedBy", model.CreatedBy);
                p[14] = new SqlParameter("@CreatedDate", model.CreatedDate);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_InsertBilletCharging", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public BilletDischargingBLL GetDichargedByHeatNo(string heatno)
        {
            try
            {
                var _obj = new BilletDischargingBLL();
                SqlParameter[] p = new SqlParameter[1];
                p[0] = new SqlParameter("@id", heatno);
                _dt = (new DBHelper().GetTableFromSP)("sp_GetChargedHeatByHeatNo", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        _obj = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<BilletDischargingBLL>>().FirstOrDefault();
                    }
                }
                return _obj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int InsertDischarging(BilletDischargingBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[17];

                p[0] = new SqlParameter("@Date", model.Date);
                p[1] = new SqlParameter("@Shift", model.Shift);
                p[2] = new SqlParameter("@HeatNo", model.HeatNo);
                p[3] = new SqlParameter("@BoardingNo", model.BoardingNo);
                p[4] = new SqlParameter("@SteelGrade", model.SteelGrade);
                p[5] = new SqlParameter("@NewSteelGrade", model.NewSteelGrade);
                p[6] = new SqlParameter("@ProductCode", model.ProductCode);
                p[7] = new SqlParameter("@PONumber", model.PONumber);
                p[8] = new SqlParameter("@Cobble", model.Cobble);
                p[9] = new SqlParameter("@HotOut", model.HotOut);
                p[10] = new SqlParameter("@TotalBillet", model.TotalBillet);
                p[11] = new SqlParameter("@TotalWeight", model.TotalWeight);
                p[12] = new SqlParameter("@DischargingSequence", model.DischargingSequence);
                p[13] = new SqlParameter("@HeatStatus", 103);
                p[14] = new SqlParameter("@StatusID", model.StatusID);
                p[15] = new SqlParameter("@CreatedBy", model.CreatedBy);
                p[16] = new SqlParameter("@CreatedOn", model.CreatedOn);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_InsertBilletDischarging", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int AddRMShiftDetails(RMShiftDetailsBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[8];

                p[0] = new SqlParameter("@Date", model.Date);
                p[1] = new SqlParameter("@Plant", model.Plant);
                p[2] = new SqlParameter("@Shift", model.Shift);
                p[3] = new SqlParameter("@Team", model.Team);
                p[4] = new SqlParameter("@ShiftIncharge", model.ShiftIncharge);
                p[5] = new SqlParameter("@StatusID", model.StatusID);
                p[6] = new SqlParameter("@CreatedBy", model.CreatedBy);
                p[7] = new SqlParameter("@CreatedDate", model.CreatedDate);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_AddRMShiftDetails", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int AddBundlingSection(BundlingSectionBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[15];

                p[0] = new SqlParameter("@HeatNo", model.HeatNo);
                p[1] = new SqlParameter("@BoardingNo", model.BilletBoardingNo);
                p[2] = new SqlParameter("@SteelGrade", model.SteelGrade);
                p[3] = new SqlParameter("@PONumber", model.PONumber);

                p[4] = new SqlParameter("@TotalBundleProduced", model.TotalBundleProduced);
                p[5] = new SqlParameter("@PerCoilWeight", model.PerCoilWeight);
                p[6] = new SqlParameter("@TheoreticalWeight", model.TheoriticalWeight);

                p[7] = new SqlParameter("@Remarks", model.Remarks ?? "");

                p[8] = new SqlParameter("@IsPOComplete", model.IsPOComplete);

                if (model.IsPOComplete == true)
                {
                    p[9] = new SqlParameter("@HeatStatus", 100);
                    p[10] = new SqlParameter("@StatusID", 2);
                }
                else
                {
                    p[9] = new SqlParameter("@HeatStatus", 104);
                    p[10] = new SqlParameter("@StatusID", model.StatusID);
                }

                p[11] = new SqlParameter("@CreatedBy", model.CreatedBy);
                p[12] = new SqlParameter("@CreatedDate", model.CreatedDate);
                p[13] = new SqlParameter("@Date", model.Date);
                p[14] = new SqlParameter("@Shift", model.Shift);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_InsertBundleSection", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public List<RMShiftDetailsBLL> RollingMillDetails()
        {
            try
            {
                var lst = new List<RMShiftDetailsBLL>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetRMShiftDetails", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<RMShiftDetailsBLL>>();
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
