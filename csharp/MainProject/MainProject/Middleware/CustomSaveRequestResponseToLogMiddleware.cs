using NLog;
using System.Text.Json;
using System.Text;

namespace MainProject.Middleware
{
    public class CustomSaveRequestResponseToLogMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Logger _requestLogger = LogManager.GetLogger("requestLogger"); // Usar para separar mensajes

        public CustomSaveRequestResponseToLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            // Extract and log URL
            string url = httpContext.Request.Path + httpContext.Request.QueryString;
            string method = httpContext.Request.Method;
            var headers = httpContext.Request.Headers;
            List<string> la_headers = new();
            // Extract and log body (assuming it's a text-based body)
            var requestBody = await ReadRequestBody(httpContext.Request);

            // working with api rest
            //JsonDocument bodyJson = JsonDocument.Parse("{}");
            //string ls_error = "";

            //try
            //{
            //    bodyJson = JsonDocument.Parse(requestBody); // util modo solo lectura
            //}
            //catch (JsonException ex)
            //{
            //    ls_error = ex.Message;
            //    //Console.WriteLine($"Error parsing JSON: {ex.Message}");
            //}


            foreach (var (key, value) in headers)
            {
                //_logger.LogInformation($"Header: {key} - {value}");
                la_headers.Add($"Header: {key} - {value}");
            }


            var originalBodyStream = httpContext.Response.Body;
            using (var memoryStream = new MemoryStream())
            {
                httpContext.Response.Body = memoryStream;

                await _next(httpContext);

                memoryStream.Seek(0, SeekOrigin.Begin);
                string responseBody = new StreamReader(memoryStream).ReadToEnd();
                if (responseBody.Length == 0)
                {
                    responseBody = "(response is empty)";
                }

                // Log or process the response as needed
                var lo_stdObj = new { url = url, method = method, headers = la_headers, body = requestBody };
                
                // working with api rest
                //string ls_std_request = JsonSerializer.Serialize(lo_stdObj, new JsonSerializerOptions
                //{
                //    WriteIndented = true
                //});

                // working with soap xml
                string ls_std_request = lo_stdObj.ToString();
                
                //_requestLogger.Info("[ INCOMING REQUESTED ]: " + ls_std_request); // Info util developers
                _requestLogger.Info($"[ RESPONSE COMING OUT ]: status code [{httpContext.Response.StatusCode}]: " + "\r\n" + responseBody);
                _requestLogger.Info("RESPONSE COMING OUT END" + "\r\n" + "\r\n");

                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering(); // Allow the body to be read multiple times

            using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                var body = await reader.ReadToEndAsync();
                request.Body.Seek(0, SeekOrigin.Begin); // Reset the stream position for subsequent middleware
                return body;
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MyCustomSaveRequestResponseToLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseMyCustomSaveRequestResponseToLogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomSaveRequestResponseToLogMiddleware>();
        }
    }
}
