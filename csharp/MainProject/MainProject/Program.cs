using Microsoft.EntityFrameworkCore;
using System.Data.Odbc;
using MainProject.Services.CustomHelper;
using SnapObjects.Data;
using MainProject;
using SnapObjects.Data.Odbc;
using MainProject.Services.CustomDatabaseInput;
using MainProject.Services.CustomHelper.Impl;
using MainProject.Services.CustomSybase.Impl;
using MainProject.Middleware;
using MainProject.Controllers.CurrentProject.Services.Impl;
using MainProject.Controllers.CurrentProject.Services;
using MainProject.Controllers.CustomJwt.Services;
using MainProject.Controllers.CustomJwt.Services.Impl;
using MainProject.Services.CustomTokenCache;
using MainProject.Services.CustomTokenCache.Impl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.OpenApi.Models;
using MainProject.StaticServices;
using MainProject.Controllers.CustomSwagger.Services;
using MainProject.Controllers.CustomSwagger.Services.Impl;
using MainProject.Controllers.CustomTiposNegocio.Services.Impl;
using MainProject.Controllers.CustomTiposNegocio.Services;
using MainProject.Controllers.CustomClientes.Services.Impl;
using MainProject.Controllers.CustomClientes.Services;
using MainProject.Controllers.CustomHelperMaestras.Services;
using MainProject.Controllers.CustomHelperMaestras.Services.Impl;
using MainProject.Controllers.CustomDepartamentos.Services;
using MainProject.Controllers.CustomDepartamentos.Services.Impl;
using MainProject.Controllers.CustomMunicipios.Services;
using MainProject.Controllers.CustomMunicipios.Services.Impl;
using MainProject.Controllers.CmUsuariosMovil.Services;
using MainProject.Controllers.CmUsuariosMovil.Services.Impl;
using MainProject.Controllers.CmLineasProductos.Services;
using MainProject.Controllers.CmLineasProductos.Services.Impl;
using MainProject.Controllers.CmProductos.Services;
using MainProject.Controllers.CmProductos.Services.Impl;
using MainProject.Controllers.CmMotivosNc.Services;
using MainProject.Controllers.CmMotivosNc.Services.Impl;
using MainProject.Controllers.CmZonasBio.Services;
using MainProject.Controllers.CmZonasBio.Services.Impl;
using System.Text.Json;

public class Program
{
    private static string _ls_default_language = "es"; // Defecto spanish - Recuerda que tambien acepta valores como: "es-CO"
    private static string[] _la_languages = new[] { "en", "es" }; // array string
    private static CultureInfo[] _la_cultureInfoList = new[] { new CultureInfo("en"), new CultureInfo("es") }; // array cultureObj    
    
