using PoleEmploiApp.Services.Models;

namespace PoleEmploiApp.Services.Interfaces
{
    public interface IPoleEmploiAPIService
    {
        PoleEmploiAPIJobOffersOutput GetPoleEmploiJobOffers(string CityCode, int rangeMin, int rangeMax);
    }
}