namespace MainProject.Controllers.CmUsuariosMovil.Models.CmUsuariosMovilFiltered.ObtainCmUsuariosMovilObjectListAzync
{
    public class ResAjaxCmUsuariosMovilFilteredAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";

        public Dictionary<string, object> Kfiltered_params { get; set; } = new Dictionary<string, object>();
        public List<ModelCmUsuariosMovilJsonFiltered> Kdata { get; set; } = new List<ModelCmUsuariosMovilJsonFiltered>();
    }
}
