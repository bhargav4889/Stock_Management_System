using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;

namespace Stock_Management_System.Areas.Manage.Models
{
    public class Dashboard_All_Models
    {


        public AllCountsModel All_Counts_Model { get; set; }


        public IEnumerable<Pending_Customers_Payment_Sort_List> Pending_Customers_Payment_Sort_List { get; set; }


        public IEnumerable<Recent_Action_Model> recent_Actions_With_Info { get; set; }

        public IEnumerable <Upcoming_Reminders_Model> upcoming_Reminders { get; set; }

    }


    public class AllCountsModel
    {
        [JsonProperty("totalCustomers")]
        public int TotalCustomers { get; set; }

        [JsonProperty("totalBags")]
        public int TotalBags { get; set; }

        [JsonProperty("totalWeight")]
        public decimal TotalWeight { get; set; }

        [JsonProperty("amountOfPurchasedStock")]
        public decimal AmountOfPurchasedStock { get; set; }

        [JsonProperty("totalPurchaseInvoice")]
        public int TotalPurchaseInvoice { get; set; }

        [JsonProperty("amountOfPurchaseInvoice")]
        public decimal AmountOfPurchaseInvoice { get; set; }

        [JsonProperty("totalSalesInvoice")]
        public int TotalSalesInvoice { get; set; }

        [JsonProperty("amountOfSalesInvoice")]
        public decimal AmountOfSalesInvoice { get; set; }

        [JsonProperty("totalPayments")]
        public int TotalPayments { get; set; }

        [JsonProperty("paidPayments")]
        public int PaidPayments { get; set; }

        [JsonProperty("remainPayments")]
        public int RemainPayments { get; set; }

        [JsonProperty("pendingPayments")]
        public int PendingPayments { get; set; }

        [JsonProperty("amountOfPaidPayments")]
        public decimal AmountOfPaidPayments { get; set; }

        [JsonProperty("amountOfRemainingPayments")]
        public decimal AmountOfRemainingPayments { get; set; }

        [JsonProperty("amountOfPendingPayments")]
        public decimal AmountOfPendingPayments { get; set; }

        [JsonProperty("amountOfSaleStock")]
        public decimal AmountOfSaleStock { get; set; }
    }


    public class Pending_Customers_Payment_Sort_List
    {
        [JsonPropertyName("stockId")]
        public int StockId { get; set; }

        [JsonPropertyName("stockDate")]
        public DateTime StockDate { get; set; }

        [JsonPropertyName("customerId")]
        public int CustomerId { get; set; }

        [JsonPropertyName("customerName")]
        public string? CustomerName { get; set; }

        [JsonPropertyName("customerType")]
        public string? CustomerType { get; set; }

        [JsonPropertyName("productId")]
        public int ProductId { get; set; }

        [JsonPropertyName("productName")]
        public string? ProductName { get; set; }

        [JsonPropertyName("totalPrice")]
        public decimal TotalPrice { get; set; }
    }


    public class Recent_Action_Model
    {
        public int Rec_Act_Id { get; set; }

        public string? Rec_Act_Table_Name { get; set; }

        public string? Rec_Act_Info { get; set; }

        public DateTime Rec_Act_Create { get; set; }
    }


    public class Upcoming_Reminders_Model
    {
        public int ReminderId { get; set; }


   
        public DateTime ReminderDateTime { get; set; }

     
        public string? ReminderType { get; set; }

    
        public string? ReminderCustomType { get; set; }

      
        public string? ReminderDescription { get; set; }

        public string? SentEmailAddress { get; set; }

        public string? SentPhoneNo { get; set; }
    }

}



