using Newtonsoft.Json;
using PoleEmploiApp.Services.Interfaces;
using PoleEmploiApp.Services.Models;
using System.Net;
using System.Web;

namespace PoleEmploiApp.Services
{
    public class PoleEmploiAPIService : IPoleEmploiAPIService
    {
        // TODO: Move APIkeys to settings

        private string ClientKey = "PAR_jobhunter_e8f18998f54e9f6b478ccbf373706177f653fdfb26faa3dd93c41cfad8ce1d74";
        private string SecretKey = "2db600e4344124c7ea2ea576a6c104bad746fad0cbd36f9ac9e41db34dd2219e";
        private string BaseAuthentificationAPIUrl = "https://entreprise.pole-emploi.fr";
        private string BaseAPIUrl = "https://api.emploi-store.fr";

        public PoleEmploiAPIService()
        {

        }


        // TODO : should me moved to a Utils/commons class
        private static string GetWebExceptionBody(WebException e)
        {
            string error = "";
            if (e != null && e.Response != null)
            {
                var response = ((HttpWebResponse)e.Response);

                var reader = new StreamReader(e.Response.GetResponseStream());
                error = "WEB EXCEPTION :  error type : " + e.Status;
                if (reader != null)
                    error = error + " : " + reader.ReadToEnd();
            }
            else
            {
                error = "EMPTY WEB EXCEPTION";
            }

            return error;
        }


        private static TResult PostFormUrlEncoded<TResult>(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var content = new FormUrlEncodedContent(postData))
                    {
                        content.Headers.Clear();
                        content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                        HttpResponseMessage response = httpClient.PostAsync(url, content).Result;

                        Stream stream = response.Content.ReadAsStream();
                        using (var streamReader = new StreamReader(stream))
                        {
                            string json= streamReader.ReadToEnd();
                            if(!String.IsNullOrWhiteSpace(json))
                            {
                                return JsonConvert.DeserializeObject<TResult>(json);
                            }
                        }

                    }
                }
            }
            catch (WebException e)
            {
                string error = GetWebExceptionBody(e);
            }

            return default(TResult);
        }

        private string GenerateAccessToken(List<string> ScopesList)
        {
            if(ScopesList==null || ScopesList.Count==0)
            {
                return null;
            }
            string url = BaseAuthentificationAPIUrl + "/connexion/oauth2/access_token?realm=%2Fpartenaire";
            IEnumerable<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>> {
              new KeyValuePair<string, string>("grant_type", "client_credentials"),
              new KeyValuePair<string, string>("client_id" ,ClientKey),
               new KeyValuePair<string, string>("client_secret" ,SecretKey),
                new KeyValuePair<string, string>("scope" ,string.Join(' ',ScopesList))
             };
            return PostFormUrlEncoded<PoleEmploiAPIAccessTokenOutput>(url, values)?.access_token;
        }

        /// <summary>
        /// Get job offers from Pole-Emploi API
        /// </summary>
        /// <returns></returns>
        private PoleEmploiAPIJobOffersOutput GetPoleEmploiJobOffers(string CityCode,int rangeMin,int rangeMax)
        {
            List<string> scopes = new List<string>() { "api_offresdemploiv2", "o2dsoffre" };
            // TODO: find a way to avoid having one token per request. A token can last several requests. Each token has an expiration date, we should use this information in the future.
            string accessToken = GenerateAccessToken(scopes);
            if (String.IsNullOrWhiteSpace(accessToken))
            {
                return null;
            }

            string url = BaseAPIUrl + "/partenaire/offresdemploi/v2/offres/search?sort=1&commune=" + CityCode+"&range="+ rangeMin+"-"+rangeMax;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + accessToken);
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string json = streamReader.ReadToEnd();
                    if (!String.IsNullOrWhiteSpace(json))
                    {
                        return JsonConvert.DeserializeObject<PoleEmploiAPIJobOffersOutput>(json);
                    }
                }
            }
            catch (WebException e)
            {
                string error = GetWebExceptionBody(e);
            }
            return null;
        }

        /// <summary>
        ///  Get the data from the API
        /// </summary>
        /// <returns></returns>
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
                    PoleEmploiAPIJobOffersOutput poleEmploiAPIJobOffersOutput = GetPoleEmploiJobOffers(cityCode, rangeMin, rangeMax);
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

    }
}