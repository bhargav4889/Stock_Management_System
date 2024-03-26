using System.Data;
using static Stock_Management_System.Areas.Invoices.Models.InvoiceModel;

namespace Stock_Management_System.Areas.Invoices.Models
{
    public class MutipleModel
    {

        public byte[]? Pdf { get; set; }

        public Sales_Invoice_Model? sales_Invoice { get; set; }

        public Purchase_Invoice_Model? purchase_Invoice { get; set; }

           
    }

  
}
