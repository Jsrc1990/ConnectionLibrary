using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TransversalLibrary.Standard;

namespace ConnectionLibrary.Standard.WebServices
{
    /// <summary>
    /// Define el conector HTTPClient
    /// </summary>
    public class HttpClientConnector
    {
        /// <summary>
        /// Define el cliente
        /// </summary>
        private readonly HttpClient Client;

        /// <summary>
        /// Las cabeceras
        /// </summary>
        public List<(string, string)> Headers { get; } = new List<(string, string)>();

        /// <summary>
        /// Inicializa las propiedades de esta clase
        /// </summary>
        public HttpClientConnector()
        {
            Client = new HttpClient();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<T> Post<T>(string endpoint, object body)
        {
            try
            {
                ManageHeaders();
                string json = JsonConvert.SerializeObject(body);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await Client?.PostAsync(endpoint, content);
                string responseContent = await response?.Content?.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<Response<T>> PostResponse<T>(string endpoint, object body)
        {
            try
            {
                ManageHeaders();
                string json = JsonConvert.SerializeObject(body);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await Client?.PostAsync(endpoint, content);
                string responseContent = await response?.Content?.ReadAsStringAsync();
                Response<T> result = JsonConvert.DeserializeObject<Response<T>>(responseContent);
                return result;
            }
            catch (Exception ex)
            {
                return Response<T>.ReturnInternalServerError(ex?.Message);
            }
        }

        /// <summary>
        /// Gestiona las cabeceras
        /// </summary>
        private void ManageHeaders()
        {
            Client?.DefaultRequestHeaders?.Clear();
            if (Headers?.Any() == true)
                foreach ((string, string) header in Headers)
                    Client?.DefaultRequestHeaders?.Add(header.Item1, header.Item2);
        }
    }
}