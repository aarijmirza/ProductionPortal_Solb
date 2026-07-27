using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
            IEnumerable<
                CCMDailyProductionReportDetailBLL
            > details)
        {
            List<
                CCMDailyProductionReportDetailBLL
            > rows =
                (
                    details ??
                    new List<
                        CCMDailyProductionReportDetailBLL
                    >()
                )
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.Grade
                    ) ||
                    x.TotalBillets > 0 ||
                    x.GoodBillets > 0 ||
                    !string.IsNullOrWhiteSpace(
                        x.Remarks
                    )
                )
                .ToList();

            XDocument xml =
                new XDocument(
                    new XElement(
                        "Details",

                        rows.Select(x =>
                            new XElement(
                                "Detail",

                                new XElement(
                                    "SequenceNo",
                                    x.SequenceNo
                                ),

                                new XElement(
                                    "Grade",
                                    x.Grade ??
                                    string.Empty
                                ),

                                new XElement(
                                    "Billet14M",
                                    x.Billet14M
                                ),

                                new XElement(
                                    "Billet13M",
                                    x.Billet13M
                                ),

                                new XElement(
                                    "Billet12M",
                                    x.Billet12M
                                ),

                                new XElement(
                                    "Billet11M",
                                    x.Billet11M
                                ),

                                new XElement(
                                    "Billet10M",
                                    x.Billet10M
                                ),

                                new XElement(
                                    "Billet09M",
                                    x.Billet09M
                                ),

                                new XElement(
                                    "Billet08M",
                                    x.Billet08M
                                ),

                                new XElement(
                                    "Billet07M",
                                    x.Billet07M
                                ),

                                new XElement(
                                    "Billet06M",
                                    x.Billet06M
                                ),

                                new XElement(
                                    "Billet05M",
                                    x.Billet05M
                                ),

                                new XElement(
                                    "Billet04M",
                                    x.Billet04M
                                ),

                                new XElement(
                                    "BilletBelow4M",
                                    x.BilletBelow4M
                                ),

                                new XElement(
                                    "CropEndStart",
                                    x.CropEndStart
                                ),

                                new XElement(
                                    "Bend",
                                    x.Bend
                                ),

                                new XElement(
                                    "GoodBillets",
                                    x.GoodBillets
                                ),

                                new XElement(
                                    "Remarks",
                                    x.Remarks ??
                                    string.Empty
                                )
                            )
                        )
                    )
                );

            return xml.ToString(
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