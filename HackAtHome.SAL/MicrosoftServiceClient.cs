using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using HackAtHome.Entities;
using Microsoft.WindowsAzure.MobileServices;

namespace HackAtHome.SAL
{
    public class MicrosoftServiceClient
    {
        const string WEB_API_BASE_ADDRESS = "https://ticapacitacion.com/hackathome/";
        const string EVENT_ID = "xamarin30";
        const string APPLICATION_JSON = "application/json";
        const string XAMARIN_DIPLOMADO_AZURE_URL = "http://xamarin-diplomado.azurewebsites.net/";

        MobileServiceClient mobileServiceCliente = null;
        private IMobileServiceTable<LabItem> LabItemTable = null;

        HttpClient httpClient = null;
        public MicrosoftServiceClient()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(WEB_API_BASE_ADDRESS);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(APPLICATION_JSON));
        }


        /// <summary>
        /// Authenticate student using webApi
        /// </summary>
        /// <param name="studentMail"></param>
        /// <param name="studentPassword"></param>
        /// <returns></returns>
        public async Task<ResultInfo> AutenticateAsync(string studentMail,
            string studentPassword)
        {
            ResultInfo resultInfo = null;
            string requestUri = "api/evidence/Authenticate";

            UserInfo user = new UserInfo
            {
                Email = studentMail,
                Password = studentPassword,
                EventID = EVENT_ID
            };

            try
            {
                var jsonUserInfo = JsonConvert.SerializeObject(user);

                HttpResponseMessage response =
                    await httpClient.PostAsync(requestUri,
                    new StringContent(jsonUserInfo.ToString(),
                    Encoding.UTF8, APPLICATION_JSON));

                var resultWebApi = await response.Content.ReadAsStringAsync();
                resultInfo = JsonConvert.DeserializeObject<ResultInfo>(resultWebApi);

            }
            catch (Exception ex)
            {

            }

            return resultInfo;
        }

        /// <summary>
        /// Get all evidence list
        /// </summary>
        /// <param name="token">user authentication token</param>
        /// <returns></returns>
        public async Task<List<Evidence>> GetEvidencesAsync(string token)
        {
            List<Evidence> evidences = null;

            string requestUri =
                $"{WEB_API_BASE_ADDRESS}api/evidence/getevidences?token={token}";

            try
            {
                var response =
                    await httpClient.GetAsync(requestUri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var resultWebApi = await
                        response.Content.ReadAsStringAsync();

                    evidences = JsonConvert.DeserializeObject<List<Evidence>>(resultWebApi);
                }
            }
            catch (Exception ex)
            {

            }

            return evidences;
        }

        /// <summary>
        /// Get evidence details
        /// </summary>
        /// <param name="token">user authentication token</param>
        /// <param name="evidenceID">evidence ID</param>
        /// <returns></returns>
        public async Task<EvidenceDetail> GetEvidenceByIDAsync(string token,
            string evidenceID)
        {
            EvidenceDetail result = null;
            string requestUri =
                $"{WEB_API_BASE_ADDRESS}api/evidence/getevidencebyid?token={token}&&evidenceid={evidenceID}";

            try
            {
                var response = await httpClient.GetAsync(requestUri);
                var resultWebApi = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<EvidenceDetail>(resultWebApi);
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }


        /// <summary>
        /// Send evidence
        /// </summary>
        /// <param name="userEvidence"></param>
        /// <returns></returns>
        public async Task SendEvidence(LabItem userEvidence)
        {
            mobileServiceCliente =
                new MobileServiceClient(XAMARIN_DIPLOMADO_AZURE_URL);
            LabItemTable = mobileServiceCliente.GetTable<LabItem>();
            await LabItemTable.InsertAsync(userEvidence);
        }
    }
}