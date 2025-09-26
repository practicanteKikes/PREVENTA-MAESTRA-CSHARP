namespace MainProject.Controllers.CustomJwt.Models.FnRes
{
    public class ObjFnResGetToken
    {        
        // Required
        //
        public string? Access_token { get; set; }
        public string? Token_type { get; set; }

        // Optional - Not required by davivienda
        //         
        public int? Expires_in { get; set; }
        public string? Error { get; set; }
        public string? Error_description { get; set; }
    }
}
