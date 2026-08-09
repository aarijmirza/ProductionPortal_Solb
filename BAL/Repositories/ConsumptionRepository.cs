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
    public class ConsumptionRepository
    {
        public static DataTable _dt;
        public static DataSet _ds;


        public ConsumptionRepository()
        {
            _dt =
                new DataTable();

            _ds =
                new DataSet();
        }


        // =====================================================
        // LIST
        // =====================================================

        public List<PlantConsumptionBLL>
            GetPlantConsumption(
                DateTime fromDate,
                DateTime toDate)
        {
            try
            {
                List<PlantConsumptionBLL> list =
                    new List<PlantConsumptionBLL>();


                SqlParameter[] p =
                {
                    new SqlParameter(
                        "@FromDate",
                        SqlDbType.Date
                    )
                    {
                        Value =
                            fromDate.Date
                    },

                    new SqlParameter(
                        "@ToDate",
                        SqlDbType.Date
                    )
                    {
                        Value =
                            toDate.Date
                    }
                };


                DataTable dt =
                    new DBHelper()
                        .GetTableFromSP(
                            "sp_GetPlantConsumption",
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
                                JsonConvert
                                    .SerializeObject(
                                        dt
                                    )
                            )
                            .ToObject<
                                List<PlantConsumptionBLL>
                            >();
                }


                return list;
            }
            catch
            {
                throw;
            }
        }


        // =====================================================
        // GET EDIT GROUP BY ID
        // =====================================================

        public List<PlantConsumptionBLL>
            GetPlantConsumptionGroupByID(
                int id)
        {
            try
            {
                List<PlantConsumptionBLL> list =
                    new List<PlantConsumptionBLL>();


                SqlParameter[] p =
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
                            "sp_GetPlantConsumptionGroupByID",
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
                                JsonConvert
                                    .SerializeObject(
                                        dt
                                    )
                            )
                            .ToObject<
                                List<PlantConsumptionBLL>
                            >();
                }


                return list;
            }
            catch
            {
                throw;
            }
        }


        // =====================================================
        // GET ALL PLANTS BY DATE
        // =====================================================

        public List<PlantConsumptionBLL>
            GetPlantConsumptionByDateAll(
                DateTime date)
        {
            try
            {
                List<PlantConsumptionBLL> list =
                    new List<PlantConsumptionBLL>();


                SqlParameter[] p =
                {
                    new SqlParameter(
                        "@Date",
                        SqlDbType.Date
                    )
                    {
                        Value =
                            date.Date
                    }
                };


                DataTable dt =
                    new DBHelper()
                        .GetTableFromSP(
                            "sp_GetPlantConsumptionByDateAll",
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
                                JsonConvert
                                    .SerializeObject(
                                        dt
                                    )
                            )
                            .ToObject<
                                List<PlantConsumptionBLL>
                            >();
                }


                return list;
            }
            catch
            {
                throw;
            }
        }


        // =====================================================
        // SAVE SMP + RM1 + RM2
        // =====================================================

        public int SavePlantWiseConsumption(
            List<PlantConsumptionBLL> records)
        {
            try
            {
                if (
                    records == null ||
                    records.Count == 0
                )
                {
                    return 0;
                }


                int firstSavedID =
                    0;


                foreach (
                    PlantConsumptionBLL model
                    in records
                )
                {
                    SqlParameter[] p =
                    {
                        new SqlParameter(
                            "@ID",
                            model.ID
                        ),

                        new SqlParameter(
                            "@Date",
                            model.Date.HasValue
                                ? (object)model.Date.Value.Date
                                : DBNull.Value
                        ),

                        new SqlParameter(
                            "@Plant",
                            DbValue(
                                model.Plant
                            )
                        ),

                        new SqlParameter(
                            "@TotalProductBillet",
                            DbValue(
                                model.TotalProductBillet
                            )
                        ),

                        new SqlParameter(
                            "@LPG",
                            DbValue(
                                model.LPG
                            )
                        ),

                        new SqlParameter(
                            "@Oxygen",
                            DbValue(
                                model.Oxygen
                            )
                        ),

                        new SqlParameter(
                            "@Nitrogen",
                            DbValue(
                                model.Nitrogen
                            )
                        ),

                        new SqlParameter(
                            "@Argon",
                            DbValue(
                                model.Argon
                            )
                        ),

                        new SqlParameter(
                            "@WaterConsumption",
                            DbValue(
                                model.WaterConsumption
                            )
                        ),

                        new SqlParameter(
                            "@PowerConsumption",
                            DbValue(
                                model.PowerConsumption
                            )
                        ),

                        new SqlParameter(
                            "@FuelConsumption",
                            model.FuelConsumption.HasValue
                                ? (object)model.FuelConsumption.Value
                                : DBNull.Value
                        ),

                        new SqlParameter(
                            "@LPGm3ton",
                            DbValue(
                                model.LPGm3ton
                            )
                        ),

                        new SqlParameter(
                            "@OxygenNm3ton",
                            DbValue(
                                model.OxygenNm3ton
                            )
                        ),

                        new SqlParameter(
                            "@NitrogenNm3ton",
                            DbValue(
                                model.NitrogenNm3ton
                            )
                        ),

                        new SqlParameter(
                            "@ArgonNm3ton",
                            DbValue(
                                model.ArgonNm3ton
                            )
                        ),

                        new SqlParameter(
                            "@PowerConsumptionKWHton",
                            DbValue(
                                model.PowerConsumptionKWHton
                            )
                        ),

                        new SqlParameter(
                            "@WaterConsumptionM3",
                            DbValue(
                                model.WaterConsumptionM3
                            )
                        ),

                        new SqlParameter(
                            "@StatusID",
                            model.StatusID ?? 1
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
                                : DBNull.Value
                        ),

                        new SqlParameter(
                            "@UpdatedBy",
                            DbValue(
                                model.UpdatedBy
                            )
                        ),

                        new SqlParameter(
                            "@UpdatedDate",
                            model.UpdatedDate.HasValue
                                ? (object)model.UpdatedDate.Value
                                : DBNull.Value
                        )
                    };


                    int savedID =
                        new DBHelper()
                            .ExecuteNonQueryReturn(
                                "sp_SavePlantConsumption",
                                p
                            );


                    if (
                        firstSavedID == 0 &&
                        savedID > 0
                    )
                    {
                        firstSavedID =
                            savedID;
                    }
                }


                return firstSavedID;
            }
            catch
            {
                throw;
            }
        }


        // =====================================================
        // DELETE
        // =====================================================

        public int DeletePlantConsumption(
            int id,
            string updatedBy)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter(
                        "@ID",
                        id
                    ),

                    new SqlParameter(
                        "@UpdatedBy",
                        DbValue(
                            updatedBy
                        )
                    )
                };


                return new DBHelper()
                    .ExecuteNonQueryReturn(
                        "sp_DeletePlantConsumption",
                        p
                    );
            }
            catch
            {
                throw;
            }
        }


        // =====================================================
        // COMMON DB NULL HANDLER
        // =====================================================

        private object DbValue(
            object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }


            string text =
                value as string;


            if (
                text != null &&
                string.IsNullOrWhiteSpace(
                    text
                )
            )
            {
                return DBNull.Value;
            }


            return value;
        }
    }
}