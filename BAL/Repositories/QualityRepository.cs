using DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using WebAPICode.Helpers;

namespace BAL.Repositories
{
    public class QualityRepository
    {
        public static DataTable _dt;
        public static DataSet _ds;

        public QualityRepository() : base()
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
                _dt = new DBHelper().GetTableFromSP("sp_GetBilletGrades_PP", p);
                if (_dt != null && _dt.Rows.Count > 0)
                    lst = JArray.Parse(JsonConvert.SerializeObject(_dt)).ToObject<List<BilletGrades>>();
                return lst;
            }
            catch
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
                _dt = new DBHelper().GetTableFromSP("sp_GetAllHeatChemistry", p);
                if (_dt != null && _dt.Rows.Count > 0)
                    lst = JArray.Parse(JsonConvert.SerializeObject(_dt)).ToObject<List<HeatChemistryBLL>>();
                return lst;
            }
            catch
            {
                return null;
            }
        }

        public List<HeatChemistryBLL> GetChemsitryHeatDetails(string heatno)
        {
            try
            {
                var list = new List<HeatChemistryBLL>();
                SqlParameter[] p = { new SqlParameter("@id", heatno) };
                _dt = new DBHelper().GetTableFromSP("sp_GetChemistryHeatDetail", p);
                if (_dt != null && _dt.Rows.Count > 0)
                    list = JArray.Parse(JsonConvert.SerializeObject(_dt)).ToObject<List<HeatChemistryBLL>>();
                return list;
            }
            catch
            {
                return new List<HeatChemistryBLL>();
            }
        }

        public int AddHeatChemistry(HeatChemistryBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[32];
                p[0] = new SqlParameter("@PlantName", model.PlantName ?? "");
                p[1] = new SqlParameter("@Date", model.Date ?? DateTime.Now);
                p[2] = new SqlParameter("@Time", model.Time);
                p[3] = new SqlParameter("@HeatNo", model.HeatNo ?? "");
                p[4] = new SqlParameter("@Grade", model.SteelGrade ?? "");
                p[5] = new SqlParameter("@Lenght", model.Lenght);
                p[6] = new SqlParameter("@CrossSection", model.CrossSection);
                p[7] = new SqlParameter("@Size", model.Size ?? "");
                p[8] = new SqlParameter("@Shift", model.Shift ?? "");
                p[9] = new SqlParameter("@SampleNo", model.SampleNo ?? "");
                p[10] = new SqlParameter("@C", model.C);
                p[11] = new SqlParameter("@Si", model.Si);
                p[12] = new SqlParameter("@Mn", model.Mn);
                p[13] = new SqlParameter("@P", model.P);
                p[14] = new SqlParameter("@S", model.S);
                p[15] = new SqlParameter("@Ni", model.Ni);
                p[16] = new SqlParameter("@Cr", model.Cr);
                p[17] = new SqlParameter("@Mo", model.Mo);
                p[18] = new SqlParameter("@V", model.V);
                p[19] = new SqlParameter("@Cu", model.Cu);
                p[20] = new SqlParameter("@Ti", model.Ti);
                p[21] = new SqlParameter("@Sn", model.Sn);
                p[22] = new SqlParameter("@Al", model.Al);
                p[23] = new SqlParameter("@Pb", model.Pb);
                p[24] = new SqlParameter("@B", model.B);
                p[25] = new SqlParameter("@Zn", model.Zn);
                p[26] = new SqlParameter("@N", model.N);
                p[27] = new SqlParameter("@MnS", model.MnS);
                p[28] = new SqlParameter("@Ceq", model.Ceq);
                p[29] = new SqlParameter("@StatusID", model.StatusID);
                p[30] = new SqlParameter("@CreatedDate", model.CreatedDate);
                p[31] = new SqlParameter("@CreatedBy", model.CreatedBy ?? "");
                return new DBHelper().ExecuteNonQueryReturn("sp_AddHeatChemistry", p);
            }
            catch
            {
                return 0;
            }
        }

        public int InsertBilletBoarding(BilletBoardBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[16];
                var profile = model.Profile + model.Size;
                p[0] = new SqlParameter("@Date", model.Date);
                p[1] = new SqlParameter("@HeatNo", model.HeatNo);
                p[2] = new SqlParameter("@BilletBoarding", model.BilletBoarding);
                p[3] = new SqlParameter("@PlantName", model.PlantName);
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
                p[15] = new SqlParameter("@Profile", profile);
                return new DBHelper().ExecuteNonQueryReturn("sp_AddBilletBoarding", p);
            }
            catch
            {
                return 0;
            }
        }

        public int InsertChemicalAnalysisRM(RMChemicalAnalysisBLL model, int srNo = 0)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[14];
                p[0] = new SqlParameter("@SrNo", srNo > 0 ? srNo : 1);
                p[1] = new SqlParameter("@HeatNo", model.HeatNo ?? "");
                p[2] = new SqlParameter("@NoOfBillets", model.NoOfBillets);
                p[3] = new SqlParameter("@C", model.C);
                p[4] = new SqlParameter("@Si", model.Si);
                p[5] = new SqlParameter("@Mn", model.Mn);
                p[6] = new SqlParameter("@S", model.S);
                p[7] = new SqlParameter("@P", model.P);
                p[8] = new SqlParameter("@N", model.N);
                p[9] = new SqlParameter("@Ceq", model.Ceq);
                p[10] = new SqlParameter("@HeatStatus", model.HeatStatus);
                p[11] = new SqlParameter("@StatusID", model.StatusID);
                p[12] = new SqlParameter("@CreatedBy", model.CreatedBy ?? "");
                p[13] = new SqlParameter("@CreatedDate", model.CreatedDate);
                return new DBHelper().ExecuteNonQueryReturn("sp_AddRMChemicalAnalysis", p);
            }
            catch
            {
                throw;
            }
        }

        public List<BilletBoardBLL> GetAllBoarding()
        {
            try
            {
                var lst = new List<BilletBoardBLL>();
                SqlParameter[] p = new SqlParameter[0];
                _dt = new DBHelper().GetTableFromSP("sp_GetAllBilletBoarding", p);
                if (_dt != null && _dt.Rows.Count > 0)
                    lst = JArray.Parse(JsonConvert.SerializeObject(_dt)).ToObject<List<BilletBoardBLL>>();
                return lst;
            }
            catch
            {
                return null;
            }
        }

        public List<BilletBoardBLL> GetAllBoarding_RMCharging()
        {
            try
            {
                var lst = new List<BilletBoardBLL>();
                SqlParameter[] p = new SqlParameter[0];
                _dt = new DBHelper().GetTableFromSP("sp_GetAllBoarding_RMCharging", p);
                if (_dt != null && _dt.Rows.Count > 0)
                    lst = JArray.Parse(JsonConvert.SerializeObject(_dt)).ToObject<List<BilletBoardBLL>>();
                return lst;
            }
            catch
            {
                return null;
            }
        }

        public List<BilletChargingBLL> GetAllCharging()
        {
            try
            {
                var lst = new List<BilletChargingBLL>();
                SqlParameter[] p = new SqlParameter[0];
                _dt = new DBHelper().GetTableFromSP("sp_GetAllCharging", p);
                if (_dt != null && _dt.Rows.Count > 0)
                    lst = JArray.Parse(JsonConvert.SerializeObject(_dt)).ToObject<List<BilletChargingBLL>>();
                return lst;
            }
            catch
            {
                return null;
            }
        }

        public BilletBoardBLL GetBilletDetails(string heatno)
        {
            try
            {
                var obj = new BilletBoardBLL();
                SqlParameter[] p = { new SqlParameter("@id", heatno) };
                _dt = new DBHelper().GetTableFromSP("sp_GetBilletDetailByHeatno", p);
                if (_dt != null && _dt.Rows.Count > 0)
                    obj = JArray.Parse(JsonConvert.SerializeObject(_dt)).ToObject<List<BilletBoardBLL>>().FirstOrDefault();
                return obj;
            }
            catch
            {
                return null;
            }
        }

        public bool IsBilletBoardingExists(string billetBoarding)
        {
            try
            {
                SqlParameter[] p = { new SqlParameter("@BilletBoarding", billetBoarding) };
                DataTable dt = new DBHelper().GetTableFromSP("sp_CheckBilletBoardingExists", p);
                return dt != null && dt.Rows.Count > 0 && Convert.ToInt32(dt.Rows[0]["Total"]) > 0;
            }
            catch
            {
                return false;
            }
        }

        public List<string> GetDuplicateHeatNos(List<string> heatNos)
        {
            try
            {
                if (heatNos == null || !heatNos.Any())
                    return new List<string>();

                string heatNoCsv = string.Join(",", heatNos.Select(x => x.Trim()));
                SqlParameter[] p = { new SqlParameter("@HeatNos", heatNoCsv) };
                DataTable dt = new DBHelper().GetTableFromSP("sp_GetDuplicateHeatNos", p);

                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.AsEnumerable()
                        .Select(x => Convert.ToString(x["HeatNo"]))
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList();
                }

                return new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        public BilletBoardBLL GetBilletDetails(int id)
        {
            try
            {
                BilletBoardBLL obj = null;
                SqlParameter[] p = { new SqlParameter("@id", id) };
                DataSet ds = new DBHelper().GetDatasetFromSP("sp_GetBilletDetailByID", p);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        obj = JArray.Parse(JsonConvert.SerializeObject(ds.Tables[0])).ToObject<List<BilletBoardBLL>>().FirstOrDefault();

                    if (obj != null)
                    {
                        obj.BilletBoardingHeats = ds.Tables.Count > 1 && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0
                            ? JArray.Parse(JsonConvert.SerializeObject(ds.Tables[1])).ToObject<List<BilletBoardBLL>>()
                            : new List<BilletBoardBLL>();

                        obj.Chemistry = ds.Tables.Count > 2 && ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0
                            ? JArray.Parse(JsonConvert.SerializeObject(ds.Tables[2])).ToObject<List<RMChemicalAnalysisBLL>>()
                            : new List<RMChemicalAnalysisBLL>();
                    }
                }

                return obj;
            }
            catch
            {
                return null;
            }
        }

        public List<HeatChemistryBLL> GetHeatChemistryDatewise(DateTime from, DateTime to)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@startdate", from),
                new SqlParameter("@enddate", to)
            };

            DataTable dt = new DBHelper().GetTableFromSP("sp_GetHeatChemistryDatewise", p);
            List<HeatChemistryBLL> list = new List<HeatChemistryBLL>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new HeatChemistryBLL
                {
                    PlantName = row["PlantName"].ToString(),
                    Date = row["Date"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["Date"]),
                    HeatNo = row["HeatNo"].ToString(),
                    SteelGrade = row["Grade"].ToString(),
                    Weight = row["Weight"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(row["Weight"]),
                    Area = row["Area"].ToString(),
                    Size = row["Size"].ToString(),
                    Time = row["Time"] == DBNull.Value ? null : (TimeSpan?)row["Time"],
                    Shift = row["Shift"].ToString(),
                    SampleNo = row["SampleNo"].ToString(),
                    C = row["C"] == DBNull.Value ? null : (decimal?)row["C"],
                    Si = row["Si"] == DBNull.Value ? null : (decimal?)row["Si"],
                    Mn = row["Mn"] == DBNull.Value ? null : (decimal?)row["Mn"],
                    P = row["P"] == DBNull.Value ? null : (decimal?)row["P"],
                    S = row["S"] == DBNull.Value ? null : (decimal?)row["S"],
                    Ni = row["Ni"] == DBNull.Value ? null : (decimal?)row["Ni"],
                    Cr = row["Cr"] == DBNull.Value ? null : (decimal?)row["Cr"],
                    Mo = row["Mo"] == DBNull.Value ? null : (decimal?)row["Mo"],
                    V = row["V"] == DBNull.Value ? null : (decimal?)row["V"],
                    Cu = row["Cu"] == DBNull.Value ? null : (decimal?)row["Cu"],
                    Ti = row["Ti"] == DBNull.Value ? null : (decimal?)row["Ti"],
                    Sn = row["Sn"] == DBNull.Value ? null : (decimal?)row["Sn"],
                    Al = row["Al"] == DBNull.Value ? null : (decimal?)row["Al"],
                    Pb = row["Pb"] == DBNull.Value ? null : (decimal?)row["Pb"],
                    B = row["B"] == DBNull.Value ? null : (decimal?)row["B"],
                    Zn = row["Zn"] == DBNull.Value ? null : (decimal?)row["Zn"],
                    N = row["N"] == DBNull.Value ? null : (decimal?)row["N"],
                    MnS = row["MnS"] == DBNull.Value ? null : (decimal?)row["MnS"],
                    Ceq = row["Ceq"] == DBNull.Value ? null : (decimal?)row["Ceq"]
                });
            }

            return list;
        }

        public bool IsSlagByProductDuplicate(DateTime productionDate, string heatNo, string byProductType, int excludeID)
        {
            SqlParameter[] p = new SqlParameter[4];
            p[0] = new SqlParameter("@DateOfProduction", SqlDbType.Date) { Value = productionDate.Date };
            p[1] = new SqlParameter("@HeatNo", (heatNo ?? string.Empty).Trim());
            p[2] = new SqlParameter("@ByProductType", (byProductType ?? string.Empty).Trim());
            p[3] = new SqlParameter("@ExcludeID", excludeID);

            object result = new DBHelper().ExecuteScalar("sp_CheckSlagByProductDuplicate", p);
            return result != null && result != DBNull.Value && Convert.ToInt32(result) > 0;
        }

        public int InsertSlagByProduct(SlagByProductAnalysisBLL data)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[9];
                p[0] = new SqlParameter("@Date", data.CreatedDate ?? (object)DBNull.Value);
                p[1] = new SqlParameter("@HeatNo", data.HeatNo ?? (object)DBNull.Value);
                p[2] = new SqlParameter("@CertificateNo", data.CertificateNo ?? (object)DBNull.Value);
                p[3] = new SqlParameter("@ByProductType", data.ByProductType ?? (object)DBNull.Value);
                p[4] = new SqlParameter("@DateOfProduction", data.DateOfProduction ?? (object)DBNull.Value);
                p[5] = new SqlParameter("@DateOfAnalysis", data.DateOfAnalysis ?? (object)DBNull.Value);
                p[6] = new SqlParameter("@StatusID", data.StatusID ?? (object)DBNull.Value);
                p[7] = new SqlParameter("@CreatedBy", data.CreatedBy ?? (object)DBNull.Value);
                p[8] = new SqlParameter("@CreatedDate", data.CreatedDate ?? (object)DBNull.Value);
                return Convert.ToInt32(new DBHelper().ExecuteScalar("sp_InsertSlagByProductAnalysis", p));
            }
            catch
            {
                return 0;
            }
        }

        public int UpdateSlagByProduct(SlagByProductAnalysisBLL data)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[10];
                p[0] = new SqlParameter("@Date", data.CreatedDate ?? (object)DBNull.Value);
                p[1] = new SqlParameter("@HeatNo", data.HeatNo ?? (object)DBNull.Value);
                p[2] = new SqlParameter("@CertificateNo", data.CertificateNo ?? (object)DBNull.Value);
                p[3] = new SqlParameter("@ByProductType", data.ByProductType ?? (object)DBNull.Value);
                p[4] = new SqlParameter("@DateOfProduction", data.DateOfProduction ?? (object)DBNull.Value);
                p[5] = new SqlParameter("@DateOfAnalysis", data.DateOfAnalysis ?? (object)DBNull.Value);
                p[6] = new SqlParameter("@StatusID", data.StatusID ?? (object)DBNull.Value);
                p[7] = new SqlParameter("@UpdatedBy", data.UpdatedBy ?? (object)DBNull.Value);
                p[8] = new SqlParameter("@UpdatedDate", data.UpdatedDate ?? (object)DBNull.Value);
                p[9] = new SqlParameter("@ID", data.ID);
                return Convert.ToInt32(new DBHelper().ExecuteScalar("sp_UpdateSlagByProduct", p));
            }
            catch
            {
                return 0;
            }
        }

        public int DeleteSlagSamplesBySlagID(int ID, string UpdatedBy)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[4];
                p[0] = new SqlParameter("@ID", ID);
                p[1] = new SqlParameter("@StatusID", 3);
                p[2] = new SqlParameter("@UpdatedDate", DateTime.Now);
                p[3] = new SqlParameter("@UpdatedBy", UpdatedBy);
                return new DBHelper().ExecuteNonQueryReturn("sp_DeleteSlagSamplesBySlagID", p);
            }
            catch
            {
                return 0;
            }
        }

        public int InsertSlagSample(SlagSampleAnalysisBLL data)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[21];
                p[0] = new SqlParameter("@SlagID", data.SlagID);
                p[1] = new SqlParameter("@SampleCode", data.SampleCode ?? (object)DBNull.Value);
                p[2] = new SqlParameter("@SampleTime", data.SampleTime ?? (object)DBNull.Value);
                p[3] = new SqlParameter("@CaO", data.CaO ?? (object)DBNull.Value);
                p[4] = new SqlParameter("@MgO", data.MgO ?? (object)DBNull.Value);
                p[5] = new SqlParameter("@SiO2", data.SiO2 ?? (object)DBNull.Value);
                p[6] = new SqlParameter("@Al2O3", data.Al2O3 ?? (object)DBNull.Value);
                p[7] = new SqlParameter("@Fe2O3", data.Fe2O3 ?? (object)DBNull.Value);
                p[8] = new SqlParameter("@S", data.S ?? (object)DBNull.Value);
                p[9] = new SqlParameter("@MnO", data.MnO ?? (object)DBNull.Value);
                p[10] = new SqlParameter("@Cr2O3", data.Cr2O3 ?? (object)DBNull.Value);
                p[11] = new SqlParameter("@P2O5", data.P2O5 ?? (object)DBNull.Value);
                p[12] = new SqlParameter("@V2O5", data.V2O5 ?? (object)DBNull.Value);
                p[13] = new SqlParameter("@TiO2", data.TiO2 ?? (object)DBNull.Value);
                p[14] = new SqlParameter("@ZnO", data.ZnO ?? (object)DBNull.Value);
                p[15] = new SqlParameter("@TotalFe", data.TotalFe ?? (object)DBNull.Value);
                p[16] = new SqlParameter("@Basicity4", data.Basicity4 ?? (object)DBNull.Value);
                p[17] = new SqlParameter("@Comment", data.Comment ?? (object)DBNull.Value);
                p[18] = new SqlParameter("@StatusID", data.StatusID ?? (object)DBNull.Value);
                p[19] = new SqlParameter("@CreatedBy", data.CreatedBy ?? (object)DBNull.Value);
                p[20] = new SqlParameter("@CreatedDate", data.CreatedDate ?? (object)DBNull.Value);
                return new DBHelper().ExecuteNonQueryReturn("sp_InsertSlagSampleAnalysis", p);
            }
            catch
            {
                return 0;
            }
        }

        public List<SlagByProductAnalysisBLL> GetSlagByProduct()
        {
            try
            {
                var lst = new List<SlagByProductAnalysisBLL>();
                SqlParameter[] p = new SqlParameter[0];
                _dt = new DBHelper().GetTableFromSP("sp_GetSlagByProduct", p);
                if (_dt != null && _dt.Rows.Count > 0)
                    lst = JArray.Parse(JsonConvert.SerializeObject(_dt)).ToObject<List<SlagByProductAnalysisBLL>>();
                return lst;
            }
            catch
            {
                return null;
            }
        }

        public int SlagByProductDelete(int ID, string UpdatedBy)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[4];
                p[0] = new SqlParameter("@ID", ID);
                p[1] = new SqlParameter("@StatusID", 3);
                p[2] = new SqlParameter("@UpdatedDate", DateTime.Now);
                p[3] = new SqlParameter("@UpdatedBy", UpdatedBy);
                return new DBHelper().ExecuteNonQueryReturn("sp_DeleteSlagByProduct", p);
            }
            catch
            {
                return 0;
            }
        }

        public int AddBundlingSection(BundlingSectionBLL data)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[18];
                return new DBHelper().ExecuteNonQueryReturn("sp_InsertDRISampleAnalysis", p);
            }
            catch
            {
                return 0;
            }
        }

        public SlagByProductAnalysisBLL GetSlagByID(int? id)
        {
            try
            {
                SlagByProductAnalysisBLL obj = null;
                SqlParameter[] p = { new SqlParameter("@id", id) };
                _dt = new DBHelper().GetTableFromSP("sp_GetSlagByProductByID", p);
                if (_dt != null && _dt.Rows.Count > 0)
                    obj = JArray.Parse(JsonConvert.SerializeObject(_dt)).ToObject<List<SlagByProductAnalysisBLL>>().FirstOrDefault();
                return obj;
            }
            catch
            {
                return null;
            }
        }

        public List<SlagSampleAnalysisBLL> GetSlagSamplesById(int id)
        {
            try
            {
                var lst = new List<SlagSampleAnalysisBLL>();
                SqlParameter[] p = { new SqlParameter("@id", id) };
                _dt = new DBHelper().GetTableFromSP("sp_GetSlagSampleByID", p);
                if (_dt != null && _dt.Rows.Count > 0)
                    lst = JArray.Parse(JsonConvert.SerializeObject(_dt)).ToObject<List<SlagSampleAnalysisBLL>>();
                return lst;
            }
            catch
            {
                return null;
            }
        }

        public List<SlagByProductAnalysisBLL> GetSlagByProductByDate(DateTime fromDate, DateTime toDateExclusive)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@FromDate", SqlDbType.Date) { Value = fromDate.Date },
                    new SqlParameter("@ToDate", SqlDbType.Date) { Value = toDateExclusive.Date }
                };
                DataTable dt = new DBHelper().GetTableFromSP("sp_GetSlagByProduct_ByDate", p);
                return dt != null && dt.Rows.Count > 0
                    ? JsonConvert.DeserializeObject<List<SlagByProductAnalysisBLL>>(JsonConvert.SerializeObject(dt))
                    : new List<SlagByProductAnalysisBLL>();
            }
            catch
            {
                return new List<SlagByProductAnalysisBLL>();
            }
        }

        public List<SlagSampleAnalysisBLL> GetSlagSamplesByDate(DateTime fromDate, DateTime toDateExclusive)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@FromDate", SqlDbType.Date) { Value = fromDate.Date },
                    new SqlParameter("@ToDate", SqlDbType.Date) { Value = toDateExclusive.Date }
                };
                DataTable dt = new DBHelper().GetTableFromSP("sp_GetSlagSample_ByDate", p);
                return dt != null && dt.Rows.Count > 0
                    ? JsonConvert.DeserializeObject<List<SlagSampleAnalysisBLL>>(JsonConvert.SerializeObject(dt))
                    : new List<SlagSampleAnalysisBLL>();
            }
            catch
            {
                return new List<SlagSampleAnalysisBLL>();
            }
        }

        public List<BilletBoardBLL> GetBilletBoardingByDate(DateTime fromDate, DateTime toDateExclusive)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@FromDate", SqlDbType.Date) { Value = fromDate.Date },
                    new SqlParameter("@ToDate", SqlDbType.Date) { Value = toDateExclusive.Date }
                };
                DataTable dt = new DBHelper().GetTableFromSP("sp_GetBilletBoarding_ByDate", p);
                return dt != null && dt.Rows.Count > 0
                    ? JsonConvert.DeserializeObject<List<BilletBoardBLL>>(JsonConvert.SerializeObject(dt))
                    : new List<BilletBoardBLL>();
            }
            catch
            {
                return new List<BilletBoardBLL>();
            }
        }

        public List<HeatChemistryBLL> GetHeatChemistryByDate(DateTime fromDate, DateTime toDateExclusive)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@FromDate", SqlDbType.Date) { Value = fromDate.Date },
                    new SqlParameter("@ToDate", SqlDbType.Date) { Value = toDateExclusive.Date }
                };
                DataTable dt = new DBHelper().GetTableFromSP("sp_GetHeatChemistry_ByDate", p);
                return dt != null && dt.Rows.Count > 0
                    ? JsonConvert.DeserializeObject<List<HeatChemistryBLL>>(JsonConvert.SerializeObject(dt))
                    : new List<HeatChemistryBLL>();
            }
            catch
            {
                return new List<HeatChemistryBLL>();
            }
        }

        // ============================================================
        // NEW BUNDLING QC SAVE
        // IMPORTANT: returns inserted ID, NOT bool.
        // ============================================================
        public int SaveQCInspection(RMQCInspectionBLL model)
        {
            try
            {
                if (model == null)
                    return 0;

                SqlParameter[] parameters =
                {
                    new SqlParameter("@ProductionDate", SqlDbType.Date)
                    {
                        Value = model.ProductionDate == DateTime.MinValue
                            ? (object)DBNull.Value
                            : model.ProductionDate?.Date
                    },
                    new SqlParameter("@Shift", SqlDbType.NVarChar, 50) { Value = DbValue(model.Shift) },
                    new SqlParameter("@HeatNo", SqlDbType.NVarChar, 100) { Value = DbValue(model.HeatNo) },
                    new SqlParameter("@SteelGrade", SqlDbType.NVarChar, 100) { Value = DbValue(model.SteelGrade) },
                    new SqlParameter("@BarSize", SqlDbType.NVarChar, 100) { Value = DbValue(model.BarSize) },
                    new SqlParameter("@TotalBundles", SqlDbType.Int) { Value = model.TotalBundles },
                    new SqlParameter("@OnHold", SqlDbType.Int) { Value = model.OnHold },
                    new SqlParameter("@Rejected", SqlDbType.Int) { Value = model.Rejected },
                    new SqlParameter("@Accepted", SqlDbType.Int) { Value = model.Accepted },
                    new SqlParameter("@BundleSeriesOnHold", SqlDbType.NVarChar, 500) { Value = DbValue(model.BundleSeriesOnHold) },
                    new SqlParameter("@DefectCodes", SqlDbType.NVarChar, 500) { Value = DbValue(model.DefectCodes) },
                    new SqlParameter("@MRBNo", SqlDbType.NVarChar, 100) { Value = DbValue(model.MRBNo) },
                    new SqlParameter("@QCStatus", SqlDbType.NVarChar, 50) { Value = DbValue(model.QCStatus) },
                    new SqlParameter("@Remarks", SqlDbType.NVarChar, -1) { Value = DbValue(model.Remarks) },
                    new SqlParameter("@CreatedOn", SqlDbType.DateTime)
                    {
                        Value = model.CreatedOn == DateTime.MinValue ? DateTime.Now : model.CreatedOn
                    },
                    new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100) { Value = DbValue(model.CreatedBy) },
                    new SqlParameter("@StatusID", SqlDbType.Int)
                    {
                        Value = model.StatusID <= 0 ? 1 : model.StatusID
                    }
                };

                object result = new DBHelper().ExecuteScalar("sp_QC_InsertRMInspection", parameters);

                if (result == null || result == DBNull.Value)
                    return 0;

                int savedID;
                return int.TryParse(Convert.ToString(result), out savedID) ? savedID : 0;
            }
            catch
            {
                throw;
            }
        }

        // ============================================================
        // RM MECHANICAL / MTC
        // ============================================================
        public List<QCBilletBoardingRowBLL> GetBilletBoardingRows(string rollingMill)
        {
            var list = new List<QCBilletBoardingRowBLL>();
            SqlParameter[] parameters =
            {
                new SqlParameter("@RollingMill", SqlDbType.NVarChar, 20)
                {
                    Value = string.IsNullOrWhiteSpace(rollingMill) ? "RM1" : rollingMill.Trim()
                }
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_QC_GetBilletBoardingRows",
                CommandType.StoredProcedure,
                parameters
            );

            if (dt == null) return list;

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new QCBilletBoardingRowBLL
                {
                    ID = GetInt(row, "ID"),
                    Site = GetString(row, "Site"),
                    BoardingNo = GetString(row, "BoardingNo"),
                    SerialNo = GetInt(row, "SerialNo"),
                    HeatNo = GetString(row, "HeatNo"),
                    SteelGrade = GetString(row, "SteelGrade"),
                    BarSize = GetString(row, "BarSize"),
                    BarsPerBundle = GetInt(row, "BarsPerBundle"),
                    ActualBundleCount = GetInt(row, "ActualBundleCount"),
                    YardInspection = GetString(row, "YardInspection"),
                    YardInspectionRemarks = GetString(row, "YardInspectionRemarks")
                });
            }

            return list;
        }

        public List<QCMTCRowBLL> GetMTCRows(string heatNo = null)
        {
            var list = new List<QCMTCRowBLL>();
            SqlParameter[] parameters =
            {
                new SqlParameter("@HeatNo", SqlDbType.NVarChar, 50)
                {
                    Value = string.IsNullOrWhiteSpace(heatNo)
                        ? (object)DBNull.Value
                        : heatNo.Trim()
                }
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_QC_GetMTCRows",
                CommandType.StoredProcedure,
                parameters
            );

            if (dt == null || dt.Rows.Count == 0)
                return list;

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new QCMTCRowBLL
                {
                    ID = GetInt(row, "ID"),
                    HeatNo = GetString(row, "HeatNo"),
                    SteelGrade = GetString(row, "SteelGrade"),
                    BarSize = GetDecimal(row, "BarSize"),
                    YieldStress = GetDecimal(row, "YieldStress"),
                    TensileStress = GetDecimal(row, "TensileStress"),
                    NoOfBundles = GetInt(row, "NoOfBundles"),
                    YSTSRatio = GetDecimal(row, "YSTSRatio")
                });
            }

            return list
                .Where(x => !string.IsNullOrWhiteSpace(x.HeatNo))
                .GroupBy(x => x.HeatNo.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g => g.OrderByDescending(x => x.ID).First())
                .OrderByDescending(x => x.ID)
                .ToList();
        }

        public QCInspectionRMDetailBLL GetMTCDetails(int mtcID)
        {
            if (mtcID <= 0) return null;

            SqlParameter[] parameters =
            {
                new SqlParameter("@MTCID", SqlDbType.Int) { Value = mtcID }
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_QC_GetMTCDetails",
                CommandType.StoredProcedure,
                parameters
            );

            if (dt == null || dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            var model = new QCInspectionRMDetailBLL
            {
                ID = GetInt(row, "ID"),
                MTCID = GetInt(row, "MTCID"),
                BilletBoardingID = GetInt(row, "BilletBoardingID"),
                Site = GetString(row, "Site"),
                ProductionShift = GetString(row, "ProductionShift"),
                HeatNo = GetString(row, "HeatNo"),
                Specification = GetString(row, "Specification"),
                SteelGrade = GetString(row, "SteelGrade"),
                BarSize = Convert.ToString(GetDecimal(row, "BarSize")),
                Length = Convert.ToString(GetDecimal(row, "Length")),
                WeightPerBundle = Convert.ToString(GetDecimal(row, "WeightPerBundle")),
                NominalWeight = Convert.ToString(GetDecimal(row, "NominalWeight")),
                CrossSectionArea = Convert.ToString(GetDecimal(row, "CrossSectionArea")),
                NoOfBarsPerBundle = Convert.ToString(GetInt(row, "NoOfBarsPerBundle")),
                NoOfBundles = Convert.ToString(GetInt(row, "NoOfBundles")),
                BendTestObserved = GetBool(row, "BendTestObserved"),
                IsWireRodOrCoil = GetBool(row, "IsWireRodOrCoil"),
                YieldStrength = Convert.ToString(GetDecimal(row, "YieldStrength")),
                TensileStrength = Convert.ToString(GetDecimal(row, "TensileStrength")),
                TensileYieldRatio = Convert.ToString(GetDecimal(row, "TensileYieldRatio")),
                Elongation = Convert.ToString(GetDecimal(row, "Elongation")),
                GaugeLength = Convert.ToString(GetDecimal(row, "GaugeLength")),
                C = Convert.ToString(GetDecimal(row, "C")),
                Si = Convert.ToString(GetDecimal(row, "Si")),
                Mn = Convert.ToString(GetDecimal(row, "Mn")),
                P = Convert.ToString(GetDecimal(row, "P")),
                S = Convert.ToString(GetDecimal(row, "S")),
                N = Convert.ToString(GetDecimal(row, "N")),
                Ceq = Convert.ToString(GetDecimal(row, "Ceq"))
            };

            DateTime productionDate;
            if (row.Table.Columns.Contains("ProductionDate")
                && row["ProductionDate"] != DBNull.Value
                && DateTime.TryParse(Convert.ToString(row["ProductionDate"]), out productionDate))
            {
                model.ProductionDate = productionDate.ToString("dd-MM-yyyy");
                model.ProductionDateValue = productionDate;
            }

            return model;
        }

        public QCInspectionRMDetailBLL GetQCInspectionRMByID(int id)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", SqlDbType.Int) { Value = id }
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_QC_GetInspectionRMByID",
                CommandType.StoredProcedure,
                parameters
            );

            if (dt == null || dt.Rows.Count == 0)
                return null;

            return MapInspection(dt.Rows[0]);
        }

        public QCInspectionRMDetailBLL GetQCInspectionRMFromBoarding(int boardingID)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@BoardingID", SqlDbType.Int) { Value = boardingID }
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_QC_GetInspectionRMFromBoarding",
                CommandType.StoredProcedure,
                parameters
            );

            if (dt == null || dt.Rows.Count == 0)
                return null;

            return MapInspection(dt.Rows[0]);
        }

        public QCMTCDetailBLL GetMTCDetail(string heatNo)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@HeatNo", SqlDbType.NVarChar, 50)
                {
                    Value = heatNo
                }
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_QC_GetMTCDetail",
                CommandType.StoredProcedure,
                parameters
            );

            if (dt == null || dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];
            return new QCMTCDetailBLL
            {
                HeatNo = GetString(row, "HeatNo"),
                YieldStrength = GetDecimal(row, "YieldStrength"),
                TensileStrength = GetDecimal(row, "TensileStrength"),
                TensileYieldRatio = GetDecimal(row, "TensileYieldRatio"),
                Elongation = GetDecimal(row, "Elongation"),
                GaugeLength = GetDecimal(row, "GaugeLength"),
                C = GetDecimal(row, "C"),
                Si = GetDecimal(row, "Si"),
                Mn = GetDecimal(row, "Mn"),
                P = GetDecimal(row, "P"),
                S = GetDecimal(row, "S"),
                N = GetDecimal(row, "N"),
                Ceq = GetDecimal(row, "Ceq")
            };
        }

        public QCInspectionRMDetailBLL GetCompleteMTCInspectionDetail(int mtcID)
        {
            var data = GetQCInspectionRMByID(mtcID);
            if (data == null) return null;

            if (data.BilletBoardingID > 0)
            {
                var boardingData = GetQCInspectionRMFromBoarding(data.BilletBoardingID);
                if (boardingData != null)
                {
                    if (string.IsNullOrWhiteSpace(data.Specification)) data.Specification = boardingData.Specification;
                    if (string.IsNullOrWhiteSpace(data.SteelGrade)) data.SteelGrade = boardingData.SteelGrade;
                    if (!data.BarSizeValue.HasValue) data.BarSizeValue = boardingData.BarSizeValue;
                    if (!data.LengthValue.HasValue) data.LengthValue = boardingData.LengthValue;
                    if (!data.WeightPerBundleValue.HasValue) data.WeightPerBundleValue = boardingData.WeightPerBundleValue;
                    if (!data.NominalWeightValue.HasValue) data.NominalWeightValue = boardingData.NominalWeightValue;
                    if (!data.CrossSectionAreaValue.HasValue) data.CrossSectionAreaValue = boardingData.CrossSectionAreaValue;
                    if (!data.NoOfBarsPerBundleValue.HasValue) data.NoOfBarsPerBundleValue = boardingData.NoOfBarsPerBundleValue;
                    if (!data.NoOfBundlesValue.HasValue) data.NoOfBundlesValue = boardingData.NoOfBundlesValue;
                }
            }

            if (!string.IsNullOrWhiteSpace(data.HeatNo))
            {
                var mtc = GetMTCDetail(data.HeatNo.Trim());
                if (mtc != null)
                {
                    if (mtc.YieldStrength != 0M) data.YieldStrengthValue = mtc.YieldStrength;
                    if (mtc.TensileStrength != 0M) data.TensileStrengthValue = mtc.TensileStrength;
                    if (mtc.TensileYieldRatio != 0M) data.TensileYieldRatioValue = mtc.TensileYieldRatio;
                    if (mtc.Elongation != 0M) data.ElongationValue = mtc.Elongation;
                    if (mtc.GaugeLength != 0M) data.GaugeLengthValue = mtc.GaugeLength;
                    data.CValue = mtc.C;
                    data.SiValue = mtc.Si;
                    data.MnValue = mtc.Mn;
                    data.PValue = mtc.P;
                    data.SValue = mtc.S;
                    data.NValue = mtc.N;
                    data.CeqValue = mtc.Ceq;
                }
            }

            return data;
        }

        // Existing Mechanical / MTC save. Keep separate from new Bundling QC save.
        public int SaveQCInspectionRM(QCInspectionRMDetailBLL model)
        {
            try
            {
                if (model == null)
                    return 0;

                SqlParameter resultParameter = new SqlParameter("@Result", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                SqlParameter[] parameters =
                {
                    new SqlParameter("@ID", SqlDbType.Int) { Value = model.ID },
                    new SqlParameter("@BilletBoardingID", SqlDbType.Int) { Value = model.BilletBoardingID },
                    new SqlParameter("@MTCID", SqlDbType.Int) { Value = model.MTCID },
                    new SqlParameter("@Site", SqlDbType.NVarChar, 50) { Value = DbValue(model.Site) },
                    new SqlParameter("@ProductionShift", SqlDbType.NVarChar, 50) { Value = DbValue(model.ProductionShift) },
                    new SqlParameter("@ProductionDate", SqlDbType.Date)
                    {
                        Value = model.ProductionDateValue.HasValue
                            ? (object)model.ProductionDateValue.Value.Date
                            : DBNull.Value
                    },
                    new SqlParameter("@HeatNo", SqlDbType.NVarChar, 100) { Value = DbValue(model.HeatNo) },
                    new SqlParameter("@Specification", SqlDbType.NVarChar, 200) { Value = DbValue(model.Specification) },
                    new SqlParameter("@SteelGrade", SqlDbType.NVarChar, 100) { Value = DbValue(model.SteelGrade) },
                    CreateNullableDecimalParameter("@Length", model.LengthValue),
                    CreateNullableDecimalParameter("@NominalWeight", model.NominalWeightValue),
                    CreateNullableDecimalParameter("@CrossSectionArea", model.CrossSectionAreaValue),
                    new SqlParameter("@BendTestObserved", SqlDbType.Bit) { Value = model.BendTestObserved },
                    CreateNullableDecimalParameter("@BarSize", model.BarSizeValue),
                    CreateNullableDecimalParameter("@WeightPerBundle", model.WeightPerBundleValue),
                    new SqlParameter("@NoOfBarsPerBundle", SqlDbType.Int)
                    {
                        Value = model.NoOfBarsPerBundleValue.HasValue
                            ? (object)model.NoOfBarsPerBundleValue.Value
                            : DBNull.Value
                    },
                    new SqlParameter("@NoOfBundles", SqlDbType.Int)
                    {
                        Value = model.NoOfBundlesValue.HasValue
                            ? (object)model.NoOfBundlesValue.Value
                            : DBNull.Value
                    },
                    new SqlParameter("@IsWireRodOrCoil", SqlDbType.Bit) { Value = model.IsWireRodOrCoil },
                    CreateNullableDecimalParameter("@YieldStrength", model.YieldStrengthValue),
                    CreateNullableDecimalParameter("@TensileStrength", model.TensileStrengthValue),
                    CreateNullableDecimalParameter("@TensileYieldRatio", model.TensileYieldRatioValue),
                    CreateNullableDecimalParameter("@Elongation", model.ElongationValue),
                    CreateNullableDecimalParameter("@GaugeLength", model.GaugeLengthValue),
                    CreateNullableDecimalParameter("@C", model.CValue),
                    CreateNullableDecimalParameter("@Si", model.SiValue),
                    CreateNullableDecimalParameter("@Mn", model.MnValue),
                    CreateNullableDecimalParameter("@P", model.PValue),
                    CreateNullableDecimalParameter("@S", model.SValue),
                    CreateNullableDecimalParameter("@N", model.NValue),
                    CreateNullableDecimalParameter("@Ceq", model.CeqValue),
                    new SqlParameter("@StatusID", SqlDbType.Int) { Value = 1 },
                    new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100) { Value = DbValue(model.CreatedBy) },
                    new SqlParameter("@CreatedDate", SqlDbType.DateTime)
                    {
                        Value = model.CreatedDate.HasValue
                            ? (object)model.CreatedDate.Value
                            : DateTime.Now
                    },
                    resultParameter
                };

                DBHelper.ExecuteNonQuery(
                    "sp_QC_SaveInspectionRM",
                    CommandType.StoredProcedure,
                    parameters
                );

                if (resultParameter.Value == null || resultParameter.Value == DBNull.Value)
                    return 0;

                int result;
                return int.TryParse(Convert.ToString(resultParameter.Value), out result) ? result : 0;
            }
            catch
            {
                throw;
            }
        }

        public int DeleteQCInspectionRM(int id, string userName)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", id),
                new SqlParameter("@UpdatedBy", DbValue(userName))
            };

            return DBHelper.ExecuteNonQuery(
                "sp_QC_DeleteInspectionRM",
                CommandType.StoredProcedure,
                parameters
            );
        }

        private QCInspectionRMDetailBLL MapInspection(DataRow row)
        {
            var model = new QCInspectionRMDetailBLL
            {
                ID = GetInt(row, "ID"),
                BilletBoardingID = GetInt(row, "BilletBoardingID"),
                MTCID = GetInt(row, "MTCID"),
                Site = GetString(row, "Site"),
                ProductionShift = GetString(row, "ProductionShift"),
                ProductionDateValue = GetNullableDate(row, "ProductionDate"),
                HeatNo = GetString(row, "HeatNo"),
                Specification = GetString(row, "Specification"),
                SteelGrade = GetString(row, "SteelGrade"),
                LengthValue = GetNullableDecimal(row, "Length"),
                NominalWeightValue = GetNullableDecimal(row, "NominalWeight"),
                CrossSectionAreaValue = GetNullableDecimal(row, "CrossSectionArea"),
                BendTestObserved = GetBool(row, "BendTestObserved"),
                BarSizeValue = GetNullableDecimal(row, "BarSize"),
                WeightPerBundleValue = GetNullableDecimal(row, "WeightPerBundle"),
                NoOfBarsPerBundleValue = GetNullableInt(row, "NoOfBarsPerBundle"),
                NoOfBundlesValue = GetNullableInt(row, "NoOfBundles"),
                IsWireRodOrCoil = GetBool(row, "IsWireRodOrCoil"),
                YieldStrengthValue = GetNullableDecimal(row, "YieldStrength"),
                TensileStrengthValue = GetNullableDecimal(row, "TensileStrength"),
                TensileYieldRatioValue = GetNullableDecimal(row, "TensileYieldRatio"),
                ElongationValue = GetNullableDecimal(row, "Elongation"),
                GaugeLengthValue = GetNullableDecimal(row, "GaugeLength"),
                CValue = GetNullableDecimal(row, "C"),
                SiValue = GetNullableDecimal(row, "Si"),
                MnValue = GetNullableDecimal(row, "Mn"),
                PValue = GetNullableDecimal(row, "P"),
                SValue = GetNullableDecimal(row, "S"),
                NValue = GetNullableDecimal(row, "N"),
                CeqValue = GetNullableDecimal(row, "Ceq")
            };

            if (model.ProductionDateValue.HasValue)
                model.ProductionDate = model.ProductionDateValue.Value.ToString("dd-MM-yyyy");

            return model;
        }

        public RMChemicalAnalysisBLL GetChemicalAnalysisByHeatNo(string heatNo)
        {
            if (string.IsNullOrWhiteSpace(heatNo)) return null;

            SqlParameter[] parameters =
            {
                new SqlParameter("@HeatNo", SqlDbType.NVarChar, 50) { Value = heatNo.Trim() }
            };

            DataTable dt = DBHelper.ExecuteDataTable(
                "sp_QC_GetChemicalAnalysisByHeat",
                CommandType.StoredProcedure,
                parameters
            );

            if (dt == null || dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];
            return new RMChemicalAnalysisBLL
            {
                ID = row["ID"] == DBNull.Value ? 0 : Convert.ToInt32(row["ID"]),
                HeatNo = Convert.ToString(row["HeatNo"]).Trim(),
                NoOfBillets = row["NoOfBillets"] == DBNull.Value ? 0 : Convert.ToInt32(row["NoOfBillets"]),
                C = GetNullableDecimal(row, "C"),
                Si = GetNullableDecimal(row, "Si"),
                Mn = GetNullableDecimal(row, "Mn"),
                S = GetNullableDecimal(row, "S"),
                P = GetNullableDecimal(row, "P"),
                N = GetNullableDecimal(row, "N"),
                Ceq = GetNullableDecimal(row, "Ceq"),
                HeatStatus = GetInt(row, "HeatStatus")
            };
        }

        // ============================================================
        // BILLET BOARD - ADD / EDIT SUPPORT
        // ============================================================
        public BilletBoardBLL GetBilletForEdit(int id)
        {
            try
            {
                SqlParameter[] p = { new SqlParameter("@ID", SqlDbType.Int) { Value = id } };
                DataTable dt = new DBHelper().GetTableFromSP("sp_GetBilletForEdit", p);
                if (dt == null || dt.Rows.Count == 0) return null;
                return JArray.Parse(JsonConvert.SerializeObject(dt)).ToObject<List<BilletBoardBLL>>().FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public List<RMChemicalAnalysisBLL> GetBilletChemistryForEdit(int id)
        {
            try
            {
                SqlParameter[] p = { new SqlParameter("@ID", SqlDbType.Int) { Value = id } };
                DataTable dt = new DBHelper().GetTableFromSP("sp_GetBilletChemistryForEdit", p);
                return dt != null && dt.Rows.Count > 0
                    ? JArray.Parse(JsonConvert.SerializeObject(dt)).ToObject<List<RMChemicalAnalysisBLL>>()
                    : new List<RMChemicalAnalysisBLL>();
            }
            catch
            {
                throw;
            }
        }

        public bool IsBilletBoardingExistsForEdit(string billetBoarding, int currentID)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@BilletBoarding", SqlDbType.NVarChar, 100)
                    {
                        Value = string.IsNullOrWhiteSpace(billetBoarding)
                            ? (object)DBNull.Value
                            : billetBoarding.Trim()
                    },
                    new SqlParameter("@CurrentID", SqlDbType.Int) { Value = currentID }
                };

                DataTable dt = new DBHelper().GetTableFromSP("sp_CheckBilletBoardingExistsForEdit", p);
                return dt != null && dt.Rows.Count > 0 && Convert.ToInt32(dt.Rows[0]["Total"]) > 0;
            }
            catch
            {
                throw;
            }
        }

        public List<string> GetDuplicateHeatNosForEdit(List<string> heatNos, int currentID)
        {
            try
            {
                var duplicates = new List<string>();
                if (heatNos == null || heatNos.Count == 0) return duplicates;

                string heatNoCsv = string.Join(",", heatNos
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim()));

                SqlParameter[] p =
                {
                    new SqlParameter("@HeatNos", SqlDbType.NVarChar, -1) { Value = heatNoCsv },
                    new SqlParameter("@CurrentID", SqlDbType.Int) { Value = currentID }
                };

                DataTable dt = new DBHelper().GetTableFromSP("sp_GetDuplicateHeatNosForEdit", p);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string heatNo = Convert.ToString(row["HeatNo"]);
                        if (!string.IsNullOrWhiteSpace(heatNo)) duplicates.Add(heatNo.Trim());
                    }
                }

                return duplicates;
            }
            catch
            {
                throw;
            }
        }

        public int DeactivateBilletChemistry(int currentID, string updatedBy)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@CurrentID", SqlDbType.Int) { Value = currentID },
                    new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100)
                    {
                        Value = string.IsNullOrWhiteSpace(updatedBy) ? (object)DBNull.Value : updatedBy.Trim()
                    }
                };
                return new DBHelper().ExecuteNonQueryReturn("sp_DeactivateBilletChemistry", p);
            }
            catch
            {
                throw;
            }
        }

        public int DeactivateBilletBoardHeatRows(int currentID, string updatedBy)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@CurrentID", SqlDbType.Int) { Value = currentID },
                    new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100)
                    {
                        Value = string.IsNullOrWhiteSpace(updatedBy) ? (object)DBNull.Value : updatedBy.Trim()
                    }
                };
                return new DBHelper().ExecuteNonQueryReturn("sp_DeactivateBilletBoardHeatRows", p);
            }
            catch
            {
                throw;
            }
        }

        public List<string> GetDuplicateHeatNosForHeatChange(List<string> newHeatNos, int currentID, List<string> oldHeatNos)
        {
            try
            {
                var duplicates = new List<string>();
                if (newHeatNos == null || newHeatNos.Count == 0) return duplicates;

                string newHeatCsv = string.Join(",", newHeatNos
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase));

                string oldHeatCsv = oldHeatNos == null
                    ? ""
                    : string.Join(",", oldHeatNos
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase));

                SqlParameter[] p =
                {
                    new SqlParameter("@NewHeatNos", SqlDbType.NVarChar, -1) { Value = newHeatCsv },
                    new SqlParameter("@CurrentID", SqlDbType.Int) { Value = currentID },
                    new SqlParameter("@OldHeatNos", SqlDbType.NVarChar, -1)
                    {
                        Value = string.IsNullOrWhiteSpace(oldHeatCsv) ? (object)DBNull.Value : oldHeatCsv
                    }
                };

                DataTable dt = new DBHelper().GetTableFromSP("sp_ValidateBilletBoardHeatChanges", p);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string heatNo = Convert.ToString(row["HeatNo"]);
                        if (!string.IsNullOrWhiteSpace(heatNo)) duplicates.Add(heatNo.Trim());
                    }
                }

                return duplicates;
            }
            catch
            {
                throw;
            }
        }

        public int UpdateBilletBoardHeatNo(int currentID, string oldHeatNo, string newHeatNo, string updatedBy)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@CurrentID", SqlDbType.Int) { Value = currentID },
                    new SqlParameter("@OldHeatNo", SqlDbType.NVarChar, 100)
                    {
                        Value = string.IsNullOrWhiteSpace(oldHeatNo) ? (object)DBNull.Value : oldHeatNo.Trim()
                    },
                    new SqlParameter("@NewHeatNo", SqlDbType.NVarChar, 100)
                    {
                        Value = string.IsNullOrWhiteSpace(newHeatNo) ? (object)DBNull.Value : newHeatNo.Trim()
                    },
                    new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100)
                    {
                        Value = string.IsNullOrWhiteSpace(updatedBy) ? (object)DBNull.Value : updatedBy.Trim()
                    }
                };

                DataTable dt = new DBHelper().GetTableFromSP("sp_UpdateBilletBoardHeatNo", p);
                if (dt != null && dt.Rows.Count > 0 && dt.Columns.Contains("ID"))
                    return Convert.ToInt32(dt.Rows[0]["ID"]);
                return currentID;
            }
            catch
            {
                throw;
            }
        }

        public List<string> GetDuplicateHeatNosExcludingIDs(List<string> heatNos, List<int> excludedIDs)
        {
            try
            {
                var duplicates = new List<string>();
                if (heatNos == null || !heatNos.Any()) return duplicates;

                string heatNoCsv = string.Join(",", heatNos
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase));

                string excludedIDCsv = excludedIDs == null
                    ? string.Empty
                    : string.Join(",", excludedIDs.Where(x => x > 0).Distinct());

                SqlParameter[] p =
                {
                    new SqlParameter("@HeatNos", SqlDbType.NVarChar, -1) { Value = heatNoCsv },
                    new SqlParameter("@ExcludedIDs", SqlDbType.NVarChar, -1)
                    {
                        Value = string.IsNullOrWhiteSpace(excludedIDCsv)
                            ? (object)DBNull.Value
                            : excludedIDCsv
                    }
                };

                DataTable dt = new DBHelper().GetTableFromSP("sp_GetDuplicateRMHeatNosExcludingIDs", p);
                if (dt != null && dt.Rows.Count > 0)
                {
                    duplicates = dt.AsEnumerable()
                        .Select(x => Convert.ToString(x["HeatNo"]))
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }

                return duplicates;
            }
            catch
            {
                throw;
            }
        }

        public int UpdateChemicalAnalysisRM(RMChemicalAnalysisBLL model, int srNo)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@ID", SqlDbType.Int) { Value = model.ID },
                    new SqlParameter("@SrNo", SqlDbType.Int) { Value = srNo },
                    new SqlParameter("@HeatNo", SqlDbType.NVarChar, 100)
                    {
                        Value = string.IsNullOrWhiteSpace(model.HeatNo) ? (object)DBNull.Value : model.HeatNo.Trim()
                    },
                    new SqlParameter("@NoOfBillets", SqlDbType.Int) { Value = (object)model.NoOfBillets ?? DBNull.Value },
                    new SqlParameter("@C", SqlDbType.Decimal) { Value = (object)model.C ?? DBNull.Value },
                    new SqlParameter("@Si", SqlDbType.Decimal) { Value = (object)model.Si ?? DBNull.Value },
                    new SqlParameter("@Mn", SqlDbType.Decimal) { Value = (object)model.Mn ?? DBNull.Value },
                    new SqlParameter("@S", SqlDbType.Decimal) { Value = (object)model.S ?? DBNull.Value },
                    new SqlParameter("@P", SqlDbType.Decimal) { Value = (object)model.P ?? DBNull.Value },
                    new SqlParameter("@N", SqlDbType.Decimal) { Value = (object)model.N ?? DBNull.Value },
                    new SqlParameter("@Ceq", SqlDbType.Decimal) { Value = (object)model.Ceq ?? DBNull.Value },
                    new SqlParameter("@HeatStatus", SqlDbType.Int) { Value = (object)model.HeatStatus ?? DBNull.Value },
                    new SqlParameter("@StatusID", SqlDbType.Int) { Value = model.StatusID },
                    new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100)
                    {
                        Value = string.IsNullOrWhiteSpace(model.UpdatedBy) ? (object)DBNull.Value : model.UpdatedBy.Trim()
                    },
                    new SqlParameter("@UpdatedDate", SqlDbType.DateTime)
                    {
                        Value = model.UpdatedDate.HasValue ? (object)model.UpdatedDate.Value : DateTime.Now
                    }
                };

                return new DBHelper().ExecuteNonQueryReturn("sp_UpdateRMChemicalAnalysis", p);
            }
            catch
            {
                throw;
            }
        }

        public int DeactivateChemicalAnalysisRM(int chemistryID, string updatedBy, DateTime updatedDate)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@ID", SqlDbType.Int) { Value = chemistryID },
                    new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100)
                    {
                        Value = string.IsNullOrWhiteSpace(updatedBy) ? (object)DBNull.Value : updatedBy.Trim()
                    },
                    new SqlParameter("@UpdatedDate", SqlDbType.DateTime) { Value = updatedDate }
                };

                return new DBHelper().ExecuteNonQueryReturn("sp_DeactivateRMChemicalAnalysisByID", p);
            }
            catch
            {
                throw;
            }
        }

        public int UpdateBilletBoarding(BilletBoardBLL model)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@ID", SqlDbType.Int) { Value = model.ID },
                    new SqlParameter("@Date", SqlDbType.Date)
                    {
                        Value = model.Date.HasValue ? (object)model.Date.Value : DBNull.Value
                    },
                    new SqlParameter("@BilletBoarding", SqlDbType.NVarChar, 100) { Value = DbValue(model.BilletBoarding) },
                    new SqlParameter("@PlantName", SqlDbType.NVarChar, 100) { Value = DbValue(model.PlantName) },
                    new SqlParameter("@Shift", SqlDbType.NVarChar, 50) { Value = DbValue(model.Shift) },
                    new SqlParameter("@SteelGrade", SqlDbType.NVarChar, 100) { Value = DbValue(model.SteelGrade) },
                    new SqlParameter("@Profile", SqlDbType.NVarChar, 100) { Value = DbValue(model.Profile) },
                    new SqlParameter("@Size", SqlDbType.NVarChar, 100) { Value = DbValue(model.Size) },
                    new SqlParameter("@ProductSpecs", SqlDbType.NVarChar, 200) { Value = DbValue(model.ProductSpecs) },
                    new SqlParameter("@BilletLength", SqlDbType.NVarChar, 50) { Value = DbValue(model.BilletLength) },
                    new SqlParameter("@CrossSection", SqlDbType.NVarChar, 100) { Value = DbValue(model.CrossSection) },
                    new SqlParameter("@BilletWeight", SqlDbType.Decimal) { Value = model.BilletWeight },
                    new SqlParameter("@Remarks", SqlDbType.NVarChar, -1) { Value = DbValue(model.Remarks) },
                    new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = DbValue(model.UpdatedBy) }
                };

                DataTable dt = new DBHelper().GetTableFromSP("sp_UpdateBilletBoarding", p);
                if (dt != null && dt.Rows.Count > 0 && dt.Columns.Contains("ID"))
                    return Convert.ToInt32(dt.Rows[0]["ID"]);
                return model.ID;
            }
            catch
            {
                throw;
            }
        }

        public void UpdateBillet(BilletBoardBLL model)
        {
            UpdateBilletBoarding(model);
        }

        // ============================================================
        // HBI / DRI
        // ============================================================
        public List<QCHBIDRIAnalysisBLL> GetDRIHBIAnalysis()
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[0];
                DataTable dt = new DBHelper().GetTableFromSP("sp_GetSampleHBIDRI", parameters);
                return dt != null && dt.Rows.Count > 0
                    ? JArray.Parse(JsonConvert.SerializeObject(dt)).ToObject<List<QCHBIDRIAnalysisBLL>>()
                    : new List<QCHBIDRIAnalysisBLL>();
            }
            catch
            {
                throw;
            }
        }

        public QCHBIDRIAnalysisBLL GetDRIHBIDetailByID(int? id)
        {
            try
            {
                if (!id.HasValue || id.Value <= 0) return null;

                SqlParameter[] parameters =
                {
                    new SqlParameter("@ID", SqlDbType.Int) { Value = id.Value }
                };

                DataSet ds = new DBHelper().GetDatasetFromSP("sp_GetDRIHBIDetailByID", parameters);
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                    return null;

                DataRow master = ds.Tables[0].Rows[0];
                var model = new QCHBIDRIAnalysisBLL
                {
                    ID = GetIntValue(master, "ID"),
                    Material = GetStringValue(master, "Material"),
                    ShipmentCodeNo = GetStringValue(master, "ShipmentCodeNo"),
                    Supplier = GetStringValue(master, "Supplier"),
                    ReceivingDate = GetDateValue(master, "ReceivingDate"),
                    Quantity = GetNullableIntValue(master, "Quantity"),
                    AnalysisDate = GetDateValue(master, "AnalysisDate"),
                    ReferenceNo = GetStringValue(master, "ReferenceNo"),
                    ReceivedQuantity = GetStringValue(master, "ReceivedQuantity"),
                    PhysicalAnalysis = GetStringValue(master, "PhysicalAnalysis"),
                    StatusID = GetNullableIntValue(master, "StatusID"),
                    CreatedDate = GetDateValue(master, "CreatedDate"),
                    CreatedBy = GetStringValue(master, "CreatedBy"),
                    UpdatedDate = GetDateValue(master, "UpdatedDate"),
                    UpdatedBy = GetStringValue(master, "UpdatedBy"),
                    Samples = new List<SampleHBIDRIBLL>()
                };

                if (ds.Tables.Count > 1 && ds.Tables[1] != null)
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        model.Samples.Add(new SampleHBIDRIBLL
                        {
                            ID = GetIntValue(row, "ID"),
                            AnalysisID = GetIntValue(row, "AnalysisID"),
                            SampleCode = GetStringValue(row, "SampleCode"),
                            FeTotal = GetDecimalValue(row, "FeTotal"),
                            FeMetallic = GetDecimalValue(row, "FeMetallic"),
                            Metallization = GetDecimalValue(row, "Metallization"),
                            C = GetDecimalValue(row, "C"),
                            S = GetDecimalValue(row, "S"),
                            P = GetDecimalValue(row, "P"),
                            SiO2 = GetDecimalValue(row, "SiO2"),
                            Al2O3 = GetDecimalValue(row, "Al2O3"),
                            MgO = GetDecimalValue(row, "MgO"),
                            CaO = GetDecimalValue(row, "CaO"),
                            TotalGangue = GetDecimalValue(row, "TotalGangue"),
                            GrainSize = GetStringValue(row, "GrainSize"),
                            Comment = GetStringValue(row, "Comment")
                        });
                    }
                }

                return model;
            }
            catch
            {
                throw;
            }
        }

        public int InsertDRIAnalysisData(QCHBIDRIAnalysisBLL data)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@ReceivingDate", SqlDbType.Date)
                    {
                        Value = data.ReceivingDate.HasValue ? (object)data.ReceivingDate.Value.Date : DBNull.Value
                    },
                    new SqlParameter("@Material", SqlDbType.NVarChar, 100) { Value = DbString(data.Material) },
                    new SqlParameter("@ShipmentCodeNo", SqlDbType.NVarChar, 100) { Value = DbString(data.ShipmentCodeNo) },
                    new SqlParameter("@Supplier", SqlDbType.NVarChar, 200) { Value = DbString(data.Supplier) },
                    new SqlParameter("@Quantity", SqlDbType.Int)
                    {
                        Value = data.Quantity.HasValue ? (object)data.Quantity.Value : DBNull.Value
                    },
                    new SqlParameter("@AnalysisDate", SqlDbType.Date)
                    {
                        Value = data.AnalysisDate.HasValue ? (object)data.AnalysisDate.Value.Date : DBNull.Value
                    },
                    new SqlParameter("@ReceivedQuantity", SqlDbType.NVarChar, 100) { Value = DbString(data.ReceivedQuantity) },
                    new SqlParameter("@ReferenceNo", SqlDbType.NVarChar, 100) { Value = DbString(data.ReferenceNo) },
                    new SqlParameter("@PhysicalAnalysis", SqlDbType.NVarChar, -1) { Value = DbString(data.PhysicalAnalysis) },
                    new SqlParameter("@StatusID", SqlDbType.Int)
                    {
                        Value = data.StatusID.HasValue ? (object)data.StatusID.Value : 1
                    },
                    new SqlParameter("@CreatedDate", SqlDbType.DateTime)
                    {
                        Value = data.CreatedDate.HasValue ? (object)data.CreatedDate.Value : DateTime.Now
                    },
                    new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100) { Value = DbString(data.CreatedBy) }
                };

                object result = new DBHelper().ExecuteScalar("sp_InsertDRISampleReceiving", parameters);
                return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
            }
            catch
            {
                throw;
            }
        }

        public int UpdateDRIAnalysisData(QCHBIDRIAnalysisBLL data)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@ID", SqlDbType.Int) { Value = data.ID },
                    new SqlParameter("@ReceivingDate", SqlDbType.Date)
                    {
                        Value = data.ReceivingDate.HasValue ? (object)data.ReceivingDate.Value.Date : DBNull.Value
                    },
                    new SqlParameter("@Material", SqlDbType.NVarChar, 100) { Value = DbString(data.Material) },
                    new SqlParameter("@ShipmentCodeNo", SqlDbType.NVarChar, 100) { Value = DbString(data.ShipmentCodeNo) },
                    new SqlParameter("@Supplier", SqlDbType.NVarChar, 200) { Value = DbString(data.Supplier) },
                    new SqlParameter("@Quantity", SqlDbType.Int)
                    {
                        Value = data.Quantity.HasValue ? (object)data.Quantity.Value : DBNull.Value
                    },
                    new SqlParameter("@AnalysisDate", SqlDbType.Date)
                    {
                        Value = data.AnalysisDate.HasValue ? (object)data.AnalysisDate.Value.Date : DBNull.Value
                    },
                    new SqlParameter("@ReceivedQuantity", SqlDbType.NVarChar, 100) { Value = DbString(data.ReceivedQuantity) },
                    new SqlParameter("@ReferenceNo", SqlDbType.NVarChar, 100) { Value = DbString(data.ReferenceNo) },
                    new SqlParameter("@PhysicalAnalysis", SqlDbType.NVarChar, -1) { Value = DbString(data.PhysicalAnalysis) },
                    new SqlParameter("@StatusID", SqlDbType.Int)
                    {
                        Value = data.StatusID.HasValue ? (object)data.StatusID.Value : 1
                    },
                    new SqlParameter("@UpdatedDate", SqlDbType.DateTime)
                    {
                        Value = data.UpdatedDate.HasValue ? (object)data.UpdatedDate.Value : DateTime.Now
                    },
                    new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = DbString(data.UpdatedBy) }
                };

                object result = new DBHelper().ExecuteScalar("sp_UpdateDRISampleReceiving", parameters);
                return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
            }
            catch
            {
                throw;
            }
        }

        public int AddDRISample(SampleHBIDRIBLL data)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@AnalysisID", SqlDbType.Int) { Value = data.AnalysisID },
                    new SqlParameter("@SampleCode", SqlDbType.NVarChar, 100) { Value = DbString(data.SampleCode) },
                    CreateNullableDecimalParameter("@FeTotal", data.FeTotal),
                    CreateNullableDecimalParameter("@FeMetallic", data.FeMetallic),
                    CreateNullableDecimalParameter("@Metallization", data.Metallization),
                    CreateNullableDecimalParameter("@C", data.C),
                    CreateNullableDecimalParameter("@S", data.S),
                    CreateNullableDecimalParameter("@P", data.P),
                    CreateNullableDecimalParameter("@SiO2", data.SiO2),
                    CreateNullableDecimalParameter("@Al2O3", data.Al2O3),
                    CreateNullableDecimalParameter("@MgO", data.MgO),
                    CreateNullableDecimalParameter("@CaO", data.CaO),
                    CreateNullableDecimalParameter("@TotalGangue", data.TotalGangue),
                    new SqlParameter("@GrainSize", SqlDbType.NVarChar, 100) { Value = DbString(data.GrainSize) },
                    new SqlParameter("@Comment", SqlDbType.NVarChar, -1) { Value = DbString(data.Comment) },
                    new SqlParameter("@StatusID", SqlDbType.Int)
                    {
                        Value = data.StatusID.HasValue ? (object)data.StatusID.Value : 1
                    },
                    new SqlParameter("@CreatedDate", SqlDbType.DateTime)
                    {
                        Value = data.CreatedDate.HasValue ? (object)data.CreatedDate.Value : DateTime.Now
                    },
                    new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100) { Value = DbString(data.CreatedBy) }
                };

                object result = new DBHelper().ExecuteScalar("sp_InsertDRISampleAnalysis", parameters);
                return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
            }
            catch
            {
                throw;
            }
        }

        public int DeleteDRISamplesByID(int analysisID, string updatedBy)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@AnalysisID", SqlDbType.Int) { Value = analysisID },
                    new SqlParameter("@UpdatedDate", SqlDbType.DateTime) { Value = DateTime.Now },
                    new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = DbString(updatedBy) }
                };

                return new DBHelper().ExecuteNonQueryReturn("sp_DeleteDRISamplesByID", parameters);
            }
            catch
            {
                throw;
            }
        }

        public int DeleteHBIDRIAnalysis(int id, string updatedBy)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@ID", SqlDbType.Int) { Value = id },
                    new SqlParameter("@UpdatedDate", SqlDbType.DateTime) { Value = DateTime.Now },
                    new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = DbString(updatedBy) }
                };

                return new DBHelper().ExecuteNonQueryReturn("sp_DeleteHBIDRIAnalysis", parameters);
            }
            catch
            {
                throw;
            }
        }

        public List<QCHBIDRIAnalysisBLL> GetHBDRIAnalysisByDate(DateTime fromDate, DateTime toDateExclusive)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@FromDate", SqlDbType.Date) { Value = fromDate.Date },
                    new SqlParameter("@ToDate", SqlDbType.Date) { Value = toDateExclusive.Date }
                };

                DataTable dt = new DBHelper().GetTableFromSP("sp_GetHBDRIAnalysis_ByDate", parameters);
                return dt == null || dt.Rows.Count == 0
                    ? new List<QCHBIDRIAnalysisBLL>()
                    : JsonConvert.DeserializeObject<List<QCHBIDRIAnalysisBLL>>(JsonConvert.SerializeObject(dt));
            }
            catch
            {
                throw;
            }
        }

        public List<SampleHBIDRIBLL> GetHBDRISamplesByDate(DateTime fromDate, DateTime toDateExclusive)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@FromDate", SqlDbType.Date) { Value = fromDate.Date },
                    new SqlParameter("@ToDate", SqlDbType.Date) { Value = toDateExclusive.Date }
                };

                DataTable dt = new DBHelper().GetTableFromSP("sp_GetHBDRISample_ByDate", parameters);
                return dt == null || dt.Rows.Count == 0
                    ? new List<SampleHBIDRIBLL>()
                    : JsonConvert.DeserializeObject<List<SampleHBIDRIBLL>>(JsonConvert.SerializeObject(dt));
            }
            catch
            {
                throw;
            }
        }

        // ============================================================
        // NEW RM QC - PENDING BUNDLING ROWS
        // ============================================================
        public List<RMBundlingQCRowBLL> GetBundlingRowsForQC()
        {
            try
            {
                var list = new List<RMBundlingQCRowBLL>();
                SqlParameter[] parameters = new SqlParameter[0];
                DataTable dt = new DBHelper().GetTableFromSP(
                    "sp_QC_GetBundlingRowsForInspection",
                    parameters
                );

                if (dt == null || dt.Rows.Count == 0)
                    return list;

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new RMBundlingQCRowBLL
                    {
                        ID = GetInt(row, "ID"),
                        ProductionDate = GetNullableDate(row, "ProductionDate"),
                        Shift = GetString(row, "Shift"),
                        Plant = GetString(row, "Plant"),
                        HeatNo = GetString(row, "HeatNo"),
                        Size = GetString(row, "Size"),
                        Profile = GetString(row, "Profile"),
                        Product = GetString(row, "Product"),
                        SteelGrade = GetString(row, "SteelGrade"),
                        TotalBundleProduced = GetNullableInt(row, "TotalBundleProduced"),
                        OnHold = GetNullableInt(row, "OnHold"),
                        Rejected = GetNullableInt(row, "Rejected"),
                        Accepted = GetNullableInt(row, "Accepted"),
                        BundleSeriesOnHold = GetString(row, "BundleSeriesOnHold"),
                        DefectCode = GetString(row, "DefectCode"),
                        MRBNo = GetString(row, "MRBNo"),
                        Remarks = GetString(row, "Remarks")
                    });
                }

                return list;
            }
            catch
            {
                throw;
            }
        }

        public bool IsRMQCInspectionDuplicate(DateTime? productionDate, string shift, string heatNo)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@ProductionDate", SqlDbType.Date) { Value = productionDate?.Date },
                    new SqlParameter("@Shift", SqlDbType.NVarChar, 50)
                    {
                        Value = string.IsNullOrWhiteSpace(shift) ? (object)DBNull.Value : shift.Trim()
                    },
                    new SqlParameter("@HeatNo", SqlDbType.NVarChar, 100)
                    {
                        Value = string.IsNullOrWhiteSpace(heatNo) ? (object)DBNull.Value : heatNo.Trim()
                    }
                };

                object result = new DBHelper().ExecuteScalar(
                    "sp_QC_CheckRMInspectionDuplicate",
                    parameters
                );

                return result != null && result != DBNull.Value && Convert.ToInt32(result) > 0;
            }
            catch
            {
                throw;
            }
        }

        // ============================================================
        // HELPERS
        // ============================================================
        private static object DbValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value.Trim();
        }

        private static object DbString(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value.Trim();
        }

        private static string GetString(DataRow row, string column)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return "";
            return Convert.ToString(row[column]).Trim();
        }

        private static int GetInt(DataRow row, string column)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return 0;

            int result;
            return int.TryParse(Convert.ToString(row[column]).Trim(), out result) ? result : 0;
        }

        private static int? GetNullableInt(
            DataRow row,
            string column)
        {
            if (
                row == null ||
                row.Table == null ||
                !row.Table.Columns.Contains(column) ||
                row[column] == DBNull.Value
            )
            {
                return null;
            }

            object value = row[column];

            try
            {
                // SQL INT / SMALLINT / BIGINT
                if (
                    value is int ||
                    value is short ||
                    value is long ||
                    value is byte
                )
                {
                    return Convert.ToInt32(value);
                }

                // SQL DECIMAL / NUMERIC / FLOAT
                if (
                    value is decimal ||
                    value is double ||
                    value is float
                )
                {
                    return Convert.ToInt32(
                        Convert.ToDecimal(value)
                    );
                }

                string text =
                    Convert.ToString(value).Trim();

                int intValue;

                if (int.TryParse(text, out intValue))
                {
                    return intValue;
                }

                decimal decimalValue;

                if (
                    decimal.TryParse(
                        text,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out decimalValue
                    )
                )
                {
                    return Convert.ToInt32(decimalValue);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private static decimal GetDecimal(DataRow row, string column)
        {
            return GetNullableDecimal(row, column) ?? 0M;
        }

        private static decimal? GetNullableDecimal(DataRow row, string column)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return null;

            object rawValue = row[column];
            try
            {
                if (rawValue is decimal || rawValue is double || rawValue is float || rawValue is int || rawValue is long || rawValue is short)
                    return Convert.ToDecimal(rawValue);
            }
            catch
            {
                return null;
            }

            string value = Convert.ToString(rawValue).Trim();
            if (string.IsNullOrWhiteSpace(value)) return null;

            decimal result;
            if (decimal.TryParse(
                value,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out result))
                return result;

            if (decimal.TryParse(
                value,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.CurrentCulture,
                out result))
                return result;

            value = value
                .Replace("MM", "")
                .Replace("mm", "")
                .Replace("MT", "")
                .Replace("mt", "")
                .Replace("Meter", "")
                .Replace("meter", "")
                .Replace("Meters", "")
                .Replace("meters", "")
                .Replace("Kg/m", "")
                .Replace("kg/m", "")
                .Replace("KG/M", "")
                .Replace(",", "")
                .Trim();

            if (decimal.TryParse(
                value,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out result))
                return result;

            return null;
        }

        private static DateTime? GetNullableDate(DataRow row, string column)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return null;

            DateTime result;
            return DateTime.TryParse(Convert.ToString(row[column]), out result) ? (DateTime?)result : null;
        }

        private static bool GetBool(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
                return false;

            bool b;
            if (bool.TryParse(Convert.ToString(row[columnName]), out b)) return b;

            int i;
            return int.TryParse(Convert.ToString(row[columnName]), out i) && i == 1;
        }

        private static SqlParameter CreateNullableDecimalParameter(string parameterName, decimal? value)
        {
            var parameter = new SqlParameter(parameterName, SqlDbType.Decimal)
            {
                Precision = 18,
                Scale = 6,
                Value = value.HasValue ? (object)value.Value : DBNull.Value
            };
            return parameter;
        }

        private static string GetStringValue(DataRow row, string column)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return string.Empty;
            return Convert.ToString(row[column]).Trim();
        }

        private static int GetIntValue(DataRow row, string column)
        {
            return GetNullableIntValue(row, column) ?? 0;
        }

        private static int? GetNullableIntValue(DataRow row, string column)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return null;

            int value;
            return int.TryParse(Convert.ToString(row[column]), out value) ? (int?)value : null;
        }

        private static decimal? GetDecimalValue(DataRow row, string column)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return null;

            try
            {
                return Convert.ToDecimal(row[column]);
            }
            catch
            {
                return null;
            }
        }

        private static DateTime? GetDateValue(DataRow row, string column)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return null;

            DateTime value;
            return DateTime.TryParse(Convert.ToString(row[column]), out value) ? (DateTime?)value : null;
        }
    }
}
