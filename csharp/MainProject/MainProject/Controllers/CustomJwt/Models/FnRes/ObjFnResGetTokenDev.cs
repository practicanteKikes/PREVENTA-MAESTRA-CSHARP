namespace MainProject.Controllers.CustomJwt.Models.FnRes
{
    public class ObjFnResGetTokenDev : ObjFnResGetToken
    {
        // Extra info
        public string? Db_name_input { get; set; }
        public string? Db_name_output { get; set; }
        

        // Default kikes                
        public string? Dev_token { get; set; }

        public bool Success { get; set; }
        public string? Message { get; set; }

        // Ignored in response
        [System.Text.Json.Serialization.JsonIgnore]
        public List<string>? Data { get; set; }        
    }
}
