using PoleEmploiApp.Services.Models;

namespace PoleEmploiApp.Services.Interfaces
{
    public interface IJobOfferService
    {
        FileProcessResult GetExcelReportData();
        JobOfferRefreshResult RefreshJobOffers();
    }
}