using Ccollab;
using EagleEye.Settings;
using Employees;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace EagleEye.Reviews
{
    public class Reviews : EagleEyeDataGeneratorDecorator, IReviewsCommands
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Reviews));

        private EagleEyeSettings settings = null;
        private List<Employee> employees = null;

        public Reviews(ICcollabDataSource ccollabDataGenerator) : base(ccollabDataGenerator)
        {
            settings = EagleEyeSettingsReader.GetEagleEyeSettings();
            employees = EmployeesReader.GetEmployees();
        }

        private List<string[]> _filteredEmployeesReviewsData = null;

        public List<string[]> FilteredEmployeesReviewsData
        {
            get
            {
                if (_filteredEmployeesReviewsData == null)
                {
                    List<string[]> reviewsRawData = GetReviewsRawData();

                    int reviewsCreatorLoginIndex = 9;

                    IEnumerable<string[]> reviewsQuery =
                        from row in reviewsRawData
                        where employees.Any(employee => employee.LoginName == row[reviewsCreatorLoginIndex])
                        select row;

                    _filteredEmployeesReviewsData = reviewsQuery.ToList<string[]>();
                }

                return _filteredEmployeesReviewsData;
            }
        }
        
        private void PutDataTableToEagleEye(string chartId, string json)
        {
            // API: <https://github.com/CVBDL/EagleEye-Docs/blob/master/rest-api/rest-api.md#edit-data-table>

            log.Info("Sending data to EagleEye platform ...");

            HttpClient httpClient = new HttpClient();

            try
            {
                StringContent payload = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = httpClient.PutAsync(settings.ApiRootEndpoint + "charts/" + chartId + "/datatable", payload).Result;
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

            log.Info("Sending data to EagleEye platform ... Done.");
        }

        public void GenerateReviewCountByMonth()
        {
            log.Info("Generating: Review Count By Month ...");



            // `Review Creation Date`'s format is "2016-09-30 23:33 UTC"
            var reviewCreationDateIndex = 2;

            var query =
                from row in FilteredEmployeesReviewsData
                group row by row[reviewCreationDateIndex].Substring(0, 7) into month
                orderby month.Key ascending
                select new { Month = month.Key, Count = month.Count() };

            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Month", "Count"],
            //     ["2016-01", 20],
            //     ["2016-02", 30]
            //   ]
            // }

            Chart chart = new Chart();
            chart.datatable = new List<List<object>>();
            chart.datatable.Add(new List<object> { "Month", "Count" });

            foreach (var date in query)
            {
                chart.datatable.Add(new List<object> { date.Month, date.Count });
            }

            string json = JsonConvert.SerializeObject(chart);
            Console.WriteLine(json);

            ChartSettings chartSettings = null;
            string chartSettingsKeyName = "ReviewCountByMonth";
            settings.Charts.TryGetValue(chartSettingsKeyName, out chartSettings);

            // send request to eagleeye platform
            //PutDataTableToEagleEye(chartSettings.ChartId, json);

            log.Info("Generating: Review Count By Month ... Done.");
        }

        public void GenerateReviewCountByProduct()
        {
            log.Info("Generating: Review Count By Product ...");

            Dictionary<string, int> product2count = new Dictionary<string, int>();

            // collect all products
            foreach (Employee employee in employees)
            {
                if (!product2count.ContainsKey(employee.ProductName))
                {
                    product2count.Add(employee.ProductName, 0);
                }
            }

            int employeeLoginNameIndex = 9;

            var query =
                from row in FilteredEmployeesReviewsData
                let productName = EmployeesReader.GetEmployeeProductName(row[employeeLoginNameIndex])
                group row by productName into productGroup
                select new { ProductName = productGroup.Key, DefectCount = productGroup.Count() };

            foreach (var product in query)
            {
                if (product2count.ContainsKey(product.ProductName))
                {
                    product2count[product.ProductName] = product.DefectCount;
                }
            }

            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Product", "Count"],
            //     ["Team1", 20],
            //     ["Team2", 16]
            //   ]
            // }

            Chart chart = new Chart();
            chart.datatable = new List<List<object>>();
            chart.datatable.Add(new List<object> { "Product", "Count" });

            foreach (KeyValuePair<string, int> item in product2count)
            {
                chart.datatable.Add(new List<object> { item.Key, item.Value });
            }

            string json = JsonConvert.SerializeObject(chart);
            Console.WriteLine(json);

            ChartSettings chartSettings = null;
            string chartSettingsKeyName = "ReviewCountByProduct";
            settings.Charts.TryGetValue(chartSettingsKeyName, out chartSettings);

            // send request to eagleeye platform
            //PutDataTableToEagleEye(chartSettings.ChartId, json);

            log.Info("Generating: Review Count By Product ... Done.");
        }
    }
}
