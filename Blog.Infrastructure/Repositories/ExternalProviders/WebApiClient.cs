using Blog.Domain.Enums;
using Blog.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Xml.Serialization;

namespace Blog.Infrastructure.Repositories.ExternalProviders
{
    public abstract class WebApiClient
    {
        private readonly ILogger<WebApiClient> _logger;
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly HttpClient _httpClient;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected WebApiClient(ILogger<WebApiClient> logger, HttpClient httpClient, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets async.
        /// </summary>
        /// <typeparam name="T">The T.</typeparam>
        /// <param name="description">The description.</param>
        /// <param name="requestUri">The requestUri.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <param name="applicableProvider">The applicableProvider.</param>
        /// <param name="ignoreChecking">The ignoreChecking.</param>
        /// <returns>Task{T}.</returns>
        public async Task<T?> GetAsync<T>(string description, Uri requestUri, string applicableProvider, CancellationToken cancellationToken, bool ignoreChecking = false, Dictionary<string, string>? listRequestHeader = null)
        {
            string message;
            try
            {
                string valueString = string.Empty;

                T? value;

                if (_httpClientFactory == null)
                {
                    throw new ArgumentException($"{nameof(GetAsync)} httpClientFactory null value | description: {description}");
                }

                HttpClient httpClient = _httpClientFactory.CreateClient() ?? throw new ArgumentException($"{nameof(GetAsync)} cannot init httpClient ");

                if (listRequestHeader != null && listRequestHeader.Count > 0)
                {
                    SetHeaderRequest(httpClient, listRequestHeader);
                }

                HttpResponseMessage? response = await httpClient.GetAsync(requestUri, cancellationToken) ?? throw new ArgumentException($"{nameof(GetAsync)} get response null value");

                if (!response.IsSuccessStatusCode)
                {
                    await ExceptionWebApi(response, ignoreChecking, description);
                }

                valueString = await response.Content.ReadAsStringAsync();

                if (typeof(T?) == typeof(string))
                {
                    //Allow string objects to bypass deserialization
                    value = (T?)(object)valueString;
                }
                else
                {
                    if (valueString.TrimStart().StartsWith('<'))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        // convert string to stream
                        byte[] byteArray = Encoding.UTF8.GetBytes(valueString);
                        using MemoryStream stream = new(byteArray);
                        value = (T?)serializer.Deserialize(stream);
                    }
                    else
                    {
                        value = JsonConvert.DeserializeObject<T?>(valueString);

                        message = $"{nameof(GetAsync)} {applicableProvider} DeserializeObject.";
                        _logger.LogInformation(message);
                    }
                }

                return value;
            }
            catch (Exception ex)
            {
                message = $"{nameof(GetAsync)} {applicableProvider} exception error message: {ex.Message} | StackTrace: {ex.StackTrace} | Description: {description}";
                _logger.LogInformation(ex, message);

                throw new UnhandledException(message);
            }
        }

        private static async Task ExceptionWebApi(HttpResponseMessage response, bool ignoreChecking, string description)
        {
            string content = response.Content != null ? await response.Content.ReadAsStringAsync() : string.Empty;

            string warning = "Call returned with " + description + " statusCode " + response.StatusCode + " and response " + content;

            if (!ignoreChecking)
            {
                throw new WebApiException(response.StatusCode, "Call unsuccessful. " + warning);
            }
        }

