using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using log4net;

namespace EagleEye
{
    public class ReviewsDataGenerator: EagleEyeDataGeneratorDecorator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReviewsDataGenerator));

        private HttpClient httpClient = new HttpClient();

        private List<string[]> filteredEmployeesReviewsData = null;
        private ApplicationSettings settings = null;
        private List<Employee> employees = null;

        public ReviewsDataGenerator(IEagleEyeDataGenerator eagleeyeDataGenerator) : base(eagleeyeDataGenerator)
        {
            httpClient = new HttpClient();
        }

        public new bool Execute()
        {
            if (!base.Execute())
            {
                return false;
            }

            employees = EmployeesGenerator.GetEmployees();

            settings = ApplicationSettingsGenerator.GetApplicationSettings();

            filteredEmployeesReviewsData = FilterEmployeesReviewData(ReviewsRawData);

            GenerateReviewCountByMonth();

            return true;
        }

        private List<string[]> FilterEmployeesReviewData(List<string[]> ReviewsRawData)
        {
            var reviewsCreatorLoginIndex = 9;

            IEnumerable<string[]> reviewsQuery =
                from row in ReviewsRawData
                where employees.Any(employee => employee.LoginName == row[reviewsCreatorLoginIndex])
                select row;

            return reviewsQuery.ToList<string[]>();
        }

        private void PutDataTableToEagleEye(string chartId, string json)
        {
            // API: <https://github.com/CVBDL/EagleEye-Docs/blob/master/rest-api/rest-api.md#edit-data-table>

            log.Info("Sending Review Count By Month Data to Server ...");

            try
            {
                StringContent payload = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = httpClient.PutAsync(settings.EagleEyeApiRootEndpoint + "charts/" + chartId + "/datatable", payload).Result;
                response.EnsureSuccessStatusCode();

                // use for debugging
                var responseContent = response.Content;
                string responseBody = responseContent.ReadAsStringAsync().Result;
                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {
                log.Info("Error: Put data table to chart with id '" + chartId + "'");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        private void GenerateReviewCountByMonth()
        {
            log.Info("Generating: Review Count By Month ...");

            string chartSettingsKeyName = "ReviewCountByMonth";
            ChartSettings chartSettings = null;

            settings.Charts.TryGetValue(chartSettingsKeyName, out chartSettings);

            // `Review Creation Date`'s format is "2016-09-30 23:33 UTC"
            var reviewCreationDateIndex = 2;

            var query =
                from row in filteredEmployeesReviewsData
                group row by row[reviewCreationDateIndex].Substring(0, 7) into month
                orderby month.Key ascending
                select new { Month = month.Key, Count = month.Count() };

            // Expected data table format:
            // https://github.com/CVBDL/EagleEye-Docs/blob/master/rest-api/rest-api.md#edit-data-table
            // {
            //   "datatable": [
            //     ["Month", "Count"],
            //     ["2016-01", 20],
            //     ["2016-02", 30],
            //     ["2016-03", 25]
            //   ]
            // }

            List<object[]> datatable = new List<object[]>();

            datatable.Add(new object[] { "Month", "Count" });

            foreach (var date in query)
            {
                datatable.Add(new object[] { date.Month, date.Count });
                Console.WriteLine("Month: " + date.Month + ", Count: " + date.Count);
            }
            
            string json = JsonConvert.SerializeObject(new Chart(datatable.ToArray()));

            Console.WriteLine(json); // {"datatable":[["Month","Count"],["2016-07",1],["2016-08",2],["2016-09",1]]}

            //PutDataTableToEagleEye(chartSettings.ChartId, json);

            log.Info("Generating: Review Count By Month ... Done.");
        }
    }
}
