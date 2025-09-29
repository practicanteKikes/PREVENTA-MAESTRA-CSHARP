using MainProject.Controllers.CustomJwt.Models.CustomJwtModule;
using MainProject.Controllers.CustomJwt.Models.DbString;
using MainProject.Controllers.CustomJwt.Models.FnRes;
using MainProject.Services.CustomDatabaseInput;
using MainProject.Services.CustomHelper;
using MainProject.Services.CustomSybase.Impl;
using MainProject.Services.CustomSybase.Models.LoadDbInfo.FnRes;
using MainProject.Services.CustomSybase.Models.LoadDbInfo.Json;
using MainProject.Services.CustomTokenCache;
using Microsoft.IdentityModel.Tokens;
using PowerScript.Bridge;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace MainProject.Controllers.CustomJwt.Services.Impl
{
    public class CustomJwtService : ICustomJwt
    {
        private readonly string? _currentServiceName;
        private readonly JsonSerializerOptions _currentJsonOptions;
        private IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ODBCBionegociosDataContext _dataContext;
        private readonly ICustomHelper _customHelper;
        private readonly ICustomTokenCache _customTokenCache;
        private readonly IBionegociosDatabaseInput _customSybaseService; // tiene los metodos de la interfaz principal


        // Constructor
        public CustomJwtService(
            IConfiguration argconfiguration,
            IHttpContextAccessor arghttpContextAccessor,
            ODBCBionegociosDataContext argdataContext,
            ICustomHelper argcustomHelperService,
            ICustomTokenCache argcustomTokenCache,
            IBionegociosDatabaseInput argcustomSybaseService
        )
        {
            _customHelper = argcustomHelperService;
            _currentServiceName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name; // set the current class name
            _currentJsonOptions = getJsonSerializeOptions();

            _configuration = argconfiguration;
            _httpContextAccessor = arghttpContextAccessor;
            _dataContext = argdataContext; // se instancia el objeto Data context

            _customTokenCache = argcustomTokenCache;
            _customSybaseService = argcustomSybaseService;
        }
        // Constructor END



        public JsonSerializerOptions getJsonSerializeOptions()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };

            return serializeOptions;
        }

        // Define a custom JsonModel
        public class RootLoginJsonModel
        {
            public string? User { get; set; }
            public string? Password { get; set; }
            public string? Jwt_token { get; set; }            
            public bool DeveloperMode { get; set; } = false;
        }
        // RootLoginJsonModel END

        public class ObjFnCreateJwtRes
        {
            public bool status { get; set; }
            public string? message { get; set; }
            public List<RootLoginJsonModel>? data { get; set; }
        }
        // END ObjFnCreateJwtRes




        public JwtSecurityToken DecodeToken(string token)
        {

            // Create a token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Decode the token
            var jwtToken = tokenHandler.ReadJwtToken(token);

            return jwtToken;
        }


        public class ObjFnReadBearerTokenRes
        {
            public bool status { get; set; }
            public string? message { get; set; }
            public List<string>? data { get; set; }
        }
        public (bool status, string message, List<string> data) ReadBearerToken()
        {
            ObjFnReadBearerTokenRes objFnRes = new ObjFnReadBearerTokenRes()
            {
                status = false,
                message = "",
                data = new List<string>()
            };

            string bearerToken;

            // PARA PROCESAR BEARER TOKEN - recuerda la palabra Bearer vendrá cuando definas en POSTMAN el Header Authorization manualmente
            // Si lo haces en POSTMAN en la pestaña Authorization, no debes poner la palabra Bearer
            HttpContext? httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is not null)
            {
                // Check if the request has the "Authorization" header
                if (httpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                {
                    // Check if the header starts with "Bearer "
                    if (authorizationHeader.ToString().StartsWith("Bearer "))
                    {
                        // Extract the token from the header (removing "Bearer " from the start)
                        bearerToken = authorizationHeader.ToString().Substring("Bearer ".Length).Trim();

                        objFnRes.status = true;
                        objFnRes.data.Add(bearerToken);
                    }
                }
            }

            return (objFnRes.status, objFnRes.message, objFnRes.data);
        }



        public void debugManuallyTokenInConsole()
        {
            HttpContext? httpContext = _httpContextAccessor.HttpContext;

            IIdentity? currentHttpUserIdentity = null;
            if (httpContext is not null)
            {
                currentHttpUserIdentity = httpContext.User.Identity;
            }
            else
            {
                Console.WriteLine($"Can not access to httpContext");
            }
            

            string ls_log = JsonSerializer.Serialize(currentHttpUserIdentity, _currentJsonOptions);
            Console.WriteLine("HttpEntity User Data");
            Console.WriteLine(ls_log);


            // debug code
            string ls_token = "";

            var rsReadBT = ReadBearerToken();
            if (rsReadBT.status && rsReadBT.data.Count() > 0)
            {
                ls_token = rsReadBT.data[0]; // token exist in first element
            }

            Console.WriteLine($"");
            Console.WriteLine($"Try to read manually token...");

            if (ls_token.Length > 0)
            {
                Console.WriteLine($"");
                Console.WriteLine($"Token founded: ");
                Console.WriteLine($"{ls_token}");

                Console.WriteLine($"");
                Console.WriteLine($"Try decoding token: ");
                var decodedToken = DecodeToken(ls_token);

                Console.WriteLine($"");
                Console.WriteLine($"Claims decoded manually from token received:");

                foreach (var claim in decodedToken.Claims)
                {
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
                }
            }
            else
            {
                Console.WriteLine($"Token was not received - is empty");
            }
            // end debug code
        }

        // Define a custom Helper to get LineNumber on Error
        static int LineNumber([System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            return lineNumber;
        }
        // LineNumber END 



        public class ObjFnValidateClaimsRes
        {
            public bool status { get; set; }
            public string? message { get; set; }
            public List<ClaimsIdentity>? data { get; set; }
        }
        public (bool status, string message, List<ClaimsIdentity> data) getMergedIdentityWithClaims()
        {
            ObjFnValidateClaimsRes objFnRes = new ObjFnValidateClaimsRes()
            {
                status = false,
                message = "",
                data = new List<ClaimsIdentity>()
            };

            string errorDescription = "";
            string errorMessage = "";


            // identity example: without token received
            // {
            //     "name": null,
            //     "authenticationType": null,
            //     "isAuthenticated": false
            // }

            // claims example: without token received
            // {
            //     "authenticationType": null,
            //     "isAuthenticated": false,
            //     "actor": null,
            //     "bootstrapContext": null,
            //     "claims": [],
            //     "label": null,
            //     "name": null,
            //     "nameClaimType": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
            //     "roleClaimType": "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            // }


            IIdentity? identity;
            HttpContext? httpContextAccesor = _httpContextAccessor.HttpContext;
            ClaimsIdentity? mergedIdentityWithClaims = new ClaimsIdentity();



            if (httpContextAccesor is not null)
            {
                identity = httpContextAccesor.User.Identity; // here are the http jwt sended claims
                if (identity is not null)
                {
                    string ls_log = JsonSerializer.Serialize(identity, _currentJsonOptions);
                    //Console.WriteLine("");
                    //Console.WriteLine("HttpEntity User Data Before merge");
                    //Console.WriteLine(ls_log);

                    ls_log = JsonSerializer.Serialize(mergedIdentityWithClaims, _currentJsonOptions);
                    //Console.WriteLine("");
                    //Console.WriteLine("mergedIdentityWithClaims Data Before merge");
                    //Console.WriteLine(ls_log);

                    //Console.WriteLine($"");
                    //Console.WriteLine($"Http User Identity is Authenticathed??");

                    if (identity.IsAuthenticated) // los claims solo funcionan con usuario authenticado
                    {
                        //Console.WriteLine($"");
                        //Console.WriteLine($"User Authenticated correctly!!! ...");

                        mergedIdentityWithClaims = identity as ClaimsIdentity; // as (casting) and is like array merge or null on fail
                        if (mergedIdentityWithClaims is not null)
                        {
                            //Console.WriteLine($"");
                            //Console.WriteLine($"toString casted mergetIdentityWithClaims: ");
                            //Console.WriteLine(mergedIdentityWithClaims.Claims.ToString());


                            //// DEBUG CODE
                            //Console.WriteLine($"");
                            //Console.WriteLine($"Triyin to show items on mergedIdentityWithClaims []");

                            //foreach (var claim in mergedIdentityWithClaims.Claims)
                            //{
                            //    Console.WriteLine($"{claim.Type}: {claim.Value}");
                            //}

                            //if (mergedIdentityWithClaims.Claims.Count() > 0)
                            //{
                            //    Console.WriteLine($"mergedIdentityWithClaims[] have been processeced");
                            //}

                            //if (mergedIdentityWithClaims.Claims.Count() == 0)
                            //{
                            //    Console.WriteLine($"mergedIdentityWithClaims[] are empty - token is not valid");
                            //}
                            //// END DEBUG CODE
                            ///

                            //string? ls_claim_id = "";
                            //int li_claim_id = 0;
                            //string lsPropertyName;
                            //string lsPropertyValue;
                            //Claim? claim_id; // because function FirtsOrDefault must be nullable

                            // If claims have elements                            
                            if (mergedIdentityWithClaims.Claims.Count() > 0)
                            {
                                // Claims collected ok - must return now
                                objFnRes.status = true;
                                objFnRes.message = "User is authenticated and claims was collected ok.";
                                objFnRes.data.Add(mergedIdentityWithClaims);
                            }
                            else
                            {
                                errorDescription = "Error: Token User claims can not be collected - please try with another token.";
                                errorMessage = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): " + errorDescription;

                                objFnRes.message = errorMessage;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"");
                        Console.WriteLine($"Http User Identity is NOT Authenticathed... :(");



                        errorDescription = "Error: Http User Identity is NOT Authenticathed :( - Token is NOT VALID - MergingIdentityWithClaim";
                        errorMessage = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): " + errorDescription;

                        objFnRes.message = errorMessage;
                    }
                }
            }

            return (objFnRes.status, objFnRes.message, objFnRes.data);
        }


        public bool tokenHttpUserIsAuthenticated()
        {
            bool lb_http_user_identity_authenticated = false;

            // identity example: without token received
            // {
            //     "name": null,
            //     "authenticationType": null,
            //     "isAuthenticated": false
            // }


            HttpContext? httpContextAccesor = _httpContextAccessor.HttpContext;
            IIdentity? identity;

            if (httpContextAccesor is not null)
            {
                identity = httpContextAccesor.User.Identity;
                if (identity is not null)
                {
                    // Debug jj - only return true or false authenticated
                    //

                    //Console.WriteLine(identity.ToString());
                    //Console.WriteLine(JsonSerializer.Serialize(identity, _currentJsonOptions););
                    //{
                    //"name": null,
                    //"authenticationType": "AuthenticationTypes.Federation",
                    //"isAuthenticated": true
                    //}

                    //

                    if (identity.IsAuthenticated)
                    {
                        lb_http_user_identity_authenticated = true;
                    }
                }
            }

            return lb_http_user_identity_authenticated;
        }



        //
        public class ObjFnEntityUsersRes
        {
            public bool status { get; set; }
            public string? message { get; set; }
            public List<string>? data { get; set; }
        }
        // END ObjFnEntityUsersRes
        public string EntityUsers()
        {
            // ===== DEFINE RESPONSE TEXT ====== //                        
            ObjFnEntityUsersRes tokenStatusRes = new ObjFnEntityUsersRes()
            {
                status = false,
                message = "",
                data = new List<string>()
            };
            string responseJsonText;
            

            // wonderfull code here
            tokenStatusRes.status = true;
            tokenStatusRes.message = "Proceso está corriendo exitosamente: Debemos instalar entity framework y listar usuarios ";


            return responseJsonText = JsonSerializer.Serialize(tokenStatusRes, _currentJsonOptions);
        }
        //




        public class ObjFnTokenStatusRes
        {
            public bool status { get; set; }
            public string? message { get; set; }
            public List<string>? data { get; set; }
        }
        // END ObjFnTokenStatusRes
        public string TokenStatus()
        {
            // ===== DEFINE RESPONSE TEXT ====== //                        
            ObjFnTokenStatusRes tokenStatusRes = new ObjFnTokenStatusRes()
            {
                status = false,
                message = "",
                data = new List<string>()
            };
            string responseJsonText;
            string errorDescription = "";
            string errorMessage = "";


            // Enable for debug token values in console
            //this.debugManuallyTokenInConsole();


            // ERROR on empty token
            string ls_token_received = "";
            var rsReadBT = ReadBearerToken();
            if (rsReadBT.status && rsReadBT.data.Count() > 0)
            {
                ls_token_received = rsReadBT.data[0]; // token exist in first array element
            }
            if (ls_token_received.Length == 0)
            {
                errorDescription = "Error: Token is empty. If you are using Postman please send a jwt_string in headers or Authorization Tab.";
                errorMessage = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): " + errorDescription;

                tokenStatusRes.message = errorMessage;
                return JsonSerializer.Serialize(tokenStatusRes, _currentJsonOptions);
            }

            // ERROR on token_user can not be authenticated
            if (!tokenHttpUserIsAuthenticated()) // calleFROMTokenStatus
            {
                errorDescription = "Error: Http User Identity is NOT Authenticathed :( - Token is NOT VALID";
                errorMessage = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): " + errorDescription;

                tokenStatusRes.message = errorMessage;
                return JsonSerializer.Serialize(tokenStatusRes, _currentJsonOptions);
            }

            // ERROR ON EMPTY CLAIMS
            ClaimsIdentity mergedIdentityWithClaims = new ClaimsIdentity();
            var rsGetIdentityWithClaims = getMergedIdentityWithClaims(); // 1
            if (!rsGetIdentityWithClaims.status)
            {
                tokenStatusRes.message = rsGetIdentityWithClaims.message;
                return JsonSerializer.Serialize(tokenStatusRes, _currentJsonOptions);
            }

            // TOKEN IS OK - WE MUST LOOK FOR PROPERTY ID - AND USERID PERMISSIONS
            if (rsGetIdentityWithClaims.status && rsGetIdentityWithClaims.data.Count() > 0)
            {
                mergedIdentityWithClaims = rsGetIdentityWithClaims.data[0];

                string ls_user_id = "";
                Claim? claim_id; // because function FirtsOrDefault must be nullable
                string? ls_claim_id = "";
                string lsPropertyName;
                string lsPropertyValue;
                int li_claim_id = 0;

                claim_id = mergedIdentityWithClaims.Claims.FirstOrDefault(objClaim => objClaim.Type == "id");

                // ERRROR ID PROPERTY NOT FOUND
                if (claim_id is null)
                {
                    errorDescription = "Error: Token is valid, but claim property id was not found. User model can not be consulted.";
                    errorMessage = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): " + errorDescription;

                    tokenStatusRes.message = errorMessage;
                    return JsonSerializer.Serialize(tokenStatusRes, _currentJsonOptions);
                }

                if (claim_id != null)
                {
                    ls_claim_id = claim_id.Value; // property founded

                    // validate id greater than zero
                    lsPropertyName = "ls_claim_id";
                    lsPropertyValue = ls_claim_id!.ToString();
                    try
                    {
                        li_claim_id = Convert.ToInt32(ls_claim_id);
                    }
                    catch (FormatException)
                    {
                        errorMessage = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): jsonStringProperty value(" + lsPropertyValue + ") conversion failed: Convert.ToInt32(" + lsPropertyName + ")";

                        tokenStatusRes.message = errorMessage;
                    }

                    // ERRROR ID PROPERTY IS LESS THAN ZERO
                    if (li_claim_id <= 0)
                    {
                        errorDescription = "Error: Token is valid, but claim property id is not greater than zero. User model can not be consulted.";
                        errorMessage = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): " + errorDescription;

                        tokenStatusRes.message = errorMessage;
                        return JsonSerializer.Serialize(tokenStatusRes, _currentJsonOptions);
                    }

                    if (li_claim_id > 0)
                    {
                        ls_user_id = ls_claim_id;

                        // get user model
                        UserJwtModel? current_user_or_null = UserJwtModel.DB().Where(table => table.Id == ls_user_id).FirstOrDefault();

                        // ERRROR USER NOT FOUND
                        if (current_user_or_null is null)
                        {
                            errorDescription = "Error: Token is valid, User model id can not be consulted. : id=" + ls_user_id;
                            errorMessage = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): " + errorDescription;

                            tokenStatusRes.message = errorMessage;
                            return JsonSerializer.Serialize(tokenStatusRes, _currentJsonOptions);
                        }

                        if (current_user_or_null is not null)
                        {

                            // ERRROR when current userModel don have permissions
                            if (current_user_or_null.Rol != "administrador")
                            {
                                errorDescription = "Error: Token is valid, but user dont have permissions to run this endpoint.";
                                errorMessage = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): " + errorDescription;

                                tokenStatusRes.message = errorMessage;
                                return JsonSerializer.Serialize(tokenStatusRes, _currentJsonOptions);
                            }

                            // TOKEN, USER, AND, PERMISSIONS ARE FINE
                            tokenStatusRes.status = true;
                            tokenStatusRes.message = "Proceso está corriendo exitosamente: Token is valid and user have correct permissions to run this enpoint. ";
                        }
                        // used was FOUND.                        
                    }
                    // li_claim_id IS GREATER THAN ZERO
                }
                // claim_id IS FINE
            }
            // token IS VALID

            return responseJsonText = JsonSerializer.Serialize(tokenStatusRes, _currentJsonOptions);
        }

        public string TokenCreate(string ls_json)
        {
            // ===== DEFINE RESPONSE TEXT ====== //                        
            ObjFnCreateJwtRes createJwtRes = new ObjFnCreateJwtRes()
            {
                status = false,
                message = "",
                data = new List<RootLoginJsonModel>()
            };
            string responseJsonText;
            // ===== END DEFINE RESPONSE TEXT ====== //                        


            // ===== DEFINE LOCAL VARS ====== //                        
            string errorMessage = "";
            List<string> errorList = new List<string>();
            string allErrorsText = "";

            // ===== JSON START PROCESS ====== //         
            RootLoginJsonModel jsonObjectReceived; // Force to be an object
            RootLoginJsonModel? rootLoginJsonModelOrNull = null; // necesary variable to try catch to cast postman json object

            Console.WriteLine("Triying to parse postman json");
            try
            {
                rootLoginJsonModelOrNull = JsonSerializer.Deserialize<RootLoginJsonModel>(ls_json, _currentJsonOptions);
            }
            catch (JsonException e)
            {
                errorMessage = $"Error in received object JSON: {e.Message}";
                Console.WriteLine(errorMessage);
                errorList.Add(errorMessage);

                createJwtRes.status = false;
            }




            jsonObjectReceived = rootLoginJsonModelOrNull ?? new RootLoginJsonModel();
            // do the stuff
            string ls_user = (jsonObjectReceived.User ?? "").ToString();
            string ls_password = (jsonObjectReceived.Password ?? "").ToString();

            // FirstOrDefault may return null if where not work
            UserJwtModel? current_user_or_null = UserJwtModel.DB().Where(currentRow => currentRow.User == ls_user && currentRow.Password == ls_password).FirstOrDefault();
            UserJwtModel currentUserJwt = current_user_or_null ?? new UserJwtModel();

            if (current_user_or_null is null)
            {
                errorMessage = $"user with credentials not found - ";
                errorMessage += "||" + _currentServiceName + "|| Error line number (" + LineNumber() + ")";
                Console.WriteLine(errorMessage);
                errorList.Add(errorMessage);

                createJwtRes.status = false;
            }
            else
            {
                currentUserJwt = current_user_or_null;
            }


            // accedemos a una propiedad mediante fullpath
            //var jwt = _configuration.GetSection("Jwt"); // obtenemos Jwt property from appsettings.json

            // cast to object
            ConfigurationJwtModel jwt = _configuration.GetSection("Jwt").Get<ConfigurationJwtModel>();

            // define a list of custom data we want to encode
            var claims = new[]
            {
                // set some config values default
                new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),

                // customs data
                new Claim("id", currentUserJwt.Id ?? ""),
                new Claim("email", currentUserJwt.Email ?? ""),
            };

            // Encriptamos la key en bytes
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            // Generamos un inicio de sesion
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                jwt.Issuer,
                jwt.Audience,
                claims,

                // Hide next var for NO expiration
                //expires: DateTime.UtcNow.AddMinutes(10), // "exp": linuxtime - 10 min de expiración

                signingCredentials: signIn
            );



            string ls_token = new JwtSecurityTokenHandler().WriteToken(token);

            var decodedToken = DecodeToken(ls_token);

            // Accessing claims from the decoded token
            foreach (var claim in decodedToken.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }

            jsonObjectReceived.Jwt_token = ls_token;
            // end doing stuff




            Console.WriteLine("Validating Error List");
            // RESPONSE ERROR CONTROL RETURN
            if (errorList.Count > 0)
            {
                // ERROR                
                allErrorsText = string.Join(" - ", errorList);

                createJwtRes.status = false;
                createJwtRes.message = allErrorsText;
            }
            else
            {
                Console.WriteLine("Everithing was OK - Returning response");

                // EVERYTHING OK                
                createJwtRes.status = true;
                createJwtRes.message = "¡Proceso finalizado con exito! Token generado correctamente.";
                createJwtRes.data.Add(jsonObjectReceived);
            }

            return responseJsonText = JsonSerializer.Serialize(createJwtRes, _currentJsonOptions);
        }
        // END CreateJsonWebToken




        public class ObjFnResGetUserJwtById
        {
            public bool status { get; set; }
            public string? message { get; set; }
            public List<UserJwtDbStringModel>? data { get; set; }
        }

        public (bool status, string message, List<UserJwtDbStringModel> data) getUserJwtById(string lsUserId)
        {
            ObjFnResGetUserJwtById objFnRes = new ObjFnResGetUserJwtById()
            {
                status = false,
                message = "",
                data = new List<UserJwtDbStringModel>()
            };


            string sql = "select 1";
            string lsFnPropertyName = "";
            string ls_id_usuario = lsUserId;




            // sql columns return
            UserJwtDbStringModel currentUser = new();

            sql = @"
                SELECT
	                id_usuario,
	                nombre_usuario,
	                password_usuario,			
	                e_mail1	,
	                id_grupo,
	                estado
                FROM dba.usuarios
                WHERE dba.usuarios.id_usuario = @ls_id_usuario ;
            ";

            //            
            SqlContext sqlContext;
            sqlContext = _dataContext.CreateSqlContext(sql);
            int li_simple_counter = -1;

            sqlContext.SetParm(++li_simple_counter, ls_id_usuario); // JAGG

            var result = _dataContext.SqlExecutor.SelectOne(sqlContext.SqlText, out SqlResult sqlResultObject, sqlContext.Parms);

            switch (sqlResultObject.SqlCode)
            {
                case -1: // sql query error                                        
                    lsFnPropertyName = "No fue posible consultar los datos del id_usuario: " + ls_id_usuario;
                    objFnRes.message = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): " + lsFnPropertyName + ".  Mensaje: " + sqlResultObject.ErrorText.ToString();
                    break;

                case 0: // sql query is OK                                        
                    objFnRes.status = true;

                    currentUser.Id = result.GetValue<string>("id_usuario");
                    currentUser.User = result.GetValue<string>("nombre_usuario");
                    currentUser.Password = result.GetValue<string>("password_usuario");
                    currentUser.Email = result.GetValue<string>("e_mail1");
                    currentUser.Rol = result.GetValue<string>("id_grupo");
                    currentUser.Estado = result.GetValue<string>("estado");


                    objFnRes.data.Add(currentUser);
                    break;

                case 100: // sql record not found                                        
                    lsFnPropertyName = lsUserId;
                    objFnRes.message = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): El el proveedor  no está creado. (" + lsFnPropertyName + ")";
                    break;

                default: // set error by default
                    lsFnPropertyName = "dba.ap_acreedores.cod_acreedor = '" + lsUserId + "'";
                    objFnRes.message = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): Table (" + lsFnPropertyName + ") can not be consulted.";
                    break;
            }


            return (objFnRes.status, objFnRes.message, objFnRes.data);
        }


        public string TokenStatusDatabase()
        {
            // ===== DEFINE RESPONSE TEXT ====== //                        
            ObjFnResTokenStatusDatabase tokenStatusRes = new()
            {
                Success = false,
                Message = "",
                Data = new List<string>()
            };
            string responseJsonText;
            string errorDescription = "";
            string errorMessage = "";


            // Enable for debug token values in console
            //this.debugManuallyTokenInConsole();


            // ERROR on empty token
            string ls_token_received = "";
            var rsReadBT = ReadBearerToken();
            HttpContext? httpContext = _httpContextAccessor.HttpContext;
            
            if (rsReadBT.status && rsReadBT.data.Count() > 0)
            {
                ls_token_received = rsReadBT.data[0]; // token exist in first array element
            }
            if (ls_token_received.Length == 0)
            {
                if (httpContext is not null)
                {
                    httpContext.Response.StatusCode = 401; // Unauthorized (401)
                }
                
                errorDescription = "Error: Token is empty. If you are using Postman please send a jwt_string in headers or Authorization Tab.";
                errorMessage = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): " + errorDescription;

                tokenStatusRes.Message = errorMessage;
                return JsonSerializer.Serialize(tokenStatusRes, _currentJsonOptions);
            }

            // ERROR GENERAL! on token_user can not be authenticated
            if (!tokenHttpUserIsAuthenticated()) // calleFROMTokenStatusDatabase
            {
                //_customHelper.consoleLog("current scode: "+ _httpContextAccessor.HttpContext.Response.StatusCode);

                // DETECTAMOS SI ES TOKEN EXPIRADO GRACIAS A LOS EVENTOS EN PROGRAM.CS
                
                if (httpContext is not null)
                {
                    //Console.WriteLine("||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): " + "status: " + httpContext.Response.StatusCode);

                    bool unauthorizedAccess = httpContext.Items.ContainsKey("UnauthorizedAccess") &&
                                  (bool)httpContext.Items["UnauthorizedAccess"]!;

                    bool signatureNotFound = httpContext.Items.ContainsKey("SignatureNotFound") &&
                                  (bool)httpContext.Items["SignatureNotFound"]!;

                    int li_middlewarejwt_http_scode = httpContext.Response.StatusCode;
                    if (li_middlewarejwt_http_scode == (int)HttpStatusCode.Unauthorized
                            && unauthorizedAccess
                    )
                    {
                        errorDescription = "Error: Unauthorized. Token is expired.";
                        errorMessage = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): " + errorDescription;

                        tokenStatusRes.Message = errorMessage;
                        return JsonSerializer.Serialize(tokenStatusRes, _currentJsonOptions);
                    }
                }


                if (httpContext is not null)
                {
                    httpContext.Response.StatusCode = 401; // Unauthorized (401)
                }
                errorDescription = "Error: Http User Identity is NOT Authenticathed :( - Token is NOT VALID";
                errorMessage = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): " + errorDescription;

                tokenStatusRes.Message = errorMessage;
                return JsonSerializer.Serialize(tokenStatusRes, _currentJsonOptions);
            }

            // ERROR ON EMPTY CLAIMS
            ClaimsIdentity mergedIdentityWithClaims = new ClaimsIdentity();
            var rsGetIdentityWithClaims = getMergedIdentityWithClaims(); // calledFromTokenStatusDatabase
            if (!rsGetIdentityWithClaims.status)
            {
                tokenStatusRes.Message = rsGetIdentityWithClaims.message;
                return JsonSerializer.Serialize(tokenStatusRes, _currentJsonOptions);
            }

            // TOKEN IS OK - WE MUST LOOK FOR PROPERTY ID - AND USERID PERMISSIONS
            if (rsGetIdentityWithClaims.status && rsGetIdentityWithClaims.data.Count() > 0)
            {
                mergedIdentityWithClaims = rsGetIdentityWithClaims.data[0];

                string ls_user_id = "";
                Claim? claim_id; // because function FirtsOrDefault must be nullable
                string? ls_claim_id = "";
                //string lsPropertyName;
                //string lsPropertyValue;
                //int li_claim_id = 0;

                claim_id = mergedIdentityWithClaims.Claims.FirstOrDefault(objClaim => objClaim.Type == "id");

                // ERRROR ID PROPERTY NOT FOUND
                if (claim_id is null)
                {
                    errorDescription = "Error: Token is valid, but claim property id was not found. User model can not be consulted.";
                    errorMessage = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): " + errorDescription;

                    tokenStatusRes.Message = errorMessage;
                    return JsonSerializer.Serialize(tokenStatusRes, _currentJsonOptions);
                }

                if (claim_id != null)
                {
                    ls_claim_id = claim_id.Value; // property founded                    

                    if (/*li_claim_id > 0*/ true)
                    {
                        ls_user_id = ls_claim_id;

                        // get user from database
                        UserJwtDbStringModel userJwtDbStringM = new(); // JsonModel properties are strings or null                        
                        var userJwtDbSearchResult = getUserJwtById(ls_user_id);
                        if (userJwtDbSearchResult.status && userJwtDbSearchResult.data.Count() > 0)
                        {
                            userJwtDbStringM = userJwtDbSearchResult.data[0];
                        }
                        // if (userJwtDbStringM.isActive())

                        // ERRROR USER NOT FOUND
                        if (userJwtDbSearchResult.status == false)
                        {
                            errorDescription = "Error: Token is valid, User model id can not be consulted. : id=" + ls_user_id;
                            errorMessage = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): " + errorDescription;

                            tokenStatusRes.Message = errorMessage;
                            return JsonSerializer.Serialize(tokenStatusRes, _currentJsonOptions);
                        }

                        if (userJwtDbStringM.IsActive())
                        {
                            // ERRROR when current userModel don have permissions
                            string LS_ROL_ADMIN = "478"; // Por ahora defino que 478 va a ser el rol administrador
                            if (userJwtDbStringM.Rol != LS_ROL_ADMIN)
                            {
                                errorDescription = "Error: Token is valid, but user dont have permissions to run this endpoint.";
                                errorMessage = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): " + errorDescription;

                                tokenStatusRes.Message = errorMessage;
                                return JsonSerializer.Serialize(tokenStatusRes, _currentJsonOptions);
                            }

                            // TOKEN, USER, AND, PERMISSIONS ARE FINE
                            tokenStatusRes.Success = true;
                            tokenStatusRes.Message = "Proceso está corriendo exitosamente: Token is valid and user have correct permissions to run this enpoint. ";
                        }
                        // used was FOUND.                        
                    }
                    // li_claim_id IS GREATER THAN ZERO
                }
                // claim_id IS FINE
            }
            // token IS VALID

            return responseJsonText = JsonSerializer.Serialize(tokenStatusRes, _currentJsonOptions);
        }




        ///
        /// OBTENDREMOS ROW USER BY USER AND PASSWORD
        /// 
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

        public class ObjFnResGetUserJwtByUserAndPassword
        {
            public bool status { get; set; }
            public string? message { get; set; }
            public List<UserJwtDbStringModel>? data { get; set; }
        }

        public (bool status, string message, List<UserJwtDbStringModel> data) getUserJwtByUserAndPassword(Dictionary<string, object> la_params)
        {
            ObjFnResGetUserJwtByUserAndPassword objFnRes = new ObjFnResGetUserJwtByUserAndPassword()
            {
                status = false,
                message = "",
                data = new List<UserJwtDbStringModel>()
            };

            // define an array asociative ( csharp dictionary ) - default
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "ls_user", "username" },
                { "ls_password", "some password" },
                { "third_param", "another value" }
            };
            Dictionary<string, object> ldictionary_mergedResult = MergeArrays(la_params_default, la_params);

            //Console.WriteLine(_currentServiceName + ": " + " | Line: " + _customHelper.LineNumber() + " Merged Dictionary:");
            foreach (var kvp in ldictionary_mergedResult)
            {
                //Console.WriteLine($"{kvp.Key} => {kvp.Value}");
            }


            // Asignamos parametros del array mergeado (realmente Diccionario merged)
            string ls_user = "";
            string ls_password = "";

            if (ldictionary_mergedResult.TryGetValue("ls_user", out object? ls_user_aux) && ls_user_aux is string)
            {
                ls_user = (string)ls_user_aux;
            }

            if (ldictionary_mergedResult.TryGetValue("ls_password", out object? ls_password_aux) && ls_password_aux is string)
            {
                ls_password = (string)ls_password_aux;
            }



            string sql = "select 1";
            string lsFnPropertyName = "";
            string ls_nombre_usuario = ls_user;
            string ls_password_usuario = ls_password;




            // sql columns return
            UserJwtDbStringModel currentUser = new();

            // Inicialmente FROM test_lanza.dba.usuarios. Ahora test_incusan.
            // Segun el ODBC se selecciona la base de datos.
            sql = @"
                SELECT
	                id_usuario,
	                nombre_usuario,
	                password_usuario,			
	                e_mail1	,
	                id_grupo,
	                estado
                FROM dba.usuarios
                WHERE 
                    dba.usuarios.e_mail1 = @ls_nombre_usuario
                    and dba.usuarios.password_usuario = @ls_password_usuario
                ;
            ";

            //            
            SqlContext sqlContext;
            sqlContext = _dataContext.CreateSqlContext(sql);
            int li_simple_counter = -1;

            sqlContext.SetParm(++li_simple_counter, ls_nombre_usuario); // JAGG
            sqlContext.SetParm(++li_simple_counter, ls_password_usuario); // some hard password

            var result = _dataContext.SqlExecutor.SelectOne(sqlContext.SqlText, out SqlResult sqlResultObject, sqlContext.Parms);

            switch (sqlResultObject.SqlCode)
            {
                case -1: // sql query error                                        
                    lsFnPropertyName = $"No fue posible consultar los datos para el usuario: {ls_nombre_usuario} {ls_password_usuario}";
                    objFnRes.message = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): " + lsFnPropertyName + ".  Mensaje: " + sqlResultObject.ErrorText.ToString();
                    break;

                case 0: // sql query is OK                                        
                    objFnRes.status = true;

                    currentUser.Id = result.GetValue<string>("id_usuario");
                    currentUser.User = result.GetValue<string>("nombre_usuario");
                    currentUser.Password = result.GetValue<string>("password_usuario");
                    currentUser.Email = result.GetValue<string>("e_mail1");
                    currentUser.Rol = result.GetValue<string>("id_grupo");
                    currentUser.Estado = result.GetValue<string>("estado");


                    objFnRes.data.Add(currentUser);
                    break;

                case 100: // sql record not found                                        
                    lsFnPropertyName = ls_nombre_usuario;
                    objFnRes.message = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): El el usuario no está creado. (" + lsFnPropertyName + ")";
                    break;

                default: // set error by default
                    lsFnPropertyName = "dba.dba.usuarios.nombre_usuario = '" + ls_nombre_usuario + "'";
                    objFnRes.message = "||" + _currentServiceName + "|| Error line number (" + LineNumber() + "): Table (" + lsFnPropertyName + ") can not be consulted.";
                    break;
            }


            return (objFnRes.status, objFnRes.message, objFnRes.data);
        }
        ///




        public string TokenCreateDatabase(string ls_json)
        {            
            ObjFnResGetTokenDev getTokenResDev = new();
            // Default kikes
            getTokenResDev.Success = false;
            getTokenResDev.Message = "Creating bearer token with basic credentials from database";
            getTokenResDev.Data = new List<string>();

            // Required davivienda
            getTokenResDev.Access_token = null;
            getTokenResDev.Token_type = "bearer";

            getTokenResDev.Expires_in = 3600; //Opcional            
            getTokenResDev.Error = null; // Opcional
            getTokenResDev.Error_description = null; // Opcional

            string responseJsonText;
            // ===== END DEFINE RESPONSE TEXT ====== //                        


            // ===== DEFINE LOCAL VARS ====== //                        
            string errorMessage = "";
            List<string> errorList = new List<string>();
            string allErrorsText = "";
            bool lb_developer_mode = false;

            // developerMode is received??
            JsonDocument jsonObjectReceivedReadMode = _customHelper.CreateJsonObjReadonly(ls_json); // readonly
            if (jsonObjectReceivedReadMode.RootElement.TryGetProperty("developerMode", out JsonElement developerMode))
            {
                if (developerMode.ValueKind == JsonValueKind.True)
                {
                    lb_developer_mode = developerMode.GetBoolean();
                }
            }


            // ===== JSON START PROCESS ====== //         
            RootLoginJsonModel jsonObjectReceived; // Force to be an object
            RootLoginJsonModel? rootLoginJsonModelOrNull = null; // necesary variable to try catch to cast postman json object

            //Console.WriteLine(_currentServiceName + ": " + " | Line: " + _customHelper.LineNumber() + " Triying to parse postman json...");
            try
            {
                rootLoginJsonModelOrNull = JsonSerializer.Deserialize<RootLoginJsonModel>(ls_json, _currentJsonOptions);
            }
            catch (JsonException e)
            {
                errorMessage = $"Error in received object JSON: {e.Message}";
                Console.WriteLine(errorMessage);
                errorList.Add(errorMessage);

                getTokenResDev.Success = false;

                return responseJsonText = this.ResponseFormatTokenCreateDatabase(getTokenResDev, lb_developer_mode);
            }            
            jsonObjectReceived = rootLoginJsonModelOrNull ?? new RootLoginJsonModel();


            // ===== GET DATABASE INFO INPUT
            if (lb_developer_mode)
            {
                DatabaseInfoInputJsonModel dbInfoJsnModelInput = new(); // JsonModel properties are strings or null
                ObjFnResLoadDbInfoInput dbInfoInputSearchResult = _customSybaseService.LoadDbInfo();

                // [0001] - error inesperado en la conexion
                if (dbInfoInputSearchResult.Success == false)
                {
                    string ls_error;
                    //httpContext.Response.StatusCode = 500; // Internal Server Error (500)
                    ls_error = this.KikesDeveloperErrorResponse("Error de conexión a la bd. " + dbInfoInputSearchResult.Message, _customHelper.LineNumber());

                    getTokenResDev.Success = false;
                    getTokenResDev.Message = ls_error;

                    return this.ResponseFormatTokenCreateDatabase(getTokenResDev, lb_developer_mode);
                }

                // RECORD FOUND - DB INFO
                if (dbInfoInputSearchResult.Success && dbInfoInputSearchResult.Data.Count() > 0)
                {
                    dbInfoJsnModelInput = dbInfoInputSearchResult.Data[0];
                }
                {
                    getTokenResDev.Db_name_output = dbInfoJsnModelInput.Db_name ?? "querying db_name return null";
                    //objFnResDev.Db_name_input = objFnResDev.Db_name_input;
                }
            }
            // ===== END GET DATABASE INFO INPUT


            // do the stuff
            string ls_user = (jsonObjectReceived.User ?? "").ToString();
            string ls_password = (jsonObjectReceived.Password ?? "").ToString();


            // RETURN OLD CACHED TOKEN IF EXIST
            string ls_key_user = $"user_{ls_user}_token";
            bool lb_token_must_added_to_cache = false;

            // Obtenemos token de la cache
            if (_customTokenCache.TryGetToken(ls_key_user, out var cachedToken))
            {
                //_customHelper.consoleLog(_currentServiceName + " | Line: " + _customHelper.LineNumber() + " | Token obtained from cache... ^^");

                jsonObjectReceived.Jwt_token = cachedToken;

                getTokenResDev.Success = true;
                getTokenResDev.Access_token = jsonObjectReceived.Jwt_token;
                getTokenResDev.Message = "¡Proceso finalizado con exito! Token obtenido de la cache correctamente.";
                getTokenResDev.Dev_token = "Bearer " + jsonObjectReceived.Jwt_token;

                return this.ResponseFormatTokenCreateDatabase(getTokenResDev, jsonObjectReceived.DeveloperMode);
            }
            else
            {
                //_customHelper.consoleLog(_currentServiceName + " | Line: " + _customHelper.LineNumber() + " | Generating a new wonderful token ...");

                lb_token_must_added_to_cache = true;
            }

            //


            // define an array asociative ( csharp dictionary )
            Dictionary<string, object> la_params = new Dictionary<string, object>
            {
                { "ls_user", ls_user },
                { "ls_password", ls_password }
            };


            // get user from database
            UserJwtDbStringModel userJwtDbStringM = new(); // JsonModel properties are strings or null                        
            var userJwtDbSearchResult = getUserJwtByUserAndPassword(la_params);
            if (userJwtDbSearchResult.status && userJwtDbSearchResult.data.Count() > 0)
            {
                userJwtDbStringM = userJwtDbSearchResult.data[0];

                // ERROR IF INACTIVE
                if (!userJwtDbStringM.IsActive())
                {
                    errorMessage = $"user with credentials is INACTIVE";
                    Console.WriteLine(errorMessage);
                    errorList.Add(errorMessage);

                    getTokenResDev.Success = false;
                }
            }

            // ERRROR NOT FOUND USER
            if (userJwtDbSearchResult.status == false)
            {
                errorMessage = $"database user with credentials not found";

                if (jsonObjectReceived.DeveloperMode)
                {
                    errorMessage += $"||query error: " + userJwtDbSearchResult.message;
                }

                errorMessage += "||" + _currentServiceName + "|| Logger line number (" + LineNumber() + ")";
                Console.WriteLine(errorMessage);
                errorList.Add(errorMessage);

                getTokenResDev.Success = false;
            }


            // LAST VALIDATION - MUST BE ACTIVE
            if (userJwtDbStringM.IsActive())
            {
                // accedemos a una propiedad mediante fullpath
                //var jwt = _configuration.GetSection("Jwt"); // obtenemos Jwt property from appsettings.json

                // cast to object
                ConfigurationJwtModel jwt = _configuration.GetSection("Jwt").Get<ConfigurationJwtModel>();

                // define a list of custom data we want to encode
                var claims = new[]
                {
                    // set some config values default
                    new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),

                    // customs data
                    new Claim("id", userJwtDbStringM.Id ?? ""),
                    new Claim("email", userJwtDbStringM.Email ?? ""),
                };

                // Encriptamos la key en bytes
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
                // Generamos un inicio de sesion
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                int li_token_minutes_to_expire = _configuration.GetValue<int>("Jwt:TokenMinutesToExpire");

                JwtSecurityToken token = new JwtSecurityToken(
                    jwt.Issuer,
                    jwt.Audience,
                    claims,

                    // Hide next var for NO expiration
                    expires: DateTime.UtcNow.AddMinutes(li_token_minutes_to_expire), // "exp": linuxtime - 10 min de expiración

                    signingCredentials: signIn
                );



                string ls_token = new JwtSecurityTokenHandler().WriteToken(token);

                var decodedToken = DecodeToken(ls_token);


                //Console.WriteLine(_currentServiceName + ": " + " | Line: " + _customHelper.LineNumber() + " These are the claims generated");
                // Accessing claims from the decoded token
                foreach (var claim in decodedToken.Claims)
                {
                    //Console.WriteLine($"{claim.Type}: {claim.Value}");
                }

                jsonObjectReceived.Jwt_token = ls_token;

                if (lb_token_must_added_to_cache)
                {
                    int li_token_minutes_to_be_cached = _configuration.GetValue<int>("Jwt:TokenMinutesToBeCached");

                    // Add the new token to the cache
                    _customTokenCache.AddToken(ls_key_user, jsonObjectReceived.Jwt_token, TimeSpan.FromMinutes(li_token_minutes_to_be_cached)); // 10 minutos menos que la expiracion
                }

                // end doing stuff
            }




            //Console.WriteLine("Validating Error List");
            // RESPONSE ERROR CONTROL RETURN
            if (errorList.Count > 0)
            {
                // ERROR                
                allErrorsText = string.Join(" - ", errorList);

                getTokenResDev.Success = false;
                getTokenResDev.Message = allErrorsText;
            }
            else
            {
                //Console.WriteLine("Everithing was OK - Returning response");

                // EVERYTHING OK
                getTokenResDev.Success = true;
                getTokenResDev.Message = "¡Proceso finalizado con exito! Token creado correctamente.";
                getTokenResDev.Access_token = jsonObjectReceived.Jwt_token;
                getTokenResDev.Dev_token = "Bearer " + jsonObjectReceived.Jwt_token;
            }

            
            return this.ResponseFormatTokenCreateDatabase(getTokenResDev, jsonObjectReceived.DeveloperMode);
        }
        // END CreateJsonWebToken

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

        public string ReadUserPasswordFromHeaders()
        {
            ObjFnResGetTokenDev getTokenResFromH = new();
            RootLoginJsonModel simulateBodyParams = new();

            HttpContext? httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is null) return "Can not access to Request Headers";

            if (httpContext.Request.Headers.TryGetValue("developerMode", out var developerModeHeader))
            {
                if ("true" == developerModeHeader.ToString())
                {
                    simulateBodyParams.DeveloperMode = true;
                }
            }

            // Check if the request has the "Authorization" header
            if (httpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                // Check if the header starts with "Basic "
                if (authorizationHeader.ToString().StartsWith("Basic "))
                {
                    // Extract the token from the header (removing "Basic " from the start)
                    string ls_basic_token = authorizationHeader.ToString().Substring("Basic ".Length).Trim();

                    try
                    {
                        // Decode the Base64-encoded credentials to get username and password
                        string credentials = Encoding.UTF8.GetString(Convert.FromBase64String(ls_basic_token));

                        // Split the credentials into username and password
                        string[] credentialsArray = credentials.Split(':');
                        string username = credentialsArray[0];
                        string password = credentialsArray[1];

                        // Now you have the username and password, you can use them as needed                                                                        
                        simulateBodyParams.User = username;
                        simulateBodyParams.Password = password;
                        
                        return JsonSerializer.Serialize(simulateBodyParams, _currentJsonOptions);
                    }
                    catch (Exception)
                    {
                        httpContext.Response.StatusCode = 400; // Bad request (400)
                        getTokenResFromH.Error = "Bad Request";
                        getTokenResFromH.Error_description = "Invalid credentials format.";
                        return JsonSerializer.Serialize(getTokenResFromH, _currentJsonOptions);
                    }
                }
                else
                {
                    httpContext.Response.StatusCode = 400; // Bad request (400)
                    getTokenResFromH.Error = "Bad Request";
                    getTokenResFromH.Error_description = "Authorization header format is not [Basic ]";
                    return JsonSerializer.Serialize(getTokenResFromH, _currentJsonOptions);

                }
            }
            else
            {
                httpContext.Response.StatusCode = 400; // Bad request (400)
                getTokenResFromH.Error = "Bad Request";
                getTokenResFromH.Error_description = "Authorization header is missing.";
                return JsonSerializer.Serialize(getTokenResFromH, _currentJsonOptions);
            }
        }
        // END read

        // Define a custom helper
        private string ResponseFormatTokenCreateDatabase(ObjFnResGetTokenDev stdFnResDev, bool developerMode = false)
        {                        
            if (developerMode)
            {
                return JsonSerializer.Serialize(stdFnResDev, _currentJsonOptions);
            }           

            var objFnResPro = new
            {
                Access_token = stdFnResDev.Access_token,
                Token_type = stdFnResDev.Token_type,
                Expires_in = stdFnResDev.Expires_in,
                Error = stdFnResDev.Error,
                Error_description = stdFnResDev.Error_description                              
            };
            return JsonSerializer.Serialize(objFnResPro, _currentJsonOptions);
        }



    }
}
