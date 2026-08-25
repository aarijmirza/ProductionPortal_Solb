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

        //public List<BilletDischargingBLL> GetDichargedHeat(DateTime startDate, DateTime endDate, string shift)
        //{
        //    try
        //    {
        //        var lst = new List<BilletDischargingBLL>();

        //        SqlParameter[] p = new SqlParameter[]
        //        {
        //    new SqlParameter("@StartDate", startDate),
        //    new SqlParameter("@EndDate", endDate),
        //    new SqlParameter("@Shift", shift)
        //        };

        //        _dt = (new DBHelper().GetTableFromSP)("sp_GetDichargedHeat", p);

        //        if (_dt != null && _dt.Rows.Count > 0)
        //        {
        //            lst = JArray.Parse(JsonConvert.SerializeObject(_dt))
        //                        .ToObject<List<BilletDischargingBLL>>();
        //        }

        //        return lst;
        //    }
        //    catch
        //    {
        //        return new List<BilletDischargingBLL>();
        //    }
        //}

        public List<BilletDischargingBLL> GetDichargedHeats(
        DateTime startDate,
        DateTime endDate,
        string plant,
        string shift)
        {
            try
            {
                SqlParameter[] parameters =
                {
            new SqlParameter("@StartDate", SqlDbType.DateTime)
            {
                Value = startDate.Date
            },
            new SqlParameter("@EndDate", SqlDbType.DateTime)
            {
                Value = endDate.Date
            },
            new SqlParameter("@Plant", SqlDbType.NVarChar, 50)
            {
                Value = string.IsNullOrWhiteSpace(plant)
                    ? (object)DBNull.Value
                    : plant.Trim()
            },
            new SqlParameter("@Shift", SqlDbType.NVarChar, 50)
            {
                Value = string.IsNullOrWhiteSpace(shift)
                    ? (object)DBNull.Value
                    : shift.Trim()
            }
        };

                DataTable dt = new DBHelper()
                    .GetTableFromSP("sp_GetDichargedHeat", parameters);

                if (dt == null || dt.Rows.Count == 0)
                    return new List<BilletDischargingBLL>();

                return JArray
                    .Parse(JsonConvert.SerializeObject(dt))
                    .ToObject<List<BilletDischargingBLL>>()
                    ?? new List<BilletDischargingBLL>();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "GetDichargedHeats Error: " + ex.Message,
                    ex
                );
            }
        }
        public List<BilletDischargingBLL> GetDichargedHeat(
    DateTime startDate,
    DateTime endDate,
    string shift)
        {
            try
            {
                var list = new List<BilletDischargingBLL>();

                SqlParameter[] parameters =
                {
            new SqlParameter("@StartDate", startDate.Date),

            new SqlParameter("@EndDate", endDate.Date),

            new SqlParameter(
                "@Shift",
                string.IsNullOrWhiteSpace(shift)
                    ? (object)DBNull.Value
                    : shift.Trim()
            )
        };

                DataTable dt = new DBHelper()
                    .GetTableFromSP(
                        "sp_GetDichargedHeat",
                        parameters
                    );

                if (dt != null && dt.Rows.Count > 0)
                {
                    list = JArray
                        .Parse(JsonConvert.SerializeObject(dt))
                        .ToObject<List<BilletDischargingBLL>>();
                }

                return list ?? new List<BilletDischargingBLL>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "GetDichargedHeat Error: " + ex
                );

                throw;
            }
        }

        public List<BilletDischargingBLL> GetDichargedHeat2()
        {
            try
            {
                var lst = new List<BilletDischargingBLL>();


                _dt = (new DBHelper().GetTableFromSP)("sp_GetDichargedHeat2");

                if (_dt != null && _dt.Rows.Count > 0)
                {
                    lst = JArray.Parse(JsonConvert.SerializeObject(_dt))
                                .ToObject<List<BilletDischargingBLL>>();
                }

                return lst;
            }
            catch
            {
                return new List<BilletDischargingBLL>();
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
                SqlParameter[] p = new SqlParameter[16];

                p[0] = new SqlParameter("@Date", model.Date);
                p[1] = new SqlParameter("@Shift", model.Shift);
                p[2] = new SqlParameter("@Plant", model.Plant);
                p[3] = new SqlParameter("@HeatNo", model.HeatNo);
                p[4] = new SqlParameter("@BoardingNo", model.BoardingNo);
                p[5] = new SqlParameter("@SteelGrade", model.SteelGrade);
                p[6] = new SqlParameter("@BilletSize", model.BilletSize);
                p[7] = new SqlParameter("@BilletLength", model.BilletLength);
                p[8] = new SqlParameter("@Weight", model.Weight);
                p[9] = new SqlParameter("@TotalBillet", model.TotalBillet);
                p[10] = new SqlParameter("@TotalWeight", model.TotalWeight);
                p[11] = new SqlParameter("@HeatSequence", model.HeatSequence);
                p[12] = new SqlParameter("@HeatStatus", 102);
                p[13] = new SqlParameter("@StatusID", model.StatusID);
                p[14] = new SqlParameter("@CreatedBy", model.CreatedBy);
                p[15] = new SqlParameter("@CreatedDate", model.CreatedDate);

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
                SqlParameter[] p = new SqlParameter[18];

                p[0] = new SqlParameter("@Date", model.Date);
                p[1] = new SqlParameter("@Shift", model.Shift);
                p[2] = new SqlParameter("@Plant", model.Plant);
                p[3] = new SqlParameter("@HeatNo", model.HeatNo);
                p[4] = new SqlParameter("@BoardingNo", model.BoardingNo);
                p[5] = new SqlParameter("@SteelGrade", model.SteelGrade);
                p[6] = new SqlParameter("@NewSteelGrade", model.NewSteelGrade);
                p[7] = new SqlParameter("@ProductCode", model.ProductCode);
                p[8] = new SqlParameter("@PONumber", model.PONumber);
                p[9] = new SqlParameter("@Cobble", model.Cobble);
                p[10] = new SqlParameter("@HotOut", model.HotOut);
                p[11] = new SqlParameter("@TotalBillet", model.TotalBillet);
                p[12] = new SqlParameter("@TotalWeight", model.TotalWeight);
                p[13] = new SqlParameter("@DischargingSequence", model.DischargingSequence);
                p[14] = new SqlParameter("@HeatStatus", 103);
                p[15] = new SqlParameter("@StatusID", model.StatusID);
                p[16] = new SqlParameter("@CreatedBy", model.CreatedBy);
                p[17] = new SqlParameter("@CreatedOn", model.CreatedOn);

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

        public int UpdateRMShiftDetails(RMShiftDetailsBLL model)
        {
            SqlParameter[] p = {
            new SqlParameter("@Date", model.Date),
            new SqlParameter("@Plant", model.Plant),
            new SqlParameter("@Shift", model.Shift),
            new SqlParameter("@Team", model.Team),
            new SqlParameter("@ShiftIncharge", model.ShiftIncharge)
        };

            return (new DBHelper().ExecuteNonQueryReturn)("sp_UpdateRMShiftDetails", p);
        }

        public bool IsShiftExist(DateTime date, string plant, string shift)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[]
                {
            new SqlParameter("@Date", date),
            new SqlParameter("@Plant", plant),
            new SqlParameter("@Shift", shift)
                };

                var result = new DBHelper().ExecuteScalar("sp_CheckRMShiftExists", p);

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result) > 0;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public int AddBundlingSection(BundlingSectionBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[18];

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
                    p[10] = new SqlParameter("@StatusID", 1);
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
                p[15] = new SqlParameter("@Plant", model.Plant);
                p[16] = new SqlParameter("@Profile", model.Profile);
                p[17] = new SqlParameter("@Size", model.Size);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_InsertBundleSection", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public List<RMShiftDetailsBLL> RollingMillDetails()
        {
            return DBHelper.GetList<RMShiftDetailsBLL>(
                "SELECT * FROM RMShiftDetails WHERE StatusID = 1"
            );
        }

        public List<RMShiftDetailsBLL> RMShiftDetailAll()
        {
            try
            {
                var lst = new List<RMShiftDetailsBLL>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetRMAllShiftDetails", p);
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

        public int DeleteShiftDetails(int? id, string createdBy)
        {
            SqlParameter[] p = {
            new SqlParameter("@StatusID", 3),
            new SqlParameter("@UpdatedDate", DateTime.Now),
            new SqlParameter("@UpdatedBy", createdBy),
            new SqlParameter("@ID", id),
        };

            return (new DBHelper().ExecuteNonQueryReturn)("sp_DeleteRMShiftDetails", p);
        }
        public int DeleteCharging(int? id, string createdBy)
        {
            SqlParameter[] p = {
            new SqlParameter("@StatusID", 3),
            new SqlParameter("@UpdatedDate", DateTime.Now),
            new SqlParameter("@UpdatedBy", createdBy),
            new SqlParameter("@ID", id),
        };

            return (new DBHelper().ExecuteNonQueryReturn)("sp_DeleteRMCharging", p);
        }
        public int DeleteDischarging(int? id, string createdBy)
        {
            SqlParameter[] p = {
            new SqlParameter("@StatusID", 3),
            new SqlParameter("@UpdatedDate", DateTime.Now),
            new SqlParameter("@UpdatedBy", createdBy),
            new SqlParameter("@ID", id),
        };

            return (new DBHelper().ExecuteNonQueryReturn)("sp_DeleteRMDischarging", p);
        }

        public int DeleteBundle(int? id, string createdBy)
        {
            SqlParameter[] p = {
            new SqlParameter("@StatusID", 3),
            new SqlParameter("@UpdatedDate", DateTime.Now),
            new SqlParameter("@UpdatedBy", createdBy),
            new SqlParameter("@ID", id),
        };

            return (new DBHelper().ExecuteNonQueryReturn)("sp_DeleteRMBundle", p);
        }

        public bool IsRMHourlyDischargeExist(DateTime date, string shift)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[2];

                p[0] = new SqlParameter("@Date", date.Date);
                p[1] = new SqlParameter("@Shift", shift ?? "");

                DataTable dt = new DBHelper().GetTableFromSP("sp_IsRMHourlyDischargeExist", p);

                if (dt != null && dt.Rows.Count > 0)
                {
                    int count = Convert.ToInt32(dt.Rows[0][0]);
                    return count > 0;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public int InsertRMHourlyDischarge(RMHourlyDischargeBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[19];

                p[0] = new SqlParameter("@Date", model.Date);
                p[1] = new SqlParameter("@Shift", model.Shift ?? "");

                p[2] = new SqlParameter("@TimeFrom", model.TimeFrom ?? "");
                p[3] = new SqlParameter("@TimeTo", model.TimeTo ?? "");

                p[4] = new SqlParameter("@NoofBillets", model.NoofBillets ?? "");
                p[5] = new SqlParameter("@NoofCobble", model.NoofCobble ?? 0);
                p[6] = new SqlParameter("@Reject", model.Reject ?? 0);
                p[7] = new SqlParameter("@BilletHeatNo", model.BilletHeatNo ?? "");

                p[8] = new SqlParameter("@SafetyIssueShift", model.SafetyIssueShift ?? "");
                p[9] = new SqlParameter("@MessageShift", model.MessageShift ?? "");

                p[10] = new SqlParameter("@FuelConsumptionStart", model.FuelConsumptionStart ?? "");
                p[11] = new SqlParameter("@FuelConsumptionEnd", model.FuelConsumptionEnd ?? "");
                p[12] = new SqlParameter("@TotalConsumption", model.TotalConsumption ?? "");
                p[13] = new SqlParameter("@ElectricityConsumption", model.ElectricityConsumption ?? "");

                p[14] = new SqlParameter("@StatusID", model.StatusID);
                p[15] = new SqlParameter("@CreatedBy", model.CreatedBy ?? "");
                p[16] = new SqlParameter("@CreatedDate", model.CreatedDate);

                p[17] = new SqlParameter("@UpdatedBy", DBNull.Value);
                p[18] = new SqlParameter("@UpdatedDate", DBNull.Value);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_InsertRMHourlyDischarge", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int UpdateBundlingSection(BundlingSectionBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[14];

                p[0] = new SqlParameter("@ID", model.ID);
                p[1] = new SqlParameter("@Date", model.Date);
                p[2] = new SqlParameter("@Shift", model.Shift ?? "");
                p[3] = new SqlParameter("@HeatNo", model.HeatNo ?? "");
                p[4] = new SqlParameter("@BilletBoardingNo", model.BilletBoardingNo ?? "");
                p[5] = new SqlParameter("@SteelGrade", model.SteelGrade ?? "");
                p[6] = new SqlParameter("@PONumber", model.PONumber ?? "");
                p[7] = new SqlParameter("@TotalBundleProduced", model.TotalBundleProduced);
                p[8] = new SqlParameter("@PerCoilWeight", model.PerCoilWeight);
                p[9] = new SqlParameter("@TheoriticalWeight", model.TheoriticalWeight);
                p[10] = new SqlParameter("@Remarks", model.Remarks ?? "");
                p[11] = new SqlParameter("@UpdatedBy", model.UpdatedBy ?? "");
                p[12] = new SqlParameter("@Profile", model.Profile ?? "");
                p[13] = new SqlParameter("@Size", model.Size ?? "");

                return (new DBHelper().ExecuteNonQueryReturn)("sp_UpdateBundlingSection", p);
            }
            catch
            {
                return 0;
            }
        }

        public int UpdateBilletCharging(BilletChargingBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[13];

                p[0] = new SqlParameter("@ID", model.ID);
                p[1] = new SqlParameter("@Date", model.Date);
                p[2] = new SqlParameter("@Shift", model.Shift ?? "");
                p[3] = new SqlParameter("@HeatNo", model.HeatNo ?? "");
                p[4] = new SqlParameter("@BoardingNo", model.BoardingNo ?? "");
                p[5] = new SqlParameter("@SteelGrade", model.SteelGrade ?? "");
                p[6] = new SqlParameter("@BilletSize", model.BilletSize ?? "");
                p[7] = new SqlParameter("@BilletLength", model.BilletLength ?? "");
                p[8] = new SqlParameter("@TotalBillet", model.TotalBillet);
                p[9] = new SqlParameter("@Weight", model.Weight);
                p[10] = new SqlParameter("@TotalWeight", model.TotalWeight);
                p[11] = new SqlParameter("@HeatSequence", model.HeatSequence);
                p[12] = new SqlParameter("@UpdatedBy", model.UpdatedBy ?? "");

                return (new DBHelper().ExecuteNonQueryReturn)("sp_UpdateBilletCharging", p);
            }
            catch
            {
                return 0;
            }
        }

        public int UpdateDischarging(BilletDischargingBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[18];

                p[0] = new SqlParameter("@ID", model.ID);
                p[1] = new SqlParameter("@Date", model.Date);
                p[2] = new SqlParameter("@Shift", model.Shift ?? "");
                p[3] = new SqlParameter("@Plant", model.Plant ?? "");
                p[4] = new SqlParameter("@HeatNo", model.HeatNo ?? "");
                p[5] = new SqlParameter("@BoardingNo", model.BoardingNo ?? "");
                p[6] = new SqlParameter("@SteelGrade", model.SteelGrade ?? "");
                p[7] = new SqlParameter("@NewSteelGrade", model.NewSteelGrade ?? "");
                p[8] = new SqlParameter("@PONumber", model.PONumber ?? "");
                p[9] = new SqlParameter("@Cobble", model.Cobble);
                p[10] = new SqlParameter("@HotOut", model.HotOut);
                p[11] = new SqlParameter("@TotalBillet", model.TotalBillet);
                p[12] = new SqlParameter("@TotalWeight", model.TotalWeight);
                p[13] = new SqlParameter("@DischargingSequence", model.DischargingSequence);
                p[14] = new SqlParameter("@StatusID", model.StatusID);
                p[15] = new SqlParameter("@UpdatedBy", model.UpdatedBy ?? "");
                p[16] = new SqlParameter("@UpdatedDate", model.UpdatedDate ?? DateTime.Now);
                p[17] = new SqlParameter("@HeatStatus", 103);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_UpdateDischarging", p);
            }
            catch
            {
                return 0;
            }
        }

        public int SyncDischargingWeightByHeat(
            string heatNo,
            decimal weightPerBillet,
            string updatedBy)
        {
            try
            {
                if (
                    string.IsNullOrWhiteSpace(heatNo) ||
                    weightPerBillet <= 0M
                )
                {
                    return 0;
                }

                SqlParameter[] p =
                    new SqlParameter[3];

                p[0] =
                    new SqlParameter(
                        "@HeatNo",
                        heatNo.Trim()
                    );

                p[1] =
                    new SqlParameter(
                        "@WeightPerBillet",
                        weightPerBillet
                    );

                p[2] =
                    new SqlParameter(
                        "@UpdatedBy",
                        string.IsNullOrWhiteSpace(updatedBy)
                            ? (object)DBNull.Value
                            : updatedBy.Trim()
                    );

                DataTable dt =
                    (new DBHelper().GetTableFromSP)(
                        "sp_RM_SyncDischargingWeightByHeat",
                        p
                    );

                if (
                    dt != null &&
                    dt.Rows.Count > 0 &&
                    dt.Columns.Contains("AffectedRows")
                )
                {
                    return Convert.ToInt32(
                        dt.Rows[0]["AffectedRows"]
                    );
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

    }
}
