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
    public class DelayRespository
    {
        public static DataTable _dt;
        public static DataSet _ds;
        public DelayRespository() : base()
        {
            _dt = new DataTable();
            _ds = new DataSet();
        }

        public List<PlantDelayBLL> GetAllRMDelay(DateTime startDate, DateTime endDate, string shift)
        {
            try
            {
                var lst = new List<PlantDelayBLL>();

                SqlParameter[] p = new SqlParameter[]
                {
                    new SqlParameter("@StartDate", startDate),
                    new SqlParameter("@EndDate", endDate),
                    new SqlParameter("@Shift", shift)
                };

                _dt = (new DBHelper().GetTableFromSP)("sp_GetAllRMDelays", p);

                if (_dt != null && _dt.Rows.Count > 0)
                {
                    lst = JArray.Parse(JsonConvert.SerializeObject(_dt))
                                .ToObject<List<PlantDelayBLL>>();
                }

                return lst;
            }
            catch
            {
                return new List<PlantDelayBLL>();
            }
        }

        public List<PlantDelayBLL> GetMaintenanceRecords(
            DateTime fromDate,
            DateTime toDate,
            string plant,
            string delayType,
            string agency,
            bool failureAnalysisOnly)
        {
            try
            {
                var parameters =
                    new SqlParameter[]
                    {
                new SqlParameter(
                    "@FromDate",
                    SqlDbType.Date
                )
                {
                    Value = fromDate.Date
                },

                new SqlParameter(
                    "@ToDate",
                    SqlDbType.Date
                )
                {
                    Value = toDate.Date
                },

                new SqlParameter(
                    "@Plant",
                    SqlDbType.NVarChar,
                    100
                )
                {
                    Value =
                        string.IsNullOrWhiteSpace(plant)
                            ? (object)DBNull.Value
                            : plant.Trim()
                },

                new SqlParameter(
                    "@DelayType",
                    SqlDbType.NVarChar,
                    50
                )
                {
                    Value =
                        string.IsNullOrWhiteSpace(delayType)
                            ? "Unscheduled"
                            : delayType.Trim()
                },

                new SqlParameter(
                    "@Agency",
                    SqlDbType.NVarChar,
                    100
                )
                {
                    Value =
                        string.IsNullOrWhiteSpace(agency)
                            ? (object)DBNull.Value
                            : agency.Trim()
                },

                new SqlParameter(
                    "@FailureAnalysisOnly",
                    SqlDbType.Bit
                )
                {
                    Value = failureAnalysisOnly
                }
                    };

                DataTable dt =
                    new DBHelper().GetTableFromSP(
                        "sp_GetMaintenanceRecords",
                        parameters
                    );

                var list =
                    new List<PlantDelayBLL>();

                if (dt == null)
                {
                    throw new Exception(
                        "Stored procedure returned null DataTable."
                    );
                }

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(
                        new PlantDelayBLL
                        {
                            ID = GetInt(row, "ID"),

                            AnalysisCode =
                                GetString(
                                    row,
                                    "FailureAnalysisID"
                                ),

                            Delaycode =
                                GetString(
                                    row,
                                    "Delaycode"
                                ),

                            Date =
                                GetNullableDateTime(
                                    row,
                                    "Date"
                                ),

                            Plant =
                                GetString(
                                    row,
                                    "Plant"
                                ),

                            ProductSize =
                                GetString(
                                    row,
                                    "ProductSize"
                                ),

                            Area =
                                GetString(
                                    row,
                                    "Area"
                                ),

                            Shift =
                                GetString(
                                    row,
                                    "Shift"
                                ),

                            Team =
                                GetString(
                                    row,
                                    "Team"
                                ),

                            ShiftIncharge =
                                GetString(
                                    row,
                                    "ShiftIncharge"
                                ),

                            DelayType =
                                GetString(
                                    row,
                                    "DelayType"
                                ),

                            StartTime =
                                GetNullableTimeSpan(
                                    row,
                                    "StartTime"
                                ),

                            EndTime =
                                GetNullableTimeSpan(
                                    row,
                                    "EndTime"
                                ),

                            TotalDuration =
                                GetInt(
                                    row,
                                    "TotalDuration"
                                ),

                            AgencyName =
                                GetString(
                                    row,
                                    "AgencyName"
                                ),

                            AgencyCode =
                                GetString(
                                    row,
                                    "AgencyCode"
                                ),

                            Equipments =
                                GetString(
                                    row,
                                    "Equipments"
                                ),

                            Component =
                                GetString(
                                    row,
                                    "Component"
                                ),

                            Reason =
                                GetString(
                                    row,
                                    "Reason"
                                ),

                            DelayDescription =
                                GetString(
                                    row,
                                    "DelayDescription"
                                ),

                            ReasonForOccurence =
                                GetString(
                                    row,
                                    "ReasonForOccurence"
                                ),

                            ActionTaken =
                                GetString(
                                    row,
                                    "ActionTaken"
                                ),

                            LastPMDate =
                                GetNullableDateTime(
                                    row,
                                    "LastPMDate"
                                ),

                            FailureReportStatus =
                                GetString(
                                    row,
                                    "FailureReportStatus"
                                ),

                            IncreaseMTBF =
                                GetString(
                                    row,
                                    "IncreaseMTBF"
                                ),

                            DecreaseMTTR =
                                GetString(
                                    row,
                                    "DecreaseMTTR"
                                ),

                            SAPBreakdownOrder =
                                GetString(
                                    row,
                                    "SAPBreakdownOrder"
                                ),

                            FailureCategory1Component =
                                GetString(
                                    row,
                                    "FailureCategory1Component"
                                ),

                            FailureCategory2RootCause =
                                GetString(
                                    row,
                                    "FailureCategory2RootCause"
                                )
                        }
                    );
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "GetMaintenanceRecords Error: " +
                    ex.Message,
                    ex
                );
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

        public List<DelayEquipmentBLL> GetAllRMEquipments()
        {
            try
            {
                var lst = new List<DelayEquipmentBLL>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetRMDelayEquipments");
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
                SqlParameter[] p = new SqlParameter[24];

                TimeSpan duration = (TimeSpan)(data.EndTime - data.StartTime);

                // ✅ Midnight crossing fix
                // Example: 11:30 PM to 12:30 AM = 60 minutes
                if (duration.TotalMinutes < 0)
                {
                    duration = duration.Add(TimeSpan.FromDays(1));
                }

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
                p[23] = new SqlParameter("@Delaycode", data.Delaycode);

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

        public int InsertFailureAnalysis(FailureAnalysisBLL data)
        {
            try
            {
                int rtn = 0;

                SqlParameter[] p = new SqlParameter[11];

                p[0] = new SqlParameter("@DelayID", data.DelayID ?? (object)DBNull.Value);
                p[1] = new SqlParameter("@LastPMDate", data.LastPMDate ?? (object)DBNull.Value);
                p[2] = new SqlParameter("@FailureReportStatus", data.FailureReportStatus ?? (object)DBNull.Value);
                p[3] = new SqlParameter("@IncreaseMTBF", data.IncreaseMTBF ?? (object)DBNull.Value);
                p[4] = new SqlParameter("@DecreaseMTTR", data.DecreaseMTTR ?? (object)DBNull.Value);
                p[5] = new SqlParameter("@SAPBreakdownOrder", data.SAPBreakdownOrder ?? (object)DBNull.Value);
                p[6] = new SqlParameter("@FailureCategory1Component", data.FailureCategory1Component ?? (object)DBNull.Value);
                p[7] = new SqlParameter("@FailureCategory2RootCause", data.FailureCategory2RootCause ?? (object)DBNull.Value);
                p[8] = new SqlParameter("@StatusID", data.StatusID ?? (object)DBNull.Value);
                p[9] = new SqlParameter("@CreatedBy", data.CreatedBy ?? (object)DBNull.Value);
                p[10] = new SqlParameter("@CreatedDate", data.CreatedDate ?? (object)DBNull.Value);

                rtn = (new DBHelper().ExecuteNonQueryReturn)("sp_InsertFailureAnalysis", p);

                return rtn;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public List<DelayEquipmentBLL> GetEquipmentByArea(int areaId)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[1];
                p[0] = new SqlParameter("@PlantArea", areaId);

                DataTable dt = new DBHelper().GetTableFromSP("sp_GetSMPEquipmentByArea", p);

                List<DelayEquipmentBLL> list = new List<DelayEquipmentBLL>();

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new DelayEquipmentBLL
                    {
                        ID = Convert.ToInt32(row["ID"]),
                        Description = Convert.ToString(row["Description"])
                    });
                }

                return list;
            }
            catch
            {
                return new List<DelayEquipmentBLL>();
            }
        }

        public PlantDelayBLL GetDelayByID(int id)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[1];
                p[0] = new SqlParameter("@ID", id);

                DataTable dt = new DBHelper().GetTableFromSP("sp_GetSMPDelayByID", p);

                if (dt == null || dt.Rows.Count == 0)
                    return null;

                DataRow row = dt.Rows[0];

                PlantDelayBLL model = new PlantDelayBLL
                {
                    ID = Convert.ToInt32(row["ID"]),

                    Date = row["Date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["Date"]),

                    Area = row["Area"] == DBNull.Value ? "" : Convert.ToString(row["Area"]),
                    Plant = row["Plant"] == DBNull.Value ? "" : Convert.ToString(row["Plant"]),
                    Delaycode = row["Delaycode"] == DBNull.Value ? "" : Convert.ToString(row["Delaycode"]),
                    Shift = row["Shift"] == DBNull.Value ? "" : Convert.ToString(row["Shift"]),
                    Team = row["Team"] == DBNull.Value ? "" : Convert.ToString(row["Team"]),
                    ShiftIncharge = row["ShiftIncharge"] == DBNull.Value ? "" : Convert.ToString(row["ShiftIncharge"]),

                    StartTime = row["StartTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)row["StartTime"],
                    EndTime = row["EndTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)row["EndTime"],

                    TotalDuration = row["TotalDuration"] == DBNull.Value ? 0 : Convert.ToInt32(row["TotalDuration"]),
                    Cobble = row["Cobble"] == DBNull.Value ? 0 : Convert.ToInt32(row["Cobble"]),
                    HotOut = row["HotOut"] == DBNull.Value ? 0 : Convert.ToInt32(row["HotOut"]),

                    DelayType = row["DelayType"] == DBNull.Value ? "" : Convert.ToString(row["DelayType"]),
                    AgencyName = row["AgencyName"] == DBNull.Value ? "" : Convert.ToString(row["AgencyName"]),
                    AgencyCode = row["AgencyCode"] == DBNull.Value ? "" : Convert.ToString(row["AgencyCode"]),
                    Component = row["Component"] == DBNull.Value ? "" : Convert.ToString(row["Component"]),
                    Equipments = row["Equipments"] == DBNull.Value ? "" : Convert.ToString(row["Equipments"]),

                    Reason = row["Reason"] == DBNull.Value ? "" : Convert.ToString(row["Reason"]),
                    DelayDescription = row["DelayDescription"] == DBNull.Value ? "" : Convert.ToString(row["DelayDescription"]),
                    ReasonForOccurence = row["ReasonForOccurence"] == DBNull.Value ? "" : Convert.ToString(row["ReasonForOccurence"]),
                    ActionTaken = row["ActionTaken"] == DBNull.Value ? "" : Convert.ToString(row["ActionTaken"]),

                    DelayDescription1 = row["DelayDescription1"] == DBNull.Value ? "" : Convert.ToString(row["DelayDescription1"]),
                    ReasonForOccurence1 = row["ReasonForOccurence1"] == DBNull.Value ? "" : Convert.ToString(row["ReasonForOccurence1"]),
                    ActionTaken1 = row["ActionTaken1"] == DBNull.Value ? "" : Convert.ToString(row["ActionTaken1"]),

                    StatusID = row["StatusID"] == DBNull.Value ? 0 : Convert.ToInt32(row["StatusID"]),
                    CreatedBy = row["CreatedBy"] == DBNull.Value ? "" : Convert.ToString(row["CreatedBy"]),
                    CreatedDate = row["CreatedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["CreatedDate"])
                };

                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string GenerateAnalysisCode()
        {
            try
            {
                DataTable dt = new DBHelper().GetTableFromQuery("sp_GenerateMaintenanceAnalysisCode", null);

                if (dt == null || dt.Rows.Count == 0)
                {
                    return "SS-" + DateTime.Now.ToString("yyyy") + "-0001";
                }

                return Convert.ToString(dt.Rows[0]["AnalysisCode"]);
            }
            catch
            {
                return "SS-" + DateTime.Now.ToString("yyyy") + "-0001";
            }
        }

        public int InsertMaintenanceAnalysis(FailureAnalysisBLL model)
        {
            try
            {
                model.AnalysisCode = GenerateAnalysisCode();

                SqlParameter[] p = new SqlParameter[14];

                p[0] = new SqlParameter("@DelayID", model.DelayID);

                p[1] = new SqlParameter("@AnalysisCode",
                    string.IsNullOrWhiteSpace(model.AnalysisCode) ? (object)DBNull.Value : model.AnalysisCode);

                p[2] = new SqlParameter("@LastPMDate",
                    model.LastPMDate.HasValue ? (object)model.LastPMDate.Value : DBNull.Value);

                p[3] = new SqlParameter("@FailureReportStatus",
                    string.IsNullOrWhiteSpace(model.FailureReportStatus) ? (object)DBNull.Value : model.FailureReportStatus);

                p[4] = new SqlParameter("@IncreaseMTBF",
                    string.IsNullOrWhiteSpace(model.IncreaseMTBF) ? (object)DBNull.Value : model.IncreaseMTBF);

                p[5] = new SqlParameter("@IncreaseMTBF1",
                    string.IsNullOrWhiteSpace(model.IncreaseMTBF1) ? (object)DBNull.Value : model.IncreaseMTBF1);

                p[6] = new SqlParameter("@DecreaseMTTR",
                    string.IsNullOrWhiteSpace(model.DecreaseMTTR) ? (object)DBNull.Value : model.DecreaseMTTR);

                p[7] = new SqlParameter("@DecreaseMTTR1",
                    string.IsNullOrWhiteSpace(model.DecreaseMTTR1) ? (object)DBNull.Value : model.DecreaseMTTR1);

                p[8] = new SqlParameter("@SAPBreakdownOrder",
                    string.IsNullOrWhiteSpace(model.SAPBreakdownOrder) ? (object)DBNull.Value : model.SAPBreakdownOrder);

                p[9] = new SqlParameter("@FailureCategory1Component",
                    string.IsNullOrWhiteSpace(model.FailureCategory1Component) ? (object)DBNull.Value : model.FailureCategory1Component);

                p[10] = new SqlParameter("@FailureCategory2RootCause",
                    string.IsNullOrWhiteSpace(model.FailureCategory2RootCause) ? (object)DBNull.Value : model.FailureCategory2RootCause);

                p[11] = new SqlParameter("@StatusID", model.StatusID);

                p[12] = new SqlParameter("@CreatedBy",
                    string.IsNullOrWhiteSpace(model.CreatedBy) ? (object)DBNull.Value : model.CreatedBy);

                p[13] = new SqlParameter("@CreatedDate",
                    model.CreatedDate.HasValue ? (object)model.CreatedDate.Value : DateTime.Now);

                return new DBHelper().ExecuteNonQueryReturn("sp_InsertMaintenanceAnalysis", p);
            }
            catch
            {
                return 0;
            }
        }

        public FailureAnalysisBLL GetMaintenanceAnalysisByDelayID(int delayID)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[1];
                p[0] = new SqlParameter("@DelayID", delayID);

                DataTable dt = new DBHelper().GetTableFromSP("sp_GetMaintenanceAnalysisByDelayID", p);

                if (dt == null || dt.Rows.Count == 0)
                    return null;

                DataRow row = dt.Rows[0];

                FailureAnalysisBLL model = new FailureAnalysisBLL
                {
                    ID = row["ID"] == DBNull.Value ? 0 : Convert.ToInt32(row["ID"]),
                    DelayID = row["DelayID"] == DBNull.Value ? 0 : Convert.ToInt32(row["DelayID"]),

                    AnalysisCode = row["AnalysisCode"] == DBNull.Value ? "" : Convert.ToString(row["AnalysisCode"]),

                    LastPMDate = row["LastPMDate"] == DBNull.Value
                        ? (DateTime?)null
                        : Convert.ToDateTime(row["LastPMDate"]),

                    FailureReportStatus = row["FailureReportStatus"] == DBNull.Value ? "" : Convert.ToString(row["FailureReportStatus"]),

                    IncreaseMTBF = row["IncreaseMTBF"] == DBNull.Value ? "" : Convert.ToString(row["IncreaseMTBF"]).Trim(),
                    IncreaseMTBF1 = row["IncreaseMTBF1"] == DBNull.Value ? "" : Convert.ToString(row["IncreaseMTBF1"]),

                    DecreaseMTTR = row["DecreaseMTTR"] == DBNull.Value ? "" : Convert.ToString(row["DecreaseMTTR"]),
                    DecreaseMTTR1 = row["DecreaseMTTR1"] == DBNull.Value ? "" : Convert.ToString(row["DecreaseMTTR1"]).Trim(),

                    SAPBreakdownOrder = row["SAPBreakdownOrder"] == DBNull.Value ? "" : Convert.ToString(row["SAPBreakdownOrder"]),

                    FailureCategory1Component = row["FailureCategory1Component"] == DBNull.Value ? "" : Convert.ToString(row["FailureCategory1Component"]),
                    FailureCategory2RootCause = row["FailureCategory2RootCause"] == DBNull.Value ? "" : Convert.ToString(row["FailureCategory2RootCause"]),

                    StatusID = row["StatusID"] == DBNull.Value ? 0 : Convert.ToInt32(row["StatusID"]),
                    CreatedBy = row["CreatedBy"] == DBNull.Value ? "" : Convert.ToString(row["CreatedBy"]),

                    CreatedDate = row["CreatedDate"] == DBNull.Value
                        ? (DateTime?)null
                        : Convert.ToDateTime(row["CreatedDate"])
                };

                return model;
            }
            catch
            {
                return null;
            }
        }

        public int UpdateDelayCorrection(PlantDelayBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[6];

                p[0] = new SqlParameter("@DelayID", model.ID);
                p[1] = new SqlParameter("@DelayDescription1",
                    string.IsNullOrWhiteSpace(model.DelayDescription1) ? (object)DBNull.Value : model.DelayDescription1);

                p[2] = new SqlParameter("@ReasonforOccurence1",
                    string.IsNullOrWhiteSpace(model.ReasonForOccurence1) ? (object)DBNull.Value : model.ReasonForOccurence1);

                p[3] = new SqlParameter("@ActionTaken1",
                    string.IsNullOrWhiteSpace(model.ActionTaken1) ? (object)DBNull.Value : model.ActionTaken1);

                p[4] = new SqlParameter("@UpdatedBy",
                    string.IsNullOrWhiteSpace(model.UpdatedBy) ? (object)DBNull.Value : model.UpdatedBy);

                p[5] = new SqlParameter("@UpdatedDate",
                    model.UpdatedDate.HasValue ? (object)model.UpdatedDate.Value : DateTime.Now);

                return new DBHelper().ExecuteNonQueryReturn("sp_UpdateDelayCorrection", p);
            }
            catch
            {
                return 0;
            }
        }

        public string GenerateDelayCode()
        {
            try
            {
                DataTable dt = new DBHelper().GetTableFromQuery("sp_GenerateDelayCode", null);

                if (dt != null && dt.Rows.Count > 0)
                {
                    return Convert.ToString(dt.Rows[0]["DelayCode"]);
                }

                return "RM-" + DateTime.Now.Year + "-0001";
            }
            catch
            {
                return "RM-" + DateTime.Now.Year + "-0001";
            }
        }

        public string GenerateSMPDelayCode()
        {
            try
            {
                DataTable dt = new DBHelper().GetTableFromQuery("sp_GenerateDelayCode", null);

                if (dt != null && dt.Rows.Count > 0)
                {
                    return Convert.ToString(dt.Rows[0]["DelayCode"]);
                }

                return "SMP-" + DateTime.Now.Year + "-0001";
            }
            catch
            {
                return "SMP-" + DateTime.Now.Year + "-0001";
            }
        }

        public string GenerateFailureActionCode()
        {
            try
            {
                SqlParameter[] p = new SqlParameter[0];

                DataTable dt = new DBHelper().GetTableFromSP("sp_GenerateFailureActionCode", p);

                if (dt != null && dt.Rows.Count > 0)
                {
                    return Convert.ToString(dt.Rows[0]["ActionCode"]);
                }

                return "SS-CMD-" + DateTime.Now.Year + "-0001";
            }
            catch
            {
                return "SS-CMD-" + DateTime.Now.Year + "-0001";
            }
        }

        public int InsertFailureAnalysisAction(FailureAnalysisActionBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[8];

                p[0] = new SqlParameter("@ActionCode",
                    string.IsNullOrWhiteSpace(model.ActionCode)
                        ? (object)DBNull.Value
                        : model.ActionCode);

                p[1] = new SqlParameter("@DelayID", model.DelayID);

                p[2] = new SqlParameter("@AnalysisID",
                    model.AnalysisID.HasValue
                        ? (object)model.AnalysisID.Value
                        : DBNull.Value);

                p[3] = new SqlParameter("@ActionType",
                    string.IsNullOrWhiteSpace(model.ActionType)
                        ? (object)DBNull.Value
                        : model.ActionType);

                p[4] = new SqlParameter("@ActionRemarks",
                    string.IsNullOrWhiteSpace(model.ActionRemarks)
                        ? (object)DBNull.Value
                        : model.ActionRemarks);

                p[5] = new SqlParameter("@StatusID",
                    model.StatusID.HasValue
                        ? (object)model.StatusID.Value
                        : 1);

                p[6] = new SqlParameter("@CreatedBy",
                    string.IsNullOrWhiteSpace(model.CreatedBy)
                        ? (object)DBNull.Value
                        : model.CreatedBy);

                p[7] = new SqlParameter("@CreatedDate",
                    model.CreatedDate.HasValue
                        ? (object)model.CreatedDate.Value
                        : DateTime.Now);

                return new DBHelper().ExecuteNonQueryReturn("sp_InsertFailureAnalysisAction", p);
            }
            catch
            {
                return 0;
            }
        }

        public List<FailureAnalysisActionBLL> GetFailureAnalysisActionsByDelayID(int delayID)
        {
            var list = new List<FailureAnalysisActionBLL>();

            try
            {
                SqlParameter[] p = new SqlParameter[1];
                p[0] = new SqlParameter("@DelayID", delayID);

                DataTable dt = new DBHelper().GetTableFromSP("sp_GetFailureAnalysisActionsByDelayID", p);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var item = new FailureAnalysisActionBLL
                        {
                            ID = row["ID"] == DBNull.Value ? 0 : Convert.ToInt32(row["ID"]),

                            ActionCode = row["ActionCode"] == DBNull.Value
                                ? ""
                                : Convert.ToString(row["ActionCode"]),

                            DelayID = row["DelayID"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(row["DelayID"]),

                            AnalysisID = row["AnalysisID"] == DBNull.Value
                                ? (int?)null
                                : Convert.ToInt32(row["AnalysisID"]),

                            ActionType = row["ActionType"] == DBNull.Value
                                ? ""
                                : Convert.ToString(row["ActionType"]),

                            ActionRemarks = row["ActionRemarks"] == DBNull.Value
                                ? ""
                                : Convert.ToString(row["ActionRemarks"]),

                            StatusID = row["StatusID"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(row["StatusID"]),

                            CreatedBy = row["CreatedBy"] == DBNull.Value
                                ? ""
                                : Convert.ToString(row["CreatedBy"]),

                            CreatedDate = row["CreatedDate"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(row["CreatedDate"]),

                            UpdatedBy = row["UpdatedBy"] == DBNull.Value
                                ? ""
                                : Convert.ToString(row["UpdatedBy"]),

                            UpdatedDate = row["UpdatedDate"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(row["UpdatedDate"])
                        };

                        list.Add(item);
                    }
                }
            }
            catch
            {
                return list;
            }

            return list;
        }

        public int UpdateMissingMaintenanceAnalysis(FailureAnalysisBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[13];

                p[0] = new SqlParameter("@ID", model.ID);
                p[1] = new SqlParameter("@DelayID", model.DelayID);

                p[2] = new SqlParameter(
                    "@LastPMDate",
                    model.LastPMDate.HasValue
                        ? (object)model.LastPMDate.Value
                        : DBNull.Value
                );

                p[3] = new SqlParameter(
                    "@FailureReportStatus",
                    string.IsNullOrWhiteSpace(model.FailureReportStatus)
                        ? (object)DBNull.Value
                        : model.FailureReportStatus.Trim()
                );

                p[4] = new SqlParameter(
                    "@SAPBreakdownOrder",
                    string.IsNullOrWhiteSpace(model.SAPBreakdownOrder)
                        ? (object)DBNull.Value
                        : model.SAPBreakdownOrder.Trim()
                );

                p[5] = new SqlParameter(
                    "@IncreaseMTBF",
                    string.IsNullOrWhiteSpace(model.IncreaseMTBF)
                        ? (object)DBNull.Value
                        : model.IncreaseMTBF.Trim()
                );

                p[6] = new SqlParameter(
                    "@DecreaseMTTR",
                    string.IsNullOrWhiteSpace(model.DecreaseMTTR)
                        ? (object)DBNull.Value
                        : model.DecreaseMTTR.Trim()
                );

                p[7] = new SqlParameter(
                    "@IncreaseMTBF1",
                    string.IsNullOrWhiteSpace(model.IncreaseMTBF1)
                        ? (object)DBNull.Value
                        : model.IncreaseMTBF1.Trim()
                );

                p[8] = new SqlParameter(
                    "@DecreaseMTTR1",
                    string.IsNullOrWhiteSpace(model.DecreaseMTTR1)
                        ? (object)DBNull.Value
                        : model.DecreaseMTTR1.Trim()
                );

                p[9] = new SqlParameter(
                    "@FailureCategory1Component",
                    string.IsNullOrWhiteSpace(model.FailureCategory1Component)
                        ? (object)DBNull.Value
                        : model.FailureCategory1Component.Trim()
                );

                p[10] = new SqlParameter(
                    "@FailureCategory2RootCause",
                    string.IsNullOrWhiteSpace(model.FailureCategory2RootCause)
                        ? (object)DBNull.Value
                        : model.FailureCategory2RootCause.Trim()
                );

                p[11] = new SqlParameter(
                    "@UpdatedBy",
                    string.IsNullOrWhiteSpace(model.UpdatedBy)
                        ? "System"
                        : model.UpdatedBy
                );

                p[12] = new SqlParameter(
                    "@UpdatedDate",
                    model.UpdatedDate ?? DateTime.Now
                );

                return new DBHelper().ExecuteNonQueryReturn(
                    "sp_UpdateMissingMaintenanceAnalysis",
                    p
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Failed to update missing maintenance analysis information: "
                    + ex.Message,
                    ex
                );
            }
        }
        private string GetString(
    DataRow row,
    string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
            {
                return string.Empty;
            }

            if (row[columnName] == DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToString(
                row[columnName]
            ).Trim();
        }

        private int GetInt(
            DataRow row,
            string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) ||
                row[columnName] == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(
                row[columnName]
            );
        }

        private int? GetNullableInt(
            DataRow row,
            string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) ||
                row[columnName] == DBNull.Value)
            {
                return null;
            }

            return Convert.ToInt32(
                row[columnName]
            );
        }

        private DateTime? GetNullableDateTime(
            DataRow row,
            string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) ||
                row[columnName] == DBNull.Value)
            {
                return null;
            }

            return Convert.ToDateTime(
                row[columnName]
            );
        }

        private TimeSpan? GetNullableTimeSpan(
            DataRow row,
            string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) ||
                row[columnName] == DBNull.Value)
            {
                return null;
            }

            if (row[columnName] is TimeSpan time)
            {
                return time;
            }

            TimeSpan parsedTime;

            return TimeSpan.TryParse(
                Convert.ToString(row[columnName]),
                out parsedTime
            )
                ? parsedTime
                : (TimeSpan?)null;
        }
    }
}
