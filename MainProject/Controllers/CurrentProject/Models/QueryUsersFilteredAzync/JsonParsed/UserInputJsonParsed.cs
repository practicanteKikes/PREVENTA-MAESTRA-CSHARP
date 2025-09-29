using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Controllers.CurrentProject.Models.QueryUsersFilteredAzync.JsonParsed
{
    public class UserInputJsonParsed
    {
        public int Id { get; set; }
        public string? Nombre_usuario { get; set; } = null;
        public string? Email { get; set; } = null;
        public string? Estado { get; set; } = null;
    }
}
