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

        /// <summary>
        /// Filtered reviews data.
        /// </summary>
        private List<string[]> _filteredEmployeesDefectsData = null;

        /// <summary>
        /// Index of "Creator Login" column in defects.csv file
        /// </summary>
        private const int indexCreatorLogin = 8;

        /// <summary>
        /// Index of "Type_CVB" column in defects.csv file
        /// </summary>
        private const int indexType = 22;

        /// <summary>
        /// Index of "Injection Stage" column in defects.csv file
        /// </summary>
        private const int indexInjectionStage = 23;
        
        public Defects(ICcollabDataSource ccollabDataGenerator) : base(ccollabDataGenerator) { }
        
        public List<string[]> FilteredEmployeesDefectsData
        {
            get
            {
                if (_filteredEmployeesDefectsData == null)
                {
                    IEnumerable<string[]> defectsQuery =
                        from row in GetDefectsRawData()
                        where EmployeesReader.GetEmployees().Any(employee => employee.LoginName == row[indexCreatorLogin])
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

        public void GenerateDefectCountByProduct()
        {
            log.Info("Generating: Defect Count By Product ...");

            EagleEyeSettings settings = EagleEyeSettingsReader.GetEagleEyeSettings();

            Dictionary<string, int> product2count = new Dictionary<string, int>();

            // collect all products
            foreach (Employee employee in EmployeesReader.GetEmployees())
            {
                if (!product2count.ContainsKey(employee.ProductName))
                {
                    product2count.Add(employee.ProductName, 0);
                }
            }
            
            var query =
                from row in FilteredEmployeesDefectsData
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

        public void GenerateDefectCountBySeverity()
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Product", "Major", "Minor"],
            //     ["Team1", 20, 3],
            //     ["Team2", 16, 0]
            //   ]
            // }

            log.Info("Generating: Defect Severity Count By Product ...");

            EagleEyeSettings settings = EagleEyeSettingsReader.GetEagleEyeSettings();

            Dictionary<string, List<int>> product2severitycount = new Dictionary<string, List<int>>();

            // collect all products
            foreach (Employee employee in EmployeesReader.GetEmployees())
            {
                if (!product2severitycount.ContainsKey(employee.ProductName))
                {
                    product2severitycount.Add(employee.ProductName, new List<int>(new int[settings.DefectSeverityTypes.Count]));
                }
            }
            var query =
                from row in FilteredEmployeesDefectsData
                let productName = EmployeesReader.GetEmployeeProductName(row[indexCreatorLogin])
                group row by productName into productGroup
                select productGroup;

            int defectSeverityIndex = 20;

            foreach (var productDefects in query)
            {
                if (!product2severitycount.ContainsKey(productDefects.Key)) continue;

                foreach (var defect in productDefects)
                {
                    int index = settings.DefectSeverityTypes.IndexOf(defect[defectSeverityIndex]);

                    if (index >= 0)
                    {
                        product2severitycount[productDefects.Key][index] += 1;
                    }
                }
            }

            Chart chart = new Chart();
            chart.datatable = new List<List<object>>();
            chart.datatable.Add(new List<object> { "Product" }.Concat<object>(settings.DefectSeverityTypes).ToList());

            foreach (KeyValuePair<string, List<int>> item in product2severitycount)
            {
                List<object> row = new List<object> { item.Key };

                foreach (int count in item.Value)
                {
                    row.Add(count);
                }

                chart.datatable.Add(row);
            }

            string json = JsonConvert.SerializeObject(chart);
            Console.WriteLine(json);

            ChartSettings chartSettings = null;
            string chartSettingsKeyName = "DefectCountBySeverity";
            settings.Charts.TryGetValue(chartSettingsKeyName, out chartSettings);

            // send request to eagleeye platform
            //PutDataTableToEagleEye(chartSettings.ChartId, json);

            log.Info("Generating: Defect Severity Count By Product ... Done.");
        }

        public void GenerateDefectCountByInjectionStage()
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["InjectionStage", "Count"],
            //     ["code/unit test", 20],
            //     ["design", 16],
            //     ["requirements", 16]
            //     ["integration/test", 16]
            //   ]
            // }

            log.Info("Generating: Defect Count By Injection Stage ...");

            EagleEyeSettings settings = EagleEyeSettingsReader.GetEagleEyeSettings();

            Dictionary<string, int> injectionstage2count = new Dictionary<string, int>();

            // collect all products
            foreach (var injectionStage in EagleEyeSettingsReader.GetEagleEyeSettings().DefectInjectionStage)
            {
                injectionstage2count.Add(injectionStage.ToLower(), 0);
            }

            var query =
                from row in FilteredEmployeesDefectsData
                group row by row[indexInjectionStage] into injectionStageGroup
                select new { InjectionStage = injectionStageGroup.Key.ToLower(), Count = injectionStageGroup.Count() };

            foreach (var item in query)
            {
                if (injectionstage2count.ContainsKey(item.InjectionStage))
                {
                    injectionstage2count[item.InjectionStage] = item.Count;
                }
            }

            Chart chart = new Chart();
            chart.datatable = new List<List<object>>();
            chart.datatable.Add(new List<object> { "InjectionStage", "Count" });

            foreach (KeyValuePair<string, int> item in injectionstage2count)
            {
                chart.datatable.Add(new List<object> { item.Key, item.Value });
            }

            string json = JsonConvert.SerializeObject(chart);
            Console.WriteLine(json);

            ChartSettings chartSettings = null;
            string chartSettingsKeyName = "DefectCountByInjectionStage";
            settings.Charts.TryGetValue(chartSettingsKeyName, out chartSettings);

            // send request to eagleeye platform
            //PutDataTableToEagleEye(chartSettings.ChartId, json);

            log.Info("Generating: Defect Count By Injection Stage ... Done.");
        }

        public void GenerateDefectCountByType()
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Type", "Count"],
            //     ["algorithm/logic", 20],
            //     ["build", 16],
            //     ...
            //   ]
            // }

            log.Info("Generating: Defect Count By Type ...");

            EagleEyeSettings settings = EagleEyeSettingsReader.GetEagleEyeSettings();

            Dictionary<string, int> type2count = new Dictionary<string, int>();

            // collect all products
            foreach (var type in EagleEyeSettingsReader.GetEagleEyeSettings().DefectTypes)
            {
                type2count.Add(type.ToLower(), 0);
            }

            var query =
                from row in FilteredEmployeesDefectsData
                group row by row[indexType] into typeGroup
                select new { Type = typeGroup.Key.ToLower(), Count = typeGroup.Count() };

            foreach (var item in query)
            {
                if (type2count.ContainsKey(item.Type))
                {
                    type2count[item.Type] = item.Count;
                }
            }

            Chart chart = new Chart();
            chart.datatable = new List<List<object>>();
            chart.datatable.Add(new List<object> { "Type", "Count" });

            foreach (KeyValuePair<string, int> item in type2count)
            {
                chart.datatable.Add(new List<object> { item.Key, item.Value });
            }

            string json = JsonConvert.SerializeObject(chart);
            Console.WriteLine(json);

            ChartSettings chartSettings = null;
            string chartSettingsKeyName = "DefectCountByType";
            settings.Charts.TryGetValue(chartSettingsKeyName, out chartSettings);

            // send request to eagleeye platform
            //PutDataTableToEagleEye(chartSettings.ChartId, json);

            log.Info("Generating: Defect Count By Type ... Done.");
        }
    }
}
