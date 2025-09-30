using MainProject.Controllers.CustomHelperMaestras.Services;
using MainProject.Controllers.CustomJwt.Services;
using MainProject.Controllers.CmZonasBio.Controllers;
using MainProject.Services.CustomDatabaseInput;
using MainProject.Services.CustomHelper;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text.Json;
using MainProject.Controllers.CmZonasBio.Models.CmZonasBioAll.CmZonasBioMainGetAllAsync;
using MainProject.Controllers.CmZonasBio.Models.CmZonasBioAll.CmZonasBioObtainAllObjectListAzync;
using MainProject.Controllers.CmZonasBio.Models.CmZonasBioFiltered.CmZonasBioMainGetFilteredAsync;
using MainProject.Controllers.CmZonasBio.Models.CmZonasBioFiltered.CmZonasBioObtainObjectListAzync;
using MainProject.Controllers.CmZonasBio.Models.CmZonasBioFiltered.CmZonasBioDefaultFilterParams;
using MainProject.Services.CustomHelper.Models.FnRes;
using MainProject.Controllers.CmZonasBio.Models.CmZonasBioFiltered.CmZonasBioObtainTotalRowsFilteredWithOutLimit;
using MainProject.Services.CustomSybase.Models.QueryAAAModelDefaultFnResFTRowsAzync;
using MainProject.Controllers.CmZonasBio.Models.CmZonasBioAll;
using MainProject.Services.CustomSybase.Models.LoadDbInfo.FnRes;
using MainProject.Services.CustomSybase.Models.LoadDbInfo.Json;

namespace MainProject.Controllers.CmZonasBio.Services.Impl
{
    public class CmZonasBioService : ICmZonasBio
    {
        // INJECTED
        private readonly ICustomHelper _customHelper;
        private IConfiguration _configuration; // Leeremos las tablas o vistas usadas
        private readonly ICustomJwt _customJwtService;
        private readonly IStringLocalizer _localizer;
        private readonly IBionegociosDatabaseInput _customSybaseService; // tiene los metodos de la interfaz principal
        private readonly ICustomHelperMaestras _customHelperMaestrasService;

        // CALCULATED
        private readonly string? _currentServiceName;
        private readonly JsonSerializerOptions _currentJsonOptions;


        // ========== CONSTRUCTOR ========= //
        public CmZonasBioService(
            ICustomHelper argcustomHelperService,
            IConfiguration argconfiguration,
            ICustomJwt argcustomJwtService,
            IStringLocalizerFactory factory,
            IBionegociosDatabaseInput argcustomSybaseService,
            ICustomHelperMaestras argcustomHelperMaestrasService
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

            _configuration = argconfiguration;
            _customJwtService = argcustomJwtService;

            _currentServiceName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name; // set the current class name
            _currentJsonOptions = _customHelper.getJsonSerializeOptions();


            // ===== TRANSLATION INIT =====
            var controllerType = typeof(CmZonasBioController);
            string ls_current_project_name = controllerType.GetTypeInfo().Assembly.FullName ?? "here must be the project name"; // "MainProject"
            var assemblyObj = new AssemblyName(ls_current_project_name); // assemblyName.Name give us "MainProject"
            assemblyObj.Name = assemblyObj.Name ?? ls_current_project_name;


            // Default route for localizer is: MainProject.Resources.Controllers.TestController
            // In Program.cs is setted folder: "Controllers" so Then Base must be: MainProject.Controllers
            // Customized is: MainProject.Controllers.CustomDavivienda.Resources.TestController
            _localizer = factory.Create("CmZonasBio.Resources.CmZonasBioController", assemblyObj.Name);
            // ===== TRANSLATION END =====

            _customSybaseService = argcustomSybaseService;
            _customHelperMaestrasService = argcustomHelperMaestrasService;
        }


