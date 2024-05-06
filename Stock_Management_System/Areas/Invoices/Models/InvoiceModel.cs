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

            public string? TolatName { get; set; }


        }

        #endregion

        #endregion

        #region Sales Invoice

        public class Sales_Invoice_Model
        {
            public int SalesInvoiceId { get; set; }
            public DateTime SalesInvoiceDate { get; set; }

            public string? BrokerName { get; set; }

            [Required(ErrorMessage ="Please Enter Party Name")]
            public string? PartyName { get; set; }

            public string? PartyGstNo { get; set; }

            public string? PartyAddress { get; set; }

            public string? OtherInvoiceType { get; set; }

            [Required(ErrorMessage ="Please Select Invoice Type")]
            public string? InvoiceType { get; set; }

            [Required(ErrorMessage ="Plese Select Product")]
            public int ProductId { get; set; }

      
            public string? ProductName { get; set; }

            [Required(ErrorMessage = "Please Enter Product Brand Name")]
            public string? ProductBrandName { get; set; }

            public decimal? Bags { get; set; }
            public decimal? BagPerKg { get; set; }

            [Required(ErrorMessage ="Please Enter Weight or Enter Bag and Bag Per Kg To Get Weight ")]
            public decimal TotalWeight { get; set; }

            [Required(ErrorMessage ="Please Enter Product Price")]
            public decimal ProductPrice { get; set; }

            public decimal? CGST { get; set; }

            public decimal? SGST { get; set; }

            public decimal? TotalCGSTPrice { get; set; }

            public decimal? TotalSGSTPrice { get; set; }

            public decimal? WithoutGSTPrice { get; set; }

            [Required(ErrorMessage="Total Price Not Empty")]
            public decimal TotalPrice { get; set; }

            [Required(ErrorMessage ="Please Select Vehicle Type")]
            public int VehicleId { get; set; }
            public string? VehicleName { get; set; }
            public string? VehicleNo { get; set; }

            public string? ContainerNo { get; set; }

            [Required(ErrorMessage ="Please Enter Driver Name")]
            public string? DriverName { get; set; }

        }


        #endregion

    }
}
