using MainProject.StaticServices;
using Microsoft.AspNetCore.Mvc;

namespace MainProject.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class CustomRouteAttribute : RouteAttribute
    {
        public CustomRouteAttribute(string template) : base(AddPrefixToTemplate(template)) { }

        private static string AddPrefixToTemplate(string template)
        {            
            string ls_customized_route = "";
            string ls_route_prefix = "";

            // StaticConfigService.Configuration was initialized in Program.cs ???
            if (StaticConfigService.Configuration is not null)
            {
                var programBuilderConfig = StaticConfigService.Configuration;
                ls_route_prefix = programBuilderConfig.GetValue<string>("CustomApp:RoutePrefix"); // Leemos de appsettings.json
            }

            // Anteponemos prefijo a la ruta
            ls_customized_route = ls_route_prefix + template; // example: "api/" + "[controller]"

            return ls_customized_route;
        }
    }
}
