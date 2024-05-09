using Microsoft.AspNetCore.Mvc.Rendering;
using Stock_Management_System.API_Services;

namespace Stock_Management_System.All_DropDowns
{
    public class DropDowns_Class
    {
        private readonly Api_Service api_Service = new Api_Service();

        public async Task<DropDown_Model> GetAllDropdownsAsync()
        {

            DropDown_Model all_DropDowns = await api_Service.Model_Of_Data_Display<DropDown_Model>("DropDowns");

            return all_DropDowns;

        }
    }
}
