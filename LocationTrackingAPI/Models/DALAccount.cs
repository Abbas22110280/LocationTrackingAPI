using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace LocationTrackingAPI.Models
{
    public class DALAccount
    {
        string _strCon = string.Empty;
        LogHelper _lLog = null;
        public DALAccount()
        {
            _lLog = new LogHelper("");
            _strCon = ConfigurationManager.AppSettings["ConnStr"].ToString().Trim();
        }

        public List<AdminUserModel> DoLogin(AdminUserModel Model)
        {
            List<AdminUserModel> _data = new List<AdminUserModel>();
            DBHelper _lhelp = new DBHelper(_strCon);
            try
            {
                SqlParameter[] spParameter = new SqlParameter[7];
                spParameter[0] = new SqlParameter("@UserName", SqlDbType.NVarChar, 1000);
                spParameter[0].Value = Model.UserName;
                spParameter[1] = new SqlParameter("@Password", SqlDbType.NVarChar, 1000);
                spParameter[1].Value = Model.UserPassword;

                spParameter[2] = new SqlParameter("@DeviceId", SqlDbType.NVarChar, 1000);
                spParameter[2].Value = Model.DeviceId;
                spParameter[3] = new SqlParameter("@DeviceCompany", SqlDbType.NVarChar, 1000);
                spParameter[3].Value = Model.DeviceCompany;
                spParameter[4] = new SqlParameter("@ModalName", SqlDbType.NVarChar, 1000);
                spParameter[4].Value = Model.ModelName;
                spParameter[5] = new SqlParameter("@PlateFormId", SqlDbType.NVarChar, 1000);
                spParameter[5].Value = Model.PlatformId;
                spParameter[6] = new SqlParameter("@DeviceVersion", SqlDbType.NVarChar, 1000);
                spParameter[6].Value = Model.DeviceVersion;

                var data = _lhelp.GetDataTableByProcedure("usp_GetAttUserByUserPassword", spParameter);
                _data = (from DataRow dr in data.Rows
                         select new AdminUserModel()
                         {
                             UserID = Convert.ToInt32(dr["UserID"]),
                             UserName = Convert.ToString(dr["UserName"]),
                             UserPassword = Convert.ToString(dr["UserPassword"]),
                             RoleID = Convert.ToInt32(dr["RoleID"]),
                             isActive = Convert.ToInt32(dr["isActive"]),
                             ApplyDonetion = Convert.ToInt32(dr["ApplyDonetion"]),
                             DeviceId = Model.DeviceId,
                             DeviceCompany = Model.DeviceCompany,
                             ModelName = Model.ModelName,
                             PlatformId = Model.PlatformId,
                             DeviceVersion = Model.DeviceVersion,
                             AllowMinDistance = 2
                         }).ToList();
                return _data;
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                _lLog.WriteFileToLocal(ex.Message.ToString());
                return _data;

            }
        }

        public bool CheckforUser(string UserName, string Password)
        {
            bool isActive = false;
            try
            {
                DBHelper _lhelp = new DBHelper(_strCon);
                DataTable dt = _lhelp.GetDataTableByExpression("Select * from Users where Username='" + UserName.Trim() + "' and Password='" + Password.Trim() + "' And Status=1");
                if (dt != null && dt.Rows.Count > 0)
                {
                    isActive = true;
                }
                else
                {
                    isActive = false;
                }
            }
            catch (Exception ex)
            {
                _lLog.WriteFileToLocal(ex.Message.ToString());
            }

            return isActive;
        }

        public List<SamplingInstitute> GetSchoolList(int UserId)
        {
            List<SamplingInstitute> siList = new List<SamplingInstitute>();
            try
            {
                DBHelper _lhelp = new DBHelper(_strCon);
                //get Ledget Id
                int LedgerID = 0, CompanyID = 1, bod = 0;
                DataTable dtLedger = _lhelp.GetDataTableByExpression(@"Select LedgerID from Users_default where User_ID='" + UserId + "'");
                if (dtLedger != null && dtLedger.Rows.Count > 0)
                {
                    LedgerID = Convert.ToInt32(dtLedger.Rows[0]["LedgerID"]);
                }

                SqlParameter[] p =
                {
                    new SqlParameter("@CompanyID",CompanyID),
                    new SqlParameter("@LedgerID",LedgerID),
                    new SqlParameter("@BoradID",bod),

                };

                DataTable tbl = new DataTable("tbl");
                tbl = _lhelp.GetDataTableByProcedure("usp_GetAllocatedSchoolsByBoard", p);
                siList = (from DataRow dr in tbl.Rows
                          select new SamplingInstitute()
                          {
                              Insitute_id = Convert.ToInt32(dr["Insitute_id"]),
                              CODE = Convert.ToString(dr["CODE"]),
                              Name = Convert.ToString(dr["Name"]),
                              NameWithCode = Convert.ToString(dr["Name"]) + " (" + Convert.ToString(dr["CODE"]) + ")",
                              Address = Convert.ToString(dr["Address"]),
                              Address2 = Convert.ToString(dr["Address2"]),
                              Address3 = Convert.ToString(dr["Address3"]),
                              Locality = Convert.ToString(dr["Locality"]),
                              Insitute_Board = Convert.ToString(dr["Insitute_Board"]),
                              Insitute_Category = Convert.ToString(dr["Insitute_Category"]),
                              Insitute_Level = Convert.ToString(dr["Insitute_Level"]),
                              State = Convert.ToString(dr["State"]),
                              District = Convert.ToString(dr["District"]),
                              LastVisit = Convert.ToString(dr["LastVisit"]),
                              LedgerID = LedgerID,
                              InstitutePinCode = Convert.ToString(dr["InstitutePin"]),
                              Latitude=0.00,
                              Longitude=0.00

                              //Insitute_id = Convert.ToInt32(dr["Insitute_id"]),
                              //CODE = Convert.ToString(dr["CODE"]),
                              //Name = "Feroz Shah Kotla",
                              //NameWithCode =  "ABC (" + Convert.ToString(dr["CODE"]) + ")",
                              //Address = "Feroz Shah Kotla ",
                              //Address2 = "Stadium St",
                              //Address3 = "Bahadur Shah Zafar Marg",
                              //Locality = "New Delhi",
                              //Insitute_Board = Convert.ToString(dr["Insitute_Board"]),
                              //Insitute_Category = Convert.ToString(dr["Insitute_Category"]),
                              //Insitute_Level = Convert.ToString(dr["Insitute_Level"]),
                              //State = "Delhi",
                              //District = "Central Delhi",
                              //LastVisit = Convert.ToString(dr["LastVisit"]),
                              //LedgerID = LedgerID,
                              //InstitutePinCode = "110002",

                          }).ToList();

                return siList;

            }
            catch (Exception ex)
            {
                string strMsg = ex.Message;
                _lLog.WriteFileToLocal(ex.Message.ToString());
                return siList;
            }
        }

        public TeacherSpecimenDetails GetTeacherList(int SchoolID, int UserId, int CompanyID)
        {
            TeacherSpecimenDetails objs = new TeacherSpecimenDetails();
            try
            {

                List<SpecimenTeachers> teachList = new List<SpecimenTeachers>();
                //List<SpecimenStocks> stkList = new List<SpecimenStocks>();


                DBHelper _lhelp = new DBHelper(_strCon);
                int LedgerID = 0;
                DataTable dtLedger = _lhelp.GetDataTableByExpression(@"Select LedgerID from Teacher_ledger where User_ID='" + UserId + "'");
                if (dtLedger != null && dtLedger.Rows.Count > 0)
                {
                    LedgerID = Convert.ToInt32(dtLedger.Rows[0]["LedgerID"]);
                }

                SqlParameter[] p =
                {
                        new SqlParameter("@CompanyID",CompanyID),
                        new SqlParameter("@LedgerID",LedgerID),
                        new SqlParameter("@SchoolID",SchoolID),

                    };


                DataSet dset = new DataSet("dset");
                dset = _lhelp.GetDataSetByProcedure("usp_GetSpecimenBalanceAndTeachers", p);

                teachList = (from DataRow dr in dset.Tables[0].Rows
                             select new SpecimenTeachers()
                             {
                                 Teacher_ID = Convert.ToInt32(dr["Teacher_ID"]),
                                 Institute_ID = Convert.ToInt32(dr["Institute_ID"]),
                                 Name = Convert.ToString(dr["Name"]),
                                 Mobile = Convert.ToString(dr["Mobile"]),
                                 subject = Convert.ToString(dr["subject"]),
                                 Designation = Convert.ToString(dr["Designation"]),
                                 LastVisit = Convert.ToString(dr["LastVisit"])

                             }).ToList();

                //stkList = (from DataRow dr in dset.Tables[1].Rows
                //           select new SpecimenStocks()
                //           {
                //               ItemID = Convert.ToInt32(dr["ItemID"]),
                //               ItemTitle = Convert.ToString(dr["ItemTitle"]),
                //               Stock_Item_Group_ID = Convert.ToInt32(dr["Stock_Item_Group_ID"]),
                //               Stock_Item_Group = Convert.ToString(dr["Stock_Item_Group"]),
                //               Stock_Item_Sub_Group_ID = Convert.ToInt32(dr["Stock_Item_Sub_Group_ID"]),
                //               Stock_Item_Sub_Group = Convert.ToString(dr["Stock_Item_Sub_Group"]),
                //               Balance = Convert.ToInt32(dr["Balance"])

                //           }).ToList();

                objs.teachList = teachList;
                //objs.stkList = stkList;

                return objs;


            }
            catch (Exception ex)
            {
                string strMsg = ex.Message;
                _lLog.WriteFileToLocal(ex.Message.ToString());
                return objs;
            }


        }

        #region SaveSchoolVisit
        public string SaveSchoolVisitAtt(VisitModel m)
        {
            string Msg = "F";
            try
            {
                DBHelper _lhelp = new DBHelper(_strCon);
                SqlParameter[] p =
                {
                    new SqlParameter("@Id", m.Identity),
                    new SqlParameter("@SchoolId", m.SchoolId),
                    new SqlParameter("@SchoolName", m.SchoolName.Trim()),
                    new SqlParameter("@TeacherId", m.TeacherId),
                    new SqlParameter("@TeacherName", m.TeacherName.Trim()),
                    new SqlParameter("@Pincode", m.Pincode),
                    new SqlParameter("@SchoolAddress", m.SchoolAddress),
                    new SqlParameter("@MyStartAddress", m.MyStartAddress),
                    new SqlParameter("@MyEndAddress", m.MyEndAddress),
                    new SqlParameter("@StartDate", Convert.ToDateTime(m.StartDate).ToString("yyyy-MM-dd")),
                    new SqlParameter("@StartTime", Convert.ToDateTime(m.StartTime).ToString("HH:mm")),
                    new SqlParameter("@EndDate", Convert.ToDateTime(m.EndDate).ToString("yyyy-MM-dd")),
                    new SqlParameter("@EndTime", Convert.ToDateTime(m.EndTime).ToString("HH:mm")),
                    new SqlParameter("@VisitType", m.VisitType),
                    new SqlParameter("@Purpose", m.Purpose),
                    new SqlParameter("@VisitEnd", m.VisitEnd),
                    new SqlParameter("@StartLatitude", m.StartLatitude),
                    new SqlParameter("@StartLongitude", m.StartLongitude),
                    new SqlParameter("@EndLatitude", m.EndLatitude),
                    new SqlParameter("@EndLongitude", m.EndLongitude),
                    new SqlParameter("@Remarks", m.Remarks),
                    new SqlParameter("@CreatedBy", m.CreatedBy),
                    new SqlParameter("@VchNo", m.VchNo.Trim()),
                    new SqlParameter("@DeviceId",m.DeviceId.Trim()),
                    new SqlParameter("@approxDestinationLatitude",m.approxDestinationLatitude),
                    new SqlParameter("@approxDestinationLongitude",m.approxDestinationLongitude),
                    new SqlParameter("@approxDistance",m.approxDistance),
                    new SqlParameter("@approxActualTravelDistance",m.approxActualTravelDistance),
                    new SqlParameter("@IsLocationFoundOnMap",m.IsLocationFoundOnMap),
                    new SqlParameter("@IsPunchOutSideLocation",m.punchOutsideLocation),
                    new SqlParameter("@Category",m.Category),
                    new SqlParameter("@TravelRate",m.TravelRate),
                    new SqlParameter("@StationCovered",m.StationCovered)
                };

                DataTable dt = _lhelp.GetDataTableByProcedure("usp_SaveAttSalePersonSchoolVisit", p);
                if(dt!=null && dt.Rows.Count>0)
                {
                    if(m.VisitEnd==1 && (m.Purpose==1 || m.Purpose == 2))
                    {
                        m.Mode = 2;
                        m.TravelClaimAmount = Convert.ToDouble(m.TravelRate * m.approxActualTravelDistance);
                        m.VchNo= dt.Rows[0]["VchNo"].ToString().Trim();
                        PostExpenseVoucher(m);
                    }
                        

                    Msg ="S-"+ dt.Rows[0]["VchNo"].ToString().Trim();
                }
            }
            catch(Exception ex)
            {
                string strMsg = ex.Message;
                _lLog.WriteFileToLocal(ex.Message.ToString());
                Msg = strMsg;
            }
            return Msg;
        }

        public DataTable GetSaveSchoolVisitAtt(int Id)
        {
            DataTable objs = new DataTable();
            try
            {
                
                DBHelper _lhelp = new DBHelper(_strCon);
                SqlParameter[] p =
                {
                    new SqlParameter("@Id",Id)
                };
                objs = _lhelp.GetDataTableByProcedure("usp_GetAttSalePersonSchoolVisit",p);
                
                return objs;


            }
            catch (Exception ex)
            {
                string strMsg = ex.Message;
                _lLog.WriteFileToLocal(ex.Message.ToString());
                return objs;
            }

            return objs;
        }
        #endregion

        #region HotalStay
        public string SaveHotelStay(HotalStay m)
        {
            string Msg = "F";
            try
            {
                DBHelper _lhelp = new DBHelper(_strCon);
                SqlParameter[] p =
                {
                    new SqlParameter("@Id", m.Id),
                    new SqlParameter("@StartLatitude", m.StartLatitude),
                    new SqlParameter("@StartLongitude", m.StartLongitude),
                    new SqlParameter("@MyStartAddress", m.MyStartAddress.Trim()),
                    new SqlParameter("@StartDate", Convert.ToDateTime(m.StartDate).ToString("yyyy-MM-dd")),
                    new SqlParameter("@StartTime", Convert.ToDateTime(m.StartTime).ToString("HH:mm")),
                    new SqlParameter("@VisitName", m.VisitName.Trim()),
                    new SqlParameter("@Remarks", m.Remarks.Trim()),
                    new SqlParameter("@Purpose", Convert.ToInt32(m.Purpose)),
                    new SqlParameter("@ImagePath", m.ImagePath.Trim()),
                    new SqlParameter("@CreatedBy", Convert.ToInt32(m.CreatedBy)),
                    new SqlParameter("@DeviceId",m.DeviceId.ToString().Trim())
                   
                };

                DataTable dt = _lhelp.GetDataTableByProcedure("usp_SaveAttHotalStay", p);
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (m.VisitName.ToLower() == "hotel stay" && m.Purpose.Trim() == "3")
                    {
                        m.Mode = 1;
                        m.VchNo = dt.Rows[0]["VchNo"].ToString().Trim();
                        PostExpenseVoucherHotel(m);
                    }

                    Msg = "S-" + dt.Rows[0]["VchNo"].ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                string strMsg = ex.Message;
                _lLog.WriteFileToLocal(ex.Message.ToString());
                Msg = strMsg;
            }
            return Msg;
        }

        public DataTable GetSaveHotalStayAtt(int Id)
        {
            DataTable objs = new DataTable();
            try
            {

                DBHelper _lhelp = new DBHelper(_strCon);
                SqlParameter[] p =
                {
                    new SqlParameter("@Id",Id)
                };
                objs =  _lhelp.GetDataTableByProcedure("usp_GetAttHotalStay", p);

                return objs;


            }
            catch (Exception ex)
            {
                string strMsg = ex.Message;
                _lLog.WriteFileToLocal(ex.Message.ToString());
                return objs;
            }

            return objs;
        }
        #endregion


        #region Reports
        public DataTable GetVisitReport(int UserId)
        {
            DataTable objs = new DataTable();
            try
            {

                DBHelper _lhelp = new DBHelper(_strCon);
                SqlParameter[] p =
                {
                    new SqlParameter("@UserId",UserId)
                };
                objs = _lhelp.GetDataTableByProcedure("usp_AttGetUserSchoolVisitReport", p);

                return objs;


            }
            catch (Exception ex)
            {
                string strMsg = ex.Message;
                _lLog.WriteFileToLocal(ex.Message.ToString());
                return objs;
            }

            return objs;
        }

        public DataTable GetTravelReport(int UserId)
        {
            DataTable objs = new DataTable();
            try
            {

                DBHelper _lhelp = new DBHelper(_strCon);
                SqlParameter[] p =
                {
                    new SqlParameter("@UserId",UserId)
                };
                objs = _lhelp.GetDataTableByProcedure("usp_AttGetUserTravelReport", p);

                return objs;


            }
            catch (Exception ex)
            {
                string strMsg = ex.Message;
                _lLog.WriteFileToLocal(ex.Message.ToString());
                return objs;
            }

            return objs;
        }
        #endregion

        #region UserDeviceSetting
        public string SaveUserDeviceSetting(UserDeviceSetting m)
        {
            string Msg = "F";
            try
            {
                DBHelper _lhelp = new DBHelper(_strCon);
                SqlParameter[] p =
                {
                    new SqlParameter("@GPSStatus", m.GpsStatus),
                    new SqlParameter("@LocationServiceEnabled", m.LocationServiceEnabled),
                    new SqlParameter("@PermissionStatus", m.PermissionStatus.Trim()),
                    new SqlParameter("@Timestamp", m.Timestamp),
                    new SqlParameter("@UserId", Convert.ToInt32(m.UserId)),
                    new SqlParameter("@DeviceId", m.DeviceId.Trim()),
                    
                };

                long nResult = _lhelp.ExecuteProcedure("usp_AttUpdateUserDeviceSetting", p);
                if (nResult > 0)
                {
                    Msg = "S-1";
                }
            }
            catch (Exception ex)
            {
                string strMsg = ex.Message;
                _lLog.WriteFileToLocal(ex.Message.ToString());
                Msg = strMsg;
            }
            return Msg;
        }

        public DataTable GetUserConfig(int UserId, string DeviceId)
        {
            DataTable objs = new DataTable();
            try
            {

                DBHelper _lhelp = new DBHelper(_strCon);
                SqlParameter[] p =
                {
                    new SqlParameter("@UserId",UserId),
                    new SqlParameter("@DeviceId",DeviceId)
                };
                objs = _lhelp.GetDataTableByProcedure("usp_AttGetUserConfig", p);

                return objs;


            }
            catch (Exception ex)
            {
                string strMsg = ex.Message;
                _lLog.WriteFileToLocal(ex.Message.ToString());
                return objs;
            }

            return objs;
        }
        #endregion

        #region getVchStatus
        public DataTable GetVchStatus(int UserId, int Purpose, int VisitEnd,string DeviceId)
        {
            DataTable objs = new DataTable();
            try
            {

                DBHelper _lhelp = new DBHelper(_strCon);
                SqlParameter[] p =
                {
                    new SqlParameter("@UserId",UserId),
                    new SqlParameter("@Purpose",Purpose),
                    new SqlParameter("@VisitEnd",VisitEnd),
                    new SqlParameter("@DeviceID",DeviceId)
                };
                objs = _lhelp.GetDataTableByProcedure("usp_AttGetVchStatus", p);

                return objs;


            }
            catch (Exception ex)
            {
                string strMsg = ex.Message;
                _lLog.WriteFileToLocal(ex.Message.ToString());
                return objs;
            }

            return objs;
        }
        #endregion

        #region Posting
        public void PostExpenseVoucher(VisitModel m)
        {
            try
            {
                DBHelper _lhelp = new DBHelper(_strCon);
                double distance = m.approxActualTravelDistance;

                int finalDistance = (int)Math.Ceiling(distance);
                SqlParameter[] p =
                {
                    new SqlParameter("@BillFromDate",m.StartDate),
                    new SqlParameter("@BillToDate",m.EndDate),
                    new SqlParameter("@UserId",m.CreatedBy),
                    new SqlParameter("@StationCovered", m.StationCovered),
                    new SqlParameter("@Mode",m.Mode),
                    new SqlParameter("@CategoryID",m.Category),
                    new SqlParameter("@TravelRate",m.TravelRate),
                    new SqlParameter("@TravelTotal",m.TravelClaimAmount),
                    new SqlParameter("@ActualDistance", finalDistance),
                    new SqlParameter("@RefVchNo", m.VchNo),
                    new SqlParameter("@HotelOwnArrangements",0)

                };
                DataTable Dt=_lhelp.GetDataTableByProcedure("usp_AttAutoPostExpense", p);
            }
            catch(Exception ex)
            {

            }
        }

        public void PostExpenseVoucherHotel(HotalStay m)
        {
            try
            {
                DBHelper _lhelp = new DBHelper(_strCon);
                
                SqlParameter[] p =
                {
                    new SqlParameter("@BillFromDate",m.StartDate),
                    new SqlParameter("@BillToDate",m.StartDate),
                    new SqlParameter("@UserId",m.CreatedBy),
                    new SqlParameter("@StationCovered", ""),
                    new SqlParameter("@Mode",m.Mode),
                    new SqlParameter("@CategoryID",0),
                    new SqlParameter("@TravelRate",0),
                    new SqlParameter("@TravelTotal",0),
                    new SqlParameter("@ActualDistance", 0),
                    new SqlParameter("@RefVchNo", m.VchNo),
                    new SqlParameter("@HotelOwnArrangements", m.HotelOwnArrangements),
                    new SqlParameter("@Taxi", m.Taki),
                    new SqlParameter("@Auto", m.Auto),
                    new SqlParameter("@HotelAutoTaxiAttachment", m.ImagePath)

                };
                DataTable Dt = _lhelp.GetDataTableByProcedure("usp_AttAutoPostExpense", p);
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}