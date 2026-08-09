using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using WebAPICode.Helpers;

namespace BAL.Repositories
{
    public class CCMDailyProductionRepository
    {
        //public int Save(
        //    CCMDailyProductionReportBLL model)
        //{
        //    if (model == null)
        //    {
        //        throw new ArgumentNullException(
        //            "model"
        //        );
        //    }

        //    if (string.IsNullOrEmpty(
        //        model.ReportNo))

        //    if (model.ReportDate ==
        //        DateTime.MinValue)
        //    {
        //        throw new ArgumentException(
        //            "Report date is required."
        //        );
        //    }

        //    if (string.IsNullOrWhiteSpace(
        //        model.Shift))
        //    {
        //        throw new ArgumentException(
        //            "Shift is required."
        //        );
        //    }

        //    if (model.Details == null)
        //    {
        //        model.Details =
        //            new List<
        //                CCMDailyProductionReportDetailBLL
        //            >();
        //    }

        //    model.Details =
        //        model.Details
        //            .Where(IsValidDetail)
        //            .ToList();

        //    if (model.Details.Count == 0)
        //    {
        //        throw new ArgumentException(
        //            "At least one billet production entry is required."
        //        );
        //    }

        //    for (int i = 0;
        //         i < model.Details.Count;
        //         i++)
        //    {
        //        model.Details[i].SequenceNo =
        //            i + 1;
        //    }

        //    string detailsXml =
        //        BuildDetailsXml(
        //            model.Details
        //        );

        //    SqlParameter[] parameters =
        //    {
        //        new SqlParameter(
        //            "@ID",
        //            SqlDbType.Int
        //        )
        //        {
        //            Value = model.ID
        //        },

        //        new SqlParameter(
        //            "@ReportNo",
        //            SqlDbType.NVarChar,
        //            50
        //        )
        //        {
        //            Value = model.ReportNo.Trim()
        //        },

        //        new SqlParameter(
        //            "@ReportDate",
        //            SqlDbType.Date
        //        )
        //        {
        //            Value = model.ReportDate.Date
        //        },

        //        new SqlParameter(
        //            "@Shift",
        //            SqlDbType.NVarChar,
        //            50
        //        )
        //        {
        //            Value = DbValue(model.Shift)
        //        },

        //        new SqlParameter(
        //            "@Team",
        //            SqlDbType.NVarChar,
        //            100
        //        )
        //        {
        //            Value = DbValue(model.Team)
        //        },

        //        new SqlParameter(
        //            "@CCMForeman",
        //            SqlDbType.NVarChar,
        //            150
        //        )
        //        {
        //            Value =
        //                DbValue(
        //                    model.CCMForeman
        //                )
        //        },

        //        new SqlParameter(
        //            "@BilletYardOperator",
        //            SqlDbType.NVarChar,
        //            150
        //        )
        //        {
        //            Value =
        //                DbValue(
        //                    model.BilletYardOperator
        //                )
        //        },

        //        new SqlParameter(
        //            "@CreatedBy",
        //            SqlDbType.NVarChar,
        //            100
        //        )
        //        {
        //            Value =
        //                DbValue(
        //                    model.CreatedBy
        //                )
        //        },

        //        new SqlParameter(
        //            "@DetailsXml",
        //            SqlDbType.Xml
        //        )
        //        {
        //            Value = detailsXml
        //        }
        //    };

        //    DataTable dt =
        //        DBHelper.ExecuteDataTable(
        //            "dbo.sp_SaveCCMDailyProductionReport",
        //            CommandType.StoredProcedure,
        //            parameters
        //        );

        //    if (dt == null ||
        //        dt.Rows.Count == 0 ||
        //        !dt.Columns.Contains("ID") ||
        //        dt.Rows[0]["ID"] ==
        //            DBNull.Value)
        //    {
        //        throw new DataException(
        //            "Saved report ID was not returned."
        //        );
        //    }

        //    return Convert.ToInt32(
        //        dt.Rows[0]["ID"]
        //    );
        //}

        public int Save(
         CCMDailyProductionReportBLL model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(
                    "model"
                );
            }

            string detailsXml =
                BuildDetailsXml(
                    model.Details
                );