        private static void SetHeaderRequest(HttpClient httpClient, Dictionary<string, string> listDictionaryHeaderRequest)
        {
            var listHeader = listDictionaryHeaderRequest.ToList();

            foreach (var item in listHeader)
            {
                httpClient.DefaultRequestHeaders.Remove(item.Key);
                httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Posts async.
        /// </summary>
        /// <typeparam name="TRequest">The TRequest.</typeparam>
        /// <typeparam name="TResponse">The TResponse.</typeparam>
        /// <param name="description">The description.</param>
        /// <param name="requestUri">The requestUri.</param>
        /// <param name="requestContent">The requestContent.</param>
        /// <param name="dataInterchangeFormat">The dataInterchangeFormat.</param>
        /// <param name="applicableProvider">The applicableProvider.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <param name="jsonSerializerSettings">The jsonSerializerSettings.</param>
        /// <param name="ignoreChecking">The ignoreChecking.</param>
        /// <returns>Task{TResponse}.</returns>
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string description,
            Uri requestUri,
            TRequest requestContent,
            DataInterchangeFormat dataInterchangeFormat,
            string applicableProvider,
            CancellationToken cancellationToken,
            JsonSerializerSettings? jsonSerializerSettings = null,
            bool ignoreChecking = false,
            Dictionary<string, string>? listRequestHeader = null)
        {
            string message;
            try
            {
                string valueString = string.Empty;

                TResponse? value;

                StringContent serializedRequestContent;
                int dataInterchangeFormatInt = Convert.ToInt32(dataInterchangeFormat);

                if (dataInterchangeFormatInt == 0)
                {
                    serializedRequestContent = SerializeAsJsonString(requestContent, jsonSerializerSettings);
                }
                else if (dataInterchangeFormatInt == 1)
                {
                    serializedRequestContent = SerializeAsXmlString(requestContent);
                }
                else if (dataInterchangeFormatInt == 2)
                {
                    serializedRequestContent = SerializeAsSoapXmlString(requestContent);
                }
                else
                {
                    message = "Method parameter 'dataInterchangeFormat' must have a value of 'xml' or 'json', currently it is: {0}";
                    _logger.LogError(message, dataInterchangeFormat);

                    return default;
                }

                if (_httpClientFactory == null)
                {
                    throw new ArgumentException($"PostAsync httpClientFactory null value | description: {description}");
                }

                HttpClient httpClient = _httpClientFactory.CreateClient() ?? throw new ArgumentException($"PostAsync cannot init httpClient ");

                if (listRequestHeader != null && listRequestHeader.Count > 0)
                {
                    SetHeaderRequest(httpClient, listRequestHeader);
                }

                HttpResponseMessage? response = await httpClient.PostAsync(requestUri, serializedRequestContent, cancellationToken) ?? throw new ArgumentException($"PostAsync get response null value");

                if (!response.IsSuccessStatusCode)
                {
                    await ExceptionWebApi(response, ignoreChecking, description);
                }

                valueString = await response.Content.ReadAsStringAsync();

                if (typeof(TResponse?) == typeof(string))
                {
                    //Allow string objects to bypass deserialization
                    value = (TResponse?)(object)valueString;
                }
                else
                {
                    if (valueString.TrimStart().StartsWith('<'))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(TResponse?));
                        // convert string to stream
                        byte[] byteArray = Encoding.UTF8.GetBytes(valueString);
                        using MemoryStream stream = new MemoryStream(byteArray);
                        value = (TResponse?)serializer.Deserialize(stream);
                    }
                    else
                    {
                        value = JsonConvert.DeserializeObject<TResponse?>(valueString);

                        message = $"PostAsync {applicableProvider} DeserializeObject.";
                        _logger.LogInformation(message);
                    }
                }

                return value;
            }
            catch (Exception ex)
            {
                message = $"PostAsync {applicableProvider} exception error message: {ex.Message} | StackTrace: {ex.StackTrace} | Description: {description}";
                _logger.LogInformation(ex, message);

                throw new UnhandledException(message);
            }
        }

