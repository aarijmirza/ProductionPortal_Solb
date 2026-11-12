using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ClosedXML.Excel;

namespace WebAPICode.Helpers
{
    public class MTCGradeLimitRule
    {
        public string Specification { get; set; }
        public string Grade { get; set; }
        public string ProductType { get; set; }
        public string SizeRule { get; set; }

        public string YieldStrength { get; set; }
        public string TensileStrength { get; set; }
        public string TensileYieldRatio { get; set; }
        public string Elongation { get; set; }

        public string C { get; set; }
        public string Si { get; set; }
        public string Mn { get; set; }
        public string P { get; set; }
        public string S { get; set; }
        public string Cu { get; set; }
        public string Ni { get; set; }
        public string Cr { get; set; }
        public string Mo { get; set; }
        public string V { get; set; }
        public string B { get; set; }
        public string N { get; set; }
        public string Ceq { get; set; }
    }

    public class MTCGradeValidationResult
    {
        public MTCGradeValidationResult()
        {
            Errors = new List<string>();
        }

        public bool IsValid
        {
            get { return Errors.Count == 0; }
        }

        public MTCGradeLimitRule MatchedRule { get; set; }

        public List<string> Errors { get; private set; }
    }

    public class MTCGradeLimitService
    {
        private readonly string _gradeLimitsPath;

        public MTCGradeLimitService(string gradeLimitsPath)
        {
            _gradeLimitsPath = gradeLimitsPath;
        }

        public MTCGradeValidationResult Validate(
            string specification,
            string steelGrade,
            bool isWireRodOrCoil,
            decimal barSize,
            decimal yieldStrength,
            decimal tensileStrength,
            decimal tensileYieldRatio,
            decimal elongation,
            decimal c,
            decimal si,
            decimal mn,
            decimal p,
            decimal s,
            decimal n,
            decimal ceq)
        {
            MTCGradeValidationResult result =
                new MTCGradeValidationResult();

            if (string.IsNullOrWhiteSpace(_gradeLimitsPath) ||
                !File.Exists(_gradeLimitsPath))
            {
                result.Errors.Add(
                    "Grades Limits.xlsx was not found."
                );

                return result;
            }

            List<MTCGradeLimitRule> rules =
                LoadRules();

            MTCGradeLimitRule matched =
                FindBestRule(
                    rules,
                    specification,
                    steelGrade,
                    isWireRodOrCoil,
                    barSize
                );

            if (matched == null)
            {
                result.Errors.Add(
                    "No grade-limit row was found for Specification '" +
                    (specification ?? "") +
                    "', Grade '" +
                    (steelGrade ?? "") +
                    "', Product Type '" +
                    (isWireRodOrCoil
                        ? "Wire Rod - Coil"
                        : "Rebar") +
                    "', Size " +
                    barSize.ToString(
                        "0.###",
                        CultureInfo.InvariantCulture
                    ) +
                    " mm."
                );

                return result;
            }

            result.MatchedRule =
                matched;

            ValidateReading(
                result,
                "Yield Strength",
                yieldStrength,
                matched.YieldStrength
            );

            ValidateReading(
                result,
                "Tensile Strength",
                tensileStrength,
                matched.TensileStrength
            );

            ValidateReading(
                result,
                "Tensile / Yield Ratio",
                tensileYieldRatio,
                matched.TensileYieldRatio
            );

            ValidateReading(
                result,
                "Elongation",
                elongation,
                matched.Elongation
            );

            ValidateReading(
                result,
                "C",
                c,
                matched.C
            );

            ValidateReading(
                result,
                "Si",
                si,
                matched.Si
            );

            ValidateReading(
                result,
                "Mn",
                mn,
                matched.Mn
            );

            ValidateReading(
                result,
                "P",
                p,
                matched.P
            );

            ValidateReading(
                result,
                "S",
                s,
                matched.S
            );

            ValidateReading(
                result,
                "N",
                n,
                matched.N
            );

            ValidateReading(
                result,
                "Ceq",
                ceq,
                matched.Ceq
            );

            /*
             * Grades Limits.xlsx also contains:
             * Cu, Ni, Cr, Mo, V and B.
             *
             * Current QCInspectionRMDetailBLL / screen does not expose
             * these readings, therefore they cannot be validated here
             * without adding those fields to the model/source data.
             */

            return result;
        }

