namespace MainProject.Controllers.CustomSwagger.Models.FnRes
{
    public class ObjFnResGetCookieDev
    {
        // Default kikes        
        //
        public bool Ksuccess { get; set; }
        public string? Kmessage { get; set; }
        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<string>? Kdata { get; set; }


        // Required by external provider        
        public int? Expires_in { get; set; }
        public string? Error { get; set; }
        public string? Error_description { get; set; }


        // EXTRA INFO        
        public string Db_name_input { get; set; } = "";
        public string Db_name_output { get; set; } = "";
    }
}
