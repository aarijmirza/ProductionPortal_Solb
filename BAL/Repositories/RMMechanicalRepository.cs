using DAL.Models;
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
    public class RMMechanicalRepository
    {
        public List<QCBilletBoardingRowBLL> GetBilletBoardingRows(
    string rm)
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
                string.IsNullOrWhiteSpace(rm)
                    ? "RM1"
                    : rm.Trim()
        }
    };


            DataTable dt =
                DBHelper.ExecuteDataTable(
                    "sp_QC_GetBilletBoardingRows",
                    CommandType.StoredProcedure,
                    parameters
                );


            if (
                dt == null ||
                dt.Rows.Count == 0
            )
            {
                return list;
            }


            foreach (
                DataRow row
                in dt.Rows
            )
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
                                "BarySize"
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


            if (
                dt == null ||
                dt.Rows.Count == 0
            )
            {
                return list;
            }


            foreach (
                DataRow row
                in dt.Rows
            )
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


            /*
             * Defensive de-duplication.
             * Group by HeatNo, NOT by ID.
             */
            return
                list
                    .Where(
                        x =>
                            !string.IsNullOrWhiteSpace(
                                x.HeatNo
                            )
                    )
                    .GroupBy(
                        x =>
                            x.HeatNo.Trim(),
                        StringComparer.OrdinalIgnoreCase
                    )
                    .Select(
                        g =>
                            g
                                .OrderByDescending(
                                    x => x.ID
                                )
                                .First()
                    )
                    .OrderByDescending(
                        x => x.ID
                    )
                    .ToList();
        }



        public QCInspectionRMDetailBLL GetQCInspectionRMByID(
            int id)
        {
            if (id <= 0)
            {
                return null;
            }


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


            return
                MapQCInspectionRMDetail(
                    dt.Rows[0]
                );
        }



        public QCInspectionRMDetailBLL GetQCInspectionRMFromBoarding(
            int boardingID)
        {
            if (boardingID <= 0)
            {
                return null;
            }


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


            return
                MapQCInspectionRMDetail(
                    dt.Rows[0]
                );
        }



        public QCInspectionRMDetailBLL GetMTCDetails(
            int mtcID)
        {
            if (mtcID <= 0)
            {
                return null;
            }


            SqlParameter[] parameters =
            {
        new SqlParameter(
            "@MTCID",
            SqlDbType.Int
        )
        {
            Value =
                mtcID
        }
    };


            /*
             * sp_QC_GetMTCDetails:
             *
             * QCInspectionRM
             *   Product + Mechanical
             *
             * RMChemicalAnalysis
             *   C, Si, Mn, P, S, N, Ceq
             */
            DataTable dt =
                DBHelper.ExecuteDataTable(
                    "sp_QC_GetMTCDetails",
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


            return
                MapQCInspectionRMDetail(
                    dt.Rows[0]
                );
        }



        public int SaveQCInspectionRM(
            QCInspectionRMDetailBLL model)
        {
            if (model == null)
            {
                return 0;
            }

            model.HeatNo =
                (model.HeatNo ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(model.HeatNo))
            {
                return 0;
            }

            /*
             * MTC rows use QCInspectionRM.ID as their identity. If the View
             * posts only MTCID, convert it to ID so the existing row updates.
             * The stored procedure also performs a HeatNo-based upsert as the
             * final database-level duplicate guard.
             */
            if (model.ID <= 0 && model.MTCID > 0)
            {
                model.ID = model.MTCID;
            }


            SqlParameter[] parameters =
            {
        new SqlParameter(
            "@ID",
            SqlDbType.Int
        )
        {
            Value =
                model.ID
        },

        new SqlParameter(
            "@BilletBoardingID",
            SqlDbType.Int
        )
        {
            Value =
                model.BilletBoardingID > 0
                    ? (object)model.BilletBoardingID
                    : DBNull.Value
        },

        new SqlParameter(
            "@MTCID",
            SqlDbType.Int
        )
        {
            Value =
                model.MTCID > 0
                    ? (object)model.MTCID
                    : DBNull.Value
        },

        new SqlParameter(
            "@Site",
            SqlDbType.NVarChar,
            20
        )
        {
            Value =
                DbValue(
                    model.Site
                )
        },

        new SqlParameter(
            "@ProductionDate",
            SqlDbType.Date
        )
        {
            Value =
                model.ProductionDateValue.HasValue
                    ? (object)model.ProductionDateValue.Value.Date
                    : DBNull.Value
        },

        new SqlParameter(
            "@ProductionShift",
            SqlDbType.NVarChar,
            30
        )
        {
            Value =
                DbValue(
                    model.ProductionShift
                )
        },

        new SqlParameter(
            "@HeatNo",
            SqlDbType.NVarChar,
            50
        )
        {
            Value =
                DbValue(
                    model.HeatNo
                )
        },

        new SqlParameter(
            "@Specification",
            SqlDbType.NVarChar,
            200
        )
        {
            Value =
                DbValue(
                    model.Specification
                )
        },

        new SqlParameter(
            "@SteelGrade",
            SqlDbType.NVarChar,
            100
        )
        {
            Value =
                DbValue(
                    model.SteelGrade
                )
        },

        new SqlParameter(
            "@BarSize",
            SqlDbType.Decimal
        )
        {
            Value =
                DbValueDecimal(
                    model.BarSizeValue,
                    model.BarSize
                ),
            Precision = 18,
            Scale = 4
        },

        new SqlParameter(
            "@Length",
            SqlDbType.Decimal
        )
        {
            Value =
                DbValueDecimal(
                    model.LengthValue,
                    model.Length
                ),
            Precision = 18,
            Scale = 4
        },

        new SqlParameter(
            "@WeightPerBundle",
            SqlDbType.Decimal
        )
        {
            Value =
                DbValueDecimal(
                    model.WeightPerBundleValue,
                    model.WeightPerBundle
                ),
            Precision = 18,
            Scale = 4
        },

        new SqlParameter(
            "@NominalWeight",
            SqlDbType.Decimal
        )
        {
            Value =
                DbValueDecimal(
                    model.NominalWeightValue,
                    model.NominalWeight
                ),
            Precision = 18,
            Scale = 4
        },

        new SqlParameter(
            "@CrossSectionArea",
            SqlDbType.Decimal
        )
        {
            Value =
                DbValueDecimal(
                    model.CrossSectionAreaValue,
                    model.CrossSectionArea
                ),
            Precision = 18,
            Scale = 4
        },

        new SqlParameter(
            "@NoOfBarsPerBundle",
            SqlDbType.Int
        )
        {
            Value =
                DbValueInt(
                    model.NoOfBarsPerBundleValue,
                    model.NoOfBarsPerBundle
                )
        },

        new SqlParameter(
            "@NoOfBundles",
            SqlDbType.Int
        )
        {
            Value =
                DbValueInt(
                    model.NoOfBundlesValue,
                    model.NoOfBundles
                )
        },

        new SqlParameter(
            "@BendTestObserved",
            SqlDbType.Bit
        )
        {
            Value =
                model.BendTestObserved
        },

        new SqlParameter(
            "@IsWireRodOrCoil",
            SqlDbType.Bit
        )
        {
            Value =
                model.IsWireRodOrCoil
        },

        new SqlParameter(
            "@YieldStrength",
            SqlDbType.Decimal
        )
        {
            Value =
                DbValueDecimal(
                    model.YieldStrengthValue,
                    model.YieldStrength
                ),
            Precision = 18,
            Scale = 4
        },

        new SqlParameter(
            "@TensileStrength",
            SqlDbType.Decimal
        )
        {
            Value =
                DbValueDecimal(
                    model.TensileStrengthValue,
                    model.TensileStrength
                ),
            Precision = 18,
            Scale = 4
        },

        new SqlParameter(
            "@TensileYieldRatio",
            SqlDbType.Decimal
        )
        {
            Value =
                DbValueDecimal(
                    model.TensileYieldRatioValue,
                    model.TensileYieldRatio
                ),
            Precision = 18,
            Scale = 6
        },

        new SqlParameter(
            "@Elongation",
            SqlDbType.Decimal
        )
        {
            Value =
                DbValueDecimal(
                    model.ElongationValue,
                    model.Elongation
                ),
            Precision = 18,
            Scale = 4
        },

        new SqlParameter(
            "@GaugeLength",
            SqlDbType.Decimal
        )
        {
            Value =
                DbValueDecimal(
                    model.GaugeLengthValue,
                    model.GaugeLength
                ),
            Precision = 18,
            Scale = 4
        },

        new SqlParameter(
            "@CreatedBy",
            SqlDbType.NVarChar,
            100
        )
        {
            Value =
                DbValue(
                    model.CreatedBy
                )
        }
    };


            DataTable dt =
                DBHelper.ExecuteDataTable(
                    "sp_QC_SaveInspectionRM",
                    CommandType.StoredProcedure,
                    parameters
                );


            if (
                dt != null &&
                dt.Rows.Count > 0 &&
                dt.Columns.Contains(
                    "ID"
                )
            )
            {
                return
                    GetInt(
                        dt.Rows[0],
                        "ID"
                    );
            }


            return 0;
        }



        public int DeleteQCInspectionRM(
            int id,
            string deletedBy)
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
        },

        new SqlParameter(
            "@UpdatedBy",
            SqlDbType.NVarChar,
            100
        )
        {
            Value =
                DbValue(
                    deletedBy
                )
        }
    };


            DataTable dt =
                DBHelper.ExecuteDataTable(
                    "sp_QC_DeleteInspectionRM",
                    CommandType.StoredProcedure,
                    parameters
                );


            if (
                dt != null &&
                dt.Rows.Count > 0 &&
                dt.Columns.Contains(
                    "AffectedRows"
                )
            )
            {
                return
                    GetInt(
                        dt.Rows[0],
                        "AffectedRows"
                    );
            }


            return 0;
        }



        /* ================================================================
           COMMON MAPPER
           ================================================================ */

        private QCInspectionRMDetailBLL MapQCInspectionRMDetail(
            DataRow row)
        {
            if (row == null)
            {
                return null;
            }


            var model =
                new QCInspectionRMDetailBLL();


            model.ID =
                GetInt(
                    row,
                    "ID"
                );

            model.MTCID =
                GetInt(
                    row,
                    "MTCID"
                );

            model.BilletBoardingID =
                GetInt(
                    row,
                    "BilletBoardingID"
                );

            model.Site =
                GetString(
                    row,
                    "Site"
                );

            model.ProductionShift =
                GetString(
                    row,
                    "ProductionShift"
                );


            DateTime? productionDate =
                GetNullableDateTime(
                    row,
                    "ProductionDateValue"
                );

            if (!productionDate.HasValue)
            {
                productionDate =
                    GetNullableDateTime(
                        row,
                        "ProductionDate"
                    );
            }

            model.ProductionDateValue =
                productionDate;

            model.ProductionDate =
                productionDate.HasValue
                    ? productionDate.Value.ToString(
                        "dd-MM-yyyy"
                    )
                    : "";


            model.HeatNo =
                GetString(
                    row,
                    "HeatNo"
                );

            model.Specification =
                GetString(
                    row,
                    "Specification"
                );

            model.SteelGrade =
                GetString(
                    row,
                    "SteelGrade"
                );


            SetDecimalPair(
                row,
                "BarSize",
                value =>
                {
                    model.BarSizeValue =
                        value;

                    model.BarSize =
                        FormatDecimal(
                            value
                        );
                }
            );

            SetDecimalPair(
                row,
                "Length",
                value =>
                {
                    model.LengthValue =
                        value;

                    model.Length =
                        FormatDecimal(
                            value
                        );
                }
            );

            SetDecimalPair(
                row,
                "WeightPerBundle",
                value =>
                {
                    model.WeightPerBundleValue =
                        value;

                    model.WeightPerBundle =
                        FormatDecimal(
                            value
                        );
                }
            );

            SetDecimalPair(
                row,
                "NominalWeight",
                value =>
                {
                    model.NominalWeightValue =
                        value;

                    model.NominalWeight =
                        FormatDecimal(
                            value
                        );
                }
            );

            SetDecimalPair(
                row,
                "CrossSectionArea",
                value =>
                {
                    model.CrossSectionAreaValue =
                        value;

                    model.CrossSectionArea =
                        FormatDecimal(
                            value
                        );
                }
            );


            int? barsPerBundle =
                GetNullableInt(
                    row,
                    "NoOfBarsPerBundle"
                );

            model.NoOfBarsPerBundleValue =
                barsPerBundle;

            model.NoOfBarsPerBundle =
                barsPerBundle.HasValue
                    ? barsPerBundle.Value.ToString()
                    : "";


            int? noOfBundles =
                GetNullableInt(
                    row,
                    "NoOfBundles"
                );

            model.NoOfBundlesValue =
                noOfBundles;

            model.NoOfBundles =
                noOfBundles.HasValue
                    ? noOfBundles.Value.ToString()
                    : "";


            model.BendTestObserved =
                GetBool(
                    row,
                    "BendTestObserved"
                );

            model.IsWireRodOrCoil =
                GetBool(
                    row,
                    "IsWireRodOrCoil"
                );


            SetDecimalPair(
                row,
                "YieldStrength",
                value =>
                {
                    model.YieldStrengthValue =
                        value;

                    model.YieldStrength =
                        FormatDecimal(
                            value
                        );
                }
            );

            SetDecimalPair(
                row,
                "TensileStrength",
                value =>
                {
                    model.TensileStrengthValue =
                        value;

                    model.TensileStrength =
                        FormatDecimal(
                            value
                        );
                }
            );

            SetDecimalPair(
                row,
                "TensileYieldRatio",
                value =>
                {
                    model.TensileYieldRatioValue =
                        value;

                    model.TensileYieldRatio =
                        FormatDecimal(
                            value
                        );
                }
            );

            SetDecimalPair(
                row,
                "Elongation",
                value =>
                {
                    model.ElongationValue =
                        value;

                    model.Elongation =
                        FormatDecimal(
                            value
                        );
                }
            );

            SetDecimalPair(
                row,
                "GaugeLength",
                value =>
                {
                    model.GaugeLengthValue =
                        value;

                    model.GaugeLength =
                        FormatDecimal(
                            value
                        );
                }
            );


            /*
             * Chemistry is returned by sp_QC_GetMTCDetails
             * from RMChemicalAnalysis.
             */

            SetDecimalPair(
                row,
                "C",
                value =>
                {
                    model.CValue =
                        value;

                    model.C =
                        FormatDecimal(
                            value
                        );
                }
            );

            SetDecimalPair(
                row,
                "Si",
                value =>
                {
                    model.SiValue =
                        value;

                    model.Si =
                        FormatDecimal(
                            value
                        );
                }
            );

            SetDecimalPair(
                row,
                "Mn",
                value =>
                {
                    model.MnValue =
                        value;

                    model.Mn =
                        FormatDecimal(
                            value
                        );
                }
            );

            SetDecimalPair(
                row,
                "P",
                value =>
                {
                    model.PValue =
                        value;

                    model.P =
                        FormatDecimal(
                            value
                        );
                }
            );

            SetDecimalPair(
                row,
                "S",
                value =>
                {
                    model.SValue =
                        value;

                    model.S =
                        FormatDecimal(
                            value
                        );
                }
            );

            SetDecimalPair(
                row,
                "N",
                value =>
                {
                    model.NValue =
                        value;

                    model.N =
                        FormatDecimal(
                            value
                        );
                }
            );

            SetDecimalPair(
                row,
                "Ceq",
                value =>
                {
                    model.CeqValue =
                        value;

                    model.Ceq =
                        FormatDecimal(
                            value
                        );
                }
            );


            return model;
        }



        /* ================================================================
           SAFE HELPERS
           If same helpers already exist in QualityRepository, reuse yours
           and do not duplicate them.
           ================================================================ */

        private string GetString(
            DataRow row,
            string columnName)
        {
            if (
                row == null ||
                row.Table == null ||
                !row.Table.Columns.Contains(
                    columnName
                ) ||
                row[columnName] == DBNull.Value
            )
            {
                return "";
            }

            return
                Convert.ToString(
                    row[columnName]
                )
                .Trim();
        }


        private int GetInt(
            DataRow row,
            string columnName)
        {
            int? value =
                GetNullableInt(
                    row,
                    columnName
                );

            return
                value.HasValue
                    ? value.Value
                    : 0;
        }


        private int? GetNullableInt(
            DataRow row,
            string columnName)
        {
            if (
                row == null ||
                row.Table == null ||
                !row.Table.Columns.Contains(
                    columnName
                ) ||
                row[columnName] == DBNull.Value
            )
            {
                return null;
            }

            int result;

            return
                int.TryParse(
                    Convert.ToString(
                        row[columnName]
                    ),
                    out result
                )
                    ? (int?)result
                    : null;
        }


        private decimal GetDecimal(
            DataRow row,
            string columnName)
        {
            decimal? value =
                GetNullableDecimal(
                    row,
                    columnName
                );

            return
                value.HasValue
                    ? value.Value
                    : 0M;
        }


        private decimal? GetNullableDecimal(
            DataRow row,
            string columnName)
        {
            if (
                row == null ||
                row.Table == null ||
                !row.Table.Columns.Contains(
                    columnName
                ) ||
                row[columnName] == DBNull.Value
            )
            {
                return null;
            }

            decimal result;

            return
                decimal.TryParse(
                    Convert.ToString(
                        row[columnName]
                    ),
                    out result
                )
                    ? (decimal?)result
                    : null;
        }


        private DateTime? GetNullableDateTime(
            DataRow row,
            string columnName)
        {
            if (
                row == null ||
                row.Table == null ||
                !row.Table.Columns.Contains(
                    columnName
                ) ||
                row[columnName] == DBNull.Value
            )
            {
                return null;
            }

            DateTime result;

            return
                DateTime.TryParse(
                    Convert.ToString(
                        row[columnName]
                    ),
                    out result
                )
                    ? (DateTime?)result
                    : null;
        }


        private bool GetBool(
            DataRow row,
            string columnName)
        {
            if (
                row == null ||
                row.Table == null ||
                !row.Table.Columns.Contains(
                    columnName
                ) ||
                row[columnName] == DBNull.Value
            )
            {
                return false;
            }

            bool boolValue;

            if (
                bool.TryParse(
                    Convert.ToString(
                        row[columnName]
                    ),
                    out boolValue
                )
            )
            {
                return boolValue;
            }

            int intValue;

            return
                int.TryParse(
                    Convert.ToString(
                        row[columnName]
                    ),
                    out intValue
                )
                &&
                intValue == 1;
        }


        private object DbValue(
            string value)
        {
            return
                string.IsNullOrWhiteSpace(
                    value
                )
                    ? (object)DBNull.Value
                    : value.Trim();
        }


        private object DbValueDecimal(
            decimal? numericValue,
            string displayValue)
        {
            if (numericValue.HasValue)
            {
                return
                    numericValue.Value;
            }

            decimal parsed;

            if (
                decimal.TryParse(
                    displayValue,
                    out parsed
                )
            )
            {
                return parsed;
            }

            return DBNull.Value;
        }


        private object DbValueInt(
            int? numericValue,
            string displayValue)
        {
            if (numericValue.HasValue)
            {
                return
                    numericValue.Value;
            }

            int parsed;

            if (
                int.TryParse(
                    displayValue,
                    out parsed
                )
            )
            {
                return parsed;
            }

            return DBNull.Value;
        }


        private string FormatDecimal(
            decimal? value)
        {
            return
                value.HasValue
                    ? value.Value.ToString(
                        "0.####"
                    )
                    : "";
        }


        private void SetDecimalPair(
            DataRow row,
            string columnName,
            Action<decimal?> setter)
        {
            if (setter == null)
            {
                return;
            }

            setter(
                GetNullableDecimal(
                    row,
                    columnName
                )
            );
        }
    }
}