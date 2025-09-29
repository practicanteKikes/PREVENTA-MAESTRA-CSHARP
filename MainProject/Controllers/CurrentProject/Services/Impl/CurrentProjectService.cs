using MainProject.Controllers.CurrentProject.Controllers;
using MainProject.Controllers.CurrentProject.Models.FnRes;
using MainProject.Controllers.CurrentProject.Models.Json;
using MainProject.Controllers.CurrentProject.Models.ObtainUsersObjectListAzync.FnRes;
using MainProject.Services.CustomDatabaseInput;
using MainProject.Services.CustomHelper;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace MainProject.Controllers.CurrentProject.Services.Impl
{
    public class CurrentProjectService : ICurrentProject
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _current_project_name;
        private readonly ICustomHelper _customHelper;
        private readonly JsonSerializerOptions _currentJsonOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer _localizer;
        private readonly IBionegociosDatabaseInput _customSybaseService; // tiene los metodos de la interfaz principal

        // CALCULATED
        private readonly string? _currentServiceName;

        public CurrentProjectService(
            IWebHostEnvironment environment,
            ICustomHelper argcustomHelperService,

            IHttpContextAccessor arghttpContextAccessor,
            IStringLocalizerFactory factory,
            IBionegociosDatabaseInput argcustomSybaseService
        )
        {
            _environment = environment;

            // Get the current project name
            var controllerType = typeof(CurrentProjectController);
            string ls_current_project_name = controllerType.GetTypeInfo().Assembly.FullName ?? "here must be the project name"; // "MainProject"
            var assemblyObj = new AssemblyName(ls_current_project_name); // assemblyName.Name give us "MainProject"
            assemblyObj.Name = assemblyObj.Name ?? ls_current_project_name;
            _current_project_name = assemblyObj.Name;
            //Console.WriteLine(assemblyObj.Name);

            // SET CURRENT CULTURE START            
            CultureInfo culture = new CultureInfo("es-CO"); // Version correcta en Servidor Dell Local
            CultureInfo cultureUI = new CultureInfo("es-CO"); // Version correcta en Servidor Dell Local - Es el usado en las traducciones
            culture.NumberFormat.NumberDecimalSeparator = "."; // Set point as default decimal separator            
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = cultureUI;
            // SET CURRENT CULTURE END
            

            _customHelper = argcustomHelperService;
            _currentServiceName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name; // set the current class name
            _currentJsonOptions = _customHelper.getJsonSerializeOptions();
            _httpContextAccessor = arghttpContextAccessor;

            // ===== TRANSLATION INIT =====           
            // Default route for localizer is: MainProject.Resources.Controllers.TestController
            // In Program.cs is setted folder: "Controllers" so Then Base must be: MainProject.Controllers
            // Customized is: MainProject.Controllers.CustomDavivienda.Resources.TestController
            _localizer = factory.Create("CustomMaestras.Resources.CurrentProjectController", assemblyObj.Name);
            // ===== TRANSLATION END =====

            _customSybaseService = argcustomSybaseService;
        }

        public string MainProjectPublishedDateTime(string ls_input_json)
        {
            ObjFnResMainProjectPublishedDateTimeDev objFnResDev = new();

            string ls_project_dll_name = "MainProject.dll"; // Al publicar el proyecto actual se crea este archivo
            ls_project_dll_name = _current_project_name + ".dll"; // "MainProject" + ".dll"

            // Get the current working directory
            string currentDirectory = Directory.GetCurrentDirectory() + "\\";
            // C:\csharp_projects_deployment\com.kikescsharp.davivienda_deposits_notification\MainProject\MainProject\
            
            
            //Console.WriteLine($"Current Directory: {currentDirectory}");                


            string filePath = currentDirectory + ls_project_dll_name;
            // C:\csharp_projects_deployment\com.kikescsharp.davivienda_deposits_notification\MainProject\MainProject\MainProject.dll

            

            // Path for developer environment
            if ("Development" == _environment.EnvironmentName)
            {
                filePath = currentDirectory + "bin\\Debug\\net6.0\\" + ls_project_dll_name;
            }

            //Console.WriteLine($"File Path: {filePath}");

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

            //var stdClass = new
            //{
            //    Status = true,
            //    Message = "Proceso está corriendo exitosamente. || version " + lastModified.ToString(),
            //    Data = new List<string>()
            //};

            DateTime? ldt_fecha_hora_servidor = _customSybaseService.HoraServidor(); // 2024-11-27T13:40:32.93

            objFnResDev.Ksuccess = true;
            objFnResDev.Kmessage = "Proceso está corriendo exitosamente. || version " + lastModified.ToString();
            objFnResDev.Kldt_fecha_servidor_ase = ldt_fecha_hora_servidor;
            objFnResDev.Kdata = new List<string>();

            return JsonSerializer.Serialize(objFnResDev, _currentJsonOptions);
        }




        // API publica por ahora
        public async Task<string> MainGetUserFromDbAsync(string ls_ctrl_json)
        {
            // INICIALIZAMOS VALORES DEFECTO DE RESPUESTA
            ObjFnResMainGetUserFromDbDev objFnResDev = new();


            // INICIALIZAMOS VARIABLES REQUERIDAS DE FLUJO                        
            HttpContext? httpContext = _httpContextAccessor.HttpContext;
            string ls_error = "";
            bool lb_developer_mode = false;

            
            if (httpContext is null)
            {                       
                ls_error += "[Error al tratar de acceder al httpContext]: ";
                ls_error = this.KikesDeveloperErrorResponse(ls_error, _customHelper.LineNumber());

                // Body response
                objFnResDev.KerrorCode = "1"; // Error code: Error inesperado
                objFnResDev.Ksuccess = false;
                objFnResDev.Kmessage = ls_error;

                return this.ResponseFormatGetUser(objFnResDev, lb_developer_mode);
            }
            else
            {
                // ===== ¡¡¡ httpContext EXIST!!! =====
                var Request = httpContext.Request; // Maybe required


                // read developerMode from Headers!!!               
                if (httpContext.Request.Headers.TryGetValue("developerMode", out var developerModeHeader))
                {
                    if ("true" == developerModeHeader.ToString()) lb_developer_mode = true;
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
               

                // QUERY DATABASE BY AJAX???
                if (true)
                {                    
                    List<UserJsonModelCP> userListToReturn = new();
                    ObjFnResObtainUsersObjList ObtainUsuariosObjListResult = await this.ObtainUsersObjectListAzync();


                    // ERROR ???
                    if (ObtainUsuariosObjListResult.Ksuccess == false)
                    {
                        // Header response [500] Server error
                        httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                        ls_error = this.KikesDeveloperErrorResponse("Error al obtener users object list . " + ObtainUsuariosObjListResult.Kmessage, _customHelper.LineNumber());

                        // Body response
                        objFnResDev.Ksuccess = false;
                        objFnResDev.Kmessage = ls_error;
                        objFnResDev.KerrorCode = "3306500"; // Error de conexion a la bd api                         
                        return this.ResponseFormatGetUser(objFnResDev, lb_developer_mode);
                    }


                    // RECORDS FOUND???
                    int li_total_records = ObtainUsuariosObjListResult.Kdata.Count;
                    if (ObtainUsuariosObjListResult.Ksuccess && li_total_records >= 0)
                    {
                        // FOREACH ORIGINAL RECORD FROM DB CONVERT TO DESIRED RESPONSE
                        for (int i = 0; i < li_total_records; i++)
                        {
                            var currentUserOriginal = ObtainUsuariosObjListResult.Kdata[i];
                            var currentUserJsonDesired = new UserJsonModelCP();                            

                            currentUserJsonDesired.Nombre_usuario = (currentUserOriginal.Nombre_usuario is null)? "" : currentUserOriginal.Nombre_usuario.Trim();

                            //OPTIONAL CUSTOMIZE PROPERTIES IN RESULT
                            if (currentUserJsonDesired.Nombre_usuario.Length >= 10)
                            {
                                string firstTenChars = currentUserJsonDesired.Nombre_usuario.Substring(0, 10);
                                currentUserJsonDesired.Nombre_usuario = firstTenChars;
                            }

                            // Finally add element to response list
                            userListToReturn.Add(currentUserJsonDesired);
                        }
                    }                    


                    objFnResDev.Kdata = userListToReturn; // Persist custom data in objfunresponse
                    // QueryEND Database
                }

                // all ok - Un registro al menos fue retornado
                if (objFnResDev.Kdata.Count > 0)
                {
                    objFnResDev.KerrorCode = "0";
                    objFnResDev.Kmessage = "Proceso finalizado con exito. El servicio Web API tiene conexion a la bd.";
                    objFnResDev.Ksuccess = true;
                }
                else
                {
                    objFnResDev.KerrorCode = "0";
                    objFnResDev.Kmessage = "Proceso finalizado con exito. Pero no se encontraron registros en la bd. Se esperaba al menos 1 registro.";
                    objFnResDev.Ksuccess = true;
                }
                

                // Return expected response
                return this.ResponseFormatGetUser(objFnResDev, lb_developer_mode); // on fail json received, activate developerMode
            }

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
        private string ResponseFormatGetUser(ObjFnResMainGetUserFromDbDev stdFnResDev, bool developerMode = false)
        {
            var httpContext = this.GetCurrentHttpContext();
            var Request = httpContext.Request;

            Dictionary<string, string> statusCodeList = this.GetStatusCodeList();
            string ls_status_desc_aux = stdFnResDev.Kmessage;
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
            ObjFnResMainGetUserFromDbPro objFnResPro = new();
            objFnResPro.Ksuccess = stdFnResDev.Ksuccess;
            objFnResPro.KerrorCode = stdFnResDev.KerrorCode;
            objFnResPro.Kmessage = stdFnResDev.Kmessage;
            objFnResPro.Kdata = stdFnResDev.Kdata;            

            return JsonSerializer.Serialize(objFnResPro, _currentJsonOptions);
        }




        // Define a custom helper
        public async Task<ObjFnResObtainUsersObjList> ObtainUsersObjectListAzync()
        {
            // INICIALIZAMOS VALORES DEFECTO DE RESPUESTA
            // A partir de aqui, obtenemos los valores originales de la base de datos
            ObjFnResObtainUsersObjList objFnResDev = new();

            var httpContext = this.GetCurrentHttpContext();
            string ls_error = "";
            string ls_jsonresponse_bdusers = "{}";

            // SOME VARS TO get full response from remote laft api            
            Dictionary<string, object> la_params_remote = new Dictionary<string, object>
            {                
                // remote required
                { "li_limit", 1 }                
            };


            // AJAX
            try
            {
                ls_jsonresponse_bdusers = await _customSybaseService.QueryUsersFilteredAzync(la_params_remote); // Esperamos una json de respuesta
                /* string lo_table_filtered_total = await this.getAllTiposNegocioTotalRowsFilteredFromDatabaseWithOutLimit(req); */
            }
            catch (JsonException e)
            {
                // Header response [500] Server error
                httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                ls_error = _customHelper.Trans(_localizer, "error_property_json_received") ?? "Error in received object JSON from custom service db: ";
                ls_error += e.Message;
                ls_error = this.KikesDeveloperErrorResponse(ls_error, _customHelper.LineNumber());

                // Body response
                objFnResDev.KerrorCode = "3306409"; // Error parseando respuesta json de bd consultar
                objFnResDev.Ksuccess = false;
                objFnResDev.Kmessage = ls_error;
                return objFnResDev;
            }


            // PARSE AJAX RESPONSE
            // ERRORRR PARSING JSON???
            ObjFnResBdConsultarUsersJsonParsed dbUsersSearchResult = new ObjFnResBdConsultarUsersJsonParsed(); // Force to be an object
            ObjFnResBdConsultarUsersJsonParsed? dbUsersSearchResultOrNull = null; // necesary variable to try catch to cast postman json object            
            try
            {
                dbUsersSearchResultOrNull = JsonSerializer.Deserialize<ObjFnResBdConsultarUsersJsonParsed>(ls_jsonresponse_bdusers, _currentJsonOptions);
            }
            catch (JsonException e)
            {
                // Header response [500] Server error
                httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                ls_error = _customHelper.Trans(_localizer, "error_property_json_received") ?? "Error in received object JSON: ";
                ls_error += e.Message;
                ls_error = this.KikesDeveloperErrorResponse(ls_error, _customHelper.LineNumber());

                // Body response
                objFnResDev.KerrorCode = "3306409"; // Error parseando respuesta json de bd consultar
                objFnResDev.Ksuccess = false;
                objFnResDev.Kmessage = ls_error;
                return objFnResDev;
            }
            dbUsersSearchResult = (dbUsersSearchResultOrNull ?? new ObjFnResBdConsultarUsersJsonParsed());


            // ERROR IN RESPONSE REMOTE DB ???
            if (dbUsersSearchResult.Success == false)
            {
                // Header response [500] Server error
                httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                ls_error = this.KikesDeveloperErrorResponse("Error de conexión a la bd. " + dbUsersSearchResult.Message, _customHelper.LineNumber());

                // Body response
                objFnResDev.Ksuccess = false;
                objFnResDev.Kmessage = ls_error;
                objFnResDev.KerrorCode = "3306500"; // Error de conexion a la bd api                                         
                return objFnResDev;
            }


            // RECORDS FOUND???
            if (dbUsersSearchResult.Data.Count >= 0)
            {
                objFnResDev.KerrorCode = "0"; // Exito!!
                objFnResDev.Ksuccess = true;
                objFnResDev.Kmessage = "Proceso listar clientes finalizado con exito.";
                objFnResDev.Kdata = dbUsersSearchResult.Data;
            }

            // Return database original data
            return objFnResDev;
        }



        // Define a custom helper
        private HttpContext GetCurrentHttpContext()
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



        // Endclass
    }

    // Endnamespace
}
