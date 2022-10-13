using PoleEmploiApp.DataEntities;
using PoleEmploiApp.DataEntities.Repositories.Interfaces;
using PoleEmploiApp.Services.Interfaces;
using PoleEmploiApp.Services.Models;

namespace PoleEmploiApp.Services
{
    public class JobOfferService : IJobOfferService
    {
        private readonly IGenericRepository<JobOffer> _jobOfferRepo;
        private IPoleEmploiAPIService _poleEmploiAPIService;

        public JobOfferService(IPoleEmploiAPIService poleEmploiAPIService, IGenericRepository<JobOffer> jobOfferRepo)
        {
            _poleEmploiAPIService = poleEmploiAPIService;
            _jobOfferRepo = jobOfferRepo;
        }

        public JobOfferRefreshResult RefreshJobOffers()
        {
            JobOfferRefreshResult result = new JobOfferRefreshResult();

            #region 1) We get the data from the API

           List<Resultat> jobOffersFromAPI = GetJobOffersData();

            #endregion

            #region 2) We update the database
            //  result = Update


            // We first get the list of the existing job offers to know which ones exist qnd which ones need to be created
            List<JobOffer> existingJobOffers = _jobOfferRepo.List().ToList();

            // Number of requests to commit changes. We dotn want to commit too many requests at the end to avoid performance issues
            int maxBulk = 500;
            int currentTransactionsNumber = 0;
            result.Success = true;
            foreach (var jobOfferFromAPI in jobOffersFromAPI)
            {
                try
                {
                    // We check if the job exists in the database
                    JobOffer existingJobOffer = existingJobOffers.FirstOrDefault(s => s.PoleEmploiId == jobOfferFromAPI.id);
                    if (existingJobOffer==null)
                    {
                        currentTransactionsNumber++;
                        result.RowsAddedNumber++;
                        _jobOfferRepo.Add(MapAPIResultatToJobOffer(jobOfferFromAPI));
                    }
                    else if(existingJobOffer.LastModificationDate< jobOfferFromAPI.dateActualisation)
                    {
                        // If the job exists but has been modified since the last modification, we update the database
                        currentTransactionsNumber++;
                        result.RowsUpdatedNumber++;
                        JobOffer jobOffer = MapAPIResultatToJobOffer(jobOfferFromAPI);
                        jobOffer.Id = existingJobOffer.Id;
                        _jobOfferRepo.Edit(jobOffer);
                    }

                    if(currentTransactionsNumber>= maxBulk)
                    {
                        result.Success = result.Success & _jobOfferRepo.Save();
                        currentTransactionsNumber = 0;
                    }
                }
                catch(Exception e)
                {
                    result.Error = result.Error + " | error for Id = " + jobOfferFromAPI.id + " : " + e.ToString();
                }
            }
            // we save at the end to avoid useless DB connections
            result.Success = result.Success & _jobOfferRepo.Save();
            #endregion
            return result;
        }

        /// <summary>
        /// Mapping between the EF object and the API object
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public JobOffer MapAPIResultatToJobOffer(Resultat input)
        {
            JobOffer jobOffer = new JobOffer();
            jobOffer.Title = input.appellationlibelle;
            jobOffer.CreationDate = input.dateCreation;
            jobOffer.LastModificationDate = input.dateActualisation;
            jobOffer.Description = input.description;
            jobOffer.ContractType = input.typeContrat;
            jobOffer.DescriptionLocation = input.lieuTravail?.libelle;
            jobOffer.CityCode = input.lieuTravail?.commune;
            jobOffer.WorkTime = input.dureeTravailLibelleConverti;
            jobOffer.PoleEmploiId = input.id;
            jobOffer.CompanyName = input.entreprise?.nom;
            jobOffer.ApplicationUrl= input.origineOffre?.urlOrigine;
            return jobOffer;
        }

        public List<Resultat> GetJobOffersData()
        {

            // codes found via https://api.emploi-store.fr/partenaire/offresdemploi/v2/referentiel/communes API
            // TODO : use the API instead of hardcoding the codes to have a cleaner code
            // We only check offers from Bordeaux, Paris and Rennes
            List<string> cityCodes = new List<string>() { "75101", "75102", "75103", "75104", "75105", "75106", "75107", "75108", "75109", "75110", "75111", "75112", "75113", "75114", "75115", "75116", "75117", "75118", "75119", "75120", "33063", "35238" };

            List<Resultat> jobOffers = new List<Resultat>();
            int maxPagination = 150;
            int maxMinRangeIndex = 1000;
            int rangeMin, rangeMax, lastRangeMin;

            foreach (string cityCode in cityCodes)
            {
                bool moreOffersToProspect = true;
                rangeMin = 0;
                rangeMax = maxPagination - 1;
                lastRangeMin = rangeMin;
                while (moreOffersToProspect)
                {
                    PoleEmploiAPIJobOffersOutput poleEmploiAPIJobOffersOutput = _poleEmploiAPIService.GetPoleEmploiJobOffers(cityCode, rangeMin, rangeMax);
                    if (poleEmploiAPIJobOffersOutput != null)
                    {
                        jobOffers.AddRange(poleEmploiAPIJobOffersOutput.resultats);
                        if (poleEmploiAPIJobOffersOutput.resultats.Count < maxPagination)
                        {
                            moreOffersToProspect = false;
                        }
                        else
                        {
                            rangeMin = rangeMin + maxPagination;
                            if (rangeMin > maxMinRangeIndex)
                            {
                                rangeMin = maxMinRangeIndex;
                            }
                            if (lastRangeMin == rangeMin)
                            {
                                // 1000 is the maximum for the min range. No need to continue
                                moreOffersToProspect = false;
                                break;
                            }
                            rangeMax = rangeMin + (maxPagination - 1);
                            lastRangeMin = rangeMin;
                        }
                    }
                }
            }
            return jobOffers;
        }

        /// <summary>
        /// Creation of the Excel file
        /// </summary>
        /// <returns></returns>
        public FileProcessResult GetExcelReportData()
        {
            FileProcessResult Result = new FileProcessResult();

            IEnumerable<JobOffer> jobData = _jobOfferRepo.List();

            var ExcelDownload = new ExcelDownload();
            ExcelDownload.AddWorksheet("Data");
            
            foreach (var Item in jobData.ToList())
            {

                ExcelDownload.RowCount = ExcelDownload.RowCount + 1;
                ExcelDownload.SetValue("Id", Item.PoleEmploiId);
                ExcelDownload.SetValue("Date de création", Item.CreationDate.ToString("dd-MM-yyyy"));
                ExcelDownload.SetValue("Type contrat", Item.ContractType);
                ExcelDownload.SetValue("Entreprise", Item.CompanyName);
                ExcelDownload.SetValue("Comune", Item.CityCode);
                ExcelDownload.SetValue("Libellé localisation", Item.DescriptionLocation);
                ExcelDownload.SetValue("Libellé", Item.Title);
                ExcelDownload.SetValue("Description", Item.Description);
                ExcelDownload.SetValue("Type de durée", Item.WorkTime);
                ExcelDownload.SetValue("Url", Item.ApplicationUrl);
            }
            

            ExcelDownload.AutoFitColumns();
            Result.FileName = string.Format("{0}_Jobs.xlsx", DateTime.UtcNow.ToString("yyyy-MM-dd"));
            Result.FileBytes = ExcelDownload.FileBytes;


            return Result;
        }


    }
}