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

namespace EagleEye.Defects
{
    public class Defects : EagleEyeDataGeneratorDecorator, IDefectsCommands
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Defects));

        private EagleEyeSettings settings = null;
        private List<Employee> employees = null;

        public Defects(ICcollabDataSource ccollabDataGenerator) : base(ccollabDataGenerator)
        {
            settings = EagleEyeSettingsReader.GetEagleEyeSettings();
            employees = EmployeesReader.GetEmployees();
        }

        private List<string[]> _filteredEmployeesDefectsData = null;

        public List<string[]> FilteredEmployeesDefectsData
        {
            get
            {
                if (_filteredEmployeesDefectsData == null)
                {
                    List<string[]> defectsRawData = GetDefectsRawData();

                    int reviewsCreatorLoginIndex = 8;

                    IEnumerable<string[]> defectsQuery =
                        from row in defectsRawData
                        where employees.Any(employee => employee.LoginName == row[reviewsCreatorLoginIndex])
                        select row;

                    _filteredEmployeesDefectsData = defectsQuery.ToList<string[]>();
                }

                return _filteredEmployeesDefectsData;
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

            log.Info("Sending data to EagleEye platform ... Done.");
        }

        public void GenerateDefectCountByProduct()
        {
            log.Info("Generating: Defect Count By Product ...");

            Dictionary<string, int> product2count = new Dictionary<string, int>();

            // collect all products
            foreach (Employee employee in employees)
            {
                if (!product2count.ContainsKey(employee.ProductName))
                {
                    product2count.Add(employee.ProductName, 0);
                }
            }

            int employeeLoginNameIndex = 8;

            var query =
                from row in FilteredEmployeesDefectsData
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
            string chartSettingsKeyName = "DefectCountByProduct";
            settings.Charts.TryGetValue(chartSettingsKeyName, out chartSettings);

            // send request to eagleeye platform
            //PutDataTableToEagleEye(chartSettings.ChartId, json);

            log.Info("Generating: Defect Count By Product ... Done.");
        }
    }
}