        /// <summary>
        /// Put async.
        /// </summary>
        /// <typeparam name="TRequest">The TRequest.</typeparam>
        /// <typeparam name="TResponse">The TResponse.</typeparam>
        /// <param name="description">The description.</param>
        /// <param name="requestUri">The requestUri.</param>
        /// <param name="requestContent">The requestContent.</param>
        /// <param name="applicableProvider">The applicableProvider.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <param name="ignoreChecking">The ignoreChecking.</param>
        /// <param name="listRequestHeader">The listRequestHeader.</param>
        /// <returns>Task{TResponse}.</returns>
        public async Task<TResponse?> PutAsync<TRequest, TResponse>(
            string description,
            Uri requestUri,
            TRequest requestContent,
            string applicableProvider,
            CancellationToken cancellationToken,
            bool ignoreChecking = false,
            Dictionary<string, string>? listRequestHeader = null)
        {
            string message;
            try
            {
                string valueString = string.Empty;

                TResponse? value;

                if (_httpClientFactory == null)
                {
                    throw new ArgumentException($"PutAsync httpClientFactory null value | description: {description}");
                }

                HttpClient httpClient = _httpClientFactory.CreateClient() ?? throw new ArgumentException($"PutAsync cannot init httpClient ");

                if (listRequestHeader != null && listRequestHeader.Count > 0)
                {
                    SetHeaderRequest(httpClient, listRequestHeader);
                }

                HttpResponseMessage? response = await httpClient.PutAsync(requestUri, SerializeAsJsonString(requestContent)) ?? throw new ArgumentException($"PutAsync get response null value");

                if (!response.IsSuccessStatusCode)
                {
                    await ExceptionWebApi(response, ignoreChecking, description);
                }

                valueString = await response.Content.ReadAsStringAsync();

                if (typeof(TResponse?) == typeof(string))
                {
                    //Allow string objects to bypass deserialization
                    value = (TResponse?)(object)valueString;
                }
                else
                {
                    if (valueString.TrimStart().StartsWith('<'))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(TResponse?));
                        // convert string to stream
                        byte[] byteArray = Encoding.UTF8.GetBytes(valueString);
                        using MemoryStream stream = new MemoryStream(byteArray);
                        value = (TResponse?)serializer.Deserialize(stream);
                    }
                    else
                    {
                        value = JsonConvert.DeserializeObject<TResponse?>(valueString);
                        message = $"PutAsync {applicableProvider} DeserializeObject.";

                        _logger.LogInformation(message);
                    }
                }

