using System.Text.Json.Serialization;

namespace MainProject.Controllers.CurrentProject.Models.FnRes
{
    public class ObjFnResMainGetUserFromDbPro : ObjFnResMainGetUserFromDbDev
    {
        // Explicit ignore json response. And Explicit ignore property with word 'new'
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public new string? Db_name_input { get; set; } = null;


        // Explicit ignore json response. And Explicit ignore property with word 'new'
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public new string? Db_name_output { get; set; } = null;
    }
}