        // API publica por ahora
        public async Task<string> CmZonasBioMainGetFilteredAsync(string id_zona)
        {
            CmZonasBioFnResMainGetFilteredDev objFnResDev = new();
         

            HttpContext httpContext = _customHelper.GetCurrentHttpContext();
            var Request = httpContext.Request;
            string ls_error = "";
            bool lb_developer_mode = false;


            // set lb_developer_mode from Headers!!!               
            if (Request.Headers.TryGetValue("developerMode", out var developerModeHeader))
            {
                if ("true" == developerModeHeader.ToString()) lb_developer_mode = true;
            }


            // set developerModeLanguage from Headers!!!               
            if (Request.Headers.TryGetValue("developerModeLanguage", out var developerModeLanguageHeader))
            {
                if ("en" == developerModeLanguageHeader.ToString())
                {
                    CultureInfo cultureUI = new CultureInfo("en"); // English desired by Seniors Devs
                    Thread.CurrentThread.CurrentUICulture = cultureUI;
                }
            }


            // ===== GET DATABASE INFO INPUT
            if (lb_developer_mode)
            {
                Console.WriteLine("consultando db name...");
                DatabaseInfoInputJsonModel dbInfoJsnModelInput = new(); // JsonModel properties are strings or null
                ObjFnResLoadDbInfoInput dbInfoInputSearchResult = _customSybaseService.LoadDbInfo();

                // [0001] - error inesperado en la conexion
                if (dbInfoInputSearchResult.Success == false)
                {
                    httpContext.Response.StatusCode = 500; // Internal Server Error (500)
                    ls_error = this.KikesDeveloperErrorResponse("Error de conexión a la bd. " + dbInfoInputSearchResult.Message, _customHelper.LineNumber());

                    objFnResDev.Ksuccess = false;
                    objFnResDev.Kmessage = ls_error;

                    return this.CmZonasBioResponseFormatGetFiltered(objFnResDev, lb_developer_mode);
                }

                // RECORD FOUND - DB INFO
                if (dbInfoInputSearchResult.Success && dbInfoInputSearchResult.Data.Count() > 0)
                {
                    dbInfoJsnModelInput = dbInfoInputSearchResult.Data[0];
                }
                {
                    objFnResDev.Db_name_output = dbInfoJsnModelInput.Db_name ?? "querying db_name return null";
                    //objFnResDev.Db_name_input = objFnResDev.Db_name_input;
                }
            }
            // ===== END GET DATABASE INFO INPUT


            // ===== [CONTROL DE ERRORES TOKEN JWT] START
            if (true)
            {

                //[2664] No se ha recibido el token
                if (!httpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                {
                    httpContext.Response.StatusCode = 401; // Unauthorized (401)
                                                           //[2664] No Se Pudo Validar Token - Bearer no recibido                
                    ls_error = "Error: Authorization header is missing. - Bearer Token is NOT RECEIVED";
                    ls_error = this.KikesDeveloperErrorResponse(ls_error, _customHelper.LineNumber());

                    objFnResDev.KerrorCode = "2664"; // No Se Pudo Validar Token                    
                    objFnResDev.Ksuccess = false;
                    objFnResDev.Kmessage = ls_error;


                    return this.CmZonasBioResponseFormatGetFiltered(objFnResDev, lb_developer_mode);
                }


                //[2675] Estado De Token No Válido
                // ERROR GENERAL! on token_user can not be authenticated
                if (!_customJwtService.tokenHttpUserIsAuthenticated())
                {


                    bool unauthorizedAccess = httpContext.Items.ContainsKey("UnauthorizedAccess") &&
                                  (bool)httpContext.Items["UnauthorizedAccess"]!;

                    int li_middlewarejwt_http_scode = httpContext.Response.StatusCode;

                    //[2675] Estado De Token No Válido
                    // DETECTAMOS SI ES TOKEN EXPIRADO GRACIAS A LOS EVENTOS EN PROGRAM.CS
                    if (li_middlewarejwt_http_scode == (int)HttpStatusCode.Unauthorized
                        && unauthorizedAccess
                    )
                    {
                        ls_error = "Error: El Token ha caducado.";
                        ls_error = this.KikesDeveloperErrorResponse(ls_error, _customHelper.LineNumber());

                        objFnResDev.KerrorCode = "2675"; // Estado De Token No Válido
                        objFnResDev.Ksuccess = false;
                        objFnResDev.Kmessage = ls_error;

                        return this.CmZonasBioResponseFormatGetFiltered(objFnResDev, lb_developer_mode);
                    }

                    //[2675] No Se Pudo Validar Token
                    httpContext.Response.StatusCode = 401; // Unauthorized (401)

                    ls_error = _customHelper.Trans(_localizer, "error_token_incorrect") ?? "Http User Identity is NOT Authenticathed :( - Token is NOT VALID.";
                    ls_error = this.KikesDeveloperErrorResponse("[error_token_incorrect]: " + ls_error, _customHelper.LineNumber());

                    objFnResDev.KerrorCode = "2675"; // No Se Pudo Validar Token
                    objFnResDev.Ksuccess = false;
                    objFnResDev.Kmessage = ls_error;

                    return this.CmZonasBioResponseFormatGetFiltered(objFnResDev, lb_developer_mode);
                }


            }
            // ===== [CONTROL DE ERRORES TOKEN JWT] END





            // Query Database???
            if (true)
            {

                int li_limit = _customHelperMaestrasService.GetLimitAllowed(); // Sera enviada en la respuesta a postman
                List<CmZonasBioModelJsonCustom> rowList = new(); // Retornaré customizado por defecto
                int li_total_rows_filtered = 0;




                // BD CONSULTAR REGISTROS FILTRADOS
                CmZonasBioFnResObtainObjList ObtainFilteredObjListResult = await this.CmZonasBioObtainObjectListAzync();

                // ERROR ???
                if (ObtainFilteredObjListResult.Ksuccess == false)
                {
                    // Header response [500] Server error
                    httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                    ls_error = this.KikesDeveloperErrorResponse("Error al obtener filtered object list . " + ObtainFilteredObjListResult.Kmessage, _customHelper.LineNumber());

                    // Body response
                    objFnResDev.Ksuccess = false;
                    objFnResDev.Kmessage = ls_error;
                    objFnResDev.KerrorCode = "3306500"; // Error de conexion a la bd api                         
                    return this.CmZonasBioResponseFormatGetFiltered(objFnResDev, lb_developer_mode);
                }

                // RECORDS FOUND???
                if (ObtainFilteredObjListResult.Ksuccess && ObtainFilteredObjListResult.Kdata.Count >= 0)
                {
                    objFnResDev.Kfiltered_params = ObtainFilteredObjListResult.Kfiltered_params;

                    // CUSTOMIZE PROPERTIES IN RESULT???
                    if (true)
                    {
                        ObtainFilteredObjListResult.Kdata = ObtainFilteredObjListResult.Kdata.Where(x => x.Id_zona==id_zona).ToList();
                        int li_total_registros = ObtainFilteredObjListResult.Kdata.Count;
                        var user = new CmZonasBioModelJsonCustom();
                        
                        for (int i = 0; i < li_total_registros; i++)
                        {
                            objFnResDev.Kdata.Add(new CmZonasBioModelJsonCustom());
                            var currentModel = ObtainFilteredObjListResult.Kdata[i];

                            objFnResDev.Kdata[i].Id = currentModel.Id;
                            
                            objFnResDev.Kdata[i].Codigo_bodega = currentModel.Codigo_bodega?.Trim();
                            objFnResDev.Kdata[i].Descripcion = currentModel.Descripcion?.Trim();                                                        
                            objFnResDev.Kdata[i].Fec_registro = currentModel.Fec_registro;
                            objFnResDev.Kdata[i].Id_zona = currentModel.Id_zona?.Trim();
                            objFnResDev.Kdata[i].Estado = currentModel.Estado?.Trim();
                            
                        }                        
                    }
                }




                // BD CONSULTAR TOTAL ROWS FILTERED
                var ObtainTotalRowsFilWithOutLimitCliRs = await this.CmZonasBioObtainTotalRowsFilteredWithOutLimit();
                // ERROR ???
                if (ObtainTotalRowsFilWithOutLimitCliRs.Ksuccess == false)
                {
                    // Header response [500] Server error
                    httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                    ls_error = this.KikesDeveloperErrorResponse("Error al obtener tipos negocio totalized object list . " + ObtainTotalRowsFilWithOutLimitCliRs.Kmessage, _customHelper.LineNumber());

                    // Body response
                    objFnResDev.Ksuccess = false;
                    objFnResDev.Kmessage = ls_error;
                    objFnResDev.KerrorCode = "3306500"; // Error de conexion a la bd api                         
                    return this.CmZonasBioResponseFormatGetFiltered(objFnResDev, lb_developer_mode);
                }

                // RECORDS FOUND???
                if (ObtainTotalRowsFilWithOutLimitCliRs.Ksuccess && ObtainTotalRowsFilWithOutLimitCliRs.Kdata.Count >= 0)
                {
                    li_total_rows_filtered = ObtainTotalRowsFilWithOutLimitCliRs.Kdata[0];
                    objFnResDev.Filtered_total = li_total_rows_filtered;
                }

                objFnResDev.Response_limited = li_limit;
            }

            // all ok
            objFnResDev.KerrorCode = "0";
            objFnResDev.Kmessage = "Proceso finalizado con exito.";
            objFnResDev.Ksuccess = true;

            // Return expected response
            return this.CmZonasBioResponseFormatGetFiltered(objFnResDev, lb_developer_mode);
        }


        // API publica por ahora
        public async Task<string> CmZonasBioMainGetAllAsync(string ls_ctrl_json)
        {
            CmZonasBioFnResMainGetAllDev objFnResDev = new();

            var httpContext = _customHelper.GetCurrentHttpContext();
            var Request = httpContext.Request;
            string ls_error = "";
            bool lb_developer_mode = false;


            // read developerMode from Headers!!!               
            if (httpContext.Request.Headers.TryGetValue("developerMode", out var developerModeHeader))
            {
                if ("true" == developerModeHeader.ToString()) lb_developer_mode = true;
            }


            // read developerModeLanguage from Headers!!!               
            if (httpContext.Request.Headers.TryGetValue("developerModeLanguage", out var developerModeLanguageHeader))
            {

                if ("en" == developerModeLanguageHeader.ToString())
                {
                    CultureInfo cultureUI = new CultureInfo("en"); // English desired by Seniors Devs - Jajajaj.
                    Thread.CurrentThread.CurrentUICulture = cultureUI;
                }
            }


            // ===== GET DATABASE INFO INPUT
            if (lb_developer_mode)
            {
                Console.WriteLine("consultando db name...");
                DatabaseInfoInputJsonModel dbInfoJsnModelInput = new(); // JsonModel properties are strings or null
                ObjFnResLoadDbInfoInput dbInfoInputSearchResult = _customSybaseService.LoadDbInfo();

                // [0001] - error inesperado en la conexion
                if (dbInfoInputSearchResult.Success == false)
                {
                    httpContext.Response.StatusCode = 500; // Internal Server Error (500)
                    ls_error = this.KikesDeveloperErrorResponse("Error de conexión a la bd. " + dbInfoInputSearchResult.Message, _customHelper.LineNumber());

                    objFnResDev.Ksuccess = false;
                    objFnResDev.Kmessage = ls_error;

                    return this.CmZonasBioResponseFormatGetAll(objFnResDev, lb_developer_mode);
                }

                // RECORD FOUND - DB INFO
                if (dbInfoInputSearchResult.Success && dbInfoInputSearchResult.Data.Count() > 0)
                {
                    dbInfoJsnModelInput = dbInfoInputSearchResult.Data[0];
                }
                {
                    objFnResDev.Db_name_output = dbInfoJsnModelInput.Db_name ?? "querying db_name return null";
                    //objFnResDev.Db_name_input = objFnResDev.Db_name_input;
                }
            }
            // ===== END GET DATABASE INFO INPUT


            // ===== [CONTROL DE ERRORES TOKEN JWT] START
            if (true)
            {

                //[2664] No se ha recibido el token
                if (!httpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                {
                    httpContext.Response.StatusCode = 401; // Unauthorized (401)                    
                    ls_error = "Error: Authorization header is missing. - Bearer Token is NOT RECEIVED";
                    ls_error = KikesDeveloperErrorResponse(ls_error, _customHelper.LineNumber());

                    objFnResDev.KerrorCode = "2664"; // No Se Pudo Validar Token                    
                    objFnResDev.Ksuccess = false;
                    objFnResDev.Kmessage = ls_error;

                    return CmZonasBioResponseFormatGetAll(objFnResDev, lb_developer_mode);
                }


                //[2675] Estado De Token No Válido
                // ERROR GENERAL! on token_user can not be authenticated
                if (!_customJwtService.tokenHttpUserIsAuthenticated())
                {
                    bool unauthorizedAccess = httpContext.Items.ContainsKey("UnauthorizedAccess") &&
                                  (bool)httpContext.Items["UnauthorizedAccess"]!;

                    int li_middlewarejwt_http_scode = httpContext.Response.StatusCode;

                    //[2675] Estado De Token No Válido
                    // DETECTAMOS SI ES TOKEN EXPIRADO GRACIAS A LOS EVENTOS EN PROGRAM.CS
                    if (li_middlewarejwt_http_scode == (int)HttpStatusCode.Unauthorized
                        && unauthorizedAccess
                    )
                    {
                        ls_error = "Error: El Token ha caducado.";
                        ls_error = KikesDeveloperErrorResponse(ls_error, _customHelper.LineNumber());

                        objFnResDev.KerrorCode = "2675"; // Estado De Token No Válido
                        objFnResDev.Ksuccess = false;
                        objFnResDev.Kmessage = ls_error;

                        return CmZonasBioResponseFormatGetAll(objFnResDev, lb_developer_mode);
                    }

                    //[2675] No Se Pudo Validar Token
                    httpContext.Response.StatusCode = 401; // Unauthorized (401)

                    ls_error = _customHelper.Trans(_localizer, "error_token_incorrect") ?? "Http User Identity is NOT Authenticathed :( - Token is NOT VALID.";
                    ls_error = KikesDeveloperErrorResponse("[error_token_incorrect]: " + ls_error, _customHelper.LineNumber());

                    objFnResDev.KerrorCode = "2675"; // No Se Pudo Validar Token
                    objFnResDev.Ksuccess = false;
                    objFnResDev.Kmessage = ls_error;

                    return CmZonasBioResponseFormatGetAll(objFnResDev, lb_developer_mode);
                }


            }
            // ===== [CONTROL DE ERRORES TOKEN JWT] END


            // Query Database???
            if (true)
            {
                // Llamar a obtain
                // BD CONSULTAR REGISTROS FILTRADOS
                CmZonasBioFnResObtainAllObjectList obtainModelAllObjListResult = await this.CmZonasBioObtainAllObjectListAzync();

                // ERROR IN RESPONSE REMOTE DB ???
                if (obtainModelAllObjListResult.Ksuccess == false)
                {
                    // Header response [500] Server error
                    httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                    ls_error = KikesDeveloperErrorResponse("Error al obtener clientes all object list . " + obtainModelAllObjListResult.Kmessage, _customHelper.LineNumber());

                    // Body response
                    objFnResDev.Ksuccess = false;
                    objFnResDev.Kmessage = ls_error;
                    objFnResDev.KerrorCode = "3306500"; // Error de conexion a la bd api                         
                    return CmZonasBioResponseFormatGetAll(objFnResDev, lb_developer_mode);
                }


                // RECORDS FOUND???
                if (obtainModelAllObjListResult.Ksuccess && obtainModelAllObjListResult.Kdata.Count >= 0)
                {


                    // CUSTOMIZE PROPERTIES IN RESULT???
                    if (true)
                    {
                        int li_total_registros = obtainModelAllObjListResult.Kdata.Count;
                        for (int i = 0; i < li_total_registros; i++)
                        {
                            objFnResDev.Kdata.Add(new CmZonasBioModelAllJsonCustom());
                            var currentModel = obtainModelAllObjListResult.Kdata[i];

                            objFnResDev.Kdata[i].Id = currentModel.Id;
                            objFnResDev.Kdata[i].Codigo_bodega = currentModel.Codigo_bodega?.Trim();
                            objFnResDev.Kdata[i].Descripcion = currentModel.Descripcion?.Trim();
                            objFnResDev.Kdata[i].Fec_registro = currentModel.Fec_registro;
                            objFnResDev.Kdata[i].Id_zona = currentModel.Id_zona?.Trim();
                            objFnResDev.Kdata[i].Estado = currentModel.Estado?.Trim();
                        }
                    }
                }

            }

            // all ok
            objFnResDev.KerrorCode = "0";
            objFnResDev.Kmessage = "Proceso listar ALL finalizado con exito.";
            objFnResDev.Ksuccess = true;

            // Return expected response
            return CmZonasBioResponseFormatGetAll(objFnResDev, lb_developer_mode); // on fail json received, activate developerMode
        }


        // Define a custom helper
        public async Task<CmZonasBioFnResObtainObjList> CmZonasBioObtainObjectListAzync()
        {
            // INICIALIZAMOS VALORES DEFECTO DE RESPUESTA
            CmZonasBioFnResObtainObjList objFnResDev = new();

            var httpContext = _customHelper.GetCurrentHttpContext();
            string ls_error = "";
            string ls_jsonresponse_retrieve_filtered = "{}";

            try
            {
                ls_jsonresponse_retrieve_filtered = await this.CmZonasBioRetrieveDbAllFilteredAzync(); // get rows filtered and limited                
            }
            catch (JsonException e)
            {
                // Header response [500] Server error
                httpContext.Response.StatusCode = 500;
                ls_error = _customHelper.Trans(_localizer, "error_property_json_received") ?? "Error in received object JSON from custom service db: ";
                ls_error += e.Message;
                ls_error = KikesDeveloperErrorResponse(ls_error, _customHelper.LineNumber());

                // Body response
                objFnResDev.KerrorCode = "3306409"; // Error parseando respuesta json de bd consultar
                objFnResDev.Ksuccess = false;
                objFnResDev.Kmessage = ls_error;
                return objFnResDev;
            }


            // ERRORRR PARSING JSON???
            CmZonasBioResAjaxFilteredAzync dbSearchResult = new(); // Force to be an object
            CmZonasBioResAjaxFilteredAzync? dbSearchResultOrNull = null; // necesary variable to try catch to cast postman json object            
            try
            {
                dbSearchResultOrNull = JsonSerializer.Deserialize<CmZonasBioResAjaxFilteredAzync>(ls_jsonresponse_retrieve_filtered, _currentJsonOptions);
            }
            catch (JsonException e)
            {
                // Header response [500] Server error
                httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                ls_error = _customHelper.Trans(_localizer, "error_property_json_received") ?? "Error in received object JSON: ";
                ls_error += e.Message;
                ls_error = KikesDeveloperErrorResponse(ls_error, _customHelper.LineNumber());

                // Body response
                objFnResDev.KerrorCode = "3306409"; // Error parseando respuesta json de bd consultar
                objFnResDev.Ksuccess = false;
                objFnResDev.Kmessage = ls_error;
                return objFnResDev;
            }
            dbSearchResult = dbSearchResultOrNull ?? new CmZonasBioResAjaxFilteredAzync();


            // ERROR IN RESPONSE REMOTE DB ???
            if (dbSearchResult.Ksuccess == false)
            {
                // Header response [500] Server error
                httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                ls_error = KikesDeveloperErrorResponse("Error de conexión a la bd. " + dbSearchResult.Kmessage, _customHelper.LineNumber());

                // Body response
                objFnResDev.Ksuccess = false;
                objFnResDev.Kmessage = ls_error;
                objFnResDev.KerrorCode = "3306500"; // Error de conexion a la bd api                                         
                return objFnResDev;
            }


            // RECORDS FOUND???
            if (dbSearchResult.Kdata.Count >= 0)
            {
                objFnResDev.KerrorCode = "0"; // Exito!!
                objFnResDev.Ksuccess = true;
                objFnResDev.Kmessage = "Proceso finalizado con exito.";
                objFnResDev.Kfiltered_params = dbSearchResult.Kfiltered_params;

                objFnResDev.Kdata = dbSearchResult.Kdata;
            }
            return objFnResDev;
        }


        // Define a custom helper
        public async Task<string> CmZonasBioRetrieveDbAllFilteredAzync()
        {
            CmZonasBioFnResDefaultFilterParams aParams = this.CmZonasBioDefaultFilterParams();

            // SOME VARS TO get full response from remote api            
            Dictionary<string, object> la_params_remote = new Dictionary<string, object>
            {                
                // remote required
                { "li_limit", aParams.Limit },
                { "ls_fec_registro_min", aParams.Fec_registro_min },
                { "li_after_id", aParams.After_id }
            };
            string ls_jsonres = await _customSybaseService.CmZonasBioQueryFilteredAzync(la_params_remote); // Esperamos una json de respuesta            
            return ls_jsonres;
        }

        //jjjjjjjjjjjjjjjjjj

        public async Task<string> CmZonasBioRetrieveDbFilteredByCodigoZonaAsync(string codigo_zona, object id_zona)
        {
            CmZonasBioFnResDefaultFilterParams aParams = this.CmZonasBioDefaultFilterParams();

            // Armamos los parámetros para la consulta remota
            Dictionary<string, object> la_params_remote = new Dictionary<string, object>
    {
        { "li_limit", aParams.Limit },
        { "ls_fec_registro_min", aParams.Fec_registro_min },
        { "li_after_id", aParams.After_id },
        { "ls_zona",id_zona } // Filtro adicional por código de zona
    };

            // Ejecutamos la consulta remota
            string ls_jsonres = await _customSybaseService.CmZonasBioQueryFilteredAzync(la_params_remote);

            return ls_jsonres;
        }





        // Define a custom helper
        public CmZonasBioFnResDefaultFilterParams CmZonasBioDefaultFilterParams()
        {
            CmZonasBioFnResDefaultFilterParams aParams = new();

            var httpContext = _customHelper.GetCurrentHttpContext();
            var Request = httpContext.Request;

            int li_limit = _customHelperMaestrasService.GetLimitAllowed(); // example 500 - defined in settingsjson
            string ls_fec_registro_min = _configuration.GetValue<string>("CustomApp:Li_fec_registro_min"); // formato year-month-day 1800-12-31
            int li_after_id = _configuration.GetValue<int>("CustomApp:Li_after_id_default");


            // StringValues cuando es vacío no da error, y al convertirlo al ToString() resulta en cadena vacía, no 'null'
            string ls_fec_registro_min_or_empty = Request.Query["fec_registro_min"].ToString(); // StringValues
            ls_fec_registro_min = ls_fec_registro_min_or_empty.Length > 0 ? ls_fec_registro_min_or_empty : ls_fec_registro_min;

            string ls_after_id_or_empty = Request.Query["after_id"].ToString(); // StringValues
            ObjFnResCustomConverToInt32 objRs = _customHelper.CustomConvertToInt32("after_id", ls_after_id_or_empty);
            if (objRs.Status == true)
            {
                if (objRs.Data is not null && objRs.Data.Count() > 0)
                {
                    li_after_id = objRs.Data[0]; // Nuevo limite recibido
                }
            }

            // HACK-PAGINATION: Pide un registro adicional y lo removerás como ultimo elemento to response
            if (li_limit > 0)
            {
                li_limit = li_limit + 1;
            }

            if (li_after_id < 0)
            {
                li_after_id = 0;
            }

            aParams.Limit = li_limit;
            aParams.Fec_registro_min = ls_fec_registro_min;
            aParams.After_id = li_after_id;
            return aParams;
        }


        // Define a custom helper
        public async Task<CmZonasBioFnResObtainTotalRowsFWOL> CmZonasBioObtainTotalRowsFilteredWithOutLimit()
        {
            // INICIALIZAMOS VALORES DEFECTO DE RESPUESTA
            CmZonasBioFnResObtainTotalRowsFWOL objFnResDev = new();

            var httpContext = _customHelper.GetCurrentHttpContext();
            string ls_error = "";
            string ls_database_response = "{}";

            try
            {
                ls_database_response = await this.CmZonasBioRetrieveDbAllFilteredTotalRowsAzync();
            }
            catch (JsonException e)
            {
                // Header response [500] Server error
                httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                ls_error = _customHelper.Trans(_localizer, "error_property_json_received") ?? "Error in received object JSON from custom service db: ";
                ls_error += e.Message;
                ls_error = KikesDeveloperErrorResponse(ls_error, _customHelper.LineNumber());

                // Body response
                objFnResDev.KerrorCode = "3306409"; // Error parseando respuesta json de bd consultar
                objFnResDev.Ksuccess = false;
                objFnResDev.Kmessage = ls_error;
                return objFnResDev;
            }


            // ERRORRR PARSING JSON???
            FnResQueryAAAModelDefaultFnResFTRowsAzync dbClientesSearchResult = new(); // Force to be an object
            FnResQueryAAAModelDefaultFnResFTRowsAzync? dbClientesSearchResultOrNull = null; // necesary variable to try catch to cast postman json object            
            try
            {
                dbClientesSearchResultOrNull = JsonSerializer.Deserialize<FnResQueryAAAModelDefaultFnResFTRowsAzync>(ls_database_response, _currentJsonOptions);
            }
            catch (JsonException e)
            {
                // Header response [500] Server error
                httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                ls_error = _customHelper.Trans(_localizer, "error_property_json_received") ?? "Error in received object JSON: ";
                ls_error += e.Message;
                ls_error = KikesDeveloperErrorResponse(ls_error, _customHelper.LineNumber());

                // Body response
                objFnResDev.KerrorCode = "3306409"; // Error parseando respuesta json de bd consultar
                objFnResDev.Ksuccess = false;
                objFnResDev.Kmessage = ls_error;
                return objFnResDev;
            }
            dbClientesSearchResult = dbClientesSearchResultOrNull ?? new FnResQueryAAAModelDefaultFnResFTRowsAzync();


            // ERROR IN RESPONSE REMOTE DB ???
            if (dbClientesSearchResult.Ksuccess == false)
            {
                // Header response [500] Server error
                httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                ls_error = KikesDeveloperErrorResponse("Error de conexión a la bd. " + dbClientesSearchResult.Kmessage, _customHelper.LineNumber());

                // Body response
                objFnResDev.Ksuccess = false;
                objFnResDev.Kmessage = ls_error;
                objFnResDev.KerrorCode = "3306500"; // Error de conexion a la bd api                                         
                return objFnResDev;
            }


            // RECORDS FOUND???
            if (dbClientesSearchResult.Kdata.Count >= 0)
            {
                objFnResDev.KerrorCode = "0"; // Exito!!
                objFnResDev.Ksuccess = true;
                objFnResDev.Kmessage = "Proceso listar clientes finalizado con exito.";
                objFnResDev.Kdata.Add(dbClientesSearchResult.Kdata[0]);
            }

            return objFnResDev;
        }


        //// Define a custom helper
        public async Task<string> CmZonasBioRetrieveDbAllFilteredTotalRowsAzync()
        {
            CmZonasBioFnResDefaultFilterParams aParams = this.CmZonasBioDefaultFilterParams();

            Dictionary<string, object> la_params_remote = new Dictionary<string, object>
            {                
                // remote required
                { "li_limit", aParams.Limit },
                { "ls_fec_registro_min", aParams.Fec_registro_min },
                { "li_after_id", aParams.After_id }
            };

            string ls_jsonres = await _customSybaseService.CmZonasBioQueryFilteredTotalRowsAzync(la_params_remote); // Esperamos una json de respuesta
            return ls_jsonres;
        }


        // Define a custom helper
        public string KikesDeveloperErrorResponse(string ls_error, int li_line_number)
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
        // Se crea este response algo diferente, para poder responder de forma generica por cada maestra.
        private string CmZonasBioResponseFormatGetFiltered(CmZonasBioFnResMainGetFilteredDev stdFnResDev, bool developerMode = false)
        {
            var httpContext = _customHelper.GetCurrentHttpContext();
            var Request = httpContext.Request;


            // wrap calculos adicionales
            if (true)
            {
                Dictionary<string, string> statusCodeList = this.GetStatusCodeList();
                string ls_status_desc_aux = stdFnResDev.Kmessage;
                string ls_status_code = stdFnResDev.KerrorCode.ToString();
                string ls_status_desc = statusCodeList[ls_status_code] + ": " + ls_status_desc_aux;

                string ls_text = ""; // Variable auxiliar para hacer transformaciones con texto o enteros

                ls_text = ls_status_code;
                ls_text = ls_text.Trim();
                int errorCodigoInt = -1;
                var objRs = _customHelper.CustomConvertToInt32("Error_codigo", ls_text);
                if (objRs.Status == true && objRs.Data?.Count() > 0)
                {
                    errorCodigoInt = objRs.Data[0];
                }
                stdFnResDev.KerrorCode = errorCodigoInt.ToString();

                ls_text = ls_status_desc;
                ls_text = ls_text.Trim();
                //ls_text = _customHelper.Substring(ls_text, 0, 100); // si proveedor requiere recortar respuesta a first 100 characters
                stdFnResDev.Kmessage = ls_text;


                // HACK-PAGINATION: si rows filtered are greather than limit, remove last before response, and paginate!
                bool mustPaginate = false;
                if (stdFnResDev.Kdata.Count > stdFnResDev.Response_limited)
                {
                    mustPaginate = true;
                    stdFnResDev.Kdata.RemoveAt(stdFnResDev.Kdata.Count - 1); // eliminamos del response el ultimo registro                                
                    stdFnResDev.Filtered_has_more = mustPaginate;
                }
                stdFnResDev.Response_count = stdFnResDev.Kdata.Count;

                // First last page
                int response_first_id = 0;
                int response_last_id = 0;
                if (stdFnResDev.Kdata.Count > 0)
                {
                    response_first_id = stdFnResDev.Kdata[0].Id ?? 0;
                    response_last_id = stdFnResDev.Kdata[stdFnResDev.Kdata.Count - 1].Id ?? 0;

                    stdFnResDev.Response_first_id = response_first_id;
                    stdFnResDev.Response_last_id = response_last_id;
                }


                // url pagination                                        
                string ls_domain = _configuration.GetValue<string>("CustomApp:Ls_response_domain"); // Manually definition: http://localhost:5255            
                string fullUrl = ls_domain + $"{Request.Path}{Request.QueryString}";

                if (mustPaginate)
                {
                    stdFnResDev.Next_page_url = _customHelper.AddParameterToRequestQuery(fullUrl, "after_id", stdFnResDev.Response_last_id.ToString());
                }
            }


            if (developerMode) return JsonSerializer.Serialize(stdFnResDev, _currentJsonOptions);
            var objFnResPro = new
            {
                Ksuccess = stdFnResDev.Ksuccess,
                KerrorCode = stdFnResDev.KerrorCode,
                Kmessage = stdFnResDev.Kmessage,
                Kdata = stdFnResDev.Kdata,

                Filtered_total = stdFnResDev.Filtered_total,
                Response_count = stdFnResDev.Response_count,
                Response_limited = stdFnResDev.Response_limited,
                Filtered_has_more = stdFnResDev.Filtered_has_more,
                Next_page_url = stdFnResDev.Next_page_url,
                Response_first_id = stdFnResDev.Response_first_id,
                Response_last_id = stdFnResDev.Response_last_id
            };
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
        private string CmZonasBioResponseFormatGetAll(CmZonasBioFnResMainGetAllDev stdFnResDev, bool developerMode = false)
        {

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
            var objFnResPro = new
            {
                Ksuccess = stdFnResDev.Ksuccess,
                KerrorCode = stdFnResDev.KerrorCode,
                Kmessage = stdFnResDev.Kmessage,
                Kdata = stdFnResDev.Kdata
            };
            return JsonSerializer.Serialize(objFnResPro, _currentJsonOptions);
        }


        // Define a custom helper
        public async Task<CmZonasBioFnResObtainAllObjectList> CmZonasBioObtainAllObjectListAzync()
        {
            // INICIALIZAMOS VALORES DEFECTO DE RESPUESTA
            CmZonasBioFnResObtainAllObjectList objFnResDev = new();

            var httpContext = _customHelper.GetCurrentHttpContext();
            string ls_error = "";
            string ls_jsonresponse_bdtable_all = "{}";

            try
            {
                ls_jsonresponse_bdtable_all = await this.CmZonasBioRetrieveDbAllAzync(); // get all rows                
            }
            catch (JsonException e)
            {
                // Header response [500] Server error
                httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                ls_error = _customHelper.Trans(_localizer, "error_property_json_received") ?? "Error in received object JSON from custom service db: ";
                ls_error += e.Message;
                ls_error = KikesDeveloperErrorResponse(ls_error, _customHelper.LineNumber());

                // Body response
                objFnResDev.KerrorCode = "3306409"; // Error parseando respuesta json de bd consultar
                objFnResDev.Ksuccess = false;
                objFnResDev.Kmessage = ls_error;
                return objFnResDev;
            }


            // ERRORRR PARSING JSON???
            CmZonasBioResAjaxAllAzync dbSearchResult = new(); // Force to be an object
            CmZonasBioResAjaxAllAzync? dbSearchResultOrNull = null; // necesary variable to try catch to cast postman json object            
            try
            {
                dbSearchResultOrNull = JsonSerializer.Deserialize<CmZonasBioResAjaxAllAzync>(ls_jsonresponse_bdtable_all, _currentJsonOptions);
            }
            catch (JsonException e)
            {
                // Header response [500] Server error
                httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                ls_error = _customHelper.Trans(_localizer, "error_property_json_received") ?? "Error in received object JSON: ";
                ls_error += e.Message;
                ls_error = KikesDeveloperErrorResponse(ls_error, _customHelper.LineNumber());

                // Body response
                objFnResDev.KerrorCode = "3306409"; // Error parseando respuesta json de bd consultar
                objFnResDev.Ksuccess = false;
                objFnResDev.Kmessage = ls_error;
                return objFnResDev;
            }
            dbSearchResult = dbSearchResultOrNull ?? new CmZonasBioResAjaxAllAzync();


            // ERROR IN RESPONSE REMOTE DB ???
            if (dbSearchResult.Ksuccess == false)
            {
                // Header response [500] Server error
                httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                ls_error = KikesDeveloperErrorResponse("Error de conexión a la bd. " + dbSearchResult.Kmessage, _customHelper.LineNumber());

                // Body response
                objFnResDev.Ksuccess = false;
                objFnResDev.Kmessage = ls_error;
                objFnResDev.KerrorCode = "3306500"; // Error de conexion a la bd api                                         
                return objFnResDev;
            }


            // RECORDS FOUND???
            if (dbSearchResult.Kdata.Count >= 0)
            {
                objFnResDev.KerrorCode = "0"; // Exito!!
                objFnResDev.Ksuccess = true;
                objFnResDev.Kmessage = "Proceso listar finalizado con exito.";
                objFnResDev.Kdata = dbSearchResult.Kdata;
            }

            // CUSTOMIZE PROPERTIES IN OBTAIN RESULT???
            //if (true)
            //{
            //    for (int i = 0; i < objFnResDev.Kdata.Count; i++)
            //    {
            //        var cliente = objFnResDev.Kdata[i];
            //        objFnResDev.Kdata[i].Id_cliente = cliente.Id_cliente?.Trim();
            //    }
            //}

            return objFnResDev;
        }


        // Define a custom helper - para mantener el nombre de la carpeta fnres
        public async Task<string> CmZonasBioRetrieveDbAllAzync()
        {
            string ls_jsonres = await _customSybaseService.CmZonasBioQueryAllAzync();
            return ls_jsonres;
        }


        // end class
    }
}
