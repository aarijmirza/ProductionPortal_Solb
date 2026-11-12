
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace BAL.Services
{
    public class RMMTCExcelBuilder
    {
        private const string MainNs =
            "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

        public byte[] Build(
            string templatePath,
            string gradeLimitsPath,
            RMMTCHeaderBLL header,
            List<RMMTCDetailBLL> details,
            string generatedBy)
        {
            if (header == null)
                throw new Exception("MTC header was not supplied.");

            if (details == null || details.Count == 0)
                throw new Exception("At least one heat is required.");

            if (details.Count > 10)
                throw new Exception("Maximum 10 Heat IDs are allowed for every MTC.");

            if (!File.Exists(templatePath))
                throw new FileNotFoundException("MTC template was not found.", templatePath);

            if (!File.Exists(gradeLimitsPath))
                throw new FileNotFoundException("Grades Limits.xlsx was not found.", gradeLimitsPath);

            byte[] templateBytes =
                File.ReadAllBytes(templatePath);

            using (MemoryStream sourceStream = new MemoryStream(templateBytes, false))
            using (ZipArchive sourceArchive = new ZipArchive(sourceStream, ZipArchiveMode.Read, false))
            {
                string sheetPath =
                    GetWorksheetPath(sourceArchive, null);

                XDocument sheetXml =
                    LoadZipXml(sourceArchive, sheetPath);

                List<string> templateSharedStrings =
                    ReadSharedStrings(sourceArchive);

                RMMTCDetailBLL first =
                    details[0];

                Dictionary<string, string> limits =
                    GetGradeLimits(
                        gradeLimitsPath,
                        first.Specification,
                        first.SteelGrade,
                        first.ProductType,
                        first.BarSize
                    );

                SetText(sheetXml, "B7", "Specification");
                SetText(sheetXml, "E7", "Grade");

                SetText(
                    sheetXml,
                    "B8",
                    FormatSpecificationForDisplay(
                        first.Specification,
                        first.SteelGrade
                    )
                );

                SetText(
                    sheetXml,
                    "E8",
                    FormatGradeForDisplay(
                        first.SteelGrade
                    )
                );

                string customerBlock =
                    (header.CustomerName ?? "").Trim();

                if (!string.IsNullOrWhiteSpace(header.ShippingAddress))
                {
                    if (!string.IsNullOrWhiteSpace(customerBlock))
                        customerBlock += "\n";

                    customerBlock +=
                        header.ShippingAddress.Trim();
                }

                SetText(sheetXml, "F8", customerBlock);

                SetText(sheetXml, "N5", header.DNNo ?? "");
                SetText(sheetXml, "N6", header.MTCNo ?? "");
                SetText(sheetXml, "N7", header.CustomerNo ?? "");
                SetText(sheetXml, "N8", header.SalesOrderNo ?? "");
                SetText(sheetXml, "N9", header.CustomerOrderNo ?? "");

                SetText(
                    sheetXml,
                    "N10",
                    header.IssueDate.ToString(
                        "dd-MMM-yyyy",
                        CultureInfo.InvariantCulture
                    )
                );

                PrepareLimitRowLayout(sheetXml);
                EnsureLimitCells(sheetXml);
                WriteLimits(sheetXml, limits);

                for (int i = 0; i < 10; i++)
                {
                    int rowNo =
                        16 + i;

                    if (i >= details.Count)
                    {
                        ClearDataRow(
                            sheetXml,
                            rowNo
                        );

                        continue;
                    }

                    WriteHeatRow(
                        sheetXml,
                        rowNo,
                        details[i]
                    );
                }

                SetText(
                    sheetXml,
                    "B49",
                    "MTC generated using : " +
                    (generatedBy ?? "") +
                    "  " +
                    DateTime.Now.ToString(
                        "dd/MM/yyyy hh:mm:ss tt",
                        CultureInfo.InvariantCulture
                    )
                );

                ApplyWorksheetProtection(
                    sheetXml,
                    "SOLB-MTC-LOCK"
                );

                using (MemoryStream outputStream = new MemoryStream())
                {
                    using (ZipArchive outputArchive = new ZipArchive(outputStream, ZipArchiveMode.Create, true))
                    {
                        foreach (ZipArchiveEntry sourceEntry in sourceArchive.Entries)
                        {
                            if (
                                string.Equals(
                                    sourceEntry.FullName,
                                    sheetPath,
                                    StringComparison.OrdinalIgnoreCase
                                )
                            )
                            {
                                continue;
                            }

                            ZipArchiveEntry outputEntry =
                                outputArchive.CreateEntry(
                                    sourceEntry.FullName,
                                    CompressionLevel.Optimal
                                );

                            using (Stream input = sourceEntry.Open())
                            using (Stream output = outputEntry.Open())
                            {
                                input.CopyTo(output);
                            }
                        }

                        ZipArchiveEntry worksheetEntry =
                            outputArchive.CreateEntry(
                                sheetPath,
                                CompressionLevel.Optimal
                            );

                        using (Stream worksheetStream = worksheetEntry.Open())
                        {
                            sheetXml.Save(
                                worksheetStream,
                                SaveOptions.DisableFormatting
                            );
                        }
                    }

                    byte[] result =
                        outputStream.ToArray();

                    ValidateXlsxBytes(
                        result
                    );

                    return result;
                }
            }
        }


        private void WriteHeatRow(
            XDocument sheetXml,
            int rowNo,
            RMMTCDetailBLL item)
        {
            string row =
                rowNo.ToString(
                    CultureInfo.InvariantCulture
                );

            SetNumber(sheetXml, "B" + row, item.BarSize);
            SetNumber(sheetXml, "C" + row, item.NominalWeight);
            SetText(sheetXml, "D" + row, item.ProductType ?? "");
            SetText(sheetXml, "E" + row, item.HeatNo ?? "");
            SetNumber(sheetXml, "F" + row, item.YieldStrength);
            SetNumber(sheetXml, "G" + row, item.TensileStrength);

            decimal? ratio =
                item.TensileYieldRatio;

            if (
                item.YieldStrength.HasValue &&
                item.TensileStrength.HasValue &&
                item.YieldStrength.Value > 0
            )
            {
                ratio =
                    decimal.Round(
                        item.TensileStrength.Value /
                        item.YieldStrength.Value,
                        3,
                        MidpointRounding.AwayFromZero
                    );
            }

            SetNumber(sheetXml, "H" + row, ratio);
            SetNumber(sheetXml, "I" + row, item.Elongation);

            SetText(
                sheetXml,
                "J" + row,
                item.BendTestObserved
                    ? "Satisfactory"
                    : ""
            );

            SetNumber(sheetXml, "K" + row, item.C);
            SetNumber(sheetXml, "L" + row, item.Si);
            SetNumber(sheetXml, "M" + row, item.Mn);
            SetNumber(sheetXml, "N" + row, item.P);
            SetNumber(sheetXml, "O" + row, item.S);
            SetNumber(sheetXml, "P" + row, item.Cu);
            SetNumber(sheetXml, "Q" + row, item.V);
            SetNumber(sheetXml, "R" + row, item.B);
            SetNumber(sheetXml, "S" + row, item.N);
            SetNumber(sheetXml, "T" + row, item.Ceq);
        }


        private void ClearDataRow(
            XDocument sheetXml,
            int rowNo)
        {
            string row =
                rowNo.ToString(
                    CultureInfo.InvariantCulture
                );

            string[] columns =
            {
                "B","C","D","E","F","G","H","I","J",
                "K","L","M","N","O","P","Q","R","S","T"
            };

            foreach (string column in columns)
            {
                SetText(
                    sheetXml,
                    column + row,
                    ""
                );
            }
        }


        private Dictionary<string, string> GetGradeLimits(
            string gradeLimitsPath,
            string specification,
            string steelGrade,
            string productType,
            decimal? barSize)
        {
            Dictionary<string, string> result =
                EmptyLimits();

            using (FileStream stream =
                new FileStream(
                    gradeLimitsPath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite
                ))
            using (ZipArchive archive =
                new ZipArchive(
                    stream,
                    ZipArchiveMode.Read,
                    false
                ))
            {
                string sheetPath =
                    GetWorksheetPath(
                        archive,
                        "Grades Limits"
                    );

                XDocument sheetXml =
                    LoadZipXml(
                        archive,
                        sheetPath
                    );

                List<string> shared =
                    ReadSharedStrings(
                        archive
                    );

                XNamespace ns =
                    MainNs;

                List<XElement> rows =
                    sheetXml
                        .Descendants(ns + "row")
                        .Where(
                            x =>
                                IntAttr(
                                    x,
                                    "r"
                                ) >= 4
                        )
                        .ToList();

                XElement matched =
                    rows.FirstOrDefault(
                        row =>
                            SpecMatches(
                                CellText(row, "A", shared, ns),
                                specification,
                                steelGrade
                            )
                            &&
                            GradeMatches(
                                CellText(row, "B", shared, ns),
                                steelGrade
                            )
                            &&
                            ProductMatches(
                                CellText(row, "C", shared, ns),
                                productType
                            )
                            &&
                            SizeMatches(
                                CellText(row, "D", shared, ns),
                                barSize
                            )
                    );

                if (matched == null)
                {
                    /*
                     * Safe standard-family fallback:
                     * A615/A706/BS use Rebar limit rows.
                     * A510 uses Wire Rod - Coil.
                     */
                    string standard =
                        StandardCode(
                            specification + " " + steelGrade
                        );

                    string fallbackProduct =
                        standard == "A510"
                            ? "WIRERODCOIL"
                            : (
                                standard == "A615"
                                || standard == "A706"
                                || standard == "BS4449"
                                    ? "REBAR"
                                    : NormalizeKey(productType)
                              );

                    matched =
                        rows.FirstOrDefault(
                            row =>
                                SpecMatches(
                                    CellText(row, "A", shared, ns),
                                    specification,
                                    steelGrade
                                )
                                &&
                                GradeMatches(
                                    CellText(row, "B", shared, ns),
                                    steelGrade
                                )
                                &&
                                NormalizeProduct(
                                    CellText(row, "C", shared, ns)
                                )
                                ==
                                fallbackProduct
                                &&
                                SizeMatches(
                                    CellText(row, "D", shared, ns),
                                    barSize
                                )
                        );
                }

                if (matched == null)
                    return result;

                result["Yield"] = CleanLimit(CellText(matched, "E", shared, ns));
                result["Tensile"] = CleanLimit(CellText(matched, "F", shared, ns));
                result["Ratio"] = CleanLimit(CellText(matched, "G", shared, ns));
                result["Elongation"] = CleanLimit(CellText(matched, "H", shared, ns));
                result["C"] = CleanLimit(CellText(matched, "I", shared, ns));
                result["Si"] = CleanLimit(CellText(matched, "J", shared, ns));
                result["Mn"] = CleanLimit(CellText(matched, "K", shared, ns));
                result["P"] = CleanLimit(CellText(matched, "L", shared, ns));
                result["S"] = CleanLimit(CellText(matched, "M", shared, ns));
                result["Cu"] = CleanLimit(CellText(matched, "N", shared, ns));
                result["V"] = CleanLimit(CellText(matched, "R", shared, ns));
                result["B"] = CleanLimit(CellText(matched, "S", shared, ns));
                result["N"] = CleanLimit(CellText(matched, "T", shared, ns));
                result["Ceq"] = CleanLimit(CellText(matched, "U", shared, ns));
            }

            return result;
        }


        private Dictionary<string, string> EmptyLimits()
        {
            return new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase
            )
            {
                { "Yield","--" },
                { "Tensile","--" },
                { "Ratio","--" },
                { "Elongation","--" },
                { "C","--" },
                { "Si","--" },
                { "Mn","--" },
                { "P","--" },
                { "S","--" },
                { "Cu","--" },
                { "V","--" },
                { "B","--" },
                { "N","--" },
                { "Ceq","--" }
            };
        }


        private void WriteLimits(
            XDocument sheetXml,
            Dictionary<string, string> limits)
        {
            SetText(sheetXml, "F15", L(limits, "Yield"));
            SetText(sheetXml, "G15", L(limits, "Tensile"));
            SetText(sheetXml, "H15", L(limits, "Ratio"));
            SetText(sheetXml, "I15", L(limits, "Elongation"));

            SetText(sheetXml, "K15", L(limits, "C"));
            SetText(sheetXml, "L15", L(limits, "Si"));
            SetText(sheetXml, "M15", L(limits, "Mn"));
            SetText(sheetXml, "N15", L(limits, "P"));
            SetText(sheetXml, "O15", L(limits, "S"));
            SetText(sheetXml, "P15", L(limits, "Cu"));
            SetText(sheetXml, "Q15", L(limits, "V"));
            SetText(sheetXml, "R15", L(limits, "B"));
            SetText(sheetXml, "S15", L(limits, "N"));
            SetText(sheetXml, "T15", L(limits, "Ceq"));
        }


        private string L(
            Dictionary<string, string> limits,
            string key)
        {
            string value;

            return
                limits != null
                && limits.TryGetValue(key, out value)
                && !string.IsNullOrWhiteSpace(value)
                    ? value
                    : "--";
        }


        private string FormatSpecificationForDisplay(
            string specification,
            string grade)
        {
            string standard =
                StandardCode(
                    (specification ?? "") +
                    " " +
                    (grade ?? "")
                );

            if (standard == "A510")
                return "ASTM A510";

            if (standard == "A615")
                return "ASTM A615";

            if (standard == "A706")
                return "ASTM A706";

            if (standard == "BS4449")
                return "BS 4449";

            return
                (specification ?? "").Trim();
        }


        private string FormatGradeForDisplay(
            string grade)
        {
            string number =
                GradeNumber(
                    grade
                );

            string standard =
                StandardCode(
                    grade
                );

            if (
                standard == "A615"
                || standard == "A706"
            )
            {
                return
                    string.IsNullOrWhiteSpace(number)
                        ? (grade ?? "")
                        : "Grade " + number;
            }

            if (
                Regex.IsMatch(
                    grade ?? "",
                    @"(?:SAE|AISI)\s*\d{3,4}",
                    RegexOptions.IgnoreCase
                )
            )
            {
                return
                    "AISI " + number;
            }

            return
                (grade ?? "").Trim();
        }


        private bool SpecMatches(
            string rowSpec,
            string specification,
            string grade)
        {
            string row =
                StandardCode(
                    rowSpec
                );

            string input =
                StandardCode(
                    (specification ?? "") +
                    " " +
                    (grade ?? "")
                );

            return
                !string.IsNullOrWhiteSpace(row)
                &&
                row == input;
        }


        private bool GradeMatches(
            string rowGrade,
            string inputGrade)
        {
            string a =
                GradeNumber(
                    rowGrade
                );

            string b =
                GradeNumber(
                    inputGrade
                );

            return
                !string.IsNullOrWhiteSpace(a)
                &&
                a == b;
        }


        private string GradeNumber(
            string value)
        {
            string upper =
                (value ?? "")
                    .ToUpperInvariant();

            Match m =
                Regex.Match(
                    upper,
                    @"(?:GRADE|GR|G)\s*0*([0-9]{2,3})\b"
                );

            if (m.Success)
                return m.Groups[1].Value.TrimStart('0');

            m =
                Regex.Match(
                    upper,
                    @"(?:AISI|SAE)\s*0*([0-9]{3,4})\b"
                );

            if (m.Success)
                return m.Groups[1].Value.TrimStart('0');

            m =
                Regex.Match(
                    upper,
                    @"\bB\s*0*([0-9]{3})\s*B\b"
                );

            if (m.Success)
                return m.Groups[1].Value.TrimStart('0');

            List<string> nums =
                Regex.Matches(
                    upper,
                    @"\d+"
                )
                .Cast<Match>()
                .Select(
                    x =>
                        x.Value.TrimStart('0')
                )
                .Where(
                    x =>
                        !string.IsNullOrWhiteSpace(x)
                )
                .ToList();

            return
                nums.Count == 0
                    ? ""
                    : nums[nums.Count - 1];
        }


        private string StandardCode(
            string value)
        {
            string compact =
                Regex.Replace(
                    (value ?? "").ToUpperInvariant(),
                    @"[^A-Z0-9]",
                    ""
                );

            if (compact.Contains("A510"))
                return "A510";

            if (compact.Contains("A615"))
                return "A615";

            if (compact.Contains("A706"))
                return "A706";

            if (compact.Contains("BS4449"))
                return "BS4449";

            return "";
        }


        private bool ProductMatches(
            string rowProduct,
            string productType)
        {
            return
                NormalizeProduct(rowProduct)
                ==
                NormalizeProduct(productType);
        }


        private string NormalizeProduct(
            string value)
        {
            string key =
                NormalizeKey(value);

            if (
                key == "WIREROD"
                || key == "WIRERODCOIL"
                || key == "WR"
            )
                return "WIRERODCOIL";

            if (
                key == "REBAR"
                || key == "RB"
                || key == "REBARCOIL"
                || key == "REBARINCOIL"
                || key == "RIC"
            )
                return "REBAR";

            if (
                key == "PLAINBAR"
                || key == "PLAINROD"
                || key == "PB"
            )
                return "PLAINBAR";

            return key;
        }


        private string NormalizeKey(
            string value)
        {
            return
                Regex.Replace(
                    (value ?? "").ToUpperInvariant(),
                    @"[^A-Z0-9]",
                    ""
                );
        }


        private bool SizeMatches(
            string sizeRule,
            decimal? barSize)
        {
            string rule =
                (sizeRule ?? "")
                    .Trim()
                    .ToUpperInvariant();

            if (
                string.IsNullOrWhiteSpace(rule)
                || rule == "--"
                || rule == "-"
            )
                return true;

            if (!barSize.HasValue)
                return false;

            decimal size =
                barSize.Value;

            string compact =
                rule
                    .Replace("SIZE", "")
                    .Replace("MM", "")
                    .Replace(" ", "")
                    .Replace("≤", "<=")
                    .Replace("≥", ">=")
                    .Replace("=<", "<=")
                    .Replace("=>", ">=");

            if (compact.StartsWith("="))
            {
                List<decimal> list =
                    Regex.Matches(
                        compact.Substring(1),
                        @"\d+(?:\.\d+)?"
                    )
                    .Cast<Match>()
                    .Select(
                        x =>
                        {
                            decimal parsed;

                            return
                                decimal.TryParse(
                                    x.Value,
                                    NumberStyles.Any,
                                    CultureInfo.InvariantCulture,
                                    out parsed
                                )
                                    ? (decimal?)parsed
                                    : null;
                        }
                    )
                    .Where(x => x.HasValue)
                    .Select(x => x.Value)
                    .ToList();

                return list.Any(x => x == size);
            }

            Match m =
                Regex.Match(
                    compact,
                    @"<=([0-9]+(?:\.[0-9]+)?)"
                );

            if (m.Success)
                return size <= DecimalValue(m.Groups[1].Value);

            m =
                Regex.Match(
                    compact,
                    @">=([0-9]+(?:\.[0-9]+)?)"
                );

            if (m.Success)
                return size >= DecimalValue(m.Groups[1].Value);

            m =
                Regex.Match(
                    compact,
                    @">([0-9]+(?:\.[0-9]+)?)"
                );

            if (m.Success)
                return size > DecimalValue(m.Groups[1].Value);

            m =
                Regex.Match(
                    compact,
                    @"<([0-9]+(?:\.[0-9]+)?)"
                );

            if (m.Success)
                return size < DecimalValue(m.Groups[1].Value);

            return false;
        }


        private decimal DecimalValue(
            string value)
        {
            decimal parsed;

            decimal.TryParse(
                value,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out parsed
            );

            return parsed;
        }


        private string CleanLimit(
            string value)
        {
            string text =
                (value ?? "").Trim();

            if (
                string.IsNullOrWhiteSpace(text)
                || text == "-"
                || text == "--"
            )
                return "--";

            return text;
        }


        private void PrepareLimitRowLayout(
            XDocument sheetXml)
        {
            XNamespace ns =
                MainNs;

            XElement mergeCells =
                sheetXml.Root.Element(
                    ns + "mergeCells"
                );

            if (mergeCells == null)
                return;

            string[] removeRefs =
            {
                "H14:H15",
                "I14:I15",
                "K14:K15",
                "L14:L15",
                "M14:M15",
                "N14:N15",
                "O14:O15",
                "P14:P15",
                "Q14:Q15",
                "R14:R15",
                "S14:S15",
                "T14:W15"
            };

            List<XElement> removeList =
                mergeCells
                    .Elements(ns + "mergeCell")
                    .Where(
                        x =>
                            removeRefs.Contains(
                                (string)x.Attribute("ref"),
                                StringComparer.OrdinalIgnoreCase
                            )
                    )
                    .ToList();

            foreach (XElement item in removeList)
                item.Remove();

            mergeCells.Add(
                new XElement(
                    ns + "mergeCell",
                    new XAttribute(
                        "ref",
                        "T14:W14"
                    )
                )
            );

            mergeCells.Add(
                new XElement(
                    ns + "mergeCell",
                    new XAttribute(
                        "ref",
                        "T15:W15"
                    )
                )
            );

            XAttribute count =
                mergeCells.Attribute(
                    "count"
                );

            if (count != null)
            {
                count.Value =
                    mergeCells
                        .Elements(ns + "mergeCell")
                        .Count()
                        .ToString(
                            CultureInfo.InvariantCulture
                        );
            }
        }


        private void EnsureLimitCells(
            XDocument sheetXml)
        {
            string[] cells =
            {
                "F15","G15","H15","I15",
                "K15","L15","M15","N15","O15",
                "P15","Q15","R15","S15","T15"
            };

            foreach (string cell in cells)
            {
                EnsureCell(
                    sheetXml,
                    cell
                );
            }
        }


        private void ApplyWorksheetProtection(
            XDocument sheetXml,
            string password)
        {
            XNamespace ns =
                MainNs;

            XElement existing =
                sheetXml.Root.Element(
                    ns + "sheetProtection"
                );

            if (existing != null)
                existing.Remove();

            XElement protection =
                new XElement(
                    ns + "sheetProtection",
                    new XAttribute("password", ExcelPasswordHash(password)),
                    new XAttribute("sheet", "1"),
                    new XAttribute("objects", "1"),
                    new XAttribute("scenarios", "1"),
                    new XAttribute("formatCells", "1"),
                    new XAttribute("formatColumns", "1"),
                    new XAttribute("formatRows", "1"),
                    new XAttribute("insertColumns", "1"),
                    new XAttribute("insertRows", "1"),
                    new XAttribute("deleteColumns", "1"),
                    new XAttribute("deleteRows", "1")
                );

            XElement sheetData =
                sheetXml.Root.Element(
                    ns + "sheetData"
                );

            if (sheetData != null)
                sheetData.AddAfterSelf(protection);
            else
                sheetXml.Root.Add(protection);
        }


        private string ExcelPasswordHash(
            string password)
        {
            string value =
                password ?? "";

            ushort hash =
                0;

            for (int i = value.Length - 1; i >= 0; i--)
            {
                hash =
                    (ushort)(
                        (((hash >> 14) & 0x0001))
                        |
                        ((hash << 1) & 0x7FFF)
                    );

                hash =
                    (ushort)(
                        hash ^ value[i]
                    );
            }

            hash =
                (ushort)(
                    hash
                    ^ value.Length
                    ^ 0xCE4B
                );

            return
                hash.ToString(
                    "X4",
                    CultureInfo.InvariantCulture
                );
        }


        private void ValidateXlsxBytes(
            byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes, false))
            using (ZipArchive archive = new ZipArchive(ms, ZipArchiveMode.Read, false))
            {
                if (
                    archive.GetEntry("[Content_Types].xml") == null
                    || archive.GetEntry("xl/workbook.xml") == null
                )
                {
                    throw new Exception("Generated MTC XLSX package is invalid.");
                }

                LoadZipXml(
                    archive,
                    "xl/workbook.xml"
                );
            }
        }


        private string GetWorksheetPath(
            ZipArchive archive,
            string requestedSheetName)
        {
            XDocument workbook =
                LoadZipXml(
                    archive,
                    "xl/workbook.xml"
                );

            XNamespace ns =
                MainNs;

            XNamespace relNs =
                "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

            XElement sheet =
                workbook
                    .Descendants(ns + "sheet")
                    .FirstOrDefault(
                        x =>
                            string.IsNullOrWhiteSpace(requestedSheetName)
                            ||
                            string.Equals(
                                (string)x.Attribute("name"),
                                requestedSheetName,
                                StringComparison.OrdinalIgnoreCase
                            )
                    );

            if (sheet == null)
                throw new Exception("Worksheet was not found.");

            string relationshipId =
                (string)sheet.Attribute(
                    relNs + "id"
                );

            XDocument rels =
                LoadZipXml(
                    archive,
                    "xl/_rels/workbook.xml.rels"
                );

            XNamespace packageRelNs =
                "http://schemas.openxmlformats.org/package/2006/relationships";

            XElement relationship =
                rels
                    .Descendants(packageRelNs + "Relationship")
                    .FirstOrDefault(
                        x =>
                            string.Equals(
                                (string)x.Attribute("Id"),
                                relationshipId,
                                StringComparison.OrdinalIgnoreCase
                            )
                    );

            if (relationship == null)
                throw new Exception("Worksheet relationship was not found.");

            string target =
                ((string)relationship.Attribute("Target") ?? "")
                    .Replace("\\", "/")
                    .TrimStart('/');

            if (!target.StartsWith("xl/", StringComparison.OrdinalIgnoreCase))
                target = "xl/" + target;

            return target;
        }


        private XDocument LoadZipXml(
            ZipArchive archive,
            string path)
        {
            ZipArchiveEntry entry =
                archive.GetEntry(path);

            if (entry == null)
                throw new Exception("XLSX entry was not found: " + path);

            using (Stream stream = entry.Open())
            {
                return
                    XDocument.Load(
                        stream,
                        LoadOptions.PreserveWhitespace
                    );
            }
        }


        private List<string> ReadSharedStrings(
            ZipArchive archive)
        {
            List<string> list =
                new List<string>();

            ZipArchiveEntry entry =
                archive.GetEntry(
                    "xl/sharedStrings.xml"
                );

            if (entry == null)
                return list;

            using (Stream stream = entry.Open())
            {
                XDocument doc =
                    XDocument.Load(stream);

                XNamespace ns =
                    MainNs;

                foreach (
                    XElement item
                    in doc.Descendants(ns + "si")
                )
                {
                    string value =
                        string.Concat(
                            item
                                .Descendants(ns + "t")
                                .Select(
                                    x =>
                                        (string)x
                                )
                        );

                    list.Add(value);
                }
            }

            return list;
        }


        private string CellText(
            XElement row,
            string column,
            List<string> sharedStrings,
            XNamespace ns)
        {
            string rowNo =
                (string)row.Attribute("r");

            string cellRef =
                column + rowNo;

            XElement cell =
                row
                    .Elements(ns + "c")
                    .FirstOrDefault(
                        x =>
                            string.Equals(
                                (string)x.Attribute("r"),
                                cellRef,
                                StringComparison.OrdinalIgnoreCase
                            )
                    );

            if (cell == null)
                return "";

            string type =
                (string)cell.Attribute("t");

            XElement value =
                cell.Element(ns + "v");

            if (
                string.Equals(
                    type,
                    "s",
                    StringComparison.OrdinalIgnoreCase
                )
                &&
                value != null
            )
            {
                int index;

                if (
                    int.TryParse(
                        value.Value,
                        out index
                    )
                    &&
                    index >= 0
                    &&
                    index < sharedStrings.Count
                )
                {
                    return
                        sharedStrings[index];
                }
            }

            XElement inline =
                cell.Element(ns + "is");

            if (inline != null)
            {
                return
                    string.Concat(
                        inline
                            .Descendants(ns + "t")
                            .Select(
                                x =>
                                    (string)x
                            )
                    );
            }

            return
                value == null
                    ? ""
                    : value.Value;
        }


        private int IntAttr(
            XElement element,
            string name)
        {
            int value;

            return
                int.TryParse(
                    (string)element.Attribute(name),
                    out value
                )
                    ? value
                    : 0;
        }


        private void SetText(
            XDocument sheetXml,
            string cellRef,
            string value)
        {
            XElement cell =
                EnsureCell(
                    sheetXml,
                    cellRef
                );

            XNamespace ns =
                MainNs;

            cell.SetAttributeValue(
                "t",
                "inlineStr"
            );

            cell.Element(ns + "v")?.Remove();
            cell.Element(ns + "is")?.Remove();

            cell.Add(
                new XElement(
                    ns + "is",
                    new XElement(
                        ns + "t",
                        new XAttribute(
                            XNamespace.Xml + "space",
                            "preserve"
                        ),
                        value ?? ""
                    )
                )
            );
        }


        private void SetNumber(
            XDocument sheetXml,
            string cellRef,
            decimal? value)
        {
            if (!value.HasValue)
            {
                SetText(
                    sheetXml,
                    cellRef,
                    ""
                );

                return;
            }

            XElement cell =
                EnsureCell(
                    sheetXml,
                    cellRef
                );

            XNamespace ns =
                MainNs;

            cell.Attribute("t")?.Remove();
            cell.Element(ns + "is")?.Remove();
            cell.Element(ns + "v")?.Remove();

            cell.Add(
                new XElement(
                    ns + "v",
                    value.Value.ToString(
                        "0.###",
                        CultureInfo.InvariantCulture
                    )
                )
            );
        }


        private XElement EnsureCell(
            XDocument sheetXml,
            string cellRef)
        {
            XNamespace ns =
                MainNs;

            Match match =
                Regex.Match(
                    cellRef,
                    @"^([A-Z]+)(\d+)$"
                );

            if (!match.Success)
                throw new Exception("Invalid cell reference: " + cellRef);

            string column =
                match.Groups[1].Value;

            int rowNo =
                Convert.ToInt32(
                    match.Groups[2].Value
                );

            XElement sheetData =
                sheetXml.Root.Element(
                    ns + "sheetData"
                );

            XElement row =
                sheetData
                    .Elements(ns + "row")
                    .FirstOrDefault(
                        x =>
                            IntAttr(x, "r") == rowNo
                    );

            if (row == null)
            {
                row =
                    new XElement(
                        ns + "row",
                        new XAttribute(
                            "r",
                            rowNo
                        )
                    );

                XElement after =
                    sheetData
                        .Elements(ns + "row")
                        .LastOrDefault(
                            x =>
                                IntAttr(x, "r") < rowNo
                        );

                if (after != null)
                    after.AddAfterSelf(row);
                else
                    sheetData.AddFirst(row);
            }

            XElement cell =
                row
                    .Elements(ns + "c")
                    .FirstOrDefault(
                        x =>
                            string.Equals(
                                (string)x.Attribute("r"),
                                cellRef,
                                StringComparison.OrdinalIgnoreCase
                            )
                    );

            if (cell != null)
                return cell;

            cell =
                new XElement(
                    ns + "c",
                    new XAttribute(
                        "r",
                        cellRef
                    )
                );

            int targetIndex =
                ColumnIndex(
                    column
                );

            XElement insertBefore =
                row
                    .Elements(ns + "c")
                    .FirstOrDefault(
                        x =>
                            ColumnIndex(
                                Regex.Match(
                                    (string)x.Attribute("r"),
                                    @"^[A-Z]+"
                                ).Value
                            )
                            > targetIndex
                    );

            if (insertBefore != null)
                insertBefore.AddBeforeSelf(cell);
            else
                row.Add(cell);

            return cell;
        }


        private int ColumnIndex(
            string column)
        {
            int value =
                0;

            foreach (char c in column)
            {
                value =
                    value * 26
                    +
                    (c - 'A' + 1);
            }

            return value;
        }
    }
}
