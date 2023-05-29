using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TransversalLibrary.Standard;

namespace ConnectionLibrary.Standard.Azure
{
    /// <summary>
    /// Define el conector de AzureComputerVision
    /// </summary>
    public class AzureComputerVisionConnector
    {
        /// <summary>
        /// Define el cliente
        /// </summary>
        private readonly ComputerVisionClient Client;

        /// <summary>
        /// Inicializa las propiedades de esta clase
        /// </summary>
        /// <param name="endpoint">El Endpoint</param>
        /// <param name="key">La llave secreta</param>
        public AzureComputerVisionConnector(string endpoint, string key)
        {
            ApiKeyServiceClientCredentials apiKeyServiceClientCredentials = new ApiKeyServiceClientCredentials(key);
            Client = new ComputerVisionClient(apiKeyServiceClientCredentials) { Endpoint = endpoint };
        }

        #region OCR

        /// <summary>
        /// Lee el texto desde la imagen desde la URL
        /// </summary>
        /// <param name="imageUrl">La URL de la imagen</param>
        /// <returns>La tarea</returns>
        public async Task<Response<string>> ReadTextFromImageUrl(string imageUrl)
        {
            try
            {
                // Read text from URL
                ReadHeaders textHeaders = await Client?.ReadAsync(imageUrl);
                // After the request, get the operation location (operation ID)
                string operationLocation = textHeaders?.OperationLocation;
                //Retrieve the URI where the extracted text will be stored from the Operation-Location header.
                //We only need the ID and not the full URL
                const int numberOfCharsInOperationId = 36;
                string operationId = operationLocation?.Substring(operationLocation.Length - numberOfCharsInOperationId);
                //Extract the text
                ReadOperationResult results;
                do
                {
                    results = await Client?.GetReadResultAsync(Guid.Parse(operationId));
                }
                while (results.Status == OperationStatusCodes.Running || results.Status == OperationStatusCodes.NotStarted);
                //Obtiene el resultado
                IList<ReadResult> textUrlFileResults = results?.AnalyzeResult?.ReadResults;
                IEnumerable<string> texts = textUrlFileResults?.SelectMany(x => x?.Lines)?.Select(y => y?.Text);
                string result = string.Join(" ", texts);
                return Response<string>.ReturnOK("Se obtuvo el texto de la imagen satisfactoriamente", result);
            }
            catch (Exception ex)
            {
                return Response<string>.ReturnInternalServerError(ex?.Message);
            }
        }

        /// <summary>
        /// Lee el texto desde la imagen desde el Stream
        /// </summary>
        /// <param name="imageStream">El stream de la imagen</param>
        /// <returns>La tarea</returns>
        public async Task<Response<string>> ReadTextFromImageStream(Stream imageStream)
        {
            try
            {
                // Read text from URL
                ReadInStreamHeaders textHeaders = await Client?.ReadInStreamAsync(imageStream);
                // After the request, get the operation location (operation ID)
                string operationLocation = textHeaders?.OperationLocation;
                //Retrieve the URI where the extracted text will be stored from the Operation-Location header.
                //We only need the ID and not the full URL
                const int numberOfCharsInOperationId = 36;
                string operationId = operationLocation?.Substring(operationLocation.Length - numberOfCharsInOperationId);
                //Extract the text
                ReadOperationResult results;
                do
                {
                    results = await Client?.GetReadResultAsync(Guid.Parse(operationId));
                }
                while (results.Status == OperationStatusCodes.Running || results.Status == OperationStatusCodes.NotStarted);
                //Obtiene el resultado
                IList<ReadResult> textUrlFileResults = results?.AnalyzeResult?.ReadResults;
                IEnumerable<string> texts = textUrlFileResults?.SelectMany(x => x?.Lines)?.Select(y => y?.Text);
                string result = string.Join(" ", texts);
                return Response<string>.ReturnOK("Se obtuvo el texto de la imagen satisfactoriamente", result);
            }
            catch (Exception ex)
            {
                return Response<string>.ReturnInternalServerError(ex?.Message);
            }
        }

        #endregion

        #region DETECT OBJECTS FROM IMAGE

        /// <summary>
        /// Obtiene la información de los objetos encontrados en la imagen
        /// </summary>
        /// <param name="imageUrl">La URL de la imagen</param>
        /// <returns>La tarea</returns>
        public async Task<Response<string>> DetectObjectsFromImageUrl(string imageUrl)
        {
            try
            {
                DetectResult detectResult = await Client?.DetectObjectsAsync(imageUrl);
                IList<DetectedObject> detectedObjects = detectResult?.Objects;
                string jsonResult = JsonSerializer.Serialize(detectedObjects);
                return Response<string>.ReturnOK("OK", jsonResult);
            }
            catch (Exception ex)
            {
                return Response<string>.ReturnInternalServerError(ex?.Message);
            }
        }

        /// <summary>
        /// Obtiene la información de los objetos encontrados en la imagen
        /// </summary>
        /// <param name="imageStream">El stream de la imagen</param>
        /// <returns>La tarea</returns>
        public async Task<Response<string>> DetectObjectsFromImageStream(Stream imageStream)
        {
            try
            {
                DetectResult detectResult = await Client?.DetectObjectsInStreamAsync(imageStream);
                IList<DetectedObject> detectedObjects = detectResult?.Objects;
                string jsonResult = JsonSerializer.Serialize(detectedObjects);
                return Response<string>.ReturnOK("OK", jsonResult);
            }
            catch (Exception ex)
            {
                return Response<string>.ReturnInternalServerError(ex?.Message);
            }
        }

        #endregion
    }
}