            SqlParameter[] parameters =
            {
        new SqlParameter(
            "@ID",
            SqlDbType.Int
        )
        {
            Value = model.ID
        },

        new SqlParameter(
            "@ReportNo",
            SqlDbType.NVarChar,
            50
        )
        {
            Value =
                string.IsNullOrWhiteSpace(
                    model.ReportNo
                )
                    ? (object)DBNull.Value
                    : model.ReportNo.Trim()
        },

        new SqlParameter(
            "@ReportDate",
            SqlDbType.Date
        )
        {
            Value = model.ReportDate.Date
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
            "@Team",
            SqlDbType.NVarChar,
            100
        )
        {
            Value =
                string.IsNullOrWhiteSpace(
                    model.Team
                )
                    ? (object)DBNull.Value
                    : model.Team.Trim()
        },

        new SqlParameter(
            "@CCMForeman",
            SqlDbType.NVarChar,
            150
        )
        {
            Value =
                string.IsNullOrWhiteSpace(
                    model.CCMForeman
                )
                    ? (object)DBNull.Value
                    : model.CCMForeman.Trim()
        },

        new SqlParameter(
            "@BilletYardOperator",
            SqlDbType.NVarChar,
            150
        )
        {
            Value =
                string.IsNullOrWhiteSpace(
                    model.BilletYardOperator
                )
                    ? (object)DBNull.Value
                    : model.BilletYardOperator.Trim()
        },

        new SqlParameter(
            "@CreatedBy",
            SqlDbType.NVarChar,
            100
        )
        {
            Value =
                string.IsNullOrWhiteSpace(
                    model.CreatedBy
                )
                    ? (object)DBNull.Value
                    : model.CreatedBy.Trim()
        },

        new SqlParameter(
            "@DetailsXml",
            SqlDbType.Xml
        )
        {
            Value = detailsXml
        }
    };

            DataTable result =
                DBHelper.ExecuteDataTable(
                    "sp_SaveCCMDailyProductionReport",
                    CommandType.StoredProcedure,
                    parameters
                );

            if (
                result == null ||
                result.Rows.Count == 0 ||
                !result.Columns.Contains("ID") ||
                result.Rows[0]["ID"] == DBNull.Value
            )
            {
                throw new Exception(
                    "Stored procedure did not return the saved report ID."
                );
            }

            return Convert.ToInt32(
                result.Rows[0]["ID"]
            );
        }
        private int InsertReport(
            CCMDailyProductionReportBLL model)
        {
            SqlParameter outputID =
                new SqlParameter(
                    "@ID",
                    SqlDbType.Int
                );

            outputID.Direction =
                ParameterDirection.Output;

            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@ReportNo",
                    SqlDbType.NVarChar,
                    50
                )
                {
                    Value = DbValue(
                        model.ReportNo
                    )
                },

                new SqlParameter(
                    "@ReportDate",
                    SqlDbType.Date
                )
                {
                    Value = model.ReportDate
                },

                new SqlParameter(
                    "@Shift",
                    SqlDbType.NVarChar,
                    50
                )
                {
                    Value = DbValue(
                        model.Shift
                    )
                },

                new SqlParameter(
                    "@Team",
                    SqlDbType.NVarChar,
                    100
                )
                {
                    Value = DbValue(
                        model.Team
                    )
                },

                new SqlParameter(
                    "@CCMForeman",
                    SqlDbType.NVarChar,
                    150
                )
                {
                    Value = DbValue(
                        model.CCMForeman
                    )
                },

                new SqlParameter(
                    "@BilletYardOperator",
                    SqlDbType.NVarChar,
                    150
                )
                {
                    Value = DbValue(
                        model.BilletYardOperator
                    )
                },

                new SqlParameter(
                    "@HeatNo",
                    SqlDbType.NVarChar,
                    -1
                )
                {
                    Value = DbValue(
                        model.HeatNo
                    )
                },

                new SqlParameter(
                    "@TotalBillets",
                    SqlDbType.Int
                )
                {
                    Value = model.TotalBillets
                },

                new SqlParameter(
                    "@PrimeBillets",
                    SqlDbType.Int
                )
                {
                    Value = model.PrimeBillets
                },

                new SqlParameter(
                    "@ShortBillets",
                    SqlDbType.Int
                )
                {
                    Value = model.ShortBillets
                },

                new SqlParameter(
                    "@StatusID",
                    SqlDbType.Int
                )
                {
                    Value = model.StatusID
                },

