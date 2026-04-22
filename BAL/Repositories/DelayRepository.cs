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

namespace BAL.Repositories
{
    public class DelayRespository
    {
        public static DataTable _dt;
        public static DataSet _ds;
        public DelayRespository() : base()
        {
            _dt = new DataTable();
            _ds = new DataSet();
        }

        public List<PlantDelayBLL> GetAllRMDelay()
        {
            try
            {
                var lst = new List<PlantDelayBLL>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetAllRMDelays", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<PlantDelayBLL>>();
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public List<PlantDelayBLL> GetAllDelay()
        {
            try
            {
                var lst = new List<PlantDelayBLL>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetAllPlantDelays", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<PlantDelayBLL>>();
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<PlantDelayBLL> GetAllAgency()
        {
            try
            {
                var lst = new List<PlantDelayBLL>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetAllDelayAgency");
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<PlantDelayBLL>>();
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<DelayEquipmentBLL> GetAllEquipments()
        {
            try
            {
                var lst = new List<DelayEquipmentBLL>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetSMPDelayEquipments");
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<DelayEquipmentBLL>>();
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<DelayComponentBLL> GetAllComponent()
        {
            try
            {
                var lst = new List<DelayComponentBLL>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetDelayComponents");
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<DelayComponentBLL>>();
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int Insert(PlantDelayBLL data)
        {
            try
            {

                int rtn = 0;
                SqlParameter[] p = new SqlParameter[23];

                TimeSpan duration = (TimeSpan)(data.EndTime - data.StartTime);

                p[0] = new SqlParameter("@Date", data.Date);
                p[1] = new SqlParameter("@Plant", data.Plant);
                p[2] = new SqlParameter("@Area", data.Area);
                p[3] = new SqlParameter("@Shift", data.Shift);
                p[4] = new SqlParameter("@Team", data.Team);
                p[5] = new SqlParameter("@ShiftIncharge", data.ShiftIncharge);
                p[6] = new SqlParameter("@StartTime", data.StartTime);
                p[7] = new SqlParameter("@EndTime", data.EndTime);
                p[8] = new SqlParameter("@TotalDuration", duration.TotalMinutes);
                p[9] = new SqlParameter("@Cobbles", data.Cobble);
                p[10] = new SqlParameter("@HotOut", data.HotOut);
                p[11] = new SqlParameter("@DelayType", data.DelayType);
                p[12] = new SqlParameter("@AgencyName", data.AgencyName);
                p[13] = new SqlParameter("@AgencyCode", data.AgencyCode);
                p[14] = new SqlParameter("@Component", data.Component);
                p[15] = new SqlParameter("@Equipments", data.Equipments);
                p[16] = new SqlParameter("@Reason", data.Reason);
                p[17] = new SqlParameter("@DelayDescription", data.DelayDescription);
                p[18] = new SqlParameter("@ReasonOccurence", data.ReasonForOccurence);
                p[19] = new SqlParameter("@ActionTaken", data.ActionTaken);
                p[20] = new SqlParameter("@StatusID", data.StatusID);
                p[21] = new SqlParameter("@CreatedBy", data.CreatedBy);
                p[22] = new SqlParameter("@CreatedDate", data.CreatedDate);

                rtn = (new DBHelper().ExecuteNonQueryReturn)("sp_InsertDelays", p);
                return rtn;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int Delete(int id, string UpdatedBy)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[4];
                int i = 0;

                p[i++] = new SqlParameter("@id", id);
                p[i++] = new SqlParameter("@StatusID", 3);
                p[i++] = new SqlParameter("@UpdatedDate", DateTime.Now);
                p[i++] = new SqlParameter("@UpdatedBy", UpdatedBy);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_DeleteDelay", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public List<PlantDelayBLL> GetDelayReport(DateTime? startdate, DateTime? enddate)
        {
            try
            {
                var lst = new List<PlantDelayBLL>();

                SqlParameter[] p = new SqlParameter[2];
                p[0] = new SqlParameter("@startdate", startdate);
                p[1] = new SqlParameter("@enddate", enddate);

                _dt = (new DBHelper().GetTableFromSP)("sp_GetDelayReportDatewise", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<PlantDelayBLL>>();
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ShiftEntryBLL GetTodayFirstEntry(DateTime date)
        {
            SqlParameter[] p =
            {
        new SqlParameter("@Date", date.Date)
        };

            DataTable dt = new DBHelper()
                .GetTableFromSP("sp_GetTodayFirstShiftEntry", p);

            if (dt.Rows.Count == 0)
                return null;

            return new ShiftEntryBLL
            {
                Plant = dt.Rows[0]["Plant"].ToString(),
                Shift = dt.Rows[0]["Shift"].ToString(),
                Team = dt.Rows[0]["Team"].ToString(),
                ShiftIncharge = dt.Rows[0]["ShiftIncharge"].ToString()
            };
        }
    }
}
