using DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using WebAPICode.Helpers;

namespace DAL.Repository
{
    public class SMPDayWiseProductionRepository
    {
        public List<SMPDayWiseProductionBLL> GetAll(
            DateTime? fromDate,
            DateTime? toDate)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@FromDate",
                    SqlDbType.Date
                )
                {
                    Value =
                        fromDate.HasValue
                            ? (object)fromDate.Value.Date
                            : DBNull.Value
                },

                new SqlParameter(
                    "@ToDate",
                    SqlDbType.Date
                )
                {
                    Value =
                        toDate.HasValue
                            ? (object)toDate.Value.Date
                            : DBNull.Value
                }
            };

            DataTable dt =
                new DBHelper()
                    .GetTableFromSP(
                        "sp_GetSMPDayWiseProduction",
                        parameters
                    );

            if (
                dt == null ||
                dt.Rows.Count == 0
            )
            {
                return
                    new List<SMPDayWiseProductionBLL>();
            }

            return
                JArray
                    .Parse(
                        JsonConvert.SerializeObject(dt)
                    )
                    .ToObject<
                        List<SMPDayWiseProductionBLL>
                    >();
        }


        public SMPDayWiseProductionBLL GetByID(
            int id)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@ID",
                    SqlDbType.Int
                )
                {
                    Value = id
                }
            };

            DataTable dt =
                new DBHelper()
                    .GetTableFromSP(
                        "sp_GetSMPDayWiseProductionByID",
                        parameters
                    );

            if (dt == null || dt.Rows.Count == 0)
                return null;

            return
                JArray
                    .Parse(
                        JsonConvert.SerializeObject(dt)
                    )
                    .ToObject<List<SMPDayWiseProductionBLL>>()
                    .FirstOrDefault();
        }


        public int ReplaceAll(
            List<SMPDayWiseProductionBLL> rows,
            string createdBy)
        {
            if (
                rows == null ||
                rows.Count == 0
            )
            {
                throw new Exception(
                    "No valid production rows were found in the Excel file."
                );
            }

            DataTable uploadTable =
                CreateUploadTable();

            foreach (
                SMPDayWiseProductionBLL item
                in rows
            )
            {
                DataRow row =
                    uploadTable.NewRow();

                row["Date"] =
                    item.Date.Date;

                row["Month"] =
                    DbValue(item.Month);

                row["NoOfHeats"] =
                    DbValue(item.NoOfHeats);

                row["ProductionPlan"] =
                    DbValue(item.ProductionPlan);

                row["TotalCastedTon"] =
                    DbValue(item.TotalCastedTon);

                row["TLSTon"] =
                    DbValue(item.TLSTon);

                row["ScrapCharge"] =
                    DbValue(item.ScrapCharge);

                row["DRI_OLD_DRI"] =
                    DbValue(item.DRI_OLD_DRI);

                row["HBI"] =
                    DbValue(item.HBI);

                row["AverageHeatWeight"] =
                    DbValue(item.AverageHeatWeight);

                row["CCMProductivity"] =
                    DbValue(item.CCMProductivity);

                row["PerformanceRate"] =
                    DbValue(item.PerformanceRate);

                row["SMPMaterialYield"] =
                    DbValue(item.SMPMaterialYield);

                row["Availability"] =
                    DbValue(item.Availability);

                row["QualityYield"] =
                    DbValue(item.QualityYield);

                row["PowerOnTime"] =
                    DbValue(item.PowerOnTime);

                row["NetTapToTap"] =
                    DbValue(item.NetTapToTap);

                row["AverageCastingTime"] =
                    DbValue(item.AverageCastingTime);

                row["LengthOfSequence"] =
                    DbValue(item.LengthOfSequence);

                row["Electrical"] =
                    DbValue(item.Electrical);

                row["EAFLF"] =
                    DbValue(item.EAFLF);

                row["LPG"] =
                    DbValue(item.LPG);

                row["O2"] =
                    DbValue(item.O2);

                row["Argon"] =
                    DbValue(item.Argon);

                row["N2"] =
                    DbValue(item.N2);

                row["DRI_HBI"] =
                    DbValue(item.DRI_HBI);

                row["ScrapConsumption"] =
                    DbValue(item.ScrapConsumption);

                row["FeSi"] =
                    DbValue(item.FeSi);

                row["SiMn"] =
                    DbValue(item.SiMn);

                row["EAFElectrode"] =
                    DbValue(item.EAFElectrode);

                row["LRFElectrode"] =
                    DbValue(item.LRFElectrode);

                row["Flourspar"] =
                    DbValue(item.Flourspar);

                row["CalcinedCarbon"] =
                    DbValue(item.CalcinedCarbon);

                row["ChargeCoal"] =
                    DbValue(item.ChargeCoal);

                row["RiceHusk"] =
                    DbValue(item.RiceHusk);

                row["Lime"] =
                    DbValue(item.Lime);

                row["LFLime"] =
                    DbValue(item.LFLime);

                row["DoloLime"] =
                    DbValue(item.DoloLime);

                row["ElectricalDelayEM"] =
                    DbValue(item.ElectricalDelayEM);

                row["MechanicalDelayMM"] =
                    DbValue(item.MechanicalDelayMM);

                row["RefractoryDelayRF"] =
                    DbValue(item.RefractoryDelayRF);

                row["OperationDelayO"] =
                    DbValue(item.OperationDelayO);

                row["UtilityDelayU"] =
                    DbValue(item.UtilityDelayU);

                row["CranesDelayCR"] =
                    DbValue(item.CranesDelayCR);

                row["MaterialHandlingRMH"] =
                    DbValue(item.MaterialHandlingRMH);

                row["ProcurementPR"] =
                    DbValue(item.ProcurementPR);

                row["CCMOperationO"] =
                    DbValue(item.CCMOperationO);

                row["OutsideOS"] =
                    DbValue(item.OutsideOS);

                row["PlannedMaintenance"] =
                    DbValue(item.PlannedMaintenance);

                row["ScheduleTime"] =
                    DbValue(item.ScheduleTime);

                row["UtilizedTime"] =
                    DbValue(item.UtilizedTime);

                row["TotalDelayTime"] =
                    DbValue(item.TotalDelayTime);

                uploadTable.Rows.Add(row);
            }


            SqlParameter rowsParameter =
                new SqlParameter(
                    "@Rows",
                    SqlDbType.Structured
                );

            rowsParameter.TypeName =
                "dbo.SMPDayWiseProductionUploadTypeV2";

            rowsParameter.Value =
                uploadTable;


            SqlParameter[] parameters =
            {
                rowsParameter,

                new SqlParameter(
                    "@CreatedBy",
                    SqlDbType.NVarChar,
                    100
                )
                {
                    Value =
                        string.IsNullOrWhiteSpace(createdBy)
                            ? (object)DBNull.Value
                            : createdBy.Trim()
                }
            };


            DataTable result =
                new DBHelper()
                    .GetTableFromSP(
                        "sp_ReplaceSMPDayWiseProduction",
                        parameters
                    );


            if (
                result != null &&
                result.Rows.Count > 0 &&
                result.Columns.Contains(
                    "InsertedRows"
                )
            )
            {
                return
                    Convert.ToInt32(
                        result.Rows[0]["InsertedRows"]
                    );
            }

            return 0;
        }


        private DataTable CreateUploadTable()
        {
            DataTable dt =
                new DataTable();

            dt.Columns.Add(
                "Date",
                typeof(DateTime)
            );

            dt.Columns.Add(
                "Month",
                typeof(string)
            );

            dt.Columns.Add("NoOfHeats", typeof(int));
            dt.Columns.Add("ProductionPlan", typeof(decimal));
            dt.Columns.Add("TotalCastedTon", typeof(decimal));
            dt.Columns.Add("TLSTon", typeof(decimal));

            dt.Columns.Add("ScrapCharge", typeof(decimal));
            dt.Columns.Add("DRI_OLD_DRI", typeof(decimal));
            dt.Columns.Add("HBI", typeof(decimal));
            dt.Columns.Add("AverageHeatWeight", typeof(decimal));
            dt.Columns.Add("CCMProductivity", typeof(decimal));
            dt.Columns.Add("PerformanceRate", typeof(decimal));
            dt.Columns.Add("SMPMaterialYield", typeof(decimal));
            dt.Columns.Add("Availability", typeof(decimal));
            dt.Columns.Add("QualityYield", typeof(decimal));
            dt.Columns.Add("PowerOnTime", typeof(decimal));
            dt.Columns.Add("NetTapToTap", typeof(decimal));
            dt.Columns.Add("AverageCastingTime", typeof(decimal));

            dt.Columns.Add("LengthOfSequence", typeof(string));

            dt.Columns.Add("Electrical", typeof(decimal));
            dt.Columns.Add("EAFLF", typeof(decimal));
            dt.Columns.Add("LPG", typeof(decimal));
            dt.Columns.Add("O2", typeof(decimal));
            dt.Columns.Add("Argon", typeof(decimal));
            dt.Columns.Add("N2", typeof(decimal));

            dt.Columns.Add("DRI_HBI", typeof(decimal));
            dt.Columns.Add("ScrapConsumption", typeof(decimal));
            dt.Columns.Add("FeSi", typeof(decimal));
            dt.Columns.Add("SiMn", typeof(decimal));
            dt.Columns.Add("EAFElectrode", typeof(decimal));
            dt.Columns.Add("LRFElectrode", typeof(decimal));
            dt.Columns.Add("Flourspar", typeof(decimal));
            dt.Columns.Add("CalcinedCarbon", typeof(decimal));
            dt.Columns.Add("ChargeCoal", typeof(decimal));
            dt.Columns.Add("RiceHusk", typeof(decimal));
            dt.Columns.Add("Lime", typeof(decimal));
            dt.Columns.Add("LFLime", typeof(decimal));
            dt.Columns.Add("DoloLime", typeof(decimal));

            dt.Columns.Add("ElectricalDelayEM", typeof(decimal));
            dt.Columns.Add("MechanicalDelayMM", typeof(decimal));
            dt.Columns.Add("RefractoryDelayRF", typeof(decimal));
            dt.Columns.Add("OperationDelayO", typeof(decimal));
            dt.Columns.Add("UtilityDelayU", typeof(decimal));
            dt.Columns.Add("CranesDelayCR", typeof(decimal));
            dt.Columns.Add("MaterialHandlingRMH", typeof(decimal));
            dt.Columns.Add("ProcurementPR", typeof(decimal));
            dt.Columns.Add("CCMOperationO", typeof(decimal));
            dt.Columns.Add("OutsideOS", typeof(decimal));

            dt.Columns.Add("PlannedMaintenance", typeof(decimal));
            dt.Columns.Add("ScheduleTime", typeof(decimal));
            dt.Columns.Add("UtilizedTime", typeof(decimal));
            dt.Columns.Add("TotalDelayTime", typeof(decimal));

            return dt;
        }


        private object DbValue(object value)
        {
            if (value == null)
                return DBNull.Value;

            string text =
                Convert.ToString(value);

            if (
                value is string &&
                string.IsNullOrWhiteSpace(text)
            )
            {
                return DBNull.Value;
            }

            return value;
        }
    }
}