using Ccollab;
using System.Collections.Generic;
using EagleEye.Settings;
using log4net;
using System;
using EagleEye.GVizApi;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Linq;
using Employees;

namespace EagleEye
{
    public abstract class EagleEyeDataGeneratorDecorator<T> : ICcollabDataSource where T : ICcollabRecord
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EagleEyeDataGeneratorDecorator<T>));

        private ICcollabDataSource _ccollabDataGenerator;

        public EagleEyeDataGeneratorDecorator(ICcollabDataSource ccollabDataGenerator)
        {
            _ccollabDataGenerator = ccollabDataGenerator;
        }

        protected void Save2EagleEye(string settingsKey, string json)
        {
            Console.WriteLine(json);

            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            ChartSettings chartSettings = null;
            if (EagleEyeSettingsReader.Settings != null && EagleEyeSettingsReader.Settings.Charts != null)
            {
                EagleEyeSettingsReader.Settings.Charts.TryGetValue(settingsKey, out chartSettings);

                if (chartSettings != null)
                {
                    // send request to eagleeye platform
                    PutDataTableToEagleEye(chartSettings.ChartId, json);
                }
            }
        }

        protected void Save2EagleEye(string settingsKey, DataTable dataTable)
        {
            string json = JsonConvert.SerializeObject(new Chart(dataTable));
            Console.WriteLine(json);

            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            ChartSettings chartSettings = null;
            if (EagleEyeSettingsReader.Settings != null && EagleEyeSettingsReader.Settings.Charts != null)
            {
                EagleEyeSettingsReader.Settings.Charts.TryGetValue(settingsKey, out chartSettings);

                if (chartSettings != null)
                {
                    // send request to eagleeye platform
                    PutDataTableToEagleEye(chartSettings.ChartId, json);
                }
            }
        }

        /// <summary>
        /// Send chart data table to EagleEye platform via a PUT request.
        /// </summary>
        /// <param name="chartId">The chart's _id property.</param>
        /// <param name="json">The data table json.</param>
        private void PutDataTableToEagleEye(string chartId, string json)
        {
            // API: <https://github.com/CVBDL/EagleEye-Docs/blob/master/rest-api/rest-api.md#edit-data-table>

            log.Info("Sending data to EagleEye platform ...");

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

        public List<ReviewRecord> GetReviewsRawData()
        {
            return _ccollabDataGenerator.GetReviewsRawData();
        }

        public List<DefectRecord> GetDefectsRawData()
        {
            return _ccollabDataGenerator.GetDefectsRawData();
        }

        protected List<T> GetValidRecords(List<T> source)
        {
            return source
                .Where(record => EmployeesReader.Employees.Any(employee => employee.LoginName == record.CreatorLogin))
                .Select(record => record)
                .ToList();
        }

        protected List<T> GetRecordsByProduct(List<T> source, string productName)
        {
            List<T> records = null;

            // for all products
            if (productName == "*")
            {
                records = source;
            }
            else if (EagleEyeSettingsReader.Settings.Products.IndexOf(productName) != -1)
            {
                records = source
                    .Where(record => record.CreatorProductName == productName)
                    .ToList();
            }

            return records;
        }
    }
}
