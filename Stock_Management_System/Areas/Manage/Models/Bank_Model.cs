namespace Stock_Management_System.Areas.Manage.Models
{
    public class Bank_Model
    {
        
            public int BankId { get; set; }

            public string BankName { get; set; }

            public string BankIcon { get; set; }
        
    }

    public class Our_Banks_Dropdown
    {
        public int BankId { get; set; }

        public int InformationId { get; set; }

        public string? AccountNo { get; set; }

        public string? BankIcon { get; set; }

    }
}
