using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stock_Management_System.UrlEncryption;
using Stock_Management_System.BAL;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Stock_Management_System.API_Services
{
    public class Api_Service
    {
        Uri baseaddress = new Uri("https://stock-manage-api-shree-ganesh-agro-ind.somee.com/api");

        private readonly HttpClient _Client;

        private readonly HttpContextAccessor _HttpContextAccessor;
       


        public Api_Service()
        {
            _Client = new HttpClient();
            _Client.BaseAddress = baseaddress;
            _HttpContextAccessor = new HttpContextAccessor();
            
        }

        


        // Common Function Of List Of Data Display

        #region Common Function Of List Of Data Display

        public async Task<List<T>> List_Of_Data_Display<T>(string requestUri, int reuqestId = 0)
        {


            try
            {
                if (reuqestId == 0)
                {


                    _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

                    HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/{requestUri}");


                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        dynamic jsonObject = JsonConvert.DeserializeObject(data);
                        var dataObject = jsonObject.data;
                        var extractedDataJson = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
                        var models = JsonConvert.DeserializeObject<List<T>>(extractedDataJson);
                        return models;

                    }
                    else
                    {

                        return null;
                    }
                }
                else
                {

                    _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

                    HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/{requestUri}/{reuqestId}");



                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        dynamic jsonObject = JsonConvert.DeserializeObject(data);
                        var dataObject = jsonObject.data;
                        var extractedDataJson = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
                        var models = JsonConvert.DeserializeObject<List<T>>(extractedDataJson);
                        return models;

                    }
                    else
                    {

                        return null;
                    }
                }




            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<T> Model_Of_Data_Display<T>(string requestUri, int? requestedId = 0)
        {


            if (requestedId == 0)
            {

                _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

                HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/{requestUri}");


                if (response.IsSuccessStatusCode)
                {

                    string data = await response.Content.ReadAsStringAsync();
                    // Directly deserialize the JSON string into type T.
                    T model = JsonConvert.DeserializeObject<T>(data);
                    return model;


                }
                else
                {

                    return default;
                }
            }
            else
            {
                _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _HttpContextAccessor.HttpContext.Session.GetString("JWT_Token"));

                HttpResponseMessage response = await _Client.GetAsync($"{_Client.BaseAddress}/{requestUri}/{requestedId}");


                if (response.IsSuccessStatusCode)
                {

                    string data = await response.Content.ReadAsStringAsync();
                    dynamic jsonObject = JsonConvert.DeserializeObject(data);
                    var dataObject = jsonObject.data;
                    var extractedDataJson = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
                    var models = JsonConvert.DeserializeObject<T>(extractedDataJson);
                    return models;


                }
                else
                {

                    return default;
                }

            }

        }





        #endregion





    }
}
