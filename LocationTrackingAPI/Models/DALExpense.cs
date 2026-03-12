using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace LocationTrackingAPI.Models
{
    public class DALExpense
    {
        string _strCon = string.Empty;
        LogHelper _lLog = null;
        public DALExpense()
        {
            _lLog = new LogHelper("");
            _strCon = ConfigurationManager.AppSettings["ConnStr"].ToString().Trim();
        }
        #region ExpenseMaster
        public DataSet GetExpenseMasterByUserId(int UserId)
        {
            DataSet objs = new DataSet();
            try
            {
                // If month is April (4) or later → FY starts this year
                int fyStartYear = DateTime.Now.Month >= 4 ? DateTime.Now.Year : DateTime.Now.Year - 1;
                int fyEndYear = fyStartYear + 1;

                DateTime fromDate = new DateTime(fyStartYear, 4, 1);     // 01-04-2025
                DateTime toDate = new DateTime(fyEndYear, 3, 31);      // 31-03-2026
                string SDate = fromDate.ToString("yyyy-MM-dd");
                DBHelper _lhelp = new DBHelper(_strCon);
                string Query = @"
                                Declare @UserId int=" + UserId + ", @LedgerId int=0,@DesignationId int=0,@endDate nvarchar(20)=GETDATE();";

                Query+=@" SET @LedgerId=(Select TOP 1 LedgerId from Users_default where User_ID=@UserId);

                                SET @DesignationId = (SElect TOP 1 DesignationID from TourAndTravel_DesignationDetails where LedgerID=@LedgerId)

                                Select B.id as DesignationId, B.DesignName, GETDATE() as VchDate 
                                from TourAndTravel_DesignationDetails A 
                                INNEr JOIN TourAndTravelDesignation B on A.DesignationID=B.id 
                                where DeleteStatus=1 AND LedgerID=@LedgerId

                                Select * from Expence_Parameters order by Label

                                SELECT       
                                    L.LEDGERID,       
         
                                    SWE.ID,       
                                    SWE.StateID,       
                                    EP.Label,       
                                    SWE.Amount,
                                    SWE.ExpenseID as ParameterID        
                                FROM StateWiseExpenseDetails SWE       
                                --LEFT JOIN LOC_State LS ON LS.State_ID = SWE.StateID       
                                LEFT JOIN Ledger_Sub_Group LSG ON LSG.Ledger_Sub_Group_ID=SWE.Ledger_Sub_Group_ID      
                                LEFT JOIN Expence_Parameters EP ON EP.ParameterID = SWE.ExpenseID       
                                INNER JOIN Ledger L ON LSG.Ledger_Sub_Group_ID = L.Ledger_Sub_Group_ID    
                                WHERE L.LEDGERID = @LedgerId
    
                                SELECT TOP 1 
                                    ISNULL(DesignationID, 0) AS DesignationID,
                                    ISNULL(LocalDA, 0) AS LocalDA,
                                    ISNULL(BreakFast, 0) AS BreakFast,
                                    ISNULL(Lunch, 0) AS Lunch,
                                    ISNULL(Dinner, 0) AS Dinner,
                                    ISNULL(Lodging, 0) AS Lodging,
                                    ISNULL(ISNULL(Lodging, 0) * 0.8, 0) AS RoomSharing,
                                    ISNULL(CreatedDate, GETDATE()) AS CreatedDate
                                    FROM(SELECT * FROM DesignWiseRateMaster WHERE DesignationID = @DesignationId and WEF=1) AS t  
                                    RIGHT JOIN(SELECT 0 AS Dummy) AS d ON 1 = 1;";

                Query += " EXEC usp_GetSpecimenLogReportSessionHead @LedgerID=@LedgerId,@sDate='" + SDate + "',@eDate=@endDate,@SessionHeadID=0";
                objs = _lhelp.GetDataSetByExpression(Query);

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

        public DataSet GetExpenseMasterByUserIdAuto(int UserId)
        {
            DataSet objs = new DataSet();
            try
            {
                // If month is April (4) or later → FY starts this year
                int fyStartYear = DateTime.Now.Month >= 4 ? DateTime.Now.Year : DateTime.Now.Year - 1;
                int fyEndYear = fyStartYear + 1;

                DateTime fromDate = new DateTime(fyStartYear, 4, 1);     // 01-04-2025
                DateTime toDate = new DateTime(fyEndYear, 3, 31);      // 31-03-2026
                string SDate = fromDate.ToString("yyyy-MM-dd");
                DBHelper _lhelp = new DBHelper(_strCon);
                string Query = @"
                                Declare @UserId int=" + UserId + ", @LedgerId int=0,@DesignationId int=0,@endDate nvarchar(20)=GETDATE();";

                Query += @" SET @LedgerId=(Select TOP 1 LedgerId from Users_default where User_ID=@UserId);

                                SET @DesignationId = (SElect TOP 1 DesignationID from TourAndTravel_DesignationDetails where LedgerID=@LedgerId)

                                
                                Select * from Expence_Parameters order by Label

                                SELECT       
                                    L.LEDGERID,       
         
                                    SWE.ID,       
                                    SWE.StateID,       
                                    EP.Label,       
                                    SWE.Amount,
                                    SWE.ExpenseID as ParameterID        
                                FROM StateWiseExpenseDetails SWE       
                                --LEFT JOIN LOC_State LS ON LS.State_ID = SWE.StateID       
                                LEFT JOIN Ledger_Sub_Group LSG ON LSG.Ledger_Sub_Group_ID=SWE.Ledger_Sub_Group_ID      
                                LEFT JOIN Expence_Parameters EP ON EP.ParameterID = SWE.ExpenseID       
                                INNER JOIN Ledger L ON LSG.Ledger_Sub_Group_ID = L.Ledger_Sub_Group_ID    
                                WHERE L.LEDGERID = @LedgerId";
    
                                
                objs = _lhelp.GetDataSetByExpression(Query);

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

        public int InsertExpenses(AddExpense expObj)
        {
            int CompanyID = 1;

            

            //if (expObj.UserID == 1000)
            //{
            //    DataTable dt = RSPLCommonDAL.GetData("Select TOP 1 User_ID from Teacher_ledger where LedgerID=" + expObj.LedgerID);

            //    if (dt != null && dt.Rows.Count > 0)
            //        expObj.UserID = Convert.ToInt32(dt.Rows[0]["User_ID"]);
            //    else
            //        expObj.UserID = Convert.ToInt32(RSPLCommonDAL.GetData("Select TOP 1 Name, User_ID from Users where Name='" + expObj.LedgerName.Trim() + "'").Rows[0]["User_ID"]);
            //    //Convert.ToInt32(dt.Rows[0][0]);
            //}

            int result = 0;
            
            try
            {
                


                DBHelper _lhelp = new DBHelper(_strCon);
                DataTable dtLedger = _lhelp.GetDataTableByExpression("Select TOP 1 LedgerID from Users_default where [User_ID]='" + expObj.UserID + "' ");
                if (dtLedger != null && dtLedger.Rows.Count > 0)
                    expObj.LedgerID = Convert.ToInt32(dtLedger.Rows[0]["LedgerID"]);

                //Check for Duplicate range
                DataTable dtCheck = _lhelp.GetDataTableByExpression(@"Select top 1 ExpenseID from TourAndTravelExpenseDetails where Convert(date,BillFromDate) between '"+ Convert.ToDateTime(expObj.BillFromDate).ToString("yyyy-MM-dd") + "' and '" + Convert.ToDateTime(expObj.BillToDate).ToString("yyyy-MM-dd") + "' AND LedgerID='"+ expObj.LedgerID + "'");
                if(dtCheck!=null && dtCheck.Rows.Count>0 && expObj.IsEdit==0)
                {
                    result = -1;
                    return result;
                }

                expObj.wefDate = DateTime.Now.ToString("yyyy-MM-dd");
                using (SqlConnection sConn = new SqlConnection(_strCon))
                {
                    sConn.Open();
                    SqlCommand cmd = new SqlCommand("usp_CrudAcceptExpenses2", sConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("ExpenseID", expObj.ExpenseID);
                    cmd.Parameters.AddWithValue("CompanyID", CompanyID);
                    cmd.Parameters.AddWithValue("UserID", expObj.UserID);
                    cmd.Parameters.AddWithValue("LedgerID", expObj.LedgerID);
                    cmd.Parameters.AddWithValue("SubmissionDate", expObj.wefDate);
                    cmd.Parameters.AddWithValue("Approved", 1);
                    cmd.Parameters.AddWithValue("BillFromDate", Convert.ToDateTime(expObj.BillFromDate).ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("BillToDate", Convert.ToDateTime(expObj.BillToDate).ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("Description", string.IsNullOrEmpty(expObj.Description) ? "" : expObj.Description);
                    cmd.Parameters.AddWithValue("StationCovered", string.IsNullOrEmpty(expObj.StationCovered) ? "" : expObj.StationCovered);


                    result = cmd.ExecuteNonQuery();
                    sConn.Close();
                    return result;
                }

            }
            catch (Exception ex)
            {
                var errorDetails = new
                {
                    Message = ex.Message,
                    Source = ex.Source,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                    LineNumber = new System.Diagnostics.StackTrace(ex, true)
                                     .GetFrame(0)?
                                     .GetFileLineNumber()
                };
                _lLog.WriteFileToLocal(Newtonsoft.Json.JsonConvert.SerializeObject(errorDetails));
                string strMsg = ex.Message;
                return result;
            }
        }

        public int InsertOtherExp(AddExpense ExpObj)
        {
            int UserType = 1;
            int result = 0;
            //var otherExpenses = JsonConvert.DeserializeObject<List<OtherExpenseDetails>>(otherExpArray);
            int ExpenseID = 0;

            DBHelper _lhelp = new DBHelper(_strCon);

            if (ExpObj.IsEdit == 0)
            {
                DataTable ExpenseDT = _lhelp.GetDataTableByExpression("Select TOP 1 ExpenseID from TourAndTravelExpenseDetails where LedgerID=" + ExpObj.LedgerID + " order by SubmissionDate Desc");
                if (ExpenseDT.Rows.Count > 0)
                {
                    ExpenseID = Convert.ToInt32(ExpenseDT.Rows[0]["expenseid"]);
                }
            }
            else
            {
                ExpenseID = ExpObj.ExpenseID;
            }


            /**************************************************************************************Other Expense Details******************************************/
            #region OtherExpense

            string folderPathOtherExp = HttpContext.Current.Server.MapPath("~/Uploads/");

            if (ExpObj.OtherExpenseDetails != null && ExpObj.OtherExpenseDetails.Count > 0)
            {
                foreach (var expense in ExpObj.OtherExpenseDetails)
                {
                    /*Photocopy Attachements*/
                    string PhotoCopyAttchfileName = string.Empty;
                    string PhotoCopyAttchmentFileName = string.Empty;
                    if (expense.PhotoCopyAttach != null)
                    {
                        var file = expense.PhotoCopyAttach;
                        if (file != null && file.ContentLength > 0)
                        {
                            string originalFileName = Path.GetFileName(file.FileName);
                            string Extenstion = Path.GetExtension(originalFileName);
                            string dateTimeWithMilliseconds = DateTime.Now.ToString("yyyy-MM-dd'T'HHmmss.fff");
                            PhotoCopyAttchfileName = "PhotoCopyAttch_" + dateTimeWithMilliseconds + originalFileName;
                            //PhotoCopyAttchmentFileName = $"{PhotoCopyAttchfileName}.{Extenstion}";
                            string fullPath = Path.Combine(folderPathOtherExp, PhotoCopyAttchfileName);
                            file.SaveAs(fullPath);
                        }

                    }
                    else
                    {
                        PhotoCopyAttchfileName = expense.strPhotoCopyAttach;
                    }
                    /*OthExpAttch Attachements*/
                    string OthExpAttchAttchfileName = string.Empty;
                    string OthExpAttchAttchmentFileName = string.Empty;
                    if (expense.OthExpAttch != null)
                    {
                        var file = expense.OthExpAttch;
                        if (file != null && file.ContentLength > 0)
                        {
                            string originalFileName = Path.GetFileName(file.FileName);
                            string Extenstion = Path.GetExtension(originalFileName);
                            string dateTimeWithMilliseconds = DateTime.Now.ToString("yyyy-MM-dd'T'HHmmss.fff");
                            OthExpAttchAttchfileName = "OthExp_" + dateTimeWithMilliseconds + originalFileName;
                            // OthExpAttchAttchmentFileName = $"{OthExpAttchAttchfileName}_{Extenstion}";
                            string fullPath = Path.Combine(folderPathOtherExp, OthExpAttchAttchfileName);
                            file.SaveAs(fullPath);
                        }

                    }
                    else
                    {
                        OthExpAttchAttchfileName = expense.strOthExpAttch;
                    }
                    /*CourierExpAttch Attachements*/
                    string CourierExpAttchAttchfileName = string.Empty;
                    string CourierExpAttchAttchmentFileName = string.Empty;
                    if (expense.CourierExpAttch != null)
                    {
                        var file = expense.CourierExpAttch;
                        if (file != null && file.ContentLength > 0)
                        {
                            string originalFileName = Path.GetFileName(file.FileName);
                            string Extenstion = Path.GetExtension(originalFileName);
                            string dateTimeWithMilliseconds = DateTime.Now.ToString("yyyy-MM-dd'T'HHmmss.fff");
                            CourierExpAttchAttchfileName = "CourierExp_" + dateTimeWithMilliseconds + originalFileName;
                            //CourierExpAttchAttchmentFileName = $"{CourierExpAttchAttchfileName}_{Extenstion}";
                            string fullPath = Path.Combine(folderPathOtherExp, CourierExpAttchAttchfileName);
                            file.SaveAs(fullPath);
                        }

                    }
                    else
                    {
                        CourierExpAttchAttchfileName = expense.strOthExpAttch;
                    }
                    /*CourierExpAttch Attachements*/
                    string MiscExpAttchAttchfileName = string.Empty;
                    string MiscExpAttchAttchmentFileName = string.Empty;
                    if (expense.MiscExpAttch != null)
                    {
                        var file = expense.MiscExpAttch;
                        if (file != null && file.ContentLength > 0)
                        {
                            string originalFileName = Path.GetFileName(file.FileName);
                            string Extenstion = Path.GetExtension(originalFileName);
                            string dateTimeWithMilliseconds = DateTime.Now.ToString("yyyy-MM-dd'T'HHmmss.fff");
                            MiscExpAttchAttchfileName = "MiscExp_" + dateTimeWithMilliseconds + originalFileName;
                            //MiscExpAttchAttchmentFileName = $"{MiscExpAttchAttchfileName}_{Extenstion}";
                            string fullPath = Path.Combine(folderPathOtherExp, MiscExpAttchAttchfileName);
                            file.SaveAs(fullPath);
                        }

                    }
                    else
                    {
                        MiscExpAttchAttchfileName = expense.strMiscExpAttch;
                    }


                    using (var connection = new SqlConnection(_strCon))
                    {
                        if (ExpObj.IsEdit == 0)//add new
                        {
                            string query = "INSERT INTO TourAndTravel_OtherExpdetails (ExpenseID,PhotoCopyExp,OtherExp,CourierFaxSTDExp,MiscExp,Remarks,PhotoCopyAttach,OthExpAttch,CourierExpAttch,MiscExpAttch,Date) " +
                                       "VALUES (@ExpenseID,@PhotoCopyExp, @OtherExp, @CourierFaxSTDExp, @MiscExp, @Remarks,@PhotoCopyAttach,@OthExpAttch,@CourierExpAttch,@MiscExpAttch,@Date)";
                            if (ExpenseID == 0)
                                continue;

                            var command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@ExpenseID", ExpenseID);
                            command.Parameters.AddWithValue("@PhotoCopyExp", expense.PhotoCopyExp);
                            command.Parameters.AddWithValue("@OtherExp", expense.OtherExp);
                            command.Parameters.AddWithValue("@CourierFaxSTDExp", expense.CourierFaxSTDExp);
                            command.Parameters.AddWithValue("@MiscExp", expense.MiscExp);
                            command.Parameters.AddWithValue("@Remarks", string.IsNullOrWhiteSpace(expense.Remarks) ? (object)DBNull.Value : expense.Remarks);
                            command.Parameters.AddWithValue("@PhotoCopyAttach", PhotoCopyAttchfileName);
                            command.Parameters.AddWithValue("@OthExpAttch", OthExpAttchAttchfileName);
                            command.Parameters.AddWithValue("@CourierExpAttch", CourierExpAttchAttchfileName);
                            command.Parameters.AddWithValue("@MiscExpAttch", MiscExpAttchAttchfileName);
                            command.Parameters.AddWithValue("@Date", expense.Date);
                            connection.Open();
                            result = command.ExecuteNonQuery();
                        }
                        else//Update
                        {
                            string query = "if EXISTS(Select top 1 OtherExpID from TourAndTravel_OtherExpdetails where OtherExpID<>0 and OtherExpID='" + expense.OtherExpID + "')";
                            query += " BEGIN ";
                            query += " Update TourAndTravel_OtherExpdetails SET ";
                            query += "PhotoCopyExp = '" + expense.PhotoCopyExp + "',";
                            query += "OtherExp='" + expense.OtherExp + "',";
                            query += "CourierFaxSTDExp='" + expense.CourierFaxSTDExp + "',";
                            query += "MiscExp='" + expense.MiscExp + "',";
                            query += "Date='" + expense.Date + "',";

                            if (PhotoCopyAttchfileName != null && PhotoCopyAttchfileName.ToString() != "")
                                query += "PhotoCopyAttach='" + PhotoCopyAttchfileName + "',";

                            if (OthExpAttchAttchfileName != null && OthExpAttchAttchfileName.ToString() != "")
                                query += "OthExpAttch='" + OthExpAttchAttchfileName + "',";
                            if (CourierExpAttchAttchfileName != null && CourierExpAttchAttchfileName.ToString() != "")
                                query += "CourierExpAttch='" + CourierExpAttchAttchfileName + "',";
                            if (MiscExpAttchAttchfileName != null && MiscExpAttchAttchfileName.ToString() != "")
                                query += "MiscExpAttch='" + MiscExpAttchAttchfileName + "',";

                            query += "Remarks='" + (string.IsNullOrEmpty(expense.Remarks) ? (object)DBNull.Value : expense.Remarks) + "'";
                            query += " Where OtherExpID='" + expense.OtherExpID + "'";
                            query += " END";
                            query += " ELSE";
                            query += " BEGIN ";
                            query += " INSERT INTO TourAndTravel_OtherExpdetails (ExpenseID,PhotoCopyExp,OtherExp,CourierFaxSTDExp,MiscExp,Remarks,PhotoCopyAttach,OthExpAttch,CourierExpAttch,MiscExpAttch,Date) " +
                                   " VALUES ('" + ExpenseID + "','" + expense.PhotoCopyExp + "', '" + expense.OtherExp + "', '" + expense.CourierFaxSTDExp + "', '" + expense.MiscExp + "', '" + (string.IsNullOrEmpty(expense.Remarks) ? (object)DBNull.Value : expense.Remarks) + "','" + PhotoCopyAttchfileName + "','" + OthExpAttchAttchfileName + "','" + CourierExpAttchAttchfileName + "','" + MiscExpAttchAttchfileName + "','" + expense.Date + "')";
                            query += " END";


                            var command = new SqlCommand(query, connection);
                            connection.Open();
                            command.CommandType = CommandType.Text;
                            result = command.ExecuteNonQuery();
                            connection.Close();

                        }

                    }




                }
            }
            #endregion
            /**************************************************************************************Journey Expense Details******************************************/
            #region JourneyExpense
            if (ExpObj.JourneyExpenseDetails != null && ExpObj.JourneyExpenseDetails.Count > 0)
            {


                foreach (var expense in ExpObj.JourneyExpenseDetails)
                {


                    string folderPathJourneyExp = HttpContext.Current.Server.MapPath("~/Uploads/");

                    string JourneyAttchfileName = string.Empty;
                    string JourneyAttchmentFileName = string.Empty;
                    if (expense.JourneyAttach != null)
                    {
                        var file = expense.JourneyAttach;
                        if (file != null && file.ContentLength > 0)
                        {
                            string originalFileName = Path.GetFileName(file.FileName);
                            string Extenstion = Path.GetExtension(originalFileName);
                            string dateTimeWithMilliseconds = DateTime.Now.ToString("yyyy-MM-dd'T'HHmmss.fff");
                            JourneyAttchfileName = "JourneyAttch_" + dateTimeWithMilliseconds + originalFileName;
                            string fullPath = Path.Combine(folderPathJourneyExp, JourneyAttchfileName);
                            file.SaveAs(fullPath);
                        }

                    }
                    else
                    {
                        JourneyAttchfileName = expense.strJourneyAttach;
                    }


                    using (var connection = new SqlConnection(_strCon))
                    {
                        if (ExpObj.IsEdit == 0)
                        {
                            if (ExpenseID == 0)
                                continue;

                            string query = "INSERT INTO TourAndTravel_JourneyExpdetails (ExpenseID,Date,FromStation,ToStation,Fare,Ticket,Remarks,JourneyAttach) " +
                                       "VALUES (@ExpenseID,@Date,@FromStation,@ToStation,@Fare,@Ticket,@Remarks,@JourneyAttach)";

                            var command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@ExpenseID", ExpenseID);
                            //command.Parameters.AddWithValue("@SNO", expense.SNO);
                            command.Parameters.AddWithValue("@Date", expense.Date);
                            command.Parameters.AddWithValue("@FromStation", string.IsNullOrWhiteSpace(expense.FromStation) ? (object)DBNull.Value : expense.FromStation);
                            command.Parameters.AddWithValue("@ToStation", string.IsNullOrWhiteSpace(expense.ToStation) ? (object)DBNull.Value : expense.ToStation);
                            command.Parameters.AddWithValue("@Fare", expense.Fare);
                            command.Parameters.AddWithValue("@Ticket", string.IsNullOrWhiteSpace(expense.Ticket) ? (object)DBNull.Value : expense.Ticket);
                            command.Parameters.AddWithValue("@Remarks", string.IsNullOrWhiteSpace(expense.Remarks) ? (object)DBNull.Value : expense.Remarks);
                            command.Parameters.AddWithValue("@JourneyAttach", JourneyAttchfileName);
                            connection.Open();
                            result = command.ExecuteNonQuery();
                        }
                        else//Update
                        {
                            string query = "if EXISTS(Select top 1 JourneyExpID from TourAndTravel_JourneyExpdetails where JourneyExpID<>0 and JourneyExpID=" + Convert.ToInt32(expense.JourneyExpID) + ")";
                            query += " BEGIN ";
                            query += "Update TourAndTravel_JourneyExpdetails SET ";

                            //query += "SNO = '" + expense.SNO + "',";
                            query += "Date='" + expense.Date + "',";
                            query += "FromStation='" + expense.FromStation + "',";
                            query += "ToStation='" + expense.ToStation + "',";
                            query += "Fare='" + expense.Fare + "',";
                            query += "Ticket='" + expense.Ticket + "',";



                            if (JourneyAttchfileName != null && JourneyAttchfileName.ToString() != "")
                                query += "JourneyAttach='" + JourneyAttchfileName + "',";



                            query += "Remarks='" + (string.IsNullOrEmpty(expense.Remarks) ? (object)DBNull.Value : expense.Remarks) + "'";
                            query += " Where JourneyExpID=" + Convert.ToInt32(expense.JourneyExpID) + "";
                            query += " END";
                            query += " ELSE";
                            query += " BEGIN ";
                            query += " INSERT INTO TourAndTravel_JourneyExpdetails (ExpenseID,Date,FromStation,ToStation,Fare,Ticket,Remarks,JourneyAttach) " +
                                       "VALUES ('" + ExpenseID + "','" + expense.Date + "','" + expense.FromStation + "','" + expense.ToStation + "','" + expense.Fare + "','" + expense.Ticket + "','" + (string.IsNullOrEmpty(expense.Remarks) ? (object)DBNull.Value : expense.Remarks) + "','" + JourneyAttchfileName + "')";
                            query += " END";

                            var command = new SqlCommand(query, connection);
                            connection.Open();
                            command.CommandType = CommandType.Text;
                            result = command.ExecuteNonQuery();
                            connection.Close();

                        }

                    }




                }
            }

            #endregion
            /**************************************************************************************Transport Expense Details******************************************/
            #region TransportExpense
            if (ExpObj.TransportExpenseDetails != null && ExpObj.TransportExpenseDetails.Count > 0)
            {
                foreach (var expense in ExpObj.TransportExpenseDetails)
                {


                    string folderPathTransportExp = HttpContext.Current.Server.MapPath("~/Uploads/");

                    string TransportAttchfileName = string.Empty;
                    if (expense.TransPortAttach != null)
                    {
                        var file = expense.TransPortAttach;
                        if (file != null && file.ContentLength > 0)
                        {
                            string originalFileName = Path.GetFileName(file.FileName);
                            string Extenstion = Path.GetExtension(originalFileName);
                            string dateTimeWithMilliseconds = DateTime.Now.ToString("yyyy-MM-dd'T'HHmmss.fff");
                            TransportAttchfileName = "TransportAttch_" + dateTimeWithMilliseconds + originalFileName;
                            string fullPath = Path.Combine(folderPathTransportExp, TransportAttchfileName);
                            file.SaveAs(fullPath);
                        }

                    }
                    else
                    {
                        TransportAttchfileName = expense.strTransPortAttach;
                    }

                    using (var connection = new SqlConnection(_strCon))
                    {
                        if (ExpObj.IsEdit == 0)
                        {
                            if (ExpenseID == 0)
                                continue;

                            string query = "INSERT INTO TourAndTravel_TransportExpdetails (ExpenseID,Station,ChallanNo,BuiltyNo,TransportName,NoofBDLS,Cartridge,Amount,Remarks,TransPortAttach,Date) " +
                                       "VALUES (@ExpenseID,  @Station, @ChallanNo, @TPTBuiltyNo, @TransName, @NoofBDLS,@Cartridge, @Amount,@Remarks, @TransPortAttach,@Date)";

                            var command = new SqlCommand(query, connection);

                            command.Parameters.AddWithValue("@ExpenseID", ExpenseID == 0 ? (object)DBNull.Value : ExpenseID);
                            //command.Parameters.AddWithValue("@SNO", expense.SNO);
                            command.Parameters.AddWithValue("@Station", string.IsNullOrEmpty(expense.Station) ? (object)DBNull.Value : expense.Station);
                            command.Parameters.AddWithValue("@Date", expense.Date);
                            command.Parameters.AddWithValue("@ChallanNo", string.IsNullOrEmpty(expense.ChallanNo) ? (object)DBNull.Value : expense.ChallanNo);
                            command.Parameters.AddWithValue("@TPTBuiltyNo", string.IsNullOrEmpty(expense.BuiltyNo) ? (object)DBNull.Value : expense.BuiltyNo);
                            command.Parameters.AddWithValue("@TransName", string.IsNullOrEmpty(expense.TransportName) ? (object)DBNull.Value : expense.TransportName);
                            command.Parameters.AddWithValue("@NoofBDLS", expense.NoofBDLS);
                            command.Parameters.AddWithValue("@Cartridge", expense.Cartridge);
                            command.Parameters.AddWithValue("@Amount", expense.Amount);
                            command.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(expense.Remarks) ? (object)DBNull.Value : expense.Remarks);
                            command.Parameters.AddWithValue("@TransPortAttach", string.IsNullOrEmpty(TransportAttchfileName) ? (object)DBNull.Value : TransportAttchfileName);
                            connection.Open();
                            result = command.ExecuteNonQuery();
                        }
                        else//Update
                        {
                            string query = "if EXISTS(Select top 1 TransportExpID from TourAndTravel_TransportExpdetails where TransportExpID<>0 and TransportExpID='" + expense.TransportExpID + "')";
                            query += " BEGIN ";
                            query += "Update TourAndTravel_TransportExpdetails SET ";

                            //query += "SNO = '" + expense.SNO + "',";
                            query += "Station='" + expense.Station + "',";
                            query += "ChallanNo='" + expense.ChallanNo + "',";
                            query += "BuiltyNo='" + expense.BuiltyNo + "',";
                            query += "TransportName='" + expense.TransportName + "',";
                            query += "NoofBDLS='" + expense.NoofBDLS + "',";
                            query += "Cartridge='" + expense.Cartridge + "',";
                            query += "Date='" + expense.Date + "',";





                            query += "Amount='" + expense.Amount + "',";




                            if (TransportAttchfileName != null && TransportAttchfileName.ToString() != "")
                                query += "TransPortAttach='" + TransportAttchfileName + "',";

                            query += "Remarks='" + (string.IsNullOrEmpty(expense.Remarks) ? (object)DBNull.Value : expense.Remarks) + "'";


                            query += " Where TransportExpID='" + expense.TransportExpID + "'";
                            query += " END";
                            query += " ELSE";
                            query += " BEGIN ";
                            query += " INSERT INTO TourAndTravel_TransportExpdetails (ExpenseID,Station,ChallanNo,BuiltyNo,TransportName,NoofBDLS,Cartridge,Amount,Remarks,TransPortAttach,Date) " +
                                       "VALUES ('" + ExpenseID + "', '" + expense.Station + "', '" + expense.ChallanNo + "', '" + expense.BuiltyNo + "','" + expense.TransportName + "', '" + expense.NoofBDLS + "', '" + expense.Cartridge + "', '" + expense.Amount + "', '" + (string.IsNullOrEmpty(expense.Remarks) ? (object)DBNull.Value : expense.Remarks) + "', '" + TransportAttchfileName + "','" + expense.Date + "')";
                            query += " END";


                            var command = new SqlCommand(query, connection);
                            connection.Open();
                            command.CommandType = CommandType.Text;
                            result = command.ExecuteNonQuery();
                            connection.Close();

                        }


                    }



                }
            }

            #endregion
            /**************************************************************************************Toll Expense Details******************************************/
            #region TollportExpense
            if (ExpObj.TollExpenseDetails != null && ExpObj.TollExpenseDetails.Count > 0)
            {


                foreach (var expense in ExpObj.TollExpenseDetails)
                {


                    string folderPathTollExp = HttpContext.Current.Server.MapPath("~/Uploads/");

                    string TollAttchfileName = string.Empty;
                    if (expense.TollAttachment != null)
                    {
                        var file = expense.TollAttachment;
                        if (file != null && file.ContentLength > 0)
                        {
                            string originalFileName = Path.GetFileName(file.FileName);
                            string Extenstion = Path.GetExtension(originalFileName);
                            string dateTimeWithMilliseconds = DateTime.Now.ToString("yyyy-MM-dd'T'HHmmss.fff");
                            TollAttchfileName = "TollExpAttch_" + dateTimeWithMilliseconds + originalFileName;
                            string fullPath = Path.Combine(folderPathTollExp, TollAttchfileName);
                            file.SaveAs(fullPath);
                        }

                    }
                    else
                    {
                        TollAttchfileName = expense.strTollAttachment;
                    }


                    using (var connection = new SqlConnection(_strCon))
                    {
                        if (ExpObj.IsEdit == 0)
                        {
                            if (ExpenseID == 0)
                                continue;

                            string query = "INSERT INTO TourAndTravel_TollExpdetails (ExpenseID,TransactionID,Date,TollPlaza,Time,Amount,TollAttachment) " +
                                       "VALUES (@ExpenseID, @TransactionID,  @Date, @TollPlaza, @Time, @Amount, @TollAttachment)";

                            var command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@ExpenseID", ExpenseID);
                            //command.Parameters.AddWithValue("@SNO", expense.SNO);
                            command.Parameters.AddWithValue("@TransactionID", expense.TransactionID);
                            command.Parameters.AddWithValue("@Date", expense.Date);
                            command.Parameters.AddWithValue("@TollPlaza", string.IsNullOrEmpty(expense.TollPlaza) ? (object)DBNull.Value : expense.TollPlaza);
                            command.Parameters.AddWithValue("@Time", string.IsNullOrEmpty(expense.Time) ? (object)DBNull.Value : expense.Time);
                            command.Parameters.AddWithValue("@Amount", expense.Amount);
                            command.Parameters.AddWithValue("@TollAttachment", TollAttchfileName);
                            connection.Open();
                            result = command.ExecuteNonQuery();
                        }
                        else//Update
                        {
                            string query = "if EXISTS(Select top 1 TollExpID from TourAndTravel_TollExpdetails where TollExpID<>0 and TollExpID='" + expense.TollExpID + "')";
                            query += " BEGIN ";
                            query += " Update TourAndTravel_TollExpdetails SET ";
                            //if (expense.PhotoCopyExp != null || expense.PhotoCopyExp.ToString() != "")
                            //query += "SNO = '" + expense.SNO + "',";
                            query += "TransactionID='" + expense.TransactionID + "',";
                            query += "Date='" + expense.Date + "',";
                            query += "TollPlaza='" + expense.TollPlaza + "',";
                            query += "Time='" + (string.IsNullOrEmpty(expense.Time) ? (object)DBNull.Value : expense.Time) + "',";

                            if (TollAttchfileName != null && TollAttchfileName.ToString() != "")
                                query += "TollAttachment='" + TollAttchfileName + "',";

                            query += "Amount='" + expense.Amount + "'";
                            query += " Where TollExpID='" + expense.TollExpID + "'";
                            query += " END";
                            query += " ELSE";
                            query += " BEGIN ";
                            query += " INSERT INTO TourAndTravel_TollExpdetails (ExpenseID,TransactionID,Date,TollPlaza,Time,Amount,TollAttachment) " +
                                       "VALUES ('" + ExpenseID + "', '" + expense.TransactionID + "',  '" + expense.Date + "', '" + expense.TollPlaza + "', '" + (string.IsNullOrEmpty(expense.Time) ? (object)DBNull.Value : expense.Time) + "', '" + expense.Amount + "', '" + TollAttchfileName + "')";
                            query += " END";

                            var command = new SqlCommand(query, connection);
                            connection.Open();
                            command.CommandType = CommandType.Text;
                            result = command.ExecuteNonQuery();
                            connection.Close();

                        }

                    }

                }
            }
            #endregion
            /**************************************************************************************Journey Accomodation Details******************************************/
            #region JourneyAccomExpense

            string folderPathJourneyAccomExp = HttpContext.Current.Server.MapPath("~/Uploads/");

            if (ExpObj.JourneyAccomExpenseDetails != null && ExpObj.JourneyAccomExpenseDetails.Count > 0)
            {
                foreach (var expense in ExpObj.JourneyAccomExpenseDetails)
                {
                    /*Photocopy Attachements*/
                    string LodingAttachmentfileName = string.Empty;
                    string PhotoCopyAttchmentFileName = string.Empty;
                    if (expense.LodingAttachment != null)
                    {
                        var file = expense.LodingAttachment;
                        if (file != null && file.ContentLength > 0)
                        {
                            string originalFileName = Path.GetFileName(file.FileName);
                            string Extenstion = Path.GetExtension(originalFileName);
                            string dateTimeWithMilliseconds = DateTime.Now.ToString("yyyy-MM-dd'T'HHmmss.fff");
                            LodingAttachmentfileName = "LodingAttachment_" + dateTimeWithMilliseconds + originalFileName;
                            PhotoCopyAttchmentFileName = $"{PhotoCopyAttchmentFileName}.{Extenstion}";
                            string fullPath = Path.Combine(folderPathJourneyAccomExp, LodingAttachmentfileName);
                            file.SaveAs(fullPath);
                        }

                    }
                    else
                    {
                        LodingAttachmentfileName = expense.strLodingAttachment;
                    }



                    using (var connection = new SqlConnection(_strCon))
                    {
                        if (ExpObj.IsEdit == 0)//add new
                        {
                            string query = "INSERT INTO TourAndTravel_JourneyAccomExpDetails (ExpenseID, Date, StartTime, EndTime, LocalDA, Breakfast, Lunch, Dinner, Lodging, LodingAttachment, Remarks,RoomSharing) " +
                                       "VALUES (@ExpenseID, @Date, @StartTime, @EndTime, @LocalDA, @Breakfast, @Lunch, @Dinner, @Lodging, @LodingAttachment, @Remarks,@RoomSharing)";
                            if (ExpenseID == 0)
                                continue;

                            var command = new SqlCommand(query, connection);

                            // ExpenseID: if it's 0, treat it as null
                            command.Parameters.AddWithValue("@ExpenseID", ExpenseID == 0 ? (object)DBNull.Value : ExpenseID);

                            // Date: if it's DateTime.MinValue or nullable and null, treat as null
                            command.Parameters.AddWithValue("@Date", expense.Date);
                            // Time fields: check for null/empty
                            command.Parameters.AddWithValue("@StartTime", string.IsNullOrEmpty(expense.StartTime) ? (object)DBNull.Value : expense.StartTime);
                            command.Parameters.AddWithValue("@EndTime", string.IsNullOrEmpty(expense.EndTime) ? (object)DBNull.Value : expense.EndTime);

                            // Numeric fields: treat 0 as null (optional - depends on your business logic)
                            command.Parameters.AddWithValue("@LocalDA", expense.LocalDA);
                            command.Parameters.AddWithValue("@Breakfast", expense.Breakfast);
                            command.Parameters.AddWithValue("@Lunch", expense.Lunch);
                            command.Parameters.AddWithValue("@Dinner", expense.Dinner);
                            command.Parameters.AddWithValue("@Lodging", expense.Lodging);

                            // File name and remarks
                            command.Parameters.AddWithValue("@LodingAttachment", string.IsNullOrEmpty(LodingAttachmentfileName) ? (object)DBNull.Value : LodingAttachmentfileName);
                            command.Parameters.AddWithValue("@Remarks", string.IsNullOrWhiteSpace(expense.Remarks) ? (object)DBNull.Value : expense.Remarks);
                            command.Parameters.AddWithValue("@RoomSharing", expense.RoomSharing);

                            connection.Open();
                            result = command.ExecuteNonQuery();

                        }
                        else//Update
                        {
                            string query = "if EXISTS(Select top 1 JourneyAccomExpID from TourAndTravel_JourneyAccomExpDetails where JourneyAccomExpID<>0 and JourneyAccomExpID='" + expense.JourneyAccomExpID + "')";
                            query += " BEGIN ";
                            query += " Update TourAndTravel_JourneyAccomExpDetails SET ";
                            query += "Date = '" + expense.Date + "',";
                            query += "StartTime='" + (string.IsNullOrEmpty(expense.StartTime) ? (object)DBNull.Value : expense.StartTime) + "',";
                            query += "EndTime='" + (string.IsNullOrEmpty(expense.EndTime) ? (object)DBNull.Value : expense.EndTime) + "',";
                            query += "LocalDA='" + expense.LocalDA + "',";
                            query += "Breakfast='" + expense.Breakfast + "',";
                            query += "Lunch='" + expense.Lunch + "',";
                            query += "Dinner='" + expense.Dinner + "',";
                            query += "Lodging='" + expense.Lodging + "',";
                            query += "RoomSharing='" + expense.RoomSharing + "',";


                            if (LodingAttachmentfileName != null && LodingAttachmentfileName.ToString() != "")
                                query += "LodingAttachment='" + LodingAttachmentfileName + "',";



                            query += "Remarks='" + (string.IsNullOrWhiteSpace(expense.Remarks) ? (object)DBNull.Value : expense.Remarks) + "'";
                            query += " Where JourneyAccomExpID='" + expense.JourneyAccomExpID + "'";
                            query += " END";
                            query += " ELSE";
                            query += " BEGIN ";
                            query += " INSERT INTO TourAndTravel_JourneyAccomExpDetails (ExpenseID, Date, StartTime, EndTime, LocalDA, Breakfast, Lunch, Dinner, Lodging, LodingAttachment, Remarks,RoomSharing) " +
                                   " VALUES ('" + ExpenseID + "','" + expense.Date + "', '" + (string.IsNullOrEmpty(expense.StartTime) ? (object)DBNull.Value : expense.StartTime) + "', '" + (string.IsNullOrEmpty(expense.EndTime) ? (object)DBNull.Value : expense.EndTime) + "', '" + expense.LocalDA + "', '" + expense.Breakfast + "','" + expense.Lunch + "','" + expense.Dinner + "','" + expense.Lodging + "','" + LodingAttachmentfileName + "','" + (string.IsNullOrEmpty(expense.Remarks) ? (object)DBNull.Value : expense.Remarks) + "','" + expense.RoomSharing + "')";
                            query += " END";


                            var command = new SqlCommand(query, connection);
                            connection.Open();
                            command.CommandType = CommandType.Text;
                            result = command.ExecuteNonQuery();
                            connection.Close();

                        }

                    }




                }
            }
            #endregion
            /**************************************************************************************Travel Expense Details******************************************/
            #region TravelExpense
            if (ExpObj.TravelExpenseDetails != null && ExpObj.TravelExpenseDetails.Count > 0)
            {
                foreach (var expense in ExpObj.TravelExpenseDetails)
                {




                    using (var connection = new SqlConnection(_strCon))
                    {
                        if (ExpObj.IsEdit == 0)
                        {
                            if (ExpenseID == 0)
                                continue;

                            string query = "INSERT INTO TourAndTravel_TravelExpenseDetails (ExpenseID,Date,CategoryID,UnitName,Rate,MeterReadingFrom,[To],Total,TotalPass,VehicleNumber) " +
                                       "VALUES (@ExpenseID, @Date, @CategoryID, @UnitName, @Rate, @MeterReadingFrom, @To, @Total,@TotalPass,@VehicleNumber)";

                            var command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@ExpenseID", ExpenseID);
                            command.Parameters.AddWithValue("@Date", expense.Date);
                            command.Parameters.AddWithValue("@CategoryID", expense.CategoryID);
                            command.Parameters.AddWithValue("@UnitName", string.IsNullOrEmpty(expense.UnitName) ? (object)DBNull.Value : expense.UnitName);
                            command.Parameters.AddWithValue("@Rate", expense.Rate);
                            command.Parameters.AddWithValue("@MeterReadingFrom", expense.MeterReadingFrom);
                            command.Parameters.AddWithValue("@To", expense.To);
                            command.Parameters.AddWithValue("@Total", expense.Total == 0 ? (object)DBNull.Value : expense.Total);
                            command.Parameters.AddWithValue("@TotalPass", expense.TotalPass == 0 ? (object)DBNull.Value : expense.TotalPass);
                            command.Parameters.AddWithValue("@VehicleNumber", string.IsNullOrEmpty(expense.VehicleNumber) ? (object)DBNull.Value : expense.VehicleNumber);
                            connection.Open();
                            result = command.ExecuteNonQuery();
                        }
                        else//Update
                        {
                            string query = "if EXISTS(Select top 1 TravelExpenseID from TourAndTravel_TravelExpenseDetails where TravelExpenseID<>0 and TravelExpenseID='" + expense.TravelExpenseID + "')";
                            query += " BEGIN ";
                            query += "Update TourAndTravel_TravelExpenseDetails SET ";

                            query += "Date = '" + expense.Date + "',";
                            query += "CategoryID='" + expense.CategoryID + "',";
                            query += "UnitName='" + expense.UnitName + "',";
                            query += "VehicleNumber='" + expense.VehicleNumber + "',";
                            query += "Rate='" + expense.Rate + "',";
                            query += "MeterReadingFrom='" + expense.MeterReadingFrom + "',";
                            query += "[To]='" + expense.To + "',";

                            query += "Total='" + expense.Total + "',";
                            query += "TotalPass='" + expense.TotalPass + "'";
                            query += " Where TravelExpenseID='" + expense.TravelExpenseID + "'";
                            query += " END";
                            query += " ELSE";
                            query += " BEGIN ";
                            query += " INSERT INTO TourAndTravel_TravelExpenseDetails (ExpenseID,Date,CategoryID,UnitName,Rate,MeterReadingFrom,[To],Total,TotalPass,VehicleNumber) " +
                                       "VALUES ('" + ExpenseID + "', '" + expense.Date + "', '" + expense.CategoryID + "', '" + expense.UnitName + "', '" + expense.Rate + "','" + expense.MeterReadingFrom + "', '" + expense.To + "', '" + expense.Total + "','" + expense.TotalPass + "','" + expense.VehicleNumber + "')";
                            query += " END";


                            var command = new SqlCommand(query, connection);
                            connection.Open();
                            command.CommandType = CommandType.Text;
                            result = command.ExecuteNonQuery();
                            connection.Close();

                        }


                    }



                }
            }

            #endregion
            /**************************************************************************************HotelAutoTaxiDetails Expense Details******************************************/
            #region HotelAutoTaxiExpense
            string folderPathHotelAutoTaxiExp = HttpContext.Current.Server.MapPath("~/Uploads/");
            if (ExpObj.HotelAutoTaxiExpenseDetails != null && ExpObj.HotelAutoTaxiExpenseDetails.Count > 0)
            {
                foreach (var expense in ExpObj.HotelAutoTaxiExpenseDetails)
                {


                    /*HotelAutoTaxi Attachements*/

                    string HotelAutoTaxiAttachmentFileName = string.Empty;
                    if (expense.HotelAutoTaxiAttachment != null)
                    {
                        var file = expense.HotelAutoTaxiAttachment;
                        if (file != null && file.ContentLength > 0)
                        {
                            string originalFileName = Path.GetFileName(file.FileName);
                            string Extenstion = Path.GetExtension(originalFileName);
                            string dateTimeWithMilliseconds = DateTime.Now.ToString("yyyy-MM-dd'T'HHmmss.fff");
                            HotelAutoTaxiAttachmentFileName = "HotelAutoTaxiAttachment_" + dateTimeWithMilliseconds + originalFileName;
                            //HotelAutoTaxiAttachmentFileName = $"{PhotoCopyAttchmentFileName}.{Extenstion}";
                            string fullPath = Path.Combine(folderPathHotelAutoTaxiExp, HotelAutoTaxiAttachmentFileName);
                            file.SaveAs(fullPath);
                        }

                    }
                    else
                    {
                        HotelAutoTaxiAttachmentFileName = expense.strHotelAutoTaxiAttachment;
                    }
                    //string PhotoCopyAttchfileName = string.Empty;
                    //string PhotoCopyAttchmentFileName = string.Empty;
                    //if (expense.PhotoCopyAttach != null)
                    //{
                    //    var file = expense.PhotoCopyAttach;
                    //    if (file != null && file.ContentLength > 0)
                    //    {
                    //        string originalFileName = Path.GetFileName(file.FileName);
                    //        string Extenstion = Path.GetExtension(originalFileName);
                    //        string dateTimeWithMilliseconds = DateTime.Now.ToString("yyyy-MM-dd'T'HHmmss.fff");
                    //        PhotoCopyAttchfileName = "PhotoCopyAttch_" + dateTimeWithMilliseconds + originalFileName;
                    //        //PhotoCopyAttchmentFileName = $"{PhotoCopyAttchfileName}.{Extenstion}";
                    //        string fullPath = Path.Combine(folderPathOtherExp, PhotoCopyAttchfileName);
                    //        file.SaveAs(fullPath);
                    //    }

                    //}

                    //if (PhotoCopyAttchfileName != null && PhotoCopyAttchfileName.ToString() != "")
                    //    query += "PhotoCopyAttach='" + PhotoCopyAttchfileName + "',";


                    using (var connection = new SqlConnection(_strCon))
                    {
                        if (ExpObj.IsEdit == 0)
                        {
                            if (ExpenseID == 0)
                                continue;

                            string query = "INSERT INTO TourAndTravel_HotelAutoTaxiExpenseDetails (ExpenseID,Date,HotelOwnArrangements,Taxi,[Auto],HotelAutoTaxiAttachment) " +
                                       "VALUES (@ExpenseID, @Date, @HotelOwnArrangements, @Taxi, @Auto,@HotelAutoTaxiAttachment)";

                            var command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@ExpenseID", ExpenseID);
                            command.Parameters.AddWithValue("@Date", expense.Date);
                            command.Parameters.AddWithValue("@HotelOwnArrangements", expense.HotelOwnArrangements);
                            command.Parameters.AddWithValue("@Taxi", expense.Taxi);
                            command.Parameters.AddWithValue("@Auto", expense.Auto);
                            command.Parameters.AddWithValue("@HotelAutoTaxiAttachment", string.IsNullOrEmpty(HotelAutoTaxiAttachmentFileName) ? (object)DBNull.Value : HotelAutoTaxiAttachmentFileName);
                            connection.Open();
                            result = command.ExecuteNonQuery();
                        }
                        else//Update
                        {
                            string query = "if EXISTS(Select top 1 HotelAutoTaxiExpenseID from TourAndTravel_HotelAutoTaxiExpenseDetails where HotelAutoTaxiExpenseID<>0 and HotelAutoTaxiExpenseID='" + expense.HotelAutoTaxiExpenseID + "')";
                            query += " BEGIN ";
                            query += "Update TourAndTravel_HotelAutoTaxiExpenseDetails SET ";

                            query += "Date = '" + expense.Date + "',";
                            query += "HotelOwnArrangements='" + expense.HotelOwnArrangements + "',";
                            query += "Taxi='" + expense.Taxi + "',";
                            if (HotelAutoTaxiAttachmentFileName != null && HotelAutoTaxiAttachmentFileName.ToString() != "")
                                query += "HotelAutoTaxiAttachment='" + HotelAutoTaxiAttachmentFileName + "',";
                            query += "Auto='" + expense.Auto + "'";


                            query += " Where HotelAutoTaxiExpenseID='" + expense.HotelAutoTaxiExpenseID + "'";
                            query += " END";
                            query += " ELSE";
                            query += " BEGIN ";
                            query += " INSERT INTO TourAndTravel_HotelAutoTaxiExpenseDetails (ExpenseID,Date,HotelOwnArrangements,Taxi,[Auto]) " +
                                       "VALUES ('" + ExpenseID + "', '" + expense.Date + "', '" + expense.HotelOwnArrangements + "', '" + expense.Taxi + "', '" + expense.Auto + "')";
                            query += " END";


                            var command = new SqlCommand(query, connection);
                            connection.Open();
                            command.CommandType = CommandType.Text;
                            result = command.ExecuteNonQuery();
                            connection.Close();

                        }


                    }



                }
            }

            #endregion

            if (UserType == 1 && ExpObj.UserID != 1000)
            {
                _lhelp.ExecuteByExpression("Update TourAndTravelExpenseDetails set ClaimAmount=(SELECT dbo.GetTotalExpense(" + ExpenseID + ")) where ExpenseID=" + ExpenseID + " ");
            }
            else if (UserType != 1 && ExpObj.UserID != 1000 && ExpObj.UserID != 0)
            {
                _lhelp.ExecuteByExpression("Update TourAndTravelExpenseDetails set ClaimAmount=(SELECT dbo.GetTotalExpense(" + ExpenseID + ")) where ExpenseID=" + ExpenseID + " ");
            }
            else
            {
                if (ExpObj.Flag == "A")
                {
                    _lhelp.ExecuteByExpression("Update TourAndTravelExpenseDetails set ClaimAmount=(SELECT dbo.GetTotalExpense(" + ExpenseID + ")) where ExpenseID=" + ExpenseID + " ");
                    _lhelp.ExecuteByExpression("Update TourAndTravelExpenseDetails set PassingAmount=(SELECT dbo.GetTotalExpensePassing(" + ExpenseID + ")) where ExpenseID=" + ExpenseID + " ");
                }
                else if (ExpObj.Flag == "S")
                {
                    _lhelp.ExecuteByExpression("Update TourAndTravelExpenseDetails set ClaimAmount=(SELECT dbo.GetTotalExpense(" + ExpenseID + ")), BillFromDate='" + Convert.ToDateTime(ExpObj.BillFromDate).ToString("yyyy-MM-dd") + "',BillToDate='" + Convert.ToDateTime(ExpObj.BillToDate).ToString("yyyy-MM-dd") + "',PassingAmount=ISNULL((SELECT dbo.GetTotalPassingExpense('" + ExpenseID + "') AS TotalPassingExpense),0) where ExpenseID=" + ExpenseID + " ");
                }

            }

            if (ExpObj.Flag == "A")
            {
                ApproveExpenseVoucher(ExpenseID.ToString());
                _lhelp.ExecuteByExpression("Update TourAndTravelExpenseDetails set AppRejectRemarks='" + ExpObj.AppRejectRemarks + "' where ExpenseID=" + ExpenseID + " ");

                if (!string.IsNullOrEmpty(ExpObj.AppRejectRemarks) && ExpObj.AppRejectRemarks.Length > 0)
                {
                    _lhelp.ExecuteByExpression("insert into TourAndTravel_Comment (ExpenseID,OnDate,Comment,User_ID) values(" + Convert.ToInt32(ExpenseID) + ", getdate(), '" + ExpObj.AppRejectRemarks + "', " + Convert.ToInt32(HttpContext.Current.Session["UserId"]) + ")");
                }
                _lhelp.ExecuteByExpression("Update TourAndTravelExpenseDetails set Remarks='" + ExpObj.Remarks + "' , StationCovered='" + ExpObj.StationCovered + "' where ExpenseID=" + ExpenseID + " ");
            }
            else if (ExpObj.Flag == "R")
            {
                RejectExpenseVoucher(ExpenseID.ToString());
                _lhelp.ExecuteByExpression("Update TourAndTravelExpenseDetails set AppRejectRemarks='" + ExpObj.AppRejectRemarks + "',PassingAmount=0,LastApprovedByID=0,LastApprovedDate='',LastRejectedDate=Getdate(),LastRejectedByID= " + Convert.ToInt32(HttpContext.Current.Session["UserId"]) + " ,Approved=0 where ExpenseID=" + ExpenseID + " ");

                if (!string.IsNullOrEmpty(ExpObj.AppRejectRemarks) && ExpObj.AppRejectRemarks.Length > 0)
                {
                    _lhelp.ExecuteByExpression("insert into TourAndTravel_Comment (ExpenseID,OnDate,Comment,User_ID) values(" + Convert.ToInt32(ExpenseID) + ", getdate(), '" + ExpObj.AppRejectRemarks + "', " + Convert.ToInt32(HttpContext.Current.Session["UserId"]) + ")");
                }
            }
            else if (ExpObj.Flag == "S")
            {
                _lhelp.ExecuteByExpression("Update TourAndTravelExpenseDetails set AppRejectRemarks='" + ExpObj.AppRejectRemarks + "' where ExpenseID=" + ExpenseID + " ");

                if (!string.IsNullOrEmpty(ExpObj.AppRejectRemarks) && ExpObj.AppRejectRemarks.Length > 0)
                {
                    _lhelp.ExecuteByExpression("insert into TourAndTravel_Comment (ExpenseID,OnDate,Comment,User_ID) values(" + Convert.ToInt32(ExpenseID) + ", getdate(), '" + ExpObj.AppRejectRemarks + "', " + Convert.ToInt32(HttpContext.Current.Session["UserId"]) + ")");
                }
                _lhelp.ExecuteByExpression("Update TourAndTravelExpenseDetails set Remarks='" + ExpObj.Remarks + "' , StationCovered='" + ExpObj.StationCovered + "' where ExpenseID=" + ExpenseID + " ");
            }
            else if (ExpObj.Flag == "U")
            {
                _lhelp.ExecuteByExpression("Update TourAndTravelExpenseDetails set AppRejectRemarks='" + ExpObj.AppRejectRemarks + "' where ExpenseID=" + ExpenseID + " ");

                if (!string.IsNullOrEmpty(ExpObj.AppRejectRemarks) && ExpObj.AppRejectRemarks.Length > 0)
                {
                    _lhelp.ExecuteByExpression("insert into TourAndTravel_Comment (ExpenseID,OnDate,Comment,User_ID) values(" + Convert.ToInt32(ExpenseID) + ", getdate(), '" + ExpObj.AppRejectRemarks + "', " + Convert.ToInt32(HttpContext.Current.Session["UserId"]) + ")");
                }

                if (ExpObj.Remarks == null)
                    ExpObj.Remarks = "";

                string safeRemarks = ExpObj.Remarks.Replace("'", "''");
                //RSPLCommonDAL.AppendData("Update TourAndTravelExpenseDetails set Remarks='" + ExpObj.Remarks + "' , StationCovered='" + ExpObj.StationCovered + "',Approved=1,PassingAmount=0,LastApprovedByID=" + Convert.ToInt32(HttpContext.Current.Session["UserId"]) + ",LastApprovedDate=Getdate(),LastRejectedByID=0,LastRejectedDate='' where ExpenseID=" + ExpenseID + " ");
                _lhelp.ExecuteByExpression("Update TourAndTravelExpenseDetails set Remarks='" + safeRemarks + @"' , StationCovered='" + ExpObj.StationCovered + "',Approved=1,PassingAmount=0,LastApprovedByID=NULL,LastApprovedDate=NULL,LastRejectedByID=0,LastRejectedDate='' where ExpenseID=" + ExpenseID + " ");
            }
            return result;
        }

        public string ApproveExpenseVoucher(string ExpenseId)
        {
            string Msg = string.Empty;
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Update TourAndTravelExpenseDetails");
                sb.AppendLine("SET Approved='" + HttpContext.Current.Session["UserType"].ToString() + "',");
                sb.AppendLine("LastApprovedDate='" + DateTime.Now.ToString("yyyy-MM-dd") + "',");
                sb.AppendLine("LastApprovedByID ='" + HttpContext.Current.Session["UserId"].ToString() + "'");
                //sb.AppendLine("LastApprovedByName ='" + HttpContext.Current.Session["UserName"].ToString() + "'");
                sb.AppendLine("Where expenseid='" + ExpenseId + "'");
                string SqlQuerry = sb.ToString();

                using (var connection = new SqlConnection(_strCon))
                {
                    var command = new SqlCommand(SqlQuerry, connection);
                    connection.Open();
                    int Res = command.ExecuteNonQuery();
                    Msg = Res > 0 ? "Approved!" : "Failed";

                }
            }
            catch (Exception ex)
            {
                Msg = "Something went wrong";
            }

            return Msg;
        }

        public string RejectExpenseVoucher(string ExpenseId)
        {
            string Msg = string.Empty;
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Update TourAndTravelExpenseDetails");
                sb.AppendLine("SET Approved='0',");
                sb.AppendLine("LastApprovedDate='" + DateTime.Now.ToString("yyyy-MM-dd") + "',");
                sb.AppendLine("LastApprovedByID ='" + HttpContext.Current.Session["UserId"].ToString() + "'");
                //sb.AppendLine("LastApprovedByName ='" + HttpContext.Current.Session["UserName"].ToString() + "'");
                sb.AppendLine("Where expenseid='" + ExpenseId + "'");
                string SqlQuerry = sb.ToString();

                using (var connection = new SqlConnection(_strCon))
                {
                    var command = new SqlCommand(SqlQuerry, connection);
                    connection.Open();
                    int Res = command.ExecuteNonQuery();
                    Msg = Res > 0 ? "Rejected!" : "Failed";

                }
            }
            catch (Exception ex)
            {
                Msg = "Something went wrong";
            }

            return Msg;
        }

        public int saveFile(string FileName)
        {
            int identity = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Declare @Identity int=0;");
            sb.AppendLine("Insert into FileInfo([FileName]) values('" + FileName.Trim() + "')");
            sb.AppendLine(" SET @Identity=Scope_Identity()");
            sb.AppendLine(" Select @Identity as ID");
            string Query = sb.ToString();
            DBHelper _lhelp = new DBHelper(_strCon);
            DataTable dt = _lhelp.GetDataTableByExpression(Query);
            if (dt != null && dt.Rows.Count > 0)
                identity=Convert.ToInt32(dt.Rows[0]["ID"]);

            return identity;
        }

        public List<Expense> GetExpenses(int CompanyID, int UserID, string sDate, string eDate, int LedgerID)
        {
            List<Expense> _data = new List<Expense>();
            try
            {
                DBHelper _lhelp = new DBHelper(_strCon);
                DataTable dtLedger = _lhelp.GetDataTableByExpression("Select TOP 1 LedgerID from Users_default where [User_ID]='" + UserID + "' ");
                if (dtLedger != null && dtLedger.Rows.Count > 0)
                    LedgerID = Convert.ToInt32(dtLedger.Rows[0]["LedgerID"]);

                using (SqlConnection sConn = new SqlConnection(_strCon))
                {
                    sConn.Open();
                    SqlCommand cmd = new SqlCommand("usp_GetExpenses2", sConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("CompanyID", CompanyID);
                    cmd.Parameters.AddWithValue("UserID", UserID);
                    cmd.Parameters.AddWithValue("LedgerID", LedgerID);
                    cmd.Parameters.AddWithValue("sDate", sDate);
                    cmd.Parameters.AddWithValue("eDate", eDate);

                    DataTable tbl = new DataTable("tbl");
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(tbl);
                    sConn.Close();
                    _data = (from DataRow dr in tbl.Rows
                             select new Expense()
                             {
                                 ExpenseID = dr["ExpenseID"] != DBNull.Value ? Convert.ToInt32(dr["ExpenseID"]) : 0,
                                 LedgerID = dr["LedgerID"] != DBNull.Value ? Convert.ToInt32(dr["LedgerID"]) : 0,
                                 VoucherNo = dr["VoucherNo"] != DBNull.Value ? Convert.ToInt32(dr["VoucherNo"]) : 0,
                                 LedgerName = dr["LedgerName"] != DBNull.Value ? Convert.ToString(dr["LedgerName"]) : string.Empty,
                                 Approved = dr["Approved"] != DBNull.Value ? Convert.ToInt32(dr["Approved"]) : 0,
                                 Status = dr["Status"] != DBNull.Value ? Convert.ToInt32(dr["Status"]) : 0,
                                 ExpDate = dr["ExpDate"] != DBNull.Value ? Convert.ToString(dr["ExpDate"]) : string.Empty,
                                 Amount = dr["Amount"] != DBNull.Value ? Convert.ToDouble(dr["Amount"]) : 0.0,
                                 LastApprovedByID = dr["LastApprovedByID"] != DBNull.Value ? Convert.ToInt32(dr["LastApprovedByID"]) : 0,
                                 BillFromDate = dr["BillFromDate"] != DBNull.Value ? Convert.ToString(dr["BillFromDate"]) : string.Empty,
                                 BillToDate = dr["BillToDate"] != DBNull.Value ? Convert.ToString(dr["BillToDate"]) : string.Empty,
                                 ClaimAmount = dr["ClaimAmount"] != DBNull.Value ? Convert.ToString(dr["ClaimAmount"]) : string.Empty,
                                 PassingAmount = dr["PassingAmount"] != DBNull.Value ? Convert.ToString(dr["PassingAmount"]) : string.Empty,


                             }).ToList();

                    return _data;
                }

            }
            catch (Exception ex)
            {
                string strMsg = ex.Message;
                return _data;
            }
        }

        public AddExpense GetExpensesTourByID(int id)
        {
            AddExpense obj = new AddExpense();
            // id = 1041;
            try
            {

                //var Deisgn = GetLedgerDesignation(LedgerID);

                using (SqlConnection sConn = new SqlConnection(_strCon))
                {
                    sConn.Open();
                    SqlCommand cmd = new SqlCommand("usp_getTourExpenseByExpenseId2", sConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("ExpenseId", id);

                    DataTable tbl = new DataTable("tbl");
                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    sConn.Close();

                    if (ds != null && ds.Tables.Count == 0)
                    {
                        obj.Message = "No Data Found!";
                        return obj;
                    }
                    /*****************OtherExpense Details*************/
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count != 0)
                    {
                        obj.OtherExpenseDetails = (from DataRow dr in ds.Tables[0].Rows
                                                   select new OtherExpenseDetails()
                                                   {
                                                       OtherExpID = Convert.ToInt32(dr["OtherExpID"]),
                                                       ExpenseID = Convert.ToInt32(dr["ExpenseID"]),
                                                       PhotoCopyExp = Convert.ToInt32(dr["PhotoCopyExp"]),
                                                       OtherExp = Convert.ToInt32(dr["OtherExp"]),
                                                       CourierFaxSTDExp = Convert.ToInt32(dr["CourierFaxSTDExp"]),
                                                       MiscExp = Convert.ToInt32(dr["MiscExp"]),
                                                       Remarks = Convert.ToString(dr["Remarks"]),
                                                       strPhotoCopyAttach = Convert.ToString(dr["PhotoCopyAttach"]),
                                                       strOthExpAttch = Convert.ToString(dr["OthExpAttch"]),
                                                       strCourierExpAttch = Convert.ToString(dr["CourierExpAttch"]),
                                                       strMiscExpAttch = Convert.ToString(dr["MiscExpAttch"]),
                                                       Date = Convert.ToDateTime(dr["Date"] ?? DateTime.MinValue).ToString("dd-MMM-yyyy"),
                                                   }).ToList();
                    }
                    else
                    {
                        obj.OtherExpenseDetails = new List<OtherExpenseDetails>
                        {
                            new OtherExpenseDetails
                            {
                                OtherExpID = 0,
                                ExpenseID = Convert.ToInt32(ds.Tables[4].Rows[0]["ExpenseID"]),
                                PhotoCopyExp = 0,
                                OtherExp = 0,
                                CourierFaxSTDExp = 0,
                                MiscExp = 0,
                                Remarks = "",
                                strPhotoCopyAttach = "",
                                strOthExpAttch = "",
                                strCourierExpAttch = "",
                                strMiscExpAttch = "",
                                Date = Convert.ToString(ds.Tables[4].Rows[0]["BillFromDate"]),
                            }
                        };
                    }
                    /*****************JourneyExpense Details*************/
                    if (ds.Tables[1] != null && ds.Tables[1].Rows.Count != 0)
                    {
                        obj.JourneyExpenseDetails = (from DataRow dr in ds.Tables[1].Rows
                                                     select new JourneyExpenseDetails()
                                                     {
                                                         JourneyExpID = Convert.ToInt32(dr["JourneyExpID"]),
                                                         ExpenseID = Convert.ToInt32(dr["ExpenseID"]),
                                                         //SNO = Convert.ToInt32(dr["SNO"]),
                                                         Date = Convert.ToString(dr["Date"]),
                                                         FromStation = Convert.ToString(dr["FromStation"]),
                                                         ToStation = Convert.ToString(dr["ToStation"]),
                                                         Fare = Convert.ToString(dr["Fare"]),
                                                         Ticket = Convert.ToString(dr["Ticket"]),
                                                         Remarks = Convert.ToString(dr["Remarks"]),
                                                         strJourneyAttach = Convert.ToString(dr["JourneyAttach"])

                                                     }).ToList();
                    }
                    else
                    {
                        obj.JourneyExpenseDetails = new List<JourneyExpenseDetails>
                        {
                            new JourneyExpenseDetails
                            {
                                JourneyExpID = 0,
                                ExpenseID = Convert.ToInt32(ds.Tables[4].Rows[0]["ExpenseID"]),
                                Date = Convert.ToString(ds.Tables[4].Rows[0]["BillFromDate"]),
                                FromStation = "N/A",
                                ToStation = "N/A",
                                Fare = "0",
                                Ticket = "0",
                                Remarks = "No data available",
                                strJourneyAttach = string.Empty
                            }
                        };
                    }
                    /*****************TransportExpense Details*************/
                    if (ds.Tables[2] != null && ds.Tables[2].Rows.Count != 0)
                    {
                        obj.TransportExpenseDetails = (from DataRow dr in ds.Tables[2].Rows
                                                       select new TransportExpenseDetails()
                                                       {
                                                           TransportExpID = dr["TransportExpID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TransportExpID"]),
                                                           ExpenseID = dr["ExpenseID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ExpenseID"]),
                                                           //SNO = dr["SNO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["SNO"]),
                                                           ChallanNo = dr["ChallanNo"] == DBNull.Value ? string.Empty : Convert.ToString(dr["ChallanNo"]),
                                                           BuiltyNo = dr["BuiltyNo"] == DBNull.Value ? string.Empty : Convert.ToString(dr["BuiltyNo"]),
                                                           Station = dr["Station"] == DBNull.Value ? string.Empty : Convert.ToString(dr["Station"]),
                                                           NoofBDLS = dr["NoofBDLS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NoofBDLS"]),
                                                           TransportName = dr["TransportName"] == DBNull.Value ? string.Empty : Convert.ToString(dr["TransportName"]),
                                                           Amount = dr["Amount"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Amount"]),
                                                           strTransPortAttach = dr["TransPortAttach"] == DBNull.Value ? string.Empty : Convert.ToString(dr["TransPortAttach"]),
                                                           Date = Convert.ToDateTime(dr["Date"] ?? DateTime.MinValue).ToString("dd-MMM-yyyy"),
                                                           Remarks = dr["Remarks"] == DBNull.Value ? string.Empty : Convert.ToString(dr["Remarks"]),
                                                           Cartridge = dr["Cartridge"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Cartridge"])



                                                       }).ToList();
                    }
                    else
                    {
                        obj.TransportExpenseDetails = new List<TransportExpenseDetails>
                        {
                            new TransportExpenseDetails
                            {
                                TransportExpID = 0,
                                ExpenseID = Convert.ToInt32(ds.Tables[4].Rows[0]["ExpenseID"]),
                                ChallanNo = "N/A",
                                BuiltyNo = "N/A",
                                Station = "N/A",
                                NoofBDLS = 0,
                                TransportName = "N/A",
                                Amount = 0,
                                strTransPortAttach = string.Empty,
                                Date = Convert.ToString(ds.Tables[4].Rows[0]["BillFromDate"]),
                                Remarks = "No data available",
                                Cartridge = 0
                            }
                        };
                    }
                    /*****************TollExpense  Details*************/
                    if (ds.Tables[3] != null && ds.Tables[3].Rows.Count != 0)
                    {
                        obj.TollExpenseDetails = (from DataRow dr in ds.Tables[3].Rows
                                                  select new TollExpenseDetails()
                                                  {
                                                      TollExpID = Convert.ToInt32(dr["TollExpID"]),
                                                      ExpenseID = Convert.ToInt32(dr["ExpenseID"]),
                                                      //SNO = Convert.ToInt32(dr["SNO"]),
                                                      TransactionID = Convert.ToString(dr["TransactionID"]),
                                                      Date = Convert.ToString(dr["Date"]),
                                                      TollPlaza = Convert.ToString(dr["TollPlaza"]),
                                                      Time = Convert.ToString(dr["Time"]),
                                                      Amount = Convert.ToInt32(dr["Amount"]),
                                                      strTollAttachment = Convert.ToString(dr["TollAttachment"])

                                                  }).ToList();
                    }
                    else
                    {
                        obj.TollExpenseDetails = new List<TollExpenseDetails>();
                    }


                    if (ds.Tables[4] != null && ds.Tables[4].Rows.Count != 0)
                    {
                        obj.LedgerID = Convert.ToInt32(ds.Tables[4].Rows[0]["LedgerID"]);
                        obj.LedgerName = Convert.ToString(ds.Tables[4].Rows[0]["LedgerName"]);
                        obj.wefDate = Convert.ToString(ds.Tables[4].Rows[0]["Date"]);
                        obj.Remarks = Convert.ToString(ds.Tables[4].Rows[0]["Description"]);
                        obj.Approved = Convert.ToInt32(ds.Tables[4].Rows[0]["Approved"]);
                        obj.LastApprovedByID = Convert.ToInt32(ds.Tables[4].Rows[0]["LastApprovedByID"]);
                        obj.Remarks = Convert.ToString(ds.Tables[4].Rows[0]["Description"]);
                        obj.BillFromDate = Convert.ToString(ds.Tables[4].Rows[0]["BillFromDate"]);
                        obj.BillToDate = Convert.ToString(ds.Tables[4].Rows[0]["BillToDate"]);
                        obj.SubmissionDate = Convert.ToString(ds.Tables[4].Rows[0]["Date"]);
                        //obj.AppRejectRemarks = Convert.ToString(ds.Tables[4].Rows[0]["AppRejectRemarks"]);
                        obj.ExpenseID = Convert.ToInt32(ds.Tables[4].Rows[0]["ExpenseID"]);
                        obj.ClaimAmount = Convert.ToString(ds.Tables[4].Rows[0]["ClaimAmount"]);
                        obj.PassingAmount = Convert.ToString(ds.Tables[4].Rows[0]["PassingAmount"]);
                        obj.StationCovered = Convert.ToString(ds.Tables[4].Rows[0]["StationCovered"]);

                    }
                    else
                    {
                        obj.TollExpenseDetails = new List<TollExpenseDetails>();
                    }
                    /*****************Journey Accomodation Details*************/
                    if (ds.Tables[5] != null && ds.Tables[5].Rows.Count != 0)
                    {
                        obj.JourneyAccomExpenseDetails = (from DataRow dr in ds.Tables[5].Rows
                                                          select new JourneyAccomExpenseDetails()
                                                          {
                                                              JourneyAccomExpID = Convert.ToInt32(dr["JourneyAccomExpID"] ?? "0"),
                                                              Date = Convert.ToDateTime(dr["Date"] ?? DateTime.MinValue).ToString("dd-MMM-yyyy"),
                                                              StartTime = Convert.ToString(dr["StartTime"] ?? ""),
                                                              EndTime = Convert.ToString(dr["EndTime"] ?? ""),
                                                              LocalDA = Convert.ToDouble(dr["LocalDA"] ?? 0),
                                                              Breakfast = Convert.ToDouble(dr["Breakfast"] ?? 0),
                                                              Lunch = Convert.ToDouble(dr["Lunch"] ?? 0),
                                                              Dinner = Convert.ToDouble(dr["Dinner"] ?? 0),
                                                              Lodging = Convert.ToDouble(dr["Lodging"] ?? 0),
                                                              strLodingAttachment = Convert.ToString(dr["LodingAttachment"] ?? ""),
                                                              Remarks = Convert.ToString(dr["Remarks"] ?? ""),
                                                              RoomSharing = Convert.ToDouble(dr["RoomSharing"] ?? 0)
                                                          }).ToList();
                    }
                    else
                    {
                        obj.JourneyAccomExpenseDetails = new List<JourneyAccomExpenseDetails>
                        {
                            new JourneyAccomExpenseDetails
                            {
                                JourneyAccomExpID = 0,
                                Date = Convert.ToString(ds.Tables[4].Rows[0]["BillFromDate"]),
                                StartTime = "",
                                EndTime = "",
                                LocalDA = 0,
                                Breakfast = 0,
                                Lunch = 0,
                                Dinner = 0,
                                Lodging = 0,
                                strLodingAttachment = "",
                                ExpenseID = Convert.ToInt32(ds.Tables[4].Rows[0]["ExpenseID"]),
                                Remarks = "",
                                RoomSharing = 0
                            }
                        };
                    }
                    if (obj.LedgerID != null)
                    {
                        obj.DesignID = GetLedgerDesignation(obj.LedgerID);
                    }
                    /*****************Travel Expense Details*************/
                    //if (ds.Tables[6] != null && ds.Tables[6].Rows.Count != 0)
                    //{
                    //    obj.TravelExpenseDetails = (from DataRow dr in ds.Tables[6].Rows
                    //                                select new TravelExpenseDetails()
                    //                                {
                    //                                    TravelExpenseID = Convert.ToInt32(dr["TravelExpenseID"] ?? "0"),
                    //                                    Date = dr["Date"] == DBNull.Value ? DateTime.MinValue.ToString("dd-MMM-yyyy") : Convert.ToDateTime(dr["Date"]).ToString("dd-MMM-yyyy"),
                    //                                    CategoryID = dr["CategoryID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CategoryID"]),
                    //                                    UnitName = dr["UnitName"] == DBNull.Value ? "" : Convert.ToString(dr["UnitName"]),
                    //                                    Rate = dr["Rate"] == DBNull.Value ? 0 : Convert.ToDouble(dr["Rate"]),
                    //                                    LastMeterField = dr["To"] == DBNull.Value ? 0 : Convert.ToInt32(dr["To"]),
                    //                                    MeterReadingFrom = dr["MeterReadingFrom"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MeterReadingFrom"]),
                    //                                    To = dr["To"] == DBNull.Value ? 0 : Convert.ToInt32(dr["To"]),
                    //                                    Total = dr["Total"] == DBNull.Value ? 0 : Convert.ToDouble(dr["Total"]),
                    //                                    TotalKM = (dr["To"] == DBNull.Value ? 0 : Convert.ToInt32(dr["To"]))
                    //                                            - (dr["MeterReadingFrom"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MeterReadingFrom"]))


                    //                                }).ToList();
                    //}
                    if (ds.Tables[6] != null && ds.Tables[6].Rows.Count != 0)
                    {
                        int previousTo = 0;
                        obj.TravelExpenseDetails = new List<TravelExpenseDetails>();

                        foreach (DataRow dr in ds.Tables[6].Rows)
                        {
                            var meterFrom = dr["MeterReadingFrom"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MeterReadingFrom"]);
                            var meterTo = dr["To"] == DBNull.Value ? 0 : Convert.ToInt32(dr["To"]);

                            var detail = new TravelExpenseDetails()
                            {
                                TravelExpenseID = Convert.ToInt32(dr["TravelExpenseID"] ?? "0"),
                                Date = dr["Date"] == DBNull.Value ? DateTime.MinValue.ToString("dd-MMM-yyyy") : Convert.ToDateTime(dr["Date"]).ToString("dd-MMM-yyyy"),
                                CategoryID = dr["CategoryID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CategoryID"]),
                                UnitName = dr["UnitName"] == DBNull.Value ? "" : Convert.ToString(dr["UnitName"]),
                                Rate = dr["Rate"] == DBNull.Value ? 0 : Convert.ToDouble(dr["Rate"]),
                                //LastMeterField = previousTo, // <-- Corrected here
                                LastMeterField = meterTo,
                                ExpenseID = dr["ExpenseID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ExpenseID"]),
                                MeterReadingFrom = meterFrom,
                                To = meterTo,
                                Total = dr["Total"] == DBNull.Value ? 0 : Convert.ToDouble(dr["Total"]),
                                TotalPass = dr["TotalPass"] == DBNull.Value ? 0 : Convert.ToDouble(dr["TotalPass"]),
                                VehicleNumber = dr["VehicleNumber"] == DBNull.Value ? "" : Convert.ToString(dr["VehicleNumber"]),
                                TotalKM = meterTo - meterFrom
                            };

                            obj.TravelExpenseDetails.Add(detail);

                            // Update previousTo for next iteration
                            previousTo = meterTo;
                        }
                    }

                    else
                    {
                        obj.TravelExpenseDetails = new List<TravelExpenseDetails>
                        {
                            new TravelExpenseDetails
                            {
                                TravelExpenseID = 0,
                                Date = Convert.ToString(ds.Tables[4].Rows[0]["BillFromDate"]),
                                CategoryID = 0,
                                UnitName = "",
                                Rate = 0,
                                LastMeterField = 0,
                                ExpenseID = Convert.ToInt32(ds.Tables[4].Rows[0]["ExpenseID"]),
                                MeterReadingFrom = 0,
                                To = 0,
                                Total = 0,
                                TotalPass = 0,
                                TotalKM = 0,
                                VehicleNumber=""
                            }
                        };
                    }
                    /*****************HotelAutoTaxi Details*************/
                    if (ds.Tables[7] != null && ds.Tables[7].Rows.Count != 0)
                    {
                        obj.HotelAutoTaxiExpenseDetails = (from DataRow dr in ds.Tables[7].Rows
                                                           select new HotelAutoTaxiExpenseDetails()
                                                           {
                                                               HotelAutoTaxiExpenseID = Convert.ToInt32(dr["HotelAutoTaxiExpenseID"] ?? "0"),
                                                               Date = Convert.ToDateTime(dr["Date"] ?? DateTime.MinValue).ToString("dd-MMM-yyyy"),
                                                               HotelOwnArrangements = Convert.ToInt32(dr["HotelOwnArrangements"] ?? ""),
                                                               Taxi = Convert.ToInt32(dr["Taxi"] ?? ""),
                                                               Auto = Convert.ToInt32(dr["Auto"] ?? "0"),
                                                               strHotelAutoTaxiAttachment = Convert.ToString(dr["HotelAutoTaxiAttachment"] ?? "")


                                                           }).ToList();
                    }
                    else
                    {
                        obj.HotelAutoTaxiExpenseDetails = new List<HotelAutoTaxiExpenseDetails>
                        {
                            new HotelAutoTaxiExpenseDetails
                            {
                                HotelAutoTaxiExpenseID = 0,
                                Date = Convert.ToString(ds.Tables[4].Rows[0]["BillFromDate"]),
                                HotelOwnArrangements = 0,
                                Taxi = 0,
                                Auto = 0,
                                ExpenseID = Convert.ToInt32(ds.Tables[4].Rows[0]["ExpenseID"]),
                                strHotelAutoTaxiAttachment = ""
                            }
                        };
                    }
                    /*****************Comment Details*************/
                    //if (ds.Tables[8] != null && ds.Tables[8].Rows.Count != 0)
                    //{
                    //    obj.CommentLists = (from DataRow dr in ds.Tables[7].Rows
                    //                        select new CommentList()
                    //                        {
                    //                            CommentID = Convert.ToInt32(dr["CommentID"] ?? "0"),
                    //                            ExpenseID = Convert.ToInt32(dr["ExpenseID"] ?? "0"),
                    //                            onDate = Convert.ToString(dr["onDate"] ?? ""),
                    //                            Comment = Convert.ToString(dr["Comment"] ?? ""),
                    //                            User_Id = Convert.ToInt32(dr["User_ID"] ?? "0"),
                    //                            UserName = Convert.ToString(dr["UserName"] ?? "")

                    //                        }).ToList();
                    //}
                    //else
                    //{
                    //    obj.CommentLists = new List<CommentList>();
                    //}

                    return obj;
                }

            }
            catch (Exception ex)
            {
                string strMsg = ex.Message;
                return obj;
            }
        }

        public int GetLedgerDesignation(int LedgerID)
        {
            int integer1 = LedgerID;
            int DesignID = 0;
            DBHelper _lhelp = new DBHelper(_strCon);

            DataTable dt = _lhelp.GetDataTableByExpression("Select B.id from TourAndTravel_DesignationDetails A INNEr JOIN TourAndTravelDesignation B on A.DesignationID=B.id where DeleteStatus=1 AND LedgerID=" + LedgerID);
            if (dt.Rows.Count > 0)
            {
                DesignID = Convert.ToInt32(dt.Rows[0]["id"]);
            }


            return DesignID;
        }
    }
}