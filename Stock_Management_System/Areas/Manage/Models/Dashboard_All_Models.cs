using System.Data;

namespace Stock_Management_System.Areas.Manage.Models
{
    public class Dashboard_All_Models
    {


        public All_Counts_Model All_Counts_Model { get; set; }


        public IEnumerable<Pending_Customers_Payment_Sort_List> Pending_Customers_Payment_Sort_List { get; set; }


        public IEnumerable<Recent_Action_Model> recent_Actions_With_Info { get; set; }



    }

    public class All_Counts_Model
    {
        public int TotalCustomers { get; set; }
        public int TotalBags { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal AmountOfPurchasedStock { get; set; }
        public int TotalPurchaseInvoice { get; set; }
        public decimal AmountOfPurchaseInvoice { get; set; }
        public int TotalSalesInvoice { get; set; }
        public decimal AmountOfSalesInvoice { get; set; }

        public int TotalPayments { get; set; }

        public int PaidPayments { get; set; }

        public int RemainPayments { get; set; }
        public int PendingPayments { get; set; }
        public decimal AmountOfPaidPayments { get; set; }

        public decimal AmountOfPendingPayments { get; set; }




    }





    public class Pending_Customers_Payment_Sort_List

    {

        public int StockId { get; set; }

        public DateTime StockDate { get; set; }

        public int CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public int ProductId { get; set; }

        public string? ProductName { get; set; }
        public decimal TotalPrice { get; set; }
    }


    public class Recent_Action_Model
    {
        public int Rec_Act_Id { get; set; }

        public string? Rec_Act_Table_Name { get; set; }

        public string? Rec_Act_Info { get; set; }

        public DateTime Rec_Act_Create { get; set; }
    }



}



