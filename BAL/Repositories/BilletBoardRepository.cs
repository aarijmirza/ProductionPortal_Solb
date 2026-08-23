using DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public sealed class BilletBoardSaveResult
    {
        public int SavedID { get; set; }
        public int SavedRows { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Separate repository used only by BilletBoardController.
    /// All inserts/updates/deletes are transactional inside SQL SPs.
    /// </summary>
    public class BilletBoardRepository
    {
        public List<BilletGrades> GetBilletGrades()
        {
            SqlParameter[] parameters = new SqlParameter[0];

            DataTable table =
                new DBHelper().GetTableFromSP(
                    "sp_GetBilletGrades_PP",
                    parameters
                );

            return ToList<BilletGrades>(table);
        }

        public BilletBoardBLL GetForEdit(int id)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", SqlDbType.Int)
                {
                    Value = id
                }
            };

            DataTable table =
                new DBHelper().GetTableFromSP(
                    "sp_QC_GetBilletForEdit",
                    parameters
                );

            return ToList<BilletBoardBLL>(table).FirstOrDefault();
        }

        public List<RMChemicalAnalysisBLL> GetChemistryForEdit(int id)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", SqlDbType.Int)
                {
                    Value = id
                }
            };

            DataTable table =
                new DBHelper().GetTableFromSP(
                    "sp_QC_GetBilletChemistryForEdit",
                    parameters
                );

            return ToList<RMChemicalAnalysisBLL>(table);
        }

        public List<string> GetDuplicateHeatNos(
            IList<string> heatNos,
            IList<int> excludedChemistryIDs,
            int currentID)
        {
            string heatNoCsv =
                string.Join(
                    ",",
                    (heatNos ?? new List<string>())
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                );

            string excludedIDCsv =
                string.Join(
                    ",",
                    (excludedChemistryIDs ?? new List<int>())
                        .Where(x => x > 0)
                        .Distinct()
                );

            SqlParameter[] parameters =
            {
                NVarCharMax("@HeatNos", heatNoCsv),
                NVarCharMax("@ExcludedChemistryIDs", excludedIDCsv),
                new SqlParameter("@CurrentID", SqlDbType.Int)
                {
                    Value = currentID
                }
            };

            DataTable table =
                new DBHelper().GetTableFromSP(
                    "sp_QC_CheckBilletHeatDuplicates",
                    parameters
                );

            if (table == null || table.Rows.Count == 0)
            {
                return new List<string>();
            }

            return table
                .AsEnumerable()
                .Select(x => Convert.ToString(x["HeatNo"]))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public BilletBoardSaveResult Save(
            BilletBoardBLL model,
            string userName)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            string chemistryXml =
                BuildChemistryXml(model.Chemistry);

            SqlParameter billetWeight =
                new SqlParameter(
                    "@BilletWeight",
                    SqlDbType.Decimal
                )
                {
                    Precision = 18,
                    Scale = 5,
                    Value = model.BilletWeight
                };

            SqlParameter[] parameters =
            {
                new SqlParameter("@CurrentID", SqlDbType.Int)
                {
                    Value = model.ID
                },
                new SqlParameter("@Date", SqlDbType.Date)
                {
                    Value = model.Date.HasValue
                        ? (object)model.Date.Value.Date
                        : DBNull.Value
                },
                NVarChar("@BilletBoarding", model.BilletBoarding, 100),
                NVarChar("@PlantName", model.PlantName, 100),
                NVarChar("@Shift", model.Shift, 50),
                NVarChar("@ProductSpecs", model.ProductSpecs, 200),
                NVarChar("@BilletLength", model.BilletLength, 50),
                NVarChar("@CrossSection", model.CrossSection, 100),
                NVarChar("@Grade", model.SteelGrade, 100),
                NVarChar("@Size", model.Size, 100),
                NVarChar("@Profile", model.Profile, 100),
                billetWeight,
                NVarCharMax("@Remarks", model.Remarks),
                new SqlParameter("@Chemistry", SqlDbType.Xml)
                {
                    Value = chemistryXml
                },
                NVarChar("@UserName", userName, 100)
            };

            DataTable table =
                new DBHelper().GetTableFromSP(
                    "sp_QC_SaveBilletBoard",
                    parameters
                );

            if (table == null || table.Rows.Count == 0)
            {
                throw new InvalidOperationException(
                    "Billet Board save procedure returned no result."
                );
            }

            DataRow row = table.Rows[0];

            return new BilletBoardSaveResult
            {
                SavedID = GetInt(row, "SavedID"),
                SavedRows = GetInt(row, "SavedRows"),
                Message = GetString(row, "Message")
            };
        }

        public int Delete(int id, string userName)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", SqlDbType.Int)
                {
                    Value = id
                },
                NVarChar("@UpdatedBy", userName, 100)
            };

            DataTable table =
                new DBHelper().GetTableFromSP(
                    "sp_QC_DeleteBilletBoard",
                    parameters
                );

            if (table == null || table.Rows.Count == 0)
            {
                throw new InvalidOperationException(
                    "Billet Board delete procedure returned no result."
                );
            }

            return GetInt(table.Rows[0], "AffectedRows");
        }

        private static string BuildChemistryXml(
            IEnumerable<RMChemicalAnalysisBLL> chemistry)
        {
            IEnumerable<RMChemicalAnalysisBLL> rows =
                chemistry ?? new List<RMChemicalAnalysisBLL>();

            XElement root =
                new XElement(
                    "Chemistry",
                    rows.Select(
                        (row, index) =>
                            new XElement(
                                "Row",
                                new XAttribute("ID", row.ID),
                                new XAttribute("SrNo", index + 1),
                                new XAttribute(
                                    "HeatNo",
                                    (row.HeatNo ?? string.Empty).Trim()
                                ),
                                new XAttribute(
                                    "NoOfBillets",
                                    ToInvariant(row.NoOfBillets)
                                ),
                                new XAttribute("C", ToInvariant(row.C)),
                                new XAttribute("Si", ToInvariant(row.Si)),
                                new XAttribute("Mn", ToInvariant(row.Mn)),
                                new XAttribute("S", ToInvariant(row.S)),
                                new XAttribute("P", ToInvariant(row.P)),
                                new XAttribute("N", ToInvariant(row.N)),
                                new XAttribute("Ceq", ToInvariant(row.Ceq)),
                                new XAttribute(
                                    "HeatStatus",
                                    ToInvariant(row.HeatStatus)
                                )
                            )
                    )
                );

            return root.ToString(SaveOptions.DisableFormatting);
        }

        private static string ToInvariant(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return Convert.ToString(
                value,
                CultureInfo.InvariantCulture
            );
        }

        private static SqlParameter NVarChar(
            string name,
            string value,
            int size)
        {
            return new SqlParameter(name, SqlDbType.NVarChar, size)
            {
                Value = string.IsNullOrWhiteSpace(value)
                    ? (object)DBNull.Value
                    : value.Trim()
            };
        }

        private static SqlParameter NVarCharMax(
            string name,
            string value)
        {
            return new SqlParameter(name, SqlDbType.NVarChar, -1)
            {
                Value = string.IsNullOrWhiteSpace(value)
                    ? (object)DBNull.Value
                    : value
            };
        }

        private static List<T> ToList<T>(DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
            {
                return new List<T>();
            }

            return JArray
                .Parse(JsonConvert.SerializeObject(table))
                .ToObject<List<T>>()
                ?? new List<T>();
        }

        private static int GetInt(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName) &&
                   row[columnName] != DBNull.Value
                ? Convert.ToInt32(row[columnName])
                : 0;
        }

        private static string GetString(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName)
                ? Convert.ToString(row[columnName])
                : string.Empty;
        }
    }
}
