using MainProject.Controllers.CustomClientes.Services;
using MainProject.Services.CustomHelper.Models.FnRes;
using MainProject.Services.CustomHelper;
using Microsoft.Extensions.Configuration;
using MainProject.Controllers.CustomClientes.Controllers;
using MainProject.Controllers.CustomJwt.Services;
using MainProject.Services.CustomDatabaseInput;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace MainProject.Controllers.CustomHelperMaestras.Services.Impl
{
    public class CustomHelperMaestrasService : ICustomHelperMaestras
    {
        // INJECTED
        private readonly ICustomHelper _customHelper;        
        private IConfiguration _configuration; // Leeremos las tablas o vistas usadas
                                        

        // ========== CONSTRUCTOR ========= //
        public CustomHelperMaestrasService(
            ICustomHelper argcustomHelperService,
            IConfiguration argconfiguration                                    
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
        }


        // Define a custom helper
        public int GetLimitAllowed()
        {
            var httpContext = _customHelper.GetCurrentHttpContext();
            var Request = httpContext.Request;

            // Limitaremos todos los resultados. minimo limit recibido entre 1 y maximo 500. Default 50
            int li_default_limit = _configuration.GetValue<int>("CustomApp:Li_limit_default");

            int li_default_max_limit = _configuration.GetValue<int>("CustomApp:Li_limit_max_allowed"); // 500 or similar            
            int li_limit = li_default_limit; // Default 50 ??

            string ls_limit_or_empty = Request.Query["limit"].ToString(); // StringValues cuando es vacío no da error, y al convertirlo al ToString() resulta en cadena vacía, no 'null'

            ObjFnResCustomConverToInt32 objRs = _customHelper.CustomConvertToInt32("limit", ls_limit_or_empty);
            if (objRs.Status == true)
            {
                if (objRs.Data is not null && objRs.Data.Count() > 0)
                {
                    li_limit = objRs.Data[0]; // Nuevo limite recibido
                }
            }



            // Sí limite es cero o negativo, retornamos 50
            if (li_limit <= 0)
            {
                li_limit = li_default_limit;
            }

            // Si limite es superior al permitido, retornamos solo el permitido
            if (li_limit > li_default_max_limit)
            {
                li_limit = li_default_max_limit;
            }

            return li_limit;
        }




        //ENDCLASS
    }
}
