using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stock_Management_System.UrlEncryption;
using Stock_Management_System.BAL;
using System.Net.Http.Headers;

namespace Stock_Management_System.API_Services
{
    public class Api_Service
    {
        Uri baseaddress = new Uri("https://localhost:7024/api");

        private readonly HttpClient _Client;
        private readonly CV _cv;


        public Api_Service()
        {
            _Client = new HttpClient();
            _Client.BaseAddress = baseaddress;
            _cv = new CV();
        }



        // Common Function Of List Of Data Display

        #region Common Function Of List Of Data Display

        public async Task<List<T>> List_Of_Data_Display<T>(string requestUri, int reuqestId = 0)
        {


            try
            {
                if (reuqestId == 0)
                {
                

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


            try
            {

                if (requestedId == 0)
                {

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
            catch
            {
                throw new Exception();
            }
        }





        #endregion





    }
}