        private List<MTCGradeLimitRule> LoadRules()
        {
            List<MTCGradeLimitRule> rules =
                new List<MTCGradeLimitRule>();

            using (
                XLWorkbook workbook =
                    new XLWorkbook(
                        _gradeLimitsPath
                    )
            )
            {
                IXLWorksheet sheet =
                    workbook.Worksheet(
                        "Grades Limits"
                    );

                int lastRow =
                    sheet.LastRowUsed().RowNumber();

                /*
                 * Uploaded Grades Limits.xlsx:
                 * Row 3 = headers
                 * Row 4 onward = data
                 */
                for (
                    int row = 4;
                    row <= lastRow;
                    row++
                )
                {
                    string specification =
                        CellText(
                            sheet,
                            row,
                            1
                        );

                    string grade =
                        CellText(
                            sheet,
                            row,
                            2
                        );

                    if (
                        string.IsNullOrWhiteSpace(
                            specification
                        ) &&
                        string.IsNullOrWhiteSpace(
                            grade
                        )
                    )
                    {
                        continue;
                    }

                    MTCGradeLimitRule rule =
                        new MTCGradeLimitRule();

                    rule.Specification =
                        specification;

                    rule.Grade =
                        grade;

                    rule.ProductType =
                        CellText(
                            sheet,
                            row,
                            3
                        );

                    rule.SizeRule =
                        CellText(
                            sheet,
                            row,
                            4
                        );

                    rule.YieldStrength =
                        CellText(
                            sheet,
                            row,
                            5
                        );

                    rule.TensileStrength =
                        CellText(
                            sheet,
                            row,
                            6
                        );

                    rule.TensileYieldRatio =
                        CellText(
                            sheet,
                            row,
                            7
                        );

                    rule.Elongation =
                        CellText(
                            sheet,
                            row,
                            8
                        );

                    rule.C =
                        CellText(
                            sheet,
                            row,
                            9
                        );

                    rule.Si =
                        CellText(
                            sheet,
                            row,
                            10
                        );

                    rule.Mn =
                        CellText(
                            sheet,
                            row,
                            11
                        );

                    rule.P =
                        CellText(
                            sheet,
                            row,
                            12
                        );

                    rule.S =
                        CellText(
                            sheet,
                            row,
                            13
                        );

                    rule.Cu =
                        CellText(
                            sheet,
                            row,
                            14
                        );

                    rule.Ni =
                        CellText(
                            sheet,
                            row,
                            15
                        );

                    rule.Cr =
                        CellText(
                            sheet,
                            row,
                            16
                        );

                    rule.Mo =
                        CellText(
                            sheet,
                            row,
                            17
                        );

                    rule.V =
                        CellText(
                            sheet,
                            row,
                            18
                        );

                    rule.B =
                        CellText(
                            sheet,
                            row,
                            19
                        );

                    rule.N =
                        CellText(
                            sheet,
                            row,
                            20
                        );

                    rule.Ceq =
                        CellText(
                            sheet,
                            row,
                            21
                        );

                    rules.Add(
                        rule
                    );
                }
            }

            return rules;
        }

        private MTCGradeLimitRule FindBestRule(
            List<MTCGradeLimitRule> rules,
            string specification,
            string steelGrade,
            bool isWireRodOrCoil,
            decimal barSize)
        {
            string specKey =
                NormalizeKey(
                    specification
                );

            string gradeKey =
                NormalizeKey(
                    steelGrade
                );

            List<MTCGradeLimitRule> candidates =
                rules
                    .Where(
                        x =>
                            NormalizeKey(
                                x.Specification
                            ) == specKey
                            &&
                            NormalizeKey(
                                x.Grade
                            ) == gradeKey
                    )
                    .ToList();

            if (candidates.Count == 0)
            {
                return null;
            }

            if (isWireRodOrCoil)
            {
                candidates =
                    candidates
                        .Where(
                            x =>
                                IsWireRodProduct(
                                    x.ProductType
                                )
                        )
                        .ToList();
            }
            else
            {
                /*
                 * Current screen only has Wire Rod / Coil checkbox.
                 * When unchecked we prefer Rebar.
                 * For ASTM A615 Grade 60 the uploaded limit sheet
                 * contains equivalent Rebar / Plain Bar rows.
                 */
                List<MTCGradeLimitRule> rebarRules =
                    candidates
                        .Where(
                            x =>
                                NormalizeKey(
                                    x.ProductType
                                )
                                .Contains(
                                    "REBAR"
                                )
                        )
                        .ToList();

                if (rebarRules.Count > 0)
                {
                    candidates =
                        rebarRules;
                }
            }

            foreach (
                MTCGradeLimitRule rule
                in candidates
            )
            {
                if (
                    SizeMatches(
                        rule.SizeRule,
                        barSize
                    )
                )
                {
                    return rule;
                }
            }

            return null;
        }

