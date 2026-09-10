using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebAPICode.Helpers;

namespace BAL.Repositories
{
    public class FerroAlloyRepository
    {
        public List<QCFerroAlloyReportBLL> GetAll()
        {
            var list = new List<QCFerroAlloyReportBLL>();

            DataTable dt = new DBHelper().GetTableFromSP(
                "sp_QC_GetFerroAlloyList",
                new SqlParameter[0]
            );

            if (dt == null)
                return list;

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapMaster(row));
            }

            return list;
        }

        public QCFerroAlloyReportBLL GetByID(int id)
        {
            if (id <= 0)
                return null;

            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", SqlDbType.Int) { Value = id }
            };

            DataSet ds = new DBHelper().GetDatasetFromSP(
                "sp_QC_GetFerroAlloyByID",
                parameters
            );

            if (
                ds == null ||
                ds.Tables.Count == 0 ||
                ds.Tables[0] == null ||
                ds.Tables[0].Rows.Count == 0
            )
            {
                return null;
            }

            QCFerroAlloyReportBLL model =
                MapMaster(ds.Tables[0].Rows[0]);

            model.Samples =
                new List<QCFerroAlloySampleBLL>();

            if (
                ds.Tables.Count > 1 &&
                ds.Tables[1] != null
            )
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    model.Samples.Add(
                        MapSample(row)
                    );
                }
            }

            return model;
        }

        public int Insert(QCFerroAlloyReportBLL model)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@Supplier", SqlDbType.NVarChar, 200)
                {
                    Value = DbString(model.Supplier)
                },

                CreateNullableDecimalParameter("@Quantity", model.Quantity, 3),

                new SqlParameter("@ShipmentCodeNo", SqlDbType.NVarChar, 100)
                {
                    Value = DbString(model.ShipmentCodeNo)
                },

                new SqlParameter("@ReceivingDate", SqlDbType.Date)
                {
                    Value = model.ReceivingDate.HasValue
                        ? (object)model.ReceivingDate.Value.Date
                        : DBNull.Value
                },

                new SqlParameter("@AnalysisDate", SqlDbType.Date)
                {
                    Value = model.AnalysisDate.HasValue
                        ? (object)model.AnalysisDate.Value.Date
                        : DBNull.Value
                },

                new SqlParameter("@Material", SqlDbType.NVarChar, 50)
                {
                    Value = DbString(model.Material)
                },

                new SqlParameter("@ReferenceNo", SqlDbType.NVarChar, 100)
                {
                    Value = DbString(model.ReferenceNo)
                },

                new SqlParameter("@DeliveryPO", SqlDbType.NVarChar, 100)
                {
                    Value = DbString(model.DeliveryPO)
                },

                new SqlParameter("@LotSize", SqlDbType.NVarChar, 100)
                {
                    Value = DbString(model.LotSize)
                },

                new SqlParameter("@CertificateNo", SqlDbType.NVarChar, 100)
                {
                    Value = DbString(model.CertificateNo)
                },

                new SqlParameter("@FinalStatus", SqlDbType.NVarChar, 30)
                {
                    Value = DbString(model.FinalStatus)
                },

                new SqlParameter("@MRBNo", SqlDbType.NVarChar, 100)
                {
                    Value = DbString(model.MRBNo)
                },

                new SqlParameter("@Remarks", SqlDbType.NVarChar, -1)
                {
                    Value = DbString(model.Remarks)
                },

                new SqlParameter("@Comments", SqlDbType.NVarChar, -1)
                {
                    Value = DbString(model.Comments)
                },

                new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100)
                {
                    Value = DbString(model.CreatedBy)
                }
            };

            object result = new DBHelper().ExecuteScalar(
                "sp_QC_InsertFerroAlloyReport",
                parameters
            );

            return ToInt(result);
        }

        public int Update(QCFerroAlloyReportBLL model)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", SqlDbType.Int)
                {
                    Value = model.ID
                },

                new SqlParameter("@Supplier", SqlDbType.NVarChar, 200)
                {
                    Value = DbString(model.Supplier)
                },

                CreateNullableDecimalParameter("@Quantity", model.Quantity, 3),

                new SqlParameter("@ShipmentCodeNo", SqlDbType.NVarChar, 100)
                {
                    Value = DbString(model.ShipmentCodeNo)
                },

                new SqlParameter("@ReceivingDate", SqlDbType.Date)
                {
                    Value = model.ReceivingDate.HasValue
                        ? (object)model.ReceivingDate.Value.Date
                        : DBNull.Value
                },

                new SqlParameter("@AnalysisDate", SqlDbType.Date)
                {
                    Value = model.AnalysisDate.HasValue
                        ? (object)model.AnalysisDate.Value.Date
                        : DBNull.Value
                },

                new SqlParameter("@Material", SqlDbType.NVarChar, 50)
                {
                    Value = DbString(model.Material)
                },

                new SqlParameter("@ReferenceNo", SqlDbType.NVarChar, 100)
                {
                    Value = DbString(model.ReferenceNo)
                },

                new SqlParameter("@DeliveryPO", SqlDbType.NVarChar, 100)
                {
                    Value = DbString(model.DeliveryPO)
                },

                new SqlParameter("@LotSize", SqlDbType.NVarChar, 100)
                {
                    Value = DbString(model.LotSize)
                },

                new SqlParameter("@CertificateNo", SqlDbType.NVarChar, 100)
                {
                    Value = DbString(model.CertificateNo)
                },

                new SqlParameter("@FinalStatus", SqlDbType.NVarChar, 30)
                {
                    Value = DbString(model.FinalStatus)
                },

                new SqlParameter("@MRBNo", SqlDbType.NVarChar, 100)
                {
                    Value = DbString(model.MRBNo)
                },

                new SqlParameter("@Remarks", SqlDbType.NVarChar, -1)
                {
                    Value = DbString(model.Remarks)
                },

                new SqlParameter("@Comments", SqlDbType.NVarChar, -1)
                {
                    Value = DbString(model.Comments)
                },

                new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100)
                {
                    Value = DbString(model.UpdatedBy)
                }
            };

            object result = new DBHelper().ExecuteScalar(
                "sp_QC_UpdateFerroAlloyReport",
                parameters
            );

            return ToInt(result);
        }

        public int InsertSample(QCFerroAlloySampleBLL sample)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ReportID", SqlDbType.Int)
                {
                    Value = sample.ReportID
                },

                new SqlParameter("@SrNo", SqlDbType.Int)
                {
                    Value = sample.SrNo
                },

                new SqlParameter("@SampleNo", SqlDbType.NVarChar, 100)
                {
                    Value = DbString(sample.SampleNo)
                },

                CreateNullableDecimalParameter("@Si", sample.Si, 4),
                CreateNullableDecimalParameter("@Mn", sample.Mn, 4),
                CreateNullableDecimalParameter("@P", sample.P, 4),
                CreateNullableDecimalParameter("@S", sample.S, 4),
                CreateNullableDecimalParameter("@Al", sample.Al, 4),
                CreateNullableDecimalParameter("@Ca", sample.Ca, 4),
                CreateNullableDecimalParameter("@V", sample.V, 4),

                new SqlParameter("@Comment", SqlDbType.NVarChar, 1000)
                {
                    Value = DbString(sample.Comment)
                },

                new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100)
                {
                    Value = DbString(sample.CreatedBy)
                }
            };

            object result = new DBHelper().ExecuteScalar(
                "sp_QC_InsertFerroAlloySample",
                parameters
            );

            return ToInt(result);
        }

        public int DeleteSamplesByReportID(
            int reportID,
            string updatedBy)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ReportID", SqlDbType.Int)
                {
                    Value = reportID
                },

                new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100)
                {
                    Value = DbString(updatedBy)
                }
            };

            return new DBHelper().ExecuteNonQueryReturn(
                "sp_QC_DeleteFerroAlloySamplesByReportID",
                parameters
            );
        }

        public int Delete(
            int id,
            string updatedBy)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", SqlDbType.Int)
                {
                    Value = id
                },

                new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100)
                {
                    Value = DbString(updatedBy)
                }
            };

            object result = new DBHelper().ExecuteScalar(
                "sp_QC_DeleteFerroAlloyReport",
                parameters
            );

            return ToInt(result);
        }

        private static QCFerroAlloyReportBLL
            MapMaster(DataRow row)
        {
            return new QCFerroAlloyReportBLL
            {
                ID = GetInt(row, "ID"),
                Supplier = GetString(row, "Supplier"),
                Quantity = GetNullableDecimal(row, "Quantity"),
                ShipmentCodeNo = GetString(row, "ShipmentCodeNo"),
                ReceivingDate = GetNullableDate(row, "ReceivingDate"),
                AnalysisDate = GetNullableDate(row, "AnalysisDate"),
                Material = GetString(row, "Material"),
                ReferenceNo = GetString(row, "ReferenceNo"),
                DeliveryPO = GetString(row, "DeliveryPO"),
                LotSize = GetString(row, "LotSize"),
                CertificateNo = GetString(row, "CertificateNo"),
                FinalStatus = GetString(row, "FinalStatus"),
                MRBNo = GetString(row, "MRBNo"),
                Remarks = GetString(row, "Remarks"),
                Comments = GetString(row, "Comments"),
                StatusID = GetInt(row, "StatusID"),
                CreatedDate = GetNullableDate(row, "CreatedDate"),
                CreatedBy = GetString(row, "CreatedBy"),
                UpdatedDate = GetNullableDate(row, "UpdatedDate"),
                UpdatedBy = GetString(row, "UpdatedBy")
            };
        }

        private static QCFerroAlloySampleBLL
            MapSample(DataRow row)
        {
            return new QCFerroAlloySampleBLL
            {
                ID = GetInt(row, "ID"),
                ReportID = GetInt(row, "ReportID"),
                SrNo = GetInt(row, "SrNo"),
                SampleNo = GetString(row, "SampleNo"),
                Si = GetNullableDecimal(row, "Si"),
                Mn = GetNullableDecimal(row, "Mn"),
                P = GetNullableDecimal(row, "P"),
                S = GetNullableDecimal(row, "S"),
                Al = GetNullableDecimal(row, "Al"),
                Ca = GetNullableDecimal(row, "Ca"),
                V = GetNullableDecimal(row, "V"),
                Comment = GetString(row, "Comment"),
                StatusID = GetInt(row, "StatusID"),
                CreatedDate = GetNullableDate(row, "CreatedDate"),
                CreatedBy = GetString(row, "CreatedBy"),
                UpdatedDate = GetNullableDate(row, "UpdatedDate"),
                UpdatedBy = GetString(row, "UpdatedBy")
            };
        }

        private static SqlParameter
            CreateNullableDecimalParameter(
                string name,
                decimal? value,
                byte scale)
        {
            var p = new SqlParameter(
                name,
                SqlDbType.Decimal
            );

            p.Precision = 18;
            p.Scale = scale;
            p.Value = value.HasValue
                ? (object)value.Value
                : DBNull.Value;

            return p;
        }

        private static object DbString(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? (object)DBNull.Value
                : value.Trim();
        }

        private static int ToInt(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            int result;
            return int.TryParse(
                Convert.ToString(value),
                out result
            )
                ? result
                : 0;
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
                Convert.ToString(row[column]),
                out result
            )
                ? result
                : 0;
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
                return string.Empty;
            }

            return Convert.ToString(
                row[column]
            ).Trim();
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

            try
            {
                return Convert.ToDecimal(
                    row[column]
                );
            }
            catch
            {
                return null;
            }
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
                Convert.ToString(row[column]),
                out result
            )
                ? (DateTime?)result
                : null;
        }
    }
}
