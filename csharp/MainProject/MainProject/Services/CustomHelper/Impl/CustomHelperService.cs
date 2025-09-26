using MainProject.Services.CustomHelper.Models.FnRes;
using Microsoft.Extensions.Localization;
//using NLog;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Xml.Serialization;
using System.Xml;
using System.Globalization;
using System.Security.Cryptography;

namespace MainProject.Services.CustomHelper.Impl
{
    public class CustomHelperService : ICustomHelper
    {
        private readonly string? _currentServiceName;
        private readonly IConfiguration _configuration;
        //private static readonly Logger _unificateLogger = LogManager.GetCurrentClassLogger();// No usar, porque aqui se guardan todos los logs juntos
        //private static readonly Logger _generalLogger = LogManager.GetLogger("generalLogger"); // Usar para separar mensajes
        //private static readonly Logger _requestLogger = LogManager.GetLogger("requestLogger"); // Usar para separar mensajes
        private readonly IWebHostEnvironment _environment;
        private readonly JsonSerializerOptions _currentJsonOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // ========== CONSTRUCTOR ========= //
        public CustomHelperService(
            IWebHostEnvironment environment,
            IConfiguration argconfiguration,
            IHttpContextAccessor arghttpContextAccessor
        )
        {
            _environment = environment;
            _currentServiceName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name; // set the current class name
            _configuration = argconfiguration;
            _currentJsonOptions = this.getJsonSerializeOptions();
            _httpContextAccessor = arghttpContextAccessor;

        }

        // Define a custom Helper to merge two arrays (Dictionary in csharp)
        public Dictionary<string, object> MergeArrays(Dictionary<string, object> array1, Dictionary<string, object> array2)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            foreach (var kvp in array1)
            {
                result[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in array2)
            {
                if (result.ContainsKey(kvp.Key))
                {
                    // If the key already exists, update the value
                    result[kvp.Key] = kvp.Value;
                }
                else
                {
                    // If the key doesn't exist, add it to the result
                    result.Add(kvp.Key, kvp.Value);
                }
            }

            return result;
        }


        // Define a custom Helper to get JsonSerialize options
        public JsonSerializerOptions getJsonSerializeOptions()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                //Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) // Convierte & en \u0026
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // No escapa el &

            };

