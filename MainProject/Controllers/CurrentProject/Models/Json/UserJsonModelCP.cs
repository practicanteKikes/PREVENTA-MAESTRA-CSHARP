namespace MainProject.Controllers.CurrentProject.Models.Json
{
    public class UserJsonModelCP
    {
        // Customizamos lo que queremos devolver
        public int Fake_id { get; set; } = 10;
        public string Nombre_usuario { get; set; } = "";        
        public string? Email { get; set; } = "fakeuser@kikes.com.co";
        public string? Estado { get; set; } = "A";
    }
}
