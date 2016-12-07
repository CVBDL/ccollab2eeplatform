using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using log4net;

namespace eeDataGenerator
{
    public class EagleEyeReviewsDataGenerator: EagleEyeDataGeneratorDecorator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EagleEyeReviewsDataGenerator));

        private List<string[]> FilteredEmployeesReviewsData;
        private List<Employee> Employees;
        private ApplicationSettings Settings;
        private HttpClient httpClient;

        public EagleEyeReviewsDataGenerator(IEagleEyeDataGenerator eagleeyeDataGenerator) : base(eagleeyeDataGenerator)
        {
            httpClient = new HttpClient();
        }

        public new bool Execute()
        {
            base.Execute();
            
            Employees = Employee.InitFromJson();
            
            Settings = ApplicationSettings.InitFromJson();

            FilteredEmployeesReviewsData = FilterEmployeesReviewData(ReviewsRawData);

            GenerateReviewCountByMonth();

            return true;
        }

        private List<string[]> FilterEmployeesReviewData(List<string[]> ReviewsRawData)
        {
            var reviewsCreatorLoginIndex = 9;

            IEnumerable<string[]> reviewsQuery =
                from row in ReviewsRawData
                where Employees.Any(employee => employee.LoginName == row[reviewsCreatorLoginIndex])
                select row;

            return reviewsQuery.ToList<string[]>();
        }

        private void GenerateReviewCountByMonth()
        {
            log.Info("Generating: Review Count By Month ...");

            string chartSettingsKeyName = "ReviewCountByMonth";
            ChartItem chartSettings = null;

            Settings.Charts.TryGetValue(chartSettingsKeyName, out chartSettings);  // { "ChartId": "57837029c66dc1a4570962b6" }

            // `Review Creation Date`'s format is "2016-09-30 23:33 UTC"
            var reviewCreationDateIndex = 2;

            var query =
                from row in FilteredEmployeesReviewsData
                group row by row[reviewCreationDateIndex].Substring(0, 7) into month
                orderby month.Key ascending
                select new { Month = month.Key, Count = month.Count() };

            List<object[]> datatable = new List<object[]>();

            datatable.Add(new object[] { "Month", "Count" });

            foreach (var date in query)
            {
                datatable.Add(new object[] { date.Month, date.Count });
                log.Info("Month: " + date.Month + ", Count: " + date.Count);
            }
            
            string payload = JsonConvert.SerializeObject(new Chart(datatable.ToArray()));

            log.Info(payload); // {"datatable":[["Month","Count"],["2016-07",1],["2016-08",2],["2016-09",1]]}

            // Expected format:
            // https://github.com/CVBDL/EagleEye-Docs/blob/master/rest-api/rest-api.md#edit-data-table
            // {
            //   "datatable": [
            //     ["Month", "Count"],
            //     ["2016-01", 20],
            //     ["2016-02", 30],
            //     ["2016-03", 25]
            //   ]
            // }


            // Calling EagleEye API to update chart

            //log.Info("Sending Review Count By Month Data to Server ...");

            //HttpClient client = new HttpClient();

            //try
            //{
            //    var chartId = "57837029c66dc1a4570962b6";
            //    StringContent payload = new StringContent(json, Encoding.UTF8, "application/json");
            //    HttpResponseMessage response = client.PutAsync("http://127.0.0.1:3000/api/v1/charts/" + chartId + "/datatable", payload).Result;
            //    response.EnsureSuccessStatusCode();
            //    var responseContent = response.Content;
            //    string responseBody = responseContent.ReadAsStringAsync().Result;

            //    Console.WriteLine(responseBody);
            //}
            //catch (HttpRequestException e)
            //{
            //    Console.WriteLine("\nUpdate task state error!");
            //    Console.WriteLine("Message :{0} ", e.Message);
            //}

            log.Info("Generating: Review Count By Month ... Done.");
        }
    }
}
