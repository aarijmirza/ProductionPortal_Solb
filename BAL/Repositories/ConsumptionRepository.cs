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
    public class ConsumptionRepository
    {
        public static DataTable _dt;
        public static DataSet _ds;
        public ConsumptionRepository() : base()
        {
            _dt = new DataTable();
            _ds = new DataSet();
        }

        public PlantConsumptionBLL GetPlantConsumptionByDate(DateTime date)
        {
            try
            {
                SqlParameter[] p =
                {
            new SqlParameter("@Date", date.Date)
        };

                DataTable dt = (new DBHelper().GetTableFromSP)("sp_GetPlantConsumptionByDate", p);

                if (dt != null && dt.Rows.Count > 0)
                {
                    return JArray.Parse(JsonConvert.SerializeObject(dt))
                                 .ToObject<List<PlantConsumptionBLL>>()
                                 .FirstOrDefault();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public int InsertPlantConsumption(PlantConsumptionBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[24];

                p[0] = new SqlParameter("@Date", model.Date);

                p[1] = new SqlParameter("@TotalProductBillet", model.TotalProductBillet ?? "");
                p[2] = new SqlParameter("@LPG", model.LPG ?? "");
                p[3] = new SqlParameter("@Oxygen", model.Oxygen ?? "");
                p[4] = new SqlParameter("@Nitrogen", model.Nitrogen ?? "");
                p[5] = new SqlParameter("@Argon", model.Argon ?? "");
                p[6] = new SqlParameter("@WaterConsumption", model.WaterConsumption ?? "");
                p[7] = new SqlParameter("@PowerConsumption", model.PowerConsumption ?? "");

                p[8] = new SqlParameter("@LPGm3ton", model.LPGm3ton ?? "");
                p[9] = new SqlParameter("@LPGNm3tonTarget", model.LPGNm3tonTarget ?? "");

                p[10] = new SqlParameter("@OxygenNm3ton", model.OxygenNm3ton ?? "");
                p[11] = new SqlParameter("@OxygenNm3tonTarget", model.OxygenNm3tonTarget ?? "");

                p[12] = new SqlParameter("@NitrogenNm3ton", model.NitrogenNm3ton ?? "");
                p[13] = new SqlParameter("@NitrogenNm3tonTarget", model.NitrogenNm3tonTarget ?? "");

                p[14] = new SqlParameter("@ArgonNm3ton", model.ArgonNm3ton ?? "");
                p[15] = new SqlParameter("@ArgonNm3tonTarget", model.ArgonNm3tonTarget ?? "");

                p[16] = new SqlParameter("@PowerConsumptionKWHton", model.PowerConsumptionKWHton ?? "");
                p[17] = new SqlParameter("@PowerConsumptionKWHtarget", model.PowerConsumptionKWHtarget ?? "");

                p[18] = new SqlParameter("@WaterConsumptionM3", model.WaterConsumptionM3 ?? "");
                p[19] = new SqlParameter("@WaterConsumptionTarget", model.WaterConsumptionTarget ?? "");

                p[20] = new SqlParameter("@StatusID", model.StatusID ?? 1);
                p[21] = new SqlParameter("@CreatedBy", model.CreatedBy ?? "");
                p[22] = new SqlParameter("@CreatedDate", model.CreatedDate ?? DateTime.Now);

                p[23] = new SqlParameter("@ID", model.ID);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_InsertPlantConsumption", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int UpdatePlantConsumption(PlantConsumptionBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[24];

                p[0] = new SqlParameter("@ID", model.ID);
                p[1] = new SqlParameter("@Date", model.Date);

                p[2] = new SqlParameter("@TotalProductBillet", model.TotalProductBillet ?? "");
                p[3] = new SqlParameter("@LPG", model.LPG ?? "");
                p[4] = new SqlParameter("@Oxygen", model.Oxygen ?? "");
                p[5] = new SqlParameter("@Nitrogen", model.Nitrogen ?? "");
                p[6] = new SqlParameter("@Argon", model.Argon ?? "");
                p[7] = new SqlParameter("@WaterConsumption", model.WaterConsumption ?? "");
                p[8] = new SqlParameter("@PowerConsumption", model.PowerConsumption ?? "");

                p[9] = new SqlParameter("@LPGm3ton", model.LPGm3ton ?? "");
                p[10] = new SqlParameter("@LPGNm3tonTarget", model.LPGNm3tonTarget ?? "");

                p[11] = new SqlParameter("@OxygenNm3ton", model.OxygenNm3ton ?? "");
                p[12] = new SqlParameter("@OxygenNm3tonTarget", model.OxygenNm3tonTarget ?? "");

                p[13] = new SqlParameter("@NitrogenNm3ton", model.NitrogenNm3ton ?? "");
                p[14] = new SqlParameter("@NitrogenNm3tonTarget", model.NitrogenNm3tonTarget ?? "");

                p[15] = new SqlParameter("@ArgonNm3ton", model.ArgonNm3ton ?? "");
                p[16] = new SqlParameter("@ArgonNm3tonTarget", model.ArgonNm3tonTarget ?? "");

                p[17] = new SqlParameter("@PowerConsumptionKWHton", model.PowerConsumptionKWHton ?? "");
                p[18] = new SqlParameter("@PowerConsumptionKWHtarget", model.PowerConsumptionKWHtarget ?? "");

                p[19] = new SqlParameter("@WaterConsumptionM3", model.WaterConsumptionM3 ?? "");
                p[20] = new SqlParameter("@WaterConsumptionTarget", model.WaterConsumptionTarget ?? "");

                p[21] = new SqlParameter("@StatusID", model.StatusID ?? 1);
                p[22] = new SqlParameter("@UpdatedBy", model.UpdatedBy ?? "");
                p[23] = new SqlParameter("@UpdatedDate", model.UpdatedDate ?? DateTime.Now);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_UpdatePlantConsumption", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int InsertPlantWiseConsumption(List<PlantConsumptionBLL> records)
        {
            try
            {
                if (records == null || records.Count == 0)
                    return 0;

                int finalResult = 0;

                foreach (var model in records)
                {
                    SqlParameter[] p = new SqlParameter[27];

                    p[0] = new SqlParameter("@Date",
                        model.Date.HasValue ? (object)model.Date.Value : DBNull.Value);

                    p[1] = new SqlParameter("@TotalProductBillet",
                        string.IsNullOrWhiteSpace(model.TotalProductBillet) ? (object)DBNull.Value : model.TotalProductBillet);

                    p[2] = new SqlParameter("@LPG",
                        string.IsNullOrWhiteSpace(model.LPG) ? (object)DBNull.Value : model.LPG);

                    p[3] = new SqlParameter("@Oxygen",
                        string.IsNullOrWhiteSpace(model.Oxygen) ? (object)DBNull.Value : model.Oxygen);

                    p[4] = new SqlParameter("@Nitrogen",
                        string.IsNullOrWhiteSpace(model.Nitrogen) ? (object)DBNull.Value : model.Nitrogen);

                    p[5] = new SqlParameter("@Argon",
                        string.IsNullOrWhiteSpace(model.Argon) ? (object)DBNull.Value : model.Argon);

                    p[6] = new SqlParameter("@WaterConsumption",
                        string.IsNullOrWhiteSpace(model.WaterConsumption) ? (object)DBNull.Value : model.WaterConsumption);

                    p[7] = new SqlParameter("@PowerConsumption",
                        string.IsNullOrWhiteSpace(model.PowerConsumption) ? (object)DBNull.Value : model.PowerConsumption);

                    p[8] = new SqlParameter("@LPGm3ton",
                        string.IsNullOrWhiteSpace(model.LPGm3ton) ? (object)DBNull.Value : model.LPGm3ton);

                    p[9] = new SqlParameter("@LPGNm3tonTarget",
                        string.IsNullOrWhiteSpace(model.LPGNm3tonTarget) ? (object)DBNull.Value : model.LPGNm3tonTarget);

                    p[10] = new SqlParameter("@OxygenNm3ton",
                        string.IsNullOrWhiteSpace(model.OxygenNm3ton) ? (object)DBNull.Value : model.OxygenNm3ton);

                    p[11] = new SqlParameter("@OxygenNm3tonTarget",
                        string.IsNullOrWhiteSpace(model.OxygenNm3tonTarget) ? (object)DBNull.Value : model.OxygenNm3tonTarget);

                    p[12] = new SqlParameter("@NitrogenNm3ton",
                        string.IsNullOrWhiteSpace(model.NitrogenNm3ton) ? (object)DBNull.Value : model.NitrogenNm3ton);

                    p[13] = new SqlParameter("@NitrogenNm3tonTarget",
                        string.IsNullOrWhiteSpace(model.NitrogenNm3tonTarget) ? (object)DBNull.Value : model.NitrogenNm3tonTarget);

                    p[14] = new SqlParameter("@ArgonNm3ton",
                        string.IsNullOrWhiteSpace(model.ArgonNm3ton) ? (object)DBNull.Value : model.ArgonNm3ton);

                    p[15] = new SqlParameter("@ArgonNm3tonTarget",
                        string.IsNullOrWhiteSpace(model.ArgonNm3tonTarget) ? (object)DBNull.Value : model.ArgonNm3tonTarget);

                    p[16] = new SqlParameter("@PowerConsumptionKWHton",
                        string.IsNullOrWhiteSpace(model.PowerConsumptionKWHton) ? (object)DBNull.Value : model.PowerConsumptionKWHton);

                    p[17] = new SqlParameter("@PowerConsumptionKWHtarget",
                        string.IsNullOrWhiteSpace(model.PowerConsumptionKWHtarget) ? (object)DBNull.Value : model.PowerConsumptionKWHtarget);

                    p[18] = new SqlParameter("@WaterConsumptionM3",
                        string.IsNullOrWhiteSpace(model.WaterConsumptionM3) ? (object)DBNull.Value : model.WaterConsumptionM3);

                    p[19] = new SqlParameter("@WaterConsumptionTarget",
                        string.IsNullOrWhiteSpace(model.WaterConsumptionTarget) ? (object)DBNull.Value : model.WaterConsumptionTarget);

                    p[20] = new SqlParameter("@StatusID",
                        model.StatusID.HasValue ? (object)model.StatusID.Value : DBNull.Value);

                    p[21] = new SqlParameter("@CreatedBy",
                        string.IsNullOrWhiteSpace(model.CreatedBy) ? (object)DBNull.Value : model.CreatedBy);

                    p[22] = new SqlParameter("@CreatedDate",
                        model.CreatedDate.HasValue ? (object)model.CreatedDate.Value : DateTime.Now);

                    p[23] = new SqlParameter("@UpdatedBy",
                        string.IsNullOrWhiteSpace(model.UpdatedBy) ? (object)DBNull.Value : model.UpdatedBy);

                    p[24] = new SqlParameter("@UpdatedDate",
                        model.UpdatedDate.HasValue ? (object)model.UpdatedDate.Value : DBNull.Value);

                    p[25] = new SqlParameter("@Plant",
                        string.IsNullOrWhiteSpace(model.Plant) ? (object)DBNull.Value : model.Plant);

                    p[26] = new SqlParameter("@FuelConsumption",
                        model.FuelConsumption.HasValue ? (object)model.FuelConsumption.Value : DBNull.Value);

                    finalResult = new DBHelper().ExecuteNonQueryReturn("sp_InsertPlantConsumption", p);
                }

                return finalResult;
            }
            catch
            {
                return 0;
            }
        }

        public UtilityDailyReportVM GetUtilityDailyReport(DateTime reportDate)
        {
            UtilityDailyReportVM vm = new UtilityDailyReportVM
            {
                ReportDate = reportDate.Date,
                Records = new List<PlantConsumptionBLL>(),
                SMP = new PlantConsumptionBLL { Plant = "SMP" },
                RM1 = new PlantConsumptionBLL { Plant = "RM1" },
                RM2 = new PlantConsumptionBLL { Plant = "RM2" }
            };

            try
            {
                SqlParameter[] p =
                {
            new SqlParameter("@Date", reportDate.Date)
        };

                DataTable dt = new DBHelper().GetTableFromSP("sp_GetUtilityDailyReport", p);

                if (dt == null || dt.Rows.Count == 0)
                    return vm;

                foreach (DataRow row in dt.Rows)
                {
                    PlantConsumptionBLL item = new PlantConsumptionBLL
                    {
                        ID = GetInt(row, "ID"),

                        Date = GetDate(row, "Date"),

                        Plant = GetString(row, "Plant"),

                        TotalProductBillet = GetString(row, "TotalProductBillet"),

                        LPG = GetString(row, "LPG"),
                        Oxygen = GetString(row, "Oxygen"),
                        Nitrogen = GetString(row, "Nitrogen"),
                        Argon = GetString(row, "Argon"),

                        WaterConsumption = GetString(row, "WaterConsumption"),
                        PowerConsumption = GetString(row, "PowerConsumption"),

                        LPGm3ton = GetString(row, "LPGm3ton"),
                        LPGNm3tonTarget = GetString(row, "LPGNm3tonTarget"),

                        OxygenNm3ton = GetString(row, "OxygenNm3ton"),
                        OxygenNm3tonTarget = GetString(row, "OxygenNm3tonTarget"),

                        NitrogenNm3ton = GetString(row, "NitrogenNm3ton"),
                        NitrogenNm3tonTarget = GetString(row, "NitrogenNm3tonTarget"),

                        ArgonNm3ton = GetString(row, "ArgonNm3ton"),
                        ArgonNm3tonTarget = GetString(row, "ArgonNm3tonTarget"),

                        PowerConsumptionKWHton = GetString(row, "PowerConsumptionKWHton"),
                        PowerConsumptionKWHtarget = GetString(row, "PowerConsumptionKWHtarget"),

                        WaterConsumptionM3 = GetString(row, "WaterConsumptionM3"),
                        WaterConsumptionTarget = GetString(row, "WaterConsumptionTarget"),

                        FuelConsumption = GetNullableDecimal(row, "FuelConsumption"),

                        StatusID = GetNullableInt(row, "StatusID"),

                        CreatedBy = GetString(row, "CreatedBy"),
                        CreatedDate = GetDate(row, "CreatedDate"),

                        UpdatedBy = GetString(row, "UpdatedBy"),
                        UpdatedDate = GetDate(row, "UpdatedDate")
                    };

                    vm.Records.Add(item);

                    string plant = (item.Plant ?? "").Trim().ToUpper();

                    if (plant == "SMP")
                    {
                        vm.SMP = item;
                    }
                    else if (plant == "RM1")
                    {
                        vm.RM1 = item;
                    }
                    else if (plant == "RM2")
                    {
                        vm.RM2 = item;
                    }
                }

                return vm;
            }
            catch
            {
                return vm;
            }
        }
        private string GetString(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return "";

            if (row[columnName] == DBNull.Value || row[columnName] == null)
                return "";

            return Convert.ToString(row[columnName]);
        }

        private int GetInt(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return 0;

            if (row[columnName] == DBNull.Value || row[columnName] == null)
                return 0;

            int result;
            return int.TryParse(row[columnName].ToString(), out result) ? result : 0;
        }

        private int? GetNullableInt(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return null;

            if (row[columnName] == DBNull.Value || row[columnName] == null)
                return null;

            int result;
            return int.TryParse(row[columnName].ToString(), out result) ? result : (int?)null;
        }

        private decimal? GetNullableDecimal(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return null;

            if (row[columnName] == DBNull.Value || row[columnName] == null)
                return null;

            decimal result;
            return decimal.TryParse(row[columnName].ToString(), out result) ? result : (decimal?)null;
        }

        private DateTime? GetDate(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return null;

            if (row[columnName] == DBNull.Value || row[columnName] == null)
                return null;

            DateTime result;
            return DateTime.TryParse(row[columnName].ToString(), out result) ? result : (DateTime?)null;
        }
    }
}
