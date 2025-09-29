using MainProject.Controllers.CustomClientes.Controllers;
using MainProject.Controllers.CustomClientes.Models.GetClientesAll.MainGetClientesAllAzync.FnRes;
using MainProject.Controllers.CustomClientes.Models.GetClientesAll.MainGetClientesAllAzync.Json;
using MainProject.Controllers.CustomClientes.Models.GetClientesAll.ObtainClientesAllObjectListAzync.FnRes;
using MainProject.Controllers.CustomClientes.Models.GetClientesAll.RetrieveDbClientesAllAzync.FnRes;
using MainProject.Controllers.CustomClientes.Models.GetClientesFiltered.GetDefaultFilterParams.FnRes;
using MainProject.Controllers.CustomClientes.Models.GetClientesFiltered.MainGetClientesFiltered.FnRes;
using MainProject.Controllers.CustomClientes.Models.GetClientesFiltered.ObtainClientesObjectListAzync.FnRes;
using MainProject.Controllers.CustomClientes.Models.GetClientesFiltered.ObtainTotalRowsFilteredWithOutLimitClientes.FnRes;
using MainProject.Controllers.CustomClientes.Models.GetClientesFiltered.RetrieveDbAllClientesFilteredAzync.FnRes;
using MainProject.Controllers.CustomClientes.Models.GetClientesFiltered.RetrieveDbAllClientesFilteredTotalRowsAzync.FnRes;
using MainProject.Controllers.CustomClientes.Models.JsonParsed;
using MainProject.Controllers.CustomHelperMaestras.Services;
using MainProject.Controllers.CustomJwt.Services;
using MainProject.Services.CustomDatabaseInput;
using MainProject.Services.CustomHelper;
using MainProject.Services.CustomHelper.Models.FnRes;
using MainProject.Services.CustomSybase.Models.LoadDbInfo.FnRes;
using MainProject.Services.CustomSybase.Models.LoadDbInfo.Json;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace MainProject.Controllers.CustomClientes.Services.Impl
{
    public class CustomClientesService : ICustomClientes
    {
        // INJECTED
        private readonly ICustomHelper _customHelper;
        //private readonly IHttpContextAccessor _httpContextAccessor; // Deleteme al final
        private IConfiguration _configuration; // Leeremos las tablas o vistas usadas
        private readonly ICustomJwt _customJwtService;
        private readonly IStringLocalizer _localizer;
        private readonly IBionegociosDatabaseInput _customSybaseService; // tiene los metodos de la interfaz principal
        private readonly ICustomHelperMaestras _customHelperMaestrasService; // tiene los metodos de la interfaz principal

        // CALCULATED
        private readonly string? _currentServiceName;
        private readonly JsonSerializerOptions _currentJsonOptions;

        // ========== CONSTRUCTOR ========= //
        public CustomClientesService(
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
            //_httpContextAccessor = arghttpContextAccessor;
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
            _localizer = factory.Create("CustomClientes.Resources.CustomClientesController", assemblyObj.Name);
            // ===== TRANSLATION END =====

            _customSybaseService = argcustomSybaseService;
            _customHelperMaestrasService = argcustomHelperMaestrasService;
        }


        // API publica por ahora
        public async Task<string> MainGetClientesAllAsync(string ls_ctrl_json)
        {
            // INICIALIZAMOS VALORES DEFECTO DE RESPUESTA
            ObjFnResMainGetClientesAllDev objFnResDev = new();


            // INICIALIZAMOS VARIABLES REQUERIDAS DE FLUJO                        
            var httpContext = _customHelper.GetCurrentHttpContext();
            string ls_error = "";
            bool lb_developer_mode = false;


            // httpContext error ???
            if (httpContext is null)
            {
                // Esto es más por seguridad

                // [1] error inesperado
                ls_error += "[Error al tratar de acceder al httpContext]: ";
                ls_error = KikesDeveloperErrorResponse(ls_error, _customHelper.LineNumber());

                // Formato esperado por proveedor externo - like bancos
                objFnResDev.KerrorCode = "1"; // Error code: Error inesperado
                objFnResDev.Ksuccess = false;
                objFnResDev.Kmessage = ls_error;


                //return " CustomFileManagerService Upload Error: httpContext is not accesible";
                return ResponseFormatGetClientesAll(objFnResDev, lb_developer_mode); // on fail json received, activate developerMode
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

                        return this.ResponseFormatGetClientesAll(objFnResDev, lb_developer_mode);
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
                        ls_error = KikesDeveloperErrorResponse(ls_error, _customHelper.LineNumber());

                        objFnResDev.KerrorCode = "2664"; // No Se Pudo Validar Token                    
                        objFnResDev.Ksuccess = false;
                        objFnResDev.Kmessage = ls_error;


                        return ResponseFormatGetClientesAll(objFnResDev, lb_developer_mode);
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

                            return ResponseFormatGetClientesAll(objFnResDev, lb_developer_mode);
                        }

                        //[2675] No Se Pudo Validar Token
                        httpContext.Response.StatusCode = 401; // Unauthorized (401)

                        ls_error = _customHelper.Trans(_localizer, "error_token_incorrect") ?? "Http User Identity is NOT Authenticathed :( - Token is NOT VALID.";
                        ls_error = KikesDeveloperErrorResponse("[error_token_incorrect]: " + ls_error, _customHelper.LineNumber());

                        objFnResDev.KerrorCode = "2675"; // No Se Pudo Validar Token
                        objFnResDev.Ksuccess = false;
                        objFnResDev.Kmessage = ls_error;

                        return ResponseFormatGetClientesAll(objFnResDev, lb_developer_mode);
                    }


                }
                // ===== [CONTROL DE ERRORES TOKEN JWT] END





                // Query Database???
                if (true)
                {
                    // Llamar a obtain
                    // BD CONSULTAR REGISTROS FILTRADOS
                    ObjFnResObtainClientesAllObjList obtainClientesAllObjListResult = await this.ObtainClientesAllObjectListAzync();


                    
                     // devuelve snap pero convertimos a jsonmodel
                    // List<ClienteJsonModelParsed> clientesList = new();

                    


                    // ERROR IN RESPONSE REMOTE DB ???
                    if (obtainClientesAllObjListResult.Ksuccess == false)
                    {
                        // Header response [500] Server error
                        httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                        ls_error = KikesDeveloperErrorResponse("Error al obtener clientes all object list . " + obtainClientesAllObjListResult.Kmessage, _customHelper.LineNumber());

                        // Body response
                        objFnResDev.Ksuccess = false;
                        objFnResDev.Kmessage = ls_error;
                        objFnResDev.KerrorCode = "3306500"; // Error de conexion a la bd api                         
                        return ResponseFormatGetClientesAll(objFnResDev, lb_developer_mode);
                    }


                    // RECORDS FOUND???
                    if (obtainClientesAllObjListResult.Ksuccess && obtainClientesAllObjListResult.Kdata.Count >= 0)
                    {
                        

                        // CUSTOMIZE PROPERTIES IN RESULT???
                        if (true)
                        {
                            int li_total_registros = obtainClientesAllObjListResult.Kdata.Count;
                            for (int i=0; i < li_total_registros; i++)
                            {
                                objFnResDev.Kdata.Add(new ClienteJsonCustomized());
                                var cliente = obtainClientesAllObjListResult.Kdata[i];

                                objFnResDev.Kdata[i].Id = cliente.Id;
                                objFnResDev.Kdata[i].Id_empresa = cliente.Id_empresa?.Trim();
                                //objFnResDev.Kdata[i].Sucursal = cliente.Sucursal?.Trim();
                                objFnResDev.Kdata[i].Id_cliente = cliente.Id_cliente?.Trim();
                                objFnResDev.Kdata[i].Ind_rut = cliente.Ind_rut?.Trim();
                                objFnResDev.Kdata[i].Canal_distribucion = cliente.Canal_distribucion?.Trim();
                                objFnResDev.Kdata[i].Estado_cliente = cliente.Estado_cliente?.Trim();
                                objFnResDev.Kdata[i].Primer_nombre = cliente.Primer_nombre?.Trim();
                                objFnResDev.Kdata[i].Segundo_nombre = cliente.Segundo_nombre?.Trim();
                                objFnResDev.Kdata[i].Primer_apellido = cliente.Primer_apellido?.Trim();
                                objFnResDev.Kdata[i].Segundo_apellido = cliente.Segundo_apellido?.Trim();
                                //objFnResDev.Kdata[i].Fecha_cumpleanos = cliente.Fecha_cumpleanos?.Trim();
                                objFnResDev.Kdata[i].Nom_negocio = cliente.Nom_negocio?.Trim();
                                objFnResDev.Kdata[i].Tipo_negocio = cliente.Tipo_negocio?.Trim();
                                objFnResDev.Kdata[i].Direccion = cliente.Direccion?.Trim();
                                objFnResDev.Kdata[i].Telefono = cliente.Telefono?.Trim();
                                objFnResDev.Kdata[i].Celular = cliente.Celular?.Trim();
                                objFnResDev.Kdata[i].E_mail = cliente.E_mail?.Trim();
                                objFnResDev.Kdata[i].Cod_departamento = cliente.Cod_departamento?.Trim();
                                objFnResDev.Kdata[i].Id_ciudad = cliente.Id_ciudad?.Trim();
                                objFnResDev.Kdata[i].Zona = cliente.Zona?.Trim();
                                objFnResDev.Kdata[i].Cod_dias_visita = cliente.Cod_dias_visita?.Trim();
                                //objFnResDev.Kdata[i].Orden_visita = cliente.Orden_visita?.Trim();
                                //objFnResDev.Kdata[i].Orden_entrega = cliente.Orden_entrega?.Trim();
                                objFnResDev.Kdata[i].Cod_ruta_distribucion = cliente.Cod_ruta_distribucion?.Trim();
                                objFnResDev.Kdata[i].Ind_controlar_cupo = cliente.Ind_controlar_cupo?.Trim();
                                objFnResDev.Kdata[i].Cupo = cliente.Cupo;
                                objFnResDev.Kdata[i].Saldo = cliente.Saldo;
                                objFnResDev.Kdata[i].Anticipos = cliente.Anticipos;
                                objFnResDev.Kdata[i].Nit_alterno = cliente.Nit_alterno?.Trim();
                                objFnResDev.Kdata[i].Nombre_alterno = cliente.Nombre_alterno?.Trim();
                                objFnResDev.Kdata[i].Apellidos_alterno = cliente.Apellidos_alterno?.Trim();
                                //objFnResDev.Kdata[i].Plazo_factura = cliente.Plazo_factura?.Trim();
                                //objFnResDev.Kdata[i].Num_factura_cartera = cliente.Num_factura_cartera?.Trim();
                                objFnResDev.Kdata[i].Ind_gran_contribuyente = cliente.Ind_gran_contribuyente?.Trim();
                                objFnResDev.Kdata[i].Ind_autoretenedor = cliente.Ind_autoretenedor?.Trim();
                                objFnResDev.Kdata[i].Resolucion_retencion_fuente = cliente.Resolucion_retencion_fuente?.Trim();
                                objFnResDev.Kdata[i].Ind_agente_retencion_renta = cliente.Ind_agente_retencion_renta?.Trim();
                                objFnResDev.Kdata[i].Ind_facturar_iva = cliente.Ind_facturar_iva?.Trim();
                                objFnResDev.Kdata[i].Regimen_iva = cliente.Regimen_iva?.Trim();
                                objFnResDev.Kdata[i].Nota1 = cliente.Nota1?.Trim();
                                //objFnResDev.Kdata[i].Fecha_ingreso = cliente.Fecha_ingreso?.Trim();
                                //objFnResDev.Kdata[i].Fecha_factura = cliente.Fecha_factura?.Trim();
                                //objFnResDev.Kdata[i].Fec_registro = cliente.Fec_registro?.Trim();
                                objFnResDev.Kdata[i].Ind_modificado = cliente.Ind_modificado?.Trim();
                                objFnResDev.Kdata[i].Naturaleza = cliente.Naturaleza?.Trim();
                                objFnResDev.Kdata[i].Razon_social = cliente.Razon_social?.Trim();
                                objFnResDev.Kdata[i].Id_zona_facturacion = cliente.Id_zona_facturacion?.Trim();
                                objFnResDev.Kdata[i].Ind_masivo = cliente.Ind_masivo?.Trim();
                                objFnResDev.Kdata[i].Id_categoria_cliente = cliente.Id_categoria_cliente?.Trim();
                                //objFnResDev.Kdata[i].Orden_visita_lunes = cliente.Orden_visita_lunes?.Trim();
                                //objFnResDev.Kdata[i].Orden_visita_martes = cliente.Orden_visita_martes?.Trim();
                                //objFnResDev.Kdata[i].Orden_visita_miercoles = cliente.Orden_visita_miercoles?.Trim();
                                //objFnResDev.Kdata[i].Orden_visita_jueves = cliente.Orden_visita_jueves?.Trim();
                                //objFnResDev.Kdata[i].Orden_visita_viernes = cliente.Orden_visita_viernes?.Trim();
                                //objFnResDev.Kdata[i].Orden_visita_sabado = cliente.Orden_visita_sabado?.Trim();
                                //objFnResDev.Kdata[i].Orden_visita_domingo = cliente.Orden_visita_domingo?.Trim();
                                objFnResDev.Kdata[i].Num_impresion_original = cliente.Num_impresion_original?.Trim();
                                objFnResDev.Kdata[i].Ind_entrega_certificada = cliente.Ind_entrega_certificada?.Trim();
                                objFnResDev.Kdata[i].Valor_latitud = cliente.Valor_latitud?.Trim();
                                objFnResDev.Kdata[i].Valor_longitud = cliente.Valor_longitud?.Trim();
                                objFnResDev.Kdata[i].Porcentaje_rotura = cliente.Porcentaje_rotura?.Trim();
                                objFnResDev.Kdata[i].Tipo_cliente = cliente.Tipo_cliente?.Trim();
                                objFnResDev.Kdata[i].Correo_factu_electronica = cliente.Correo_factu_electronica?.Trim();
                            }                            
                        }
                    }

                }

                // all ok
                objFnResDev.KerrorCode = "0";
                objFnResDev.Kmessage = "Proceso listar clientes ALL finalizado con exito.";
                objFnResDev.Ksuccess = true;

                // Return expected response
                return ResponseFormatGetClientesAll(objFnResDev, lb_developer_mode); // on fail json received, activate developerMode
            }

        }


        // API publica por ahora
        public async Task<string> MainGetClientesFilteredAsync(string ls_ctrl_json)
        {
            // INICIALIZAMOS VALORES DEFECTO DE RESPUESTA
            ObjFnResMainGetClientesFilteredDev objFnResDev = new();


            // INICIALIZAMOS VARIABLES REQUERIDAS DE FLUJO                        
            var httpContext = _customHelper.GetCurrentHttpContext();
            string ls_error = "";
            bool lb_developer_mode = false;


            // httpContext error ???
            if (httpContext is null)
            {
                // Header response [500] Server error
                // httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)                
                ls_error += "[Error al tratar de acceder al httpContext]: ";
                ls_error = KikesDeveloperErrorResponse(ls_error, _customHelper.LineNumber());

                // Body response
                objFnResDev.KerrorCode = "1"; // Error code: Error inesperado
                objFnResDev.Ksuccess = false;
                objFnResDev.Kmessage = ls_error;

                return ResponseFormatGetClientesFiltered(objFnResDev, lb_developer_mode);
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

                        return this.ResponseFormatGetClientesFiltered(objFnResDev, lb_developer_mode);
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
                        ls_error = KikesDeveloperErrorResponse(ls_error, _customHelper.LineNumber());

                        objFnResDev.KerrorCode = "2664"; // No Se Pudo Validar Token                    
                        objFnResDev.Ksuccess = false;
                        objFnResDev.Kmessage = ls_error;


                        return ResponseFormatGetClientesFiltered(objFnResDev, lb_developer_mode);
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

                            return ResponseFormatGetClientesFiltered(objFnResDev, lb_developer_mode);
                        }

                        //[2675] No Se Pudo Validar Token
                        httpContext.Response.StatusCode = 401; // Unauthorized (401)

                        ls_error = _customHelper.Trans(_localizer, "error_token_incorrect") ?? "Http User Identity is NOT Authenticathed :( - Token is NOT VALID.";
                        ls_error = KikesDeveloperErrorResponse("[error_token_incorrect]: " + ls_error, _customHelper.LineNumber());

                        objFnResDev.KerrorCode = "2675"; // No Se Pudo Validar Token
                        objFnResDev.Ksuccess = false;
                        objFnResDev.Kmessage = ls_error;

                        return ResponseFormatGetClientesFiltered(objFnResDev, lb_developer_mode);
                    }


                }
                // ===== [CONTROL DE ERRORES TOKEN JWT] END





                // Query Database???
                if (true)
                {

                    int li_limit = _customHelperMaestrasService.GetLimitAllowed(); // Sera enviada en la respuesta a postman
                    List<ClienteJsonModelFiltered> clientesList = new();
                    int li_total_rows_filtered = 0;




                    // BD CONSULTAR REGISTROS FILTRADOS
                    ObjFnResObtainClientesObjList ObtainClientesObjListResult = await this.ObtainClientesObjectListAzync();
                    // ERROR ???
                    if (ObtainClientesObjListResult.Ksuccess == false)
                    {
                        // Header response [500] Server error
                        httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                        ls_error = KikesDeveloperErrorResponse("Error al obtener clientes object list . " + ObtainClientesObjListResult.Kmessage, _customHelper.LineNumber());

                        // Body response
                        objFnResDev.Ksuccess = false;
                        objFnResDev.Kmessage = ls_error;
                        objFnResDev.KerrorCode = "3306500"; // Error de conexion a la bd api                         
                        return ResponseFormatGetClientesFiltered(objFnResDev, lb_developer_mode);
                    }

                    // RECORDS FOUND???
                    if (ObtainClientesObjListResult.Ksuccess && ObtainClientesObjListResult.Kdata.Count >= 0)
                    {
                        //clientesList = ObtainClientesObjListResult.Kdata;                                                                                                

                        // CUSTOMIZE PROPERTIES IN RESULT???
                        if (true)
                        {
                            for (int i = 0; i < ObtainClientesObjListResult.Kdata.Count; i++)                            
                            {
                                var temporalCustomJson = new CmClientesModelJsonCustom();
                                objFnResDev.Kdata.Add(temporalCustomJson);

                                var cliente = ObtainClientesObjListResult.Kdata[i];
                                Console.WriteLine(JsonSerializer.Serialize(cliente));
                                                               

                                objFnResDev.Kdata[i].Id = cliente.Id;

                                objFnResDev.Kdata[i].Id_empresa = cliente.Id_empresa?.Trim();

                                objFnResDev.Kdata[i].Sucursal = cliente.Sucursal;

                                objFnResDev.Kdata[i].Id_cliente = cliente.Id_cliente?.Trim();

                                objFnResDev.Kdata[i].Ind_rut = cliente.Ind_rut?.Trim();

                                objFnResDev.Kdata[i].Canal_distribucion = cliente.Canal_distribucion?.Trim();

                                objFnResDev.Kdata[i].Estado_cliente = cliente.Estado_cliente?.Trim();

                                objFnResDev.Kdata[i].Primer_nombre = cliente.Primer_nombre?.Trim();

                                objFnResDev.Kdata[i].Segundo_nombre = cliente.Segundo_nombre?.Trim();

                                objFnResDev.Kdata[i].Primer_apellido = cliente.Primer_apellido?.Trim();

                                objFnResDev.Kdata[i].Segundo_apellido = cliente.Segundo_apellido?.Trim();

                                objFnResDev.Kdata[i].Fecha_cumpleanos = cliente.Fecha_cumpleanos;

                                objFnResDev.Kdata[i].Nom_negocio = cliente.Nom_negocio?.Trim();

                                objFnResDev.Kdata[i].Tipo_negocio = cliente.Tipo_negocio?.Trim();

                                objFnResDev.Kdata[i].Direccion = cliente.Direccion?.Trim();

                                objFnResDev.Kdata[i].Telefono = cliente.Telefono?.Trim();

                                objFnResDev.Kdata[i].Celular = cliente.Celular?.Trim();

                                objFnResDev.Kdata[i].E_mail = cliente.E_mail?.Trim();

                                objFnResDev.Kdata[i].Cod_departamento = cliente.Cod_departamento?.Trim();

                                objFnResDev.Kdata[i].Id_ciudad = cliente.Id_ciudad?.Trim();

                                objFnResDev.Kdata[i].Zona = cliente.Zona?.Trim();

                                objFnResDev.Kdata[i].Cod_dias_visita = cliente.Cod_dias_visita?.Trim();

                                objFnResDev.Kdata[i].Orden_visita = cliente.Orden_visita;

                                objFnResDev.Kdata[i].Orden_entrega = cliente.Orden_entrega;

                                objFnResDev.Kdata[i].Cod_ruta_distribucion = cliente.Cod_ruta_distribucion?.Trim();

                                objFnResDev.Kdata[i].Ind_controlar_cupo = cliente.Ind_controlar_cupo?.Trim();

                                objFnResDev.Kdata[i].Cupo = cliente.Cupo?.ToString();

                                objFnResDev.Kdata[i].Saldo = cliente.Saldo?.ToString();

                                objFnResDev.Kdata[i].Anticipos = cliente.Anticipos?.ToString();

                                objFnResDev.Kdata[i].Nit_alterno = cliente.Nit_alterno?.Trim();

                                objFnResDev.Kdata[i].Nombre_alterno = cliente.Nombre_alterno?.Trim();

                                objFnResDev.Kdata[i].Apellidos_alterno = cliente.Apellidos_alterno?.Trim();

                                objFnResDev.Kdata[i].Plazo_factura = cliente.Plazo_factura?.ToString();

                                objFnResDev.Kdata[i].Num_factura_cartera = cliente.Num_factura_cartera;

                                objFnResDev.Kdata[i].Ind_gran_contribuyente = cliente.Ind_gran_contribuyente?.Trim();

                                objFnResDev.Kdata[i].Ind_autoretenedor = cliente.Ind_autoretenedor?.Trim();

                                objFnResDev.Kdata[i].Resolucion_retencion_fuente = cliente.Resolucion_retencion_fuente?.Trim();

                                objFnResDev.Kdata[i].Ind_agente_retencion_renta = cliente.Ind_agente_retencion_renta?.Trim();

                                objFnResDev.Kdata[i].Ind_facturar_iva = cliente.Ind_facturar_iva?.Trim();

                                objFnResDev.Kdata[i].Regimen_iva = cliente.Regimen_iva?.Trim();

                                objFnResDev.Kdata[i].Nota1 = cliente.Nota1?.Trim();

                                objFnResDev.Kdata[i].Fecha_ingreso = cliente.Fecha_ingreso;

                                objFnResDev.Kdata[i].Fecha_factura = cliente.Fecha_factura;

                                objFnResDev.Kdata[i].Fec_registro = cliente.Fec_registro;

                                objFnResDev.Kdata[i].Ind_modificado = cliente.Ind_modificado?.Trim();

                                objFnResDev.Kdata[i].Naturaleza = cliente.Naturaleza?.Trim();

                                objFnResDev.Kdata[i].Razon_social = cliente.Razon_social?.Trim();

                                objFnResDev.Kdata[i].Id_zona_facturacion = cliente.Id_zona_facturacion?.Trim();

                                objFnResDev.Kdata[i].Ind_masivo = cliente.Ind_masivo?.Trim();

                                objFnResDev.Kdata[i].Id_categoria_cliente = cliente.Id_categoria_cliente?.Trim();

                                objFnResDev.Kdata[i].Orden_visita_lunes = cliente.Orden_visita_lunes;

                                objFnResDev.Kdata[i].Orden_visita_martes = cliente.Orden_visita_martes;

                                objFnResDev.Kdata[i].Orden_visita_miercoles = cliente.Orden_visita_miercoles;

                                objFnResDev.Kdata[i].Orden_visita_jueves = cliente.Orden_visita_jueves;

                                objFnResDev.Kdata[i].Orden_visita_viernes = cliente.Orden_visita_viernes;

                                objFnResDev.Kdata[i].Orden_visita_sabado = cliente.Orden_visita_sabado;

                                objFnResDev.Kdata[i].Orden_visita_domingo = cliente.Orden_visita_domingo;

                                objFnResDev.Kdata[i].Num_impresion_original = cliente.Num_impresion_original?.Trim();

                                objFnResDev.Kdata[i].Ind_entrega_certificada = cliente.Ind_entrega_certificada?.Trim();

                                objFnResDev.Kdata[i].Valor_latitud = cliente.Valor_latitud?.Trim();

                                objFnResDev.Kdata[i].Valor_longitud = cliente.Valor_longitud?.Trim();

                                objFnResDev.Kdata[i].Porcentaje_rotura = cliente.Porcentaje_rotura?.Trim();

                                objFnResDev.Kdata[i].Tipo_cliente = cliente.Tipo_cliente?.Trim();
                                
                                objFnResDev.Kdata[i].Correo_factu_electronica = cliente.Correo_factu_electronica?.Trim();

    }
                        }
                    }




                    // BD CONSULTAR TOTAL ROWS FILTERED
                    var ObtainTotalRowsFilWithOutLimitCliRs = await ObtainTotalRowsFilteredWithOutLimitClientes();
                    // ERROR ???
                    if (ObtainTotalRowsFilWithOutLimitCliRs.Ksuccess == false)
                    {
                        // Header response [500] Server error
                        httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                        ls_error = KikesDeveloperErrorResponse("Error al obtener clientes totalized object list . " + ObtainTotalRowsFilWithOutLimitCliRs.Kmessage, _customHelper.LineNumber());

                        // Body response
                        objFnResDev.Ksuccess = false;
                        objFnResDev.Kmessage = ls_error;
                        objFnResDev.KerrorCode = "3306500"; // Error de conexion a la bd api                         
                        return ResponseFormatGetClientesFiltered(objFnResDev, lb_developer_mode);
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
                return ResponseFormatGetClientesFiltered(objFnResDev, lb_developer_mode); // on fail json received, activate developerMode
            }

        }
        

        // Define a custom helper
        public async Task<ObjFnResObtainClientesObjList> ObtainClientesObjectListAzync()
        {
            // INICIALIZAMOS VALORES DEFECTO DE RESPUESTA
            ObjFnResObtainClientesObjList objFnResDev = new();

            var httpContext = _customHelper.GetCurrentHttpContext();
            string ls_error = "";
            string ls_jsonresponse_bdclientesfiltered = "{}";

            try
            {
                ls_jsonresponse_bdclientesfiltered = await this.RetrieveDbAllClientesFilteredAzync(); // get rows filtered and limited                
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
            ObjFnResBdConsultarClientesFilteredJsonModel dbClientesSearchResult = new ObjFnResBdConsultarClientesFilteredJsonModel(); // Force to be an object
            ObjFnResBdConsultarClientesFilteredJsonModel? dbClientesSearchResultOrNull = null; // necesary variable to try catch to cast postman json object            
            try
            {
                dbClientesSearchResultOrNull = JsonSerializer.Deserialize<ObjFnResBdConsultarClientesFilteredJsonModel>(ls_jsonresponse_bdclientesfiltered, _currentJsonOptions);
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
            dbClientesSearchResult = dbClientesSearchResultOrNull ?? new ObjFnResBdConsultarClientesFilteredJsonModel();


            // ERROR IN RESPONSE REMOTE DB ???
            if (dbClientesSearchResult.Success == false)
            {
                // Header response [500] Server error
                httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                ls_error = KikesDeveloperErrorResponse("Error de conexión a la bd. " + dbClientesSearchResult.Message, _customHelper.LineNumber());

                // Body response
                objFnResDev.Ksuccess = false;
                objFnResDev.Kmessage = ls_error;
                objFnResDev.KerrorCode = "3306500"; // Error de conexion a la bd api                                         
                return objFnResDev;
            }


            // RECORDS FOUND???
            if (dbClientesSearchResult.Data.Count >= 0)
            {
                objFnResDev.KerrorCode = "0"; // Exito!!
                objFnResDev.Ksuccess = true;
                objFnResDev.Kmessage = "Proceso listar clientes finalizado con exito.";
                objFnResDev.Kdata = dbClientesSearchResult.Data;
            }

            // CUSTOMIZE PROPERTIES IN RESULT???
            if (true)
            {
                for (int i = 0; i < objFnResDev.Kdata.Count; i++)
                {
                    var cliente = objFnResDev.Kdata[i];
                    objFnResDev.Kdata[i].Id_cliente = cliente.Id_cliente?.Trim();
                }
            }

            return objFnResDev;
        }


        // Define a custom helper
        public async Task<ObjFnResObtainClientesAllObjList> ObtainClientesAllObjectListAzync()
        {
            // INICIALIZAMOS VALORES DEFECTO DE RESPUESTA
            ObjFnResObtainClientesAllObjList objFnResDev = new();

            var httpContext = _customHelper.GetCurrentHttpContext();
            string ls_error = "";
            string ls_jsonresponse_bdclientes_all = "{}";

            try
            {
                ls_jsonresponse_bdclientes_all = await this.RetrieveDbClientesAllAzync(); // get all rows                
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
            ObjFnResRetrieveDbClientesAllAzync dbClientesSearchResult = new ObjFnResRetrieveDbClientesAllAzync(); // Force to be an object
            ObjFnResRetrieveDbClientesAllAzync? dbClientesSearchResultOrNull = null; // necesary variable to try catch to cast postman json object            
            try
            {
                dbClientesSearchResultOrNull = JsonSerializer.Deserialize<ObjFnResRetrieveDbClientesAllAzync>(ls_jsonresponse_bdclientes_all, _currentJsonOptions);
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
            dbClientesSearchResult = dbClientesSearchResultOrNull ?? new ObjFnResRetrieveDbClientesAllAzync();


            // ERROR IN RESPONSE REMOTE DB ???
            if (dbClientesSearchResult.Success == false)
            {
                // Header response [500] Server error
                httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                ls_error = KikesDeveloperErrorResponse("Error de conexión a la bd. " + dbClientesSearchResult.Message, _customHelper.LineNumber());

                // Body response
                objFnResDev.Ksuccess = false;
                objFnResDev.Kmessage = ls_error;
                objFnResDev.KerrorCode = "3306500"; // Error de conexion a la bd api                                         
                return objFnResDev;
            }


            // RECORDS FOUND???
            if (dbClientesSearchResult.Data.Count >= 0)
            {
                objFnResDev.KerrorCode = "0"; // Exito!!
                objFnResDev.Ksuccess = true;
                objFnResDev.Kmessage = "Proceso listar clientes finalizado con exito.";
                objFnResDev.Kdata = dbClientesSearchResult.Data;
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


        // Define a custom helper
        public async Task<ObjFnResObtainTotalRowsFilteredWithOutLimitedClientes> ObtainTotalRowsFilteredWithOutLimitClientes()
        {
            // INICIALIZAMOS VALORES DEFECTO DE RESPUESTA
            ObjFnResObtainTotalRowsFilteredWithOutLimitedClientes objFnResDev = new();

            var httpContext = _customHelper.GetCurrentHttpContext();
            string ls_error = "";
            string ls_jsonresponse_bdclientesfiltered = "{}";

            try
            {
                ls_jsonresponse_bdclientesfiltered = await this.RetrieveDbAllClientesFilteredTotalRowsAzync(); // get rows filtered and limited
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
            ObjFnResRetrieveDbAllClientesFilteredTotalRows dbClientesSearchResult = new ObjFnResRetrieveDbAllClientesFilteredTotalRows(); // Force to be an object
            ObjFnResRetrieveDbAllClientesFilteredTotalRows? dbClientesSearchResultOrNull = null; // necesary variable to try catch to cast postman json object            
            try
            {
                dbClientesSearchResultOrNull = JsonSerializer.Deserialize<ObjFnResRetrieveDbAllClientesFilteredTotalRows>(ls_jsonresponse_bdclientesfiltered, _currentJsonOptions);
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
            dbClientesSearchResult = dbClientesSearchResultOrNull ?? new ObjFnResRetrieveDbAllClientesFilteredTotalRows();


            // ERROR IN RESPONSE REMOTE DB ???
            if (dbClientesSearchResult.Success == false)
            {
                // Header response [500] Server error
                httpContext.Response.StatusCode = 500; // Report error in server to client - Internal Server Error (500)
                ls_error = KikesDeveloperErrorResponse("Error de conexión a la bd. " + dbClientesSearchResult.Message, _customHelper.LineNumber());

                // Body response
                objFnResDev.Ksuccess = false;
                objFnResDev.Kmessage = ls_error;
                objFnResDev.KerrorCode = "3306500"; // Error de conexion a la bd api                                         
                return objFnResDev;
            }


            // RECORDS FOUND???
            if (dbClientesSearchResult.Data.Count >= 0)
            {
                objFnResDev.KerrorCode = "0"; // Exito!!
                objFnResDev.Ksuccess = true;
                objFnResDev.Kmessage = "Proceso listar clientes finalizado con exito.";
                objFnResDev.Kdata.Add(dbClientesSearchResult.Data[0]);
            }

            return objFnResDev;
        }


        // Define a custom helper
        public async Task<string> RetrieveDbAllClientesFilteredAzync()
        {
            ObjFnResGetDefaultFilterParams aParams = this.GetDefaultFilterParams();


            // SOME VARS TO get full response from remote laft api            
            Dictionary<string, object> la_params_remote = new Dictionary<string, object>
            {                
                // remote required
                { "li_limit", aParams.Limit },
                { "ls_fec_registro_min", aParams.Fec_registro_min },
                { "ls_zona", aParams.Zona },
                { "li_after_id", aParams.After_id }
            };


            string ls_jsonres = await _customSybaseService.QueryClientesFilteredAzync(la_params_remote); // Esperamos una json de respuesta


            return ls_jsonres;
        }
        
        // Define a custom helper
        public async Task<string> RetrieveDbClientesAllAzync()
        {            
            string ls_jsonres = await _customSybaseService.QueryClientesAllAzync();
            return ls_jsonres;
        }


        // Define a custom helper
        public async Task<string> RetrieveDbAllClientesFilteredTotalRowsAzync()
        {
            ObjFnResGetDefaultFilterParams aParams = this.GetDefaultFilterParams();


            // SOME VARS TO get full response from remote laft api            
            Dictionary<string, object> la_params_remote = new Dictionary<string, object>
            {                
                // remote required
                { "li_limit", aParams.Limit },
                { "ls_fec_registro_min", aParams.Fec_registro_min },
                { "ls_zona", aParams.Zona },
                { "li_after_id", aParams.After_id }
            };


            string ls_jsonres = await _customSybaseService.QueryClientesFilteredTotalRowsAzync(la_params_remote); // Esperamos una json de respuesta


            return ls_jsonres;
        }


        



        // Define a custom helper
        public ObjFnResGetDefaultFilterParams GetDefaultFilterParams()
        {
            // INICIALIZAMOS VALORES DEFECTO DE RESPUESTA
            ObjFnResGetDefaultFilterParams aParams = new();
            var httpContext = _customHelper.GetCurrentHttpContext();
            var Request = httpContext.Request;

            int li_limit = _customHelperMaestrasService.GetLimitAllowed(); // example 500            
            string ls_fec_registro_min = "1900-12-31";
            int li_after_id = -1;
            string ls_zona = "";


            string ls_fec_registro_min_or_empty = Request.Query["fec_registro_min"].ToString(); // StringValues cuando es vacío no da error, y al convertirlo al ToString() resulta en cadena vacía, no 'null'
            ls_fec_registro_min = ls_fec_registro_min_or_empty.Length > 0 ? ls_fec_registro_min_or_empty : ls_fec_registro_min;

            string ls_after_id_or_empty = Request.Query["after_id"].ToString(); // StringValues cuando es vacío no da error, y al convertirlo al ToString() resulta en cadena vacía, no 'null'
            ObjFnResCustomConverToInt32 objRs = _customHelper.CustomConvertToInt32("after_id", ls_after_id_or_empty);
            if (objRs.Status == true)
            {
                if (objRs.Data is not null && objRs.Data.Count() > 0)
                {
                    li_after_id = objRs.Data[0]; // Nuevo limite recibido
                }
            }

            string ls_zona_or_empty = Request.Query["zona"].ToString(); // StringValues cuando es vacío no da error, y al convertirlo al ToString() resulta en cadena vacía, no 'null'
            ls_zona = ls_zona_or_empty.Length > 0 ? ls_zona_or_empty : ls_zona;


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
            aParams.Zona = ls_zona;

            return aParams;
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
        private string ResponseFormatGetClientesAll(ObjFnResMainGetClientesAllDev stdFnResDev, bool developerMode = false)
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
        private string ResponseFormatGetClientesFiltered(ObjFnResMainGetClientesFilteredDev stdFnResDev, bool developerMode = false)
        {
            var httpContext = _customHelper.GetCurrentHttpContext();
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





    //Endclass
    }
}
