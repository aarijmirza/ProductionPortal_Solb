using BAL.Repositories;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static DAL.Models.ViewModel;

namespace ProductionPortal_Solb.Controllers
{
    public class DelayCounterMeasureController : Controller
    {
        private readonly DelayCounterMeasureRepository repo;

        public DelayCounterMeasureController()
        {
            repo =
                new DelayCounterMeasureRepository();
        }


        // =========================================================
        // ADD / DETAIL PAGE
        // =========================================================

        [HttpGet]
        public ActionResult add(
            int plantDelayID)
        {
            try
            {
                if (plantDelayID <= 0)
                {
                    TempData["Error"] =
                        "Invalid Plant Delay ID.";

                    return RedirectToAction(
                        "list",
                        "Maintenance"
                    );
                }

                DelayCounterMeasureVM model =
                    repo.GetPageData(
                        plantDelayID
                    );

                if (model == null)
                {
                    TempData["Error"] =
                        "Delay record could not be loaded.";

                    return RedirectToAction(
                        "list",
                        "Maintenance"
                    );
                }

                FailureAnalysisBLL analysis =
                    repo.GetFailureAnalysisByDelayID(
                        plantDelayID
                    );

                ViewBag.Analysis =
                    analysis;

                if (
                    analysis == null ||
                    analysis.ID <= 0
                )
                {
                    ViewBag.AnalysisMessage =
                        "Failure Analysis must be saved before adding countermeasures.";
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Unable to load countermeasure page. Error: " +
                    ex.Message;

                return RedirectToAction(
                    "list",
                    "Maintenance"
                );
            }
        }


        // =========================================================
        // SAVE MULTIPLE COUNTERMEASURES
        // Each row is linked to the active FailureAnalysis.ID.
        // Each row may have its own Evidence attachment.
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMultiple(
            DelayCounterMeasureVM model)
        {
            int plantDelayID =
                model != null
                    ? model.PlantDelayID
                    : 0;

            try
            {
                if (
                    model == null ||
                    plantDelayID <= 0
                )
                {
                    throw new Exception(
                        "Invalid countermeasure request."
                    );
                }


                FailureAnalysisBLL analysis =
                    repo.GetFailureAnalysisByDelayID(
                        plantDelayID
                    );

                if (
                    analysis == null ||
                    analysis.ID <= 0
                )
                {
                    throw new Exception(
                        "Failure Analysis is required before saving countermeasures."
                    );
                }


                List<DelayCounterMeasureBLL> rows =
                    model.CounterMeasures
                    ?? new List<DelayCounterMeasureBLL>();

                if (rows.Count == 0)
                {
                    throw new Exception(
                        "At least one countermeasure is required."
                    );
                }


                string currentUser =
                    GetCurrentUser();

                int savedCount =
                    0;


                for (
                    int index = 0;
                    index < rows.Count;
                    index++
                )
                {
                    DelayCounterMeasureBLL item =
                        rows[index];

                    if (
                        item == null ||
                        string.IsNullOrWhiteSpace(
                            item.CounterMeasure
                        )
                    )
                    {
                        continue;
                    }


                    // Server-side link. Never trust a posted AnalysisID.
                    item.PlantDelayID =
                        plantDelayID;

                    item.AnalysisID =
                        analysis.ID;

                    if (string.IsNullOrWhiteSpace(
                        item.CounterMeasureStatus
                    ))
                    {
                        item.CounterMeasureStatus =
                            "Open";
                    }


                    HttpPostedFileBase evidenceFile =
                        Request.Files[
                            "EvidenceFiles[" +
                            index +
                            "]"
                        ];


                    string savedRelativePath =
                        null;

                    string originalFileName =
                        null;


                    if (
                        evidenceFile != null &&
                        evidenceFile.ContentLength > 0
                    )
                    {
                        originalFileName =
                            Path.GetFileName(
                                evidenceFile.FileName
                            );

                        savedRelativePath =
                            SaveEvidenceFile(
                                evidenceFile,
                                plantDelayID,
                                analysis.ID,
                                index
                            );

                        item.EvidenceFile =
                            savedRelativePath;

                        item.EvidenceFileName =
                            originalFileName;
                    }


                    int counterMeasureID =
                        repo.Save(
                            item,
                            currentUser
                        );


                    if (counterMeasureID <= 0)
                    {
                        DeleteUploadedFile(
                            savedRelativePath
                        );

                        throw new Exception(
                            "Countermeasure " +
                            (index + 1) +
                            " could not be saved."
                        );
                    }


                    item.ID =
                        counterMeasureID;

                    savedCount++;
                }


                if (savedCount <= 0)
                {
                    throw new Exception(
                        "No valid countermeasure was available to save."
                    );
                }


                TempData["Success"] =
                    savedCount +
                    " countermeasure(s) saved successfully against Analysis ID " +
                    analysis.ID +
                    ".";


                return RedirectToAction(
                    "add",
                    new
                    {
                        plantDelayID =
                            plantDelayID
                    }
                );
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Unable to save countermeasures. Error: " +
                    ex.Message;

                return RedirectToAction(
                    "add",
                    new
                    {
                        plantDelayID =
                            plantDelayID
                    }
                );
            }
        }


        // =========================================================
        // DELETE
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(
            int id)
        {
            try
            {
                DelayCounterMeasureBLL item =
                    repo.GetByID(id);

                if (item == null)
                {
                    return Json(
                        new
                        {
                            success = false,
                            message =
                                "Countermeasure not found."
                        }
                    );
                }

                bool deleted =
                    repo.Delete(
                        id,
                        GetCurrentUser()
                    );

                return Json(
                    new
                    {
                        success =
                            deleted,

                        message =
                            deleted
                                ? "Countermeasure deleted successfully."
                                : "Countermeasure could not be deleted."
                    }
                );
            }
            catch (Exception ex)
            {
                return Json(
                    new
                    {
                        success = false,
                        message = ex.Message
                    }
                );
            }
        }


        // =========================================================
        // EVIDENCE FILE
        // =========================================================

        private string SaveEvidenceFile(
            HttpPostedFileBase uploadedFile,
            int plantDelayID,
            int analysisID,
            int rowIndex)
        {
            const int maximumFileSize =
                10 * 1024 * 1024;


            if (
                uploadedFile == null ||
                uploadedFile.ContentLength <= 0
            )
            {
                return null;
            }


            if (
                uploadedFile.ContentLength >
                maximumFileSize
            )
            {
                throw new Exception(
                    "Evidence attachment size cannot exceed 10 MB."
                );
            }


            string extension =
                Path.GetExtension(
                    uploadedFile.FileName
                );

            extension =
                string.IsNullOrWhiteSpace(
                    extension
                )
                    ? ""
                    : extension.ToLowerInvariant();


            HashSet<string> allowedExtensions =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase
                )
                {
                    ".pdf",
                    ".doc",
                    ".docx",
                    ".xls",
                    ".xlsx",
                    ".jpg",
                    ".jpeg",
                    ".png"
                };


            if (!allowedExtensions.Contains(
                extension
            ))
            {
                throw new Exception(
                    "Evidence attachment must be PDF, Word, Excel, JPG, JPEG or PNG."
                );
            }


            string uploadDirectory =
                Server.MapPath(
                    "~/Uploads/CounterMeasureEvidence"
                );


            if (!Directory.Exists(
                uploadDirectory
            ))
            {
                Directory.CreateDirectory(
                    uploadDirectory
                );
            }


            string storedFileName =
                "CM-EVID-" +
                plantDelayID +
                "-FA" +
                analysisID +
                "-R" +
                (rowIndex + 1) +
                "-" +
                DateTime.Now.ToString(
                    "yyyyMMddHHmmssfff"
                ) +
                "-" +
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 8) +
                extension;


            string physicalPath =
                Path.Combine(
                    uploadDirectory,
                    storedFileName
                );


            uploadedFile.SaveAs(
                physicalPath
            );


            return
                "~/Uploads/CounterMeasureEvidence/" +
                storedFileName;
        }


        private void DeleteUploadedFile(
            string relativePath)
        {
            if (string.IsNullOrWhiteSpace(
                relativePath
            ))
            {
                return;
            }

            string physicalPath =
                Server.MapPath(
                    relativePath
                );

            if (System.IO.File.Exists(
                physicalPath
            ))
            {
                System.IO.File.Delete(
                    physicalPath
                );
            }
        }


        private string GetCurrentUser()
        {
            string currentUser =
                Convert.ToString(
                    Session["UserName"]
                );

            if (
                string.IsNullOrWhiteSpace(
                    currentUser
                ) &&
                User != null &&
                User.Identity != null
            )
            {
                currentUser =
                    User.Identity.Name;
            }

            return
                string.IsNullOrWhiteSpace(
                    currentUser
                )
                    ? "System"
                    : currentUser;
        }
    }
}
