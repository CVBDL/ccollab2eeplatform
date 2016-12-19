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

        /// <summary>
        /// Filtered reviews data.
        /// </summary>
        private List<string[]> _filteredEmployeesReviewsData = null;

        /// <summary>
        /// Index of "Review Creation Date" column in reviews.csv file
        /// </summary>
        private const int indexReviewCreationDate = 2;

        /// <summary>
        /// Index of "Creator Login" column in reviews.csv file
        /// </summary>
        private const int indexCreatorLogin = 9;
        
        public Reviews(ICcollabDataSource ccollabDataGenerator) : base(ccollabDataGenerator) { }
        

        public List<string[]> FilteredEmployeesReviewsData
        {
            get
            {
                if (_filteredEmployeesReviewsData == null)
                {
                    IEnumerable<string[]> reviewsQuery =
                        from row in GetReviewsRawData()
                        where EmployeesReader.GetEmployees().Any(employee => employee.LoginName == row[indexCreatorLogin])
                        select row;

                    _filteredEmployeesReviewsData = reviewsQuery.ToList<string[]>();
                }

                return _filteredEmployeesReviewsData;
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

            EagleEyeSettings settings = EagleEyeSettingsReader.GetEagleEyeSettings();

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

        /// <summary>
        /// Generate review count by month.
        /// </summary>
        public void GenerateReviewCountByMonth()
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Month", "Count"],
            //     ["2016-01", 20],
            //     ["2016-02", 30]
            //   ]
            // }

            log.Info("Generating: Review Count By Month ...");

            EagleEyeSettings settings = EagleEyeSettingsReader.GetEagleEyeSettings();

            var query =
                from row in FilteredEmployeesReviewsData
                
                // `Review Creation Date` column data format is "2016-09-30 23:33 UTC"
                group row by row[indexReviewCreationDate].Substring(0, 7) into month
                orderby month.Key ascending
                select new { Month = month.Key, Count = month.Count() };

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

        /// <summary>
        /// Generate review count by product.
        /// </summary>
        public void GenerateReviewCountByProduct()
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Product", "Count"],
            //     ["Team1", 20],
            //     ["Team2", 16]
            //   ]
            // }

            log.Info("Generating: Review Count By Product ...");

            List<Employee> employees = EmployeesReader.GetEmployees();
            EagleEyeSettings settings = EagleEyeSettingsReader.GetEagleEyeSettings();

            Dictionary<string, int> product2count = new Dictionary<string, int>();

            // collect all products
            foreach (Employee employee in employees)
            {
                if (!product2count.ContainsKey(employee.ProductName))
                {
                    product2count.Add(employee.ProductName, 0);
                }
            }
            
            var query =
                from row in FilteredEmployeesReviewsData
                let productName = EmployeesReader.GetEmployeeProductName(row[indexCreatorLogin])
                group row by productName into productGroup
                select new { ProductName = productGroup.Key, DefectCount = productGroup.Count() };

            foreach (var product in query)
            {
                if (product2count.ContainsKey(product.ProductName))
                {
                    product2count[product.ProductName] = product.DefectCount;
                }
            }

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

        public void GenerateReviewCountByEmployeeOfProduct()
        {
            foreach (var item in EagleEyeSettingsReader.GetEagleEyeSettings().ReviewCountByEmployeeOfProduct)
            {
                ReviewCountByEmployeeOfProduct(item.ProductName, item.ChartSettingsKey);
            }
        }

        /// <summary>
        /// Generate review count submitted by employees belongs to the given product.
        /// </summary>
        /// <param name="productName">Product name.</param>
        /// <param name="settingsKey">EagleEye settings key name.</param>
        private void ReviewCountByEmployeeOfProduct(string productName, string settingsKey)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["EmployeeName", "ReviewCount"],
            //     ["Patrick Zhong", 16],
            //     ["Merlin Mo", 16]
            //   ]
            // }

            log.Info("Generating: Review Count For " + productName + " ...");

            EagleEyeSettings settings = EagleEyeSettingsReader.GetEagleEyeSettings();

            Dictionary<string, int> employee2count = new Dictionary<string, int>();

            // collect all employees of the product
            foreach (Employee employee in EmployeesReader.GetEmployeesByProduct(productName))
            {
                employee2count.Add(employee.LoginName, 0);
            }
            
            var query =
                from row in FilteredEmployeesReviewsData
                let _productName = EmployeesReader.GetEmployeeProductName(row[indexCreatorLogin])
                where _productName == productName
                group row by row[indexCreatorLogin] into employeeReviewsGroup
                select new { LoginName = employeeReviewsGroup.Key, ReviewCount = employeeReviewsGroup.Count() };

            foreach (var employee in query)
            {
                if (employee2count.ContainsKey(employee.LoginName))
                {
                    employee2count[employee.LoginName] = employee.ReviewCount;
                }
            }

            Chart chart = new Chart();
            chart.datatable = new List<List<object>>();
            chart.datatable.Add(new List<object> { "EmployeeName", "ReviewCount" });

            foreach (KeyValuePair<string, int> item in employee2count)
            {
                chart.datatable.Add(new List<object> { EmployeesReader.GetEmployeeFullNameByLoginName(item.Key), item.Value });
            }

            string json = JsonConvert.SerializeObject(chart);
            Console.WriteLine(json);

            ChartSettings chartSettings = null;
            settings.Charts.TryGetValue(settingsKey, out chartSettings);

            // send request to eagleeye platform
            //PutDataTableToEagleEye(chartSettings.ChartId, json);
            
            log.Info("Generating: Review Count For " + productName + " ... Done.");
        }
    }
}
