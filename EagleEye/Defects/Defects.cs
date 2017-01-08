using Ccollab;
using EagleEye.Settings;
using Employees;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EagleEye.Defects
{
    public class Defects : EagleEyeDataGeneratorDecorator, IDefectsCommands
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Defects));
        
        public Defects(ICcollabDataSource ccollabDataGenerator) : base(ccollabDataGenerator) { }

        private List<DefectRecord> validRecords = null;

        /// <summary>
        /// From the raw review records, only the ones which are created by employees in
        /// `employees.json` are valid.
        /// </summary>
        /// <returns></returns>
        public List<DefectRecord> GetValidRecords()
        {
            if (validRecords == null)
            {
                validRecords = GetDefectsRawData()
                    .Where(record => EmployeesReader.Employees.Any(employee => employee.LoginName == record.CreatorLogin))
                    .Select(record => record)
                    .ToList();
            }

            return validRecords;
        }

        /// <summary>
        /// Get all defect records for one/all product.
        /// </summary>
        /// <param name="productName">Product name, like: "ViewPoint".</param>
        /// <returns>List of defect records.</returns>
        public List<DefectRecord> GetRecordsByProduct(string productName)
        {
            List<DefectRecord> records = null;

            // for all products
            if (productName == "*")
            {
                records = GetValidRecords();
            }
            else if (EagleEyeSettingsReader.Settings.Products.IndexOf(productName) != -1)
            {
                records = GetValidRecords()
                    .Where(record => record.CreatorProductName == productName)
                    .ToList();
            }

            return records;
        }
        
        public void GenerateDefectCountByProduct(string settingsKey)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Product", "Count"],
            //     ["Team1", 20],
            //     ["Team2", 16]
            //   ]
            // }

            log.Info("Generating: Defect count by product ...");
            
            Dictionary<string, int> product2count = new Dictionary<string, int>();

            // collect all products
            foreach (Employee employee in EmployeesReader.Employees)
            {
                if (!product2count.ContainsKey(employee.ProductName))
                {
                    product2count.Add(employee.ProductName, 0);
                }
            }
            
            var query = GetValidRecords()
                        .GroupBy(record => record.CreatorProductName)
                        .Select(group => new { ProductName = group.Key, DefectCount = group.Count() });

            foreach (var item in query)
            {
                if (product2count.ContainsKey(item.ProductName))
                {
                    product2count[item.ProductName] = item.DefectCount;
                }
            }

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Product", "Count" };
            datatable.Add(header);

            foreach (KeyValuePair<string, int> item in product2count)
            {
                datatable.Add(new List<object> { item.Key, item.Value });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(settingsKey, json);

            log.Info("Generating: Defect count by product ... Done.");
        }

        public void GenerateDefectCountOfSeverityByProduct(string settingsKey)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Product", "Major", "Minor"],
            //     ["Team1", 20, 3],
            //     ["Team2", 16, 0]
            //   ]
            // }

            log.Info("Generating: Defect severity count by product ...");

            EagleEyeSettings settings = EagleEyeSettingsReader.Settings;

            Dictionary<string, List<int>> product2severitycount = new Dictionary<string, List<int>>();

            // collect all products
            foreach (Employee employee in EmployeesReader.Employees)
            {
                if (!product2severitycount.ContainsKey(employee.ProductName))
                {
                    product2severitycount.Add(employee.ProductName, new List<int>(new int[settings.DefectSeverityTypes.Count]));
                }
            }

            var query = GetValidRecords()
                        .GroupBy(record => record.CreatorProductName)
                        .Select(group => group);

            foreach (var item in query)
            {
                if (!product2severitycount.ContainsKey(item.Key)) continue;

                foreach (var defect in item)
                {
                    int index = settings.DefectSeverityTypes.IndexOf(defect.Severity);

                    if (index >= 0)
                    {
                        product2severitycount[item.Key][index] += 1;
                    }
                }
            }

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Product" }.Concat(settings.DefectSeverityTypes).ToList();
            datatable.Add(header);

            foreach (KeyValuePair<string, List<int>> item in product2severitycount)
            {
                List<object> row = new List<object> { item.Key };

                foreach (int count in item.Value)
                {
                    row.Add(count);
                }

                datatable.Add(row);
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(settingsKey, json);

            log.Info("Generating: Defect severity count by product ... Done.");
        }

        public void GenerateDefectCountOfSeverityByCreatorFromProduct()
        {
            foreach (var item in EagleEyeSettingsReader.Settings.DefectSeverityCountByCreator)
            {
                List<DefectRecord> datasource = GetRecordsByProduct(item.ProductName);
                
                if (datasource != null && !string.IsNullOrEmpty(item.ChartId))
                {
                    log.Info("Generating: Defect severity count by injection stage from " + (item.ProductName == "*" ? "all products" : item.ProductName) + " ...");

                    DefectCountOfSeverityByCreator(datasource, item);

                    log.Info("Generating: Defect severity count by injection stage from " + (item.ProductName == "*" ? "all products" : item.ProductName) + " ... Done");
                }
            }
        }

        private void DefectCountOfSeverityByCreator(List<DefectRecord> datasource, ProductChartSettings chartSettings)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Creator", "Major", "Minor"],
            //     ["Patrick", 20, 3],
            //     ["Lily", 16, 0]
            //   ]
            // }
            Dictionary<string, List<int>> creator2severitycount = new Dictionary<string, List<int>>();
            
            // collect all employees of the product
            foreach (Employee employee in EmployeesReader.GetEmployeesByProduct(chartSettings.ProductName))
            {
                creator2severitycount.Add(employee.LoginName, new List<int>(new int[EagleEyeSettingsReader.Settings.DefectSeverityTypes.Count]));
            }

            var query = datasource
                        .GroupBy(record => record.CreatorLogin)
                        .Select(group => group);

            foreach (var item in query)
            {
                if (!creator2severitycount.ContainsKey(item.Key)) continue;

                foreach (var defect in item)
                {
                    int index = EagleEyeSettingsReader.Settings.DefectSeverityTypes.IndexOf(defect.Severity);

                    if (index >= 0)
                    {
                        creator2severitycount[item.Key][index] += 1;
                    }
                }
            }

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Creator" }.Concat(EagleEyeSettingsReader.Settings.DefectSeverityTypes).ToList();
            datatable.Add(header);

            foreach (KeyValuePair<string, List<int>> item in creator2severitycount)
            {
                List<object> row = new List<object> { EmployeesReader.GetEmployeeFullNameByLoginName(item.Key) };

                foreach (int count in item.Value)
                {
                    row.Add(count);
                }

                datatable.Add(row);
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(chartSettings.ChartId, json);
        }

        public void GenerateDefectCountByInjectionStageFromProduct()
        {
            foreach (var item in EagleEyeSettingsReader.Settings.DefectCountByInjectionStage)
            {
                List<DefectRecord> datasource = GetRecordsByProduct(item.ProductName);

                if (datasource != null && !string.IsNullOrEmpty(item.ChartId))
                {
                    log.Info("Generating: Defect count by injection stage from " + (item.ProductName == "*" ? "all products" : item.ProductName) + " ...");

                    DefectCountByInjectionStage(datasource, item.ChartId);

                    log.Info("Generating: Defect count by injection stage from " + (item.ProductName == "*" ? "all products" : item.ProductName) + " ... Done");
                }
            }
        }

        private void DefectCountByInjectionStage(List<DefectRecord> datasource, string settingsKey)
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
            
            Dictionary<string, int> injectionstage2count = new Dictionary<string, int>();

            // collect all products
            foreach (var injectionStage in EagleEyeSettingsReader.Settings.DefectInjectionStage)
            {
                injectionstage2count.Add(injectionStage.ToLower(), 0);
            }

            var query = datasource
                        .GroupBy(record => record.InjectionStage)
                        .Select(group => new { InjectionStage = group.Key.ToLower(), Count = group.Count() });

            foreach (var item in query)
            {
                if (injectionstage2count.ContainsKey(item.InjectionStage))
                {
                    injectionstage2count[item.InjectionStage] = item.Count;
                }
            }

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "InjectionStage", "Count" };
            datatable.Add(header);

            foreach (KeyValuePair<string, int> item in injectionstage2count)
            {
                datatable.Add(new List<object> { item.Key, item.Value });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(settingsKey, json);
        }

        public void GenerateDefectCountByTypeFromProduct()
        {
            foreach (var item in EagleEyeSettingsReader.Settings.DefectCountByType)
            {
                List<DefectRecord> datasource = GetRecordsByProduct(item.ProductName);

                if (datasource != null && !string.IsNullOrEmpty(item.ChartId))
                {
                    log.Info("Generating: Defect count by type from " + (item.ProductName == "*" ? "all products" : item.ProductName) + " ...");

                    DefectCountByType(datasource, item.ChartId);

                    log.Info("Generating: Defect count by type from " + (item.ProductName == "*" ? "all products" : item.ProductName) + " ... Done");
                }
            }
        }

        private void DefectCountByType(List<DefectRecord> datasource, string settingsKey)
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
            
            Dictionary<string, int> type2count = new Dictionary<string, int>();

            // collect all products
            foreach (var type in EagleEyeSettingsReader.Settings.DefectTypes)
            {
                type2count.Add(type.ToLower(), 0);
            }

            var query = datasource
                        .GroupBy(record => record.Type)
                        .Select(group => new { Type = group.Key.ToLower(), Count = group.Count() });

            foreach (var item in query)
            {
                if (type2count.ContainsKey(item.Type))
                {
                    type2count[item.Type] = item.Count;
                }
            }

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Type", "Count" };
            datatable.Add(header);

            foreach (KeyValuePair<string, int> item in type2count)
            {
                datatable.Add(new List<object> { item.Key, item.Value });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(settingsKey, json);
        }

        public void GenerateDefectCountOfTypeByCreatorFromProduct()
        {
            foreach (var item in EagleEyeSettingsReader.Settings.DefectCountOfTypeByCreator)
            {
                List<DefectRecord> datasource = GetRecordsByProduct(item.ProductName);

                if (datasource != null && !string.IsNullOrEmpty(item.ChartId))
                {
                    log.Info("Generating: Defect count of type by creator from " + (item.ProductName == "*" ? "all products" : item.ProductName) + " ...");

                    DefectCountOfTypeByCreator(datasource, item);

                    log.Info("Generating: Defect count of type by creator from " + (item.ProductName == "*" ? "all products" : item.ProductName) + " ... Done");
                }
            }
        }

        private void DefectCountOfTypeByCreator(List<DefectRecord> datasource, ProductChartSettings chartSettings)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Creator", "algorithm/logic", "build", ..., "testing"],
            //     ["Patrick Zhong", 3, 1, ..., 0]
            //   ]
            // }
            Dictionary<string, List<int>> creator2typecount = new Dictionary<string, List<int>>();

            // collect all employees of the product
            foreach (Employee employee in EmployeesReader.GetEmployeesByProduct(chartSettings.ProductName))
            {
                creator2typecount.Add(employee.LoginName, new List<int>(new int[EagleEyeSettingsReader.Settings.DefectTypes.Count]));
            }

            var query = datasource
                        .GroupBy(record => record.CreatorLogin)
                        .Select(group => group);

            foreach (var item in query)
            {
                if (!creator2typecount.ContainsKey(item.Key)) continue;

                foreach (var defect in item)
                {
                    int index = EagleEyeSettingsReader.Settings.DefectTypes.IndexOf(defect.Type.ToLower());

                    if (index >= 0)
                    {
                        creator2typecount[item.Key][index] += 1;
                    }
                }
            }

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Creator" }.Concat(EagleEyeSettingsReader.Settings.DefectTypes).ToList();
            datatable.Add(header);

            foreach (KeyValuePair<string, List<int>> item in creator2typecount)
            {
                List<object> row = new List<object> { EmployeesReader.GetEmployeeFullNameByLoginName(item.Key) };

                foreach (int count in item.Value)
                {
                    row.Add(count);
                }

                datatable.Add(row);
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(chartSettings.ChartId, json);
        }

        public void GenerateDefectCountOfTypeByProduct(string settingsKey)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Product", "algorithm/logic", "build", ..., "testing"],
            //     ["ViewPoint", 1, 3, ..., 0],
            //     ...
            //   ]
            // }
            log.Info("Generating: Defect count of type by product ...");

            Dictionary<string, List<int>> product2typecount = new Dictionary<string, List<int>>();

            // collect all products
            foreach (Employee employee in EmployeesReader.Employees)
            {
                if (!product2typecount.ContainsKey(employee.ProductName))
                {
                    product2typecount.Add(employee.ProductName, new List<int>(new int[EagleEyeSettingsReader.Settings.DefectTypes.Count]));
                }
            }

            var query = GetValidRecords()
                        .GroupBy(record => record.CreatorProductName)
                        .Select(group => group);

            foreach (var item in query)
            {
                if (!product2typecount.ContainsKey(item.Key)) continue;

                foreach (var defect in item)
                {
                    int index = EagleEyeSettingsReader.Settings.DefectTypes.IndexOf(defect.Type);

                    if (index >= 0)
                    {
                        product2typecount[item.Key][index] += 1;
                    }
                }
            }

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Product" }.Concat(EagleEyeSettingsReader.Settings.DefectTypes).ToList();
            datatable.Add(header);

            foreach (KeyValuePair<string, List<int>> item in product2typecount)
            {
                List<object> row = new List<object> { item.Key };

                foreach (int count in item.Value)
                {
                    row.Add(count);
                }

                datatable.Add(row);
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(settingsKey, json);

            log.Info("Generating: Defect count of type by product ... Done.");
        }

        public void GenerateDefectCountByCreatorFromProduct()
        {
            foreach (var item in EagleEyeSettingsReader.Settings.DefectCountByCreator)
            {
                List<DefectRecord> datasource = GetRecordsByProduct(item.ProductName);

                if (datasource != null && !string.IsNullOrEmpty(item.ChartId))
                {
                    log.Info("Generating: Defect count by creator from " + (item.ProductName == "*" ? "all products" : item.ProductName) + " ...");

                    DefectCountByCreator(datasource, item);

                    log.Info("Generating: Defect count by creator from " + (item.ProductName == "*" ? "all products" : item.ProductName) + " ... Done");
                }
            }
        }

        private void DefectCountByCreator(List<DefectRecord> datasource, ProductChartSettings chartSettings)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Creator", "Count"],
            //     ["Patrick Zhong", 16],
            //     ["Merlin Mo", 16]
            //   ]
            // }

            Dictionary<string, int> creator2count = new Dictionary<string, int>();

            // collect all employees of the product
            foreach (Employee employee in EmployeesReader.GetEmployeesByProduct(chartSettings.ProductName))
            {
                creator2count.Add(employee.LoginName, 0);
            }

            var query = datasource
                        .GroupBy(record => record.CreatorLogin)
                        .Select(group => group);

            foreach (var item in query)
            {
                string creatorLoginName = item.Key;

                if (creator2count.ContainsKey(creatorLoginName))
                {
                    List<DefectRecord> records = item.ToList();

                    DefectStatistics stat = new DefectStatistics(records);
                    int count = stat.Count;

                    creator2count[creatorLoginName] = count;
                }
            }

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Creator", "Count" };
            datatable.Add(header);

            foreach (KeyValuePair<string, int> item in creator2count)
            {
                datatable.Add(new List<object> { EmployeesReader.GetEmployeeFullNameByLoginName(item.Key), item.Value });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(chartSettings.ChartId, json);
        }
    }
}
