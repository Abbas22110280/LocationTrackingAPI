using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace LocationTrackingAPI.Models
{
    public class AddExpense
    {
        public int LastApprovedByID { get; set; }
        public int IsEdit { get; set; }
        public string Message { get; set; }
        public string PrefixUrl { get; set; }
        public int ExpenseID { get; set; }
        public int LedgerID { get; set; }
        
        [DisplayName("Ledger Name")]
        public string LedgerName { get; set; }
        
        [DisplayName("Date")]
        public string wefDate { get; set; }
        public double Amount { get; set; }
        
        //[Required(AllowEmptyStrings = true, ErrorMessage = "Naration is Required")]
        public string Naration { get; set; }
        [DisplayName("Remarks")]
        public string Description { get; set; }
        public int CompanyID { get; set; }
        public int UserID { get; set; }
        public int VoucherNo { get; set; }
        public string GUID { get; set; }
        public int Approved { get; set; }
        public string Report { get; set; }

        /*Design Details*/
        public string Designation { get; set; }
        public int DesignID { get; set; }

        [DisplayName("Hotel on arrangements")]
        public double HotelOnArrangements { get; set; }
        public double Taxi { get; set; }
        public double Auto { get; set; }
        public string Remarks { get; set; }

        /*New Version*/
        public string SubmissionDate { get; set; }
        [DisplayName("Bill from date")]
        public string BillFromDate { get; set; }
        [DisplayName("Bill to date")]
        public string BillToDate { get; set; }
        public string Flag { get; set; }

        [DisplayName("Approve or Reject Remarks")]
        public string AppRejectRemarks { get; set; }
        [DisplayName("Claim Amount")]
        public string ClaimAmount { get; set; }
        [DisplayName("Passing Amount")]
        public string PassingAmount { get; set; }
        [DisplayName("Station Convered")]
        public string StationCovered { get; set; }
        public List<HotelAutoTaxiExpenseDetails> HotelAutoTaxiExpenseDetails { get; set; }
        public List<TravelExpenseDetails> TravelExpenseDetails { get; set; }
        public List<OtherExpenseDetails> OtherExpenseDetails { get; set; }
        public List<JourneyExpenseDetails> JourneyExpenseDetails { get; set; }

        public List<TransportExpenseDetails> TransportExpenseDetails { get; set; }
        public List<TollExpenseDetails> TollExpenseDetails { get; set; }

        public List<JourneyAccomExpenseDetails> JourneyAccomExpenseDetails { get; set; }
        public List<CommentList> CommentLists { get; set; }
    }


    public class HotelAutoTaxiExpenseDetails
    {
        public int HotelAutoTaxiExpenseID { get; set; }
        public int ExpenseID { get; set; }
        public string Date { get; set; }
        public int HotelOwnArrangements { get; set; }
        public int Taxi { get; set; }
        public int Auto { get; set; }
        public string strHotelAutoTaxiAttachment { get; set; }
        public HttpPostedFileBase HotelAutoTaxiAttachment { get; set; }

    }
    public class TravelExpenseDetails
    {
        public int TravelExpenseID { get; set; }
        public int ExpenseID { get; set; }
        public string Date { get; set; }
        public int CategoryID { get; set; }
        public string UnitName { get; set; }
        public double Rate { get; set; }
        public int MeterReadingFrom { get; set; }
        public int To { get; set; }
        public double Total { get; set; }
        public int TotalKM { get; set; }
        public int LastMeterField { get; set; }

        public double TotalPass { get; set; }
        public string VehicleNumber { get; set; }

    }
    public class JourneyAccomExpenseDetails
    {
        public int JourneyAccomExpID { get; set; }
        public int ExpenseID { get; set; }
        public string Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public double LocalDA { get; set; }
        public double Breakfast { get; set; }
        public double Lunch { get; set; }
        public double Dinner { get; set; }
        public double Lodging { get; set; }
        public string Remarks { get; set; }
        public double RoomSharing { get; set; }
        public string strLodingAttachment { get; set; }
        public HttpPostedFileBase LodingAttachment { get; set; }

    }
    public class OtherExpenseDetails
    {
        public int OtherExpID { get; set; }
        public string Date { get; set; }
        public int ExpenseID { get; set; }
        [DisplayName("Photo Copy Exp.")]
        public double PhotoCopyExp { get; set; }
        [DisplayName("Other Exp.")]
        public double OtherExp { get; set; }
        [DisplayName("Courier Fax S.T.D Exp.")]
        public double CourierFaxSTDExp { get; set; }
        [DisplayName("Misc. Exp.")]
        public double MiscExp { get; set; }
        [DisplayName("Remarks")]
        public string Remarks { get; set; }
        public string strPhotoCopyAttach { get; set; }
        public string strOthExpAttch { get; set; }
        public string strCourierExpAttch { get; set; }
        public string strMiscExpAttch { get; set; }


        public HttpPostedFileBase PhotoCopyAttach { get; set; }
        public HttpPostedFileBase OthExpAttch { get; set; }
        public HttpPostedFileBase CourierExpAttch { get; set; }
        public HttpPostedFileBase MiscExpAttch { get; set; }
    }
    public class JourneyExpenseDetails
    {
        public int JourneyExpID { get; set; }
        public int ExpenseID { get; set; }
        public int SNO { get; set; }
        public string Date { get; set; }
        public string FromStation { get; set; }
        public string ToStation { get; set; }
        public string Fare { get; set; }
        public string Ticket { get; set; }
        public string Remarks { get; set; }
        public string strJourneyAttach { get; set; }
        public HttpPostedFileBase JourneyAttach { get; set; }

    }
    public class TransportExpenseDetails
    {
        public string Date { get; set; }
        public int TransportExpID { get; set; }
        public int ExpenseID { get; set; }
        public int SNO { get; set; }
        public string Station { get; set; }
        public string ChallanNo { get; set; }
        public string BuiltyNo { get; set; }
        public string TransportName { get; set; }
        public int NoofBDLS { get; set; }
        public int Cartridge { get; set; }
        public int Amount { get; set; }
        public string Remarks { get; set; }
        public string strTransPortAttach { get; set; }
        public HttpPostedFileBase TransPortAttach { get; set; }

    }
    public class TollExpenseDetails
    {
        public int TollExpID { get; set; }
        public int ExpenseID { get; set; }
        public int SNO { get; set; }
        public string TransactionID { get; set; }
        public string Date { get; set; }
        public string TollPlaza { get; set; }
        public string Time { get; set; }
        public int Amount { get; set; }
        public string strTollAttachment { get; set; }
        public HttpPostedFileBase TollAttachment { get; set; }

    }
    public class AttachmentDetails
    {
        public string PhotoCopyKey { get; set; }
        public string PetrolExpKey { get; set; }
    }
    public class CurrentExpense
    {
        public int CurrentID { get; set; }
        public int ExpenseID { get; set; }
        public string Description { get; set; }
        public bool isFixed { get; set; }
        public double Amount { get; set; }
        public string GUID { get; set; }
        public int UserID { get; set; }

    }
    public class DefaultExpense
    {
        public int ParameterID { get; set; }
        public string Category { get; set; }
        public int Unit { get; set; }
        public double Rate { get; set; }
        public string UnitName { get; set; }
    }
    public class Visits
    {
        public int SchoolID { get; set; }
        public string Ondate { get; set; }
        public string CODE { get; set; }
        public string SchoolName { get; set; }
        public string address { get; set; }
        public int Quantity { get; set; }
        public int NosTeachers { get; set; }
        public string Locality { get; set; }
        public string District { get; set; }
        public string State { get; set; }
        public double Amount { get; set; }
        public int TotalTeacher { get; set; }
        public int TotalTR { get; set; }
        public int DataType { get; set; }
    }
    public class PendingTotal
    {
        public int Counts { get; set; }
        public string Insitute_Board { get; set; }
        public int Insitute_Board_id { get; set; }
    }
    public class SchoolLog
    {
        public string Name { get; set; }
        public string Designation { get; set; }
        public int SchoolID { get; set; }
        public int Teacher_ID { get; set; }
        public string email { get; set; }
        public string Mobile { get; set; }
        public string Subject { get; set; }
        public int ID { get; set; }
        public int LedgerID { get; set; }
        public string Date { get; set; }
        public string ItemName { get; set; }
        public string Remarks { get; set; }
        public bool isResidence { get; set; }
        public int Quantity { get; set; }
        public int Type { get; set; }
        public string Cdate { get; set; }

    }

    public class PendingDetails
    {
        public string CODE { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Locality { get; set; }
        public string District { get; set; }
        public string State { get; set; }

    }

    public class CommentList
    {
        public int CommentID { get; set; }
        public int ExpenseID { get; set; }
        public string onDate { get; set; }
        public string Comment { get; set; }
        public int User_Id { get; set; }
        public string UserName { get; set; }

    }
}