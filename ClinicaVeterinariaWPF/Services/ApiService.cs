using ClinicaVeterinariaWPF.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaVeterinariaWPF.Services
{
    public class ApiService
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<Response> GetAllAsync<T>(string urlBase, string controller)
        {
            try
            {
                client.BaseAddress = new Uri(urlBase);

                var response = await client.GetAsync(controller);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result
                    };
                }

                var jsonResult = JsonConvert.DeserializeObject<List<T>>(result);

                return new Response
                {
                    IsSuccess = true,
                    Result = jsonResult
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response> GetIdAsync<T>(string urlBase, string controller, int id)
        {
            try
            {
                client.BaseAddress = new Uri(urlBase);

                var response = await client.GetAsync(controller + "/" + id);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result
                    };
                }

                var jsonResult = JsonConvert.DeserializeObject<T>(result);

                return new Response
                {
                    IsSuccess = true,
                    Result = jsonResult
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response> PostAsync<T>(string urlBase, string controller, T item)
        {
            try
            {
                client.BaseAddress = new Uri(urlBase);

                string jsonResult = JsonConvert.SerializeObject(item);
                var content = new StringContent(jsonResult, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(controller, content);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result
                    };
                }

                return new Response
                {
                    IsSuccess = true,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response> PutAsync<T>(string urlBase, string controller, T item, int id)
        {
            try
            {
                client.BaseAddress = new Uri(urlBase);

                string jsonResult = JsonConvert.SerializeObject(item);
                var content = new StringContent(jsonResult, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(controller + "/" + id, content);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result
                    };
                }

                return new Response
                {
                    IsSuccess = true,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response> DeleteAsync(string urlBase, string controller, int id)
        {
            try
            {
                client.BaseAddress = new Uri(urlBase);

                var response = await client.DeleteAsync(controller + "/" + id);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result
                    };
                }

                return new Response
                {
                    IsSuccess = true,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
