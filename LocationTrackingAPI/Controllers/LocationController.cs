using System;
using System.Collections.Generic;
using System.Web.Http;
using LocationTrackingAPI.Models;
using System.Data;
using System.IO;
using System.Web;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;

namespace LocationTrackingAPI.Controllers
{
    [RoutePrefix("api/Location")]
    public class LocationController : ApiController
    {
        LogHelper _lLog = null;

        
        [HttpPost]
        [Route("GetUserData")]
        public IHttpActionResult GetUserData(AdminUserModel Model)
        {
            ResponseConversion RObj = new ResponseConversion();
            DALAccount _obj = new DALAccount();
            List<AdminUserModel> objR = new List<AdminUserModel>();
            _lLog = new LogHelper("");
            try
            {
                if (Model.UserName.ToString().Trim() == "" || Model.UserName == null)
                {
                    RObj.status = "false";
                    RObj.message = "user name can not be blank.";
                    RObj.userData = Model;

                    return Json(new { RObj });
                }

                if (Model.UserPassword.ToString().Trim() == "" || Model.UserPassword == null)
                {
                    RObj.status = "false";
                    RObj.message = "user password can not be blank.";
                    RObj.userData = Model;

                    return Json(new { RObj });
                }


                Model.UserPassword = CommonBLL.Encrypt(Model.UserPassword);
                objR = _obj.DoLogin(Model);

                if (objR != null && objR.Count > 0)
                {
                    if (objR[0].RoleID > 1)
                    {
                        RObj.status = "false";
                        RObj.message = "Please used your executive login ID.";
                        RObj.userData = null;
                        return Json(new { RObj });
                    }


                    if (_obj.CheckforUser(Model.UserName, Model.UserPassword))
                    {
                        RObj.status = "true";
                        RObj.message = "Successfully login.";
                        RObj.userData = objR;
                    }
                    else
                    {
                        RObj.status = "false";
                        RObj.message = "User Account Disabled.";
                        RObj.userData = null;
                    }

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

        [Authorize]
        [HttpPost]
        [Route("GetUserSchoolList")]
        public IHttpActionResult GetUserSchoolList(int UserId)
        {
            ResponseConversion RObj = new ResponseConversion();
            DALAccount _obj = new DALAccount();
            List<SamplingInstitute> objR = new List<SamplingInstitute>();
            _lLog = new LogHelper("");
            try
            {
                if (UserId.ToString().Trim() == "" || UserId == 0)
                {
                    RObj.status = "false";
                    RObj.message = "you must pass user id.";
                    RObj.userData = null;

                    return Json(new { RObj });
                }

                objR = _obj.GetSchoolList(UserId);

                if (objR != null && objR.Count > 0)
                {
                    RObj.status = "true";
                    RObj.message = "Success";
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
        [Route("GetUserTeacherList")]
        public IHttpActionResult GetUserTeacherList(int SchoolId,int UserId)
        {
            ResponseConversion RObj = new ResponseConversion();
            DALAccount _obj = new DALAccount();
            TeacherSpecimenDetails objR = new TeacherSpecimenDetails();
            _lLog = new LogHelper("");
            try
            {
                if (UserId.ToString().Trim() == "" || UserId == 0)
                {
                    RObj.status = "false";
                    RObj.message = "you must pass user id.";
                    RObj.userData = null;

                    return Json(new { RObj });
                }


                objR = _obj.GetTeacherList(SchoolId,UserId,1);

                if (objR != null)
                {
                    RObj.status = "true";
                    RObj.message = "Success";
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

        #region SaveSchoolVisit
        [HttpPost]
        [Route("SaveSchoolVisit")]
        public IHttpActionResult SaveSchoolVisit([FromBody] VisitModel m)
        {
            ResponseConversion RObj = new ResponseConversion();
            DALAccount _obj = new DALAccount();
            string StrMsg = string.Empty;
            _lLog = new LogHelper("");
            try
            {
                if (m.CreatedBy.ToString().Trim() == "" || m.CreatedBy == 0)
                {
                    RObj.status = "false";
                    RObj.message = "you must pass user id.";
                    RObj.userData = null;

                    return Json(new { RObj });
                }

                if (Convert.ToDateTime(m.StartDate)> Convert.ToDateTime(m.EndDate))
                {
                    RObj.status = "false";
                    RObj.message = "provided date range out of stock.";
                    RObj.userData = null;

                    return Json(new { RObj });
                }
                StrMsg = _obj.SaveSchoolVisitAtt(m);

                if (StrMsg.Split('-')[0].ToLower()=="s")
                {
                    m.VchNo= StrMsg.Split('-')[1];
                    RObj.status = "true";
                    RObj.message = "Success";
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

        
        [HttpGet]
        [Route("GetUserVisit")]
        public IHttpActionResult GetUserVisit(int UserId)
        {
            ResponseConversion RObj = new ResponseConversion();
            DALAccount _obj = new DALAccount();
            DataTable objR = new DataTable();
            _lLog = new LogHelper("");
            try
            {
                objR = _obj.GetSaveSchoolVisitAtt(UserId);
                
                if (objR != null && objR.Rows.Count > 0)
                {
                    RObj.status = "true";
                    RObj.message = "Success";
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

        [HttpGet]
        [Route("GetVisitReport")]
        public IHttpActionResult GetVisitReport(int UserId)
        {
            ResponseConversion RObj = new ResponseConversion();
            DALAccount _obj = new DALAccount();
            DataTable objR = new DataTable();
            _lLog = new LogHelper("");
            try
            {
                objR = _obj.GetVisitReport(UserId);

                if (objR != null && objR.Rows.Count > 0)
                {
                    RObj.status = "true";
                    RObj.message = "Success";
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
        #endregion

        #region HotelStay

        [HttpPost]
        [Route("SaveHotelStay")]
        public async Task<IHttpActionResult> SaveHotelStay()
        {
            ResponseConversion RObj = new ResponseConversion();
            DALAccount _obj = new DALAccount();
            LogHelper _lLog = new LogHelper("");

            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    return BadRequest("multipart/form-data required");

                string uploadPath = HttpContext.Current.Server.MapPath("~/Uploads/");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var provider = new MultipartFormDataStreamProvider(uploadPath);

                await Request.Content.ReadAsMultipartAsync(provider);

                // --------- Get Form Fields ----------
                HotalStay m = new HotalStay();
                m.Id = Convert.ToInt32(provider.FormData["Id"]);
                m.StartLatitude = provider.FormData["StartLatitude"];
                m.StartLongitude = provider.FormData["StartLongitude"];
                m.MyStartAddress = provider.FormData["MyStartAddress"];
                m.StartDate = provider.FormData["StartDate"];
                m.StartTime = provider.FormData["StartTime"];
                m.VisitName = provider.FormData["VisitName"];
                m.Remarks = provider.FormData["Remarks"];
                m.Purpose = provider.FormData["Purpose"];
                m.CreatedBy = Convert.ToInt32(provider.FormData["CreatedBy"]);
                m.CreatedOn = provider.FormData["CreatedOn"];
                m.UpdatedBy = Convert.ToInt32(provider.FormData["UpdatedBy"]);
                m.UpdatedOn = provider.FormData["UpdatedOn"];
                m.DeviceId = provider.FormData["DeviceId"];
                // --------- Handle File Upload ----------
                var fileData = provider.FileData.FirstOrDefault();
                if (fileData != null)
                {
                    string extension = Path.GetExtension(fileData.Headers.ContentDisposition.FileName.Trim('"'));
                    string fileName = Guid.NewGuid() + extension;

                    string newPath = Path.Combine(uploadPath, fileName);
                    File.Move(fileData.LocalFileName, newPath);

                    m.ImagePath = fileName;
                }

                // --------- Save in DB ----------
                string StrMsg = _obj.SaveHotelStay(m);

                if (StrMsg.Split('-')[0].ToLower() == "s")
                {
                    m.Id = Convert.ToInt32(StrMsg.Split('-')[1]);
                    RObj.status = "true";
                    RObj.message = "Success";
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
                RObj.message = ex.Message;
                RObj.userData = null;
                _lLog.WriteFileToLocal(Newtonsoft.Json.JsonConvert.SerializeObject(RObj));
                return Json(new { RObj });
            }
        }


        [HttpGet]
        [Route("GetHotelStay")]
        public IHttpActionResult GetHotelStay(int UserId)
        {
            ResponseConversion RObj = new ResponseConversion();
            DALAccount _obj = new DALAccount();
            DataTable objR = new DataTable();
            _lLog = new LogHelper("");
            try
            {
                objR =  _obj.GetSaveHotalStayAtt(UserId);

                if (objR != null && objR.Rows.Count > 0)
                {
                    RObj.status = "true";
                    RObj.message = "Success";
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
        #endregion

        #region Travel
        [HttpGet]
        [Route("GetTravelReport")]
        public IHttpActionResult GetTravelReport(int UserId)
        {
            ResponseConversion RObj = new ResponseConversion();
            DALAccount _obj = new DALAccount();
            DataTable objR = new DataTable();
            _lLog = new LogHelper("");
            try
            {
                objR = _obj.GetTravelReport(UserId);

                if (objR != null && objR.Rows.Count > 0)
                {
                    RObj.status = "true";
                    RObj.message = "Success";
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
        #endregion

        #region UserDeviceSetting
        [HttpPost]
        [Route("SaveUserDeviceSetting")]
        public IHttpActionResult SaveUserDeviceSetting([FromBody] UserDeviceSetting m)
        {
            ResponseConversion RObj = new ResponseConversion();
            DALAccount _obj = new DALAccount();
            string StrMsg = string.Empty;
            _lLog = new LogHelper("");
            try
            {
                if (m.DeviceId.ToString().Trim() == "" || m.DeviceId == null)
                {
                    RObj.status = "false";
                    RObj.message = "you must pass device id.";
                    RObj.userData = null;

                    return Json(new { RObj });
                }

                if (m.UserId.ToString().Trim() == "" || m.UserId == null)
                {
                    RObj.status = "false";
                    RObj.message = "you must pass user id.";
                    RObj.userData = null;

                    return Json(new { RObj });
                }


                StrMsg = _obj.SaveUserDeviceSetting(m);

                if (StrMsg.Split('-')[0].ToLower() == "s")
                {
                    
                    RObj.status = "true";
                    RObj.message = "Success";
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

        [HttpGet]
        [Route("GetUserStatus")]
        public IHttpActionResult GetUserStatus(int UserId, string DeviceId)
        {
            ResponseConversion RObj = new ResponseConversion();
            DALAccount _obj = new DALAccount();
            DataTable objR = new DataTable();
            _lLog = new LogHelper("");
            try
            {
                objR = _obj.GetUserConfig(UserId,DeviceId);

                if (objR != null && objR.Rows.Count > 0)
                {
                    RObj.status = "true";
                    RObj.message = "Success";
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
        #endregion

        [HttpGet]
        [Route("GetVchStatus")]
        public IHttpActionResult GetVchStatus(int UserId, int Purpose, int VisitEnd, string DeviceId)
        {
            ResponseConversion RObj = new ResponseConversion();
            DALAccount _obj = new DALAccount();
            DataTable objR = new DataTable();
            _lLog = new LogHelper("");
            try
            {
                objR = _obj.GetVchStatus(UserId, Purpose,VisitEnd,DeviceId);

                if (objR != null && objR.Rows.Count > 0)
                {
                    RObj.status = "true";
                    RObj.message = "Success";
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

                objR = _obj.GetExpenseMasterByUserIdAuto(UserId);

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
    }
}
