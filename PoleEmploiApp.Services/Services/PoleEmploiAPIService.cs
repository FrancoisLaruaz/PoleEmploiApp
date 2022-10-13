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


        public static TResult PostFormUrlEncoded<TResult>(string url, IEnumerable<KeyValuePair<string, string>> postData)
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
        public PoleEmploiAPIJobOffersOutput GetPoleEmploiJobOffers(string CityCode,int rangeMin,int rangeMax)
        {
            List<string> scopes = new List<string>() { "api_offresdemploiv2", "o2dsoffre" };
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

    }
}