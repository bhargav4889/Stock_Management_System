using static Stock_Management_System.Areas.Manage.Models.Payment_All_Models;

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

            public string? Payment_Status { get; set; }


        }


        public class Remain_Payment_Model
        {
            public int Remain_Id { get; set; }

            public int Stock_Id { get; set; }

            public int Payment_Id { get; set; }

            public int Customer_Id { get; set; }

            public int Product_Id { get; set; }

            public string? Product_Name { get; set; }

            public string? Customer_Name { get; set; }

            public DateTime Payment_Date { get; set; }

            public DateTime Remain_Payment_Date { get; set; }

            public decimal Total_Amount { get; set; }

            public decimal Paid_Amount { get; set; }
            public decimal Remain_Amount { get; set; }

            public decimal Pay_Amount { get; set; }

            public string? First_Payment_Method { get; set; }
            public string? Remain_Payment_Method { get; set; }

            public int Bank_Id { get; set; }


            public string? Bank_Name { get; set; }


            public string? Bank_Ac_No { get; set; }

            public string? Bank_Icon { get; set; }

            public string? CHEQ_NO { get; set; }


            public string? RTGS_No { get; set; }

            public int Remain_Bank_Id { get; set; }


            public string? Remain_Bank_Name { get; set; }


            public string? Remain_Bank_Ac_No { get; set; }

            public string? Remain_Bank_Icon { get; set; }

            public string? Remain_CHEQ_NO { get; set; }


            public string? Remain_RTGS_No { get; set; }

            public string? Remain_Payment_Status { get; set; }


        }

        public class Show_Payment_Info
        {
            // Customer information
            public int CustomerID { get; set; }
            public string CustomerName { get; set; }

            // Payment information
            public int PaymentID { get; set; }
            public DateTime PaymentDate { get; set; }
            public int ProductID { get; set; }
            public int StockID { get; set; }
            public decimal AmountPaid { get; set; }
            public string PaymentMethod { get; set; }
            public int? BankID { get; set; }
            public string BankAcNo { get; set; }
            public string CheqNo { get; set; }
            public string RtgsNo { get; set; }

            // Product information
            public string ProductName { get; set; }

            // Purchase Stock information
            public int PurStockID { get; set; }
            public decimal TotalPrice { get; set; }

            // Bank information
            public string BankName { get; set; }
            public string BankIcon { get; set; }

            // Remain Payment information
            public int RemainPaymentID { get; set; }

            public string RemainBankName { get; set; }
            public DateTime? RemainPaymentDate { get; set; }
            public decimal RemainPaymentAmount { get; set; }
            public string RemainPaymentMethod { get; set; }

            public string RemainBankIcon { get; set; }

            public int RemainBankID { get; set; }
            public string RemainBankAcNo { get; set; }
            public string RemainCheqNo { get; set; }
            public string RemainRtgsNo { get; set; }

            public string Payment_Status { get; set; }


        }

        public class Pending_Customers_Payments
        {
            public int PaymentId { get; set; }

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
