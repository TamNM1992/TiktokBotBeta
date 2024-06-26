using Newtonsoft.Json;
using SmartBot.Common;
using SmartBot.Common.Configuration;
using SmartBot.Common.Extention;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;

namespace SmartBot.Service.Api
{
    public class ApiServiceBase
    {
        public virtual ResponseBase<T> Get<T>(string url, params KeyValuePair<string, object>[] args)
        {
            try
            {
                string param = "";
                if (args.Length > 0)
                {
                    param = "?";

                    for (int i = 0; i < args.Count(); i++)
                    {
                        if (i == 0)
                        {
                            param += $"{args[i].Key}={args[i].Value}";
                        }
                        else
                        {
                            param += $"&{args[i].Key}={args[i].Value}";
                        }

                    }
                }
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(@$"{AppConfigs.ApiUrlBase}/{url}{param}");
                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(


                new MediaTypeWithQualityHeaderValue("application/json"));

                // List data response.
                HttpResponseMessage response = client.GetAsync("").Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<ResponseBase<T>>(str)!;
                    if (result != null)
                    {
                        return result;
                    }
                }
                else
                {
                    return new ResponseBase<T>
                    {
                        Code = (int)response.StatusCode,
                        Message = response.StatusCode.GetEnumDescription()
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseBase<T>
                {
                    Code = 99,
                    Message = $"Service base exception :{ex.Message}"
                };
                // Log error
            }

            return default(ResponseBase<T>);
        }

        public virtual ResponseBase<T> Delete<T>(string url, params KeyValuePair<string, object>[] args)
        {
            try
            {
                string param = "";
                if (args.Length > 0)
                {
                    param = "?";

                    for (int i = 0; i < args.Count(); i++)
                    {
                        if (i == 0)
                        {
                            param += $"{args[i].Key}={args[i].Value}";
                        }
                        else
                        {
                            param += $"&{args[i].Key}={args[i].Value}";
                        }

                    }
                }
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(@$"{AppConfigs.ApiUrlBase}/{url}{param}");
                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                // List data response.
                HttpResponseMessage response = client.DeleteAsync("").Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<ResponseBase<T>>(str)!;
                    if (result != null)
                    {
                        return result;
                    }
                }
                else
                {
                    return new ResponseBase<T>
                    {
                        Code = (int)response.StatusCode,
                        Message = response.StatusCode.GetEnumDescription()
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseBase<T>
                {
                    Code = 99,
                    Message = $"Service base exception :{ex.Message}"
                };
                // Log error
            }

            return default(ResponseBase<T>);
        }

        public virtual ResponseBase<Tout> Post<Tin, Tout>(string url, Tin body, params string[] args)
        {
            try
            {
                string Serialized = JsonConvert.SerializeObject(body);
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpContent content = new StringContent(Serialized, Encoding.Unicode, "application/json");
                var response = client.PostAsync(@$"{AppConfigs.ApiUrlBase}/{url}", content).Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<ResponseBase<Tout>>(str)!;
                    if (result != null)
                    {
                        return result;
                    }
                }
                else
                {
                    return new ResponseBase<Tout>
                    {
                        Code = (int)response.StatusCode,
                        Message = response.StatusCode.GetEnumDescription()
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseBase<Tout>
                {
                    Code = 99,
                    Message = $"Service base exception :{ex.Message}"
                };
                // Log error
            }

            return default(ResponseBase<Tout>);
        }

        public virtual ResponseBase<Tout> Put<Tin, Tout>(string url, Tin body, params string[] args)
        {
            try
            {
                string Serialized = JsonConvert.SerializeObject(body);
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpContent content = new StringContent(Serialized, Encoding.Unicode, "application/json");
                var response = client.PutAsync(@$"{AppConfigs.ApiUrlBase}/{url}", content).Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<ResponseBase<Tout>>(str)!;
                    if (result != null)
                    {
                        return result;
                    }
                }
                else
                {
                    return new ResponseBase<Tout>
                    {
                        Code = (int)response.StatusCode,
                        Message = response.StatusCode.GetEnumDescription()
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseBase<Tout>
                {
                    Code = 99,
                    Message = $"Service base exception :{ex.Message}"
                };
                // Log error
            }

            return default(ResponseBase<Tout>);
        }

        public async Task<Stream> DownLoadFileGet(string url)
        {
            var httpClient = new HttpClient();
            //var url = "https://localhost:7300/ctv/hr-report-synthesis-excel";
            var request = new HttpRequestMessage(HttpMethod.Get, @$"{AppConfigs.ApiUrlBase}/{url}");
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStream();
        }
        public async Task<Stream> DownLoadFilePost<T>(string url, T parram)
        {
            var httpClient = new HttpClient();
            //var url = "https://localhost:7300/ctv/hr-report-synthesis-excel";
            string serialized = JsonConvert.SerializeObject(parram);
            HttpContent content = new StringContent(serialized, null, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, @$"{AppConfigs.ApiUrlBase}/{url}");
            request.Content = content;
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStream();
        }
    }
}