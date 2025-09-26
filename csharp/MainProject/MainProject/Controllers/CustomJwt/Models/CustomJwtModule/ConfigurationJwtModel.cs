using System.Security.Claims;

namespace MainProject.Controllers.CustomJwt.Models.CustomJwtModule
{
    public class ConfigurationJwtModel
    {
        public string Key { get; set; } = "";
        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";
        public string Subject { get; set; } = "";

        public static dynamic validarToken(ClaimsIdentity identity)
        {
            // DEBUG CODE
            Console.WriteLine($"");
            Console.WriteLine($"Validando identity claims[]");

            foreach (var claim in identity.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }

            if (identity.Claims.Count() > 0)
            {
                Console.WriteLine($"Identity claims[] have been processeced");
            }

            if (identity.Claims.Count() == 0)
            {
                Console.WriteLine($"Identity claims are empty - token not valid");
            }
            // END DEBUG CODE



            try
            {
                // error on invalid token
                if (identity.Claims.Count() == 0)
                {
                    return new
                    {
                        status = false,
                        message = "Por favor verifica si el token que estás enviando  es un token valido.",
                        result = ""
                    };
                }

                // extract data from token
                string? id = null;
                UserJwtModel? current_user_or_null = null;
                Claim? claim = null;

                claim = identity.Claims.FirstOrDefault(x => x.Type == "id");
                if (claim != null)
                {
                    id = claim.Value;   
                }                
                if (id is not null)
                {
                    current_user_or_null = UserJwtModel.DB().Where(table => table.Id == id).FirstOrDefault();
                }

                

                return new
                {
                    status = true,
                    message = "Token validado con exito.",
                    user = current_user_or_null
                };
            }
            catch (Exception e)
            {

                return new
                {
                    status = false,
                    message = "Catch: " + e.Message,
                    result = ""
                };
            }
        }
    }
}