                new SqlParameter(
                    "@CreatedBy",
                    SqlDbType.NVarChar,
                    100
                )
                {
                    Value = DbValue(
                        model.CreatedBy
                    )
                },

                new SqlParameter(
                    "@CreatedDate",
                    SqlDbType.DateTime
                )
                {
                    Value =
                        model.CreatedDate
                        ?? DateTime.Now
                },

                outputID
            };

            DBHelper.ExecuteNonQuery(
                "sp_SaveCCMDailyProductionReport",
                CommandType.StoredProcedure,
                parameters
            );

            if (
                outputID.Value == null ||
                outputID.Value == DBNull.Value
            )
            {
                return 0;
            }

            return Convert.ToInt32(
                outputID.Value
            );
        }

        private void InsertDetail(
            CCMDailyProductionReportDetailBLL detail)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@ReportID",
                    SqlDbType.Int
                )
                {
                    Value = detail.ReportID
                },

                new SqlParameter(
                    "@SequenceNo",
                    SqlDbType.Int
                )
                {
                    Value = detail.SequenceNo
                },

                new SqlParameter(
                    "@HeatNo",
                    SqlDbType.NVarChar,
                    50
                )
                {
                    Value = DbValue(
                        detail.HeatNo
                    )
                },

                new SqlParameter(
                    "@Grade",
                    SqlDbType.NVarChar,
                    100
                )
                {
                    Value = DbValue(
                        detail.Grade
                    )
                },

                new SqlParameter(
                    "@Billet14M",
                    SqlDbType.Int
                )
                {
                    Value = detail.Billet14M
                },

                new SqlParameter(
                    "@Billet13M",
                    SqlDbType.Int
                )
                {
                    Value = detail.Billet13M
                },

                new SqlParameter(
                    "@Billet12M",
                    SqlDbType.Int
                )
                {
                    Value = detail.Billet12M
                },

                new SqlParameter(
                    "@Billet11M",
                    SqlDbType.Int
                )
                {
                    Value = detail.Billet11M
                },

                new SqlParameter(
                    "@GoodBillets",
                    SqlDbType.Int
                )
                {
                    Value = detail.GoodBillets
                },

                new SqlParameter(
                    "@ShortBillets",
                    SqlDbType.Int
                )
                {
                    Value = detail.ShortBillets
                },

                new SqlParameter(
                    "@Bend",
                    SqlDbType.Int
                )
                {
                    Value = detail.Bend
                },

                new SqlParameter(
                    "@TotalBillets",
                    SqlDbType.Int
                )
                {
                    Value = detail.TotalBillets
                },

                DecimalParameter(
                    "@TotalLength",
                    detail.TotalLength
                ),

                DecimalParameter(
                    "@ShortBilletTotalLength",
                    detail.ShortBilletTotalLength
                ),

                DecimalParameter(
                    "@ShortBilletAvgLength",
                    detail.ShortBilletAvgLength
                ),

                DecimalParameter(
                    "@PerCoilBundleWeight",
                    detail.PerCoilBundleWeight
                ),

                DecimalParameter(
                    "@PrimeBilletWeight",
                    detail.PrimeBilletWeight
                ),

                DecimalParameter(
                    "@ShortBilletWeight",
                    detail.ShortBilletWeight
                ),

                DecimalParameter(
                    "@TotalWeight",
                    detail.TotalWeight
                ),

                new SqlParameter(
                    "@Remarks",
                    SqlDbType.NVarChar,
                    -1
                )
                {
                    Value = DbValue(
                        detail.Remarks
                    )
                },

                new SqlParameter(
                    "@StatusID",
                    SqlDbType.Int
                )
                {
                    Value = detail.StatusID
                },

                new SqlParameter(
                    "@CreatedBy",
                    SqlDbType.NVarChar,
                    100
                )
                {
                    Value = DbValue(
                        detail.CreatedBy
                    )
                },

                new SqlParameter(
                    "@CreatedDate",
                    SqlDbType.DateTime
                )
                {
                    Value =
                        detail.CreatedDate
                        ?? DateTime.Now
                }
            };

            DBHelper.ExecuteNonQuery(
                "sp_InsertCCMDailyProductionReportDetail",
                CommandType.StoredProcedure,
                parameters
            );
        }

        private static SqlParameter DecimalParameter(
            string parameterName,
            decimal? value)
        {
            SqlParameter parameter =
                new SqlParameter(
                    parameterName,
                    SqlDbType.Decimal
                );

            parameter.Precision = 18;
            parameter.Scale = 3;

            parameter.Value =
                value.HasValue
                    ? (object)value.Value
                    : DBNull.Value;

            return parameter;
        }

        private static object DbValue(
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

        public CCMDailyProductionReportBLL
                    GetByID(int id)
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

            DataSet ds =
                DBHelper.ExecuteDataSet(
                    "dbo.sp_GetCCMDailyProductionReportByID",
                    CommandType.StoredProcedure,
                    parameters
                );

            if (ds == null ||
                ds.Tables.Count == 0 ||
                ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            CCMDailyProductionReportBLL model =
                MapHeader(
                    ds.Tables[0].Rows[0]
                );

            model.Details =
                new List<
                    CCMDailyProductionReportDetailBLL
                >();

            if (ds.Tables.Count > 1)
            {
                foreach (
                    DataRow row
                    in ds.Tables[1].Rows)
                {
                    model.Details.Add(
                        MapDetail(row)
                    );
                }
            }

            return model;
        }

        public List<
            CCMDailyProductionReportBLL
        > GetAll(
            DateTime? fromDate,
            DateTime? toDate,
            string shift)
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
                            ? (object)
                                fromDate.Value.Date
                            : DBNull.Value
                },

                new SqlParameter(
                    "@ToDate",
                    SqlDbType.Date
                )
                {
                    Value =
                        toDate.HasValue
                            ? (object)
                                toDate.Value.Date
                            : DBNull.Value
                },

                new SqlParameter(
                    "@Shift",
                    SqlDbType.NVarChar,
                    50
                )
                {
                    Value = DbValue(shift)
                }
            };

            DataTable dt =
                DBHelper.ExecuteDataTable(
                    "dbo.sp_GetCCMDailyProductionReports",
                    CommandType.StoredProcedure,
                    parameters
                );

            var list =
                new List<
                    CCMDailyProductionReportBLL
                >();

            if (dt == null)
            {
                return list;
            }

            foreach (DataRow row in dt.Rows)
            {
                list.Add(
                    MapHeader(row)
                );
            }

            return list;
        }

        public bool Delete(
            int id,
            string updatedBy)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@ID",
                    SqlDbType.Int
                )
                {
                    Value = id
                },

                new SqlParameter(
                    "@UpdatedBy",
                    SqlDbType.NVarChar,
                    100
                )
                {
                    Value =
                        DbValue(updatedBy)
                }
            };

            DataTable dt =
                DBHelper.ExecuteDataTable(
                    "dbo.sp_DeleteCCMDailyProductionReport",
                    CommandType.StoredProcedure,
                    parameters
                );

            return
                dt != null &&
                dt.Rows.Count > 0 &&
                dt.Columns.Contains(
                    "AffectedRows"
                ) &&
                dt.Rows[0]["AffectedRows"] !=
                    DBNull.Value &&
                Convert.ToInt32(
                    dt.Rows[0]["AffectedRows"]
                ) > 0;
        }

        private string BuildDetailsXml(
    List<CCMDailyProductionReportDetailBLL> details)
        {
            XElement root =
                new XElement(
                    "Details"
                );

            if (details == null)
            {
                return root.ToString(
                    SaveOptions.DisableFormatting
                );
            }

            foreach (
                CCMDailyProductionReportDetailBLL detail
                in details
            )
            {
                if (detail == null)
                {
                    continue;
                }

                root.Add(
                    new XElement(
                        "Detail",

                        new XElement(
                            "ID",
                            detail.ID
                        ),

                        new XElement(
                            "SequenceNo",
                            detail.SequenceNo
                        ),

                        new XElement(
                            "HeatNo",
                            detail.HeatNo ?? ""
                        ),

                        new XElement(
                            "Grade",
                            detail.Grade ?? ""
                        ),

                        new XElement(
                            "Billet14M",
                            detail.Billet14M
                        ),

                        new XElement(
                            "Billet13M",
                            detail.Billet13M
                        ),

                        new XElement(
                            "Billet12M",
                            detail.Billet12M
                        ),

                        new XElement(
                            "Billet11M",
                            detail.Billet11M
                        ),

                        new XElement(
                            "GoodBillets",
                            detail.GoodBillets
                        ),

                        new XElement(
                            "ShortBillets",
                            detail.ShortBillets
                        ),

                        new XElement(
                            "Bend",
                            detail.Bend
                        ),

                        new XElement(
                            "TotalBillets",
                            detail.TotalBillets
                        ),

                        new XElement(
                            "TotalLength",
                            detail.TotalLength ?? 0M
                        ),

                        new XElement(
                            "ShortBilletTotalLength",
                            detail.ShortBilletTotalLength
                            ?? 0M
                        ),

                        new XElement(
                            "ShortBilletAvgLength",
                            detail.ShortBilletAvgLength
                            ?? 0M
                        ),

                        /*
                         * SP element name: PerUnitWeight
                         * BLL property: PerCoilBundleWeight
                         */
                        new XElement(
                            "PerUnitWeight",
                            detail.PerCoilBundleWeight
                            ?? 0M
                        ),

                        new XElement(
                            "PrimeBilletWeight",
                            detail.PrimeBilletWeight
                            ?? 0M
                        ),

                        new XElement(
                            "ShortBilletWeight",
                            detail.ShortBilletWeight
                            ?? 0M
                        ),

                        new XElement(
                            "TotalWeight",
                            detail.TotalWeight
                            ?? 0M
                        ),

                        new XElement(
                            "Remarks",
                            detail.Remarks ?? ""
                        )
                    )
                );
            }

            return root.ToString(
                SaveOptions.DisableFormatting
            );
        }

        private bool IsValidDetail(
            CCMDailyProductionReportDetailBLL item)
        {
            if (item == null)
            {
                return false;
            }

            return
                !string.IsNullOrWhiteSpace(
                    item.HeatNo
                ) ||
                !string.IsNullOrWhiteSpace(
                    item.Grade
                ) ||
                item.Billet14M > 0 ||
                item.Billet13M > 0 ||
                item.Billet12M > 0 ||
                item.Billet11M > 0 ||
                item.GoodBillets > 0 ||
                item.ShortBillets > 0 ||
                item.Bend > 0 ||
                item.TotalBillets > 0 ||
                (item.TotalLength ?? 0M) > 0 ||
                (item.ShortBilletTotalLength ?? 0M) > 0 ||
                (item.ShortBilletAvgLength ?? 0M) > 0 ||
                (item.PerCoilBundleWeight ?? 0M) > 0 ||
                (item.PrimeBilletWeight ?? 0M) > 0 ||
                (item.ShortBilletWeight ?? 0M) > 0 ||
                (item.TotalWeight ?? 0M) > 0 ||
                !string.IsNullOrWhiteSpace(
                    item.Remarks
                );
        }

        private string DecimalText(
            decimal? value)
        {
            return
                (value ?? 0M).ToString(
                    CultureInfo.InvariantCulture
                );
        }

        private CCMDailyProductionReportBLL
            MapHeader(DataRow row)
        {
            return new CCMDailyProductionReportBLL
            {
                ID =
                    GetInt(row, "ID"),

                ReportNo =
                    GetString(
                        row,
                        "ReportNo"
                    ),

                ReportDate =
                    GetDate(
                        row,
                        "ReportDate"
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

                CCMForeman =
                    GetString(
                        row,
                        "CCMForeman"
                    ),

                BilletYardOperator =
                    GetString(
                        row,
                        "BilletYardOperator"
                    ),

                TotalBillets =
                    GetInt(
                        row,
                        "TotalBillets"
                    ),

                PrimeBillets =
                    GetInt(
                        row,
                        "PrimeBillets"
                    ),

                ShortBillets =
                    GetInt(
                        row,
                        "ShortBillets"
                    ),

                StatusID =
                    GetInt(
                        row,
                        "StatusID"
                    ),

                CreatedBy =
                    GetString(
                        row,
                        "CreatedBy"
                    ),

                CreatedDate =
                    GetNullableDate(
                        row,
                        "CreatedDate"
                    ),

                UpdatedBy =
                    GetString(
                        row,
                        "UpdatedBy"
                    ),

                UpdatedDate =
                    GetNullableDate(
                        row,
                        "UpdatedDate"
                    )
            };
        }

        private CCMDailyProductionReportDetailBLL
            MapDetail(DataRow row)
        {
            return
                new CCMDailyProductionReportDetailBLL
                {
                    ID =
                        GetInt(row, "ID"),

                    ReportID =
                        GetInt(
                            row,
                            "ReportID"
                        ),

                    SequenceNo =
                        GetInt(
                            row,
                            "SequenceNo"
                        ),

                    HeatNo =
                        GetString(
                            row,
                            "HeatNo"
                        ),

                    Grade =
                        GetString(
                            row,
                            "Grade"
                        ),

                    Billet14M =
                        GetInt(
                            row,
                            "Billet14M"
                        ),

                    Billet13M =
                        GetInt(
                            row,
                            "Billet13M"
                        ),

                    Billet12M =
                        GetInt(
                            row,
                            "Billet12M"
                        ),

                    Billet11M =
                        GetInt(
                            row,
                            "Billet11M"
                        ),

                    GoodBillets =
                        GetInt(
                            row,
                            "GoodBillets"
                        ),

                    ShortBillets =
                        GetInt(
                            row,
                            "ShortBillets"
                        ),

                    Bend =
                        GetInt(
                            row,
                            "Bend"
                        ),

                    TotalBillets =
                        GetInt(
                            row,
                            "TotalBillets"
                        ),

                    TotalLength =
                        GetNullableDecimal(
                            row,
                            "TotalLength"
                        ),

                    ShortBilletTotalLength =
                        GetNullableDecimal(
                            row,
                            "ShortBilletTotalLength"
                        ),

                    ShortBilletAvgLength =
                        GetNullableDecimal(
                            row,
                            "ShortBilletAvgLength"
                        ),

                    PerCoilBundleWeight =
                        GetNullableDecimal(
                            row,
                            "PerUnitWeight"
                        ),

                    PrimeBilletWeight =
                        GetNullableDecimal(
                            row,
                            "PrimeBilletWeight"
                        ),

                    ShortBilletWeight =
                        GetNullableDecimal(
                            row,
                            "ShortBilletWeight"
                        ),

                    TotalWeight =
                        GetNullableDecimal(
                            row,
                            "TotalWeight"
                        ),

                    Remarks =
                        GetString(
                            row,
                            "Remarks"
                        ),

                    StatusID =
                        GetInt(
                            row,
                            "StatusID"
                        ),

                    CreatedBy =
                        GetString(
                            row,
                            "CreatedBy"
                        ),

                    CreatedDate =
                        GetNullableDate(
                            row,
                            "CreatedDate"
                        ),

                    UpdatedBy =
                        GetString(
                            row,
                            "UpdatedBy"
                        ),

                    UpdatedDate =
                        GetNullableDate(
                            row,
                            "UpdatedDate"
                        )
                };
        }

        private object DbValue(
            string value)
        {
            return string.IsNullOrWhiteSpace(
                value)
                    ? (object)DBNull.Value
                    : value.Trim();
        }

        private int GetInt(
            DataRow row,
            string columnName)
        {
            return
                row.Table.Columns.Contains(
                    columnName
                ) &&
                row[columnName] != DBNull.Value
                    ? Convert.ToInt32(
                        row[columnName]
                    )
                    : 0;
        }

        private string GetString(
            DataRow row,
            string columnName)
        {
            return
                row.Table.Columns.Contains(
                    columnName
                ) &&
                row[columnName] != DBNull.Value
                    ? Convert.ToString(
                        row[columnName]
                    ).Trim()
                    : string.Empty;
        }

        private DateTime GetDate(
            DataRow row,
            string columnName)
        {
            return
                row.Table.Columns.Contains(
                    columnName
                ) &&
                row[columnName] != DBNull.Value
                    ? Convert.ToDateTime(
                        row[columnName]
                    )
                    : DateTime.MinValue;
        }

        private DateTime? GetNullableDate(
            DataRow row,
            string columnName)
        {
            return
                row.Table.Columns.Contains(
                    columnName
                ) &&
                row[columnName] != DBNull.Value
                    ? Convert.ToDateTime(
                        row[columnName]
                    )
                    : (DateTime?)null;
        }

        private decimal? GetNullableDecimal(
            DataRow row,
            string columnName)
        {
            return
                row.Table.Columns.Contains(
                    columnName
                ) &&
                row[columnName] != DBNull.Value
                    ? Convert.ToDecimal(
                        row[columnName],
                        CultureInfo.InvariantCulture
                    )
                    : (decimal?)null;
        }
    }
}