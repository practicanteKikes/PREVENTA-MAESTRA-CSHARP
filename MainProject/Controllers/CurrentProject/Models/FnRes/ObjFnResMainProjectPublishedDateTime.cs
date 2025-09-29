namespace MainProject.Controllers.CurrentProject.Models.FnRes
{
    public class ObjFnResMainProjectPublishedDateTime
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public DateTime? Kldt_fecha_servidor_ase { get; set; } = null;
        public List<string> Kdata { get; set; } = new List<string>();
    }
}