    public static void Main(string[] args)
    {
        // ===== En el codigo se utiliza WRAP para hacer referencia a bloques opcionales
        // De esta forma es más facil ver el código minimo requerido para la app/api

        var builder = WebApplication.CreateBuilder(args); // Create builder        

        // ===== WRAP FOR TESTING PRODUCTION MODE
        if (true)
        {
            bool lb_simular_ejecucion_en_modo_produccion = false; // Default: false
            lb_simular_ejecucion_en_modo_produccion = builder.Configuration.GetValue<bool>("CustomApp:EnableAppSettingsProductionMode");
            if (lb_simular_ejecucion_en_modo_produccion)
            {                
                // Recuerda testear tambien en ambiente de Produccion para detectar errores.
                // Asi puedes evitar error 500 antes de publicar en Servidor IIS ...                

                // Enable debug production Mode for developers
                builder = WebApplication.CreateBuilder(new WebApplicationOptions
                {
                    Args = args,
                    EnvironmentName = Environments.Production
                });
                
                // Error in required property?? - Stop program and notify developer
                if (builder.Configuration["Jwt:Issuer"] is null)
                {
                    throw new Exception("No se ha definido las credenciales para JWT en appsettings.json");
                }
            }
        }
        
        

        ConfigureApp(builder); // Set Configurations
        var app = builder.Build(); // Create App
        

        // Some appsettings values
        bool lb_log_in_request_to_file = builder.Configuration.GetValue<bool>("CustomErrorReporting:EnableInMessagesLogToFile");
        bool lb_log_out_request_to_file = builder.Configuration.GetValue<bool>("CustomErrorReporting:EnableOutMessagesLogToFile");
        bool lb_log_out_request_to_database = builder.Configuration.GetValue<bool>("CustomErrorReporting:EnableOutMessagesLogToDatabase");
        bool lb_swaggerauth_middleware = builder.Configuration.GetValue<bool>("CustomApp:EnableSwaggerAuthCookieMiddleware");


        // ===== WRAP CUSTOM MIDDLEWARES PARA ARCHIVOS DE LOG
        if (true)
        {                        
            if (lb_log_in_request_to_file) app.UseMyCustomSaveRequestToLogMiddleware(); // Enable log to file [incoming requests]
            if (lb_log_out_request_to_file) app.UseMyCustomSaveRequestResponseToLogMiddleware(); // Enable log to file[outgoing requests]
        }


        // WRAP STATIC FILES FOR [RAZORPAGES 2/3] - [LOGIN FORM MVC 2/4]
        if (true)
        {            
            app.UseStaticFiles();
        }


        app.UseRouting(); // Default controllers                
        app.UseAuthentication(); // jwt authentication - cookies pueden ser leidas despues de esta linea        
        app.UseAuthorization();


        // ===== WRAP REQUIRE MIDD AUTH - FOR ROUTES SWAGGER - [LOGIN FORM MVC 4/4]
        if (true)
        {
            if (lb_swaggerauth_middleware) app.UseMiddleware<CustomSwaggerAuthMiddleware>();
        }        


        // WRAP ENABLE SWAGGER [USE-SWAGGER] 2/2
        if (true)
        {            
            StaticConfigService.Configuration = builder.Configuration; // Set static property para el acceso al Config json            

            // Configure the HTTP request pipeline.
            string ls_swagger_api_name = builder.Configuration.GetValue<string>("CustomApp:SwaggerApiName");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", ls_swagger_api_name);                
                c.DefaultModelsExpandDepth(-1);  // Disable schemas at bottom of webpage
            });
        }


        // Set default languages
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(_ls_default_language),
            SupportedCultures = _la_cultureInfoList,
            SupportedUICultures = _la_cultureInfoList
        });


        // WRAP STATIC FILES FOR [RAZORPAGES 3/3] - [LOGIN FORM MVC 3/4]
        if (true)
        {
            app.MapRazorPages(); // Mapea Razor Pages

        }


        app.MapControllers(); // Sin middleware

        app.Run();
    }





    // another static function helper
    public static void ConfigureApp(WebApplicationBuilder builder)
    {
        // Look at these file for understand multiple file logs. Do not delete
        // Here we capture incoming requests and responses to File
        string nlogDefaultConfigPath = "NLog.config".ToString(); // This line is informational only

        // Look at these file for understand default config values loaded. Do not delete
        // Here we remember which file is loaded by default when starting "builder" in Program.cs
        string appsettingsjsonDefaultConfigPath = "appsettings.json".ToString(); // This line is informational only  
        
        // Some logs in programcs boot only
        string ls_current_environment = "Ambiente Productivo"; // Util to dev logs
        bool lb_enable_programcs_info_console_log = builder.Configuration.GetValue<bool>("CustomApp:EnableProgramcsInfoConsoleLog");
        if ( builder.Environment.IsDevelopment() ) ls_current_environment = builder.Environment.EnvironmentName;

        Console.WriteLine("// ===== ===== ===== ===== ===== ===== ===== ===== //");        
        Console.WriteLine($"Environment Mode: [{ls_current_environment}]");
        Console.WriteLine($"CustomApp:EnableProgramcsInfoConsoleLog: [{lb_enable_programcs_info_console_log}]");
        Console.WriteLine("// ===== ===== ===== ===== ===== ===== ===== ===== //");

        // ===== Add services to the container
        // =====

        // Set path lo search translation language
        builder.Services.AddLocalization(options => options.ResourcesPath = "Controllers"); // base path is setted to: MainProject.Controllers. Then each controller define a custom route

        // WRAP REGISTER CONTROLLERS
        if (true)
        {
            // Register controllers
            builder.Services.AddControllers(m =>
            {
                // m.UseCoreIntegrated(); // Soporta Clases y tipos de datos de SnapObject.Data (DynamicModel, IDataUnpacker).
                // m.UsePowerBuilderIntegrated(); // Permite integrar DataWindow JSON
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null; // set null on non received properties json
            });
        }


        // WRAP ENABLE [RAZORPAGES 1/3] - [LOGIN FORM MVC 1/4]
        if (true)
        {            
            builder.Services.AddControllersWithViews(); // Añadir soporte para controladores con vistas
            builder.Services.AddRazorPages(); // Añadir soporte para Razor Pages
        }


        // WRAP ENABLE [USE-SWAGGER] 1/2
        if (true)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.EnableAnnotations(); // Enable annotations for Swagger

                // Enable support for jwt tokens
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Autorización JWT usando el esquema Bearer. \r\n\r\n Ingrese 'Bearer' [espacio] y luego su token en el campo de texto abajo.\r\n\r\nEjemplo: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });                
            });
        }


        // WRAP CUSTOM SERVICES
        if (true)
        {
            builder.Services.AddHttpContextAccessor(); // Allow calls to httpContext inside Services            

            // BEARER + COOKIE
            if (true)
            {
                // MIDDLEWARE PARA PROCESAR BEARER TOKEN - // Hora actual es seteada en UTC. Para colombia es -5UTC
                builder.Services.AddAuthentication(options =>
                {
                    // Por defecto jwt, las cookies las validaremos con un helper
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;                    
                })
                .AddJwtBearer(options =>
                {
                    //options.RequireHttpsMetadata = false;
                    //options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true, // para validar las variables de entorno appsettings.json
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,

                        // if token.expires is not defined. then set false [ValidateLifetime = false]
                        ValidateLifetime = true, // false because we dont want expiration                
                        ClockSkew = TimeSpan.Zero, // Super requerido para que funcione la expiracion


                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            // Check if the authentication failure is due to the token being expired
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.StatusCode = 401; // Unauthorized (401)
                                context.Response.Headers.Add("Token-Expired", "true");
                                // Set an item in HttpContext.Items to indicate unauthorized access

                                context.HttpContext.Items["UnauthorizedAccess"] = true;
                            }

                            // Check if the authentication failure is due to the token is not valid
                            if (context.Exception.GetType() == typeof(SecurityTokenSignatureKeyNotFoundException))
                            {
                                context.Response.StatusCode = 401; // Unauthorized (401)
                                context.Response.Headers.Add("Token-Signature-Not-Found", "true");
                                context.HttpContext.Items["SignatureNotFound"] = true;
                            }
                            return Task.CompletedTask;
                        }
                    };
                })
                .AddCookie(options =>
                {
                    options.Cookie.Name = "kswgbnmaestras"; // Establece el nombre de la cookie aquí                    
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(10); // Duración de la cookie valor default
                    options.SlidingExpiration = true; // si es true, la cookie sera renovada si usuario interactua antes de expiracion
                });                
            }

            builder.Services.AddScoped<ICustomJwt, CustomJwtService>(); // Emitir de tokens
            builder.Services.AddScoped<ICustomHelper, CustomHelperService>(); // Funciones utilitarias - Csharp        
            builder.Services.AddScoped<ICustomTokenCache, CustomTokenCacheService>(); // Cachear tokens            

            // Maestras disponibles en este api
            builder.Services.AddScoped<ICustomClientes, CustomClientesService>(); // status api - version published
            builder.Services.AddScoped<ICustomTiposNegocio, CustomTiposNegocioService>();
            builder.Services.AddScoped<ICustomDepartamentos, CustomDepartamentosService>(); // status api - version published
            builder.Services.AddScoped<ICustomMunicipios, CustomMunicipiosService>(); // status api - version published
            builder.Services.AddScoped<ICmUsuariosMovil, CmUsuariosMovilService>(); // status api - version published
            builder.Services.AddScoped<ICmLineasProductos, CmLineasProductosService>(); // status api - version published
            builder.Services.AddScoped<ICmProductos, CmProductosService>(); // status api - version published
            builder.Services.AddScoped<ICmMotivosNc, CmMotivosNcService>(); // status api - version published
            builder.Services.AddScoped<ICmZonasBio, CmZonasBioService>(); // status api - version published

            builder.Services.AddScoped<ICustomHelperMaestras, CustomHelperMaestrasService>(); // status api - version published

            // Habilitamos Swagger views
            builder.Services.AddScoped<ICustomSwagger, CustomSwaggerService>(); // status api - version published

            // Interfaz Custom Database (Transaccional) y Servicio segun bd: En este caso Sybase
            builder.Services.AddScoped<IBionegociosDatabaseInput, CustomSybaseService>();             

            builder.Services.AddScoped<ICurrentProject, CurrentProjectService>(); // status api - version published            
        }

        // WRAP CREATE DATACONTEXT FROM PRIMARY DATABASE
        if (true)
        {
            WsConfiguracionesDbJsonModel stdDbInputRes = new();
            string ls_input_db_connection = ""; // Dsn=ODBC64-PROJECT;Server=192.168.1.27;Port=5000;Database=test_incusan;uid=customuser;pwd=custompassword            
            if (true)
            {
                // Default initial values from appsettings.json
                WsConfiguracionesDbJsonModel stdDbInput = new()
                {
                    Id = builder.Configuration.GetValue<string>("CustomAseDbConfig:DatabaseInputOuput:Id"),
                    Odbc_name = builder.Configuration.GetValue<string>("CustomAseDbConfig:DatabaseInputOuput:Odbc_name"),
                    Host = builder.Configuration.GetValue<string>("CustomAseDbConfig:DatabaseInputOuput:Host"),
                    Port = builder.Configuration.GetValue<string>("CustomAseDbConfig:DatabaseInputOuput:Port"),
                    Db_name = builder.Configuration.GetValue<string>("CustomAseDbConfig:DatabaseInputOuput:Db_name"),
                    User = builder.Configuration.GetValue<string>("CustomAseDbConfig:DatabaseInputOuput:User"),
                    Password = builder.Configuration.GetValue<string>("CustomAseDbConfig:DatabaseInputOuput:Password")
                };
                //Console.WriteLine("raw params from configjson");
                //Console.WriteLine(JsonSerializer.Serialize(stdDbInput));


                // SI O SI, DEBEMOS SETEAR LA CADENA DE CONEXION A SYBASE ASE
                stdDbInputRes = StaticReadStringConnectionFromPrimaryDB(builder, stdDbInput);
                ls_input_db_connection = $"Dsn={stdDbInputRes.Odbc_name};Server={stdDbInputRes.Host};Port={stdDbInputRes.Port};Database={stdDbInputRes.Db_name};uid={stdDbInputRes.User};pwd={stdDbInputRes.Password};";

                // Log primary odbc connection
                if (lb_enable_programcs_info_console_log)
                {
                    string ls_primary_background_db_connection = builder.Configuration.GetConnectionString("ODBC64-INCUSANWS-MASTER") ?? "Valor para la propiedad ODBC64-INCUSANWS-MASTER is not configured in appsettingsjson.";

                    Console.WriteLine("PRIMARY odbc: ");
                    Console.WriteLine(ls_primary_background_db_connection + " // Review inside windows odbc el nombre del servidor..." + "\n");
                }

                // Dev mode console log - primary string connection
                if (lb_enable_programcs_info_console_log)
                {
                    bool lb_enable_read_credentials_from_db = builder.Configuration.GetValue<bool>("CustomApp:EnableRewriteCredentialsFromToDatabase");
                    if (lb_enable_read_credentials_from_db)
                    {
                        Console.WriteLine($"{ls_current_environment}: string connection FINALLY from DATABASE ws_config  to [input/output sybase ase] es: ");
                        Console.WriteLine(ls_input_db_connection + "\n");
                    }
                    else
                    {
                        Console.WriteLine($"{ls_current_environment}: string connection FINALLY from APPSETTINGS to [input/output sybase ase] es: ");
                        Console.WriteLine(ls_input_db_connection + "\n");
                    }
                }                

            }

            builder.Services.AddDataContext<ODBCBionegociosDataContext>(m => m.UseAse(ls_input_db_connection));
        }

        // Main plugin for cache
        builder.Services.AddMemoryCache(); // this line must be after ICustomTokenCache - and is Used in jwt


        // IDIOMAS
        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(_ls_default_language)
            .AddSupportedCultures(_la_languages)
            .AddSupportedUICultures(_la_languages);
        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(_ls_default_language);
            options.SupportedCultures = _la_cultureInfoList;
            options.SupportedUICultures = _la_cultureInfoList;
        });
        builder.Services.AddSingleton(localizationOptions);


        // End helper Con-figure-App
    }





    // another static function helper
    public static string ReadInputConnectionFromPrimaryDB(WebApplicationBuilder builder)
    {
        // Crearemos un objeto tipo std con credenciales de bd. Si no llega el valor, se inicializa con un objeto convertido leido de JSON config.        
        //
        
        string ls_input_db_connection = "";                

        // Default initial values from appsettings.json
        WsConfiguracionesDbJsonModel stdDbInput = new()
        {
            Id = builder.Configuration.GetValue<string>("CustomDbInfoConfig:DatabaseInputOuput:Id"),
            Odbc_name = builder.Configuration.GetValue<string>("CustomDbInfoConfig:DatabaseInputOuput:Odbc_name"),
            Host = builder.Configuration.GetValue<string>("CustomDbInfoConfig:DatabaseInputOuput:Host"),
            Port = builder.Configuration.GetValue<string>("CustomDbInfoConfig:DatabaseInputOuput:Port"),
            Db_name = builder.Configuration.GetValue<string>("CustomDbInfoConfig:DatabaseInputOuput:Db_name"),
            User = builder.Configuration.GetValue<string>("CustomDbInfoConfig:DatabaseInputOuput:User"),
            Password = builder.Configuration.GetValue<string>("CustomDbInfoConfig:DatabaseInputOuput:Password")            
        };        

        // Reescribir de DB
        if (true)
        {
            // Get odbc credentials for current project
            string ls_desired_query = $@"
                select 
                    top 1 
                    id, nombre_odbc, ip_servidor, puerto_servidor, nombre_base_dato, usr_base_dato, dba.DecryptText(password_base_dato) AS password_base_dato, aplicacion_id, motivo_conexion, fecha_registro, activo
                from dba.ws_configuraciones
                where 
                    id={stdDbInput.Id}                  
                    and activo=1
            ";
            ObjFnResConsultarResult dbInputDbSearchResult = PrimaryBdConsultar( builder, ls_desired_query);

            // Error de conexion
            if (dbInputDbSearchResult.Success == false) Console.WriteLine("Algo ocurrio a la hora de consultar... :" + dbInputDbSearchResult.Message, LineNumberInline());


            // error record notfound
            if (dbInputDbSearchResult.Success && dbInputDbSearchResult.Data.Count() == 0) Console.WriteLine("Nit no encontrado en la bd. " + dbInputDbSearchResult.Message, LineNumberInline());            


            // BdConsultar() was ok then take values from db
            if (dbInputDbSearchResult.Success && dbInputDbSearchResult.Data.Count() > 0)
            {
                stdDbInput = dbInputDbSearchResult.Data[0];
                Console.WriteLine("CONSULTAR RETORNO ALGO:");
                Console.WriteLine(JsonSerializer.Serialize(stdDbInput));
            }
        }
              
        return ls_input_db_connection = $"Dsn={stdDbInput.Odbc_name};Server={stdDbInput.Host};Port={stdDbInput.Port};Database={stdDbInput.Db_name};uid={stdDbInput.User};pwd={stdDbInput.Password};";
    }


    // inline object dbjsonmodel
    public class WsConfiguracionesDbJsonModel
    {        
        // =====
        // NOTE: WE CUSTOM USE STRINGS FOR JSON VALUES
        // ====

        public string? Id { get; set; } = null;
        public string? Odbc_name { get; set; } = null;
        public string? Host { get; set; } = null;
        public string? Port { get; set; } = null;
        public string? Db_name { get; set; } = null;
        public string? User { get; set; } = null;
        public string? Password { get; set; } = null;
        
        public string? ApplicationId { get; set; } = null;
        public string? Reason_connection { get; set; } = null;        
    }


    // inline object dbjsonmodel
    public class ObjFnResConsultarResult
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public List<WsConfiguracionesDbJsonModel> Data { get; set; } = new List<WsConfiguracionesDbJsonModel>();
    }


    // another static function helper
    public static ObjFnResConsultarResult PrimaryBdConsultar(WebApplicationBuilder builder, string ls_query)
    {
        ObjFnResConsultarResult objFnRes = new ObjFnResConsultarResult()
        {
            Success = false,
            Message = "",
            Data = new List<WsConfiguracionesDbJsonModel>()
        };

        WsConfiguracionesDbJsonModel rowModelJsn = new();

        string lsFnPropertyName = "";
        string lsFnErrorMessage = "";

        string _currentServiceNameInLine = "Bionegocios_files_api Program.cs";


        // WRAP RAW QUERY CONNECTION
        if (true)
        {
            // ===== Add services to the container - ODBC DB Context - FIRST FROM SETTINGSJSON
            string ls_primary_background_db_connection = builder.Configuration.GetConnectionString("ODBC64-INCUSANWS-MASTER");

            string connectionString = ls_primary_background_db_connection;

            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    //Console.WriteLine("Connection opened successfully.");

                    //string query = "select top 3 * from dba.usuarios";
                    string query = ls_query;
                    OdbcCommand command = new OdbcCommand(query, connection);

                    using (OdbcDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //Console.WriteLine($"ID: {reader["id"]}, Name: {reader["nombre_usuario"]}, Email: {reader["e_mail1"]}");

                            rowModelJsn.Id                  = reader["id"].ToString();
                            rowModelJsn.Odbc_name           = reader["nombre_odbc"].ToString();
                            rowModelJsn.Host                = reader["ip_servidor"].ToString();
                            rowModelJsn.Port                = reader["puerto_servidor"].ToString();
                            rowModelJsn.Db_name             = reader["nombre_base_dato"].ToString();
                            rowModelJsn.User                = reader["usr_base_dato"].ToString();
                            rowModelJsn.Password            = reader["password_base_dato"].ToString();
                            rowModelJsn.ApplicationId       = reader["aplicacion_id"].ToString();
                            rowModelJsn.Reason_connection   = reader["motivo_conexion"].ToString();
                            
                            objFnRes.Data.Add(rowModelJsn);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("An error occurred: " + ex.Message);

                    // Default error message                    
                    lsFnPropertyName = "No fue posible realizar la consulta: " + ex.Message;
                    lsFnErrorMessage = "||" + _currentServiceNameInLine + "|| Error line number (" + LineNumberInline() + "): " + lsFnPropertyName;

                    objFnRes.Success = false;
                    objFnRes.Message = lsFnErrorMessage;
                    return objFnRes;
                }
                finally
                {
                    connection.Close();
                    //Console.WriteLine("Connection closed.");
                }
            }
        }

        // ROW NOT FOUND
        if (objFnRes.Data.Count() == 0)
        {            
            objFnRes.Message = "||" + _currentServiceNameInLine + "|| Error line number (" + LineNumberInline() + "): Registro no encontrado";
            objFnRes.Success = true;            
        }

        // ROW FOUND
        if (objFnRes.Data.Count() > 0)
        {
            objFnRes.Success = true;            
        }

        return objFnRes;
    }


    // another static function helper
    public static int LineNumberInline([System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
    {
        return lineNumber;
    }


    // another static function helper
    public static WsConfiguracionesDbJsonModel StaticReadStringConnectionFromPrimaryDB(WebApplicationBuilder builder, WsConfiguracionesDbJsonModel stdDbInput)
    {
        // Crearemos un objeto tipo std con credenciales de bd. Si no llega el valor, se inicializa con un objeto convertido leido de JSON config.        
        //
        

        //string ls_input_db_connection = "";
        WsConfiguracionesDbJsonModel stdDbRowRes = new();
        stdDbRowRes = stdDbInput;
        bool lb_enable_read_credentials_from_db = builder.Configuration.GetValue<bool>("CustomApp:EnableRewriteCredentialsFromToDatabase");
        //Console.WriteLine("boolean read from ws configuraciones??");
        //Console.WriteLine(lb_enable_read_credentials_from_db);


        // Default initial values from appsettings.json
        //WsConfiguracionesDbJsonModel stdDbInput = new()
        //{
        //    Id = builder.Configuration.GetValue<string>("CustomAseDbConfig:DatabaseInputOuput:Id"),
        //    Odbc_name = builder.Configuration.GetValue<string>("CustomAseDbConfig:DatabaseInputOuput:Odbc_name"),
        //    Host = builder.Configuration.GetValue<string>("CustomAseDbConfig:DatabaseInputOuput:Host"),
        //    Port = builder.Configuration.GetValue<string>("CustomAseDbConfig:DatabaseInputOuput:Port"),
        //    Db_name = builder.Configuration.GetValue<string>("CustomAseDbConfig:DatabaseInputOuput:Db_name"),
        //    User = builder.Configuration.GetValue<string>("CustomAseDbConfig:DatabaseInputOuput:User"),
        //    Password = builder.Configuration.GetValue<string>("CustomAseDbConfig:DatabaseInputOuput:Password")            
        //};
        // Deja comentado lo anterior ya que es un ejemplo de la variable recibida

        // Reescribir de DB
        if (lb_enable_read_credentials_from_db)
        {
            // Get odbc credentials for current project
            string ls_desired_query = $@"
                select 
                    top 1 
                    id, nombre_odbc, ip_servidor, puerto_servidor, nombre_base_dato, usr_base_dato, 
                    dba.DecryptText(password_base_dato) AS password_base_dato, -- Esta funcion debe existir en sybase ase
                    aplicacion_id, motivo_conexion, fecha_registro, activo
                from dba.ws_configuraciones
                where 
                    id={stdDbInput.Id}                  
                    and activo=1
            ";
            ObjFnResConsultarResult dbInputDbSearchResult = PrimaryBdConsultar( builder, ls_desired_query);

            // Error de conexion
            if (dbInputDbSearchResult.Success == false) Console.WriteLine("Algo ocurrio a la hora de consultar... :" + dbInputDbSearchResult.Message, LineNumberInline());


            // error record notfound
            if (dbInputDbSearchResult.Success && dbInputDbSearchResult.Data.Count() == 0) Console.WriteLine("Registro odbc no encontrado en la bd. " + dbInputDbSearchResult.Message, LineNumberInline());            


            // BdConsultar() was ok then take values from db
            if (dbInputDbSearchResult.Success && dbInputDbSearchResult.Data.Count() > 0)
            {
                stdDbRowRes = dbInputDbSearchResult.Data[0];
                //Console.WriteLine("CONSULTAR RETORNO ALGO:");
                //Console.WriteLine(JsonSerializer.Serialize(stdDbInput));
            }
        }

        //Console.WriteLine("raw params from configdatabase");
        //Console.WriteLine(JsonSerializer.Serialize(stdDbRowRes));

        //return ls_input_db_connection = $"Dsn={stdDbInput.Odbc_name};Server={stdDbInput.Host};Port={stdDbInput.Port};Database={stdDbInput.Db_name};uid={stdDbInput.User};pwd={stdDbInput.Password};";
        return stdDbRowRes;
    }


    //end class Program 
}




