using LocationTrackingAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace LocationTrackingAPI.Controllers
{
    [RoutePrefix("api/Expense")]
    public class ExpenseController : ApiController
    {
        LogHelper _lLog = null;

        [HttpPost]
        [Route("GetExpenseMaster")]
        public IHttpActionResult GetExpenseMaster(int UserId)
        {
            ResponseConversion RObj = new ResponseConversion();
            DALExpense _obj = new DALExpense();
            DataSet objR = new DataSet();
            _lLog = new LogHelper("");
            try
            {
                if (UserId.ToString().Trim() == "" || UserId == 0)
                {
                    RObj.status = "false";
                    RObj.message = "user id can not be blank.";
                    RObj.userData = UserId;

                    return Json(new { RObj });
                }

                objR = _obj.GetExpenseMasterByUserId(UserId);

                if (objR != null && objR.Tables.Count > 0)
                {
                    RObj.status = "true";
                    RObj.message = "Successfully";
                    RObj.userData = objR;
                }
                else
                {
                    RObj.status = "false";
                    RObj.message = "Data Not Found.";
                    RObj.userData = objR;
                }
                return Json(new { RObj });
            }
            catch (Exception ex)
            {
                RObj.status = "false";
                RObj.message = ex.Message.ToString();
                RObj.userData = ex;
                _lLog.WriteFileToLocal(Newtonsoft.Json.JsonConvert.SerializeObject(RObj).ToString());
                return Json(new { RObj });
            }

        }

        [HttpPost]
        [Route("SaveExpense")]
        public IHttpActionResult SaveExpense([FromBody] AddExpense m)
        {
            ResponseConversion RObj = new ResponseConversion();
            DALExpense _obj = new DALExpense();
            string StrMsg = string.Empty;
            _lLog = new LogHelper("");
            try
            {
                if (m.UserID.ToString().Trim() == "" || m.UserID == 0)
                {
                    RObj.status = "false";
                    RObj.message = "you must pass user id.";
                    RObj.userData = null;

                    return Json(new { RObj });
                }


                int result = _obj.InsertExpenses(m);

                if (result == 1)
                {
                    result = _obj.InsertOtherExp(m);
                    StrMsg = result > 0 ? "s-1" : "";
                }

                if (StrMsg.Split('-')[0].ToLower() == "s")
                {
                    //m.VchNo = StrMsg.Split('-')[1];
                    RObj.status = "true";
                    RObj.message = "Success";
                    RObj.userData = m;

                }
                else if(result<0)
                {
                    RObj.status = "false";
                    RObj.message = "already exists";
                    RObj.userData = m;
                }
                else
                {
                    RObj.status = "false";
                    RObj.message = StrMsg;
                    RObj.userData = m;
                }
                return Json(new { RObj });
            }
            catch (Exception ex)
            {
                RObj.status = "false";
                RObj.message = ex.Message.ToString();
                RObj.userData = ex;
                _lLog.WriteFileToLocal(Newtonsoft.Json.JsonConvert.SerializeObject(RObj).ToString());
                return Json(new { RObj });
            }

        }

        

        [HttpPost]
        [Route("upload")]
        public IHttpActionResult UploadFile()
        {
            ResponseConversion RObj = new ResponseConversion();
            DALExpense _obj = new DALExpense();
            string StrMsg = string.Empty;
            _lLog = new LogHelper("");

            if (!Request.Content.IsMimeMultipartContent())
            {
                return BadRequest("Unsupported media type");
            }

            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var file = httpRequest.Files[0];
            var fileName = Path.GetFileName(file.FileName);
            var extension = Path.GetExtension(fileName);

            // Generate unique name
            var uniqueName = Guid.NewGuid().ToString() + extension;
            var uploadPath = HttpContext.Current.Server.MapPath("~/Uploads");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var filePath = Path.Combine(uploadPath, uniqueName);
            file.SaveAs(filePath);

            int FileId =_obj.saveFile(uniqueName);

            //string sourceFileUrl = "http://localhost:19656/Uploads/" + uniqueName;
            ////"http://49.200.63.179/LocationTrack/Uploads/"+ uniqueName;
            //string uploadApiUrl = "http://localhost/RSERP/Uploads/" + uniqueName;
            //    //"http://49.200.63.181/rserp/Uploads/"+ uniqueName; // Your API that saves file

            //CopyFileFromUrlToServer(sourceFileUrl, uploadApiUrl);

            return Ok(new
            {
                Status = true,
                Message = "File uploaded successfully",
                FileId= FileId,
                FileName = uniqueName,
                Path = "/Uploads/" + uniqueName
            });
        }

        [HttpPost]
        [Route("uploads")]
        public IHttpActionResult UploadFiles()
        {
            ResponseConversion RObj = new ResponseConversion();
            DALExpense _obj = new DALExpense();
            _lLog = new LogHelper("");

            if (!Request.Content.IsMimeMultipartContent())
                return BadRequest("Unsupported media type");

            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count == 0)
                return BadRequest("No files uploaded.");

            var uploadPath = HttpContext.Current.Server.MapPath("~/Uploads");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var uploadedFiles = new List<object>();

            for (int i = 0; i < httpRequest.Files.Count; i++)
            {
                var file = httpRequest.Files[i];
                var fileName = Path.GetFileName(file.FileName);
                var extension = Path.GetExtension(fileName);
                var KeyName = httpRequest.Files.AllKeys[i];
                var uniqueName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(uploadPath, uniqueName);

                file.SaveAs(filePath);

                int fileId = _obj.saveFile(uniqueName);

                uploadedFiles.Add(new
                {
                    KeyName = KeyName,
                    FileId = fileId,
                    FileName = uniqueName,
                    Path = "/Uploads/" + uniqueName
                });
            }

            return Ok(new
            {
                Status = true,
                Message = "Files uploaded successfully",
                Files = uploadedFiles
            });
        }

        public void CopyFileFromUrlToServer(string sourceUrl, string targetApiUrl)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Content-Type", "multipart/form-data");

                // 1️⃣ Download file from URL
                byte[] fileBytes = client.DownloadData(sourceUrl);

                // 2️⃣ Upload file to API
                byte[] response = client.UploadData(targetApiUrl, "POST", fileBytes);

                string result = Encoding.UTF8.GetString(response);
            }
        }

        [HttpPost]
        [Route("GetExpenseList")]
        public IHttpActionResult GetExpenseList(string StartDate, string EndDate, int UserId)
        {
            ResponseConversion RObj = new ResponseConversion();
            DALExpense _obj = new DALExpense();
            List<Expense> objR = new List<Expense>();
            _lLog = new LogHelper("");
            try
            {
                if (UserId.ToString().Trim() == "" || UserId == 0)
                {
                    RObj.status = "false";
                    RObj.message = "user id can not be blank.";
                    RObj.userData = UserId;

                    return Json(new { RObj });
                }

                objR = _obj.GetExpenses(1,UserId,StartDate,EndDate,0);

                if (objR != null && objR.Count > 0)
                {
                    RObj.status = "true";
                    RObj.message = "Successfully";
                    RObj.userData = objR;
                }
                else
                {
                    RObj.status = "false";
                    RObj.message = "Data Not Found.";
                    RObj.userData = objR;
                }
                return Json(new { RObj });
            }
            catch (Exception ex)
            {
                RObj.status = "false";
                RObj.message = ex.Message.ToString();
                RObj.userData = ex;
                _lLog.WriteFileToLocal(Newtonsoft.Json.JsonConvert.SerializeObject(RObj).ToString());
                return Json(new { RObj });
            }

        }

        [HttpPost]
        [Route("GetExpenseListById")]
        public IHttpActionResult GetExpenseListById(int ExpenseId)
        {
            ResponseConversion RObj = new ResponseConversion();
            DALExpense _obj = new DALExpense();
            AddExpense objR = new AddExpense();
            _lLog = new LogHelper("");
            try
            {
                if (ExpenseId.ToString().Trim() == "" || ExpenseId == 0)
                {
                    RObj.status = "false";
                    RObj.message = "user id can not be blank.";
                    RObj.userData = ExpenseId;

                    return Json(new { RObj });
                }

                objR = _obj.GetExpensesTourByID(ExpenseId);

                if (objR != null)
                {
                    RObj.status = "true";
                    RObj.message = "Successfully";
                    RObj.userData = objR;
                }
                else
                {
                    RObj.status = "false";
                    RObj.message = "Data Not Found.";
                    RObj.userData = objR;
                }
                return Json(new { RObj });
            }
            catch (Exception ex)
            {
                RObj.status = "false";
                RObj.message = ex.Message.ToString();
                RObj.userData = ex;
                _lLog.WriteFileToLocal(Newtonsoft.Json.JsonConvert.SerializeObject(RObj).ToString());
                return Json(new { RObj });
            }

        }
    }
}
