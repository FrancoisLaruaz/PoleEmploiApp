namespace PoleEmploiApp.Services.Models
{
    public class JobOfferRefreshResult : ProcessResult
    {
        public int RowsAddedNumber { get; set; }

        public int RowsUpdatedNumber { get; set; }

        public JobOfferRefreshResult()
        {
            this.Error = "";
        }
    }
}