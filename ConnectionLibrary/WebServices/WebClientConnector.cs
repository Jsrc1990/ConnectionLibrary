using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using TransversalLibrary;

namespace ConnectionLibrary.WebServices
{
    /// <summary>
    /// Define el conector por mediante WebClient
    /// </summary>
    public class WebClientConnector
    {
        /// <summary>
        /// Define el token
        /// </summary>
        private static string _Token;

        /// <summary>
        /// Obtiene o establece el token
        /// </summary>
        public static string Token
        {
            get
            {
                return _Token;
            }
            set
            {
                _Token = value;
            }
        }

        /// <summary>
        /// Las cabeceras
        /// </summary>
        public List<(string, string)> Headers { get; } = new List<(string, string)>();

        /// <summary>
        /// Obtiene el WebClient
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        private WebClient GetWebClient()
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "Application/json; charset=utf-8");
            webClient.Headers.Add("Accept-Type", "Application/json; charset=utf-8");
            if (Headers?.Any() == true)
                foreach ((string, string) header in Headers)
                    webClient.Headers.Add(header.Item1, header.Item2);
            webClient.Encoding = Encoding.UTF8;
            if (!string.IsNullOrWhiteSpace(Token))
                webClient.Headers.Add("Authorization", "Bearer " + Token);
            return webClient;
        }

        /// <summary>
        /// Realiza un Get al EndPoint especificado, y retorna la cadena resultante
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public Response<string> Get(string endPoint)
        {
            //Define la respuesta
            Response<string> response = new Response<string>();
            try
            {
                //Obtiene el cliente web
                using (WebClient context = GetWebClient())
                {
                    //Obtiene la cadena resultante
                    string result = context?.DownloadString(endPoint?.ToLower());
                    //Establece la cadena resultante en la respuesta
                    response.Data = result;
                }
            }
            catch (Exception ex)
            {
                response?.Errors?.Add(ex?.Message);
            }
            return response;
        }

        /// <summary>
        /// Realiza un Get al EndPoint especificado, y retorna la cadena resultante
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public Response<T> Get<T>(string endPoint, string queries = "")
        {
            //Define la respuesta
            Response<T> response = new Response<T>();
            try
            {
                //Obtiene el cliente web
                using (WebClient context = GetWebClient())
                {
                    //Obtiene la cadena resultante
                    string url = endPoint + queries;
                    string result = context?.DownloadString(url);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        //Establece la cadena resultante en la respuesta
                        response.Data = JsonConvert.DeserializeObject<T>(result);
                    }
                }
            }
            catch (Exception ex)
            {
                response?.Errors?.Add(ex?.Message);
            }
            return response;
        }

        /// <summary>
        /// Realiza un Post al EndPoint especificado, le envía el body, y retorna la cadena resultante
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public Response<T> Post<T>(string endPoint, object body = null)
        {
            //Define la respuesta
            Response<T> response = new Response<T>();
            try
            {
                //Serializa la entidad
                string parameters = JsonConvert.SerializeObject(body);
                //Obtiene el cliente web
                using (WebClient context = GetWebClient())
                {
                    //Obtiene la cadena resultante
                    string result = context?.UploadString(endPoint, "POST", parameters);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        //Establece la cadena resultante en la respuesta
                        response.Data = JsonConvert.DeserializeObject<T>(result);
                    }
                }
            }
            catch (Exception ex)
            {
                response?.Errors?.Add(ex?.Message);
            }
            return response;
        }

        /// <summary>
        /// Realiza un Post al EndPoint especificado, le envía el body, y retorna la cadena resultante
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public Response<T> Post<T>(string endPoint, string body = null)
        {
            //Define la respuesta
            Response<T> response = new Response<T>();
            try
            {
                //Obtiene el cliente web
                using (WebClient context = GetWebClient())
                {
                    //Obtiene la cadena resultante
                    string result = context?.UploadString(endPoint, "POST", body);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        //Establece la cadena resultante en la respuesta
                        response.Data = JsonConvert.DeserializeObject<T>(result);
                    }
                }
            }
            catch (Exception ex)
            {
                response?.Errors?.Add(ex?.Message);
            }
            return response;
        }

        /// <summary>
        /// Realiza un Post al EndPoint especificado, le envía el body, y retorna la cadena resultante
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public Response<string> Post(string endPoint, object body)
        {
            //Define la respuesta
            Response<string> response = new Response<string>();
            try
            {
                //Serializa la entidad
                string parameters = JsonConvert.SerializeObject(body);
                //Obtiene el cliente web
                using (WebClient context = GetWebClient())
                {
                    //Obtiene la cadena resultante
                    string result = context?.UploadString(endPoint?.ToLower(), "POST", parameters);
                    //Establece la cadena resultante en la respuesta
                    response.Data = result;
                }
            }
            catch (Exception ex)
            {
                response?.Errors?.Add(ex?.Message);
            }
            return response;
        }

        /// <summary>
        /// Realiza un Put al EndPoint especificado, le envía el body, y retorna la cadena resultante
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public Response<T> Put<T>(string endPoint, object body = null)
        {
            //Define la respuesta
            Response<T> response = new Response<T>();
            try
            {
                //Serializa la entidad
                string parameters = JsonConvert.SerializeObject(body);
                //Obtiene el cliente web
                using (WebClient context = GetWebClient())
                {
                    //Obtiene la cadena resultante
                    string result = context?.UploadString(endPoint, "PUT", parameters);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        //Establece la cadena resultante en la respuesta
                        response.Data = JsonConvert.DeserializeObject<T>(result);
                    }
                }
            }
            catch (Exception ex)
            {
                response?.Errors?.Add(ex?.Message);
            }
            return response;
        }

        /// <summary>
        /// Realiza un Put al EndPoint especificado, le envía el body, y retorna la cadena resultante
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public Response<string> Put(string endPoint, object body)
        {
            //Define la respuesta
            Response<string> response = new Response<string>();
            try
            {
                //Serializa la entidad
                string parameters = JsonConvert.SerializeObject(body);
                //Obtiene el cliente web
                using (WebClient context = GetWebClient())
                {
                    //Obtiene la cadena resultante
                    string result = context?.UploadString(endPoint?.ToLower(), "PUT", parameters);
                    //Establece la cadena resultante en la respuesta
                    response.Data = result;
                }
            }
            catch (Exception ex)
            {
                response?.Errors?.Add(ex?.Message);
            }
            return response;
        }
    }
}