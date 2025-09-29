using MainProject.Controllers.CustomJwt.Models.DbString;
using MainProject.Controllers.CustomJwt.Services;
using MainProject.Controllers.CustomSwagger.Models.FnRes;
using MainProject.Services.CustomDatabaseInput;
using MainProject.Services.CustomHelper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using MainProject.Controllers.CustomClientes.Controllers;

namespace MainProject.Controllers.CustomSwagger.Services.Impl
{
    public class CustomSwaggerService : ICustomSwagger
    {
        // INJECTED
        private readonly ICustomHelper _customHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IConfiguration _configuration; // Leeremos las tablas o vistas usadas
        private readonly ICustomJwt _customJwtService;
        private readonly IStringLocalizer _localizer;
        private readonly IBionegociosDatabaseInput _customSybaseService; // tiene los metodos de la interfaz principal

        // CALCULATED
        private readonly string? _currentServiceName;
        private readonly JsonSerializerOptions _currentJsonOptions;


        // ========== CONSTRUCTOR ========= //
        public CustomSwaggerService(
            ICustomHelper argcustomHelperService,
            IConfiguration argconfiguration,
            IHttpContextAccessor arghttpContextAccessor,
            ICustomJwt argcustomJwtService,
            IStringLocalizerFactory factory,
            IBionegociosDatabaseInput argcustomSybaseService
        )
        {
            // SET CURRENT CULTURE START            
            CultureInfo culture = new CultureInfo("es-CO"); // Version correcta en Servidor Dell Local
            CultureInfo cultureUI = new CultureInfo("es-CO"); // Version correcta en Servidor Dell Local - Es el usado en las traducciones
            culture.NumberFormat.NumberDecimalSeparator = "."; // Set point as default decimal separator            
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = cultureUI;
            // SET CURRENT CULTURE END

            _customHelper = argcustomHelperService;
            _httpContextAccessor = arghttpContextAccessor;
            _configuration = argconfiguration;
            _customJwtService = argcustomJwtService;

            _currentServiceName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name; // set the current class name
            _currentJsonOptions = _customHelper.getJsonSerializeOptions();


            // ===== TRANSLATION INIT =====
            var controllerType = typeof(CustomClientesController);
            string ls_current_project_name = controllerType.GetTypeInfo().Assembly.FullName ?? "here must be the project name"; // "MainProject"
            var assemblyObj = new AssemblyName(ls_current_project_name); // assemblyName.Name give us "MainProject"
            assemblyObj.Name = assemblyObj.Name ?? ls_current_project_name;


            // Default route for localizer is: MainProject.Resources.Controllers.TestController
            // In Program.cs is setted folder: "Controllers" so Then Base must be: MainProject.Controllers
            // Customized is: MainProject.Controllers.CustomDavivienda.Resources.TestController
            _localizer = factory.Create("CustomMaestras.Resources.CustomMaestrasController", assemblyObj.Name);
            // ===== TRANSLATION END =====

            _customSybaseService = argcustomSybaseService;
        }