        private bool SizeMatches(
            string sizeRule,
            decimal size)
        {
            if (
                string.IsNullOrWhiteSpace(
                    sizeRule
                ) ||
                sizeRule.Trim() == "--"
            )
            {
                return true;
            }

            string text =
                sizeRule
                    .Trim()
                    .ToUpperInvariant()
                    .Replace(
                        "MM",
                        ""
                    )
                    .Replace(
                        "M ",
                        " "
                    );

            List<decimal> numbers =
                Regex
                    .Matches(
                        text,
                        @"\d+(?:\.\d+)?"
                    )
                    .Cast<Match>()
                    .Select(
                        x =>
                            decimal.Parse(
                                x.Value,
                                CultureInfo.InvariantCulture
                            )
                    )
                    .ToList();

            if (numbers.Count == 0)
            {
                return true;
            }

            decimal first =
                numbers[0];

            if (
                text.Contains("≤") ||
                text.Contains("<=")
            )
            {
                return size <= first;
            }

            if (
                text.Contains("≥") ||
                text.Contains(">=")
            )
            {
                return size >= first;
            }

            if (
                text.Contains(">")
            )
            {
                return size > first;
            }

            if (
                text.Contains("<")
            )
            {
                return size < first;
            }

            if (
                text.Contains("=")
            )
            {
                return numbers.Any(
                    x =>
                        Math.Abs(
                            x - size
                        ) < 0.0001m
                );
            }

            return true;
        }

        private void ValidateReading(
            MTCGradeValidationResult result,
            string readingName,
            decimal reading,
            string limitText)
        {
            decimal? minimum;
            decimal? maximum;

            if (
                !TryParseLimit(
                    limitText,
                    out minimum,
                    out maximum
                )
            )
            {
                return;
            }

            if (
                minimum.HasValue &&
                reading < minimum.Value
            )
            {
                result.Errors.Add(
                    readingName +
                    " = " +
                    reading.ToString(
                        "0.###",
                        CultureInfo.InvariantCulture
                    ) +
                    " is below minimum " +
                    minimum.Value.ToString(
                        "0.###",
                        CultureInfo.InvariantCulture
                    ) +
                    "."
                );
            }

            if (
                maximum.HasValue &&
                reading > maximum.Value
            )
            {
                result.Errors.Add(
                    readingName +
                    " = " +
                    reading.ToString(
                        "0.###",
                        CultureInfo.InvariantCulture
                    ) +
                    " exceeds maximum " +
                    maximum.Value.ToString(
                        "0.###",
                        CultureInfo.InvariantCulture
                    ) +
                    "."
                );
            }
        }

        private bool TryParseLimit(
            string limitText,
            out decimal? minimum,
            out decimal? maximum)
        {
            minimum =
                null;

            maximum =
                null;

            if (
                string.IsNullOrWhiteSpace(
                    limitText
                ) ||
                limitText.Trim() == "--"
            )
            {
                return false;
            }

            string text =
                limitText
                    .Trim()
                    .ToUpperInvariant();

            Match minMatch =
                Regex.Match(
                    text,
                    @"MIN\s*\.?\s*([0-9]+(?:\.[0-9]+)?)"
                );

            Match maxMatch =
                Regex.Match(
                    text,
                    @"MAX\s*\.?\s*([0-9]+(?:\.[0-9]+)?)"
                );

            if (minMatch.Success)
            {
                minimum =
                    decimal.Parse(
                        minMatch.Groups[1].Value,
                        CultureInfo.InvariantCulture
                    );
            }

            if (maxMatch.Success)
            {
                maximum =
                    decimal.Parse(
                        maxMatch.Groups[1].Value,
                        CultureInfo.InvariantCulture
                    );
            }

            if (
                minimum.HasValue ||
                maximum.HasValue
            )
            {
                return true;
            }

            Match rangeMatch =
                Regex.Match(
                    text,
                    @"([0-9]+(?:\.[0-9]+)?)\s*TO\s*([0-9]+(?:\.[0-9]+)?)"
                );

            if (rangeMatch.Success)
            {
                minimum =
                    decimal.Parse(
                        rangeMatch.Groups[1].Value,
                        CultureInfo.InvariantCulture
                    );

                maximum =
                    decimal.Parse(
                        rangeMatch.Groups[2].Value,
                        CultureInfo.InvariantCulture
                    );

                return true;
            }

            return false;
        }

        private string CellText(
            IXLWorksheet sheet,
            int row,
            int column)
        {
            return
                sheet
                    .Cell(
                        row,
                        column
                    )
                    .GetFormattedString()
                    .Trim();
        }

        private bool IsWireRodProduct(
            string productType)
        {
            string key =
                NormalizeKey(
                    productType
                );

            return
                key.Contains(
                    "WIREROD"
                )
                ||
                key.Contains(
                    "COIL"
                );
        }

        private string NormalizeKey(
            string value)
        {
            if (
                string.IsNullOrWhiteSpace(
                    value
                )
            )
            {
                return "";
            }

            return
                Regex.Replace(
                    value.ToUpperInvariant(),
                    @"[^A-Z0-9]",
                    ""
                );
        }
    }
}
