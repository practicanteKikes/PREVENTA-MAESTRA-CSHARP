namespace MainProject.Controllers.CustomJwt.Models.FnRes
{
    public class ObjFnResTokenStatusDatabase
    {
        // Default kikes        
        public bool Success { get; set; }
        public string? Message { get; set; }

        // Ignored in response
        [System.Text.Json.Serialization.JsonIgnore]
        public List<string>? Data { get; set; }
    }
}