        public async Task<string> MainCreateCookieLoginDatabaseAsync(string ls_json)
        {
            ObjFnResGetCookieDev objFnResDev = new();

            // INICIALIZAMOS VARIABLES REQUERIDAS DE FLUJO                        
            HttpContext? httpContext = _httpContextAccessor.HttpContext;
            string ls_error = "";
            bool lb_developer_mode = false;

            // httpContext error ???
            if (httpContext is null)
            {
                // Header response [500] Server error
                // httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)                
                ls_error += "[Error al tratar de acceder al httpContext]: ";
                ls_error = this.KikesDeveloperErrorResponse(ls_error, _customHelper.LineNumber());

                // Body response
                objFnResDev.KerrorCode = "1"; // Error code: Error inesperado
                objFnResDev.Ksuccess = false;
                objFnResDev.Kmessage = ls_error;

                return this.ResponseFormatCreateCookieLoginDatabase(objFnResDev, lb_developer_mode);
            }
            else
            {
                // ===== ¡¡¡ httpContext EXIST!!! =====
                var Request = httpContext.Request;


                // read developerMode from Headers!!!               
                if (httpContext.Request.Headers.TryGetValue("developerMode", out var developerModeHeader))
                {
                    if ("true" == developerModeHeader.ToString())
                    {
                        lb_developer_mode = true;
                    }
                }
                // Now is available lb_developer_mode


                // read developerModeLanguage from Headers!!!               
                if (httpContext.Request.Headers.TryGetValue("developerModeLanguage", out var developerModeLanguageHeader))
                {

                    if ("en" == developerModeLanguageHeader.ToString())
                    {
                        CultureInfo cultureUI = new CultureInfo("en"); // English desired by Seniors Devs - Jajajaj.
                        Thread.CurrentThread.CurrentUICulture = cultureUI;
                    }
                }


                // Leer los valores de username y password desde el formulario
                var stringvalues_user = httpContext.Request.Form["username"];
                var stringvalues_password = httpContext.Request.Form["password"];

                string? ls_user = stringvalues_user.Count > 0 ? stringvalues_user[0] : string.Empty; // Usa string.Empty si no hay valor
                string? ls_password = stringvalues_password.Count > 0 ? stringvalues_password[0] : string.Empty; // Usa string.Empty si no hay valor                

                // define an array asociative ( csharp dictionary )
                Dictionary<string, object> la_params = new Dictionary<string, object>
                {
                    { "ls_user", ls_user?? "one_email" },
                    { "ls_password", ls_password ?? "one_password" }
                };


                // Consultar base de datos
                if(true)
                {
                    // get user from database
                    UserJwtDbStringModel userJwtDbStringM = new(); // JsonModel properties are strings or null                        
                    var userJwtDbSearchResult = _customJwtService.getUserJwtByUserAndPassword(la_params);
                    if (userJwtDbSearchResult.status && userJwtDbSearchResult.data.Count() > 0)
                    {
                        userJwtDbStringM = userJwtDbSearchResult.data[0];

                        // ERROR IF INACTIVE
                        if (!userJwtDbStringM.IsActive())
                        {                            

                            // Header response [500] Server error
                            httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                            ls_error = this.KikesDeveloperErrorResponse("user with credentials is INACTIVE . " + userJwtDbSearchResult.message, _customHelper.LineNumber());

                            // Body response
                            objFnResDev.Ksuccess = false;
                            objFnResDev.Kmessage = ls_error;
                            objFnResDev.KerrorCode = "3306500"; // Error de conexion a la bd api                         
                            return this.ResponseFormatCreateCookieLoginDatabase(objFnResDev, lb_developer_mode);

                        }
                    }

                    // ERRROR NOT FOUND USER
                    if (userJwtDbSearchResult.status == false)
                    {                        
                        // Header response [500] Server error
                        httpContext.Response.StatusCode = 404; // Report error in server to client - Internal Server Error (500)
                        ls_error = this.KikesDeveloperErrorResponse("database user with credentials not found . " + userJwtDbSearchResult.message, _customHelper.LineNumber());

                        // Body response
                        objFnResDev.Ksuccess = false;
                        objFnResDev.Kmessage = ls_error;
                        objFnResDev.KerrorCode = "3306500"; // Error de conexion a la bd api                         
                        return this.ResponseFormatCreateCookieLoginDatabase(objFnResDev, lb_developer_mode);

                    }


                    // LAST VALIDATION - MUST BE ACTIVE
                    if (userJwtDbStringM.IsActive())
                    {                        

                        //////////// username == "user" && password == "password"
                        if (true)
                        {
                            // Aquí podrías generar un token o establecer una cookie de autenticación.
                            // Pero para simplicidad, simplemente redirigiremos al usuario a Swagger.
                            int li_minutos_sesion = 1; // 60
                            li_minutos_sesion = _configuration.GetValue<int>("CustomApp:SwaggerMinutesExpirationCookie");

                            //Console.WriteLine("voy a durar sesion minutos cookies");
                            //Console.WriteLine(li_minutos_sesion.ToString());


                            var claims = new List<Claim>
                            {                                
                                // Default no requerido - pero recomendado para identificar al usuario. Puede ser email o nombre usuario
                                new Claim(ClaimTypes.Name, userJwtDbStringM.Email ?? ""), // Default recomendado

                                // Custom extra claims                                
                                new Claim("minutes_expiration_cookie", li_minutos_sesion.ToString() )
                            };
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            var authProperties = new AuthenticationProperties
                            {
                                // true para mantener la cookie incluso si se cierra el navegador
                                IsPersistent = true, 

                                // Tiempo de expiración - Reescribe valor default de program.cs
                                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(li_minutos_sesion) 
                            };

                            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                                new ClaimsPrincipal(claimsIdentity),
                                authProperties
                            );


                            // Redirige a Swagger si las credenciales son correctas
                            //return Redirect("/swagger/index.html");
                        }
                        ///



                        // end doing stuff
                    }




                }

            }

