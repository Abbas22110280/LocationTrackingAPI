using LocationTrackingAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LocationTrackingAPI.Controllers
{
    [RoutePrefix("api/Distance")]
    public class DistanceMatrixController : ApiController
    {
        LogHelper _lLog = null;

        [HttpGet]
        [Route("GetDistance")]
        public async Task<IHttpActionResult> GetDistance(DistanceRequest Model)
        {
            ResponseConversion RObj = new ResponseConversion();
            DALDistanceMatrix _obj = new DALDistanceMatrix();
            DistanceResponse objR = new DistanceResponse();
            _lLog = new LogHelper("");
            try
            {
                 objR = await _obj.CalculateMatrix(Model);

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
    }
}
