//using DAL.Models;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using WebAPICode.Helpers;

//namespace BAL.Repositories
//{
//    public class UtilityRepository
//    {
//        public UtilityDailyReportVM GetUtilityDailyReport(DateTime date)
//        {
//            UtilityDailyReportVM vm = new UtilityDailyReportVM();
//            vm.ReportDate = date;

//            SqlParameter[] p =
//            {
//            new SqlParameter("@Date", date.Date)
//        };

//            DataTable dt = new DBHelper().GetTableFromSP("sp_GetPlantConsumptionDailyReport", p);

//            List<PlantConsumptionBLL> list = new List<PlantConsumptionBLL>();

//            foreach (DataRow row in dt.Rows)
//            {
//                list.Add(new PlantConsumptionBLL
//                {
//                    ID = row["ID"] == DBNull.Value ? 0 : Convert.ToInt32(row["ID"]),
//                    Date = row["Date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["Date"]),

//                    TotalProductBillet = ToDecimal(row["TotalProductBillet"]),
//                    LPG = ToDecimal(row["LPG"]),
//                    Oxygen = ToDecimal(row["Oxygen"]),
//                    Nitrogen = ToDecimal(row["Nitrogen"]),
//                    Argon = ToDecimal(row["Argon"]),
//                    WaterConsumption = ToDecimal(row["WaterConsumption"]),
//                    PowerConsumption = ToDecimal(row["PowerConsumption"]),

//                    LPGm3ton = ToDecimal(row["LPGm3ton"]),
//                    LPGNm3tonTarget = ToDecimal(row["LPGNm3tonTarget"]),

//                    OxygenNm3ton = ToDecimal(row["OxygenNm3ton"]),
//                    OxygenNm3tonTarget = ToDecimal(row["OxygenNm3tonTarget"]),

//                    NitrogenNm3ton = ToDecimal(row["NitrogenNm3ton"]),
//                    NitrogenNm3tonTarget = ToDecimal(row["NitrogenNm3tonTarget"]),

//                    ArgonNm3ton = ToDecimal(row["ArgonNm3ton"]),
//                    ArgonNm3tonTarget = ToDecimal(row["ArgonNm3tonTarget"]),

//                    PowerConsumptionKWHton = ToDecimal(row["PowerConsumptionKWHton"]),
//                    PowerConsumptionKWHtarget = ToDecimal(row["PowerConsumptionKWHtarget"]),

//                    WaterConsumptionM3 = ToDecimal(row["WaterConsumptionM3"]),
//                    WaterConsumptionTarget = ToDecimal(row["WaterConsumptionTarget"]),

//                    StatusID = row["StatusID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["StatusID"]),

//                    CreatedBy = row["CreatedBy"] == DBNull.Value ? "" : row["CreatedBy"].ToString(),
//                    CreatedDate = row["CreatedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["CreatedDate"]),

//                    UpdatedBy = row["UpdatedBy"] == DBNull.Value ? "" : row["UpdatedBy"].ToString(),
//                    UpdatedDate = row["UpdatedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["UpdatedDate"]),

//                    Plant = row["Plant"] == DBNull.Value ? "" : row["Plant"].ToString(),

//                    FuelConsumption = ToDecimal(row["FuelConsumption"])
//                });
//            }

//            vm.SMP = list.FirstOrDefault(x =>
//                !string.IsNullOrEmpty(x.Plant) &&
//                x.Plant.Trim().Equals("SMP", StringComparison.OrdinalIgnoreCase)
//            ) ?? new PlantConsumptionBLL { Plant = "SMP" };

//            vm.RM1 = list.FirstOrDefault(x =>
//                !string.IsNullOrEmpty(x.Plant) &&
//                x.Plant.Trim().Equals("RM1", StringComparison.OrdinalIgnoreCase)
//            ) ?? new PlantConsumptionBLL { Plant = "RM1" };

//            vm.RM2 = list.FirstOrDefault(x =>
//                !string.IsNullOrEmpty(x.Plant) &&
//                x.Plant.Trim().Equals("RM2", StringComparison.OrdinalIgnoreCase)
//            ) ?? new PlantConsumptionBLL { Plant = "RM2" };

//            return vm;
//        }

//        private decimal? ToDecimal(object value)
//        {
//            if (value == null || value == DBNull.Value)
//                return null;

//            decimal result;
//            if (decimal.TryParse(value.ToString(), out result))
//                return result;

//            return null;
//        }
//    }
//}
