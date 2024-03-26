using Microsoft.AspNetCore.Mvc.Rendering;
using Stock_Management_System.API_Services;

namespace Stock_Management_System.All_DropDowns
{
    public class All_DropDowns_Class
    {
        Api_Service api_Service = new Api_Service();

        public async Task<All_DropDown_Model> Get_All_DropdDowns_Data()
        {

            All_DropDown_Model all_DropDowns = await api_Service.Model_Of_Data_Display<All_DropDown_Model>("All_DropDowns");

            return all_DropDowns;

        }
    }
}
