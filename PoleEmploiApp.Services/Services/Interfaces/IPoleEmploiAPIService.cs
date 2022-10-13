using PoleEmploiApp.Services.Models;

namespace PoleEmploiApp.Services.Interfaces
{
    public interface IPoleEmploiAPIService
    {
        public List<Resultat> GetJobOffersData();
    }
}