                return value;
            }
            catch (Exception ex)
            {
                message = $"PutAsync {applicableProvider} exception error message: {ex.Message} | StackTrace: {ex.StackTrace} | Description: {description}";
                _logger.LogInformation(ex, message);

                throw new UnhandledException(message);
            }
        }

        /// <summary>
        /// Delete async.
        /// </summary>
        /// <typeparam name="T">The T</typeparam>
        /// <param name="description">The description.</param>
        /// <param name="requestUri">The requestUri.</param>
        /// <param name="applicableProvider">The applicableProvider.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <param name="ignoreChecking">The ignoreChecking.</param>
        /// <param name="listRequestHeader">The listRequestHeader.</param>
        /// <returns>Task{T}.</returns>
        public async Task<T?> DeleteAsync<T>(
            string description,
            Uri requestUri,
            string applicableProvider,
            CancellationToken cancellationToken,
            bool ignoreChecking = false,
            Dictionary<string, string>? listRequestHeader = null)
        {
            string message;
            try
            {
                string valueString = string.Empty;

                T? value;

                if (_httpClientFactory == null)
                {
                    throw new ArgumentException($"DeleteAsync httpClientFactory null value | description: {description}");
                }

                HttpClient httpClient = _httpClientFactory.CreateClient() ?? throw new ArgumentException($"DeleteAsync cannot init httpClient ");

                if (listRequestHeader != null && listRequestHeader.Count > 0)
                {
                    SetHeaderRequest(httpClient, listRequestHeader);
                }

                HttpResponseMessage? response = await httpClient.DeleteAsync(requestUri, cancellationToken) ?? throw new ArgumentException($"DeleteAsync get response null value");

                if (!response.IsSuccessStatusCode)
                {
                    await ExceptionWebApi(response, ignoreChecking, description);
                }

                valueString = await response.Content.ReadAsStringAsync();

                if (typeof(T?) == typeof(string))
                {
                    //Allow string objects to bypass deserialization
                    value = (T?)(object)valueString;
                }
                else
                {
                    if (valueString.TrimStart().StartsWith('<'))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T?));
                        // convert string to stream
                        byte[] byteArray = Encoding.UTF8.GetBytes(valueString);
                        using MemoryStream stream = new MemoryStream(byteArray);
                        value = (T?)serializer.Deserialize(stream);
                    }
                    else
                    {
                        value = JsonConvert.DeserializeObject<T?>(valueString);
                        message = $"DeleteAsync {applicableProvider} DeserializeObject.";

                        _logger.LogInformation(message);
                    }
                }

                return value;
            }
            catch (Exception ex)
            {
                message = $"DeleteAsync {applicableProvider} exception error message: {ex.Message} | StackTrace: {ex.StackTrace} | Description: {description}";
                _logger.LogInformation(ex, message);

                throw new UnhandledException(message);
            }
        }

        /// <summary>
        /// Deletes async.
        /// </summary>
        /// <typeparam name="TRequest">The TRequest.</typeparam>
        /// <typeparam name="TResponse">The TResponse.</typeparam>
        /// <param name="description">The description.</param>
        /// <param name="requestUri">The requestUri.</param>
        /// <param name="requestContent">The requestContent.</param>
        /// <param name="dataInterchangeFormat">The dataInterchangeFormat.</param>
        /// <param name="applicableProvider">The applicableProvider.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <param name="jsonSerializerSettings">The jsonSerializerSettings.</param>
        /// <param name="ignoreChecking">The ignoreChecking.</param>
        /// <returns>Task{TResponse}.</returns>
        public Task<TResponse?> DeleteAsync<TRequest, TResponse>(string description,
            Uri requestUri,
            TRequest requestContent,
            DataInterchangeFormat dataInterchangeFormat,
            string applicableProvider,
            CancellationToken cancellationToken,
            JsonSerializerSettings? jsonSerializerSettings = null,
            bool ignoreChecking = false)
        {
            StringContent serializedRequestContent;
            int dataInterchangeFormatInt = Convert.ToInt32(dataInterchangeFormat);

            if (dataInterchangeFormatInt == 0)
            {
                serializedRequestContent = SerializeAsJsonString(requestContent, jsonSerializerSettings);
            }
            else if (dataInterchangeFormatInt == 1)
            {
                serializedRequestContent = SerializeAsXmlString(requestContent);
            }
            else
            {
                throw new ArgumentException("Method parameter 'dataInterchangeFormat' must have a value of 'xml' or 'json'.", nameof(dataInterchangeFormat));
            }

            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, requestUri)
            {
                Content = serializedRequestContent
            };

            return CallApiAsync<TResponse?>(description,
                client => CheckForWebException(description, client.SendAsync(requestMessage, cancellationToken), ignoreChecking), applicableProvider);
        }

        /// <summary>
        /// Get Async with result return always
        /// </summary>
        /// <typeparam name="T">The T</typeparam>
        /// <param name="description">The description</param>
        /// <param name="requestUri">The requestUri</param>
        /// <param name="applicableProvider">The applicableProvider</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <param name="ignoreChecking">The ignoreChecking.</param>
        /// <param name="listRequestHeader">The listRequestHeader.</param>
        /// <returns>Task{T}.</returns>
        public async Task<T?> SafeGetAsync<T>(
            string description,
            Uri requestUri,
            string applicableProvider,
            CancellationToken cancellationToken,
            bool ignoreChecking = false,
            Dictionary<string, string>? listRequestHeader = null)
        {
            string message;
            try
            {
                string valueString = string.Empty;

                T? value;

                if (_httpClientFactory == null)
                {
                    throw new ArgumentException($"SafeGetAsync httpClientFactory null value | description: {description}");
                }

                HttpClient httpClient = _httpClientFactory.CreateClient() ?? throw new ArgumentException($"SafeGetAsync cannot init httpClient ");

                if (listRequestHeader != null && listRequestHeader.Count > 0)
                {
                    SetHeaderRequest(httpClient, listRequestHeader);
                }

                HttpResponseMessage? response = await httpClient.GetAsync(requestUri, cancellationToken) ?? throw new ArgumentException($"SafeGetAsync get response null value");

                if (!response.IsSuccessStatusCode)
                {
                    await ExceptionWebApi(response, ignoreChecking, description);
                }

                valueString = await response.Content.ReadAsStringAsync();

                if (typeof(T?) == typeof(string))
                {
                    //Allow string objects to bypass deserialization
                    value = (T?)(object)valueString;
                }
                else
                {
                    if (valueString.TrimStart().StartsWith('<'))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T?));
                        // convert string to stream
                        byte[] byteArray = Encoding.UTF8.GetBytes(valueString);
                        using MemoryStream stream = new MemoryStream(byteArray);
                        value = (T?)serializer.Deserialize(stream);
                    }
                    else
                    {
                        value = JsonConvert.DeserializeObject<T?>(valueString);
                        message = $"SafeGetAsync {applicableProvider} DeserializeObject.";
                        _logger.LogInformation(message);
                    }
                }

                return value;
            }
            catch (Exception ex)
            {
                message = $"SafeGetAsync {applicableProvider} exception error message: {ex.Message} | StackTrace: {ex.StackTrace} | Description: {description}";
                _logger.LogInformation(ex, message);

                throw new UnhandledException(message);
            }
        }

        /// <summary>
        /// Post Async with result return always.
        /// </summary>
        /// <typeparam name="TRequest">The TRequest.</typeparam>
        /// <typeparam name="TResponse">The TResponse.</typeparam>
        /// <param name="description">The description.</param>
        /// <param name="requestUri">The requestUri.</param>
        /// <param name="requestContent">The requestContent.</param>
        /// <param name="dataInterchangeFormat">The dataInterchangeFormat.</param>
        /// <param name="applicableProvider">The applicableProvider.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <param name="jsonSerializerSettings">The jsonSerializerSettings.</param>
        /// <returns>Task{TResponse}.</returns>
        public async Task<TResponse?> SafePostAsync<TRequest, TResponse>(
            string description,
            Uri requestUri,
            TRequest requestContent,
            DataInterchangeFormat dataInterchangeFormat,
            string applicableProvider,
            CancellationToken cancellationToken,
            JsonSerializerSettings? jsonSerializerSettings = null,
            bool ignoreChecking = false,
            Dictionary<string, string>? listRequestHeader = null)
        {
            string message;
            try
            {
                string valueString = string.Empty;

                TResponse? value;

                StringContent serializedRequestContent;
                int dataInterchangeFormatInt = Convert.ToInt32(dataInterchangeFormat);

                if (dataInterchangeFormatInt == 0)
                {
                    serializedRequestContent = SerializeAsJsonString(requestContent, jsonSerializerSettings);
                }
                else if (dataInterchangeFormatInt == 1)
                {
                    serializedRequestContent = SerializeAsXmlString(requestContent);
                }
                else if (dataInterchangeFormatInt == 2)
                {
                    serializedRequestContent = SerializeAsSoapXmlString(requestContent);
                }
                else
                {
                    message = $"Method parameter 'dataInterchangeFormat' must have a value of 'xml' or 'json', currently it is: {dataInterchangeFormat}";
                    _logger.LogError(message);

                    return default;
                }

                if (_httpClientFactory == null)
                {
                    throw new ArgumentException($"SafePostAsync httpClientFactory null value | description: {description}");
                }

                HttpClient httpClient = _httpClientFactory.CreateClient() ?? throw new ArgumentException($"SafePostAsync cannot init httpClient ");

                if (listRequestHeader != null && listRequestHeader.Count > 0)
                {
                    SetHeaderRequest(httpClient, listRequestHeader);
                }

                HttpResponseMessage? response = await httpClient.PostAsync(requestUri, serializedRequestContent, cancellationToken) ?? throw new ArgumentException($"SafePostAsync get response null value");

                if (!response.IsSuccessStatusCode)
                {
                    await ExceptionWebApi(response, ignoreChecking, description);
                }

                valueString = await response.Content.ReadAsStringAsync();

                if (typeof(TResponse?) == typeof(string))
                {
                    //Allow string objects to bypass deserialization
                    value = (TResponse)(object)valueString;
                }
                else
                {
                    if (valueString.TrimStart().StartsWith('<'))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(TResponse?));
                        // convert string to stream
                        byte[] byteArray = Encoding.UTF8.GetBytes(valueString);
                        using MemoryStream stream = new MemoryStream(byteArray);
                        value = (TResponse?)serializer.Deserialize(stream);
                    }
                    else
                    {
                        value = JsonConvert.DeserializeObject<TResponse?>(valueString);

                        message = $"SafePostAsync {applicableProvider} DeserializeObject.";
                        _logger.LogInformation(message);
                    }
                }

                return value;
            }
            catch (Exception ex)
            {
                message = $"SafePostAsync {applicableProvider} exception error message: {ex.Message} | StackTrace: {ex.StackTrace} | Description: {description}";
                _logger.LogInformation(ex, message);

                throw new UnhandledException(message);
            }
        }

        /// <summary>
        /// Put Async with result return always.
        /// </summary>
        /// <typeparam name="TRequest">The TRequest.</typeparam>
        /// <typeparam name="TResponse">The TResponse.</typeparam>
        /// <param name="description">The description.</param>
        /// <param name="requestUri">The requestUri.</param>
        /// <param name="requestContent">The requestContent.</param>
        /// <param name="applicableProvider">The applicableProvider.</param>
        /// <returns>Task{TResponse}.</returns>
        public async Task<TResponse?> SafePutAsync<TRequest, TResponse>(
            string description,
            Uri requestUri,
            TRequest requestContent,
            string applicableProvider,
            bool ignoreChecking = false,
            Dictionary<string, string>? listRequestHeader = null)
        {
            string message;
            try
            {
                string valueString = string.Empty;

                TResponse? value;

                if (_httpClientFactory == null)
                {
                    throw new ArgumentException($"SafePutAsync httpClientFactory null value | description: {description}");
                }

                HttpClient httpClient = _httpClientFactory.CreateClient() ?? throw new ArgumentException($"SafePutAsync cannot init httpClient ");

                if (listRequestHeader != null && listRequestHeader.Count > 0)
                {
                    SetHeaderRequest(httpClient, listRequestHeader);
                }

                HttpResponseMessage? response = await httpClient.PutAsync(requestUri, SerializeAsJsonString(requestContent)) ?? throw new ArgumentException($"SafePutAsync get response null value");

                if (!response.IsSuccessStatusCode)
                {
                    await ExceptionWebApi(response, ignoreChecking, description);
                }

                valueString = await response.Content.ReadAsStringAsync();

                if (typeof(TResponse?) == typeof(string))
                {
                    //Allow string objects to bypass deserialization
                    value = (TResponse?)(object)valueString;
                }
                else
                {
                    if (valueString.TrimStart().StartsWith('<'))
                    {
                        XmlSerializer? serializer = new(typeof(TResponse?));
                        // convert string to stream
                        byte[] byteArray = Encoding.UTF8.GetBytes(valueString);
                        using MemoryStream stream = new(byteArray);
                        value = (TResponse?)serializer.Deserialize(stream);
                    }
                    else
                    {
                        value = JsonConvert.DeserializeObject<TResponse?>(valueString);
                        message = $"SafePutAsync {applicableProvider} DeserializeObject.";
                        _logger.LogInformation(message);
                    }
                }

                return value;
            }
            catch (Exception ex)
            {
                message = $"SafePutAsync {applicableProvider} exception error message: {ex.Message} | StackTrace: {ex.StackTrace} | Description: {description}";
                _logger.LogInformation(ex, message);

                throw new UnhandledException(message);
            }
        }

        /// <summary>
        /// Delete Async with result return always.
        /// </summary>
        /// <typeparam name="T">The T</typeparam>
        /// <param name="description">The description</param>
        /// <param name="requestUri">The requestUri</param>
        /// <param name="applicableProvider">The applicableProvider</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>Task{T}.</returns>
        public async Task<T?> SafeDeleteAsync<T>(
            string description,
            Uri requestUri,
            string applicableProvider,
            CancellationToken cancellationToken,
            bool ignoreChecking = false,
            Dictionary<string, string>? listRequestHeader = null)
        {
            string message;
            try
            {
                string valueString = string.Empty;

                T? value;

                if (_httpClientFactory == null)
                {
                    throw new ArgumentException($"SafeDeleteAsync httpClientFactory null value | description: {description}");
                }

                HttpClient httpClient = _httpClientFactory.CreateClient() ?? throw new ArgumentException($"SafeDeleteAsync cannot init httpClient ");
                if (listRequestHeader != null && listRequestHeader.Count > 0)
                {
                    SetHeaderRequest(httpClient, listRequestHeader);
                }

                HttpResponseMessage? response = await httpClient.DeleteAsync(requestUri, cancellationToken) ?? throw new ArgumentException($"SafeDeleteAsync get response null value");

                if (!response.IsSuccessStatusCode)
                {
                    await ExceptionWebApi(response, ignoreChecking, description);
                }

                valueString = await response.Content.ReadAsStringAsync();

                if (typeof(T?) == typeof(string))
                {
                    //Allow string objects to bypass deserialization
                    value = (T?)(object)valueString;
                }
                else
                {
                    if (valueString.TrimStart().StartsWith('<'))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T?));
                        // convert string to stream
                        byte[] byteArray = Encoding.UTF8.GetBytes(valueString);
                        using MemoryStream stream = new MemoryStream(byteArray);
                        value = (T?)serializer.Deserialize(stream);
                    }
                    else
                    {
                        value = JsonConvert.DeserializeObject<T?>(valueString);
                        message = $"SafeDeleteAsync {applicableProvider} DeserializeObject.";
                        _logger.LogInformation(message);
                    }
                }

                return value;
            }
            catch (Exception ex)
            {
                message = $"SafeDeleteAsync {applicableProvider} exception error message: {ex.Message} | StackTrace: {ex.StackTrace} | Description: {description}";
                _logger.LogInformation(ex, message);

                throw new UnhandledException(message);
            }
        }

        /// <summary>
        /// Creates request.
        /// </summary>
        /// <param name="uri">The uri.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns> FluentUriBuilder.</returns>
        public virtual FluentUriBuilder CreateRequest(string uri, params KeyValuePair<string, object>[] parameters)
        {
            return new FluentUriBuilder(uri);
        }

        private async Task<T?> CallApiAsync<T>(Func<HttpClient, Task<HttpResponseMessage>> apiCall, string applicableProvider)
        {
            string valueString = string.Empty;
            string message;
            try
            {
                var timer = Stopwatch.StartNew();
                T? value;

                using (HttpResponseMessage response = await apiCall(_httpClient ?? new HttpClient()))
                {
                    if (response == null)
                    {
                        message = $"CallApiAsync {applicableProvider} Response is null";
                        _logger.LogInformation(message);
                    }
                    else if (response.Content == null)
                    {
                        message = $"CallApiAsync {applicableProvider} response.Content is null";
                        _logger.LogInformation(message);
                    }
                    else
                    {
                        valueString = await response.Content.ReadAsStringAsync();
                    }

                    if (string.IsNullOrEmpty(valueString))
                    {
                        message = $"Response from {applicableProvider}: {valueString}";
                        _logger.LogInformation(message);
                    }

                    timer.Stop();

                    if (typeof(T?) == typeof(string))
                    {
                        //Allow string objects to bypass deserialization
                        value = (T)(object)valueString;
                    }
                    else
                    {
                        if (valueString.TrimStart().StartsWith('<'))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(T?));
                            // convert string to stream
                            byte[] byteArray = Encoding.UTF8.GetBytes(valueString);
                            using MemoryStream stream = new MemoryStream(byteArray);
                            value = (T?)serializer.Deserialize(stream);
                        }
                        else
                        {
                            value = JsonConvert.DeserializeObject<T?>(valueString);
                            message = $"CallApiAsync {applicableProvider} DeserializeObject.";
                            _logger.LogInformation(message);
                        }
                    }
                }

                return value;
            }
            catch (Exception ex)
            {
                message = $"CallApiAsync {applicableProvider} Exception valueString: {valueString}";
                _logger.LogInformation(ex, message);

                message = $"Call to {nameof(CallApiAsync)} {applicableProvider} error message: {ex.Message}";
                _logger.LogError(ex, message);

                throw new UnhandledException(message);
            }
        }

        private async Task<T?> CallApiAsync<T>(string description, Func<HttpClient, Task<HttpResponseMessage>> apiCall, string applicableProvider)
        {
            string message;
            try
            {
                var result = await CallApiAsync<T>(apiCall, applicableProvider);
                return result;
            }
            catch (WebApiException ex) when (!ex.IsTransientError)
            {
                throw;
            }
            catch (Exception ex)
            {
                message = $"HTTP call to {applicableProvider} failed. - msg: {ex.Message} - description: {description}";
                _logger.LogInformation(ex, message);

                throw new UnhandledException(message);
            }
        }

        private async Task<T?> SafeCallApiAsync<T>(string description, Func<HttpClient, Task<HttpResponseMessage>> apiCall, string applicableProvider)
        {
            string message;
            try
            {
                T? result = await CallApiAsync<T?>(apiCall, applicableProvider);

                return result;
            }
            catch (WebApiException ex) when (!ex.IsTransientError)
            {
                message = $"HTTP call to {applicableProvider} failed with WebApiException - msg: {ex.Message}";
                _logger.LogError(ex, message);

                return default;
            }
            catch (Exception ex)
            {
                message = $"HTTP call to {applicableProvider} failed - msg: {ex.Message} - StackTrace: {ex.StackTrace}";
                _logger.LogError(ex, message);

                return default;
            }
        }

        private async Task<HttpResponseMessage?> CheckForWebException(string description, Task<HttpResponseMessage?> response, bool ignoreChecking = false)
        {
            string message;
            try
            {
                HttpResponseMessage? res = await response;

                if (res == null)
                {
                    _logger.LogInformation(description);
                }

                if (res != null && !res.IsSuccessStatusCode)
                {
                    string content = res.Content != null ? await res.Content.ReadAsStringAsync() : string.Empty;
                    string warning = "Call returned with " + description + " statusCode " + res.StatusCode + " and response " + content;
                    _logger.LogWarning(warning);

                    if (!ignoreChecking)
                    {
                        throw new WebApiException(res.StatusCode, "Call unsuccessful. " + warning);
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                message = $"Call to CheckForWebException error message: {ex.Message} - {ex.InnerException} | Description: {description}";
                _logger.LogError(ex, message);

                throw new UnhandledException(message);
            }
        }

        private StringContent SerializeAsJsonString<T>(T obj, JsonSerializerSettings? jsonSerializerSettings = null)
        {
            try
            {
                string content = JsonConvert.SerializeObject(obj, jsonSerializerSettings);
                return new StringContent(content, Encoding.UTF8, "application/json");
            }
            catch (Exception ex)
            {
                string message = $"Serizalize Error: payload: {obj} -msg: {ex.Message} | StackTrace: {ex.StackTrace}";
                _logger.LogError(ex, message);

                throw new UnhandledException(message);
            }
        }

        private static StringContent SerializeAsXmlString<T>(T obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(obj!.GetType());

            using StringWriter textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, obj);
            StringContent xmlString = new StringContent(textWriter.ToString(), Encoding.UTF8, "text/xml");

            return xmlString;
        }

        private static StringContent SerializeAsSoapXmlString<T>(T obj)
        {
            return new StringContent(obj as string, Encoding.UTF8, "text/xml");
        }

        private string GetAccessToken()
        {
            if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null)
            {
                Claim? claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(e => e.Type == "AccessToken");

                if (claim != null)
                {
                    return claim.Value;
                }
            }

            return string.Empty;
        }

        protected Dictionary<string, string> AddDefaultAccessTokenRequestHeaders()
        {
            string accessToken = GetAccessToken();

            var requestHeaders = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {accessToken}" }
            };

            return requestHeaders;
        }
    }
}