namespace Stock_Management_System.All_DropDowns
{
    public class All_DropDown_Model
    {
        
        public All_DropDown_Model()
        {

            Products_DropDowns_List = new List<Product_DropDown_Model>();

            Products_Grade_DropDowns_List = new List<Product_Grade_DropDown_Model>();

            Vehicle_DropDowns_List = new List<Vehicle_DropDown_Model>();

        }

        public List<Product_DropDown_Model> Products_DropDowns_List { get; set; }

        public List<Product_Grade_DropDown_Model> Products_Grade_DropDowns_List { get; set; }

        public List<Vehicle_DropDown_Model> Vehicle_DropDowns_List { get; set; }
    }

    public class Product_DropDown_Model
    {
        public int ProductId { get; set; }

        public string? ProductNameInGujarati { get; set; }


        public string? ProductNameInEnglish { get; set; }

    }




    public class Product_Grade_DropDown_Model
    {
        public int ProductGradeId { get; set; }

        public string? ProductGrade { get; set; }

    }


    public class Vehicle_DropDown_Model
    {
        public int VehicleId { get; set; }

        public string? VehicleName { get; set; }

    }

   


}