            return serializeOptions;
        }


        // Define a custom Helper

        public ObjFnResCustomConverToInt32 CustomConvertToInt32(string? lsPropertyName, string? lsPropertyValue)
        {
            ObjFnResCustomConverToInt32 objFnRes = new ObjFnResCustomConverToInt32()
            {
                Status = false,
                Message = "",
                Data = new List<int>()
            };

            string lsPropertyNameNotNull = lsPropertyName ?? "Property name received is null";
            string lsPropertyValueNotNull = lsPropertyValue ?? "-1";

            int? li_converted_or_null = -1;
            int li_converted_aux = -1;
            try
            {
                // falla al venir decimales - errorJahir
                li_converted_or_null = Convert.ToInt32(lsPropertyValueNotNull);
                li_converted_aux = li_converted_or_null ?? -1;

                objFnRes.Status = true;
                objFnRes.Data.Add(li_converted_aux);
            }
            catch (FormatException ex)
            {
                //Console.WriteLine("An exception occurred: " + ex.Message);
                objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): stringProperty value(" + lsPropertyValueNotNull + ") conversion failed: Convert.ToInt32(" + lsPropertyNameNotNull + "). Exception: " + ex.Message;
            }

            return objFnRes;
        }


        // Define a custom helper
        public class ObjFnResCustomConverToString
        {
            public bool status { get; set; } = false;
            public string message { get; set; } = "";
            public List<string> data { get; set; } = new();
        }
        public (bool status, string message, List<string> data) CustomConvertToString(string? lsPropertyName, string? lsPropertyValue)
        {
            ObjFnResCustomConverToString objFnRes = new ObjFnResCustomConverToString()
            {
                status = false,
                message = "",
                data = new List<string>()
            };

            string lsPropertyNameNotNull = lsPropertyName ?? "Property name received is null";
            string lsPropertyValueNotNull = lsPropertyValue ?? ""; // "" empty string

            string? ls_converted_or_null = "";
            string ls_converted_aux = "";
            try
            {
                ls_converted_or_null = Convert.ToString(lsPropertyValueNotNull);
                ls_converted_aux = ls_converted_or_null ?? "";

                objFnRes.status = true;
                objFnRes.data.Add(ls_converted_aux);
            }
            catch (FormatException ex)
            {
                //Console.WriteLine("An exception occurred: " + ex.Message);
                objFnRes.message = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): jsonStringProperty value(" + lsPropertyValueNotNull + ") conversion failed: Convert.ToString(" + lsPropertyNameNotNull + "). Exception: " + ex.Message;
            }

            return (objFnRes.status, objFnRes.message, objFnRes.data);
        }


        // Define a custom Helper
        public class ObjFnResCustomConverToDateTime
        {
            public bool Status { get; set; } = false;
            public string Message { get; set; } = "";
            public List<DateTime> Data { get; set; } = new();
        }
        public (bool status, string message, List<DateTime> data) CustomConvertToDateTime(string? lsPropertyName, string? lsPropertyValue, int? li_line_number = null)
        {
            ObjFnResCustomConverToDateTime objFnRes = new ObjFnResCustomConverToDateTime()
            {
                Status = false,
                Message = "",
                Data = new List<DateTime>()
            };

            string lsPropertyNameNotNull = lsPropertyName ?? "Property name received is null";
            string lsPropertyValueNotNull = lsPropertyValue ?? "35/1/1900 00:00:00"; // Error 35 enero is error
            int liLineNumberValueNotNull = li_line_number ?? LineNumber();


            DateTime rsDateTime = DateTime.Parse("1/1/1900 00:00:00"); // Defino e inicializo 
            try
            {
                rsDateTime = Convert.ToDateTime(lsPropertyValueNotNull);

                objFnRes.Status = true;
                objFnRes.Data.Add(rsDateTime);
            }
            catch (FormatException)
            {
                objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + liLineNumberValueNotNull + "): jsonStringProperty value(" + lsPropertyValueNotNull + ") conversion failed: Convert.ToDateTime(" + lsPropertyNameNotNull + ")";
            }

            return (objFnRes.Status, objFnRes.Message, objFnRes.Data);
        }


        // Define a custom Helper to get LineNumber on Error
        public int LineNumber([System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            return lineNumber;
        }

        // Define a custom Helper
        public double howManyHoursHavePassedSinceByDateTime(DateTime oldDateTime)
        {
            // Get the current date and time
            DateTime currentDateTime = DateTime.Now;

            // Calculate the time difference
            TimeSpan timeDifference = currentDateTime - oldDateTime;

            return timeDifference.TotalHours;
        }

        // Define a custom Helper
        public string? getMergedPropertyString(string ls_property_name, Dictionary<string, object> ldictionary_mergedResult)
        {
            string? ls_property_value = null;
            if (ldictionary_mergedResult.TryGetValue(ls_property_name, out object? ls_property_value_aux) && ls_property_value_aux is string)
            {
                ls_property_value = (string)ls_property_value_aux;
            }
            return ls_property_value;
        }

        // Define a custom Helper
        public int? getMergedPropertyInt(string ls_property_name, Dictionary<string, object> ldictionary_mergedResult)
        {
            int? li_property_value = null;

            if (ldictionary_mergedResult.TryGetValue(ls_property_name, out object? li_property_value_aux))
            {
                li_property_value = (int)li_property_value_aux;
            }

            return li_property_value;
        }

        // Define a custom Helper
        public long? getMergedPropertyLong(string ls_property_name, Dictionary<string, object> ldictionary_mergedResult)
        {
            long? ll_property_value = null;

            if (ldictionary_mergedResult.TryGetValue(ls_property_name, out object? ll_property_value_aux) && ll_property_value_aux is long)
            {
                ll_property_value = (long)ll_property_value_aux;
            }

            return ll_property_value;
        }

        // Define a custom Helper
        public ulong? getMergedPropertyUlong(string ls_property_name, Dictionary<string, object> ldictionary_mergedResult)
        {
            ulong? lul_property_value = null;

            if (ldictionary_mergedResult.TryGetValue(ls_property_name, out object? lul_property_value_aux) && lul_property_value_aux is ulong)
            {
                lul_property_value = (ulong)lul_property_value_aux;
            }

            return lul_property_value;
        }

        // Define a custom Helper
        public void consoleLog(object message)
        {
            bool showConsoleLogs = _configuration.GetValue<bool>("CustomErrorReporting:EnableDevelopmentConsoleLog");

            if (showConsoleLogs)
            {
                Console.WriteLine(message);
            }
        }

        // Define a custom Helper
        public void errorLog(object message)
        {
            bool enableErrorLogToFile = _configuration.GetValue<bool>("CustomErrorReporting:EnableErrorLogToFile");

            if (enableErrorLogToFile)
            {
                //_generalLogger.Error(message);                
            }
        }

        // Define a custom Helper
        public void requestInLog(object message)
        {
            bool enableRequestInLogToFile = _configuration.GetValue<bool>("CustomErrorReporting:EnableInMessagesLogToFile");

            if (enableRequestInLogToFile)
            {
                //_requestLogger.Info(message);
            }
        }

        // Define a custom Helper
        public void requestOutLog(object message)
        {
            bool enableRequestOutLogToFile = _configuration.GetValue<bool>("CustomErrorReporting:EnableOutMessagesLogToFile");

            if (enableRequestOutLogToFile)
            {
                //_requestLogger.Info(message);
            }
        }

        // Define a custom Helper
        public string? Trans(IStringLocalizer _localizer, string ls_property)
        {
            string? value = null;
            if (_localizer[ls_property].ResourceNotFound)
            {
                return value;
            }

            return value = _localizer[ls_property];
        }

        // Define a custom Helper
        public string Substring(string ls_input, int li_start, int maxLength)
        {

            string cutString = ls_input.Length <= maxLength
                ? ls_input
                : ls_input.Substring(li_start, maxLength);

            return cutString;
        }

        // Define a custom Helper
        public JsonDocument CreateJsonObjReadonly(string ls_json = "")
        {
            string ls_error;
            // json Object modo lectura
            JsonDocument lo_jsonObject = JsonDocument.Parse("{}"); // Define an Empty Json Object
            try
            {
                lo_jsonObject = JsonDocument.Parse(ls_json); // redeclare with received values
            }
            catch (JsonException ex)
            {
                // Exception is allowed. Dont break our script. Continue ok.
                ls_error = ex.Message;
            }

            return lo_jsonObject; // Empty json object or json with properties received
        }

        // Define a custom Helper
        public string RemoveLeadingZeros(string input)
        {
            int index = 0;

            // Find the index of the first non-zero character
            while (index < input.Length && input[index] == '0')
            {
                index++;
            }

            // Remove leading zeros
            return input.Substring(index);
        }

        // Define a custom Helper
        public DateTime? GetLastCompiledProject(string ls_main_project_name)
        {
            string ls_project_dll_name = "MainProject.dll"; // Al publicar el proyecto actual se crea este archivo
            ls_project_dll_name = ls_main_project_name + ".dll"; // "MainProject" + ".dll"

            // Get the current working directory
            string currentDirectory = Directory.GetCurrentDirectory() + "\\"; // C:\csharp_projects_deployment\com.kikescsharp.davivienda_deposits_notification\MainProject\MainProject\
            /*Extra info dev*/
            //Console.WriteLine($"Current Directory: {currentDirectory}");                


            string filePath = currentDirectory + ls_project_dll_name; // C:\csharp_projects_deployment\com.kikescsharp.davivienda_deposits_notification\MainProject\MainProject\MainProject.dll

            // Path for developer environment
            if ("Development" == _environment.EnvironmentName)
            {
                filePath = currentDirectory + "bin\\Debug\\net6.0\\" + ls_project_dll_name;
            }

            DateTime? lastModified = null;

            if (System.IO.File.Exists(filePath))
            {
                lastModified = System.IO.File.GetLastWriteTime(filePath);
                //Console.WriteLine($"The file was last modified on: {lastModified}");
            }
            else
            {
                //Console.WriteLine("File does not exist.");
            }

            return lastModified;
        }

        // Define a custom helper
        public string ConvertJsonToXml(string ls_json)
        {
            string xmlOutput = "";

            // Deserialize JSON string to JsonDocument
            using (JsonDocument document = JsonDocument.Parse(ls_json))
            {
                // Iterate over the properties of the JSON document
                foreach (JsonProperty property in document.RootElement.EnumerateObject())
                {
                    xmlOutput += $"<{property.Name}>";

                    switch (property.Value.ValueKind)
                    {
                        case JsonValueKind.Object:
                            xmlOutput += this.ConvertJsonToXml(property.Value.GetRawText());
                            break;

                        case JsonValueKind.Array:
                            foreach (JsonElement element in property.Value.EnumerateArray())
                            {
                                xmlOutput += $"<{property.Name}>";
                                xmlOutput += this.ConvertJsonToXml(element.GetRawText());
                                xmlOutput += $"</{property.Name}>";
                            }
                            break;

                        default:
                            // Otherwise, add the property value directly
                            xmlOutput += property.Value.ToString();
                            break;
                    }

                    xmlOutput += $"</{property.Name}>";
                }
            }

            return xmlOutput;
        }

        // Define a custom helper
        public string XmlObjectToString<T>(T obj, bool enableIndentation = false)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", ""); // Remove namespaces

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true; // Remove XML declaration
            settings.Indent = enableIndentation ? true : false; // Enable or disable indentation based on the boolean parameter




            using (StringWriter textWriter = new StringWriter())
            using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
            {
                xmlSerializer.Serialize(xmlWriter, obj, namespaces);
                return textWriter.ToString();
            }
        }

        // Define a custom helper
        public void CustomNumberDecimalSeparator(string ls_number_decimal_separator = ".")
        {
            // Set point as default decimal separator
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ls_number_decimal_separator;
            Thread.CurrentThread.CurrentCulture = culture;
        }

        // Define a custom helper
        public string CustomStringToDoubleWithLastTwoDecimal(string ls_numbers)
        {
            // "125" equivale a "1.25" segun Banco Occidente
            // El caso de negativos NO ha sido tenido en cuenta
            // ¡¡¡Este metodo se puede reprogramar simplemente poniendo un punto antes de los ultimos 2 decimales!!!

            string ls_aux_total = ""; // Initial value for string received
            string ls_double = "0";   // Default value Response            
            string ls_entera = "0";   // Default zero
            string ls_decimal = "";   // Default empty            
            ls_aux_total = ls_numbers;

            // Empty string then return "0"
            if (ls_aux_total.Length == 0)
            {
                return ls_double;
            }

            // Si viene solo 1 caracter, anteponemos un zero para que sean 2 characters
            if (ls_aux_total.Length == 1)
            {
                ls_decimal = "0" + ls_aux_total;
            }

            // Al menos 2 caracteres para tomarlos como decimales
            if (ls_aux_total.Length >= 2)
            {
                ls_decimal = ls_aux_total.Substring(ls_aux_total.Length - 2);
            }

            // Al menos 3 caracteres para tomar cinco como parte entera. ej: 5,99
            if (ls_aux_total.Length > 2)
            {
                ls_entera = this.Substring(ls_aux_total, 0, ls_aux_total.Length - 2); // first length-2 characters
            }

            // Concatenamos el punto
            if (ls_decimal.Length > 0)
            {
                ls_double = ls_entera + "." + ls_decimal;
            }

            return ls_double;
        }


        // Define a custom Helper
        // Encriptar una cadena de texto usando AES
        public string EncryptString(string ls_plain_ext, string ls_key)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(ls_key);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.GenerateIV();

                var iv = aes.IV;
                using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
                using (var ms = new MemoryStream())
                {
                    ms.Write(iv, 0, iv.Length);
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(ls_plain_ext);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        // Desencriptar una cadena de texto encriptada usando AES
        public string DecryptString(string ls_cipher_text, string ls_key)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(ls_key);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                var ivLength = aes.IV.Length;

                using (var ms = new MemoryStream(Convert.FromBase64String(ls_cipher_text)))
                {
                    var iv = new byte[ivLength];
                    ms.Read(iv, 0, iv.Length);
                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }


        // Para ATH no se requiere el root tagxml del objeto a devolver.
        public string RemoveRootElement(string ls_xmlContent)
        {
            // Load the XML into an XmlDocument
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(ls_xmlContent);

            // Get the root element
            var rootElement = xmlDoc.DocumentElement;

            // Remove the root element and get its inner XML
            string innerXml = rootElement.InnerXml;

            return innerXml;
        }


        // Define a custom helper
        public string AddParameterToRequestQuery(string url, string param, string value)
        {
            // Check if the URL already has a query string
            var uriBuilder = new UriBuilder(url);
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);

            // Add or update the parameter
            query[param] = value;
            uriBuilder.Query = query.ToString();            

            // Return the modified URL
            return uriBuilder.ToString();            
        }


        // Define a custom helper
        public HttpContext GetCurrentHttpContext()
        {            
            HttpContext httpContext;            
            
            if (_httpContextAccessor.HttpContext is null)
            {
                // Esto es más por seguridad
                throw new Exception("||" + _currentServiceName + "|| Error line number (" + this.LineNumber() + "):" + "Error al tratar de acceder al httpContext");
            }

            httpContext = _httpContextAccessor.HttpContext;

            return httpContext;
        }


        /// END CLASS SERVICE
    }
}
