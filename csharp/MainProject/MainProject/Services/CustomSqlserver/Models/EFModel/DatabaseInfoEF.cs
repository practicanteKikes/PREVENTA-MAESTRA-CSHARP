using Microsoft.EntityFrameworkCore;

namespace MainProject.Services.CustomSqlserver.Models.EFModel
{
    //[Keyless]
    public class DatabaseInfoEF
    {
        public string? Db_name { get; set; } = null;
    }
}
