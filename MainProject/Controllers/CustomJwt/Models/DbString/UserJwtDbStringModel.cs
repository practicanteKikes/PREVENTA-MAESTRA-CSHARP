namespace MainProject.Controllers.CustomJwt.Models.DbString
{
    public class UserJwtDbStringModel
    {
        // custom csharp developer logic properties
        const string ACTIVECHARACTER = "A";

        public string? Id { get; set; }
        public string? User { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Rol { get; set; }
        public string? Estado { get; set; }

        // custom csharp developer logic methods
        public bool IsActive()
        {
            if (Estado == ACTIVECHARACTER)
            {
                return true;
            }
            return false;
        }
    }
}
