namespace Stock_Management_System.Areas.Manage.Models
{
    public class Payment_All_Models
    {

        public class Payment_Model
        {
            public int Stock_Id { get; set; }

            public DateTime Stock_Added_Date { get; set; }

            public DateTime Payment_Date { get; set; }

            public int Customer_Id { get; set; }

            public string? Customer_Name { get; set; }

            public string? Customer_Type { get; set; }

            public int Product_Id { get; set; }

            public string? Product_Name { get; set; }

            public decimal Total_Price { get; set; }

            public string? Stock_Loaction { get; set; }

            public decimal Paid_Amount { get; set; }

            public decimal Remain_Amount { get; set; }

            public string? Payment_Method { get; set; }

            public int? Bank_Id { get; set; }

            public string? Bank_Name { get; set; }

            public string? Bank_Ac_No { get; set; }

            public string? CHEQ_No { get; set; }


            public string? RTGS_No { get; set; }




        }

        


        public class Pending_Customers_Payments
        {
            public int StockId { get; set; }

            public DateTime StockDate { get; set; }

            public int CustomerId { get; set; }

            public string? CustomerName { get; set; }

            public int ProductId { get; set; }

            public string? ProductName { get; set; }

            public string? Location { get; set; }
            public decimal TotalPrice { get; set; }

            public string? Payment_Status { get; set; }
        }




    }


}