            // all ok
            objFnResDev.KerrorCode = "0";
            objFnResDev.Kmessage = "Proceso finalizado con exito.";
            objFnResDev.Ksuccess = true;

            // Return expected response
            return this.ResponseFormatCreateCookieLoginDatabase(objFnResDev, lb_developer_mode); // on fail json received, activate developerMode
        }


        // Define a custom helper
        private string KikesDeveloperErrorResponse(string ls_error, int li_line_number)
        {
            string ls_error_lng = "";
            //ls_error_lng = _customHelper.Trans(_localizer, "error_line_number") ?? "Error line number";
            ls_error_lng = "Error line number";

            var stdClass = new
            {
                Success = false,
                Message = "||" + _currentServiceName + "|| " + ls_error_lng + "(" + li_line_number + "): " + ls_error
            };

            return stdClass.Message;
        }


        // Define a custom helper
        private string ResponseFormatCreateCookieLoginDatabase(ObjFnResGetCookieDev stdFnResDev, bool developerMode = false)
        {
            var httpContext = this.GetCurrentHttpContext();
            var Request = httpContext.Request;

            Dictionary<string, string> statusCodeList = this.GetStatusCodeList();
            string ls_status_desc_aux = stdFnResDev.Kmessage ?? "";
            string ls_status_code = stdFnResDev.KerrorCode.ToString();
            string ls_status_desc = statusCodeList[ls_status_code] + ": " + ls_status_desc_aux;

            string ls_text = ""; // Variable auxiliar para hacer transformaciones con texto o enteros

            ls_text = ls_status_code;
            ls_text = ls_text.Trim();
            int errorCodigoInt = -1;
            var objRs = _customHelper.CustomConvertToInt32("Error_codigo", ls_text);
            if (objRs.Status == true && objRs.Data.Count() > 0)
            {
                errorCodigoInt = objRs.Data[0];
            }
            stdFnResDev.KerrorCode = errorCodigoInt.ToString();

            ls_text = ls_status_desc;
            ls_text = ls_text.Trim();
            //ls_text = _customHelper.Substring(ls_text, 0, 100); // si proveedor requiere recortar respuesta a first 100 characters
            stdFnResDev.Kmessage = ls_text;




            if (developerMode)
            {
                // RETURN NAKED XML - NO WRAPING INSIDE CDATA
                return JsonSerializer.Serialize(stdFnResDev, _currentJsonOptions);
            }

            // PRODUCTION RESPONSE
            ObjFnResGetCookiePro objFnResPro = new();
            objFnResPro.Ksuccess = stdFnResDev.Ksuccess;
            objFnResPro.KerrorCode = stdFnResDev.KerrorCode;
            objFnResPro.Kmessage = stdFnResDev.Kmessage;
            objFnResPro.Kdata = stdFnResDev.Kdata;                        

            return JsonSerializer.Serialize(objFnResPro, _currentJsonOptions);
        }


        // Define a custom helper
        private Dictionary<string, string> GetStatusCodeList()
        {
            Dictionary<string, string> statusCodeList = new();

            statusCodeList["380"] = "Datos errados";
            statusCodeList["381"] = "Datos errados en api bd";
            statusCodeList["2664"] = "No Se Pudo Validar Token";
            statusCodeList["2804"] = "Estructura Invalida";
            statusCodeList["2675"] = "Estado De Token No Válido";

            statusCodeList["0"] = "Fue exitoso";
            statusCodeList["1"] = "Error inesperado";
            statusCodeList["3306500"] = "Error de conexion a la bd api";
            statusCodeList["3306409"] = "Error parseando respuesta json al realizar consulta bd api";

            return statusCodeList;
        }


        // Define a custom helper
        public HttpContext GetCurrentHttpContext()
        {
            // INICIALIZAMOS VARIABLES REQUERIDAS DE FLUJO                        
            HttpContext? httpContext = _httpContextAccessor.HttpContext;

            // httpContext error ???
            if (httpContext is null)
            {
                // Esto es más por seguridad
                throw new Exception("Error al tratar de acceder al httpContext");
            }

            return httpContext;
        }


    }
}
