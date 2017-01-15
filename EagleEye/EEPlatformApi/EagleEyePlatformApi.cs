using EagleEye.GVizApi;
using log4net;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace EagleEye.EEPlatformApi
{
    public static class EagleEyePlatformApi
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EagleEyePlatformApi));

        /// <summary>
        /// Send chart data table to EagleEye platform via a PUT request.
        /// </summary>
        /// <param name="chartId">The chart's _id property.</param>
        /// <param name="json">The data table json.</param>
        private static void PutDataTableToEagleEye(string chartId, string json)
        {
            // API: <https://github.com/CVBDL/EagleEye-Docs/blob/master/rest-api/rest-api.md#edit-data-table>

            log.Info("Sending data to EagleEye platform ...");
            log.Info("ChartId :: " + chartId);
            log.Info("JSON :: " + json);

            //EagleEyeSettings settings = EagleEyeSettingsReader.Settings;

            //HttpClient httpClient = new HttpClient();

            //try
            //{
            //    StringContent payload = new StringContent(json, Encoding.UTF8, "application/json");
            //    HttpResponseMessage response = httpClient.PutAsync(settings.ApiRootEndpoint + "charts/" + chartId + "/datatable", payload).Result;
            //    response.EnsureSuccessStatusCode();

            //    // use for debugging
            //    var responseContent = response.Content;
            //    string responseBody = responseContent.ReadAsStringAsync().Result;
            //    Console.WriteLine(responseBody);
            //}
            //catch (HttpRequestException e)
            //{
            //    log.Info("Error: Put data table to chart with id '" + chartId + "'");
            //    Console.WriteLine("Message :{0} ", e.Message);
            //}

            log.Info("Sending data to EagleEye platform ... Done.");
        }

        public static void EditDataTable(string chartId, DataTable dataTable)
        {
            if (string.IsNullOrWhiteSpace(chartId)) return;

            Chart chart = new Chart(dataTable);

            string json = null;
            try
            {
                json = JsonConvert.SerializeObject(chart);
            }
            catch (Exception e)
            {
                log.Error("Unable to serialize data table to JSON for chart: " + chartId + ".");
                log.Error(e.ToString());

                return;
            }

            PutDataTableToEagleEye(chartId, json);
        }
    }
}
