using Ccollab;
using EagleEye.EEPlatformApi;
using EagleEye.GVizApi;
using EagleEye.Settings;
using Employees;
using log4net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace EagleEye.Reviews
{
    public class Reviews : EagleEyeDataGeneratorDecorator<ReviewRecord>, IReviewsCommands
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Reviews));
        
        public Reviews(ICcollabDataSource ccollabDataGenerator) : base(ccollabDataGenerator) { }

        private List<ReviewRecord> validRecords = null;

        /// <summary>
        /// From the raw review records, only the ones which are created by employees in
        /// `employees.json` are valid.
        /// </summary>
        /// <returns></returns>
        public List<ReviewRecord> GetValidRecords()
        {
            if (validRecords == null)
            {
                validRecords = GetValidRecords(GetReviewsRawData());
            }

            return validRecords;
        }

        /// <summary>
        /// Get all review records for one/all product.
        /// </summary>
        /// <param name="productName">Product name, like: "ViewPoint".</param>
        /// <returns>List of review records.</returns>
        public List<ReviewRecord> GetRecordsByProduct(string productName)
        {
            return GetRecordsByProduct(GetValidRecords(), productName);
        }
        
        public void GenerateReviewCountByMonthFromProduct()
        {
            foreach (var setting in EagleEyeSettingsReader.Settings.ReviewCountByMonth)
            {
                List<ReviewRecord> records = GetRecordsByProduct(setting.ProductName);

                if (records != null && !string.IsNullOrWhiteSpace(setting.ChartId))
                {
                    log.Info("Generating: Review count by month from " + (setting.ProductName == "*" ? "all products" : setting.ProductName) + " ...");

                    DataTable dataTable = ReviewCountByMonth(records);
                    EagleEyePlatformApi.EditDataTable(setting.ChartId, dataTable);

                    log.Info("Generating: Review count by month from " + (setting.ProductName == "*" ? "all products" : setting.ProductName) + " ... Done");
                }
            }
        }

        /// <summary>
        /// Filter by: Review Creation Date.
        /// 
        /// Expected data table format:
        /// {
        ///   "datatable": [
        ///     ["Month", "Count"],
        ///     ["2016-01", 20],
        ///     ["2016-02", 30]
        ///   ]
        /// }
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private DataTable ReviewCountByMonth(List<ReviewRecord> source)
        {
            DataTable dataTable = new DataTable();

            dataTable.AddColumn(new Column("string", "Month"));
            dataTable.AddColumn(new Column("number", "Count"));

            var query = source
                .GroupBy(record => record.ReviewCreationYear + "-" + record.ReviewCreationMonth)
                .OrderBy(group => group.Key)
                .Select(group => new { ReviewCreationYYYYMM = group.Key, Count = group.Count() });
            
            foreach (var item in query)
            {
                Row row = new Row(item.ReviewCreationYYYYMM, item.Count);
                dataTable.AddRow(row);
            }

            return dataTable;
        }

        /// <summary>
        /// Expected data table format:
        /// {
        ///   "datatable": [
        ///     ["Product", "Count"],
        ///     ["Team1", 20],
        ///     ["Team2", 16]
        ///   ]
        /// }
        /// </summary>
        public void GenerateReviewCountByProduct()
        {
            log.Info("Generating: Review count by product ...");

            DataTable dataTable = new DataTable();

            dataTable.AddColumn(new Column("string", "Product"));
            dataTable.AddColumn(new Column("number", "Count"));

            var query = EagleEyeSettingsReader.Settings.Products
                .GroupJoin(
                    GetValidRecords(),
                    product => product,
                    record => record.CreatorProductName,
                    (product, matching) => new { ProductName = product, ReviewCount = matching.Count() }
                );

            foreach (var item in query)
            {
                Row row = new Row(item.ProductName, item.ReviewCount);
                dataTable.AddRow(row);
            }
            
            EagleEyePlatformApi.EditDataTable(EagleEyeSettingsReader.Settings.ReviewCountByProduct.ChartId, dataTable);

            log.Info("Generating: Review count by product ... Done.");
        }
        
        public void GenerateReviewCountByCreatorFromProduct()
        {
            foreach (var setting in EagleEyeSettingsReader.Settings.ReviewCountByCreator)
            {
                List<ReviewRecord> records = GetRecordsByProduct(setting.ProductName);
                List<Employee> creators = EmployeesReader.GetEmployeesByProduct(setting.ProductName);

                if (records != null && !string.IsNullOrWhiteSpace(setting.ChartId))
                {
                    log.Info("Generating: Review count by creator from " + (setting.ProductName == "*" ? "all products" : setting.ProductName) + " ...");

                    DataTable dataTable = ReviewCountByCreator(records, creators);
                    EagleEyePlatformApi.EditDataTable(setting.ChartId, dataTable);

                    log.Info("Generating: Review count by creator from " + (setting.ProductName == "*" ? "all products" : setting.ProductName) + " ... Done");
                }
            }
        }

        /// <summary>
        /// Expected data table format:
        /// {
        ///   "datatable": [
        ///     ["Creator", "Count"],
        ///     ["Patrick Zhong", 9],
        ///     ["Merlin Mo", 16]
        ///   ]
        /// }
        /// </summary>
        /// <param name="source"></param>
        /// <param name="productName"></param>
        /// <returns></returns>
        private DataTable ReviewCountByCreator(List<ReviewRecord> source, List<Employee> creators)
        {
            DataTable dataTable = new DataTable();

            dataTable.AddColumn(new Column("string", "Creator"));
            dataTable.AddColumn(new Column("number", "Count"));

            var query = creators
                .GroupJoin(
                    source,
                    employee => employee.LoginName,
                    record => record.CreatorLogin,
                    (employee, matching) => new { Creator = employee.FullName, Count = matching.Count() }
                );

            foreach (var item in query)
            {
                Row row = new Row(item.Creator, item.Count);
                dataTable.AddRow(row);
            }

            return dataTable;
        }

        /// <summary>
        /// Formula:
        ///     CommentDensity(uploaded) = (CommentCount * 1000) / LOC;
        /// </summary>
        /// <param name="settingsKey">EagleEye chart id.</param>
        public void GenerateCommentDensityUploadedByProduct(string settingsKey)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Product", "Comments/KLOC"],
            //     ["Team1", 0.1],
            //     ["Team2", 0.034]
            //   ]
            // }
            log.Info("Generating: Code comment density (uploaded) by product ...");

            Dictionary<string, double> product2density = new Dictionary<string, double>();

            foreach (var product in EagleEyeSettingsReader.Settings.Products)
            {
                product2density.Add(product, 0);
            }

            var query = GetValidRecords()
                .GroupBy(record => record.CreatorProductName)
                .Select(record => record);
            
            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Product", "Comments/KLOC" };
            datatable.Add(header);

            foreach (var item in query)
            {
                List<ReviewRecord> records = item.ToList();

                ReviewsStatistics stat = new ReviewsStatistics(records);
                var totalCommentCount = stat.TotalCommentCount;
                var totalLOC = stat.TotalLOC;

                double density = 0;
                if (totalLOC != 0)
                {
                    density = (totalCommentCount * 1000) / totalLOC;
                }

                datatable.Add(new List<object> { item.Key, density });
            }
            
            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(settingsKey, json);

            log.Info("Generating: Code comment density (uploaded) by product ... Done");
        }

        /// <summary>
        /// Formula:
        ///     CommentDensity(changed) = (CommentCount * 1000) / LOCChanged;
        /// </summary>
        /// <param name="settingsKey">EagleEye chart id.</param>
        public void GenerateCommentDensityChangedByProduct(string settingsKey)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Product", "Comments/KLOCC"],
            //     ["Team1", 0.1],
            //     ["Team2", 0.034]
            //   ]
            // }
            log.Info("Generating: code comment density (changed) by product ...");

            Dictionary<string, double> product2density = new Dictionary<string, double>();
            
            foreach (var product in EagleEyeSettingsReader.Settings.Products)
            {
                product2density.Add(product, 0);
            }

            var query = GetValidRecords()
                .GroupBy(record => record.CreatorProductName)
                .Select(record => record);

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Product", "Comments/KLOCC" };
            datatable.Add(header);

            foreach (var item in query)
            {
                List<ReviewRecord> records = item.ToList();

                ReviewsStatistics stat = new ReviewsStatistics(records);
                var totalCommentCount = stat.TotalCommentCount;
                var totalLOCChanged = stat.TotalLOCChanged;

                double density = 0;
                if (totalLOCChanged != 0)
                {
                    density = (totalCommentCount * 1000) / totalLOCChanged;
                }

                datatable.Add(new List<object> { item.Key, density });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(settingsKey, json);

            log.Info("Generating: code comment density (changed) by product ... Done");
        }

        public void GenerateCommentDensityChangedByMonthFromProduct()
        {
            foreach (var item in EagleEyeSettingsReader.Settings.CommentDensityChangedByMonth)
            {
                List<ReviewRecord> records = GetRecordsByProduct(item.ProductName);

                if (records != null && !string.IsNullOrEmpty(item.ChartId))
                {
                    log.Info("Generating: Code comment density (changed) by month from " + (item.ProductName == "*" ? "all products" : item.ProductName) + " ...");

                    CommentDensityChangedByMonth(records, item.ChartId);

                    log.Info("Generating: Code comment density (changed) by month from " + (item.ProductName == "*" ? "all products" : item.ProductName) + " ... Done");
                }
            }
        }

        /// <summary>
        /// Formula:
        ///     Comment Density (changed) = (CommentCount * 1000) / LOCChanged;
        /// </summary>
        /// <param name="settingsKey">EagleEye chart id.</param>
        private void CommentDensityChangedByMonth(List<ReviewRecord> source, string settingsKey)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Month", "Comments/KLOCC"],
            //     ["2016-01", 0.1],
            //     ["2016-02", 0.03]
            //   ]
            // }
            var query = source
                .GroupBy(record => record.ReviewCreationYear + "-" + record.ReviewCreationMonth)
                .OrderBy(group => group.Key)
                .Select(group => group);

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Month", "Comments/KLOCC" };
            datatable.Add(header);

            foreach (var item in query)
            {
                List<ReviewRecord> records = item.ToList();

                ReviewsStatistics stat = new ReviewsStatistics(records);
                var totalCommentCount = stat.TotalCommentCount;
                var totalLOCChanged = stat.TotalLOCChanged;

                double density = 0;
                if (totalLOCChanged != 0)
                {
                    density = (totalCommentCount * 1000) / totalLOCChanged;
                }

                datatable.Add(new List<object> { item.Key, density });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(settingsKey, json);
        }

        /// <summary>
        /// Formula:
        ///     DefectDensity(uploaded) = (DefectCount * 1000) / LOC;
        /// </summary>
        /// <param name="settingsKey">EagleEye chart id.</param>
        public void GenerateDefectDensityUploadedByProduct(string settingsKey)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Product", "Defects/KLOC"],
            //     ["Team1", 0.1],
            //     ["Team2", 0.034]
            //   ]
            // }
            log.Info("Generating: code defect density (uploaded) by product ...");

            Dictionary<string, double> product2density = new Dictionary<string, double>();
            
            foreach (var product in EagleEyeSettingsReader.Settings.Products)
            {
                product2density.Add(product, 0);
            }

            var query = GetValidRecords()
                .GroupBy(record => record.CreatorProductName)
                .Select(record => record);

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Product", "Defects/KLOC" };
            datatable.Add(header);

            foreach (var item in query)
            {
                List<ReviewRecord> records = item.ToList();

                ReviewsStatistics stat = new ReviewsStatistics(records);
                var totalDefectCount = stat.TotalDefectCount;
                var totalLOC = stat.TotalLOC;

                double density = 0;
                if (totalLOC != 0)
                {
                    density = (totalDefectCount * 1000) / totalLOC;
                }

                datatable.Add(new List<object> { item.Key, density });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(settingsKey, json);

            log.Info("Generating: code defect density (uploaded) by product ... Done");
        }

        /// <summary>
        /// Formula:
        ///     DefectDensity(changed) = (DefectCount * 1000) / LOCChanged;
        /// </summary>
        /// <param name="settingsKey">EagleEye chart id.</param>
        public void GenerateDefectDensityChangedByProduct(string settingsKey)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Product", "Defects/KLOCC"],
            //     ["Team1", 0.1],
            //     ["Team2", 0.034]
            //   ]
            // }

            log.Info("Generating: code defect density (changed) by product ...");

            Dictionary<string, double> product2density = new Dictionary<string, double>();
            
            foreach (var product in EagleEyeSettingsReader.Settings.Products)
            {
                product2density.Add(product, 0);
            }

            var query = GetValidRecords()
                .GroupBy(record => record.CreatorProductName)
                .Select(record => record);

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Product", "Defects/KLOCC" };
            datatable.Add(header);

            foreach (var item in query)
            {
                List<ReviewRecord> records = item.ToList();

                ReviewsStatistics stat = new ReviewsStatistics(records);
                var totalDefectCount = stat.TotalDefectCount;
                var totalLOCChanged = stat.TotalLOCChanged;

                double density = 0;
                if (totalLOCChanged != 0)
                {
                    density = (totalDefectCount * 1000) / totalLOCChanged;
                }

                datatable.Add(new List<object> { item.Key, density });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(settingsKey, json);

            log.Info("Generating: code defect density (changed) by product ... Done");
        }

        public void GenerateDefectDensityChangedByMonthFromProduct()
        {
            foreach (var item in EagleEyeSettingsReader.Settings.DefectDensityChangedByMonth)
            {
                List<ReviewRecord> records = GetRecordsByProduct(item.ProductName);

                if (records != null && !string.IsNullOrEmpty(item.ChartId))
                {
                    log.Info("Generating: Code defect density (changed) by month from " + (item.ProductName == "*" ? "all products" : item.ProductName) + " ...");

                    DefectDensityChangedByMonth(records, item.ChartId);

                    log.Info("Generating: Code defect density (changed) by month from " + (item.ProductName == "*" ? "all products" : item.ProductName) + " ... Done");
                }
            }
        }

        /// <summary>
        /// Formula:
        ///     DefectDensity(changed) = (DefectCount * 1000) / LOCChanged;
        /// </summary>
        /// <param name="source">Review datasource to calculate.</param>
        /// <param name="settingsKey">EagleEye chart id.</param>
        private void DefectDensityChangedByMonth(List<ReviewRecord> source, string settingsKey)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Month", "Defects/KLOCC"],
            //     ["2016-01", 20],
            //     ["2016-02", 30]
            //   ]
            // }
            
            var query = source
                .GroupBy(record => record.ReviewCreationYear + "-" + record.ReviewCreationMonth)
                .OrderBy(group => group.Key)
                .Select(group => group);

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Month", "Defects/KLOCC" };
            datatable.Add(header);

            foreach (var item in query)
            {
                List<ReviewRecord> records = item.ToList();
                
                ReviewsStatistics stat = new ReviewsStatistics(records);
                var totalDefectCount = stat.TotalDefectCount;
                var totalLOCChanged = stat.TotalLOCChanged;

                double density = 0;
                if (totalLOCChanged != 0)
                {
                    density = totalDefectCount * 1000 / totalLOCChanged;
                }

                datatable.Add(new List<object> { item.Key, density });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(settingsKey, json);
        }
        
        public void GenerateInspectionRateByMonthFromProduct()
        {
            foreach (var item in EagleEyeSettingsReader.Settings.InspectionRateByMonth)
            {
                List<ReviewRecord> records = GetRecordsByProduct(item.ProductName);

                if (records != null && !string.IsNullOrEmpty(item.ChartId))
                {
                    log.Info("Generating: Inspection rate by month from " + (item.ProductName == "*" ? "all products" : item.ProductName) + " ...");

                    InspectionRateByMonth(records, item.ChartId);

                    log.Info("Generating: Inspection rate by month from " + (item.ProductName == "*" ? "all products" : item.ProductName) + " ... Done");
                }
            }
        }

        /// <summary>
        /// Formula:
        ///     InspectionRate = (LOCC) / (TotalPersonTime * 1000)
        /// </summary>
        /// <param name="settingsKey"></param>
        private void InspectionRateByMonth(List<ReviewRecord> source, string settingsKey)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Month", "Rate"],
            //     ["2016-01", 0.18],
            //     ["2016-02", 0.6]
            //   ]
            // }            
            var query = source
                .GroupBy(record => record.ReviewCreationYear + "-" + record.ReviewCreationMonth)
                .OrderBy(group => group.Key)
                .Select(group => group);
            
            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Month", "Rate" };
            datatable.Add(header);

            foreach (var item in query)
            {
                List<ReviewRecord> records = item.ToList();

                ReviewsStatistics stat = new ReviewsStatistics(records);
                double totalPersonTimeInHour = stat.TotalPersonTimeInSecond / (60 * 60);

                double inspectionRate = 0;
                if (totalPersonTimeInHour != 0)
                {
                    inspectionRate = stat.TotalLOCChanged / (totalPersonTimeInHour * 1000);
                }
                
                datatable.Add(new List<object> { item.Key, inspectionRate });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(settingsKey, json);
        }
    }
}
