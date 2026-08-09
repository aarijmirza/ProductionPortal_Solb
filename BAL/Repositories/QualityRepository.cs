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

                // Sample-specific data
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

                // Audit / Status
                p[29] = new SqlParameter("@StatusID", model.StatusID);
                p[30] = new SqlParameter("@CreatedDate", model.CreatedDate);
                p[31] = new SqlParameter("@CreatedBy", model.CreatedBy ?? "");

                return (new DBHelper().ExecuteNonQueryReturn)("sp_AddHeatChemistry", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int InsertBilletBoarding(BilletBoardBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[16];

                var Profile = model.Profile + model.Size;

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

        public List<BilletBoardBLL> GetAllBoarding_RMCharging()
        {
            try
            {
                var lst = new List<BilletBoardBLL>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetAllBoarding_RMCharging", p);
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


        public List<BilletChargingBLL> GetAllCharging()
        {
            try
            {
                var lst = new List<BilletChargingBLL>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetAllCharging", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<BilletChargingBLL>>();
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public BilletBoardBLL GetBilletDetails(string heatno)
        {
            try
            {
                var _obj = new BilletBoardBLL();
                SqlParameter[] p = new SqlParameter[1];
                p[0] = new SqlParameter("@id", heatno);
                _dt = (new DBHelper().GetTableFromSP)("sp_GetBilletDetailByHeatno", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        _obj = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<BilletBoardBLL>>().FirstOrDefault();
                    }
                }
                return _obj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool IsBilletBoardingExists(string billetBoarding)
        {
            try
            {
                SqlParameter[] p =
                {
            new SqlParameter("@BilletBoarding", billetBoarding)
        };

                DataTable dt = (new DBHelper().GetTableFromSP)("sp_CheckBilletBoardingExists", p);

                if (dt != null && dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0]["Total"]) > 0;
                }

                return false;
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

                SqlParameter[] p =
                {
            new SqlParameter("@HeatNos", heatNoCsv)
        };

                DataTable dt = (new DBHelper().GetTableFromSP)("sp_GetDuplicateHeatNos", p);

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

                SqlParameter[] p =
                {
            new SqlParameter("@id", id)
        };

                DataSet ds = (new DBHelper().GetDatasetFromSP)("sp_GetBilletDetailByID", p);

                if (ds != null && ds.Tables.Count > 0)
                {
                    // Table 0 = Header single record
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                        obj = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0]))
                                    .ToObject<List<BilletBoardBLL>>()
                                    .FirstOrDefault();
                    }

                    if (obj != null)
                    {
                        // Table 1 = Same Billet Boarding ke multiple Heat records
                        if (ds.Tables.Count > 1 && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                        {
                            obj.BilletBoardingHeats = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[1]))
                                                            .ToObject<List<BilletBoardBLL>>();
                        }
                        else
                        {
                            obj.BilletBoardingHeats = new List<BilletBoardBLL>();
                        }

                        // Table 2 = Chemical Analysis records
                        if (ds.Tables.Count > 2 && ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                        {
                            obj.Chemistry = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[2]))
                                                  .ToObject<List<RMChemicalAnalysisBLL>>();
                        }
                        else
                        {
                            obj.Chemistry = new List<RMChemicalAnalysisBLL>();
                        }
                    }
                }

                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //public BilletBoardBLL GetBilletDetails(int id)
        //{
        //    try
        //    {
        //        BilletBoardBLL obj = null;

        //        SqlParameter[] p =
        //        {
        //            new SqlParameter("@id", id)
        //        };

        //        _dt = (new DBHelper().GetTableFromSP)("sp_GetBilletDetailByID", p);

        //        if (_dt != null && _dt.Rows.Count > 0)
        //        {
        //            obj = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt))
        //                       .ToObject<List<BilletBoardBLL>>()
        //                       .FirstOrDefault();

        //            if (obj != null)
        //            {
        //                obj.Chemistry = !string.IsNullOrWhiteSpace(obj.HeatNo)
        //                    ? GetChemicalAnalysisByHeatNo(obj.HeatNo)
        //                    : new List<RMChemicalAnalysisBLL>();
        //            }
        //        }

        //        return obj;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        //public List<RMChemicalAnalysisBLL> GetChemicalAnalysisByHeatNo(string heatNo)
        //{
        //    try
        //    {
        //        var lst = new List<RMChemicalAnalysisBLL>();

        //        SqlParameter[] p = new SqlParameter[1];
        //        p[0] = new SqlParameter("@HeatNo", heatNo);

        //        _dt = (new DBHelper().GetTableFromSP)("sp_GetChemicalAnalysisByHeatNo", p);

        //        if (_dt != null && _dt.Rows.Count > 0)
        //        {
        //            lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt))
        //                        .ToObject<List<RMChemicalAnalysisBLL>>();
        //        }

        //        return lst;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public List<HeatChemistryBLL> GetHeatChemistryDatewise(DateTime from, DateTime to)
        {
            SqlParameter[] p = {
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

                int insertedId = Convert.ToInt32(new DBHelper().ExecuteScalar("sp_InsertSlagByProductAnalysis", p));

                return insertedId;
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int DeleteSlagSamplesBySlagID(int ID, string UpdatedBy)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[4];
                int i = 0;

                p[i++] = new SqlParameter("@ID", ID);
                p[i++] = new SqlParameter("@StatusID", 3);
                p[i++] = new SqlParameter("@UpdatedDate", DateTime.Now);
                p[i++] = new SqlParameter("@UpdatedBy", UpdatedBy);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_DeleteSlagSamplesBySlagID", p);
            }
            catch (Exception ex)
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

                return (new DBHelper().ExecuteNonQueryReturn)("sp_InsertSlagSampleAnalysis", p);
            }
            catch (Exception ex)
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

                _dt = (new DBHelper().GetTableFromSP)("sp_GetSlagByProduct", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<SlagByProductAnalysisBLL>>();
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<QCHBIDRIAnalysisBLL> GetDRIHBIAnalysis()
        {
            try
            {
                var lst = new List<QCHBIDRIAnalysisBLL>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetSampleHBIDRI", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<QCHBIDRIAnalysisBLL>>();
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int DeleteDRISamplesByID(int ID, string UpdatedBy)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[4];
                int i = 0;

                p[i++] = new SqlParameter("@ID", ID);
                p[i++] = new SqlParameter("@StatusID", 3);
                p[i++] = new SqlParameter("@UpdatedDate", DateTime.Now);
                p[i++] = new SqlParameter("@UpdatedBy", UpdatedBy);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_DeleteDRISamplesByID", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int SlagByProductDelete(int ID, string UpdatedBy)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[4];
                int i = 0;

                p[i++] = new SqlParameter("@ID", ID);
                p[i++] = new SqlParameter("@StatusID", 3);
                p[i++] = new SqlParameter("@UpdatedDate", DateTime.Now);
                p[i++] = new SqlParameter("@UpdatedBy", UpdatedBy);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_DeleteSlagByProduct", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int DeleteHBIDRIAnalysis(int ID, string UpdatedBy)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[4];
                int i = 0;

                p[i++] = new SqlParameter("@ID", ID);
                p[i++] = new SqlParameter("@StatusID", 3);
                p[i++] = new SqlParameter("@UpdatedDate", DateTime.Now);
                p[i++] = new SqlParameter("@UpdatedBy", UpdatedBy);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_DeleteHBIDRIAnalysis", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public QCHBIDRIAnalysisBLL GetDRIHBIDetailByID(int? id)
        {
            SqlParameter[] p = {
        new SqlParameter("@id", id)
        };

            DataSet ds = new DBHelper().GetDatasetFromSP("sp_GetDRIHBIDetailByID", p);

            if (ds.Tables[0].Rows.Count == 0)
                return null;

            DataRow m = ds.Tables[0].Rows[0];

            QCHBIDRIAnalysisBLL model = new QCHBIDRIAnalysisBLL
            {
                ID = Convert.ToInt32(m["ID"]),
                Material = m["Material"].ToString(),
                ShipmentCodeNo = m["ShipmentCodeNo"].ToString(),
                Supplier = m["Supplier"].ToString(),
                ReceivingDate = m["ReceivingDate"] as DateTime?,
                Quantity = m["Quantity"] as int?,
                AnalysisDate = m["AnalysisDate"] as DateTime?,
                ReferenceNo = m["ReferenceNo"].ToString(),
                ReceivedQuantity = m["ReceivedQuantity"].ToString(),
                PhysicalAnalysis = m["PhysicalAnalysis"].ToString(),
                StatusID = m["StatusID"] as int?,
                CreatedDate = m["CreatedDate"] as DateTime?,
                CreatedBy = m["CreatedBy"].ToString(),
                Samples = new List<SampleHBIDRIBLL>()
            };

            foreach (DataRow r in ds.Tables[1].Rows)
            {
                model.Samples.Add(new SampleHBIDRIBLL
                {
                    ID = Convert.ToInt32(r["ID"]),
                    AnalysisID = Convert.ToInt32(r["AnalysisID"]),
                    SampleCode = r["SampleCode"].ToString(),
                    FeTotal = r["FeTotal"] as decimal?,
                    FeMetallic = r["FeMetallic"] as decimal?,
                    Metallization = r["Metallization"] as decimal?,
                    C = r["C"] as decimal?,
                    S = r["S"] as decimal?,
                    P = r["P"] as decimal?,
                    SiO2 = r["SiO2"] as decimal?,
                    Al2O3 = r["Al2O3"] as decimal?,
                    MgO = r["MgO"] as decimal?,
                    CaO = r["CaO"] as decimal?,
                    TotalGangue = r["TotalGangue"] as decimal?,
                    GrainSize = r["GrainSize"].ToString(),
                    Comment = r["Comment"].ToString()
                });
            }

            return model;
        }

        public int AddDRISample(SampleHBIDRIBLL data)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[18];

                p[0] = new SqlParameter("@SampleCode", data.SampleCode);
                p[1] = new SqlParameter("@FeTotal", data.FeTotal);
                p[2] = new SqlParameter("@FeMetallic", data.FeMetallic);
                p[3] = new SqlParameter("@Metallization", data.Metallization);
                p[4] = new SqlParameter("@C", data.C);
                p[5] = new SqlParameter("@S", data.S);
                p[6] = new SqlParameter("@P", data.P);
                p[7] = new SqlParameter("@SiO2", data.SiO2);
                p[8] = new SqlParameter("@Al2O3", data.Al2O3);
                p[9] = new SqlParameter("@MgO", data.MgO);
                p[10] = new SqlParameter("@CaO", data.CaO);
                p[11] = new SqlParameter("@TotalGangue", data.TotalGangue);
                p[12] = new SqlParameter("@GrainSize", data.GrainSize);
                p[13] = new SqlParameter("@Comment", data.Comment);
                p[14] = new SqlParameter("@AnalysisID", data.AnalysisID);
                p[15] = new SqlParameter("@StatusID", data.StatusID);
                p[16] = new SqlParameter("@CreatedDate", data.CreatedDate);
                p[17] = new SqlParameter("@CreatedBy", data.CreatedBy);

                return (new DBHelper().ExecuteNonQueryReturn)("sp_InsertDRISampleAnalysis", p);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int InsertDRIAnalysisData(QCHBIDRIAnalysisBLL data)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[12];

                p[0] = new SqlParameter("@ReceivingDate", data.ReceivingDate);
                p[1] = new SqlParameter("@Material", data.Material);
                p[2] = new SqlParameter("@ShipmentCodeNo", data.ShipmentCodeNo);
                p[3] = new SqlParameter("@Supplier", data.Supplier);
                p[4] = new SqlParameter("@Quantity", data.Quantity);
                p[5] = new SqlParameter("@AnalysisDate", data.AnalysisDate);
                p[6] = new SqlParameter("@ReceivedQuantity", data.ReceivedQuantity);
                p[7] = new SqlParameter("@ReferenceNo", data.ReferenceNo);
                p[8] = new SqlParameter("@PhysicalAnalysis", data.PhysicalAnalysis);
                p[9] = new SqlParameter("@StatusID", data.StatusID);
                p[10] = new SqlParameter("@CreatedDate", data.CreatedDate);
                p[11] = new SqlParameter("@CreatedBy", data.CreatedBy);

                int insertedId = Convert.ToInt32(new DBHelper().ExecuteScalar("sp_InsertDRISampleReceiving", p));

                return insertedId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int UpdateDRIAnalysisData(QCHBIDRIAnalysisBLL data)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[13];

                p[0] = new SqlParameter("@ReceivingDate", data.ReceivingDate);
                p[1] = new SqlParameter("@Material", data.Material);
                p[2] = new SqlParameter("@ShipmentCodeNo", data.ShipmentCodeNo);
                p[3] = new SqlParameter("@Supplier", data.Supplier);
                p[4] = new SqlParameter("@Quantity", data.Quantity);
                p[5] = new SqlParameter("@AnalysisDate", data.AnalysisDate);
                p[6] = new SqlParameter("@ReceivedQuantity", data.ReceivedQuantity);
                p[7] = new SqlParameter("@ReferenceNo", data.ReferenceNo);
                p[8] = new SqlParameter("@PhysicalAnalysis", data.PhysicalAnalysis);
                p[9] = new SqlParameter("@StatusID", data.StatusID);
                p[10] = new SqlParameter("@UpdatedDate", data.UpdatedDate);
                p[11] = new SqlParameter("@UpdateBy", data.UpdatedBy);
                p[12] = new SqlParameter("@ID", data.ID);

                return Convert.ToInt32(new DBHelper().ExecuteScalar("sp_UpdateDRISampleReceiving", p));

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int AddBundlingSection(BundlingSectionBLL data)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[18];

                return (new DBHelper().ExecuteNonQueryReturn)("sp_InsertDRISampleAnalysis", p);
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        public SlagByProductAnalysisBLL GetSlagByID(int? id)
        {
            try
            {
                var _obj = new SlagByProductAnalysisBLL();
                SqlParameter[] p = new SqlParameter[1];
                p[0] = new SqlParameter("@id", id);

                _dt = (new DBHelper().GetTableFromSP)("sp_GetSlagByProductByID", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        _obj = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<SlagByProductAnalysisBLL>>().FirstOrDefault();
                    }
                }

                return _obj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<SlagSampleAnalysisBLL> GetSlagSamplesById(int id)
        {
            try
            {
                var lst = new List<SlagSampleAnalysisBLL>();

                SqlParameter[] p = new SqlParameter[1];

                p[0] = new SqlParameter("@id", id);

                _dt = (new DBHelper().GetTableFromSP)("sp_GetSlagSampleByID", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<SlagSampleAnalysisBLL>>();
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<SlagByProductAnalysisBLL> GetSlagByProductByDate(DateTime fromDate, DateTime toDateExclusive)
        {
            try
            {
                var lst = new List<SlagByProductAnalysisBLL>();

                SqlParameter[] p = new SqlParameter[2];
                p[0] = new SqlParameter("@FromDate", SqlDbType.Date) { Value = fromDate.Date };
                p[1] = new SqlParameter("@ToDate", SqlDbType.Date) { Value = toDateExclusive.Date };

                DataTable dt = (new DBHelper().GetTableFromSP)("sp_GetSlagByProduct_ByDate", p);

                if (dt != null && dt.Rows.Count > 0)
                {
                    lst = Newtonsoft.Json.JsonConvert
                        .DeserializeObject<List<SlagByProductAnalysisBLL>>(
                            Newtonsoft.Json.JsonConvert.SerializeObject(dt)
                        );
                }

                return lst;
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
                var lst = new List<SlagSampleAnalysisBLL>();

                SqlParameter[] p = new SqlParameter[2];
                p[0] = new SqlParameter("@FromDate", SqlDbType.Date) { Value = fromDate.Date };
                p[1] = new SqlParameter("@ToDate", SqlDbType.Date) { Value = toDateExclusive.Date };

                DataTable dt = (new DBHelper().GetTableFromSP)("sp_GetSlagSample_ByDate", p);

                if (dt != null && dt.Rows.Count > 0)
                {
                    lst = Newtonsoft.Json.JsonConvert
                        .DeserializeObject<List<SlagSampleAnalysisBLL>>(
                            Newtonsoft.Json.JsonConvert.SerializeObject(dt)
                        );
                }

                return lst;
            }
            catch
            {
                return new List<SlagSampleAnalysisBLL>();
            }
        }
        public List<QCHBIDRIAnalysisBLL> GetHBDRIAnalysisByDate(DateTime fromDate, DateTime toDateExclusive)
        {
            try
            {
                var lst = new List<QCHBIDRIAnalysisBLL>();

                SqlParameter[] p = new SqlParameter[2];
                p[0] = new SqlParameter("@FromDate", SqlDbType.Date) { Value = fromDate.Date };
                p[1] = new SqlParameter("@ToDate", SqlDbType.Date) { Value = toDateExclusive.Date };

                DataTable dt = (new DBHelper().GetTableFromSP)("sp_GetHBDRIAnalysis_ByDate", p);

                if (dt != null && dt.Rows.Count > 0)
                {
                    lst = Newtonsoft.Json.JsonConvert
                        .DeserializeObject<List<QCHBIDRIAnalysisBLL>>(
                            Newtonsoft.Json.JsonConvert.SerializeObject(dt)
                        );
                }

                return lst;
            }
            catch
            {
                return new List<QCHBIDRIAnalysisBLL>();
            }
        }
        public List<SampleHBIDRIBLL> GetHBDRISamplesByDate(DateTime fromDate, DateTime toDateExclusive)
        {
            try
            {
                var lst = new List<SampleHBIDRIBLL>();

                SqlParameter[] p = new SqlParameter[2];
                p[0] = new SqlParameter("@FromDate", SqlDbType.Date) { Value = fromDate.Date };
                p[1] = new SqlParameter("@ToDate", SqlDbType.Date) { Value = toDateExclusive.Date };

                DataTable dt = (new DBHelper().GetTableFromSP)("sp_GetHBDRISample_ByDate", p);

                if (dt != null && dt.Rows.Count > 0)
                {
                    lst = Newtonsoft.Json.JsonConvert
                        .DeserializeObject<List<SampleHBIDRIBLL>>(
                            Newtonsoft.Json.JsonConvert.SerializeObject(dt)
                        );
                }

                return lst;
            }
            catch
            {
                return new List<SampleHBIDRIBLL>();
            }
        }
        public List<BilletBoardBLL> GetBilletBoardingByDate(DateTime fromDate, DateTime toDateExclusive)
        {
            try
            {
                var lst = new List<BilletBoardBLL>();

                SqlParameter[] p = new SqlParameter[2];
                p[0] = new SqlParameter("@FromDate", SqlDbType.Date) { Value = fromDate.Date };
                p[1] = new SqlParameter("@ToDate", SqlDbType.Date) { Value = toDateExclusive.Date };

                DataTable dt = (new DBHelper().GetTableFromSP)("sp_GetBilletBoarding_ByDate", p);

                if (dt != null && dt.Rows.Count > 0)
                {
                    lst = Newtonsoft.Json.JsonConvert
                        .DeserializeObject<List<BilletBoardBLL>>(
                            Newtonsoft.Json.JsonConvert.SerializeObject(dt)
                        );
                }

                return lst;
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
                var lst = new List<HeatChemistryBLL>();

                SqlParameter[] p = new SqlParameter[2];
                p[0] = new SqlParameter("@FromDate", SqlDbType.Date) { Value = fromDate.Date };
                p[1] = new SqlParameter("@ToDate", SqlDbType.Date) { Value = toDateExclusive.Date };

                DataTable dt = (new DBHelper().GetTableFromSP)("sp_GetHeatChemistry_ByDate", p);

                if (dt != null && dt.Rows.Count > 0)
                {
                    lst = Newtonsoft.Json.JsonConvert
                        .DeserializeObject<List<HeatChemistryBLL>>(
                            Newtonsoft.Json.JsonConvert.SerializeObject(dt)
                        );
                }

                return lst;
            }
            catch
            {
                return new List<HeatChemistryBLL>();
            }
        }

        public bool SaveQCInspection(RMQCInspectionBLL model)
        {
            try
            {
                SqlParameter[] p = new SqlParameter[]
                {
                    new SqlParameter("@ProductionDate", model.ProductionDate),
                    new SqlParameter("@Shift", model.Shift),
                    new SqlParameter("@HeatNo", model.HeatNo),

                    new SqlParameter("@SteelGrade", model.SteelGrade),
                    new SqlParameter("@BarSize", model.BarSize),

                    new SqlParameter("@TotalBundles", model.TotalBundles),
                    new SqlParameter("@OnHold", model.OnHold),
                    new SqlParameter("@Rejected", model.Rejected),
                    new SqlParameter("@Accepted", model.Accepted),

                    new SqlParameter("@BundleSeriesOnHold", model.BundleSeriesOnHold ?? ""),
                    new SqlParameter("@DefectCodes", model.DefectCodes ?? ""),
                    new SqlParameter("@MRBNo", model.MRBNo ?? ""),

                    new SqlParameter("@QCStatus", model.QCStatus ?? ""),
                    new SqlParameter("@Remarks", model.Remarks ?? ""),

                    new SqlParameter("@CreatedOn", model.CreatedOn),
                    new SqlParameter("@CreatedBy", model.CreatedBy ?? ""),
                    new SqlParameter("@StatusID", model.StatusID),
                };

                new DBHelper().ExecuteNonQuery("sp_SaveRMQCInspection", p);

                return true;
            }
            catch
            {
                return false;
            }
        }



        public List<QCBilletBoardingRowBLL>
                    GetBilletBoardingRows(
                        string rollingMill)
        {
            var list =
                new List<QCBilletBoardingRowBLL>();

            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@RollingMill",
                    SqlDbType.NVarChar,
                    20
                )
                {
                    Value =
                        string.IsNullOrWhiteSpace(
                            rollingMill
                        )
                            ? "RM1"
                            : rollingMill.Trim()
                }
            };

            DataTable dt =
                DBHelper.ExecuteDataTable(
                    "sp_QC_GetBilletBoardingRows",
                    CommandType.StoredProcedure,
                    parameters
                );

            if (dt == null)
            {
                return list;
            }

            foreach (DataRow row in dt.Rows)
            {
                list.Add(
                    new QCBilletBoardingRowBLL
                    {
                        ID =
                            GetInt(
                                row,
                                "ID"
                            ),

                        Site =
                            GetString(
                                row,
                                "Site"
                            ),

                        BoardingNo =
                            GetString(
                                row,
                                "BoardingNo"
                            ),

                        SerialNo =
                            GetInt(
                                row,
                                "SerialNo"
                            ),

                        HeatNo =
                            GetString(
                                row,
                                "HeatNo"
                            ),

                        SteelGrade =
                            GetString(
                                row,
                                "SteelGrade"
                            ),

                        BarSize =
                            GetString(
                                row,
                                "BarSize"
                            ),

                        BarsPerBundle =
                            GetInt(
                                row,
                                "BarsPerBundle"
                            ),

                        ActualBundleCount =
                            GetInt(
                                row,
                                "ActualBundleCount"
                            ),

                        YardInspection =
                            GetString(
                                row,
                                "YardInspection"
                            ),

                        YardInspectionRemarks =
                            GetString(
                                row,
                                "YardInspectionRemarks"
                            )
                    }
                );
            }

            return list;
        }


        public List<QCMTCRowBLL> GetMTCRows(
            string heatNo = null)
        {
            var list =
                new List<QCMTCRowBLL>();

            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@HeatNo",
                    SqlDbType.NVarChar,
                    50
                )
                {
                    Value =
                        string.IsNullOrWhiteSpace(
                            heatNo
                        )
                            ? (object)DBNull.Value
                            : heatNo.Trim()
                }
            };

            DataTable dt =
                DBHelper.ExecuteDataTable(
                    "sp_QC_GetMTCRows",
                    CommandType.StoredProcedure,
                    parameters
                );

            if (dt == null)
            {
                return list;
            }

            foreach (DataRow row in dt.Rows)
            {
                list.Add(
                    new QCMTCRowBLL
                    {
                        ID =
                            GetInt(
                                row,
                                "ID"
                            ),

                        HeatNo =
                            GetString(
                                row,
                                "HeatNo"
                            ),

                        SteelGrade =
                            GetString(
                                row,
                                "SteelGrade"
                            ),

                        BarSize =
                            GetDecimal(
                                row,
                                "BarSize"
                            ),

                        YieldStress =
                            GetDecimal(
                                row,
                                "YieldStress"
                            ),

                        TensileStress =
                            GetDecimal(
                                row,
                                "TensileStress"
                            ),

                        NoOfBundles =
                            GetInt(
                                row,
                                "NoOfBundles"
                            ),

                        YSTSRatio =
                            GetDecimal(
                                row,
                                "YSTSRatio"
                            )
                    }
                );
            }

            return list;
        }


        public QCInspectionRMDetailBLL
            GetQCInspectionRMByID(
                int id)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@ID",
                    SqlDbType.Int
                )
                {
                    Value =
                        id
                }
            };

            DataTable dt =
                DBHelper.ExecuteDataTable(
                    "sp_QC_GetInspectionRMByID",
                    CommandType.StoredProcedure,
                    parameters
                );

            if (
                dt == null ||
                dt.Rows.Count == 0
            )
            {
                return null;
            }

            return MapInspection(
                dt.Rows[0]
            );
        }


        public QCInspectionRMDetailBLL
            GetQCInspectionRMFromBoarding(
                int boardingID)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@BoardingID",
                    SqlDbType.Int
                )
                {
                    Value =
                        boardingID
                }
            };

            DataTable dt =
                DBHelper.ExecuteDataTable(
                    "sp_QC_GetInspectionRMFromBoarding",
                    CommandType.StoredProcedure,
                    parameters
                );

            if (
                dt == null ||
                dt.Rows.Count == 0
            )
            {
                return null;
            }

            return MapInspection(
                dt.Rows[0]
            );
        }


        public QCMTCDetailBLL GetMTCDetail(
            string heatNo)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@HeatNo",
                    SqlDbType.NVarChar,
                    50
                )
                {
                    Value =
                        heatNo
                }
            };

            DataTable dt =
                DBHelper.ExecuteDataTable(
                    "sp_QC_GetMTCDetail",
                    CommandType.StoredProcedure,
                    parameters
                );

            if (
                dt == null ||
                dt.Rows.Count == 0
            )
            {
                return null;
            }

            DataRow row =
                dt.Rows[0];

            return new QCMTCDetailBLL
            {
                HeatNo =
                    GetString(
                        row,
                        "HeatNo"
                    ),

                YieldStrength =
                    GetDecimal(
                        row,
                        "YieldStrength"
                    ),

                TensileStrength =
                    GetDecimal(
                        row,
                        "TensileStrength"
                    ),

                TensileYieldRatio =
                    GetDecimal(
                        row,
                        "TensileYieldRatio"
                    ),

                Elongation =
                    GetDecimal(
                        row,
                        "Elongation"
                    ),

                GaugeLength =
                    GetDecimal(
                        row,
                        "GaugeLength"
                    ),

                C =
                    GetDecimal(
                        row,
                        "C"
                    ),

                Si =
                    GetDecimal(
                        row,
                        "Si"
                    ),

                Mn =
                    GetDecimal(
                        row,
                        "Mn"
                    ),

                P =
                    GetDecimal(
                        row,
                        "P"
                    ),

                S =
                    GetDecimal(
                        row,
                        "S"
                    ),

                N =
                    GetDecimal(
                        row,
                        "N"
                    ),

                Ceq =
                    GetDecimal(
                        row,
                        "Ceq"
                    )
            };
        }


        public int SaveQCInspectionRM(
            QCInspectionRMDetailBLL model)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@ID",
                    model.ID
                ),

                new SqlParameter(
                    "@BilletBoardingID",
                    model.BilletBoardingID
                ),

                new SqlParameter(
                    "@MTCID",
                    model.MTCID
                ),

                new SqlParameter(
                    "@Site",
                    DbValue(
                        model.Site
                    )
                ),

                new SqlParameter(
                    "@ProductionShift",
                    DbValue(
                        model.ProductionShift
                    )
                ),

                new SqlParameter(
                    "@ProductionDate",
                    model.ProductionDateValue.HasValue
                        ? (object)model.ProductionDateValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@HeatNo",
                    DbValue(
                        model.HeatNo
                    )
                ),

                new SqlParameter(
                    "@Specification",
                    DbValue(
                        model.Specification
                    )
                ),

                new SqlParameter(
                    "@SteelGrade",
                    DbValue(
                        model.SteelGrade
                    )
                ),

                new SqlParameter(
                    "@Length",
                    model.LengthValue.HasValue
                        ? (object)model.LengthValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@NominalWeight",
                    model.NominalWeightValue.HasValue
                        ? (object)model.NominalWeightValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@CrossSectionArea",
                    model.CrossSectionAreaValue.HasValue
                        ? (object)model.CrossSectionAreaValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@BendTestObserved",
                    model.BendTestObserved
                ),

                new SqlParameter(
                    "@BarSize",
                    model.BarSizeValue.HasValue
                        ? (object)model.BarSizeValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@WeightPerBundle",
                    model.WeightPerBundleValue.HasValue
                        ? (object)model.WeightPerBundleValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@NoOfBarsPerBundle",
                    model.NoOfBarsPerBundleValue.HasValue
                        ? (object)model.NoOfBarsPerBundleValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@NoOfBundles",
                    model.NoOfBundlesValue.HasValue
                        ? (object)model.NoOfBundlesValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@IsWireRodOrCoil",
                    model.IsWireRodOrCoil
                ),

                new SqlParameter(
                    "@YieldStrength",
                    model.YieldStrengthValue.HasValue
                        ? (object)model.YieldStrengthValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@TensileStrength",
                    model.TensileStrengthValue.HasValue
                        ? (object)model.TensileStrengthValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@TensileYieldRatio",
                    model.TensileYieldRatioValue.HasValue
                        ? (object)model.TensileYieldRatioValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@Elongation",
                    model.ElongationValue.HasValue
                        ? (object)model.ElongationValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@GaugeLength",
                    model.GaugeLengthValue.HasValue
                        ? (object)model.GaugeLengthValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@C",
                    model.CValue.HasValue
                        ? (object)model.CValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@Si",
                    model.SiValue.HasValue
                        ? (object)model.SiValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@Mn",
                    model.MnValue.HasValue
                        ? (object)model.MnValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@P",
                    model.PValue.HasValue
                        ? (object)model.PValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@S",
                    model.SValue.HasValue
                        ? (object)model.SValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@N",
                    model.NValue.HasValue
                        ? (object)model.NValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@Ceq",
                    model.CeqValue.HasValue
                        ? (object)model.CeqValue.Value
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@StatusID",
                    1
                ),

                new SqlParameter(
                    "@CreatedBy",
                    DbValue(
                        model.CreatedBy
                    )
                ),

                new SqlParameter(
                    "@CreatedDate",
                    model.CreatedDate.HasValue
                        ? (object)model.CreatedDate.Value
                        : DateTime.Now
                ),

                new SqlParameter(
                    "@Result",
                    SqlDbType.Int
                )
                {
                    Direction =
                        ParameterDirection.Output
                }
            };

            DBHelper.ExecuteNonQuery(
                "sp_QC_SaveInspectionRM",
                CommandType.StoredProcedure,
                parameters
            );

            return parameters[
                parameters.Length - 1
            ].Value == DBNull.Value
                ? 0
                : Convert.ToInt32(
                    parameters[
                        parameters.Length - 1
                    ].Value
                );
        }


        public int DeleteQCInspectionRM(
            int id,
            string userName)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@ID",
                    id
                ),

                new SqlParameter(
                    "@UpdatedBy",
                    DbValue(
                        userName
                    )
                )
            };

            return DBHelper.ExecuteNonQuery(
                "sp_QC_DeleteInspectionRM",
                CommandType.StoredProcedure,
                parameters
            );
        }


        private QCInspectionRMDetailBLL
            MapInspection(
                DataRow row)
        {
            return new QCInspectionRMDetailBLL
            {
                ID =
                    GetInt(
                        row,
                        "ID"
                    ),

                BilletBoardingID =
                    GetInt(
                        row,
                        "BilletBoardingID"
                    ),

                MTCID =
                    GetInt(
                        row,
                        "MTCID"
                    ),

                Site =
                    GetString(
                        row,
                        "Site"
                    ),

                ProductionShift =
                    GetString(
                        row,
                        "ProductionShift"
                    ),

                ProductionDateValue =
                    GetNullableDate(
                        row,
                        "ProductionDate"
                    ),

                HeatNo =
                    GetString(
                        row,
                        "HeatNo"
                    ),

                Specification =
                    GetString(
                        row,
                        "Specification"
                    ),

                SteelGrade =
                    GetString(
                        row,
                        "SteelGrade"
                    ),

                LengthValue =
                    GetNullableDecimal(
                        row,
                        "Length"
                    ),

                NominalWeightValue =
                    GetNullableDecimal(
                        row,
                        "NominalWeight"
                    ),

                CrossSectionAreaValue =
                    GetNullableDecimal(
                        row,
                        "CrossSectionArea"
                    ),

                BendTestObserved =
                    GetBool(
                        row,
                        "BendTestObserved"
                    ),

                BarSizeValue =
                    GetNullableDecimal(
                        row,
                        "BarSize"
                    ),

                WeightPerBundleValue =
                    GetNullableDecimal(
                        row,
                        "WeightPerBundle"
                    ),

                NoOfBarsPerBundleValue =
                    GetNullableInt(
                        row,
                        "NoOfBarsPerBundle"
                    ),

                NoOfBundlesValue =
                    GetNullableInt(
                        row,
                        "NoOfBundles"
                    ),

                IsWireRodOrCoil =
                    GetBool(
                        row,
                        "IsWireRodOrCoil"
                    ),

                YieldStrengthValue =
                    GetNullableDecimal(
                        row,
                        "YieldStrength"
                    ),

                TensileStrengthValue =
                    GetNullableDecimal(
                        row,
                        "TensileStrength"
                    ),

                TensileYieldRatioValue =
                    GetNullableDecimal(
                        row,
                        "TensileYieldRatio"
                    ),

                ElongationValue =
                    GetNullableDecimal(
                        row,
                        "Elongation"
                    ),

                GaugeLengthValue =
                    GetNullableDecimal(
                        row,
                        "GaugeLength"
                    ),

                CValue =
                    GetNullableDecimal(
                        row,
                        "C"
                    ),

                SiValue =
                    GetNullableDecimal(
                        row,
                        "Si"
                    ),

                MnValue =
                    GetNullableDecimal(
                        row,
                        "Mn"
                    ),

                PValue =
                    GetNullableDecimal(
                        row,
                        "P"
                    ),

                SValue =
                    GetNullableDecimal(
                        row,
                        "S"
                    ),

                NValue =
                    GetNullableDecimal(
                        row,
                        "N"
                    ),

                CeqValue =
                    GetNullableDecimal(
                        row,
                        "Ceq"
                    )
            };
        }


        private static object DbValue(
            string value)
        {
            return string.IsNullOrWhiteSpace(
                value
            )
                ? (object)DBNull.Value
                : value.Trim();
        }


        private static string GetString(
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
                return "";
            }

            return Convert.ToString(
                row[column]
            ).Trim();
        }


        private static int GetInt(
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
                return 0;
            }

            int result;

            return int.TryParse(
                Convert.ToString(
                    row[column]
                ).Trim(),
                out result
            )
                ? result
                : 0;
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

            int result;

            return int.TryParse(
                Convert.ToString(
                    row[column]
                ).Trim(),
                out result
            )
                ? (int?)result
                : null;
        }


        private static decimal GetDecimal(
            DataRow row,
            string column)
        {
            decimal? value =
                GetNullableDecimal(
                    row,
                    column
                );

            return value ?? 0M;
        }


        private static decimal? GetNullableDecimal(
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

            object rawValue =
                row[column];

            if (
                rawValue is decimal ||
                rawValue is double ||
                rawValue is float ||
                rawValue is int ||
                rawValue is long ||
                rawValue is short
            )
            {
                try
                {
                    return Convert.ToDecimal(
                        rawValue
                    );
                }
                catch
                {
                    return null;
                }
            }

            string value =
                Convert.ToString(
                    rawValue
                )
                .Trim();

            if (
                string.IsNullOrWhiteSpace(
                    value
                )
            )
            {
                return null;
            }

            decimal result;

            if (
                decimal.TryParse(
                    value,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out result
                )
            )
            {
                return result;
            }

            if (
                decimal.TryParse(
                    value,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.CurrentCulture,
                    out result
                )
            )
            {
                return result;
            }

            value =
                value
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

            if (
                decimal.TryParse(
                    value,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out result
                )
            )
            {
                return result;
            }

            return null;
        }


        private static DateTime? GetNullableDate(
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

            DateTime result;

            return DateTime.TryParse(
                Convert.ToString(
                    row[column]
                ),
                out result
            )
                ? (DateTime?)result
                : null;
        }


        private static bool GetBool(
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
                return false;
            }

            bool boolResult;

            if (
                bool.TryParse(
                    Convert.ToString(
                        row[column]
                    ),
                    out boolResult
                )
            )
            {
                return boolResult;
            }

            int intResult;

            return int.TryParse(
                Convert.ToString(
                    row[column]
                ),
                out intResult
            )
                && intResult == 1;
        }


        public RMChemicalAnalysisBLL GetChemicalAnalysisByHeatNo(
    string heatNo)
        {
            if (string.IsNullOrWhiteSpace(heatNo))
            {
                return null;
            }

            SqlParameter[] parameters =
            {
        new SqlParameter(
            "@HeatNo",
            SqlDbType.NVarChar,
            50
        )
        {
            Value = heatNo.Trim()
        }
    };

            DataTable dt =
                DBHelper.ExecuteDataTable(
                    "sp_QC_GetChemicalAnalysisByHeat",
                    CommandType.StoredProcedure,
                    parameters
                );

            if (
                dt == null ||
                dt.Rows.Count == 0
            )
            {
                return null;
            }

            DataRow row =
                dt.Rows[0];

            return new RMChemicalAnalysisBLL
            {
                ID =
                    row["ID"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(
                            row["ID"]
                        ),

                HeatNo =
                    Convert.ToString(
                        row["HeatNo"]
                    ).Trim(),

                NoOfBillets =
                    row["NoOfBillets"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(
                            row["NoOfBillets"]
                        ),

                C =
                    GetNullableDecimal(
                        row,
                        "C"
                    ),

                Si =
                    GetNullableDecimal(
                        row,
                        "Si"
                    ),

                Mn =
                    GetNullableDecimal(
                        row,
                        "Mn"
                    ),

                S =
                    GetNullableDecimal(
                        row,
                        "S"
                    ),

                P =
                    GetNullableDecimal(
                        row,
                        "P"
                    ),

                N =
                    GetNullableDecimal(
                        row,
                        "N"
                    ),

                Ceq =
                    GetNullableDecimal(
                        row,
                        "Ceq"
                    ),

                HeatStatus =
                    GetInt(
                        row,
                        "HeatStatus"
                    )
            };
        }


        // ============================================================
        // BILLET BOARD - ADD / EDIT SUPPORT
        // ============================================================

        public BilletBoardBLL GetBilletForEdit(int id)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@ID", SqlDbType.Int)
                    {
                        Value = id
                    }
                };

                DataTable dt =
                    (new DBHelper().GetTableFromSP)(
                        "sp_GetBilletForEdit",
                        p
                    );

                if (
                    dt == null ||
                    dt.Rows.Count == 0
                )
                {
                    return null;
                }

                return JArray
                    .Parse(
                        JsonConvert.SerializeObject(dt)
                    )
                    .ToObject<List<BilletBoardBLL>>()
                    .FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }


        public List<RMChemicalAnalysisBLL>
            GetBilletChemistryForEdit(
                int id)
        {
            try
            {
                var list =
                    new List<RMChemicalAnalysisBLL>();

                SqlParameter[] p =
                {
                    new SqlParameter("@ID", SqlDbType.Int)
                    {
                        Value = id
                    }
                };

                DataTable dt =
                    (new DBHelper().GetTableFromSP)(
                        "sp_GetBilletChemistryForEdit",
                        p
                    );

                if (
                    dt != null &&
                    dt.Rows.Count > 0
                )
                {
                    list =
                        JArray
                            .Parse(
                                JsonConvert.SerializeObject(dt)
                            )
                            .ToObject<
                                List<RMChemicalAnalysisBLL>
                            >();
                }

                return list;
            }
            catch
            {
                throw;
            }
        }


        public bool IsBilletBoardingExistsForEdit(
            string billetBoarding,
            int currentID)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter(
                        "@BilletBoarding",
                        SqlDbType.NVarChar,
                        100
                    )
                    {
                        Value =
                            string.IsNullOrWhiteSpace(
                                billetBoarding
                            )
                                ? (object)DBNull.Value
                                : billetBoarding.Trim()
                    },

                    new SqlParameter(
                        "@CurrentID",
                        SqlDbType.Int
                    )
                    {
                        Value = currentID
                    }
                };

                DataTable dt =
                    (new DBHelper().GetTableFromSP)(
                        "sp_CheckBilletBoardingExistsForEdit",
                        p
                    );

                if (
                    dt == null ||
                    dt.Rows.Count == 0
                )
                {
                    return false;
                }

                return Convert.ToInt32(
                    dt.Rows[0]["Total"]
                ) > 0;
            }
            catch
            {
                throw;
            }
        }


        public List<string> GetDuplicateHeatNosForEdit(
            List<string> heatNos,
            int currentID)
        {
            try
            {
                var duplicates =
                    new List<string>();

                if (
                    heatNos == null ||
                    heatNos.Count == 0
                )
                {
                    return duplicates;
                }

                string heatNoCsv =
                    string.Join(
                        ",",
                        heatNos
                            .Where(
                                x =>
                                    !string.IsNullOrWhiteSpace(x)
                            )
                            .Select(
                                x => x.Trim()
                            )
                    );

                SqlParameter[] p =
                {
                    new SqlParameter(
                        "@HeatNos",
                        SqlDbType.NVarChar,
                        -1
                    )
                    {
                        Value = heatNoCsv
                    },

                    new SqlParameter(
                        "@CurrentID",
                        SqlDbType.Int
                    )
                    {
                        Value = currentID
                    }
                };

                DataTable dt =
                    (new DBHelper().GetTableFromSP)(
                        "sp_GetDuplicateHeatNosForEdit",
                        p
                    );

                if (
                    dt != null &&
                    dt.Rows.Count > 0
                )
                {
                    foreach (
                        DataRow row
                        in dt.Rows
                    )
                    {
                        string heatNo =
                            Convert.ToString(
                                row["HeatNo"]
                            );

                        if (
                            !string.IsNullOrWhiteSpace(
                                heatNo
                            )
                        )
                        {
                            duplicates.Add(
                                heatNo.Trim()
                            );
                        }
                    }
                }

                return duplicates;
            }
            catch
            {
                throw;
            }
        }


        public int DeactivateBilletChemistry(
            int currentID,
            string updatedBy)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter(
                        "@CurrentID",
                        SqlDbType.Int
                    )
                    {
                        Value = currentID
                    },

                    new SqlParameter(
                        "@UpdatedBy",
                        SqlDbType.NVarChar,
                        100
                    )
                    {
                        Value =
                            string.IsNullOrWhiteSpace(
                                updatedBy
                            )
                                ? (object)DBNull.Value
                                : updatedBy.Trim()
                    }
                };

                return new DBHelper()
                    .ExecuteNonQueryReturn(
                        "sp_DeactivateBilletChemistry",
                        p
                    );
            }
            catch
            {
                throw;
            }
        }


        public int DeactivateBilletBoardHeatRows(
            int currentID,
            string updatedBy)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter(
                        "@CurrentID",
                        SqlDbType.Int
                    )
                    {
                        Value = currentID
                    },

                    new SqlParameter(
                        "@UpdatedBy",
                        SqlDbType.NVarChar,
                        100
                    )
                    {
                        Value =
                            string.IsNullOrWhiteSpace(
                                updatedBy
                            )
                                ? (object)DBNull.Value
                                : updatedBy.Trim()
                    }
                };

                return new DBHelper()
                    .ExecuteNonQueryReturn(
                        "sp_DeactivateBilletBoardHeatRows",
                        p
                    );
            }
            catch
            {
                throw;
            }
        }


        public int UpdateBilletBoarding(
            BilletBoardBLL model)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter(
                        "@ID",
                        SqlDbType.Int
                    )
                    {
                        Value = model.ID
                    },

                    new SqlParameter(
                        "@Date",
                        SqlDbType.Date
                    )
                    {
                        Value =
                            model.Date.HasValue
                                ? (object)model.Date.Value
                                : DBNull.Value
                    },

                    new SqlParameter(
                        "@BilletBoarding",
                        SqlDbType.NVarChar,
                        100
                    )
                    {
                        Value =
                            string.IsNullOrWhiteSpace(
                                model.BilletBoarding
                            )
                                ? (object)DBNull.Value
                                : model.BilletBoarding.Trim()
                    },

                    new SqlParameter(
                        "@PlantName",
                        SqlDbType.NVarChar,
                        100
                    )
                    {
                        Value =
                            string.IsNullOrWhiteSpace(
                                model.PlantName
                            )
                                ? (object)DBNull.Value
                                : model.PlantName.Trim()
                    },

                    new SqlParameter(
                        "@Shift",
                        SqlDbType.NVarChar,
                        50
                    )
                    {
                        Value =
                            string.IsNullOrWhiteSpace(
                                model.Shift
                            )
                                ? (object)DBNull.Value
                                : model.Shift.Trim()
                    },

                    new SqlParameter(
                        "@SteelGrade",
                        SqlDbType.NVarChar,
                        100
                    )
                    {
                        Value =
                            string.IsNullOrWhiteSpace(
                                model.SteelGrade
                            )
                                ? (object)DBNull.Value
                                : model.SteelGrade.Trim()
                    },

                    new SqlParameter(
                        "@Profile",
                        SqlDbType.NVarChar,
                        100
                    )
                    {
                        Value =
                            string.IsNullOrWhiteSpace(
                                model.Profile
                            )
                                ? (object)DBNull.Value
                                : model.Profile.Trim()
                    },

                    new SqlParameter(
                        "@Size",
                        SqlDbType.NVarChar,
                        100
                    )
                    {
                        Value =
                            string.IsNullOrWhiteSpace(
                                model.Size
                            )
                                ? (object)DBNull.Value
                                : model.Size.Trim()
                    },

                    new SqlParameter(
                        "@ProductSpecs",
                        SqlDbType.NVarChar,
                        200
                    )
                    {
                        Value =
                            string.IsNullOrWhiteSpace(
                                model.ProductSpecs
                            )
                                ? (object)DBNull.Value
                                : model.ProductSpecs.Trim()
                    },

                    new SqlParameter(
                        "@BilletLength",
                        SqlDbType.NVarChar,
                        50
                    )
                    {
                        Value =
                            string.IsNullOrWhiteSpace(
                                model.BilletLength
                            )
                                ? (object)DBNull.Value
                                : model.BilletLength.Trim()
                    },

                    new SqlParameter(
                        "@CrossSection",
                        SqlDbType.NVarChar,
                        100
                    )
                    {
                        Value =
                            string.IsNullOrWhiteSpace(
                                model.CrossSection
                            )
                                ? (object)DBNull.Value
                                : model.CrossSection.Trim()
                    },

                    new SqlParameter(
                        "@BilletWeight",
                        SqlDbType.Decimal
                    )
                    {
                        Value = model.BilletWeight
                    },

                    new SqlParameter(
                        "@Remarks",
                        SqlDbType.NVarChar,
                        -1
                    )
                    {
                        Value =
                            string.IsNullOrWhiteSpace(
                                model.Remarks
                            )
                                ? (object)DBNull.Value
                                : model.Remarks.Trim()
                    },

                    new SqlParameter(
                        "@UpdatedBy",
                        SqlDbType.NVarChar,
                        100
                    )
                    {
                        Value =
                            string.IsNullOrWhiteSpace(
                                model.UpdatedBy
                            )
                                ? (object)DBNull.Value
                                : model.UpdatedBy.Trim()
                    }
                };

                DataTable dt =
                    (new DBHelper().GetTableFromSP)(
                        "sp_UpdateBilletBoarding",
                        p
                    );

                if (
                    dt != null &&
                    dt.Rows.Count > 0 &&
                    dt.Columns.Contains("ID")
                )
                {
                    return Convert.ToInt32(
                        dt.Rows[0]["ID"]
                    );
                }

                return model.ID;
            }
            catch
            {
                throw;
            }
        }


        // Compatibility wrapper for older controller code.
        public void UpdateBillet(
            BilletBoardBLL model)
        {
            UpdateBilletBoarding(model);
        }
    }
}