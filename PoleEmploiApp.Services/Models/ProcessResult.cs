namespace PoleEmploiApp.Services.Models
{
    public class ProcessResult
    {
        public bool Success { get; set; }

        public string Error { get; set; }

        public ProcessResult()
        {
            this.Error = "";
        }
    }
}