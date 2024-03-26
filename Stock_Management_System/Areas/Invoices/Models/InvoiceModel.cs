using System.ComponentModel.DataAnnotations;

namespace Stock_Management_System.Areas.Invoices.Models
{
    public class InvoiceModel
    {
        #region Purchase_Invoice_Model

      

        #region Purchase_Invoice_Model

        public class Purchase_Invoice_Model
        {
            public int PurchaseInvoiceId { get; set; }
            public DateTime PurchaseInvoiceDate { get; set; }

            [Required(ErrorMessage = "Please Enter Customer Name")]
            public string? CustomerName { get; set; }

            [Required(ErrorMessage = "Please Select Product Type")]
            public int ProductId { get; set; }
            public string? ProductName { get; set; }
            public int? ProductGradeId { get; set; }
            public string? ProductGrade { get; set; }


            public decimal? Bags { get; set; }

            public decimal? BagPerKg { get; set; }

            [Required(ErrorMessage ="Please Enter Weight")]
            public decimal TotalWeight { get; set; }

            [Required(ErrorMessage = "Please Enter Product Price")]
            public decimal ProductPrice { get; set; }

            [Required(ErrorMessage = "Please Enter Total Price")]
            public decimal TotalPrice { get; set; }

            [Required(ErrorMessage = "Please Select Vehicle")]
            public int VehicleId { get; set; }
            public string? VehicleName { get; set; }

            [Required(ErrorMessage = "Please Enter Vehicle NO")]
            public string? VehicleNo { get; set; }

            [Required(ErrorMessage = "Please Enter Driver Name")]
            public string? DriverName { get; set; }

            [Required(ErrorMessage = "Please Enter Tolat Name")]
            public string? TolatName { get; set; }


        }

        #endregion

        #endregion

        #region Sell_Invoice_Model
        public class Sell_InvoiceModel
        {
            public int INVOICE_ID { get; set; }

            public DateTime? InvoiceDate { get; set; }


            [Required(ErrorMessage = "Please Enter Patry Name")]
            public string? Party_Name { get; set; }


            [Required(ErrorMessage = "Please Enter Patry Address")]
            public string? Party_Address { get; set; }


            public string? OtherInvoiceType { get; set; }


            public string? Party_GSTNO { get; set; }


            public double? SGST { get; set; }

            public double? CGST { get; set; }


            public double? SGST_Total { get; set; }

            public double? CGST_Total { get; set; }

            [Required(ErrorMessage = "Please Enter Product Brand Name")]
            public string? Product_Brand_Name { get; set; }


            [Required(ErrorMessage = "Please Select Grain Type")]
            public string? GrainTypeName { get; set; }

            [Required(ErrorMessage = "Please Select Invoice Type")]
            public string? InvoiceType { get; set; }


            public string? Bag { get; set; }
            public string? BagPerKg { get; set; }

            [Required(ErrorMessage = "Please Enter Weight !")]
            public double Weight { get; set; }

            [Required(ErrorMessage = "Please Enter Product Price !")]
            public double ProductPrice { get; set; }


            public double Before_Tax_Total_Price { get; set; }


            [Required(ErrorMessage = "Please Calculate Total Price!")]
            public double TotalPrice { get; set; }

            [Required(ErrorMessage = "Please Select Vehicle Type")]
            public string? VehicleTypeName { get; set; }

            [Required(ErrorMessage = "Please Enter Vehicle No ! ")]
            public string? VehicleNo { get; set; }

            [Required(ErrorMessage = "Please Enter Driver Name")]
            public string? DriverName { get; set; }


            public string? Broker_Name { get; set; }


            public string? Container_Number { get; set; }





        }

        #endregion

        #region Sell_Invoice_Model

        public class Sales_Invoice_Model
        {
            public int SalesInvoiceId { get; set; }
            public DateTime SalesInvoiceDate { get; set; }

            public string? BrokerName { get; set; }

            public string? PartyName { get; set; }

            public string? PartyGstNo { get; set; }

            public string? PartyAddress { get; set; }

            public string? OtherInvoiceType { get; set; }

            public string? InvoiceType { get; set; }


            public int ProductId { get; set; }
            public string? ProductName { get; set; }

            public string? ProductBrandName { get; set; }

            public decimal? Bags { get; set; }
            public decimal? BagPerKg { get; set; }
            public decimal TotalWeight { get; set; }

            public decimal ProductPrice { get; set; }

            public decimal? CGST { get; set; }

            public decimal? SGST { get; set; }

            public decimal? TotalCGSTPrice { get; set; }

            public decimal? TotalSGSTPrice { get; set; }

            public decimal? WithoutGSTPrice { get; set; }


            public decimal TotalPrice { get; set; }
            public int VehicleId { get; set; }
            public string? VehicleName { get; set; }
            public string? VehicleNo { get; set; }

            public string? ContainerNo { get; set; }

            public string? DriverName { get; set; }

        }


        #endregion

    }
}
