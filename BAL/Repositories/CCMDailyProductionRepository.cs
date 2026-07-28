using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            string detailsXml =
                BuildDetailsXml(
                    model.Details
                );

            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@ID",
                    model.ID
                ),

                new SqlParameter(
                    "@ReportNo",
                    model.ReportNo ??
                    string.Empty
                ),

                new SqlParameter(
                    "@ReportDate",
                    model.ReportDate.Date
                ),

                new SqlParameter(
                    "@Shift",
                    string.IsNullOrWhiteSpace(
                        model.Shift
                    )
                        ? (object)DBNull.Value
                        : model.Shift
                ),

                new SqlParameter(
                    "@Team",
                    string.IsNullOrWhiteSpace(
                        model.Team
                    )
                        ? (object)DBNull.Value
                        : model.Team
                ),

                new SqlParameter(
                    "@CCMForeman",
                    string.IsNullOrWhiteSpace(
                        model.CCMForeman
                    )
                        ? (object)DBNull.Value
                        : model.CCMForeman
                ),

                new SqlParameter(
                    "@BilletYardOperator",
                    string.IsNullOrWhiteSpace(
                        model.BilletYardOperator
                    )
                        ? (object)DBNull.Value
                        : model.BilletYardOperator
                ),

                new SqlParameter(
                    "@CreatedBy",
                    string.IsNullOrWhiteSpace(
                        model.CreatedBy
                    )
                        ? (object)DBNull.Value
                        : model.CreatedBy
                ),

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
                    "sp_SaveCCMDailyProductionReport",
                    CommandType.StoredProcedure,
                    parameters
                );

            if (dt != null &&
                dt.Rows.Count > 0)
            {
                return Convert.ToInt32(
                    dt.Rows[0]["ID"]
                );
            }

            return 0;
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
                    fromDate.HasValue
                        ? (object)
                            fromDate.Value.Date
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@ToDate",
                    toDate.HasValue
                        ? (object)
                            toDate.Value.Date
                        : DBNull.Value
                ),

                new SqlParameter(
                    "@Shift",
                    string.IsNullOrWhiteSpace(
                        shift
                    )
                        ? (object)DBNull.Value
                        : shift
                )
            };

            DataTable dt =
                DBHelper.ExecuteDataTable(
                    "sp_GetCCMDailyProductionReports",
                    CommandType.StoredProcedure,
                    parameters
                );

            List<
                CCMDailyProductionReportBLL
            > list =
                new List<
                    CCMDailyProductionReportBLL
                >();

            if (dt == null ||
                dt.Rows.Count == 0)
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

        public CCMDailyProductionReportBLL
            GetByID(int id)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@ID",
                    id
                )
            };

            DataSet ds =
                DBHelper.ExecuteDataSet(
                    "sp_GetCCMDailyProductionReportByID",
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

        public bool Delete(
            int id,
            string updatedBy)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@ID",
                    id
                ),

                new SqlParameter(
                    "@UpdatedBy",
                    string.IsNullOrWhiteSpace(
                        updatedBy
                    )
                        ? (object)DBNull.Value
                        : updatedBy
                )
            };

            DataTable dt =
                DBHelper.ExecuteDataTable(
                    "sp_DeleteCCMDailyProductionReport",
                    CommandType.StoredProcedure,
                    parameters
                );

            if (dt != null &&
                dt.Rows.Count > 0)
            {
                int affectedRows =
                    Convert.ToInt32(
                        dt.Rows[0][
                            "AffectedRows"
                        ]
                    );

                return affectedRows > 0;
            }

            return false;
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

            foreach (CCMDailyProductionReportDetailBLL item
                in details)
            {
                XElement row =
                    new XElement(
                        "Detail",

                        new XElement(
                            "ID",
                            item.ID
                        ),

                        new XElement(
                            "ReportID",
                            item.ReportID
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
                            "Billet10M",
                            item.Billet10M
                        ),

                        new XElement(
                            "Billet09M",
                            item.Billet09M
                        ),

                        new XElement(
                            "Billet08M",
                            item.Billet08M
                        ),

                        new XElement(
                            "Billet07M",
                            item.Billet07M
                        ),

                        new XElement(
                            "Billet06M",
                            item.Billet06M
                        ),

                        new XElement(
                            "Billet05M",
                            item.Billet05M
                        ),

                        new XElement(
                            "Billet04M",
                            item.Billet04M
                        ),

                        new XElement(
                            "BilletBelow4M",
                            item.BilletBelow4M
                        ),

                        new XElement(
                            "CropEndStart",
                            item.CropEndStart
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
                            "GoodBillets",
                            item.GoodBillets
                        ),

                        new XElement(
                            "PrimeBilletWeight",
                            (item.PrimeBilletWeight ?? 0M)
                             .ToString(
                                 CultureInfo.InvariantCulture
                             )
                        ),

                        new XElement(
                            "ShortBilletWeight",
                            (item.ShortBilletWeight ?? 0M)
                            .ToString(
                                CultureInfo.InvariantCulture
                            )
                        ),

                        new XElement(
                            "TotalWeight",
                            (item.TotalWeight ?? 0M)
                            .ToString(
                                CultureInfo.InvariantCulture
                            )
                        ),

                        new XElement(
                            "PerCoilBundleWeight",
                            (item.PerCoilBundleWeight ?? 0M)
                            .ToString(
                                CultureInfo.InvariantCulture
                            )
                        ),

                        new XElement(
                            "Remarks",
                            item.Remarks ?? ""
                        )
                    );

                root.Add(row);
            }

            return root.ToString(
                SaveOptions.DisableFormatting
            );
        }

        private CCMDailyProductionReportBLL
            MapHeader(DataRow row)
        {
            return new CCMDailyProductionReportBLL
            {
                ID =
                    GetInt(
                        row,
                        "ID"
                    ),

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
                        GetInt(
                            row,
                            "ID"
                        ),

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

                    Billet10M =
                        GetInt(
                            row,
                            "Billet10M"
                        ),

                    Billet09M =
                        GetInt(
                            row,
                            "Billet09M"
                        ),

                    Billet08M =
                        GetInt(
                            row,
                            "Billet08M"
                        ),

                    Billet07M =
                        GetInt(
                            row,
                            "Billet07M"
                        ),

                    Billet06M =
                        GetInt(
                            row,
                            "Billet06M"
                        ),

                    Billet05M =
                        GetInt(
                            row,
                            "Billet05M"
                        ),

                    Billet04M =
                        GetInt(
                            row,
                            "Billet04M"
                        ),

                    BilletBelow4M =
                        GetInt(
                            row,
                            "BilletBelow4M"
                        ),

                    CropEndStart =
                        GetInt(
                            row,
                            "CropEndStart"
                        ),

                    Bend =
                        GetInt(
                            row,
                            "Bend"
                        ),

                    GoodBillets =
                        GetInt(
                            row,
                            "GoodBillets"
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
                        )
                };
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
                    )
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
    }
}