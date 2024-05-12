using Newtonsoft.Json;
using Stock_Management_System.Areas.Accounts.Models;
using System.ComponentModel.DataAnnotations;

namespace Stock_Management_System.Areas.Sales.Models
{
    public class SaleModel
    {
        public int SaleId { get; set; }

        public DateTime Create_Sales { get; set; }

        [Required(ErrorMessage ="Please Select Product")]
        public int Product_Id { get; set; }

        [Required(ErrorMessage ="Please Select Receive Payment Date")]
        public DateTime Receive_Payment_Date { get; set; }


        public string? Product_Name { get; set; }

        [Required(ErrorMessage ="Please Enter Brand Name")]
        public string? Brand_Name { get; set; }

        [Required(ErrorMessage ="Plese Select Payment Method")]
        public string? Payment_Method { get; set; }

        public int Receive_Bank_Id { get; set; }


        [Required(ErrorMessage ="Please Select Bank")]
        public int? Receive_Information_Id { get; set; }


        public decimal? Bags { get; set; }

        public decimal? BagPerKg { get; set; }

        [Required(ErrorMessage ="Please Enter Weight or Enter Bag and Bag Per Kg Get Weight")]
        public decimal? Total_Weight { get; set; }

        [Required(ErrorMessage ="Please Enter Rate")]
        public decimal Rate { get; set; }

        [Required(ErrorMessage ="Total Price Not Empty")]
        public decimal Total_Price { get; set; }

        [Required(ErrorMessage ="Please Enter Receive Amount")]
        public decimal Receive_Amount { get; set; }


        public decimal? CGST { get; set; }

        public decimal? SGST { get; set; }

        public decimal? TotalCGSTPrice { get; set; }

        public decimal? TotalSGSTPrice { get; set; }

        public decimal? WithoutGSTPrice { get; set; }


        public decimal? Discount { get; set; }

        public decimal? Deducted_Amount { get; set; }

        [Required(ErrorMessage ="Please Select Full Payment Receive or Not !")]
        public string? IsFullPaymentReceive { get; set; }


        [Required(ErrorMessage ="Please Select Remain Payment Date")]
        public DateTime? Remain_Payment_Date { get; set; }
        [Required(ErrorMessage = "Please Enter Remain Payment Amount")]
        public decimal? Receive_Remain_Amount { get; set; }
        [Required(ErrorMessage = "Please Select Remain Payment Method")]
        public string? Remain_Payment_Method { get; set; }


        public int? Remain_Amount_Receive_Bank_Id { get; set; }

        public int? Remain_Infromation_ID { get; set; }




    }

    public class Sale_Customer_Combied_Model
    {
        [JsonProperty("customer")]
        public Customer_Model customer { get; set; }

        [JsonProperty("sale")]
        public SaleModel sale { get; set; }
    }

    public class Show_Sale
    {
        public int saleId { get; set; }
        public DateTime Create_Sales { get; set; }
        public int Product_Id { get; set; }

        public int? CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerType { get; set; }


        public DateTime Receive_Payment_Date { get; set; }

        public string? Product_Name { get; set; }

        public string? Brand_Name { get; set; }

        public string? Payment_Method { get; set; }

        public int? Receive_Information_ID { get; set; }

        public string? Receive_Account_No { get; set; }

        public string? Account_Holder_Name { get; set; }

        public string? Bank_Icon { get; set; }

        public decimal? Bags { get; set; }

        public decimal? BagPerKg { get; set; }


        public decimal? Total_Weight { get; set; }

        public decimal Rate { get; set; }

        public decimal? CGST { get; set; }

        public decimal? SGST { get; set; }

        public decimal? TotalCGSTPrice { get; set; }

        public decimal? TotalSGSTPrice { get; set; }

        public decimal? WithoutGSTPrice { get; set; }

        public decimal Total_Price { get; set; }

        public decimal Receive_Amount { get; set; }

        public decimal? Discount { get; set; }

        public decimal? Deducted_Amount { get; set; }

        public string? IsFullPaymentReceive { get; set; }

        public DateTime? Remain_Payment_Date { get; set; }

        public decimal? Receive_Remain_Amount { get; set; }

        public string? Remain_Payment_Method { get; set; }

        public int? Remain_Receive_Information_ID { get; set; }

        public string? Receive_Remain_Account_No { get; set; }

        public string? Remain_Account_Holder_Name { get; set; }

        public string? Remain_Bank_Icon { get; set; }

    }
}
