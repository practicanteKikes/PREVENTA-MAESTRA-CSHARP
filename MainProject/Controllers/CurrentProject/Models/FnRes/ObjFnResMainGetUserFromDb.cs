using MainProject.Controllers.CurrentProject.Models.Json;

namespace MainProject.Controllers.CurrentProject.Models.FnRes
{
    public class ObjFnResMainGetUserFromDb
    {
        // DEFAULT KIKES DEVELOPERS RESPONSES        

        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";

        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<UserJsonModelCP> Kdata { get; set; } = new List<UserJsonModelCP>();
    }
}
