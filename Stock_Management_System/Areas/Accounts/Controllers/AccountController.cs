using Microsoft.AspNetCore.Mvc;
using Stock_Management_System.Areas.Accounts.Models;
using System.Data;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Newtonsoft.Json;
using Stock_Management_System.Areas.Stocks.Models;
using Stock_Management_System.UrlEncryption;
using ClosedXML.Excel;
using Stock_Management_System.API_Services;
using Customer_Model = Stock_Management_System.Areas.Accounts.Models.Customer_Model;
using static Stock_Management_System.Areas.Invoices.Models.InvoiceModel;
using System.Net;
using Stock_Management_System.Areas.Manage.Models;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Stock_Management_System.Areas.Accounts.Controllers
{

    [Area("Accounts")]
    [Route("~/[controller]/[action]")]
    public class AccountController : Controller
    {



        public IConfiguration Configuration;

        Uri baseaddress = new Uri("https://localhost:7024/api");

        public readonly HttpClient _Client;

        public Api_Service api_Service = new Api_Service();


        public AccountController(IConfiguration configuration)
        {
            Configuration = configuration;
            _Client = new HttpClient();
            _Client.BaseAddress = baseaddress;
        }



        public async Task Dropdown_For_Bank_Names()
        {
            List<Bank_Model> bank_Models = await api_Service.List_Of_Data_Display<Bank_Model>("Bank/Get_Bank_Names");

            if (bank_Models != null)
            {
                // Base URL where the images are hosted, ensure this matches the actual location
                string baseUrl = "https://localhost:7024/";

                // Append the base URL to each bank's icon path
                foreach (var bank in bank_Models)
                {
                    if (!string.IsNullOrEmpty(bank.BankIcon))
                    {
                        bank.BankIcon = baseUrl + bank.BankIcon;
                    }
                }

                ViewBag.Banks = bank_Models;
            }
        }




        #region Method : Create Customer Account

        public IActionResult Create_Customer_Account()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Insert_Customer_Manually(Customer_Model customers)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(customers);
                var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Customers/Insert_Customer", stringContent);

                if (response.IsSuccessStatusCode)
                {

                    return RedirectToAction("Manage_Customers_Account");
                }

            }
            catch
            {
                return null;
            }

            return RedirectToAction("Create_Customer_Account");

        }

        #endregion

        /*
                #region Method : Customer ID Wise Stock Info

                public IActionResult Customer_Wise_Stock_Details(string TN_ID)
                {
                    if (HttpContext.Request.Headers["Referer"].ToString() == "")
                    {
                        return RedirectToAction("Manage_Customers_Account");
                    }



                    AddStockModel customers_wise_stock_model = new AddStockModel();

                    HttpResponseMessage response = _Client.GetAsync($"{_Client.BaseAddress}/Stock/Add_Stock_Details/{UrlEncryptor.Decrypt(TN_ID)}").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string data = response.Content.ReadAsStringAsync().Result;
                        dynamic? jsonObject = JsonConvert.DeserializeObject(data);
                        var DataObject = jsonObject?.data;
                        var extractedDtaJson = JsonConvert.SerializeObject(DataObject, Formatting.Indented);
                        customers_wise_stock_model = JsonConvert.DeserializeObject<AddStockModel>(extractedDtaJson);

                    }
                    return View(customers_wise_stock_model);
                }

                #endregion*/

        #region Method : Update Customer Account

        public async Task<IActionResult> Update_Customer_Info(string Customer_ID)
        {


            Customer_Model customers_Model = new Customer_Model();

            customers_Model = await api_Service.Model_Of_Data_Display<Customer_Model>("Customers/Get_Customer", Convert.ToInt32(UrlEncryptor.Decrypt(Customer_ID)));


            return View(customers_Model);
        }


        public async Task<IActionResult> Update_Customer_Info_Details(Customers_Model customers_Model)
        {


            var jsonContent = JsonConvert.SerializeObject(customers_Model);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _Client.PutAsync($"{_Client.BaseAddress}/Customers/Update_Customer", stringContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Manage_Customers_Account");
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Log or display the responseContent for more detailed error information
                return StatusCode((int)response.StatusCode, responseContent);
            }




        }

        #endregion

        #region Method : Delete Customer And Account Statement 



        #endregion

        #region Method : Convert List To DataTable For Customer Info


        public DataTable Convert_List_To_DataTable_For_Customers_Info_Statement(List<Customer_Model> customers)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Customer-Name", typeof(string));
            dataTable.Columns.Add("Customer-Type", typeof(string));
            dataTable.Columns.Add("Customer-Address", typeof(string));
            dataTable.Columns.Add("Customer-Contact", typeof(string));



            foreach (var customer in customers)
            {
                DataRow row = dataTable.NewRow();

                row["Customer-Name"] = customer.CustomerName;
                row["Customer-Type"] = customer.CustomerType;
                row["Customer-Address"] = customer.CustomerAddress;
                row["Customer-Contact"] = customer.CustomerContact;





                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        public (DataTable, DataTable) Convert_Model_To_DataTable_For_Customers_Account_Details_Statement(CustomerDetails_With_Purchased_Stock_Model customerDetails_With_Purchased_Stock_)
        {


            DataTable Customers_Info = new DataTable();

            DataTable Statements = new DataTable();


            Customers_Info.Columns.Add("Customer-ID", typeof(int));

            Customers_Info.Columns.Add("Customer-Name", typeof(string));

            Customers_Info.Columns.Add("Customer-Type", typeof(string));

            Customers_Info.Columns.Add("Customer-Contact", typeof(string));

            Customers_Info.Columns.Add("Customer-Address", typeof(string));


            DataRow customerRow = Customers_Info.NewRow();
            customerRow["Customer-ID"] = customerDetails_With_Purchased_Stock_.Customers.CustomerId; // Assuming you have CustomerID
            customerRow["Customer-Name"] = customerDetails_With_Purchased_Stock_.Customers.CustomerName;
            customerRow["Customer-Type"] = customerDetails_With_Purchased_Stock_.Customers.CustomerType;
            customerRow["Customer-Contact"] = customerDetails_With_Purchased_Stock_.Customers.CustomerContact;
            customerRow["Customer-Address"] = customerDetails_With_Purchased_Stock_.Customers.CustomerAddress;
            Customers_Info.Rows.Add(customerRow);


            Statements.Columns.Add("Stock-Date", typeof(string));

            Statements.Columns.Add("Product", typeof(string));

            Statements.Columns.Add("Location", typeof(string));
            Statements.Columns.Add("Bags", typeof(string));
            Statements.Columns.Add("Bag-Per-Kg", typeof(string));
            Statements.Columns.Add("Weight", typeof(string));
            Statements.Columns.Add("Total-Price", typeof(string));
            Statements.Columns.Add("Vehicle-Name", typeof(string));
            Statements.Columns.Add("Vehicle-No", typeof(string));
            Statements.Columns.Add("Tolat", typeof(string));
            Statements.Columns.Add("Driver-Name", typeof(string));
            Statements.Columns.Add("Payment-Status", typeof(string));






            foreach (var statement in customerDetails_With_Purchased_Stock_.Purchased_Stocks)
            {
                DataRow statementRow = Statements.NewRow();

                DateTime date = statement.StockDate; // Your original date

                string formattedDate = date.ToString("dd/MM/yyyy"); // Formatting the date

                statementRow["Stock-Date"] = formattedDate;
                statementRow["Product"] = statement.ProductName;
                statementRow["Location"] = statement.PurchaseStockLocation;
                statementRow["Bags"] = statement.Bags.HasValue ? statement.Bags.Value.ToString() : "--";
                statementRow["Bag-Per-Kg"] = statement.BagPerKg.HasValue ? statement.BagPerKg.Value.ToString() : "--";
                statementRow["Weight"] = statement.TotalWeight;
                statementRow["Total-Price"] = statement.TotalPrice;
                statementRow["Vehicle-Name"] = statement.VehicleName;
                statementRow["Vehicle-No"] = statement.VehicleNo;
                statementRow["Tolat"] = statement.TolatName;
                statementRow["Driver-Name"] = statement.DriverName;
                statementRow["Payment-Status"] = string.IsNullOrEmpty(statement.PaymentStatus) ? "--" : statement.PaymentStatus;


                Statements.Rows.Add(statementRow);

            }

            return (Customers_Info, Statements);
        }



        #endregion


        #region Method : Show All Customers Account 

        public async Task<IActionResult> Manage_Customers_Account()
        {


            List<Customer_Model> customers = await api_Service.List_Of_Data_Display<Customer_Model>("Customers/Customers_List");

            return View(customers);
        }


        #endregion


        #region Method : Customer Info With Statements

        public async Task<IActionResult> Account_Details(string Customer_ID, string Customer_Type)
        {
            if (HttpContext.Request.Headers["Referer"].ToString() == "")
            {
                return RedirectToAction("Manage_Customers_Account");
            }

            await Dropdown_For_Bank_Names();

            CustomerDetails_With_Purchased_Stock_Model customerDetails_With_Purchased_Stock = new CustomerDetails_With_Purchased_Stock_Model();

           /*  CustomerDetails_With_Purchased_Stock_Model customerDetails_With_Purchased_Stock = await api_Service.Model_Of_Data_Display<CustomerDetails_With_Purchased_Stock_Model>("Customers/Account_Details", Convert.ToInt32(UrlEncryptor.Decrypt(Customer_ID)),Customer_Type);*/
           HttpResponseMessage response = _Client.GetAsync($"{_Client.BaseAddress}/Customers/Account_Details/{UrlEncryptor.Decrypt(Customer_ID)}&{Customer_Type}").Result;

            string data = await response.Content.ReadAsStringAsync();
            dynamic jsonObject = JsonConvert.DeserializeObject(data);
            var dataObject = jsonObject.data;
            var extractedDataJson = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
            customerDetails_With_Purchased_Stock = JsonConvert.DeserializeObject<CustomerDetails_With_Purchased_Stock_Model>(extractedDataJson);
           

            return View(customerDetails_With_Purchased_Stock);



        }


        #endregion




        #region Method : Customer Accounts PDF And Excel

        public async Task<IActionResult> Customers_Statement_PDF()
        {


            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/Customers_Account_Statement_PDF");

            if (response.IsSuccessStatusCode)
            {
                // Extract filename from Content-Disposition header
                var contentDisposition = response.Content.Headers.ContentDisposition;
                string filename = contentDisposition?.FileName;

                var pdfContent = await response.Content.ReadAsByteArrayAsync();
                return File(pdfContent, "application/pdf", filename);
            }
            else
            {
                // Handle error or return an error response
                return BadRequest("Could not generate PDF.");
            }
        }

        public async Task<IActionResult> Customers_Statement_EXCEL()
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/Customers_Statement_EXCEL");

            if (response.IsSuccessStatusCode)
            {
                // Extract filename from Content-Disposition header
                var contentDisposition = response.Content.Headers.ContentDisposition;
                string filename = contentDisposition?.FileName;

                var pdfContent = await response.Content.ReadAsByteArrayAsync();
                return File(pdfContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
            else
            {
                // Handle error or return an error response
                return BadRequest("Could not generate Excel.");
            }
        }


        #endregion


        #region Method : Accounts Statement PDF And Excel

        public async Task<IActionResult> Customer_Account_Statement_CreatePDF(string Customer_ID,string Customer_Type)
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/Customer_Account_Statement_PDF/{Convert.ToInt32(UrlEncryptor.Decrypt(Customer_ID))}&{Customer_Type}");

            if (response.IsSuccessStatusCode)
            {
                // Extract filename from Content-Disposition header
                var contentDisposition = response.Content.Headers.ContentDisposition;
                string filename = contentDisposition?.FileName;

                var pdfContent = await response.Content.ReadAsByteArrayAsync();
                return File(pdfContent, "application/pdf", filename);
            }
            else
            {
                // Handle error or return an error response
                return BadRequest("Could not generate PDF.");
            }
        }

        public async Task<IActionResult> Customer_Account_Statement_CreateEXCEL(string Customer_ID,string Customer_Type)
        {
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Download/Customer_Account_Statement_EXCEL/{Convert.ToInt32(UrlEncryptor.Decrypt(Customer_ID))}&{Customer_Type}");

            if (response.IsSuccessStatusCode)
            {
                // Extract filename from Content-Disposition header
                var contentDisposition = response.Content.Headers.ContentDisposition;
                string filename = contentDisposition?.FileName;

                var pdfContent = await response.Content.ReadAsByteArrayAsync();
                return File(pdfContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
            else
            {
                // Handle error or return an error response
                return BadRequest("Could not generate Excel.");
            }




        }


        #endregion


        #region Method : Get Data For Payment For Modal

        public async Task<ActionResult> Get_Payment_Info(string Customer_ID, string Stock_ID)
        {
            Payment_All_Models.Payment_Model payment_Model = new Payment_All_Models.Payment_Model();
            // Assuming _Client is properly initialized HttpClient
            HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/Payment/Get_Payment_Info/{UrlEncryptor.Decrypt(Customer_ID)}&{UrlEncryptor.Decrypt(Stock_ID)}");

            if (response.IsSuccessStatusCode)
            {
                await Dropdown_For_Bank_Names();

                string data = await response.Content.ReadAsStringAsync();
                dynamic jsonObject = JsonConvert.DeserializeObject(data);
                var dataObject = jsonObject.data;
                var extractedDataJson = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
                payment_Model = JsonConvert.DeserializeObject<Payment_All_Models.Payment_Model>(extractedDataJson);



                return PartialView("_PaymentInfo_Box", payment_Model);
            }
            else
            {
                return null;
            }






            #endregion





        }
    }
}
