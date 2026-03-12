using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocationTrackingAPI.Models
{
    public class Expense
    {
        public int LastApprovedByID { get; set; }
        public int ExpenseID { get; set; }
        public int LedgerID { get; set; }
        
        public string LedgerName { get; set; }
        
        public int VoucherNo { get; set; }
        
        public string ExpDate { get; set; }
        
        public int Approved { get; set; }
        public int Status { get; set; }
        public double Amount { get; set; }
        public string BillFromDate { get; set; }
        public string BillToDate { get; set; }
        public string ClaimAmount { get; set; }
        public string PassingAmount { get; set; }


    }
}