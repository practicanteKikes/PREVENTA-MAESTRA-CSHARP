using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSqlserver.Models.EFModel
{
    [Table("user_list")]

    public class UserEF
    {
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = "";

        [Column("email")]
        public string Email { get; set; } = "";
    }
}
