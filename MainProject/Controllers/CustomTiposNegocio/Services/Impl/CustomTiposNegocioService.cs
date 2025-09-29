using MainProject.Controllers.CustomClientes.Models.GetClientesAll.MainGetClientesAllAzync.Json;
using MainProject.Controllers.CustomClientes.Models.GetClientesAll.ObtainClientesAllObjectListAzync.FnRes;
using MainProject.Controllers.CustomHelperMaestras.Services;
using MainProject.Controllers.CustomJwt.Services;
using MainProject.Controllers.CustomTiposNegocio.Controllers;
using MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioAll.MainGetTiposNegocioAllAsync.FnRes;
using MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioAll.MainGetTiposNegocioAllAsync.Json;
using MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioAll.ObtainTiposNegocioAllObjectListAzync.FnRes;
using MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioAll.ObtainTiposNegocioAllObjectListAzync.JsonParsed;
using MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioAll.RetrieveDbTiposNegocioAllAzync;
using MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioFiltered.MainGetTiposNegocioFiltered.FnRes;
using MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioFiltered.MainGetTiposNegocioFiltered.Json;
using MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioFiltered.ObtainTiposNegocioObjectListAzync.FnRes;
using MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioFiltered.TiposNegocioDefaultFilterParams;
using MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioFiltered.TotalRowsFilteredWithOutLimitTiposNegocio.FnRes;
using MainProject.Services.CustomDatabaseInput;
using MainProject.Services.CustomHelper;
using MainProject.Services.CustomHelper.Models.FnRes;
using MainProject.Services.CustomSybase.Models.QueryTiposNegocioFilteredAzync.FnRes;
using MainProject.Services.CustomSybase.Models.QueryTiposNegocioFilteredTotalRowsAzync.FnRes;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace MainProject.Controllers.CustomTiposNegocio.Services.Impl
{
    public class CustomTiposNegocioService : ICustomTiposNegocio
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
        public CustomTiposNegocioService(
            ICustomHelper argcustomHelperService,
            IConfiguration argconfiguration,
            IHttpContextAccessor arghttpContextAccessor,
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
            var controllerType = typeof(CustomTiposNegocioController);
            string ls_current_project_name = controllerType.GetTypeInfo().Assembly.FullName ?? "here must be the project name"; // "MainProject"
            var assemblyObj = new AssemblyName(ls_current_project_name); // assemblyName.Name give us "MainProject"
            assemblyObj.Name = assemblyObj.Name ?? ls_current_project_name;


            // Default route for localizer is: MainProject.Resources.Controllers.TestController
            // In Program.cs is setted folder: "Controllers" so Then Base must be: MainProject.Controllers
            // Customized is: MainProject.Controllers.CustomDavivienda.Resources.TestController
            _localizer = factory.Create("CustomTiposNegocio.Resources.CustomTiposNegocioController", assemblyObj.Name);
            // ===== TRANSLATION END =====

            _customSybaseService = argcustomSybaseService;
            _customHelperMaestrasService = argcustomHelperMaestrasService;
        }




        // API publica por ahora
        public async Task<string> MainGetTiposNegocioFilteredAsync(string ls_ctrl_json)
        {            
            ObjFnResMainGetTiposNegocioFilteredDev objFnResDev = new();            
            
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


                    return this.ResponseFormatGetTiposNegocioFiltered(objFnResDev, lb_developer_mode);
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

                        return this.ResponseFormatGetTiposNegocioFiltered(objFnResDev, lb_developer_mode);
                    }

                    //[2675] No Se Pudo Validar Token
                    httpContext.Response.StatusCode = 401; // Unauthorized (401)

                    ls_error = _customHelper.Trans(_localizer, "error_token_incorrect") ?? "Http User Identity is NOT Authenticathed :( - Token is NOT VALID.";
                    ls_error = this.KikesDeveloperErrorResponse("[error_token_incorrect]: " + ls_error, _customHelper.LineNumber());

                    objFnResDev.KerrorCode = "2675"; // No Se Pudo Validar Token
                    objFnResDev.Ksuccess = false;
                    objFnResDev.Kmessage = ls_error;

                    return this.ResponseFormatGetTiposNegocioFiltered(objFnResDev, lb_developer_mode);
                }


            }
            // ===== [CONTROL DE ERRORES TOKEN JWT] END





            // Query Database???
            if (true)
            {

                int li_limit = _customHelperMaestrasService.GetLimitAllowed(); // Sera enviada en la respuesta a postman
                List<TipoNegocioMaestraCustomized> rowList = new(); // Retornaré customizado por defecto
                int li_total_rows_filtered = 0;




                // BD CONSULTAR REGISTROS FILTRADOS
                ObjFnResObtainTiposNegocioObjList ObtainFilteredObjListResult = await this.ObtainTiposNegocioObjectListAzync();
                
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
                    return this.ResponseFormatGetTiposNegocioFiltered(objFnResDev, lb_developer_mode);
                }

                // RECORDS FOUND???
                if (ObtainFilteredObjListResult.Ksuccess && ObtainFilteredObjListResult.Kdata.Count >= 0)
                {                    

                    // CUSTOMIZE PROPERTIES IN RESULT???
                    if (true)
                    {
                        int li_total_registros = ObtainFilteredObjListResult.Kdata.Count;

                        for (int i = 0; i < li_total_registros; i++)
                        {
                            objFnResDev.Kdata.Add(new TipoNegocioMaestraCustomized());
                            var currentModel = ObtainFilteredObjListResult.Kdata[i];

                             objFnResDev.Kdata[i].Id = (int) currentModel.Id;
                            objFnResDev.Kdata[i].Id_negocio = currentModel.Id_negocio?.Trim();
                            objFnResDev.Kdata[i].Nom_negocio = currentModel.Nom_negocio?.Trim();
                            objFnResDev.Kdata[i].Nota = currentModel.Nota?.Trim();                            
                            objFnResDev.Kdata[i].Fec_registro = currentModel.Fec_registro;
                            objFnResDev.Kdata[i].Fecha = currentModel.Fecha;                                                        
                        }                        
                    }                     
                }

                


                // BD CONSULTAR TOTAL ROWS FILTERED
                var ObtainTotalRowsFilWithOutLimitCliRs = await this.ObtainTotalRowsFilteredWithOutLimitTiposNegocio();
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
                    return this.ResponseFormatGetTiposNegocioFiltered(objFnResDev, lb_developer_mode);
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
            return this.ResponseFormatGetTiposNegocioFiltered(objFnResDev, lb_developer_mode);
        }


        // Define a custom helper
        public async Task<ObjFnResObtainTiposNegocioObjList> ObtainTiposNegocioObjectListAzync()
        {
            // INICIALIZAMOS VALORES DEFECTO DE RESPUESTA
            ObjFnResObtainTiposNegocioObjList objFnResDev = new();

            var httpContext = _customHelper.GetCurrentHttpContext();
            string ls_error = "";
            string ls_jsonresponse_retrieve_filtered = "{}";

            try
            {
                ls_jsonresponse_retrieve_filtered = await this.RetrieveDbAllTiposNegocioFilteredAzync(); // get rows filtered and limited                
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
            FnResQueryTiposNegocioFilteredAzync dbSearchResult = new FnResQueryTiposNegocioFilteredAzync(); // Force to be an object
            FnResQueryTiposNegocioFilteredAzync? dbSearchResultOrNull = null; // necesary variable to try catch to cast postman json object            
            try
            {
                dbSearchResultOrNull = JsonSerializer.Deserialize<FnResQueryTiposNegocioFilteredAzync>(ls_jsonresponse_retrieve_filtered, _currentJsonOptions);
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
            dbSearchResult = dbSearchResultOrNull ?? new FnResQueryTiposNegocioFilteredAzync();


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
                objFnResDev.Kdata = dbSearchResult.Kdata;
            }            
            return objFnResDev;
        }


        // Define a custom helper
        public async Task<string> RetrieveDbAllTiposNegocioFilteredAzync()
        {
            FnResTiposNegocioDefaultFilterParams aParams = this.TiposNegocioDefaultFilterParams();

            // SOME VARS TO get full response from remote api            
            Dictionary<string, object> la_params_remote = new Dictionary<string, object>
            {                
                // remote required
                { "li_limit", aParams.Limit },
                { "ls_fec_registro_min", aParams.Fec_registro_min },                
                { "li_after_id", aParams.After_id }
            };
            string ls_jsonres = await _customSybaseService.QueryTiposNegocioFilteredAzync(la_params_remote); // Esperamos una json de respuesta
            return ls_jsonres;
        }


        // Define a custom helper
        public FnResTiposNegocioDefaultFilterParams TiposNegocioDefaultFilterParams()
        {            
            FnResTiposNegocioDefaultFilterParams aParams = new();

            var httpContext = _customHelper.GetCurrentHttpContext();
            var Request = httpContext.Request;

            int li_limit = _customHelperMaestrasService.GetLimitAllowed(); // example 500            
            string ls_fec_registro_min = "1900-12-31";
            int li_after_id = -1;


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
        public async Task<FnResObtainTotalRowsFilteredWithOutLimitTiposNegocio> ObtainTotalRowsFilteredWithOutLimitTiposNegocio()
        {
            // INICIALIZAMOS VALORES DEFECTO DE RESPUESTA
            FnResObtainTotalRowsFilteredWithOutLimitTiposNegocio objFnResDev = new();

            var httpContext = _customHelper.GetCurrentHttpContext();
            string ls_error = "";
            string ls_database_response = "{}";

            try
            {
                ls_database_response = await this.RetrieveDbAllTiposNegocioFilteredTotalRowsAzync();
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
            FnResQueryTiposNegocioFilteredTotalRowsAzync dbClientesSearchResult = new FnResQueryTiposNegocioFilteredTotalRowsAzync(); // Force to be an object
            FnResQueryTiposNegocioFilteredTotalRowsAzync? dbClientesSearchResultOrNull = null; // necesary variable to try catch to cast postman json object            
            try
            {
                dbClientesSearchResultOrNull = JsonSerializer.Deserialize<FnResQueryTiposNegocioFilteredTotalRowsAzync>(ls_database_response, _currentJsonOptions);
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
            dbClientesSearchResult = dbClientesSearchResultOrNull ?? new FnResQueryTiposNegocioFilteredTotalRowsAzync();


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


        // Define a custom helper
        public async Task<string> RetrieveDbAllTiposNegocioFilteredTotalRowsAzync()
        {
            FnResTiposNegocioDefaultFilterParams aParams = this.TiposNegocioDefaultFilterParams();
            
            Dictionary<string, object> la_params_remote = new Dictionary<string, object>
            {                
                // remote required
                { "li_limit", aParams.Limit },
                { "ls_fec_registro_min", aParams.Fec_registro_min },                
                { "li_after_id", aParams.After_id }
            };

            string ls_jsonres = await _customSybaseService.QueryTiposNegocioFilteredTotalRowsAzync(la_params_remote); // Esperamos una json de respuesta
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
        private string ResponseFormatGetTiposNegocioFiltered(ObjFnResMainGetTiposNegocioFilteredDev stdFnResDev, bool developerMode = false)
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


        // API publica por ahora
        public async Task<string> MainGetTiposNegocioAllAsync(string ls_ctrl_json)
        {
            ObjFnResMainGetTiposNegocioAllDev objFnResDev = new();
         
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

                    return ResponseFormatGetTiposNegocioAll(objFnResDev, lb_developer_mode);
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

                        return ResponseFormatGetTiposNegocioAll(objFnResDev, lb_developer_mode);
                    }

                    //[2675] No Se Pudo Validar Token
                    httpContext.Response.StatusCode = 401; // Unauthorized (401)

                    ls_error = _customHelper.Trans(_localizer, "error_token_incorrect") ?? "Http User Identity is NOT Authenticathed :( - Token is NOT VALID.";
                    ls_error = KikesDeveloperErrorResponse("[error_token_incorrect]: " + ls_error, _customHelper.LineNumber());

                    objFnResDev.KerrorCode = "2675"; // No Se Pudo Validar Token
                    objFnResDev.Ksuccess = false;
                    objFnResDev.Kmessage = ls_error;

                    return ResponseFormatGetTiposNegocioAll(objFnResDev, lb_developer_mode);
                }


            }
            // ===== [CONTROL DE ERRORES TOKEN JWT] END


            // Query Database???
            if (true)
            {
                // Llamar a obtain
                // BD CONSULTAR REGISTROS FILTRADOS
                FnResObtainTiposNegocioAllObjectList obtainModelAllObjListResult = await this.ObtainTiposNegocioAllObjectListAzync();

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
                    return ResponseFormatGetTiposNegocioAll(objFnResDev, lb_developer_mode);
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
                            objFnResDev.Kdata.Add(new TipoNegocioAllMaestraCustomized());
                            var currentModel = obtainModelAllObjListResult.Kdata[i];

                            objFnResDev.Kdata[i].Id = currentModel.Id;
                            objFnResDev.Kdata[i].Id_negocio = currentModel.Id_negocio?.Trim();
                            objFnResDev.Kdata[i].Nom_negocio = currentModel.Nom_negocio?.Trim();
                            objFnResDev.Kdata[i].Nota = currentModel.Nota?.Trim();
                            objFnResDev.Kdata[i].Fec_registro = currentModel.Fec_registro;
                            objFnResDev.Kdata[i].Fecha = currentModel.Fecha;
                        }
                    }
                }

            }

            // all ok
            objFnResDev.KerrorCode = "0";
            objFnResDev.Kmessage = "Proceso listar Tipos negocio ALL finalizado con exito.";
            objFnResDev.Ksuccess = true;

            // Return expected response
            return ResponseFormatGetTiposNegocioAll(objFnResDev, lb_developer_mode); // on fail json received, activate developerMode
        }


        // Define a custom helper
        private string ResponseFormatGetTiposNegocioAll(ObjFnResMainGetTiposNegocioAllDev stdFnResDev, bool developerMode = false)
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
        public async Task<FnResObtainTiposNegocioAllObjectList> ObtainTiposNegocioAllObjectListAzync()
        {
            // INICIALIZAMOS VALORES DEFECTO DE RESPUESTA
            FnResObtainTiposNegocioAllObjectList objFnResDev = new();

            var httpContext = _customHelper.GetCurrentHttpContext();
            string ls_error = "";
            string ls_jsonresponse_bdtable_all = "{}";

            try
            {
                ls_jsonresponse_bdtable_all = await this.RetrieveDbTiposNegocioAllAzync(); // get all rows                
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
            FnResRetrieveDbTiposNegocioAllAzync dbClientesSearchResult = new(); // Force to be an object
            FnResRetrieveDbTiposNegocioAllAzync? dbClientesSearchResultOrNull = null; // necesary variable to try catch to cast postman json object            
            try
            {
                dbClientesSearchResultOrNull = JsonSerializer.Deserialize<FnResRetrieveDbTiposNegocioAllAzync>(ls_jsonresponse_bdtable_all, _currentJsonOptions);
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
            dbClientesSearchResult = dbClientesSearchResultOrNull ?? new FnResRetrieveDbTiposNegocioAllAzync();


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
                objFnResDev.Kdata = dbClientesSearchResult.Kdata;
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
        public async Task<string> RetrieveDbTiposNegocioAllAzync()
        {
            string ls_jsonres = await _customSybaseService.QueryTiposNegocioAllAzync();
            return ls_jsonres;
        }


        // end class
    }
}
