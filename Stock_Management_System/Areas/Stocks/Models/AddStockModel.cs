
using Newtonsoft.Json;
using Stock_Management_System.Areas.Accounts.Models;
using System.ComponentModel.DataAnnotations;

namespace Stock_Management_System.Areas.Stocks.Models
{
   
    public class Purchase_Stock
    {
        public int PurchaseStockId { get; set; }
        public DateTime PurchaseStockDate { get; set; }
        public int? CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerAddress { get; set; }

        public string? CustomerType { get; set; }


        [Required]
        public int ProductId { get; set; }


        public string? ProductName { get; set; }
        public int? ProductGradeId { get; set; }
        public string? ProductGrade { get; set; }

        [Required]
        public string? PurchaseStockLocation { get; set; }
        public decimal? Bags { get; set; }
        public decimal? BagPerKg { get; set; }

        [Required]
        public decimal? TotalWeight { get; set; }

        [Required]
        public decimal ProductPrice { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public int VehicleId { get; set; }
        public string? VehicleName { get; set; }

        [Required]
        public string? VehicleNo { get; set; }

        [Required]
        public string? DriverName { get; set; }

        [Required]
        public string? TolatName { get; set; }
        public string? PaymentStatus { get; set; }
    }


    public class Show_Purchase_Stock
    {
        public int PurchaseStockId { get; set; }
        public DateTime PurchaseStockDate { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerType { get; set; }

        public string? CustomerAddress { get; set; }

        public string? CustomerPhoneNo { get; set; }

        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? ProductGradeId { get; set; }
        public string? ProductGrade { get; set; }
        public string? PurchaseStockLocation { get; set; }
        public decimal? Bags { get; set; }
        public decimal? BagPerKg { get; set; }
        public decimal? TotalWeight { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int VehicleId { get; set; }
        public string? VehicleName { get; set; }
        public string? VehicleNo { get; set; }
        public string? DriverName { get; set; }
        public string? TolatName { get; set; }
        public string? PaymentStatus { get; set; }
    }

    public class Insert_And_Update_Purchase_Stock
    {
        public int PurchaseStockId { get; set; }
        public DateTime PurchaseStockDate { get; set; }

        [Required(ErrorMessage ="Please Select Product")]
        public int ProductId { get; set; }
        public int? ProductGradeId { get; set; }
        public string? PurchaseStockLocation { get; set; }
        public decimal? Bags { get; set; }
        public decimal? BagPerKg { get; set; }

        [Required(ErrorMessage = "Please Enter Weight Or Enter Bag And Bags Per Kg And Get Weight")]
        public decimal? TotalWeight { get; set; }

        [Required(ErrorMessage = "Please Enter Product Price")]
        public decimal ProductPrice { get; set; }

        [Required(ErrorMessage = "Please Enter Total Price")]
        public decimal TotalPrice { get; set; }

        [Required(ErrorMessage = "Please Select Vehicle Type")]
        public int VehicleId { get; set; }
        public string? VehicleName { get; set; }

        [Required(ErrorMessage = "Please Enter Vehicle No")]
        public string? VehicleNo { get; set; }

        [Required(ErrorMessage = "Please Enter Driver Name")]
        public string? DriverName { get; set; }

        [Required(ErrorMessage = "Please Enter Tolat Name")]
        public string? TolatName { get; set; }
        public string? PaymentStatus { get; set; }
    }

    public class Customers_Model
    {
        public int CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerType { get; set; }

        public string? CustomerContact { get; set; }

        public string? CustomerAddress { get; set; }
    }


    public class Customers_Stock_Combined_Model
    {
        [JsonProperty("purchase_Stock")]
        public Insert_And_Update_Purchase_Stock Insert_Purchase_Stock { get; set; }

        [JsonProperty("customers_Model")]
        public Customer_Model Customers { get; set; }



    }

}
