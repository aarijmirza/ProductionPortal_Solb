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
        public int Save(
            CCMDailyProductionReportBLL model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(
                    "model"
                );
            }

            if (string.IsNullOrWhiteSpace(
                model.ReportNo))
            {
                throw new ArgumentException(
                    "Report number is required."
                );
            }

            if (model.ReportDate ==
                DateTime.MinValue)
            {
                throw new ArgumentException(
                    "Report date is required."
                );
            }

            if (string.IsNullOrWhiteSpace(
                model.Shift))
            {
                throw new ArgumentException(
                    "Shift is required."
                );
            }

            if (model.Details == null)
            {
                model.Details =
                    new List<
                        CCMDailyProductionReportDetailBLL
                    >();
            }

            model.Details =
                model.Details
                    .Where(IsValidDetail)
                    .ToList();

            if (model.Details.Count == 0)
            {
                throw new ArgumentException(
                    "At least one billet production entry is required."
                );
            }

            for (int i = 0;
                 i < model.Details.Count;
                 i++)
            {
                model.Details[i].SequenceNo =
                    i + 1;
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
                    Value = model.ReportNo.Trim()
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
                    Value = DbValue(model.Shift)
                },

                new SqlParameter(
                    "@Team",
                    SqlDbType.NVarChar,
                    100
                )
                {
                    Value = DbValue(model.Team)
                },

                new SqlParameter(
                    "@CCMForeman",
                    SqlDbType.NVarChar,
                    150
                )
                {
                    Value =
                        DbValue(
                            model.CCMForeman
                        )
                },

                new SqlParameter(
                    "@BilletYardOperator",
                    SqlDbType.NVarChar,
                    150
                )
                {
                    Value =
                        DbValue(
                            model.BilletYardOperator
                        )
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
                },

                new SqlParameter(
                    "@DetailsXml",
                    SqlDbType.Xml
                )
                {
                    Value = detailsXml
                }
            };

            DataTable dt =
                DBHelper.ExecuteDataTable(
                    "dbo.sp_SaveCCMDailyProductionReport",
                    CommandType.StoredProcedure,
                    parameters
                );

            if (dt == null ||
                dt.Rows.Count == 0 ||
                !dt.Columns.Contains("ID") ||
                dt.Rows[0]["ID"] ==
                    DBNull.Value)
            {
                throw new DataException(
                    "Saved report ID was not returned."
                );
            }

            return Convert.ToInt32(
                dt.Rows[0]["ID"]
            );
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
            List<
                CCMDailyProductionReportDetailBLL
            > details)
        {
            XElement root =
                new XElement("Details");

            foreach (
                CCMDailyProductionReportDetailBLL item
                in details)
            {
                root.Add(
                    new XElement(
                        "Detail",

                        new XElement(
                            "ID",
                            item.ID
                        ),

                        new XElement(
                            "SequenceNo",
                            item.SequenceNo
                        ),

                        new XElement(
                            "HeatNo",
                            item.HeatNo ?? ""
                        ),

                        new XElement(
                            "Grade",
                            item.Grade ?? ""
                        ),

                        new XElement(
                            "Billet14M",
                            item.Billet14M
                        ),

                        new XElement(
                            "Billet13M",
                            item.Billet13M
                        ),

                        new XElement(
                            "Billet12M",
                            item.Billet12M
                        ),

                        new XElement(
                            "Billet11M",
                            item.Billet11M
                        ),

                        new XElement(
                            "GoodBillets",
                            item.GoodBillets
                        ),

                        new XElement(
                            "ShortBillets",
                            item.ShortBillets
                        ),

                        new XElement(
                            "Bend",
                            item.Bend
                        ),

                        new XElement(
                            "TotalBillets",
                            item.TotalBillets
                        ),

                        new XElement(
                            "TotalLength",
                            DecimalText(
                                item.TotalLength
                            )
                        ),

                        new XElement(
                            "ShortBilletTotalLength",
                            DecimalText(
                                item.ShortBilletTotalLength
                            )
                        ),

                        new XElement(
                            "ShortBilletAvgLength",
                            DecimalText(
                                item.ShortBilletAvgLength
                            )
                        ),

                        new XElement(
                            "PerUnitWeight",
                            DecimalText(
                                item.PerCoilBundleWeight
                            )
                        ),

                        new XElement(
                            "PrimeBilletWeight",
                            DecimalText(
                                item.PrimeBilletWeight
                            )
                        ),

                        new XElement(
                            "ShortBilletWeight",
                            DecimalText(
                                item.ShortBilletWeight
                            )
                        ),

                        new XElement(
                            "TotalWeight",
                            DecimalText(
                                item.TotalWeight
                            )
                        ),

                        new XElement(
                            "Remarks",
                            item.Remarks ?? ""
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