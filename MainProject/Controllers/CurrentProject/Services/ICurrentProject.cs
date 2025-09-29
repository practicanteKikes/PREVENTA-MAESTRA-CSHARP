namespace MainProject.Controllers.CurrentProject.Services
{
    public interface ICurrentProject
    {
        string MainProjectPublishedDateTime(string ls_input_json);
        Task<string> MainGetUserFromDbAsync(string ls_ctrl_json);
    }
}
