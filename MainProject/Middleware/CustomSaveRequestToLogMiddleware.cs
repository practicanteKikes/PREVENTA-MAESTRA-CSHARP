using NLog;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace MainProject.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class CustomSaveRequestToLogMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Logger _requestLogger = LogManager.GetLogger("requestLogger"); // Usar para separar mensajes

        public CustomSaveRequestToLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            // Extract and log URL
            string url = httpContext.Request.Path + httpContext.Request.QueryString;
            string method = httpContext.Request.Method;
            var headers = httpContext.Request.Headers;
            List<string> la_headers = new();
            // Extract and log body (assuming it's a text-based body)
            var requestBody = await ReadRequestBody(httpContext.Request);
            //Console.WriteLine("body recibido");
            //Console.WriteLine(requestBody);
            JsonDocument bodyJson = JsonDocument.Parse("{}");
            string ls_error = "";

            try
            {
                bodyJson = JsonDocument.Parse(requestBody); // util modo solo lectura
            }
            catch (JsonException ex)
            {
                ls_error = ex.Message;
                //Console.WriteLine($"Error parsing JSON: {ex.Message}");
            }


            foreach (var (key, value) in headers)
            {
                //_logger.LogInformation($"Header: {key} - {value}");
                la_headers.Add($"Header: {key} - {value}");
            }

            // working with api rest
            //var lo_stdObj = new { url = url, method = method, headers = la_headers, body = bodyJson };

            // working with soap xml
            string? ls_input_xml = requestBody ?? "";
            string ls_cdata_content = "";
            if (ls_input_xml.Length > 0)
            {
                var xmlDocInput = new XmlDocument();
                xmlDocInput.LoadXml(ls_input_xml); // load FULL cdata element
                ls_cdata_content = xmlDocInput.OuterXml; // <referenciar-recaudo-input>...</referenciar-recaudo-input>                            
                ls_cdata_content = ls_input_xml.Trim(); // full body input
            }
            

            var lo_stdObj = new { url = url, method = method, headers = la_headers, body = ls_cdata_content };

            // working with api rest
            //string messageLog = JsonSerializer.Serialize(lo_stdObj, new JsonSerializerOptions
            //{
            //    WriteIndented = false
            //});

            // working with soap xml
            string messageLog = lo_stdObj.ToString();

            _requestLogger.Info("[ REQUEST INCOMING START ]: "+ "\r\n" + messageLog);
            _requestLogger.Info("REQUEST INCOMING END" + "\r\n"+ "\r\n");
            await _next(httpContext);
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
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMyCustomSaveRequestToLogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomSaveRequestToLogMiddleware>();
        }
    }
}
